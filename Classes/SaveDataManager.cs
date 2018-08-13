using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ACSE
{
    public struct Offsets
    {
        public int SaveSize;
        public int Checksum;
        public int TownId;
        public int TownName;
        public int TownNameSize;
        public int PlayerStart;
        public int PlayerSize;
        public int TownData;
        public int TownDataSize;
        public int AcreData;
        public int AcreDataSize;
        public int BuriedData;
        public int BuriedDataSize;
        public int IslandAcreData;
        public int IslandWorldData;
        public int IslandWorldSize;
        public int IslandBuriedData;
        public int IslandBuriedSize;
        public int IslandHouse;
        public int IslandBuildings;
        public int HouseData;
        public int HouseDataSize;
        public int VillagerData;
        public int VillagerSize;
        public int CampsiteVisitor;
        public int IslanderData;
        public int Debt; //Only global in WW
        public int Buildings; //CF/NL
        public int BuildingsCount;
        public int GrassWear;
        public int GrassWearSize;
        public int GrassType;
        public int TrainStationType;
        public int Weather;
        public int NativeFruit;
        public int PublicWorkProjects; //NL only
        public int PastVillagers;
        public int[] CrcOffsets;
    }

    public struct SaveInfo
    {
        public bool ContainsIsland;
        public bool HasIslander;
        public int PatternCount;
        public bool PlayerJpegPicture;
        public Offsets SaveOffsets;
        public int AcreCount;
        public int XAcreCount;
        public int TownAcreCount;
        public int TownYAcreStart;
        public int IslandAcreCount;
        public int IslandXAcreCount;
        public int VillagerCount;
    }

    public enum SaveFileDataOffset
    {
        nafjfla = 0,
        nafjsta = 0, // iQue Animal Forest
        gafjgci = 0x2040, // Doubutsu_no_Mori_Plus
        gafegci = 0x26040,
        gafegcs = 0x26150,
        gaferaw = 0x30000,
        gaejgci = 0x10040, // Doubutsu_no_Mori_e_Plus
        gaeegci = 0x10040, // AnimalForestEPlus (Fan translation)
        gaejraw = 0x1A000,
        gaeeraw = 0x1A000,
        admeduc = 0x1F4,
        admedss = 0x1F4,
        admedsv = 0,
        admesav = 0, // These are for v1.0
        ruuedat = 0,
        edgedat = 0x80,
        edgebin = 0, //RAM Dump
        eaaedat = 0x80,
    }

    public static class SaveDataManager
    {
        #region SaveOffsets
        public static readonly Offsets DoubutsuNoMoriOffsets = new Offsets // Remember that these addresses are valid *after* byteswapping
        {
            SaveSize = 0x10000,
            PlayerStart = 0x20,
            PlayerSize = 0xBD0,
            TownName = 0x2F60,
            TownNameSize = 6,
            TownId = 0x2F68,
            HouseData = 0x3588,
            HouseDataSize = 0xB48,
            AcreData = 0x9EA8,
            AcreDataSize = 0x70,
            VillagerData = 0x9F18,
            VillagerSize = 0x528,
            TownData = 0x62A8,
            TownDataSize = 0x3C00,
            TrainStationType = 0xF438, // Confirm
            Weather = 0xF439, // Confirm
            BuriedData = 0xF43C,
            BuriedDataSize = 0x3C0,
            Checksum = 0x12,
            GrassType = -1, // Find
        };

        public static readonly Offsets DoubutsuNoMoriPlusOffsets = new Offsets
        {
            SaveSize = 0x1E000,
            PlayerStart = 0x20, // Might not be right
            PlayerSize = 0x1E00,
            TownName = 0x7820,
            TownId = 0x7828,
            HouseData = 0x7E48,
            HouseDataSize = 0x2128,
            IslandAcreData = 0x13F60,
            IslandWorldData = 0x1B450,
            IslandWorldSize = 0x400,
            IslandBuriedData = 0x1C908,
            IslandBuriedSize = 0x40,
            IslanderData = 0x1C340,
            TownData = 0x102E8,
            TownDataSize = 0x3C00,
            AcreData = 0x13EE8,
            AcreDataSize = 0x8C,
            VillagerData = 0x13F78,
            VillagerSize = 0x5C8,
            TrainStationType = 0x19E08, // Confirm
            Weather = 0x19E09, // Confirm
            BuriedData = 0x19E0C,
            BuriedDataSize = 0x3C0,
            GrassType = 0x1CCA4, // Confirm
            TownNameSize = 6,
            Checksum = 0x12,
        };

        public static readonly Offsets AnimalCrossingOffsets = new Offsets
        {
            SaveSize = 0x26000,
            TownName = 0x9120,
            TownNameSize = 8,
            TownId = 0x912A,
            PlayerStart = 0x20,
            PlayerSize = 0x2440,
            VillagerData = 0x17438,
            VillagerSize = 0x988,
            AcreData = 0x173A8,
            AcreDataSize = 0x8C,
            TownData = 0x137A8,
            TownDataSize = 0x3C00,
            NativeFruit = 0x20688,
            TrainStationType = 0x20F18,
            Weather = 0x20F19,
            BuriedData = 0x20F1C,
            BuriedDataSize = 0x3C0,
            IslandWorldData = 0x22554,
            IslandWorldSize = 0x400,
            IslandHouse = 0x22960,
            IslandBuriedData = 0x23DC8,
            IslandBuriedSize = 0x40,
            IslanderData = 0x23440,
            HouseData = 0x9CE8,
            HouseDataSize = 0x26B0,
            IslandAcreData = 0x17420,
            CampsiteVisitor = 0x20644,
            GrassType = 0x24184,
            Buildings = -1,
            Debt = -1,
            GrassWear = -1,
            PastVillagers = -1,
            PublicWorkProjects = -1,
            IslandBuildings = -1,
            Checksum = 0x12
            // Lighthouse Event active: 0x2416F (byte != 0)
        };

        public static readonly Offsets DoubutsuNoMoriEPlusOffsets = new Offsets
        {
            SaveSize = 0x2E000,
            TownName = 0x14,
            TownId = 0x1C,
            PlayerStart = 0x1C0, // Might not be right
            PlayerSize = 0x26A0,
            HouseData = 0xA340,
            HouseDataSize = 0x3860,
            IslandWorldData = -1, // Each Player has their own island
            IslandWorldSize = 0x400,
            IslanderData = -1, // Confirm this in game. Couldn't find one in the save file.
            TownData = 0x184C0,
            TownDataSize = 0x3C00,
            AcreData = 0x1C0C0,
            AcreDataSize = 0x8C,
            VillagerData = 0x1C150,
            VillagerSize = 0x680,
            // Shop Size 0x223A8 (appears to be the upper nibble of the byte 0 = 0, 4 = 1, 8 = 2, C = 3)
            // Shop Spend count = 0x223AC (uint)
            // Shop Visitor Spend count = 0x223C0 (uint)
            NativeFruit = 0x2259C,
            TrainStationType = 0x22B18,
            Weather = 0x22B19,
            BuriedData = 0x22B1C,
            BuriedDataSize = 0x3C0,
            // Town Flags? 0x23002 (0x80 = Bridge Placed)
            GrassType = 0x24484,
            // Shop update in progress byte (0x2E004 (Has to be 1B))
            TownNameSize = 6,
            Checksum = 0x12,
        };

        public static readonly Offsets AnimalForestOffsets = new Offsets
        {
            SaveSize = 0x10000,
            PlayerStart = 0x20,
            PlayerSize = 0xBD0,
            TownName = 0x2F60,
            TownNameSize = 6,
            TownId = 0x2F68,
            HouseData = 0x3588,
            HouseDataSize = 0xB48,
            AcreData = 0x9EA8,
            AcreDataSize = 0x70,
            VillagerData = 0x9F18,
            VillagerSize = 0x528,
            TownData = 0x62A8,
            TownDataSize = 0x3C00,
            TrainStationType = 0xF438, // Confirm
            Weather = -1, // Confirm
            BuriedData = 0xF43C,
            BuriedDataSize = 0x3C0,
            Checksum = 0x12,
            GrassType = -1, // Find
        };

        public static readonly Offsets WildWorldOffsets = new Offsets
        {
            SaveSize = 0x15FE0,
            TownId = 0x0002,
            TownName = 0x0004,
            TownNameSize = 8,
            PlayerStart = 0x000C,
            PlayerSize = 0x228C,
            VillagerData = 0x8A3C,
            VillagerSize = 0x700,
            AcreData = 0xC330,
            AcreDataSize = 0x24,
            TownData = 0xC354,
            TownDataSize = 0x2000,
            BuriedData = 0xE354,
            BuriedDataSize = 0x200,
            GrassType = 0xE554,
            HouseData = 0xE558,
            HouseDataSize = 0x15A0,
            Debt = 0xFAE8,
            Buildings = -1,
            GrassWear = -1,
            IslanderData = -1,
            IslandBuriedData = -1,
            IslandWorldData = -1,
            PastVillagers = -1,
            PublicWorkProjects = -1,
            IslandAcreData = -1,
            IslandBuildings = -1,
            Weather = -1,
            TrainStationType = -1,
            Checksum = 0x15FDC
        };

        public static readonly Offsets CityFolkOffsets = new Offsets
        {
            SaveSize = 0x40F340,
            PlayerStart = 0,
            PlayerSize = 0x86C0,
            Buildings = 0x5EB0A,
            BuildingsCount = 0x33, //Not sure
            AcreData = 0x68414, //Don't forget about the additional acres before?
            AcreDataSize = 0x62,
            TownId = 0x640E6,
            TownName = 0x640E8,
            TownNameSize = 16,
            TownData = 0x68476,
            TownDataSize = 0x3200,
            BuriedData = 0x6B676,
            BuriedDataSize = 400,
            GrassWear = 0x6BCB6,
            GrassWearSize = 0x1900,
            GrassType = 0x6D5B7,
            HouseData = 0x6D5C0,
            HouseDataSize = 0x15C0,
            Checksum = -1,
            Debt = -1,
            IslanderData = -1,
            IslandBuriedData = -1,
            IslandWorldData = -1,
            PastVillagers = -1,
            PublicWorkProjects = -1,
            VillagerData = -1, //finish this sometime
            IslandAcreData = -1,
            IslandBuildings = -1,
            TrainStationType = -1,
            Weather = -1,
        };

        private static readonly Offsets NewLeafOffsets = new Offsets
        {
            SaveSize = 0x7F980,
            PlayerStart = 0x20,
            PlayerSize = 0x9F10,
            VillagerData = 0x27C90,
            VillagerSize = 0x24F8,
            PastVillagers = 0x3F17E,
            CampsiteVisitor = 0x3F1CA,
            Buildings = 0x49528,
            BuildingsCount = 58, //TODO: Add island buildings (Island Hut & Loaner Gyroid at 59 and 60)
            AcreData = 0x4DA04,
            AcreDataSize = 0x54,
            TownData = 0x4DA58,
            TownDataSize = 0x5000,
            BuriedData = -1,
            TownName = 0x5C73A,
            TownNameSize = 16,
            GrassWear = 0x53E80,
            HouseData = 0x57F7A,
            HouseDataSize = 0x1228,
            GrassWearSize = 0x3000, //Extra row of "Invisible" X Acres
            GrassType = 0x4DA01,
            IslandAcreData = 0x6A408,
            IslandWorldData = 0x6A428,
            IslandBuildings = 0x6B428,
            TownId = -1, //
            Weather = -1,
            TrainStationType = -1
        };

        public static readonly Offsets WelcomeAmiiboOffsets = new Offsets
        {
            SaveSize = 0x89A80,
            PlayerStart = 0x20,
            PlayerSize = 0xA480,
            VillagerData = 0x29250,
            VillagerSize = 0x2518,
            PastVillagers = 0x4087A,
            CampsiteVisitor = 0x408C6,
            Buildings = 0x4BE08,
            BuildingsCount = 58, //TODO: Add island buildings (Island Hut & Loaner Gyroid at 59 and 60)
            AcreData = 0x53404,
            AcreDataSize = 0x54,
            TownData = 0x53458,
            TownDataSize = 0x5000,
            BuriedData = -1,
            TownName = 0x6213A,
            TownNameSize = 16,
            GrassWear = 0x59880,
            GrassWearSize = 0x3000, //Extra row of "Invisible" X Acres
            GrassType = 0x53401,
            HouseData = 0x5D8FA - 0x44,
            HouseDataSize = 0x1228,
            IslandAcreData = 0x6FE38,
            IslandWorldData = 0x6FE58,
            IslandBuildings = 0x70E58,
            // Enable Statistics Menu (Player 1) 0x57CF set to 0xC0 (binary 1000 0000)
            // ShopSize = 0x621F0, (two bytes in a row?)
            TownId = -1, //
            Weather = -1,
            TrainStationType = -1
        };
        #endregion

        #region SaveInfo
        public static readonly SaveInfo DoubutsuNoMori = new SaveInfo
        {
            ContainsIsland = false,
            HasIslander = false,
            PlayerJpegPicture = false,
            PatternCount = 0,
            SaveOffsets = DoubutsuNoMoriOffsets,
            AcreCount = 56,
            XAcreCount = 7,
            TownAcreCount = 30,
            TownYAcreStart = 1,
            IslandAcreCount = 0,
            IslandXAcreCount = 0,
            VillagerCount = 15
        };

        public static readonly SaveInfo DoubutsuNoMoriPlus = new SaveInfo
        {
            ContainsIsland = true,
            HasIslander = true,
            PlayerJpegPicture = false,
            PatternCount = 8,
            SaveOffsets = DoubutsuNoMoriPlusOffsets,
            AcreCount = 70,
            XAcreCount = 7,
            TownAcreCount = 30,
            IslandAcreCount = 2,
            IslandXAcreCount = 2,
            TownYAcreStart = 1,
            VillagerCount = 16
        };

        public static readonly SaveInfo AnimalCrossing = new SaveInfo // Valid for GAFE and GAFP
        {
            ContainsIsland = true,
            HasIslander = true,
            PlayerJpegPicture = false,
            PatternCount = 8,
            SaveOffsets = AnimalCrossingOffsets,
            AcreCount = 70,
            XAcreCount = 7,
            TownAcreCount = 30,
            IslandAcreCount = 2,
            IslandXAcreCount = 2,
            TownYAcreStart = 1,
            VillagerCount = 16
        };

        public static readonly SaveInfo DoubutsuNoMoriEPlus = new SaveInfo
        {
            ContainsIsland = true,
            HasIslander = true,
            PlayerJpegPicture = false,
            PatternCount = 8,
            SaveOffsets = DoubutsuNoMoriEPlusOffsets,
            AcreCount = 70,
            XAcreCount = 7,
            TownAcreCount = 30,
            IslandAcreCount = 2,
            IslandXAcreCount = 2,
            TownYAcreStart = 1,
            VillagerCount = 15 // Has an islander for every player
        };

        public static readonly SaveInfo AnimalForest = new SaveInfo
        {
            ContainsIsland = false,
            HasIslander = false,
            PlayerJpegPicture = false,
            PatternCount = 0,
            SaveOffsets = AnimalForestOffsets,
            AcreCount = 56,
            XAcreCount = 7,
            TownAcreCount = 30,
            TownYAcreStart = 1,
            IslandAcreCount = 0,
            IslandXAcreCount = 0,
            VillagerCount = 15
        };

        public static readonly SaveInfo WildWorld = new SaveInfo
        {
            ContainsIsland = false,
            HasIslander = false,
            PlayerJpegPicture = false,
            PatternCount = 8,
            SaveOffsets = WildWorldOffsets,
            AcreCount = 36,
            XAcreCount = 6,
            TownAcreCount = 16,
            TownYAcreStart = 1,
            VillagerCount = 8
        };

        public static readonly SaveInfo CityFolk = new SaveInfo
        {
            ContainsIsland = false,
            HasIslander = false,
            PlayerJpegPicture = false,
            PatternCount = 8,
            SaveOffsets = CityFolkOffsets,
            AcreCount = 49,
            XAcreCount = 7,
            TownAcreCount = 25,
            TownYAcreStart = 1,
            VillagerCount = 10
        };

        public static readonly SaveInfo NewLeaf = new SaveInfo
        {
            ContainsIsland = true,
            HasIslander = false,
            PlayerJpegPicture = true,
            PatternCount = 10,
            SaveOffsets = NewLeafOffsets,
            AcreCount = 42,
            XAcreCount = 7,
            TownAcreCount = 20,
            TownYAcreStart = 1,
            VillagerCount = 10,
            IslandAcreCount = 16,
            IslandXAcreCount = 4
        };

        public static readonly SaveInfo WelcomeAmiibo = new SaveInfo
        {
            ContainsIsland = true,
            HasIslander = false,
            PlayerJpegPicture = true,
            PatternCount = 10,
            SaveOffsets = WelcomeAmiiboOffsets,
            AcreCount = 42,
            XAcreCount = 7,
            TownAcreCount = 20,
            TownYAcreStart = 1,
            VillagerCount = 10,
            IslandAcreCount = 16,
            IslandXAcreCount = 4
        };
        #endregion

        public static byte[] ByteSwap(byte[] saveBuff)
        {
            var swappedBuffer = new byte[saveBuff.Length];
            for (var i = 0; i < saveBuff.Length; i += 4)
            {
                var a = saveBuff[i];
                var b = saveBuff[i + 1];
                var c = saveBuff[i + 2];
                var d = saveBuff[i + 3];
                swappedBuffer[i] = d;
                swappedBuffer[i + 1] = c;
                swappedBuffer[i + 2] = b;
                swappedBuffer[i + 3] = a;
            }
            return swappedBuffer;
        }

        public static SaveType? GetSaveType(byte[] saveData)
        {
            switch (saveData.Length)
            {
                case 0x20000:
                    return null; // Return null for this since we set it in the save constructor 
                case 0x72040:
                case 0x72150:
                    {
                        var gameId = Encoding.ASCII.GetString(saveData, saveData.Length == 0x72150 ? 0x110 : 0, 4);
                        switch (gameId)
                        {
                            case "GAFE":
                            case "GAFP":
                            case "GAFU": // GAFP is PAL, GAFU is Australian.
                                return SaveType.AnimalCrossing;
                            case "GAFJ":
                                return SaveType.DoubutsuNoMoriPlus;
                            case "GAEJ":
                                return SaveType.DoubutsuNoMoriEPlus;
                            case "GAEE":
                                return SaveType.AnimalForestEPlus;
                        }
                        break;
                    }
                // Nintendont RAW file length
                case 0x200000:
                    {
                        var gameId = Encoding.ASCII.GetString(saveData, 0x2000, 4);
                        switch (gameId)
                        {
                            case "GAFE":
                            case "GAFP":
                            case "GAFU":
                                return SaveType.AnimalCrossing;
                            case "GAFJ":
                                return SaveType.DoubutsuNoMoriPlus;
                            case "GAEJ":
                                return SaveType.DoubutsuNoMoriEPlus;
                            case "GAEE":
                                return SaveType.AnimalForestEPlus;
                        }
                        break;
                    }
                default:
                    if (Encoding.ASCII.GetString(saveData, 0x1E40, 4) == "EMDA" || saveData.Length == 0x4007A || saveData.Length == 0x401F4 || saveData.Length == 0x40000)
                        return SaveType.WildWorld;
                    else switch (saveData.Length)
                        {
                            case 0x40F340:
                            case 0x47A0DA:
                                return SaveType.CityFolk;
                            case 0x7FA00:
                            case 0x80000:
                                return SaveType.NewLeaf;
                            case 0x89B00:
                                return SaveType.WelcomeAmiibo;
                        }
                    break;
            }

            return SaveType.Unknown;
        }

        public static int GetSaveDataOffset(string gameId, string extension)
        {
            if (Enum.TryParse(gameId + extension, out SaveFileDataOffset extensionEnum))
                return (int)extensionEnum;
            return 0;
        }

        public static string GetGameId(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                    return "NAFJ";
                case SaveType.AnimalCrossing:
                    return "GAFE";
                case SaveType.DoubutsuNoMoriPlus: // The Save Data Sector still uses NAFJ as the identification marker
                    return "GAFJ";
                case SaveType.DoubutsuNoMoriEPlus:
                    return "GAEJ";
                case SaveType.AnimalForestEPlus:
                    return "GAEE";
                case SaveType.AnimalForest:
                    return "NAFJ"; // internally still has NAFJ code
                case SaveType.WildWorld: //Currently only supporting the English versions of WW+
                    return "ADME";
                case SaveType.CityFolk:
                    return "RUUE";
                case SaveType.NewLeaf:
                    return "EDGE";
                case SaveType.WelcomeAmiibo:
                    return "EAAE";
                default:
                    return "Unknown";
            }
        }

        public static string GetGameTitle(SaveType saveType, Region region = Region.NTSC)
        {
            switch (region)
            {
                case Region.Japan:
                    switch (saveType)
                    {
                        case SaveType.DoubutsuNoMori:
                            return "どうぶつの森";
                        case SaveType.DoubutsuNoMoriPlus:
                            return "どうぶつの森+";
                        case SaveType.AnimalCrossing:
                            return "Animal Crossing";
                        case SaveType.DoubutsuNoMoriEPlus:
                            return "どうぶつの森e+";
                        case SaveType.AnimalForestEPlus:
                            return "Animal Forest e+";
                        case SaveType.AnimalForest:
                            return "どうぶつの森 iQue";
                        case SaveType.WildWorld:
                            return "おいでよどうぶつの森";
                        case SaveType.CityFolk:
                            return "街へ行こうよどうぶつの森";
                        case SaveType.NewLeaf:
                            return "とびだせ どうぶつの森 ";
                        case SaveType.WelcomeAmiibo:
                            return "とびだせ どうぶつの森 amiibo+";
                        default:
                            return "不明な保存タイプ";
                    }

                case Region.PAL:
                    switch (saveType)
                    {
                        case SaveType.DoubutsuNoMori:
                            return "Dōbutsu no Mori";
                        case SaveType.DoubutsuNoMoriPlus:
                            return "Dōbutsu no Mori+";
                        case SaveType.AnimalCrossing:
                            return "Animal Crossing";
                        case SaveType.DoubutsuNoMoriEPlus:
                            return "Dōbutsu no Mori e+";
                        case SaveType.AnimalForestEPlus:
                            return "Animal Forest e+";
                        case SaveType.AnimalForest:
                            return "Animal Forest";
                        case SaveType.WildWorld:
                            return "Wild World";
                        case SaveType.CityFolk:
                            return "Let's Go to the City";
                        case SaveType.NewLeaf:
                            return "New Leaf";
                        case SaveType.WelcomeAmiibo:
                            return "Welcome Amiibo";
                        default:
                            return "Unknown Save Type";
                    }

                case Region.NTSC:
                default:
                    switch (saveType)
                    {
                        case SaveType.DoubutsuNoMori:
                            return "Dōbutsu no Mori";
                        case SaveType.DoubutsuNoMoriPlus:
                            return "Dōbutsu no Mori+";
                        case SaveType.AnimalCrossing:
                            return "Animal Crossing";
                        case SaveType.DoubutsuNoMoriEPlus:
                            return "Dōbutsu no Mori e+";
                        case SaveType.AnimalForestEPlus:
                            return "Animal Forest e+";
                        case SaveType.AnimalForest:
                            return "Animal Forest";
                        case SaveType.WildWorld:
                            return "Wild World";
                        case SaveType.CityFolk:
                            return "City Folk";
                        case SaveType.NewLeaf:
                            return "New Leaf";
                        case SaveType.WelcomeAmiibo:
                            return "Welcome Amiibo";
                        case SaveType.Unknown:
                        default:
                            return "Unknown Save Type";
                    }
            }
        }

        // This method is useful for grouping games by the console they were released on (since they're normally expansions/revisions)
        public static SaveGeneration GetSaveGeneration(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                    return SaveGeneration.N64;
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return SaveGeneration.GCN;
                case SaveType.AnimalForest:
                    return SaveGeneration.iQue;
                case SaveType.WildWorld:
                    return SaveGeneration.NDS;
                case SaveType.CityFolk:
                    return SaveGeneration.Wii;
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                    return SaveGeneration.N3DS;
                default:
                    return SaveGeneration.Unknown;
            }
        }

        public static SaveInfo GetSaveInfo(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                    return DoubutsuNoMori;
                case SaveType.DoubutsuNoMoriPlus:
                    return DoubutsuNoMoriPlus;
                case SaveType.AnimalCrossing:
                    return AnimalCrossing;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus: // TODO: Change this?
                    return DoubutsuNoMoriEPlus;
                case SaveType.AnimalForest:
                    return AnimalForest;
                case SaveType.WildWorld:
                    return WildWorld;
                case SaveType.CityFolk:
                    return CityFolk;
                case SaveType.NewLeaf:
                    return NewLeaf;
                case SaveType.WelcomeAmiibo:
                    return WelcomeAmiibo;
                default:
                    return WildWorld;
            }
        }

        public static Dictionary<ushort, string> GetItemInfo(SaveType saveType, string language = "en")
        {
            StreamReader contents;
            var itemDbLocation = MainForm.AssemblyLocation + "\\Resources\\";
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalForest:
                    itemDbLocation += "DnM_Items_" + language + ".txt";
                    break;
                case SaveType.DoubutsuNoMoriPlus:
                    itemDbLocation += "DBNM_Plus_Items_" + language + ".txt";
                    break;
                case SaveType.AnimalCrossing:
                    itemDbLocation += "AC_Items_" + language + ".txt";
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    itemDbLocation += "DBNM_e_Plus_Items_" + language + ".txt";
                    break;
                case SaveType.WildWorld:
                    itemDbLocation += "WW_Items_" + language + ".txt";
                    break;
                case SaveType.CityFolk:
                    itemDbLocation += "CF_Items_" + language + ".txt";
                    break;
                case SaveType.NewLeaf:
                    itemDbLocation += "NL_Items_" + language + ".txt";
                    break;
                case SaveType.WelcomeAmiibo:
                    itemDbLocation += "WA_Items_" + language + ".txt";
                    break;
            }

            try { contents = File.OpenText(itemDbLocation); }
            catch (Exception e)
            {
                MainForm.DebugManager.WriteLine(
                    $"An error occured opening item database file:\n\"{itemDbLocation}\"\nError Info:\n{e.Message}", DebugLevel.Error);
                return null;
            }
            var itemDictionary = new Dictionary<ushort, string>();
            string line;
            while ((line = contents.ReadLine()) != null)
            {
                if (!Properties.Settings.Default.DebuggingEnabled && line.Contains("//"))
                    MessageBox.Show("Now loading item type: " + line.Replace("//", ""));
                else if (!line.Contains("//") && line.Contains("0x"))
                {
                    string itemIdString = line.Substring(0, 6), itemName = line.Substring(7).Trim();
                    if (ushort.TryParse(itemIdString.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out var itemId))
                        itemDictionary.Add(itemId, itemName);
                    else
                        MainForm.DebugManager.WriteLine("Unable to add item: " + itemIdString + " | " + itemName, DebugLevel.Error);
                }
            }

            contents.Close();
            contents.Dispose();

            return itemDictionary;
        }

        public static Dictionary<byte, string> GetAcreInfo(SaveType saveType, string language = "en")
        {
            StreamReader contents;
            var acreDbLocation = MainForm.AssemblyLocation + "\\Resources\\";
            if (saveType == SaveType.WildWorld)
                acreDbLocation += "WW_Acres_" + language + ".txt";
            try { contents = File.OpenText(acreDbLocation); }
            catch (Exception e)
            {
                MainForm.DebugManager.WriteLine(
                    $"An error occured opening acre database file:\n\"{acreDbLocation}\"\nError Info:\n{e.Message}", DebugLevel.Error);
                return null;
            }
            var acreDictionary = new Dictionary<byte, string>();
            string line;
            while ((line = contents.ReadLine()) != null)
            {
                if (!Properties.Settings.Default.DebuggingEnabled && line.Contains("//"))
                    MessageBox.Show("Now loading Acre type: " + line.Replace("//", ""));
                else if (line.Contains("0x"))
                {
                    string acreIdString = line.Substring(0, 4), acreName = line.Substring(5);
                    if (byte.TryParse(acreIdString.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out var acreId))
                        acreDictionary.Add(acreId, acreName);
                    else
                        MainForm.DebugManager.WriteLine("Unable to add Acre: " + acreIdString + " | " + acreName, DebugLevel.Error);
                }
            }

            contents.Close();
            contents.Dispose();

            return acreDictionary;
        }

        public static Dictionary<ushort, string> GetAcreInfoUInt16(SaveType saveType, string language = "en")
        {
            StreamReader contents;
            var acreDbLocation = MainForm.AssemblyLocation + "\\Resources\\";
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalForest:
                case SaveType.AnimalCrossing: // TODO: DnM needs to have a custom list, since the docks/islands don't exist
                    acreDbLocation += "AC_Acres_" + language + ".txt";
                    break;
                case SaveType.DoubutsuNoMoriPlus:
                    acreDbLocation += "DBNM_Plus_Acres_" + language + ".txt";
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    acreDbLocation += "DBNM_e_Plus_Acres_" + language + ".txt";
                    break;
                case SaveType.CityFolk:
                    acreDbLocation += "CF_Acres_" + language + ".txt";
                    break;
                case SaveType.NewLeaf:
                    acreDbLocation += "NL_Acres_" + language + ".txt";
                    break;
                case SaveType.WelcomeAmiibo:
                    acreDbLocation += "WA_Acres_" + language + ".txt";
                    break;
            }

            try { contents = File.OpenText(acreDbLocation); }
            catch (Exception e)
            {
                MainForm.DebugManager.WriteLine(
                    $"An error occured opening acre database file:\n\"{acreDbLocation}\"\nError Info:\n{e.Message}", DebugLevel.Error);
                return null;
            }
            var acreDictionary = new Dictionary<ushort, string>();
            string line;
            while ((line = contents.ReadLine()) != null)
            {
                if (!Properties.Settings.Default.DebuggingEnabled && line.Contains("//"))
                    MessageBox.Show("Now loading Acre type: " + line.Replace("//", ""));
                else if (line.Contains("0x"))
                {
                    string acreIdString = line.Substring(0, 6), acreName = line.Substring(7);
                    if (ushort.TryParse(acreIdString.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out var acreId))
                        acreDictionary.Add(acreId, acreName);
                    else
                        MainForm.DebugManager.WriteLine("Unable to add Acre: " + acreIdString + " | " + acreName, DebugLevel.Error);
                }
            }

            contents.Close();
            contents.Dispose();

            return acreDictionary;
        }


        public static Dictionary<string, List<byte>> GetFiledAcreData(SaveType saveType, string language = "en")
        {
            StreamReader contents;
            var acreDbLocation = MainForm.AssemblyLocation + "\\Resources\\";
            if (saveType == SaveType.WildWorld)
                acreDbLocation += "WW_Acres_" + language + ".txt";
            try { contents = File.OpenText(acreDbLocation); }
            catch (Exception e)
            {
                MainForm.DebugManager.WriteLine(
                    $"An error occured opening acre database file:\n\"{acreDbLocation}\"\nError Info:\n{e.Message}", DebugLevel.Error);
                return null;
            }
            var filedList = new Dictionary<string, List<byte>>();
            string line;
            var currentAcreType = "Unsorted";
            while ((line = contents.ReadLine()) != null)
            {
                if (line.Contains("//"))
                {
                    if (currentAcreType != "Unsorted")
                    {
                        if (!filedList.ContainsKey(currentAcreType))
                            filedList.Add(currentAcreType, new List<byte>());
                    }
                    currentAcreType = line.Replace("//", "");
                }
                else if (line.Contains("0x"))
                {
                    if (!filedList.ContainsKey(currentAcreType))
                        filedList.Add(currentAcreType, new List<byte>());
                    string acreIdString = line.Substring(0, 4), acreName = line.Substring(5);
                    if (byte.TryParse(acreIdString.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out var acreId))
                        filedList[currentAcreType].Add(acreId);
                    else
                        MainForm.DebugManager.WriteLine("Unable to add Acre: " + acreIdString + " | " + acreName, DebugLevel.Error);
                }
            }

            contents.Close();
            contents.Dispose();

            return filedList;
        }

        public static Dictionary<string, Dictionary<ushort, string>> GetFiledAcreDataUInt16(SaveType saveType, string language = "en")
        {
            StreamReader contents;
            var acreDbLocation = MainForm.AssemblyLocation + "\\Resources\\";
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalForest:
                case SaveType.AnimalCrossing: // TODO: DnM needs to have a custom list, since the docks/islands don't exist
                    acreDbLocation += "AC_Acres_" + language + ".txt";
                    break;
                case SaveType.DoubutsuNoMoriPlus:
                    acreDbLocation += "DBNM_Plus_Acres_" + language + ".txt";
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    acreDbLocation += "DBNM_e_Plus_Acres_" + language + ".txt";
                    break;
                case SaveType.CityFolk:
                    acreDbLocation += "CF_Acres_" + language + ".txt";
                    break;
                case SaveType.NewLeaf:
                    acreDbLocation += "NL_Acres_" + language + ".txt";
                    break;
                case SaveType.WelcomeAmiibo:
                    acreDbLocation += "WA_Acres_" + language + ".txt";
                    break;
            }

            try { contents = File.OpenText(acreDbLocation); }
            catch (Exception e)
            {
                MainForm.DebugManager.WriteLine(
                    $"An error occured opening acre database file:\n\"{acreDbLocation}\"\nError Info:\n{e.Message}", DebugLevel.Error);
                return null;
            }
            var filedList = new Dictionary<string, Dictionary<ushort, string>>();
            string line;
            var currentAcreType = "Unsorted";
            while ((line = contents.ReadLine()) != null)
            {
                if (line.Contains("//"))
                {
                    if (currentAcreType != "Unsorted")
                    {
                        if (!filedList.ContainsKey(currentAcreType))
                            filedList.Add(currentAcreType, new Dictionary<ushort, string>());
                    }
                    currentAcreType = line.Replace("//", "");
                }
                else if (line.Contains("0x"))
                {
                    if (!filedList.ContainsKey(currentAcreType))
                        filedList.Add(currentAcreType, new Dictionary<ushort, string>());
                    string acreIdString = line.Substring(0, 6), acreName = line.Substring(7);
                    if (ushort.TryParse(acreIdString.Replace("0x", ""), NumberStyles.AllowHexSpecifier, null, out var acreId))
                        filedList[currentAcreType].Add(acreId, line.Substring(7));
                    else
                        MainForm.DebugManager.WriteLine("Unable to add Acre: " + acreIdString + " | " + acreName, DebugLevel.Error);
                }
            }

            contents.Close();
            contents.Dispose();

            return filedList;
        }
    }
}
