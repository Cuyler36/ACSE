using System.Windows.Forms;
using ACSE.Core.Items;

namespace ACSE.WinForms.Controls
{
    public class ShopEditorControl : Panel
    {
        protected ItemEditor ShopEditor;
        protected Label ShopLabel;

        public Item[] Items { get => ShopEditor.Items; set => ShopEditor.Items = value; }

        public ShopEditorControl(MainForm mainWindow, string shopName, Item[] shopItems, int itemsPerRow)
        {
            ShopLabel = new Label
            {
                Text = shopName,
                Dock = DockStyle.Top
            };

            ShopEditor = new ItemEditor(mainWindow, shopItems, itemsPerRow, 16);

            var width = ShopLabel.Size.Width > ShopEditor.Size.Width ? ShopLabel.Size.Width : ShopEditor.Size.Width;
            Size = new System.Drawing.Size(width, ShopLabel.Size.Height + ShopEditor.Size.Height);

            ShopEditor.Location = new System.Drawing.Point((width - ShopEditor.Size.Width) / 2, ShopLabel.Size.Height);
            Controls.Add(ShopLabel);
            Controls.Add(ShopEditor);
        }
    }
}
