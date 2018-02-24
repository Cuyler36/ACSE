using ACSE.Classes.Utilities;

namespace ACSE
{
    public abstract class PlayerRelation
    {
        public int Offset { get; protected set; }
        public Save SaveFile { get; protected set; }
        public NewVillager Villager { get; protected set; }
        public NewPlayer Player { get; protected set; }
        public string PlayerName;
        public string PlayerTownName;
        public ushort PlayerId;
        public ushort PlayerTownId;
        public ACDate MetDate;
        public string MetTownName;
        public ushort MetTownId;
        // byte[] Unknown [10]
        public byte Friendship;
        public byte Flags;
        // byte[] Unknown2 [29]
        public Mail Saved_Letter;
        // byte[] Unknown3 [41]
    }

    public class ACPlayerRelation : PlayerRelation
    {
        public byte[] Unknown1;
        public byte[] Unknown2;
        public byte[] Unknown3;

        public ACPlayerRelation(Save SaveFile, NewVillager Villager, int DataOffset)
        {
            this.SaveFile = SaveFile;
            this.Villager = Villager;
            Offset = DataOffset;

            PlayerName = SaveFile.ReadString(Offset, 8);
            PlayerTownName = SaveFile.ReadString(Offset + 8, 8);
            PlayerId = SaveFile.ReadUInt16(Offset + 0x10, true);
            PlayerTownId = SaveFile.ReadUInt16(Offset + 0x12, true);
            MetDate = new ACDate(SaveFile.ReadByteArray(Offset + 0x14, 8));
            MetTownName = SaveFile.ReadString(Offset + 0x1C, 8);
            MetTownId = SaveFile.ReadUInt16(Offset + 0x24);
            Unknown1 = SaveFile.ReadByteArray(Offset + 0x26, 0x0A);
            Friendship = SaveFile.ReadByte(Offset + 0x30);
            Flags = SaveFile.ReadByte(Offset + 0x31);
            // Mail Bytes? 0x5
            // Mail Body: 0xF8
        }
    }
}