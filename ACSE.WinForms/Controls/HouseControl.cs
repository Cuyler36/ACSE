using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ACSE.Core.Housing;
using ACSE.Core.Saves;
using ItemChangedEventArgs = ACSE.Core.Items.ItemChangedEventArgs;

namespace ACSE.WinForms.Controls
{
    /// <inheritdoc/>
    /// <summary>
    /// An all-in-one Control for editing <see cref="Core.Housing.House"/> objects.
    /// </summary>
    internal sealed class HouseControl : FlowLayoutPanel
    {
        private  House _house;

        public readonly IReadOnlyList<FurnitureItemEditor[]> RoomEditors;
        public readonly IReadOnlyList<SingleItemEditor> WallpaperEditors;
        public readonly IReadOnlyList<SingleItemEditor> CarpetEditors;
        public readonly IReadOnlyList<SingleItemEditor> SongEditors;

        public House House
        {
            get => _house;
            set
            {
                _house = value;
                RefreshHouse();
            }
        }

        public HouseControl(MainForm mainFormReference, Save saveFile, House house)
        {
            _house = house;

            // Initialize the FlowLayoutPanel container
            FlowDirection = FlowDirection.TopDown;
            AutoSize = true;

            // Create the editors & room labels
            var roomEditors = new List<FurnitureItemEditor[]>();
            var wallpaperEditors = new List<SingleItemEditor>();
            var carpetEditors = new List<SingleItemEditor>();
            var songEditors = new List<SingleItemEditor>();

            foreach (var room in House.Data.Rooms)
            {
                // Create the info panel
                var infoFlowPanel = new FlowLayoutPanel
                {
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    MaximumSize = new Size(0, 28)
                };
                Controls.Add(infoFlowPanel);

                // Create the room panel
                var roomPanel = new FlowLayoutPanel
                {
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight
                };
                Controls.Add(roomPanel);

                // Add the room label
                var roomLabel = new Label
                {
                    AutoSize = true,
                    Font = new Font("Microsoft Sans Serif", 18, FontStyle.Regular),
                    Text = room.Name,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                infoFlowPanel.Controls.Add(roomLabel);

                // Add each layer editor
                var editors = new List<FurnitureItemEditor>();
                foreach (var layer in room.Layers)
                {
                    var layerSize = 16;
                    if (saveFile.SaveGeneration == SaveGeneration.N3DS)
                    {
                        layerSize = editors.Count == 0 ? 10 : 8;
                    }

                    var editor = new FurnitureItemEditor(layer.Items, layerSize, 16);
                    roomPanel.Controls.Add(editor);

                    if (roomPanel.MaximumSize.Height == 0)
                    {
                        roomPanel.MaximumSize = new Size(0, editor.Height);
                    }

                    editor.SetCenterMargins(5, 5);
                    editors.Add(editor);
                }

                // Set Room Label center margin
                roomLabel.SetCenterMargins(5, 5);

                roomEditors.Add(editors.ToArray());

                // Add the wallpaper, carpet, & song editors
                var wallpaperEditor = new SingleItemEditor(room.Wallpaper, 16);
                infoFlowPanel.Controls.Add(wallpaperEditor);
                wallpaperEditor.SetCenterMargins(5, 5);
                wallpaperEditors.Add(wallpaperEditor);
                wallpaperEditor.ItemChanged += delegate(object sender, ItemChangedEventArgs e)
                {
                    room.Wallpaper = e.NewItem;
                };

                var carpetEditor = new SingleItemEditor(room.Carpet, 16);
                infoFlowPanel.Controls.Add(carpetEditor);
                carpetEditor.SetCenterMargins(5, 5);
                carpetEditors.Add(carpetEditor);
                carpetEditor.ItemChanged += delegate(object sender, ItemChangedEventArgs e)
                {
                    room.Carpet = e.NewItem;
                };

                if (saveFile.SaveGeneration != SaveGeneration.N3DS) continue;
                var songEditor = new SingleItemEditor(room.Song, 16);
                infoFlowPanel.Controls.Add(songEditor);
                songEditor.SetCenterMargins(5, 5);
                songEditors.Add(songEditor);
                songEditor.ItemChanged += delegate(object sender, ItemChangedEventArgs e) { room.Song = e.NewItem; };
            }

            // Set the IReadOnlyList components
            RoomEditors = roomEditors;
            WallpaperEditors = wallpaperEditors;
            CarpetEditors = carpetEditors;
            SongEditors = songEditors;
        }

        private void RefreshHouse()
        {
            for (var i = 0; i < House.Data.Rooms.Length; i++)
            {
                var room = House.Data.Rooms[i];
                for (var x = 0; x < room.Layers.Length; x++)
                {
                    var layer = room.Layers[x];
                    RoomEditors[i][x].Items = layer.Items;
                }

                WallpaperEditors[i].Item = room.Wallpaper;
                CarpetEditors[i].Item = room.Carpet;
                if (SongEditors == null || SongEditors.Count <= i) continue;
                SongEditors[i].Item = room.Song;
            }
        }
    }
}
