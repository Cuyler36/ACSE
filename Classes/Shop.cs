namespace ACSE
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

    public static class ShopInfo
    {
        private static readonly ShopOffsets AnimalCrossing_ShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x20466
        };

        private static readonly ShopOffsets WelcomeAmiibo_ShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x621E4,
            GardenShopUpgrade = 0x66674
        };

        private static readonly string[] NookShopNames = new string[4]
        {
            "Nook's Cranny", "Nook 'n' Go", "Nookway", "Nookington's"
        };

        private static readonly string[] NewLeaf_NookShopNames = new string[5]
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

    public abstract class Shop
    {
        public Item[] Stock;
        public string Name;

        protected Save SaveFile;
        protected int Offset;
        protected ShopOffsets ShopOffsets;

        public Shop(Save saveFile, int offset)
        {
            SaveFile = saveFile;
            Offset = offset;
            ShopOffsets = ShopInfo.GetShopOffsets(SaveFile.Save_Type);
        }

        public abstract void Write();
    }

    public class FurnitureShop : Shop
    {
        public uint PurchaseSum;
        public byte Size;

        public FurnitureShop(Save saveFile, int offset) : base(saveFile, offset)
        {
            Item[] Items = null;

            Size = GetSize(saveFile.Save_Generation);
            Name = ShopInfo.GetShopName(saveFile.Save_Generation, Size);
            int ItemCount = 0;

            switch (saveFile.Save_Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
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

            Items = new Item[ItemCount];
            for (int i = 0; i < ItemCount; i++)
            {
                if (SaveFile.Save_Generation == SaveGeneration.N3DS)
                {
                    Items[i] = new Item(SaveFile.ReadUInt32(Offset + ShopOffsets.FurnitureShop + i * 4));
                }
                else
                {
                    Items[i] = new Item(SaveFile.ReadUInt16(Offset + ShopOffsets.FurnitureShop + i * 2));
                }
            }

            Stock = Items;
        }

        public byte GetSize(SaveGeneration Generation)
        {
            var SaveFile = MainForm.Save_File;
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
            var SaveFile = MainForm.Save_File;
            var ShopOffsets = ShopInfo.GetShopOffsets(SaveFile.Save_Type);
            if (ShopOffsets != null)
            {
                switch (SaveFile.Save_Generation)
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

        public override void Write()
        {
            throw new System.NotImplementedException();
        }
    }

    public class TailorShop : Shop
    {
        public Pattern[] DisplayedPatterns;

        public TailorShop(Save saveFile, int offset) : base(saveFile, offset)
        {

        }

        public override void Write()
        {
            throw new System.NotImplementedException();
        }
    }
}
