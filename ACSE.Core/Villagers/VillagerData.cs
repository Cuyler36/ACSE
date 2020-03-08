using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using ACSE.Core.Debug;
using ACSE.Core.Items;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Villagers
{
    public static class VillagerData
    {
        public static readonly Dictionary<ushort, string> WaSpecialVillagers = new Dictionary<ushort, string>
        {
            {0x1000, "Copper"},
            {0x1001, "Booker"},
            {0x1002, "Jingle"},
            {0x1003, "Jack"},
            {0x1004, "Zipper"},
            {0x1005, "Blanca"},
            {0x1006, "Pavé"},
            {0x1007, "Chip"},
            {0x1008, "Nat"},
            {0x1009, "Franklin"},
            {0x100A, "Joan"},
            {0x100B, "Wendell"},
            {0x100C, "Pascal"},
            {0x100D, "Gulliver"},
            {0x100E, "Saharah"},
            {0x2000, "Isabelle (1)"},
            {0x2001, "Digby"},
            {0x2002, "Reese"},
            {0x2003, "Cyrus"},
            {0x2004, "Tom Nook"},
            {0x2005, "Lottie"},
            {0x2006, "Mabel"},
            {0x2007, "K.K. Slider"},
            {0x2008, "Kicks"},
            {0x2009, "Resetti"},
            {0x200A, "Celeste"},
            {0x200B, "Blathers"},
            {0x200C, "Rover"},
            {0x200D, "Timmy"},
            {0x200E, "Kapp'n"},
            {0x200F, "Wisp"},
            {0x2010, "Isabelle (2)"}
        };

        public static Dictionary<ushort, SimpleVillager> GetCaravanBindingSource()
        {
            var waDatabase = VillagerInfo.GetVillagerDatabase(SaveType.WelcomeAmiibo);
            if (waDatabase == null) return null;

            foreach (var v in WaSpecialVillagers)
            {
                var specialVillager = new SimpleVillager
                {
                    VillagerId = v.Key,
                    Name = v.Value
                };
                waDatabase.Add(v.Key, specialVillager);
            }

            return waDatabase;
        }
    }

    public struct VillagerOffsets
    {
        public int VillagerId;
        public int NameId;
        public int Catchphrase;
        public int CatchphraseSize;
        public int Nicknames;
        public int Personality;
        public int TownId;
        public int TownName;
        public int TownNameSize;
        public int Shirt;
        public int Umbrella;
        public int Song;
        public int Carpet;
        public int Wallpaper;
        public int Furniture;
        public int FurnitureCount;
        public int HouseCoordinates;
        public int HouseCoordinatesCount;
        public int Status;
    }

    //Rename when VillagerData class is removed
    public struct VillagerDataStruct
    {
        public ushort VillagerId;
        public byte NameId; // In N64/GC, this is their "speech pattern" and not their "AI"
        public string Catchphrase;
        public byte Personality;
        public ushort TownId;
        public string TownName;
        public Item Shirt;
        public Item Umbrella;
        public Item Song;
        public Item Carpet;
        public Item Wallpaper;
        public Item[] Furniture;

        public byte[]
            HouseCoordinates; //(In N64 & GCN, it's X-Acre, Y-Acre, X-Position, Y-Position)[X/Y Positions are the bottom center item (house item + 0x10)]

        public byte Status;
        //Player Entries?
    }

    public static class VillagerInfo
    {
        public static readonly VillagerOffsets DoubutsuNoMoriVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0,
            TownId = 2,
            TownName = 4,
            TownNameSize = 6,
            NameId = 0xA,
            Personality = 0xB,
            HouseCoordinates = 0x4E1,
            HouseCoordinatesCount = 4,
            Catchphrase = 0x4E5,
            CatchphraseSize = 0x4,
            Shirt = 0x520,
            Status = -1,
            Umbrella = -1,
            Furniture = -1,
            Carpet = -1,
            Wallpaper = -1,
            Nicknames = -1,
            Song = -1,
        };

        public static readonly VillagerOffsets DoubtusuNoMoriPlusVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0,
            TownId = 2,
            TownName = 4,
            TownNameSize = 6,
            NameId = 0xA, // Goes unused??
            Personality = 0xB,
            HouseCoordinates = 0x4E1,
            HouseCoordinatesCount = 4,
            Catchphrase = 0x4E5,
            CatchphraseSize = 0x4,
            Shirt = 0x520,
            Status = -1, //Research
            Umbrella = -1, //Research this as well
            Furniture = -1, //No Furniture customization in AC
            Carpet = -1,
            Wallpaper = -1,
            Nicknames = -1, //Inside of "Player Entries"
            Song = -1,
            //Add Player Entries (Relationships)
        };

        public static readonly VillagerOffsets AcVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0,
            TownId = 2,
            TownName = 4,
            TownNameSize = 8,
            NameId = 0xC,
            Personality = 0xD,
            HouseCoordinates = 0x899,
            HouseCoordinatesCount = 4,
            Catchphrase = 0x89D,
            CatchphraseSize = 0xA,
            Shirt = 0x8E4,
            Status = -1, //Research
            Umbrella = -1, //Research this as well
            Furniture = -1, //No Furniture customization in AC
            Carpet = -1,
            Wallpaper = -1,
            Nicknames = -1, //Inside of "Player Entries"
            Song = -1,
            //Add Player Entries (Relationships)
        };

        public static readonly VillagerOffsets DoubtusuNoMoriEPlusVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0,
            TownId = 2,
            TownName = 4,
            TownNameSize = 6,
            NameId = 0xA, // Goes unused??
            Personality = 0xB,
            HouseCoordinates = 0x591,
            HouseCoordinatesCount = 4,
            Catchphrase = 0x595,
            CatchphraseSize = 0x4,
            // Space (3) ?
            // QuestData = 0x59C, // 0xC in size (total struct is 0x24)
            Shirt = 0x5DA,
            Status = -1, //Research
            Umbrella = -1, //Research this as well
            Furniture = -1, //No Furniture customization in AC
            Carpet = -1,
            Wallpaper = -1,
            Nicknames = -1, //Inside of "Player Entries"
            Song = -1,
            //Add Player Entries (Relationships)
        };

        public static readonly VillagerOffsets WwVillagerOffsets = new VillagerOffsets
        {
            //0 = Relationships (0x68 bytes each)
            //Pattern as well
            Furniture = 0x6AC,
            FurnitureCount = 0xA,
            Personality = 0x6CA,
            VillagerId = 0x6CB,
            Wallpaper = 0x6CC,
            Carpet = 0x6CE,
            Song = 0x6D0, //Check this
            HouseCoordinates = 0x6E8,
            HouseCoordinatesCount = 2,
            Shirt = 0x6EC,
            Catchphrase = 0x6DE,
            CatchphraseSize = 0xA,
            NameId = -1, // Research
            TownId = -1, //Research
            TownName = -1, //Research
            Nicknames = -1, //Research
            Status = -1, //Research
            Umbrella = -1, //Research
            //Finish rest of offsets
        };

        public static readonly VillagerOffsets CfVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0x1824,
            Shirt = 0x1826,
            Carpet = 0x1828,
            Wallpaper = 0x182A,
            Umbrella = 0x182C,
            Furniture = 0x182E,
            FurnitureCount = 10,
            Song = 0x1842,
            // StoredName = 0x1858 // (US EN)
            Catchphrase = 0x18EC, // EN US
            CatchphraseSize = 20,
            TownId = 0x224C,
            TownName = 0x224E,
            TownNameSize = 8,
            Personality = 0x230A,
            Status = 0x3030, // Moving State Flags?
            HouseCoordinates = -1,
            Nicknames = -1,
            NameId = -1
        };

        public static readonly VillagerOffsets NlVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0,
            Personality = 2,
            Status = 0x24C4,
            Catchphrase = 0x24A6,
            CatchphraseSize = 0x16,
            TownName = 0x24CE,
            TownNameSize = 0x12,
            TownId = -1, // Research
            Shirt = 0x244E,
            Song = 0x2452,
            Wallpaper = 0x2456,
            Carpet = 0x245A,
            Umbrella = 0x245E,
            Furniture = 0x2462,
            FurnitureCount = 16,
            HouseCoordinates = -1,
            Nicknames = -1, //Research
            NameId = -1,
        };

        public static readonly VillagerOffsets WaVillagerOffsets = new VillagerOffsets
        {
            VillagerId = 0,
            Personality = 2,
            Shirt = 0x246E,
            Song = 0x2472,
            Wallpaper = 0x2476,
            Carpet = 0x247A,
            Umbrella = 0x247E,
            Furniture = 0x2482,
            FurnitureCount = 16,
            Catchphrase = 0x24C6,
            CatchphraseSize = 0x16,
            Status = 0x24E4,
            TownName = 0x24EE,
            TownNameSize = 0x12,
            TownId = -1, // Research
            HouseCoordinates = -1,
            Nicknames = -1, //Research
            NameId = -1,
        };

        public static readonly string[] AcPersonalities = {
            "Normal ♀", "Peppy ♀", "Lazy ♂", "Jock ♂", "Cranky ♂", "Snooty ♀", "Not Set"
        };

        public static readonly string[] WwPersonalities = {
            "Lazy ♂", "Jock ♂", "Cranky ♂", "Normal ♀", "Peppy ♀", "Snooty ♀", "Not Set"
        };

        public static readonly string[] NlPersonalities = {
            "Lazy ♂", "Jock ♂", "Cranky ♂", "Smug ♂", "Normal ♀", "Peppy ♀", "Snooty ♀", "Uchi ♀", "Not Set"
        };

        public static string[] GetPersonalities(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                case SaveType.DongwuSenlin:
                    return AcPersonalities;
                case SaveType.WildWorld:
                case SaveType.CityFolk:
                    return WwPersonalities;
                case SaveType.NewLeaf:
                    return NlPersonalities;
                case SaveType.WelcomeAmiibo:
                    return NlPersonalities;
                case SaveType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }

            return new string[0];
        }

        public static Dictionary<ushort, SimpleVillager> GetVillagerDatabase(SaveType saveType, string language = "en")
        {
            var database = new Dictionary<ushort, SimpleVillager>();
            StreamReader contents;
            var databaseFilename = "{0}_Villagers_" + language + ".txt";
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    databaseFilename = string.Format(databaseFilename, "DnM");
                    break;
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                    databaseFilename = string.Format(databaseFilename, "AC");
                    break;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    databaseFilename = string.Format(databaseFilename, "DBNM_e_Plus");
                    break;
                case SaveType.WildWorld:
                    databaseFilename = string.Format(databaseFilename, "WW");
                    break;
                case SaveType.CityFolk:
                    databaseFilename = string.Format(databaseFilename, "CF");
                    break;
                case SaveType.NewLeaf:
                    databaseFilename = string.Format(databaseFilename, "NL");
                    break;
                case SaveType.WelcomeAmiibo:
                    databaseFilename = string.Format(databaseFilename, "WA");
                    break;
                case SaveType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }

            databaseFilename = Path.Combine(PathUtility.GetResourcesDirectory(), databaseFilename);

            try
            {
                contents = File.OpenText(databaseFilename);
            }
            catch (Exception e)
            {
                DebugUtility.DebugManagerInstance.WriteLine(
                    $"An error occured opening villager database file:\n\"{databaseFilename}\"\nError Info:\n{e.Message}",
                    DebugLevel.Error);
                return null;
            }

            string line;
            switch (saveType)
            {
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                    while ((line = contents.ReadLine()) != null)
                    {
                        if (!line.Contains("0x")) continue;
                        var entry = new SimpleVillager();
                        var id = Regex.Match(line, @"ID = 0x....,").Value.Substring(7, 4);
                        entry.VillagerId = ushort.Parse(id, NumberStyles.AllowHexSpecifier);
                        var nameStr = Regex.Match(line, @"Name = .+").Value.Substring(7);
                        entry.Name = nameStr.Substring(0, nameStr.IndexOf(','));
                        var personality = Regex.Match(line, @"Personality = .").Value;
                        entry.Personality = byte.Parse(personality.Substring(personality.Length - 1, 1));
                        database.Add(entry.VillagerId, entry);
                    }

                    break;
                case SaveType.WildWorld:
                    while ((line = contents.ReadLine()) != null)
                    {
                        if (!line.Contains("0x")) continue;
                        var entry = new SimpleVillager
                        {
                            VillagerId = ushort.Parse(line.Substring(2, 2), NumberStyles.AllowHexSpecifier),
                            Name = line.Substring(6)
                        };
                        database.Add(entry.VillagerId, entry);
                    }

                    break;
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                case SaveType.DongwuSenlin:
                case SaveType.CityFolk:
                    while ((line = contents.ReadLine()) != null)
                    {
                        if (!line.Contains("0x")) continue;
                        var entry = new SimpleVillager
                        {
                            VillagerId = ushort.Parse(line.Substring(2, 4), NumberStyles.AllowHexSpecifier),
                            Name = line.Substring(8)
                        };
                        database.Add(entry.VillagerId, entry);
                    }

                    break;
                case SaveType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }

            contents.Close();
            contents.Dispose();

            return database;
        }

        public static VillagerOffsets GetVillagerInfo(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                    return DoubutsuNoMoriVillagerOffsets;
                case SaveType.DoubutsuNoMoriPlus:
                    return DoubtusuNoMoriPlusVillagerOffsets;
                case SaveType.AnimalCrossing:
                    return AcVillagerOffsets;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return DoubtusuNoMoriEPlusVillagerOffsets;
                case SaveType.DongwuSenlin:
                    return DoubutsuNoMoriVillagerOffsets; // TEMP
                case SaveType.WildWorld:
                    return WwVillagerOffsets;
                case SaveType.CityFolk:
                    return CfVillagerOffsets;
                case SaveType.NewLeaf:
                    return NlVillagerOffsets;
                case SaveType.WelcomeAmiibo:
                    return WaVillagerOffsets;
                case SaveType.Unknown:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(saveType), saveType, null);
            }

            return new VillagerOffsets();
        }
    }
}
