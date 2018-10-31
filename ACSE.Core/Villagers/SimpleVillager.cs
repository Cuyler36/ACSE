using ACSE.Core.Items;

namespace ACSE.Core.Villagers
{
    public class SimpleVillager
    {
        public ushort VillagerId;
        public byte Personality;
        public string Name;
        public string Catchphrase;
        public Item Shirt;

        public Item[] Furniture;
        //public uint AI (Last bytes in NL Villagers?)

        public override string ToString()
        {
            return Name ?? "Unknown";
        }
    }
}
