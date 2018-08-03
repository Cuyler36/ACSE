using System;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ACSE
{
    public partial class SettingsMenuForm : Form
    {
        private readonly MainForm _mainFormReference;
        private readonly bool _loaded;

        public SettingsMenuForm(MainForm reference)
        {
            _mainFormReference = reference;
            InitializeComponent();

            var enumItems = Enum.GetValues(typeof(InterpolationMode));
            foreach (var Enum in enumItems)
                imageSizeModeComboBox.Items.Add(Enum.ToString());
            imageSizeModeComboBox.SelectedIndex = Properties.Settings.Default.ImageResizeMode;
            debugLevelComboBox.SelectedIndex = (int)Properties.Settings.Default.DebugLevel;
            scanForInt32Checkbox.Checked = Properties.Settings.Default.OutputInt32s;
            BackupCheckBox.Checked = Properties.Settings.Default.BackupFiles;
            townMapSizeTrackBar.Value = Math.Max(0, (Properties.Settings.Default.TownMapSize - 128) / 16);
            acreMapSizeTrackBar.Value = Math.Max(0, (Properties.Settings.Default.AcreMapSize - 64) / 8);

            imageSizeModeComboBox.SelectedIndexChanged += (o, e) => ImageResizeMode_Changed();
            debugLevelComboBox.SelectedIndexChanged += (o, e) => DebugLevel_Changed();
            BackupCheckBox.CheckedChanged += BackupCheckBox_CheckedChanged;
            _loaded = true;
        }

        private void ImageResizeMode_Changed()
        {
            Properties.Settings.Default.ImageResizeMode = imageSizeModeComboBox.SelectedIndex;
            //TODO: Redraw pictureboxes with the image mode, rather than wait to restart
        }

        private void DebugLevel_Changed()
        {
            if (!_loaded) return;
            Properties.Settings.Default.DebugLevel = (DebugLevel)Math.Max(0, debugLevelComboBox.SelectedIndex);
            if (Properties.Settings.Default.DebugLevel == DebugLevel.None)
            {
                MainForm.DebugManager.CloseDebugLogWriter();
                MainForm.DebugManager.DeleteLogFile(MainForm.DebugManager.GetLogFilePath());
            }
            else
            {
                MainForm.DebugManager.InitiateDebugLogWriter();
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
            if (!_loaded) return;
            var newSize = (ushort)(128 + townMapSizeTrackBar.Value * 16);
            newSize = Math.Max((ushort)128, Math.Min((ushort)256, newSize));
            Properties.Settings.Default.TownMapSize = newSize;

            _mainFormReference?.SetMapPictureBoxSize(newSize);
        }

        private void acreMapSizeTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_loaded) return;
            var newSize = (byte)(64 + acreMapSizeTrackBar.Value * 8);
            newSize = Math.Max((byte)64, Math.Min((byte)128, newSize));
            Properties.Settings.Default.AcreMapSize = newSize;

            _mainFormReference?.SetAcreMapPictureBoxSize(newSize);
        }

        private void BackupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BackupFiles = BackupCheckBox.Checked;
        }
    }
}
