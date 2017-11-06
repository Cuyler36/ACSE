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

        public static Dictionary<int, byte> New_Leaf_Encyclopedia_Bit_Map = new Dictionary<int, byte>
        {
            { 0x6C50, 0xC0 }, // Insect -- can be equal to FF even if C0 unlock everything in this byte
            { 0x6C51, 0xFF },
            { 0x6C52, 0xFF },
            { 0x6C53, 0xFF },
            { 0x6C54, 0xFF },
            { 0x6C55, 0xFF },
            { 0x6C56, 0xFF },
            { 0x6C57, 0xFF },
            { 0x6C58, 0xFF },
            { 0x6C59, 0x3F },
            { 0x6C5A, 0x80 }, // Fish
            { 0x6C5B, 0xFF },
            { 0x6C5C, 0xFF },
            { 0x6C5D, 0xFF },
            { 0x6C5E, 0xFF },
            { 0x6C5F, 0xFF },
            { 0x6C60, 0xFF },
            { 0x6C61, 0xFF },
            { 0x6C62, 0xFF },
            { 0x6C63, 0x7F }, 
            { 0x6C64, 0xF8 }, // Sea Food 
            { 0x6C65, 0xFF },
            { 0x6C66, 0xFF },
            { 0x6C67, 0xFF },
            { 0x6C68, 0x01 },
        };

        public static string[][] New_Leaf_Encyclopedia_Names = new string[25][]
{
            // Insect
            new string[8] { "", "", "", "", "", "", "Common Butterfly", "Yellow Butterfly" },
            new string[8] { "Tiger butterfly", "Peacock butterfly", "Monarch Butterfly", "Emperor butterfly", "Agrias butterfly", "Raja B. butterfly", "Birdwing butterfly", "Moth" },
            new string[8] { "Oak silk moth", "Honeybee", "Bee", "Long locust", "Migraty locust", "Rice grasshoper", "Mantis", "Orchid mantis" },
            new string[8] { "Brown cicada", "Robust cicada", "Giant cicada", "Walker cicada", "Evening cicada", "Cicada shell", "Lantern fly", "Red dragonfly" },
            new string[8] { "Darner dragonfly", "Banded dragonfly", "Peltaltail dragonfly", "Ant", "Pondskater", "Diving beetle", "Stinkbug", "Snail" },
            new string[8] { "Cricket", "Bell cricket", "Grasshopper", "Mole cricket", "Walking leaf", "Walking stick", "Bagworm", "Ladybug" },
            new string[8] { "Violin beetle", "Longhorn beetle", "Tiger beetle", "Dung beetle", "Wharf roach", "Hermit crab", "Firefly", "Fruit beetle" },
            new string[8] { "Scarab beetle", "Jewel beetle", "Miyama stag", "Saw stag", "Giant stag", "Rainbow stag", "Cyclommatus stag", "Golden stag" },
            new string[8] { "Horned dynastid", "Horned atlas", "Horned elephant", "Horned hercules", "Goliath beetle", "Flea", "Pill bug", "Mosquito" },
            new string[8] { "Fly", "House centipede", "Centipede", "Spider", "Tarantula", "Scorpion", "", "" },
            // Fish
            new string[8] { "", "", "", "", "", "", "", "Bitterling" },
            new string[8] { "Pale chub", "Crucian carp", "Dace", "Barbel steed", "Carp", "Koi", "Goldfish", "Pop-eyed goldfish" },
            new string[8] { "Killifish", "Crawfish", "Soft-shelled turtle", "Tadpole", "Frog", "Freshwater goby", "Loach", "Catfish" },
            new string[8] { "Eel", "Giant snakehead", "Bluegill", "Yellow perch", "Black bass", "Pike", "Pond smelt", "Sweetfish" },
            new string[8] { "Cherry salmon", "Char", "Rainbow trout", "Stringfish", "Salmon", "King salmon", "Mitten crab", "Guppy" },
            new string[8] { "Nibble fish", "Angelfish", "Neon tetra", "Piranha", "Arowana", "Dorado", "Gar", "Arapaima" },
            new string[8] { "Saddled bichir", "Sea butterfly", "Sea horse", "Clown fish", "Suregeonfish", "Butterfly fish", "Napeleonfish", "Zebra turkeyfish" },
            new string[8] { "Blowfish", "Puffer fish", "Horse mackerel", "Barred knifejaw", "Sea bass", "Red snapper", "Dab", "Olive flounder" },
            new string[8] { "Squid", "Moray eel", "Ribbon eel", "Football fish", "Tuna", "Blue marlin", "Giant trevally", "Ray" },
            new string[8] { "Ocean sunfish", "Hammerhead shark", "Shark", "Saw shark", "Whale shark", "Oarfish", "Coelacanth", "" },
            // Sea Food
            new string[8] { "", "", "", "Seaweed", "Sea grapes", "Sea urchin", "Acorn barnacle", "Oyster" },
            new string[8] { "Turban shell", "Abalone", "Ear shell", "Clam", "Peral oyster", "Scallop", "Sea anemone", "Sea star" },
            new string[8] { "Sea cucumber", "Sea slug", "Flatworm", "Mantis shrimp", "Sweet shrimp", "Tiger prawn", "Spiny lobster", "Lobster" },
            new string[8] { "Snow crab", "Horsehair crab", "Red king crab", "Spider crab", "Octopus", "Spotted garden eel", "Chambered nautilus", "Horseshoe crab" },
            new string[8] { "Giant isopod", "", "", "", "", "", "", "" },
};

        public static Dictionary<int, byte> Welcome_Amiibo_Encyclopedia_Bit_Map = new Dictionary<int, byte>
        {
            { 0x6C71, 0xC0 }, // Insect -- can be equal to FF even if C0 unlock everything in this byte
            { 0x6C72, 0xFF },
            { 0x6C73, 0xFF },
            { 0x6C74, 0xFF },
            { 0x6C75, 0xFF },
            { 0x6C76, 0xFF },
            { 0x6C77, 0xFF },
            { 0x6C78, 0xFF },
            { 0x6C79, 0xFF },
            { 0x6C7A, 0x3F },
            { 0x6C7C, 0xFE }, // Fish
            { 0x6C7D, 0xFF },
            { 0x6C7E, 0xFF },
            { 0x6C7F, 0xFF },
            { 0x6C80, 0xFF },
            { 0x6C81, 0xFF },
            { 0x6C82, 0xFF },
            { 0x6C83, 0xFF },
            { 0x6C84, 0xFF },
            { 0x6C85, 0xD1 }, // Sea Food 
            { 0x6C86, 0xFF },
            { 0x6C87, 0xFF },
            { 0x6C88, 0xFF },
            { 0x6C89, 0x07 },
        };

        public static string[][] Welcome_Amiibo_Encyclopedia_Names = new string[24][]
        {
            // Insect
            new string[8] { "", "", "", "", "", "", "Common Butterfly", "Yellow Butterfly" },
            new string[8] { "Tiger butterfly", "Peacock butterfly", "Monarch Butterfly", "Emperor butterfly", "Agrias butterfly", "Raja B. butterfly", "Birdwing butterfly", "Moth" },
            new string[8] { "Oak silk moth", "Honeybee", "Bee", "Long locust", "Migraty locust", "Rice grasshoper", "Mantis", "Orchid mantis" },
            new string[8] { "Brown cicada", "Robust cicada", "Giant cicada", "Walker cicada", "Evening cicada", "Cicada shell", "Lantern fly", "Red dragonfly" },
            new string[8] { "Darner dragonfly", "Banded dragonfly", "Peltaltail dragonfly", "Ant", "Pondskater", "Diving beetle", "Stinkbug", "Snail" },
            new string[8] { "Cricket", "Bell cricket", "Grasshopper", "Mole cricket", "Walking leaf", "Walking stick", "Bagworm", "Ladybug" },
            new string[8] { "Violin beetle", "Longhorn beetle", "Tiger beetle", "Dung beetle", "Wharf roach", "Hermit crab", "Firefly", "Fruit beetle" },
            new string[8] { "Scarab beetle", "Jewel beetle", "Miyama stag", "Saw stag", "Giant stag", "Rainbow stag", "Cyclommatus stag", "Golden stag" },
            new string[8] { "Horned dynastid", "Horned atlas", "Horned elephant", "Horned hercules", "Goliath beetle", "Flea", "Pill bug", "Mosquito" },
            new string[8] { "Fly", "House centipede", "Centipede", "Spider", "Tarantula", "Scorpion", "", "" },
            // Fish
            new string[8] { "", "Bitterling", "Pale chub", "Crucian carp", "Dace", "Barbel steed", "Carp", "Koi" },
            new string[8] { "Goldfish", "Pop-eyed goldfish", "Killifish", "Crawfish", "Soft-shelled turtule", "Tadpole", "Frog", "Freshwater goby" },
            new string[8] { "Loach", "Catfish", "Eel", "Giant snakehead", "Bluegill", "Yellow perch", "Black bass", "Pike" },
            new string[8] { "Pond smelt", "Sweetfish", "Cherry salmon", "Char", "Rainbow trout", "Stringfish", "Salmon", "King Salmon" },
            new string[8] { "Mitten crab", "Guppy", "Nibble fish", "Angelfish", "Neon tetra", "Piranha", "Arowana", "Dorado" },
            new string[8] { "Gar", "Arapaima", "Saddled bichir", "Sea butterfly", "Sea horse", "Clown fish", "Surgeonfish", "Butterfly fish" },
            new string[8] { "Napoleonfish", "Zebra turkeyfish", "Blowfish", "Puffer fish", "Horse mackerel", "Barred knifejaw", "Sea bass", "Red snapper" },
            new string[8] { "Dab", "Olive flounder", "Squid", "Moray eel", "Ribbon eel", "Football fish", "Tuna", "Blue marlin" },
            new string[8] { "Giant trevally", "Ray", "Ocean sunfish", "Hammerhead shark", "Shark", "Saw shark", "Whale shark", "Oarfish" },
            new string[8] { "Coelacanth", "", "", "", "", "Seaweed", "Sea grapes", "Sea urchin" },
            // Sea Food
            new string[8] { "Acorn barnacle", "Oyster", "Turban shell", "Abalone", "Ear shell", "Clam", "Pearl oyster", "Scallop" },
            new string[8] { "Sea anemone", "Sea star", "Sea cucumber", "Sea slug", "Flatworm", "Mantis shrimp", "Swwet shrimp", "Tiger prawn" },
            new string[8] { "Spiny lobster", "Lobster", "Snow crab", "Horsehair", "Red king crab", "Spider crab", "Octopus", "Spotted garden eel" },
            new string[8] { "Chambered nautilus", "Horseshoe crab", "Giant isopod", "", "", "", "", "" },
        };

        private static Dictionary<int, byte> GetBitMap(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return Animal_Crossing_Encyclopedia_Bit_Map;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return Doubutsu_no_Mori_e_Plus_Encyclopedia_Bit_Map;
                case SaveType.New_Leaf:
                    return New_Leaf_Encyclopedia_Bit_Map;
                case SaveType.Welcome_Amiibo:
                    return Welcome_Amiibo_Encyclopedia_Bit_Map;
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
