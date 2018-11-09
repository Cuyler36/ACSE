using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACSE.Core.Town.Shops;

namespace ACSE.WinForms.Controls.ShopControls
{
    public class FurnitureShopEditorControl : ShopEditorControl
    {
        public FurnitureShopEditorControl(MainForm form, Shop shop, int itemsPerRow, bool hasBellsSum = false)
            : base(form, shop, itemsPerRow, hasBellsSum)
        {

        }
    }
}
