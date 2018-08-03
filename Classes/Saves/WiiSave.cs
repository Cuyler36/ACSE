using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Saves
{
    /// <summary>
    /// Wii Save File
    /// </summary>
    class WiiSave : SaveBase
    {
        public WiiSave(string path) : base(path, true)
        {
            Generation = SaveGeneration.Wii;
            SaveType = SaveType.CityFolk;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
