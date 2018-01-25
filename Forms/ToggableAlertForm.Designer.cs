namespace ACSE.Classes
{
    partial class ToggableAlertForm
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
            this.toggleCheckBox = new System.Windows.Forms.CheckBox();
            this.infoLabel = new System.Windows.Forms.Label();
            this.okayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // toggleCheckBox
            // 
            this.toggleCheckBox.AutoSize = true;
            this.toggleCheckBox.Location = new System.Drawing.Point(16, 89);
            this.toggleCheckBox.Name = "toggleCheckBox";
            this.toggleCheckBox.Size = new System.Drawing.Size(172, 17);
            this.toggleCheckBox.TabIndex = 0;
            this.toggleCheckBox.Text = "Don\'t show this message again";
            this.toggleCheckBox.UseVisualStyleBackColor = true;
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(13, 13);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(329, 44);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "Alert";
            // 
            // okayButton
            // 
            this.okayButton.Location = new System.Drawing.Point(140, 60);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(75, 23);
            this.okayButton.TabIndex = 2;
            this.okayButton.Text = "OK";
            this.okayButton.UseVisualStyleBackColor = true;
            // 
            // ToggableAlertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 111);
            this.Controls.Add(this.okayButton);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.toggleCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToggableAlertForm";
            this.ShowInTaskbar = false;
            this.Text = "Alert Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox toggleCheckBox;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button okayButton;
    }
}