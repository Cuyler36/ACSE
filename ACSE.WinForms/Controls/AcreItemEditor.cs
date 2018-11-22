using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Saves;
using ACSE.Core.Town.Acres;
using ACSE.Core.Town.Buildings;
using ACSE.WinForms.Imaging;

namespace ACSE.WinForms.Controls
{
    /// <inheritdoc cref="ItemEditor" />
    /// <summary>
    /// All-in-one class for editing <see cref="Item" />s and <see cref="Building"/>s which belong to a <see cref="WorldAcre" />.
    /// </summary>
    public sealed class AcreItemEditor : ItemEditor
    {
        /// <summary>
        /// Determines if the item that is modified is set as buried.
        /// </summary>
        public static bool BuryItem;

        /// <summary>
        /// This event fires when a new item is selected.
        /// </summary>
        public new event ItemSelectedHandler ItemSelected;

        /// <summary>
        /// Signals when a building has changed location via the AcreItemEditor.
        /// </summary>
        public event BuildingChangedHandler BuildingChanged;

        /// <summary>
        /// Signals when a building is selected.
        /// </summary>
        public event BuildingSelectedHandler BuildingSelected;

        /// <summary>
        /// The <see cref="WorldAcre"/> whose items are being edited.
        /// </summary>
        public WorldAcre Acre { get; }

        /// <summary>
        /// The <see cref="Item"/>s associated with the acre.
        /// </summary>
        public new Item[] Items
        {
            get => Acre?.Items;
            set
            {
                if (value == null || value.Length != 256)
                    throw new ArgumentException($"{nameof(value)} cannot be null and must contain 256 items.");

                Acre.Items = value;
                SetItemPicture();
            }
        }

        /// <summary>
        /// This handler is used when a <see cref="Building"/> has changed some property.
        /// </summary>
        /// <param name="previousAcre">The previous acre index the building was in.</param>
        /// <param name="newAcre">The current acre index the building is now in.</param>
        /// <param name="building">The <see cref="Building"/> that was modified in some way.</param>
        public delegate void BuildingChangedHandler(int previousAcre, int newAcre, Building building);

        /// <summary>
        /// This handler is used when a <see cref="Building"/> is selected.
        /// </summary>
        /// <param name="building"></param>
        public delegate void BuildingSelectedHandler(Building building);

        /// <summary>
        /// Initializes a new AcreItemEditor control.
        /// </summary>
        /// <param name="acre">The <see cref="WorldAcre"/> whose items will be edited.</param>
        /// <param name="itemsPerRow">The number of items per row. Defaults to 16.</param>
        /// <param name="cellSize">The pixel size of each item cell.</param>
        public AcreItemEditor(WorldAcre acre, int itemsPerRow = 16, int cellSize = 8)
            : base(acre?.Items, itemsPerRow, cellSize)
        {
            InterpolationMode = (InterpolationMode) Properties.Settings.Default.ImageResizeMode;
            DrawBaseImage = false;
            Acre = acre;
            SetItemPicture();
        }

        protected override void SetItemPicture()
        {
            if (Items == null) return;

            Size = new Size(ItemCellSize * ItemsPerRow + 3,
                ItemCellSize * (int) Math.Ceiling((decimal) Items.Length / ItemsPerRow) + 3);

            Image?.Dispose();

            CurrentItemImage = Inventory.GetItemPic(ItemCellSize, ItemsPerRow, Items, MainForm.SaveFile.SaveType,
                Size);
            ImageGeneration.DrawBuildings((Bitmap) CurrentItemImage, Building.Buildings, Acre.Index, ItemCellSize); // TODO: How do we draw island buildings?
            ImageGeneration.DrawBuriedIcons((Bitmap) CurrentItemImage, Acre, ItemCellSize);

            Image = (Image) CurrentItemImage.Clone();
            base.Refresh();
        }

        protected override void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            if (!GetXyPosition(e, out var x, out var y, out var index)) return;
            var selectedItem = Items[index];
            var selectedBuilding = Building.IsBuildingHere(Acre.Index, x, y);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Building.SelectedBuilding != null)
                    {
                        if (selectedBuilding == null)
                        {
                            var previousAcre = Building.SelectedBuilding.AcreIndex;
                            Building.SelectedBuilding.AcreIndex = (byte) Acre.Index;
                            Building.SelectedBuilding.XPos = (byte) x;
                            Building.SelectedBuilding.YPos = (byte) y;

                            // TODO: Update the selected building's X & Y acre positions.

                            // It is the responsibility of the user to hook into the BuildingChanged event.
                            BuildingChanged?.Invoke(previousAcre, Acre.Index, Building.SelectedBuilding);

                            Refresh();
                        }

                        Building.SelectedBuilding = null;
                        BuildingSelected?.Invoke(null); // Clear the selected building even if there was a building at the selected location.
                        return;
                    }

                    break;

                case MouseButtons.Right:
                    if (Building.Buildings != null && selectedBuilding != null)
                    {
                        Building.SelectedBuilding = selectedBuilding;
                        BuildingSelected?.Invoke(selectedBuilding);
                        return; // End execution immediately. We don't need to update the image or any item info when a building was selected.
                    }

                    BuildingSelected?.Invoke(null); // Clear selected building.
                    break;
            }

            // Execute base logic.
            base.OnEditorMouseDown(sender, e);

            switch (e.Button)
            {
                case MouseButtons.Left:
                    // Set the buried state of the new item.
                    Acre.SetItemBuried(Items[index], BuryItem, Save.SaveInstance.SaveGeneration);

                    SetItemPicture();

                    // Set the updated image since the one set before is inaccurate.
                    this.SetNewImage((Image) CurrentItemImage.Clone());
                    break;

                case MouseButtons.Right:
                    ItemSelected?.Invoke(selectedItem, Acre.IsItemBuried(selectedItem));
                    break;
            }
        }

        protected override string GetToolTipString(int x, int y, params object[] parameters)
        {
            var item = (Item) parameters[0];
            var b = Building.IsBuildingHere(Acre.Index, x, y); // TODO: How do we determine if this is an island acre?

            if (b != null)
            {
                return $"{b.Name} - [0x{b.Id:X2} - Building]";
            }

            var buried = Acre.IsItemBuried(item);
            var watered = (item.Flag1 & 0x40) != 0;
            var hasPerfectFruit = item.Type == ItemType.Tree && item.Flag1 == 1;

            var statusString = buried ? " (Buried)" : "";
            if (watered)
            {
                statusString += "(Watered)";
            }

            if (hasPerfectFruit)
            {
                statusString += " (Perfect Fruit)";
            }

            return $"{item.Name}{statusString} - [0x{item.ItemId:X4}]";
        }

        /// <summary>
        /// Manually refreshes the editor's item image.
        /// </summary>
        public new void Refresh()
        {
            SetItemPicture();
            base.Refresh();
        }
    }
}
