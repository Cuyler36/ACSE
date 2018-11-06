using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Modifiable;
using ACSE.Core.Utilities;
using ACSE.WinForms.Imaging;

namespace ACSE.WinForms.Controls
{
    /// <inheritdoc cref="IModifiable" />
    /// <summary>
    /// An all-in-one control for editing items.
    /// </summary>
    public class ItemEditor : UserControl, IModifiable
    {
        /// <summary>
        /// This event fires when an item is set or changed. PreviousItem will be null if it is the first time setting the item.
        /// </summary>
        public event EventHandler<IndexedItemChangedEventArgs> ItemChanged;

        public readonly PictureBox EditorPictureBox;
        public readonly ToolTip ItemToolTip;
        public readonly Stack<ItemChange> UndoStack = new Stack<ItemChange>();
        public readonly Stack<ItemChange> RedoStack = new Stack<ItemChange>();
        public bool Modified { get; protected set; }
        public string HoverText = "{0} - [0x{1}]";
        public bool ShowHoveredItemCellHighlight = true;

        protected Image CurrentItemImage;

        private int _itemsPerRow;
        public int ItemsPerRow
        {
            get => _itemsPerRow;
            set
            {
                _itemsPerRow = value;
                if (EditorPictureBox != null)
                {
                    SetItemPicture();
                }
            }
        }

        private int _itemCellSize = 8;
        public int ItemCellSize
        {
            get => _itemCellSize;
            set
            {
                _itemCellSize = value;
                if (EditorPictureBox != null)
                {
                    SetItemPicture();
                }
            }
        }

        private Item[] _items;
        public virtual Item[] Items { get => _items;
            set
            {
                _items = value;
                SetItemPicture();
            }
        }

        protected virtual void SetItemPicture()
        {
            if (_items == null) return;

            Size = new Size(_itemCellSize * _itemsPerRow + 3,
                _itemCellSize * (int) (Math.Ceiling((decimal) _items.Length / _itemsPerRow)) + 3);

            CurrentItemImage?.Dispose();
            EditorPictureBox.Image?.Dispose();

            CurrentItemImage = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items, MainForm.SaveFile.SaveType,
                EditorPictureBox.Size);
            EditorPictureBox.Image = (Image) CurrentItemImage.Clone();
            EditorPictureBox.Refresh();
        }

        protected readonly MainForm MainFormReference;
        protected int LastX = -1, LastY = -1;

        protected ItemEditor()
        {
            ItemToolTip = new ToolTip
            {
                ShowAlways = true
            };

            EditorPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill
            };

            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(EditorPictureBox);
        }

        public ItemEditor(MainForm mainForm, Item[] items, int itemsPerRow, int itemCellSize = 8) : this()
        {
            MainFormReference = mainForm;
            _items = items;
            _itemsPerRow = itemsPerRow;
            ItemCellSize = itemCellSize;

            EditorPictureBox.MouseMove += OnEditorMouseMove;
            EditorPictureBox.MouseLeave += delegate
            {
                ItemToolTip.Hide(this);
                EditorPictureBox.Image?.Dispose();
                EditorPictureBox.Image = (Image) CurrentItemImage.Clone();
                EditorPictureBox.Refresh();
            };

            EditorPictureBox.MouseDown += OnEditorMouseDown;
        }

        protected virtual void OnItemChanged(Item previousItem, Item newItem, int index)
        {
            ItemChanged?.Invoke(this,
                new IndexedItemChangedEventArgs {PreviousItem = previousItem, NewItem = newItem, Index = index});
        }

        protected virtual void PushNewItemChange(Item oldItem, int itemIndex, Stack<ItemChange> stack)
        {
            stack.Push(new ItemChange { Item = oldItem, Index = itemIndex });
        }

        protected virtual bool GetXyPosition(MouseEventArgs e, out int x, out int y, out int index)
        {
            x = e.X / _itemCellSize;
            y = e.Y / _itemCellSize;
            index = y * _itemsPerRow + x;

            return index > -1 && index < _items.Length;
        }

        protected virtual void OnEditorMouseMove(object sender, MouseEventArgs e)
        {
            if (_items == null || !GetXyPosition(e, out var x, out var y, out var index) ||
                (e.X == LastX && e.Y == LastY)) return;
            
            // Update Last Hover Position
            LastX = e.X;
            LastY = e.Y;

            // Draw the item highlight if necessary.
            if (ShowHoveredItemCellHighlight)
            {
                EditorPictureBox.Image?.Dispose();
                EditorPictureBox.Image = (Bitmap) CurrentItemImage.Clone();
                ImageGeneration.OverlayItemBoxGlow((Bitmap) EditorPictureBox.Image, _itemCellSize, x, y);
                EditorPictureBox.Refresh();
            }

            var hoveredItem = _items[index];

            // Refresh ToolTip
            ItemToolTip.Show(
                string.Format(HoverText, hoveredItem.Name, hoveredItem.ItemId.ToString("X4"),
                    hoveredItem.ItemFlag.ToString()), this, e.X + 10, e.Y + 10, 100000);

            // Check for MouseDown
            if (e.Button != MouseButtons.None)
                OnEditorMouseDown(sender, e);
        }

        protected virtual void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            if (!GetXyPosition(e, out var x, out var y, out var index)) return;
            var selectedItem = _items[index];
            switch (e.Button)
            {
                case MouseButtons.Left:
                    var newItem = MainFormReference.GetCurrentItem();

                    if (selectedItem != newItem)
                    {
                        // Save Old Item
                        PushNewItemChange(selectedItem, index, UndoStack);

                        // Clear Redo Stack
                        NewChange(null);

                        // Set New Item
                        _items[index] = newItem;

                        // Redraw Item Image
                        CurrentItemImage?.Dispose();
                        EditorPictureBox.Image?.Dispose();

                        CurrentItemImage = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items, MainForm.SaveFile.SaveType,
                            EditorPictureBox.Size);
                        EditorPictureBox.Image = (Image)CurrentItemImage.Clone();
                        ImageGeneration.OverlayItemBoxGlow((Bitmap) EditorPictureBox.Image, _itemCellSize, x, y);
                        EditorPictureBox.Refresh();

                        // Update ToolTip
                        ItemToolTip.Show(
                            string.Format(HoverText, newItem.Name, newItem.ItemId.ToString("X4"),
                                newItem.ItemFlag.ToString()), this, e.X + 10, e.Y + 10, 100000);

                        // Fire ItemChanged Event
                        OnItemChanged(selectedItem, newItem, index);
                        MainForm.SaveFile.ChangesMade = true;
                        Modified = true;
                    }

                    break;
                case MouseButtons.Right:
                    MainFormReference.SetCurrentItem(selectedItem);
                    break;
                case MouseButtons.Middle:
                    Utility.FloodFillItemArray(ref _items, ItemsPerRow, index, _items[index],
                        MainFormReference.GetCurrentItem());
                    SetItemPicture();
                    break;
            }
        }

        public virtual void Undo()
        {
            if (!UndoStack.Any()) return;
            // Get Previous Change
            var previousItemChange = UndoStack.Pop();

            var selectedItem = _items[previousItemChange.Index];

            // Set Redo Change
            PushNewItemChange(selectedItem, previousItemChange.Index, RedoStack);

            // Undo
            _items[previousItemChange.Index] = previousItemChange.Item;
            var img = EditorPictureBox.Image;
            EditorPictureBox.Image = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items,
                MainForm.SaveFile.SaveType, EditorPictureBox.Size);
            img?.Dispose();

            OnItemChanged(selectedItem, Items[previousItemChange.Index], previousItemChange.Index);
        }

        public virtual void Redo()
        {
            if (!RedoStack.Any()) return;
            // Get Previous Change
            var previousItemChange = RedoStack.Pop();

            var selectedItem = _items[previousItemChange.Index];

            // Set Undo Change
            PushNewItemChange(selectedItem, previousItemChange.Index, UndoStack);

            // Redo
            _items[previousItemChange.Index] = previousItemChange.Item;
            var img = EditorPictureBox.Image;
            EditorPictureBox.Image = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items,
                MainForm.SaveFile.SaveType, EditorPictureBox.Size);
            img?.Dispose();

            OnItemChanged(selectedItem, Items[previousItemChange.Index], previousItemChange.Index);
        }

        public virtual void NewChange(object change)
        {
            if (!(change is ItemChange newItem)) return;
            RedoStack.Clear();
            UndoStack.Push(newItem);
        }

        protected new virtual void Dispose()
        {
            ItemToolTip.Dispose();
            EditorPictureBox.Image?.Dispose();
            EditorPictureBox.Dispose();
            base.Dispose();
        }
    }
}