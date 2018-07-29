namespace ACSE.Messages.Mail
{
    public class GCNPlayerMail : PlayerMailBase
    {
        public readonly int Index;
        public readonly int Offset;

        public GCNPlayerMail(Save SaveFile, Player Owner, int Index)
        {
            this.Index = Index;
            switch (SaveFile.Save_Type)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                    break;
                case SaveType.Animal_Crossing:
                    Offset = Owner.Offset + 0x4E0 + Index * 0x12A;
                    Receipiant = SaveFile.ReadString(Offset, 8);
                    TownName = SaveFile.ReadString(Offset + 8, 8);
                    ReceipiantID = SaveFile.ReadUInt16(Offset + 0x10, true);
                    TownID = SaveFile.ReadUInt16(Offset + 0x12, true);
                    Sender = SaveFile.ReadString(Offset + 0x16, 8);
                    SenderTownName = SaveFile.ReadString(Offset + 0x1E, 8);
                    SenderID = SaveFile.ReadUInt16(Offset + 0x26, true);
                    SenderTownID = SaveFile.ReadUInt16(Offset + 0x28, true);
                    // Unknown @ 2A, might be Event Flags?
                    // Unknown @ 2B
                    Present = new Item(SaveFile.ReadUInt16(Offset + 0x2C, true)); // TODO: There has to be a flag that tells the game if the item is wrapped or a quest item.
                    Read = SaveFile.ReadByte(Offset + 0x2E) == 1; // Check this.
                    // Unknown @ 2F
                    // Unknown @ 30, might be Sender Type?
                    StationaryType = new Item((ushort)(0x2000 | SaveFile.ReadByte(Offset + 0x31)));
                    Contents = SaveFile.ReadString(Offset + 0x32, 0xF8);
                    break;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    break;
            }
        }
    }
}
