using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ACSE
{
    public partial class SettingsMenuForm : Form
    {
        public SettingsMenuForm()
        {
            InitializeComponent();
            Array EnumItems = Enum.GetValues(typeof(InterpolationMode));
            foreach (var Enum in EnumItems)
                imageSizeModeComboBox.Items.Add(Enum.ToString());
            imageSizeModeComboBox.SelectedIndex = Properties.Settings.Default.ImageResizeMode;
            imageSizeModeComboBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => ImageResizeMode_Changed());
            debugLevelComboBox.SelectedIndex = (int)Properties.Settings.Default.DebugLevel;
            debugLevelComboBox.SelectedIndexChanged += new EventHandler((object o, EventArgs e) => DebugLevel_Changed());
            scanForInt32Checkbox.Checked = Properties.Settings.Default.OutputInt32s;
        }

        private void ImageResizeMode_Changed()
        {
            Properties.Settings.Default.ImageResizeMode = imageSizeModeComboBox.SelectedIndex;
            //TODO: Redraw pictureboxes with the image mode, rather than wait to restart
        }

        private void DebugLevel_Changed()
        {
            Properties.Settings.Default.DebugLevel = (DebugLevel)Math.Max(0, debugLevelComboBox.SelectedIndex);
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
    }
}
