using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Saves
{
    /// <summary>
    /// Nintendo 64 Save File
    /// </summary>
    class N64Save : SaveBase
    {
        public N64Save(string path) : base(path, true)
        {
            Generation = SaveGeneration.N64;
            SaveType = SaveType.DoubutsuNoMori;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
