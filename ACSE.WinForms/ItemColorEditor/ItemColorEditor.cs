using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using ACSE.Core.Items;

namespace ACSE.WinForms
{
    public partial class ItemColorEditor : Form
    {
        private static Panel[] _panels;

        public ItemColorEditor()
        {
            InitializeComponent();
            UpdateItemColors(); // Initialize at the start 
            CreateEditorControls();
        }

        private void CreateEditorControls()
        {
            var panels = new List<Panel>();
            for (var i = 1; i < ItemData.ItemTypeNames.Length; i++) // from 1 to the end, so we don't include the empty type
            {
                var isInvalidColor = i == ItemData.ItemTypeNames.Length - 1;

                var container = new FlowLayoutPanel
                {
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight,
                };

                var label = new Label
                {
                    AutoSize = false,
                    Size = new Size(60, 28),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = ItemData.ItemTypeNames[i]
                };

                var colorPreview = new Panel
                {
                    BackColor = Color.FromArgb((int)ItemData.GetItemColor(isInvalidColor ? ItemType.Invalid : (ItemType)i)),
                    BorderStyle = BorderStyle.FixedSingle,
                    Size = new Size(28, 28),
                    Tag = isInvalidColor ? -1 : i
                };
                colorPreview.Click += (s, e) => ColorPreviewClick(s);

                container.Controls.Add(label);
                container.Controls.Add(colorPreview);

                ColorContainer.Controls.Add(container);
                panels.Add(colorPreview);
            }

            _panels = panels.ToArray();

            var resetButton = new Button
            {
                AutoSize = false,
                Size = new Size(100, 24),
                Text = "Reset Colors"
            };

            resetButton.Click += (s, e) => ResetColors();

            var closeButton = new Button
            {
                AutoSize = false,
                Size = new Size(100, 24),
                Text = "Save"
            };

            closeButton.Click += (s, e) => Done();

            ColorContainer.Controls.Add(resetButton);
            ColorContainer.Controls.Add(closeButton);
        }

        private static void ColorPreviewClick(object sender)
        {
            if (!(sender is Panel colorPreview)) return;

            using (var colorPicker = new ColorDialog())
            {
                if (colorPicker.ShowDialog() != DialogResult.OK) return;
                var colorSettingName = (ItemType) (int) colorPreview.Tag + "Color";
                var currentColor = (uint) ItemColorSettings.Default[colorSettingName];
                var realColor = currentColor & 0xFF000000 | ((uint)colorPicker.Color.R << 16) | ((uint)colorPicker.Color.G << 8) |
                                colorPicker.Color.B;
                SetColor(colorPreview, realColor);
                ItemColorSettings.Default[colorSettingName] = realColor;
            }
        }

        private static void ResetColors()
        {
            ItemColorSettings.Default.Reset();
            foreach (var panel in _panels)
            {
                var colorSettingName = ((ItemType) (int) panel.Tag) + "Color";
                var settingValue = (uint) ItemColorSettings.Default[colorSettingName];
                panel.BackColor = Color.FromArgb((int) settingValue);
            }

            ItemColorSettings.Default.Save();
        }

        private static void SetColor(Control colorPreview, uint newColor)
        {
            colorPreview.BackColor = Color.FromArgb((int)newColor);
        }

        // TODO: I don't like this at all. Come up with a better way. Possibly expose the colors from ACSE.Core instead.
        public static void UpdateItemColors()
        {
            var database = new Dictionary<string, uint>();

            foreach (SettingsProperty prop in ItemColorSettings.Default.Properties)
            {
                database.Add(prop.Name, (uint) ItemColorSettings.Default[prop.Name]);
            }

            ItemData.ItemColorsSettings = database;
        }

        private void Done()
        {
            ItemColorSettings.Default.Save();
            UpdateItemColors();
            Close();
            Dispose();
        }
    }
}
