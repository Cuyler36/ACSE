using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

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
                    SimpleVillager SpecialVillager = new SimpleVillager();
                    SpecialVillager.Villager_ID = v.Key;
                    SpecialVillager.Name = v.Value;
                    WA_Database.Add(v.Key, SpecialVillager);
                }
                return new BindingSource(WA_Database, null);
            }
            return new BindingSource(WA_Special_Villagers, null);
        }

        public static Dictionary<ushort, string> Villagers = new Dictionary<ushort, string>()
        {
            
        };

        public static List<KeyValuePair<ushort, string>> VillagerDatabase = new List<KeyValuePair<ushort, string>>();

        public static void CreateVillagerDatabase()
        {
            foreach (KeyValuePair<ushort, string> k in Villagers)
                VillagerDatabase.Add(k);
        }

        public static string[] Personalities = new string[6]
        {
            "Lazy", "Normal", "Peppy", "Jock", "Cranky", "Snooty"
        };

        public static string GetVillagerName(ushort villagerId)
        {
            if (Villagers.ContainsKey(villagerId))
                return Villagers[villagerId];
            return villagerId == 0x0 ? "No Villager" : "Unknown";
        }

        public static string GetVillagerPersonality(int type)
        {
            return type < 6 ? Personalities[type] : "Lazy";
        }

        public static int GetVillagerPersonalityID(string personality)
        {
            return Array.IndexOf(Personalities, personality) > -1 ? Array.IndexOf(Personalities, personality) : 0;
        }

        public static ushort GetVillagerID(string villagerName)
        {
            if (Villagers.ContainsValue(villagerName))
                return Villagers.FirstOrDefault(x => x.Value == villagerName).Key;
            return 0xE000;
        }

        public static ushort GetVillagerIdByIndex(int i)
        {
            if (Villagers.Count > i)
                return Villagers.Keys.ElementAt(i);
            return 0xE000;
        }
    }

    public class Villager
    {
        public ushort ID = 0;
        public ushort TownIdentifier = 0;
        public string Name = "";
        public string Personality = "";
        public byte PersonalityID = 0;
        public int Index = 0;
        public string Catchphrase = "";
        public bool Exists = false;
        public bool Modified = false;
        public Item Shirt;
        public byte[] House_Coords = new byte[4]; //X-Acre, Y-Acre, Y-Position, X-Position - 1 (This is actually the location of their sign, also dictates map location)
        public Villager_Player_Entry[] Villager_Player_Entries = new Villager_Player_Entry[7];
        public int Offset = 0;

        public Villager(int idx)
        {
            Index = idx;
            //Offset = Index == 16 ? MainForm.Islander_Offset : MainForm.VillagerData_Offset + (Index - 1) * 0x988;
            ID = DataConverter.ReadUShort(Offset);
            TownIdentifier = DataConverter.ReadUShort(Offset + 2);
            Name = VillagerData.GetVillagerName(ID);
            PersonalityID = DataConverter.ReadDataRaw(Offset + 0xD, 1)[0];
            Personality = VillagerData.GetVillagerPersonality(PersonalityID);
            Catchphrase = DataConverter.ReadString(Offset + 0x89D, 10).Trim();
            Shirt = new Item(DataConverter.ReadUShort(Offset + 0x8E4));
            House_Coords = DataConverter.ReadDataRaw(Offset + 0x899, 4); //Could make this WorldCoords class if used for other things
            //House_Coords[2] = (byte)(House_Coords[2] + 1);
            //House_Coords[3] = (byte)(House_Coords[3] + 1); //X-Position is the position of the Villager Name Sign, which is to the left of the house object, so we add one.
            Exists = ID != 0x0000 && ID != 0xFFFF;
            for (int i = 0; i < 7; i++)
            {
                int Entry_Offset = Offset + 0x10 + (i * 0x138); //Offset + 16 data bytes + entrynum * entrysize
                uint Player_ID = DataConverter.ReadUInt(Entry_Offset + 0x10);
                if (Player_ID < 0xFFFFFFFF && Player_ID >= 0xF0000000)
                    Villager_Player_Entries[i] = new Villager_Player_Entry(DataConverter.ReadDataRaw(Entry_Offset, 0x138), Entry_Offset);
            }
        }

        public void Write()
        {
            //House_Coords[2] = (byte)(House_Coords[2] - 1);
            //House_Coords[3] = (byte)(House_Coords[3] - 1);
            DataConverter.Write(Offset, ID);
            DataConverter.Write(Offset + 2, TownIdentifier);
            //DataConverter.WriteString(Offset + 4, DataConverter.ReadString(MainForm.Town_Name_Offset, 8).Trim(), 8); //Set town name
            DataConverter.WriteByte(Offset + 0xC, Index == 16 ? (byte)0xFF : (byte)(ID & 0x00FF)); //Normally same as villager identifier, but is 0xFF for islanders. This is likely the byte for what AI the villager will use.
            DataConverter.WriteByte(Offset + 0xD, PersonalityID);
            DataConverter.WriteString(Offset + 0x89D, Catchphrase, 10);
            DataConverter.WriteByteArray(Offset + 0x899, House_Coords, false);
            if (Shirt != null)
                DataConverter.Write(Offset + 0x8E4, Shirt.ItemID);
            if (!Exists && Modified)
            {
                DataConverter.WriteByteArray(Offset + 0x8EB, new byte[] { 0xFF, 0x01 }, false); //This byte might be the met flag. Setting it just in case
                Exists = true;
                if (Index < 16)
                    Add_House();
            }
            foreach (Villager_Player_Entry Entry in Villager_Player_Entries)
                if (Entry != null && Entry.Exists)
                    Entry.Write(); //Update Player Entries
            Modified = false;
            //Second byte here is always a random number. This could be responsible for the Villager's AI, but I'm not sure. Just writing it for good measure.
            //If the Villager's house location is out of bounds, (or just left 0xFFFF) the game will pick a random signboard as the new house location and write it on load.
        }

        public void Delete()
        {
            if (Index < 16) //Don't delete islander
            {
                //if (Properties.Settings.Default.ModifyVillagerHouse)
                    //Remove_House();
                ID = 0;
                TownIdentifier = 0xFFFF;
                PersonalityID = 6;
                Catchphrase = "";
                House_Coords = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };
                Shirt = new Item(0);
                Exists = false;
                Modified = false;
            }
        }

        public void Remove_House()
        {
            ushort House_ID = BitConverter.ToUInt16(new byte[] { (byte)(ID & 0x00FF), 0x50 }, 0);
            /*ushort[] World_Buffer = DataConverter.ReadUShortArray(MainForm.AcreData_Offset, MainForm.AcreData_Size / 2);
            for (int i = 0; i < World_Buffer.Length; i++)
            {
                if (World_Buffer[i] == House_ID)
                {
                    for (int x = i - 17; x < i - 14; x++) //First Row
                        World_Buffer[x] = 0;
                    for (int x = i - 1; x < i + 2; x++) //Middle Row
                        World_Buffer[x] = 0;
                    for (int x = i + 15; x < i + 18; x++) //Final Row
                        World_Buffer[x] = 0;
                    World_Buffer[i] = BitConverter.ToUInt16(new byte[] { (byte)(new Random().Next(0x10, 0x25)), 0x58 }, 0); //New Signboard to replace house
                    //This is akin to actual game behavior
                }
            }*/
            //DataConverter.Write(MainForm.AcreData_Offset, World_Buffer);
        }

        public void Add_House()
        {
            ushort House_ID = BitConverter.ToUInt16(new byte[] { (byte)(ID & 0x00FF), 0x50 }, 0);
            /*ushort[] World_Buffer = DataConverter.ReadUShortArray(MainForm.AcreData_Offset, MainForm.AcreData_Size / 2);
            if (House_Coords[0] > 5 || House_Coords[1] > 6 || House_Coords[2] > 15 || House_Coords[3] > 15) //Houses can't be on edge of acres
                return;
            int Position = (House_Coords[0] - 1) * 256 + (House_Coords[1] - 1) * 1280 + (House_Coords[2]) + (House_Coords[3] - 1) * 16; //X Acre + Y Acre + X Pos + Y Pos
            if (Position > 0x1E00) //7,680 item spots per town (minus island acres) (5 * 6 * 16^2)
                return;
            for (int x = Position - 17; x < Position - 14; x++) //First Row
                World_Buffer[x] = 0xFFFF;
            for (int x = Position - 1; x < Position + 2; x++) //Middle Row
                World_Buffer[x] = 0xFFFF;
            for (int x = Position + 15; x < Position + 18; x++) //Final Row
                World_Buffer[x] = 0xFFFF;
            World_Buffer[Position] = House_ID;
            World_Buffer[Position + 15] = 0xA012; //Add Nameplate

            DataConverter.Write(MainForm.AcreData_Offset, World_Buffer);*/
        }

        public Villager_Player_Entry[] Get_Player_Entries(uint Matching_Player_ID)
        {
            return Villager_Player_Entries.Where(Entry => Entry.Player_ID == Matching_Player_ID).ToArray();
        }
    }

    /*
      Animal Crossing: Population Growing Villager Player Entry Structure:

        Villager Player Entry Structure (Size: 0x138)
        Player Name: 0x000 - 0x007
        Town Name: 0x008 - 0x00F
        Player ID: 0x010 - 0x013
        Met Date: 0x014 - 0x01B
        Met Town Name: 0x01C - 0x023
        Met Town ID: 0x024 - 0x025
        Padding??: 0x026 - 0x027
        Unknown Data: 0x028 - 0x02F
        Friendship: 0x030 (Min = 0 (1), Max = 7F (128))
        Unknown Bytes: 0x031 - 0x032
        Saved Message: 0x033? - 0x138
    */
    public class Villager_Player_Entry
    {
        public bool Exists = false;
        public int Offset;
        //Struct Start
        public string Player_Name;
        public string Player_Town_Name;
        public uint Player_ID;
        public ACDate Met_Date;
        public string Met_Town_Name;
        public ushort Met_Town_ID;
        public byte[] Garbage = new byte[8]; //I have no idea wtf these are for. Might investigate some day.
        public sbyte Friendship;
        public Mail Saved_Letter; //Going to have to change this to a custom class, as it strips most mail header data (Length is 0x100? Message part is still 0xF8)
        //

        public Villager_Player_Entry(byte[] entryData, int offset)
        {
            Exists = true;
            Offset = offset;
            byte[] playerNameBytes = new byte[8], playerTownName = new byte[8], metTownName = new byte[8], metDate = new byte[8], playerId = new byte[4], metTownId = new byte[2];
            Buffer.BlockCopy(entryData, 0, playerNameBytes, 0, 8);
            Buffer.BlockCopy(entryData, 8, playerTownName, 0, 8);
            Buffer.BlockCopy(entryData, 0x1C, metTownName, 0, 8);
            Buffer.BlockCopy(entryData, 0x10, playerId, 0, 4);
            Buffer.BlockCopy(entryData, 0x14, metDate, 0, 8);
            Buffer.BlockCopy(entryData, 0x24, metTownId, 0, 2);
            Array.Reverse(playerId);
            Array.Reverse(metTownId);

            Player_Name = new ACString(playerNameBytes).Trim();
            Player_Town_Name = new ACString(playerTownName).Trim();
            Met_Town_Name = new ACString(metTownName).Trim();
            Met_Date = new ACDate(metDate);
            Player_ID = BitConverter.ToUInt32(playerId, 0);
            Met_Town_ID = BitConverter.ToUInt16(metTownId, 0);
            Friendship = (sbyte)entryData[0x30];
        }

        public void Max_Friendship()
        {
            Friendship = 0x7F;
        }

        public void Write()
        {
            //Player Name is handled by renaming the player
            //DataConverter.WriteByteArray(Offset + 0x8, DataConverter.ReadDataRaw(MainForm.Town_Name_Offset, 8)); //Update Town Name
            //if (DataConverter.ReadUShort(8) == Met_Town_ID)
            //    DataConverter.WriteByteArray(Offset + 0x1C, DataConverter.ReadDataRaw(MainForm.Town_Name_Offset, 8)); //Update Met Town Name
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
        public int House_CoordinatesSize;
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
        public byte[] House_Coordinates; //Animal Crossing only?
        public byte Status;
        //Player Entries?
    }

    public static class VillagerInfo
    {
        public static VillagerOffsets AC_Villager_Offsets = new VillagerOffsets
        {
            Villager_ID = 0,
            Town_ID = 2,
            Town_Name = 4,
            Town_NameSize = 8,
            Villager_AI = 0xC,
            Personality = 0xD,
            House_Coordinates = 0x899,
            House_CoordinatesSize = 4,
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
                case SaveType.Animal_Crossing:
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
                        SimpleVillager Entry = new SimpleVillager();
                        Entry.Villager_ID = ushort.Parse(Line.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                        Entry.Name = Line.Substring(6);
                        Database.Add(Entry.Villager_ID, Entry);
                    }
                }
            }
            else if (Save_Type == SaveType.Animal_Crossing)
            {
                while ((Line = Contents.ReadLine()) != null)
                {
                    if (Line.Contains("0x"))
                    {
                        SimpleVillager Entry = new SimpleVillager();
                        Entry.Villager_ID = ushort.Parse(Line.Substring(2, 4), NumberStyles.AllowHexSpecifier);
                        Entry.Name = Line.Substring(8);
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
                case SaveType.Animal_Crossing:
                    return AC_Villager_Offsets;
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
                            else if (FieldType == typeof(string))
                            {
                                SaveData.Write(DataOffset, ACString.GetBytes((string)VillagerDataType.GetField(Field.Name).GetValue(Data),
                                    (int)VillagerOffsetData.GetField(Field.Name + "Size").GetValue(Offsets)));
                            }
                            else if (FieldType == typeof(byte))
                            {
                                SaveData.Write(DataOffset, (byte)VillagerDataType.GetField(Field.Name).GetValue(Data));
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
            // Special case here, since Villager_Data_Start + 0xC is always the same as the villager's ID. (Lower byte)
            // TODO: Add check to see if the villager is an islander. If they are, use 0xFF instead.
            if (SaveData.Save_Type == SaveType.Animal_Crossing)
            {
                SaveData.Write(Offset + 0xC, (byte)Data.Villager_ID);
            }
            // TODO: Add House Coordinate Saving/Updating (Also automatic placing/removing??) for the GC version
        }
    }
}
