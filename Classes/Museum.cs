namespace ACSE
{
    class Museum
    {
        private static int GetMuseumFieldSize(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                    return 0x3F;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return 0x47;
                case SaveType.Wild_World:
                    return 0x5F;
                case SaveType.City_Folk:
                    return 0x6C;
                case SaveType.New_Leaf:
                case SaveType.Welcome_Amiibo:
                    return 0x112;
                default:
                    return 0;
            }
        }

        // City Folk Breakdown:
        //  First 30 bytes: Fossils (60);
        //  Next 32 bytes: Fish (64);
        //  Next 32 bytes: Insects (64);
        //  Last 14 bytes: Paintings (22) (First 3 bytes aren't used?);

        private static int GetBaseOffset(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                    return 0x1A298; // Verify this.
                case SaveType.Animal_Crossing:
                    return 0x213A8;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return 0x22FB0;
                case SaveType.Wild_World:
                    return 0x15D50;
                case SaveType.City_Folk:
                    return 0x7352A;
                case SaveType.New_Leaf:
                    return 0x65860;
                case SaveType.Welcome_Amiibo:
                    return 0x6B280;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Fills the Museum's donations. The selected Player will be the one who donated them.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">The Player who it will show as the donor</param>
        public static void FillMuseum(Save saveFile, Player player)
        {
            int MuseumDataOffset = saveFile.Save_Data_Start_Offset + GetBaseOffset(saveFile.Save_Type);
            if (MuseumDataOffset != -1)
            {
                int MuseumDataSize = GetMuseumFieldSize(saveFile.Save_Type);
                byte PlayerDonationIndex = saveFile.Save_Generation == SaveGeneration.N3DS ? 
                    (byte)(player.Index + 1) : (byte)(((player.Index + 1) << 4) | (byte)(player.Index + 1));

                for (int i = MuseumDataOffset; i < MuseumDataOffset + MuseumDataSize; i++)
                {
                    saveFile.Write(i, PlayerDonationIndex);
                }

                // Set Date/Time for donation to current system time
                if (saveFile.Save_Generation == SaveGeneration.N3DS)
                {
                    byte[] NowDate = new Classes.Utilities.ACDate().ToYearMonthDayDateData();
                    int DonationDateOffset = saveFile.Save_Data_Start_Offset + (saveFile.Save_Type == SaveType.New_Leaf ? 0x658C8 : 0x6AE38);
                    for (int i = DonationDateOffset; i < DonationDateOffset + 0x448; i += 4)
                    {
                        saveFile.Write(i, NowDate);
                    }
                }
            }
        }

        /// <summary>
        /// Clears the Museum's donations.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        public static void ClearMuseum(Save saveFile)
        {
            int MuseumDataOffset = saveFile.Save_Data_Start_Offset + GetBaseOffset(saveFile.Save_Type);
            if (MuseumDataOffset != -1)
            {
                int MuseumDataSize = GetMuseumFieldSize(saveFile.Save_Type);
                for (int i = MuseumDataOffset; i < MuseumDataOffset + MuseumDataSize; i++)
                {
                    saveFile.Write(i, (byte)0);
                }

                // Clear Date/Time for donation to current system time
                if (saveFile.Save_Generation == SaveGeneration.N3DS)
                {
                    byte[] ClearedDate = new byte[4];
                    int DonationDateOffset = saveFile.Save_Data_Start_Offset + (saveFile.Save_Type == SaveType.New_Leaf ? 0x65948 : 0x6AEB8);
                    for (int i = DonationDateOffset; i < DonationDateOffset + 0x448; i += 4)
                    {
                        saveFile.Write(i, ClearedDate);
                    }
                }
            }
        }
    }
}
