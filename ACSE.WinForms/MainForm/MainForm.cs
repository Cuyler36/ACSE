using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ACSE.Core.BitFields.Catalog;
using ACSE.Core.BitFields.Encyclopedia;
using ACSE.Core.BitFields.Museum;
using ACSE.Core.BitFields.SongLibrary;
using ACSE.Core.Debug;
using ACSE.Core.Emotions;
using ACSE.Core.Encryption;
using ACSE.Core.Enums;
using ACSE.Core.Generators;
using ACSE.Core.Housing;
using ACSE.Core.Items;
using ACSE.Core.Modifiable;
using ACSE.Core.Patterns;
using ACSE.Core.Players;
using ACSE.Core.Saves;
using ACSE.Core.Town;
using ACSE.Core.Town.Acres;
using ACSE.Core.Town.Buildings;
using ACSE.Core.Town.Island;
using ACSE.Core.Town.TownBuildings;
using ACSE.Core.Updater;
using ACSE.Core.Utilities;
using ACSE.Core.Villagers;
using ACSE.Core.Weather;
using ACSE.WinForms.Backups;
using ACSE.WinForms.Controls;
using ACSE.WinForms.Imaging;
using ACSE.WinForms.Managers;
using ACSE.WinForms.Utilities;
using ContentAlignment = System.Drawing.ContentAlignment;
using ItemChangedEventArgs = ACSE.Core.Items.ItemChangedEventArgs;

namespace ACSE.WinForms
{
    public sealed partial class MainForm : Form
    {
        #region Variables
        public static readonly string AssemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static DebugManager DebugManager => DebugUtility.DebugManagerInstance;
        public static Save SaveFile;
        public static SaveInfo CurrentSaveInfo;
        public static WorldAcre[] TownAcres;
        public static WorldAcre[] IslandAcres;

        internal static ModifiedHandler UndoRedoHandler;
        private static readonly byte[] GridColor = BitConverter.GetBytes(0x56FFFFFF);

        private readonly TabPage[] _mainTabs;
        private readonly TabPage[] _playerTabs = new TabPage[4];
        private readonly TabPage[] _playerPatternTabs = new TabPage[4];
        private readonly Player[] _players = new Player[4];
        private House[] _houses;
        private Villager[] _villagers;
        private PictureBoxWithInterpolationMode[] _acreMap;
        private PictureBoxWithInterpolationMode[] _townAcreMap;
        private PictureBoxWithInterpolationMode[] _islandAcreMap;
        private PictureBoxWithInterpolationMode[] _newLeafIslandAcreMap;
        private PictureBoxWithInterpolationMode[] _grassMap;
        private PictureBoxWithInterpolationMode[] _patternBoxes;
        private readonly PictureBoxWithInterpolationMode _tpcPicture;
        private Image _selectedPattern;
        private int _selectedPaletteIndex;
        private Pattern _selectedPatternObject;
        private Panel[] _buildingListPanels;
        private WorldAcre[] _acres;
        private Building[] _buildings;
        private Building[] _islandBuildings;
        private List<KeyValuePair<ushort, string>> _itemList;
        private Player _selectedPlayer;
        private House _selectedHouse;
        private Dictionary<byte, string> _acreInfo; //Name Database
        private Dictionary<string, List<byte>> _filedAcreData; //Grouped info for Acre Editor TreeView
        private Dictionary<ushort, string> _uInt16AcreInfo;
        private Dictionary<string, Dictionary<ushort, string>> _uInt16FiledAcreData;
        private Dictionary<ushort, SimpleVillager> _villagerDatabase;
        private SimpleVillager[] _pastVillagers;
        private string[] _personalityDatabase;
        private string[] _villagerNames;
        private byte[] _grassWear;
        private AboutBox _aboutBox = new AboutBox();
        private readonly SecureValueForm _secureNandValueForm = new SecureValueForm();
        private readonly SettingsMenuForm _settingsMenu;
        private bool _clicking;
        private byte[] _islandBuriedBuffer;
        private ushort _selectedAcreId;
        private int _selectedBuilding = -1;
        private ushort _lastSelectedItem;
        private readonly EventHandler _campsiteEventHandler;
        private Item _currentItem;
        private readonly PictureBoxWithInterpolationMode _selectedAcrePicturebox;
        private int _lastMonth;
        private ushort _acreHeightModifier;
        private Dictionary<ushort, byte> _acMapIconIndex;
        private Island[] _islands;
        private Island _selectedIsland;
        private House _islandCabana; // DnM+/AC
        private byte[] _buildingDb;
        private string[] _buildingNames;
        private PictureBoxWithInterpolationMode _newLeafGrassOverlay;
        private PlaceholderTextBox _replaceItemBox;
        private PlaceholderTextBox _replacingItemBox;
        private PlaceholderTextBox _seedBox;
        private ItemEditor _inventoryEditor;
        private SingleItemEditor _shirtEditor;
        private ItemEditor _dresserEditor;
        private ItemEditor _islandBoxEditor;
        private HouseControl _houseEditor;
        private HouseControl _islandHouseEditor;
        private StalkMarketEditor _stalkMarketEditor;
        private bool _loading;

        #region MapSizeVariables

        private static int _townMapCellSize = Properties.Settings.Default.TownMapSize / 16;
        private static int _townMapTotalSize = _townMapCellSize * 16;

        private static int _acreMapSize = Properties.Settings.Default.AcreMapSize;

        #endregion

        #region Static Resources

        public static readonly Bitmap NoTPC = Properties.Resources.no_tpc;
        public static readonly Bitmap ImageX = Properties.Resources.X;

        #endregion

        #endregion

        public MainForm()
        {
            InitializeComponent();

            // Initialize Debug Manager
            DebugUtility.InitializeDebugManager(null, (DebugLevel) Properties.Settings.Default.DebugLevel);

            // Set initial title to include version
            SetProgramTitle();

            // Setup Drag-n-Drop Connection
            AllowDrop = true;
            DragEnter += OnDragEnter;
            DragDrop += OnDragDrop;

            // Clamp Map Sizes
            if (_townMapCellSize < 8)
            {
                _townMapCellSize = 8;
                _townMapTotalSize = 8 * 16;
            }

            if (_acreMapSize < 64)
                _acreMapSize = 64;

            _settingsMenu = new SettingsMenuForm(this);
            _tpcPicture = new PictureBoxWithInterpolationMode
            {
                Name = "TPC_PictureBox",
                Size = new Size(128, 208),
                Location = new Point(6, 6),
                InterpolationMode = InterpolationMode.NearestNeighbor,
                UseInternalInterpolationSetting = true,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = NoTPC,
                ContextMenuStrip = pictureContextMenu,
            };

            playersTab.Controls.Add(_tpcPicture);

            _mainTabs = new TabPage[tabControl1.TabCount];
            for (var i = 0; i < tabControl1.TabCount; i++)
                _mainTabs[i] = tabControl1.TabPages[i];
            for (var i = 0; i < playerEditorSelect.TabCount; i++)
                _playerTabs[i] = playerEditorSelect.TabPages[i];
            for (var i = 0; i < patternGroupTabControl.TabCount; i++)
                _playerPatternTabs[i] = patternGroupTabControl.TabPages[i];

            selectedItem.SelectedValueChanged += ItemSelectedIndexChanged;
            acreTreeView.AfterSelect += AcreTreeViewEntryClicked;
            playerEditorSelect.Selected += PlayerTabIndexChanged;
            patternGroupTabControl.Selected += PlayerTabIndexChanged;

            //Setup selectedAcrePictureBox
            _selectedAcrePicturebox = new PictureBoxWithInterpolationMode
            {
                Size = new Size(64, 64),
                Location = new Point(883, 620),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackgroundImageLayout = ImageLayout.Stretch,
                InterpolationMode = InterpolationMode.HighQualityBicubic,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            Controls.Add(_selectedAcrePicturebox);

            //Town Name TextBox TextChanged
            townNameBox.TextChanged += TownNameBoxTextChanged;

            //Grass Type ComboBox SelectedIndexChanged
            grassTypeBox.SelectedIndexChanged += delegate
            {
                if (!_loading && grassTypeBox.SelectedIndex > -1 && SaveFile != null && CurrentSaveInfo.SaveOffsets.GrassType != -1)
                {
                    SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.GrassType, (byte)grassTypeBox.SelectedIndex);
                }
            };

            //Player Item PictureBox Event Hookups
            BindPlayerItemBoxEvents(heldItemPicturebox);
            BindPlayerItemBoxEvents(hatPicturebox);
            BindPlayerItemBoxEvents(facePicturebox);
            BindPlayerItemBoxEvents(pocketsBackgroundPicturebox);
            BindPlayerItemBoxEvents(bedPicturebox);
            BindPlayerItemBoxEvents(pantsPicturebox);
            BindPlayerItemBoxEvents(socksPicturebox);
            BindPlayerItemBoxEvents(shoesPicturebox);
            BindPlayerItemBoxEvents(playerWetsuit);

            //Player Event Hookups
            playerGender.SelectedIndexChanged += (sender, e) => Gender_Changed();
            playerFace.SelectedIndexChanged += (sender, e) => Face_Changed();
            playerHairType.SelectedIndexChanged += (sender, e) => Hair_Changed();
            playerHairColor.SelectedIndexChanged += (sender, e) => Hair_Color_Changed();
            playerEyeColor.SelectedIndexChanged += (sender, e) => Eye_Color_Changed();
            playerShoeColor.SelectedIndexChanged += (sender, e) => Shoe_Color_Changed();

            //Setup Campsite EventHandler
            _campsiteEventHandler = (sender, e) => Campsite_Villager_Changed();

            //Birthday Event Hookups
            birthdayMonth.SelectedIndexChanged += (sender, e) => Birthday_Month_SelectedIndexChanged();
            birthdayDay.SelectedIndexChanged += (sender, e) => Birthday_Day_SelectedIndexChanged();

            //Custom Acre ID Event Hookup
            acreCustomIdBox.TextChanged += delegate
            {
                if (SaveFile != null && ushort.TryParse(acreCustomIdBox.Text, NumberStyles.AllowHexSpecifier, null, out var customId))
                {
                    SetSelectedAcre(customId, false);
                }
            };

            // Roof ComboBox Changed
            roofColorComboBox.SelectedIndexChanged += delegate
            {
                if (!_loading && roofColorComboBox.Enabled && roofColorComboBox.SelectedIndex > -1 && _selectedHouse != null)
                {
                    _selectedHouse.Data.RoofColor = (byte)roofColorComboBox.SelectedIndex;
                }
            };

            // House Size ComboBox
            houseSizeComboBox.SelectedIndexChanged += delegate
            {
                if (!_loading && houseSizeComboBox.Enabled && houseSizeComboBox.SelectedIndex > -1 && _selectedHouse != null)
                {
                    HouseInfo.SetHouseSize(_selectedHouse.Offset, SaveFile.SaveType, houseSizeComboBox.SelectedIndex);
                }
            };

            // Train Station Type Changed
            stationTypeComboBox.SelectedIndexChanged += delegate
            {
                if (_loading) return;
                if (stationTypeComboBox.SelectedIndex <= -1) return;
                SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TrainStationType,
                    (byte)stationTypeComboBox.SelectedIndex);
                SetTrainStationImage();
            };

            basementCheckBox.CheckedChanged += BasementCheckBoxCheckChanged;
            houseTabSelect.Selected += House_Tab_Index_Changed;

            // Construct Replace Item Menu
            ConstructReplaceItemMenu();

            // Construct Town Generation Seed TextBox
            ConstructGenerationSeedTextBox();

            // Island Tab Index Changed
            islandSelectionTab.Selected += IslandTabIndexChanged;

            // Palette Change Buttons
            paletteNextButton.MouseClick += (sender, e) => ChangePatternPalette(1);
            palettePreviousButton.MouseClick += (sender, e) => ChangePatternPalette(-1);

            // Check for updates
            var updater = new Updater();
            if (updater.HasUpdate() &&
                MessageBox.Show(
                    "An updated version of ACSE is available! Would you like to be taken to the download page?",
                    "ACSE Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(updater.UpdateUrl);
            }
        }

        #region Settings Changing Functions

        public void SetMapPictureBoxSize(ushort size)
        {
            _townMapTotalSize = size;
            _townMapCellSize = size / 16;

            if (SaveFile != null)
                SetupMapPictureBoxes();
        }

        public void SetAcreMapPictureBoxSize(byte size)
        {
            _acreMapSize = size;

            if (SaveFile != null)
                SetupMapPictureBoxes();
        }

        #endregion

        #region Replace MenuStrip Construction

        private void ConstructReplaceItemMenu()
        {
            _replaceItemBox = new PlaceholderTextBox
            {
                PlaceholderText = "Replaced ID",
                Size = new Size(replaceToolStripMenuItem.Size.Width, 18),
                PlaceholderTextColor = Color.Gray
            };

            _replacingItemBox = new PlaceholderTextBox
            {
                PlaceholderText = "Replacing ID",
                Size = new Size(replaceToolStripMenuItem.Size.Width, 18),
                PlaceholderTextColor = Color.Gray
            };

            var replaceToolStripHost = new ToolStripControlHost(_replaceItemBox);
            var replacingToolStripHost = new ToolStripControlHost(_replacingItemBox);

            replaceToolStripHost.AutoSize = false;
            replacingToolStripHost.AutoSize = false;

            replaceToolStripHost.Size = _replaceItemBox.Size;
            replacingToolStripHost.Size = _replacingItemBox.Size;

            replaceItemsToolStripMenuItem.DropDown.Items.Insert(0, replaceToolStripHost);
            replaceItemsToolStripMenuItem.DropDown.Items.Insert(1, replacingToolStripHost);

            _replaceItemBox.TextChanged += (s, e) => ReplaceVerifyHex(_replaceItemBox, 4);
            _replacingItemBox.TextChanged += (s, e) => ReplaceVerifyHex(_replacingItemBox, 4);
        }

        private static void ReplaceVerifyHex(PlaceholderTextBox textBox, short maxLength)
        {
            var text = textBox.Text;
            if (!int.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out _) && text != string.Empty)
            {
                textBox.Text = text.Remove(text.Length - 1, 1);
                textBox.SelectionStart = textBox.Text.Length;
            }
            textBox.MaxLength = textBox.IsPlaceholderActive ? short.MaxValue : maxLength;
        }

        #endregion

        #region Town Generation Seed TextBox Construction

        private void ConstructGenerationSeedTextBox()
        {
            _seedBox = new PlaceholderTextBox
            {
                PlaceholderText = "Seed",
                Size = new Size(generateToolStripMenuItem.Size.Width, 18),
                PlaceholderTextColor = Color.Gray
            };

            var seedBoxToolStripHost = new ToolStripControlHost(_seedBox)
            {
                AutoSize = false,
                Size = _seedBox.Size
            };

            generateRandomTownToolStripMenuItem.DropDownItems.Insert(0, seedBoxToolStripHost);

            _seedBox.TextChanged += (s, e) => VerifyNumbers(_seedBox, 10);
        }

        private static void VerifyNumbers(PlaceholderTextBox textBox, short maxLength)
        {
            var text = textBox.Text;
            if (!string.IsNullOrWhiteSpace(text) && !text.All(o => char.IsDigit(o) || o.Equals('-')))
            {
                textBox.Text = text.Remove(text.Length - 1, 1);
                textBox.SelectionStart = textBox.Text.Length;
            }
            textBox.MaxLength = textBox.IsPlaceholderActive ? short.MaxValue : maxLength;
        }

        #endregion

        private void SetStatusText(string text)
        {
            StatusLabel.Text = text;
        }

        private void UpdateBirthdayDays(int month, Player newPlayer = null)
        {
            var currentSelectedIndex =
                newPlayer != null ? (int)newPlayer.Data.Birthday.Day : birthdayDay.SelectedIndex;

            if (_lastMonth != month)
            {

                birthdayDay.Items.Clear();
                birthdayDay.Items.Add("Not Set");
                if (birthdayMonth.SelectedIndex > 1 && birthdayMonth.SelectedIndex < 13)
                {
                    for (var i = 1; i <= DateTime.DaysInMonth(2000, birthdayMonth.SelectedIndex); i++)
                    {
                        birthdayDay.Items.Add(i.ToString());
                    }
                }
            }

            birthdayDay.SelectedIndex = currentSelectedIndex >= birthdayDay.Items.Count ? 0 : currentSelectedIndex;
            _lastMonth = birthdayMonth.SelectedIndex;
        }

        private void Birthday_Month_SelectedIndexChanged(Player newPlayer = null)
        {
            UpdateBirthdayDays(birthdayMonth.SelectedIndex, newPlayer);

            if (_loading || _selectedPlayer?.Data.Birthday == null) return;
            if (birthdayMonth.SelectedIndex < 1 || birthdayMonth.SelectedIndex > 12)
            {
                _selectedPlayer.Data.Birthday.Month = 0xFF; // Not sure about this for Non-GCN games
            }
            else
            {
                _selectedPlayer.Data.Birthday.Month = SaveFile.SaveGeneration == SaveGeneration.Wii
                    ? (uint) (birthdayMonth.SelectedIndex - 1) // For some reason in City Folk the month doesn't go by 1-12. It's 0-11.
                    : (uint) birthdayMonth.SelectedIndex;
            }
        }

        private void Birthday_Day_SelectedIndexChanged()
        {
            if (_loading || _selectedPlayer?.Data.Birthday == null) return;
            if (birthdayDay.Items.Count < 2 || birthdayDay.SelectedIndex < 1 || birthdayDay.SelectedIndex > 31)
            {
                _selectedPlayer.Data.Birthday.Day = 0xFF;
            }
            else
            {
                _selectedPlayer.Data.Birthday.Day = (uint)birthdayDay.SelectedIndex;
            }
        }

        private void Campsite_Villager_Changed()
        {
            if (SaveFile == null || !campsiteComboBox.Enabled) return;
            try
            {
                if (campsiteComboBox.SelectedValue == null) return;
                var camperId = (ushort)campsiteComboBox.SelectedValue;
                SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.CampsiteVisitor, camperId,
                    SaveFile.IsBigEndian);
            }
            catch
            {
                // Implement Debug Line
            }
        }

        private void BindPlayerItemBoxEvents(Control bindee)
        {
            bindee.MouseClick += PlayersMouseClick;
            bindee.MouseMove += PlayersMouseMove;
            bindee.MouseLeave += HideTip;
        }

        public void SetCurrentItem(Item newItem)
        {
            if (newItem == null) return;
            if (newItem is Furniture furniture &&
                (SaveFile.SaveGeneration == SaveGeneration.N64 || SaveFile.SaveGeneration == SaveGeneration.GCN
                                                               || SaveFile.SaveGeneration == SaveGeneration.iQue))
            {
                _currentItem = new Item(furniture);
            }
            else
            {
                _currentItem = newItem;
            }

            selectedItem.SelectedValue = newItem.ItemId;
            itemFlag1.Text = newItem.Flag1.ToString("X2");
            itemFlag2.Text = newItem.Flag2.ToString("X2");
            if (!itemIdTextBox.Focused)
                itemIdTextBox.Text = newItem.ItemId.ToString("X4");
        }

        public Item GetCurrentItem()
        {
            return _currentItem == null ? new Item() : new Item(_currentItem);
        }

        private void SelectedItem_Changed(object sender, EventArgs e)
        {
            if (selectedItem.SelectedValue == null)
            {
                SetCurrentItem(_currentItem);
            }
        }

        private void EnableEditorControls(Save save)
        {
            townMapViewCheckbox.Enabled = save.SaveGeneration == SaveGeneration.N64 ||
                                          save.SaveGeneration == SaveGeneration.GCN ||
                                          save.SaveGeneration == SaveGeneration.iQue;

            acreHeightTrackBar.Enabled = save.SaveGeneration == SaveGeneration.N64 ||
                                         save.SaveGeneration == SaveGeneration.GCN ||
                                         save.SaveGeneration == SaveGeneration.iQue;
            playerName.Enabled = true;
            playerWallet.Enabled = true;
            playerDebt.Enabled = true;
            playerSavings.Enabled = true;
            playerGender.Enabled = true;
            playerFace.Enabled = true;
            tanTrackbar.Enabled = true;
            birthdayDay.Enabled = true;
            birthdayMonth.Enabled = true;
            resettiCheckBox.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveTownMapToolStripMenuItem.Enabled = true;
            secureValueToolStripMenuItem.Enabled = save.SaveGeneration == SaveGeneration.N3DS;
            unlockHHDItemsToolStripMenuItem.Enabled = save.SaveType == SaveType.WelcomeAmiibo;
            clearWeedsToolStripMenuItem.Enabled = true;
            removeAllItemsToolStripMenuItem.Enabled = true;
            replaceItemsToolStripMenuItem.Enabled = true;
            generateRandomTownToolStripMenuItem.Enabled = true;
            fillMuseumToolStripMenuItem.Enabled =
                save.SaveGeneration != SaveGeneration.N64 && save.SaveGeneration != SaveGeneration.iQue;
            clearMuseumToolStripMenuItem.Enabled =
                save.SaveGeneration != SaveGeneration.N64 && save.SaveGeneration != SaveGeneration.iQue;
            unlockAllPublicWorkProjectsToolStripMenuItem.Enabled = save.SaveGeneration == SaveGeneration.N3DS;
            acreCustomIdBox.Enabled = true;
            selectedItem.Enabled = true;
            itemIdTextBox.Enabled = true;
            townNameBox.Enabled = true;
            buriedCheckbox.Enabled = true;
            grassTypeBox.Enabled =
                save.SaveGeneration != SaveGeneration.N64 && save.SaveGeneration != SaveGeneration.iQue;
            weatherComboBox.Enabled = true;
            stationTypeComboBox.Enabled = TrainStation.HasModifiableTrainStation(save.SaveGeneration);
            houseOwnerComboBox.Enabled = save.SaveGeneration != SaveGeneration.NDS;
            houseTabSelect.Visible = save.SaveGeneration != SaveGeneration.NDS;
            houseTabSelect.Enabled = save.SaveGeneration != SaveGeneration.NDS;
            basementCheckBox.Enabled = save.SaveGeneration == SaveGeneration.GCN;
            StatueCheckBox.Enabled = save.SaveGeneration == SaveGeneration.GCN;
            grassLevelBox.Enabled = true;
            setAllGrass.Enabled = true;
            reviveGrass.Enabled = true;
            removeGrass.Enabled = true;
            censusMenuEnabled.Enabled = save.SaveType == SaveType.WelcomeAmiibo;
            fillCatalogButton.Enabled = true;
            clearCatalogButton.Enabled = true;
            fillEncyclopediaButton.Enabled = true;
            clearEncylopediaButton.Enabled = true;
            fillSongLibraryButton.Enabled = true;
            clearSongLibraryButton.Enabled = true;
            fillEmotionsButton.Enabled = SaveFile.SaveGeneration != SaveGeneration.N64 &&
                                         SaveFile.SaveGeneration != SaveGeneration.GCN &&
                                         save.SaveGeneration != SaveGeneration.iQue;
            clearEmotionsButton.Enabled = fillEmotionsButton.Enabled;
            townGateComboBox.Enabled = SaveFile.SaveGeneration == SaveGeneration.NDS ||
                                       SaveFile.SaveGeneration == SaveGeneration.Wii;
        }

        private void LoadAcres(Save save)
        {
            _acres = new WorldAcre[CurrentSaveInfo.AcreCount];
            TownAcres = new WorldAcre[CurrentSaveInfo.TownAcreCount];

            switch (save.SaveGeneration)
            {
                case SaveGeneration.GCN:
                case SaveGeneration.N64:
                case SaveGeneration.iQue:
                    {
                        _uInt16AcreInfo = SaveDataManager.GetAcreInfoUInt16(save.SaveType);
                        var x = 0;
                        var acreData = save.ReadUInt16Array(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData,
                            CurrentSaveInfo.AcreCount, true);

                        if (save.SaveGeneration == SaveGeneration.GCN)
                        {
                            _islandBuriedBuffer =
                                save.ReadByteArray(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.IslandBuriedData,
                                    CurrentSaveInfo.SaveOffsets.IslandBuriedSize);
                        }

                        for (var i = 0; i < acreData.Length; i++)
                        {
                            if (i >= CurrentSaveInfo.XAcreCount + 1 && (i % CurrentSaveInfo.XAcreCount > 0
                                                                    &&  i % CurrentSaveInfo.XAcreCount <
                                                                        CurrentSaveInfo.XAcreCount - 1) && i <= 47)
                            {
                                var itemsBuff = save.ReadUInt16Array(save.SaveDataStartOffset +
                                                                     CurrentSaveInfo.SaveOffsets.TownData + x * 512, 256,
                                    true);
                                TownAcres[x] = new WorldAcre(acreData[i], i, itemsBuff, null, x);
                                x++;
                            }

                            _acres[i] = new WorldAcre(acreData[i], i);
                        }

                        break;
                    }

                case SaveGeneration.NDS:
                    {
                        _acreInfo = SaveDataManager.GetAcreInfo(SaveType.WildWorld);
                        var x = 0;
                        var acreData = save.ReadByteArray(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData,
                            36);

                        for (var i = 0; i < 36; i++)
                        {
                            if (i >= 7 && (i % 6 > 0 && i % 6 < 5) && i <= 28)
                            {
                                var itemsBuff = save.ReadUInt16Array(save.SaveDataStartOffset +
                                                                     CurrentSaveInfo.SaveOffsets.TownData + x * 512, 256);
                                TownAcres[x] = new WorldAcre(acreData[i], x, itemsBuff, null, x);
                                x++;
                            }

                            _acres[i] = new WorldAcre(acreData[i], i);
                        }

                        break;
                    }

                case SaveGeneration.Wii:
                    {
                        _uInt16AcreInfo = SaveDataManager.GetAcreInfoUInt16(SaveType.CityFolk);
                        _buildings = Building.GetBuildings(save);
                        var x = 0;
                        var acreData = save.ReadUInt16Array(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData,
                            CurrentSaveInfo.AcreCount, true);

                        for (var i = 0; i < acreData.Length; i++)
                        {
                            if (i >= CurrentSaveInfo.XAcreCount + 1
                                && (i % CurrentSaveInfo.XAcreCount > 0
                                && i % CurrentSaveInfo.XAcreCount < CurrentSaveInfo.XAcreCount - 1)
                                && i <= 41)
                            {
                                var itemsBuff = save.ReadUInt16Array(
                                    save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TownData + x * 512, 256,
                                    true);
                                TownAcres[x] = new WorldAcre(acreData[i], x, itemsBuff, null, x);
                                x++;
                            }

                            _acres[i] = new WorldAcre(acreData[i], i);
                        }

                        break;
                    }

                case SaveGeneration.N3DS:
                    {
                        _uInt16AcreInfo = SaveDataManager.GetAcreInfoUInt16(SaveFile.SaveType);

                        //Load Past Villagers for NL/WA
                        _pastVillagers = new SimpleVillager[16];
                        for (var i = 0; i < 16; i++)
                        {
                            var villagerId =
                                save.ReadUInt16(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.PastVillagers +
                                                i * 2);
                            _pastVillagers[i] = _villagerDatabase.Values.FirstOrDefault(o => o.VillagerId == villagerId);
                        }

                        //UInt16_Acre_Info = SaveDataManager.GetAcreInfoUInt16(SaveType.New_Leaf);
                        _buildings = Building.GetBuildings(save);
                        _islandBuildings = Building.GetBuildings(save, true);
                        var x = 0;
                        var acreData = save.ReadUInt16Array(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData,
                            CurrentSaveInfo.AcreCount);
                        for (var i = 0; i < acreData.Length; i++)
                        {
                            if (i >= CurrentSaveInfo.XAcreCount + 1
                                && (i % CurrentSaveInfo.XAcreCount > 0
                                && i % CurrentSaveInfo.XAcreCount < CurrentSaveInfo.XAcreCount - 1)
                                && i <= 33)
                            {
                                var itemsBuff = save.ReadUInt32Array(save.SaveDataStartOffset +
                                                                     CurrentSaveInfo.SaveOffsets.TownData + x * 1024, 256);
                                TownAcres[x] = new WorldAcre(acreData[i], x, itemsBuff);
                                x++;
                            }

                            _acres[i] = new WorldAcre(acreData[i], i);
                        }

                        break;
                    }
            }
        }

        private void LoadVillagers(Save save)
        {
            _villagers = new Villager[CurrentSaveInfo.VillagerCount];
            for (var i = 0; i < _villagers.Length; i++)
            {
                if (SaveFile.SaveType == SaveType.AnimalCrossing || SaveFile.SaveType == SaveType.DoubutsuNoMoriPlus)
                {
                    if (i < 15)
                    {
                        _villagers[i] =
                            new Villager(
                                save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.VillagerData +
                                CurrentSaveInfo.SaveOffsets.VillagerSize * i, i, save);
                    }
                    else
                    {
                        _villagers[i] = new Villager(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.IslanderData, i, save);
                    }
                }
                else
                {
                    _villagers[i] =
                        new Villager(
                            save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.VillagerData +
                            CurrentSaveInfo.SaveOffsets.VillagerSize * i, i, save);
                }
            }

            try
            {
                foreach (var villager in _villagers)
                {
                    if (!villager.Exists || villager.AnimalMemories == null) continue;
                    foreach (var memory in villager.AnimalMemories)
                    {
                        if (memory.Exists)
                        {
                            memory.Player = _players.FirstOrNull(p =>
                                p.Data.Identifier.Equals(memory.PlayerId) &&
                                p.Data.Name.Equals(memory.PlayerName));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private void CreateVillagerPanelControls()
        {
            // Set labels for the villager panel
            villagerPanel.Controls.DisposeChildren();
            var labelFlowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                MaximumSize = new Size(0, 20),
                FlowDirection = FlowDirection.LeftToRight
            };

            // Add common labels
            var indexLabel = new Label
            {
                Text = "Index",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(45, 20)
            };
            indexLabel.SetCenterMargins(labelFlowPanel);

            var villagerLabel = new Label
            {
                Text = "Villager",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(120, 20)
            };
            villagerLabel.SetCenterMargins(labelFlowPanel, 0, 10);

            var personalityLabel = new Label
            {
                Text = "Personality",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(80, 20)
            };
            personalityLabel.SetCenterMargins(labelFlowPanel, 0, 10);

            var catchphraseLabel = new Label
            {
                Text = "Catchphrase",
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(100, 20)
            };
            catchphraseLabel.SetCenterMargins(labelFlowPanel, 0, 10);

            var itemsLabel = new Label
            {
                Text = "Items",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(80, 20)
            };
            itemsLabel.SetCenterMargins(labelFlowPanel, 0, 10);

            labelFlowPanel.Controls.Add(indexLabel);
            labelFlowPanel.Controls.Add(villagerLabel);
            labelFlowPanel.Controls.Add(personalityLabel);
            labelFlowPanel.Controls.Add(catchphraseLabel);
            labelFlowPanel.Controls.Add(itemsLabel);
            villagerPanel.Controls.Add(labelFlowPanel);

            foreach (var v in _villagers)
            {
                GenerateVillagerPanel(v);
            }

            if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                GeneratePastVillagersPanel();
            }
        }

        private void CreateItemEditorControls(Save save)
        {
            // Create Item Editor Controls
            if (_dresserEditor != null && !_dresserEditor.IsDisposed)
                _dresserEditor.Dispose();

            if (_islandBoxEditor != null && !_islandBoxEditor.IsDisposed)
                _islandBoxEditor.Dispose();

            if (_inventoryEditor != null && !_inventoryEditor.IsDisposed)
                _inventoryEditor.Dispose();

            if (_shirtEditor == null)
            {
                _shirtEditor = new SingleItemEditor(this, _selectedPlayer?.Data.Shirt, 16)
                {
                    Location = new Point(98, 249)
                };

                _shirtEditor.ItemChanged += delegate (object sender, ItemChangedEventArgs e)
                {
                    if (_selectedPlayer != null && !_loading)
                    {
                        _selectedPlayer.Data.Shirt = e.NewItem;
                    }
                };

                playersTab.Controls.Add(_shirtEditor);
            }

            _inventoryEditor = new ItemEditor(this, _selectedPlayer?.Data.Pockets.Items,
                save.SaveGeneration == SaveGeneration.N3DS ? 4 : 5, 16)
            {
                Location = new Point(26, 340),
                HoverText = "{0} ({2}) - [0x{1}]"
            };

            playersTab.Controls.Add(_inventoryEditor);

            if (SaveFile.SaveGeneration != SaveGeneration.N64 && SaveFile.SaveGeneration != SaveGeneration.GCN &&
                save.SaveGeneration != SaveGeneration.iQue)
            {
                var itemsPerRow = 9;
                switch (SaveFile.SaveGeneration)
                {
                    case SaveGeneration.Wii:
                        itemsPerRow = 16;
                        break;
                    case SaveGeneration.N3DS:
                        itemsPerRow = 18;
                        break;
                }

                _dresserEditor = new ItemEditor(this, _selectedPlayer?.Data.Dressers, itemsPerRow, 16)
                {
                    Location = new Point(202, 340)
                };

                playersTab.Controls.Add(_dresserEditor);
            }

            if (SaveFile.SaveGeneration != SaveGeneration.N3DS) return;

            _islandBoxEditor = new ItemEditor(this, _selectedPlayer?.Data.IslandBox, 5, 16)
            {
                Location = new Point(114, 340)
            };

            playersTab.Controls.Add(_islandBoxEditor);
        }

        private async Task SetupEditor(Save save)
        {
            // Set the debug manager's save reference
            DebugUtility.DebugManagerInstance.SaveFile = save;

            // Create a backup if backups are enabled
            if (Properties.Settings.Default.BackupFiles)
            {
                new Backup(save);
            }

            progressBar1.Value = 0;
            loadingPanel.BringToFront();
            loadingPanel.Visible = true;
            loadingPanel.Enabled = true;
            _loading = true;

            if (save.SuccessfullyLoaded && save.SaveType == SaveType.Unknown)
            {
                MessageBox.Show(
                    $"The file [{save.SaveName + save.SaveExtension}] could not be identified as a valid Animal Crossing save file.\n" +
                    "Please ensure you have a valid save file.",
                    "Save File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadingPanel.SendToBack();
                loadingPanel.Visible = false;
                loadingPanel.Enabled = false;
                _loading = false;
                progressBar1.Value = 0;
                return;
            }

            if (!save.SuccessfullyLoaded)
            {
                loadingPanel.SendToBack();
                loadingPanel.Visible = false;
                loadingPanel.Enabled = false;
                _loading = false;
                progressBar1.Value = 0;
                return;
            }

            // Clear items
            selectedItem.DataSource = null;
            selectedItem.Items.Clear();

            // Initialize item colors
            ItemColorEditor.UpdateItemColors(); // TODO: Move this logic to ACSE.Core

            SaveFile = null; //Set to null so we can set the checkbox to false without having the method run
            UndoRedoHandler = new ModifiedHandler();
            townMapViewCheckbox.Checked = false;
            SaveFile = save;
            _currentItem = new Item();
            DebugManager.WriteLine("Save File Loaded");
            _acreHeightModifier = 0;
            _selectedAcreId = 0;
            _selectedAcrePicturebox.Image = null;
            _lastTownAcre = -1;
            _selectedAcrePicturebox.BackgroundImage = null;
            _buildings = null;
            _islandBuildings = null;
            _selectedPatternObject = null;
            _islandBuriedBuffer = null;
            _selectedHouse = null;
            _tpcPicture.Image = NoTPC;
            _secureNandValueForm.Hide();
            itemFlag1.Text = "00";
            itemFlag2.Text = "00";

            await Task.Run(() =>
            {
                _itemList = SaveDataManager.GetItemInfo(save.SaveType).ToList();
                _itemList.Sort((x, y) => x.Key.CompareTo(y.Key));
                ItemData.ItemDatabase = _itemList;
            });

            selectedItem.DataSource = new BindingSource(_itemList, null);
            selectedItem.DisplayMember = "Value";
            selectedItem.ValueMember = "Key";

            progressBar1.Value = 5;

            CurrentSaveInfo = SaveDataManager.GetSaveInfo(SaveFile.SaveType);

            // Load the correct acre database
            if (save.SaveType == SaveType.WildWorld)
            {
                await Task.Run(() =>
                {
                    _filedAcreData = SaveDataManager.GetFiledAcreData(SaveFile.SaveType);
                    _uInt16FiledAcreData = null;
                    _uInt16AcreInfo = null;
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    _uInt16FiledAcreData = SaveDataManager.GetFiledAcreDataUInt16(SaveFile.SaveType);
                    _filedAcreData = null;
                    _acreInfo = null;
                });
            }

            progressBar1.Value = 7;

            // Enable Controls
            EnableEditorControls(save);

            townGateComboBox.Items.Clear();

            if (townGateComboBox.Enabled)
            {
                townGateComboBox.Items.AddRange(TownGate.GetTownGateTypeNames(SaveFile.SaveGeneration));
                
                var selectedIndex = TownGate.GetTownGateType(SaveFile);
                townGateComboBox.SelectedIndex = selectedIndex > -1 && selectedIndex < townGateComboBox.Items.Count
                    ? selectedIndex
                    : 0;
                // TODO: Set gate image
            }

            SetTrainStationImage();

            //Setup Welcome Amiibo Caravan ComboBoxes
            if (save.SaveGeneration == SaveGeneration.N3DS)
            {
                if (caravan1ComboBox.DataSource == null)
                {
                    caravan1ComboBox.DataSource = new BindingSource(VillagerData.GetCaravanBindingSource(), null);
                    caravan2ComboBox.DataSource = new BindingSource(VillagerData.GetCaravanBindingSource(), null);
                    caravan1ComboBox.ValueMember = "Key";
                    caravan1ComboBox.DisplayMember = "Value";
                    caravan2ComboBox.ValueMember = "Key";
                    caravan2ComboBox.DisplayMember = "Value";
                }
            }
            else
            {
                caravan1ComboBox.DataSource = null;
                caravan2ComboBox.DataSource = null;
                caravan1ComboBox.Items.Clear();
                caravan2ComboBox.Items.Clear();
            }

            // Clear Acre TreeView
            acreTreeView.Nodes.Clear();

            // Load Houses
            _houses = HouseInfo.LoadHouses(save);
            _selectedHouse = _houses[0];

            // Clear House ComboBoxes Box
            roofColorComboBox.Items.Clear();
            houseSizeComboBox.Items.Clear();

            // Add Roof Color Items
            roofColorComboBox.Items.AddRange(HouseInfo.GetRoofColors(save.SaveType));
            roofColorComboBox.Enabled = roofColorComboBox.Items.Count > 0;

            // Add House Size Items
            houseSizeComboBox.Items.AddRange(HouseInfo.GetHouseSizes(SaveFile.SaveGeneration));
            houseSizeComboBox.Enabled = houseSizeComboBox.Items.Count > 0;

            //Load Villager Database
            if (SaveFile.SaveType != SaveType.CityFolk) //City Folk has completely editable villagers! That's honestly a pain for editing...
            {
                _villagerDatabase = VillagerInfo.GetVillagerDatabase(SaveFile.SaveType);
                _personalityDatabase = VillagerInfo.GetPersonalities(SaveFile.SaveType);
                _villagerNames = new string[_villagerDatabase.Count];
                for (var i = 0; i < _villagerNames.Length; i++)
                    _villagerNames[i] = _villagerDatabase.ElementAt(i).Value.Name;

                //Clear Villager Editor
                for (var i = villagerPanel.Controls.Count - 1; i > -1; i--)
                    if (villagerPanel.Controls[i] is Panel)
                        villagerPanel.Controls[i].Dispose();

                //Add Campsite Villager Database
                campsiteComboBox.DataSource = null;
                campsiteComboBox.SelectedValueChanged -= _campsiteEventHandler;
                campsiteComboBox.Items.Clear();
                campsiteComboBox.Enabled = true;
                if (SaveFile.SaveType == SaveType.AnimalCrossing || SaveFile.SaveType == SaveType.NewLeaf ||
                    SaveFile.SaveType == SaveType.WelcomeAmiibo)
                {
                    campsiteComboBox.DataSource = new BindingSource(_villagerDatabase, null);
                    campsiteComboBox.DisplayMember = "Value";
                    campsiteComboBox.ValueMember = "Key";
                    //Load Campsite (or Igloo) Visitor
                    try
                    {
                        var camperId =
                            SaveFile.ReadUInt16(
                                SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.CampsiteVisitor,
                                SaveFile.IsBigEndian);
                        campsiteComboBox.SelectedValue = camperId;

                        //Setup Campsite Event
                        campsiteComboBox.SelectedValueChanged += _campsiteEventHandler;
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else
                {
                    campsiteComboBox.Enabled = false;
                }
                //Set Caravan Availablity
                caravan1ComboBox.Enabled = SaveFile.SaveType == SaveType.WelcomeAmiibo;
                caravan2ComboBox.Enabled = caravan1ComboBox.Enabled;
            }

            //Clear Buildings
            for (var i = buildingsPanel.Controls.Count - 1; i > -1; i--)
                buildingsPanel.Controls[i].Dispose();

            //Set building panel visibility
            var visibility = save.SaveGeneration == SaveGeneration.Wii || save.SaveGeneration == SaveGeneration.N3DS;
            buildingsPanel.Visible = visibility;
            buildingsLabel.Visible = visibility;
            townPanel.Size = new Size(visibility ? townTab.Size.Width - 213 : townTab.Size.Width - 9,
                townPanel.Size.Height);

            //Cleanup old dictionary
            AcreImageManager.DisposeLoadedImages();

            //Clear Past Villager Panel
            while (pastVillagersPanel.Controls.Count > 0)
                pastVillagersPanel.Controls[0].Dispose();

            // Clear Hair Preview Box
            if (hairPictureBox.Image != null)
            {
                var image = hairPictureBox.Image;
                hairPictureBox.Image = null;
                image.Dispose();
            }

            SetEnabledControls(SaveFile.SaveType);
            SetupPatternBoxes();
            playerFace.Items.Clear();
            playerHairColor.Items.Clear();
            playerHairType.Items.Clear();
            playerShoeColor.Items.Clear();
            playerFace.Text = "";
            playerHairColor.Text = "";
            playerHairType.Text = "";
            playerShoeColor.Text = "";
            switch (save.SaveGeneration)
            {
                case SaveGeneration.NDS:
                    {
                        foreach (var faceName in PlayerInfo.WwFaces)
                            playerFace.Items.Add(faceName);
                        foreach (var hairColor in PlayerInfo.WwHairColors)
                            playerHairColor.Items.Add(hairColor);
                        foreach (var hairStyle in PlayerInfo.WwHairStyles)
                            playerHairType.Items.Add(hairStyle);
                        playerDebt.Text = save.ReadUInt32(CurrentSaveInfo.SaveOffsets.Debt).ToString();
                        break;
                    }

                case SaveGeneration.Wii:
                    {
                        foreach (var faceName in PlayerInfo.WwFaces)   //Same order as WW
                            playerFace.Items.Add(faceName);
                        foreach (var hairColor in PlayerInfo.WwHairColors)
                            playerHairColor.Items.Add(hairColor);
                        foreach (var shoeColor in PlayerInfo.CfShoeColors)
                            playerShoeColor.Items.Add(shoeColor);
                        foreach (var hairStyle in PlayerInfo.CfHairStyles)
                            playerHairType.Items.Add(hairStyle);
                        _grassWear =
                            save.ReadByteArray(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.GrassWear,
                                CurrentSaveInfo.SaveOffsets.GrassWearSize);
                        break;
                    }

                case SaveGeneration.N3DS:
                {
                    _grassWear = save.ReadByteArray(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.GrassWear,
                        CurrentSaveInfo.SaveOffsets.GrassWearSize);
                        foreach (var hairStyle in PlayerInfo.NlHairStyles)
                            playerHairType.Items.Add(hairStyle);
                        foreach (var hairColor in PlayerInfo.NlHairColors)
                            playerHairColor.Items.Add(hairColor);
                        foreach (var eyeColor in PlayerInfo.NlEyeColors)
                            playerEyeColor.Items.Add(eyeColor);
                        _secureNandValueForm.Set_Secure_NAND_Value(SaveFile.ReadUInt64(0));
                        break;
                    }

                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    {
                        foreach (var faceName in PlayerInfo.AcFaces)
                            playerFace.Items.Add(faceName);
                        break;
                    }
            }

            progressBar1.Value = 20;
            _selectedPlayer = null;

            // Load players
            await Task.Run(() =>
            {
                for (var i = 0; i < 4; i++)
                {
                    _players[i] = new Player(SaveFile.SaveDataStartOffset
                                             + CurrentSaveInfo.SaveOffsets.PlayerStart +
                                             i * CurrentSaveInfo.SaveOffsets.PlayerSize, i, SaveFile);
                }

                _selectedPlayer = _players.FirstOrNull(o => o.Exists);
            });

            progressBar1.Value = 40;

            // Display the selected player's information
            if (_selectedPlayer != null)
            {
                ReloadPlayer(_selectedPlayer);
            }
            else
            {
                MessageBox.Show("No Player was found on the file!", "Player Find Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            SetPlayersEnabled();

            if (_selectedPlayer != null && _selectedPlayer.Exists)
            {
                playerEditorSelect.SelectedIndex = Array.IndexOf(_players, _selectedPlayer);
            }

            // Grass Type Stuff
            grassTypeBox.Items.Clear();
            grassTypeBox.Enabled = true;

            // Load islands if DnMe+
            _selectedIsland = null;
            _islands = null;
            
            if (save.SaveType == SaveType.DoubutsuNoMoriEPlus || save.SaveType == SaveType.AnimalForestEPlus)
            {
                islandSelectionTab.Visible = true;
                _islands = new Island[4];

                for (var i = 0; i < 4; i++)
                {
                    var islandOffset = save.SaveDataStartOffset + 0xC560 + i * 0x3860;
                    _islands[i] = new Island(islandOffset, _players, save);
                }

                islandSelectionTab.SelectedIndex = 0;
                _selectedIsland = _islands[0];
            }
            else
            {
                islandSelectionTab.Visible = false;
            }

            // Load acre & town item data
            await Task.Run(() => { LoadAcres(save); });

            switch (SaveFile.SaveGeneration)
            {
                case SaveGeneration.N3DS:
                    foreach (var Enum in Enum.GetValues(typeof(NewLeafGrassType)))
                        grassTypeBox.Items.Add(Enum);

                    if (_selectedPlayer != null)
                    {
                        playerFace.Items.Clear();
                        foreach (var faceName in _selectedPlayer.Data.Gender == 0 ? PlayerInfo.NlMaleFaces : PlayerInfo.NlFemaleFaces)
                        {
                            playerFace.Items.Add(faceName);
                        }

                        switch (SaveFile.SaveGeneration)
                        {
                            case SaveGeneration.N64:
                            case SaveGeneration.GCN:
                            case SaveGeneration.iQue:
                                playerFace.SelectedIndex =
                                    (_selectedPlayer.Data.Gender == 1
                                        ? (byte)(_selectedPlayer.Data.FaceType + 8)
                                        : _selectedPlayer.Data.FaceType);
                                break;
                            default:
                                playerFace.SelectedIndex = _selectedPlayer.Data.FaceType;
                                break;
                        }
                    }

                    break;
                case SaveGeneration.NDS:
                    foreach (var Enum in Enum.GetValues(typeof(WildWorldGrassType)))
                        grassTypeBox.Items.Add(Enum);
                    break;
                case SaveGeneration.Wii:
                    foreach (var Enum in Enum.GetValues(typeof(CityFolkGrassType)))
                        grassTypeBox.Items.Add(Enum);
                    break;
                default:
                    foreach (var Enum in Enum.GetValues(typeof(AnimalCrossingGrassType)))
                        grassTypeBox.Items.Add(Enum);
                    break;
            }

            progressBar1.Value = 75;

            townNameBox.MaxLength = CurrentSaveInfo.SaveOffsets.TownNameSize;
            if (save.SaveGeneration == SaveGeneration.N3DS)
            {
                townNameBox.Text = new AcString(save.ReadByteArray(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TownName,
                    CurrentSaveInfo.SaveOffsets.TownNameSize * 2), save.SaveType).Trim();
            }
            else
            {
                townNameBox.Text = new AcString(save.ReadByteArray(save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TownName,
                    CurrentSaveInfo.SaveOffsets.TownNameSize), save.SaveType).Trim();
            }
            
            // Load island cabana if DnM+/AC
            _islandCabana = null;
            if (SaveFile.SaveType == SaveType.DoubutsuNoMoriPlus || SaveFile.SaveType == SaveType.AnimalCrossing)
            {
                _islandCabana = new House(-1, save.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.IslandHouse, 1, 0);
                _islandCabana.Data.Rooms[0].Name = "Cabana";
            }

            // Load acre editor controls
            SetupAcreEditorTreeView();
            SetupMapPictureBoxes();
            SetupIslandHouseBoxes();

            if (_buildings != null)
            {
                SetupBuildingList();
            }

            // Load villagers
            if (SaveFile.SaveType != SaveType.CityFolk)
            {
                await Task.Run(() => { LoadVillagers(save); });
                CreateVillagerPanelControls();
            }

            progressBar1.Value = 90;

            // Set House Owner ComboBox List
            houseOwnerComboBox.Items.Clear();
            if (save.SaveGeneration != SaveGeneration.NDS)
            {
                houseOwnerComboBox.Items.Add("No One");
                for (var i = 0; i < 4; i++)
                {
                    if (_players[i] != null && _players[i].Exists)
                    {
                        houseOwnerComboBox.Items.Add(_players[i].Data.Name);
                    }
                }
            }

            // Draw House PictureBoxes
            houseTabSelect.SelectedIndex = 0; // Reset the house selected
            SetupHousePictureBoxes();
            if (roofColorComboBox.Enabled && _selectedHouse != null)
            {
                roofColorComboBox.SelectedIndex =
                    Math.Min(roofColorComboBox.Items.Count - 1, _selectedHouse.Data.RoofColor);
            }

            if (houseSizeComboBox.Enabled && _selectedHouse != null)
            {
                houseSizeComboBox.SelectedIndex = HouseInfo.GetHouseSize(_selectedHouse.Offset, save.SaveType);
            }

            // Set TextBox max values
            playerName.MaxLength = CurrentSaveInfo.SaveOffsets.TownNameSize; // As far as I know, town name and player name are always the same size

            // Enable Tasks
            clearWeedsToolStripMenuItem.Enabled = true;
            removeAllItemsToolStripMenuItem.Enabled = true;
            waterFlowersToolStripMenuItem.Enabled = SaveFile.SaveGeneration != SaveGeneration.GCN &&
                                                    SaveFile.SaveGeneration != SaveGeneration.N64 &&
                                                    save.SaveGeneration != SaveGeneration.iQue;
            makeFruitsPerfectToolStripMenuItem.Enabled = SaveFile.SaveGeneration == SaveGeneration.N3DS;
            replaceItemsToolStripMenuItem.Enabled = true;
            importTownToolStripMenuItem.Enabled = true;
            exportTownToolStripMenuItem.Enabled = true;

            // Set Grass Type
            if (CurrentSaveInfo.SaveOffsets.GrassType != -1)
            {
                var grassType = SaveFile.ReadByte(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.GrassType);
                grassTypeBox.SelectedIndex = (SaveFile.SaveGeneration == SaveGeneration.NDS
                    ? Utility.GetWildWorldGrassBaseType(grassType)
                    : (grassType < 3 ? grassType : 0));
            }

            // Set Native Fruit Types
            SetPossibleNativeFruits(save.SaveGeneration);

            // Set Weather Info
            weatherComboBox.Enabled = save.SaveGeneration == SaveGeneration.GCN;
            weatherComboBox.Items.Clear();
            weatherComboBox.Items.AddRange(Weather.GetWeatherTypesForGame(save.SaveGeneration));
            if (CurrentSaveInfo.SaveOffsets.Weather != -1)
                weatherComboBox.SelectedIndex = Weather.GetWeatherIndex(
                    save.ReadByte(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.Weather),
                    save.SaveGeneration);

            // Set Default Item to "Empty"
            SetCurrentItem(new Item());

            // Create the necessary item editor controls.
            CreateItemEditorControls(save);

            // Badges (for New Leaf)
            badgeGroupBox.Visible = save.SaveGeneration == SaveGeneration.N3DS;
            if (badgeGroupBox.Controls.Count > 0)
            {
                ClearBadges();
            }

            if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                AddBadges();
            }

            // Town Ordinances for New Leaf
            SetOrdinanceCheckBoxes();

            // Create Stalk Market Editor
            _stalkMarketEditor?.Dispose();

            _stalkMarketEditor = new StalkMarketEditor(SaveFile)
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(720, 14)
            };

            if (!_stalkMarketEditor.IsDisposed)
            {
                townMisc.Controls.Add(_stalkMarketEditor);
            }

            progressBar1.Value = 100;
            _loading = false;
            loadingPanel.Enabled = false;
            loadingPanel.Visible = false;
            loadingPanel.SendToBack();
        }

        private void AddBadges()
        {
            if (SaveFile == null || _selectedPlayer == null || !_selectedPlayer.Exists) return;
            const int badgeValueOffset = 0x55DC; // These are the same for each version.
            const int badgeLevelOffset = 0x569C; // These are also the same.

            for (var i = 0; i < 24; i++)
            {
                var badgeControl = new BadgeControl(SaveFile, i, _selectedPlayer.Offset + badgeLevelOffset + i,
                    _selectedPlayer.Offset + badgeValueOffset + i * 8);
                badgeGroupBox.Controls.Add(badgeControl);
                badgeControl.Location = new Point(10 + (i % 6) * 30, 16 + (i / 6) * 30);
            }
        }

        private void ClearBadges()
        {
            // Dispose to clear memory
            foreach (Control c in badgeGroupBox.Controls)
            {
                c.Dispose();
            }
            badgeGroupBox.Controls.Clear();
        }

        private void SetPlayersEnabled()
        {
            for (var i = 0; i < 4; i++)
            {
                if (_players[i] == null) continue;

                if (_players[i].Exists)
                {
                    _players[i].Data.Name = _players[i].Data.Name?.Replace("\0", "");
                    SetPlayerSelectionTabText(_players[i]);
                    if (playerEditorSelect.TabPages.IndexOf(_playerTabs[i]) >= 0) continue;

                    if (i >= playerEditorSelect.TabCount)
                    {
                        playerEditorSelect.TabPages.Add(_playerTabs[i]);
                        patternGroupTabControl.TabPages.Add(_playerPatternTabs[i]);
                    }
                    else
                    {
                        playerEditorSelect.TabPages.Insert(i, _playerTabs[i]);
                        patternGroupTabControl.TabPages.Insert(i, _playerPatternTabs[i]);
                    }
                }
                else
                {
                    if (playerEditorSelect.TabPages.IndexOf(_playerTabs[i]) <= -1) continue;

                    playerEditorSelect.TabPages.Remove(_playerTabs[i]);
                    patternGroupTabControl.TabPages.Remove(_playerPatternTabs[i]);
                }
            }
        }

        private void SetMainTabEnabled(string tabName, bool enabled)
        {
            if (!enabled)
            {
                foreach (TabPage page in tabControl1.TabPages)
                    if (page.Name == tabName)
                        tabControl1.TabPages.Remove(page);
            }
            else
            {
                foreach (TabPage page in tabControl1.TabPages)
                    if (page.Name == tabName)
                        return;
                for (var i = 0; i < _mainTabs.Length; i++)
                    if (_mainTabs[i].Name == tabName)
                    {
                        if (i >= playerEditorSelect.TabCount)
                            tabControl1.TabPages.Add(_mainTabs[i]);
                        else
                            tabControl1.TabPages.Insert(i, _mainTabs[i]);
                    }
            }
        }

        private void SetProgramTitle()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            var versionString = $"{version.Major}.{version.Minor}";
            if (version.Build != 0)
            {
                versionString += $".{version.Build}";
                if (version.Revision != 0)
                {
                    versionString += $".{version.Revision}";
                }
            }

            Text = SaveFile != null
                ? $"ACSE {versionString} - {SaveFile.SaveName} - [{SaveDataManager.GetGameTitle(SaveFile.SaveType)}]"
                : $"ACSE {versionString}";
        }

        private void SetEnabledControls(SaveType currentSaveType)
        {
            SetProgramTitle();
            itemFlag1.Text = "00";
            itemFlag2.Text = "00";

            //Load all tabs so alignment is kept
            SetMainTabEnabled("housesTab", false);
            SetMainTabEnabled("islandTab", false);
            SetMainTabEnabled("grassTab", false);
            SetMainTabEnabled("patternsTab", false);
            SetMainTabEnabled("housesTab", true);
            SetMainTabEnabled("islandTab", true);
            SetMainTabEnabled("grassTab", true);
            SetMainTabEnabled("patternsTab", true);

            playerSavings.Enabled =
                currentSaveType != SaveType.DoubutsuNoMori && currentSaveType != SaveType.DongwuSenlin;
            tanTrackbar.Enabled =
                currentSaveType != SaveType.DoubutsuNoMori && currentSaveType != SaveType.DongwuSenlin;

            switch (currentSaveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                case SaveType.DongwuSenlin:
                    SetMainTabEnabled("islandTab",
                        currentSaveType != SaveType.DoubutsuNoMori && currentSaveType != SaveType.DongwuSenlin);
                    SetMainTabEnabled("patternsTab",
                        currentSaveType != SaveType.DoubutsuNoMori && currentSaveType != SaveType.DongwuSenlin);
                    SetMainTabEnabled("grassTab", false);
                    playerHairType.Enabled = false;
                    playerHairColor.Enabled = false;
                    playerEyeColor.Enabled = false;
                    playerNookPoints.Enabled = false;
                    bedPicturebox.Image = ImageX;
                    bedPicturebox.Enabled = false;
                    hatPicturebox.Image = ImageX;
                    hatPicturebox.Enabled = false;
                    pantsPicturebox.Image = ImageX;
                    pantsPicturebox.Enabled = false;
                    facePicturebox.Image = ImageX;
                    facePicturebox.Enabled = false;
                    socksPicturebox.Image = ImageX;
                    socksPicturebox.Enabled = false;
                    shoesPicturebox.Image = ImageX;
                    shoesPicturebox.Enabled = false;
                    playerWetsuit.Enabled = false;
                    playerWetsuit.Image = ImageX;
                    playerShoeColor.Enabled = false;
                    pocketsBackgroundPicturebox.Enabled = true;
                    playerIslandMedals.Enabled = false;
                    playerMeowCoupons.Enabled = false;
                    itemFlag1.Enabled = false;
                    itemFlag2.Enabled = false;
                    earlyBirdCheckBox.Enabled = false;
                    nightOwlCheckBox.Enabled = false;
                    bellBoomCheckBox.Enabled = false;
                    keepTownBeautifulCheckBox.Enabled = false;
                    dresserText.Visible = false;
                    islandBoxText.Visible = false;
                    tanTrackbar.Maximum = 9;
                    break;
                case SaveType.WildWorld:
                    SetMainTabEnabled("islandTab", false);
                    SetMainTabEnabled("grassTab", false);
                    SetMainTabEnabled("patternsTab", true);
                    playerHairType.Enabled = true;
                    playerHairColor.Enabled = true;
                    playerNookPoints.Enabled = true;
                    bedPicturebox.Enabled = true;
                    playerEyeColor.Enabled = false;
                    hatPicturebox.Enabled = true;
                    pantsPicturebox.Image = ImageX;
                    pantsPicturebox.Enabled = false;
                    facePicturebox.Enabled = true;
                    socksPicturebox.Image = ImageX;
                    socksPicturebox.Enabled = false;
                    shoesPicturebox.Image = ImageX;
                    shoesPicturebox.Enabled = false;
                    playerWetsuit.Enabled = false;
                    playerWetsuit.Image = ImageX;
                    playerShoeColor.Enabled = false;
                    pocketsBackgroundPicturebox.Enabled = true;
                    playerIslandMedals.Enabled = false;
                    playerMeowCoupons.Enabled = false;
                    itemFlag1.Enabled = false;
                    itemFlag2.Enabled = false;
                    earlyBirdCheckBox.Enabled = false;
                    nightOwlCheckBox.Enabled = false;
                    bellBoomCheckBox.Enabled = false;
                    keepTownBeautifulCheckBox.Enabled = false;
                    dresserText.Visible = true;
                    islandBoxText.Visible = false;
                    tanTrackbar.Maximum = 4;
                    break;
                case SaveType.CityFolk:
                    SetMainTabEnabled("islandTab", false);
                    SetMainTabEnabled("grassTab", true);
                    SetMainTabEnabled("patternsTab", true);
                    playerHairType.Enabled = true;
                    playerHairColor.Enabled = true;
                    playerNookPoints.Enabled = true;
                    playerEyeColor.Enabled = false;
                    bedPicturebox.Enabled = true;
                    hatPicturebox.Enabled = true;
                    pantsPicturebox.Image = ImageX;
                    pantsPicturebox.Enabled = false;
                    facePicturebox.Enabled = true;
                    socksPicturebox.Image = ImageX;
                    socksPicturebox.Enabled = false;
                    shoesPicturebox.Image = ImageX;
                    shoesPicturebox.Enabled = false;
                    playerWetsuit.Enabled = false;
                    playerWetsuit.Image = ImageX;
                    playerShoeColor.Enabled = true;
                    pocketsBackgroundPicturebox.Image = ImageX;
                    pocketsBackgroundPicturebox.Enabled = false;
                    playerIslandMedals.Enabled = false;
                    playerMeowCoupons.Enabled = false;
                    itemFlag1.Enabled = false;
                    itemFlag2.Enabled = false;
                    earlyBirdCheckBox.Enabled = false;
                    nightOwlCheckBox.Enabled = false;
                    bellBoomCheckBox.Enabled = false;
                    keepTownBeautifulCheckBox.Enabled = false;
                    dresserText.Visible = true;
                    islandBoxText.Visible = false;
                    tanTrackbar.Maximum = 8;
                    break;
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                    SetMainTabEnabled("islandTab", true);
                    SetMainTabEnabled("grassTab", true);
                    SetMainTabEnabled("patternsTab", true);
                    //SetMainTabEnabled("housesTab", false); // TEMP! Remove this once I get around to implementing 3DS house editing
                    playerHairType.Enabled = true;
                    playerHairColor.Enabled = true;
                    playerEyeColor.Enabled = true;
                    bedPicturebox.Image = ImageX;
                    bedPicturebox.Enabled = false;
                    playerNookPoints.Enabled = false;
                    hatPicturebox.Enabled = true;
                    pantsPicturebox.Enabled = true;
                    facePicturebox.Enabled = true;
                    socksPicturebox.Enabled = true;
                    shoesPicturebox.Enabled = true;
                    playerShoeColor.Enabled = false;
                    pocketsBackgroundPicturebox.Image = ImageX;
                    pocketsBackgroundPicturebox.Enabled = false;
                    playerWetsuit.Enabled = true;
                    playerIslandMedals.Enabled = true;
                    itemFlag1.Enabled = true;
                    itemFlag2.Enabled = true;
                    playerMeowCoupons.Enabled = currentSaveType == SaveType.WelcomeAmiibo;
                    earlyBirdCheckBox.Enabled = true;
                    nightOwlCheckBox.Enabled = true;
                    bellBoomCheckBox.Enabled = true;
                    keepTownBeautifulCheckBox.Enabled = true;
                    dresserText.Visible = true;
                    islandBoxText.Visible = true;
                    tanTrackbar.Maximum = 16;
                    break;
            }
        }

        private static void RefreshPictureBoxImage(PictureBox box, Image newImage, bool backgroundImage = false,
            bool dispose = true)
        {
            if (box == null) return;
            var oldImage = backgroundImage ? box.BackgroundImage : box.Image;
            if (backgroundImage)
                box.BackgroundImage = newImage;
            else
                box.Image = newImage;
            if (dispose)
                oldImage?.Dispose();
        }

        private void SetTrainStationImage()
        {
            if (stationPictureBox.Image != null)
            {
                var img = stationPictureBox.Image;
                stationPictureBox.Image = null;
                img.Dispose();
            }

            if (!stationTypeComboBox.Enabled) return;
            var stationType = SaveFile.ReadByte(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TrainStationType);
            if (stationType >= 15) return;
            stationTypeComboBox.SelectedIndex = stationType;
            stationPictureBox.Image = TrainStation.GetStationImage(stationType);
        }

        private void UpdatePlayerItemPictureBox(PictureBox box, Image image)
        {
            if (box == null) return;
            if (box.Image == ImageX)
            {
                box.Image = image;
            }
            else
            {
                RefreshPictureBoxImage(box, image);
            }
        }

        private void ReloadPlayer(Player player)
        {
            //TODO: Hook up face swap on gender change for New Leaf

            // Hack: Set _loading to true to stop data from changing
            var wasLoading = _loading;
            _loading = true;

            if (SaveFile.SaveGeneration != SaveGeneration.N3DS)
            {
                playerWallet.Text = player.Data.Bells.ToString();
                if (SaveFile.SaveType != SaveType.WildWorld)
                    playerDebt.Text = player.Data.Debt.ToString();
                playerSavings.Text = player.Data.Savings.ToString();
            }
            playerName.Text = player.Data.Name;
            playerGender.SelectedIndex = player.Data.Gender == 0 ? 0 : 1;
            RefreshPictureBoxImage(heldItemPicturebox, Inventory.GetItemPic(16, player.Data.HeldItem, SaveFile.SaveType));

            //Birthday
            if (player.Data.Birthday != null)
            {
                switch (SaveFile.SaveGeneration)
                {
                    case SaveGeneration.Wii:
                        if (player.Data.Birthday.Month > 11)
                        {
                            birthdayMonth.SelectedIndex = 0;
                        }
                        else
                        {
                            birthdayMonth.SelectedIndex = (int)player.Data.Birthday.Month + 1;
                        }

                        break;
                    default:
                        if (player.Data.Birthday.Month < 1 || player.Data.Birthday.Month > 12)
                        {
                            birthdayMonth.SelectedIndex = 0;
                        }
                        else
                        {
                            birthdayMonth.SelectedIndex = (int)player.Data.Birthday.Month;
                        }

                        break;
                }

                try
                {
                    birthdayDay.SelectedIndex = player.Data.Birthday.Day < 32 ? (int)player.Data.Birthday.Day : 0;
                }
                catch
                {
                    birthdayDay.SelectedIndex = 0;
                }
            }

            //These vary game to game
            if (playerFace.Items.Count > 0)
            {
                switch (SaveFile.SaveGeneration)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                    case SaveGeneration.iQue:
                        playerFace.SelectedIndex =
                            (player.Data.Gender == 1 ? (byte)(player.Data.FaceType + 8) : player.Data.FaceType);
                        break;
                    default:
                        playerFace.SelectedIndex = player.Data.FaceType;
                        break;
                }
            }

            if (playerHairType.Items.Count > 0)
                playerHairType.SelectedIndex = player.Data.HairType;
            if (playerHairColor.Enabled && playerHairColor.Items.Count > 0)
                playerHairColor.SelectedIndex = player.Data.HairColor;
            if (playerNookPoints.Enabled)
                playerNookPoints.Text = player.Data.NookPoints.ToString();
            if (bedPicturebox.Enabled)
                UpdatePlayerItemPictureBox(bedPicturebox, Inventory.GetItemPic(16, player.Data.Bed, SaveFile.SaveType));
            if (hatPicturebox.Enabled)
                UpdatePlayerItemPictureBox(hatPicturebox, Inventory.GetItemPic(16, player.Data.Hat, SaveFile.SaveType));
            if (facePicturebox.Enabled)
                UpdatePlayerItemPictureBox(facePicturebox, Inventory.GetItemPic(16, player.Data.FaceItem, SaveFile.SaveType));
            if (pocketsBackgroundPicturebox.Enabled)
                UpdatePlayerItemPictureBox(pocketsBackgroundPicturebox,
                    Inventory.GetItemPic(16, player.Data.InventoryBackground, SaveFile.SaveType));
            //City Folk only
            if (playerShoeColor.Enabled)
                playerShoeColor.SelectedIndex = player.Data.ShoeColor;
            //New Leaf only
            if (pantsPicturebox.Enabled)
                UpdatePlayerItemPictureBox(pantsPicturebox, Inventory.GetItemPic(16, player.Data.Pants, SaveFile.SaveType));
            if (socksPicturebox.Enabled)
                UpdatePlayerItemPictureBox(socksPicturebox, Inventory.GetItemPic(16, player.Data.Socks, SaveFile.SaveType));
            if (shoesPicturebox.Enabled)
                UpdatePlayerItemPictureBox(shoesPicturebox, Inventory.GetItemPic(16, player.Data.Shoes, SaveFile.SaveType));
            if (playerEyeColor.Enabled && playerEyeColor.Items.Count > 0)
                playerEyeColor.SelectedIndex = player.Data.EyeColor; // Is this good now?
            if (playerWetsuit.Enabled)
                UpdatePlayerItemPictureBox(playerWetsuit, Inventory.GetItemPic(16, player.Data.Wetsuit, SaveFile.SaveType));

            if (player.Data.Tan <= tanTrackbar.Maximum)
                tanTrackbar.Value = player.Data.Tan + 1;

            if (SaveFile.SaveType == SaveType.WelcomeAmiibo)
                censusMenuEnabled.Checked = (SaveFile.ReadByte(player.Offset + 0x572F) & 0x40) == 0x40;

            if (player.Data.Patterns != null)
            {
                for (var i = 0; i < player.Data.Patterns.Length; i++)
                {
                    if (player.Data.Patterns[i] != null && player.Data.Patterns[i].PatternBitmap != null)
                    {
                        RefreshPictureBoxImage(_patternBoxes[i], player.Data.Patterns[i].PatternBitmap, false, false);
                    }
                }
                _selectedPaletteIndex = player.Data.Patterns[0].Palette;
                patternEditorPictureBox.Image = ImageGeneration.DrawGrid2(_patternBoxes[0].Image, 16, new Size (513, 513));
                paletteSelectionPictureBox.Image = PatternUtility.GeneratePalettePreview(
                    player.Data.Patterns[0].PaletteData, _selectedPaletteIndex,
                    (uint) paletteSelectionPictureBox.Size.Width, (uint) paletteSelectionPictureBox.Size.Height);
                _selectedPatternObject = player.Data.Patterns[0];
                paletteIndexLabel.Text = "Palette: " + (_selectedPatternObject.Palette + 1);
                patternNameTextBox.Text = _selectedPatternObject.Name;
            }

            resettiCheckBox.Checked = player.Data.Reset;

            if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                if (_tpcPicture.Image == NoTPC)
                {
                    _tpcPicture.Image = player.Data.TownPassCardImage;
                }
                else
                {
                    RefreshPictureBoxImage(_tpcPicture, player.Data.TownPassCardImage, false, false);
                }

                playerWallet.Text = player.Data.NewLeafWallet.Value.ToString();
                playerSavings.Text = player.Data.NewLeafSavings.Value.ToString();
                playerDebt.Text = player.Data.NewLeafDebt.Value.ToString();
                playerIslandMedals.Text = player.Data.IslandMedals.Value.ToString();
                if (SaveFile.SaveType == SaveType.WelcomeAmiibo)
                    playerMeowCoupons.Text = player.Data.MeowCoupons.Value.ToString();
            }

            // Refresh Inventory
            if (_inventoryEditor != null && !_inventoryEditor.IsDisposed)
            {
                _inventoryEditor.Items = player.Data.Pockets.Items;
            }

            // Refresh Shirt
            if (_shirtEditor != null)
            {
                _shirtEditor.Item = player.Data.Shirt;
            }

            // Refresh Dressers
            if (_dresserEditor != null && !_dresserEditor.IsDisposed && player.Data.IslandBox != null)
            {
                _dresserEditor.Items = player.Data.Dressers;
            }

            // Refresh Island Box
            if (_islandBoxEditor != null && !_islandBoxEditor.IsDisposed && player.Data.IslandBox != null)
            {
                _islandBoxEditor.Items = player.Data.IslandBox;
            }

            _loading = wasLoading;

            // Refresh Badges (New Leaf)
            if (SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            ClearBadges();
            AddBadges();
        }

        private void TownNameBoxTextChanged(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || string.IsNullOrEmpty(townNameBox.Text.Trim())) return;
            SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TownName, AcString.GetBytes(townNameBox.Text.Trim(),
                townNameBox.MaxLength));
            townNameBox.Text = townNameBox.Text.Trim();
            foreach (var player in _players)
            {
                if (player == null || !player.Exists) continue;
                player.Data.TownName = townNameBox.Text;
                if (player.House != null)
                {
                    player.House.Data.TownName = townNameBox.Text;
                }
            }

            foreach (var villager in _villagers)
            {
                if (villager == null || !villager.Exists) continue;
                var villagerTownName = villager.Data.TownName;
                villager.Data.TownName = townNameBox.Text;

                if (villager.AnimalMemories == null) continue;
                foreach (var memory in villager.AnimalMemories)
                {
                    if (!memory.Exists) continue;
                    if (memory.PlayerTownName.Equals(villagerTownName) && memory.PlayerTownId == villager.Data.TownId)
                    {
                        memory.PlayerTownName = townNameBox.Text;
                    }

                    if (memory.MetTownName.Equals(villagerTownName) && memory.MetTownId == villager.Data.TownId)
                    {
                        memory.MetTownName = townNameBox.Text;
                    }
                }
            }

            // TODO: Island Town Name for DnM+/AC

            if (_islands == null) return;
            foreach (var isle in _islands)
            {
                isle.TownName = townNameBox.Text;
            }
        }

        private void Gender_Changed()
        {
            if (SaveFile == null || _loading) return;

            //New Leaf face swap on gender change
            if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                playerFace.Items.Clear();
                playerFace.Items.AddRange(playerGender.Text == "Male" ? PlayerInfo.NlMaleFaces : PlayerInfo.NlFemaleFaces);
                playerFace.SelectedIndex = _selectedPlayer.Data.FaceType;
            }
            _selectedPlayer.Data.Gender = playerGender.Text == "Female" ? (byte)1 : (byte)0;
            SetFace(playerFace.SelectedIndex);
        }

        private void SetFace(int faceType)
        {
            switch (SaveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    _selectedPlayer.Data.FaceType = (byte) (_selectedPlayer.Data.Gender == 1
                        ? faceType - 8
                        : faceType);
                    break;
                default:
                    _selectedPlayer.Data.FaceType = (byte) faceType;
                    break;
            }

        }

        private void Face_Changed()
        {
            if (SaveFile == null || _selectedPlayer == null || playerFace.SelectedIndex <= -1) return;

            if (!_loading)
            {
                SetFace(playerFace.SelectedIndex);
            }

            RefreshPictureBoxImage(facePreviewPictureBox,
                ImageGeneration.GetFaceImage(SaveFile.SaveGeneration, playerFace.SelectedIndex,
                    _selectedPlayer.Data.Gender));
        }

        private void Hair_Changed()
        {
            if (SaveFile == null || _selectedPlayer == null || playerHairType.SelectedIndex <= -1) return;

            _selectedPlayer.Data.HairType = (byte)playerHairType.SelectedIndex;
            if (SaveFile.SaveGeneration == SaveGeneration.NDS || SaveFile.SaveGeneration == SaveGeneration.Wii ||
                SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                RefreshPictureBoxImage(hairPictureBox,
                    ImageGeneration.GetHairImage(SaveFile.SaveGeneration, _selectedPlayer.Data.HairType,
                        _selectedPlayer.Data.HairColor));
            }
        }

        private void Hair_Color_Changed()
        {
            if (SaveFile == null || _selectedPlayer == null || playerHairColor.SelectedIndex <= -1) return;

            _selectedPlayer.Data.HairColor = (byte)playerHairColor.SelectedIndex;
            if (SaveFile.SaveGeneration == SaveGeneration.NDS || SaveFile.SaveGeneration == SaveGeneration.Wii ||
                SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                RefreshPictureBoxImage(hairPictureBox,
                    ImageGeneration.GetHairImage(SaveFile.SaveGeneration, _selectedPlayer.Data.HairType,
                        _selectedPlayer.Data.HairColor));
            }
        }

        private void Eye_Color_Changed()
        {
            if (SaveFile != null && _selectedPlayer != null && playerEyeColor.SelectedIndex > -1)
            {
                _selectedPlayer.Data.EyeColor = (byte)playerEyeColor.SelectedIndex;
            }
        }

        private void Shoe_Color_Changed()
        {
            if (SaveFile != null && _selectedPlayer != null && playerShoeColor.SelectedIndex > -1)
            {
                _selectedPlayer.Data.ShoeColor = (byte)playerShoeColor.SelectedIndex;
            }
        }

        private void SetupAcreEditorTreeView()
        {
            acreTreeView.Nodes.Clear();
            acreTreeView.BeginUpdate();

            if (_filedAcreData != null)
            {
                foreach (var acreGroup in _filedAcreData)
                {
                    var acreType = new TreeNode
                    {
                        Text = acreGroup.Key
                    };
                    acreTreeView.Nodes.Add(acreType);

                    foreach (var acreId in acreGroup.Value)
                    {
                        var acre = new TreeNode
                        {
                            Tag = acreId.ToString("X2")
                        };
                        acre.Text = _acreInfo != null ? _acreInfo[acreId] : (string)acre.Tag;
                        acre.Name = (string)acre.Tag;
                        acreType.Nodes.Add(acre);
                    }
                }
            }
            else if (_uInt16FiledAcreData != null)
            {
                foreach (var acreGroup in _uInt16FiledAcreData)
                {
                    var acreType = new TreeNode
                    {
                        Text = acreGroup.Key
                    };
                    acreTreeView.Nodes.Add(acreType);

                    foreach (var acreInfo in acreGroup.Value)
                    {
                        var acre = new TreeNode
                        {
                            Tag = acreInfo.Key.ToString("X4"),
                            Text = acreInfo.Value
                        };
                        acre.Name = (string)acre.Tag;
                        acreType.Nodes.Add(acre);
                    }
                }
            }

            acreTreeView.EndUpdate();
        }

        private void SetPossibleNativeFruits(SaveGeneration saveGeneration)
        {

            nativeFruitBox.Items.Clear();
            nativeFruitBox.Enabled = saveGeneration != SaveGeneration.NDS;
            nativeFruitBox.Items.AddRange(NativeFruit.GetNativeFruitTypes(saveGeneration));

            var fruitIndex = NativeFruit.GetNativeFruit(SaveFile);

            if (fruitIndex > -1 && fruitIndex < nativeFruitBox.Items.Count)
            {
                nativeFruitBox.SelectedIndex = fruitIndex;
            }
        }

        private void ItemSelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedItem.SelectedValue == null) return;
            if (ushort.TryParse(selectedItem.SelectedValue.ToString(), out var itemId))
            {
                //selectedItemText.Text = string.Format("Selected Item: [0x{0}]", Item_ID.ToString("X4"));
                SetCurrentItem(new Item(itemId, byte.Parse(itemFlag1.Text, NumberStyles.HexNumber),
                    byte.Parse(itemFlag2.Text, NumberStyles.HexNumber)));
            }
        }

        private void CurrentItemIdTextChanged(object sender, EventArgs e)
        {
            ReplaceVerifyHex(itemIdTextBox, 4);
            if (ushort.TryParse(itemIdTextBox.Text, NumberStyles.HexNumber, null, out var itemId))
            {
                SetCurrentItem(new Item(itemId, byte.Parse(itemFlag1.Text, NumberStyles.HexNumber),
                    byte.Parse(itemFlag2.Text, NumberStyles.HexNumber)));
            }
        }

        private void CurrentItemIdLostFocus(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(itemIdTextBox.Text))
            {
                itemIdTextBox.Text = _currentItem.ItemId.ToString("X4");
            }
        }

        private void SetSelectedAcre(ushort acreId, bool focusTreeNode = true)
        {
            if (acreId <= 0) throw new ArgumentOutOfRangeException(nameof(acreId));
            _selectedAcreId = acreId;

            if (SaveFile.SaveGeneration == SaveGeneration.N64 || SaveFile.SaveGeneration == SaveGeneration.GCN ||
                SaveFile.SaveGeneration == SaveGeneration.iQue)
            {
                acreHeightTrackBar.Value = _selectedAcreId & 3;
                _acreHeightModifier = (ushort) acreHeightTrackBar.Value;
                _selectedAcreId -= (ushort) acreHeightTrackBar.Value;
            }

            var acreIdStr = _selectedAcreId.ToString(SaveFile.SaveType == SaveType.WildWorld ? "X2 " : "X4");

            var image = AcreImageManager.FetchAcreImage(SaveFile.SaveType, _selectedAcreId)
                        ?? AcreImageManager.FetchAcreImage(SaveFile.SaveType,
                            SaveFile.SaveType == SaveType.WildWorld ? (ushort) 0xFF : (ushort) 0xFFFF);

            _selectedAcrePicturebox.Image = image;
            if (SaveFile.SaveType == SaveType.WildWorld)
                acreID.Text = "Acre ID: 0x" + _selectedAcreId.ToString("X2");
            else
                acreID.Text = "Acre ID: 0x" + (_selectedAcreId + _acreHeightModifier).ToString("X4");
            if (_acreInfo != null && _acreInfo.ContainsKey((byte)_selectedAcreId))
                acreDesc.Text = _acreInfo[(byte)acreId];
            else if (_uInt16AcreInfo != null)
                acreDesc.Text = _uInt16AcreInfo.ContainsKey(_selectedAcreId) ? _uInt16AcreInfo[_selectedAcreId] : "No Acre Description";

            if (SaveFile.SaveGeneration == SaveGeneration.N64 || SaveFile.SaveGeneration == SaveGeneration.GCN ||
                SaveFile.SaveGeneration == SaveGeneration.iQue)
            {
                if (IsOcean(acreId))
                {
                    _selectedAcrePicturebox.Image = AcreImageManager.FetchAcreImage(SaveFile.SaveType, 0x03DC);
                }
            }

            foreach (TreeNode node in acreTreeView.Nodes)
            {
                var matchingNodes = node.Nodes.Find(acreIdStr, true);
                if (matchingNodes.Length <= 0) continue;
                node.Toggle();
                acreTreeView.SelectedNode = matchingNodes[0];
                if (focusTreeNode)
                    acreTreeView.Focus();
                return;
            }
        }

        // TODO: Change byte based indexes to use Dictionary<ushort, Dictionary<ushort, string>>
        private void AcreTreeViewEntryClicked(object sender, TreeViewEventArgs e)
        {
            var node = acreTreeView.SelectedNode;
            if (node?.Tag == null) return;
            Image image = null;
            if (ushort.TryParse((string)node.Tag, NumberStyles.HexNumber, null, out var acreId))
                image = AcreImageManager.FetchAcreImage(SaveFile.SaveType, acreId);

            if (image == null)
                image = AcreImageManager.FetchAcreImage(SaveFile.SaveType,
                    SaveFile.SaveType == SaveType.WildWorld ? (ushort) 0xFF : (ushort) 0xFFFF);

            var oldImage = _selectedAcrePicturebox.Image;
            _selectedAcrePicturebox.Image = image;
            AcreImageManager.CheckReferencesAndDispose(oldImage, _acreMap, _selectedAcrePicturebox);

            if (_acreInfo != null)
                _selectedAcreId = byte.Parse((string)node.Tag, NumberStyles.AllowHexSpecifier);
            else if (_uInt16FiledAcreData != null)
                _selectedAcreId = ushort.Parse((string)node.Tag, NumberStyles.AllowHexSpecifier);
            if (SaveFile.SaveType == SaveType.WildWorld)
                acreID.Text = "Acre ID: 0x" + _selectedAcreId.ToString("X2");
            else
                acreID.Text = "Acre ID: 0x" + (_selectedAcreId + _acreHeightModifier).ToString("X4");
            if (_acreInfo != null && _acreInfo.ContainsKey((byte)_selectedAcreId))
                acreDesc.Text = _acreInfo[(byte)_selectedAcreId];
            else if (_uInt16FiledAcreData != null)
                acreDesc.Text = node.Text;

            if (SaveFile.SaveGeneration == SaveGeneration.N64 || SaveFile.SaveGeneration == SaveGeneration.GCN ||
                SaveFile.SaveGeneration == SaveGeneration.iQue)
            {
                if (IsOcean(_selectedAcreId))
                {
                    oldImage = _selectedAcrePicturebox.Image;
                    _selectedAcrePicturebox.Image = AcreImageManager.FetchAcreImage(SaveFile.SaveType, 0x03DC);
                    AcreImageManager.CheckReferencesAndDispose(oldImage, _acreMap, _selectedAcrePicturebox);
                }
            }

            // Warnings for N64/GameCube titles
            if (SaveFile.SaveGeneration != SaveGeneration.N64 && SaveFile.SaveGeneration != SaveGeneration.GCN &&
                SaveFile.SaveGeneration != SaveGeneration.iQue) return;

            if (Properties.Settings.Default.ShowBetaAcreWarning &&
                (node.Parent.Text.Equals("Beta Acres") || node.Parent.Text.Equals("Misc. Acres")))
            {
                var alert = new ToggleableAlertForm(
                    "Placing beta acres in the Town Region (anywhere your map would show) will cause your game to crash when you open the map!" +
                    "It's recommended you only place them in border acres or ocean/island acres!",
                    "Beta Acre Warning");
                if (alert.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.ShowBetaAcreWarning = !alert.AlertDisabled;
                    Properties.Settings.Default.Save();
                }

                alert.Dispose();
            }
            else if (node.Tag != null && ushort.TryParse((string)node.Tag, NumberStyles.AllowHexSpecifier, null, out acreId))
            {
                if (!Properties.Settings.Default.ShowDumpAcreWarning ||
                    (acreId != 0x0118 && acreId != 0x0294 && acreId != 0x0298)) return;
                var alert = new ToggleableAlertForm(
                    "Placing a dump acre without adding a dump item to the acre will cause your game to crash. Please be careful!",
                    "Dump Placement Warning");
                if (alert.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.ShowDumpAcreWarning = !alert.AlertDisabled;
                    Properties.Settings.Default.Save();
                }
                alert.Dispose();
            }
        }

        private static bool IsOcean(ushort id)
        {
            return ((SaveFile.SaveGeneration == SaveGeneration.GCN || SaveFile.SaveType == SaveType.DoubutsuNoMori ||
                     SaveFile.SaveType == SaveType.DongwuSenlin)
                    && (id >= 0x03DC && id <= 0x03EC) || id == 0x49C || (id >= 0x04A8 && id <= 0x058C) ||
                    (id >= 0x05B4 && id <= 0x05B8));
        }

        private static Image GetAcreImage(ushort id)
        {
            Image acreImage = null;

            switch (SaveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                    id = (ushort)(id & 0xFFFC);
                    if (IsOcean(id))
                    {
                        acreImage = AcreImageManager.FetchAcreImage(SaveFile.SaveType, 0x03DC);
                    }
                    else
                    {
                        acreImage = AcreImageManager.FetchAcreImage(SaveFile.SaveType, id) ??
                                    AcreImageManager.FetchAcreImage(SaveFile.SaveType, 0xFFFF);
                    }
                    break;
                case SaveGeneration.NDS:
                    acreImage = AcreImageManager.FetchAcreImage(SaveFile.SaveType, id) ??
                                AcreImageManager.FetchAcreImage(SaveFile.SaveType, 0xFF);
                    break;
                case SaveGeneration.Wii:
                case SaveGeneration.N3DS:
                    acreImage = AcreImageManager.FetchAcreImage(SaveFile.SaveType, id) ??
                                AcreImageManager.FetchAcreImage(SaveFile.SaveType, 0xFFFF);
                    break;
            }

            return acreImage;
        }

        private void SetupMapPictureBoxes()
        {
            acrePanel.SuspendLayout();
            townPanel.SuspendLayout();
            islandPanel.SuspendLayout();
            grassPanel.SuspendLayout();

            _lastBoxEntered = null;
            if (_acreMap != null)
                foreach (var map in _acreMap)
                    map.Dispose();

            if (_townAcreMap != null)
                foreach (var map in _townAcreMap)
                {
                    var img = map.Image;
                    map.Dispose();
                    img?.Dispose();
                }
            if (_islandAcreMap != null)
                foreach (var map in _islandAcreMap)
                    if (map != null)
                    {
                        var img = map.Image;
                        map.Dispose();
                        img?.Dispose();
                    }

            if (_newLeafIslandAcreMap != null)
                foreach (var map in _newLeafIslandAcreMap)
                {
                    map?.Dispose();
                }

            if (_grassMap != null)
                foreach (var grassMap in _grassMap)
                {
                    var img = grassMap.Image;
                    grassMap.Dispose();
                    img?.Dispose();
                }
            _acreMap = new PictureBoxWithInterpolationMode[CurrentSaveInfo.AcreCount];
            _townAcreMap = new PictureBoxWithInterpolationMode[CurrentSaveInfo.TownAcreCount];
            _grassMap = SaveFile.SaveType == SaveType.CityFolk ? new PictureBoxWithInterpolationMode[_townAcreMap.Length] : null;
            _newLeafGrassOverlay?.Dispose();
            _newLeafGrassOverlay = null;

            //Assume First Acre Row + Side Acre Columns are Border Acres
            var numYAcres = CurrentSaveInfo.AcreCount / CurrentSaveInfo.XAcreCount;
            var townAcreCount = 0;
            for (var y = 0; y < numYAcres; y++)
                for (var x = 0; x < CurrentSaveInfo.XAcreCount; x++)
                {
                    var acre = y * CurrentSaveInfo.XAcreCount + x;
                    var currentAcre = _acres[acre];
                    var acreImage = GetAcreImage(currentAcre.AcreId);

                    _acreMap[acre] = new PictureBoxWithInterpolationMode()
                    {
                        Size = new Size(_acreMapSize, _acreMapSize),
                        Location = new Point(x * _acreMapSize, (acre / CurrentSaveInfo.XAcreCount) * _acreMapSize),
                        BackgroundImage = acreImage,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode,
                        UseInternalInterpolationSetting = false,
                    };
                    
                    _acreMap[acre].MouseMove += (s, e) => AcreEditorMouseMove(s, e);
                    _acreMap[acre].MouseLeave += HideAcreTip;
                    _acreMap[acre].MouseClick += (s, e) => AcreClick(s, e,
                        SaveFile.SaveGeneration == SaveGeneration.GCN
                            ? Array.IndexOf(_acreMap, s) == 0x3C || Array.IndexOf(_acreMap, s) == 0x3D
                            : false);
                    _acreMap[acre].MouseEnter += AcreEditorMouseEnter;
                    _acreMap[acre].MouseLeave += AcreEditorMouseLeave;
                    acrePanel.Controls.Add(_acreMap[acre]);
                    if (y < CurrentSaveInfo.TownYAcreStart || x <= 0 || x >= CurrentSaveInfo.XAcreCount - 1) continue;
                    var townAcre = (y - CurrentSaveInfo.TownYAcreStart) * (CurrentSaveInfo.XAcreCount - 2) + (x - 1);
                    if (townAcre >= CurrentSaveInfo.TownAcreCount) continue;
                    {
                        var townAcreBitmap = GenerateAcreItemsBitmap(TownAcres[townAcreCount]);
                        if (townAcre >= CurrentSaveInfo.TownAcreCount) continue;
                        _townAcreMap[townAcre] = new PictureBoxWithInterpolationMode
                        {
                            InterpolationMode = InterpolationMode.HighQualityBicubic,
                            Size = new Size(_townMapTotalSize, _townMapTotalSize),
                            Location = new Point((x - 1) * (_townMapTotalSize + 1),
                                (townAcre / (CurrentSaveInfo.XAcreCount - 2)) * (_townMapTotalSize + 1)),
                            Image = TownAcres[townAcreCount] != null ? townAcreBitmap : null,
                            BackgroundImage = acreImage,
                            BackgroundImageLayout = ImageLayout.Stretch,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            UseInternalInterpolationSetting = true,
                            Tag = townAcre
                        };
                        townAcreCount++;
                        _townAcreMap[townAcre].MouseMove += (sender, e) => TownMove(sender, e);
                        _townAcreMap[townAcre].MouseLeave += HideTownTip;
                        _townAcreMap[townAcre].MouseDown += (sender, e) => TownMouseDown(sender, e);
                        _townAcreMap[townAcre].MouseEnter += (sender, e) => TownEnter(sender);
                        _townAcreMap[townAcre].MouseUp += TownMouseUp;
                        townPanel.Controls.Add(_townAcreMap[townAcre]);
                        if (_grassMap == null || _grassMap.Length != _townAcreMap.Length) continue;
                        _grassMap[townAcre] = new PictureBoxWithInterpolationMode()
                        {
                            InterpolationMode = InterpolationMode.NearestNeighbor,
                            Size = new Size(96, 96),
                            Location = new Point((x - 1) * 96, 30 + (townAcre / (CurrentSaveInfo.XAcreCount - 2) * 96)),
                            Image = ImageGeneration.DrawGrassWear(_grassWear.Skip(townAcre * 256).Take(256).ToArray()),
                            BackgroundImage = ImageGeneration.MakeGrayscale((Bitmap)acreImage),
                            BackgroundImageLayout = ImageLayout.Stretch,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            UseInternalInterpolationSetting = true,
                        };
                        _grassMap[townAcre].MouseDown += GrassEditorMouseDown;
                        _grassMap[townAcre].MouseUp += GrassEditorMouseUp;
                        _grassMap[townAcre].MouseMove += GrassEditorMouseMove;
                        grassPanel.Controls.Add(_grassMap[townAcre]);
                    }
                }
            if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                if (_newLeafGrassOverlay == null)
                {
                    _newLeafGrassOverlay = new PictureBoxWithInterpolationMode
                    {
                        Size = new Size(96 * 7, 96 * 6),
                        Location = new Point(0, 30),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        InterpolationMode = InterpolationMode.NearestNeighbor,
                        UseInternalInterpolationSetting = true,
                    };
                    grassPanel.Controls.Add(_newLeafGrassOverlay);
                    _newLeafGrassOverlay.MouseDown += GrassEditorMouseDown;
                    _newLeafGrassOverlay.MouseUp += GrassEditorMouseUp;
                    _newLeafGrassOverlay.MouseMove += GrassEditorMouseMove;
                }
                _newLeafGrassOverlay.Image = ImageGeneration.DrawGrassWear(_grassWear);
                _newLeafGrassOverlay.BackgroundImage = ImageGeneration.DrawNewLeafGrassBg(_acreMap);
                //Add events here
            }

            if (CurrentSaveInfo.ContainsIsland)
            {
                IslandAcres = new WorldAcre[CurrentSaveInfo.IslandAcreCount];
                _islandAcreMap = new PictureBoxWithInterpolationMode[IslandAcres.Length];
                _newLeafIslandAcreMap = new PictureBoxWithInterpolationMode[16];

                for (var y = 0; y < CurrentSaveInfo.IslandAcreCount / CurrentSaveInfo.IslandXAcreCount; y++)
                {
                    for (var x = 0; x < CurrentSaveInfo.IslandXAcreCount; x++)
                    {
                        var idx = y * CurrentSaveInfo.IslandXAcreCount + x;
                        var acreId =
                            SaveFile.ReadUInt16(
                                SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.IslandAcreData + idx * 2,
                                SaveFile.IsBigEndian);

                        var acreItems = new Item[256];
                        for (var i = 0; i < 256; i++)
                            if (SaveFile.SaveGeneration == SaveGeneration.GCN)
                            {
                                acreItems[i] =
                                    new Item(
                                        SaveFile.ReadUInt16(
                                            SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.IslandWorldData +
                                            idx * 512 + i * 2, true));
                            }
                            else if ((idx > 4 && idx < 7) || (idx > 8 && idx < 11)) //Other acres are water acres
                            {
                                var worldIdx = (y - 1) * 2 + ((x - 1) % 4);
                                acreItems[i] =
                                    new Item(
                                        SaveFile.ReadUInt32(SaveFile.SaveDataStartOffset +
                                                            CurrentSaveInfo.SaveOffsets.IslandWorldData +
                                                            worldIdx * 1024 +
                                                            i * 4));
                            }

                        IslandAcres[idx] = new WorldAcre(acreId, idx, acreItems);

                        if (SaveFile.SaveGeneration == SaveGeneration.GCN ||
                            ((idx > 4 && idx < 7) || (idx > 8 && idx < 11)))
                        {
                            _islandAcreMap[idx] = new PictureBoxWithInterpolationMode
                            {
                                Size = new Size(_townMapTotalSize, _townMapTotalSize),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                BackgroundImageLayout = ImageLayout.Stretch,
                                InterpolationMode = InterpolationMode.HighQualityBicubic,
                                UseInternalInterpolationSetting = false,
                                Tag = idx
                            };
                            if (SaveFile.SaveType == SaveType.DoubutsuNoMoriEPlus ||
                                SaveFile.SaveType == SaveType.AnimalForestEPlus)
                            {
                                _islandAcreMap[idx].Image =
                                    GenerateAcreItemsBitmap(_selectedIsland.Acres[idx], true);
                                _islandAcreMap[idx].BackgroundImage = GetAcreImage(_acres[0x3C + idx].BaseAcreId);
                            }
                            else
                            {
                                _islandAcreMap[idx].Image = GenerateAcreItemsBitmap(IslandAcres[idx], true);
                                _islandAcreMap[idx].BackgroundImage = GetAcreImage(acreId);
                            }

                            _islandAcreMap[idx].MouseMove += (sender, e) => TownMove(sender, e, true);
                            _islandAcreMap[idx].MouseLeave += HideTownTip;
                            _islandAcreMap[idx].MouseDown += (sender, e) => TownMouseDown(sender, e, true);
                            _islandAcreMap[idx].MouseEnter += (sender, e) => TownEnter(sender);
                            _islandAcreMap[idx].MouseUp += TownMouseUp;
                            _islandAcreMap[idx].Location = (SaveFile.SaveGeneration == SaveGeneration.GCN)
                                ? new Point(x * _townMapTotalSize, y * _townMapTotalSize)
                                : new Point(((x - 1) % 4) * _townMapTotalSize, (y - 1) * _townMapTotalSize);
                            islandPanel.Controls.Add(_islandAcreMap[idx]);
                        }

                        if (SaveFile.SaveGeneration != SaveGeneration.N3DS) continue;
                        {
                            _newLeafIslandAcreMap[idx] = new PictureBoxWithInterpolationMode
                            {
                                Size = new Size(_acreMapSize, _acreMapSize),
                                Location = new Point(x * _acreMapSize, _townMapTotalSize * 2 + 24 + y * _acreMapSize),
                                BackgroundImage = GetAcreImage(IslandAcres[idx].AcreId),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                BackgroundImageLayout = ImageLayout.Stretch,
                                UseInternalInterpolationSetting = true,
                            };
                            _newLeafIslandAcreMap[idx].MouseMove += (s, e) => AcreEditorMouseMove(s, e, true);
                            _newLeafIslandAcreMap[idx].MouseLeave += HideAcreTip;
                            _newLeafIslandAcreMap[idx].MouseClick += (s, e) => AcreClick(s, e, true);
                            _newLeafIslandAcreMap[idx].MouseEnter += AcreEditorMouseEnter;
                            _newLeafIslandAcreMap[idx].MouseLeave += AcreEditorMouseLeave;
                            islandPanel.Controls.Add(_newLeafIslandAcreMap[idx]);
                        }
                    }
                }
            }

            acrePanel.ResumeLayout();
            townPanel.ResumeLayout();
            islandPanel.ResumeLayout();
            grassPanel.ResumeLayout();

            acrePanel.Refresh();
            townPanel.Refresh();
            islandPanel.Refresh();
            grassPanel.Refresh();

            if ((SaveFile.SaveType != SaveType.DoubutsuNoMoriEPlus && SaveFile.SaveType != SaveType.AnimalForestEPlus)
                || _selectedIsland == null || _islandAcreMap == null) return;

            var islandAcreIds = _selectedIsland.GetAcreIds();
            _islandAcreMap[0].BackgroundImage = GetAcreImage(islandAcreIds[0]);
            _islandAcreMap[1].BackgroundImage = GetAcreImage(islandAcreIds[1]);
        }

        #region Houses

        private void SetupHousePictureBoxes()
        {
            if (SaveFile == null || _houses == null) return;

            // Cleanup any existing controls
            _houseEditor?.Dispose();
            housePanel.Controls.Clear();

            if (_selectedHouse != null && (SaveFile.SaveGeneration == SaveGeneration.N64 ||
                                           SaveFile.SaveGeneration == SaveGeneration.GCN ||
                                           SaveFile.SaveGeneration == SaveGeneration.iQue))
            {
                basementCheckBox.Checked = HouseInfo.HasBasement(_selectedHouse.Offset, SaveFile.SaveType);
                StatueCheckBox.Checked = HouseInfo.HasStatue(_selectedHouse.Offset, SaveFile.SaveType);
            }

            if (houseOwnerComboBox.Enabled && _selectedHouse != null)
            {
                if (_selectedHouse.Owner != null)
                {
                    var ownerIndex = -1;
                    for (var i = 0; i < houseOwnerComboBox.Items.Count; i++)
                    {
                        if (!_selectedHouse.Owner.Data.Name.Equals(houseOwnerComboBox.Items[i])) continue;
                        ownerIndex = i;
                        break;
                    }

                    if (ownerIndex < 0 || ownerIndex > 4) // TODO: Change this to the total count in 
                        houseOwnerComboBox.SelectedIndex = 0;
                    else
                        houseOwnerComboBox.SelectedIndex = ownerIndex;
                }
                else
                {
                    houseOwnerComboBox.SelectedIndex = 0;
                }
            }

            if (_selectedHouse == null) return;
            _houseEditor = new HouseControl(this, SaveFile, _selectedHouse);
            housePanel.Controls.Add(_houseEditor);
        }

        private void BasementCheckBoxCheckChanged(object sender, EventArgs e)
        {
            if (!_loading && SaveFile != null && (SaveFile.SaveGeneration == SaveGeneration.N64 ||
                                                  SaveFile.SaveGeneration == SaveGeneration.GCN ||
                                                  SaveFile.SaveGeneration == SaveGeneration.iQue))
                HouseInfo.SetHasBasement(basementCheckBox.Checked, _selectedHouse);
        }

        private void House_Tab_Index_Changed(object sender, TabControlEventArgs e)
        {
            if (houseTabSelect.SelectedIndex < 0 || houseTabSelect.SelectedIndex > 3)
                return;
            _selectedHouse = _houses[houseTabSelect.SelectedIndex];
            if (_selectedHouse != null)
                ReloadHouse(_selectedHouse);
        }

        private void ReloadHouse(House selectedHouse)
        {
            if (selectedHouse == null) return;
            _loading = true;
            if (SaveFile.SaveGeneration == SaveGeneration.N64 ||
                SaveFile.SaveGeneration == SaveGeneration.GCN ||
                SaveFile.SaveGeneration == SaveGeneration.iQue)
            {
                basementCheckBox.Checked = HouseInfo.HasBasement(selectedHouse.Offset, SaveFile.SaveType);
                StatueCheckBox.Checked = HouseInfo.HasStatue(_selectedHouse.Offset, SaveFile.SaveType);
            }

            if (houseOwnerComboBox.Enabled)
            {
                if (selectedHouse.Owner != null)
                {
                    var ownerIndex = -1;
                    for (var i = 0; i < houseOwnerComboBox.Items.Count; i++)
                    {
                        if (!selectedHouse.Owner.Data.Name.Equals(houseOwnerComboBox.Items[i])) continue;
                        ownerIndex = i;
                        break;
                    }

                    if (ownerIndex < 0 || ownerIndex > 4) // TODO: Change this to the total count in 
                        houseOwnerComboBox.SelectedIndex = 0;
                    else
                        houseOwnerComboBox.SelectedIndex = ownerIndex;
                }
                else
                {
                    houseOwnerComboBox.SelectedIndex = 0;
                }
            }

            if (roofColorComboBox.Enabled && _selectedHouse != null)
                roofColorComboBox.SelectedIndex = Math.Min(roofColorComboBox.Items.Count - 1, selectedHouse.Data.RoofColor);

            if (houseSizeComboBox.Enabled && _selectedHouse != null)
                houseSizeComboBox.SelectedIndex = HouseInfo.GetHouseSize(_selectedHouse.Offset, SaveFile.SaveType);

            _houseEditor.House = _selectedHouse;
            _loading = false;
        }

        #endregion

        #region Island Cottage

        private void SetupIslandHouseBoxes()
        {
            _islandHouseEditor?.Dispose();

            if (SaveFile.SaveGeneration != SaveGeneration.GCN) return;
            switch (SaveFile.SaveType) // TODO: Add DnM+ & AC support.
            {
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    _islandHouseEditor = new HouseControl(this, SaveFile, _selectedIsland.Cabana);
                    break;
                default:
                    _islandHouseEditor = new HouseControl(this, SaveFile, _islandCabana);
                    break;
            }

            _islandHouseEditor.Location = new Point(0, _townMapTotalSize + 20);
            islandPanel.Controls.Add(_islandHouseEditor);
        }

        #endregion

        #region Villagers

        private void GenerateVillagerPanel(Villager villager)
        {
            var villagerEditor = new VillagerControl(this, villager.Index, SaveFile, villager, _villagerDatabase,
                _villagerNames, _personalityDatabase);
            villagerEditor.Location = new Point(0, 32 + villager.Index * villagerEditor.Height);

            villagerPanel.Controls.Add(villagerEditor);
        }

        private void GeneratePastVillagersPanel()
        {
            if (_pastVillagers == null) return;
            for (var i = 0; i < _pastVillagers.Length - 1; i++)
            {
                var villagerBox = new ComboBox { Size = new Size(150, 20), Location = new Point(5, 5 + 24 * i) };
                villagerBox.Items.AddRange(_villagerNames);
                villagerBox.SelectedIndex = Array.IndexOf(_villagerDatabase.Keys.ToArray(), _pastVillagers[i].VillagerId);
                pastVillagersPanel.Controls.Add(villagerBox);
            }
        }

        #endregion

        private Bitmap GenerateAcreItemsBitmap(WorldAcre acre, bool islandAcre = false,
            int hoveredAcre = -1, int xLoc = -1, int yLoc = -1)
        {
            var itemSize = _townMapCellSize;
            var acreSize = _townMapTotalSize;
            var width = itemSize * 16;
            var acreBitmap = new Bitmap(acreSize, acreSize);
            var bitmapBuffer = new byte[4 * (acreSize * acreSize)];
            for (var i = 0; i < 256; i++)
            {
                var item = acre.Items[i];
                var xLocation = i % 16;
                var yLocation = i / 16;
                if (item.Type == ItemType.Empty) continue;
                var itemColor = ItemData.GetItemColor(item.Type);
                // Draw Item Box
                for (var x = 0; x < itemSize * itemSize; x++)
                    Buffer.BlockCopy(BitConverter.GetBytes(itemColor), 0, bitmapBuffer,
                        ((yLocation * itemSize + x / itemSize) * acreSize * 4) +
                        ((xLocation * itemSize + x % itemSize) * 4), 4);
            }

            // Draw Border
            //ImageGeneration.DrawGrid(acreBitmap, 8);
            for (var i = 0; i < (width * width); i++)
                if ((i / itemSize > 0 && i % (itemSize * 16) > 0 && i % (itemSize) == 0) ||
                    (i / (itemSize * 16) > 0 && (i / (itemSize * 16)) % (itemSize) == 0))
                    Buffer.BlockCopy(GridColor, 0, bitmapBuffer,
                        ((i / (itemSize * 16)) * width * 4) + ((i % (itemSize * 16)) * 4), 4);

            // Construct Bitmap
            var bitmapData = acreBitmap.LockBits(new Rectangle(0, 0, acreSize, acreSize), ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            Marshal.Copy(bitmapBuffer, 0, bitmapData.Scan0, bitmapBuffer.Length);
            acreBitmap.UnlockBits(bitmapData);
            // Draw Buried X
            ImageGeneration.DrawBuriedIcons(acreBitmap, acre, _townMapCellSize);

            // Draw Current Cell Glow
            if (hoveredAcre == acre.Index && hoveredAcre > -1 && xLoc > -1 && yLoc > -1)
            {
                ImageGeneration.OverlayItemBoxGlow(acreBitmap, itemSize, xLoc, yLoc);
            }

            // Draw Buildings (if needed)
            if (SaveFile.SaveGeneration == SaveGeneration.Wii || SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                return islandAcre
                    ? ((SaveFile.SaveGeneration == SaveGeneration.N3DS)
                        ? ImageGeneration.DrawBuildings(acreBitmap, _islandBuildings, acre.Index - 5, _townMapCellSize)
                        : acreBitmap)
                    : ImageGeneration.DrawBuildings(acreBitmap, _buildings, acre.Index, _townMapCellSize);
            }

            return acreBitmap;
        }

        #region Grass
        private bool _grassEditorDown;

        private void GrassEditorMouseDown(object sender, MouseEventArgs e)
        {
            GrassEditorMouseClick(sender, e);
            _grassEditorDown = true;
        }

        private void GrassEditorMouseUp(object sender, MouseEventArgs e)
        {
            _grassEditorDown = false;
        }

        private void GrassEditorMouseMove(object sender, MouseEventArgs e)
        {
            if (_grassEditorDown)
                GrassEditorMouseClick(sender, e);
        }

        private void GrassEditorMouseClick(object sender, MouseEventArgs e)
        {
            if (!(sender is PictureBoxWithInterpolationMode grassBox)) return;
            grassBox.Capture = false;
            if (SaveFile.SaveType == SaveType.CityFolk)
            {
                var acre = Array.IndexOf(_grassMap, grassBox);
                int x = e.X / 6, y = e.Y / 6;
                int blockX = x / 8, blockY = y / 4;
                var xPos = x - (blockX * 8);
                var yPos = y - (blockY * 4);
                var dataPos = acre * 256 + blockY * 64 + blockX * 32 + yPos * 8 + xPos;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        byte.TryParse(grassLevelBox.Text, out var wearValue);
                        _grassWear[dataPos] = wearValue;
                        var oldImage = grassBox.Image;
                        grassBox.Image = ImageGeneration.DrawGrassWear(_grassWear.Skip(acre * 256).Take(256).ToArray());
                        oldImage?.Dispose();
                        grassBox.Refresh();
                        break;
                    case MouseButtons.Right:
                        grassLevelBox.Text = _grassWear[dataPos].ToString();
                        break;
                }
            }
            else //NL
            {
                var x = e.X / 6;
                var y = e.Y / 6;
                var grassDataOffset = 64 * ((y / 8) * 16 + (x / 8)) +
                    ImageGeneration.GrassWearOffsetMap[(y % 8) * 8 + (x % 8)];

                switch (e.Button)
                {
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        byte.TryParse(grassLevelBox.Text, out var wearValue);
                        _grassWear[grassDataOffset] = wearValue;
                        grassBox.Image.Dispose();
                        grassBox.Image = ImageGeneration.DrawGrassWear(_grassWear);
                        grassBox.Refresh();
                        break;
                    case MouseButtons.Right:
                        grassLevelBox.Text = _grassWear[grassDataOffset].ToString();
                        break;
                }
            }
        }
        #endregion

        #region Acres

        private readonly Bitmap _acreHighlightImage = ImageGeneration.DrawAcreHighlight();
        private PictureBoxWithInterpolationMode _lastBoxEntered;
        private void AcreEditorMouseEnter(object sender, EventArgs e)
        {
            if (!(sender is PictureBoxWithInterpolationMode box)) return;
            box.Capture = false;
            box.Image = _acreHighlightImage;
            box.Refresh();

            if (_lastBoxEntered != null && _lastBoxEntered != box)
            {
                _lastBoxEntered.Image = null;
            }

            _lastBoxEntered = box;
        }

        private static void AcreEditorMouseLeave(object sender, EventArgs e)
        {
            if (!(sender is PictureBoxWithInterpolationMode box)) return;
            box.Capture = false;
            box.Image = null;
            box.Refresh();
        }

        private int _lastAcreX = -1, _lastAcreY = -1;
        private void AcreEditorMouseMove(object sender, MouseEventArgs e, bool island = false, bool forceOverride = false)
        {
            if (!(sender is PictureBoxWithInterpolationMode box)) return;

            box.Capture = false;
            box.Image = _acreHighlightImage;
            if (_loading || (!forceOverride && e.X == _lastAcreX && e.Y == _lastAcreY)) return;

            acreToolTip.Hide(this);
            acreToolTip.RemoveAll();
            var acreIdx = SaveFile.SaveGeneration == SaveGeneration.N3DS
                ? Array.IndexOf(island ? _newLeafIslandAcreMap : _acreMap, box)
                : Array.IndexOf(_acreMap, box);

            Acre hoveredAcre = island && SaveFile.SaveGeneration == SaveGeneration.N3DS
                ? IslandAcres[acreIdx]
                : _acres[acreIdx];

            if (_acreInfo != null)
                acreToolTip.Show(
                    $"{(_acreInfo.ContainsKey((byte) hoveredAcre.AcreId) ? _acreInfo[(byte) hoveredAcre.AcreId] + " - " : "")}0x{hoveredAcre.AcreId:X2}",
                    box, e.X + 15, e.Y + 10);
            else if (_uInt16AcreInfo != null)
                if (SaveFile.SaveType == SaveType.DoubutsuNoMori || SaveFile.SaveGeneration == SaveGeneration.GCN ||
                    SaveFile.SaveType == SaveType.DongwuSenlin)
                {
                    acreToolTip.Show(string.Format("{0}[{2}] - 0x{1:X4}",
                            _uInt16AcreInfo.ContainsKey(hoveredAcre.BaseAcreId)
                                ? _uInt16AcreInfo[hoveredAcre.BaseAcreId] + " "
                                : (IsOcean(hoveredAcre.AcreId) ? "Ocean " : ""), hoveredAcre.AcreId,
                            AcreImageManager.AcreHeightIdentifiers[hoveredAcre.AcreId & 3]),
                        box, e.X + 15, e.Y + 10);
                }
                else
                    acreToolTip.Show(
                        $"{(_uInt16AcreInfo.ContainsKey(hoveredAcre.AcreId) ? _uInt16AcreInfo[hoveredAcre.AcreId] + " - " : "")}0x{hoveredAcre.AcreId:X4}",
                        box, e.X + 15, e.Y + 10);
            else
                acreToolTip.Show("0x" + hoveredAcre.AcreId.ToString("X"), box, e.X + 15, e.Y + 10);
            _lastAcreX = e.X;
            _lastAcreY = e.Y;
        }

        private void HideAcreTip(object sender, EventArgs e)
        {
            _lastAcreX = -1;
            _lastAcreY = -1;
            acreToolTip.Hide(this);
        }

        private void AcreClick(object sender, MouseEventArgs e, bool island = false)
        {
            var acreBox = sender as PictureBoxWithInterpolationMode;
            var acreIndex = SaveFile.SaveGeneration == SaveGeneration.N3DS
                ? Array.IndexOf(island ? _newLeafIslandAcreMap : _acreMap, acreBox)
                : Array.IndexOf(_acreMap, acreBox);

            int acreX, acreY;

            switch (SaveFile.SaveGeneration)
            {
                case SaveGeneration.GCN:
                    acreX = acreIndex % CurrentSaveInfo.XAcreCount;
                    acreY = acreIndex / CurrentSaveInfo.XAcreCount;

                    if (island)
                    {
                        acreIndex -= 0x3C;
                    }

                    break;

                case SaveGeneration.N3DS when !island:
                    acreX = acreIndex % CurrentSaveInfo.XAcreCount;
                    acreY = acreIndex / CurrentSaveInfo.XAcreCount;
                    break;

                case SaveGeneration.N3DS when island:
                    acreX = acreIndex % CurrentSaveInfo.IslandXAcreCount;
                    acreY = acreIndex / CurrentSaveInfo.IslandXAcreCount;
                    break;

                default:
                    acreX = acreIndex % CurrentSaveInfo.XAcreCount;
                    acreY = acreIndex / CurrentSaveInfo.XAcreCount;
                    break;
            }

            if (acreIndex <= -1) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                {
                    SaveFile.ChangesMade = true;

                    if (island)
                    {
                        IslandAcres[acreIndex] = new WorldAcre((ushort)(_selectedAcreId + _acreHeightModifier), acreIndex,
                            IslandAcres[acreIndex].Items);
                    }
                    else
                    {
                        _acres[acreIndex] = new WorldAcre((ushort)(_selectedAcreId + _acreHeightModifier), acreIndex);
                    }

                    if (acreBox != null)
                    {
                        var oldImage = acreBox.BackgroundImage;
                        acreBox.BackgroundImage = _selectedAcrePicturebox.Image;
                        AcreImageManager.CheckReferencesAndDispose(oldImage, _acreMap, _selectedAcrePicturebox);

                        if (!island && _grassMap != null && _grassMap.Length == _acreMap.Length)
                            _grassMap[acreIndex].BackgroundImage = acreBox.BackgroundImage;

                        var isTownAcre = acreY >= CurrentSaveInfo.TownYAcreStart &&
                                         acreY < CurrentSaveInfo.TownYAcreCount + CurrentSaveInfo.TownYAcreStart &&
                                         acreX > 0 && acreX < CurrentSaveInfo.XAcreCount - 1;

                        var isIslandAcre = SaveFile.SaveGeneration == SaveGeneration.GCN
                            ? acreY == 8 && acreX == 4 || acreX == 5
                            : acreY > 0 && acreX > 0 && acreY < 3 && acreX < 3;
                        var loadDefaultItems = false;

                        if (SaveFile.SaveGeneration == SaveGeneration.GCN && (isTownAcre || isIslandAcre))
                        {
                            loadDefaultItems = MessageBox.Show("Would you like to load the default items for this acre?", "Load Default Items?",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                        }

                        if (!island && isTownAcre)
                        {
                            var townAcre = (acreY - CurrentSaveInfo.TownYAcreStart) * (CurrentSaveInfo.XAcreCount - 2) + (acreX - 1);

                            if (townAcre < CurrentSaveInfo.TownAcreCount)
                            {
                                var currentTownAcre = TownAcres[townAcre];
                                TownAcres[townAcre] = new WorldAcre((ushort)(_selectedAcreId + _acreHeightModifier),
                                    townAcre, currentTownAcre.Items);
                                if (loadDefaultItems)
                                {
                                    TownAcres[townAcre].LoadDefaultItems(SaveFile);
                                    _acres[acreIndex].Items = TownAcres[townAcre].Items;
                                }
                                _townAcreMap[townAcre].BackgroundImage = acreBox.BackgroundImage;
                                RefreshPictureBoxImage(_townAcreMap[townAcre], GenerateAcreItemsBitmap(currentTownAcre));
                            }

                            if (_grassMap != null && _grassMap.Length == _townAcreMap.Length)
                                _grassMap[townAcre].BackgroundImage = ImageGeneration.MakeGrayscale((Bitmap)acreBox.BackgroundImage);

                            else if (_newLeafGrassOverlay != null)
                            {
                                _newLeafGrassOverlay.BackgroundImage.Dispose();
                                _newLeafGrassOverlay.BackgroundImage = ImageGeneration.DrawNewLeafGrassBg(_acreMap);
                            }
                        }

                        // TODO: Support default item loading for Animal Forest e+'s islands.
                        else if (SaveFile.SaveType != SaveType.DoubutsuNoMoriEPlus &&
                                 SaveFile.SaveType != SaveType.AnimalForestEPlus && island && isIslandAcre)
                        {
                            oldImage = _islandAcreMap[acreIndex].BackgroundImage;
                            _islandAcreMap[acreIndex].BackgroundImage = _selectedAcrePicturebox.Image;
                            AcreImageManager.CheckReferencesAndDispose(oldImage, _islandAcreMap, _selectedAcrePicturebox);
                            if (loadDefaultItems)
                            {
                                IslandAcres[acreIndex].LoadDefaultItems(SaveFile);
                            }

                            RefreshPictureBoxImage(_islandAcreMap[acreIndex],
                                GenerateAcreItemsBitmap(IslandAcres[acreIndex]));
                        }
                    }

                    AcreEditorMouseMove(sender, e, island, true);
                    break;
                }
                case MouseButtons.Right:
                {
                    var oldImage = _selectedAcrePicturebox.Image;
                    if (acreBox != null)
                    {
                        _selectedAcrePicturebox.Image = acreBox.BackgroundImage;
                    }

                    if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                    {
                        AcreImageManager.CheckReferencesAndDispose(oldImage, island ? _newLeafIslandAcreMap : _acreMap,
                            _selectedAcrePicturebox);
                    }
                    else
                    {
                        AcreImageManager.CheckReferencesAndDispose(oldImage, island ? _islandAcreMap : _acreMap,
                            _selectedAcrePicturebox);
                    }

                    _selectedAcreId = island ? IslandAcres[acreIndex].AcreId : _acres[acreIndex].AcreId;
                    var acreStr = SaveFile.SaveGeneration == SaveGeneration.NDS
                        ? _selectedAcreId.ToString("X2")
                        : _selectedAcreId.ToString("X4");
                    var baseAcreStr = acreStr;

                    switch (SaveFile.SaveGeneration)
                    {
                        case SaveGeneration.N64:
                        case SaveGeneration.GCN:
                        case SaveGeneration.iQue:
                            baseAcreStr = (_selectedAcreId & 0xFFFC).ToString("X4");
                            break;
                    }

                    if (SaveFile.SaveGeneration == SaveGeneration.N64 ||
                        SaveFile.SaveGeneration == SaveGeneration.GCN || SaveFile.SaveGeneration == SaveGeneration.iQue)
                    {
                        acreHeightTrackBar.Value = _selectedAcreId & 3;
                        _acreHeightModifier = (ushort) acreHeightTrackBar.Value;
                        _selectedAcreId -= (ushort) acreHeightTrackBar.Value;
                    }

                    if (SaveFile.SaveType == SaveType.WildWorld)
                    {
                        acreID.Text = "Acre ID: 0x" + _selectedAcreId.ToString("X2");
                    }
                    else if (SaveFile.SaveGeneration == SaveGeneration.N64 ||
                             SaveFile.SaveGeneration == SaveGeneration.GCN ||
                             SaveFile.SaveGeneration == SaveGeneration.iQue)
                    {
                        acreID.Text = "Acre ID: 0x" + (_selectedAcreId + _acreHeightModifier).ToString("X4");
                    }
                    else
                    {
                        acreID.Text = "Acre ID: 0x" + _selectedAcreId.ToString("X4");
                    }

                    if (_acreInfo != null)
                    {
                        acreDesc.Text = _acreInfo.ContainsKey((byte) _selectedAcreId)
                            ? _acreInfo[(byte) _selectedAcreId]
                            : "No Description";
                    }
                    else if (_uInt16AcreInfo != null)
                    {
                        if ((SaveFile.SaveGeneration == SaveGeneration.N64 ||
                             SaveFile.SaveGeneration == SaveGeneration.GCN ||
                             SaveFile.SaveGeneration == SaveGeneration.iQue) &&
                            _uInt16AcreInfo.ContainsKey((ushort) (_selectedAcreId & 0xFFFC)))
                        {
                            acreDesc.Text = _uInt16AcreInfo[(ushort) (_selectedAcreId & 0xFFFC)];
                        }
                        else if (_uInt16AcreInfo.ContainsKey(_selectedAcreId))
                        {
                            acreDesc.Text = _uInt16AcreInfo[_selectedAcreId];
                        }
                        else
                        {
                            acreDesc.Text = "No Acre Description";
                        }
                    }
                    else
                    {
                        acreDesc.Text = "No Acre Description";
                    }

                    foreach (TreeNode node in acreTreeView.Nodes)
                    {
                        var matchingNodes = node.Nodes.Find(baseAcreStr, true);
                        if (matchingNodes.Length <= 0) continue;
                        node.Toggle();
                        acreTreeView.SelectedNode = matchingNodes[0];
                        acreTreeView.Focus();
                        return;
                    }

                    break;
                }
            }
        }

        private void ImportAcres(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading) return;
            DataUtility.ImportAcres(ref _acres, SaveFile.SaveGeneration);
            SetupMapPictureBoxes();
            // TODO: Refresh Island PictureBoxes for DnM+/AC
        }

        private void ExportAcres(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading)
            {
                DataUtility.ExportAcres(_acres, SaveFile.SaveGeneration, SaveFile.SaveName);
            }
        }

        #endregion

        #region Players

        private void PlayerTabIndexChanged(object sender, TabControlEventArgs e)
        {
            if (_loading || !(sender is TabControl senderTab) || (senderTab.SelectedIndex < 0 || senderTab.SelectedIndex > 3)) return;
            _selectedPlayer = _players[senderTab.SelectedIndex];
            playerEditorSelect.SelectedIndex = senderTab.SelectedIndex;
            patternGroupTabControl.SelectedIndex = senderTab.SelectedIndex;
            if (_selectedPlayer != null && _selectedPlayer.Exists)
                ReloadPlayer(_selectedPlayer);
        }

        private int _lastX, _lastY;
        private void PlayersMouseMove(object sender, MouseEventArgs e)
        {
            if (e.X == _lastX && e.Y == _lastY || SaveFile == null || !(sender is PictureBox box)) return;
            playersToolTip.Hide(this);
            playersToolTip.RemoveAll();
            _lastX = e.X;
            _lastY = e.Y;
            if (box == heldItemPicturebox)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.HeldItem.Name} - [0x{_selectedPlayer.Data.HeldItem.ItemId:X4}]", box,
                    e.X + 15, e.Y + 10);
            }
            else if (box == hatPicturebox && hatPicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.Hat.Name} - [0x{_selectedPlayer.Data.Hat.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
            else if (box == facePicturebox && facePicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.FaceItem.Name} - [0x{_selectedPlayer.Data.FaceItem.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
            else if (box == pantsPicturebox && pantsPicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.Pants.Name} - [0x{_selectedPlayer.Data.Pants.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
            else if (box == socksPicturebox && socksPicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.Socks.Name} - [0x{_selectedPlayer.Data.Socks.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
            else if (box == shoesPicturebox && shoesPicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.Shoes.Name} - [0x{_selectedPlayer.Data.Shoes.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
            else if (box == pocketsBackgroundPicturebox && pocketsBackgroundPicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.InventoryBackground.Name} - [0x{_selectedPlayer.Data.InventoryBackground.ItemId:X4}]",
                    box, e.X + 15, e.Y + 10);
            }
            else if (box == bedPicturebox && bedPicturebox.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.Bed.Name} - [0x{_selectedPlayer.Data.Bed.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
            else if (box == playerWetsuit && playerWetsuit.Enabled)
            {
                playersToolTip.Show(
                    $"{_selectedPlayer.Data.Wetsuit.Name} - [0x{_selectedPlayer.Data.Wetsuit.ItemId:X4}]", box, e.X + 15, e.Y + 10);
            }
        }

        private void HideTip(object sender, EventArgs e)
        {
            playersToolTip.Hide(this);
            _lastX = -1;
            _lastY = -1;
        }

        // TODO: replace this horrible method
        private void PlayersMouseClick(object sender, MouseEventArgs e)
        {
            if (SaveFile == null || !(sender is PictureBox itemBox)) return;
            if (itemBox == heldItemPicturebox)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.HeldItem);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.HeldItem = new Item(GetCurrentItem());
                        heldItemPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.HeldItem, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == hatPicturebox && hatPicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.Hat);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.Hat = new Item(GetCurrentItem());
                        hatPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.Hat, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == facePicturebox && facePicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.FaceItem);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.FaceItem = new Item(GetCurrentItem());
                        facePicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.FaceItem, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == pocketsBackgroundPicturebox && pocketsBackgroundPicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.InventoryBackground);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.InventoryBackground = new Item(GetCurrentItem());
                        pocketsBackgroundPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.InventoryBackground, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == bedPicturebox && bedPicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.Bed);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.Bed = new Item(GetCurrentItem());
                        bedPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.Bed, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == pantsPicturebox && pantsPicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.Pants);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.Pants = new Item(GetCurrentItem());
                        pantsPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.Pants, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == socksPicturebox && socksPicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.Socks);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.Socks = new Item(GetCurrentItem());
                        socksPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.Socks, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == shoesPicturebox && shoesPicturebox.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.Shoes);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.Shoes = new Item(GetCurrentItem());
                        shoesPicturebox.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.Shoes, SaveFile.SaveType);
                        break;
                }
            }
            else if (itemBox == playerWetsuit && playerWetsuit.Enabled)
            {
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        SetCurrentItem(_selectedPlayer.Data.Wetsuit);
                        break;
                    case MouseButtons.Left:
                        SaveFile.ChangesMade = true;
                        _selectedPlayer.Data.Wetsuit = new Item(GetCurrentItem());
                        playerWetsuit.Image = Inventory.GetItemPic(16, _selectedPlayer.Data.Wetsuit, SaveFile.SaveType);
                        break;
                }
            }
        }

        #endregion

        #region Patterns

        private int _lastPatX, _lastPatY;
        private void PatternMove(object sender, MouseEventArgs e)
        {
            if ((e.X == _lastPatX && e.Y == _lastPatY) || SaveFile == null || !_selectedPlayer.Exists ||
                !(sender is PictureBoxWithInterpolationMode box)) return;
            patternToolTip.Hide(this);
            patternToolTip.RemoveAll();
            _lastPatX = e.X;
            _lastPatY = e.Y;
            var idx = Array.IndexOf(_patternBoxes, box);
            if (idx > -1 && idx < _selectedPlayer.Data.Patterns.Length)
                patternToolTip.Show(_selectedPlayer.Data.Patterns[idx].Name, sender as PictureBox, e.X + 15, e.Y + 10);
        }

        private void HidePatTip(object sender, EventArgs e)
        {
            patternToolTip.Hide(this);
            _lastPatX = -1;
            _lastPatY = -1;
        }

        private void PatternExportClick(int idx)
        {
            if (idx <= -1 || _selectedPlayer == null || _selectedPlayer.Data.Patterns.Length <= idx) return;
            var exportPattern = _selectedPlayer.Data.Patterns[idx].PatternBitmap;
            exportPatternFile.FileName = _selectedPlayer.Data.Patterns[idx].Name + ".png";
            if (exportPatternFile.ShowDialog() == DialogResult.OK)
            {
                exportPattern?.Save(exportPatternFile.FileName);
            }
        }

        private void PatternImportClick(int idx)
        {
            if (idx <= -1 || _selectedPlayer == null) return;
            if (importPatternFile.ShowDialog() != DialogResult.OK) return;
            if (!File.Exists(importPatternFile.FileName) ||
                Path.GetExtension(importPatternFile.FileName) != ".png") return;
            var pixelData = ImageGeneration.GetBitmapDataFromPng(importPatternFile.FileName);
            if (pixelData == null) return;
            _selectedPlayer.Data.Patterns[idx].Import(pixelData);
            _selectedPlayer.Data.Patterns[idx].Name = Path.GetFileNameWithoutExtension(importPatternFile.FileName);
            RefreshPictureBoxImage(_patternBoxes[idx], _selectedPlayer.Data.Patterns[idx].PatternBitmap, false, false);

            if (_selectedPatternObject.Index != idx) return;
            _selectedPattern = _selectedPatternObject.PatternBitmap;
            patternNameTextBox.Text = _selectedPatternObject.Name;
            patternEditorPictureBox.Image = ImageGeneration.DrawGrid2(_selectedPattern, 0x10, new Size(513, 513));
        }

        // TODO: Allow New Leaf / Welcome Amiibo to change their color palette somehow
        private void ChangePatternPalette(int additive)
        {
            if (_selectedPatternObject == null || SaveFile == null ||
                SaveFile.SaveGeneration == SaveGeneration.N3DS) return;
            if (_selectedPatternObject.Palette + additive < 0)
                _selectedPatternObject.Palette = 15;
            else if (_selectedPatternObject.Palette + additive > 15)
                _selectedPatternObject.Palette = 0;
            else
                _selectedPatternObject.Palette += (byte)additive;

            if (SaveFile.SaveGeneration != SaveGeneration.N3DS)
                _selectedPatternObject.PaletteData = _selectedPatternObject.GetPaletteArray(SaveFile.SaveGeneration)[_selectedPatternObject.Palette];
            paletteIndexLabel.Text = "Palette: " + (_selectedPatternObject.Palette + 1);
            paletteSelectionPictureBox.Image = PatternUtility.GeneratePalettePreview(_selectedPatternObject.PaletteData, _selectedPaletteIndex,
                (uint)paletteSelectionPictureBox.Size.Width, (uint)paletteSelectionPictureBox.Size.Height);
            _selectedPatternObject.RedrawBitmap();
            _selectedPattern = _selectedPatternObject.PatternBitmap;
            patternEditorPictureBox.Image = ImageGeneration.DrawGrid2(_selectedPattern, 0x10, new Size(513, 513));
            _patternBoxes[_selectedPatternObject.Index].Image = _selectedPattern;
            patternEditorPictureBox.Refresh();
        }

        //Add ContextMenuStrips for importing/exporting patterns & renaming/setting palette
        private void SetupPatternBoxes()
        {
            paletteNextButton.Visible = SaveFile.SaveGeneration != SaveGeneration.N3DS;
            palettePreviousButton.Visible = SaveFile.SaveGeneration != SaveGeneration.N3DS;

            for (var i = patternEditorPreviewPanel.Controls.Count - 1; i > -1; i--)
                if (patternEditorPreviewPanel.Controls[i] is PictureBoxWithInterpolationMode disposingBox)
                {
                    var oldImage = disposingBox.Image;
                    disposingBox.Dispose();
                    oldImage?.Dispose();
                }
            _patternBoxes = new PictureBoxWithInterpolationMode[CurrentSaveInfo.PatternCount];
            for (var i = 0; i < CurrentSaveInfo.PatternCount; i++)
            {
                var patternStrip = new ContextMenuStrip();
                var export = new ToolStripMenuItem()
                {
                    Name = "export",
                    Text = "Export"
                };

                var import = new ToolStripMenuItem()
                {
                    Name = "import",
                    Text = "Import"
                };

                patternStrip.Items.Add(import);
                patternStrip.Items.Add(export);

                var patternBox = new PictureBoxWithInterpolationMode()
                {
                    InterpolationMode = InterpolationMode.NearestNeighbor,
                    Name = "pattern" + i,
                    Size = new Size(64, 64),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackgroundImageLayout = ImageLayout.Stretch,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point((i % 2) * 72, 3 + (i / 2) * 72),
                    ContextMenuStrip = patternStrip,
                };

                patternBox.MouseMove += PatternMove;
                patternBox.MouseLeave += HidePatTip;
                _patternBoxes[i] = patternBox;
                patternEditorPreviewPanel.Controls.Add(patternBox);

                // ToolStrip Item Events
                export.Click += (sender, e) => PatternExportClick(Array.IndexOf(_patternBoxes, patternBox));
                import.Click += (sender, e) => PatternImportClick(Array.IndexOf(_patternBoxes, patternBox));

                patternBox.MouseClick += delegate
                {
                    _selectedPatternObject = _selectedPlayer.Data.Patterns[Array.IndexOf(_patternBoxes, patternBox)];
                    _selectedPattern = _selectedPatternObject.PatternBitmap;
                    paletteSelectionPictureBox.Image = PatternUtility.GeneratePalettePreview(
                        _selectedPatternObject.PaletteData, _selectedPaletteIndex,
                        (uint) paletteSelectionPictureBox.Size.Width, (uint) paletteSelectionPictureBox.Size.Height);
                    patternEditorPictureBox.Image = ImageGeneration.DrawGrid2(_selectedPattern, 0x10, new Size(513, 513));
                    patternNameTextBox.Text = _selectedPatternObject.Name;
                    paletteIndexLabel.Text = "Palette: " + (_selectedPatternObject.Palette + 1);
                };
            }

            patternEditorPictureBox.Image = null;
            patternEditorPictureBox.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        private void PaletteImageBoxClick(object sender, MouseEventArgs e)
        {
            if (_selectedPatternObject == null) return;
            _selectedPaletteIndex = e.Y / (paletteSelectionPictureBox.Height / 15);
            paletteSelectionPictureBox.Image = PatternUtility.GeneratePalettePreview(_selectedPatternObject.PaletteData,
                _selectedPaletteIndex,
                (uint) paletteSelectionPictureBox.Size.Width, (uint) paletteSelectionPictureBox.Size.Height);

            // Set Color Selected Arrow
            paletteColorSelectedPictureBox.Location = new Point(paletteSelectionPictureBox.Location.X - 16,
                paletteSelectionPictureBox.Location.Y + _selectedPaletteIndex * 32);
        }

        private bool _patternEditorMouseDown;
        private int _lastPatternPixel = -1;

        private void PatternEditorBoxClick(MouseEventArgs e)
        {
            if (_selectedPatternObject == null) return;
            var cellX = e.X / (patternEditorPictureBox.Width / 32);
            var cellY = e.Y / (patternEditorPictureBox.Height / 32);
            var pixelPosition = cellY * 32 + cellX;

            if (pixelPosition <= -1 || pixelPosition >= _selectedPatternObject.DecodedData.Length ||
                cellX >= 32) return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (_lastPatternPixel == pixelPosition) return;
                    _lastPatternPixel = pixelPosition;
                    _selectedPatternObject.DecodedData[pixelPosition] = SaveFile.SaveGeneration == SaveGeneration.N3DS
                        ? (byte) _selectedPaletteIndex
                        : (byte) (_selectedPaletteIndex + 1);
                    _selectedPatternObject.RedrawBitmap();
                    _selectedPattern = _selectedPatternObject.PatternBitmap;
                    _patternBoxes[_selectedPatternObject.Index].Image = _selectedPattern;
                    patternEditorPictureBox.Image =
                        ImageGeneration.DrawGrid2(_selectedPattern, 0x10, new Size(513, 513));
                    patternEditorPictureBox.Refresh();
                    return;
                case MouseButtons.Right:
                    var prevIndex = _selectedPaletteIndex;
                    _selectedPaletteIndex = SaveFile.SaveGeneration == SaveGeneration.N3DS
                        ? _selectedPatternObject.DecodedData[pixelPosition]
                        : Math.Max(0, _selectedPatternObject.DecodedData[pixelPosition] - 1);
                    if (_selectedPaletteIndex == prevIndex) return;

                    paletteColorSelectedPictureBox.Location = new Point(paletteSelectionPictureBox.Location.X - 16,
                        paletteSelectionPictureBox.Location.Y + _selectedPaletteIndex * 32);
                    return;
                case MouseButtons.Middle:
                    // TODO: Implement flood fill
                    return;
            }
        }

        private void PatternEditorBoxMouseLeave(object sender, EventArgs e)
        {
            _patternEditorMouseDown = false;
            _lastPatternPixel = -1;
        }

        private void PatternEditorBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (_selectedPatternObject == null) return;
            _patternEditorMouseDown = true;
            PatternEditorBoxClick(e);
        }

        private void PatternEditorBoxMouseUp(object sender, MouseEventArgs e)
        {
            _patternEditorMouseDown = false;
            _lastPatternPixel = -1;
        }

        private void PatternEditorBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (_patternEditorMouseDown && _selectedPatternObject != null)
            {
                PatternEditorBoxClick(e);
            }
        }

        private void PatternEditorNameBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedPatternObject != null)
            {
                _selectedPatternObject.Name = patternNameTextBox.Text;
            }
        }

        #endregion

        #region Town

        private void BuildingListIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is ComboBox senderBox) || !(senderBox.Parent is Panel buildingPanel)) return;
            var buildingIdx = Array.IndexOf(_buildingListPanels, buildingPanel);
            var editedBuilding = _buildings[buildingIdx];
            editedBuilding.Id = _buildingDb[Array.IndexOf(_buildingNames, senderBox.Text)];
            editedBuilding.Name = senderBox.Text;

            editedBuilding.Exists = SaveFile.SaveType == SaveType.NewLeaf
                ? editedBuilding.Id != 0xF8
                : editedBuilding.Id != 0xFC;

            _townAcreMap[editedBuilding.AcreIndex].Image =
                GenerateAcreItemsBitmap(TownAcres[editedBuilding.AcreIndex]);
        }

        //TODO: Update textboxes on change with mouse
        private void BuildingPositionChanged(object sender, bool isY = false)
        {
            if (!(sender is TextBox positionBox) || !(positionBox.Parent is Panel parentPanel)) return;
            if (!byte.TryParse(positionBox.Text, out var newPosition)) return;
            var buildingIdx = Array.IndexOf(_buildingListPanels, parentPanel);
            var editedBuilding = _buildings[buildingIdx];
            if ((SaveFile.SaveType == SaveType.CityFolk && newPosition < 16 * 5) ||
                (SaveFile.SaveGeneration == SaveGeneration.N3DS && newPosition < 16 * (isY ? 4 : 5)))
            {
                int oldAcre = editedBuilding.AcreIndex;
                var newAcre = newPosition / 16;
                if (!isY)
                {
                    editedBuilding.AcreX = (byte)(newAcre + 1);
                    editedBuilding.XPos = (byte)(newPosition % 16);
                }
                else
                {
                    editedBuilding.AcreY = (byte)(newAcre + 1);
                    editedBuilding.YPos = (byte)(newPosition % 16);
                }
                editedBuilding.AcreIndex = (byte)((editedBuilding.AcreY - 1) * 5 + (editedBuilding.AcreX - 1));
                _townAcreMap[oldAcre].Image = GenerateAcreItemsBitmap(TownAcres[oldAcre]);
                if (oldAcre != newAcre)
                    _townAcreMap[newAcre].Image = GenerateAcreItemsBitmap(TownAcres[newAcre]);
            }
            else //Return text to original position
            {
                positionBox.Text = isY ? (editedBuilding.YPos + (editedBuilding.AcreY - 1) * 16).ToString()
                    : (editedBuilding.XPos + (editedBuilding.AcreX - 1) * 16).ToString();
            }
        }

        private void UpdateBuildingPositionBoxes(Building editedBuilding)
        {
            var buildingPanel = _buildingListPanels[Array.IndexOf(_buildings, editedBuilding)];
            buildingPanel.Controls[1].Text = (editedBuilding.XPos + (editedBuilding.AcreX - 1) * 16).ToString();
            buildingPanel.Controls[2].Text = (editedBuilding.YPos + (editedBuilding.AcreY - 1) * 16).ToString();
        }

        private void SetupBuildingList()
        {
            switch (SaveFile.SaveType)
            {
                case SaveType.NewLeaf:
                    _buildingDb = Building.NewLeafBuildingNames.Keys.ToArray();
                    _buildingNames = Building.NewLeafBuildingNames.Values.ToArray();
                    break;
                case SaveType.WelcomeAmiibo:
                    _buildingDb = Building.WelcomeAmiiboBuildingNames.Keys.ToArray();
                    _buildingNames = Building.WelcomeAmiiboBuildingNames.Values.ToArray();
                    break;
            }

            buildingsPanel.SuspendLayout();
            _buildingListPanels = new Panel[_buildings.Length];

            for (var i = 0; i < _buildings.Length; i++)
            {
                var buildingPanel = new Panel
                {
                    Name = "Panel_" + i,
                    Size = new Size(180, 22),
                    Location = new Point(0, i * 25)
                };

                if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    var buildingListBox = new ComboBox
                    {
                        Size = new Size(120, 22),
                        DropDownWidth = 200
                    };
                    buildingListBox.Items.AddRange(_buildingNames);
                    buildingPanel.Controls.Add(buildingListBox);
                    buildingListBox.SelectedIndex = Array.IndexOf(_buildingDb, _buildings[i].Id);
                    buildingListBox.SelectedIndexChanged += BuildingListIndexChanged;
                }
                else
                {
                    var buildingListLabel = new Label
                    {
                        Size = new Size(120, 22),
                        Text = _buildings[i].Name,
                    };
                    buildingPanel.Controls.Add(buildingListLabel);
                }
                var xLocation = new TextBox
                {
                    Size = new Size(22, 22),
                    Location = new Point(130, 0),
                    Text = Math.Max(0, _buildings[i].XPos + (_buildings[i].AcreX - 1) * 16).ToString(),
                    Name = "X_Position"
                };
                var yLocation = new TextBox
                {
                    Size = new Size(22, 22),
                    Location = new Point(154, 0),
                    Text = Math.Max(0, _buildings[i].YPos + (_buildings[i].AcreY - 1) * 16).ToString(),
                    Name = "Y_Position"
                };
                xLocation.LostFocus += (sender, e) => BuildingPositionChanged(sender);
                yLocation.LostFocus += (sender, e) => BuildingPositionChanged(sender, true);
                buildingPanel.Controls.Add(xLocation);
                buildingPanel.Controls.Add(yLocation);
                _buildingListPanels[i] = buildingPanel;
            }

            buildingsPanel.Controls.AddRange(_buildingListPanels);
            buildingsPanel.ResumeLayout();
        }

        private void UpdateBuildingCount()
        {
            if (SaveFile == null || _loading || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            var count = (byte)(ItemData.GetBuildingCount(_buildings, SaveFile.SaveType) + 0); // Sometimes is count + 1, sometimes is count - 1??
            Console.WriteLine("Set building count to: " + count);
            SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.Buildings - 4, count);
        }

        private void TownEnter(object sender)
        {
            if (!(sender is PictureBoxWithInterpolationMode)) return;
            _lastTownIndex = -1;
        }

        private void HandleTownClick(object sender, Item item, WorldAcre acre, int index, MouseEventArgs e, bool island = false)
        {
            if (!(sender is PictureBoxWithInterpolationMode box)) return;
            Building b;
            switch (e.Button)
            {
                case MouseButtons.Left:
                {
                    SaveFile.ChangesMade = true;
                    box.Capture = false;
                    if (_selectedBuilding > -1)
                    {
                        b = island ? _islandBuildings[_selectedBuilding] : _buildings[_selectedBuilding];
                        int oldAcre = b.AcreIndex;
                        var adjustedAcre = island ? acre.Index - 5 : acre.Index;
                        if (CheckBuildingIsHere(adjustedAcre, index % 16, index / 16, island) != null)
                            return; //Don't place buildings on top of each other
                        b.AcreIndex = (byte)adjustedAcre;

                        b.AcreX = island
                            ? (byte) ((adjustedAcre % 2) + 1)
                            : (byte) (adjustedAcre % (CurrentSaveInfo.XAcreCount - 2) + 1); //Might have to change for NL

                        b.AcreY = island
                            ? (byte) ((adjustedAcre / 2) + 1)
                            : (byte) (adjustedAcre / (CurrentSaveInfo.XAcreCount - 2) + 1);

                        b.XPos = (byte)(index % 16);
                        b.YPos = (byte)(index / 16);

                        //These two items has "actor" items at their location
                        if (b.Name != "Sign" && b.Name != "Bus Stop")
                        {
                            acre.Items[index] = new Item();
                        }
                        else
                        {
                            acre.Items[index] = new Item(b.Name == "Sign" ? (ushort) 0xD000 : (ushort) 0x7003);
                        }

                        if ((!island && oldAcre != acre.Index) || (island && oldAcre != adjustedAcre))
                        {
                            var oldImage = island ? _islandAcreMap[oldAcre + 5].Image : _townAcreMap[oldAcre].Image;
                            if (island)
                            {
                                _islandAcreMap[oldAcre + 5].Image =
                                    GenerateAcreItemsBitmap(IslandAcres[oldAcre + 5], true);
                                _islandAcreMap[oldAcre + 5].Refresh();
                            }
                            else
                            {
                                _townAcreMap[oldAcre].Image = GenerateAcreItemsBitmap(TownAcres[oldAcre]);
                                _townAcreMap[oldAcre].Refresh();
                            }
                            oldImage?.Dispose();
                        }

                        if (!island) //TODO: Add Island Building Panel
                        {
                            UpdateBuildingPositionBoxes(b);
                        }

                        _selectedBuilding = -1;
                        selectedItem.SelectedValue = _lastSelectedItem;
                        selectedItem.Enabled = true;
                    }
                    else
                    {
                        if (item.ItemId == _currentItem.ItemId) return;

                        if (itemFlag1.Enabled)
                        {
                            var newItem = new Item(_currentItem.ItemId);
                            /*byte.TryParse(itemFlag1.Text, NumberStyles.AllowHexSpecifier, null, out newItem.Flag1);
                            byte.TryParse(itemFlag2.Text, NumberStyles.AllowHexSpecifier, null, out newItem.Flag2);
                            switch (newItem.Flag1)
                            {
                                case 0x40:
                                    newItem.Buried = false;
                                    newItem.Watered = true;
                                    break;
                                case 0x80:
                                    newItem.Watered = false;
                                    newItem.Buried = true;
                                    break;
                            }*/
                            acre.Items[index] = newItem;
                        }
                        else
                        {
                            acre.Items[index] = new Item(_currentItem.ItemId);

                            Villager villager;
                            switch (SaveFile.SaveGeneration)
                            {
                                // Update Villager House Coordinates if a valid villager exists for the selected house
                                case SaveGeneration.N64 when _currentItem.ItemId >= 0x5000 && _currentItem.ItemId <= 0x50FF:
                                case SaveGeneration.GCN when _currentItem.ItemId >= 0x5000 && _currentItem.ItemId <= 0x50FF:
                                case SaveGeneration.iQue when _currentItem.ItemId >= 0x5000 && _currentItem.ItemId <= 0x50FF:
                                    villager = Utility.GetVillagerFromHouse(_currentItem.ItemId, _villagers);
                                    if (villager != null)
                                    {
                                        var houseCoordinatesInfo =
                                            Utility.FindVillagerHouse(villager.Data.VillagerId, TownAcres);
                                        villager.Data.HouseCoordinates = houseCoordinatesInfo.Item1;
                                    }
                                    break;
                                case SaveGeneration.NDS when _currentItem.ItemId >= 0x5001 && _currentItem.ItemId <= 0x5008:
                                    villager = _villagers[(_currentItem.ItemId - 1) % 8];
                                    var (houseCoordinates, found) =
                                        Utility.FindVillagerHouseWildWorld(villager.Index, TownAcres);
                                    villager.Data.HouseCoordinates = houseCoordinates;
                                    break;
                            }
                        }

                        if (buriedCheckbox.Checked)
                        {
                            switch (SaveFile.SaveGeneration)
                            {
                                case SaveGeneration.N3DS:
                                    if (acre.Items[index].ItemId != 0x7FFE)
                                    {
                                        acre.Items[index].Flag1 = 0x80;
                                        //TownAcres[acre].Items[index].Buried = true;
                                    }

                                    break;
                                default:
                                    if (island && _selectedIsland != null)
                                    {
                                            // TODO: Island buried items
                                    }
                                    else
                                    {
                                        //TownAcres[acre].Items[index].Buried =
                                        acre.SetItemBuried(acre.Items[index], true, SaveFile.SaveGeneration);
                                    }

                                    break;
                            }
                        }
                    }

                    var img = box.Image;
                    RefreshPictureBoxImage(box, GenerateAcreItemsBitmap(acre, island));
                    box.Refresh();
                    img?.Dispose();
                    TownMove(sender, e, island, true); // Force ToolTip update
                    break;
                }
                case MouseButtons.Right:
                    b = CheckBuildingIsHere(acre.Index, index % 16, index / 16, island);
                    if (b != null)
                    {
                        if (_selectedBuilding == -1)
                            _lastSelectedItem = _currentItem.ItemId;
                        selectedItem.SelectedValue = (ushort)0xFFFF;
                        _selectedBuilding = island ? Array.IndexOf(_islandBuildings, b) : Array.IndexOf(_buildings, b);
                        selectedItem.Enabled = false;
                        selectedItem.Text = b.Name;
                        selectedItemText.Text = $"Building: [{b.Name}]";
                        itemIdTextBox.Visible = false;
                        itemIdLabel.Visible = false;
                    }
                    else
                    {
                        selectedItem.Enabled = true;
                        var oldSelectedItemId = _selectedBuilding > -1 ? _lastSelectedItem : _currentItem.ItemId;
                        _selectedBuilding = -1;
                        selectedItem.SelectedValue = item.ItemId;
                        if (selectedItem.SelectedValue == null)
                            selectedItem.SelectedValue = oldSelectedItemId;
                        else
                        {
                            buriedCheckbox.Checked = acre.IsItemBuried(item) || item.Flag1 == 0x80;
                            if (itemFlag1.Enabled)
                            {
                                itemFlag1.Text = item.Flag1.ToString("X2");
                                itemFlag2.Text = item.Flag2.ToString("X2");
                            }
                        }
                        itemIdTextBox.Visible = true;
                        itemIdLabel.Visible = true;
                        selectedItemText.Text = "Selected Item"; //string.Format("Selected Item: [0x{0}]", (GetCurrentItem()).ToString("X4"));
                    }
                    selectedItem.Refresh();
                    break;
                case MouseButtons.Middle:
                {
                    box.Capture = false;
                    Utility.FloodFillItemArray(ref acre.Items, 16, index, acre.Items[index],
                        new Item(_currentItem.ItemId, byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text)));
                    RefreshPictureBoxImage(box, GenerateAcreItemsBitmap(acre, island));
                    break;
                }
            }
        }

        private int _lastTownAcre, _lastTownIndex;
        private void TownMove(object sender, MouseEventArgs e, bool island = false, bool forceOverride = false)
        {
            if (SaveFile == null || !(sender is PictureBoxWithInterpolationMode box)) return;
            var x = e.X / _townMapCellSize;
            var y = e.Y / _townMapCellSize;
            var index = x + y * 16;
            var acre = (int) box.Tag;

            if (index < 0 || index > 255 || TownAcres == null || acre >= TownAcres.Length) return;
            if (index == _lastTownIndex && !forceOverride) return;

            _lastTownIndex = index;
            box.Capture = false;
            townToolTip.Hide(box);
            townToolTip.RemoveAll();

            // Set Info Label
            townInfoLabel.Text = $"X: {x} | Y: {y} | Index: {index}";
            townInfoLabel.Refresh();

            Item item;
            WorldAcre wAcre;
            if (island)
            {
                wAcre = _selectedIsland == null ? IslandAcres[acre] : _selectedIsland.Acres[acre];
                item = _selectedIsland == null ? IslandAcres[acre].Items[index] : _selectedIsland.Acres[acre].Items[index];

                // TODO: This doesn't handle the ocean acres to the left & right.
                if (SaveFile.SaveGeneration == SaveGeneration.N3DS && _lastTownAcre < 5 || _lastTownAcre > 10)
                {
                    _lastTownAcre = 5;
                }
            }
            else
            {
                wAcre = TownAcres[acre];
                item = TownAcres[acre].Items[index];
            }

            if (_clicking && !forceOverride)
            {
                HandleTownClick(sender, item, wAcre, index, e, island);
            }

            //Console.WriteLine($"Index: {index} | ItemId: {item.ItemId:X4} | Name: {item.Name}");

            // Draw ToolTip
            if (_buildings != null)
            {
                var b = CheckBuildingIsHere(acre, x, y, island);
                townToolTip.Show(
                    b != null
                        ? $"{b.Name} - [0x{b.Id:X2} - Building]"
                        : $"{item.Name}{(wAcre.IsItemBuried(item) ? " (Buried)" : ((item.Flag1 & 0x40) != 0 ? " (Watered)" : (item.Flag1 == 1 ? " (Perfect Fruit)" : "")))} - [0x{item.ItemId:X4}]",
                    box, e.X + 15, e.Y + 10, 100000);
            }
            else
            {
                townToolTip.Show($"{item.Name}{(wAcre.IsItemBuried(item) ? " (Buried)" : "")} - [0x{item.ItemId:X4}]", box, e.X + 15,
                    e.Y + 10, 100000);
            }
            
            // Draw Cell Highlight
            RefreshPictureBoxImage(box, GenerateAcreItemsBitmap(wAcre, island, acre, x, y));
            box.Refresh();

            if (acre == _lastTownAcre) return;
            if (island && _lastTownAcre >= _islandAcreMap.Length)
            {
                _lastTownAcre = 0;
                return;
            }

            if (_lastTownAcre < 0)
            {
                _lastTownAcre = 0;
            }

            WorldAcre lastWorldAcre;
            if (island)
            {
                lastWorldAcre = _selectedIsland == null ? IslandAcres[_lastTownAcre] : _selectedIsland.Acres[_lastTownAcre];
            }
            else
            {
                lastWorldAcre = TownAcres[_lastTownAcre];
            }

            RefreshPictureBoxImage(island ? _islandAcreMap[_lastTownAcre] : _townAcreMap[_lastTownAcre],
                GenerateAcreItemsBitmap(lastWorldAcre, island));
            if (island)
            {
                _islandAcreMap[_lastTownAcre].Refresh();
            }
            else
            {
                _townAcreMap[_lastTownAcre].Refresh();
            }
            _lastTownAcre = acre;
        }

        private void HideTownTip(object sender, EventArgs e)
        {
            townToolTip.Hide(this);
        }

        private void TownMouseDown(object sender, MouseEventArgs e, bool island = false)
        {
            if (!(sender is PictureBoxWithInterpolationMode box)) return;
            var acre = island ? Array.IndexOf(_islandAcreMap, box) : Array.IndexOf(_townAcreMap, box);
            var x = e.X / _townMapCellSize;
            var y = e.Y / _townMapCellSize;
            var index = x + y * 16;
            if (index > 255) return;

            WorldAcre wAcre;
            if (island)
            {
                wAcre = _selectedIsland == null ? IslandAcres[acre] : _selectedIsland.Acres[acre];
            }
            else
            {
                wAcre = TownAcres[acre];
            }

            HandleTownClick(sender, wAcre.Items[index], wAcre, index, e, island);
            _clicking = true;
        }

        private void TownMouseUp(object sender, EventArgs e)
        {
            _clicking = false;
        }

        private Building CheckBuildingIsHere(int acre, int x, int y, bool island = false)
        {
            if (!island)
                return _buildings?.FirstOrDefault(b => b.Exists && b.AcreIndex == acre && b.XPos == x && b.YPos == y);
            acre = acre - 5;
            return _islandBuildings?.FirstOrDefault(b => b.Exists && b.AcreIndex == acre && b.XPos == x && b.YPos == y);
        }

        #endregion

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile == null) return;
            //Save Players
            foreach (var player in _players)
            {
                if (player != null && player.Exists)
                {
                    player.Write();
                }
            }

            //Save Villagers
            if (_villagers != null && SaveFile.SaveGeneration != SaveGeneration.Wii)
            {
                foreach (var villager in _villagers)
                {
                    if (villager == null) continue;
                    switch (SaveFile.SaveGeneration)
                    {
                        case SaveGeneration.N64:
                        case SaveGeneration.GCN:
                        case SaveGeneration.iQue:
                            // TODO: Update islander house location. In e+, update all four islander house locations.
                            if (SaveFile.SaveGeneration != SaveGeneration.GCN || villager.Index < 15)
                            {
                                var houseCoordinatesInfo =
                                    Utility.FindVillagerHouse(villager.Data.VillagerId, TownAcres);
                                villager.Data.HouseCoordinates = houseCoordinatesInfo.Item1;
                            }

                            break;
                        case SaveGeneration.NDS:
                            var (houseCoordinates, found) =
                                Utility.FindVillagerHouseWildWorld(villager.Index, TownAcres);
                            villager.Data.HouseCoordinates = houseCoordinates;
                            break;
                    }

                    villager.Write(townNameBox.Text.Length < 1 ? null : townNameBox.Text);
                }
                    
                // Update Villager Count in N64/GCN (Possibly others too?)
                if (SaveFile.SaveType == SaveType.AnimalCrossing) // TODO: Others
                {
                    var villagerCount = _villagers.Count(v => v.Exists && v.Index < 15);
                    SaveFile.Write(SaveFile.SaveDataStartOffset + 0x18, (byte)villagerCount);
                }
            }

            // Save Houses
            foreach (var house in _houses)
            {
                house?.Write();
            }

            //Save Acre & Town Data
            for (var i = 0; i < _acres.Length; i++)
            {
                if (SaveFile.SaveGeneration == SaveGeneration.N64 || SaveFile.SaveGeneration == SaveGeneration.GCN ||
                    SaveFile.SaveGeneration == SaveGeneration.iQue || SaveFile.SaveType == SaveType.CityFolk)
                {
                    SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData + i * 2, _acres[i].AcreId, true);
                }
                else if (SaveFile.SaveType == SaveType.WildWorld)
                {
                    SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData + i,
                        Convert.ToByte(_acres[i].AcreId), SaveFile.IsBigEndian);
                }
                else if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.AcreData + i * 2, _acres[i].AcreId);
                }
            }

            if (CurrentSaveInfo.SaveOffsets.TownData != 0)
            {
                for (var i = 0; i < TownAcres.Length; i++)
                {
                    for (var x = 0; x < 256; x++)
                    {
                        if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                        {
                            SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TownData + i * 1024 + x * 4,
                                ItemData.EncodeItem(TownAcres[i].Items[x]));
                        }
                        else
                        {
                            SaveFile.Write(
                                SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.TownData + i * 512 + x * 2,
                                TownAcres[i].Items[x].ItemId,
                                SaveFile.IsBigEndian);
                        }
                    }
                }
            }

            if (CurrentSaveInfo.SaveOffsets.GrassWear > 0)
            {
                SaveFile.Write(SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.GrassWear, _grassWear);
            }

            if (CurrentSaveInfo.SaveOffsets.Buildings > 0)
            {
                if (SaveFile.SaveType == SaveType.CityFolk)
                {
                    for (var i = 0; i < _buildings.Length; i++)
                    {
                        if (i < 33)
                        {
                            var dataOffset = SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.Buildings + i * 2;
                            byte x = (byte) (((_buildings[i].AcreX << 4) & 0xF0) + (_buildings[i].XPos & 0x0F)),
                                y = (byte) (((_buildings[i].AcreY << 4) & 0xF0) + (_buildings[i].YPos & 0x0F));
                            SaveFile.Write(dataOffset, x);
                            SaveFile.Write(dataOffset + 1, y);
                        }
                        else if (_buildings[i].Id == 33) //Pave's Sign
                        {
                            var dataOffset = SaveFile.SaveDataStartOffset + 0x5EB90;
                            byte x = (byte) (((_buildings[i].AcreX << 4) & 0xF0) + (_buildings[i].XPos & 0x0F)),
                                y = (byte) (((_buildings[i].AcreY << 4) & 0xF0) + (_buildings[i].YPos & 0x0F));
                            SaveFile.Write(dataOffset, x);
                            SaveFile.Write(dataOffset + 1, y);
                        }
                        else if (_buildings[i].Id == 34) //Bus Stop
                        {
                            var dataOffset = SaveFile.SaveDataStartOffset + 0x5EB8A;
                            byte x = (byte) (((_buildings[i].AcreX << 4) & 0xF0) + (_buildings[i].XPos & 0x0F)),
                                y = (byte) (((_buildings[i].AcreY << 4) & 0xF0) + (_buildings[i].YPos & 0x0F));
                            SaveFile.Write(dataOffset, x);
                            SaveFile.Write(dataOffset + 1, y);
                        }
                        else if (i >= 35) //Signs
                        {
                            var dataOffset = SaveFile.SaveDataStartOffset + 0x5EB92 + (i - 35) * 2;
                            byte x = (byte) (((_buildings[i].AcreX << 4) & 0xF0) + (_buildings[i].XPos & 0x0F)),
                                y = (byte) (((_buildings[i].AcreY << 4) & 0xF0) + (_buildings[i].YPos & 0x0F));
                            SaveFile.Write(dataOffset, x);
                            SaveFile.Write(dataOffset + 1, y);
                        }
                    }
                }
                else if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    for (var i = 0; i < _buildings.Length; i++)
                    {
                        var dataOffset = SaveFile.SaveDataStartOffset + CurrentSaveInfo.SaveOffsets.Buildings + i * 4;
                        byte x = (byte) (((_buildings[i].AcreX << 4) & 0xF0) + (_buildings[i].XPos & 0x0F)),
                            y = (byte) (((_buildings[i].AcreY << 4) & 0xF0) + (_buildings[i].YPos & 0x0F));
                        SaveFile.Write(dataOffset, new byte[] { _buildings[i].Id, 0x00, x, y });
                    }
                }
            }

            // Update Building Count in New Leaf
            UpdateBuildingCount();

            // Save Town Ordinances in New Leaf
            UpdateNewLeafOrdinances();

            // Save DnM+/AC Island Cabana
            if (SaveFile.SaveType == SaveType.DoubutsuNoMoriPlus || SaveFile.SaveType == SaveType.AnimalCrossing)
            {
                _islandCabana.Data.Rooms[0].Write();
            }

            // Save DnMe+ Islands   
            if (_islands != null)
            {
                foreach (var isle in _islands)
                {
                    isle.Write();
                }
            }

            // Update Checksums and save file
            SaveFile.Flush();
        }

        private static bool ConfirmSave(string confirmationString)
        {
            if (SaveFile == null) return true;
            if (SaveFile.ChangesMade) // TODO: Changes aren't updated when placing items, etc. Must change that.
            {
                var result = MessageBox.Show(confirmationString, "Save File", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Cancel) return false;
                SaveFile.Close(result == DialogResult.Yes);
                return true;
            }

            SaveFile.Close(false);
            return true;
        }

        private async void OpenSave(string saveFileLocation)
        {
            if (File.Exists(saveFileLocation))
            {
                if (ConfirmSave("A save file is already being edited. Would you like to save your changes before opening another file?"))
                {
                    await SetupEditor(new Save(saveFileLocation, Properties.Settings.Default.BackupFiles));
                }
            }
            else
            {
                MessageBox.Show("The save file doesn't exist! Nothing was changed.", "Save File Loading Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile != null)
            {
                openSaveFile.FileName = SaveFile.SaveName + SaveFile.SaveExtension;
            }
            else
            {
                openSaveFile.FileName = "";
            }
            if (openSaveFile.ShowDialog() == DialogResult.OK)
            {
                OpenSave(openSaveFile.FileName);
            }
        }

        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files[0] != null)
            {
                OpenSave(files[0]);
            }
        }

        private void ClearWeedsButtonClick()
        {
            var weedsCleared = 0;
            foreach (var box in _townAcreMap)
            {
                var idx = Array.IndexOf(_townAcreMap, box);
                var acreIdx = idx;
                var acre = TownAcres[acreIdx];
                if (acre.Items == null) continue;
                for (var i = 0; i < 256; i++)
                {
                    if (ItemData.GetItemType(acre.Items[i].ItemId, SaveFile.SaveType) != ItemType.Weed) continue; // Weed
                    switch (SaveFile.SaveGeneration)
                    {
                        case SaveGeneration.NDS:
                        case SaveGeneration.Wii:
                            acre.Items[i] = new Item(0xFFF1);
                            break;
                        case SaveGeneration.N3DS:
                            acre.Items[i] = new Item(0x7FFE);
                            break;
                        default:
                            acre.Items[i] = new Item(0);
                            break;
                    }
                    weedsCleared++;
                }
                RefreshPictureBoxImage(box, GenerateAcreItemsBitmap(acre));
            }
            MessageBox.Show($"{weedsCleared} weeds {(weedsCleared == 1 ? "was" : "were")} removed!",
                "Weeds Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_aboutBox == null || _aboutBox.IsDisposed)
                _aboutBox = new AboutBox();
            _aboutBox.Show();
        }

        private void TanTrackbarScroll(object sender, EventArgs e)
        {
            if (SaveFile != null && _selectedPlayer != null && _selectedPlayer.Exists)
                _selectedPlayer.Data.Tan = (byte)(tanTrackbar.Value - 1);
        }

        private void RemoveGrassClick(object sender, EventArgs e)
        {
            if (SaveFile == null) return;
            Array.Clear(_grassWear, 0, _grassWear.Length);
            if (SaveFile.SaveType == SaveType.CityFolk)
                for (var i = 0; i < _grassMap.Length; i++)
                    _grassMap[i].Image = ImageGeneration.DrawGrassWear(_grassWear.Skip(i * 256).Take(256).ToArray());
            else if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                _newLeafGrassOverlay.Image = ImageGeneration.DrawGrassWear(_grassWear);
        }

        private void ReviveGrassClick(object sender, EventArgs e)
        {
            if (SaveFile == null) return;
            for (var i = 0; i < _grassWear.Length; i++)
                _grassWear[i] = 0xFF;
            if (SaveFile.SaveType == SaveType.CityFolk)
                for (var i = 0; i < _grassMap.Length; i++)
                    _grassMap[i].Image = ImageGeneration.DrawGrassWear(_grassWear.Skip(i * 256).Take(256).ToArray());
            else if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                _newLeafGrassOverlay.Image = ImageGeneration.DrawGrassWear(_grassWear);
        }

        private void SetAllGrassClick(object sender, EventArgs e)
        {
            if (SaveFile == null) return;
            byte.TryParse(grassLevelBox.Text, out var setValue);
            for (var i = 0; i < _grassWear.Length; i++)
                _grassWear[i] = setValue;
            if (SaveFile.SaveType == SaveType.CityFolk)
                for (var i = 0; i < _grassMap.Length; i++)
                    _grassMap[i].Image = ImageGeneration.DrawGrassWear(_grassWear.Skip(i * 256).Take(256).ToArray());
            else if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                _newLeafGrassOverlay.Image = ImageGeneration.DrawGrassWear(_grassWear);
        }

        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile == null) return;
            var filterString =
                $"{SaveDataManager.GetGameTitle(SaveFile.SaveType)} Save File|*{SaveFile.SaveExtension}|All Files (*.*)|*.*";
            saveSaveFile.FileName = SaveFile.SaveName + SaveFile.SaveExtension;
            saveSaveFile.Filter = filterString;
            var saveOk = saveSaveFile.ShowDialog();
            if (saveOk != DialogResult.OK) return;
            SaveFile.SavePath = Path.GetDirectoryName(saveSaveFile.FileName) + Path.DirectorySeparatorChar;
            SaveFile.SaveName = Path.GetFileNameWithoutExtension(saveSaveFile.FileName);
            SaveFile.SaveExtension = Path.GetExtension(saveSaveFile.FileName);
            SaveToolStripMenuItemClick(sender, e);
            Text = string.Format("ACSE - {1} - [{0}]", SaveDataManager.GetGameTitle(SaveFile.SaveType), SaveFile.SaveName);
        }
        
        private void ExportPictureClick(object sender, EventArgs e)
        {
            if (SaveFile == null || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            exportPatternFile.Filter = "JPEG Image (*.jpg)|*.jpg";
            exportPatternFile.FileName = "TPC Picture";
            var saved = exportPatternFile.ShowDialog();
            if (saved != DialogResult.OK) return;
            using (var picture = new FileStream(exportPatternFile.FileName, FileMode.Create))
                picture.Write(_selectedPlayer.Data.TownPassCardData, 0, 0x1400);
        }

        private void ImportPictureClick(object sender, EventArgs e)
        {
            if (SaveFile == null || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            importPatternFile.Filter = "JPEG Image (*.jpg)|*.jpg";
            var opened = importPatternFile.ShowDialog();
            if (opened != DialogResult.OK) return;
            Image originalImage = null;
            try
            {
                originalImage = Image.FromFile(importPatternFile.FileName);
            }
            catch
            {
                MessageBox.Show("Unable to import the image! The file may be corrupt or opened in another program. Please try again.",
                    "TPC Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (originalImage != null)
                {
                    if (originalImage.Width != 64 || originalImage.Height != 104 ||
                        new FileInfo(importPatternFile.FileName).Length > 0x1400)
                        MessageBox.Show(
                            "The image you tried to import is incompatible. Please ensure the following:\n\nImage Width is 64 pixels\nImage Hight is 104 pixels\n" +
                            "Image file size is equal to or less than 5,120 bytes", "TPC Import Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        //Image passed validation checks, so import it
                        var imageDataBuffer = new byte[0x1400];
                        using (var file = new FileStream(importPatternFile.FileName, FileMode.Open, FileAccess.Read))
                        {
                            file.Read(imageDataBuffer, 0, 0x1400);
                        }
                        //Scan for the actual end of image (0xFF 0xD9) and trim excess
                        var trimmedTpcBuffer = ImageGeneration.GetTpcTrimmedBytes(imageDataBuffer);
                        _selectedPlayer.Data.TownPassCardData = trimmedTpcBuffer;
                        //Draw the new image
                        var oldImage = _tpcPicture.Image;
                        _tpcPicture.Image = ImageGeneration.GetTpcImage(trimmedTpcBuffer);
                        _selectedPlayer.Data.TownPassCardImage = _tpcPicture.Image;
                        oldImage?.Dispose();
                    }

                    originalImage.Dispose();
                }
            }
        }

        private void PerfectFruitsButtonClick()
        {
            if (SaveFile == null || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            var treesMadePerfect = 0;
            foreach (var box in _townAcreMap)
            {
                var acre = TownAcres[Array.IndexOf(_townAcreMap, box)];
                if (acre.Items == null) continue;
                foreach (var fruitTree in acre.Items.Where(i =>
                    i.ItemId >= 0x3A && i.ItemId <= 0x52 && i.Flag1 == 0 && i.Flag2 == 0))
                {
                    fruitTree.Flag1 = 1;
                    treesMadePerfect++;
                }
            }

            MessageBox.Show($"Converted fruit for {treesMadePerfect} trees to perfect fruit!", "Perfect Fruit Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetPlayerSelectionTabText(Player player)
        {
            if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
            {
                _playerTabs[player.Index].Text =
                    player.Data.Name + $" {(player.Index == 0 ? "[Mayor]" : "[Resident]")}";
            }
            else
            {
                _playerTabs[player.Index].Text = player.Data.Name + $" [Player {player.Index + 1}]";
            }
        }

        private void PlayerNameTextChanged(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || playerName.Text.Length <= 0) return;

            if (_villagers != null)
            {
                foreach (var villager in _villagers)
                {
                    if (villager.AnimalMemories == null) continue;
                    foreach (var memory in villager.AnimalMemories)
                    {
                        if (memory.Exists && (memory.Player == _selectedPlayer ||
                                                memory.PlayerId == _selectedPlayer.Data.Identifier &&
                                                memory.PlayerName.Equals(_selectedPlayer.Data.Name)))
                        {
                            memory.PlayerName = playerName.Text;
                        }
                    }
                }
            }

            // Find and update all name references
            switch (SaveFile.SaveGeneration)
            {
                case SaveGeneration.N3DS:
                    var searchData = new byte[0x12 + 0x02 + 0x02];
                    BitConverter.GetBytes(_selectedPlayer.Data.Identifier).CopyTo(searchData, 0);
                    AcString.GetBytes(_selectedPlayer.Data.Name, 16).CopyTo(searchData, 2);
                    searchData[0x14] = 0x01;

                    var replaceData = (byte[]) searchData.Clone();
                    AcString.GetBytes(playerName.Text, 16).CopyTo(replaceData, 2);

                    SaveFile.FindAndReplaceByteArray(searchData, replaceData);
                    break;
            }

            _selectedPlayer.Data.Name = playerName.Text;
            SetPlayerSelectionTabText(_selectedPlayer);
        }

        private void SecureValueToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile != null && SaveFile.SaveGeneration == SaveGeneration.N3DS)
                _secureNandValueForm.ShowDialog();
        }

        private void PlayerWalletFocusLost(object sender, EventArgs e)
        {
            if (SaveFile == null || _selectedPlayer == null) return;
            if (uint.TryParse(playerWallet.Text, out var walletValue))
            {
                if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    _selectedPlayer.Data.NewLeafWallet = new NewLeafInt32(walletValue);
                }
                else
                {
                    _selectedPlayer.Data.Bells = walletValue;
                }
            }
            else
            {
                playerWallet.Text = SaveFile.SaveGeneration == SaveGeneration.N3DS ?
                    _selectedPlayer.Data.NewLeafWallet.Value.ToString() : _selectedPlayer.Data.Bells.ToString();
            }
        }

        private void PlayerDebtFocusLost(object sender, EventArgs e)
        {
            if (SaveFile == null || _selectedPlayer == null) return;
            if (uint.TryParse(playerDebt.Text, out var debtValue))
            {
                if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    _selectedPlayer.Data.NewLeafDebt = new NewLeafInt32(debtValue);
                }
                else
                {
                    _selectedPlayer.Data.Debt = debtValue;
                }
            }
            else
            {
                playerDebt.Text = SaveFile.SaveGeneration == SaveGeneration.N3DS ?
                    _selectedPlayer.Data.NewLeafDebt.Value.ToString() : _selectedPlayer.Data.Debt.ToString();
            }
        }

        private void PlayerSavingsFocusLost(object sender, EventArgs e)
        {
            if (SaveFile == null || _selectedPlayer == null) return;
            if (uint.TryParse(playerSavings.Text, out var savingsValue))
            {
                if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    _selectedPlayer.Data.NewLeafSavings = new NewLeafInt32(savingsValue);
                }
                else
                {
                    _selectedPlayer.Data.Savings = savingsValue;
                }
            }
            else
            {
                playerSavings.Text = SaveFile.SaveGeneration == SaveGeneration.N3DS ?
                    _selectedPlayer.Data.NewLeafSavings.Value.ToString() : _selectedPlayer.Data.Savings.ToString();
            }
        }

        private void PlayerMeowCouponsFocusLost(object sender, EventArgs e)
        {
            if (SaveFile == null || _selectedPlayer == null || SaveFile.SaveType != SaveType.WelcomeAmiibo) return;
            if (uint.TryParse(playerMeowCoupons.Text, out var meowCouponsValue))
            {
                _selectedPlayer.Data.MeowCoupons = new NewLeafInt32(meowCouponsValue);
            }
            else
            {
                playerMeowCoupons.Text = _selectedPlayer.Data.MeowCoupons.Value.ToString();
            }
        }

        private void PlayerIslandMedalsFocusLost(object sender, EventArgs e)
        {
            if (SaveFile == null || _selectedPlayer == null || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            if (uint.TryParse(playerIslandMedals.Text, out var islandMedalsValue))
            {
                _selectedPlayer.Data.IslandMedals = new NewLeafInt32(islandMedalsValue);
            }
            else
            {
                playerIslandMedals.Text = _selectedPlayer.Data.IslandMedals.Value.ToString();
            }
        }

        private void PlayerNookPointsFocusLost(object sender, EventArgs e)
        {
            if (SaveFile == null || _selectedPlayer == null ||
                (SaveFile.SaveType != SaveType.WildWorld && SaveFile.SaveType != SaveType.CityFolk)) return;
            if (ushort.TryParse(playerNookPoints.Text, out var nookPointsValue))
            {
                _selectedPlayer.Data.NookPoints = nookPointsValue;
            }
            else
            {
                playerNookPoints.Text = _selectedPlayer.Data.NookPoints.ToString();
            }
        }

        private void AcreHeightTrackBarScroll(object sender, EventArgs e)
        {
            if (SaveFile == null || (SaveFile.SaveGeneration != SaveGeneration.N64 &&
                                     SaveFile.SaveGeneration != SaveGeneration.GCN &&
                                     SaveFile.SaveGeneration != SaveGeneration.iQue)) return;
            _acreHeightModifier = (ushort)acreHeightTrackBar.Value;
            acreID.Text = "Acre ID: 0x" + (_selectedAcreId + _acreHeightModifier).ToString("X4");
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            if (SaveFile == null || (SaveFile.SaveGeneration != SaveGeneration.N64 &&
                                     SaveFile.SaveGeneration != SaveGeneration.GCN &&
                                     SaveFile.SaveGeneration != SaveGeneration.iQue)) return;
            if (_acMapIconIndex == null)
            {
                _acMapIconIndex = AcreImageManager.LoadAcMapIndex(SaveFile.SaveType);
            }

            if (townMapViewCheckbox.Checked)
            {
                if (_acMapIconIndex == null) return;
                for (var i = 0; i < _acres.Length; i++)
                {
                    Image image;
                    if (_acMapIconIndex.ContainsKey(_acres[i].AcreId))
                    {
                        image = AcreImageManager.FetchAcMapIcon(_acMapIconIndex[_acres[i].AcreId]);
                    }
                    else if (_acMapIconIndex.ContainsKey(_acres[i].BaseAcreId))
                    {
                        image = AcreImageManager.FetchAcMapIcon(_acMapIconIndex[_acres[i].BaseAcreId]);
                    }
                    else
                    {
                        image = AcreImageManager.FetchAcMapIcon(99);
                    }
                    _acreMap[i].BackgroundImage = image; // We don't dispose the previous images here, because they're needed in the town editor
                }
                ImageGeneration.DrawTownMapViewHouseImages(_villagers, _acreMap, new Size(_acreMap[0].Size.Width, _acreMap[0].Height));
            }
            else
            {
                for (var i = 0; i < _acres.Length; i++)
                {
                    var oldImage = _acreMap[i].BackgroundImage;
                    _acreMap[i].BackgroundImage = GetAcreImage(_acres[i].BaseAcreId);
                    AcreImageManager.CheckReferencesAndDispose(oldImage, _acreMap, _selectedAcrePicturebox);
                }
            }
        }

        private void ResettiCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (_loading || SaveFile == null || _selectedPlayer == null) return;
            switch (SaveFile.SaveType)
            {
                case SaveType.CityFolk:
                    if (resettiCheckBox.Checked)
                    {
                        SaveFile.Write(_selectedPlayer.Offset + 0x8670,
                            (byte) (SaveFile.ReadByte(_selectedPlayer.Offset + 0x8670) | 0x02));
                    }
                    else
                    {
                        SaveFile.Write(_selectedPlayer.Offset + 0x8670,
                            (byte) (SaveFile.ReadByte(_selectedPlayer.Offset + 0x8670) & 0xFD));
                    }

                    break;
                case SaveType.NewLeaf:
                    if (resettiCheckBox.Checked)
                    {
                        SaveFile.Write(_selectedPlayer.Offset + 0x5702,
                            (byte) (SaveFile.ReadByte(_selectedPlayer.Offset + 0x5702) | 0x02));
                    }
                    else
                    {
                        SaveFile.Write(_selectedPlayer.Offset + 0x5702,
                            (byte) (SaveFile.ReadByte(_selectedPlayer.Offset + 0x5702) & 0xFD));
                    }

                    break;
                case SaveType.WelcomeAmiibo:
                    if (resettiCheckBox.Checked)
                    {
                        SaveFile.Write(_selectedPlayer.Offset + 0x570A,
                            (byte) (SaveFile.ReadByte(_selectedPlayer.Offset + 0x570A) | 0x02));
                    }
                    else
                    {
                        SaveFile.Write(_selectedPlayer.Offset + 0x570A,
                            (byte) (SaveFile.ReadByte(_selectedPlayer.Offset + 0x570A) & 0xFD));
                    }

                    break;
                default:
                    _selectedPlayer.Data.Reset = resettiCheckBox.Checked;
                    break;
            }
        }

        private void ClearWeedsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ClearWeedsButtonClick();
        }

        private void RemoveAllItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            for (var i = 0; i < TownAcres.Length; i++)
            {
                for (var x = 0; x < TownAcres[i].Items.Length; x++)
                {
                    TownAcres[i].Items[x] = new Item(TownAcres[i].Items[x]);
                }
                _townAcreMap[i].Image = GenerateAcreItemsBitmap(TownAcres[i]);
            }
        }

        private void MakeFruitsPerfectToolStripMenuItemClick(object sender, EventArgs e)
        {
            PerfectFruitsButtonClick();
        }

        private void WaterFlowersToolStripMenuItemClick(object sender, EventArgs e)
        {
            WaterFlowersButtonClick();
        }

        private void CensusMenuEnabledCheckedChanged(object sender, EventArgs e)
        {
            if (_loading || SaveFile == null || SaveFile.SaveType != SaveType.WelcomeAmiibo) return;
            if (censusMenuEnabled.Checked)
            {
                SaveFile.Write(_selectedPlayer.Offset + 0x572F, (byte)(SaveFile.ReadByte(_selectedPlayer.Offset + 0x572F) | 0x40));
            }
            else
            {
                SaveFile.Write(_selectedPlayer.Offset + 0x572F, (byte)(SaveFile.ReadByte(_selectedPlayer.Offset + 0x572F) & 0xBF));
            }
        }

        private void ImportTownToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading) return;
            DataUtility.ImportTown(ref TownAcres, SaveFile.SaveGeneration);
            SetupMapPictureBoxes();
        }

        private void ExportTownToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading)
            {
                DataUtility.ExportTown(TownAcres, SaveFile.SaveGeneration, SaveFile.SaveName);
            }
        }

        private void FillMuseumToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                Museum.FillMuseum(SaveFile, _selectedPlayer);
            }
        }

        private void ClearMuseumToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading)
            {
                Museum.ClearMuseum(SaveFile);
            }
        }

        private void UnlockAllPublicWorkProjectsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            var pwpUnlockedOffset = SaveFile.SaveDataStartOffset + (SaveFile.SaveType == SaveType.NewLeaf ? 0x4D9C8 : 0x502A8);
            for (var i = 0; i < 20; i++)
            {
                SaveFile.Write(pwpUnlockedOffset + i, (byte)0xFF);
            }
        }

        private void SaveTownMapToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading)
            {
                ImageGeneration.DumpTownAcreBitmap(CurrentSaveInfo.XAcreCount, CurrentSaveInfo.AcreCount / CurrentSaveInfo.XAcreCount,
                    ref _acreMap);
            }
        }

        private void FillCatalogButtonClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                Catalog.FillCatalog(SaveFile, _selectedPlayer);
            }
        }

        private void FillEmotionsButtonClick(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || _selectedPlayer == null || !_selectedPlayer.Exists) return;
            if(Emotion.FillEmotions(SaveFile, _selectedPlayer))
            {
                MessageBox.Show($"Emotions filled for {_selectedPlayer.Data.Name}!", "Info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void FillEncyclopediaButtonClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                Encyclopedia.FillEncyclopedia(SaveFile, _selectedPlayer);
            }
        }

        private void ClearEncyclopediaButtonClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                Encyclopedia.ClearEncylopedia(SaveFile, _selectedPlayer);
            }
        }

        private void ClearSongLibraryButtonClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                SongLibrary.ClearSongLibrary(SaveFile, _selectedPlayer);
            }
        }

        private void FillSongLibraryButtonClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                SongLibrary.FillSongLibrary(SaveFile, _selectedPlayer);
            }
        }

        private void ClearEmotionsButtonClick(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || _selectedPlayer == null || !_selectedPlayer.Exists) return;
            if (Emotion.ClearEmotions(SaveFile, _selectedPlayer))
            {
                MessageBox.Show($"Emotions cleared for {_selectedPlayer.Data.Name}!", "Info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void ClearCatalogButtonClick(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && _selectedPlayer != null && _selectedPlayer.Exists)
            {
                Catalog.ClearCatalog(SaveFile, _selectedPlayer);
            }
        }

        private void UnlockHhdItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || SaveFile.SaveType != SaveType.WelcomeAmiibo) return;
            var hhdOffset = SaveFile.SaveDataStartOffset + 0x6215C;
            SaveFile.Write(hhdOffset, (byte)(SaveFile.ReadByte(hhdOffset) | 0x04));
            MessageBox.Show("Happy Home Designer Content is now unlocked!", "HHD Content", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WeatherComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (SaveFile == null || _loading || weatherComboBox.SelectedIndex <= -1) return;
            if (!Weather.UpdateWeather(SaveFile, (byte)weatherComboBox.SelectedIndex) && SaveFile.SaveGeneration == SaveGeneration.GCN)
            {
                weatherComboBox.SelectedIndex = Weather.GetWeatherIndex(SaveFile.ReadByte(SaveFile.SaveDataStartOffset + SaveFile.SaveInfo.SaveOffsets.Weather),
                    SaveFile.SaveGeneration);
            }
        }

        private readonly string CitraNewLeafSavePath = Path.Combine(Environment.GetEnvironmentVariable("appdata"),
            "Citra", "sdmc", "Nintendo 3DS", "00000000000000000000000000000000", "00000000000000000000000000000000",
            "title", "00040000");

        private void EurToolStripMenuItem1Click(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00198f00", "data", "00000001", "garden_plus.dat"));
        }

        private void UsaToolStripMenuItem1Click(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00198e00", "data", "00000001", "garden_plus.dat"));
        }

        private void JpnToolStripMenuItem1Click(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00198d00", "data", "00000001", "garden_plus.dat"));
        }

        private void EurToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00086400", "data", "00000001", "garden.dat"));
        }

        private void UsaToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00086300", "data", "00000001", "garden.dat"));
        }

        private void JpnToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00086200", "data", "00000001", "garden.dat"));
        }

        private void KorToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00086500", "data", "00000001", "garden.dat"));
        }

        private void KorToolStripMenuItem1Click(object sender, EventArgs e)
        {
            OpenSave(Path.Combine(CitraNewLeafSavePath, "00199000", "data", "00000001", "garden_plus.dat"));
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (SaveFile == null || _loading || !SaveFile.ChangesMade) return;
            if (!ConfirmSave("You've made changes to this save file. Would you like to save them before closing it?"))
            {
                e.Cancel = true; // Don't close the form
            }
        }

        private void ReplaceToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (TownAcres == null ||
                !ushort.TryParse(_replaceItemBox.Text, NumberStyles.HexNumber, null, out var replaceId) ||
                !ushort.TryParse(_replacingItemBox.Text, NumberStyles.HexNumber, null, out var replacingId)) return;
            var replacedItems = 0;
            var replacingItem = new Item(replacingId, 0, 0);

            for (var i = 0; i < TownAcres.Length; i++)
            {
                var changed = false;
                var acre = TownAcres[i];
                if (acre.Items == null) continue;

                for (var index = 0; index < acre.Items.Length; index++)
                {
                    if (acre.Items[index].ItemId != replaceId) continue;

                    changed = true;
                    replacedItems++;
                    SaveFile.ChangesMade = true;
                    acre.Items[index] = new Item(replacingItem);

                }

                if (changed)
                {
                    RefreshPictureBoxImage(_townAcreMap[i],
                        GenerateAcreItemsBitmap(acre)); // TODO: Make this work on island acres somehow.
                }
            }

            MessageBox.Show($"{replacedItems} items were replaced with {replacingItem.Name}!", "Replace Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            _settingsMenu.ShowDialog();
        }

        private void StatueCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (SaveFile != null && !_loading && SaveFile.SaveGeneration == SaveGeneration.GCN && _selectedHouse != null)
            {
                HouseInfo.SetStatueEnabled(_selectedHouse.Offset, SaveFile.SaveType, StatueCheckBox.Checked);
            }
        }

        private void GenerateRandomTownToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (_loading || SaveFile == null) return;
            int? seed = !string.IsNullOrWhiteSpace(_seedBox.Text) ? (int)long.Parse(_seedBox.Text) : Environment.TickCount;

            switch (SaveFile.SaveGeneration)
            {
                case SaveGeneration.GCN:
                    var newAcreData = Generator.GetGenerator(SaveFile.SaveGeneration).Generate(seed);

                    for (var i = 0; i < newAcreData.Length; i++)
                    {
                        _acres[i] = new WorldAcre(newAcreData[i], i);
                        _acres[i].LoadDefaultItems(SaveFile);
                        var oldImage = _acreMap[i].BackgroundImage;
                        _acreMap[i].BackgroundImage = GetAcreImage(_acres[i].BaseAcreId);
                        AcreImageManager.CheckReferencesAndDispose(oldImage, _acreMap, _selectedAcrePicturebox);
                        _acreMap[i].Refresh();
                        var x = i % CurrentSaveInfo.XAcreCount;
                        var y = i / CurrentSaveInfo.XAcreCount;
                        if (y < CurrentSaveInfo.TownYAcreStart || x <= 0 || x >= CurrentSaveInfo.XAcreCount - 1)
                            continue;

                        var townAcre = (y - CurrentSaveInfo.TownYAcreStart) * (CurrentSaveInfo.XAcreCount - 2) + (x - 1);
                        if (townAcre >= CurrentSaveInfo.TownAcreCount) continue;

                        TownAcres[townAcre] = new WorldAcre(_acres[i].AcreId, townAcre, _acres[i].Items);
                        RefreshPictureBoxImage(_townAcreMap[townAcre],
                            GenerateAcreItemsBitmap(TownAcres[townAcre]));
                        _townAcreMap[townAcre].BackgroundImage = _acreMap[i].BackgroundImage;
                        _townAcreMap[townAcre].Refresh();

                        // TODO: Island Acres
                    }
                    SetStatusText("A new town layout was successfully generated with the seed: " + seed.Value);
                    break;
                default:
                    MessageBox.Show("Town Generation for this game hasn't been implemented yet!", "Generation Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }

        private void WaterFlowersButtonClick()
        {
            var flowersWatered = 0;
            foreach (var box in _townAcreMap)
            {
                var acreIdx = Array.IndexOf(_townAcreMap, box);
                var acre = TownAcres[acreIdx];
                if (acre.Items == null) continue;
                switch (SaveFile.SaveGeneration)
                {
                    case SaveGeneration.NDS:
                        for (var i = 0; i < 256; i++)
                        {
                            var itemType = ItemData.GetItemType(acre.Items[i].ItemId, SaveFile.SaveType);
                            switch (itemType)
                            {
                                case ItemType.ParchedFlower:
                                    acre.Items[i] = new Item((ushort) (acre.Items[i].ItemId + 0x1C));
                                    flowersWatered++;
                                    break;
                                case ItemType.Flower:
                                    acre.Items[i] = new Item((ushort) (acre.Items[i].ItemId + 0x8A));
                                    flowersWatered++;
                                    break;
                            }
                        }

                        break;
                    case SaveGeneration.Wii:
                        for (var i = 0; i < 256; i++)
                        {
                            if (ItemData.GetItemType(acre.Items[i].ItemId, SaveFile.SaveType) != ItemType.ParchedFlower)
                                continue;
                            acre.Items[i] = new Item((ushort) (acre.Items[i].ItemId - 0x20));
                            flowersWatered++;
                        }

                        break;
                    case SaveGeneration.N3DS:
                        for (var i = 0; i < 256; i++)
                        {
                            if (ItemData.GetItemType(acre.Items[i].ItemId, SaveFile.SaveType) != ItemType.Flower)
                                continue;
                            acre.Items[i].Flag1 = 0x40;
                            //acre.Items[i].Watered = true;
                            flowersWatered++;
                        }

                        break;
                }
                var oldImage = box.Image;
                box.Image = GenerateAcreItemsBitmap(acre);
                oldImage?.Dispose();
            }

            MessageBox.Show($"Watered {flowersWatered} flowers!", "Flowers Watered", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void itemColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var colorEditor = new ItemColorEditor())
            {
                MessageBox.Show(
                    "After changing the settings, you'll need to reload your save file if you have one open!",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                colorEditor.ShowDialog();
            }
        }

        private void openBackupFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Properties.Settings.Default.BackupLocation))
            {
                System.Diagnostics.Process.Start(Properties.Settings.Default.BackupLocation);
            }
            else
            {
                MessageBox.Show(
                    !string.IsNullOrWhiteSpace(Properties.Settings.Default.BackupLocation)
                        ? $"The backup folder located at \"{Properties.Settings.Default.BackupLocation}\" couldn't be accessed!"
                        : "The backup folder hasn't been set yet.\nPlease set it first!",
                    "Backup Folder Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void townGateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading && SaveFile != null && townGateComboBox.Enabled && townGateComboBox.SelectedIndex > -1 &&
                townGateComboBox.SelectedIndex < 3)
            {
                TownGate.SetTownGateType(SaveFile, townGateComboBox.SelectedIndex);
            }
        }

        private void nativeFruitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading && SaveFile != null && nativeFruitBox.SelectedIndex > -1 && nativeFruitBox.SelectedIndex < 5)
            {
                NativeFruit.SetNativeFruit(SaveFile, nativeFruitBox.SelectedIndex);
            }
        }

        private void SetOrdinanceCheckBoxes()
        {
            if (SaveFile == null || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            var ordinanceFlags = SaveFile.SaveType == SaveType.NewLeaf
                ? SaveFile.ReadByte(SaveFile.SaveDataStartOffset + 0x5C74F)
                : SaveFile.ReadByte(SaveFile.SaveDataStartOffset + 0x6214F);

            earlyBirdCheckBox.Checked = (ordinanceFlags & 0x02) == 0x02;
            nightOwlCheckBox.Checked = (ordinanceFlags & 0x04) == 0x04;
            bellBoomCheckBox.Checked = (ordinanceFlags & 0x08) == 0x08;
            keepTownBeautifulCheckBox.Checked = (ordinanceFlags & 0x10) == 0x10;
        }

        private void UpdateNewLeafOrdinances()
        {
            if (SaveFile == null || _loading || SaveFile.SaveGeneration != SaveGeneration.N3DS) return;
            byte ordinancesInEffect = 0;
            byte ordinancesEnabled = 0;
            var ordinances = 0;

            if (earlyBirdCheckBox.Checked)
            {
                ordinancesInEffect |= 0x02;
                ordinancesEnabled = 0;
                ordinances++;
            }

            if (nightOwlCheckBox.Checked)
            {
                ordinancesInEffect |= 0x04;
                ordinancesEnabled = 0x10;
                ordinances++;
            }

            if (bellBoomCheckBox.Checked)
            {
                ordinancesInEffect |= 0x08;
                ordinancesEnabled = 0x20;
                ordinances++;
            }

            if (keepTownBeautifulCheckBox.Checked)
            {
                ordinancesInEffect |= 0x10;
                ordinancesEnabled = 0x30;
                ordinances++;
            }

            if (ordinances == 0)
            {
                ordinancesEnabled = 0x40;
            }
            else if (ordinances > 1)
            {
                ordinancesEnabled = 0x70; // Max of 0x70
            }

            if (SaveFile.SaveType == SaveType.NewLeaf)
            {
                SaveFile.Write(SaveFile.SaveDataStartOffset + 0x5C74F,
                    (byte)((SaveFile.ReadByte(SaveFile.SaveDataStartOffset + 0x5C74F) & (~0x1E)) | (ordinancesInEffect & 0x1E)));
                SaveFile.Write(SaveFile.SaveDataStartOffset + 0x5C753, (byte) (ordinancesEnabled |
                                                                               (SaveFile.ReadByte(
                                                                                    SaveFile.SaveDataStartOffset +
                                                                                    0x5C753) & 0x0F)));
            }
            else // Welcome Amiibo
            {
                SaveFile.Write(SaveFile.SaveDataStartOffset + 0x6214F,
                    (byte)((SaveFile.ReadByte(SaveFile.SaveDataStartOffset + 0x6214F) & (~0x1E)) | (ordinancesInEffect & 0x1E)));
                SaveFile.Write(SaveFile.SaveDataStartOffset + 0x62153, (byte) (ordinancesEnabled |
                                                                               (SaveFile.ReadByte(
                                                                                    SaveFile.SaveDataStartOffset +
                                                                                    0x62153) & 0x0F)));
            }
        }

        #region Doubutsu no Mori e+ Islands

        private void IslandTabIndexChanged(object sender, TabControlEventArgs e)
        {
            if (islandSelectionTab.SelectedIndex < 0 || islandSelectionTab.SelectedIndex > 3)
                return;
            _selectedIsland = _islands[islandSelectionTab.SelectedIndex];
            if (_selectedIsland == null) return;
            ReloadIslandItemPicture();

            var islandAcreIds = _selectedIsland.GetAcreIds();
            _islandAcreMap[0].BackgroundImage = GetAcreImage(islandAcreIds[0]);
            _islandAcreMap[1].BackgroundImage = GetAcreImage(islandAcreIds[1]);

            // Reload Island Cabana
            if (_islandHouseEditor == null) return;
            _islandHouseEditor.House = _selectedIsland.Cabana;

        }

        private void ReloadIslandItemPicture()
        {
            if (_selectedIsland?.Acres == null) return;
            for (var i = 0; i < 2; i++)
            {
                RefreshPictureBoxImage(_islandAcreMap[i], GenerateAcreItemsBitmap(_selectedIsland.Acres[i], true));
            }
        }

        #endregion
    }
}