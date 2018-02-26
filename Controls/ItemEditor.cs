using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACSE
{
    /// <summary>
    /// An all-in-one control for editing items.
    /// </summary>
    public partial class ItemEditor : UserControl
    {
        /// <summary>
        /// This event fires when an item is set or changed. PreviousItem will be null if it is the first time setting the item.
        /// </summary>
        public event EventHandler<IndexedItemChangedEventArgs> ItemChanged;

        public readonly PictureBox EditorPictureBox;
        public readonly ToolTip ItemToolTip;
        public readonly Stack<ItemChange> UndoStack = new Stack<ItemChange>();
        public readonly Stack<ItemChange> RedoStack = new Stack<ItemChange>();
        public bool Modified { get; protected set; } = false;

        private int itemsPerRow;
        public int ItemsPerRow
        {
            get => itemsPerRow;
            set
            {
                itemsPerRow = value;
                if (EditorPictureBox != null)
                {
                    SetItemPicture();
                }
            }
        }

        private int itemCellSize = 8;
        public int ItemCellSize
        {
            get => itemCellSize;
            set
            {
                itemCellSize = value;
                if (EditorPictureBox != null)
                {
                    SetItemPicture();
                }
            }
        }

        private Item[] items;
        public virtual Item[] Items { get => items;
            set
            {
                items = value;
                SetItemPicture();
            }
        }

        private void SetItemPicture()
        {
            var Img = EditorPictureBox.Image;

            if (items != null)
            {
                Size = new Size(itemCellSize * itemsPerRow + 3, itemCellSize * (int)(Math.Ceiling((decimal)items.Length / itemsPerRow)) + 3);
                EditorPictureBox.Image = Inventory.GetItemPic(itemCellSize, itemsPerRow, items, MainForm.Save_File.Save_Type, EditorPictureBox.Size);
            }

            if (Img != null)
                Img.Dispose();
        }

        private MainForm MainFormReference;
        private int LastX = -1, LastY = -1;
        private bool IsMouseDown = false;

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
            this.items = items;
            this.itemsPerRow = itemsPerRow;
            ItemCellSize = itemCellSize;

            EditorPictureBox.MouseMove += OnEditorMouseMove;
            EditorPictureBox.MouseLeave += (sender, e) => ItemToolTip.Hide(this);

            EditorPictureBox.MouseDown += OnEditorMouseDown;
            EditorPictureBox.MouseUp += (sender, e) => IsMouseDown = false;
        }

        protected virtual void OnItemChanged(Item PreviousItem, Item NewItem, int Index)
        {
            ItemChanged?.Invoke(this, new IndexedItemChangedEventArgs { PreviousItem = PreviousItem, NewItem = NewItem, Index = Index });
        }

        protected virtual void PushNewItemChange(Item OldItem, int ItemIndex, Stack<ItemChange> Stack)
        {
            Stack.Push(new ItemChange { Item = OldItem, Index = ItemIndex });
        }

        protected virtual bool GetXYPosition(MouseEventArgs e, out int X, out int Y, out int Index)
        {
            X = e.X / itemCellSize;
            Y = e.Y / itemCellSize;
            Index = Y * itemsPerRow + X;

            return Index > -1 && Index < items.Length;
        }

        protected virtual void OnEditorMouseMove(object sender, MouseEventArgs e)
        {
            if (items != null && GetXYPosition(e, out int X, out int Y, out int Index) && (e.X != LastX || e.Y != LastY))
            {
                // Update Last Hover Position
                LastX = e.X;
                LastY = e.Y;

                Item HoveredItem = items[Index];

                // Refresh ToolTip
                ItemToolTip.Show(string.Format("{0} - [0x{1}]", HoveredItem.Name, HoveredItem.ItemID.ToString("X4")), this, e.X + 10, e.Y + 10, 100000);

                // Check for MouseDown
                if (IsMouseDown)
                    OnEditorMouseDown(sender, e);
            }
        }

        protected virtual void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
            if (GetXYPosition(e, out int X, out int Y, out int Index))
            {
                Item SelectedItem = items[Index];
                if (e.Button == MouseButtons.Left)
                {
                    Item NewItem = MainFormReference.GetCurrentItem();

                    if (SelectedItem != NewItem)
                    {
                        // Save Old Item
                        PushNewItemChange(SelectedItem, Index, UndoStack);

                        // Set New Item
                        items[Index] = NewItem;

                        // Redraw Item Image
                        var Img = EditorPictureBox.Image;
                        EditorPictureBox.Image = Inventory.GetItemPic(itemCellSize, itemsPerRow, items, MainForm.Save_File.Save_Type, EditorPictureBox.Size);

                        if (Img != null)
                            Img.Dispose();

                        // Update ToolTip
                        ItemToolTip.Show(string.Format("{0} - [0x{1}]", NewItem.Name, NewItem.ItemID.ToString("X4")), this, e.X + 10, e.Y + 10, 100000);

                        // Fire ItemChanged Event
                        OnItemChanged(SelectedItem, NewItem, Index);

                        Modified = true;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    MainFormReference.SetCurrentItem(SelectedItem);
                }
            }
        }

        protected virtual void Undo()
        {
            if (UndoStack.Any())
            {
                // Get Previous Change
                ItemChange PreviousItemChange = UndoStack.Pop();

                Item SelectedItem = items[PreviousItemChange.Index];

                // Set Redo Change
                PushNewItemChange(SelectedItem, PreviousItemChange.Index, RedoStack);

                // Undo
                items[PreviousItemChange.Index] = PreviousItemChange.Item;
                var Img = EditorPictureBox.Image;
                EditorPictureBox.Image = Inventory.GetItemPic(itemCellSize, itemsPerRow, items, MainForm.Save_File.Save_Type, EditorPictureBox.Size);

                if (Img != null)
                    Img.Dispose();

                OnItemChanged(SelectedItem, Items[PreviousItemChange.Index], PreviousItemChange.Index);
            }
        }

        protected virtual void Redo()
        {
            if (RedoStack.Any())
            {
                // Get Previous Change
                ItemChange PreviousItemChange = RedoStack.Pop();

                Item SelectedItem = items[PreviousItemChange.Index];

                // Set Undo Change
                PushNewItemChange(SelectedItem, PreviousItemChange.Index, UndoStack);

                // Redo
                items[PreviousItemChange.Index] = PreviousItemChange.Item;
                var Img = EditorPictureBox.Image;
                EditorPictureBox.Image = Inventory.GetItemPic(itemCellSize, itemsPerRow, items, MainForm.Save_File.Save_Type, EditorPictureBox.Size);

                if (Img != null)
                    Img.Dispose();

                OnItemChanged(SelectedItem, Items[PreviousItemChange.Index], PreviousItemChange.Index);
            }
        }

        protected virtual void Dipose()
        {
            ItemToolTip.Dispose();

            if (EditorPictureBox.Image != null)
                EditorPictureBox.Image.Dispose();

            EditorPictureBox.Dispose();
            base.Dispose();
        }
    }
}