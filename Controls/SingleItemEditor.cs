using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACSE
{
    /// <summary>
    /// An all-in-one control for editing a single item.
    /// </summary>
    public partial class SingleItemEditor : UserControl
    {
        /// <summary>
        /// This event fires when the item is set or changed. PreviousItem will be null if it is the first time setting the item.
        /// </summary>
        public event EventHandler<ItemChangedEventArgs> ItemChanged;

        public readonly PictureBox EditorPictureBox;
        public readonly ToolTip ItemToolTip;
        public readonly Stack<ItemChange> UndoStack = new Stack<ItemChange>();
        public readonly Stack<ItemChange> RedoStack = new Stack<ItemChange>();
        public bool Modified { get; protected set; } = false;

        private bool disabled = false;
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                var Img = EditorPictureBox.Image;
                EditorPictureBox.Image = Properties.Resources.X;

                if (Img != null)
                    Img.Dispose();
            }
        }

        private int itemCellSize = 8;
        public int ItemCellSize
        {
            get => itemCellSize;
            set
            {
                itemCellSize = value;
                if (!disabled && EditorPictureBox != null)
                {
                    SetItemPicture();
                }
            }
        }

        private Item item;
        public virtual Item Item
        {
            get => item;
            set
            {
                Item PreviousItem = item;
                item = value;

                OnItemChanged(PreviousItem, value);

                if (!disabled)
                    SetItemPicture();
            }
        }

        private void SetItemPicture()
        {
            var Img = EditorPictureBox.Image;

            if (Item != null)
            {
                Size = new Size(itemCellSize, itemCellSize);
                EditorPictureBox.Image = Inventory.GetItemPic(itemCellSize, item, MainForm.Save_File.Save_Type);
            }

            if (Img != null)
                Img.Dispose();
        }

        private MainForm MainFormReference;
        private int LastX = -1, LastY = -1;
        private bool IsMouseDown = false;

        protected SingleItemEditor()
        {
            ItemToolTip = new ToolTip
            {
                ShowAlways = true
            };

            EditorPictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(EditorPictureBox);
        }

        public SingleItemEditor(MainForm mainForm, Item item, int itemCellSize = 8) : this()
        {
            MainFormReference = mainForm;
            this.item = item;
            ItemCellSize = itemCellSize;

            EditorPictureBox.MouseMove += OnEditorMouseMove;
            EditorPictureBox.MouseLeave += (sender, e) => ItemToolTip.Hide(this);

            EditorPictureBox.MouseDown += OnEditorMouseDown;
            EditorPictureBox.MouseUp += (sender, e) => IsMouseDown = false;
        }

        protected virtual void OnItemChanged(Item PreviousItem, Item NewItem)
        {
            ItemChanged?.Invoke(this, new ItemChangedEventArgs { PreviousItem = PreviousItem, NewItem = NewItem });
        }

        protected virtual void PushNewItemChange(Item OldItem, int ItemIndex, Stack<ItemChange> Stack)
        {
            Stack.Push(new ItemChange { Item = OldItem, Index = ItemIndex });
        }

        protected virtual void OnEditorMouseMove(object sender, MouseEventArgs e)
        {
            if (item != null && !disabled && (e.X != LastX || e.Y != LastY))
            {
                // Update Last Hover Position
                LastX = e.X;
                LastY = e.Y;

                // Refresh ToolTip
                ItemToolTip.Show(string.Format("{0} - [0x{1}]", item.Name, item.ItemID.ToString("X4")), this, e.X + 10, e.Y + 10, 100000);

                // Check for MouseDown
                if (IsMouseDown)
                    OnEditorMouseDown(sender, e);
            }
        }

        protected virtual void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            if (!disabled)
            {
                IsMouseDown = true;
                if (e.Button == MouseButtons.Left)
                {
                    Item NewItem = MainFormReference.GetCurrentItem();
                    Item PreviousItem = item;

                    if (PreviousItem != NewItem)
                    {

                        // Save Old Item
                        PushNewItemChange(PreviousItem, 0, UndoStack);

                        // Set New Item
                        Item = NewItem;

                        // Update ToolTip
                        ItemToolTip.Show(string.Format("{0} - [0x{1}]", NewItem.Name, NewItem.ItemID.ToString("X4")), this, e.X + 10, e.Y + 10, 100000);

                        Modified = true;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    MainFormReference.SetCurrentItem(item);
                }
            }
        }

        protected virtual void Undo()
        {
            if (UndoStack.Any())
            {
                Item PreviousItem = item;
                // Get Previous Change
                ItemChange PreviousItemChange = UndoStack.Pop();

                // Set Redo Change
                PushNewItemChange(item, PreviousItemChange.Index, RedoStack);

                // Undo
                Item = PreviousItemChange.Item;

                Modified = UndoStack.Any();
            }
        }

        protected virtual void Redo()
        {
            if (RedoStack.Any())
            {
                Item PreviousItem = item;

                // Get Previous Change
                ItemChange PreviousItemChange = RedoStack.Pop();

                // Set Undo Change
                PushNewItemChange(item, PreviousItemChange.Index, UndoStack);

                // Redo
                Item = PreviousItemChange.Item;

                Modified = true;
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