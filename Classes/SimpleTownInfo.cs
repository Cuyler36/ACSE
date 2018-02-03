namespace ACSE
{
    public enum GrassType
    {
        Circle,
        Square,
        Triangle
    }

    public class SimpleTownInfo
    {
        public ushort TownID;
        public string TownName;
        public GrassType GrassType;

        public SimpleTownInfo(ushort TownId, string Town_Name)
        {
            TownID = TownId;
            TownName = Town_Name;
        }
    }
}
