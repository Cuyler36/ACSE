namespace ACSE
{
    class Museum
    {
        private static int GetMuseumFieldSize(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.New_Leaf:
                case SaveType.Welcome_Amiibo:
                    return 0x112;
                default:
                    return 0;
            }
        }

        private static int GetBaseOffset(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.New_Leaf:
                    return 0x658E0;
                case SaveType.Welcome_Amiibo:
                    return 0x6B300;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Fills the Museum's donations. The selected Player will be the one who donated them.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">The Player who it will show as the donor</param>
        public void FillMuseum(Save saveFile, NewPlayer player)
        {
            int MuseumDataOffset = GetBaseOffset(saveFile.Save_Type);
            if (MuseumDataOffset != -1)
            {
                int MuseumDataSize = GetMuseumFieldSize(saveFile.Save_Type);
                for (int i = MuseumDataOffset; i < MuseumDataOffset + MuseumDataSize; i++)
                {
                    saveFile.Write(i, (byte)(player.Index + 1));
                }

                // Set Date/Time for donation to current system time
                if (saveFile.Game_System == SaveGeneration.N3DS)
                {
                    byte[] NowDate = new Classes.Utilities.ACDate().ToYearMonthDayDateData();
                    int DonationDateOffset = saveFile.Save_Type == SaveType.New_Leaf ? 0x65948 : 0x6AEB8;
                    for (int i = DonationDateOffset; i < DonationDateOffset + 0x448; i++)
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
        public void ClearMuseum(Save saveFile)
        {
            int MuseumDataOffset = GetBaseOffset(saveFile.Save_Type);
            if (MuseumDataOffset != -1)
            {
                int MuseumDataSize = GetMuseumFieldSize(saveFile.Save_Type);
                for (int i = MuseumDataOffset; i < MuseumDataOffset + MuseumDataSize; i++)
                {
                    saveFile.Write(i, (byte)0);
                }

                // Clear Date/Time for donation to current system time
                if (saveFile.Game_System == SaveGeneration.N3DS)
                {
                    byte[] ClearedDate = new byte[4];
                    int DonationDateOffset = saveFile.Save_Type == SaveType.New_Leaf ? 0x65948 : 0x6AEB8;
                    for (int i = DonationDateOffset; i < DonationDateOffset + 0x448; i++)
                    {
                        saveFile.Write(i, ClearedDate);
                    }
                }
            }
        }
    }
}
