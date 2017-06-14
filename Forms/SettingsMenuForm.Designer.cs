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
            this.label2 = new System.Windows.Forms.Label();
            this.debugLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
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
            this.doneButton.Location = new System.Drawing.Point(317, 132);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 2;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "(Requires Restart)";
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
            // SettingsMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 167);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.debugLevelComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imageSizeModeComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsMenuForm";
            this.Text = "ACSE Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox imageSizeModeComboBox;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox debugLevelComboBox;
        private System.Windows.Forms.Label label3;
    }
}