using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACSE
{
    public struct PlayerSaveInfo
    {
        public bool Null;
        public int Name;
        public int NameSize;
        public int TownName;
        public int TownNameSize;
        public int FaceType;
        public int Gender;
        public int Tan;
        public int HairType;
        public int HairColor;
        public int EyeColor;
        public int Bells;
        public int Savings;
        public int Debt;
        public int Shirt;
        public int Pants;
        public int Socks;
        public int Shoes;
        public int ShoeColor; //City Folk only
        public int FaceItem; //Glasses/Masks
        public int Hat;
        public int HeldItem;
        public int Messages;
        public int MessageCount;
        public int MessageSize;
        public int InventoryBackground; //Anything e+ and before
        public int Identifier;
        public int IdentifierSize;
        public int Pockets; //Not sure if New Leaf uses ints for storage as well (since they have Flag1 & Flag2)
        public int PocketsCount;
        public int Dressers;
        public int DressersSize; //Number of items per dresser
        public int DressersCount; //Number of dresser "drawers"
        public int Patterns;
        public int PatternSize;
        public int PatternCount;
        public int RegisterDate;
        public int RegisterDateSize;
        public int LastPlayDate;
        public int LastPlayDateSize;
        public int Reset;
        public int ResetSize;
        public int Catalog; //Remember in e+ and before, your catalog "includes" your encyclopedia...
        public int CatalogSize;
        public int Encyclopedia;
        public int EncyclopediaSize;
        public int TownPassCardImage; //Use parsing method to determine size, as this will prevent custom pictures that are smaller from not being displayed properly
        public int FriendCode;
        public int Emotions;
        public int EmotionCount;
        public int NookPoints;
        public int Bed;
        public int Wetsuit;
        public int Birthday;
        public int TownIdentifier;
        public int TownIdentifierSize;
        public int NL_Wallet;
        public int NL_Savings;
        public int NL_Debt;
        public int MeowCoupons;
    }

    public struct PlayerData
    {
        public string Name;
        public string TownName;
        public ushort Identifier;
        public ushort TownIdentifier;
        public uint Bells;
        public uint Debt;
        public uint Savings;
        public Inventory Pockets;
        public Item Shirt;
        public Item Pants;
        public Item Socks;
        public Item Shoes;
        public Item Hat;
        public Item FaceItem;
        public Item HeldItem;
        public Item InventoryBackground;
        public Item Bed;
        public Item Wetsuit; //NL only
        public Item[] Dessers;
        public Mail[] Letters;
        public Pattern[] Patterns;
        public byte[] Emotions; //Add Emotions class?
        public ushort NookPoints;
        public ACDate LastPlayDate;
        public ACDate RegisterDate;
        public ACDate Birthday;
        public byte Gender;
        public byte HairType;
        public byte HairColor;
        public byte FaceType;
        public byte Tan;
        public byte EyeColor;
        public byte ShoeColor; //CF only
        public Image TownPassCardImage;
        public byte[] TownPassCardData;
        public NL_Int32 NL_Wallet;
        public NL_Int32 NL_Savings;
        public NL_Int32 NL_Debt;
        public NL_Int32 MeowCoupons;
    }

    public static class PlayerInfo
    {
        public static PlayerSaveInfo Animal_Crossing = new PlayerSaveInfo
        {
            Name = 0,
            NameSize = 8,
            TownName = 8,
            TownNameSize = 8,
            Identifier = 0x10,
            IdentifierSize = 2,
            TownIdentifier = 0x12,
            TownIdentifierSize = 2,
            Gender = 0x14,
            FaceType = 0x15,
            Pockets = 0x68,
            PocketsCount = 15,
            Bells = 0x8C,
            Debt = 0x90,
            HeldItem = 0x4A4,
            InventoryBackground = 0x1084,
            Shirt = 0x108A, //The lower byte in the item id is also at 0x1089
            Reset = 0x10F4,
            ResetSize = 8,
            Savings = 0x122C,
            Patterns = 0x1240,
            PatternCount = 8,
            PatternSize = 0x220, //Actual Size is 0x200, with the first 0x20 bytes for Name, then for palette & padding?
            Tan = 0x23C8,
            TownPassCardImage = -1,
            HairType = -1,
            Bed = -1,
            Birthday = -1,
            Catalog = -1, //Actually research
            Encyclopedia = -1,
            Dressers = -1,
            Emotions = -1,
            EyeColor = -1,
            FaceItem = -1,
            FriendCode = -1,
            HairColor = -1,
            Hat = -1,
            LastPlayDate = -1,
            Messages = -1,
            NookPoints = -1,
            Pants = -1,
            RegisterDate = -1,
            ShoeColor = -1,
            Shoes = -1,
            Socks = -1,
            Wetsuit = -1,
            NL_Debt = -1,
            NL_Savings = -1,
            NL_Wallet = -1,
            MeowCoupons = -1,
        };

        public static PlayerSaveInfo Wild_World = new PlayerSaveInfo
        {
            Patterns = 0,
            PatternCount = 8,
            PatternSize = 0x228, //Extra 8 bytes for the Town Name over AC
            Messages = 0x1140,
            MessageSize = 0xF4,
            MessageCount = 10, //0x1AD4 = Dear Future Letter
            Pockets = 0x1B22,
            PocketsCount = 15,
            Bells = 0x1B40, //0x1B44
            Catalog = 0x1B48, //Includes Encyclopedia...
            CatalogSize = 0x123,
            FriendCode = 0x1E2C,
            Savings = 0x21E4,
            Emotions = 0x21EC,
            EmotionCount = 4,
            Reset = 0x21FC,
            ResetSize = 2,
            NookPoints = 0x2208,
            HeldItem = 0x220A,
            Shirt = 0x220C,
            Hat = 0x220E,
            FaceItem = 0x2210,
            Bed = 0x2212,
            InventoryBackground = 0x2214,
            LastPlayDate = 0x2216,
            LastPlayDateSize = 4,
            Birthday = 0x2218,
            HairType = 0x223C, //Upper Nibble is Hair Type, Lower Nibble is Face Type
            HairColor = 0x223D, //Upper Nibble is Hair Color, Lower Nibble is Tan
            Gender = 0x228A,
            Identifier = 0x2280,
            IdentifierSize = 2,
            Name = 0x2282,
            NameSize = 8,
            TownIdentifier = 0x2276,
            TownIdentifierSize = 2,
            TownName = 0x2278,
            TownNameSize = 8,
            //Have to use -1 to specify it doesn't exist..
            Tan = -1,
            FaceType = -1,
            TownPassCardImage = -1,
            Debt = -1,
            Dressers = -1, //Research
            DressersCount = 0,
            DressersSize = 0,
            RegisterDate = -1,
            ShoeColor = -1,
            Shoes = -1,
            Socks = -1,
            Pants = -1,
            EyeColor = -1,
            Encyclopedia = -1, //Research
            Wetsuit = -1,
            NL_Debt = -1,
            NL_Savings = -1,
            NL_Wallet = -1,
            MeowCoupons = -1,
        };

        public static PlayerSaveInfo City_Folk = new PlayerSaveInfo
        {
            Bells = 0x1154,
            Debt = 0x1158,
            Savings = 0x115C,
            Patterns = 0x1160,
            PatternSize = 0x880,
            PatternCount = 8,
            TownIdentifier = 0x7EE2,
            TownIdentifierSize = 2,
            TownName = 0x7EE4,  //0x7EF6 = "special" town byte
            TownNameSize = 16,
            Identifier = 0x7EF8,
            IdentifierSize = 2,
            Name = 0x7EFA,
            NameSize = 16,
            HeldItem = 0x7F3A,
            Shirt = 0x7F3C,
            Hat = 0x7F3E,
            FaceItem = 0x7F40,
            Pockets = 0x7F42,
            PocketsCount = 15,
            NookPoints = 0x7FC0, //7FC4 gets written to if 7FC0 is greater
            FaceType = 0x840A,
            HairType = 0x840B,
            HairColor = 0x840C,
            ShoeColor = 0x8416,
            Tan = 0x8418,
            TownPassCardImage = -1,
            Dressers = -1, //Research
            DressersCount = 0,
            DressersSize = 0,
            RegisterDate = -1,
            Shoes = -1,
            Socks = -1,
            Pants = -1,
            EyeColor = -1,
            Encyclopedia = -1, //Research
            FriendCode = -1,
            Bed = -1,
            Birthday = -1, //Research
            LastPlayDate = -1, //Research
            Emotions = -1, //Research
            Catalog = -1,
            Gender = -1, //Research
            InventoryBackground = -1,
            Messages = -1,
            Reset = -1,
            Wetsuit = -1,
            NL_Debt = -1,
            NL_Savings = -1,
            NL_Wallet = -1,
            MeowCoupons = -1,
        };

        public static PlayerSaveInfo New_Leaf = new PlayerSaveInfo
        {
            HairType = 4,
            HairColor = 5,
            FaceType = 6,
            EyeColor = 7,
            Tan = 8,
            Hat = 0xA,
            FaceItem = 0xE,
            Wetsuit = 0x12,
            Shirt = 0x16,
            Pants = 0x1A,
            Socks = 0x1E,
            Shoes = 0x22,
            HeldItem = 0x26,
            Patterns = 0x2C,
            PatternCount = 10,
            PatternSize = 0x870,
            Name = 0x55A8,
            NameSize = 0x12,
            Identifier = 0x55A6,
            IdentifierSize = 2,
            TownName = 0x55BE,
            TownNameSize = 0x12,
            Gender = 0x55BA,
            TownPassCardImage = 0x5724,
            Pockets = 0x6BB0,
            PocketsCount = 16,
            MeowCoupons = -1,
            NookPoints = -1,
            Debt = -1,
            Bells = -1,
            Bed = -1,
            Savings = -1,
            ShoeColor = -1,
        };

        public static PlayerSaveInfo Welcome_Amiibo = new PlayerSaveInfo
        {
            HairType = 4,
            HairColor = 5,
            FaceType = 6,
            EyeColor = 7,
            Tan = 8,
            Hat = 0xA,
            FaceItem = 0xE,
            Wetsuit = 0x12,
            Shirt = 0x16,
            Pants = 0x1A,
            Socks = 0x1E,
            Shoes = 0x22,
            HeldItem = 0x26,
            Patterns = 0x2C,
            PatternCount = 10,
            PatternSize = 0x870,
            Identifier = 0x55A6,
            IdentifierSize = 2,
            Name = 0x55A8,
            NameSize = 0x12,
            TownName = 0x55BE,
            TownNameSize = 0x12,
            Gender = 0x55BA,
            TownPassCardImage = 0x5738,
            Pockets = 0x6BD0,
            PocketsCount = 16,
            NL_Savings = 0x6B8C,
            NL_Debt = 0x6B94,
            NL_Wallet = 0x6F08,
            MeowCoupons = 0x8D1C,
            NookPoints = -1,
            Debt = -1,
            Bells = -1,
            Bed = -1,
            Savings = -1,
            ShoeColor = -1,
        };

        public static PlayerSaveInfo GetPlayerInfo(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return Animal_Crossing;
                case SaveType.Wild_World:
                    return Wild_World;
                case SaveType.City_Folk:
                    return City_Folk;
                case SaveType.New_Leaf:
                    return New_Leaf;
                case SaveType.Welcome_Amiibo:
                    return Welcome_Amiibo;
                default:
                    return new PlayerSaveInfo { Null = true };
            }
        }

        public static string[] AC_Faces = new string[33]
        {
            "Male Eyes w/ Eyelashes",
            "Male Circle Eyes w/ Eyebrows",
            "Male Eyes w/ Purple Bags",
            "Male Circle Dot Eyes",
            "Male Large Oval Eyes",
            "Male Half Oval Eyes",
            "Male Eyes w/ Flushed Cheeks",
            "Male Blue Circle Eyes",
            "Female Full Black Eyes (Pink Hair)",
            "Female Black Squinty Eyes (Purple Hair)",
            "Female Eyes w/ Flushed Cheeks (Brown Hair)",
            "Female Eyes w/ Bags (Blue Hair)",
            "Female Oval Eyes w/ Eyelashes (Pink Hair)",
            "Female Half Oval Eyes (Ginger Hair)",
            "Female Blue Eyes (Dark Blue Hair)",
            "Female Circle Eyes /w Eyelashes (Red Hair)",
            "Male Bee Sting (1)",
            "Male Bee Sting (2)",
            "Male Bee Sting (3)",
            "Male Bee Sting (4)",
            "Male Bee Sting (5)",
            "Male Bee Sting (6)",
            "Male Bee Sting (7)",
            "Male Bee Sting (8)",
            "Female Bee Sting (1)",
            "Female Bee Sting (2)",
            "Female Bee Sting (3)",
            "Female Bee Sting (4)",
            "Female Bee Sting (5)",
            "Female Bee Sting (6)",
            "Female Bee Sting (7)",
            "Female Bee Sting (8)",
            "Lloyd Face"
        };

        public static string[] WW_Faces = new string[16]
        {
            "Male #1 - Brown Eyes w/ Lashes",
            "Male #2 - Black Eyes w/ Brows",
            "Male #3 - Blue Eyes w/ Eyelids",
            "Male #4 - Blue Eyes w/ Small Pupils & Brows",
            "Male #5 - Brown Eyes",
            "Male #6 - Arched Black Eyes",
            "Male #7 - Blue Eyes w/ Rosey Cheeks",
            "Male #8 - Blue Eyes w/ Small Pupils",
            "Female #1 - Black Squinted Eyes w/ Lash",
            "Female #2 - Black Oval Eyes w/ Lash",
            "Female #3 - Blue Eyes w/ Rosey Cheeks",
            "Female #4 - Blue Eyes w/ Eyelids",
            "Female #5 - Green Oval Eyes w/ Lashes",
            "Female #6 - Brown Arched Eyes w/ Lash",
            "Female #7 - Blue Eyes",
            "Female #8 - Green Circle Eyes w/ Lashes"
        };

        public static string[] WW_Hair_Colors = new string[8]
        {
            "Dark Brown",
            "Light Brown",
            "Orange",
            "Blue",
            "Yellow",
            "Green",
            "Pink",
            "White"
        };

        public static string[] WW_Hair_Styles = new string[16]
        {
            "Male #1 - Buzz Cut",
            "Male #2 - Front Cowlick",
            "Male #3 - Long Bangs w/ Left Part",
            "Male #4 - Spiked Up",
            "Male #5 - Middle Part",
            "Male #6 - Inward Spikes",
            "Male #7 - Bowl Cut",
            "Male #8 - Round w/ Spiked Bangs",
            "Female #1 - Rightward Bangs w/ Side Curls",
            "Female #2 - Mop Top w/ Rightward Bangs",
            "Female #3 - Pigtails",
            "Female #4 - Tritails (Three Braids)",
            "Female #5 - Ponytail",
            "Female #6 - Bun",
            "Female #7 - Shortcut",
            "Female #8 - Curly"
        };

        public static string[] CF_Shoe_Colors = new string[20]
        {
            "Yellow & Pink",
            "Red",
            "Orange",
            "Green",
            "Blue",
            "Blue & Purple",
            "Black & Green",
            "Purple",
            "Brown",
            "Pink",
            "Yellow & Green",
            "Red #2",
            "Orange #2",
            "Green #2",
            "Blue #2",
            "White",
            "Black",
            "Purple #2",
            "Brown #2",
            "Pink #2"
        };

        //Set accurate names
        public static string[] CF_Hair_Styles = new string[26]
        {
            "Male #1 Regular",
            "Male #2 Cowlick",
            "Male #3 Covereye",
            "Male #4 Spikey",
            "Male #5 Coiff",
            "Male #6 One Spike",
            "Male #7 Bowlcut, Two Lines",
            "Male #8 Bowlcut",
            "Male #9 Cone",
            "Male #10 Long, Shaggy",
            "Male #11 Parted",
            "Male #12 Short, Shaggy",
            "Male #13 Messy",
            "Female #1 Regular",
            "Female #2 Ears Long",
            "Female #3 Pig Tails",
            "Female #4 Triangle Spikes",
            "Female #5 Pony Tail",
            "Female #6 Pony Stub",
            "Female #7 Ears Short",
            "Female #8 Curls",
            "Female #9 Long, Pony Tail",
            "Female #10 Bangs Down",
            "Female #11 Bangs Up",
            "Female #12 Parted",
            "Female #13 Messy"
        };

        public static string[] NL_Hair_Styles = new string[34]
        {
            "Male #1",
            "Male #2",
            "Male #3",
            "Male #4",
            "Male #5",
            "Male #6",
            "Male #7",
            "Male #8",
            "Male #9",
            "Male #10",
            "Male #11",
            "Male #12",
            "Male #13",
            "Male #14",
            "Male #15",
            "Male #16",
            "Male #17 - Bed Head",
            "Female #1",
            "Female #2",
            "Female #3",
            "Female #4",
            "Female #5",
            "Female #6",
            "Female #7",
            "Female #8",
            "Female #9",
            "Female #10",
            "Female #11",
            "Female #12",
            "Female #13",
            "Female #14",
            "Female #15",
            "Female #16",
            "Female #17 - Bed Head",
        };

        public static string[] NL_Hair_Colors = new string[16]
        {
            "Dark Brown", "Light Brown", "Orange", "Light Blue", "Gold", "Light Green", "Pink", "White",
            "Black", "Auburn", "Red", "Dark Blue", "Blonde", "Dark Green", "Light Purple", "Ash Brown"
        };

        public static string[] NL_Eye_Colors = new string[8]
        {
            "Black", "Brown", "Green", "Gray", "Blue", "Light Blue", "Light Blue", "Light Blue"
        };

        //TODO: Name NL Faces
        public static string[] NL_Male_Faces = new string[12]
        {
            "Male #1",
            "Male #2",
            "Male #3",
            "Male #4",
            "Male #5",
            "Male #6",
            "Male #7",
            "Male #8",
            "Male #9",
            "Male #10",
            "Male #11",
            "Male #12",
        };

        public static string[] NL_Female_Faces = new string[12]
        {
            "Female #1",
            "Female #2",
            "Female #3",
            "Female #4",
            "Female #5",
            "Female #6",
            "Female #7",
            "Female #8",
            "Female #9",
            "Female #10",
            "Female #11",
            "Female #12",
        };
    }

    public class NewPlayer
    {
        public PlayerSaveInfo Offsets;
        public PlayerData Data;
        public int Index;
        public int Offset;
        public bool Exists = false;
        private Save SaveData;

        public NewPlayer(int offset, int idx, Save save)
        {
            SaveData = save;
            Index = idx;
            Offset = offset;
            Offsets = PlayerInfo.GetPlayerInfo(save.Save_Type);
            Exists = SaveData.ReadByte(offset + Offsets.Identifier) != 0 && SaveData.ReadByte(offset + Offsets.Identifier) != 0xFF;
            if (Exists)
            {
                object BoxedData = new PlayerData();
                foreach (var Field in typeof(PlayerSaveInfo).GetFields(BindingFlags.Public | BindingFlags.Instance))
                    if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                        if (typeof(PlayerData).GetField(Field.Name) != null)
                        {
                            if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                            {
                                var Current_Field = typeof(PlayerData).GetField(Field.Name);
                                Type FieldType = Current_Field.FieldType;
                                int DataOffset = Offset + (int)Field.GetValue(Offsets);

                                if (Field.Name == "TownPassCardImage" && (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo))
                                {
                                    typeof(PlayerData).GetField("TownPassCardData").SetValue(BoxedData, SaveData.ReadByteArray(DataOffset, 0x1400));
                                    Current_Field.SetValue(BoxedData,
                                        ImageGeneration.GetTPCImage((byte[])typeof(PlayerData).GetField("TownPassCardData").GetValue(BoxedData)));
                                }
                                else if (FieldType == typeof(byte))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadByte(DataOffset));
                                else if (FieldType == typeof(byte[]) && typeof(PlayerSaveInfo).GetField(Field.Name + "Count") != null)
                                    Current_Field.SetValue(BoxedData, SaveData.ReadByteArray(DataOffset,
                                        (int)typeof(PlayerSaveInfo).GetField(Field.Name + "Count").GetValue(Offsets)));
                                else if (FieldType == typeof(ushort))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt16(DataOffset, SaveData.Is_Big_Endian));
                                else if (FieldType == typeof(ushort[]))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt16Array(DataOffset,
                                        (int)typeof(PlayerSaveInfo).GetField(Field.Name + "Count").GetValue(Offsets), SaveData.Is_Big_Endian));
                                else if (FieldType == typeof(uint))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt32(DataOffset, SaveData.Is_Big_Endian));
                                else if (FieldType == typeof(string))
                                    Current_Field.SetValue(BoxedData, new ACString(SaveData.ReadByteArray(DataOffset,
                                        (int)typeof(PlayerSaveInfo).GetField(Field.Name + "Size").GetValue(Offsets)), SaveData.Save_Type).Trim());
                                else if (FieldType == typeof(Inventory))
                                    if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo)
                                        Current_Field.SetValue(BoxedData, new Inventory(SaveData.ReadUInt32Array(DataOffset,
                                            (int)typeof(PlayerSaveInfo).GetField(Field.Name + "Count").GetValue(Offsets), false)));
                                    else
                                        Current_Field.SetValue(BoxedData, new Inventory(SaveData.ReadUInt16Array(DataOffset,
                                            (int)typeof(PlayerSaveInfo).GetField(Field.Name + "Count").GetValue(Offsets), SaveData.Is_Big_Endian)));
                                else if (FieldType == typeof(Item))
                                    if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo)
                                        Current_Field.SetValue(BoxedData, new Item(SaveData.ReadUInt32(DataOffset, false)));
                                    else
                                        Current_Field.SetValue(BoxedData, new Item(SaveData.ReadUInt16(DataOffset, SaveData.Is_Big_Endian)));
                                else if (FieldType == typeof(NL_Int32))
                                {
                                    uint[] Int_Data = SaveData.ReadUInt32Array(DataOffset, 2);
                                    Current_Field.SetValue(BoxedData, new NL_Int32(Int_Data[0], Int_Data[1]));
                                }
                            }
                        }
                Data = (PlayerData)BoxedData;
                //MessageBox.Show("ID: " + Data.Identifier.ToString("X"));
                if (save.Save_Type == SaveType.Wild_World)
                {
                    byte Condensed_Data = Data.HairColor;
                    Data.HairColor = (byte)(Condensed_Data & 0x0F);
                    Data.Tan = (byte)((Condensed_Data & 0xF0) >> 4); //Has to be 0 - 3
                    Condensed_Data = Data.HairType;
                    Data.FaceType = (byte)(Condensed_Data & 0x0F);
                    Data.HairType = (byte)((Condensed_Data & 0xF0) >> 4);

                    if (Data.Tan > 3)
                        Data.Tan = 0;
                    if (Data.HairColor > 7)
                        Data.HairColor = 0;
                }
                else if (save.Save_Type == SaveType.City_Folk)
                {
                    Data.Tan = (byte)(Data.Tan >> 1); //Not 100% sure about this, but this is what ACToolkit does
                    if (Data.Tan > 7)
                        Data.Tan = 0;
                    if (Data.HairType > 0x19)
                        Data.HairType = 0x19;
                    Data.FaceType = (byte)(Data.FaceType & 0x0F);
                    Data.EyeColor = (byte)(Data.FaceType & 0xF0); //Not actually eye color, just there to hold the upper nibble
                }
                if (Offsets.Patterns > -1)
                {
                    Data.Patterns = new Pattern[Offsets.PatternCount];
                    for (int i = 0; i < Data.Patterns.Length; i++)
                        Data.Patterns[i] = new Pattern(offset + Offsets.Patterns + Offsets.PatternSize * i, save);
                }
            }
        }

        public void Write()
        {
            Type PlayerSaveInfoType = typeof(PlayerSaveInfo);
            Type PlayerDataType = typeof(PlayerData);
            foreach (var Field in PlayerSaveInfoType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                {
                    if (PlayerDataType.GetField(Field.Name) != null)
                    {
                        if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                        {
                            Type FieldType = typeof(PlayerData).GetField(Field.Name).FieldType;
                            int DataOffset = Offset + (int)Field.GetValue(Offsets);
                            //MessageBox.Show("Field Name: " + Field.Name + " | Data Offset: " + DataOffset.ToString("X"));
                            if (Field.Name == "TownPassCardImage" && (SaveData.Save_Type == SaveType.New_Leaf || SaveData.Save_Type == SaveType.Welcome_Amiibo))
                            {
                                SaveData.Write(DataOffset, Data.TownPassCardData);
                            }
                            else if (FieldType == typeof(string))
                            {
                                SaveData.Write(DataOffset, ACString.GetBytes((string)PlayerDataType.GetField(Field.Name).GetValue(Data),
                                    (int)PlayerSaveInfoType.GetField(Field.Name + "Size").GetValue(Offsets)));
                            }
                            else if (FieldType == typeof(byte))
                            {
                                if (SaveData.Save_Type == SaveType.Wild_World)
                                {
                                    if (Field.Name == "HairColor")
                                    {
                                        byte Condensed_Data = (byte)(Data.HairColor & 0x0F); //Remove upper nibble just incase
                                        Condensed_Data += (byte)((Data.Tan & 0x0F) << 4); //Add in tan to the byte
                                        SaveData.Write(DataOffset, Condensed_Data);
                                        //MessageBox.Show("HairColor: " + Condensed_Data.ToString("X2"));
                                    }
                                    else if (Field.Name == "HairType")
                                    {
                                        byte Condensed_Data = (byte)(Data.FaceType & 0x0F);
                                        Condensed_Data += (byte)((Data.HairType & 0x0F) << 4);
                                        SaveData.Write(DataOffset, Condensed_Data);
                                        //MessageBox.Show("HairType: " + Condensed_Data.ToString("X2"));
                                    }
                                    else
                                    {
                                        SaveData.Write(DataOffset, (byte)PlayerDataType.GetField(Field.Name).GetValue(Data));
                                        //MessageBox.Show("Hello! " + Field.Name + " Offset: " + DataOffset.ToString("X"));
                                    }
                                }
                                else if (SaveData.Save_Type == SaveType.City_Folk)
                                {
                                    if (Field.Name == "Tan")
                                    {
                                        byte Shifted_Data = (byte)(Data.Tan << 1); //ACToolkit does this
                                        SaveData.Write(DataOffset, Shifted_Data);
                                    }
                                    else if (Field.Name == "FaceType")
                                    {
                                        SaveData.Write(DataOffset, (byte)(Data.EyeColor + Data.FaceType));
                                    }
                                    else
                                    {
                                        SaveData.Write(DataOffset, (byte)PlayerDataType.GetField(Field.Name).GetValue(Data));
                                    }
                                }
                                else
                                {
                                    SaveData.Write(DataOffset, (byte)PlayerDataType.GetField(Field.Name).GetValue(Data));
                                }
                            }
                            else if (FieldType == typeof(ushort) || FieldType == typeof(uint))
                            {
                                SaveData.Write(DataOffset, (dynamic)PlayerDataType.GetField(Field.Name).GetValue(Data), SaveData.Is_Big_Endian);
                            }
                            else if (FieldType == typeof(Inventory))
                            {
                                if (SaveData.Save_Type == SaveType.New_Leaf || SaveData.Save_Type == SaveType.Welcome_Amiibo)
                                {
                                    uint[] Items = new uint[Offsets.PocketsCount];
                                    for (int i = 0; i < Items.Length; i++)
                                        Items[i] = ItemData.EncodeItem(Data.Pockets.Items[i]);
                                    SaveData.Write(DataOffset, Items);
                                }
                                else
                                {
                                    ushort[] Items = new ushort[Offsets.PocketsCount];
                                    for (int i = 0; i < Items.Length; i++)
                                        Items[i] = Data.Pockets.Items[i].ItemID;
                                    SaveData.Write(DataOffset, Items, SaveData.Is_Big_Endian);
                                }
                            }
                            else if (FieldType == typeof(NL_Int32))
                            {
                                if (SaveData.Save_Type == SaveType.New_Leaf || SaveData.Save_Type == SaveType.Welcome_Amiibo)
                                {
                                    NL_Int32 Encrypted_Int = (NL_Int32)PlayerDataType.GetField(Field.Name).GetValue(Data);
                                    SaveData.Write(DataOffset, Encrypted_Int.Int_1);
                                    SaveData.Write(DataOffset + 4, Encrypted_Int.Int_2);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class Player
    {
        public static Dictionary<byte, string> Male_Faces = new Dictionary<byte, string>()
        {
            {0x00, "Male Eyes w/ Eyelashes" },
            {0x01, "Male Circle Eyes w/ Eyebrows" },
            {0x02, "Male Eyes w/ Purple Bags" },
            {0x03, "Male Circle Dot Eyes" },
            {0x04, "Male Large Oval Eyes" },
            {0x05, "Male Half Oval Eyes" },
            {0x06, "Male Eyes w/ Flushed Cheeks" },
            {0x07, "Male Blue Circle Eyes" },
            {0x08, "Female Full Black Eyes (Pink Hair)" },
            {0x09, "Female Black Squinty Eyes (Purple Hair)" },
            {0x0A, "Female Eyes w/ Flushed Cheeks (Brown Hair)" },
            {0x0B, "Female Eyes w/ Bags (Blue Hair)" },
            {0x0C, "Female Oval Eyes w/ Eyelashes (Pink Hair)" },
            {0x0D, "Female Half Oval Eyes (Ginger Hair)" },
            {0x0E, "Female Blue Eyes (Dark Blue Hair)" },
            {0x0F, "Female Circle Eyes /w Eyelashes (Red Hair)" },
            {0x10, "Male Bee Sting (1)" },
            {0x11, "Male Bee Sting (2)" },
            {0x12, "Male Bee Sting (3)" },
            {0x13, "Male Bee Sting (4)" },
            {0x14, "Male Bee Sting (5)" },
            {0x15, "Male Bee Sting (6)" },
            {0x16, "Male Bee Sting (7)" },
            {0x17, "Male Bee Sting (8)" },
            {0x18, "Female Bee Sting (1)" },
            {0x19, "Female Bee Sting (2)" },
            {0x1A, "Female Bee Sting (3)" },
            {0x1B, "Female Bee Sting (4)" },
            {0x1C, "Female Bee Sting (5)" },
            {0x1D, "Female Bee Sting (6)" },
            {0x1E, "Female Bee Sting (7)" },
            {0x1F, "Female Bee Sting (8)" },
            {0x20, "Lloyd Face" }
        };

        public static Dictionary<byte, string> Female_Faces = new Dictionary<byte, string>() //Can switch these to simple arrays, if wanted.
        {
            {0x00, "Female Full Black Eyes (Pink Hair)" },
            {0x01, "Female Black Squinty Eyes (Purple Hair)" },
            {0x02, "Female Eyes w/ Flushed Cheeks (Brown Hair)" },
            {0x03, "Female Eyes w/ Bags (Blue Hair)" },
            {0x04, "Female Oval Eyes w/ Eyelashes (Pink Hair)" },
            {0x05, "Female Half Oval Eyes (Ginger Hair)" },
            {0x06, "Female Blue Eyes (Dark Blue Hair)" },
            {0x07, "Female Circle Eyes /w Eyelashes (Red Hair)" },
            {0x08, "Male Eyes w/ Eyelashes" },
            {0x09, "Male Circle Eyes w/ Eyebrows" },
            {0x0A, "Male Eyes w/ Purple Bags" },
            {0x0B, "Male Circle Dot Eyes" },
            {0x0C, "Male Large Oval Eyes" },
            {0x0D, "Male Half Oval Eyes" },
            {0x0E, "Male Eyes w/ Flushed Cheeks" },
            {0x0F, "Male Blue Circle Eyes" },
            {0x10, "Female Bee Sting (1)" },
            {0x11, "Female Bee Sting (2)" },
            {0x12, "Female Bee Sting (3)" },
            {0x13, "Female Bee Sting (4)" },
            {0x14, "Female Bee Sting (5)" },
            {0x15, "Female Bee Sting (6)" },
            {0x16, "Female Bee Sting (7)" },
            {0x17, "Female Bee Sting (8)" },
            {0x18, "Male Bee Sting (1)" },
            {0x19, "Male Bee Sting (2)" },
            {0x1A, "Male Bee Sting (3)" },
            {0x1B, "Male Bee Sting (4)" },
            {0x1C, "Male Bee Sting (5)" },
            {0x1D, "Male Bee Sting (6)" },
            {0x1E, "Male Bee Sting (7)" },
            {0x1F, "Male Bee Sting (8)" },
            {0x20, "Lloyd Face" }
        };
        static int Player_Length = 0x2440;

        public int Index = 0;
        public string Name;
        public string Town_Name;
        public Inventory Inventory;
        public uint Bells = 0;
        public uint Debt = 0;
        public Item Held_Item;
        public Item Shirt;
        public Item Inventory_Background;
        //public Item[] Stored_Items;
        public byte Face;
        public byte Gender;
        public byte Tan;
        public uint Identifier;
        public int House_Number = 0;
        public House House;
        public uint Savings = 0;
        public byte[] Bugs_and_Fish_Caught = new byte[11]; //Contains some furntiure set as well
        public Pattern[] Patterns = new Pattern[8];
        public Mail[] Letters = new Mail[10];
        public bool Reset = false;
        public bool Exists = false;
        public ACDate Last_Played_Date;

        public Player(int idx)
        {
            Index = idx;
            Read();
        }

        //Town Identifier is: 0x30 0x??
        //Player Identifier is: 0xF0 0x??
        //Villager Model Identifier is: 0xD0 ??
        //Villager Identifier is: 0xE0 0x??
        //Can Look up resetti values, if wanted.
        //Documented ones: 0x250C | 0xAE8A | 0x85A6

        public void Read()
        {
            int offset = 0x20 + Index * Player_Length;
            Name = DataConverter.ReadString(offset + 0, 8).Trim();
            Town_Name = DataConverter.ReadString(offset + 0x8, 8).Trim();
            Identifier = DataConverter.ReadUInt(offset + 0x10); //First two are player identifier bytes. Second two bytes are town identifier bytes.
            Gender = DataConverter.ReadData(offset + 0x14, 1)[0];
            Face = DataConverter.ReadData(offset + 0x15, 1)[0];
            Inventory = new Inventory(DataConverter.ReadUShortArray(offset + 0x68, 0x1E / 2));
            Bells = DataConverter.ReadUInt(offset + 0x8C);
            Debt = DataConverter.ReadUInt(offset + 0x90);
            Held_Item = new Item(DataConverter.ReadUShort(offset + 0x4A4));
            Inventory_Background = new Item(DataConverter.ReadUShort(offset + 0x1084));
            Shirt = new Item(DataConverter.ReadUShort(offset + 0x1089 + 1)); //Research Patterns used as shirt.
            Reset = DataConverter.ReadUInt(offset + 0x10F4) > 0;
            Savings = DataConverter.ReadUInt(offset + 0x122C);
            Exists = Identifier != 0xFFFFFFFF;

            for (int i = 0; i < 8; i++)
                Patterns[i] = new Pattern(offset + 0x1240 + i * 0x220);
            if (Exists)
            {
                House_Number = GetHouse();
                House = new House(0x9CE8 + (House_Number - 1) * 0x26B0);
                Last_Played_Date = new ACDate(Exists ? DataConverter.ReadDataRaw(House.Offset + 0x2640, 8) : new byte[8]);
            }
        }

        public void WriteName()
        {
            int offset = 0x20 + Index * Player_Length;
            DataConverter.WriteString(offset, Name, 8);
            DataConverter.WriteString(0x9CF8 + (House_Number - 1) * 0x26B0 - 0x10, Name, 8); //House Name
            /*foreach (Villager v in MainForm.Villagers)
                if (v.Exists)
                    for (int i = 0; i < v.Villager_Player_Entries.Length; i++)
                        if (v.Villager_Player_Entries[i] != null && v.Villager_Player_Entries[i].Exists)
                            if (v.Villager_Player_Entries[i].Player_ID == Identifier)
                            {
                                v.Villager_Player_Entries[i].Player_Name = Name;
                                DataConverter.WriteString(v.Offset + 0x10 + (i * 0x138), Name, 8); //Update name in save
                            }*/
        }

        public void Write()
        {
            int offset = 0x20 + Index * Player_Length;
            //DataConverter.WriteString(offset + 0, Name, 8);
            WriteName();
            DataConverter.WriteString(offset + 0x8, Town_Name, 8);
            DataConverter.Write(offset + 0x14, Gender);
            DataConverter.Write(offset + 0x15, Face);
            DataConverter.Write(offset + 0x68, Inventory.GetItemIDs());
            DataConverter.Write(offset + 0x8C, Bells);
            DataConverter.Write(offset + 0x90, Debt);
            DataConverter.Write(offset + 0x4A4, Held_Item.ItemID);
            DataConverter.Write(offset + 0x1084, Inventory_Background.ItemID);
            DataConverter.WriteByteArray(offset + 0x1089, new byte[] { (byte)(Shirt.ItemID & 0xFF), 0x24, (byte)(Shirt.ItemID & 0xFF) }, false);

            //if (Properties.Settings.Default.StopResetti)
                //DataConverter.Write(offset + 0x10F4, 0);

            DataConverter.Write(offset + 0x122C, Savings);

            foreach (Pattern p in Patterns)
                p.Write();
        }

        public int GetHouse()
        {
            for (int i = 0; i < 4; i++)
                if (Identifier != 0xFFFFFFFF && DataConverter.ReadUInt(0x9CF8 + i * 0x26B0) == Identifier)
                    return i + 1;
            return 0;
        }

        public void Fill_Catchables()
        {
            //This will add some items to Nook's Catalog as well (They're stored in binary again for space saving)
            int offset = 0x20 + Index * Player_Length;
            DataConverter.WriteByteArray(offset + 0x1164, new byte[] { 0xFF, 0xFF }, false);
            DataConverter.WriteByteArray(offset + 0x1168, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, false);
            DataConverter.WriteByte(offset + 0x1173, 0xFF);
        }

        public void Fill_Catalog()
        {
            int offset = 0x20 + Index * Player_Length + 0x10F0;
            for (int i = 0; i < 0x4; i++)
                DataConverter.WriteByte(offset + i, 0xFF);
            for (int i = 0; i < 0xB0; i++)
                DataConverter.WriteByte(offset + 0x8 + i, 0xFF);
            for (int i = 0; i < 0x28; i++)
                DataConverter.WriteByte(offset + 0xC4 + i, 0xFF);
        }
    }
}
