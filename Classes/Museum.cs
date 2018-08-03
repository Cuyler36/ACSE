namespace ACSE
{
    class Museum
    {
        private static int GetMuseumFieldSize(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                    return 0x3F;
                case SaveType.DoubutsuNoMoriEPlus:
                    return 0x47;
                case SaveType.WildWorld:
                    return 0x5F;
                case SaveType.CityFolk:
                    return 0x6C;
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
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
                case SaveType.DoubutsuNoMoriPlus:
                    return 0x1A298; // Verify this.
                case SaveType.AnimalCrossing:
                    return 0x213A8;
                case SaveType.DoubutsuNoMoriEPlus:
                    return 0x22FB0;
                case SaveType.WildWorld:
                    return 0x15D50;
                case SaveType.CityFolk:
                    return 0x7352A;
                case SaveType.NewLeaf:
                    return 0x65860;
                case SaveType.WelcomeAmiibo:
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
            int MuseumDataOffset = saveFile.SaveDataStartOffset + GetBaseOffset(saveFile.SaveType);
            if (MuseumDataOffset != -1)
            {
                int MuseumDataSize = GetMuseumFieldSize(saveFile.SaveType);
                byte PlayerDonationIndex = saveFile.SaveGeneration == SaveGeneration.N3DS ? 
                    (byte)(player.Index + 1) : (byte)(((player.Index + 1) << 4) | (byte)(player.Index + 1));

                for (int i = MuseumDataOffset; i < MuseumDataOffset + MuseumDataSize; i++)
                {
                    saveFile.Write(i, PlayerDonationIndex);
                }

                // Set Date/Time for donation to current system time
                if (saveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    byte[] NowDate = new Classes.Utilities.ACDate().ToYearMonthDayDateData();
                    int DonationDateOffset = saveFile.SaveDataStartOffset + (saveFile.SaveType == SaveType.NewLeaf ? 0x658C8 : 0x6AE38);
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
            int MuseumDataOffset = saveFile.SaveDataStartOffset + GetBaseOffset(saveFile.SaveType);
            if (MuseumDataOffset != -1)
            {
                int MuseumDataSize = GetMuseumFieldSize(saveFile.SaveType);
                for (int i = MuseumDataOffset; i < MuseumDataOffset + MuseumDataSize; i++)
                {
                    saveFile.Write(i, (byte)0);
                }

                // Clear Date/Time for donation to current system time
                if (saveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    byte[] ClearedDate = new byte[4];
                    int DonationDateOffset = saveFile.SaveDataStartOffset + (saveFile.SaveType == SaveType.NewLeaf ? 0x65948 : 0x6AEB8);
                    for (int i = DonationDateOffset; i < DonationDateOffset + 0x448; i += 4)
                    {
                        saveFile.Write(i, ClearedDate);
                    }
                }
            }
        }
    }
}
