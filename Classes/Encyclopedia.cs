using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    public static class Encyclopedia
    {
        public static Dictionary<int, byte> Animal_Crossing_Encyclopedia_Bit_Map = new Dictionary<int, byte>
        {
            { 0x1164, 0xFF },
            { 0x1165, 0xFC },
            { 0x1168, 0xFF },
            { 0x1169, 0xFF },
            { 0x116A, 0xFF },
            { 0x116B, 0xFF },
            { 0x116C, 0xFF },
            { 0x116D, 0xFF },
            { 0x116E, 0xFF },
            { 0x116F, 0xFF },
            { 0x1173, 0x03 }
        };

        public static string[][] Animal_Crossing_Encyclopedia_Names = new string[11][]
        {
            new string[8] { "Evening Cicada", "Brown Cicada", "Bee", "Common Dragonfly", "Red Dragonfly", "Darner Dragonfly", "Banded Dragonfly", "Long Locust" },
            new string[8] { "", "", "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Purple Butterfly", "Robust Cicada", "Walker Cicada" },
            new string[8] { "Ant", "Mosquito", "Crucian Carp", "Brook Trout", "Carp", "Koi", "Catfish", "Small Bass" },
            new string[8] { "Mountain Beetle", "Giant Beetle", "Snail", "Mole Cricket", "Pond Skater", "Bagworm", "Pillbug", "Spider" },
            new string[8] { "Jewel Beetle", "Longhorn Beetle", "Ladybug", "Spotted Ladybug", "Mantis", "Firefly", "Cockroach", "Saw Stag Beetle" },
            new string[8] { "Migratory Locust", "Cricket", "Grasshopper", "Bell Cricket", "Pine Cricket", "Drone Beetle", "Dynastid Beetle", "Flat Stag Beetle" },
            new string[8] { "Popeyed Goldfish", "Coelacanth", "Crawfish", "Frog", "Killifish", "Jellyfish", "Sea Bass", "Red Snapper" },
            new string[8] { "Salmon", "Goldfish", "Piranha", "Arowana", "Eel", "Freshwater Goby", "Angelfish", "Guppy" },
            new string[8] { "Bitterling", "Loach", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Large Char", "Rainbow Trout", "Stringfish" },
            new string[8] { "Bass", "Large Bass", "Bluegill", "Giant Catfish", "Giant Snakehead", "Barbel Steed", "Dace", "Pale Chub" },
            new string[8] { "Barred Knifejaw", "Arapaima", "", "", "", "", "", "" }
        };

        public static Dictionary<int, byte> Doubutsu_no_Mori_e_Plus_Encyclopedia_Bit_Map = new Dictionary<int, byte>
        {
            { 0x10E4, 0xFF },
            { 0x10E5, 0xFC },
            { 0x10E8, 0xFF },
            { 0x10E9, 0xFF },
            { 0x10EA, 0xFF },
            { 0x10EB, 0xFF },
            { 0x10EC, 0xFF },
            { 0x10ED, 0xFF },
            { 0x10EE, 0xFF },
            { 0x10EF, 0xFF },
            { 0x10F1, 0x03 },
            { 0x10F2, 0xFF },
            { 0x10F3, 0xFF }
        };

        public static string[][] Doubutsu_no_Mori_e_Plus_Encyclopedia_Names = new string[13][]
        {
            new string[8] { "Evening Cicada", "Brown Cicada", "Bee", "Common Dragonfly", "Red Dragonfly", "Darner Dragonfly", "Banded Dragonfly", "Long Locust" },
            new string[8] { "", "", "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Purple Butterfly", "Robust Cicada", "Walker Cicada" },
            new string[8] { "Ant", "Mosquito", "Large Butterfly", "Dung Beetle", "Diving Beetle", "Hercules Beetle", "Crab", "Flea" },
            new string[8] { "Mountain Beetle", "Giant Beetle", "Snail", "Mole Cricket", "Pond Skater", "Bagworm", "Pillbug", "Spider" },
            new string[8] { "Jewel Beetle", "Longhorn Beetle", "Ladybug", "Spotted Ladybug", "Mantis", "Firefly", "Cockroach", "Saw Stag Beetle" },
            new string[8] { "Migratory Locust", "Cricket", "Grasshopper", "Bell Cricket", "Pine Cricket", "Drone Beetle", "Dynastid Beetle", "Flat Stag Beetle" },
            new string[8] { "Salmon", "Goldfish", "Piranha", "Arowana", "Eel", "Freshwater Goby", "Angelfish", "Guppy" },
            new string[8] { "Bitterling", "Loach", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Large Char", "Rainbow Trout", "Stringfish" },
            new string[8] { "Bass", "Large Bass", "Bluegill", "Giant Catfish", "Giant Snakehead", "Barbel Steed", "Dace", "Pale Chub" },
            new string[8] { "Hermit Crab", "Coconut Crab", "Crucian Carp", "Brook Trout", "Carp", "Koi", "Catfish", "Small Bass" },
            new string[8] { "Sea Horse", "Horse Mackerel", "", "", "", "", "", "" },
            new string[8] { "Barred Knifejaw", "Arapaima", "Puffer Fish", "Dab", "Squid", "Swordfish", "Flounder", "Octopus" },
            new string[8] { "Popeyed Goldfish", "Coelacanth", "Crawfish", "Frog", "Killifish", "Jellyfish", "Sea Bass", "Red Snapper" },
        };

        private static Dictionary<int, byte> GetBitMap(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return Animal_Crossing_Encyclopedia_Bit_Map;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return Doubutsu_no_Mori_e_Plus_Encyclopedia_Bit_Map;
                default:
                    throw new NotImplementedException(string.Format("Encyclopedia Bit Map for save type {0} has not been implemented!", Save_Type.ToString()));
            }
        }

        public static void FillEncyclopedia(Save Save_File, NewPlayer Player)
        {
            Dictionary<int, byte> Current_Bit_Map = GetBitMap(Save_File.Save_Type);
            foreach (KeyValuePair<int, byte> Bit_Value in Current_Bit_Map)
                Save_File.Write(Player.Offset + Bit_Value.Key, (byte)(Save_File.ReadByte(Player.Offset + Bit_Value.Key) | Bit_Value.Value));
        }
    }
}
