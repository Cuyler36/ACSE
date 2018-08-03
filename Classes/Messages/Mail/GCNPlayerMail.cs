namespace ACSE.Messages.Mail
{
    public class GcnPlayerMail : PlayerMailBase
    {
        public readonly int Index;
        public readonly int Offset;

        public GcnPlayerMail(Save saveFile, Player owner, int index)
        {
            Index = index;
            switch (saveFile.SaveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                    break;
                case SaveType.AnimalCrossing:
                    Offset = owner.Offset + 0x4E0 + index * 0x12A;
                    Receipiant = saveFile.ReadString(Offset, 8);
                    TownName = saveFile.ReadString(Offset + 8, 8);
                    ReceipiantId = saveFile.ReadUInt16(Offset + 0x10, true);
                    TownId = saveFile.ReadUInt16(Offset + 0x12, true);
                    // Unknown @ 13, receipiant type?
                    // Unknown @ 14
                    Sender = saveFile.ReadString(Offset + 0x16, 8);
                    SenderTownName = saveFile.ReadString(Offset + 0x1E, 8);
                    SenderId = saveFile.ReadUInt16(Offset + 0x26, true);
                    SenderTownId = saveFile.ReadUInt16(Offset + 0x28, true);
                    // Unknown @ 2A, might be Event Flags?
                    // Unknown @ 2B
                    Present = new Item(saveFile.ReadUInt16(Offset + 0x2C, true)); // TODO: There has to be a flag that tells the game if the item is wrapped or a quest item.
                    Read = saveFile.ReadByte(Offset + 0x2E) == 1; // Check this.
                    HeaderReceipiantStartOffset = saveFile.ReadByte(Offset + 0x2F); // How many characters until the receipiant's name should be inserted
                    // Unknown @ 30, might be Sender Type?
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
