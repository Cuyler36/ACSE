using System.Collections.Generic;
using System.Linq;

namespace ACSE
{
    public class ItemData
    {
        public static List<KeyValuePair<ushort, string>> ItemDatabase = new List<KeyValuePair<ushort, string>>();

        public static readonly string[] CfBuildingNames = {
            "Player House #1",
            "Player House #2",
            "Player House #3",
            "Player House #4",
            "Unknown #1",
            "Unknown #2",
            "Unknown #3",
            "Unknown #4",
            "Villager House #1",
            "Villager House #2",
            "Villager House #3",
            "Villager House #4",
            "Villager House #5",
            "Villager House #6",
            "Villager House #7",
            "Villager House #8",
            "Villager House #9",
            "Villager House #10",
            "Town Hall",
            "Gate",
            "Nook's Store",
            "Able Sister's Store",
            "Museum",
            "Bulletin Board",
            "New Year's Sign",
            "Chip's Stand",
            "Nat's Stand",
            "Lighthouse",
            "Windmill",
            "Fountain",
            "Harvest Festival Table #1",
            "Harvest Festival Table #2",
            "Gulliver's UFO",
            "Pavé's Sign",
            "Bus Stop",
            "Sign",
            //0x24+??
        };

        public static readonly Dictionary<byte, string> NlBuildingNames = new Dictionary<byte, string>
        {
            {0x00, "Player 1's House"},
            {0x01, "Player 2's House"},
            {0x02, "Player 3's House"},
            {0x03, "Player 4's House"},
            {0x04, "Mailbox Only (Player 1)"},
            {0x05, "Mailbox Only (Player 2)"},
            {0x06, "Mailbox Only (Player 3)"},
            {0x07, "Mailbox Only (Player 4)"},
            {0x08, "Villager 1's House"},
            {0x09, "Villager 2's House"},
            {0x0A, "Villager 3's House"},
            {0x0B, "Villager 4's House"},
            {0x0C, "Villager 5's House"},
            {0x0D, "Villager 6's House"},
            {0x0E, "Villager 7's House"},
            {0x0F, "Villager 8's House"},
            {0x10, "Villager 9's House"},
            {0x11, "Villager 10's House"},
            {0x4C, "Modern Police Station"},
            {0x4D, "Classic Police Station"},
            {0x4E, "Café"},
            {0x4F, "Reset Center"},
            {0x50, "Classic Town Hall"},
            {0x51, "Zen Town Hall"},
            {0x52, "Fairy-Tale Town Hall"},
            {0x53, "Modern Town Hall"},
            {0x54, "Classic Train Station"},
            {0x55, "Zen Train Station"},
            {0x56, "Fairy-Tale Train Station"},
            {0x57, "Modern Train Station"},
            {0x58, "Recycle Shop"},
            {0x59, "Town Plaza"},
            {0x5A, "Railroad Crossing Arms"},
            {0x5B, "Bulletin Board"},
            {0x5C, "Island Storage Bin"},
            {0x5D, "Dock"},
            {0x5E, "Katrina's Tent"},
            {0x5F, "Camper's Tent"},
            {0x60, "Redd's Tent"},
            {0x61, "Chip's Tent"},
            {0x62, "Nat's Tent"},
            {0x63, "Franklin's Table"},
            {0x64, "Pavé's Dance Stage"},
            {0x65, "Countdown Board"},
            {0x66, "Redd's Stand"},
            {0x67, "Dream World Bed"},
            {0x69, "Island Hut"},
            {0x6A, "Lloid"},
            {0x6B, "Face Cutout Standee - New Years Day"},
            {0x6C, "Face Cutout Standee - Japanese Rite"},
            {0x6D, "Face Cutout Standee - Sky Night"},
            {0x6E, "Face Cutout Standee - Groundhog Day"},
            {0x6F, "Face Cutout Standee - Bean Throwing Festival (Setsubun)"},
            {0x70, "Face Cutout Standee - Japanese Wedding"},
            {0x71, "Face Cutout Standee - Shamrock Day"},
            {0x72, "Face Cutout Standee - Garden Kids"},
            {0x73, "Face Cutout Standee - Nature Day"},
            {0x74, "Face Cutout Standee - Carp Banner"},
            {0x75, "Face Cutout Standee - Japanese School"},
            {0x76, "Face Cutout Standee - Summer Solstice"},
            {0x77, "Face Cutout Standee - Starcrossed Day (Tanabata)"},
            {0x78, "Face Cutout Standee - Obon"},
            {0x79, "Face Cutout Standee - Labor Day"},
            {0x7A, "Face Cutout Standee - Tsukimi"},
            {0x7B, "Face Cutout Standee - Autumn Moon"},
            {0x7C, "Face Cutout Standee - Explorer's Day"},
            {0x7D, "Face Cutout Standee - Toy Day"},
            {0x7E, "Face Cutout Standee - Winter Solstice"},
            {0x8C, "Main Street Lamp"},
            {0x8D, "Main Street Bench (Vertical)"},
            {0x8E, "Cobblestone Bridge (N-S)"},
            {0x8F, "Cobblestone Bridge (SW-NE)"},
            {0x90, "Cobblestone Bridge (NW-SE)"},
            {0x91, "Cobblestone Bridge (W-E)"},
            {0x92, "Suspension Bridge (N-S)"},
            {0x93, "Suspension Bridge (SW-NE)"},
            {0x94, "Suspension Bridge (NW-SE)"},
            {0x95, "Suspension Bridge (W-E)"},
            {0x96, "Brick Bridge (N-S)"},
            {0x97, "Brick Bridge (SW-NE)"},
            {0x98, "Brick Bridge (NW-SE)"},
            {0x99, "Brick Bridge (W-E)"},
            {0x9A, "Modern Bridge (N-S)"},
            {0x9B, "Modern Bridge (SW-NE)"},
            {0x9C, "Modern Bridge (NW-SE)"},
            {0x9D, "Modern Bridge (W-E)"},
            {0x9E, "Wooden Bridge (N-S)"},
            {0x9F, "Wooden Bridge (SW-NE)"},
            {0xA0, "Wooden Bridge (NW-SE)"},
            {0xA1, "Wooden Bridge (W-E)"},
            {0xA2, "Fairy-Tale Bridge (N-S)"},
            {0xA3, "Fairy-Tale Bridge (SW-NE)"},
            {0xA4, "Fairy-Tale Bridge (NW-SE)"},
            {0xA5, "Fairy-Tale Bridge (W-E)"},
            {0xA6, "Yellow Bench"},
            {0xA7, "Blue Bench"},
            {0xA8, "Wood Bench"},
            {0xA9, "Metal Bench"},
            {0xAA, "Log Bench"},
            {0xAB, "Modern Bench"},
            {0xAC, "Fairy-Tale Bench"},
            {0xAD, "Zen Bench"},
            {0xAE, "Flower Bed"},
            {0xAF, "Drinking Fountain"},
            {0xB0, "Instrument Shelter"},
            {0xB1, "Sandbox"},
            {0xB2, "Garbage Can"},
            {0xB3, "Pile Of Pipes"},
            {0xB4, "Water Well"},
            {0xB5, "Fountain"},
            {0xB6, "Tire Toy"},
            {0xB7, "Jungle Gym"},
            {0xB8, "Park Clock"},
            {0xB9, "Modern Clock"},
            {0xBA, "Fairy-Tale Clock"},
            {0xBB, "Zen Clock"},
            {0xBC, "Street Lamp"},
            {0xBD, "Round Streetlight"},
            {0xBE, "Streetlight"},
            {0xBF, "Modern Streetlight"},
            {0xC0, "Fairy-Tale Streetlight"},
            {0xC1, "Zen Streetlight"},
            {0xC2, "Balloon Arch"},
            {0xC3, "Flower Arch"},
            {0xC4, "Campsite"},
            {0xC5, "Picnic Blanket"},
            {0xC6, "Hammock"},
            {0xC7, "Fire Pit"},
            {0xC8, "Camping Cot"},
            {0xC9, "Outdoor Chair"},
            {0xCA, "Torch"},
            {0xCB, "Zen Garden"},
            {0xCC, "Hot Spring"},
            {0xCD, "Geyser"},
            {0xCE, "Statue Fountain"},
            {0xCF, "Stone Tablet"},
            {0xD0, "Water Pump"},
            {0xD1, "Wisteria Trellis"},
            {0xD2, "Bell"},
            {0xD3, "Zen Bell"},
            {0xD4, "Scarecrow"},
            {0xD5, "Rack Of Rice"},
            {0xD6, "Fence"},
            {0xD7, "Bus Stop"},
            {0xD8, "Fire Hydrant"},
            {0xD9, "Traffic Signal"},
            {0xDA, "Custom-Design Sign"},
            {0xDB, "Face-Cutout Standee"},
            {0xDC, "Caution Sign"},
            {0xDD, "Do-Not-Enter Sign"},
            {0xDE, "Yield Sign"},
            {0xDF, "Cube Sculpture"},
            {0xE0, "Archway Sculpture"},
            {0xE1, "Chair Sculpture"},
            {0xE2, "Illuminated Heart"},
            {0xE3, "Illuminated Arch"},
            {0xE4, "Illuminated Clock"},
            {0xE5, "Illuminated Tree"},
            {0xE6, "Stadium Light"},
            {0xE7, "Video Screen"},
            {0xE8, "Drilling Rig"},
            {0xE9, "Parabolic Antenna"},
            {0xEA, "Solar Panel"},
            {0xEB, "Wind Turbine"},
            {0xEC, "Windmill"},
            {0xED, "Lighthouse"},
            {0xEE, "Tower"},
            {0xEF, "Stonehenge"},
            {0xF0, "Totem Pole"},
            {0xF1, "Moai Statue"},
            {0xF2, "Pyramid"},
            {0xF3, "Sphinx"},
            {0xF4, "Round Topiary"},
            {0xF5, "Square Topiary"},
            {0xF6, "Tulip Topiary"},
            {0xF7, "Flower Clock"},
            {0xF8, "No Building"},
        };

        public static readonly Dictionary<byte, string> WaBuildingNames = new Dictionary<byte, string>
        {
            {0x00, "Player 1's House"},
            {0x01, "Player 2's House"},
            {0x02, "Player 3's House"},
            {0x03, "Player 4's House"},
            {0x04, "Mailbox Only (Player 1)"},
            {0x05, "Mailbox Only (Player 2)"},
            {0x06, "Mailbox Only (Player 3)"},
            {0x07, "Mailbox Only (Player 4)"},
            {0x08, "Villager 1's House"},
            {0x09, "Villager 2's House"},
            {0x0A, "Villager 3's House"},
            {0x0B, "Villager 4's House"},
            {0x0C, "Villager 5's House"},
            {0x0D, "Villager 6's House"},
            {0x0E, "Villager 7's House"},
            {0x0F, "Villager 8's House"},
            {0x10, "Villager 9's House"},
            {0x11, "Villager 10's House"},
            {0x4C, "Modern Police Station"},
            {0x4D, "Classic Police Station"},
            {0x4E, "Café"},
            {0x4F, "Reset Center"},
            {0x50, "Classic Town Hall"},
            {0x51, "Zen Town Hall"},
            {0x52, "Fairy-Tale Town Hall"},
            {0x53, "Modern Town Hall"},
            {0x54, "Classic Train Station"},
            {0x55, "Zen Train Station"},
            {0x56, "Fairy-Tale Train Station"},
            {0x57, "Modern Train Station"},
            {0x58, "Recycle Shop"},
            {0x59, "Town Plaza"},
            {0x5A, "Railroad Crossing Arms"},
            {0x5B, "Bulletin Board"},
            {0x5C, "Island Storage Bin"},
            {0x5D, "Dock"},
            {0x5E, "Katrina's Tent"},
            {0x5F, "Camper's Tent"},
            {0x60, "Redd's Tent"},
            {0x61, "Chip's Tent"},
            {0x62, "Nat's Tent"},
            {0x63, "Franklin's Table"},
            {0x64, "Pavé's Dance Stage"},
            {0x65, "Countdown Board"},
            {0x66, "Redd's Stand"},
            {0x67, "Dream World Bed"},
            {0x69, "Island Hut"},
            {0x6A, "Lloid"},
            {0x6B, "Face Cutout Standee - New Years Day"},
            {0x6C, "Face Cutout Standee - Japanese Rite"},
            {0x6D, "Face Cutout Standee - Sky Night"},
            {0x6E, "Face Cutout Standee - Groundhog Day"},
            {0x6F, "Face Cutout Standee - Bean Throwing Festival (Setsubun)"},
            {0x70, "Face Cutout Standee - Japanese Wedding"},
            {0x71, "Face Cutout Standee - Shamrock Day"},
            {0x72, "Face Cutout Standee - Garden Kids"},
            {0x73, "Face Cutout Standee - Nature Day"},
            {0x74, "Face Cutout Standee - Carp Banner"},
            {0x75, "Face Cutout Standee - Japanese School"},
            {0x76, "Face Cutout Standee - Summer Solstice"},
            {0x77, "Face Cutout Standee - Starcrossed Day (Tanabata)"},
            {0x78, "Face Cutout Standee - Obon"},
            {0x79, "Face Cutout Standee - Labor Day"},
            {0x7A, "Face Cutout Standee - Tsukimi"},
            {0x7B, "Face Cutout Standee - Autumn Moon"},
            {0x7C, "Face Cutout Standee - Explorer's Day"},
            {0x7D, "Face Cutout Standee - Toy Day"},
            {0x7E, "Face Cutout Standee - Winter Solstice"},
            {0x8E, "Main Street Lamp"},
            {0x8F, "Main Street Bench (Vertical)"},
            {0x90, "Cobblestone Bridge (N-S)"},
            {0x91, "Cobblestone Bridge (SW-NE)"},
            {0x92, "Cobblestone Bridge (NW-SE)"},
            {0x93, "Cobblestone Bridge (W-E)"},
            {0x94, "Suspension Bridge (N-S)"},
            {0x95, "Suspension Bridge (SW-NE)"},
            {0x96, "Suspension Bridge (NW-SE)"},
            {0x97, "Suspension Bridge (W-E)"},
            {0x98, "Brick Bridge (N-S)"},
            {0x99, "Brick Bridge (SW-NE)"},
            {0x9A, "Brick Bridge (NW-SE)"},
            {0x9B, "Brick Bridge (W-E)"},
            {0x9C, "Modern Bridge (N-S)"},
            {0x9D, "Modern Bridge (SW-NE)"},
            {0x9E, "Modern Bridge (NW-SE)"},
            {0x9F, "Modern Bridge (W-E)"},
            {0xA0, "Wooden Bridge (N-S)"},
            {0xA1, "Wooden Bridge (SW-NE)"},
            {0xA2, "Wooden Bridge (NW-SE)"},
            {0xA3, "Wooden Bridge (W-E)"},
            {0xA4, "Fairy-Tale Bridge (N-S)"},
            {0xA5, "Fairy-Tale Bridge (SW-NE)"},
            {0xA6, "Fairy-Tale Bridge (NW-SE)"},
            {0xA7, "Fairy-Tale Bridge (W-E)"},
            {0xA8, "Yellow Bench"},
            {0xA9, "Blue Bench"},
            {0xAA, "Wood Bench"},
            {0xAB, "Metal Bench"},
            {0xAC, "Log Bench"},
            {0xAD, "Modern Bench"},
            {0xAE, "Fairy-Tale Bench"},
            {0xAF, "Zen Bench"},
            {0xB0, "Flower Bed"},
            {0xB1, "Drinking Fountain"},
            {0xB2, "Instrument Shelter"},
            {0xB3, "Sandbox"},
            {0xB4, "Garbage Can"},
            {0xB5, "Pile Of Pipes"},
            {0xB6, "Water Well"},
            {0xB7, "Fountain"},
            {0xB8, "Tire Toy"},
            {0xB9, "Jungle Gym"},
            {0xBA, "Park Clock"},
            {0xBB, "Modern Clock"},
            {0xBC, "Fairy-Tale Clock"},
            {0xBD, "Zen Clock"},
            {0xBE, "Street Lamp"},
            {0xBF, "Round Streetlight"},
            {0xC0, "Streetlight"},
            {0xC1, "Modern Streetlight"},
            {0xC2, "Fairy-Tale Streetlight"},
            {0xC3, "Zen Streetlight"},
            {0xC4, "Balloon Arch"},
            {0xC5, "Flower Arch"},
            {0xC6, "Campsite"},
            {0xC7, "Picnic Blanket"},
            {0xC8, "Hammock"},
            {0xC9, "Fire Pit"},
            {0xCA, "Camping Cot"},
            {0xCB, "Outdoor Chair"},
            {0xCC, "Torch"},
            {0xCD, "Zen Garden"},
            {0xCE, "Hot Spring"},
            {0xCF, "Geyser"},
            {0xD0, "Statue Fountain"},
            {0xD1, "Stone Tablet"},
            {0xD2, "Water Pump"},
            {0xD3, "Wisteria Trellis"},
            {0xD4, "Bell"},
            {0xD5, "Zen Bell"},
            {0xD6, "Scarecrow"},
            {0xD7, "Rack Of Rice"},
            {0xD8, "Fence"},
            {0xD9, "Bus Stop"},
            {0xDA, "Fire Hydrant"},
            {0xDB, "Traffic Signal"},
            {0xDC, "Custom-Design Sign"},
            {0xDD, "Face-Cutout Standee"},
            {0xDE, "Caution Sign"},
            {0xDF, "Do-Not-Enter Sign"},
            {0xE0, "Yield Sign"},
            {0xE1, "Cube Sculpture"},
            {0xE2, "Archway Sculpture"},
            {0xE3, "Chair Sculpture"},
            {0xE4, "Illuminated Heart"},
            {0xE5, "Illuminated Arch"},
            {0xE6, "Illuminated Clock"},
            {0xE7, "Illuminated Tree"},
            {0xE8, "Stadium Light"},
            {0xE9, "Video Screen"},
            {0xEA, "Drilling Rig"},
            {0xEB, "Parabolic Antenna"},
            {0xEC, "Solar Panel"},
            {0xED, "Wind Turbine"},
            {0xEE, "Windmill"},
            {0xEF, "Lighthouse"},
            {0xF0, "Tower"},
            {0xF1, "Stonehenge"},
            {0xF2, "Totem Pole"},
            {0xF3, "Moai Statue"},
            {0xF4, "Pyramid"},
            {0xF5, "Sphinx"},
            {0xF6, "Round Topiary"},
            {0xF7, "Square Topiary"},
            {0xF8, "Tulip Topiary"},
            {0xF9, "Flower Clock"},
            {0xFC, "No Building"},
        };

        public static int GetBuildingCount(Building[] buildings, SaveType saveType)
        {
            var noBuildingId = saveType == SaveType.New_Leaf ? 0xF8 : 0xFC;
            return buildings.Count(b => b.Exists && b.Id != noBuildingId);
        }

        public static string GetItemType(ushort id, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                // TODO: DnM+, and DnMe+ need their own cases
                case SaveType.Animal_Forest:
                    if (id == 0)
                        return "Empty";
                    else if (id == 0xFFFF)
                        return "Occupied";
                    else if (id >= 0x8 && id <= 0xA)
                        return "Weed";
                    else if (id >= 0x845 && id <= 0x84D)
                        return "Flower";
                    else if ((id >= 0x2100 && id <= 0x2103) || id == 0x005C) //0x005C = Glowing Money/Shovel spot
                        return "Money";
                    else if (id >= 0x63 && id <= 0x68)
                        return "Rock";
                    else if (id >= 0x6A && id <= 0x6F)
                        return "Money Rock";
                    else if (id >= 0x900 && id <= 0x920)
                        return "Signboard";
                    else if (id >= 0x2514 && id <= 0x251B)
                        return "Shell";
                    else if (id >= 0x2A00 && id <= 0x2A36)
                        return "Song";
                    else if (id >= 0x2B00 && id <= 0x2B0F)
                        return "Diary";
                    else if (id >= 0x2000 && id <= 0x20FF)
                        return "Paper";
                    else if (id >= 0x2F00 && id <= 0x2F03)
                        return "Turnip";
                    else if ((id >= 0x2800 && id <= 0x2804) || id == 0x2807)
                        return "Fruit";
                    else if ((id >= 0x2300 && id <= 0x2327) || (id >= 0x2D00 && id <= 0x2D27))
                        return "Catchable";
                    else if (id >= 0x2503 && id <= 0x250C)
                        return "Quest Item";
                    else if (id >= 0x250E && id <= 0x2510)
                        return "Trash";
                    else if ((id >= 0x2523 && id <= 0x2530) || (id >= 0x2900 && id <= 0x290A) || (id >= 0x2805 && id <= 0x2806))
                        return "Item";
                    else if (id >= 0x2C00 && id <= 0x2C5F)
                        return "Raffle Ticket";
                    else if ((id >= 0x2600 && id <= 0x2642) || (id >= 0x2700 && id <= 0x2742))
                        return "Wall/Floor";
                    else if (id >= 0x2400 && id <= 0x24FF)
                        return "Clothes";
                    else if (id >= 0x15B0 && id <= 0x17A8)
                        return "Gyroids";
                    else if (id == 0x2511)
                        return "Fossil";
                    else if ((id >= 0x2200 && id <= 0x225B) || id == 0x251E)
                        return "Tool";  //0x251E = Signboard (not a 'tool', but it's still classified as one)
                    else if ((id >= 0x1 && id <= 0x4) || (id >= 0x005E && id <= 0x0060) || id == 0x69 || (id >= 0x0070 && id <= 0x0082) || (id >= 0x0800 && id <= 0x0869))
                        return "Tree";
                    else if (id >= 0x4000 && id < 0x5000)
                        return "HouseObject";
                    else if ((id >= 0x5 && id <= 0x7) || (id >= 0xB && id <= 0x10) || (id >= 0x5000 && id <= 0xB000) || id >= 0xFE00)
                        return "Building";
                    else if ((id >= 0x1000 && id <= 0x15AC) || (id >= 0x17AC && id <= 0x1FFC) || (id >= 0x3000 && id <= 0x33C4)) // || ID >= 0xFE20
                        return "Furniture";
                    else
                        return "Unknown";
                case SaveType.Wild_World when id == 0xFFF1:
                    return "Empty";
                case SaveType.Wild_World when id == 0xF030:
                    return "Occupied";
                case SaveType.Wild_World when id <= 0x1C:
                    return "Flower";
                case SaveType.Wild_World when id >= 0x1D && id <= 0x24:
                    return "Weed";
                case SaveType.Wild_World when (id >= 0x25 && id <= 0x6D) || (id >= 0xC7 && id <= 0xD3):
                    return "Tree";
                case SaveType.Wild_World when id >= 0x6E && id <= 0x89:
                    return "Parched Flower";
                case SaveType.Wild_World when id >= 0x8A && id <= 0xA5:
                    return "Watered Flower";
                case SaveType.Wild_World when id >= 0xA7 && id <= 0xC6:
                    return "Pattern";
                case SaveType.Wild_World when (id >= 0xE3 && id <= 0xE7) || (id >= 0xED && id <= 0xFB):
                    return "Rock";
                case SaveType.Wild_World when id >= 0xE8 && id <= 0xEC:
                    return "Money Rock";
                case SaveType.Wild_World when id >= 0x1000 && id <= 0x10FF:
                    return "Paper";
                //Wallpaper
                case SaveType.Wild_World when id >= 0x1100 && id <= 0x1143:
                //Carpet
                case SaveType.Wild_World when id >= 0x1144 && id <= 0x1187:
                    return "Wall/Floor";
                case SaveType.Wild_World when id >= 0x11A8 && id <= 0x12AF:
                    return "Clothes"; //Shirts
                case SaveType.Wild_World when id >= 0x12B0 && id <= 0x131F:
                    return "Catchable";
                case SaveType.Wild_World when id >= 0x1323 && id <= 0x1368:
                    return "Song";
                case SaveType.Wild_World when id >= 0x1369 && id <= 0x139F:
                    return "Tool";
                case SaveType.Wild_World when id >= 0x13A8 && id <= 0x1457:
                    return "Clothes"; //Hats, Flowers, & Glasses/Masks
                case SaveType.Wild_World when id >= 0x1492 && id <= 0x14FD:
                    return "Money";
                case SaveType.Wild_World when (id >= 0x14FE && id <= 0x1530) || (id >= 0x1542 && id <= 0x1548) || (id >= 0x155E && id <= 0x156D):
                    return "Item";
                case SaveType.Wild_World when (id >= 0x1531 && id <= 0x1541):
                    return "Turnip";
                case SaveType.Wild_World when id == 0x1549:
                    return "Fossil";
                case SaveType.Wild_World when id >= 0x1554 && id <= 0x155C:
                    return "Shell";
                case SaveType.Wild_World when (id >= 0x3000 && id <= 0x45D8) || (id >= 0x47D8 && id <= 0x4BA0):
                    return "Furniture";
                case SaveType.Wild_World when id >= 0x45DC && id <= 0x47D4:
                    return "Gyroids";
                // F030-1 = Multispace furniture item
                case SaveType.Wild_World when (id >= 0x5000 && id <= 0x5021) || id == 0xF030 || id == 0xF031:
                    return "Building";
                case SaveType.Wild_World:
                    return "Unknown";
                case SaveType.City_Folk when id == 0xFFF1:
                    return "Empty";
                case SaveType.City_Folk when (id >= 0xB710 && id <= 0xCE50) || (id >= 0x93F0 && id <= 0x9414) || (id >= 0x9CC0 && id < 0x9EC0):
                    return "Furniture";
                case SaveType.City_Folk when id >= 0xA518 && id <= 0xAA7C:
                    return "Clothes";
                case SaveType.City_Folk when (id >= 0x93E8 && id <= 0x93EC) || (id >= 0x9FA0 && id <= 0xA420):
                    return "Wall/Floor";
                case SaveType.City_Folk when id >= 0x94B0 && id <= 0x95D8:
                    return "Song";
                case SaveType.City_Folk when id >= 0x9640 && id <= 0x974F:
                    return "Paper";
                case SaveType.City_Folk when id >= 0x97D0 && id <= 0x9814:
                    return "Turnip";
                //Actually separated a little bit
                case SaveType.City_Folk when id >= 0x9960 && id <= 0x9BEC:
                    return "Catchable";
                case SaveType.City_Folk when id >= 0xAC20 && id <= 0xB2E4:
                    return "Clothes"; //Hats, Masks/Glasses & Flowers
                case SaveType.City_Folk when id >= 0xB3F0 && id <= 0xB5E8:
                    return "Gyroids";
                case SaveType.City_Folk when (id >= 0xCE80 && id <= 0xCF54) || (id >= 0xAA90 && id <= 0xAB14):
                    return "Tool";
                case SaveType.City_Folk when id == 0x9018:
                    return "Fossil";
                case SaveType.City_Folk when id >= 0x0001 && id <= 0x0056:
                    return "Tree";
                case SaveType.City_Folk when (id >= 0x0057 && id <= 0x005A) || (id >= 0x00DD && id <= 0x00E1):
                    return "Weed";
                case SaveType.City_Folk when (id >= 0x005B && id <= 0x005F) || (id >= 0x0065 && id <= 0x0073):
                    return "Rock";
                case SaveType.City_Folk when id >= 0x0060 && id <= 0x0064:
                    return "Money Rock";
                case SaveType.City_Folk when id >= 0x0074 && id <= 0x0093:
                    return "Pattern";
                case SaveType.City_Folk when id >= 0x009E && id <= 0x00BD:
                    return "Flower";
                case SaveType.City_Folk when id >= 0x00BE && id <= 0x00DD:
                    return "Parched Flower";
                case SaveType.City_Folk when id >= 0x9118 && id <= 0x9138:
                    return "Shell";
                case SaveType.City_Folk when id >= 0x9194 && id <= 0x9340:
                    return "Money";
                case SaveType.City_Folk when id >= 0x9348 && id <= 0x93D0:
                    return "Quest Item";
                case SaveType.City_Folk when (id >= 0x901C && id <= 0x9108) || (id >= 0x093D4 && id <= 0x93D8) || (id >= 0x9140 && id <= 0x9150) || (id >= 0x915C && id <= 0x9164):
                    return "Item";
                case SaveType.City_Folk when (id >= 0x9000 && id <= 0x9014) || (id >= 0x9168 && id <= 0x9178):
                    return "Fruit";
                case SaveType.City_Folk when id == 0xD000 || id == 0x7003 || id == 0xF030:
                    return "Building";
                case SaveType.City_Folk:
                    return "Unknown";
                case SaveType.New_Leaf when id == 0x7FFE:
                    return "Empty";
                case SaveType.New_Leaf when id == 0x009D:
                    return "Pattern";
                //C9 & CA = "weed" flowers
                case SaveType.New_Leaf when id >= 0x009F && id <= 0x00C8:
                    return "Flower";
                case SaveType.New_Leaf when id >= 0x00CE && id <= 0x00F7:
                    return "Wilted Flower";
                case SaveType.New_Leaf when (id >= 0x0005 && id <= 0x007B) || (id >= 0x0080 && id <= 0x0097):
                    return "Tree";
                case SaveType.New_Leaf when id >= 0x0098 && id <= 0x009C:
                    return "Rock";
                case SaveType.New_Leaf when (id >= 0x007C && id <= 0x007F) || (id >= 0x00C9 && id <= 0x00CD) || id == 0x00F8:
                    return "Weed";
                case SaveType.New_Leaf when id >= 0x28B2 && id <= 0x2934:
                    return "Gyroids";
                case SaveType.New_Leaf when id == 0x202A:
                    return "Fossil";
                case SaveType.New_Leaf when id >= 0x2087 && id <= 0x2090:
                    return "Shell";
                case SaveType.New_Leaf when id >= 0x2126 && id <= 0x2239:
                    return "Song";
                case SaveType.New_Leaf when id >= 0x223A && id <= 0x227A:
                    return "Paper";
                case SaveType.New_Leaf when id >= 0x227B && id <= 0x2285:
                    return "Turnip";
                case SaveType.New_Leaf when id >= 0x2286 && id <= 0x2341:
                    return "Catchable";
                case SaveType.New_Leaf when (id >= 0x2342 && id <= 0x2445) || (id >= 0x2119 && id <= 0x211A):
                    return "Wall/Floor";
                case SaveType.New_Leaf when id >= 0x2446 && id <= 0x28B1:
                    return "Clothes";
                case SaveType.New_Leaf when id >= 0x303B && id <= 0x307A:
                    return "Tool";
                case SaveType.New_Leaf when id >= 0x20A7 && id <= 0x2112:
                    return "Money";
                case SaveType.New_Leaf when id >= 0x209A && id <= 0x209B:
                    return "Item";
                case SaveType.New_Leaf when id == 0x7FFC:
                    return "Occupied";
                case SaveType.New_Leaf:
                    return "Furniture"; //Just until I gather furniture offsets
                case SaveType.Welcome_Amiibo when id == 0x7FFE:
                    return "Empty";
                case SaveType.Welcome_Amiibo when (id >= 0x0005 && id <= 0x007B) || (id >= 0x0080 && id <= 0x0097):
                    return "Tree";
                case SaveType.Welcome_Amiibo when (id >= 0x007C && id <= 0x007F) || (id >= 0x00C9 && id <= 0x00CD) || id == 0x00F8:
                    return "Weed";
                case SaveType.Welcome_Amiibo when id == 0x009D:
                    return "Pattern";
                case SaveType.Welcome_Amiibo when id >= 0x0098 && id <= 0x009C:
                    return "Rock";
                case SaveType.Welcome_Amiibo when id >= 0x009F && id <= 0x00C8:
                    return "Flower";
                case SaveType.Welcome_Amiibo when id >= 0x00CE && id <= 0x00F7:
                    return "Wilted Flower";
                case SaveType.Welcome_Amiibo when id == 0x202A:
                    return "Fossil";
                case SaveType.Welcome_Amiibo when id >= 0x20AC && id <= 0x2117:
                    return "Money";
                case SaveType.Welcome_Amiibo when id >= 0x212B && id <= 0x223E:
                    return "Song";
                case SaveType.Welcome_Amiibo when id >= 0x223F && id <= 0x2282:
                    return "Paper";
                case SaveType.Welcome_Amiibo when id >= 0x2283 && id <= 0x228D:
                    return "Turnip";
                case SaveType.Welcome_Amiibo when id >= 0x228E && id <= 0x234B:
                    return "Catchable";
                case SaveType.Welcome_Amiibo when id >= 0x234C && id <= 0x2492:
                    return "Wall/Floor";
                case SaveType.Welcome_Amiibo when id >= 0x2495 && id <= 0x295B:
                    return "Clothes";
                case SaveType.Welcome_Amiibo when id >= 0x295C && id <= 0x29DE:
                    return "Gyroids";
                case SaveType.Welcome_Amiibo when id >= 0x334C && id <= 0x338B:
                    return "Tool";
                case SaveType.Welcome_Amiibo when id >= 0x209F && id <= 0x20A0:
                    return "Item";
                case SaveType.Welcome_Amiibo when id >= 0x208C && id <= 0x2095:
                    return "Shell";
                case SaveType.Welcome_Amiibo when id == 0x7FFC:
                    return "Occupied";
                case SaveType.Welcome_Amiibo:
                    return "Furniture";
                default:
                    return "Unknown Save Type";
            }
        }
        public static uint GetItemColor(string itemType)
        {
            switch (itemType)
            {
                case "Furniture":
                    return 0xC83CDE30;
                case "Flower":
                    return 0xC8EC67B8;
                case "Pattern":
                    return 0xC89999FF;
                case "Parched Flower":
                    return 0xC8A36700;
                case "Wilted Flower":
                    return 0xC84D4D33;
                case "Watered Flower":
                    return 0xC800A0A0;
                case "Money":
                    return 0xC8FFFF00;
                case "Rock":
                    return 0xC8000000;
                case "Money Rock":
                    return 0xC8A32F2F;
                case "Signboard":
                    return 0xC8663300;
                case "Song":
                    return 0xC8A4ECB8;
                case "Paper":
                    return 0xC8A4ECE8;
                case "Turnip":
                    return 0xC8BBAC9D;
                case "Catchable":
                    return 0xC8BAE33E;
                case "Wall/Floor":
                    return 0xC8994040;
                case "Clothes":
                    return 0xC82874AA;
                case "Gyroids":
                    return 0xC8D48324;
                case "Fossil":
                    return 0xC8513D2F;
                case "Tool":
                    return 0xC8818181;
                case "Item":
                    return 0xC8FFA500;
                case "Fruit":
                    return 0xC8DDA0DD;
                case "Trash":
                    return 0xC8556B2F;
                case "Quest Item":
                    return 0xC8BDB76B;
                case "Raffle Ticket":
                    return 0xC81E90FF;
                case "Tree":
                    return 0xC88B4513; //0xC800FF00;
                case "Weed":
                    return 0xC8008000;
                case "Shell":
                    return 0xC8FFC0CB;
                case "Empty":
                    return 0x00FFFFFF;
                case "Occupied":
                    return 0xDD999999;
                case "Building":
                    return 0xFF777777;
                case "Diary":
                    return 0xC8FF007F;
                case "HouseObject":
                    return 0xC8A59895;
                default:
                    return 0xC8FF0000;
            }
        }

        public static string GetItemFlag1Type(Item item, byte itemFlag)
        {
            var itemType = GetItemType(item.ItemId, MainForm.Save_File.Save_Type);
            switch (itemType)
            {
                case "Fruit":
                    switch (itemFlag)
                    {
                        case 0: return "None";
                        case 1: return "x2";
                        case 2: return "x3";
                        case 3: return "x4";
                        case 4: return "x5";
                        case 5: return "x6";
                        case 6: return "x7";
                        case 7: return "x8";
                        case 8: return "x9";
                        default: return "Undocumented Fruit Flag 1";
                    }
                case "Paper":
                    switch(itemFlag)
                    {
                        case 0: return "None";
                        case 1: return "x2";
                        case 2: return "x3";
                        case 3: return "x4";
                        default: return "Undocumented Paper Flag 1";
                    }
                case "Pattern":
                    switch(itemFlag)
                    {
                        case 0: return "1-1";
                        case 1: return "1-2";
                        case 2: return "1-3";
                        case 3: return "1-4";
                        case 4: return "1-5";
                        case 5: return "1-6";
                        case 6: return "1-7";
                        case 7: return "1-8";
                        case 8: return "1-9";
                        case 9: return "1-10";
                        case 0xA: return "2-1";
                        case 0xB: return "2-2";
                        case 0xC: return "2-3";
                        case 0xD: return "2-4";
                        case 0xE: return "2-5";
                        case 0xF: return "2-6";
                        case 0x10: return "2-7";
                        case 0x11: return "2-8";
                        case 0x12: return "2-9";
                        case 0x13: return "2-10";
                        case 0x14: return "3-1";
                        case 0x15: return "3-2";
                        case 0x16: return "3-3";
                        case 0x17: return "3-4";
                        case 0x18: return "3-5";
                        case 0x19: return "3-6";
                        case 0x1A: return "3-7";
                        case 0x1B: return "3-8";
                        case 0x1C: return "3-9";
                        case 0x1D: return "3-10";
                        case 0x1E: return "4-1";
                        case 0x1F: return "4-2";
                        case 0x20: return "4-3";
                        case 0x21: return "4-4";
                        case 0x22: return "4-5";
                        case 0x23: return "4-6";
                        case 0x24: return "4-7";
                        case 0x25: return "4-8";
                        case 0x26: return "4-9";
                        case 0x27: return "4-10";
                        default: return "Unknown Pattern Flag 1";
                    }
                //Standard Item Flag 1s
                case "Tree":
                    switch (itemFlag)
                    {
                        case 0:
                            return "None";
                        case 1:
                            return "Perfect Fruit";
                        case 3:
                            return "No Fruit";
                        case 8:
                            return "Perfect Fruit";
                        case 0x12:
                            return "Prefect Fruit";
                        case 0x30:
                            return "Perfect Fruit";
                        default:
                            return "Undocumented Tree Flag 1";
                    }
                default:
                    switch(itemFlag)
                    {
                        case 0:
                            return "None";
                        case 0x20:
                            return "Present"; //Needs testing, from MarcRobledo's editor
                        case 0x40:
                            return "Watered";
                        case 0x80:
                            return "Buried";
                        default:
                            return "Undocumented Generic Flag 1";
                    }
            }
        }

        public static string GetFurnitureItemFlag2Type(byte itemFlag) //Our Flag 2, but treated as flag 1 by other editors
        {
            switch (itemFlag)
            {
                case 0:
                    return "None";
                case 0x40:
                    return "Rotated 90 Degrees";
                case 0x80:
                    return "Rotated 180 Degrees";
                case 0xC0:
                    return "Rotated 270 Degrees";
                default:
                    return "Undocumented Flag 2";
            }
        }

        public static string GetItemName(ushort itemId)
        {
            var found = ItemDatabase.Where(o => o.Key == itemId).Select(o => new { o.Key, o.Value }).FirstOrDefault();
            if (found != null)
                return found.Value;
            var baseId = (ushort)(itemId & 0xFFFC);
            var foundBase = ItemDatabase.Where(o => o.Key == baseId).Select(o => new { o.Key, o.Value }).FirstOrDefault();
            return foundBase != null ? foundBase.Value : "Unknown";
        }

        //Used for DataSource for ComboBoxes
        public static Item[] ToItemArray(Dictionary<ushort, string> itemDict)
        {
            var itemArray = new Item[itemDict.Count];
            for (var i = 0; i < itemDict.Count; i++)
                itemArray[i] = new Item(itemDict.ElementAt(i).Key);
            return itemArray;
        }

        //New Leaf Item Decoding
        public static void DecodeItem(uint encodedItemId, out byte flag1, out byte flag2, out ushort itemId)
        {
            flag1 = (byte)(encodedItemId >> 24); //Flag 1
            flag2 = (byte)(encodedItemId >> 16); //Flag 2
            itemId = (ushort)(encodedItemId);
        }

        //New Leaf Item Encoding
        public static uint EncodeItem(byte flag1, byte flag2, ushort itemId)
        {
            uint encodedItemId = itemId;
            encodedItemId += (uint)(flag2 << 16);
            encodedItemId += (uint)(flag1 << 24);
            return encodedItemId;
        }

        public static uint EncodeItem(Item item)
        {
            uint encodedItemId = item.ItemId;
            encodedItemId += (uint)(item.Flag2 << 16);
            encodedItemId += (uint)(item.Flag1 << 24);
            return encodedItemId;
        }

        public static Building[] GetBuildings(Save save, bool islandBuildings = false)
        {
            var buildings = new List<Building>();
            if (save.Save_Type == SaveType.City_Folk)
            {
                for (var i = 0; i < 33; i++)
                {
                    var dataOffset = save.Save_Data_Start_Offset + SaveDataManager.City_Folk_Offsets.Buildings + i * 2;
                    buildings.Add(new Building((byte)i, save.ReadByte(dataOffset), save.ReadByte(dataOffset + 1), SaveType.City_Folk));
                }
                //Add Pave's Table
                var paveOffset = save.Save_Data_Start_Offset + 0x5EB90;
                var paveTable = new Building(0x21, save.ReadByte(paveOffset), save.ReadByte(paveOffset + 1), SaveType.City_Folk);
                buildings.Add(paveTable);
                //Add Bus Stop
                var busStopOffset = save.Save_Data_Start_Offset + 0x5EB8A;
                var busStop = new Building(0x22, save.ReadByte(busStopOffset), save.ReadByte(busStopOffset + 1), SaveType.City_Folk);
                buildings.Add(busStop);
                //Add Signs
                for (var i = 0; i < 100; i++)
                {
                    var dataOffset = save.Save_Data_Start_Offset + 0x5EB92 + i * 2;
                    var sign = new Building(0x23, save.ReadByte(dataOffset), save.ReadByte(dataOffset + 1), SaveType.City_Folk);
                    buildings.Add(sign);
                }
            }
            else if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo) //TODO: Add changed offsets for Welcome Amiibo
            {
                if (islandBuildings == false)
                    for (var i = 0; i < 58; i++)
                    {
                        var dataOffset = save.Save_Data_Start_Offset + MainForm.Current_Save_Info.Save_Offsets.Buildings + i * 4;
                        buildings.Add(new Building(save.ReadByte(dataOffset), save.ReadByte(dataOffset + 2), save.ReadByte(dataOffset + 3), save.Save_Type));
                        //Technically, Building IDs are shorts, but since they only use the lower byte, we'll just ignore that
                    }
                else
                    for (var i = 0; i < 2; i++)
                    {
                        var dataOffset = save.Save_Data_Start_Offset + MainForm.Current_Save_Info.Save_Offsets.Island_Buildings + i * 4;
                        buildings.Add(new Building(save.ReadByte(dataOffset), save.ReadByte(dataOffset + 2), save.ReadByte(dataOffset + 3), save.Save_Type));
                    }
            }
            return buildings.ToArray();
        }
    }
}
