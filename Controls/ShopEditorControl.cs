using System.Windows.Forms;

namespace ACSE
{
    class ShopEditorControl : Panel
    {
        protected ItemEditor ShopEditor;
        protected Label ShopLabel;

        public Item[] Items { get => ShopEditor.Items; set => ShopEditor.Items = value; }

        public ShopEditorControl(MainForm MainWindow, string ShopName, Item[] ShopItems, int ItemsPerRow)
        {
            ShopLabel = new Label
            {
                Text = ShopName,
                Dock = DockStyle.Top
            };

            ShopEditor = new ItemEditor(MainWindow, ShopItems, ItemsPerRow, 16);

            int Width = ShopLabel.Size.Width > ShopEditor.Size.Width ? ShopLabel.Size.Width : ShopEditor.Size.Width;
            Size = new System.Drawing.Size(Width, ShopLabel.Size.Height + ShopEditor.Size.Height);

            ShopEditor.Location = new System.Drawing.Point((Width - ShopEditor.Size.Width) / 2, ShopLabel.Size.Height);
            Controls.Add(ShopLabel);
            Controls.Add(ShopEditor);
        }
    }
}
