namespace ACSE.WinForms.Controls
{
    partial class ItemDisplay
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
            this.IDlabel = new System.Windows.Forms.Label();
            this.categoryLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoEllipsis = true;
            this.nameLabel.BackColor = System.Drawing.Color.Transparent;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.nameLabel.Location = new System.Drawing.Point(10, 10);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(150, 20);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "Item Name";
            this.nameLabel.Click += new System.EventHandler(this.nameLabel_Click);
            // 
            // IDlabel
            // 
            this.IDlabel.AutoSize = true;
            this.IDlabel.BackColor = System.Drawing.Color.Transparent;
            this.IDlabel.Location = new System.Drawing.Point(11, 30);
            this.IDlabel.Name = "IDlabel";
            this.IDlabel.Size = new System.Drawing.Size(18, 13);
            this.IDlabel.TabIndex = 4;
            this.IDlabel.Text = "ID";
            // 
            // categoryLabel
            // 
            this.categoryLabel.AutoSize = true;
            this.categoryLabel.BackColor = System.Drawing.Color.Transparent;
            this.categoryLabel.Location = new System.Drawing.Point(11, 45);
            this.categoryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.categoryLabel.Name = "categoryLabel";
            this.categoryLabel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.categoryLabel.Size = new System.Drawing.Size(59, 13);
            this.categoryLabel.TabIndex = 5;
            this.categoryLabel.Text = "Category";
            // 
            // ItemDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.categoryLabel);
            this.Controls.Add(this.IDlabel);
            this.Controls.Add(this.nameLabel);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "ItemDisplay";
            this.Size = new System.Drawing.Size(173, 69);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label IDlabel;
        private System.Windows.Forms.Label categoryLabel;
    }
}
