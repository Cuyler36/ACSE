using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Globalization;
using ACSE.Classes.Utilities;

namespace ACSE
{
    class VillagerData
    {
        public static Dictionary<ushort, string> WA_Special_Villagers = new Dictionary<ushort, string>
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

        public static BindingSource GetCaravanBindingSource()
        {
            Dictionary<ushort, SimpleVillager> WA_Database = VillagerInfo.GetVillagerDatabase(SaveType.Welcome_Amiibo);
            if (WA_Database != null)
            {
                foreach (var v in WA_Special_Villagers)
                {
                    SimpleVillager SpecialVillager = new SimpleVillager
                    {
                        Villager_ID = v.Key,
                        Name = v.Value
                    };
                    WA_Database.Add(v.Key, SpecialVillager);
                }
                return new BindingSource(WA_Database, null);
            }
            return new BindingSource(WA_Special_Villagers, null);
        }
    }

    public struct VillagerOffsets
    {
        public int Villager_ID;
        public int Villager_AI;
        public int Catchphrase;
        public int CatchphraseSize;
        public int Nicknames;
        public int NicknamesSize;
        public int NicknamesCount;
        public int Personality;
        public int Town_ID;
        public int Town_Name;
        public int Town_NameSize;
        public int Shirt;
        public int Umbrella;
        public int Song;
        public int Carpet;
        public int Wallpaper;
        public int Furniture;
        public int FurnitureCount;
        public int House_Coordinates;
        public int House_CoordinatesCount;
        public int Status;
    }

    //Rename when VillagerData class is removed
    public struct VillagerDataStruct
    {
        public ushort Villager_ID;
        public byte Villager_AI;
        public string Catchphrase;
        public string[] Nicknames;
        public byte Personality;
        public ushort Town_ID;
        public string Town_Name;
        public Item Shirt;
        public Item Umbrella;
        public Item Song;
        public Item Carpet;
        public Item Wallpaper;
        public Item[] Furniture;
        public byte[] House_Coordinates; //N64 - NDS
        public byte Status;
        //Player Entries?
    }

    public static class VillagerInfo
    {
        public static VillagerOffsets Doubutsu_no_Mori_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Town_ID = 2,
            Town_Name = 4,
            Town_NameSize = 6,
            Villager_AI = 0xA,
            Personality = 0xB,
            House_Coordinates = 0x4E1,
            House_CoordinatesCount = 4,
            Catchphrase = 0x4E5,
            CatchphraseSize = 0x4,
            Shirt = 0x520
        };

        public static VillagerOffsets Doubtusu_no_Mori_Plus_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Town_ID = 2,
            Town_Name = 4,
            Town_NameSize = 6,
            Villager_AI = 0xA, // Goes unused??
            Personality = 0xB,
            House_Coordinates = 0x4E1,
            House_CoordinatesCount = 4,
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

        public static VillagerOffsets AC_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Town_ID = 2,
            Town_Name = 4,
            Town_NameSize = 8,
            Villager_AI = 0xC,
            Personality = 0xD,
            House_Coordinates = 0x899,
            House_CoordinatesCount = 4,
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

        public static VillagerOffsets Doubtusu_no_Mori_e_Plus_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Town_ID = 2,
            Town_Name = 4,
            Town_NameSize = 6,
            Villager_AI = 0xA, // Goes unused??
            Personality = 0xB,
            House_Coordinates = 0x591, // Confirm
            House_CoordinatesCount = 4,
            Catchphrase = 0x595, // Confirm
            CatchphraseSize = 0x4,
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

        public static VillagerOffsets WW_Villager_Offsets = new VillagerOffsets
        {
            //0 = Relationships (0x68 bytes each)
            //Pattern as well
            Furniture = 0x6AC,
            FurnitureCount = 0xA,
            Personality = 0x6CA,
            Villager_ID = 0x6CB,
            Wallpaper = 0x6CC,
            Carpet = 0x6CE,
            Song = 0x6D0, //Check this
            Shirt = 0x6EC,
            Catchphrase = 0x6DE,
            CatchphraseSize = 0xA,
            Villager_AI = -1, // Research
            Town_ID = -1, //Research
            Town_Name = -1, //Research
            House_Coordinates = -1, //Research
            Nicknames = -1, //Research
            Status = -1, //Research
            Umbrella = -1, //Research
            //Finish rest of offsets
        };

        public static VillagerOffsets CF_Villager_Offsets = new VillagerOffsets
        {
            //Villagers in City Folk are interesting.
            //The actual data structure is stored in the save file, allowing for customization of the entire villager.
            //This includes name, textures, and what villager model it uses!
            //That will mean a lot more work will have to go into this part of the villager editor, though.
            //I'll have to finish it at a later date. Unfortunately, I can't find the source to NPC Tool, a tool that allowed all of these modifications to be done
            //This means I'll probably have to reverse engineer the format myself
        };

        public static VillagerOffsets NL_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Personality = 2,
            Status = 0x24C4,
            Catchphrase = 0x24A6,
            CatchphraseSize = 0x16,
            Town_Name = 0x24CE,
            Town_NameSize = 0x12,
            Town_ID = -1, // Research
            Shirt = 0x244E,
            Song = 0x2452,
            Wallpaper = 0x2456,
            Carpet = 0x245A,
            Umbrella = 0x245E,
            Furniture = 0x2462,
            FurnitureCount = 16,
            House_Coordinates = -1,
            Nicknames = -1, //Research
            Villager_AI = -1,
        };

        public static VillagerOffsets WA_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Personality = 2,
            Status = 0x24E4,
            Catchphrase = 0x24C6,
            CatchphraseSize = 0x16,
            Town_Name = 0x24CE,
            Town_NameSize = 0x12,
            Town_ID = -1, // Research
            Shirt = 0x246E,
            Song = 0x2472,
            Wallpaper = 0x2476,
            Carpet = 0x247A,
            Umbrella = 0x247E,
            Furniture = 0x2482,
            FurnitureCount = 16,
            House_Coordinates = -1,
            Nicknames = -1, //Research
            Villager_AI = -1,
        };

        public static string[] AC_Personalities = new string[7]
        {
            "Normal (Female)", "Peppy (Female)", "Lazy (Male)", "Jock (Male)", "Cranky (Male)", "Snooty (Female)", "Not Set"
        };

        public static string[] WW_Personalities = new string[7]
        {
            "Lazy (Male)", "Jock (Male)", "Cranky (Male)", "Normal (Female)", "Peppy (Female)", "Snooty (Female)", "Not Set"
        };

        public static string[] NL_Personalities = new string[9]
        {
            "Lazy (Male)", "Jock (Male)", "Cranky (Male)", "Smug (Male)", "Normal (Female)", "Peppy (Female)", "Snooty (Female)", "Caring (Uchi) (Female)", "Not Set"
        };

        public static string[] GetPersonalities(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return AC_Personalities;
                case SaveType.Wild_World:
                    return WW_Personalities;
                case SaveType.New_Leaf:
                    return NL_Personalities;
                case SaveType.Welcome_Amiibo:
                    return NL_Personalities;
            }
            return new string[0];
        }

        public static Dictionary<ushort, SimpleVillager> GetVillagerDatabase(SaveType Save_Type, string Language = "en")
        {
            Dictionary<ushort, SimpleVillager> Database = new Dictionary<ushort, SimpleVillager>();
            StreamReader Contents = null;
            string Database_Filename = NewMainForm.Assembly_Location + "\\Resources\\{0}_Villagers_" + Language + ".txt";
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori: // TODO: Needs its own database (no islanders or punchy)
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                case SaveType.Animal_Crossing:
                    Database_Filename = string.Format(Database_Filename, "AC");
                    break;
                case SaveType.Wild_World:
                    Database_Filename = string.Format(Database_Filename, "WW");
                    break;
                case SaveType.New_Leaf:
                    Database_Filename = string.Format(Database_Filename, "NL");
                    break;
                case SaveType.Welcome_Amiibo:
                    Database_Filename = string.Format(Database_Filename, "WA");
                    break;
            }
            try { Contents = File.OpenText(Database_Filename); }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error occured opening villager database file:\n\"{0}\"\nError Info:\n{1}", Database_Filename, e.Message));
                return null;
            }
            string Line;
            if (Save_Type == SaveType.New_Leaf || Save_Type == SaveType.Welcome_Amiibo)
            {
                while ((Line = Contents.ReadLine()) != null)
                {
                    if (Line.Contains("0x"))
                    {
                        SimpleVillager Entry = new SimpleVillager();
                        string ID = Regex.Match(Line, @"ID = 0x....,").Value.Substring(7, 4);
                        Entry.Villager_ID = ushort.Parse(ID, NumberStyles.AllowHexSpecifier);
                        string Name_Str = Regex.Match(Line, @"Name = .+").Value.Substring(7);
                        Entry.Name = Name_Str.Substring(0, Name_Str.IndexOf(','));
                        string Personality = Regex.Match(Line, @"Personality = .").Value;
                        Entry.Personality = byte.Parse(Personality.Substring(Personality.Length - 1, 1));
                        Database.Add(Entry.Villager_ID, Entry);
                        //MessageBox.Show("ID: " + Entry.Villager_ID.ToString("X4") + " | Name: " + Entry.Name + " | Personality: " + Entry.Personality);
                    }
                }
            }
            else if (Save_Type == SaveType.Wild_World)
            {
                while ((Line = Contents.ReadLine()) != null)
                {
                    if (Line.Contains("0x"))
                    {
                        SimpleVillager Entry = new SimpleVillager
                        {
                            Villager_ID = ushort.Parse(Line.Substring(2, 2), NumberStyles.AllowHexSpecifier),
                            Name = Line.Substring(6)
                        };
                        Database.Add(Entry.Villager_ID, Entry);
                    }
                }
            }
            else if (Save_Type == SaveType.Doubutsu_no_Mori || Save_Type == SaveType.Animal_Crossing || Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_Type == SaveType.Doubutsu_no_Mori_e_Plus)
            {
                while ((Line = Contents.ReadLine()) != null)
                {
                    if (Line.Contains("0x"))
                    {
                        SimpleVillager Entry = new SimpleVillager
                        {
                            Villager_ID = ushort.Parse(Line.Substring(2, 4), NumberStyles.AllowHexSpecifier),
                            Name = Line.Substring(8)
                        };
                        Database.Add(Entry.Villager_ID, Entry);
                    }
                }
            }
            return Database;
        }

        public static VillagerOffsets GetVillagerInfo(SaveType Save_Type)
        {
            switch(Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                    return Doubutsu_no_Mori_Villager_Offsets;
                case SaveType.Doubutsu_no_Mori_Plus:
                    return Doubtusu_no_Mori_Plus_Villager_Offsets;
                case SaveType.Animal_Crossing:
                    return AC_Villager_Offsets;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return Doubtusu_no_Mori_e_Plus_Villager_Offsets;
                case SaveType.Wild_World:
                    return WW_Villager_Offsets;
                case SaveType.New_Leaf:
                    return NL_Villager_Offsets;
                case SaveType.Welcome_Amiibo:
                    return WA_Villager_Offsets;
            }
            return new VillagerOffsets { };
        }
    }

    public class SimpleVillager
    {
        public ushort Villager_ID;
        public byte Personality;
        public string Name;
        public string Catchphrase;
        public Item Shirt;
        public Item[] Furniture;
        //public uint AI (Last bytes in NL Villagers?)

        public override string ToString()
        {
            return Name ?? "Unknown";
        }
    }

    public class NewVillager
    {
        public VillagerOffsets Offsets;
        public VillagerDataStruct Data;
        public VillagerPlayerEntry[] PlayerEntries;
        public int Index;
        public int Offset;
        public string Name;
        public bool Exists = false;
        private Save SaveData;

        public NewVillager(int offset, int idx, Save save)
        {
            SaveData = save;
            Index = idx;
            Offset = offset;
            Offsets = VillagerInfo.GetVillagerInfo(save.Save_Type);
            if (save.Save_Type == SaveType.Wild_World)
                Exists = SaveData.ReadByte(offset + Offsets.Villager_ID) != 0 && SaveData.ReadByte(offset + Offsets.Villager_ID) != 0xFF;
            else
                Exists = SaveData.ReadUInt16(offset + Offsets.Villager_ID, save.Is_Big_Endian) != 0 && SaveData.ReadUInt16(offset + Offsets.Villager_ID, save.Is_Big_Endian) != 0xFFFF;
            object BoxedData = new VillagerDataStruct();
            foreach (var Field in typeof(VillagerOffsets).GetFields(BindingFlags.Public | BindingFlags.Instance))
                if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                    if (typeof(VillagerDataStruct).GetField(Field.Name) != null)
                    {
                        if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                        {
                            Type FieldType = typeof(VillagerDataStruct).GetField(Field.Name).FieldType;
                            int DataOffset = Offset + (int)Field.GetValue(Offsets);

                            if (Field.Name == "Villager_ID" && save.Save_Type == SaveType.Wild_World) //Villager IDs are only a byte in WW
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, SaveData.ReadByte(DataOffset));
                            else if (FieldType == typeof(byte))
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, SaveData.ReadByte(DataOffset));
                            else if (FieldType == typeof(byte[]) && typeof(VillagerOffsets).GetField(Field.Name + "Count") != null)
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, SaveData.ReadByteArray(DataOffset,
                                    (int)typeof(VillagerOffsets).GetField(Field.Name + "Count").GetValue(Offsets)));
                            else if (FieldType == typeof(ushort))
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, SaveData.ReadUInt16(DataOffset, SaveData.Is_Big_Endian));
                            else if (FieldType == typeof(ushort[]))
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, SaveData.ReadUInt16Array(DataOffset,
                                    (int)typeof(VillagerOffsets).GetField(Field.Name + "Count").GetValue(Offsets), SaveData.Is_Big_Endian));
                            else if (FieldType == typeof(uint))
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, SaveData.ReadUInt32(DataOffset, SaveData.Is_Big_Endian));
                            else if (FieldType == typeof(string))
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, new ACString(SaveData.ReadByteArray(DataOffset,
                                    (int)typeof(VillagerOffsets).GetField(Field.Name + "Size").GetValue(Offsets)), SaveData.Save_Type).Trim());
                            else if (FieldType == typeof(string[])) { }
                            //Add logic
                            else if (FieldType == typeof(Item))
                                if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo)
                                    typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, new Item(SaveData.ReadUInt32(DataOffset, false)));
                                else
                                    typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, new Item(SaveData.ReadUInt16(DataOffset, SaveData.Is_Big_Endian)));
                            else if (FieldType == typeof(Item[]))
                            {
                                Item[] Collection = new Item[(int)typeof(VillagerOffsets).GetField(Field.Name + "Count").GetValue(Offsets)];
                                for (int i = 0; i < Collection.Length; i++)
                                {
                                    if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo)
                                        Collection[i] = new Item(SaveData.ReadUInt32(DataOffset + i * 4, false));
                                    else
                                        Collection[i] = new Item(SaveData.ReadUInt16(DataOffset + i * 2, SaveData.Is_Big_Endian));
                                }
                                typeof(VillagerDataStruct).GetField(Field.Name).SetValue(BoxedData, Collection);
                            }
                        }
                    }
            Data = (VillagerDataStruct)BoxedData;
        }

        public override string ToString()
        {
            return Name ?? "Unknown";
        }

        public VillagerPlayerEntry GetPlayerEntry(NewPlayer Player)
        {
            if (PlayerEntries != null)
            {
                return PlayerEntries.First(o => o.Player == Player);
            }
            return null;
        }

        public void Write()
        {
            if (Exists)
            {
                // Set Villager TownID & Name
                if (Offsets.Town_ID != -1)
                {
                    Data.Town_ID = SaveData.ReadUInt16(SaveData.Save_Data_Start_Offset + NewMainForm.Current_Save_Info.Save_Offsets.Town_ID, SaveData.Is_Big_Endian); // Might not be UInt16 in all games
                }
                if (Offsets.Town_Name != -1)
                {
                    Data.Town_Name = SaveData.ReadString(SaveData.Save_Data_Start_Offset + NewMainForm.Current_Save_Info.Save_Offsets.Town_Name,
                        NewMainForm.Current_Save_Info.Save_Offsets.Town_NameSize);
                }
                //MessageBox.Show(string.Format("Writing Villager #{0} with data offset of 0x{1}", Index, Offset.ToString("X")));
                Type VillagerOffsetData = typeof(VillagerOffsets);
                Type VillagerDataType = typeof(VillagerDataStruct);
                foreach (var Field in VillagerOffsetData.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                    {
                        if (VillagerDataType.GetField(Field.Name) != null)
                        {
                            if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                            {
                                Type FieldType = VillagerDataType.GetField(Field.Name).FieldType;
                                int DataOffset = Offset + (int)Field.GetValue(Offsets);
                                //MessageBox.Show("Field Name: " + Field.Name + " | Data Offset: " + DataOffset.ToString("X"));
                                if (Field.Name == "Villager_ID")
                                {
                                    if (SaveData.Save_Type == SaveType.Wild_World)
                                    {
                                        SaveData.Write(DataOffset, Convert.ToByte(VillagerDataType.GetField(Field.Name).GetValue(Data)));
                                    }
                                    else //Might not encompass City Folk
                                    {
                                        SaveData.Write(DataOffset, (ushort)VillagerDataType.GetField(Field.Name).GetValue(Data), SaveData.Is_Big_Endian);
                                    }
                                }
                                else if (FieldType == typeof(string) && SaveData.Game_System != SaveGeneration.N3DS) // Temp 3DS exclusion
                                {
                                    SaveData.Write(DataOffset, ACString.GetBytes((string)VillagerDataType.GetField(Field.Name).GetValue(Data),
                                        (int)VillagerOffsetData.GetField(Field.Name + "Size").GetValue(Offsets)));
                                }
                                else if (FieldType == typeof(byte))
                                {
                                    SaveData.Write(DataOffset, (byte)VillagerDataType.GetField(Field.Name).GetValue(Data));
                                }
                                else if (FieldType == typeof(byte[]))
                                {
                                    SaveData.Write(DataOffset, (byte[])VillagerDataType.GetField(Field.Name).GetValue(Data));
                                }
                                else if (FieldType == typeof(ushort))
                                {
                                    SaveData.Write(DataOffset, (ushort)VillagerDataType.GetField(Field.Name).GetValue(Data), SaveData.Is_Big_Endian);
                                }
                                else if (FieldType == typeof(Item))
                                {
                                    if (SaveData.Save_Type == SaveType.New_Leaf || SaveData.Save_Type == SaveType.Welcome_Amiibo)
                                    {
                                        SaveData.Write(DataOffset, ItemData.EncodeItem((Item)VillagerDataType.GetField(Field.Name).GetValue(Data)), SaveData.Is_Big_Endian);
                                    }
                                    else
                                    {
                                        SaveData.Write(DataOffset, ((Item)VillagerDataType.GetField(Field.Name).GetValue(Data)).ItemID, SaveData.Is_Big_Endian);
                                    }
                                }
                            }
                        }
                    }
                }

                // Special case here, since Villager_Data_Start + 0xC is always the same as the villager's ID. This seems to dictate something about their speech. (Lower byte)
                // TODO: Add check to see if the villager is an islander. If they are, use 0xFF instead.
                if (SaveData.Save_Type == SaveType.Animal_Crossing)
                {
                    SaveData.Write(Offset + 0xC, (byte)Data.Villager_ID);
                }
            }
            // TODO: Add House Coordinate Saving/Updating (Also automatic placing/removing??) for the GC/DS version
        }
    }
}
