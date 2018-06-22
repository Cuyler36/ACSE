namespace ACSE.Forms
{
    partial class CodeGeneratorForm
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
            this.codeTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // codeTypeComboBox
            // 
            this.codeTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.codeTypeComboBox.FormattingEnabled = true;
            this.codeTypeComboBox.Items.AddRange(new object[] {
            "NES",
            "Villager (DnM+, AC)",
            "e-Reader Card",
            "Magazine Contest",
            "Player-to-Player",
            "e-Reader Card Mini",
            "Villager (DnMe+)",
            "Object Delivery Service"});
            this.codeTypeComboBox.Location = new System.Drawing.Point(12, 25);
            this.codeTypeComboBox.Name = "codeTypeComboBox";
            this.codeTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.codeTypeComboBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Code Type:";
            // 
            // CodeGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.codeTypeComboBox);
            this.Name = "CodeGeneratorForm";
            this.Text = "CodeGeneratorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox codeTypeComboBox;
        private System.Windows.Forms.Label label1;
    }
}