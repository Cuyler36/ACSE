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
        private static readonly ShopOffsets AnimalCrossingShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x20466
        };

        private static readonly ShopOffsets WelcomeAmiiboShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x621E4,
            GardenShopUpgrade = 0x66674
        };

        private static readonly string[] NookShopNames = {
            "Nook's Cranny", "Nook 'n' Go", "Nookway", "Nookington's"
        };

        private static readonly string[] NewLeafNookShopNames = {
            "Nookling Junction", "T&T Mart", "Super T&T", "T.I.Y.", "T&T Emporium"
        };

        public static ShopOffsets GetShopOffsets(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.AnimalCrossing:
                    return AnimalCrossingShopOffsets;
                case SaveType.WelcomeAmiibo:
                    return WelcomeAmiiboShopOffsets;
                default:
                    return null;
            }
        }

        public static string GetShopName(SaveGeneration generation, byte shopSize)
        {
            if (generation == SaveGeneration.N3DS)
            {
                if (shopSize > 4)
                    shopSize = 4;
                return NewLeafNookShopNames[shopSize];
            }

            if (shopSize > 3)
                shopSize = 3;
            return NookShopNames[shopSize];
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
            ShopOffsets = ShopInfo.GetShopOffsets(SaveFile.SaveType);
        }

        public abstract void Write();
    }

    public class FurnitureShop : Shop
    {
        public uint PurchaseSum;
        public byte Size;

        public FurnitureShop(Save saveFile, int offset) : base(saveFile, offset)
        {
            Size = GetSize(saveFile.SaveGeneration);
            Name = ShopInfo.GetShopName(saveFile.SaveGeneration, Size);
            var itemCount = 0;

            switch (saveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    if (Size == 0)
                        itemCount = 0;
                    else if (Size == 1)
                        itemCount = 0;
                    else if (Size == 2)
                        itemCount = 0;
                    else
                        itemCount = 35;
                    break;
                case SaveGeneration.N3DS:
                    break;
            }

            var items = new Item[itemCount];
            for (var i = 0; i < itemCount; i++)
            {
                if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    items[i] = new Item(SaveFile.ReadUInt32(Offset + ShopOffsets.FurnitureShop + i * 4));
                }
                else
                {
                    items[i] = new Item(SaveFile.ReadUInt16(Offset + ShopOffsets.FurnitureShop + i * 2));
                }
            }

            Stock = items;
        }

        public byte GetSize(SaveGeneration generation)
        {
            var saveFile = MainForm.SaveFile;
            var shopOffsets = ShopInfo.GetShopOffsets(saveFile.SaveType);
            if (shopOffsets == null) return 0;
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return (byte)(saveFile.ReadByte(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade) >> 6);
                case SaveGeneration.N3DS:
                    return saveFile.ReadByte(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade);
                default:
                    return 0;
            }
        }

        public void SetSize(byte size)
        {
            var saveFile = MainForm.SaveFile;
            var shopOffsets = ShopInfo.GetShopOffsets(saveFile.SaveType);
            if (shopOffsets == null) return;
            switch (saveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    saveFile.Write(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade,
                        (byte)((saveFile.ReadByte(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade) & 0x3F) | ((size & 3) << 6)));
                    break;
                case SaveGeneration.N3DS:
                    saveFile.Write(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade, size);
                    break;
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
