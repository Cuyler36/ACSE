using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Utilities;
using ACSE.WinForms.Imaging;

namespace ACSE.WinForms.Controls
{
    internal sealed class FurnitureItemEditor : ItemEditor
    {
        public FurnitureItemEditor(MainForm mainForm, Furniture[] furniture, int itemsPerRow, int itemCellSize = 8) :
            base(mainForm, furniture, itemsPerRow, itemCellSize) { }

        protected override void SetItemPicture()
        {
            if (Items == null) return;

            Size = new Size(ItemCellSize * ItemsPerRow + 3, ItemCellSize * (int)(Math.Ceiling((decimal)Items.Length / ItemsPerRow)) + 3);

            CurrentItemImage?.Dispose();
            CurrentItemImage = ImageGeneration.DrawFurnitureArrows((Bitmap) Inventory.GetItemPic(ItemCellSize,
                    ItemsPerRow, Items, MainForm.SaveFile.SaveType, EditorPictureBox.Size), (Furniture[]) Items,
                ItemsPerRow);

            EditorPictureBox.Image?.Dispose();
            EditorPictureBox.Image = (Image) CurrentItemImage.Clone();
        }

        protected override void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            if (!GetXyPosition(e, out var x, out var y, out var index)) return;
            var selectedItem = Items[index];
            switch (e.Button)
            {
                case MouseButtons.Left:
                    var newItem = new Furniture(MainFormReference.GetCurrentItem());

                    if (selectedItem != newItem)
                    {
                        // Save Old Item
                        PushNewItemChange(selectedItem, index, UndoStack);

                        // Clear Redo Stack
                        NewChange(null);

                        // Set New Item
                        Items[index] = newItem;

                        // Redraw Item Image
                        CurrentItemImage?.Dispose();
                        CurrentItemImage = ImageGeneration.DrawFurnitureArrows((Bitmap) Inventory.GetItemPic(
                                ItemCellSize,
                                ItemsPerRow, Items, MainForm.SaveFile.SaveType, EditorPictureBox.Size),
                            (Furniture[]) Items,
                            ItemsPerRow);

                        EditorPictureBox.Image?.Dispose();
                        EditorPictureBox.Image = (Image)CurrentItemImage.Clone();
                        ImageGeneration.OverlayItemBoxGlow((Bitmap) EditorPictureBox.Image, ItemCellSize, x, y);

                        // Update ToolTip
                        ItemToolTip.Show(
                            string.Format(HoverText, newItem.Name, newItem.ItemId.ToString("X4"),
                                newItem.ItemFlag.ToString()), this, e.X + 10, e.Y + 10, int.MaxValue);

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
                    var tempItems = (Furniture[]) Items;
                    Utility.FloodFillFurnitureArray(ref tempItems, ItemsPerRow, index,
                        (Furniture) Items[index], new Furniture(MainFormReference.GetCurrentItem()));
                    Items = tempItems;
                    break;
            }
        }

        public override void Undo()
        {
            if (!UndoStack.Any()) return;
            // Get Previous Change
            var previousItemChange = UndoStack.Pop();

            var selectedItem = Items[previousItemChange.Index];

            // Set Redo Change
            PushNewItemChange(selectedItem, previousItemChange.Index, RedoStack);

            // Undo
            Items[previousItemChange.Index] = (Furniture)previousItemChange.Item;
            var img = EditorPictureBox.Image;
            EditorPictureBox.Image = ImageGeneration.DrawFurnitureArrows((Bitmap)Inventory.GetItemPic(ItemCellSize,
                ItemsPerRow, Items, MainForm.SaveFile.SaveType, EditorPictureBox.Size), (Furniture[])Items, ItemsPerRow);
            img?.Dispose();

            OnItemChanged(selectedItem, Items[previousItemChange.Index], previousItemChange.Index);
        }

        public override void Redo()
        {
            if (!RedoStack.Any()) return;
            // Get Previous Change
            var previousItemChange = RedoStack.Pop();

            var selectedItem = Items[previousItemChange.Index];

            // Set Undo Change
            PushNewItemChange(selectedItem, previousItemChange.Index, UndoStack);

            // Redo
            Items[previousItemChange.Index] = (Furniture)previousItemChange.Item;
            var img = EditorPictureBox.Image;
            EditorPictureBox.Image = ImageGeneration.DrawFurnitureArrows((Bitmap)Inventory.GetItemPic(ItemCellSize,
                ItemsPerRow, Items, MainForm.SaveFile.SaveType, EditorPictureBox.Size), (Furniture[])Items, ItemsPerRow);
            img?.Dispose();

            OnItemChanged(selectedItem, Items[previousItemChange.Index], previousItemChange.Index);
        }
    }
}
