using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Modifiable;
using ACSE.WinForms.Utilities;
using ItemChangedEventArgs = ACSE.Core.Items.ItemChangedEventArgs;

namespace ACSE.WinForms.Controls
{
    /// <inheritdoc cref="IModifiable" />
    /// <summary>
    /// An all-in-one control for editing a single item.
    /// </summary>
    public partial class SingleItemEditor : UserControl, IModifiable
    {
        /// <summary>
        /// This event fires when the item is set or changed. PreviousItem will be null if it is the first time setting the item.
        /// </summary>
        public event EventHandler<ItemChangedEventArgs> ItemChanged;

        /// <summary>
        /// This event fires when a new item is selected.
        /// </summary>
        public event ItemSelectedHandler ItemSelected;


        // Delegates
        public delegate void ItemSelectedHandler(Item selectedItem, bool buried);

        public readonly PictureBox EditorPictureBox;
        public readonly ToolTip ItemToolTip;
        public readonly Stack<ItemChange> UndoStack = new Stack<ItemChange>();
        public readonly Stack<ItemChange> RedoStack = new Stack<ItemChange>();
        public bool Modified { get; protected set; }

        private bool _disabled;
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;

                var img = EditorPictureBox.Image;
                EditorPictureBox.Image = MainForm.ImageX;

                img?.Dispose();
            }
        }

        private int _itemCellSize = 8;
        public int ItemCellSize
        {
            get => _itemCellSize;
            set
            {
                _itemCellSize = value;
                if (!_disabled && EditorPictureBox != null)
                {
                    SetItemPicture();
                }
            }
        }

        private Item _item;
        public virtual Item Item
        {
            get => _item;
            set
            {
                var previousItem = _item;
                _item = value;

                OnItemChanged(previousItem, value);

                if (!_disabled)
                    SetItemPicture();
            }
        }

        private void SetItemPicture()
        {
            var img = EditorPictureBox.Image;

            if (Item != null)
            {
                Size = new Size(_itemCellSize, _itemCellSize);
                EditorPictureBox.Image = Inventory.GetItemPic(_itemCellSize, _item, MainForm.SaveFile.SaveType);
            }

            img?.Dispose();
        }

        private int _lastX = -1, _lastY = -1;
        private bool _isMouseDown;

        protected SingleItemEditor()
        {
            ItemToolTip = ToolTipManager.GetSharedToolTip();

            EditorPictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Dock = DockStyle.Fill
            };

            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(EditorPictureBox);
        }

        public SingleItemEditor(Item item, int itemCellSize = 8) : this()
        {
            _item = item;
            ItemCellSize = itemCellSize;

            EditorPictureBox.MouseMove += OnEditorMouseMove;
            EditorPictureBox.MouseLeave += (sender, e) => ItemToolTip.Hide(this);

            EditorPictureBox.MouseDown += OnEditorMouseDown;
            EditorPictureBox.MouseUp += (sender, e) => _isMouseDown = false;
        }

        protected virtual void OnItemChanged(Item previousItem, Item newItem)
        {
            ItemChanged?.Invoke(this, new ItemChangedEventArgs { PreviousItem = previousItem, NewItem = newItem });
        }

        protected virtual void PushNewItemChange(Item oldItem, int itemIndex, Stack<ItemChange> stack)
        {
            stack.Push(new ItemChange { Item = oldItem, Index = itemIndex });
        }

        protected virtual void OnEditorMouseMove(object sender, MouseEventArgs e)
        {
            if (_item == null || _disabled || (e.X == _lastX && e.Y == _lastY)) return;
            // Update Last Hover Position
            _lastX = e.X;
            _lastY = e.Y;

            // Refresh ToolTip
            ItemToolTip.Show($"{_item.Name} - [0x{_item.ItemId:X4}]", this, e.X + 10, e.Y + 10, 100000);

            // Check for MouseDown
            if (_isMouseDown)
                OnEditorMouseDown(sender, e);
        }

        protected virtual void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            if (_disabled) return;
            _isMouseDown = true;
            switch (e.Button)
            {
                case MouseButtons.Left:
                case MouseButtons.Middle:
                    var newItem = Item.SelectedItem;
                    var previousItem = _item;

                    if (previousItem != newItem)
                    {

                        // Save Old Item
                        PushNewItemChange(previousItem, 0, UndoStack);

                        // Set New Item
                        Item = newItem;

                        // Update ToolTip
                        ItemToolTip.Show($"{newItem.Name} - [0x{newItem.ItemId:X4}]", this, e.X + 10, e.Y + 10, 100000);
                        MainForm.SaveFile.ChangesMade = true;
                        Modified = true;
                    }

                    break;
                case MouseButtons.Right:
                    ItemSelected?.Invoke(_item, false);
                    break;
            }
        }

        public virtual void Undo()
        {
            if (!UndoStack.Any()) return;
            // Get Previous Change
            var previousItemChange = UndoStack.Pop();

            // Set Redo Change
            PushNewItemChange(_item, previousItemChange.Index, RedoStack);

            // Undo
            Item = previousItemChange.Item;

            Modified = UndoStack.Any();
        }

        public virtual void Redo()
        {
            if (!RedoStack.Any()) return;
            // Get Previous Change
            var previousItemChange = RedoStack.Pop();

            // Set Undo Change
            PushNewItemChange(_item, previousItemChange.Index, UndoStack);

            // Redo
            Item = previousItemChange.Item;

            Modified = true;
        }

        public virtual void NewChange(object change)
        {
            if (!(change is ItemChange newItem)) return;
            RedoStack.Clear();
            UndoStack.Push(newItem);
        }

        protected virtual void Dipose()
        {
            EditorPictureBox.Image?.Dispose();
            EditorPictureBox.Dispose();
            Dispose();
        }
    }
}