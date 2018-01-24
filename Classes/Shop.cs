namespace ACSE
{
    public static class ShopInfo
    {
        public class ShopOffsets
        {
            public int FurnitureShop = -1;
            public int FurnitureShopUpgrade = -1;
            public int TailorShop = -1;
            public int BlackMarket = -1;
            public int LostAndFound = -1;
            public int RecycleShop = -1;
            public int AccessoriesShop = -1;
            public int GardenShop = -1;
            public int GardenShopUpgrade = -1;
            public int ShoeShop = -1;
            public int MuseumShop = -1;
            public int HomeCustomizationShop = -1;
            public int GiraffeShop = -1;
            public int ClubShop = -1;
            public int IslandShop = -1;

            public int FurnitureShopSize = 0;
            public int GardenShopSize = 0;
            public int TailorShopSize = 0;
            public int BlackMarketSize = 0;
            public int LostAndFoundSize = 0;
        }

        private static ShopOffsets AnimalCrossing_ShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x20466
        };

        private static ShopOffsets WelcomeAmiibo_ShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x621E4,
            GardenShopUpgrade = 0x66674
        };

        private static string[] NookShopNames = new string[4]
        {
            "Nook's Cranny", "Nook 'n' Go", "Nookway", "Nookington's"
        };

        private static string[] NewLeaf_NookShopNames = new string[5]
        {
            "Nookling Junction", "T&T Mart", "Super T&T", "T.I.Y.", "T&T Emporium"
        };

        public static ShopOffsets GetShopOffsets(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return AnimalCrossing_ShopOffsets;
                case SaveType.Welcome_Amiibo:
                    return WelcomeAmiibo_ShopOffsets;
                default:
                    return null;
            }
        }

        public static string GetShopName(SaveGeneration Generation, byte ShopSize)
        {
            if (Generation == SaveGeneration.N3DS)
            {
                if (ShopSize > 4)
                    ShopSize = 4;
                return NewLeaf_NookShopNames[ShopSize];
            }
            else
            {
                if (ShopSize > 3)
                    ShopSize = 3;
                return NookShopNames[ShopSize];
            }
        }
    }

    public class Shop
    {
        public Item[] Stock;
        public string Name;
    }

    public class FurnitureShop : Shop
    {
        public byte Size;

        public FurnitureShop(SaveGeneration Generation)
        {
            Item[] Items = null;
            var SaveFile = NewMainForm.Save_File;

            Size = GetSize(Generation);

            switch (Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    int ItemCount = 0;
                    if (Size == 0)
                        ItemCount = 0;
                    else if (Size == 1)
                        ItemCount = 0;
                    else if (Size == 2)
                        ItemCount = 0;
                    else
                        ItemCount = 35;
                    break;
                case SaveGeneration.N3DS:
                    
                    break;
            }

            Stock = Items;
        }

        public byte GetSize(SaveGeneration Generation)
        {
            var SaveFile = NewMainForm.Save_File;
            var ShopOffsets = ShopInfo.GetShopOffsets(SaveFile.Save_Type);
            if (ShopOffsets != null)
            {
                switch (Generation)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                        return (byte)(SaveFile.ReadByte(SaveFile.Save_Data_Start_Offset + ShopOffsets.FurnitureShopUpgrade) >> 6);
                    case SaveGeneration.N3DS:
                        return SaveFile.ReadByte(SaveFile.Save_Data_Start_Offset + ShopOffsets.FurnitureShopUpgrade);
                    default:
                        return 0;
                }
            }
            return 0;
        }

        public void SetSize(byte Size)
        {
            var SaveFile = NewMainForm.Save_File;
            var ShopOffsets = ShopInfo.GetShopOffsets(SaveFile.Save_Type);
            if (ShopOffsets != null)
            {
                switch (SaveFile.Game_System)
                {
                    case SaveGeneration.N64:
                    case SaveGeneration.GCN:
                        SaveFile.Write(SaveFile.Save_Data_Start_Offset + ShopOffsets.FurnitureShopUpgrade,
                            (byte)((SaveFile.ReadByte(SaveFile.Save_Data_Start_Offset + ShopOffsets.FurnitureShopUpgrade) & 0x3F) | ((Size & 3) << 6)));
                        break;
                    case SaveGeneration.N3DS:
                        SaveFile.Write(SaveFile.Save_Data_Start_Offset + ShopOffsets.FurnitureShopUpgrade, Size);
                        break;
                }
            }
        }
    }
}
