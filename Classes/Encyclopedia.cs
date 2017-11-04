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
            new string[8] { "Jewel Beetle", "Longhorn Beetle", "Ladybug", "Spotted Ladybug", "Mantis", "Firefly", "Cockroach", "Swa Stag Beetle" },
            new string[8] { "Migratory Locust", "Cricket", "Grasshopper", "Bell Cricket", "Pine Cricket", "Drone Beetle", "Dynastid Beetle", "Flat Stag Beetle" },
            new string[8] { "Popeyed Goldfish", "Coelacanth", "Crawfish", "Frog", "Killifish", "Jellyfish", "Sea Bass", "Red Snapper" },
            new string[8] { "Salmon", "Goldfish", "Piranha", "Arowana", "Eel", "Freshwater Goby", "Angelfish", "Guppy" },
            new string[8] { "Bitterling", "Loach", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Large Char", "Rainbow Trout", "Stringfish" },
            new string[8] { "Bass", "Large Bass", "Bluegill", "Giant Catfish", "Giant Snakehead", "Barbel Steed", "Dace", "Pale Chub" },
            new string[8] { "Barred Knifejaw", "Arapaima", "", "", "", "", "", "" }
        };
    }
}
