using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Modifiable;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;
using ACSE.WinForms.Imaging;
using ACSE.WinForms.Utilities;

namespace ACSE.WinForms.Controls
{
    /// <inheritdoc cref="IModifiable" />
    /// <summary>
    /// An all-in-one control for editing items.
    /// </summary>
    public class ItemEditor : PictureBoxWithInterpolationMode, IModifiable
    {
        /// <summary>
        /// This event fires when an item is set or changed. PreviousItem will be null if it is the first time setting the item.
        /// </summary>
        public event EventHandler<IndexedItemChangedEventArgs> ItemChanged;

        /// <summary>
        /// This event fires when a new item is selected.
        /// </summary>
        public event ItemSelectedHandler ItemSelected;
        
        
        // Delegates
        public delegate void ItemSelectedHandler(Item selectedItem, bool buried);


        public readonly ToolTip ItemToolTip;
        public readonly Stack<ItemChange> UndoStack = new Stack<ItemChange>();
        public readonly Stack<ItemChange> RedoStack = new Stack<ItemChange>();
        public bool Modified { get; protected set; }
        public string HoverText = "{0} - [0x{1}]";
        public bool ShowHoveredItemCellHighlight = true;

        protected bool DrawBaseImage = true;

        private Image _currentItemImage;
        protected Image CurrentItemImage
        {
            get => _currentItemImage;
            set
            {
                _currentItemImage?.Dispose();
                _currentItemImage = value;
            }
        }

        private int _itemsPerRow;
        public int ItemsPerRow
        {
            get => _itemsPerRow;
            set
            {
                _itemsPerRow = value;
                SetItemPicture();
            }
        }

        private int _itemCellSize = 8;
        public int ItemCellSize
        {
            get => _itemCellSize;
            set
            {
                _itemCellSize = value;
                SetItemPicture();
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

            Image?.Dispose();

            CurrentItemImage =
                Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items, Save.SaveInstance.SaveType, Size);
            Image = (Image) CurrentItemImage.Clone();
            Refresh();
        }

        protected int LastX = -1, LastY = -1;

        protected ItemEditor()
        {
            ItemToolTip = ToolTipManager.GetSharedToolTip();
            BorderStyle = BorderStyle.FixedSingle;
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        public ItemEditor(Item[] items, int itemsPerRow, int itemCellSize = 8) : this()
        {
            _items = items;
            _itemsPerRow = itemsPerRow;
            ItemCellSize = itemCellSize;

            MouseMove += OnEditorMouseMove;
            MouseLeave += delegate
            {
                ItemToolTip.Hide(this);
                Image.Dispose();
                Image = (Image) CurrentItemImage.Clone();
                Refresh();
            };

            MouseDown += OnEditorMouseDown;
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
                (e.X == LastX && e.Y == LastY) || x < 0 || x > 15 || y < 0 || y > 15) return;
            
            // Update Last Hover Position
            LastX = e.X;
            LastY = e.Y;

            // Draw the item highlight if necessary.
            if (ShowHoveredItemCellHighlight)
            {
                Image?.Dispose();
                Image = (Bitmap) CurrentItemImage.Clone();
                ImageGeneration.OverlayItemBoxGlow((Bitmap) Image, _itemCellSize, x, y);
                Refresh();
            }

            var hoveredItem = _items[index];

            // Refresh ToolTip
            ShowToolTip(GetToolTipString(x, y, hoveredItem), e.X, e.Y);

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
                    var newItem = Item.SelectedItem;

                    if (selectedItem != newItem)
                    {
                        // Save Old Item
                        PushNewItemChange(selectedItem, index, UndoStack);

                        // Clear Redo Stack
                        NewChange(null);

                        // Set New Item
                        _items[index] = new Item(newItem); // Ensure we create a copy of the item.

                        // Redraw Item Image
                        if (DrawBaseImage)
                        {
                            Image?.Dispose();

                            CurrentItemImage = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items,
                                Save.SaveInstance.SaveType, Size);
                            Image = (Image) CurrentItemImage.Clone();
                            ImageGeneration.OverlayItemBoxGlow((Bitmap) Image, _itemCellSize, x, y);
                            Refresh();
                        }

                        // Update ToolTip
                        ShowToolTip(GetToolTipString(x, y, newItem), e.X, e.Y);

                        // Fire ItemChanged Event
                        OnItemChanged(selectedItem, newItem, index);
                        Save.SaveInstance.ChangesMade = true;
                        Modified = true;
                    }

                    break;
                case MouseButtons.Right:
                    ItemSelected?.Invoke(selectedItem, false);
                    break;
                case MouseButtons.Middle:
                    Utility.FloodFillItemArray(ref _items, ItemsPerRow, index, _items[index], Item.SelectedItem);
                    SetItemPicture();
                    break;
            }
        }

        protected virtual string GetToolTipString(int x, int y, params object[] parameters)
        {
            var item = (Item) parameters[0];
            return $"{item.Name} - [0x{item.ItemId:X4}]";
        }

        public virtual void ShowToolTip(string text, int x, int y)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                ItemToolTip.Show(text, this, x + 10, y + 10, 100000);
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
            var img = Image;
            Image = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items,
                Save.SaveInstance.SaveType, Size);
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
            var img = Image;
            Image = Inventory.GetItemPic(_itemCellSize, _itemsPerRow, _items,
                Save.SaveInstance.SaveType, Size);
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
            Image?.Dispose();
            base.Dispose();
        }
    }
}