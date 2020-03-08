namespace ACSE.WinForms.Controls
{
    partial class MessageEditor
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
            this.MessageContents = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // MessageContents
            // 
            this.MessageContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageContents.Location = new System.Drawing.Point(3, 59);
            this.MessageContents.Multiline = true;
            this.MessageContents.Name = "MessageContents";
            this.MessageContents.Size = new System.Drawing.Size(344, 138);
            this.MessageContents.TabIndex = 0;
            // 
            // MessageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MessageContents);
            this.Name = "MessageEditor";
            this.Size = new System.Drawing.Size(350, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox MessageContents;
    }
}
