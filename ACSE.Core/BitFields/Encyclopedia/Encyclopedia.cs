using System.Collections.Generic;
using ACSE.Core.Players;
using ACSE.Core.Saves;

namespace ACSE.Core.BitFields.Encyclopedia
{
    public static class Encyclopedia
    {
        public static readonly Dictionary<int, byte> DoubutsuNoMoriEncyclopediaBitMap = new Dictionary<int, byte>
        {
            // Fish
            { 0xABC, 0xFF },
            { 0xABD, 0xFF },
            { 0xABE, 0xFF },
            { 0xABF, 0xFF },
            // Insects
            { 0xAC0, 0xFF },
            { 0xAC1, 0xFF },
            { 0xAC2, 0xFF },
            { 0xAC3, 0xFF }
        };

        public static readonly Dictionary<int, byte> AnimalCrossingEncyclopediaBitMap = new Dictionary<int, byte>
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

        public static readonly string[][] AnimalCrossingEncyclopediaNames = {
            new[] { "Evening Cicada", "Brown Cicada", "Bee", "Common Dragonfly", "Red Dragonfly", "Darner Dragonfly", "Banded Dragonfly", "Long Locust" },
            new[] { "", "", "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Purple Butterfly", "Robust Cicada", "Walker Cicada" },
            new[] { "Ant", "Mosquito", "Crucian Carp", "Brook Trout", "Carp", "Koi", "Catfish", "Small Bass" },
            new[] { "Mountain Beetle", "Giant Beetle", "Snail", "Mole Cricket", "Pond Skater", "Bagworm", "Pillbug", "Spider" },
            new[] { "Jewel Beetle", "Longhorn Beetle", "Ladybug", "Spotted Ladybug", "Mantis", "Firefly", "Cockroach", "Saw Stag Beetle" },
            new[] { "Migratory Locust", "Cricket", "Grasshopper", "Bell Cricket", "Pine Cricket", "Drone Beetle", "Dynastid Beetle", "Flat Stag Beetle" },
            new[] { "Popeyed Goldfish", "Coelacanth", "Crawfish", "Frog", "Killifish", "Jellyfish", "Sea Bass", "Red Snapper" },
            new[] { "Salmon", "Goldfish", "Piranha", "Arowana", "Eel", "Freshwater Goby", "Angelfish", "Guppy" },
            new[] { "Bitterling", "Loach", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Large Char", "Rainbow Trout", "Stringfish" },
            new[] { "Bass", "Large Bass", "Bluegill", "Giant Catfish", "Giant Snakehead", "Barbel Steed", "Dace", "Pale Chub" },
            new[] { "Barred Knifejaw", "Arapaima", "", "", "", "", "", "" }
        };

        public static Dictionary<int, byte> DoubutsuNoMoriEPlusEncyclopediaBitMap = new Dictionary<int, byte>
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

        public static readonly string[][] DoubutsuNoMoriEPlusEncyclopediaNames = {
            new[] { "Evening Cicada", "Brown Cicada", "Bee", "Common Dragonfly", "Red Dragonfly", "Darner Dragonfly", "Banded Dragonfly", "Long Locust" },
            new[] { "", "", "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Purple Butterfly", "Robust Cicada", "Walker Cicada" },
            new[] { "Ant", "Mosquito", "Large Butterfly", "Dung Beetle", "Diving Beetle", "Hercules Beetle", "Crab", "Flea" },
            new[] { "Mountain Beetle", "Giant Beetle", "Snail", "Mole Cricket", "Pond Skater", "Bagworm", "Pillbug", "Spider" },
            new[] { "Jewel Beetle", "Longhorn Beetle", "Ladybug", "Spotted Ladybug", "Mantis", "Firefly", "Cockroach", "Saw Stag Beetle" },
            new[] { "Migratory Locust", "Cricket", "Grasshopper", "Bell Cricket", "Pine Cricket", "Drone Beetle", "Dynastid Beetle", "Flat Stag Beetle" },
            new[] { "Salmon", "Goldfish", "Piranha", "Arowana", "Eel", "Freshwater Goby", "Angelfish", "Guppy" },
            new[] { "Bitterling", "Loach", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Large Char", "Rainbow Trout", "Stringfish" },
            new[] { "Bass", "Large Bass", "Bluegill", "Giant Catfish", "Giant Snakehead", "Barbel Steed", "Dace", "Pale Chub" },
            new[] { "Hermit Crab", "Coconut Crab", "Crucian Carp", "Brook Trout", "Carp", "Koi", "Catfish", "Small Bass" },
            new[] { "Sea Horse", "Horse Mackerel", "", "", "", "", "", "" },
            new[] { "Barred Knifejaw", "Arapaima", "Puffer Fish", "Dab", "Squid", "Swordfish", "Flounder", "Octopus" },
            new[] { "Popeyed Goldfish", "Coelacanth", "Crawfish", "Frog", "Killifish", "Jellyfish", "Sea Bass", "Red Snapper" },
        };

        public static readonly Dictionary<int, byte> WildWorldEncyclopediaBitMap = new Dictionary<int, byte>
        {
            { 0x1BDD, 0xFE },
            { 0x1BDE, 0xFF },
            { 0x1BDF, 0xFF },
            { 0x1BE0, 0xFF },
            { 0x1BE1, 0xFF },
            { 0x1BE2, 0xFF },
            { 0x1BE3, 0xFF },
            { 0x1BE4, 0xFF },
            { 0x1BE5, 0xFF },
            { 0x1BE6, 0xFF },
            { 0x1BE7, 0xFF },
            { 0x1BE8, 0xFF },
            { 0x1BE9, 0xFF },
            { 0x1BEA, 0xFF },
            { 0x1BEB, 0x01 }
        };

        public static readonly string[][] WildWorldEncyclopediaNames = {
            new[] { "", "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Peacock", "Monarch", "Emperor", "Agrias Butterfly" },
            new[] { "Birdwing", "Moth", "Oak Silk Moth", "Honeybee", "Bee", "Long Locust", "Migratory Locust", "Mantis" },
            new[] { "Orchid Mantis", "Brown Cicada", "Robust Cicada", "Walker Cicada", "Evening Cicada", "Lantern Fly", "Red Dragonfly", "Darner Dragonfly" },
            new[] { "Banded Dragonfly", "Ant", "Pondskater", "Snail", "Cricket", "Bell Cricket", "Grasshopper", "Mole Cricket" },
            new[] { "Walkingstick", "Ladybug", "Fruit Beetle", "Scarab Beetle", "Dung Beetle", "Goliath Beetle", "Firefly", "Jewel Beetle" },
            new[] { "Longhorn Beetle", "Saw Stag Beetle", "Stag Beetle", "Giant Beetle", "Rainbow Stag", "Dynastid Beetle", "Atlas Beetle", "Elephant Beetle" },
            new[] { "Hercules Beetle", "Flea", "Pill Bug", "Mosquito", "Fly", "Cockroach", "Spider", "Tarantula" },
            new[] { "Scorpion", "Bitterling", "Pale Chub", "Crucian Carp", "Dace", "Barbel Steed", "Carp", "Koi" },
            new[] { "Goldfish", "Popeyed Goldfish", "Killifish", "Crawfish", "Frog", "Freshwater Goby", "Loach", "Catfish" },
            new[] { "Eel", "Giant Snakehead", "Bluegill", "Yellow Perch", "Black Bass", "Pond Smelt", "Sweetfish", "Cherry Salmon" },
            new[] { "Char", "Rainbow Trout", "stringfish", "Salmon", "King Salmon", "Guppy", "Angelfish", "Piranha" },
            new[] { "Arowana", "Dorado", "Gar", "Arapaima", "Sea Butterfly", "Jellyfish", "Seahorse", "Clowfish" },
            new[] { "Zebra Turkeyfish", "Puffer Fish", "Horse Mackerel", "Barred Knifejaw", "Sea Bass", "Red Snapper", "Dab", "Olive Flounder" },
            new[] { "Squid", "Octopus", "Football Fish", "Tuna", "Blue Marlin", "Ocean Sunfish", "Hammerhead Shark", "Shark" },
            new[] { "Coelacanth", "", "", "", "", "", "", "" },
        };

        public static readonly Dictionary<int, byte> CityFolkEncyclopediaBitMap = new Dictionary<int, byte>
        {
            { 0x8465, 0xFF },
            { 0x8466, 0xFF },
            { 0x8467, 0xFF },
            { 0x8468, 0xFF },
            { 0x8469, 0xFF },
            { 0x846A, 0xFF },
            { 0x846B, 0xFF },
            { 0x846C, 0xFF },
            { 0x8471, 0xF0 },
            { 0x8472, 0xFF },
            { 0x8473, 0xFF },
            { 0x8474, 0xFF },
            { 0x8475, 0xFF },
            { 0x8476, 0xFF },
            { 0x8477, 0xFF },
            { 0x8478, 0xFF },
            { 0x8479, 0x0F },
        };

        public static readonly string[][] CityFolkEncyclopediaNames = {
            new[] { "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Peacock", "Monarch", "Emperor", "Agrias Butterfly", "Raja Brooke" },
            new[] { "Birdwing", "Moth", "Oak Silk Moth", "Honeybee", "Bee", "Long Locust", "Migratory Locust", "Mantis" },
            new[] { "Orchid Mantis", "Brown Cicada", "Robust Cicada", "Walker Cicada", "Evening Cicada", "Lantern Fly", "Red Dragonfly", "Darner Dragonfly" },
            new[] { "Banded Dragonfly", "Giant Petaltail", "Ant", "Pondskater", "Diving Beetle", "Snail", "Cricket", "Bell Cricket" },
            new[] { "Grasshopper", "Mole Cricket", "Walking Leaf", "Walkingstick", "Bagworm", "Ladybug", "Violin Beetle", "Longhorn Beetle" },
            new[] { "Dung Beetle", "Firefly", "Fruit Beetle", "Scarab Beetle", "Jewel Beetle", "Miyama Stag", "Saw Stag Beetle", "Giant Beetle" },
            new[] { "Rainbow Stag", "Cyclommatus", "Golden Stag", "Dynastid Beetle", "Atlas Beetle", "Elephant Beetle", "Hercules Beetle", "Goliath Beetle" },
            new[] { "Flea", "Pill Bug", "Mosquito", "Fly", "Centipede", "Spider", "Tarantula", "Scorpion" },
            new[] { "", "", "", "", "Bitterling", "Pale Chub", "Crucian Carp", "Dace" },
            new[] { "Barbel Steed", "Carp", "Koi", "Goldfish", "Popeyed Goldfish", "Killifish", "Crawfish", "Frog" },
            new[] { "Freshwater Goby", "Loach", "Catfish", "Eel", "Giant Snakehead", "Bluegill", "Yellow Perch", "Black Bass" },
            new[] { "Pike", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Char", "Rainbow Trout", "Stringfish", "Salmon" },
            new[] { "King Salmon", "Guppy", "Angelfish", "Neon Tetra", "Piranha", "Arowana", "Dorado", "Gar" },
            new[] { "Arapaima", "Sea Butterfly", "Jellyfish", "Sea Horse", "Clownfish", "Suregonfish", "Butterflyfish", "Napoleonfish" },
            new[] { "Zebra Turkeyfish", "Puffer Fish", "Horse Mackerel", "Barred Knifejaw", "Sea Bass", "Red Snapper", "Dab", "Olive Flounder" },
            new[] { "Squid", "Octopus", "Lobster", "Moray Eel", "Football Fish", "Tuna", "Blue Marlin", "Ray" },
            new[] { "Ocean Sunfish", "Hammerhead Shark", "Shark", "Coelacanth", "", "", "", "" },
        };

        public static readonly Dictionary<int, byte> NewLeafEncyclopediaBitMap = new Dictionary<int, byte>
        {
            { 0x6C50, 0xC0 }, // Insect 
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

        public static string[][] NewLeafEncyclopediaNames = {
            // Insect
            new[] { "", "", "", "", "", "", "Common Butterfly", "Yellow Butterfly" },
            new[] { "Tiger butterfly", "Peacock butterfly", "Monarch Butterfly", "Emperor butterfly", "Agrias butterfly", "Raja B. butterfly", "Birdwing butterfly", "Moth" },
            new[] { "Oak silk moth", "Honeybee", "Bee", "Long locust", "Migraty locust", "Rice grasshoper", "Mantis", "Orchid mantis" },
            new[] { "Brown cicada", "Robust cicada", "Giant cicada", "Walker cicada", "Evening cicada", "Cicada shell", "Lantern fly", "Red dragonfly" },
            new[] { "Darner dragonfly", "Banded dragonfly", "Peltaltail dragonfly", "Ant", "Pondskater", "Diving beetle", "Stinkbug", "Snail" },
            new[] { "Cricket", "Bell cricket", "Grasshopper", "Mole cricket", "Walking leaf", "Walking stick", "Bagworm", "Ladybug" },
            new[] { "Violin beetle", "Longhorn beetle", "Tiger beetle", "Dung beetle", "Wharf roach", "Hermit crab", "Firefly", "Fruit beetle" },
            new[] { "Scarab beetle", "Jewel beetle", "Miyama stag", "Saw stag", "Giant stag", "Rainbow stag", "Cyclommatus stag", "Golden stag" },
            new[] { "Horned dynastid", "Horned atlas", "Horned elephant", "Horned hercules", "Goliath beetle", "Flea", "Pill bug", "Mosquito" },
            new[] { "Fly", "House centipede", "Centipede", "Spider", "Tarantula", "Scorpion", "", "" },
            // Fish
            new[] { "", "", "", "", "", "", "", "Bitterling" },
            new[] { "Pale chub", "Crucian carp", "Dace", "Barbel steed", "Carp", "Koi", "Goldfish", "Pop-eyed goldfish" },
            new[] { "Killifish", "Crawfish", "Soft-shelled turtle", "Tadpole", "Frog", "Freshwater goby", "Loach", "Catfish" },
            new[] { "Eel", "Giant snakehead", "Bluegill", "Yellow perch", "Black bass", "Pike", "Pond smelt", "Sweetfish" },
            new[] { "Cherry salmon", "Char", "Rainbow trout", "Stringfish", "Salmon", "King salmon", "Mitten crab", "Guppy" },
            new[] { "Nibble fish", "Angelfish", "Neon tetra", "Piranha", "Arowana", "Dorado", "Gar", "Arapaima" },
            new[] { "Saddled bichir", "Sea butterfly", "Sea horse", "Clown fish", "Suregeonfish", "Butterfly fish", "Napeleonfish", "Zebra turkeyfish" },
            new[] { "Blowfish", "Puffer fish", "Horse mackerel", "Barred knifejaw", "Sea bass", "Red snapper", "Dab", "Olive flounder" },
            new[] { "Squid", "Moray eel", "Ribbon eel", "Football fish", "Tuna", "Blue marlin", "Giant trevally", "Ray" },
            new[] { "Ocean sunfish", "Hammerhead shark", "Shark", "Saw shark", "Whale shark", "Oarfish", "Coelacanth", "" },
            // Sea Food
            new[] { "", "", "", "Seaweed", "Sea grapes", "Sea urchin", "Acorn barnacle", "Oyster" },
            new[] { "Turban shell", "Abalone", "Ear shell", "Clam", "Peral oyster", "Scallop", "Sea anemone", "Sea star" },
            new[] { "Sea cucumber", "Sea slug", "Flatworm", "Mantis shrimp", "Sweet shrimp", "Tiger prawn", "Spiny lobster", "Lobster" },
            new[] { "Snow crab", "Horsehair crab", "Red king crab", "Spider crab", "Octopus", "Spotted garden eel", "Chambered nautilus", "Horseshoe crab" },
            new[] { "Giant isopod", "", "", "", "", "", "", "" },
};

        public static readonly Dictionary<int, byte> WelcomeAmiiboEncyclopediaBitMap = new Dictionary<int, byte>
        {
            { 0x6C71, 0xC0 }, // Insect 
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

        public static readonly string[][] WelcomeAmiiboEncyclopediaNames = {
            // Insect
            new[] { "", "", "", "", "", "", "Common Butterfly", "Yellow Butterfly" },
            new[] { "Tiger butterfly", "Peacock butterfly", "Monarch Butterfly", "Emperor butterfly", "Agrias butterfly", "Raja B. butterfly", "Birdwing butterfly", "Moth" },
            new[] { "Oak silk moth", "Honeybee", "Bee", "Long locust", "Migraty locust", "Rice grasshoper", "Mantis", "Orchid mantis" },
            new[] { "Brown cicada", "Robust cicada", "Giant cicada", "Walker cicada", "Evening cicada", "Cicada shell", "Lantern fly", "Red dragonfly" },
            new[] { "Darner dragonfly", "Banded dragonfly", "Peltaltail dragonfly", "Ant", "Pondskater", "Diving beetle", "Stinkbug", "Snail" },
            new[] { "Cricket", "Bell cricket", "Grasshopper", "Mole cricket", "Walking leaf", "Walking stick", "Bagworm", "Ladybug" },
            new[] { "Violin beetle", "Longhorn beetle", "Tiger beetle", "Dung beetle", "Wharf roach", "Hermit crab", "Firefly", "Fruit beetle" },
            new[] { "Scarab beetle", "Jewel beetle", "Miyama stag", "Saw stag", "Giant stag", "Rainbow stag", "Cyclommatus stag", "Golden stag" },
            new[] { "Horned dynastid", "Horned atlas", "Horned elephant", "Horned hercules", "Goliath beetle", "Flea", "Pill bug", "Mosquito" },
            new[] { "Fly", "House centipede", "Centipede", "Spider", "Tarantula", "Scorpion", "", "" },
            // Fish
            new[] { "", "Bitterling", "Pale chub", "Crucian carp", "Dace", "Barbel steed", "Carp", "Koi" },
            new[] { "Goldfish", "Pop-eyed goldfish", "Killifish", "Crawfish", "Soft-shelled turtule", "Tadpole", "Frog", "Freshwater goby" },
            new[] { "Loach", "Catfish", "Eel", "Giant snakehead", "Bluegill", "Yellow perch", "Black bass", "Pike" },
            new[] { "Pond smelt", "Sweetfish", "Cherry salmon", "Char", "Rainbow trout", "Stringfish", "Salmon", "King Salmon" },
            new[] { "Mitten crab", "Guppy", "Nibble fish", "Angelfish", "Neon tetra", "Piranha", "Arowana", "Dorado" },
            new[] { "Gar", "Arapaima", "Saddled bichir", "Sea butterfly", "Sea horse", "Clown fish", "Surgeonfish", "Butterfly fish" },
            new[] { "Napoleonfish", "Zebra turkeyfish", "Blowfish", "Puffer fish", "Horse mackerel", "Barred knifejaw", "Sea bass", "Red snapper" },
            new[] { "Dab", "Olive flounder", "Squid", "Moray eel", "Ribbon eel", "Football fish", "Tuna", "Blue marlin" },
            new[] { "Giant trevally", "Ray", "Ocean sunfish", "Hammerhead shark", "Shark", "Saw shark", "Whale shark", "Oarfish" },
            new[] { "Coelacanth", "", "", "", "", "Seaweed", "Sea grapes", "Sea urchin" },
            // Sea Food
            new[] { "Acorn barnacle", "Oyster", "Turban shell", "Abalone", "Ear shell", "Clam", "Pearl oyster", "Scallop" },
            new[] { "Sea anemone", "Sea star", "Sea cucumber", "Sea slug", "Flatworm", "Mantis shrimp", "Swwet shrimp", "Tiger prawn" },
            new[] { "Spiny lobster", "Lobster", "Snow crab", "Horsehair", "Red king crab", "Spider crab", "Octopus", "Spotted garden eel" },
            new[] { "Chambered nautilus", "Horseshoe crab", "Giant isopod", "", "", "", "", "" },
        };

        private static Dictionary<int, byte> GetBitMap(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.DoubutsuNoMori:
                case SaveType.DongwuSenlin:
                    return DoubutsuNoMoriEncyclopediaBitMap;
                case SaveType.AnimalCrossing:
                    return AnimalCrossingEncyclopediaBitMap;
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return DoubutsuNoMoriEPlusEncyclopediaBitMap;
                case SaveType.WildWorld:
                    return WildWorldEncyclopediaBitMap;
                case SaveType.CityFolk:
                    return CityFolkEncyclopediaBitMap;
                case SaveType.NewLeaf:
                    return NewLeafEncyclopediaBitMap;
                case SaveType.WelcomeAmiibo:
                    return WelcomeAmiiboEncyclopediaBitMap;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Clears a Player's encyclopedia.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">Player whose encyclopedia will be cleared</param>
        public static void ClearEncylopedia(Save saveFile, Player player)
        {
            var currentBitMap = GetBitMap(saveFile.SaveType);
            if (currentBitMap == null) return;
            foreach (var bitValue in currentBitMap)
                saveFile.Write(player.Offset + bitValue.Key, (byte)(saveFile.ReadByte(player.Offset + bitValue.Key) & ~bitValue.Value));
        }

        /// <summary>
        /// Fills a Player's encyclopedia.
        /// </summary>
        /// <param name="saveFile">Current Save File</param>
        /// <param name="player">Player whose encyclopedia will be filled</param>
        public static void FillEncyclopedia(Save saveFile, Player player)
        {
            var currentBitMap = GetBitMap(saveFile.SaveType);
            if (currentBitMap == null) return;
            foreach (var bitValue in currentBitMap)
                saveFile.Write(player.Offset + bitValue.Key, (byte)(saveFile.ReadByte(player.Offset + bitValue.Key) | bitValue.Value));
        }
    }
}
