namespace ACSE.Messages.Mail
{
    public sealed class GcnPlayerMail : PlayerMailBase
    {
        public enum PersonType
        {
            Player = 0,
            Villager = 1,
            Special = 2,
            Unknown = 3 // TODO: Figure out when this is used.
        }

        public sealed class GCMailName
        {
            public string Name;
            public string TownName;
            public ushort PlayerId;
            public ushort TownId;
            public PersonType PersonType;

            public GCMailName(Save saveFile, int offset)
            {
                Name = saveFile.ReadString(offset, 8);
                TownName = saveFile.ReadString(offset + 8, 8);
                PlayerId = saveFile.ReadUInt16(offset + 0x10, true);
                TownId = saveFile.ReadUInt16(offset + 0x12, true);
                PersonType = (PersonType) saveFile.ReadByte(offset + 0x14);
            }
        }

        public readonly int Index;
        public readonly int Offset;

        public GCMailName RecipientInfo;
        public GCMailName SenderInfo;

        public LetterType LetterType;
        public LetterSenderType SenderType;

        public GcnPlayerMail(Save saveFile, Player owner, int index)
        {
            Index = index;
            switch (saveFile.SaveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                    break;
                case SaveType.AnimalCrossing:
                    Offset = owner.Offset + 0x4E0 + index * 0x12A;
                    RecipientInfo = new GCMailName(saveFile, Offset);
                    SenderInfo = new GCMailName(saveFile, Offset + 0x16);
                    Present = new Item(saveFile.ReadUInt16(Offset + 0x2C, true)); // TODO: There has to be a flag that tells the game if the item is wrapped or a quest item.
                    LetterType = (LetterType) saveFile.ReadByte(Offset + 0x2E); // "Font"
                    HeaderReceipiantStartOffset = saveFile.ReadByte(Offset + 0x2F); // How many characters until the recipient's name should be inserted
                    SenderType = (LetterSenderType) saveFile.ReadByte(Offset + 0x30);
                    StationaryType = new Item((ushort)(0x2000 | saveFile.ReadByte(Offset + 0x31)));
                    Header = saveFile.ReadString(Offset + 0x32, 0x18);
                    Contents = saveFile.ReadString(Offset + 0x4A, 0xC0);
                    Footer = saveFile.ReadString(Offset + 0x10A, 0x20);
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                    break;
            }
        }

        public string GetFormattedMailString()
            => string.Format("{0}{1}{2}\n{3}\n{4}",
                Header.Substring(0, HeaderReceipiantStartOffset), Receipiant, Header.Substring(HeaderReceipiantStartOffset), Contents, Footer);
    }
}
