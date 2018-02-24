using System;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ACSE
{
    public partial class SettingsMenuForm : Form
    {
        MainForm MainFormReference;
        private bool Loaded = false;

        public SettingsMenuForm(MainForm Reference)
        {
            MainFormReference = Reference;
            InitializeComponent();
            Array EnumItems = Enum.GetValues(typeof(InterpolationMode));
            foreach (var Enum in EnumItems)
                imageSizeModeComboBox.Items.Add(Enum.ToString());
            imageSizeModeComboBox.SelectedIndex = Properties.Settings.Default.ImageResizeMode;
            imageSizeModeComboBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => ImageResizeMode_Changed());
            debugLevelComboBox.SelectedIndex = (int)Properties.Settings.Default.DebugLevel;
            debugLevelComboBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => DebugLevel_Changed());
            scanForInt32Checkbox.Checked = Properties.Settings.Default.OutputInt32s;
            townMapSizeTrackBar.Value = Math.Max(0, (Properties.Settings.Default.TownMapSize - 128) / 16);
            acreMapSizeTrackBar.Value = Math.Max(0, (Properties.Settings.Default.AcreMapSize - 64) / 8);
            Loaded = true;
        }

        private void ImageResizeMode_Changed()
        {
            Properties.Settings.Default.ImageResizeMode = imageSizeModeComboBox.SelectedIndex;
            //TODO: Redraw pictureboxes with the image mode, rather than wait to restart
        }

        private void DebugLevel_Changed()
        {
            if (Loaded)
            {
                Properties.Settings.Default.DebugLevel = (DebugLevel)Math.Max(0, debugLevelComboBox.SelectedIndex);
                if (Properties.Settings.Default.DebugLevel == DebugLevel.None)
                {
                    MainForm.Debug_Manager.CloseDebugLogWriter();
                }
                else
                {
                    MainForm.Debug_Manager.InitiateDebugLogWriter();
                }
            }
        }

        private void scanForInt32Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.OutputInt32s = scanForInt32Checkbox.Checked;
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Hide();
        }

        private void townMapSizeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (Loaded)
            {
                ushort NewSize = (ushort)(128 + townMapSizeTrackBar.Value * 16);
                NewSize = Math.Max((ushort)128, Math.Min((ushort)256, NewSize));
                Properties.Settings.Default.TownMapSize = NewSize;

                if (MainFormReference != null)
                    MainFormReference.SetMapPictureBoxSize(NewSize);
            }
        }

        private void acreMapSizeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (Loaded)
            {
                byte NewSize = (byte)(64 + acreMapSizeTrackBar.Value * 8);
                NewSize = Math.Max((byte)64, Math.Min((byte)128, NewSize));
                Properties.Settings.Default.AcreMapSize = NewSize;

                if (MainFormReference != null)
                    MainFormReference.SetAcreMapPictureBoxSize(NewSize);
            }
        }
    }
}
