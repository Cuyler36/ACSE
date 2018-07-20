using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Classes.Saves
{
    /// <summary>
    /// Wii Save File
    /// </summary>
    class WiiSave : SaveBase
    {
        public WiiSave(string path) : base(path, true)
        {
            Generation = SaveGeneration.Wii;
            SaveType = SaveType.City_Folk;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
