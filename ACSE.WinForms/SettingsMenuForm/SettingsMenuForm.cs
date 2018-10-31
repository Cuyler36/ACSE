using System;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using ACSE.Core.Debug;

namespace ACSE.WinForms
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
                if ((InterpolationMode)Enum != InterpolationMode.Invalid)
                    imageSizeModeComboBox.Items.Add(Enum.ToString());
            imageSizeModeComboBox.SelectedIndex = Properties.Settings.Default.ImageResizeMode;
            debugLevelComboBox.SelectedIndex = Properties.Settings.Default.DebugLevel;
            BackupCheckBox.Checked = Properties.Settings.Default.BackupFiles;
            backupFolderTextBox.Text = Properties.Settings.Default.BackupLocation;
            townMapSizeTrackBar.Value = Math.Max(0, (Properties.Settings.Default.TownMapSize - 128) / 16);
            acreMapSizeTrackBar.Value = Math.Max(0, (Properties.Settings.Default.AcreMapSize - 64) / 8);

            imageSizeModeComboBox.SelectedIndexChanged += (o, e) => ImageResizeMode_Changed();
            debugLevelComboBox.SelectedIndexChanged += (o, e) => DebugLevel_Changed();
            BackupCheckBox.CheckedChanged += BackupCheckBox_CheckedChanged;
            backupFolderTextBox.TextChanged += (_, __) => SetBackupLocation(backupFolderTextBox.Text);
            _loaded = true;
        }

        private void ImageResizeMode_Changed()
        {
            Properties.Settings.Default.ImageResizeMode = imageSizeModeComboBox.SelectedIndex;
            _mainFormReference?.SetMapPictureBoxSize(Properties.Settings.Default.TownMapSize);
            _mainFormReference?.SetAcreMapPictureBoxSize(Properties.Settings.Default.AcreMapSize);
        }

        private void DebugLevel_Changed()
        {
            if (!_loaded) return;
            Properties.Settings.Default.DebugLevel = Math.Max(0, debugLevelComboBox.SelectedIndex);
            if ((DebugLevel) Properties.Settings.Default.DebugLevel == DebugLevel.None)
            {
                MainForm.DebugManager.CloseDebugLogWriter();
                MainForm.DebugManager.DeleteLogFile(DebugManager.GetLogFilePath());
            }
            else
            {
                MainForm.DebugManager.InitializeDebugLogWriter();
            }
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

        private void dataFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog =
                new FolderBrowserDialog {SelectedPath = Properties.Settings.Default.BackupLocation})
            {
                if (folderDialog.ShowDialog() != DialogResult.OK) return;
                SetBackupLocation(folderDialog.SelectedPath);
            }
        }

        private void SetBackupLocation(string location)
        {
            if (!Directory.Exists(location) || location == Properties.Settings.Default.BackupLocation)
            {
                backupFolderTextBox.Text = Properties.Settings.Default.BackupLocation;
                MessageBox.Show(
                    $"The folder at {location} doesn't appear to exist, or is already the current backup folder!",
                    "Backup Folder Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                backupFolderTextBox.Text = location;
                Properties.Settings.Default.BackupLocation = location;
                Properties.Settings.Default.Save();
            }
        }
    }
}
