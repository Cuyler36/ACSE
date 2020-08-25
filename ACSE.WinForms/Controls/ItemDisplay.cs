using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACSE.Core.Items;
using ACSE.Core.Saves;

namespace ACSE.WinForms.Controls
{
    /// <summary>
    /// A simple control that displays a user-selectable item
    /// </summary>
    public partial class ItemDisplay : UserControl
    {
        public Item Item
        {
            get;
        }
        public ItemDisplay(Core.Items.Item item)
        {
            InitializeComponent();
            //preview.Image = Inventory.GetItemPic(70, item, Save.SaveInstance.SaveType);
            BackColor = Color.FromArgb((int)ItemData.GetItemColor(item.Type));
            nameLabel.Text = item.Name;
            IDlabel.Text = "ID: " + item.ItemId.ToString("X4");
            categoryLabel.Text = Enum.GetName(typeof(ItemType), item.Type);
            Item = item;
        }

        private void nameLabel_Click(object sender, EventArgs e)
        {            
            base.OnClick(e);
        }
    }
}
