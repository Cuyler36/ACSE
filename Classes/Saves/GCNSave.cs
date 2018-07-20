using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACSE.Classes.Saves
{
    /// <summary>
    /// Nintendo GameCube Save File
    /// </summary>
    class GCNSave : SaveBase
    {
        public GCNSave(string path) : base(path, true)
        {
            Generation = SaveGeneration.GCN;
        }

        protected override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
