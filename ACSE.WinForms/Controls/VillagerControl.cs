using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ACSE.Core.Compression;
using ACSE.Core.Items;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;
using ACSE.Core.Villagers;
using ItemChangedEventArgs = ACSE.Core.Items.ItemChangedEventArgs;

namespace ACSE.WinForms.Controls
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
        private readonly TextBox _nameBox;
        private readonly Button _importDlcButton;

        /// <summary>
        /// This event fires when the villager has been changed to another.
        /// </summary>
        public event EventHandler<Villager> VillagerChanged;

        /// <summary>
        /// The index of the control.
        /// </summary>
        public readonly int Index;

        private readonly Save _saveFile;
        private Villager _villager;
        private readonly Dictionary<ushort, SimpleVillager> _villagers;
        private readonly string[] _villagerNames;
        private readonly string[] _personalityTypes;

        private static readonly Bitmap VillagerList = Properties.Resources.Villagers; // currently for GC only

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
                Text = saveFile.SaveGeneration == SaveGeneration.GCN && index == 15 ? "Islander" : (index + 1).ToString()
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

            _shirtEditor = new SingleItemEditor(_villager.Data.Shirt, 16);
            margin = CalculateControlVerticalMargin(_shirtEditor);
            _shirtEditor.Margin = new Padding(0, margin, 10, margin);
            _shirtEditor.ItemChanged += delegate(object sender, ItemChangedEventArgs e)
            {
                _villager.Data.Shirt = e.NewItem;
            };

            if (_villager.Data.Umbrella != null)
            {
                _umbrellaEditor = new SingleItemEditor(_villager.Data.Umbrella, 16);
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

            switch (_saveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    // e+ exclusive controls TODO: These will probably be used in City Folk as well.
                    if (_saveFile.SaveType == SaveType.DoubutsuNoMoriEPlus || _saveFile.SaveType == SaveType.AnimalForestEPlus)
                    {
                        _nameBox = new TextBox
                        {
                            Size = new Size(60, 32),
                            Text = _villager.Name,
                            MaxLength = _saveFile.SaveType == SaveType.AnimalForestEPlus ? 8 : 6
                        };

                        margin = CalculateControlVerticalMargin(_nameBox);
                        _nameBox.Margin = new Padding(0, margin, 10, margin);
                        _nameBox.TextChanged += (s, e) => NameTextChanged();

                        _importDlcButton = new Button
                        {
                            Text = "Import DLC Data",
                            AutoSize = true
                        };

                        margin = CalculateControlVerticalMargin(_importDlcButton);
                        _importDlcButton.Margin = new Padding(0, margin, 10, margin);
                        _importDlcButton.Click += (s, e) => ImportDlcVillager();

                        Controls.Add(_nameBox);
                        Controls.Add(_importDlcButton);
                    }

                    _villagerPreviewBox = new OffsetablePictureBox
                    {
                        Size = new Size(64, 64),
                        Image = VillagerList,
                        Offset = (_villager.Data.VillagerId < 0xE000 || _villager.Data.VillagerId > 0xE0EB) ? new Point(64 * 6, 64 * 23)
                            : new Point(64 * ((_villager.Data.VillagerId & 0xFF) % 10), 64 * ((_villager.Data.VillagerId & 0xFF) / 10))
                    };
                    Controls.Add(_villagerPreviewBox);

                    break;
                case SaveGeneration.NDS:
                case SaveGeneration.Wii:
                case SaveGeneration.N3DS:
                    _carpetWallpaperEditor =
                        new ItemEditor(new[] {_villager.Data.Carpet, _villager.Data.Wallpaper}, 2, 16);
                    margin = CalculateControlVerticalMargin(_carpetWallpaperEditor);
                    _carpetWallpaperEditor.Margin = new Padding(0, margin, 10, margin);

                    _musicEditor = new SingleItemEditor(_villager.Data.Song, 16);
                    margin = CalculateControlVerticalMargin(_musicEditor);
                    _musicEditor.Margin = new Padding(0, margin, 10, margin);

                    _musicEditor.ItemChanged += delegate(object sender, ItemChangedEventArgs e)
                    {
                        _villager.Data.Song = e.NewItem;
                    };

                    _furnitureEditor = new ItemEditor(_villager.Data.Furniture,
                        _villager.Data.Furniture.Length, 16);
                    margin = CalculateControlVerticalMargin(_furnitureEditor);
                    _furnitureEditor.Margin = new Padding(0, margin, 10, margin);

                    Controls.Add(_carpetWallpaperEditor);
                    Controls.Add(_musicEditor);
                    Controls.Add(_furnitureEditor);

                    if (_saveFile.SaveGeneration == SaveGeneration.N3DS)
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
                    Console.Write($"Unhandled save generation {_saveFile.SaveGeneration}!");
                    break;
            }
        }

        private void VillagerSelectionBoxChanged(bool fireChangedEvent = true)
        {
            if (_villagerSelectionBox.SelectedIndex < 0) return;

            var kvPair = _villagers.ElementAt(_villagerSelectionBox.SelectedIndex);
            if (_saveFile.SaveType != SaveType.DoubutsuNoMoriEPlus && _saveFile.SaveType != SaveType.AnimalForestEPlus)
            {
                _villager.Name = _villagerNames[_villagerSelectionBox.SelectedIndex];
            }

            _villager.Data.VillagerId = kvPair.Value.VillagerId;
            _villager.Exists = _villager.Data.VillagerId != 0 && _villager.Data.VillagerId != 0xFFFF;

            switch (_saveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    if (!_villager.Exists)
                    {
                        _villager.Data.HouseCoordinates = new byte[] {0xFF, 0xFF, 0xFF, 0xFF};
                    }
                    else
                    {
                        // TODO: Update islander house location. In e+, update all four islander house locations.
                        // TODO: Remove the static reference to MainForm.TownAcres.
                        if (_saveFile.SaveGeneration != SaveGeneration.GCN || _villager.Index < 15)
                        {
                            var houseCoordinatesInfo =
                                Utility.FindVillagerHouse(_villager.Data.VillagerId, MainForm.TownAcres);
                            _villager.Data.HouseCoordinates = houseCoordinatesInfo.Item1;
                            if (!houseCoordinatesInfo.Item2)
                            {
                                MessageBox.Show(
                                    $"Couldn't find a valid house for {_villager.Name}!\nThey will have a random sign chosen as their house location if you don't place one.",
                                    "Villager House Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    break;
                case SaveGeneration.NDS:
                    if (!_villager.Exists)
                    {
                        _villager.Data.HouseCoordinates = new byte[] { 0xFF, 0xFF };
                    }
                    else
                    {
                        var (houseCoordinates, found) =
                            Utility.FindVillagerHouseWildWorld(_villager.Index, MainForm.TownAcres);
                        _villager.Data.HouseCoordinates = houseCoordinates;
                        if (!found)
                        {
                            MessageBox.Show(
                                $"Couldn't find a valid house for Villager #{_villager.Index}!\nThey will have a random sign chosen as their house location if you don't place one.",
                                "Villager House Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    break;
            }

            if (_saveFile.SaveGeneration != SaveGeneration.N64 && _saveFile.SaveGeneration != SaveGeneration.GCN &&
                _saveFile.SaveGeneration != SaveGeneration.iQue) return;

            if (_saveFile.SaveType != SaveType.DoubutsuNoMoriEPlus && _saveFile.SaveType != SaveType.AnimalForestEPlus)
            {
                _villager.Data.NameId = _villager.Index == 15 ? (byte) 0xFF : (byte) _villager.Data.VillagerId;
            }

            if (_villagerPreviewBox != null)
            {
                _villagerPreviewBox.Offset =
                    (_villager.Data.VillagerId < 0xE000 || _villager.Data.VillagerId > 0xE0EB)
                        ? new Point(64 * 6, 64 * 23)
                        : new Point(64 * ((_villager.Data.VillagerId & 0xFF) % 10),
                            64 * ((_villager.Data.VillagerId & 0xFF) / 10));
            }

            if (fireChangedEvent)
                VillagerChanged?.Invoke(this, _villager);
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

        private void NameTextChanged()
        {
            if (!string.IsNullOrWhiteSpace(_nameBox.Text))
            {
                _villager.Name = _nameBox.Text;
            }
        }

        private void ImportDlcVillager()
        {
            if (_villager.Data.VillagerId < 0xE0EC || _villager.Data.VillagerId > 0xE0FF)
            {
                MessageBox.Show("You must set your villager to one of the DLC villagers before importing!", "DLC Villager Import Info");
                return;
            }

            using (var openFileDialog = new OpenFileDialog {Filter = "Yaz0 compressed file|*.yaz0"})
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                byte[] villagerData = null;
                try
                {
                    villagerData = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("An error occurred while importing the DLC villager info.\nPlease try again!",
                        "DLC Villager Import Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (villagerData != null)
                    {
                        _villager.ImportDlcVillager(villagerData, (_villager.Data.VillagerId - 0xEC) & 0xFF);

                        // Decompress Yaz0 data and set accordingly
                        var decompressedData = Yaz0.Decompress(villagerData);
                        _villager.Name = new AcString(decompressedData.Skip(1).Take(6).ToArray(), _saveFile.SaveType).Trim();
                        _villager.Data.Catchphrase = new AcString(decompressedData.Skip(7).Take(4).ToArray(), _saveFile.SaveType).Trim();
                        _shirtEditor.Item = new Item((ushort)(0x2400 | decompressedData[0xE]));
                        _villager.Data.Personality = decompressedData[0xD];

                        // Update controls to reflect changes
                        _nameBox.Text = _villager.Name;
                        _catchphraseBox.Text = _villager.Data.Catchphrase;
                        _personalityBox.SelectedIndex = _villager.Data.Personality;

                        MessageBox.Show("DLC Villager import was succsessful!", "DLC Villager Import Info",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private int CalculateControlVerticalMargin(Control c)
            => (int) (0.5f * (Height - c.Height));

        public void RefreshData() => VillagerSelectionBoxChanged(false);
        public void SetVillager(Villager villager)
        {
            _villager = villager;
            RefreshData();
        }
    }
}
