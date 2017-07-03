using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    public class SimpleTownInfo
    {
        public ushort TownID;
        public string TownName;
        // More??

        public SimpleTownInfo(ushort TownId, string Town_Name)
        {
            TownID = TownId;
            TownName = Town_Name;
        }
    }
}
