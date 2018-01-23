using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ACSE.Classes;

namespace ACSE
{
    public partial class NewMainForm : Form
    {
        public static readonly string Assembly_Location = Directory.GetCurrentDirectory();
        public static DebugManager Debug_Manager = new DebugManager();
        TabPage[] Main_Tabs;
        TabPage[] Player_Tabs = new TabPage[4];
        NewPlayer[] Players = new NewPlayer[4];
        House[] Houses;
        NewVillager[] Villagers;
        PictureBoxWithInterpolationMode[] Acre_Map;
        PictureBoxWithInterpolationMode[] Town_Acre_Map;
        PictureBoxWithInterpolationMode[] Island_Acre_Map;
        PictureBoxWithInterpolationMode[] NL_Island_Acre_Map;
        PictureBoxWithInterpolationMode[] Grass_Map;
        PictureBoxWithInterpolationMode[] Pattern_Boxes;
        PictureBoxWithInterpolationMode[][] House_Boxes;
        PictureBoxWithInterpolationMode[] Island_House_Boxes;
        PictureBoxWithInterpolationMode TPC_Picture;
        Image Selected_Pattern = null;
        Panel[] Building_List_Panels;
        Normal_Acre[] Acres;
        public static Normal_Acre[] Town_Acres;
        public static Normal_Acre[] Island_Acres;
        Building[] Buildings;
        Building[] Island_Buildings;
        public static Save Save_File;
        public static Save_Info Current_Save_Info;
        List<KeyValuePair<ushort, string>> Item_List;
        NewPlayer Selected_Player;
        House Selected_House;
        Dictionary<string, Image> Acre_Image_List; //Image Database
        Dictionary<byte, string> Acre_Info; //Name Database
        Dictionary<string, List<byte>> Filed_Acre_Data; //Grouped info for Acre Editor TreeView
        Dictionary<ushort, string> UInt16_Acre_Info;
        Dictionary<string, Dictionary<ushort, string>> UInt16_Filed_Acre_Data;
        Dictionary<ushort, SimpleVillager> Villager_Database;
        SimpleVillager[] Past_Villagers;
        string[] Personality_Database;
        string[] Villager_Names;
        byte[] Grass_Wear;
        AboutBox1 About_Box = new AboutBox1();
        SecureValueForm Secure_NAND_Value_Form = new SecureValueForm();
        SettingsMenuForm Settings_Menu;
        int ScrollbarWidth = SystemInformation.VerticalScrollBarWidth;
        bool Clicking = false;
        byte[] Buried_Buffer;
        byte[] Island_Buried_Buffer;
        ushort Selected_Acre_ID = 0;
        int Selected_Building = -1;
        ushort Last_Selected_Item = 0;
        EventHandler Campsite_EventHandler;
        Item CurrentItem = new Item();
        PictureBoxWithInterpolationMode selectedAcrePicturebox;
        int Last_Month = 0;
        ushort Acre_Height_Modifier = 0;
        Dictionary<ushort, byte> AC_Map_Icon_Index;
        Dictionary<byte, Image> AC_Map_Icons;
        Island[] Islands;
        Island SelectedIsland;
        Room IslandRoom; // DnM+/AC
        private byte[] Building_DB;
        private string[] Building_Names;
        private PictureBoxWithInterpolationMode NL_Grass_Overlay;
        private PlaceholderTextBox ReplaceItemBox;
        private PlaceholderTextBox ReplacingItemBox;
        private bool Loading = false;

        #region MapSizeVariables

        private static int TownMapCellSize = Properties.Settings.Default.TownMapSize / 16;
        private static int TownMapTotalSize = TownMapCellSize * 16;

        private static int AcreMapSize = Properties.Settings.Default.AcreMapSize;

        #endregion

        public NewMainForm(Save save = null)
        {
            InitializeComponent();

            // Clamp Map Sizes
            if (TownMapCellSize < 8)
            {
                TownMapCellSize = 8;
                TownMapTotalSize = 8 * 16;
            }

            if (AcreMapSize < 64)
                AcreMapSize = 64;

            Settings_Menu = new SettingsMenuForm(this);
            TPC_Picture = new PictureBoxWithInterpolationMode
            {
                Name = "TPC_PictureBox",
                Size = new Size(128, 208),
                Location = new Point(6, 6),
                InterpolationMode = InterpolationMode.NearestNeighbor,
                UseInternalInterpolationSetting = true,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = Properties.Resources.no_tpc,
                ContextMenuStrip = pictureContextMenu,
            };
            //Temp
            /*float[][] Test_Matrix =
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0 },
                new float[] {0, 0, 1, 0, 0 },
                new float[] {0, 0, 0, 1, 0 },
                new float[] {0.2f, 0.2f, 0.2f, 0, 1 },
            };
            TPC_Picture.Image = Utility.Set_Image_Color(TPC_Picture.Image, new ColorMatrix(Test_Matrix));*/
            playersTab.Controls.Add(TPC_Picture);
            //openSaveFile.FileOk += new CancelEventHandler(open_File_OK);

            Main_Tabs = new TabPage[tabControl1.TabCount];
            for (int i = 0; i < tabControl1.TabCount; i++)
                Main_Tabs[i] = tabControl1.TabPages[i];
            for (int i = 0; i < playerEditorSelect.TabCount; i++)
                Player_Tabs[i] = playerEditorSelect.TabPages[i];

            selectedItem.SelectedValueChanged += new EventHandler(Item_Selected_Index_Changed);
            acreTreeView.AfterSelect += new TreeViewEventHandler(Acre_Tree_View_Entry_Clicked);
            playerEditorSelect.Selected += new TabControlEventHandler(Player_Tab_Index_Changed);

            //Setup selectedAcrePictureBox
            selectedAcrePicturebox = new PictureBoxWithInterpolationMode
            {
                Size = new Size(64, 64),
                Location = new Point(883, 620),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackgroundImageLayout = ImageLayout.Stretch,
                InterpolationMode = InterpolationMode.HighQualityBicubic,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            Controls.Add(selectedAcrePicturebox);

            //Town Name TextBox LostFocus
            townNameBox.LostFocus += TownName_Box_FocusLost;

            //Grass Type ComboBox SelectedIndexChanged
            grassTypeBox.SelectedIndexChanged += delegate (object sender, EventArgs e)
            {
                if (!Loading && grassTypeBox.SelectedIndex > -1 && Save_File != null && Current_Save_Info.Save_Offsets.Grass_Type != -1)
                {
                    Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Grass_Type, (byte)grassTypeBox.SelectedIndex);
                }
            };

            //Player Item PictureBox Event Hookups
            BindPlayerItemBoxEvents(inventoryPicturebox);
            BindPlayerItemBoxEvents(heldItemPicturebox);
            BindPlayerItemBoxEvents(shirtPicturebox);
            BindPlayerItemBoxEvents(hatPicturebox);
            BindPlayerItemBoxEvents(facePicturebox);
            BindPlayerItemBoxEvents(pocketsBackgroundPicturebox);
            BindPlayerItemBoxEvents(bedPicturebox);
            BindPlayerItemBoxEvents(pantsPicturebox);
            BindPlayerItemBoxEvents(socksPicturebox);
            BindPlayerItemBoxEvents(shoesPicturebox);
            BindPlayerItemBoxEvents(playerWetsuit);

            //Player Event Hookups
            playerGender.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => Gender_Changed());
            playerFace.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => Face_Changed());
            playerHairType.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => Hair_Changed());
            playerHairColor.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => Hair_Color_Changed());
            playerEyeColor.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => Eye_Color_Changed());
            playerShoeColor.SelectedIndexChanged += new EventHandler((object sender, EventArgs e) => Shoe_Color_Changed());

            //Setup Campsite EventHandler
            Campsite_EventHandler = new EventHandler((object sender, EventArgs e) => Campsite_Villager_Changed());

            //Setup Welcome Amiibo Caravan ComboBoxes
            caravan1ComboBox.DataSource = VillagerData.GetCaravanBindingSource();
            caravan2ComboBox.DataSource = VillagerData.GetCaravanBindingSource();
            caravan1ComboBox.ValueMember = "Key";
            caravan1ComboBox.DisplayMember = "Value";
            caravan2ComboBox.ValueMember = "Key";
            caravan2ComboBox.DisplayMember = "Value";

            //Birthday Event Hookups
            birthdayMonth.LostFocus += new EventHandler((object sender, EventArgs e) => Birthday_Month_FocusLost());
            birthdayDay.LostFocus += new EventHandler((object sender, EventArgs e) => Birthday_Day_FocusLost());

            //Custom Acre ID Event Hookup
            acreCustomIdBox.TextChanged += delegate (object sender, EventArgs e)
            {
                if (Save_File != null && ushort.TryParse(acreCustomIdBox.Text, NumberStyles.AllowHexSpecifier, null, out ushort Custom_ID))
                {
                    Set_Selected_Acre(Custom_ID);
                }
            };

            // Roof ComboBox Changed
            roofColorComboBox.SelectedIndexChanged += delegate (object sender, EventArgs e)
            {
                if (roofColorComboBox.Enabled && roofColorComboBox.SelectedIndex > -1 && Selected_House != null)
                {
                    Selected_House.Data.Roof_Color = (byte)(roofColorComboBox.SelectedIndex);
                }
            };

            basementCheckBox.CheckedChanged += BasementCheckBoxCheckChanged;
            houseTabSelect.Selected += House_Tab_Index_Changed;

            // Construct Replace Item Menu
            ConstructReplaceItemMenu();

            // Island Tab Index Changed
            islandSelectionTab.Selected += IslandTabIndexChanged;

            if (save != null)
                SetupEditor(save);
        }

        #region Settings Changing Functions

        public void SetMapPictureBoxSize(ushort Size)
        {
            TownMapTotalSize = Size;
            TownMapCellSize = Size / 16;

            if (Save_File != null)
                SetupMapPictureBoxes();
        }

        public void SetAcreMapPictureBoxSize(byte Size)
        {
            AcreMapSize = Size;

            if (Save_File != null)
                SetupMapPictureBoxes();
        }

        #endregion

        #region Replace MenuStrip Construction

        private void ConstructReplaceItemMenu()
        {
            ReplaceItemBox = new PlaceholderTextBox
            {
                PlaceholderText = "Replaced ID",
                Size = new Size(replaceToolStripMenuItem.Size.Width, 18),
                MaxLength = 4
            };

            ReplacingItemBox = new PlaceholderTextBox
            {
                PlaceholderText = "Replacing ID",
                Size = new Size(replaceToolStripMenuItem.Size.Width, 18)
            };
            ReplaceItemBox.MaxLength = 4;

            var ReplaceToolStripHost = new ToolStripControlHost(ReplaceItemBox);
            var ReplacingToolStripHost = new ToolStripControlHost(ReplacingItemBox);

            ReplaceToolStripHost.AutoSize = false;
            ReplacingToolStripHost.AutoSize = false;

            ReplaceToolStripHost.Size = ReplaceItemBox.Size;
            ReplacingToolStripHost.Size = ReplacingItemBox.Size;

            replaceItemsToolStripMenuItem.DropDown.Items.Insert(0, ReplaceToolStripHost);
            replaceItemsToolStripMenuItem.DropDown.Items.Insert(1, ReplacingToolStripHost);

            ReplaceItemBox.TextChanged += new EventHandler((object s, EventArgs e) => ReplaceVerifyHex(ReplaceItemBox));
            ReplacingItemBox.TextChanged += new EventHandler((object s, EventArgs e) => ReplaceVerifyHex(ReplacingItemBox));
        }

        private void ReplaceVerifyHex(PlaceholderTextBox TextBox)
        {
            string Text = TextBox.Text;
            if (!int.TryParse(Text, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out int Hex) && Text != string.Empty)
            {
                TextBox.Text = Text.Remove(Text.Length - 1, 1);
                TextBox.SelectionStart = TextBox.Text.Length;
            }
        }

        #endregion

        private void Birthday_Month_FocusLost()
        {
            if (Last_Month != birthdayMonth.SelectedIndex)
            {
                birthdayDay.Items.Clear();
                if (birthdayMonth.SelectedIndex > -1 && birthdayMonth.SelectedIndex < 12)
                {
                    for (int i = 1; i <= DateTime.DaysInMonth(2000, birthdayMonth.SelectedIndex + 1); i++)
                    {
                        birthdayDay.Items.Add(i.ToString());
                    }
                }
                birthdayDay.Items.Add("Not Set");
                birthdayDay.SelectedIndex = birthdayDay.Items.Count - 1;
                Last_Month = birthdayMonth.SelectedIndex;
            }

            if (Selected_Player != null && Selected_Player.Data.Birthday != null)
            {
                if (birthdayMonth.SelectedIndex < 1 || birthdayMonth.SelectedIndex > 12)
                {
                    Selected_Player.Data.Birthday.Month = 0xFF; // Not sure about this for Non-GCN games
                }
                else
                {
                    Selected_Player.Data.Birthday.Month = (uint)birthdayMonth.SelectedIndex + 1;
                }
            }
        }

        private void Birthday_Day_FocusLost()
        {
            if (Selected_Player != null && Selected_Player.Data.Birthday != null)
            {
                if (birthdayDay.Items.Count < 2 || birthdayDay.SelectedIndex < 1 || birthdayDay.SelectedIndex > 31)
                {
                    Selected_Player.Data.Birthday.Day = 0xFF;
                }
                else
                {
                    Selected_Player.Data.Birthday.Day = (uint)birthdayDay.SelectedIndex + 1;
                }
            }
        }

        private void Campsite_Villager_Changed()
        {
            if (Save_File != null && campsiteComboBox.Enabled)
            {
                try
                {
                    if (campsiteComboBox.SelectedValue != null)
                    {
                        ushort Camper_ID = (ushort)campsiteComboBox.SelectedValue;
                        Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Campsite_Visitor, Camper_ID, Save_File.Is_Big_Endian);
                    }
                }
                catch
                {
                    // Implement Debug Line
                }
            }
        }

        private void BindPlayerItemBoxEvents(Control Bindee)
        {
            Bindee.MouseClick += new MouseEventHandler(Players_Mouse_Click);
            Bindee.MouseMove += new MouseEventHandler(Players_Mouse_Move);
            Bindee.MouseLeave += new EventHandler(Hide_Tip);
        }

        private void SetCurrentItem(Item New_Item)
        {
            if (New_Item != null)
            {
                CurrentItem = New_Item;
                selectedItem.SelectedValue = New_Item.ItemID;
                itemFlag1.Text = New_Item.Flag1.ToString("X2");
                itemFlag2.Text = New_Item.Flag2.ToString("X2");
            }
        }

        private ushort GetCurrentItem()
        {
            return selectedItem.SelectedValue == null ? new Item().ItemID : (ushort)selectedItem.SelectedValue;
        }

        private void SelectedItem_Changed(object sender, EventArgs e)
        {
            if (selectedItem.SelectedValue == null)
            {
                SetCurrentItem(CurrentItem);
            }
        }

        private void SetupEditor(Save save)
        {
            Loading = true;
            if (save.Save_Type == SaveType.Unknown)
            {
                MessageBox.Show(string.Format("The file [{0}] could not be identified as a valid Animal Crossing save file.\nPlease ensure you have a valid save file.",
                    save.Save_Name + save.Save_Extension), "Save File Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Save_File = null; //Set to null so we can set the checkbox to false without having the method run
            townMapViewCheckbox.Checked = false;
            townMapViewCheckbox.Enabled = save.Game_System == SaveGeneration.N64 || save.Game_System == SaveGeneration.GCN;
            acreHeightTrackBar.Enabled = save.Game_System == SaveGeneration.N64 || save.Game_System == SaveGeneration.GCN;
            Save_File = save;
            Debug_Manager.WriteLine("Save File Loaded");
            Acre_Height_Modifier = 0;
            Selected_Acre_ID = 0;
            selectedAcrePicturebox.Image = null;
            selectedAcrePicturebox.BackgroundImage = null;
            Buildings = null;
            Island_Buildings = null;
            Buried_Buffer = null;
            Island_Buried_Buffer = null;
            Selected_House = null;
            TPC_Picture.Image = Properties.Resources.no_tpc;
            Secure_NAND_Value_Form.Hide();
            Item_List = SaveDataManager.GetItemInfo(save.Save_Type).ToList(); //Probably should rename to GetItemDictionary or GetItemList
            Item_List.Sort((x, y) => x.Key.CompareTo(y.Key));
            ItemData.ItemDatabase = Item_List;
            selectedItem.DataSource = new BindingSource(Item_List, null);
            selectedItem.DisplayMember = "Value";
            selectedItem.ValueMember = "Key";
            //selectedItemText.Text = string.Format("Selected Item: [0x{0}]", (GetCurrentItem()).ToString("X4"));
            Current_Save_Info = SaveDataManager.GetSaveInfo(Save_File.Save_Type);
            if (save.Save_Type == SaveType.Wild_World)
            {
                Filed_Acre_Data = SaveDataManager.GetFiledAcreData(Save_File.Save_Type);
                UInt16_Filed_Acre_Data = null;
                UInt16_Acre_Info = null;
            }
            else
            {
                UInt16_Filed_Acre_Data = SaveDataManager.GetFiledAcreDataUInt16(Save_File.Save_Type);
                Filed_Acre_Data = null;
                Acre_Info = null;
            }

            if (save.Game_System == SaveGeneration.N64 || save.Game_System == SaveGeneration.GCN)
            {
                basementCheckBox.Enabled = true;
            }

            // Enable Controls
            playerName.Enabled = true;
            playerTownName.Enabled = true;
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
            secureValueToolStripMenuItem.Enabled = save.Game_System == SaveGeneration.N3DS;
            clearWeedsToolStripMenuItem.Enabled = true;
            removeAllItemsToolStripMenuItem.Enabled = true;
            replaceItemsToolStripMenuItem.Enabled = true;
            fillEncyclopediaToolStripMenuItem.Enabled = true;
            getAllKKSongsToolStripMenuItem.Enabled = true;
            acreCustomIdBox.Enabled = true;
            selectedItem.Enabled = true;
            townNameBox.Enabled = true;
            buriedCheckbox.Enabled = true;
            grassTypeBox.Enabled = true;
            weatherComboBox.Enabled = true;
            nativeFruitBox.Enabled = true;
            houseSizeComboBox.Enabled = true;
            roofColorComboBox.Enabled = true;
            houseOwnerComboBox.Enabled = true;
            grassLevelBox.Enabled = true;
            setAllGrass.Enabled = true;
            reviveGrass.Enabled = true;
            removeGrass.Enabled = true;

            //Clear Acre Images
            if (Acre_Map != null)
                foreach (PictureBoxWithInterpolationMode Box in Acre_Map)
                    Box.BackgroundImage = null;
            if (Town_Acre_Map != null)
                foreach (PictureBoxWithInterpolationMode Box in Town_Acre_Map)
                    Box.BackgroundImage = null;

            //Clear Acre TreeView (Hopefully .Remove doesn't cause a Memory Leak)
            while (acreTreeView.Nodes.Count > 0)
            {
                while (acreTreeView.Nodes[0].Nodes.Count > 0)
                {
                    acreTreeView.Nodes[0].Nodes[0].Remove();
                }
                acreTreeView.Nodes[0].Remove();
            }

            // Load Houses
            Houses = HouseInfo.LoadHouses(save);
            Selected_House = Houses[0];

            // Clear House ComboBoxes Box
            roofColorComboBox.Items.Clear();
            houseSizeComboBox.Items.Clear();
            houseOwnerComboBox.Items.Clear();

            // Add "No One" Option to House Owner ComboBox
            houseOwnerComboBox.Items.Add("No One");

            // Add Roof Color Items
            roofColorComboBox.Items.AddRange(HouseInfo.GetRoofColors(save.Save_Type));
            roofColorComboBox.Enabled = roofColorComboBox.Items.Count > 0;

            //Load Villager Database
            if (Save_File.Save_Type != SaveType.City_Folk) //City Folk has completely editable villagers! That's honestly a pain for editing...
            {
                Villager_Database = VillagerInfo.GetVillagerDatabase(Save_File.Save_Type);
                Personality_Database = VillagerInfo.GetPersonalities(Save_File.Save_Type);
                Villager_Names = new string[Villager_Database.Count];
                for (int i = 0; i < Villager_Names.Length; i++)
                    Villager_Names[i] = Villager_Database.ElementAt(i).Value.Name;

                //Clear Villager Editor
                for (int i = villagerPanel.Controls.Count - 1; i > -1; i--)
                    if (villagerPanel.Controls[i] is Panel)
                        villagerPanel.Controls[i].Dispose();

                //Add Campsite Villager Database
                campsiteComboBox.DataSource = null;
                campsiteComboBox.SelectedValueChanged -= Campsite_EventHandler;
                campsiteComboBox.Items.Clear();
                campsiteComboBox.Enabled = true;
                if (Save_File.Save_Type == SaveType.Animal_Crossing || Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                {
                    campsiteComboBox.DataSource = new BindingSource(Villager_Database, null);
                    campsiteComboBox.DisplayMember = "Value";
                    campsiteComboBox.ValueMember = "Key";
                    //Load Campsite (or Igloo) Visitor
                    try
                    {
                        ushort Camper_ID = Save_File.ReadUInt16(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Campsite_Visitor, Save_File.Is_Big_Endian);
                        campsiteComboBox.SelectedValue = Camper_ID;

                        //Setup Campsite Event
                        campsiteComboBox.SelectedValueChanged += Campsite_EventHandler;
                    }
                    catch { } // TODO: Implement debug writeline.
                }
                else
                {
                    campsiteComboBox.Enabled = false;
                }
                //Set Caravan Availablity
                caravan1ComboBox.Enabled = Save_File.Save_Type == SaveType.Welcome_Amiibo;
                caravan2ComboBox.Enabled = caravan1ComboBox.Enabled;
            }

            //Clear Buildings
            for (int i = buildingsPanel.Controls.Count - 1; i > -1; i--)
                buildingsPanel.Controls[i].Dispose();

            //Set building panel visibility
            bool Visibility = (save.Save_Type == SaveType.City_Folk || save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo);
            buildingsPanel.Visible = Visibility;
            buildingsLabel.Visible = Visibility;
            townPanel.Size = new Size(Visibility ? townTab.Size.Width - 213 : townTab.Size.Width - 9, townTab.Size.Height);

            //Cleanup old dictionary
            if (Acre_Image_List != null)
            {
                foreach (KeyValuePair<string, Image> KVPair in Acre_Image_List)
                    if (KVPair.Value != null)
                    {
                        KVPair.Value.Dispose();
                    }
            }

            //Clear Past Villager Panel
            while (pastVillagersPanel.Controls.Count > 0)
                pastVillagersPanel.Controls[0].Dispose();

            // Clear Hair Preview Box
            if (hairPictureBox.Image != null)
            {
                var Image = hairPictureBox.Image;
                hairPictureBox.Image = null;
                Image.Dispose();
            }

            Acre_Image_List = AcreData.GetAcreImageSet(save.Save_Type);
            SetEnabledControls(Save_File.Save_Type);
            SetupPatternBoxes();
            playerFace.Items.Clear();
            playerHairColor.Items.Clear();
            playerHairType.Items.Clear();
            playerShoeColor.Items.Clear();
            playerFace.Text = "";
            playerHairColor.Text = "";
            playerHairType.Text = "";
            playerShoeColor.Text = "";
            if (save.Save_Type == SaveType.Wild_World)
            {
                inventoryPicturebox.Size = new Size(5 * 16 + 2, 3 * 16 + 2);
                foreach (string Face_Name in PlayerInfo.WW_Faces)
                    playerFace.Items.Add(Face_Name);
                foreach (string Hair_Color in PlayerInfo.WW_Hair_Colors)
                    playerHairColor.Items.Add(Hair_Color);
                foreach (string Hair_Style in PlayerInfo.WW_Hair_Styles)
                    playerHairType.Items.Add(Hair_Style);
                playerDebt.Text = save.ReadUInt32(Current_Save_Info.Save_Offsets.Debt).ToString();
            }
            else if (save.Save_Type == SaveType.City_Folk)
            {
                inventoryPicturebox.Size = new Size(5 * 16 + 2, 3 * 16 + 2);
                foreach (string Face_Name in PlayerInfo.WW_Faces)   //Same order as WW
                    playerFace.Items.Add(Face_Name);
                foreach (string Hair_Color in PlayerInfo.WW_Hair_Colors)
                    playerHairColor.Items.Add(Hair_Color);
                foreach (string Shoe_Color in PlayerInfo.CF_Shoe_Colors)
                    playerShoeColor.Items.Add(Shoe_Color);
                foreach (string Hair_Style in PlayerInfo.CF_Hair_Styles)
                    playerHairType.Items.Add(Hair_Style);
                Grass_Wear = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Grass_Wear, Current_Save_Info.Save_Offsets.Grass_Wear_Size);
            }
            else if (save.Game_System == SaveGeneration.N3DS)
            {
                inventoryPicturebox.Size = new Size(4 * 16 + 2, 4 * 16 + 2);
                Grass_Wear = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Grass_Wear, Current_Save_Info.Save_Offsets.Grass_Wear_Size);
                foreach (string Hair_Style in PlayerInfo.NL_Hair_Styles)
                    playerHairType.Items.Add(Hair_Style);
                foreach (string Hair_Color in PlayerInfo.NL_Hair_Colors)
                    playerHairColor.Items.Add(Hair_Color);
                foreach (string Eye_Color in PlayerInfo.NL_Eye_Colors)
                    playerEyeColor.Items.Add(Eye_Color);
                Secure_NAND_Value_Form.Set_Secure_NAND_Value(Save_File.ReadUInt64(0));
            }
            else if (save.Save_Type == SaveType.Doubutsu_no_Mori || save.Game_System == SaveGeneration.GCN)
            {
                foreach (string Face_Name in PlayerInfo.AC_Faces)
                    playerFace.Items.Add(Face_Name);
                inventoryPicturebox.Size = new Size(5 * 16 + 2, 3 * 16 + 2);
            }
            for (int i = 0; i < 4; i++)
                Players[i] = new NewPlayer(Save_File.Save_Data_Start_Offset
                    + Current_Save_Info.Save_Offsets.Player_Start + i * Current_Save_Info.Save_Offsets.Player_Size, i, Save_File);
            Selected_Player = Players.FirstOrDefault(o => o.Exists);
            //Temp
            if (Selected_Player == null)
                MessageBox.Show("No Player was found on the file!", "Player Find Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                Reload_Player(Selected_Player);
            SetPlayersEnabled();
            if (Selected_Player != null && Selected_Player.Exists)
                playerEditorSelect.SelectedIndex = Array.IndexOf(Players, Selected_Player);

            // Grass Type Stuff
            grassTypeBox.Items.Clear();
            grassTypeBox.Enabled = true;

            // Load islands if DnMe+
            SelectedIsland = null;
            islandSelectionTab.Visible = save.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus;
            if (save.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus)
            {
                Islands = new Island[4];

                for (int i = 0; i < 4; i++)
                {
                    int IslandOffset = save.Save_Data_Start_Offset + 0xC560 + i * 0x3860;
                    Islands[i] = new Island(IslandOffset, Players, save);
                }

                islandSelectionTab.SelectedIndex = 0;
                SelectedIsland = Islands[0];
            }

            //Load Town Info
            Acres = new Normal_Acre[Current_Save_Info.Acre_Count];
            Town_Acres = new Normal_Acre[Current_Save_Info.Town_Acre_Count];
            
            if (save.Save_Type == SaveType.Wild_World)
            {
                foreach (var Enum in Enum.GetValues(typeof(Wild_World_Grass_Type)))
                    grassTypeBox.Items.Add(Enum);

                Acre_Info = SaveDataManager.GetAcreInfo(SaveType.Wild_World);
                int x = 0;
                byte[] Acre_Data = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data, 36);
                Buried_Buffer = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Buried_Data, Current_Save_Info.Save_Offsets.Buried_Data_Size);
                for (int i = 0; i < 36; i++)
                {
                    ushort[] Items_Buff = new ushort[256];
                    if (i >= 7 && (i % 6 > 0 && i % 6 < 5) && i <= 28)
                    {
                        Items_Buff = save.ReadUInt16Array(save.Save_Data_Start_Offset +
                            Current_Save_Info.Save_Offsets.Town_Data + x * 512, 256, false);
                        Town_Acres[x] = new Normal_Acre(Acre_Data[i], x, Items_Buff, Buried_Buffer, SaveType.Wild_World);
                        x++;
                    }
                    Acres[i] = new Normal_Acre(Acre_Data[i], i);
                }
            }
            else if (save.Game_System == SaveGeneration.GCN || save.Game_System == SaveGeneration.N64)
            {
                foreach (var Enum in Enum.GetValues(typeof(Animal_Crossing_Grass_Type)))
                    grassTypeBox.Items.Add(Enum);

                UInt16_Acre_Info = SaveDataManager.GetAcreInfoUInt16(SaveType.Animal_Crossing);
                int x = 0;
                ushort[] Acre_Data = save.ReadUInt16Array(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data, Current_Save_Info.Acre_Count, true);
                Buried_Buffer = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Buried_Data, Current_Save_Info.Save_Offsets.Buried_Data_Size);
                Island_Buried_Buffer = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Island_Buried_Data, Current_Save_Info.Save_Offsets.Island_Buried_Size);
                for (int i = 0; i < Acre_Data.Length; i++)
                {
                    ushort[] Items_Buff = new ushort[256];
                    if (i >= Current_Save_Info.X_Acre_Count + 1 && (i % Current_Save_Info.X_Acre_Count > 0
                        && i % Current_Save_Info.X_Acre_Count < Current_Save_Info.X_Acre_Count - 1) && i <= 47)
                    {
                        Items_Buff = save.ReadUInt16Array(save.Save_Data_Start_Offset +
                            Current_Save_Info.Save_Offsets.Town_Data + x * 512, 256, true);
                        Town_Acres[x] = new Normal_Acre(Acre_Data[i], i, Items_Buff, Buried_Buffer, SaveType.Animal_Crossing, null, x);
                        x++;
                    }
                    Acres[i] = new Normal_Acre(Acre_Data[i], i);
                }
            }
            else if (save.Save_Type == SaveType.City_Folk)
            {
                foreach (var Enum in Enum.GetValues(typeof(City_Folk_Grass_Type)))
                    grassTypeBox.Items.Add(Enum);

                UInt16_Acre_Info = SaveDataManager.GetAcreInfoUInt16(SaveType.City_Folk);
                Buildings = ItemData.GetBuildings(save);
                int x = 0;
                ushort[] Acre_Data = save.ReadUInt16Array(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data, Current_Save_Info.Acre_Count, true);
                Buried_Buffer = save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Buried_Data, Current_Save_Info.Save_Offsets.Buried_Data_Size);
                for (int i = 0; i < Acre_Data.Length; i++)
                {
                    ushort[] Items_Buff = new ushort[256];
                    if (i >= Current_Save_Info.X_Acre_Count + 1 && (i % Current_Save_Info.X_Acre_Count > 0
                        && i % Current_Save_Info.X_Acre_Count < Current_Save_Info.X_Acre_Count - 1) && i <= 41)
                    {
                        Items_Buff = save.ReadUInt16Array(save.Save_Data_Start_Offset +
                            Current_Save_Info.Save_Offsets.Town_Data + x * 512, 256, true);
                        Town_Acres[x] = new Normal_Acre(Acre_Data[i], x, Items_Buff, Buried_Buffer, SaveType.City_Folk);
                        x++;
                    }
                    Acres[i] = new Normal_Acre(Acre_Data[i], i);
                }
            }
            else if (save.Game_System == SaveGeneration.N3DS)
            {
                foreach (var Enum in Enum.GetValues(typeof(New_Leaf_Grass_Type)))
                    grassTypeBox.Items.Add(Enum);

                UInt16_Acre_Info = SaveDataManager.GetAcreInfoUInt16(Save_File.Save_Type);
                if (Selected_Player != null)
                {
                    foreach (string Face_Name in Selected_Player.Data.Gender == 0 ? PlayerInfo.NL_Male_Faces : PlayerInfo.NL_Female_Faces)
                        playerFace.Items.Add(Face_Name);
                    playerFace.SelectedIndex = Selected_Player.Data.FaceType;
                }
                //Load Past Villagers for NL/WA
                Past_Villagers = new SimpleVillager[16];
                for (int i = 0; i < 16; i++)
                {
                    ushort Villager_ID = save.ReadUInt16(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Past_Villagers + i * 2);
                    Past_Villagers[i] = Villager_Database.Values.FirstOrDefault(o => o.Villager_ID == Villager_ID);
                }
                GeneratePastVillagersPanel();
                //UInt16_Acre_Info = SaveDataManager.GetAcreInfoUInt16(SaveType.New_Leaf);
                Buildings = ItemData.GetBuildings(save);
                Island_Buildings = ItemData.GetBuildings(save, true);
                int x = 0;
                ushort[] Acre_Data = save.ReadUInt16Array(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data, Current_Save_Info.Acre_Count, false);
                for (int i = 0; i < Acre_Data.Length; i++)
                {
                    uint[] Items_Buff = new uint[256];
                    if (i >= Current_Save_Info.X_Acre_Count + 1 && (i % Current_Save_Info.X_Acre_Count > 0
                        && i % Current_Save_Info.X_Acre_Count < Current_Save_Info.X_Acre_Count - 1) && i <= 33)
                    {
                        Items_Buff = save.ReadUInt32Array(save.Save_Data_Start_Offset +
                            Current_Save_Info.Save_Offsets.Town_Data + x * 1024, 256, false);
                        Town_Acres[x] = new Normal_Acre(Acre_Data[i], x, Items_Buff, Buried_Buffer, SaveType.New_Leaf);
                        x++;
                    }
                    Acres[i] = new Normal_Acre(Acre_Data[i], i);
                }
            }

            townNameBox.MaxLength = Current_Save_Info.Save_Offsets.Town_NameSize;
            townNameBox.Text = new ACString(save.ReadByteArray(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Town_Name,
                Current_Save_Info.Save_Offsets.Town_NameSize), save.Save_Type).Trim();

            SetupAcreEditorTreeView();
            SetupMapPictureBoxes();
            SetupIslandHouseBoxes();

            if (Buildings != null)
               SetupBuildingList();

            //Load Villagers
            if (Save_File.Save_Type != SaveType.City_Folk)
            {
                Villagers = new NewVillager[Current_Save_Info.Villager_Count];
                for (int i = 0; i < Villagers.Length; i++)
                    if (Save_File.Save_Type == SaveType.Animal_Crossing || Save_File.Save_Type == SaveType.Doubutsu_no_Mori_Plus)
                    {
                        if (i < 15)
                        {
                            Villagers[i] = new NewVillager(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Villager_Data + Current_Save_Info.Save_Offsets.Villager_Size * i, i, save);
                        }
                        else
                        {
                            Villagers[i] = new NewVillager(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Islander_Data, i, save);
                        }
                    }
                    else
                    {
                        Villagers[i] = new NewVillager(save.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Villager_Data + Current_Save_Info.Save_Offsets.Villager_Size * i, i, save);
                    }
                for (int i = villagerPanel.Controls.Count - 1; i > -1; i--)
                    if (villagerPanel.Controls[i] is Panel)
                    {
                        villagerPanel.Controls[i].Dispose();
                    }
                foreach (NewVillager v in Villagers)
                    GenerateVillagerPanel(v);
            }

            // Set House Owner ComboBox List
            for (int i = 0; i < 4; i++)
            {
                if (Players[i] != null && Players[i].Exists)
                {
                    houseOwnerComboBox.Items.Add(Players[i].Data.Name);
                }
            }

            // Draw House PictureBoxes
            SetupHousePictureBoxes();
            if (roofColorComboBox.Enabled && Selected_House != null)
                roofColorComboBox.SelectedIndex = Math.Min(roofColorComboBox.Items.Count - 1, Selected_House.Data.Roof_Color);

            if (Properties.Settings.Default.OutputInt32s && Save_File.Game_System == SaveGeneration.N3DS)
                Utility.Scan_For_NL_Int32();

            // Set TextBox max values
            playerTownName.MaxLength = Current_Save_Info.Save_Offsets.Town_NameSize;
            playerName.MaxLength = Current_Save_Info.Save_Offsets.Town_NameSize; // As far as I know, town name and player name are always the same size

            // Enable Tasks
            clearWeedsToolStripMenuItem.Enabled = true;
            removeAllItemsToolStripMenuItem.Enabled = true;
            waterFlowersToolStripMenuItem.Enabled = Save_File.Game_System != SaveGeneration.GCN && Save_File.Game_System != SaveGeneration.N64;
            makeFruitsPerfectToolStripMenuItem.Enabled = Save_File.Game_System == SaveGeneration.N3DS;
            replaceItemsToolStripMenuItem.Enabled = true;

            // Set Grass Type
            if (Current_Save_Info.Save_Offsets.Grass_Type != -1)
            {
                byte Grass_Type = Save_File.ReadByte(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Grass_Type);
                grassTypeBox.SelectedIndex = (Save_File.Game_System == SaveGeneration.NDS ? Utility.GetWildWorldGrassBaseType(Grass_Type) : (Grass_Type < 3 ? Grass_Type : 0));
            }

            // Set Native Fruit Types
            SetPossibleNativeFruits(save.Game_System);

            // Set Weather Info
            weatherComboBox.Enabled = true;
            weatherComboBox.Items.Clear();
            weatherComboBox.Items.AddRange(Weather.GetWeatherTypesForGame(save.Game_System));
            if (Current_Save_Info.Save_Offsets.Weather != -1)
                weatherComboBox.SelectedIndex = Weather.GetWeatherIndex(save.ReadByte(Current_Save_Info.Save_Offsets.Weather), save.Game_System);

            // Load islands if DnM+/AC
            IslandRoom = null;
            if (Save_File.Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_File.Save_Type == SaveType.Animal_Crossing)
            {
                IslandRoom = new Room
                {
                    Offset = save.Save_Data_Start_Offset + save.Save_Info.Save_Offsets.Island_House,
                    Layers = new Layer[4]
                };

                for (int y = 0; y < 4; y++)
                {
                    Layer L = new Layer
                    {
                        Items = new Furniture[256]
                    };
                    for (int x = 0; x < 256; x++)
                    {
                        L.Items[x] = new Furniture(save.ReadUInt16(IslandRoom.Offset + y * 0x200 + x * 2, true));
                    }
                    IslandRoom.Layers[y] = L;
                    Refresh_PictureBox_Image(Island_House_Boxes[y], ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, L.Items, save.Save_Type), L.Items));
                }
            }

            Loading = false;
        }

        private void SetPlayersEnabled()
        {
            for (int i = 0; i < 4; i++)
                if (Players[i] != null)
                    if (Players[i].Exists && playerEditorSelect.TabPages.IndexOf(Player_Tabs[i]) < 0)
                        playerEditorSelect.TabPages.Insert(i, Player_Tabs[i]);
                    else if (!Players[i].Exists && playerEditorSelect.TabPages.IndexOf(Player_Tabs[i]) > -1)
                        playerEditorSelect.TabPages.Remove(Player_Tabs[i]);
        }

        private void SetMainTabEnabled(string tabName, bool enabled)
        {
            IntPtr h = tabControl1.Handle; //Necessary to use TabPages.Insert... (Can switch to tabControl1.CreateControl()) if wanted
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
                for (int i = 0; i < Main_Tabs.Length; i++)
                    if (Main_Tabs[i].Name == tabName)
                    {
                        if (i >= playerEditorSelect.TabCount)
                            tabControl1.TabPages.Add(Main_Tabs[i]);
                        else
                            tabControl1.TabPages.Insert(i, Main_Tabs[i]);
                    }
            }
        }

        private void SetEnabledControls(SaveType Current_Save_Type)
        {
            Text = string.Format("ACSE - {1} - [{0}]", Current_Save_Type.ToString().Replace("_", " ").Replace(" Plus", "+"), Save_File.Save_Name);
            itemFlag1.Text = "00";
            itemFlag2.Text = "00";

            //Load all tabs so alignment is kept
            SetMainTabEnabled("islandTab", true);
            SetMainTabEnabled("grassTab", true);

            playerSavings.Enabled = Current_Save_Type != SaveType.Doubutsu_no_Mori;
            tanTrackbar.Enabled = Current_Save_Type != SaveType.Doubutsu_no_Mori;

            if (Current_Save_Type == SaveType.Doubutsu_no_Mori || Current_Save_Type == SaveType.Doubutsu_no_Mori_Plus
                || Current_Save_Type == SaveType.Animal_Crossing || Current_Save_Type == SaveType.Doubutsu_no_Mori_e_Plus)
            {
                SetMainTabEnabled("islandTab", Current_Save_Type == SaveType.Doubutsu_no_Mori ? false : true);
                SetMainTabEnabled("grassTab", false);
                playerTownName.Enabled = true;
                playerHairType.Enabled = false;
                playerHairColor.Enabled = false;
                playerEyeColor.Enabled = false;
                playerNookPoints.Enabled = false;
                bedPicturebox.Image = Properties.Resources.X;
                bedPicturebox.Enabled = false;
                hatPicturebox.Image = Properties.Resources.X;
                hatPicturebox.Enabled = false;
                pantsPicturebox.Image = Properties.Resources.X;
                pantsPicturebox.Enabled = false;
                facePicturebox.Image = Properties.Resources.X;
                facePicturebox.Enabled = false;
                socksPicturebox.Image = Properties.Resources.X;
                socksPicturebox.Enabled = false;
                shoesPicturebox.Image = Properties.Resources.X;
                shoesPicturebox.Enabled = false;
                playerWetsuit.Enabled = false;
                playerWetsuit.Image = Properties.Resources.X;
                playerShoeColor.Enabled = false;
                pocketsBackgroundPicturebox.Enabled = true;
                playerIslandMedals.Enabled = false;
                playerMeowCoupons.Enabled = false;
                itemFlag1.Enabled = false;
                itemFlag2.Enabled = false;
                dresserAPictureBox.Visible = false;
                dresserAPictureBox.Enabled = false;
                islandPictureBox.Visible = false;
                islandPictureBox.Enabled = false;
                dresserText.Visible = false;
                islandBoxText.Visible = false;
                tanTrackbar.Maximum = 9;
            }
            else if (Current_Save_Type == SaveType.Wild_World)
            {
                SetMainTabEnabled("islandTab", false);
                SetMainTabEnabled("grassTab", false);
                playerTownName.Enabled = true;
                playerHairType.Enabled = true;
                playerHairColor.Enabled = true;
                playerNookPoints.Enabled = true;
                bedPicturebox.Enabled = true;
                playerEyeColor.Enabled = false;
                hatPicturebox.Enabled = true;
                pantsPicturebox.Image = Properties.Resources.X;
                pantsPicturebox.Enabled = false;
                facePicturebox.Enabled = true;
                socksPicturebox.Image = Properties.Resources.X;
                socksPicturebox.Enabled = false;
                shoesPicturebox.Image = Properties.Resources.X;
                shoesPicturebox.Enabled = false;
                playerWetsuit.Enabled = false;
                playerWetsuit.Image = Properties.Resources.X;
                playerShoeColor.Enabled = false;
                pocketsBackgroundPicturebox.Enabled = true;
                playerIslandMedals.Enabled = false;
                playerMeowCoupons.Enabled = false;
                itemFlag1.Enabled = false;
                itemFlag2.Enabled = false;
                dresserAPictureBox.Visible = true;
                dresserAPictureBox.Enabled = true;
                islandPictureBox.Visible = false;
                islandPictureBox.Enabled = false;
                dresserText.Visible = true;
                islandBoxText.Visible = false;
                tanTrackbar.Maximum = 4;
            }
            else if (Current_Save_Type == SaveType.City_Folk)
            {
                SetMainTabEnabled("islandTab", false);
                SetMainTabEnabled("grassTab", true);
                playerTownName.Enabled = true;
                playerHairType.Enabled = true;
                playerHairColor.Enabled = true;
                playerNookPoints.Enabled = true;
                playerEyeColor.Enabled = false;
                bedPicturebox.Enabled = true;
                hatPicturebox.Enabled = true;
                pantsPicturebox.Image = Properties.Resources.X;
                pantsPicturebox.Enabled = false;
                facePicturebox.Enabled = true;
                socksPicturebox.Image = Properties.Resources.X;
                socksPicturebox.Enabled = false;
                shoesPicturebox.Image = Properties.Resources.X;
                shoesPicturebox.Enabled = false;
                playerWetsuit.Enabled = false;
                playerWetsuit.Image = Properties.Resources.X;
                playerShoeColor.Enabled = true;
                pocketsBackgroundPicturebox.Image = Properties.Resources.X;
                pocketsBackgroundPicturebox.Enabled = false;
                playerIslandMedals.Enabled = false;
                playerMeowCoupons.Enabled = false;
                itemFlag1.Enabled = false;
                itemFlag2.Enabled = false;
                dresserAPictureBox.Visible = true;
                dresserAPictureBox.Enabled = true;
                islandPictureBox.Visible = false;
                islandPictureBox.Enabled = false;
                dresserText.Visible = true;
                islandBoxText.Visible = false;
                tanTrackbar.Maximum = 8;
            }
            else if (Current_Save_Type == SaveType.New_Leaf || Current_Save_Type == SaveType.Welcome_Amiibo)
            {
                SetMainTabEnabled("islandTab", true);
                SetMainTabEnabled("grassTab", true);
                playerTownName.Enabled = false;
                playerHairType.Enabled = true;
                playerHairColor.Enabled = true;
                playerEyeColor.Enabled = true;
                bedPicturebox.Image = Properties.Resources.X;
                bedPicturebox.Enabled = false;
                playerNookPoints.Enabled = false;
                hatPicturebox.Enabled = true;
                pantsPicturebox.Enabled = true;
                facePicturebox.Enabled = true;
                socksPicturebox.Enabled = true;
                shoesPicturebox.Enabled = true;
                playerShoeColor.Enabled = false;
                pocketsBackgroundPicturebox.Image = Properties.Resources.X;
                pocketsBackgroundPicturebox.Enabled = false;
                playerWetsuit.Enabled = true;
                playerIslandMedals.Enabled = true;
                itemFlag1.Enabled = true;
                itemFlag2.Enabled = true;
                playerMeowCoupons.Enabled = Current_Save_Type == SaveType.Welcome_Amiibo;
                dresserAPictureBox.Visible = true;
                dresserAPictureBox.Enabled = true;
                islandPictureBox.Visible = true;
                islandPictureBox.Enabled = true;
                dresserText.Visible = true;
                islandBoxText.Visible = true;
                tanTrackbar.Maximum = 16;
            }
        }

        private void Refresh_PictureBox_Image(PictureBox Box, Image New_Image, bool Background_Image = false, bool Dispose = true)
        {
            if (Box != null)
            {
                var Old_Image = Background_Image ? Box.BackgroundImage : Box.Image;
                if (Background_Image)
                    Box.BackgroundImage = New_Image;
                else
                    Box.Image = New_Image;
                if (Dispose && Old_Image != null)
                    Old_Image.Dispose();
            }
        }

        private void Reload_Player(NewPlayer Player)
        {
            //TODO: Hook up face swap on gender change for New Leaf
            if (Save_File.Save_Type != SaveType.New_Leaf && Save_File.Save_Type != SaveType.Welcome_Amiibo)
            {
                playerWallet.Text = Player.Data.Bells.ToString();
                if (Save_File.Save_Type != SaveType.Wild_World)
                    playerDebt.Text = Player.Data.Debt.ToString();
                playerSavings.Text = Player.Data.Savings.ToString();
            }
            playerName.Text = Player.Data.Name;
            Refresh_PictureBox_Image(inventoryPicturebox, Inventory.GetItemPic(16, inventoryPicturebox.Width / 16, Player.Data.Pockets.Items, Save_File.Save_Type));
            playerGender.SelectedIndex = Player.Data.Gender == 0 ? 0 : 1;
            Refresh_PictureBox_Image(shirtPicturebox, Inventory.GetItemPic(16, Player.Data.Shirt, Save_File.Save_Type));
            Refresh_PictureBox_Image(heldItemPicturebox, Inventory.GetItemPic(16, Player.Data.HeldItem, Save_File.Save_Type));

            //Birthday
            if (Player.Data.Birthday != null)
            {
                birthdayMonth.SelectedIndex = Player.Data.Birthday.Month < 13 ? (int)Player.Data.Birthday.Month - 1 : 12;
                Birthday_Month_FocusLost(); // Update days
                try { birthdayDay.SelectedIndex = Player.Data.Birthday.Day < 32 ? (int)Player.Data.Birthday.Day - 1 : (birthdayDay.Items.Count - 1); } catch { birthdayDay.SelectedIndex = 0; }
            }

            //These vary game to game
            if (playerFace.Items.Count > 0)
                playerFace.SelectedIndex = Player.Data.FaceType;
            if (playerHairType.Items.Count > 0)
                playerHairType.SelectedIndex = Player.Data.HairType;
            if (playerHairColor.Enabled && playerHairColor.Items.Count > 0)
                playerHairColor.SelectedIndex = Player.Data.HairColor;
            if (playerTownName.Enabled)
                playerTownName.Text = Player.Data.TownName;
            if (playerNookPoints.Enabled)
                playerNookPoints.Text = Player.Data.NookPoints.ToString();
            if (bedPicturebox.Enabled)
                Refresh_PictureBox_Image(bedPicturebox, Inventory.GetItemPic(16, Player.Data.Bed, Save_File.Save_Type));
            if (hatPicturebox.Enabled)
                Refresh_PictureBox_Image(hatPicturebox, Inventory.GetItemPic(16, Player.Data.Hat, Save_File.Save_Type));
            if (facePicturebox.Enabled)
                Refresh_PictureBox_Image(facePicturebox, Inventory.GetItemPic(16, Player.Data.FaceItem, Save_File.Save_Type));
            if (pocketsBackgroundPicturebox.Enabled)
                Refresh_PictureBox_Image(pocketsBackgroundPicturebox, Inventory.GetItemPic(16, Player.Data.InventoryBackground, Save_File.Save_Type));
            //City Folk only
            if (playerShoeColor.Enabled)
                playerShoeColor.SelectedIndex = Player.Data.ShoeColor;
            //New Leaf only
            if (pantsPicturebox.Enabled)
                Refresh_PictureBox_Image(pantsPicturebox, Inventory.GetItemPic(16, Player.Data.Pants, Save_File.Save_Type));
            if (socksPicturebox.Enabled)
                Refresh_PictureBox_Image(socksPicturebox, Inventory.GetItemPic(16, Player.Data.Socks, Save_File.Save_Type));
            if (shoesPicturebox.Enabled)
                Refresh_PictureBox_Image(shoesPicturebox, Inventory.GetItemPic(16, Player.Data.Shoes, Save_File.Save_Type));
            if (playerEyeColor.Enabled && playerEyeColor.Items.Count > 0)
                try { playerEyeColor.SelectedIndex = Player.Data.EyeColor; } catch { } // Breaks on Welcome Amiibo. Some update broke it? Look into it.
            if (playerWetsuit.Enabled)
                Refresh_PictureBox_Image(playerWetsuit, Inventory.GetItemPic(16, Player.Data.Wetsuit, Save_File.Save_Type));

            if (Player.Data.Tan <= tanTrackbar.Maximum)
                tanTrackbar.Value = Player.Data.Tan + 1;

            if (Player.Data.Patterns != null)
                for (int i = 0; i < Player.Data.Patterns.Length; i++)
                    if (Player.Data.Patterns[i] != null && Player.Data.Patterns[i].Pattern_Bitmap != null)
                        Refresh_PictureBox_Image(Pattern_Boxes[i], Player.Data.Patterns[i].Pattern_Bitmap, false, false);

            resettiCheckBox.Checked = Player.Data.Reset;

            if (Save_File.Game_System == SaveGeneration.N3DS)
            {
                Refresh_PictureBox_Image(TPC_Picture, Player.Data.TownPassCardImage, false, false);
                playerWallet.Text = Player.Data.NL_Wallet.Value.ToString();
                playerSavings.Text = Player.Data.NL_Savings.Value.ToString();
                playerDebt.Text = Player.Data.NL_Debt.Value.ToString();
                playerIslandMedals.Text = Player.Data.Island_Medals.Value.ToString();
                if (Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    playerMeowCoupons.Text = Player.Data.MeowCoupons.Value.ToString();
            }

            // Set Face Image
            Refresh_PictureBox_Image(facePreviewPictureBox, ImageGeneration.GetFaceImage(Save_File.Game_System, Player.Data.FaceType, Selected_Player.Data.Gender));
        }

        private void TownName_Box_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && !string.IsNullOrEmpty(townNameBox.Text.Trim()))
            {
                Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Town_Name, ACString.GetBytes(townNameBox.Text.Trim()));
                townNameBox.Text = townNameBox.Text.Trim();
            }
        }

        private void Gender_Changed()
        {
            if (Save_File == null)
                return;
            //New Leaf face swap on gender change
            if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
            {
                playerFace.Items.Clear();
                playerFace.Items.AddRange(playerGender.Text == "Male" ? PlayerInfo.NL_Male_Faces : PlayerInfo.NL_Female_Faces);
                playerFace.SelectedIndex = Selected_Player.Data.FaceType;
            }
            Selected_Player.Data.Gender = playerGender.Text == "Female" ? (byte)1 : (byte)0;
        }

        private void Face_Changed()
        {
            if (Save_File != null && Selected_Player != null && playerFace.SelectedIndex > -1)
            {
                Selected_Player.Data.FaceType = (byte)playerFace.SelectedIndex;
                Refresh_PictureBox_Image(facePreviewPictureBox, ImageGeneration.GetFaceImage(Save_File.Game_System, Selected_Player.Data.FaceType, Selected_Player.Data.Gender));
            }
        }

        private void Hair_Changed()
        {
            if (Save_File != null && Selected_Player != null && playerHairType.SelectedIndex > -1)
            {
                Selected_Player.Data.HairType = (byte)playerHairType.SelectedIndex;
                if (Save_File.Game_System == SaveGeneration.NDS || Save_File.Game_System == SaveGeneration.Wii || Save_File.Game_System == SaveGeneration.N3DS)
                    Refresh_PictureBox_Image(hairPictureBox, ImageGeneration.GetHairImage(Save_File.Game_System, Selected_Player.Data.HairType, Selected_Player.Data.HairColor));
            }
        }

        private void Hair_Color_Changed()
        {
            if (Save_File != null && Selected_Player != null && playerHairColor.SelectedIndex > -1)
            {
                Selected_Player.Data.HairColor = (byte)playerHairColor.SelectedIndex;
                if (Save_File.Game_System == SaveGeneration.NDS || Save_File.Game_System == SaveGeneration.Wii || Save_File.Game_System == SaveGeneration.N3DS)
                    Refresh_PictureBox_Image(hairPictureBox, ImageGeneration.GetHairImage(Save_File.Game_System, Selected_Player.Data.HairType, Selected_Player.Data.HairColor));
            }
        }

        private void Eye_Color_Changed()
        {
            if (Save_File != null && Selected_Player != null && playerEyeColor.SelectedIndex > -1)
            {
                Selected_Player.Data.EyeColor = (byte)playerEyeColor.SelectedIndex;
            }
        }

        private void Shoe_Color_Changed()
        {
            if (Save_File != null && Selected_Player != null && playerShoeColor.SelectedIndex > -1)
            {
                Selected_Player.Data.ShoeColor = (byte)playerShoeColor.SelectedIndex;
            }
        }

        private void SetupAcreEditorTreeView()
        {
            if (Filed_Acre_Data != null)
            {
                acreTreeView.Nodes.Clear();
                foreach (KeyValuePair<string, List<byte>> Acre_Group in Filed_Acre_Data)
                {
                    TreeNode Acre_Type = new TreeNode
                    {
                        Text = Acre_Group.Key
                    };
                    acreTreeView.Nodes.Add(Acre_Type);

                    foreach (byte Acre_ID in Acre_Group.Value)
                    {
                        TreeNode Acre = new TreeNode
                        {
                            Tag = Acre_ID.ToString("X2")
                        };
                        Acre.Text = Acre_Info != null ? Acre_Info[Acre_ID] : (string)Acre.Tag;
                        Acre.Name = (string)Acre.Tag;
                        Acre_Type.Nodes.Add(Acre);
                    }
                }
            }
            else if (UInt16_Filed_Acre_Data != null)
            {
                acreTreeView.Nodes.Clear();
                foreach (KeyValuePair<string, Dictionary<ushort, string>> Acre_Group in UInt16_Filed_Acre_Data)
                {
                    TreeNode Acre_Type = new TreeNode
                    {
                        Text = Acre_Group.Key
                    };
                    acreTreeView.Nodes.Add(Acre_Type);

                    foreach (KeyValuePair<ushort, string> Acre_Info in Acre_Group.Value)
                    {
                        TreeNode Acre = new TreeNode
                        {
                            Tag = Acre_Info.Key.ToString("X4"),
                            Text = Acre_Info.Value
                        };
                        Acre.Name = (string)Acre.Tag;
                        Acre_Type.Nodes.Add(Acre);
                    }
                }
            }
        }

        private void SetPossibleNativeFruits(SaveGeneration Save_Generation)
        {

            nativeFruitBox.Items.Clear();
            nativeFruitBox.Enabled = false;
            if (Current_Save_Info.Save_Offsets.NativeFruit > 0)
            {
                nativeFruitBox.Enabled = true;
                if (Save_Generation == SaveGeneration.N64 || Save_Generation == SaveGeneration.GCN)
                {
                    nativeFruitBox.Items.Add("Apple");
                    nativeFruitBox.Items.Add("Cherry");
                    nativeFruitBox.Items.Add("Pear");
                    nativeFruitBox.Items.Add("Peach");
                    nativeFruitBox.Items.Add("Orange");

                    int FruitIndex = Save_File.ReadUInt16(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.NativeFruit, true) - 0x2800;

                    if (FruitIndex > -1 && FruitIndex < nativeFruitBox.Items.Count)
                        nativeFruitBox.SelectedIndex = FruitIndex;
                }
            }
        }

        private void Item_Selected_Index_Changed(object sender, EventArgs e)
        {
            if (selectedItem.SelectedValue != null)
            {
                if (ushort.TryParse(selectedItem.SelectedValue.ToString(), out ushort Item_ID))
                    selectedItemText.Text = string.Format("Selected Item: [0x{0}]", Item_ID.ToString("X4"));
            }
        }

        private void Set_Selected_Acre(ushort AcreID)
        {
            Selected_Acre_ID = AcreID;
            if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
            {
                acreHeightTrackBar.Value = Selected_Acre_ID % 4;
                Acre_Height_Modifier = (ushort)acreHeightTrackBar.Value;
                Selected_Acre_ID -= (ushort)acreHeightTrackBar.Value;
            }
            string Acre_ID_Str = Selected_Acre_ID.ToString(Save_File.Save_Type == SaveType.Wild_World ? "X2 " : "X4");
            if (Save_File.Is_Big_Endian)
                selectedAcrePicturebox.Image = Acre_Image_List.ContainsKey(Acre_ID_Str) ? Acre_Image_List[Acre_ID_Str] : Acre_Image_List["FFFF"];
            else
                selectedAcrePicturebox.Image = Acre_Image_List.ContainsKey(Acre_ID_Str) ? Acre_Image_List[Acre_ID_Str] : Acre_Image_List["FF"];
            if (Save_File.Save_Type == SaveType.Wild_World)
                acreID.Text = "Acre ID: 0x" + Selected_Acre_ID.ToString("X2");
            else
                acreID.Text = "Acre ID: 0x" + (Selected_Acre_ID + Acre_Height_Modifier).ToString("X4");
            if (Acre_Info != null && Acre_Info.ContainsKey((byte)Selected_Acre_ID))
                acreDesc.Text = Acre_Info[(byte)AcreID];
            else if (UInt16_Acre_Info != null)
                acreDesc.Text = UInt16_Acre_Info.ContainsKey(Selected_Acre_ID) ? UInt16_Acre_Info[Selected_Acre_ID] : "No Acre Description";

            if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
            {
                if (Is_Ocean(AcreID))
                {
                    selectedAcrePicturebox.Image = Acre_Image_List["03DC"];
                }
            }

            foreach (TreeNode Node in acreTreeView.Nodes)
            {
                TreeNode[] Matching_Nodes = Node.Nodes.Find(Acre_ID_Str, true);
                if (Matching_Nodes.Length > 0)
                {
                    Node.Toggle();
                    acreTreeView.SelectedNode = Matching_Nodes[0];
                    acreTreeView.Focus();
                    return;
                }
            }
        }

        // TODO: Change byte based indexes to use Dictionary<ushort, Dictionary<ushort, string>>
        private void Acre_Tree_View_Entry_Clicked(object sender, TreeViewEventArgs e)
        {
            TreeNode Node = acreTreeView.SelectedNode;
            if (Node != null && Node.Tag != null)
            {
                if (Save_File.Is_Big_Endian)
                    selectedAcrePicturebox.Image = Acre_Image_List.ContainsKey((string)Node.Tag) ? Acre_Image_List[(string)Node.Tag] : Acre_Image_List["FFFF"];
                else
                    selectedAcrePicturebox.Image = Acre_Image_List.ContainsKey((string)Node.Tag) ? Acre_Image_List[(string)Node.Tag] : Acre_Image_List["FF"];
                if (Acre_Info != null)
                    Selected_Acre_ID = byte.Parse((string)Node.Tag, NumberStyles.AllowHexSpecifier);
                else if (UInt16_Filed_Acre_Data != null)
                    Selected_Acre_ID = ushort.Parse((string)Node.Tag, NumberStyles.AllowHexSpecifier);
                if (Save_File.Save_Type == SaveType.Wild_World)
                    acreID.Text = "Acre ID: 0x" + Selected_Acre_ID.ToString("X2");
                else
                    acreID.Text = "Acre ID: 0x" + (Selected_Acre_ID + Acre_Height_Modifier).ToString("X4");
                if (Acre_Info != null && Acre_Info.ContainsKey((byte)Selected_Acre_ID))
                    acreDesc.Text = Acre_Info[(byte)Selected_Acre_ID];
                else if (UInt16_Filed_Acre_Data != null)
                    acreDesc.Text = Node.Text;

                if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                {
                    if (Is_Ocean(Selected_Acre_ID))
                    {
                        selectedAcrePicturebox.Image = Acre_Image_List["03DC"];
                    }
                }

                // Warnings for N64/GameCube titles
                if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                {
                    if (Properties.Settings.Default.ShowBetaAcreWarning && (Node.Parent.Text.Equals("Beta Acres") || Node.Parent.Text.Equals("Misc. Acres")))
                    {
                        var Alert = new ToggableAlertForm("Placing beta acres in the Town Region (anywhere your map would show) will cause your game to crash when you open the map! It's recommended you only place them in border acres or ocean/island acres!",
                            "Beta Acre Warning");
                        if (Alert.ShowDialog() == DialogResult.OK)
                        {
                            Properties.Settings.Default.ShowBetaAcreWarning = !Alert.AlertDisabled;
                            Properties.Settings.Default.Save();
                        }
                    }
                    else if (Node.Tag != null && ushort.TryParse((string)Node.Tag, NumberStyles.AllowHexSpecifier, null, out ushort Acre_ID))
                    {
                        if (Properties.Settings.Default.ShowDumpAcreWarning && (Acre_ID == 0x0118 || Acre_ID == 0x0294 || Acre_ID == 0x0298))
                        {
                            var Alert = new ToggableAlertForm("Placing a dump acre without adding a dump item to the acre will cause your game to crash. Please be careful!", "Dump Placement Warning");
                            if (Alert.ShowDialog() == DialogResult.OK)
                            {
                                Properties.Settings.Default.ShowDumpAcreWarning = !Alert.AlertDisabled;
                                Properties.Settings.Default.Save();
                            }
                        }
                    }
                }
            }
        }

        private bool Is_Ocean(ushort ID)
        {
            return ((Save_File.Game_System == SaveGeneration.GCN || Save_File.Save_Type == SaveType.Doubutsu_no_Mori)
                && (ID >= 0x03DC && ID <= 0x03EC) || ID == 0x49C || (ID >= 0x04A8 && ID <= 0x058C) || (ID >= 0x05B4 && ID <= 0x05B8));
        }

        private Image Get_Acre_Image(Normal_Acre CurrentAcre, string Acre_ID_Str)
        {
            Image Acre_Image = null;
            if (Acre_Image_List != null)
            {
                if ((Save_File.Save_Type == SaveType.Doubutsu_no_Mori || Save_File.Game_System == SaveGeneration.GCN)
                    && ushort.TryParse(Acre_ID_Str, NumberStyles.AllowHexSpecifier, null, out ushort ID))
                {
                    
                    if (Is_Ocean(ID))
                    {
                        Acre_Image = Acre_Image_List["03DC"];
                    }
                    else if (Acre_Image_List.ContainsKey(Acre_ID_Str))
                    {
                        Acre_Image = Acre_Image_List[Acre_ID_Str];
                    }
                    else
                        Acre_Image = Acre_Image_List["FFFF"];
                }
                else if (Save_File.Save_Type == SaveType.City_Folk || Save_File.Game_System == SaveGeneration.N3DS)
                {
                    if (Acre_Image_List.ContainsKey(CurrentAcre.AcreID.ToString("X4")))
                        Acre_Image = Acre_Image_List[CurrentAcre.AcreID.ToString("X4")];
                    else
                        Acre_Image = Acre_Image_List["FFFF"];
                }
                else
                {
                    if (Acre_Image_List.ContainsKey(Acre_ID_Str))
                        Acre_Image = Acre_Image_List[Acre_ID_Str];
                    else if (Acre_Image_List.Keys.Contains("FF"))
                        Acre_Image = Acre_Image_List["FF"];
                }
            }
            return Acre_Image;
        }

        private void SetupMapPictureBoxes()
        {
            if (Acre_Map != null)
                for (int i = 0; i < Acre_Map.Length; i++)
                    Acre_Map[i].Dispose();
            if (Town_Acre_Map != null)
                for (int i = 0; i < Town_Acre_Map.Length; i++)
                {
                    var img = Town_Acre_Map[i].Image;
                    Town_Acre_Map[i].Dispose();
                    if (img != null)
                        img.Dispose();
                }
            if (Island_Acre_Map != null)
                for (int i = 0; i < Island_Acre_Map.Length; i++)
                    if (Island_Acre_Map[i] != null)
                    {
                        var img = Island_Acre_Map[i].Image;
                        Island_Acre_Map[i].Dispose();
                        if (img != null)
                            img.Dispose();
                    }
            if (NL_Island_Acre_Map != null)
                for (int i = 0; i < NL_Island_Acre_Map.Length; i++)
                    if (NL_Island_Acre_Map[i] != null)
                        NL_Island_Acre_Map[i].Dispose();
            if (Grass_Map != null)
                for (int i = 0; i < Grass_Map.Length; i++)
                {
                    var img = Grass_Map[i].Image;
                    Grass_Map[i].Dispose();
                    if (img != null)
                        img.Dispose();
                }
            Acre_Map = new PictureBoxWithInterpolationMode[Current_Save_Info.Acre_Count];
            Town_Acre_Map = new PictureBoxWithInterpolationMode[Current_Save_Info.Town_Acre_Count];
            if (Save_File.Save_Type == SaveType.City_Folk)
            {
                Grass_Map = new PictureBoxWithInterpolationMode[Town_Acre_Map.Length];
            }
            else
                Grass_Map = null;
            if (NL_Grass_Overlay != null)
                NL_Grass_Overlay.Dispose();
            NL_Grass_Overlay = null;

            //Assume First Acre Row + Side Acre Columns are Border Acres
            int Num_Y_Acres = Current_Save_Info.Acre_Count / Current_Save_Info.X_Acre_Count;
            int Town_Acre_Count = 0;
            for (int y = 0; y < Num_Y_Acres; y++)
                for (int x = 0; x < Current_Save_Info.X_Acre_Count; x++)
                {
                    int Acre = y * Current_Save_Info.X_Acre_Count + x;
                    Normal_Acre CurrentAcre = Acres[Acre];
                    string Acre_ID_Str = "";
                    if (Save_File.Save_Type == SaveType.Wild_World)
                        Acre_ID_Str = CurrentAcre.AcreID.ToString("X2");
                    else if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                        Acre_ID_Str = (CurrentAcre.AcreID - (CurrentAcre.AcreID % 4)).ToString("X4");

                    Image Acre_Image = Get_Acre_Image(CurrentAcre, Acre_ID_Str);

                    Acre_Map[Acre] = new PictureBoxWithInterpolationMode()
                    {
                        Size = new Size(AcreMapSize, AcreMapSize),
                        Location = new Point(x * AcreMapSize, (Acre / Current_Save_Info.X_Acre_Count) * AcreMapSize),
                        BackgroundImage = Acre_Image,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        InterpolationMode = InterpolationMode.HighQualityBicubic,
                        UseInternalInterpolationSetting = false,
                    };
                    
                    Acre_Map[Acre].MouseMove += new MouseEventHandler((object s, MouseEventArgs e) => Acre_Editor_Mouse_Move(s, e));
                    Acre_Map[Acre].MouseLeave += new EventHandler(Hide_Acre_Tip);
                    Acre_Map[Acre].MouseClick += new MouseEventHandler((object s, MouseEventArgs e) => Acre_Click(s, e));
                    Acre_Map[Acre].MouseEnter += new EventHandler(Acre_Editor_Mouse_Enter);
                    Acre_Map[Acre].MouseLeave += new EventHandler(Acre_Editor_Mouse_Leave);
                    acrePanel.Controls.Add(Acre_Map[Acre]);
                    if (y >= Current_Save_Info.Town_Y_Acre_Start && x > 0 && x < Current_Save_Info.X_Acre_Count - 1)
                    {
                        int Town_Acre = (y - Current_Save_Info.Town_Y_Acre_Start) * (Current_Save_Info.X_Acre_Count - 2) + (x - 1);
                        if (Town_Acre < Current_Save_Info.Town_Acre_Count)
                        {
                            Bitmap Town_Acre_Bitmap = GenerateAcreItemsBitmap(Town_Acres[Town_Acre_Count].Acre_Items, Town_Acre);
                            if (Town_Acre < Current_Save_Info.Town_Acre_Count)
                            {
                                Town_Acre_Map[Town_Acre] = new PictureBoxWithInterpolationMode()
                                {
                                    InterpolationMode = InterpolationMode.HighQualityBicubic,
                                    Size = new Size(TownMapTotalSize, TownMapTotalSize),
                                    Location = new Point((x - 1) * (TownMapTotalSize + 1), (Town_Acre / (Current_Save_Info.X_Acre_Count - 2)) * (TownMapTotalSize + 1)),
                                    Image = Town_Acres[Town_Acre_Count] != null ? Town_Acre_Bitmap : null,
                                    BackgroundImage = Acre_Image,
                                    BackgroundImageLayout = ImageLayout.Stretch,
                                    SizeMode = PictureBoxSizeMode.StretchImage,
                                    UseInternalInterpolationSetting = true,
                                };
                                Town_Acre_Count++;
                                Town_Acre_Map[Town_Acre].MouseMove += new MouseEventHandler((object sender, MouseEventArgs e) => Town_Move(sender, e));
                                Town_Acre_Map[Town_Acre].MouseLeave += new EventHandler(Hide_Town_Tip);
                                Town_Acre_Map[Town_Acre].MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) => Town_Mouse_Down(sender, e));
                                Town_Acre_Map[Town_Acre].MouseUp += new MouseEventHandler(Town_Mouse_Up);
                                townPanel.Controls.Add(Town_Acre_Map[Town_Acre]);
                                if (Grass_Map != null && Grass_Map.Length == Town_Acre_Map.Length)
                                {
                                    Grass_Map[Town_Acre] = new PictureBoxWithInterpolationMode()
                                    {
                                        InterpolationMode = InterpolationMode.NearestNeighbor,
                                        Size = new Size(96, 96),
                                        Location = new Point((x - 1) * 96, 30 + (Town_Acre / (Current_Save_Info.X_Acre_Count - 2) * 96)),
                                        Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear.Skip(Town_Acre * 256).Take(256).ToArray()),
                                        BackgroundImage = ImageGeneration.MakeGrayscale((Bitmap)Acre_Image),
                                        BackgroundImageLayout = ImageLayout.Stretch,
                                        SizeMode = PictureBoxSizeMode.StretchImage,
                                        UseInternalInterpolationSetting = true,
                                    };
                                    Grass_Map[Town_Acre].MouseDown += new MouseEventHandler(Grass_Editor_Mouse_Down);
                                    Grass_Map[Town_Acre].MouseUp += new MouseEventHandler(Grass_Editor_Mouse_Up);
                                    Grass_Map[Town_Acre].MouseMove += new MouseEventHandler(Grass_Editor_Mouse_Move);
                                    grassPanel.Controls.Add(Grass_Map[Town_Acre]);
                                }
                            }
                        }
                    }
                }
            if (Save_File.Game_System == SaveGeneration.N3DS)
            {
                if (NL_Grass_Overlay == null)
                {
                    NL_Grass_Overlay = new PictureBoxWithInterpolationMode
                    {
                        Size = new Size(96 * 7, 96 * 6),
                        Location = new Point(0, 30),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackgroundImageLayout = ImageLayout.Stretch,
                        InterpolationMode = InterpolationMode.NearestNeighbor,
                        UseInternalInterpolationSetting = true,
                    };
                    grassPanel.Controls.Add(NL_Grass_Overlay);
                    NL_Grass_Overlay.MouseDown += new MouseEventHandler(Grass_Editor_Mouse_Down);
                    NL_Grass_Overlay.MouseUp += new MouseEventHandler(Grass_Editor_Mouse_Up);
                    NL_Grass_Overlay.MouseMove += new MouseEventHandler(Grass_Editor_Mouse_Move);
                }
                NL_Grass_Overlay.Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear);
                NL_Grass_Overlay.BackgroundImage = ImageGeneration.Draw_NL_Grass_BG(Acre_Map);
                //Add events here
            }
            if (Current_Save_Info.Contains_Island)
            {
                Island_Acres = new Normal_Acre[Current_Save_Info.Island_Acre_Count];
                Island_Acre_Map = new PictureBoxWithInterpolationMode[Island_Acres.Length];
                NL_Island_Acre_Map = new PictureBoxWithInterpolationMode[16];

                for (int Y = 0; Y < Current_Save_Info.Island_Acre_Count / Current_Save_Info.Island_X_Acre_Count; Y++)
                    for (int X = 0; X < Current_Save_Info.Island_X_Acre_Count; X++)
                    {
                        int Idx = Y * Current_Save_Info.Island_X_Acre_Count + X;
                        ushort Acre_ID = Save_File.ReadUInt16(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Island_Acre_Data + Idx * 2, Save_File.Is_Big_Endian);
                        WorldItem[] Acre_Items = new WorldItem[256];
                        for (int i = 0; i < 256; i++)
                            if (Save_File.Game_System == SaveGeneration.GCN)
                                Acre_Items[i] = new WorldItem(Save_File.ReadUInt16(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Island_World_Data + Idx * 512 + i * 2, true), i);
                            else if ((Idx > 4 && Idx < 7) || (Idx > 8 && Idx < 11)) //Other acres are water acres
                            {
                                int World_Idx = (Y - 1) * 2 + ((X - 1) % 4);
                                Acre_Items[i] = new WorldItem(Save_File.ReadUInt32(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Island_World_Data + World_Idx * 1024 + i * 4), i);
                            }
                        Island_Acres[Idx] = new Normal_Acre(Acre_ID, Idx, Acre_Items, Island_Buried_Buffer); //Add buried item data for Animal Crossing
                        string Acre_ID_Str = Save_File.Game_System == SaveGeneration.GCN ? (Acre_ID - (Acre_ID % 4)).ToString("X4") : Island_Acres[Idx].AcreID.ToString("X2");

                        if (Save_File.Game_System == SaveGeneration.GCN || ((Idx > 4 && Idx < 7) || (Idx > 8 && Idx < 11)))
                        {
                            Island_Acre_Map[Idx] = new PictureBoxWithInterpolationMode
                            {
                                Size = new Size(TownMapTotalSize, TownMapTotalSize),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                BackgroundImageLayout = ImageLayout.Stretch,
                                InterpolationMode = InterpolationMode.HighQualityBicubic,
                                UseInternalInterpolationSetting = false,
                            };
                            if (Save_File.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus)
                            {
                                Island_Acre_Map[Idx].Image = GenerateAcreItemsBitmap(SelectedIsland.Items[Idx], Idx, true);
                                Island_Acre_Map[Idx].BackgroundImage = Get_Acre_Image(Acres[0x3C + Idx], Acres[0x3C + Idx].BaseAcreID.ToString("X4"));
                            }
                            else
                            {
                                Island_Acre_Map[Idx].Image = GenerateAcreItemsBitmap(Island_Acres[Idx].Acre_Items, Island_Acres[Idx].Index, true);
                                Island_Acre_Map[Idx].BackgroundImage = Get_Acre_Image(Island_Acres[Idx], Acre_ID_Str);
                            }
                            Island_Acre_Map[Idx].MouseMove += new MouseEventHandler((object sender, MouseEventArgs e) => Town_Move(sender, e, true));
                            Island_Acre_Map[Idx].MouseLeave += new EventHandler(Hide_Town_Tip);
                            Island_Acre_Map[Idx].MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) => Town_Mouse_Down(sender, e, true));
                            Island_Acre_Map[Idx].MouseUp += new MouseEventHandler(Town_Mouse_Up);
                            Island_Acre_Map[Idx].Location = (Save_File.Game_System == SaveGeneration.GCN)
                                ? new Point(X * TownMapTotalSize, Y * TownMapTotalSize) : new Point(((X - 1) % 4) * TownMapTotalSize, (Y - 1) * TownMapTotalSize);
                            islandPanel.Controls.Add(Island_Acre_Map[Idx]);
                        }
                        if (Save_File.Game_System == SaveGeneration.N3DS)
                        {
                            NL_Island_Acre_Map[Idx] = new PictureBoxWithInterpolationMode
                            {
                                Size = new Size(AcreMapSize, AcreMapSize),
                                Location = new Point(X * AcreMapSize, TownMapTotalSize * 2 + 24 + Y * AcreMapSize),
                                BackgroundImage = Get_Acre_Image(Island_Acres[Idx], Island_Acres[Idx].AcreID.ToString("X")),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                BackgroundImageLayout = ImageLayout.Stretch,
                                UseInternalInterpolationSetting = true,
                            };
                            NL_Island_Acre_Map[Idx].MouseMove += new MouseEventHandler((object s, MouseEventArgs e) => Acre_Editor_Mouse_Move(s, e, true));
                            NL_Island_Acre_Map[Idx].MouseLeave += new EventHandler(Hide_Acre_Tip);
                            NL_Island_Acre_Map[Idx].MouseClick += new MouseEventHandler((object s, MouseEventArgs e) => Acre_Click(s, e, true));
                            NL_Island_Acre_Map[Idx].MouseEnter += new EventHandler(Acre_Editor_Mouse_Enter);
                            NL_Island_Acre_Map[Idx].MouseLeave += new EventHandler(Acre_Editor_Mouse_Leave);
                            islandPanel.Controls.Add(NL_Island_Acre_Map[Idx]);
                        }
                    }
            }
        }

        #region Houses
        private int House_X = -1;
        private int House_Y = -1;
        private byte Clicking_House = 0;

        private void SetupHousePictureBoxes()
        {
            if (Save_File != null && Houses != null)
            {
                // Cleanup any existing controls
                housePanel.Controls.Clear();

                var HouseOffsets = HouseInfo.GetHouseOffsets(Save_File.Save_Type);
                var RoomNames = HouseInfo.GetRoomNames(Save_File.Game_System);

                if (Selected_House != null && (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN))
                {
                    basementCheckBox.Checked = HouseInfo.HasBasement(Selected_House.Offset, Save_File.Save_Type);
                }

                if (Selected_House != null)
                {
                    if (Selected_House.Owner != null)
                    {
                        var Owner_Index = -1;
                        for (int i = 0; i < houseOwnerComboBox.Items.Count; i++)
                        {
                            if (Selected_House.Owner.Data.Name.Equals(houseOwnerComboBox.Items[i]))
                            {
                                Owner_Index = i;
                                break;
                            }
                        }

                        if (Owner_Index < 0 || Owner_Index > 4) // TODO: Change this to the total count in 
                            houseOwnerComboBox.SelectedIndex = 0;
                        else
                            houseOwnerComboBox.SelectedIndex = Owner_Index;
                    }
                    else
                    {
                        houseOwnerComboBox.SelectedIndex = 0;
                    }
                }

                House_Boxes = new PictureBoxWithInterpolationMode[HouseOffsets.Room_Count][];

                for (int x = 0; x < HouseOffsets.Room_Count; x++)
                {
                    House_Boxes[x] = new PictureBoxWithInterpolationMode[HouseOffsets.Layer_Count];
                    Label RoomLabel = new Label
                    {
                        Text = RoomNames[x],
                        Size = new Size(256, 30),
                        Location = new Point(10 + x * 266, 20),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Microsoft Sans Serif", 18, FontStyle.Regular)
                    };
                    housePanel.Controls.Add(RoomLabel);

                    for (int y = 0; y < HouseOffsets.Layer_Count; y++)
                    {
                        PictureBoxWithInterpolationMode LayerBox = new PictureBoxWithInterpolationMode
                        {
                            Size = new Size(256, 256),
                            Location = new Point(10 + x * 266, 50 + y * 266),
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        House_Boxes[x][y] = LayerBox;

                        if (Selected_House != null)
                        {
                            var Current_Layer = Selected_House.Data.Rooms[x].Layers[y];
                            var LayerFurnitureMap = ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Selected_House.Data.Rooms[x].Layers[y].Items, Save_File.Save_Type),
                                Selected_House.Data.Rooms[x].Layers[y].Items);
                            LayerBox.Image = LayerFurnitureMap;

                            LayerBox.MouseMove += delegate (object sender, MouseEventArgs e)
                            {
                                if (e.X != House_X || e.Y != House_Y)
                                {
                                    int Item_Index = (e.X / 16) + (e.Y / 16) * ((sender as PictureBox).Width / 16);

                                    if (Item_Index < Current_Layer.Items.Length)
                                    {
                                        if (Clicking_House > 0)
                                        {
                                            if (Clicking_House == 1)
                                            {
                                                Current_Layer.Items[Item_Index] = new Furniture(GetCurrentItem());
                                                Refresh_PictureBox_Image(LayerBox, ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Current_Layer.Items, Save_File.Save_Type),
                                                    Current_Layer.Items));
                                                LayerBox.Refresh();
                                            }
                                            else if (Clicking_House == 2)
                                            {
                                                SetCurrentItem(Current_Layer.Items[Item_Index]);
                                            }
                                        }

                                        Item Selected_Item = Current_Layer.Items[Item_Index];
                                        houseToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Item.Name, Selected_Item.ItemID.ToString("X4")), sender as Control, e.X + 15, e.Y + 10);
                                        House_X = e.X;
                                        House_Y = e.Y;
                                    }
                                }
                            };

                            LayerBox.MouseLeave += delegate (object sender, EventArgs e)
                            {
                                houseToolTip.Hide(this);
                                Clicking_House = 0;
                            };

                            LayerBox.MouseDown += delegate (object sender, MouseEventArgs e)
                            {
                                int Item_Index = (e.X / 16) + (e.Y / 16) * ((sender as PictureBox).Width / 16);
                                if (e.Button == MouseButtons.Right && Item_Index < Current_Layer.Items.Length)
                                {
                                    SetCurrentItem(Current_Layer.Items[Item_Index]);
                                    Clicking_House = 2;
                                }
                                else if (e.Button == MouseButtons.Left && Item_Index < Current_Layer.Items.Length)
                                {
                                    Save_File.ChangesMade = true;
                                    Current_Layer.Items[Item_Index] = new Furniture(GetCurrentItem());
                                    Refresh_PictureBox_Image(LayerBox, ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Current_Layer.Items, Save_File.Save_Type),
                                        Current_Layer.Items));
                                    Clicking_House = 1;
                                }
                                else if (e.Button == MouseButtons.Middle)
                                {
                                    Save_File.ChangesMade = true;
                                    Clicking_House = 0;
                                    Utility.FloodFillFurnitureArray(ref Current_Layer.Items, 16, Item_Index, Current_Layer.Items[Item_Index], new Furniture(GetCurrentItem(), byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text)));
                                    Refresh_PictureBox_Image(LayerBox, ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Current_Layer.Items, Save_File.Save_Type),
                                        Current_Layer.Items));
                                }
                            };

                            LayerBox.MouseUp += delegate (object sender, MouseEventArgs e)
                            {
                                if ((e.Button == MouseButtons.Left && Clicking_House == 1) || (e.Button == MouseButtons.Right && Clicking_House == 2))
                                {
                                    Clicking_House = 0;
                                }
                            };
                        }

                        housePanel.Controls.Add(LayerBox);
                    }
                }
            }
        }

        private void BasementCheckBoxCheckChanged(object sender, EventArgs e)
        {
            if (!Loading && Save_File != null && (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN))
                HouseInfo.SetHasBasement(basementCheckBox.Checked, Selected_House);
        }

        private void House_Tab_Index_Changed(object sender, TabControlEventArgs e)
        {
            if (houseTabSelect.SelectedIndex < 0 || houseTabSelect.SelectedIndex > 3)
                return;
            Selected_House = Houses[houseTabSelect.SelectedIndex];
            if (Selected_House != null)
                ReloadHouse(Selected_House);
        }

        private void ReloadHouse(House SelectedHouse)
        {
            if (SelectedHouse != null)
            {
                if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                {
                    basementCheckBox.Checked = HouseInfo.HasBasement(SelectedHouse.Offset, Save_File.Save_Type);
                }

                if (SelectedHouse != null)
                {
                    if (SelectedHouse.Owner != null)
                    {
                        var Owner_Index = -1;
                        for (int i = 0; i < houseOwnerComboBox.Items.Count; i++)
                        {
                            if (SelectedHouse.Owner.Data.Name.Equals(houseOwnerComboBox.Items[i]))
                            {
                                Owner_Index = i;
                                break;
                            }
                        }

                        if (Owner_Index < 0 || Owner_Index > 4) // TODO: Change this to the total count in 
                            houseOwnerComboBox.SelectedIndex = 0;
                        else
                            houseOwnerComboBox.SelectedIndex = Owner_Index;
                    }
                    else
                    {
                        houseOwnerComboBox.SelectedIndex = 0;
                    }
                }

                roofColorComboBox.SelectedIndex = Math.Min(roofColorComboBox.Items.Count - 1, SelectedHouse.Data.Roof_Color);

                for (int i = 0; i < House_Boxes.Length; i++)
                {
                    for (int x = 0; x < House_Boxes[i].Length; x++)
                    {
                        if (SelectedHouse.Data.Rooms.Length > i && SelectedHouse.Data.Rooms[i].Layers.Length > x)
                        {
                            var Image = ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, SelectedHouse.Data.Rooms[i].Layers[x].Items, Save_File.Save_Type),
                                SelectedHouse.Data.Rooms[i].Layers[x].Items);
                            Refresh_PictureBox_Image(House_Boxes[i][x], Image);
                        }
                    }
                }
            }
        }

        #endregion

        #region Island Cottage
        private int Island_House_X = -1;
        private int Island_House_Y = -1;
        private byte Clicking_Island_House = 0;

        private void SetupIslandHouseBoxes()
        {
            if (Island_House_Boxes != null)
            {
                foreach (Control c in Island_House_Boxes)
                    c.Dispose();
            }

            if (Save_File.Game_System == SaveGeneration.GCN)
            {
                Island_House_Boxes = new PictureBoxWithInterpolationMode[4]; // Static size of four since the island house is only in DnM+, AC, and DnMe+

                for (int i = 0; i < 4; i++)
                {
                    Island_House_Boxes[i] = new PictureBoxWithInterpolationMode
                    {
                        Size = new Size(256, 256),
                        Location = new Point(40 + TownMapTotalSize * 2, i * 266),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    if (Save_File.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus && SelectedIsland != null) // Remove this after adding DnM+/AC support
                    {
                        var LayerFurnitureMap = ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, SelectedIsland.Cabana.MainRoom.Layers[i].Items, Save_File.Save_Type),
                            SelectedIsland.Cabana.MainRoom.Layers[i].Items);
                        Island_House_Boxes[i].Image = LayerFurnitureMap;
                    }

                    var LayerBox = Island_House_Boxes[i];

                    LayerBox.MouseMove += delegate (object sender, MouseEventArgs e)
                    {
                        if (e.X != Island_House_X || e.Y != Island_House_Y)
                        {
                            Layer Current_Layer = null;
                            if ((Save_File.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus && SelectedIsland != null)) // TODO: DnM+/AC support
                            {
                                Current_Layer = SelectedIsland.Cabana.MainRoom.Layers[Array.IndexOf(Island_House_Boxes, sender)];
                            }
                            else if ((Save_File.Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_File.Save_Type == SaveType.Animal_Crossing))
                            {
                                Current_Layer = IslandRoom.Layers[Array.IndexOf(Island_House_Boxes, sender)];
                            }

                            if (Current_Layer != null)
                            {
                                int Item_Index = (e.X / 16) + (e.Y / 16) * ((sender as PictureBox).Width / 16);

                                if (Item_Index < Current_Layer.Items.Length)
                                {
                                    if (Clicking_House > 0)
                                    {
                                        if (Clicking_House == 1)
                                        {
                                            Current_Layer.Items[Item_Index] = new Furniture(GetCurrentItem());
                                            Refresh_PictureBox_Image(LayerBox, ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Current_Layer.Items, Save_File.Save_Type),
                                                Current_Layer.Items));
                                            LayerBox.Refresh();
                                        }
                                        else if (Clicking_House == 2)
                                        {
                                            SetCurrentItem(Current_Layer.Items[Item_Index]);
                                        }
                                    }

                                    Item Selected_Item = Current_Layer.Items[Item_Index];
                                    houseToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Item.Name, Selected_Item.ItemID.ToString("X4")), sender as Control, e.X + 15, e.Y + 10);
                                    Island_House_X = e.X;
                                    Island_House_Y = e.Y;
                                }
                            }
                        }
                    };

                    LayerBox.MouseLeave += delegate (object sender, EventArgs e)
                    {
                        houseToolTip.Hide(this);
                        Clicking_Island_House = 0;
                    };

                    LayerBox.MouseDown += delegate (object sender, MouseEventArgs e)
                    {
                        Layer Current_Layer = null;
                        if (Save_File.Save_Type == SaveType.Doubutsu_no_Mori_e_Plus && SelectedIsland != null) // TODO: DnM+/AC
                        {
                            Current_Layer = SelectedIsland.Cabana.MainRoom.Layers[Array.IndexOf(Island_House_Boxes, sender)];
                        }
                        else if ((Save_File.Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_File.Save_Type == SaveType.Animal_Crossing))
                        {
                            Current_Layer = IslandRoom.Layers[Array.IndexOf(Island_House_Boxes, sender)];
                        }

                        if (Current_Layer != null)
                        {
                            int Item_Index = (e.X / 16) + (e.Y / 16) * ((sender as PictureBox).Width / 16);
                            if (e.Button == MouseButtons.Right && Item_Index < Current_Layer.Items.Length)
                            {
                                SetCurrentItem(Current_Layer.Items[Item_Index]);
                                Clicking_Island_House = 2;
                            }
                            else if (e.Button == MouseButtons.Left && Item_Index < Current_Layer.Items.Length)
                            {
                                Save_File.ChangesMade = true;
                                Current_Layer.Items[Item_Index] = new Furniture(GetCurrentItem());
                                Refresh_PictureBox_Image(LayerBox, ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Current_Layer.Items, Save_File.Save_Type),
                                    Current_Layer.Items));
                                Clicking_Island_House = 1;
                            }
                            else if (e.Button == MouseButtons.Middle)
                            {
                                Save_File.ChangesMade = true;
                                Clicking_Island_House = 0;
                                Utility.FloodFillFurnitureArray(ref Current_Layer.Items, 16, Item_Index, Current_Layer.Items[Item_Index], new Furniture(GetCurrentItem(), byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text)));
                                Refresh_PictureBox_Image(LayerBox, ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, Current_Layer.Items, Save_File.Save_Type),
                                    Current_Layer.Items));
                            }
                        }
                    };

                    LayerBox.MouseUp += delegate (object sender, MouseEventArgs e)
                    {
                        if ((e.Button == MouseButtons.Left && Clicking_Island_House == 1) || (e.Button == MouseButtons.Right && Clicking_Island_House == 2))
                        {
                            Clicking_Island_House = 0;
                        }
                    };

                    islandPanel.Controls.Add(Island_House_Boxes[i]);
                }
            }
        }

        #endregion

        #region Villagers
        private int Villager_X = -1, Villager_Y = -1;
        private void GenerateVillagerPanel(NewVillager Villager)
        {
            // TODO: Draw House Coordinate boxes for AC/WW, and also Furniture Boxes
            Panel Container = new Panel { Size = new Size(700, 30), Location = new Point(0, 20 + Villager.Index * 30) };
            Label Index = new Label { AutoSize = false, Size = new Size(45, 20), TextAlign = ContentAlignment.MiddleCenter,
                Text = ((Save_File.Save_Type == SaveType.Animal_Crossing || Save_File.Save_Type == SaveType.Doubutsu_no_Mori_Plus) && Villager.Index == 15) ? "Islander" : (Villager.Index + 1).ToString() };
            ComboBox Villager_Selection_Box = new ComboBox { Size = new Size(120, 30), Location = new Point(45, 0) };
            Villager_Selection_Box.Items.AddRange(Villager_Names);
            Villager_Selection_Box.SelectedIndex = Array.IndexOf(Villager_Database.Keys.ToArray(), Villager.Data.Villager_ID);
            ComboBox Personality_Selection_Box = new ComboBox { Size = new Size(80, 30), Location = new Point(175, 0), DropDownWidth = 120 };
            Personality_Selection_Box.Items.AddRange(Personality_Database);
            if (Save_File.Game_System == SaveGeneration.N3DS)
            {
                Personality_Selection_Box.SelectedIndex = Villager.Data.Personality < 9 ? Villager.Data.Personality : 8;
            }
            else
            {
                Personality_Selection_Box.SelectedIndex = Villager.Data.Personality < 7 ? Villager.Data.Personality : 6;
            }
            TextBox Villager_Catchphrase_Box = new TextBox { Size = new Size(100, 30), Location = new Point (265, 0), MaxLength = Villager.Offsets.CatchphraseSize, Text = Villager.Data.Catchphrase };
            CheckBox Boxed = new CheckBox { Size = new Size(22, 22), Location = new Point(375, 0), Checked = Villager.Data.Status == 1 };
            PictureBox Shirt_Box = new PictureBox {BorderStyle = BorderStyle.FixedSingle, Size = new Size(16, 16), Location = new Point(415, 3), Image = Inventory.GetItemPic(16, Villager.Data.Shirt, Save_File.Save_Type)};
            if (Save_File.Save_Type == SaveType.Wild_World || Save_File.Save_Type == SaveType.City_Folk || Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
            {
                //TODO: Add wallpaper/floor/song boxes
                PictureBox Furniture_Box = new PictureBox { BorderStyle = BorderStyle.FixedSingle, Size = new Size(16 * Villager.Data.Furniture.Length, 16), Location = new Point(441, 3) };
                Refresh_PictureBox_Image(Furniture_Box, Inventory.GetItemPic(16, Villager.Data.Furniture.Length, Villager.Data.Furniture, Save_File.Save_Type));

                Furniture_Box.MouseMove += delegate (object sender, MouseEventArgs e)
                {
                    if (e.X != Villager_X || e.Y != Villager_Y)
                    {
                        Item Selected_Item = Villager.Data.Furniture[e.X / 16];
                        villagerToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Item.Name, Selected_Item.ItemID.ToString("X4")), sender as Control, e.X + 15, e.Y + 10);
                        Villager_X = e.X;
                        Villager_Y = e.Y;
                    }
                };

                Furniture_Box.MouseLeave += delegate (object sender, EventArgs e)
                {
                    villagerToolTip.Hide(this);
                };

                Furniture_Box.MouseClick += delegate (object sender, MouseEventArgs e)
                {
                    int Item_Index = (e.X / 16) + (e.Y / 16) * ((sender as PictureBox).Width / 16);
                    if (e.Button == MouseButtons.Right)
                    {
                        //selectedItem.SelectedValue = Villager.Data.Furniture[Item_Index].ItemID;
                        SetCurrentItem(Villager.Data.Furniture[Item_Index]);
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        Save_File.ChangesMade = true;
                        Villager.Data.Furniture[Item_Index] = new Item(GetCurrentItem());
                        Refresh_PictureBox_Image(Furniture_Box, Inventory.GetItemPic(16, Villager.Data.Furniture.Length, Villager.Data.Furniture, Save_File.Save_Type));
                    }
                    else if (e.Button == MouseButtons.Middle)
                    {
                        Utility.FloodFillItemArray(ref Villager.Data.Furniture, 16, Item_Index, Villager.Data.Furniture[Item_Index], new Item(GetCurrentItem(), byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text)));
                        Refresh_PictureBox_Image(Furniture_Box, Inventory.GetItemPic(16, Villager.Data.Furniture.Length, Villager.Data.Furniture, Save_File.Save_Type));
                    }
                };
                Container.Controls.Add(Furniture_Box);
            }
            if (Save_File.Save_Type == SaveType.Animal_Crossing)
            {
                /*byte[] Location = Utility.Find_Villager_House(Villager.Data.Villager_ID);
                MessageBox.Show(string.Format("{0}, {1}, {2}, {3}",
                    Location[0] == Villager.Data.House_Coordinates[0], Location[1] == Villager.Data.House_Coordinates[1],
                    Location[2] == Villager.Data.House_Coordinates[2], Location[3] == Villager.Data.House_Coordinates[3]));*/
            }
            Villager_Selection_Box.SelectedIndexChanged += delegate (object sender, EventArgs e)
            {
                int Villager_Idx = Array.IndexOf(Villager_Names, Villager_Selection_Box.Text);
                if (Villager_Idx > -1)
                {
                    Villager.Data.Villager_ID = Villager_Database.Keys.ElementAt(Villager_Idx);
                    //Animal Crossing has a Villager "AI" byte. Normally its just the lower byte of the Villager ID, but it's always 0xFF for the Islander (setting a non-islander to it might cause problems).
                    if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                        Villager.Data.Villager_AI = (Villager.Index == 15) ? (byte)0xFF : (byte)(Villager.Data.Villager_ID & 0xFF);
                    if (Villager.Data.Villager_ID == 0 && Save_File.Game_System == SaveGeneration.GCN)
                    {
                        Villager.Data.House_Coordinates = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
                    }
                }
            };

            Personality_Selection_Box.SelectedIndexChanged += delegate (object sender, EventArgs e)
            {
                if (Personality_Selection_Box.SelectedIndex > -1)
                    Villager.Data.Personality = (byte)Personality_Selection_Box.SelectedIndex;
            };

            Villager_Catchphrase_Box.TextChanged += delegate (object sender, EventArgs e)
            {
                Villager.Data.Catchphrase = Villager_Catchphrase_Box.Text;
            };

            Shirt_Box.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (e.X != Villager_X || e.Y != Villager_Y)
                {
                    villagerToolTip.Show(string.Format("{0} - [0x{1}]", Villager.Data.Shirt.Name, Villager.Data.Shirt.ItemID.ToString("X4")), sender as Control, e.X + 15, e.Y + 10);
                    Villager_X = e.X;
                    Villager_Y = e.Y;
                }
            };

            Shirt_Box.MouseLeave += delegate (object sender, EventArgs e)
            {
                villagerToolTip.Hide(this);
            };

            Shirt_Box.MouseClick += delegate (object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    SetCurrentItem(Villager.Data.Shirt);
                    //selectedItem.SelectedValue = Villager.Data.Shirt.ItemID;
                    itemFlag1.Text = Villager.Data.Shirt.Flag1.ToString("X2");
                    itemFlag2.Text = Villager.Data.Shirt.Flag2.ToString("X2");
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Villager.Data.Shirt = new Item(GetCurrentItem(), byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text));
                    Refresh_PictureBox_Image(Shirt_Box, Inventory.GetItemPic(16, Villager.Data.Shirt, Save_File.Save_Type));
                }
            };

            Container.Controls.Add(Index);
            Container.Controls.Add(Villager_Selection_Box);
            Container.Controls.Add(Personality_Selection_Box);
            Container.Controls.Add(Villager_Catchphrase_Box);
            Container.Controls.Add(Boxed);
            Container.Controls.Add(Shirt_Box);
            villagerPanel.Controls.Add(Container);
        }

        private void GeneratePastVillagersPanel()
        {
            if (Past_Villagers != null)
            {
                for (int i = 0; i < Past_Villagers.Length - 1; i++)
                {
                    //Change to combobox after confirmed working
                    ComboBox Villager_Box = new ComboBox { Size = new Size(150, 20), Location = new Point(5, 5 + 24 * i) };
                    Villager_Box.Items.AddRange(Villager_Names);
                    Villager_Box.SelectedIndex = Array.IndexOf(Villager_Database.Keys.ToArray(), Past_Villagers[i].Villager_ID);
                    pastVillagersPanel.Controls.Add(Villager_Box);
                }
            }
        }

        #endregion

        private Bitmap GenerateAcreItemsBitmap(WorldItem[] items, int Acre, bool Island_Acre = false)
        {
            int itemSize = TownMapCellSize;
            int acreSize = TownMapTotalSize;
            int width = itemSize * 16;
            Bitmap acreBitmap = new Bitmap(acreSize, acreSize);
            byte[] bitmapBuffer = new byte[4 * (acreSize * acreSize)];
            for (int i = 0; i < 256; i++)
            {
                WorldItem item = items[i];
                if (item.Name != "Empty")
                {
                    uint itemColor = ItemData.GetItemColor(ItemData.GetItemType(item.ItemID, Save_File.Save_Type));
                    // Draw Item Box
                    for (int x = 0; x < itemSize * itemSize; x++)
                        Buffer.BlockCopy(BitConverter.GetBytes(itemColor), 0, bitmapBuffer,
                            ((item.Location.Y * itemSize + x / itemSize) * acreSize * 4) + ((item.Location.X * itemSize + x % itemSize) * 4), 4);
                }
            }
            // Draw Border
            //ImageGeneration.Draw_Grid(acreBitmap, 8);
            for (int i = 0; i < (width * width); i++)
                if ((i / itemSize > 0 && i % (itemSize * 16) > 0 && i % (itemSize) == 0) || (i / (itemSize * 16) > 0 && (i / (itemSize * 16)) % (itemSize) == 0))
                    Buffer.BlockCopy(BitConverter.GetBytes(0x56FFFFFF), 0, bitmapBuffer,
                        ((i / (itemSize * 16)) * width * 4) + ((i % (itemSize * 16)) * 4), 4);

            // Construct Bitmap
            BitmapData bitmapData = acreBitmap.LockBits(new Rectangle(0, 0, acreSize, acreSize), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bitmapBuffer, 0, bitmapData.Scan0, bitmapBuffer.Length);
            acreBitmap.UnlockBits(bitmapData);
            // Draw Buried X
            ImageGeneration.Draw_Buried_Icons(acreBitmap, items, TownMapCellSize);
            // Draw Buildings (if needed)
            if (Save_File.Game_System == SaveGeneration.Wii || Save_File.Game_System == SaveGeneration.N3DS)
            {
                return Island_Acre ? ((Save_File.Game_System == SaveGeneration.N3DS)
                    ? ImageGeneration.Draw_Buildings(acreBitmap, Island_Buildings, Acre - 5, TownMapCellSize) : acreBitmap)
                    : ImageGeneration.Draw_Buildings(acreBitmap, Buildings, Acre, TownMapCellSize);
            }
            else
            {
                return acreBitmap;
            }
        }

        //TODO: Remove this when I finish all methods
        private void Not_Implemented()
        {
            MessageBox.Show("Feature not yet implemented! Sorry for that.");
        }

        #region Grass
        private bool Grass_Editor_Down = false;

        private void Grass_Editor_Mouse_Down(object sender, MouseEventArgs e)
        {
            Grass_Editor_Mouse_Click(sender, e);
            Grass_Editor_Down = true;
        }

        private void Grass_Editor_Mouse_Up(object sender, MouseEventArgs e)
        {
            Grass_Editor_Down = false;
        }

        private void Grass_Editor_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (Grass_Editor_Down)
                Grass_Editor_Mouse_Click(sender, e);
        }

        private void Grass_Editor_Mouse_Click(object sender, MouseEventArgs e)
        {
            PictureBox Grass_Box = sender as PictureBox;
            Grass_Box.Capture = false;
            if (Save_File.Save_Type == SaveType.City_Folk)
            {
                int acre = Array.IndexOf(Grass_Map, sender);
                int x = e.X / 6, y = e.Y / 6;
                int block_x = x / 8, block_y = y / 4;
                int x_pos = x - (block_x * 8);
                int y_pos = y - (block_y * 4);
                int data_Pos = acre * 256 + block_y * 64 + block_x * 32 + y_pos * 8 + x_pos;
                if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    byte.TryParse(grassLevelBox.Text, out byte wear_Value);
                    Grass_Wear[data_Pos] = wear_Value;
                    var Old_Image = Grass_Box.Image;
                    Grass_Box.Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear.Skip(acre * 256).Take(256).ToArray());
                    if (Old_Image != null)
                        Old_Image.Dispose();
                    Grass_Box.Refresh();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    grassLevelBox.Text = Grass_Wear[data_Pos].ToString();
                }
            }
            else //NL
            {
                int X = e.X / 6;
                int Y = e.Y / 6;
                int Grass_Data_Offset = 64 * ((Y / 8) * 16 + (X / 8)) + ImageGeneration.Grass_Wear_Offset_Map[(Y % 8) * 8 + (X % 8)];

                if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    byte.TryParse(grassLevelBox.Text, out byte wear_Value);
                    Grass_Wear[Grass_Data_Offset] = wear_Value;
                    Grass_Box.Image.Dispose();
                    Grass_Box.Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear);
                    Grass_Box.Refresh();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    grassLevelBox.Text = Grass_Wear[Grass_Data_Offset].ToString();
                }
            }
        }
        #endregion

        #region Acres

        private Bitmap Acre_Highlight_Image = ImageGeneration.Draw_Acre_Highlight();
        private void Acre_Editor_Mouse_Enter(object sender, EventArgs e)
        {
            (sender as Control).Capture = false;
            (sender as PictureBox).Image = Acre_Highlight_Image;
            (sender as PictureBox).Refresh();
        }

        private void Acre_Editor_Mouse_Leave(object sender, EventArgs e)
        {
            (sender as Control).Capture = false;
            (sender as PictureBox).Image = null;
            (sender as PictureBox).Refresh();
        }

        int Last_Acre_X = -1, Last_Acre_Y = -1;
        private void Acre_Editor_Mouse_Move(object sender, MouseEventArgs e, bool Island = false, bool Override = false)
        {
            (sender as Control).Capture = false;
            (sender as PictureBox).Image = Acre_Highlight_Image;
            if (Override || e.X != Last_Acre_X || e.Y != Last_Acre_Y)
            {
                acreToolTip.Hide(this);
                acreToolTip.RemoveAll();
                int Acre_Idx = Array.IndexOf(Island ? NL_Island_Acre_Map : Acre_Map, sender as PictureBox);
                Acre Hovered_Acre = Island ? Island_Acres[Acre_Idx] : Acres[Acre_Idx];
                if (Acre_Info != null)
                    acreToolTip.Show(string.Format("{0}0x{1}", Acre_Info.ContainsKey((byte)Hovered_Acre.AcreID) ? Acre_Info[(byte)Hovered_Acre.AcreID] + " - " : "",
                        Hovered_Acre.AcreID.ToString("X2")), sender as Control, e.X + 15, e.Y + 10);
                else if (UInt16_Acre_Info != null)
                    if (Save_File.Save_Type == SaveType.Doubutsu_no_Mori || Save_File.Game_System == SaveGeneration.GCN)
                    {
                        acreToolTip.Show(string.Format("{0}[{2}] - 0x{1}", UInt16_Acre_Info.ContainsKey(Hovered_Acre.BaseAcreID) ? UInt16_Acre_Info[Hovered_Acre.BaseAcreID] + " "
                            : (Is_Ocean(Hovered_Acre.AcreID) ? "Ocean " : ""),
                            Hovered_Acre.AcreID.ToString("X4"), AcreData.Acre_Height_Identifiers[Hovered_Acre.AcreID % 4]), sender as Control, e.X + 15, e.Y + 10);
                    }
                    else
                        acreToolTip.Show(string.Format("{0}0x{1}", UInt16_Acre_Info.ContainsKey(Hovered_Acre.AcreID) ? UInt16_Acre_Info[Hovered_Acre.AcreID] + " - " : "",
                            Hovered_Acre.AcreID.ToString("X4")), sender as Control, e.X + 15, e.Y + 10);
                else
                    acreToolTip.Show("0x" + Hovered_Acre.AcreID.ToString("X"), sender as Control, e.X + 15, e.Y + 10);
                Last_Acre_X = e.X;
                Last_Acre_Y = e.Y;
            }
        }

        private void Hide_Acre_Tip(object sender, EventArgs e)
        {
            Last_Acre_X = -1;
            Last_Acre_Y = -1;
            acreToolTip.Hide(this);
        }

        private void Acre_Click(object sender, MouseEventArgs e, bool Island = false)
        {
            PictureBox Acre_Box = sender as PictureBox;
            int Acre_Index = Array.IndexOf(Island ? NL_Island_Acre_Map : Acre_Map, Acre_Box);
            if (Acre_Index > -1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    string Acre_ID_String = Selected_Acre_ID.ToString("X");
                    if (Island)
                        Island_Acres[Acre_Index] = new Normal_Acre((ushort)(Selected_Acre_ID + Acre_Height_Modifier), Acre_Index);
                    else
                        Acres[Acre_Index] = new Normal_Acre((ushort)(Selected_Acre_ID + Acre_Height_Modifier), Acre_Index);

                    Acre_Box.BackgroundImage = selectedAcrePicturebox.Image;
                    int Acre_X = Acre_Index % (Island ? Current_Save_Info.Island_X_Acre_Count : Current_Save_Info.X_Acre_Count);
                    int Acre_Y = Acre_Index / (Island ? Current_Save_Info.Island_X_Acre_Count : Current_Save_Info.X_Acre_Count);
                    if (!Island && Grass_Map != null && Grass_Map.Length == Acre_Map.Length)
                        Grass_Map[Acre_Index].BackgroundImage = Acre_Box.BackgroundImage;
                    if (!Island && Acre_Y >= Current_Save_Info.Town_Y_Acre_Start && Acre_X > 0 && Acre_X < Current_Save_Info.X_Acre_Count - 1)
                    {
                        int Town_Acre = (Acre_Y - Current_Save_Info.Town_Y_Acre_Start) * (Current_Save_Info.X_Acre_Count - 2) + (Acre_X - 1);
                        if (Town_Acre < Current_Save_Info.Town_Acre_Count)
                        {
                            Normal_Acre Current_Town_Acre = Town_Acres[Town_Acre];
                            Town_Acres[Town_Acre] = new Normal_Acre((ushort)(Selected_Acre_ID + Acre_Height_Modifier),
                                Town_Acre, Current_Town_Acre.Acre_Items, Buried_Buffer, Save_File.Save_Type);
                            Town_Acre_Map[Town_Acre].BackgroundImage = Acre_Box.BackgroundImage;
                        }
                        if (Grass_Map != null && Grass_Map.Length == Town_Acre_Map.Length)
                            Grass_Map[Town_Acre].BackgroundImage = ImageGeneration.MakeGrayscale((Bitmap)Acre_Box.BackgroundImage);
                        else if (NL_Grass_Overlay != null)
                        {
                            NL_Grass_Overlay.BackgroundImage.Dispose();
                            NL_Grass_Overlay.BackgroundImage = ImageGeneration.Draw_NL_Grass_BG(Acre_Map);
                        }
                    }
                    else if (Island && Acre_Y > 0 && Acre_X > 0 && Acre_Y < 3 && Acre_X < 3)
                    {
                        Island_Acre_Map[Acre_Index].BackgroundImage = selectedAcrePicturebox.Image;
                    }
                    Acre_Editor_Mouse_Move(sender, e, Island, true);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    selectedAcrePicturebox.Image = Acre_Box.BackgroundImage;
                    Selected_Acre_ID = Island ? Island_Acres[Acre_Index].AcreID : Acres[Acre_Index].AcreID;
                    string Acre_Str = Save_File.Game_System == SaveGeneration.NDS ? Selected_Acre_ID.ToString("X2") : Selected_Acre_ID.ToString("X4");
                    if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                    {
                        acreHeightTrackBar.Value = Selected_Acre_ID % 4;
                        Acre_Height_Modifier = (ushort)acreHeightTrackBar.Value;
                        Selected_Acre_ID -= (ushort)acreHeightTrackBar.Value;
                    }
                    if (Save_File.Save_Type == SaveType.Wild_World)
                        acreID.Text = "Acre ID: 0x" + Selected_Acre_ID.ToString("X2");
                    else if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN)
                        acreID.Text = "Acre ID: 0x" + (Selected_Acre_ID + Acre_Height_Modifier).ToString("X4");
                    else
                        acreID.Text = "Acre ID: 0x" + Selected_Acre_ID.ToString("X4");
                    if (Acre_Info != null)
                        acreDesc.Text = Acre_Info.ContainsKey((byte)Selected_Acre_ID) ? Acre_Info[(byte)Selected_Acre_ID] : "No Description";
                    else if (UInt16_Acre_Info != null)
                        if ((Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN) && UInt16_Acre_Info.ContainsKey((ushort)(Selected_Acre_ID - Selected_Acre_ID % 4)))
                            acreDesc.Text = UInt16_Acre_Info[(ushort)(Selected_Acre_ID - Selected_Acre_ID % 4)];
                        else if (UInt16_Acre_Info.ContainsKey(Selected_Acre_ID))
                            acreDesc.Text = UInt16_Acre_Info[Selected_Acre_ID];
                        else
                            acreDesc.Text = "No Acre Description";
                    else
                        acreDesc.Text = "No Acre Description";

                    foreach (TreeNode Node in acreTreeView.Nodes)
                    {
                        TreeNode[] Matching_Nodes = Node.Nodes.Find(Acre_Str, true);
                        if (Matching_Nodes.Length > 0)
                        {
                            Node.Toggle();
                            acreTreeView.SelectedNode = Matching_Nodes[0];
                            acreTreeView.Focus();
                            return;
                        }
                    }
                }
            }
        }

        #endregion

        #region Players

        private void Player_Tab_Index_Changed(object sender, TabControlEventArgs e)
        {
            if (playerEditorSelect.SelectedIndex < 0 || playerEditorSelect.SelectedIndex > 3)
                return;
            Selected_Player = Players[playerEditorSelect.SelectedIndex];
            if (Selected_Player != null && Selected_Player.Exists)
                Reload_Player(Selected_Player);
        }

        private int Last_X = 0, Last_Y = 0;
        private void Players_Mouse_Move(object sender, MouseEventArgs e)
        {
            if ((e.X != Last_X || e.Y != Last_Y) && Save_File != null)
            {
                playersToolTip.Hide(this);
                playersToolTip.RemoveAll();
                PictureBox Box = sender as PictureBox;
                Last_X = e.X;
                Last_Y = e.Y;
                if (Box == inventoryPicturebox)
                {
                    int Item_Index = (e.X / 16) + (e.Y / 16) * (Box.Width / 16);
                    if (Item_Index >= Selected_Player.Data.Pockets.Items.Length)
                        return;
                    Item Hovered_Item = Selected_Player.Data.Pockets.Items[Item_Index];
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Hovered_Item.Name, Hovered_Item.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                }
                else if (Box == heldItemPicturebox)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.HeldItem.Name, Selected_Player.Data.HeldItem.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == shirtPicturebox)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Shirt.Name, Selected_Player.Data.Shirt.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == hatPicturebox && hatPicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Hat.Name, Selected_Player.Data.Hat.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == facePicturebox && facePicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.FaceItem.Name, Selected_Player.Data.FaceItem.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == pantsPicturebox && pantsPicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Pants.Name, Selected_Player.Data.Pants.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == socksPicturebox && socksPicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Socks.Name, Selected_Player.Data.Socks.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == shoesPicturebox && shoesPicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Shoes.Name, Selected_Player.Data.Shoes.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == pocketsBackgroundPicturebox && pocketsBackgroundPicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.InventoryBackground.Name,
                        Selected_Player.Data.InventoryBackground.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == bedPicturebox && bedPicturebox.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Bed.Name, Selected_Player.Data.Bed.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
                else if (Box == playerWetsuit && playerWetsuit.Enabled)
                    playersToolTip.Show(string.Format("{0} - [0x{1}]", Selected_Player.Data.Wetsuit.Name, Selected_Player.Data.Wetsuit.ItemID.ToString("X4")), Box, e.X + 15, e.Y + 10);
            }
        }

        private void Hide_Tip(object sender, EventArgs e)
        {
            playersToolTip.Hide(this);
            Last_X = -1;
            Last_Y = -1;
        }

        private void Players_Mouse_Click(object sender, MouseEventArgs e)
        {
            if (Save_File == null)
                return;
            PictureBox ItemBox = sender as PictureBox;
            if (ItemBox == inventoryPicturebox)
            {
                int Item_Index = (e.X / 16) + (e.Y / 16) * (ItemBox.Width / 16);

                // Make sure Item_Index is in bounds
                if (Item_Index > Selected_Player.Data.Pockets.Items.Length - 1)
                    return;

                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Pockets.Items[Item_Index].ItemID;
                    SetCurrentItem(Selected_Player.Data.Pockets.Items[Item_Index]);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Pockets.Items[Item_Index] = new Item(GetCurrentItem());
                    inventoryPicturebox.Image = Inventory.GetItemPic(16, inventoryPicturebox.Size.Width / 16, Selected_Player.Data.Pockets.Items, Save_File.Save_Type);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    Utility.FloodFillItemArray(ref Selected_Player.Data.Pockets.Items, inventoryPicturebox.Size.Width / 16, Item_Index, Selected_Player.Data.Pockets.Items[Item_Index], new Item(GetCurrentItem(), byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text)));
                    Refresh_PictureBox_Image(inventoryPicturebox, Inventory.GetItemPic(16, inventoryPicturebox.Size.Width / 16, Selected_Player.Data.Pockets.Items, Save_File.Save_Type));
                }
            }
            else if (ItemBox == shirtPicturebox)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Shirt.ItemID;
                    SetCurrentItem(Selected_Player.Data.Shirt);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Shirt = new Item(GetCurrentItem());
                    shirtPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.Shirt, Save_File.Save_Type);
                }
            }
            else if (ItemBox == heldItemPicturebox)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.HeldItem.ItemID;
                    SetCurrentItem(Selected_Player.Data.HeldItem);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.HeldItem = new Item(GetCurrentItem());
                    heldItemPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.HeldItem, Save_File.Save_Type);
                }
            }
            else if (ItemBox == hatPicturebox && hatPicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    SetCurrentItem(Selected_Player.Data.Hat);
                    //selectedItem.SelectedValue = Selected_Player.Data.Hat.ItemID;
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Hat = new Item(GetCurrentItem());
                    hatPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.Hat, Save_File.Save_Type);
                }
            }
            else if (ItemBox == facePicturebox && facePicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.FaceItem.ItemID;
                    SetCurrentItem(Selected_Player.Data.FaceItem);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.FaceItem = new Item(GetCurrentItem());
                    facePicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.FaceItem, Save_File.Save_Type);
                }
            }
            else if (ItemBox == pocketsBackgroundPicturebox && pocketsBackgroundPicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.InventoryBackground.ItemID;
                    SetCurrentItem(Selected_Player.Data.InventoryBackground);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.InventoryBackground = new Item(GetCurrentItem());
                    pocketsBackgroundPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.InventoryBackground, Save_File.Save_Type);
                }
            }
            else if (ItemBox == bedPicturebox && bedPicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Bed.ItemID;
                    SetCurrentItem(Selected_Player.Data.Bed);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Bed = new Item(GetCurrentItem());
                    bedPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.Bed, Save_File.Save_Type);
                }
            }
            else if (ItemBox == pantsPicturebox && pantsPicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Pants.ItemID;
                    SetCurrentItem(Selected_Player.Data.Pants);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Pants = new Item(GetCurrentItem());
                    pantsPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.Pants, Save_File.Save_Type);
                }
            }
            else if (ItemBox == socksPicturebox && socksPicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Socks.ItemID;
                    SetCurrentItem(Selected_Player.Data.Socks);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Socks = new Item(GetCurrentItem());
                    socksPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.Socks, Save_File.Save_Type);
                }
            }
            else if (ItemBox == shoesPicturebox && shoesPicturebox.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Shoes.ItemID;
                    SetCurrentItem(Selected_Player.Data.Shoes);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Shoes = new Item(GetCurrentItem());
                    shoesPicturebox.Image = Inventory.GetItemPic(16, Selected_Player.Data.Shoes, Save_File.Save_Type);
                }
            }
            else if (ItemBox == playerWetsuit && playerWetsuit.Enabled)
            {
                if (e.Button == MouseButtons.Right)
                {
                    //selectedItem.SelectedValue = Selected_Player.Data.Wetsuit.ItemID;
                    SetCurrentItem(Selected_Player.Data.Wetsuit);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    Save_File.ChangesMade = true;
                    Selected_Player.Data.Wetsuit = new Item(GetCurrentItem());
                    playerWetsuit.Image = Inventory.GetItemPic(16, Selected_Player.Data.Wetsuit, Save_File.Save_Type);
                }
            }
        }

        #endregion

        #region Patterns

        int Last_Pat_X = 0, Last_Pat_Y = 0;
        private void Pattern_Move(object sender, MouseEventArgs e)
        {
            if ((e.X != Last_Pat_X || e.Y != Last_Pat_Y) && Save_File != null && Selected_Player.Exists)
            {
                patternToolTip.Hide(this);
                patternToolTip.RemoveAll();
                Last_Pat_X = e.X;
                Last_Pat_Y = e.Y;
                int Idx = Array.IndexOf(Pattern_Boxes, sender as PictureBox);
                if (Idx > -1 && Idx < Selected_Player.Data.Patterns.Length)
                    patternToolTip.Show(Selected_Player.Data.Patterns[Idx].Name, sender as PictureBox, e.X + 15, e.Y + 10);
            }
        }

        private void Hide_Pat_Tip(object sender, EventArgs e)
        {
            patternToolTip.Hide(this);
            Last_Pat_X = -1;
            Last_Pat_Y = -1;
        }

        private void Pattern_Export_Click(object sender, EventArgs e, int Idx)
        {
            if (Idx > -1 && Selected_Player != null && Selected_Player.Data.Patterns.Length > Idx)
            {
                Selected_Pattern = Selected_Player.Data.Patterns[Idx].Pattern_Bitmap;
                exportPatternFile.FileName = Selected_Player.Data.Patterns[Idx].Name + ".png";
                if (exportPatternFile.ShowDialog() == DialogResult.OK && Selected_Pattern != null)
                {
                    Selected_Pattern.Save(exportPatternFile.FileName);
                }
            }
        }

        private void Pattern_Import_Click(object sender, EventArgs e, int Idx)
        {
            if (Idx > -1 && Selected_Player != null)
            {
                if (importPatternFile.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(importPatternFile.FileName) && Path.GetExtension(importPatternFile.FileName) == ".png")
                    {
                        uint[] Pixel_Data = ImageGeneration.GetBitmapDataFromPNG(importPatternFile.FileName);
                        if (Pixel_Data != null)
                        {
                            Selected_Player.Data.Patterns[Idx].Import(Pixel_Data);
                            Refresh_PictureBox_Image(Pattern_Boxes[Idx], Selected_Player.Data.Patterns[Idx].Pattern_Bitmap, false, false);
                        }
                    }
                }
            }
        }

        //Add ContextMenuStrips for importing/exporting patterns & renaming/setting palette
        private void SetupPatternBoxes()
        {
            for (int i = patternPanel.Controls.Count - 1; i > -1; i--)
                if (patternPanel.Controls[i] is PictureBoxWithInterpolationMode)
                {
                    PictureBoxWithInterpolationMode disposingBox = patternPanel.Controls[i] as PictureBoxWithInterpolationMode;
                    var Old_Image = disposingBox.Image;
                    disposingBox.Dispose();
                    if (Old_Image != null)
                        Old_Image.Dispose();
                }
            Pattern_Boxes = new PictureBoxWithInterpolationMode[Current_Save_Info.Pattern_Count];
            for (int i = 0; i < Current_Save_Info.Pattern_Count; i++)
            {
                ContextMenuStrip PatternStrip = new ContextMenuStrip();
                ToolStripMenuItem Export = new ToolStripMenuItem()
                {
                    Name = "export",
                    Text = "Export"
                };
                ToolStripMenuItem Import = new ToolStripMenuItem()
                {
                    Name = "import",
                    Text = "Import"
                };
                ToolStripMenuItem Rename = new ToolStripMenuItem()
                {
                    Name = "rename",
                    Text = "Rename"
                };
                ToolStripMenuItem Set_Palette = new ToolStripMenuItem()
                {
                    Name = "setPalette",
                    Text = "Set Palette"
                };
                PatternStrip.Items.Add(Import);
                PatternStrip.Items.Add(Export);
                PatternStrip.Items.Add(Rename);
                PatternStrip.Items.Add(Set_Palette);

                PictureBoxWithInterpolationMode patternBox = new PictureBoxWithInterpolationMode()
                {
                    InterpolationMode = InterpolationMode.NearestNeighbor,
                    Name = "pattern" + i.ToString(),
                    Size = new Size(128, 128),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = new Point(Current_Save_Info.Pattern_Count > 4 ? (22 - ScrollbarWidth) / 2 : 11, 3 + i * 131),
                    Image = Acre_Image_List.Keys.Contains("FF") ? Acre_Image_List["FF"] : null,
                    ContextMenuStrip = PatternStrip,
                    UseInternalInterpolationSetting = true,
                };
                patternBox.MouseMove += new MouseEventHandler(Pattern_Move);
                patternBox.MouseLeave += new EventHandler(Hide_Pat_Tip);
                Pattern_Boxes[i] = patternBox;
                patternPanel.Controls.Add(patternBox);

                // ToolStrip Item Events
                Export.Click += new EventHandler((object sender, EventArgs e) => Pattern_Export_Click(sender, e, Array.IndexOf(Pattern_Boxes, patternBox)));
                Import.Click += new EventHandler((object sender, EventArgs e) => Pattern_Import_Click(sender, e, Array.IndexOf(Pattern_Boxes, patternBox)));
                Rename.Click += (object sender, EventArgs e) => Not_Implemented();
                Set_Palette.Click += (object sender, EventArgs e) => Not_Implemented();
            }
        }

        #endregion

        #region Town

        private void Fix_Buried_Empty_Spots()
        {
            int Occurances = 0;
            for (int i = 0; i < Town_Acres.Length; i++)
            {
                for (int x = 0; x < 256; x++)
                    if (ItemData.GetItemType(Town_Acres[i].Acre_Items[x].ItemID, Save_File.Save_Type) == "Empty" && Town_Acres[i].Acre_Items[x].Burried)
                    {
                        Town_Acres[i].SetBuriedInMemory(Town_Acres[i].Acre_Items[x], i, Buried_Buffer, false, Save_File.Save_Type);
                        Occurances++;
                    }
                Town_Acre_Map[i].Image = GenerateAcreItemsBitmap(Town_Acres[i].Acre_Items, i);
            }
            MessageBox.Show("Fixed Buried Empty Spots: " + Occurances);
        }

        private void Building_List_Index_Changed(object sender, EventArgs e)
        {
            ComboBox Sender_Box = sender as ComboBox;
            int Building_Idx = Array.IndexOf(Building_List_Panels, Sender_Box.Parent);
            Building Edited_Building = Buildings[Building_Idx];
            Edited_Building.ID = Building_DB[Array.IndexOf(Building_Names, Sender_Box.Text)];
            Edited_Building.Name = Sender_Box.Text;
            Edited_Building.Exists = Save_File.Save_Type == SaveType.New_Leaf ? Edited_Building.ID != 0xF8 : Edited_Building.ID != 0xFC;
            Town_Acre_Map[Edited_Building.Acre_Index].Image = GenerateAcreItemsBitmap(Town_Acres[Edited_Building.Acre_Index].Acre_Items, Edited_Building.Acre_Index);
        }

        //TODO: Update textboxes on change with mouse
        private void Building_Position_Changed(object sender, EventArgs e, bool isY = false)
        {
            TextBox Position_Box = sender as TextBox;
            Panel Parent_Panel = Position_Box.Parent as Panel;
            if (byte.TryParse(Position_Box.Text, out byte New_Position))
            {
                int Building_Idx = Array.IndexOf(Building_List_Panels, Parent_Panel);
                Building Edited_Building = Buildings[Building_Idx];
                if ((Save_File.Save_Type == SaveType.City_Folk && New_Position < 16 * 5) ||
                    ((Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo) && New_Position < 16 * (isY ? 4 : 5)))
                {
                    int Old_Acre = Edited_Building.Acre_Index;
                    int New_Acre = New_Position / 16;
                    if (!isY)
                    {
                        Edited_Building.Acre_X = (byte)(New_Acre + 1);
                        Edited_Building.X_Pos = (byte)(New_Position % 16);
                    }
                    else
                    {
                        Edited_Building.Acre_Y = (byte)(New_Acre + 1);
                        Edited_Building.Y_Pos = (byte)(New_Position % 16);
                    }
                    Edited_Building.Acre_Index = (byte)((Edited_Building.Acre_Y - 1) * 5 + (Edited_Building.Acre_X - 1));
                    Town_Acre_Map[Old_Acre].Image = GenerateAcreItemsBitmap(Town_Acres[Old_Acre].Acre_Items, Old_Acre);
                    if (Old_Acre != New_Acre)
                        Town_Acre_Map[New_Acre].Image = GenerateAcreItemsBitmap(Town_Acres[New_Acre].Acre_Items, New_Acre);
                }
                else //Return text to original position
                {
                    Position_Box.Text = isY ? (Edited_Building.Y_Pos + (Edited_Building.Acre_Y - 1) * 16).ToString()
                        : (Edited_Building.X_Pos + (Edited_Building.Acre_X - 1) * 16).ToString();
                }
            }
        }

        private void Update_Building_Position_Boxes(Building Edited_Building)
        {
            Panel Building_Panel = Building_List_Panels[Array.IndexOf(Buildings, Edited_Building)];
            Building_Panel.Controls[1].Text = (Edited_Building.X_Pos + (Edited_Building.Acre_X - 1) * 16).ToString();
            Building_Panel.Controls[2].Text = (Edited_Building.Y_Pos + (Edited_Building.Acre_Y - 1) * 16).ToString();
        }

        private void SetupBuildingList()
        {
            if (Save_File.Save_Type == SaveType.New_Leaf)
            {
                Building_DB = ItemData.NL_Building_Names.Keys.ToArray();
                Building_Names = ItemData.NL_Building_Names.Values.ToArray();
            }
            else if (Save_File.Save_Type == SaveType.Welcome_Amiibo)
            {
                Building_DB = ItemData.WA_Building_Names.Keys.ToArray();
                Building_Names = ItemData.WA_Building_Names.Values.ToArray();
            }
            Building_List_Panels = new Panel[Buildings.Length];
            for (int i = 0; i < Buildings.Length; i++)
            {
                Panel Building_Panel = new Panel
                {
                    Name = "Panel_" + i.ToString(),
                    Size = new Size(180, 22),
                    Location = new Point(0, i * 25)
                };
                if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                {
                    ComboBox Building_List_Box = new ComboBox
                    {
                        Size = new Size(120, 22),
                        DropDownWidth = 200
                    };
                    Building_List_Box.Items.AddRange(Building_Names);
                    Building_Panel.Controls.Add(Building_List_Box);
                    Building_List_Box.SelectedIndex = Array.IndexOf(Building_DB, Buildings[i].ID);
                    Building_List_Box.SelectedIndexChanged += new EventHandler(Building_List_Index_Changed);
                }
                else
                {
                    Label Building_List_Label = new Label
                    {
                        Size = new Size(120, 22),
                        Text = Buildings[i].Name,
                    };
                    Building_Panel.Controls.Add(Building_List_Label);
                }
                TextBox X_Location = new TextBox
                {
                    Size = new Size(22, 22),
                    Location = new Point(130, 0),
                    Text = Math.Max(0, Buildings[i].X_Pos + (Buildings[i].Acre_X - 1) * 16).ToString(),
                    Name = "X_Position"
                };
                TextBox Y_Location = new TextBox
                {
                    Size = new Size(22, 22),
                    Location = new Point(154, 0),
                    Text = Math.Max(0, Buildings[i].Y_Pos + (Buildings[i].Acre_Y - 1) * 16).ToString(),
                    Name = "Y_Position"
                };
                X_Location.LostFocus += new EventHandler((object sender, EventArgs e) => Building_Position_Changed(sender, e, false));
                Y_Location.LostFocus += new EventHandler((object sender, EventArgs e) => Building_Position_Changed(sender, e, true));
                Building_Panel.Controls.Add(X_Location);
                Building_Panel.Controls.Add(Y_Location);
                buildingsPanel.Controls.Add(Building_Panel);
                Building_List_Panels[i] = Building_Panel;
            }
        }

        private void Handle_Town_Click(object sender, WorldItem Item, int Acre, int index, MouseEventArgs e, bool Island = false)
        {
            if (e.Button == MouseButtons.Left)
            {
                Save_File.ChangesMade = true;
                PictureBoxWithInterpolationMode Box = sender as PictureBoxWithInterpolationMode;
                Box.Capture = false;
                if (Selected_Building > -1)
                {
                    Building B = Island ? Island_Buildings[Selected_Building] : Buildings[Selected_Building];
                    int Old_Acre = B.Acre_Index;
                    int Adjusted_Acre = Island ? Acre - 5 : Acre;
                    if (Check_Building_is_Here(Adjusted_Acre, index % 16, index / 16, Island) != null)
                        return; //Don't place buildings on top of each other
                    B.Acre_Index = (byte)Adjusted_Acre;
                    B.Acre_X = Island ? (byte)((Adjusted_Acre % 2) + 1) : (byte)((Adjusted_Acre % (Current_Save_Info.X_Acre_Count - 2) + 1)); //Might have to change for NL
                    B.Acre_Y = Island ? (byte)((Adjusted_Acre / 2) + 1) : (byte)((Adjusted_Acre / (Current_Save_Info.X_Acre_Count - 2) + 1));
                    B.X_Pos = (byte)(index % 16);
                    B.Y_Pos = (byte)(index / 16);
                    if (B.Name != "Sign" && B.Name != "Bus Stop") //These two items has "actor" items at their location
                        if (Island)
                            Island_Acres[Acre].Acre_Items[index] = new WorldItem(index);
                        else
                            Town_Acres[Acre].Acre_Items[index] = new WorldItem(index); //Clear any item at the new building position
                    else
                        Town_Acres[Acre].Acre_Items[index] = new WorldItem(B.Name == "Sign" ? (ushort)0xD000 : (ushort)0x7003, index);
                    if ((!Island && Old_Acre != Acre) || (Island && Old_Acre != Adjusted_Acre))
                    {
                        var Old_Image = Island ? Island_Acre_Map[Old_Acre + 5].Image : Town_Acre_Map[Old_Acre].Image;
                        if (Island)
                        {
                            Island_Acre_Map[Old_Acre + 5].Image = GenerateAcreItemsBitmap(Island_Acres[Old_Acre + 5].Acre_Items, Old_Acre + 5, Island);
                            Island_Acre_Map[Old_Acre + 5].Refresh();
                        }
                        else
                        {
                            Town_Acre_Map[Old_Acre].Image = GenerateAcreItemsBitmap(Town_Acres[Old_Acre].Acre_Items, Old_Acre);
                            Town_Acre_Map[Old_Acre].Refresh();
                        }
                        Old_Image.Dispose();
                    }
                    if (!Island) //TODO: Add Island Building Panel
                        Update_Building_Position_Boxes(B);
                    Selected_Building = -1;
                    selectedItem.SelectedValue = Last_Selected_Item;
                    selectedItem.Enabled = true;
                }
                else
                {
                    if (Item.ItemID == GetCurrentItem())
                        return;
                    if (itemFlag1.Enabled)
                    {
                        WorldItem NewItem = new WorldItem(GetCurrentItem(), index);
                        byte.TryParse(itemFlag1.Text, NumberStyles.AllowHexSpecifier, null, out NewItem.Flag1);
                        byte.TryParse(itemFlag2.Text, NumberStyles.AllowHexSpecifier, null, out NewItem.Flag2);
                        switch (NewItem.Flag1)
                        {
                            case 0x40:
                                NewItem.Burried = false;
                                NewItem.Watered = true;
                                break;
                            case 0x80:
                                NewItem.Watered = false;
                                NewItem.Burried = true;
                                break;
                        }
                        if (Island)
                            Island_Acres[Acre].Acre_Items[index] = NewItem;
                        else
                            Town_Acres[Acre].Acre_Items[index] = NewItem;
                    }
                    else
                    {
                        if (Island)
                        {
                            if (SelectedIsland == null)
                                Island_Acres[Acre].Acre_Items[index] = new WorldItem(GetCurrentItem(), index);
                            else
                                SelectedIsland.Items[Acre][index] = new WorldItem(GetCurrentItem(), index);
                        }
                        else
                            Town_Acres[Acre].Acre_Items[index] = new WorldItem(GetCurrentItem(), index);
                    }
                    if (buriedCheckbox.Checked)
                    {
                        if (Save_File.Game_System == SaveGeneration.N3DS)
                        {
                            if (Island)
                            {
                                if (Island_Acres[Acre].Acre_Items[index].ItemID != 0x7FFE)
                                {
                                    Island_Acres[Acre].Acre_Items[index].Flag1 = 0x80;
                                    Island_Acres[Acre].Acre_Items[index].Burried = true;
                                }
                            }
                            else
                            {
                                if (Town_Acres[Acre].Acre_Items[index].ItemID != 0x7FFE)
                                {
                                    Town_Acres[Acre].Acre_Items[index].Flag1 = 0x80;
                                    Town_Acres[Acre].Acre_Items[index].Burried = true;
                                }
                            }
                        }
                        else
                        {
                            if (Island)
                            {
                                if (SelectedIsland != null)
                                {
                                    // TODO: Island buried items
                                }
                                else
                                    Island_Acres[Acre].SetBuriedInMemory(Island_Acres[Acre].Acre_Items[index], Acre, Island_Buried_Buffer, true, Save_File.Save_Type);
                            }
                            else
                                Town_Acres[Acre].SetBuriedInMemory(Town_Acres[Acre].Acre_Items[index], Acre, Buried_Buffer, true, Save_File.Save_Type);
                        }
                    }
                }
                var OldImage = Box.Image;
                WorldItem[] Items = null;
                if (Island)
                {
                    if (SelectedIsland == null)
                        Items = Island_Acres[Acre].Acre_Items;
                    else
                        Items = SelectedIsland.Items[Acre];
                }
                else
                    Items = Town_Acres[Acre].Acre_Items;

                Refresh_PictureBox_Image(Box, GenerateAcreItemsBitmap(Items, Acre, Island));
                Box.Refresh();
                OldImage.Dispose();
                Town_Move(sender, e, Island, true); // Force ToolTip update
            }
            else if (e.Button == MouseButtons.Right)
            {
                Building B = Check_Building_is_Here(Acre, index % 16, index / 16, Island);
                if (B != null)
                {
                    if (Selected_Building == -1)
                        Last_Selected_Item = GetCurrentItem();
                    selectedItem.SelectedValue = (ushort)0xFFFF;
                    Selected_Building = Island ? Array.IndexOf(Island_Buildings, B) : Array.IndexOf(Buildings, B);
                    selectedItem.Enabled = false;
                    selectedItem.Text = B.Name;
                    selectedItemText.Text = string.Format("Selected Building: [{0}]", B.Name);
                }
                else
                {
                    selectedItem.Enabled = true;
                    ushort Old_Selected_Item_ID = Selected_Building > -1 ? Last_Selected_Item : GetCurrentItem();
                    Selected_Building = -1;
                    selectedItem.SelectedValue = Item.ItemID;
                    if (selectedItem.SelectedValue == null)
                        selectedItem.SelectedValue = Old_Selected_Item_ID;
                    else
                    {
                        buriedCheckbox.Checked = Item.Burried || Item.Flag1 == 0x80;
                        if (itemFlag1.Enabled)
                        {
                            itemFlag1.Text = Item.Flag1.ToString("X2");
                            itemFlag2.Text = Item.Flag2.ToString("X2");
                        }
                    }
                    //selectedItemText.Text = string.Format("Selected Item: [0x{0}]", (GetCurrentItem()).ToString("X4"));
                }
                selectedItem.Refresh();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                PictureBoxWithInterpolationMode Box = sender as PictureBoxWithInterpolationMode;
                Box.Capture = false;
                WorldItem[] Items = null;
                if (Island)
                {
                    if (SelectedIsland == null)
                        Items = Island_Acres[Acre].Acre_Items;
                    else
                        Items = SelectedIsland.Items[Acre];
                }
                else
                {
                    Items = Town_Acres[Acre].Acre_Items;
                }
                Utility.FloodFillWorldItemArray(ref Items, 16, index, Items[index], new WorldItem(GetCurrentItem(), byte.Parse(itemFlag1.Text), byte.Parse(itemFlag2.Text), Items[index].Index));
                Refresh_PictureBox_Image(Box, GenerateAcreItemsBitmap(Items, Acre, Island));
            }
        }

        int Last_Town_X = 0, Last_Town_Y = 0;
        private void Town_Move(object sender, MouseEventArgs e, bool Island = false, bool Override = false)
        {
            (sender as Control).Capture = false;
            if ((Override || (e.X != Last_Town_X || e.Y != Last_Town_Y)) && Save_File != null)
            {
                townToolTip.Hide(this);
                townToolTip.RemoveAll();
                Last_Town_X = e.X;
                Last_Town_Y = e.Y;
                int idx = Island ? Array.IndexOf(Island_Acre_Map, sender as PictureBox) : Array.IndexOf(Town_Acre_Map, sender as PictureBoxWithInterpolationMode);
                int X = e.X / TownMapCellSize;
                int Y = e.Y / TownMapCellSize;
                int index = X + Y * 16;
                int Acre = idx;
                if (index > 255)
                    return;
                // Set Info Label
                townInfoLabel.Text = string.Format("X: {0} | Y: {1} | Index: {2}", X, Y, index);
                WorldItem Item = null;
                if (Island)
                {
                    if (SelectedIsland == null)
                        Item = Island_Acres[Acre].Acre_Items[index];
                    else
                        Item = SelectedIsland.Items[Acre][index];
                }
                else
                    Item = Town_Acres[Acre].Acre_Items[index];
                if (Clicking && !Override)
                    Handle_Town_Click(sender, Item, Acre, index, e, Island);
                if (Buildings != null)
                {
                    Building B = Check_Building_is_Here(Acre, X, Y, Island);
                    if (B != null)
                        townToolTip.Show(string.Format("{0} - [0x{1} - Building]", B.Name, B.ID.ToString("X2")), sender as PictureBox, e.X + 15, e.Y + 10);
                    else
                        townToolTip.Show(string.Format("{0}{1} - [0x{2}]", Item.Name,
                            Item.Burried ? " (Buried)" : (Item.Watered ? " (Watered)" : (Item.Flag1 == 1 ? " (Perfect Fruit)" : "")),
                            Item.ItemID.ToString("X4")), sender as PictureBox, e.X + 15, e.Y + 10);
                }
                else
                    townToolTip.Show(string.Format("{0}{1} - [0x{2}]", Item.Name, Item.Burried ? " (Buried)" : "", Item.ItemID.ToString("X4")), sender as PictureBox, e.X + 15, e.Y + 10);
            }
        }

        private void Hide_Town_Tip(object sender, EventArgs e)
        {
            townToolTip.Hide(this);
            Last_Town_X = 0;
            Last_Town_Y = 0;
        }

        private void Town_Mouse_Down(object sender, MouseEventArgs e, bool Island = false)
        {
            int idx = Island ? Array.IndexOf(Island_Acre_Map, sender as PictureBoxWithInterpolationMode) : Array.IndexOf(Town_Acre_Map, sender as PictureBoxWithInterpolationMode);
            int X = e.X / TownMapCellSize;
            int Y = e.Y / TownMapCellSize;
            int index = X + Y * 16;
            int Acre = idx;
            if (index > 255)
                return;
            WorldItem Item = Island ? Island_Acres[Acre].Acre_Items[index] : Town_Acres[Acre].Acre_Items[index];
            Handle_Town_Click(sender, Item, Acre, index, e, Island);
            Clicking = true;
        }

        private void Town_Mouse_Up(object sender, EventArgs e)
        {
            Clicking = false;
        }

        private Building Check_Building_is_Here(int acre, int x, int y, bool Island = false)
        {
            if (Island)
            {
                acre = acre - 5;
                if (Island_Buildings != null)
                    foreach (Building B in Island_Buildings)
                        if (B.Exists && B.Acre_Index == acre && B.X_Pos == x && B.Y_Pos == y)
                            return B;
            }
            else
                if (Buildings != null)
                    foreach (Building B in Buildings)
                        if (B.Exists && B.Acre_Index == acre && B.X_Pos == x && B.Y_Pos == y)
                            return B;
            return null;
        }

        #endregion

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save_File != null)
            {
                //Save Players
                foreach (NewPlayer Player in Players)
                    if (Player != null && Player.Exists)
                        Player.Write();

                //Save Villagers
                if (Villagers != null)
                {
                    foreach (NewVillager Villager in Villagers)
                        if (Villager != null)
                            Villager.Write();
                }

                // Save Houses
                foreach (House House in Houses)
                    if (House != null && Save_File.Game_System == SaveGeneration.GCN) // TODO: Finish WW+ House editing
                        House.Write();

                //Save Acre & Town Data
                for (int i = 0; i < Acres.Length; i++)
                    if (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN || Save_File.Save_Type == SaveType.City_Folk)
                    {
                        Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data + i * 2, Acres[i].AcreID, true);
                    }
                    else if (Save_File.Save_Type == SaveType.Wild_World)
                    {
                        Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data + i, Convert.ToByte(Acres[i].AcreID), Save_File.Is_Big_Endian);
                    }
                    else if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    {
                        Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Acre_Data + i * 2, Acres[i].AcreID, false);
                    }
                if (Current_Save_Info.Save_Offsets.Town_Data != 0)
                {
                    for (int i = 0; i < Town_Acres.Length; i++)
                        for (int x = 0; x < 256; x++)
                            if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                            {
                                Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Town_Data + i * 1024 + x * 4,
                                    ItemData.EncodeItem(Town_Acres[i].Acre_Items[x]));
                            }
                            else
                                Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Town_Data + i * 512 + x * 2, Town_Acres[i].Acre_Items[x].ItemID,
                                    Save_File.Is_Big_Endian);
                }
                if ((Save_File.Save_Type != SaveType.New_Leaf && Save_File.Save_Type != SaveType.Welcome_Amiibo) && Current_Save_Info.Save_Offsets.Buried_Data != 0)
                    Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Buried_Data, Buried_Buffer);
                if (Current_Save_Info.Save_Offsets.Grass_Wear > 0)
                    Save_File.Write(Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Grass_Wear, Grass_Wear);
                if (Current_Save_Info.Save_Offsets.Buildings > 0)
                {
                    if (Save_File.Save_Type == SaveType.City_Folk)
                    {
                        for (int i = 0; i < Buildings.Length; i++)
                        {
                            if (i < 33)
                            {
                                int DataOffset = Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Buildings + i * 2;
                                byte X = (byte)(((Buildings[i].Acre_X << 4) & 0xF0) + (Buildings[i].X_Pos & 0x0F)), Y = (byte)(((Buildings[i].Acre_Y << 4) & 0xF0) + (Buildings[i].Y_Pos & 0x0F));
                                Save_File.Write(DataOffset, X);
                                Save_File.Write(DataOffset + 1, Y);
                            }
                            else if (Buildings[i].ID == 33) //Pave's Sign
                            {
                                int DataOffset = Save_File.Save_Data_Start_Offset + 0x5EB90;
                                byte X = (byte)(((Buildings[i].Acre_X << 4) & 0xF0) + (Buildings[i].X_Pos & 0x0F)), Y = (byte)(((Buildings[i].Acre_Y << 4) & 0xF0) + (Buildings[i].Y_Pos & 0x0F));
                                Save_File.Write(DataOffset, X);
                                Save_File.Write(DataOffset + 1, Y);
                            }
                            else if (Buildings[i].ID == 34) //Bus Stop
                            {
                                int DataOffset = Save_File.Save_Data_Start_Offset + 0x5EB8A;
                                byte X = (byte)(((Buildings[i].Acre_X << 4) & 0xF0) + (Buildings[i].X_Pos & 0x0F)), Y = (byte)(((Buildings[i].Acre_Y << 4) & 0xF0) + (Buildings[i].Y_Pos & 0x0F));
                                Save_File.Write(DataOffset, X);
                                Save_File.Write(DataOffset + 1, Y);
                            }
                            else if (i >= 35) //Signs
                            {
                                int DataOffset = Save_File.Save_Data_Start_Offset + 0x5EB92 + (i - 35) * 2;
                                byte X = (byte)(((Buildings[i].Acre_X << 4) & 0xF0) + (Buildings[i].X_Pos & 0x0F)), Y = (byte)(((Buildings[i].Acre_Y << 4) & 0xF0) + (Buildings[i].Y_Pos & 0x0F));
                                Save_File.Write(DataOffset, X);
                                Save_File.Write(DataOffset + 1, Y);
                            }
                        }
                    }
                    else if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    {
                        for (int i = 0; i < Buildings.Length; i++)
                        {
                            int DataOffset = Save_File.Save_Data_Start_Offset + Current_Save_Info.Save_Offsets.Buildings + i * 4;
                            byte X = (byte)(((Buildings[i].Acre_X << 4) & 0xF0) + (Buildings[i].X_Pos & 0x0F)), Y = (byte)(((Buildings[i].Acre_Y << 4) & 0xF0) + (Buildings[i].Y_Pos & 0x0F));
                            Save_File.Write(DataOffset, new byte[4] { Buildings[i].ID, 0x00, X, Y });
                        }
                    }
                }
                Save_File.Flush();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save_File != null)
            {
                openSaveFile.FileName = Save_File.Save_Name + Save_File.Save_Extension;
            }
            else
            {
                openSaveFile.FileName = "";
            }
            if (openSaveFile.ShowDialog() == DialogResult.OK)
            {
                if (Save_File != null)
                {
                    if (Save_File.ChangesMade) // TOOD: Changes aren't updated when placing items, etc. Must change that.
                    {
                        DialogResult Result = MessageBox.Show("A save file is already being edited. Would you like to save your changes before opening another file?", "Save File", MessageBoxButtons.YesNo);
                        if (Result == DialogResult.Yes)
                        {
                            Save_File.Flush();
                        }
                        //Add Save_File.Close(); ??
                    }
                }
                SetupEditor(new Save(openSaveFile.FileName));
            }
        }

        private void clearWeedsButton_Click(object sender, EventArgs e)
        {
            int Weeds_Cleared = 0;
            foreach (PictureBoxWithInterpolationMode Box in Town_Acre_Map)
            {
                int idx = Array.IndexOf(Town_Acre_Map, Box);
                int Acre_Idx = idx;
                Normal_Acre Acre = Town_Acres[Acre_Idx];
                if (Acre.Acre_Items != null)
                {
                    for (int i = 0; i < 256; i++)
                        if (ItemData.GetItemType(Acre.Acre_Items[i].ItemID, Save_File.Save_Type) == "Weed")
                        {
                            if (Save_File.Save_Type == SaveType.Wild_World || Save_File.Save_Type == SaveType.City_Folk)
                                Acre.Acre_Items[i] = new WorldItem(0xFFF1, Acre.Acre_Items[i].Index);
                            else if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                                Acre.Acre_Items[i] = new WorldItem(0x7FFE, Acre.Acre_Items[i].Index);
                            else
                                Acre.Acre_Items[i] = new WorldItem(0, Acre.Acre_Items[i].Index);
                            Weeds_Cleared++;
                        }
                    Box.Image = GenerateAcreItemsBitmap(Acre.Acre_Items, Acre_Idx);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (About_Box == null || About_Box.IsDisposed)
                About_Box = new AboutBox1();
            About_Box.Show();
        }

        private void tanTrackbar_Scroll(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null && Selected_Player.Exists)
                Selected_Player.Data.Tan = (byte)(tanTrackbar.Value - 1);
        }

        private void removeGrass_Click(object sender, EventArgs e)
        {
            if (Save_File != null)
            {
                Array.Clear(Grass_Wear, 0, Grass_Wear.Length);
                if (Save_File.Save_Type == SaveType.City_Folk)
                    for (int i = 0; i < Grass_Map.Length; i++)
                        Grass_Map[i].Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear.Skip(i * 256).Take(256).ToArray());
                else if (Save_File.Game_System == SaveGeneration.N3DS)
                    NL_Grass_Overlay.Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear);
            }
        }

        private void reviveGrass_Click(object sender, EventArgs e)
        {
            if (Save_File != null)
            {
                for (int i = 0; i < Grass_Wear.Length; i++)
                    Grass_Wear[i] = 0xFF;
                if (Save_File.Save_Type == SaveType.City_Folk)
                    for (int i = 0; i < Grass_Map.Length; i++)
                        Grass_Map[i].Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear.Skip(i * 256).Take(256).ToArray());
                else if (Save_File.Game_System == SaveGeneration.N3DS)
                    NL_Grass_Overlay.Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear);
            }
        }

        private void setAllGrass_Click(object sender, EventArgs e)
        {
            if (Save_File != null)
            {
                byte.TryParse(grassLevelBox.Text, out byte Set_Value);
                for (int i = 0; i < Grass_Wear.Length; i++)
                    Grass_Wear[i] = Set_Value;
                if (Save_File.Save_Type == SaveType.City_Folk)
                    for (int i = 0; i < Grass_Map.Length; i++)
                        Grass_Map[i].Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear.Skip(i * 256).Take(256).ToArray());
                else if (Save_File.Game_System == SaveGeneration.N3DS)
                    NL_Grass_Overlay.Image = ImageGeneration.Draw_Grass_Wear(Grass_Wear);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save_File != null)
            {
                string Filter_String = string.Format("{0} Save File|*{1}|All Files (*.*)|*.*", Save_File.Save_Type.ToString().Replace("_", " "), Save_File.Save_Extension);
                saveSaveFile.FileName = Save_File.Save_Name + Save_File.Save_Extension;
                saveSaveFile.Filter = Filter_String;
                DialogResult Save_OK = saveSaveFile.ShowDialog();
                if (Save_OK == DialogResult.OK)
                {
                    Save_File.Save_Path = Path.GetDirectoryName(saveSaveFile.FileName) + Path.DirectorySeparatorChar;
                    Save_File.Save_Name = Path.GetFileNameWithoutExtension(saveSaveFile.FileName);
                    Save_File.Save_Extension = Path.GetExtension(saveSaveFile.FileName);
                    saveToolStripMenuItem_Click(sender, e); //Realllly dislike this but it's so much easier than rewriting code
                    Text = string.Format("ACSE - {1} - [{0}]", Save_File.Save_Type.ToString().Replace("_", " "), Save_File.Save_Name);
                }
            }
        }
        
        private void exportPicture_Click(object sender, EventArgs e)
        {
            if (Save_File != null && (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                exportPatternFile.Filter = "JPEG Image (*.jpg)|*.jpg";
                exportPatternFile.FileName = "TPC Picture";
                DialogResult Saved = exportPatternFile.ShowDialog();
                if (Saved == DialogResult.OK)
                {
                    using (FileStream Picture = new FileStream(exportPatternFile.FileName, FileMode.Create))
                        Picture.Write(Selected_Player.Data.TownPassCardData, 0, 0x1400);
                }
            }
        }

        private void importPicture_Click(object sender, EventArgs e)
        {
            if (Save_File != null && (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                importPatternFile.Filter = "JPEG Image (*.jpg)|*.jpg";
                DialogResult Opened = importPatternFile.ShowDialog();
                if (Opened == DialogResult.OK)
                {
                    Image Original_Image = Image.FromFile(importPatternFile.FileName);
                    if (Original_Image.Width != 64 || Original_Image.Height != 104 || new FileInfo(importPatternFile.FileName).Length > 0x1400)
                        MessageBox.Show("The image you tried to import is incompatible. Please ensure the following:\n\nImage Width is 64 pixels\nImage Hight is 104 pixels\nImage file size is equal to or less than 5,120 bytes");
                    else
                    {
                        //Image passed validation checks, so import it
                        byte[] Image_Data_Buffer = new byte[0x1400];
                        using (FileStream File = new FileStream(importPatternFile.FileName, FileMode.Open, FileAccess.Read))
                        {
                            File.Read(Image_Data_Buffer, 0, 0x1400);
                        }
                        //Scan for the actual end of image (0xFF 0xD9) and trim excess
                        byte[] Trimmed_TPC_Buffer = ImageGeneration.GetTPCTrimmedBytes(Image_Data_Buffer);
                        Selected_Player.Data.TownPassCardData = Trimmed_TPC_Buffer;
                        //Draw the new image
                        var Old_Image = TPC_Picture.Image;
                        TPC_Picture.Image = ImageGeneration.GetTPCImage(Trimmed_TPC_Buffer);
                        Selected_Player.Data.TownPassCardImage = TPC_Picture.Image;
                        if (Old_Image != null)
                            Old_Image.Dispose();
                    }
                }
            }
        }

        private void perfectFruitsButton_Click(object sender, EventArgs e)
        {
            if (Save_File != null && (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                int Trees_Made_Perfect = 0;
                foreach (PictureBoxWithInterpolationMode Box in Town_Acre_Map)
                {
                    int idx = Array.IndexOf(Town_Acre_Map, Box);
                    int Acre_Idx = idx;
                    Normal_Acre Acre = Town_Acres[Acre_Idx];
                    if (Acre.Acre_Items != null)
                    {
                        if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                        {
                            foreach (WorldItem Fruit_Tree in Acre.Acre_Items.Where(i => (i.ItemID >= 0x3A && i.ItemID <= 0x52 && i.Flag1 == 0 && i.Flag2 == 0)))
                            {
                                Fruit_Tree.Flag1 = 1;
                                Trees_Made_Perfect++;
                            }
                        }
                    }
                }
            }
        }

        private void playerName_TextChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerName.Text.Length > 0)
                Selected_Player.Data.Name = playerName.Text;
        }

        private void secureValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save_File != null && Save_File.Game_System == SaveGeneration.N3DS)
                Secure_NAND_Value_Form.Show();
        }

        private void playerWallet_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null)
            {
                if (uint.TryParse(playerWallet.Text, out uint Wallet_Value))
                {
                    if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    {
                        Selected_Player.Data.NL_Wallet = new NL_Int32(Wallet_Value);
                    }
                    else
                    {
                        Selected_Player.Data.Bells = Wallet_Value;
                    }
                }
                else
                {
                    if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                        playerWallet.Text = Selected_Player.Data.NL_Wallet.Value.ToString();
                    else
                        playerWallet.Text = Selected_Player.Data.Bells.ToString();
                }
            }
        }

        private void playerDebt_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null)
            {
                if (uint.TryParse(playerDebt.Text, out uint Debt_Value))
                {
                    if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    {
                        Selected_Player.Data.NL_Debt = new NL_Int32(Debt_Value);
                    }
                    else
                    {
                        Selected_Player.Data.Debt = Debt_Value;
                    }
                }
                else
                {
                    if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                        playerDebt.Text = Selected_Player.Data.NL_Debt.Value.ToString();
                    else
                        playerDebt.Text = Selected_Player.Data.Debt.ToString();
                }
            }
        }

        private void playerSavings_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null)
            {
                if (uint.TryParse(playerSavings.Text, out uint Savings_Value))
                {
                    if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    {
                        Selected_Player.Data.NL_Savings = new NL_Int32(Savings_Value);
                    }
                    else
                    {
                        Selected_Player.Data.Savings = Savings_Value;
                    }
                }
                else
                {
                    if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                        playerSavings.Text = Selected_Player.Data.NL_Savings.Value.ToString();
                    else
                        playerSavings.Text = Selected_Player.Data.Savings.ToString();
                }
            }
        }

        private void playerMeowCoupons_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null && (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                if (uint.TryParse(playerMeowCoupons.Text, out uint MeowCoupons_Value))
                {
                    Selected_Player.Data.MeowCoupons = new NL_Int32(MeowCoupons_Value);
                }
                else
                {
                    playerMeowCoupons.Text = Selected_Player.Data.MeowCoupons.Value.ToString();
                }
            }
        }

        private void playerIslandMedals_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null && (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                if (uint.TryParse(playerIslandMedals.Text, out uint IslandMedals_Value))
                {
                    Selected_Player.Data.Island_Medals = new NL_Int32(IslandMedals_Value);
                }
                else
                {
                    playerIslandMedals.Text = Selected_Player.Data.Island_Medals.Value.ToString();
                }
            }
        }

        private void playerNookPoints_FocusLost(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null && (Save_File.Save_Type == SaveType.Wild_World || Save_File.Save_Type == SaveType.City_Folk))
            {
                if (ushort.TryParse(playerNookPoints.Text, out ushort NookPoints_Value))
                {
                    Selected_Player.Data.NookPoints = NookPoints_Value;
                }
                else
                {
                    playerNookPoints.Text = Selected_Player.Data.NookPoints.ToString();
                }
            }
        }

        private void playerTownName_TextChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerTownName.Text.Length > 0)
                Selected_Player.Data.TownName = playerTownName.Text;
        }

        private void playerShoeColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerShoeColor.SelectedIndex > -1)
                Selected_Player.Data.ShoeColor = (byte)playerShoeColor.SelectedIndex;
        }

        private void playerEyeColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerEyeColor.SelectedIndex > -1)
                Selected_Player.Data.EyeColor = (byte)playerEyeColor.SelectedIndex;
        }

        private void playerHairColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerHairColor.SelectedIndex > -1)
                Selected_Player.Data.HairColor = (byte)playerHairColor.SelectedIndex;
        }

        private void playerHairType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerHairType.SelectedIndex > -1)
                Selected_Player.Data.HairType = (byte)playerHairType.SelectedIndex;
        }

        private void playerFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerFace.SelectedIndex > -1)
                Selected_Player.Data.FaceType = (byte)playerFace.SelectedIndex;
        }

        private void playerGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Save_File != null && playerGender.SelectedIndex > -1)
                Selected_Player.Data.Gender = (byte)playerGender.SelectedIndex;
        }

        private void acreHeightTrackBar_Scroll(object sender, EventArgs e)
        {
            if (Save_File != null && (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN))
            {
                Acre_Height_Modifier = (ushort)acreHeightTrackBar.Value;
                acreID.Text = "Acre ID: 0x" + (Selected_Acre_ID + Acre_Height_Modifier).ToString("X4");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Save_File != null && (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN))
            {
                if (AC_Map_Icons == null)
                {
                    AC_Map_Icons = AcreData.Load_AC_Map_Icons();
                }
                if (AC_Map_Icon_Index == null)
                {
                    AC_Map_Icon_Index = AcreData.Load_AC_Map_Index(Save_File.Save_Type);
                }
                if (townMapViewCheckbox.Checked)
                {
                    if (AC_Map_Icon_Index != null && AC_Map_Icons != null)
                    {
                        for (int i = 0; i < Acres.Length; i++)
                        {
                            if (AC_Map_Icon_Index.ContainsKey(Acres[i].AcreID) && AC_Map_Icons.ContainsKey(AC_Map_Icon_Index[Acres[i].AcreID]))
                            {
                                Acre_Map[i].BackgroundImage = AC_Map_Icons[AC_Map_Icon_Index[Acres[i].AcreID]];
                            }
                            else if (AC_Map_Icon_Index.ContainsKey(Acres[i].BaseAcreID) && AC_Map_Icons.ContainsKey(AC_Map_Icon_Index[Acres[i].BaseAcreID]))
                            {
                                Acre_Map[i].BackgroundImage = AC_Map_Icons[AC_Map_Icon_Index[Acres[i].BaseAcreID]];
                            }
                            else
                            {
                                Acre_Map[i].BackgroundImage = AC_Map_Icons[99];
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Acres.Length; i++)
                    {
                        Acre_Map[i].BackgroundImage = Get_Acre_Image(Acres[i], Acres[i].BaseAcreID.ToString("X4"));
                    }
                }
            }
        }

        //TODO: Implement for WW+
        private void resettiCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Save_File != null && Selected_Player != null && (Save_File.Game_System == SaveGeneration.N64 || Save_File.Game_System == SaveGeneration.GCN))
            {
                Selected_Player.Data.Reset = resettiCheckBox.Checked;
            }
        }

        private void clearWeedsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearWeedsButton_Click(sender, e);
        }

        private void removeAllItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Town_Acres.Length; i++)
            {
                for (int x = 0; x < Town_Acres[i].Acre_Items.Length; x++)
                {
                    Town_Acres[i].Acre_Items[x] = new WorldItem(Town_Acres[i].Acre_Items[x].Index);
                }
                Town_Acre_Map[i].Image = GenerateAcreItemsBitmap(Town_Acres[i].Acre_Items, i, false);
            }
        }

        private void makeFruitsPerfectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            perfectFruitsButton_Click(sender, e);
        }

        private void waterFlowersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            waterFlowersButton_Click(sender, e);
        }

        private void fillEncyclopediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Encyclopedia.FillEncyclopedia(Save_File, Selected_Player);
        }

        private void getAllKKSongsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SongLibrary.FillSongLibrary(Save_File, Selected_Player);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Town_Acres != null && ushort.TryParse(ReplaceItemBox.Text, NumberStyles.HexNumber, null, out ushort ReplaceId)
                && ushort.TryParse(ReplacingItemBox.Text, NumberStyles.HexNumber, null, out ushort ReplacingId))
            {
                string ReplacingName = ItemData.GetItemName(ReplacingId);
                bool Unbury = ReplacingName.Equals("Empty");
                
                for (int i = 0; i < Town_Acres.Length; i++)
                {
                    bool Changed = false;
                    Normal_Acre Acre = Town_Acres[i];
                    if (Acre.Acre_Items != null)
                    {
                        foreach (WorldItem Item in Acre.Acre_Items)
                        {
                            if (Item.ItemID == ReplaceId)
                            {
                                Changed = true;
                                Item.ItemID = ReplacingId;
                                Item.Name = ReplacingName;
                                if (Item.Burried && Unbury)
                                {
                                    Item.Burried = false;
                                    Item.Flag1 &= 0x7F;
                                }
                            }
                        }
                    }
                    if (Changed)
                        Refresh_PictureBox_Image(Town_Acre_Map[i], GenerateAcreItemsBitmap(Acre.Acre_Items, i)); // TODO: Make this work on island acres somehow.
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings_Menu.Show();
        }

        private void waterFlowersButton_Click(object sender, EventArgs e)
        {
            int Flowers_Watered = 0;
            foreach (PictureBoxWithInterpolationMode Box in Town_Acre_Map)
            {
                int idx = Array.IndexOf(Town_Acre_Map, Box);
                int Acre_Idx = idx;
                Normal_Acre Acre = Town_Acres[Acre_Idx];
                if (Acre.Acre_Items != null)
                {
                    if (Save_File.Save_Type == SaveType.Wild_World)
                    {
                        for (int i = 0; i < 256; i++)
                            if (ItemData.GetItemType(Acre.Acre_Items[i].ItemID, Save_File.Save_Type) == "Parched Flower")
                            {
                                Acre.Acre_Items[i] = new WorldItem((ushort)(Acre.Acre_Items[i].ItemID + 0x1C), Acre.Acre_Items[i].Index);
                                Flowers_Watered++;
                            }
                    }
                    else if (Save_File.Save_Type == SaveType.City_Folk)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            if (ItemData.GetItemType(Acre.Acre_Items[i].ItemID, Save_File.Save_Type) == "Parched Flower")
                            {
                                Acre.Acre_Items[i] = new WorldItem((ushort)(Acre.Acre_Items[i].ItemID - 0x20), Acre.Acre_Items[i].Index);
                            }
                        }
                    }
                    else if (Save_File.Save_Type == SaveType.New_Leaf || Save_File.Save_Type == SaveType.Welcome_Amiibo)
                    {
                        for (int i = 0; i < 256; i++)
                            if (ItemData.GetItemType(Acre.Acre_Items[i].ItemID, Save_File.Save_Type) == "Flower")
                            {
                                Acre.Acre_Items[i].Flag1 = 0x40;
                                Acre.Acre_Items[i].Watered = true;
                                Flowers_Watered++;
                            }
                    }
                    var Old_Image = Box.Image;
                    Box.Image = GenerateAcreItemsBitmap(Acre.Acre_Items, Acre_Idx);
                    if (Old_Image != null)
                        Old_Image.Dispose();
                }
            }
        }

        #region Doubutsu no Mori e+ Islands

        private void IslandTabIndexChanged(object sender, TabControlEventArgs e)
        {
            if (islandSelectionTab.SelectedIndex < 0 || islandSelectionTab.SelectedIndex > 3)
                return;
            SelectedIsland = Islands[islandSelectionTab.SelectedIndex];
            if (SelectedIsland != null)
            {
                ReloadIslandItemPicture();

                // Reload Island House Pictureboxes
                for (int i = 0; i < 4; i++)
                {
                    var LayerFurnitureMap = ImageGeneration.Draw_Furniture_Arrows((Bitmap)Inventory.GetItemPic(16, 16, SelectedIsland.Cabana.MainRoom.Layers[i].Items, Save_File.Save_Type),
                        SelectedIsland.Cabana.MainRoom.Layers[i].Items);
                    Island_House_Boxes[i].Image = LayerFurnitureMap;
                }
            }
        }

        private void ReloadIslandItemPicture()
        {
            if (SelectedIsland != null)
            {
                if (SelectedIsland.Items != null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Refresh_PictureBox_Image(Island_Acre_Map[i], GenerateAcreItemsBitmap(SelectedIsland.Items[i], i, true));
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// </summary>
    public class PictureBoxWithInterpolationMode : PictureBox
    {
        public InterpolationMode InterpolationMode { get; set; }
        public bool UseInternalInterpolationSetting { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            if (paintEventArgs != null)
            {
                if (UseInternalInterpolationSetting)
                    paintEventArgs.Graphics.InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode; //InterpolationMode;
                else
                    paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
                try { base.OnPaint(paintEventArgs); } catch { }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
        {
            if (UseInternalInterpolationSetting)
                paintEventArgs.Graphics.InterpolationMode = (InterpolationMode)Properties.Settings.Default.ImageResizeMode; //InterpolationMode;
            else
                paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            try { base.OnPaintBackground(paintEventArgs); } catch { }
        }
    }
}