using ACSE.Core.Players;
using ACSE.Core.Saves;

namespace ACSE.Core.BitFields.Museum
{
    public static class Museum
    {
        private static int GetMuseumFieldSize(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                    return 0x3F;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
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
                case SaveType.AnimalForestEPlus:
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
            var museumDataOffset = saveFile.SaveDataStartOffset + GetBaseOffset(saveFile.SaveType);
            if (museumDataOffset == -1) return;
            var museumDataSize = GetMuseumFieldSize(saveFile.SaveType);
            var playerDonationIndex = saveFile.SaveGeneration == SaveGeneration.N3DS ? 
                (byte)(player.Index + 1) : (byte)(((player.Index + 1) << 4) | (byte)(player.Index + 1));

            for (var i = museumDataOffset; i < museumDataOffset + museumDataSize; i++)
            {
                saveFile.Write(i, playerDonationIndex);
            }

            // Set Date/Time for donation to current system time
            if (saveFile.SaveGeneration != SaveGeneration.N3DS) return;
            var nowDate = new Utilities.AcDate().ToYearMonthDayDateData();
            var donationDateOffset = saveFile.SaveDataStartOffset + (saveFile.SaveType == SaveType.NewLeaf ? 0x658C8 : 0x6AE38);
            for (var i = donationDateOffset; i < donationDateOffset + 0x448; i += 4)
            {
                saveFile.Write(i, nowDate);
            }
        }

        /// <summary>
        /// Clears the Museum's donations.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        public static void ClearMuseum(Save saveFile)
        {
            var museumDataOffset = saveFile.SaveDataStartOffset + GetBaseOffset(saveFile.SaveType);
            if (museumDataOffset == -1) return;
            var museumDataSize = GetMuseumFieldSize(saveFile.SaveType);
            for (var i = museumDataOffset; i < museumDataOffset + museumDataSize; i++)
            {
                saveFile.Write(i, (byte)0);
            }

            // Clear Date/Time for donation to current system time
            if (saveFile.SaveGeneration != SaveGeneration.N3DS) return;
            var clearedDate = new byte[4];
            var donationDateOffset = saveFile.SaveDataStartOffset + (saveFile.SaveType == SaveType.NewLeaf ? 0x65948 : 0x6AEB8);
            for (var i = donationDateOffset; i < donationDateOffset + 0x448; i += 4)
            {
                saveFile.Write(i, clearedDate);
            }
        }
    }
}
