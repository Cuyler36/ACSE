using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ACSE.Core.Debug;
using ACSE.Core.Saves;
using ACSE.Core.Town.Buildings;
using ACSE.Core.Utilities;

namespace ACSE.Core.Items
{
    public enum ItemType
    {
        Empty = 0,
        Furniture = 1,
        Gyroid = 2,
        Diary = 3,
        Clothes = 4,
        Song = 5,
        Paper = 6,
        Trash = 7,
        Shell = 8,
        Fruit = 9,
        Turnip = 0xA,
        Catchable = 0xB,
        QuestItem = 0xC,
        Item = 0xD,
        RaffleTicket = 0xE,
        WallpaperCarpet = 0xF,
        Fossil = 0x10,
        Tool = 0x11,
        Tree = 0x12,
        Weed = 0x13,
        Flower = 0x14,
        Rock = 0x15,
        MoneyRock = 0x16,
        Signboard = 0x17,
        Money = 0x18,
        HouseObject = 0x19,
        Building = 0x1A,
        ParchedFlower = 0x1B,
        WateredFlower = 0x1C,
        Pattern = 0x1D,
        WiltedFlower = 0x1E,
        Occupied = 0x1F,

        Invalid = -1
    }
    
    public class ItemData
    {
        /// <summary>
        /// A dictionary containing string-uint pairs for item colors.
        /// </summary>
        public static Dictionary<string, uint> ItemColorsSettings;

        public static List<KeyValuePair<ushort, string>> ItemDatabase = new List<KeyValuePair<ushort, string>>();

        public static readonly string[] ItemTypeNames =
        {
            "Empty",
            "Furniture",
            "Gyroid",
            "Diary",
            "Clothes",
            "Song",
            "Paper",
            "Trash",
            "Shell",
            "Fruit",
            "Turnip",
            "Catchable",
            "Quest Item",
            "Item",
            "Raffle Ticket",
            "Wallpaper & Carpet",
            "Fossil",
            "Tool",
            "Tree",
            "Weed",
            "Flower",
            "Rock",
            "Money Rock",
            "Signboard",
            "Money",
            "House Object",
            "Building",
            "Parched Flower",
            "Watered Flower",
            "Pattern",
            "Wilted Flower",
            "Occupied",
            "Invalid"
        };

        public static int GetBuildingCount(in Building[] buildings, SaveType saveType)
        {
            var noBuildingId = saveType == SaveType.NewLeaf ? 0xF8 : 0xFC;
            return buildings.Count(b => b.Exists && b.Id != noBuildingId);
        }

        public static ItemType GetItemType(ushort id, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriPlus:
                
                // TODO: DnM+, and DnMe+ need their own cases
                case SaveType.DongwuSenlin:
                    if (id == 0)
                        return ItemType.Empty;
                    else if (id == 0xFFFF)
                        return ItemType.Occupied;
                    else if (id >= 0x8 && id <= 0xA)
                        return ItemType.Weed;
                    else if ((id >= 0x83C && id <= 0x84D) || (id >= 0xBF && id <= 0xD0))
                        return ItemType.Flower;
                    else if ((id >= 0x2100 && id <= 0x2103) || id == 0x005C) //0x005C = Glowing 0x18/Shovel spot
                        return ItemType.Money;
                    else if (id >= 0x63 && id <= 0x68)
                        return ItemType.Rock;
                    else if (id >= 0x6A && id <= 0x6F)
                        return ItemType.MoneyRock;
                    else if (id >= 0x900 && id <= 0x920)
                        return ItemType.Signboard;
                    else if (id >= 0x2514 && id <= 0x251B)
                        return ItemType.Shell;
                    else if (id >= 0x2A00 && id <= 0x2A36)
                        return ItemType.Song;
                    else if (id >= 0x2B00 && id <= 0x2B0F)
                        return ItemType.Diary;
                    else if (id >= 0x2000 && id <= 0x20FF)
                        return ItemType.Paper;
                    else if (id >= 0x2F00 && id <= 0x2F03)
                        return ItemType.Turnip;
                    else if ((id >= 0x2800 && id <= 0x2804) || id == 0x2807)
                        return ItemType.Fruit;
                    else if ((id >= 0x2300 && id <= 0x2327) || (id >= 0x2D00 && id <= 0x2D27))
                        return ItemType.Catchable;
                    else if (id >= 0x2503 && id <= 0x250C)
                        return ItemType.QuestItem;
                    else if (id >= 0x250E && id <= 0x2510)
                        return ItemType.Trash;
                    else if ((id >= 0x2523 && id <= 0x2530) || (id >= 0x2900 && id <= 0x290A) || (id >= 0x2805 && id <= 0x2806) || (id >= 0x2E00 && id <= 0x2E01))
                        return ItemType.Item;
                    else if (id >= 0x2C00 && id <= 0x2C5F)
                        return ItemType.RaffleTicket;
                    else if ((id >= 0x2600 && id <= 0x2642) || (id >= 0x2700 && id <= 0x2742))
                        return ItemType.WallpaperCarpet;
                    else if (id >= 0x2400 && id <= 0x24FF)
                        return ItemType.Clothes;
                    else if (id >= 0x15B0 && id <= 0x17A8)
                        return ItemType.Gyroid;
                    else if (id == 0x2511)
                        return ItemType.Fossil;
                    else if ((id >= 0x2200 && id <= 0x225B) || id == 0x251E)
                        return ItemType.Tool;  //0x251E = 0x17 (not a 'tool', but it's still classified as one)
                    else if ((id >= 0x1 && id <= 0x4) || (id >= 0x005E && id <= 0x0060) || id == 0x69 || (id >= 0x0070 && id <= 0x00BE) ||
                             (id >= 0xD1 && id <= 0xEC) || (id >= 0x0800 && id <= 0x0869))
                        return ItemType.Tree;
                    else if (id >= 0x4000 && id < 0x5000)
                        return ItemType.HouseObject;
                    else if ((id >= 0x5 && id <= 0x7) || (id >= 0xB && id <= 0x10) || (id >= 0x5000 && id <= 0xB000) || id >= 0xFE00)
                        return ItemType.Building;
                    else if ((id >= 0x1000 && id <= 0x15AC) || (id >= 0x17AC && id <= 0x1FFC) || (id >= 0x3000 && id <= 0x33C4)) // || ID >= 0xFE20
                        return ItemType.Furniture;
                    else
                        return ItemType.Invalid;

                // Doubutsu no Mori e+ / Animal Forest e+
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    if (id == 0x0000)
                        return ItemType.Empty;
                    else if (id == 0xFFFF)
                        return ItemType.Occupied;
                    else if (id >= 0x0008 && id <= 0x000A)
                        return ItemType.Weed;
                    else if (id == 0x0092 || (id >= 0x00CF && id <= 0x00E0) || (id >= 0x0845 && id <= 0x084D))
                        return ItemType.Flower;
                    else if ((id >= 0x2100 && id <= 0x2103) || id == 0x005C) //0x005C = Glowing 0x18/Shovel spot
                        return ItemType.Money;
                    else if (id >= 0x0063 && id <= 0x0068)
                        return ItemType.Rock;
                    else if ((id >= 0x006A && id <= 0x006F) || (id >= 0x0083 && id <= 0x008C))
                        return ItemType.MoneyRock;
                    else if (id >= 0x0900 && id <= 0x0920)
                        return ItemType.Signboard;
                    else if (id >= 0x2514 && id <= 0x251B)
                        return ItemType.Shell;
                    else if (id >= 0x2A00 && id <= 0x2A8B)
                        return ItemType.Song;
                    else if (id >= 0x2B00 && id <= 0x2B0F)
                        return ItemType.Diary;
                    else if (id >= 0x2000 && id <= 0x20FF)
                        return ItemType.Paper;
                    else if (id >= 0x2F00 && id <= 0x2F03)
                        return ItemType.Turnip;
                    else if ((id >= 0x2800 && id <= 0x2804) || id == 0x2807)
                        return ItemType.Fruit;
                    else if ((id >= 0x2300 && id <= 0x232F) || (id >= 0x2D00 && id <= 0x2D2F))
                        return ItemType.Catchable;
                    else if (id >= 0x2503 && id <= 0x250C)
                        return ItemType.QuestItem;
                    else if (id >= 0x250E && id <= 0x2510)
                        return ItemType.Trash;
                    else if ((id >= 0x2523 && id <= 0x2530) || (id >= 0x2900 && id <= 0x290A) ||
                             (id >= 0x2805 && id <= 0x2806) || (id >= 0x2E00 && id <= 0x2E01) ||
                             (id >= 0x2E00 && id <= 0x2E01))
                        return ItemType.Item;
                    else if (id >= 0x2C00 && id <= 0x2C5F)
                        return ItemType.RaffleTicket;
                    else if ((id >= 0x2600 && id <= 0x2644) || (id >= 0x2700 && id <= 0x2744))
                        return ItemType.WallpaperCarpet;
                    else if (id >= 0x2400 && id <= 0x24FF)
                        return ItemType.Clothes;
                    else if (id >= 0x15B0 && id <= 0x17A8)
                        return ItemType.Gyroid;
                    else if (id == 0x2511)
                        return ItemType.Fossil;
                    else if ((id >= 0x2200 && id <= 0x2266) || id == 0x251E)
                        return ItemType.Tool;  //0x251E = 0x17 (not a 'tool', but it's still classified as one)
                    else if ((id >= 0x0001 && id <= 0x0004) || (id >= 0x005E && id <= 0x0060) || id == 0x0069 ||
                             (id >= 0x0070 && id <= 0x0082) || (id >= 0x0093 && id <= 0x00CE) ||
                             (id >= 0x00E1 && id <= 0x00FC) || (id >= 0x0800 && id <= 0x0869))
                        return ItemType.Tree;
                    else if (id >= 0x4000 && id < 0x5000)
                        return ItemType.HouseObject;
                    else if ((id >= 0x0005 && id <= 0x0007) || (id >= 0x000B && id <= 0x0010) ||
                             (id >= 0x008D && id <= 0x0091) || (id >= 0x5000 && id <= 0xB000) ||
                             (id >= 0xFE00 && id != 0xFE23))
                        return ItemType.Building;
                    else if ((id >= 0x1000 && id <= 0x15AC) || (id >= 0x17AC && id <= 0x1FFC) ||
                             (id >= 0x3000 && id <= 0x345C)) // || ID >= 0xFE20
                        return ItemType.Furniture;
                    else
                        return ItemType.Invalid;

                // Wild World

                case SaveType.WildWorld when id == 0xFFF1:
                    return 0;
                case SaveType.WildWorld when id == 0xF030:
                    return ItemType.Occupied;
                case SaveType.WildWorld when id <= 0x1C:
                    return ItemType.Flower;
                case SaveType.WildWorld when id >= 0x1D && id <= 0x24:
                    return ItemType.Weed;
                case SaveType.WildWorld when (id >= 0x25 && id <= 0x6D) || (id >= 0xC7 && id <= 0xD3):
                    return ItemType.Tree;
                case SaveType.WildWorld when id >= 0x6E && id <= 0x89:
                    return ItemType.ParchedFlower;
                case SaveType.WildWorld when id >= 0x8A && id <= 0xA5:
                    return ItemType.WateredFlower;
                case SaveType.WildWorld when id >= 0xA7 && id <= 0xC6:
                    return ItemType.Pattern;
                case SaveType.WildWorld when (id >= 0xE3 && id <= 0xE7) || (id >= 0xED && id <= 0xFB):
                    return ItemType.Rock;
                case SaveType.WildWorld when id >= 0xE8 && id <= 0xEC:
                    return ItemType.MoneyRock;
                case SaveType.WildWorld when id >= 0x1000 && id <= 0x10FF:
                    return ItemType.Paper;
                //Wallpaper
                case SaveType.WildWorld when id >= 0x1100 && id <= 0x1143:
                //Carpet
                case SaveType.WildWorld when id >= 0x1144 && id <= 0x1187:
                    return ItemType.WallpaperCarpet;
                case SaveType.WildWorld when id >= 0x11A8 && id <= 0x12AF:
                    return ItemType.Clothes; //Shirts
                case SaveType.WildWorld when id >= 0x12B0 && id <= 0x131F:
                    return ItemType.Catchable;
                case SaveType.WildWorld when id >= 0x1323 && id <= 0x1368:
                    return ItemType.Song;
                case SaveType.WildWorld when id >= 0x1369 && id <= 0x139F:
                    return ItemType.Tool;
                case SaveType.WildWorld when id >= 0x13A8 && id <= 0x1457:
                    return ItemType.Clothes; //Hats, 0x14s, & Glasses/Masks
                case SaveType.WildWorld when id >= 0x1492 && id <= 0x14FD:
                    return ItemType.Money;
                case SaveType.WildWorld when (id >= 0x14FE && id <= 0x1530) || (id >= 0x1542 && id <= 0x1548) || (id >= 0x155E && id <= 0x156D):
                    return ItemType.Item;
                case SaveType.WildWorld when (id >= 0x1531 && id <= 0x1541):
                    return ItemType.Turnip;
                case SaveType.WildWorld when id == 0x1549:
                    return ItemType.Fossil;
                case SaveType.WildWorld when id >= 0x1554 && id <= 0x155C:
                    return ItemType.Shell;
                case SaveType.WildWorld when (id >= 0x3000 && id <= 0x45D8) || (id >= 0x47D8 && id <= 0x4BA0):
                    return ItemType.Furniture;
                case SaveType.WildWorld when id >= 0x45DC && id <= 0x47D4:
                    return ItemType.Gyroid;
                // F030-1 = Multispace furniture item
                case SaveType.WildWorld when (id >= 0x5000 && id <= 0x5021) || id == 0xF030 || id == 0xF031:
                    return ItemType.Building;
                case SaveType.WildWorld:
                    return ItemType.Invalid;

                // City Folk

                case SaveType.CityFolk when id == 0xFFF1:
                    return 0;
                case SaveType.CityFolk when (id >= 0xB710 && id <= 0xCE50) || (id >= 0x93F0 && id <= 0x9414) || (id >= 0x9CC0 && id < 0x9EC0):
                    return ItemType.Furniture;
                case SaveType.CityFolk when id >= 0xA518 && id <= 0xAA7C:
                    return ItemType.Clothes;
                case SaveType.CityFolk when (id >= 0x93E8 && id <= 0x93EC) || (id >= 0x9FA0 && id <= 0xA420):
                    return ItemType.WallpaperCarpet;
                case SaveType.CityFolk when id >= 0x94B0 && id <= 0x95D8:
                    return ItemType.Song;
                case SaveType.CityFolk when id >= 0x9640 && id <= 0x974F:
                    return ItemType.Paper;
                case SaveType.CityFolk when id >= 0x97D0 && id <= 0x9814:
                    return ItemType.Turnip;
                //Actually separated a little bit
                case SaveType.CityFolk when id >= 0x9960 && id <= 0x9BEC:
                    return ItemType.Catchable;
                case SaveType.CityFolk when id >= 0xAC20 && id <= 0xB2E4:
                    return ItemType.Clothes; //Hats, Masks/Glasses & 0x14s
                case SaveType.CityFolk when id >= 0xB3F0 && id <= 0xB5E8:
                    return ItemType.Gyroid;
                case SaveType.CityFolk when (id >= 0xCE80 && id <= 0xCF54) || (id >= 0xAA90 && id <= 0xAB14):
                    return ItemType.Tool;
                case SaveType.CityFolk when id == 0x9018:
                    return ItemType.Fossil;
                case SaveType.CityFolk when id >= 0x0001 && id <= 0x0056:
                    return ItemType.Tree;
                case SaveType.CityFolk when (id >= 0x0057 && id <= 0x005A) || (id >= 0x00DD && id <= 0x00E1):
                    return ItemType.Weed;
                case SaveType.CityFolk when (id >= 0x005B && id <= 0x005F) || (id >= 0x0065 && id <= 0x0073):
                    return ItemType.Rock;
                case SaveType.CityFolk when id >= 0x0060 && id <= 0x0064:
                    return ItemType.MoneyRock;
                case SaveType.CityFolk when id >= 0x0074 && id <= 0x0093:
                    return ItemType.Pattern;
                case SaveType.CityFolk when id >= 0x009E && id <= 0x00BD:
                    return ItemType.Flower;
                case SaveType.CityFolk when id >= 0x00BE && id <= 0x00DD:
                    return ItemType.ParchedFlower;
                case SaveType.CityFolk when id >= 0x9118 && id <= 0x9138:
                    return ItemType.Shell;
                case SaveType.CityFolk when id >= 0x9194 && id <= 0x9340:
                    return ItemType.Money;
                case SaveType.CityFolk when id >= 0x9348 && id <= 0x93D0:
                    return ItemType.QuestItem;
                case SaveType.CityFolk when (id >= 0x901C && id <= 0x9108) || (id >= 0x093D4 && id <= 0x93D8) || (id >= 0x9140 && id <= 0x9150) || (id >= 0x915C && id <= 0x9164):
                    return ItemType.Item;
                case SaveType.CityFolk when (id >= 0x9000 && id <= 0x9014) || (id >= 0x9168 && id <= 0x9178):
                    return ItemType.Fruit;
                case SaveType.CityFolk when id == 0xD000 || id == 0x7003 || id == 0xF030:
                    return ItemType.Building;
                case SaveType.CityFolk:
                    return ItemType.Invalid;

                // New Leaf
                
                case SaveType.NewLeaf when id == 0x7FFE:
                    return 0;
                case SaveType.NewLeaf when id == 0x009D:
                    return ItemType.Pattern;
                //C9 & CA = weed flowers
                case SaveType.NewLeaf when id >= 0x009F && id <= 0x00C8:
                    return ItemType.Flower;
                case SaveType.NewLeaf when id >= 0x00CE && id <= 0x00F7:
                    return ItemType.WiltedFlower;
                case SaveType.NewLeaf when (id >= 0x0005 && id <= 0x007B) || (id >= 0x0080 && id <= 0x0097):
                    return ItemType.Tree;
                case SaveType.NewLeaf when id >= 0x0098 && id <= 0x009C:
                    return ItemType.Rock;
                case SaveType.NewLeaf when (id >= 0x007C && id <= 0x007F) || (id >= 0x00C9 && id <= 0x00CD) || id == 0x00F8:
                    return ItemType.Weed;
                case SaveType.NewLeaf when id >= 0x28B2 && id <= 0x2934:
                    return ItemType.Gyroid;
                case SaveType.NewLeaf when id == 0x202A:
                    return ItemType.Fossil;
                case SaveType.NewLeaf when id >= 0x2087 && id <= 0x2090:
                    return ItemType.Shell;
                case SaveType.NewLeaf when id >= 0x2126 && id <= 0x2239:
                    return ItemType.Song;
                case SaveType.NewLeaf when id >= 0x223A && id <= 0x227A:
                    return ItemType.Paper;
                case SaveType.NewLeaf when id >= 0x227B && id <= 0x2285:
                    return ItemType.Turnip;
                case SaveType.NewLeaf when id >= 0x2286 && id <= 0x2341:
                    return ItemType.Catchable;
                case SaveType.NewLeaf when (id >= 0x2342 && id <= 0x2445) || (id >= 0x2119 && id <= 0x211A):
                    return ItemType.WallpaperCarpet;
                case SaveType.NewLeaf when id >= 0x2446 && id <= 0x28B1:
                    return ItemType.Clothes;
                case SaveType.NewLeaf when id >= 0x303B && id <= 0x307A:
                    return ItemType.Tool;
                case SaveType.NewLeaf when id >= 0x20A7 && id <= 0x2112:
                    return ItemType.Money;
                case SaveType.NewLeaf when id >= 0x209A && id <= 0x209B:
                    return ItemType.Item;
                case SaveType.NewLeaf when id == 0x7FFC:
                    return ItemType.Occupied;
                case SaveType.NewLeaf:
                    return ItemType.Furniture; //Just until I gather furniture offsets

                // Welcome Amiibo

                case SaveType.WelcomeAmiibo when id == 0x7FFE:
                    return 0;
                case SaveType.WelcomeAmiibo when (id >= 0x0005 && id <= 0x007B) || (id >= 0x0080 && id <= 0x0097):
                    return ItemType.Tree;
                case SaveType.WelcomeAmiibo when (id >= 0x007C && id <= 0x007F) || (id >= 0x00C9 && id <= 0x00CD) || id == 0x00F8:
                    return ItemType.Weed;
                case SaveType.WelcomeAmiibo when id == 0x009D:
                    return ItemType.Pattern;
                case SaveType.WelcomeAmiibo when id >= 0x0098 && id <= 0x009C:
                    return ItemType.Rock;
                case SaveType.WelcomeAmiibo when id >= 0x009F && id <= 0x00C8:
                    return ItemType.Flower;
                case SaveType.WelcomeAmiibo when id >= 0x00CE && id <= 0x00F7:
                    return ItemType.WiltedFlower;
                case SaveType.WelcomeAmiibo when id == 0x202A:
                    return ItemType.Fossil;
                case SaveType.WelcomeAmiibo when id >= 0x20AC && id <= 0x2117:
                    return ItemType.Money;
                case SaveType.WelcomeAmiibo when id >= 0x212B && id <= 0x223E:
                    return ItemType.Song;
                case SaveType.WelcomeAmiibo when id >= 0x223F && id <= 0x2282:
                    return ItemType.Paper;
                case SaveType.WelcomeAmiibo when id >= 0x2283 && id <= 0x228D:
                    return ItemType.Turnip;
                case SaveType.WelcomeAmiibo when id >= 0x228E && id <= 0x234B:
                    return ItemType.Catchable;
                case SaveType.WelcomeAmiibo when id >= 0x234C && id <= 0x2492:
                    return ItemType.WallpaperCarpet;
                case SaveType.WelcomeAmiibo when id >= 0x2495 && id <= 0x295B:
                    return ItemType.Clothes;
                case SaveType.WelcomeAmiibo when id >= 0x295C && id <= 0x29DE:
                    return ItemType.Gyroid;
                case SaveType.WelcomeAmiibo when id >= 0x334C && id <= 0x338B:
                    return ItemType.Tool;
                case SaveType.WelcomeAmiibo when id >= 0x209F && id <= 0x20A0:
                    return ItemType.Item;
                case SaveType.WelcomeAmiibo when id >= 0x208C && id <= 0x2095:
                    return ItemType.Shell;
                case SaveType.WelcomeAmiibo when id == 0x7FFC:
                    return ItemType.Occupied;
                case SaveType.WelcomeAmiibo:
                    return ItemType.Furniture;
                default:
                    return ItemType.Invalid;
            }
        }
        public static uint GetItemColor(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Furniture:
                    return ItemColorsSettings["FurnitureColor"];
                case ItemType.Flower:
                    return ItemColorsSettings["FlowerColor"];
                case ItemType.Pattern:
                    return ItemColorsSettings["PatternColor"];
                case ItemType.ParchedFlower:
                    return ItemColorsSettings["ParchedFlowerColor"];
                case ItemType.WiltedFlower:
                    return ItemColorsSettings["WiltedFlowerColor"];
                case ItemType.WateredFlower:
                    return ItemColorsSettings["WateredFlowerColor"];
                case ItemType.Money:
                    return ItemColorsSettings["MoneyColor"];
                case ItemType.Rock:
                    return ItemColorsSettings["RockColor"];
                case ItemType.MoneyRock:
                    return ItemColorsSettings["MoneyRockColor"];
                case ItemType.Signboard:
                    return ItemColorsSettings["SignboardColor"];
                case ItemType.Song:
                    return ItemColorsSettings["SongColor"];
                case ItemType.Paper:
                    return ItemColorsSettings["PaperColor"];
                case ItemType.Turnip:
                    return ItemColorsSettings["TurnipColor"];
                case ItemType.Catchable:
                    return ItemColorsSettings["CatchableColor"];
                case ItemType.WallpaperCarpet:
                    return ItemColorsSettings["WallpaperCarpetColor"];
                case ItemType.Clothes:
                    return ItemColorsSettings["ClothesColor"];
                case ItemType.Gyroid:
                    return ItemColorsSettings["GyroidColor"];
                case ItemType.Fossil:
                    return ItemColorsSettings["FossilColor"];
                case ItemType.Tool:
                    return ItemColorsSettings["ToolColor"];
                case ItemType.Item:
                    return ItemColorsSettings["ItemColor"];
                case ItemType.Fruit:
                    return ItemColorsSettings["FruitColor"];
                case ItemType.Trash:
                    return ItemColorsSettings["TrashColor"];
                case ItemType.QuestItem:
                    return ItemColorsSettings["QuestItemColor"];
                case ItemType.RaffleTicket:
                    return ItemColorsSettings["RaffleTicketColor"];
                case ItemType.Tree:
                    return ItemColorsSettings["TreeColor"];
                case ItemType.Weed:
                    return ItemColorsSettings["WeedColor"];
                case ItemType.Shell:
                    return ItemColorsSettings["ShellColor"];
                case ItemType.Empty:
                    return 0x00FFFFFF; // No setting for empty since we always want it transparent
                case ItemType.Occupied:
                    return ItemColorsSettings["OccupiedColor"];
                case ItemType.Building:
                    return ItemColorsSettings["BuildingColor"];
                case ItemType.Diary:
                    return ItemColorsSettings["DiaryColor"];
                case ItemType.HouseObject:
                    return ItemColorsSettings["HouseObjectColor"];
                default:
                    return ItemColorsSettings["InvalidColor"];
            }
        }

        public static string GetItemFlag1Type(Item item, byte itemFlag)
        {
            var itemType = GetItemType(item.ItemId, Save.SaveInstance.SaveType);
            switch (itemType)
            {
                case ItemType.Fruit:
                    switch (itemFlag)
                    {
                        case 0: return "None";
                        case 1: return "x2";
                        case 2: return "x3";
                        case 3: return "x4";
                        case 4: return "x5";
                        case 5: return "x6";
                        case 6: return "x7";
                        case 7: return "x8";
                        case 8: return "x9";
                        default: return "Undocumented Fruit Flag 1";
                    }
                case ItemType.Paper:
                    switch(itemFlag)
                    {
                        case 0: return "None";
                        case 1: return "x2";
                        case 2: return "x3";
                        case 3: return "x4";
                        default: return "Undocumented Paper Flag 1";
                    }
                case ItemType.Pattern:
                    switch(itemFlag)
                    {
                        case 0: return "1-1";
                        case 1: return "1-2";
                        case 2: return "1-3";
                        case 3: return "1-4";
                        case 4: return "1-5";
                        case 5: return "1-6";
                        case 6: return "1-7";
                        case 7: return "1-8";
                        case 8: return "1-9";
                        case 9: return "1-10";
                        case 0xA: return "2-1";
                        case 0xB: return "2-2";
                        case 0xC: return "2-3";
                        case 0xD: return "2-4";
                        case 0xE: return "2-5";
                        case 0xF: return "2-6";
                        case 0x10: return "2-7";
                        case 0x11: return "2-8";
                        case 0x12: return "2-9";
                        case 0x13: return "2-10";
                        case 0x14: return "3-1";
                        case 0x15: return "3-2";
                        case 0x16: return "3-3";
                        case 0x17: return "3-4";
                        case 0x18: return "3-5";
                        case 0x19: return "3-6";
                        case 0x1A: return "3-7";
                        case 0x1B: return "3-8";
                        case 0x1C: return "3-9";
                        case 0x1D: return "3-10";
                        case 0x1E: return "4-1";
                        case 0x1F: return "4-2";
                        case 0x20: return "4-3";
                        case 0x21: return "4-4";
                        case 0x22: return "4-5";
                        case 0x23: return "4-6";
                        case 0x24: return "4-7";
                        case 0x25: return "4-8";
                        case 0x26: return "4-9";
                        case 0x27: return "4-10";
                        default: return "Unknown Pattern Flag 1";
                    }
                //Standard Item Flag 1s
                case ItemType.Tree:
                    switch (itemFlag)
                    {
                        case 0:
                            return "None";
                        case 1:
                            return "Perfect Fruit";
                        case 3:
                            return "No Fruit";
                        case 8:
                            return "Perfect Fruit";
                        case 0x12:
                            return "Prefect Fruit";
                        case 0x30:
                            return "Perfect Fruit";
                        default:
                            return "Undocumented Tree Flag 1";
                    }
                default:
                    switch(itemFlag)
                    {
                        case 0:
                            return "None";
                        case 0x20:
                            return "Present"; //Needs testing, from MarcRobledo's editor
                        case 0x40:
                            return "Watered";
                        case 0x80:
                            return "Buried";
                        default:
                            return "Undocumented Generic Flag 1";
                    }
            }
        }

        public static string GetFurnitureItemFlag2Type(byte itemFlag) //Our Flag 2, but treated as flag 1 by other editors
        {
            switch (itemFlag)
            {
                case 0:
                    return "None";
                case 0x40:
                    return "Rotated 90 Degrees";
                case 0x80:
                    return "Rotated 180 Degrees";
                case 0xC0:
                    return "Rotated 270 Degrees";
                default:
                    return "Undocumented Flag 2";
            }
        }

        public static string GetItemName(ushort itemId)
        {
            switch (Save.SaveInstance.SaveGeneration)
            {
                case SaveGeneration.N3DS:
                    return ItemDatabase.FirstOrDefault(o => o.Key == itemId).Value ?? "Unknown";
                default:
                    if (GetItemType(itemId, Save.SaveInstance.SaveType) == ItemType.Furniture)
                    {
                        return ItemDatabase.FirstOrDefault(o => o.Key == (ushort)(itemId & 0xFFFC)).Value ?? "Unknown";
                    }
                    else
                    {
                        return ItemDatabase.FirstOrDefault(o => o.Key == itemId).Value ?? "Unknown";
                    }
            }
        }

        public static Dictionary<ushort, string> LoadItemDatabase(StreamReader reader)
        {
            var itemDatabase = new Dictionary<ushort, string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Trim() ?? "";
                if (line.StartsWith("//")) continue;

                var itemId = line.Substring(0, 6);
                if (!itemId.StartsWith("0x")) continue;

                try
                {
                    itemDatabase.Add(ushort.Parse(itemId.Replace("0x", ""), NumberStyles.HexNumber), line.Substring(8));
                }
                catch
                {
                    DebugUtility.DebugManagerInstance.WriteLine($"Error in loading item: {line}", DebugLevel.Error);
                }
            }

            return itemDatabase;
        }

        //New Leaf Item Decoding
        public static void DecodeItem(uint encodedItemId, out byte flag1, out byte flag2, out ushort itemId)
        {
            flag1 = (byte)(encodedItemId >> 24); //Flag 1
            flag2 = (byte)(encodedItemId >> 16); //Flag 2
            itemId = (ushort)(encodedItemId);
        }

        //New Leaf Item Encoding
        public static uint EncodeItem(byte flag1, byte flag2, ushort itemId)
        {
            uint encodedItemId = itemId;
            encodedItemId += (uint)(flag2 << 16);
            encodedItemId += (uint)(flag1 << 24);
            return encodedItemId;
        }

        public static uint EncodeItem(Item item)
        {
            uint encodedItemId = item.ItemId;
            encodedItemId += (uint)(item.Flag2 << 16);
            encodedItemId += (uint)(item.Flag1 << 24);
            return encodedItemId;
        }
    }
}
