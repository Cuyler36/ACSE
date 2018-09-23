namespace ACSE
{
    partial class SettingsMenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imageSizeModeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.doneButton = new System.Windows.Forms.Button();
            this.debugLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.scanForInt32Checkbox = new System.Windows.Forms.CheckBox();
            this.townMapSizeTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.acreMapSizeTrackBar = new System.Windows.Forms.TrackBar();
            this.BackupCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.backupFolderTextBox = new System.Windows.Forms.TextBox();
            this.dataFolderBrowseButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.townMapSizeTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.acreMapSizeTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // imageSizeModeComboBox
            // 
            this.imageSizeModeComboBox.FormattingEnabled = true;
            this.imageSizeModeComboBox.Location = new System.Drawing.Point(125, 6);
            this.imageSizeModeComboBox.Name = "imageSizeModeComboBox";
            this.imageSizeModeComboBox.Size = new System.Drawing.Size(121, 21);
            this.imageSizeModeComboBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Image Resize Mode: ";
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(326, 164);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 22);
            this.doneButton.TabIndex = 2;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // debugLevelComboBox
            // 
            this.debugLevelComboBox.FormattingEnabled = true;
            this.debugLevelComboBox.Items.AddRange(new object[] {
            "None",
            "Error",
            "Info",
            "Debug"});
            this.debugLevelComboBox.Location = new System.Drawing.Point(125, 33);
            this.debugLevelComboBox.Name = "debugLevelComboBox";
            this.debugLevelComboBox.Size = new System.Drawing.Size(121, 21);
            this.debugLevelComboBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Debug Message Level:";
            // 
            // scanForInt32Checkbox
            // 
            this.scanForInt32Checkbox.AutoSize = true;
            this.scanForInt32Checkbox.Cursor = System.Windows.Forms.Cursors.Default;
            this.scanForInt32Checkbox.Location = new System.Drawing.Point(10, 168);
            this.scanForInt32Checkbox.Name = "scanForInt32Checkbox";
            this.scanForInt32Checkbox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.scanForInt32Checkbox.Size = new System.Drawing.Size(190, 17);
            this.scanForInt32Checkbox.TabIndex = 7;
            this.scanForInt32Checkbox.Text = ":Scan and Output NL Int32 Offsets";
            this.scanForInt32Checkbox.UseVisualStyleBackColor = true;
            this.scanForInt32Checkbox.CheckedChanged += new System.EventHandler(this.scanForInt32Checkbox_CheckedChanged);
            // 
            // townMapSizeTrackBar
            // 
            this.townMapSizeTrackBar.AutoSize = false;
            this.townMapSizeTrackBar.LargeChange = 2;
            this.townMapSizeTrackBar.Location = new System.Drawing.Point(125, 60);
            this.townMapSizeTrackBar.Maximum = 8;
            this.townMapSizeTrackBar.Name = "townMapSizeTrackBar";
            this.townMapSizeTrackBar.Size = new System.Drawing.Size(104, 22);
            this.townMapSizeTrackBar.TabIndex = 8;
            this.townMapSizeTrackBar.TickFrequency = 2;
            this.townMapSizeTrackBar.Value = 8;
            this.townMapSizeTrackBar.Scroll += new System.EventHandler(this.townMapSizeTrackBar_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Town Tab Image Size:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Acre Tab Image Size:";
            // 
            // acreMapSizeTrackBar
            // 
            this.acreMapSizeTrackBar.AutoSize = false;
            this.acreMapSizeTrackBar.LargeChange = 2;
            this.acreMapSizeTrackBar.Location = new System.Drawing.Point(125, 88);
            this.acreMapSizeTrackBar.Maximum = 8;
            this.acreMapSizeTrackBar.Name = "acreMapSizeTrackBar";
            this.acreMapSizeTrackBar.Size = new System.Drawing.Size(104, 22);
            this.acreMapSizeTrackBar.TabIndex = 10;
            this.acreMapSizeTrackBar.TickFrequency = 2;
            this.acreMapSizeTrackBar.Value = 8;
            this.acreMapSizeTrackBar.Scroll += new System.EventHandler(this.acreMapSizeTrackBar_Scroll);
            // 
            // BackupCheckBox
            // 
            this.BackupCheckBox.AutoSize = true;
            this.BackupCheckBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.BackupCheckBox.Location = new System.Drawing.Point(8, 116);
            this.BackupCheckBox.Name = "BackupCheckBox";
            this.BackupCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.BackupCheckBox.Size = new System.Drawing.Size(105, 17);
            this.BackupCheckBox.TabIndex = 12;
            this.BackupCheckBox.Text = ":Create Backups";
            this.BackupCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Backup Folder:";
            // 
            // backupFolderTextBox
            // 
            this.backupFolderTextBox.Location = new System.Drawing.Point(125, 137);
            this.backupFolderTextBox.Name = "backupFolderTextBox";
            this.backupFolderTextBox.Size = new System.Drawing.Size(195, 20);
            this.backupFolderTextBox.TabIndex = 14;
            // 
            // dataFolderBrowseButton
            // 
            this.dataFolderBrowseButton.Location = new System.Drawing.Point(326, 136);
            this.dataFolderBrowseButton.Name = "dataFolderBrowseButton";
            this.dataFolderBrowseButton.Size = new System.Drawing.Size(75, 22);
            this.dataFolderBrowseButton.TabIndex = 15;
            this.dataFolderBrowseButton.Text = "Browse";
            this.dataFolderBrowseButton.UseVisualStyleBackColor = true;
            this.dataFolderBrowseButton.Click += new System.EventHandler(this.dataFolderBrowseButton_Click);
            // 
            // SettingsMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 197);
            this.Controls.Add(this.dataFolderBrowseButton);
            this.Controls.Add(this.backupFolderTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BackupCheckBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.acreMapSizeTrackBar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.townMapSizeTrackBar);
            this.Controls.Add(this.scanForInt32Checkbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.debugLevelComboBox);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imageSizeModeComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ACSE Settings";
            ((System.ComponentModel.ISupportInitialize)(this.townMapSizeTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.acreMapSizeTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox imageSizeModeComboBox;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.ComboBox debugLevelComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox scanForInt32Checkbox;
        private System.Windows.Forms.TrackBar townMapSizeTrackBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar acreMapSizeTrackBar;
        private System.Windows.Forms.CheckBox BackupCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox backupFolderTextBox;
        private System.Windows.Forms.Button dataFolderBrowseButton;
    }
}