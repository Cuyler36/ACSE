namespace ACSE.WinForms.Forms
{
    partial class ItemSelectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemSelectionDialog));
            this.selectionLabel = new System.Windows.Forms.Label();
            this._label1 = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.categorySwitcher = new System.Windows.Forms.TabControl();
            this.pageFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.searchButton = new System.Windows.Forms.Button();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.loadingProgress = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.loadingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectionLabel
            // 
            this.selectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionLabel.AutoSize = true;
            this.selectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectionLabel.Location = new System.Drawing.Point(70, 418);
            this.selectionLabel.Name = "selectionLabel";
            this.selectionLabel.Size = new System.Drawing.Size(50, 15);
            this.selectionLabel.TabIndex = 13;
            this.selectionLabel.Text = "Nothing";
            // 
            // _label1
            // 
            this._label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._label1.AutoSize = true;
            this._label1.Location = new System.Drawing.Point(12, 420);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(52, 13);
            this._label1.TabIndex = 12;
            this._label1.Text = "Selected:";
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(697, 415);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(91, 23);
            this.CancelButton.TabIndex = 11;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(512, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Item Name:";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Location = new System.Drawing.Point(573, 12);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(146, 20);
            this.searchBox.TabIndex = 7;
            this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchBox_KeyPress);
            // 
            // categorySwitcher
            // 
            this.categorySwitcher.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.categorySwitcher.Location = new System.Drawing.Point(12, 38);
            this.categorySwitcher.Name = "categorySwitcher";
            this.categorySwitcher.SelectedIndex = 0;
            this.categorySwitcher.Size = new System.Drawing.Size(776, 371);
            this.categorySwitcher.TabIndex = 14;
            this.categorySwitcher.Selected += new System.Windows.Forms.TabControlEventHandler(this.categorySwitcher_Selected);
            this.categorySwitcher.TabIndexChanged += new System.EventHandler(this.categorySwitcher_TabIndexChanged);
            // 
            // pageFlow
            // 
            this.pageFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pageFlow.AutoScroll = true;
            this.pageFlow.Location = new System.Drawing.Point(13, 5);
            this.pageFlow.Name = "pageFlow";
            this.pageFlow.Size = new System.Drawing.Size(493, 32);
            this.pageFlow.TabIndex = 15;
            this.pageFlow.WrapContents = false;
            // 
            // searchButton
            // 
            this.searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchButton.Location = new System.Drawing.Point(725, 10);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(63, 23);
            this.searchButton.TabIndex = 16;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // loadingPanel
            // 
            this.loadingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadingPanel.Controls.Add(this.loadingProgress);
            this.loadingPanel.Controls.Add(this.label2);
            this.loadingPanel.Location = new System.Drawing.Point(14, 41);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(774, 368);
            this.loadingPanel.TabIndex = 17;
            this.loadingPanel.Visible = false;
            // 
            // loadingProgress
            // 
            this.loadingProgress.Location = new System.Drawing.Point(338, 185);
            this.loadingProgress.Name = "loadingProgress";
            this.loadingProgress.Size = new System.Drawing.Size(111, 23);
            this.loadingProgress.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(354, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Loading...";
            // 
            // ItemSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.loadingPanel);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.pageFlow);
            this.Controls.Add(this.categorySwitcher);
            this.Controls.Add(this.selectionLabel);
            this.Controls.Add(this._label1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemSelectionDialog";
            this.Text = "Item Selection";
            this.loadingPanel.ResumeLayout(false);
            this.loadingPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label selectionLabel;
        private System.Windows.Forms.Label _label1;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.TabControl categorySwitcher;
        private System.Windows.Forms.FlowLayoutPanel pageFlow;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.ProgressBar loadingProgress;
        private System.Windows.Forms.Label label2;
    }
}