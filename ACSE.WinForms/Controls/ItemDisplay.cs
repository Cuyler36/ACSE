using System;
using System.Drawing;
using System.Windows.Forms;
using ACSE.Core.Items;

namespace ACSE.WinForms.Controls
{
    /// <summary>
    /// A simple control that displays a user-selectable item
    /// </summary>
    public partial class ItemDisplay : UserControl
    {
        public Item Item { get; }

        public ItemDisplay(Item item)
        {
            InitializeComponent();
            BackColor = Color.FromArgb((int)ItemData.GetItemColor(item.Type));
            nameLabel.Text = item.Name;
            IDlabel.Text = $"ID: {item.ItemId:X4}";
            categoryLabel.Text = Enum.GetName(typeof(ItemType), item.Type);
            Item = item;
        }

        private void nameLabel_Click(object sender, EventArgs e)
        {            
            base.OnClick(e);
        }
    }
}
