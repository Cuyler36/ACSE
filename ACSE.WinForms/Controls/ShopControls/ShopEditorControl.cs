using System.Globalization;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Patterns;
using ACSE.Core.Town.Shops;

namespace ACSE.WinForms.Controls.ShopControls
{
    public class ShopEditorControl : Panel
    {
        protected ItemEditor ShopEditor;
        protected Label ShopLabel;
        protected NumericTextBox BellsSumTextBox;
        protected Pattern[] Patterns;

        public Shop Shop;

        public Item[] Items
        {
            get => Shop.Stock;
            set
            {
                ShopEditor.Items = value;
                Shop.Stock = value;
            }
        }

        public ShopEditorControl(MainForm mainWindow, Shop shop, int itemsPerRow, bool hasBellsSum = false)
        {
            Shop = shop;

            ShopLabel = new Label
            {
                Text = shop.Name,
                Dock = DockStyle.Top
            };

            if (hasBellsSum)
            {
                BellsSumTextBox = new NumericTextBox
                {
                    Text = shop.BellsSum.ToString(),
                    MaxLength = 10
                };

                BellsSumTextBox.TextChanged += delegate
                {
                    if (uint.TryParse(BellsSumTextBox.Text, NumberStyles.HexNumber, null, out var value))
                    {
                        shop.BellsSum = value;
                    }
                };

                Controls.Add(BellsSumTextBox);
            }

            ShopEditor = new ItemEditor(mainWindow, shop.Stock, itemsPerRow, 16);

            var width = ShopLabel.Size.Width > ShopEditor.Size.Width ? ShopLabel.Size.Width : ShopEditor.Size.Width;
            Size = new System.Drawing.Size(width, ShopLabel.Size.Height + ShopEditor.Size.Height);

            ShopEditor.Location = new System.Drawing.Point((width - ShopEditor.Size.Width) / 2, ShopLabel.Size.Height);
            Controls.Add(ShopLabel);
            Controls.Add(ShopEditor);
        }
    }
}
