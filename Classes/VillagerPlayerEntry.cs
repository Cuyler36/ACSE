using ACSE.Classes.Utilities;

namespace ACSE
{
    public class PlayerRelation
    {
        public NewVillager VillagerReference;
        public string PlayerName;
        public string PlayerTownName;
        public ushort PlayerId;
        public ushort PlayerTownId;
        public ACDate MetDate;
        string MetTownName;
        ushort MetTownId;
        // byte[] Unknown [10]
        public byte Friendship;
        public byte Flags;
        // byte[] Unknown2 [29]
        public Mail Saved_Letter;
        // byte[] Unknown3 [41]
    }
}