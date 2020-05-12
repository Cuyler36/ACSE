namespace ACSE.WinForms.Controls
{
    partial class VillagerDisplay
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nameLabel = new System.Windows.Forms.Label();
            this.wikiButton = new System.Windows.Forms.Button();
            this.villagerPreview = new ACSE.WinForms.Controls.OffsetablePictureBox();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.nameLabel.Location = new System.Drawing.Point(80, 21);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(107, 20);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Villager Name";
            this.nameLabel.Click += new System.EventHandler(this.nameLabel_Click);
            // 
            // wikiButton
            // 
            this.wikiButton.Location = new System.Drawing.Point(84, 44);
            this.wikiButton.Name = "wikiButton";
            this.wikiButton.Size = new System.Drawing.Size(143, 23);
            this.wikiButton.TabIndex = 2;
            this.wikiButton.Text = "More Information";
            this.wikiButton.UseVisualStyleBackColor = true;
            this.wikiButton.Click += new System.EventHandler(this.wikiButton_Click);
            // 
            // villagerPreview
            // 
            this.villagerPreview.Image = null;
            this.villagerPreview.Location = new System.Drawing.Point(10, 10);
            this.villagerPreview.Name = "villagerPreview";
            this.villagerPreview.Offset = new System.Drawing.Point(0, 0);
            this.villagerPreview.Size = new System.Drawing.Size(64, 64);
            this.villagerPreview.TabIndex = 0;
            this.villagerPreview.Click += new System.EventHandler(this.villagerPreview_Click_1);
            // 
            // VillagerDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.wikiButton);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.villagerPreview);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "VillagerDisplay";
            this.Size = new System.Drawing.Size(244, 83);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OffsetablePictureBox villagerPreview;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Button wikiButton;
    }
}
