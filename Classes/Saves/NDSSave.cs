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
        public NDSSave(string path) : base(path, false)
        {
            Generation = SaveGeneration.NDS;
            SaveType = SaveType.WildWorld;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
