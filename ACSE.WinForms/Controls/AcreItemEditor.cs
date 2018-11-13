using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACSE.Core.Items;
using ACSE.Core.Town.Acres;

namespace ACSE.WinForms.Controls
{
    public sealed class AcreItemEditor : ItemEditor
    {
        /// <summary>
        /// The acre whose items are being edited.
        /// </summary>
        public WorldAcre Acre { get; private set; }

        /// <summary>
        /// The <see cref="WorldItem"/>s associated with the acre.
        /// </summary>
        public new Item[] Items => Acre?.Items;


        public AcreItemEditor(MainForm form, Item items, int itemsPerRow)
        {

        }
    }
}
