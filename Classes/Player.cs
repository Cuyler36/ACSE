using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

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
        public int ResetCount;
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
        public int BirthdaySize;
        public int TownIdentifier;
        public int TownIdentifierSize;
        public int NL_Wallet;
        public int NL_Savings;
        public int NL_Debt;
        public int MeowCoupons;
        public int Island_Medals;
    }

    [Serializable]
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
        //public Mail[] Letters;
        public Pattern[] Patterns;
        public byte[] Emotions; //Add Emotions class?
        public ushort NookPoints;
        public ACDate LastPlayDate;
        public ACDate RegisterDate;
        public ACDate Birthday;
        public bool Reset;
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
        public NL_Int32 Island_Medals;
    }

    public static class PlayerInfo
    {
        public static PlayerSaveInfo Doubutsu_no_Mori = new PlayerSaveInfo
        {
            Name = 0,
            NameSize = 6,
            TownName = 6,
            TownNameSize = 6,
            Identifier = 0xC,
            IdentifierSize = 2,
            TownIdentifier = 0xE,
            TownIdentifierSize = 2,
            Gender = 0x10, //
            FaceType = 0x11, //
            Pockets = 0x14,
            PocketsCount = 15,
            Bells = 0x38,
            Debt = 0x3C, 
            HeldItem = 0x3EC,
            InventoryBackground = 0xA72,
            Shirt = 0xA78,
            Birthday = -1,
            BirthdaySize = 2,
            Reset = -1, //
            ResetSize = 8,
            Savings = -1, // Does DnM have savings?
            Patterns = -1, // No Patterns in DnM
            PatternCount = 8,
            PatternSize = 0x220, //Actual Size is 0x200, with the first 0x20 bytes for Name, then for palette & padding?
            Tan = -1, // Confirm in-game (I don't think DnM has tans)
            TownPassCardImage = -1,
            HairType = -1,
            Bed = -1,
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
            Island_Medals = -1,
            ResetCount = -1,
        };

        public static PlayerSaveInfo Doubutsu_no_Mori_Plus = new PlayerSaveInfo
        {
            Name = 0,
            NameSize = 6,
            TownName = 6,
            TownNameSize = 6,
            Identifier = 0xC,
            IdentifierSize = 2,
            TownIdentifier = 0xE,
            TownIdentifierSize = 2,
            Gender = 0x14, //
            FaceType = 0x15, //
            Pockets = 0x64,
            PocketsCount = 15,
            Bells = 0x88, //
            Debt = 0x8C, // 
            HeldItem = 0x43E, //
            InventoryBackground = 0xAC2,
            Shirt = 0xAC8,
            Birthday = 0xAE2,
            BirthdaySize = 2,
            Reset = 0x10F4, //
            ResetSize = 4,
            Savings = 0x11B4,
            Patterns = 0xC40,
            PatternCount = 8,
            PatternSize = 0x220, //Actual Size is 0x200, with the first 0x20 bytes for Name, then for palette & padding?
            Tan = 0x2348, // Confirm in-game
            TownPassCardImage = -1,
            HairType = -1,
            Bed = -1,
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
            Island_Medals = -1,
            ResetCount = -1,
        };

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
            ResetCount = 0x16,
            Pockets = 0x68,
            PocketsCount = 15,
            Bells = 0x8C,
            Debt = 0x90,
            HeldItem = 0x4A4,
            InventoryBackground = 0x1084,
            Shirt = 0x108A, //If the shirt is a pattern, Player+0x1089 will be the pattern number. Otherwise, it's just the lower byte of the shirt id.
            Birthday = 0x10A6,
            BirthdaySize = 2,
            Reset = 0x10F4,
            ResetSize = 4,
            Savings = 0x122C,
            Patterns = 0x1240,
            PatternCount = 8,
            PatternSize = 0x220, //Actual Size is 0x200, with the first 0x20 bytes for Name, then for palette & padding?
            Tan = 0x23C8,
            TownPassCardImage = -1,
            HairType = -1,
            Bed = -1,
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
            Island_Medals = -1,
        };

        public static PlayerSaveInfo Doubutsu_no_Mori_e_Plus = new PlayerSaveInfo
        {
            Name = 0,
            NameSize = 6,
            TownName = 6,
            TownNameSize = 6,
            Identifier = 0xC,
            IdentifierSize = 2,
            TownIdentifier = 0xE,
            TownIdentifierSize = 2,
            Gender = 0x10, //
            FaceType = 0x11, //
            Pockets = 0x64,
            PocketsCount = 15,
            Bells = 0x94,
            Debt = 0x98,
            HeldItem = 0x874,
            InventoryBackground = 0xFF0,
            Shirt = 0xFF6,
            Birthday = 0x1018,
            BirthdaySize = 2,
            Reset = 0x1064,
            ResetSize = 4,
            Savings = 0x11B4,
            Patterns = 0x11C0,
            PatternCount = 8,
            PatternSize = 0x220, //Actual Size is 0x200, with the first 0x20 bytes for Name, then for palette & padding?
            Tan = 0x2348, // Confirm in-game
            TownPassCardImage = -1,
            HairType = -1,
            Bed = -1,
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
            Island_Medals = -1,
            ResetCount = -1,
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
            Island_Medals = -1,
            ResetCount = -1,
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
            Island_Medals = -1,
            ResetCount = -1,
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
            Birthday = 0x55D4,
            BirthdaySize = 2,
            RegisterDate = 0x55D6,
            RegisterDateSize = 0x4,
            TownPassCardImage = 0x5724,
            NL_Savings = 0x6B6C,
            NL_Debt = 0x6B74,
            Island_Medals = 0x6B7C,
            Pockets = 0x6BB0,
            NL_Wallet = 0x6E38,
            PocketsCount = 16,
            NookPoints = -1,
            Debt = -1,
            Bells = -1,
            Bed = -1,
            Savings = -1,
            ShoeColor = -1,
            MeowCoupons = -1,
            ResetCount = -1,
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
            Birthday = 0x55D4,
            BirthdaySize = 0x2,
            RegisterDate = 0x55D6,
            RegisterDateSize = 0x4,
            TownPassCardImage = 0x5738,
            Pockets = 0x6BD0,
            PocketsCount = 16,
            NL_Savings = 0x6B8C,
            NL_Debt = 0x6B94,
            Island_Medals = 0x6B9C,
            NL_Wallet = 0x6F08,
            MeowCoupons = 0x8D1C,
            NookPoints = -1,
            Debt = -1,
            Bells = -1,
            Bed = -1,
            Savings = -1,
            ShoeColor = -1,
            ResetCount = -1,
        };

        public static PlayerSaveInfo GetPlayerInfo(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                    return Doubutsu_no_Mori;
                case SaveType.Doubutsu_no_Mori_Plus:
                    return Doubutsu_no_Mori_Plus;
                case SaveType.Animal_Crossing:
                    return Animal_Crossing;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return Doubutsu_no_Mori_e_Plus;
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

        public static string[] AC_Faces = new string[64]
        {
            "Male #1 Eyes w/ Eyelashes",
            "Male #2 Circle Eyes w/ Eyebrows",
            "Male #3 Eyes w/ Purple Bags",
            "Male #4 Circle Dot Eyes",
            "Male #5 Large Oval Eyes",
            "Male #6 Half Oval Eyes",
            "Male #7 Eyes w/ Flushed Cheeks",
            "Male #8 Blue Circle Eyes",
            "Female #1 Full Black Eyes (Pink Hair)",
            "Female #2 Black Squinty Eyes (Purple Hair)",
            "Female #3 Eyes w/ Flushed Cheeks (Brown Hair)",
            "Female #4 Eyes w/ Bags (Blue Hair)",
            "Female #5 Oval Eyes w/ Eyelashes (Pink Hair)",
            "Female #6 Half Oval Eyes (Ginger Hair)",
            "Female #7 Blue Eyes (Dark Blue Hair)",
            "Female #8 Circle Eyes /w Eyelashes (Red Hair)",
            "Male #1 Bee Sting",
            "Male #2 Bee Sting",
            "Male #3 Bee Sting",
            "Male #4 Bee Sting",
            "Male #5 Bee Sting",
            "Male #6 Bee Sting",
            "Male #7 Bee Sting",
            "Male #8 Bee Sting",
            "Female #1 Bee Sting",
            "Female #2 Bee Sting",
            "Female #3 Bee Sting",
            "Female #4 Bee Sting",
            "Female #5 Bee Sting",
            "Female #6 Bee Sting",
            "Female #7 Bee Sting",
            "Female #8 Bee Sting",
            "Lloyd Face [Male #1]",
            "Lloyd Face [Male #2]",
            "Lloyd Face [Male #3]",
            "Lloyd Face [Male #4]",
            "Lloyd Face [Male #5]",
            "Lloyd Face [Male #6]",
            "Lloyd Face [Male #7]",
            "Lloyd Face [Male #8]",
            "Lloyd Face [Female #1]",
            "Lloyd Face [Female #2]",
            "Lloyd Face [Female #3]",
            "Lloyd Face [Female #4]",
            "Lloyd Face [Female #5]",
            "Lloyd Face [Female #6]",
            "Lloyd Face [Female #7]",
            "Lloyd Face [Female #8]",
            "Lloyd Face (Bee Sting) [Male #1]",
            "Lloyd Face (Bee Sting) [Male #2]",
            "Lloyd Face (Bee Sting) [Male #3]",
            "Lloyd Face (Bee Sting) [Male #4]",
            "Lloyd Face (Bee Sting) [Male #5]",
            "Lloyd Face (Bee Sting) [Male #6]",
            "Lloyd Face (Bee Sting) [Male #7]",
            "Lloyd Face (Bee Sting) [Male #8]",
            "Lloyd Face (Bee Sting) [Female #1]",
            "Lloyd Face (Bee Sting) [Female #2]",
            "Lloyd Face (Bee Sting) [Female #3]",
            "Lloyd Face (Bee Sting) [Female #4]",
            "Lloyd Face (Bee Sting) [Female #5]",
            "Lloyd Face (Bee Sting) [Female #6]",
            "Lloyd Face (Bee Sting) [Female #7]",
            "Lloyd Face (Bee Sting) [Female #8]"
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

        public static uint[] NL_Hair_Color_Values = new uint[16]
        {
            0xFF593A38, 0xFF935929, 0xFFEF572E, 0xFF41A6DC, 0xFFFFE779, 0xFF8BCF62, 0xFFEE798B, 0xFFFFF8DE,
            0xFF171806, 0xFF550601, 0xFFBB0C07, 0xFF001449, 0xFFDEA70F, 0xFF015A22, 0xFFAD75BC, 0xFF7A795A
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
        public House House;
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
                Type PlayerDataType = typeof(PlayerData);
                Type PlayerSaveInfoType = typeof(PlayerSaveInfo);
                object BoxedData = new PlayerData();
                foreach (var Field in PlayerSaveInfoType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                        if (PlayerDataType.GetField(Field.Name) != null)
                        {
                            if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                            {
                                var Current_Field = PlayerDataType.GetField(Field.Name);
                                Type FieldType = Current_Field.FieldType;
                                int DataOffset = Offset + (int)Field.GetValue(Offsets);

                                if (Field.Name == "TownPassCardImage" && (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo))
                                {
                                    PlayerDataType.GetField("TownPassCardData").SetValue(BoxedData, SaveData.ReadByteArray(DataOffset, 0x1400));
                                    Current_Field.SetValue(BoxedData,
                                        ImageGeneration.GetTPCImage((byte[])PlayerDataType.GetField("TownPassCardData").GetValue(BoxedData)));
                                }
                                else if (Field.Name == "Reset" && save.Game_System == SaveGeneration.GCN)
                                {
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt32(DataOffset, SaveData.Is_Big_Endian) != 0);
                                }
                                else if (FieldType == typeof(byte))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadByte(DataOffset));
                                else if (FieldType == typeof(byte[]) && PlayerSaveInfoType.GetField(Field.Name + "Count") != null)
                                    Current_Field.SetValue(BoxedData, SaveData.ReadByteArray(DataOffset,
                                        (int)PlayerSaveInfoType.GetField(Field.Name + "Count").GetValue(Offsets)));
                                else if (FieldType == typeof(ushort))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt16(DataOffset, SaveData.Is_Big_Endian));
                                else if (FieldType == typeof(ushort[]))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt16Array(DataOffset,
                                        (int)PlayerSaveInfoType.GetField(Field.Name + "Count").GetValue(Offsets), SaveData.Is_Big_Endian));
                                else if (FieldType == typeof(uint))
                                    Current_Field.SetValue(BoxedData, SaveData.ReadUInt32(DataOffset, SaveData.Is_Big_Endian));
                                else if (FieldType == typeof(string))
                                    Current_Field.SetValue(BoxedData, new ACString(SaveData.ReadByteArray(DataOffset,
                                        (int)PlayerSaveInfoType.GetField(Field.Name + "Size").GetValue(Offsets)), SaveData.Save_Type).Trim());
                                else if (FieldType == typeof(Inventory))
                                    if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo)
                                        Current_Field.SetValue(BoxedData, new Inventory(SaveData.ReadUInt32Array(DataOffset,
                                            (int)PlayerSaveInfoType.GetField(Field.Name + "Count").GetValue(Offsets), false)));
                                    else
                                        Current_Field.SetValue(BoxedData, new Inventory(SaveData.ReadUInt16Array(DataOffset,
                                            (int)PlayerSaveInfoType.GetField(Field.Name + "Count").GetValue(Offsets), SaveData.Is_Big_Endian)));
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
                                else if (FieldType == typeof(ACDate) && DataOffset > 0)
                                {
                                    Current_Field.SetValue(BoxedData, new ACDate(SaveData.ReadByteArray(DataOffset,
                                        (int)PlayerSaveInfoType.GetField(Field.Name + "Size").GetValue(Offsets))));
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

                // Get the Player's House
                House = HouseInfo.GetHouse(this, save.Save_Type);
                Console.WriteLine(string.Format("Player {0}'s house = {1}", Index, House));
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
                            else if (Field.Name == "Reset" && SaveData.Game_System == SaveGeneration.GCN)
                            {
                                SaveData.Write(DataOffset, Data.Reset ? (uint)0x250C : (uint)0 ,SaveData.Is_Big_Endian);
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
                            else if (FieldType == typeof(Item))
                            {
                                Item Item = (Item)PlayerDataType.GetField(Field.Name).GetValue(Data);
                                if (SaveData.Game_System == SaveGeneration.N3DS)
                                {
                                    SaveData.Write(DataOffset, Item.ToUInt32());
                                }
                                else
                                {
                                    SaveData.Write(DataOffset, Item.ItemID, SaveData.Is_Big_Endian);
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
                            else if (FieldType == typeof(ACDate) && SaveData.Game_System == SaveGeneration.GCN)
                            {
                                SaveData.Write(DataOffset, ((ACDate)PlayerDataType.GetField(Field.Name).GetValue(Data)).ToBytes());
                            }
                        }
                    }
                }
            }
        }

        public void WriteToFile(string Path)
        {
            if (Exists && !string.IsNullOrEmpty(Path))
            {
                /*using (BinaryWriter Writer = new BinaryWriter(File.Create(Path)))
                {
                    using (XmlSerializer Serializer = new XmlSerializer(Data))
                    {
                        Writer.Write("ACSEPlayerSave"); // File Identifier
                        Writer.Write((int)SaveData.Save_Type); // Save Type
                        Writer.Write(Index);
                    }
                }*/
            }
        }
    }
}
