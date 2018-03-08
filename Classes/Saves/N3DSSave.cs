using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Classes.Saves
{
    /// <summary>
    /// Nintendo 3DS Save File
    /// </summary>
    class N3DSSave : SaveBase
    {
        public N3DSSave(string path) : base(path)
        {
            Generation = SaveGeneration.N3DS;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
