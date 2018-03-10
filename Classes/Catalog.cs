using System.Collections.Generic;

namespace ACSE
{
    class Catalog
    {
        private static Dictionary<int, byte> Animal_Crossing_Catalog_Bitmap = new Dictionary<int, byte>
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

        private static int GetCatalogBaseOffset(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Animal_Crossing:
                    return 0x1108;
                default:
                    return -1;
            }
        }

        private static int GetCatalogSize(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Animal_Crossing:
                    return 0xD4;
                default:
                    return 0;
            }
        }

        private static Dictionary<int, byte> GetNonCatalogFields(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Animal_Crossing:
                    return Animal_Crossing_Catalog_Bitmap;
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
            int CatalogOffset = GetCatalogBaseOffset(saveFile.Save_Type);
            if (CatalogOffset > -1)
            {
                int OriginalOffset = CatalogOffset;
                CatalogOffset += player.Offset;
                int CatalogSize = GetCatalogSize(saveFile.Save_Type);
                var NonCatalogBitmapFields = GetNonCatalogFields(saveFile.Save_Type);

                for (int i = 0; i < CatalogSize; i++)
                {
                    if (NonCatalogBitmapFields != null && NonCatalogBitmapFields.ContainsKey(OriginalOffset + i))
                    {
                        saveFile.Write(CatalogOffset + i, (byte)(saveFile.ReadByte(CatalogOffset + i) | NonCatalogBitmapFields[OriginalOffset + i]));
                    }
                    else
                    {
                        saveFile.Write(CatalogOffset + i, (byte)0xFF);
                    }
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
            int CatalogOffset = GetCatalogBaseOffset(saveFile.Save_Type);
            if (CatalogOffset > -1)
            {
                int OriginalOffset = CatalogOffset;
                CatalogOffset += player.Offset;
                int CatalogSize = GetCatalogSize(saveFile.Save_Type);
                var NonCatalogBitmapFields = GetNonCatalogFields(saveFile.Save_Type);

                for (int i = 0; i < CatalogSize; i++)
                {
                    if (NonCatalogBitmapFields != null && NonCatalogBitmapFields.ContainsKey(OriginalOffset + i))
                    {
                        saveFile.Write(CatalogOffset + i, (byte)(saveFile.ReadByte(CatalogOffset + i) & ~NonCatalogBitmapFields[OriginalOffset + i]));
                    }
                    else
                    {
                        saveFile.Write(CatalogOffset + i, (byte)0x00);
                    }
                }
            }
        }
    }
}
