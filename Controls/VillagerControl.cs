using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACSE.Classes.Utilities;

namespace ACSE.Controls
{
    /// <inheritdoc/>
    /// <summary>
    /// A one-in-all control for villager editing.
    /// </summary>
    public sealed class VillagerControl : FlowLayoutPanel
    {
        private readonly Label _indexLabel;
        private readonly ComboBox _villagerSelectionBox;
        private readonly ComboBox _personalityBox;
        private readonly OffsetablePictureBox _villagerPreviewBox;
        private readonly TextBox _catchphraseBox;
        private readonly CheckBox _boxedCheckBox;
        private readonly SingleItemEditor _shirtEditor;
        private readonly ItemEditor _furnitureEditor;
        private readonly ItemEditor _carpetWallpaperEditor;
        private readonly SingleItemEditor _umbrellaEditor;
        private readonly SingleItemEditor _musicEditor;

        /// <summary>
        /// The index of the control.
        /// </summary>
        public readonly int Index;

        private readonly Save _saveFile;
        private readonly Villager _villager;
        private readonly Dictionary<ushort, SimpleVillager> _villagers;
        private readonly string[] _villagerNames;
        private readonly string[] _personalityTypes;

        /// <inheritdoc/>
        /// <summary>
        /// Initializes a new VillagerControl object.
        /// </summary>
        /// <param name="mainFormReference">The main form reference, which is used by the item editor subcontrols.</param>
        /// <param name="index">The index of the control.</param>
        /// <param name="saveFile">The currently open <see cref="T:ACSE.Save" />.</param>
        /// <param name="villager">The <see cref="T:ACSE.Villager" /> that is being edited.</param>
        /// <param name="villagers">A dictionary of <see cref="T:ACSE.SimpleVillager" />s and their villager ids.</param>
        /// <param name="villagerNames">An array of villager names.</param>
        /// <param name="personalityTypes">An array of personality names.</param>
        public VillagerControl(MainForm mainFormReference, int index, Save saveFile, Villager villager,
            Dictionary<ushort, SimpleVillager> villagers,
            string[] villagerNames, string[] personalityTypes)
        {
            Index = index;
            _saveFile = saveFile;
            _villager = villager;
            _villagers = villagers;
            _villagerNames = villagerNames;
            _personalityTypes = personalityTypes;

            FlowDirection = FlowDirection.LeftToRight;
            AutoSize = true;
            Margin = new Padding(0);
            MaximumSize = new Size(0, 66);
            
            // Set up controls that are always used

            _indexLabel = new Label
            {
                AutoSize = false,
                Size = new Size(45, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = index == 16 ? "Islander" : (index + 1).ToString()
            };

            var margin = CalculateControlVerticalMargin(_indexLabel);
            _indexLabel.Margin = new Padding(0, margin, 0, margin);

            _villagerSelectionBox = new ComboBox
            {
                Size = new Size(120, 32),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            margin = CalculateControlVerticalMargin(_villagerSelectionBox);
            _villagerSelectionBox.Margin = new Padding(0, margin, 10, margin);
            _villagerSelectionBox.Items.AddRange(_villagerNames);

            for (var i = 0; i < _villagers.Count; i++)
            {
                if (villagers.ElementAt(i).Key != _villager.Data.VillagerId) continue;
                _villagerSelectionBox.SelectedIndex = i;
                break;
            }

            _villagerSelectionBox.SelectedIndexChanged += (s, e) => VillagerSelectionBoxChanged();

            _personalityBox = new ComboBox
            {
                Size = new Size(80, 32),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            margin = CalculateControlVerticalMargin(_personalityBox);
            _personalityBox.Margin = new Padding(0, margin, 10, margin);
            _personalityBox.Items.AddRange(_personalityTypes);
            _personalityBox.SelectedIndex = _villager.Data.Personality % _personalityTypes.Length;
            _personalityBox.SelectedIndexChanged += (o, e) => PersonalityChanged();

            _catchphraseBox = new TextBox
            {
                Size = new Size(100, 32),
                MaxLength = _villager.Offsets.CatchphraseSize,
                Text = _villager.Data.Catchphrase
            };

            margin = CalculateControlVerticalMargin(_catchphraseBox);
            _catchphraseBox.Margin = new Padding(0, margin, 10, margin);
            _catchphraseBox.TextChanged += (s, e) => CatchphraseChanged();

            _shirtEditor = new SingleItemEditor(mainFormReference, _villager.Data.Shirt, 16);
            margin = CalculateControlVerticalMargin(_shirtEditor);
            _shirtEditor.Margin = new Padding(0, margin, 10, margin);

            if (_villager.Data.Umbrella != null)
            {
                _umbrellaEditor = new SingleItemEditor(mainFormReference, _villager.Data.Umbrella, 16);
                margin = CalculateControlVerticalMargin(_umbrellaEditor);
                _umbrellaEditor.Margin = new Padding(0, margin, 10, margin);
            }

            // Add controls to flow panel

            Controls.Add(_indexLabel);
            Controls.Add(_villagerSelectionBox);
            Controls.Add(_personalityBox);
            Controls.Add(_catchphraseBox);
            Controls.Add(_shirtEditor);
            Controls.Add(_umbrellaEditor);

            // Set up controls that are used on a per-save generation basis

            switch (_saveFile.Save_Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    _villagerPreviewBox = new OffsetablePictureBox
                    {
                        Size = new Size(64, 64),
                        Image = Properties.Resources.Villagers,
                        Offset = (_villager.Data.VillagerId < 0xE000 || _villager.Data.VillagerId > 0xE0EB) ? new Point(64 * 6, 64 * 23)
                            : new Point(64 * ((_villager.Data.VillagerId & 0xFF) % 10), 64 * ((_villager.Data.VillagerId & 0xFF) / 10))
                    };
                    Controls.Add(_villagerPreviewBox);
                    break;
                case SaveGeneration.NDS:
                case SaveGeneration.Wii:
                case SaveGeneration.N3DS:
                    _carpetWallpaperEditor = new ItemEditor(mainFormReference,
                        new[] {_villager.Data.Carpet, _villager.Data.Wallpaper}, 2, 16);
                    margin = CalculateControlVerticalMargin(_carpetWallpaperEditor);
                    _carpetWallpaperEditor.Margin = new Padding(0, margin, 10, margin);

                    _musicEditor = new SingleItemEditor(mainFormReference, _villager.Data.Song, 16);
                    margin = CalculateControlVerticalMargin(_musicEditor);
                    _musicEditor.Margin = new Padding(0, margin, 10, margin);

                    _furnitureEditor = new ItemEditor(mainFormReference, _villager.Data.Furniture,
                        _villager.Data.Furniture.Length, 16);
                    margin = CalculateControlVerticalMargin(_furnitureEditor);
                    _furnitureEditor.Margin = new Padding(0, margin, 10, margin);

                    Controls.Add(_carpetWallpaperEditor);
                    Controls.Add(_musicEditor);
                    Controls.Add(_furnitureEditor);

                    if (_saveFile.Save_Generation == SaveGeneration.N3DS)
                    {
                        _boxedCheckBox = new CheckBox
                        {
                            Text = "Boxed",
                            Checked = _villager.Boxed()
                        };

                        margin = CalculateControlVerticalMargin(_boxedCheckBox);
                        _boxedCheckBox.Margin = new Padding(0, margin, 10, margin);
                        _boxedCheckBox.CheckedChanged += (o, e) => BoxedCheckBoxChanged();

                        Controls.Add(_boxedCheckBox);
                    }

                    break;
                case SaveGeneration.Unknown:
                    break;
                default:
                    Console.Write($"Unhandled save generation {_saveFile.Save_Generation}!");
                    break;
            }
        }

        private void VillagerSelectionBoxChanged()
        {
            if (_villagerSelectionBox.SelectedIndex < 0) return;

            var kvPair = _villagers.ElementAt(_villagerSelectionBox.SelectedIndex);
            _villager.Name = _villagerNames[_villagerSelectionBox.SelectedIndex];
            _villager.Data.VillagerId = kvPair.Value.VillagerId;
            _villager.Exists = _villager.Data.VillagerId != 0 && _villager.Data.VillagerId != 0xFFFF;

            if (_saveFile.Save_Generation != SaveGeneration.N64 && _saveFile.Save_Generation != SaveGeneration.GCN &&
                _saveFile.Save_Generation != SaveGeneration.iQue) return;

            if (_saveFile.Save_Type != SaveType.Doubutsu_no_Mori_e_Plus)
            {
                _villager.Data.NameId = _villager.Index == 15 ? (byte) 0xFF : (byte) _villager.Data.VillagerId;
            }

            if (!_villager.Exists)
            {
                _villager.Data.HouseCoordinates = new byte[] {0xFF, 0xFF, 0xFF, 0xFF};
            }
            else
            {
                var houseCoordinatesInfo = Utility.Find_Villager_House(_villager.Data.VillagerId);
                _villager.Data.HouseCoordinates = houseCoordinatesInfo.Item1;
                if (!houseCoordinatesInfo.Item2)
                {
                    MessageBox.Show(
                        $"Couldn't find a valid house for {_villager.Name}!\nThey will have a random sign chosen as their house location if you don't place one.",
                        "Villager House Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (_villagerPreviewBox != null)
            {
                _villagerPreviewBox.Offset =
                    (_villager.Data.VillagerId < 0xE000 || _villager.Data.VillagerId > 0xE0EB)
                        ? new Point(64 * 6, 64 * 23)
                        : new Point(64 * ((_villager.Data.VillagerId & 0xFF) % 10),
                            64 * ((_villager.Data.VillagerId & 0xFF) / 10));
            }
        }

        private void PersonalityChanged()
        {
            if (_personalityBox.SelectedIndex > -1)
            {
                _villager.Data.Personality = (byte) _personalityBox.SelectedIndex;
            }
        }

        private void CatchphraseChanged()
        {
            if (!string.IsNullOrWhiteSpace(_catchphraseBox.Text))
            {
                _villager.Data.Catchphrase = _catchphraseBox.Text;
            }
        }

        private void BoxedCheckBoxChanged()
        {
            if (_boxedCheckBox.Checked)
            {
                _villager.Data.Status |= 1;
            }
            else
            {
                _villager.Data.Status &= 0xFE;
            }
        }

        private int CalculateControlVerticalMargin(Control c)
            => (int) (0.5f * (Height - c.Height));
    }
}
