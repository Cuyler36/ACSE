using ACSE.Core.Messages.Mail;
using ACSE.Core.Players;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Villagers.AnimalMemories
{
    public abstract class AnimalMemory
    {
        public bool Exists;

        public int Offset { get; protected set; }
        public Save SaveFile { get; protected set; }
        public Villager Villager { get; protected set; }
        public Player Player;
        public string PlayerName;
        public string PlayerTownName;
        public ushort PlayerId;
        public ushort PlayerTownId;
        public AcDate MetDate;
        public string MetTownName;
        public ushort MetTownId;
        // byte[] Unknown [10]
        public byte Friendship;
        public byte Flags;
        // byte[] Unknown2 [29]
        public MailBase SavedLetter;
        // byte[] Unknown3 [41]

        public abstract void Write();
    }

    public class AnimalCrossingAnimalMemory : AnimalMemory
    {
        public byte[] Unknown1;
        public byte[] Unknown2;
        public byte[] Unknown3;

        public AnimalCrossingAnimalMemory(Save saveFile, Villager villager, int dataOffset)
        {
            SaveFile = saveFile;
            Villager = villager;
            Offset = dataOffset;

            PlayerName = saveFile.ReadString(Offset, 8);
            PlayerTownName = saveFile.ReadString(Offset + 8, 8);
            PlayerId = saveFile.ReadUInt16(Offset + 0x10, true);
            PlayerTownId = saveFile.ReadUInt16(Offset + 0x12, true);
            MetDate = new AcDate(saveFile.ReadByteArray(Offset + 0x14, 8));
            MetTownName = saveFile.ReadString(Offset + 0x1C, 8);
            MetTownId = saveFile.ReadUInt16(Offset + 0x24, true);
            Unknown1 = saveFile.ReadByteArray(Offset + 0x26, 0x0A);
            Friendship = saveFile.ReadByte(Offset + 0x30);
            Flags = saveFile.ReadByte(Offset + 0x31);
            // Mail Bytes? 0x5
            // Mail Body: 0xF8

            Exists = PlayerId != 0xFFFF;
        }

        public override void Write()
        {
            SaveFile.Write(Offset, AcString.GetBytes(PlayerName, 8));
            SaveFile.Write(Offset + 8, AcString.GetBytes(PlayerTownName, 8));
            SaveFile.Write(Offset + 0x10, PlayerId, true);
            SaveFile.Write(Offset + 0x12, PlayerTownId, true);
            // Met Date
            SaveFile.Write(Offset + 0x1C, AcString.GetBytes(MetTownName, 8));
            SaveFile.Write(Offset + 0x24, MetTownId, true);
            // Unknown 1
            SaveFile.Write(Offset + 0x30, Friendship);
            SaveFile.Write(Offset + 0x31, Flags);
            // Letter Data
        }
    }
}