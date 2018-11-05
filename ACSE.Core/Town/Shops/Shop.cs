using System;
using ACSE.Core.Items;
using ACSE.Core.Patterns;
using ACSE.Core.Saves;

namespace ACSE.Core.Town.Shops
{
    public enum ShopType
    {
        FurnitureShop,
        AbleSisters,
        LostAndFound,
        BlackMarket,
        GardenShop,
        ShoeShop,
        GracieGrace,
        ReTail,
        MuseumShop,
    }

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

        public int FurnitureShopBells = -1;
        public int FurnitureVisitorShopBells = -1;
    }

    public static class ShopInfo
    {
        private static readonly ShopOffsets AnimalCrossingShopOffsets = new ShopOffsets
        {
            FurnitureShopUpgrade = 0x20466
        };

        private static readonly ShopOffsets AnimalForestEPlusShopOffsets = new ShopOffsets
        {
            FurnitureShop = 0x22302,
            FurnitureShopSize = 0x223A8,
            FurnitureShopBells = 0x223AC,
            FurnitureVisitorShopBells = 0x223C0,
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
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return AnimalForestEPlusShopOffsets;
                case SaveType.WelcomeAmiibo:
                    return WelcomeAmiiboShopOffsets;
                default:
                    return null;
            }
        }

        public static string GetShopName(SaveGeneration generation, ShopType shopType, byte shopSize = 0)
        {
            switch (shopType)
            {
                case ShopType.FurnitureShop:
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

            throw new NotImplementedException("Shop type is unimplemented!");
        }
    }

    public abstract class Shop
    {
        public string Name;
        public Item[] Stock;

        public uint BellsSum
        {
            get => SaveFile.ReadUInt32(BellsSumOffset, SaveFile.IsBigEndian, true);
            set => SaveFile.Write(BellsSumOffset, value, SaveFile.IsBigEndian, true);
        }

        protected Save SaveFile;
        protected int Offset;
        protected ShopOffsets ShopOffsets;

        protected int BellsSumOffset = -1;

        protected Shop(Save saveFile, int offset)
        {
            SaveFile = saveFile;
            Offset = offset;
            ShopOffsets = ShopInfo.GetShopOffsets(SaveFile.SaveType);
        }

        public abstract void Write();
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
