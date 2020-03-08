using ACSE.Core.Items;
using ACSE.Core.Saves;

namespace ACSE.Core.Messages.Mail
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

            public GCMailName(Save saveFile, int offset, int stringSize = 8)
            {
                var stringSizeDoubled = stringSize * 2;
                Name = saveFile.ReadString(offset, stringSize);
                TownName = saveFile.ReadString(offset + stringSize, stringSize);
                PlayerId = saveFile.ReadUInt16(offset + stringSizeDoubled, true);
                TownId = saveFile.ReadUInt16(offset + stringSizeDoubled + 2, true);
                PersonType = (PersonType) saveFile.ReadByte(offset + stringSizeDoubled + 4);
            }
        }

        public readonly int Index;
        public readonly int Offset;

        public GCMailName RecipientInfo;
        public GCMailName SenderInfo;

        public LetterType LetterType;
        public LetterSenderType SenderType;

        public GcnPlayerMail(Save saveFile, int offset, int index = -1)
        {
            Index = index;
            Offset = offset;
            var stringSize = -1;


            switch (saveFile.SaveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                    break;
                case SaveType.AnimalCrossing:
                    stringSize = 8;

                    Header = saveFile.ReadString(Offset + 0x32, 0x18);
                    Contents = saveFile.ReadString(Offset + 0x4A, 0xC0);
                    Footer = saveFile.ReadString(Offset + 0x10A, 0x20);
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    stringSize = 6;

                    Header = saveFile.ReadString(Offset + 0x2A, 0xA);
                    Contents = saveFile.ReadString(Offset + 0x38, 0x60);
                    Footer = saveFile.ReadString(Offset + 0xA6, 0x10);
                    break;
            }

            var mailNameSize = stringSize * 2 + 6;
            RecipientInfo = new GCMailName(saveFile, Offset);
            SenderInfo = new GCMailName(saveFile, Offset + mailNameSize);

            var mailInfoStart = Offset + mailNameSize * 2;
            Present = new Item(saveFile.ReadUInt16(mailInfoStart, true));
            LetterType = (LetterType) saveFile.ReadByte(mailInfoStart + 2);
            HeaderReceipiantStartOffset = saveFile.ReadByte(mailInfoStart + 3);
            SenderType = (LetterSenderType) saveFile.ReadByte(mailInfoStart + 4);
            StationaryType = new Item((ushort) (0x2000 | saveFile.ReadByte(mailInfoStart + 5)));
        }

        public string GetFormattedMailString()
            => $"{Header.Substring(0, HeaderReceipiantStartOffset)}{RecipientInfo.Name}{Header.Substring(HeaderReceipiantStartOffset)}\n{Contents}\n{Footer}";

        public bool IsMailUsed => LetterType == LetterType.None;
    }
}
