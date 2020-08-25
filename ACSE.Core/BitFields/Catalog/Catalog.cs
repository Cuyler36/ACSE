using System.Collections.Generic;
using ACSE.Core.Players;
using ACSE.Core.Saves;

namespace ACSE.Core.BitFields.Catalog
{
    public static class Catalog
    {
        private static readonly Dictionary<int, byte> AnimalCrossingCatalogBitmap = new Dictionary<int, byte>
        {
            { 0x1164, 0x00 },
            { 0x1165, 0x03 },
            { 0x1168, 0x00 },
            { 0x1169, 0x00 },
            { 0x116A, 0x00 },
            { 0x116B, 0x00 },
            { 0x116C, 0x00 },
            { 0x116D, 0x00 },
            { 0x116E, 0x00 },
            { 0x116F, 0x00 },
            { 0x1173, 0xFC }
        };

        public static readonly Dictionary<int, byte> WildWorldCatalogBitmap = new Dictionary<int, byte>
        {
            { 0x1BDD, 0x01 },
            { 0x1BDE, 0x00 },
            { 0x1BDF, 0x00 },
            { 0x1BE0, 0x00 },
            { 0x1BE1, 0x00 },
            { 0x1BE2, 0x00 },
            { 0x1BE3, 0x00 },
            { 0x1BE4, 0x00 },
            { 0x1BE5, 0x00 },
            { 0x1BE6, 0x00 },
            { 0x1BE7, 0x00 },
            { 0x1BE8, 0x00 },
            { 0x1BE9, 0x00 },
            { 0x1BEA, 0x00 },
            { 0x1BEB, 0xFE }
        };

        private static int GetCatalogBaseOffset(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    return 0xAF0;
                case SaveType.AnimalCrossing:
                    return 0x1108;
                case SaveType.WildWorld:
                    return 0x1B48;
                case SaveType.NewLeaf:
                    return 0x6C70;
                case SaveType.WelcomeAmiibo:
                    return 0x6C90;
                default:
                    return -1;
            }
        }

        private static int GetCatalogSize(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    return 0x98;
                case SaveType.AnimalCrossing:
                    return 0xD4;
                case SaveType.WildWorld:
                    return 0x124; // Needs verification.
                case SaveType.NewLeaf:
                    return 0xE0;
                case SaveType.WelcomeAmiibo:
                    return 0x1A8;
                default:
                    return 0;
            }
        }

        private static Dictionary<int, byte> GetNonCatalogFields(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.AnimalCrossing:
                    return AnimalCrossingCatalogBitmap;
                case SaveType.WildWorld:
                    return WildWorldCatalogBitmap;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Fills a Player's catalog
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">The Player whose catalog will be filled</param>
        public static void FillCatalog(Save saveFile, Player player)
        {
            var catalogOffset = GetCatalogBaseOffset(saveFile.SaveType);
            if (catalogOffset <= -1) return;
            var originalOffset = catalogOffset;
            catalogOffset += player.Offset;
            var catalogSize = GetCatalogSize(saveFile.SaveType);
            var nonCatalogBitmapFields = GetNonCatalogFields(saveFile.SaveType);

            for (var i = 0; i < catalogSize; i++)
            {
                if (nonCatalogBitmapFields != null && nonCatalogBitmapFields.ContainsKey(originalOffset + i))
                {
                    saveFile.Write(catalogOffset + i, (byte)(saveFile.ReadByte(catalogOffset + i) | nonCatalogBitmapFields[originalOffset + i]));
                }
                else
                {
                    saveFile.Write(catalogOffset + i, (byte)0xFF);
                }
            }
        }

        /// <summary>
        /// Clears a Player's catalog
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">The Player whose catalog will be cleared</param>
        public static void ClearCatalog(Save saveFile, Player player)
        {
            var catalogOffset = GetCatalogBaseOffset(saveFile.SaveType);
            if (catalogOffset <= -1) return;
            var originalOffset = catalogOffset;
            catalogOffset += player.Offset;
            var catalogSize = GetCatalogSize(saveFile.SaveType);
            var nonCatalogBitmapFields = GetNonCatalogFields(saveFile.SaveType);

            for (var i = 0; i < catalogSize; i++)
            {
                if (nonCatalogBitmapFields != null && nonCatalogBitmapFields.ContainsKey(originalOffset + i))
                {
                    saveFile.Write(catalogOffset + i, (byte)(saveFile.ReadByte(catalogOffset + i) & ~nonCatalogBitmapFields[originalOffset + i]));
                }
                else
                {
                    saveFile.Write(catalogOffset + i, (byte)0x00);
                }
            }
        }
    }
}
