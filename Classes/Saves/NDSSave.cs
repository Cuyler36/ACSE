using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Classes.Saves
{
    /// <summary>
    /// Nintendo DS Save File
    /// </summary>
    class NDSSave : SaveBase
    {
        public NDSSave(string path) : base(path)
        {
            Generation = SaveGeneration.NDS;
            SaveType = SaveType.Wild_World;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
