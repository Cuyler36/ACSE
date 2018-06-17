using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace ACSE
{
    public class ItemData
    {
        public static List<KeyValuePair<ushort, string>> ItemDatabase = new List<KeyValuePair<ushort, string>>();

        public static string[] CF_Building_Names = new string[]
        {
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

        public static Dictionary<byte, string> NL_Building_Names = new Dictionary<byte, string>
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

        public static Dictionary<byte, string> WA_Building_Names = new Dictionary<byte, string>
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

        public static int GetBuildingCount(Building[] Buildings, SaveType Save_Type)
        {
            int NoBuildingID = Save_Type == SaveType.New_Leaf ? 0xF8 : 0xFC;
            return Buildings.Where(b => b.Exists && b.ID != NoBuildingID).Count();
        }

        public static string GetItemType(ushort ID, SaveType Save_Type)
        {
            if (Save_Type == SaveType.Doubutsu_no_Mori || Save_Type == SaveType.Animal_Crossing || Save_Type == SaveType.Doubutsu_no_Mori_Plus || Save_Type == SaveType.Doubutsu_no_Mori_e_Plus || Save_Type == SaveType.Animal_Forest) // TODO: DnM+, and DnMe+ need their own cases
                if (ID == 0)
                    return "Empty";
                else if (ID == 0xFFFF)
                    return "Occupied";
                else if (ID >= 0x8 && ID <= 0xA)
                    return "Weed";
                else if (ID >= 0x845 && ID <= 0x84D)
                    return "Flower";
                else if ((ID >= 0x2100 && ID <= 0x2103) || ID == 0x005C) //0x005C = Glowing Money/Shovel spot
                    return "Money";
                else if (ID >= 0x63 && ID <= 0x68)
                    return "Rock";
                else if (ID >= 0x6A && ID <= 0x6F)
                    return "Money Rock";
                else if (ID >= 0x900 && ID <= 0x920)
                    return "Signboard";
                else if (ID >= 0x2514 && ID <= 0x251B)
                    return "Shell";
                else if (ID >= 0x2A00 && ID <= 0x2A36)
                    return "Song";
                else if (ID >= 0x2B00 && ID <= 0x2B0F)
                    return "Diary";
                else if (ID >= 0x2000 && ID <= 0x20FF)
                    return "Paper";
                else if (ID >= 0x2F00 && ID <= 0x2F03)
                    return "Turnip";
                else if ((ID >= 0x2800 && ID <= 0x2804) || ID == 0x2807)
                    return "Fruit";
                else if ((ID >= 0x2300 && ID <= 0x2327) || (ID >= 0x2D00 && ID <= 0x2D27))
                    return "Catchable";
                else if (ID >= 0x2503 && ID <= 0x250C)
                    return "Quest Item";
                else if (ID >= 0x250E && ID <= 0x2510)
                    return "Trash";
                else if ((ID >= 0x2523 && ID <= 0x2530) || (ID >= 0x2900 && ID <= 0x290A) || (ID >= 0x2805 && ID <= 0x2806))
                    return "Item";
                else if (ID >= 0x2C00 && ID <= 0x2C5F)
                    return "Raffle Ticket";
                else if ((ID >= 0x2600 && ID <= 0x2642) || (ID >= 0x2700 && ID <= 0x2742))
                    return "Wall/Floor";
                else if (ID >= 0x2400 && ID <= 0x24FF)
                    return "Clothes";
                else if (ID >= 0x15B0 && ID <= 0x17A8)
                    return "Gyroids";
                else if (ID == 0x2511)
                    return "Fossil";
                else if ((ID >= 0x2200 && ID <= 0x225B) || ID == 0x251E)
                    return "Tool";  //0x251E = Signboard (not a 'tool', but it's still classified as one)
                else if ((ID >= 0x1 && ID <= 0x4) || (ID >= 0x005E && ID <= 0x0060) || ID == 0x69 || (ID >= 0x0070 && ID <= 0x0082) || (ID >= 0x0800 && ID <= 0x0869))
                    return "Tree";
                else if (ID >= 0x4000 && ID < 0x5000)
                    return "HouseObject";
                else if ((ID >= 0x5 && ID <= 0x7) || (ID >= 0xB && ID <= 0x10) || (ID >= 0x5000 && ID <= 0xB000) || ID >= 0xFE00)
                    return "Building";
                else if ((ID >= 0x1000 && ID <= 0x15AC) || (ID >= 0x17AC && ID <= 0x1FFC) || (ID >= 0x3000 && ID <= 0x33C4)) // || ID >= 0xFE20
                    return "Furniture";
                else
                    return "Unknown";
            else if (Save_Type == SaveType.Wild_World)
                if (ID == 0xFFF1)
                    return "Empty";
                else if (ID == 0xF030)
                    return "Occupied";
                else if (ID >= 0 && ID <= 0x1C)
                    return "Flower";
                else if (ID >= 0x1D && ID <= 0x24)
                    return "Weed";
                else if ((ID >= 0x25 && ID <= 0x6D) || (ID >= 0xC7 && ID <= 0xD3))
                    return "Tree";
                else if (ID >= 0x6E && ID <= 0x89)
                    return "Parched Flower";
                else if (ID >= 0x8A && ID <= 0xA5)
                    return "Watered Flower";
                //0x00A6??
                else if (ID >= 0xA7 && ID <= 0xC6)
                    return "Pattern";
                //Buried Red Turnips: 0xD4 - 0xE1
                else if ((ID >= 0xE3 && ID <= 0xE7) || (ID >= 0xED && ID <= 0xFB))
                    return "Rock";
                else if (ID >= 0xE8 && ID <= 0xEC)
                    return "Money Rock";
                //Unused Rocks: 0xED - 0xFB
                else if (ID >= 0x1000 && ID <= 0x10FF)
                    return "Paper";
                else if (ID >= 0x1100 && ID <= 0x1143) //Wallpaper
                    return "Wall/Floor";
                else if (ID >= 0x1144 && ID <= 0x1187) //Carpet
                    return "Wall/Floor";
                else if (ID >= 0x11A8 && ID <= 0x12AF)
                    return "Clothes"; //Shirts
                else if (ID >= 0x12B0 && ID <= 0x131F)
                    return "Catchable";
                //Garbage: 0x1320 - 0x1322
                else if (ID >= 0x1323 && ID <= 0x1368)
                    return "Song";
                else if (ID >= 0x1369 && ID <= 0x139F)
                    return "Tool";
                else if (ID >= 0x13A8 && ID <= 0x1457)
                    return "Clothes"; //Hats, Flowers, & Glasses/Masks
                else if (ID >= 0x1492 && ID <= 0x14FD)
                    return "Money";
                else if ((ID >= 0x14FE && ID <= 0x1530) || (ID >= 0x1542 && ID <= 0x1548) || (ID >= 0x155E && ID <= 0x156D))
                    return "Item";
                else if ((ID >= 0x1531 && ID <= 0x1541))
                    return "Turnip";
                else if (ID == 0x1549)
                    return "Fossil";
                else if (ID >= 0x1554 && ID <= 0x155C)
                    return "Shell";
                else if ((ID >= 0x3000 && ID <= 0x45D8) || (ID >= 0x47D8 && ID <= 0x4BA0))
                    return "Furniture";
                else if (ID >= 0x45DC && ID <= 0x47D4)
                    return "Gyroids";
                else if ((ID >= 0x5000 && ID <= 0x5021) || ID == 0xF030 || ID == 0xF031) // F030-1 = Multispace furniture item
                    return "Building";
                else
                    return "Unknown";
            else if (Save_Type == SaveType.City_Folk)
            {
                if (ID == 0xFFF1)
                    return "Empty";
                else if ((ID >= 0xB710 && ID <= 0xCE50) || (ID >= 0x93F0 && ID <= 0x9414) || (ID >= 0x9CC0 && ID < 0x9EC0))
                    return "Furniture";
                else if (ID >= 0xA518 && ID <= 0xAA7C)
                    return "Clothes";
                else if ((ID >= 0x93E8 && ID <= 0x93EC) || (ID >= 0x9FA0 && ID <= 0xA420))
                    return "Wall/Floor";
                else if (ID >= 0x94B0 && ID <= 0x95D8)
                    return "Song";
                else if (ID >= 0x9640 && ID <= 0x974F)
                    return "Paper";
                else if (ID >= 0x97D0 && ID <= 0x9814)
                    return "Turnip";
                else if (ID >= 0x9960 && ID <= 0x9BEC) //Actually separated a little bit
                    return "Catchable";
                //Garbage - 0x9BF0 = 0x9BF8
                //Umbrellas 0xAA90 - 0xAB14
                else if (ID >= 0xAC20 && ID <= 0xB2E4)
                    return "Clothes"; //Hats, Masks/Glasses & Flowers
                else if (ID >= 0xB3F0 && ID <= 0xB5E8)
                    return "Gyroids";
                else if ((ID >= 0xCE80 && ID <= 0xCF54) || (ID >= 0xAA90 && ID <= 0xAB14))
                    return "Tool";
                else if (ID == 0x9018)
                    return "Fossil";
                else if (ID >= 0x0001 && ID <= 0x0056)
                    return "Tree";
                else if ((ID >= 0x0057 && ID <= 0x005A) || (ID >= 0x00DD && ID <= 0x00E1))
                    return "Weed";
                else if ((ID >= 0x005B && ID <= 0x005F) || (ID >= 0x0065 && ID <= 0x0073))
                    return "Rock";
                else if (ID >= 0x0060 && ID <= 0x0064)
                    return "Money Rock";
                else if (ID >= 0x0074 && ID <= 0x0093)
                    return "Pattern";
                //0x94 = Hole
                //0x95 - 0x9C = Turnip
                else if (ID >= 0x009E && ID <= 0x00BD)
                    return "Flower";
                else if (ID >= 0x00BE && ID <= 0x00DD)
                    return "Parched Flower";
                else if (ID >= 0x9118 && ID <= 0x9138)
                    return "Shell";
                else if (ID >= 0x9194 && ID <= 0x9340)
                    return "Money";
                else if (ID >= 0x9348 && ID <= 0x93D0)
                    return "Quest Item";
                else if ((ID >= 0x901C && ID <= 0x9108) || (ID >= 0x093D4 && ID <= 0x93D8) || (ID >= 0x9140 && ID <= 0x9150) || (ID >= 0x915C && ID <= 0x9164))
                    return "Item";
                else if ((ID >= 0x9000 && ID <= 0x9014) || (ID >= 0x9168 && ID <= 0x9178))
                    return "Fruit";
                else if (ID == 0xD000 || ID == 0x7003 || ID == 0xF030)
                    return "Building";
                else
                    return "Unknown";
            }
            else if (Save_Type == SaveType.New_Leaf)
            {
                if (ID == 0x7FFE)
                    return "Empty";
                else if (ID == 0x009D)
                    return "Pattern";
                else if (ID >= 0x009F && ID <= 0x00C8) //C9 & CA = "weed" flowers
                    return "Flower";
                else if (ID >= 0x00CE && ID <= 0x00F7)
                    return "Wilted Flower";
                else if ((ID >= 0x0005 && ID <= 0x007B) || (ID >= 0x0080 && ID <= 0x0097))
                    return "Tree";
                else if (ID >= 0x0098 && ID <= 0x009C)
                    return "Rock";
                else if ((ID >= 0x007C && ID <= 0x007F) || (ID >= 0x00C9 && ID <= 0x00CD) || ID == 0x00F8)
                    return "Weed";
                else if (ID >= 0x28B2 && ID <= 0x2934)
                    return "Gyroids";
                else if (ID == 0x202A)
                    return "Fossil";
                else if (ID >= 0x2087 && ID <= 0x2090)
                    return "Shell";
                else if (ID >= 0x2126 && ID <= 0x2239)
                    return "Song";
                else if (ID >= 0x223A && ID <= 0x227A)
                    return "Paper";
                else if (ID >= 0x227B && ID <= 0x2285)
                    return "Turnip";
                else if (ID >= 0x2286 && ID <= 0x2341)
                    return "Catchable";
                else if ((ID >= 0x2342 && ID <= 0x2445) || (ID >= 0x2119 && ID <= 0x211A))
                    return "Wall/Floor";
                else if (ID >= 0x2446 && ID <= 0x28B1)
                    return "Clothes";
                else if (ID >= 0x303B && ID <= 0x307A)
                    return "Tool";
                else if (ID >= 0x20A7 && ID <= 0x2112)
                    return "Money";
                else if (ID >= 0x209A && ID <= 0x209B)
                    return "Item";
                else if (ID == 0x7FFC)
                    return "Occupied";
                else
                    return "Furniture"; //Just until I gather furniture offsets
            }
            else if (Save_Type == SaveType.Welcome_Amiibo)
            {
                if (ID == 0x7FFE)
                    return "Empty";
                else if ((ID >= 0x0005 && ID <= 0x007B) || (ID >= 0x0080 && ID <= 0x0097))
                    return "Tree";
                else if ((ID >= 0x007C && ID <= 0x007F) || (ID >= 0x00C9 && ID <= 0x00CD) || ID == 0x00F8)
                    return "Weed";
                else if (ID == 0x009D)
                    return "Pattern";
                else if (ID >= 0x0098 && ID <= 0x009C)
                    return "Rock";
                else if (ID >= 0x009F && ID <= 0x00C8)
                    return "Flower";
                else if (ID >= 0x00CE && ID <= 0x00F7)
                    return "Wilted Flower";
                else if (ID == 0x202A)
                    return "Fossil";
                else if (ID >= 0x20AC && ID <= 0x2117)
                    return "Money";
                else if (ID >= 0x212B && ID <= 0x223E)
                    return "Song";
                else if (ID >= 0x223F && ID <= 0x2282)
                    return "Paper";
                else if (ID >= 0x2283 && ID <= 0x228D)
                    return "Turnip";
                else if (ID >= 0x228E && ID <= 0x234B)
                    return "Catchable";
                else if (ID >= 0x234C && ID <= 0x2492)
                    return "Wall/Floor";
                else if (ID >= 0x2495 && ID <= 0x295B)
                    return "Clothes";
                else if (ID >= 0x295C && ID <= 0x29DE)
                    return "Gyroids";
                //else if (ID >= 0x30CC && ID <= 0x30CF)
                //return "Mannequin";
                else if (ID >= 0x334C && ID <= 0x338B)
                    return "Tool";
                else if (ID >= 0x209F && ID <= 0x20A0)
                    return "Item";
                else if (ID >= 0x208C && ID <= 0x2095)
                    return "Shell";
                else if (ID == 0x7FFC)
                    return "Occupied";
                else
                    return "Furniture";
            }
            else
                return "Unknown Save Type";
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
                    return 0xC800FF00;
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
                case "Unknown":
                default:
                    return 0xC8FF0000;
            }
        }

        public static string GetItemFlag1Type(Item Item, byte ItemFlag)
        {
            string ItemType = GetItemType(Item.ItemID, MainForm.Save_File.Save_Type);
            if (ItemType == "Fruit")
            {
                switch (ItemFlag)
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
            }
            else if (ItemType == "Paper")
            {
                switch(ItemFlag)
                {
                    case 0: return "None";
                    case 1: return "x2";
                    case 2: return "x3";
                    case 3: return "x4";
                    default: return "Undocumented Paper Flag 1";
                }
            }
            else if (ItemType == "Pattern")
            {
                switch(ItemFlag)
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
            }
            else if (ItemType == "Tree")
            {
                switch (ItemFlag)
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
            }
            //Standard Item Flag 1s
            else
            {
                switch(ItemFlag)
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

        public static string GetFurnitureItemFlag2Type(byte ItemFlag) //Our Flag 2, but treated as flag 1 by other editors
        {
            switch (ItemFlag)
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

        public static string GetItemName(ushort itemID)
        {
            var Found = ItemDatabase.Where(o => o.Key == itemID).Select(o => new { o.Key, o.Value }).FirstOrDefault();
            if (Found != null)
                return Found.Value;
            else
            {
                ushort BaseID = (ushort)(itemID & 0xFFFC);
                var FoundBase = ItemDatabase.Where(o => o.Key == BaseID).Select(o => new { o.Key, o.Value }).FirstOrDefault();
                if (FoundBase != null)
                    return FoundBase.Value;
                else
                    return "Unknown";
            }
        }

        //Used for DataSource for ComboBoxes
        public static Item[] ToItemArray(Dictionary<ushort, string> Item_Dict)
        {
            Item[] Item_Array = new Item[Item_Dict.Count];
            for (int i = 0; i < Item_Dict.Count; i++)
                Item_Array[i] = new Item(Item_Dict.ElementAt(i).Key);
            return Item_Array;
        }

        //New Leaf Item Decoding
        public static void DecodeItem(uint Encoded_Item_ID, out byte Flag1, out byte Flag2, out ushort Item_ID)
        {
            Flag1 = (byte)(Encoded_Item_ID >> 24); //Flag 1
            Flag2 = (byte)(Encoded_Item_ID >> 16); //Flag 2
            Item_ID = (ushort)(Encoded_Item_ID);
        }

        //New Leaf Item Encoding
        public static uint EncodeItem(byte Flag1, byte Flag2, ushort Item_ID)
        {
            uint Encoded_Item_ID = Item_ID;
            Encoded_Item_ID += (uint)(Flag2 << 16);
            Encoded_Item_ID += (uint)(Flag1 << 24);
            return Encoded_Item_ID;
        }

        public static uint EncodeItem(Item item)
        {
            uint Encoded_Item_ID = item.ItemID;
            Encoded_Item_ID += (uint)(item.Flag2 << 16);
            Encoded_Item_ID += (uint)(item.Flag1 << 24);
            return Encoded_Item_ID;
        }

        public static Building[] GetBuildings(Save save, bool Island_Buildings = false)
        {
            List<Building> Buildings = new List<Building>();
            if (save.Save_Type == SaveType.City_Folk)
            {
                for (int i = 0; i < 33; i++)
                {
                    int DataOffset = save.Save_Data_Start_Offset + SaveDataManager.City_Folk_Offsets.Buildings + i * 2;
                    Buildings.Add(new Building((byte)i, save.ReadByte(DataOffset), save.ReadByte(DataOffset + 1), SaveType.City_Folk));
                }
                //Add Pave's Table
                int Pave_Offset = save.Save_Data_Start_Offset + 0x5EB90;
                Building Pave_Table = new Building(0x21, save.ReadByte(Pave_Offset), save.ReadByte(Pave_Offset + 1), SaveType.City_Folk);
                Buildings.Add(Pave_Table);
                //Add Bus Stop
                int Bus_Stop_Offset = save.Save_Data_Start_Offset + 0x5EB8A;
                Building Bus_Stop = new Building(0x22, save.ReadByte(Bus_Stop_Offset), save.ReadByte(Bus_Stop_Offset + 1), SaveType.City_Folk);
                Buildings.Add(Bus_Stop);
                //Add Signs
                for (int i = 0; i < 100; i++)
                {
                    int DataOffset = save.Save_Data_Start_Offset + 0x5EB92 + i * 2;
                    Building Sign = new Building(0x23, save.ReadByte(DataOffset), save.ReadByte(DataOffset + 1), SaveType.City_Folk);
                    Buildings.Add(Sign);
                }
            }
            else if (save.Save_Type == SaveType.New_Leaf || save.Save_Type == SaveType.Welcome_Amiibo) //TODO: Add changed offsets for Welcome Amiibo
            {
                if (Island_Buildings == false)
                    for (int i = 0; i < 58; i++)
                    {
                        int DataOffset = save.Save_Data_Start_Offset + MainForm.Current_Save_Info.Save_Offsets.Buildings + i * 4;
                        Buildings.Add(new Building(save.ReadByte(DataOffset), save.ReadByte(DataOffset + 2), save.ReadByte(DataOffset + 3), save.Save_Type));
                        //Technically, Building IDs are shorts, but since they only use the lower byte, we'll just ignore that
                    }
                else
                    for (int i = 0; i < 2; i++)
                    {
                        int DataOffset = save.Save_Data_Start_Offset + MainForm.Current_Save_Info.Save_Offsets.Island_Buildings + i * 4;
                        Buildings.Add(new Building(save.ReadByte(DataOffset), save.ReadByte(DataOffset + 2), save.ReadByte(DataOffset + 3), save.Save_Type));
                    }
            }
            return Buildings.ToArray();
        }
    }

    public class Item
    {
        public ushort ItemID = 0;
        public byte Flag1 = 0;
        public byte Flag2 = 0;
        public string Name = "";

        public Item()
        {
            SaveType saveType = MainForm.Save_File == null ? SaveType.Animal_Crossing : MainForm.Save_File.Save_Type;
            if (saveType == SaveType.Wild_World || saveType == SaveType.City_Folk)
                ItemID = 0xFFF1;
            else if (saveType == SaveType.New_Leaf || saveType == SaveType.Welcome_Amiibo)
                ItemID = 0x7FFE;
            Name = ItemData.GetItemName(ItemID);
        }

        public Item(ushort itemId)
        {
            ItemID = itemId;
            Name = ItemData.GetItemName(ItemID);
        }

        public Item(uint itemId)
        {
            ItemID = (ushort)itemId;
            Flag1 = (byte)(itemId >> 24);
            Flag2 = (byte)(itemId >> 16);
            Name = ItemData.GetItemName(ItemID);
        }

        public Item(Item CloningItem)
        {
            ItemID = CloningItem.ItemID;
            Flag1 = CloningItem.Flag1;
            Flag2 = CloningItem.Flag2;
            Name = CloningItem.Name;
        }

        public Item (ushort itemId, byte flag1, byte flag2)
        {
            ItemID = itemId;
            Flag1 = flag1;
            Flag2 = flag2;
            Name = ItemData.GetItemName(ItemID);
        }

        public uint ToUInt32()
        {
            return (uint)((Flag1 << 24) + (Flag2 << 16) + ItemID);
        }

        public override bool Equals(object obj)
        {
            if (obj is Item)
            {
                var ComparingItem = obj as Item;
                return (ComparingItem.ItemID == ItemID && ComparingItem.Flag1 == Flag1 && ComparingItem.Flag2 == Flag2);
            }
            else if (obj is ushort)
            {
                return (ushort)obj == ItemID;
            }

            return false;
        }

        public static bool operator == (Item obj1, Item obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator != (Item obj1, Item obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class WorldItem : Item
    {
        public Point Location;
        public int Index = 0;
        public bool Buried = false;
        public bool Watered = false;

        public WorldItem(ushort itemId, int position) : base(itemId)
        {
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(ushort itemId, byte flag1, byte flag2, int position) : base(itemId)
        {
            Flag1 = flag1;
            Flag2 = flag2;
            Buried = Flag1 == 0x80;
            Watered = Flag1 == 0x40;
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(int position) : base()
        {
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(WorldItem CloningItem)
        {
            ItemID = CloningItem.ItemID;
            Flag1 = CloningItem.Flag1;
            Flag2 = CloningItem.Flag2;
            Name = CloningItem.Name;
            Index = CloningItem.Index;
            Location = new Point(Index % 16, Index / 16);
        }

        public WorldItem(uint itemId, int position) : base(itemId)
        {
            Index = position;
            Location = new Point(position % 16, position / 16);
            ItemID = (ushort)itemId;
            Flag1 = (byte)(itemId >> 24);
            Flag2 = (byte)(itemId >> 16);
            Name = ItemData.GetItemName(ItemID);
            Buried = Flag1 == 0x80;
            Watered = Flag1 == 0x40;
        }

        public override bool Equals(object obj)
        {
            if (obj is WorldItem)
            {
                var ComparingItem = obj as WorldItem;
                return (ComparingItem.ItemID == ItemID && ComparingItem.Flag1 == Flag1 && ComparingItem.Flag2 == Flag2);
            }
            else if (obj is ushort)
            {
                return (ushort)obj == ItemID;
            }

            return false;
        }

        public static bool operator ==(WorldItem obj1, WorldItem obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(WorldItem obj1, WorldItem obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Furniture : Item
    {
        public ushort BaseItemID = 0;
        public int Rotation = 0;

        public Furniture(ushort itemId) : base(itemId)
        {
            if (MainForm.Save_File.Save_Generation == SaveGeneration.N3DS)
            {
                BaseItemID = itemId;
                Rotation = 0;
            }
            else
            {
                BaseItemID = (ushort)(ItemID & 0xFFFC);
                if (ItemData.GetItemType(itemId, MainForm.Save_File.Save_Type) == "Furniture" || ItemData.GetItemType(itemId, MainForm.Save_File.Save_Type).Equals("Gyroids"))
                {
                    Rotation = (ItemID % 4) * 90;
                }
            }
        }

        public Furniture(uint item) : base(item)
        {
            BaseItemID = ItemID;
            Rotation = ((Flag1 >> 4) / 4) * 90;
        }

        public Furniture(ushort item, byte flag1, byte flag2) : base (item, flag1, flag2)
        {
            if (MainForm.Save_File.Save_Generation == SaveGeneration.N3DS)
            {
                BaseItemID = ItemID;
                Rotation = ((Flag1 >> 4) / 4) * 90;
            }
            else
            {
                BaseItemID = (ushort)(ItemID & 0xFFFC);
                if (ItemData.GetItemType(ItemID, MainForm.Save_File.Save_Type) == "Furniture" || ItemData.GetItemType(ItemID, MainForm.Save_File.Save_Type).Equals("Gyroids"))
                {
                    Rotation = (ItemID % 4) * 90;
                }
            }
        }

        public Furniture(Item item)
        {
            ItemID = item.ItemID;
            Name = item.Name;
            Flag1 = item.Flag1;
            Flag2 = item.Flag2;

            if (MainForm.Save_File.Save_Generation == SaveGeneration.N3DS)
            {
                BaseItemID = ItemID;
                Rotation = ((Flag1 >> 4) / 4) * 90;
            }
            else
            {
                BaseItemID = (ushort)(ItemID & 0xFFFC);
                if (ItemData.GetItemType(ItemID, MainForm.Save_File.Save_Type) == "Furniture" || ItemData.GetItemType(ItemID, MainForm.Save_File.Save_Type).Equals("Gyroids"))
                {
                    Rotation = (ItemID % 4) * 90;
                }
            }
        }

        public void SetRotation(int degrees)
        {
            if (degrees % 90 == 0)
            {
                Rotation = degrees;
                ItemID = (ushort)(BaseItemID + (degrees / 90));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Furniture)
            {
                var ComparingItem = obj as Furniture;
                if (MainForm.Save_File.Save_Generation == SaveGeneration.N3DS)
                    return (ComparingItem.ItemID == ItemID && ComparingItem.Flag1 == Flag1 && ComparingItem.Flag2 == Flag2);
                else
                    return ComparingItem.BaseItemID == BaseItemID;
            }
            else if (obj is ushort)
            {
                return (ushort)obj == ItemID;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Gyroid_Item : Item
    {
        public uint Price;
        public byte Sale_Type;
        public bool Free;

        public Gyroid_Item(ushort itemId, uint price, byte sellType) : base(itemId)
        {
            Price = price;
            Sale_Type = sellType;
            Free = sellType == 0;
        }
    }

    public class Shirt_Item : Item // Used in Animal Crossing only?
    {
        public byte Design_Database; // Specifies which Design Database to pull the design from. 0 = Default Shirt Database (Ex: 00 = Flame Shirt), 1 = Player's Patterns
        public byte Design_ID; // Specifies which entry in the Database to use for the design

        public Shirt_Item(ushort itemId, byte id, byte database = 0) : base(itemId)
        {
            Design_ID = database == 0 ? (byte)(itemId) : id;
            Design_Database = database;
        }

        public byte[] GetBytes()
        {
            return new byte[4] { Design_Database, Design_ID, (byte)(ItemID >> 8), (byte)(ItemID) };
        }
    }

    public class Building
    {
        public byte ID;
        public bool Exists;
        public byte Acre_Index;
        public byte X_Pos;
        public byte Y_Pos;
        public byte Acre_X;
        public byte Acre_Y;
        public string Name;

        public Building(byte id, byte x, byte y, SaveType saveType)
        {
            ID = id;
            //Despite what previous editors assume, I'm fairly certain that the X & Y location bytes are structured like this:
            //Upper Nibble = Acre
            //Lower Nibble = Position in Acre
            //I say this, as a town hall in New Leaf with location bytes of X = 0x28, Y = 0x19 is positioned on the third X acre and second Y acre at 0x8, 0x9.
            SaveDataManager.GetNibbles(x, out X_Pos, out Acre_X);
            SaveDataManager.GetNibbles(y, out Y_Pos, out Acre_Y);
            Acre_Index = (byte)((Acre_Y - 1) * 5 + (Acre_X - 1)); // * 5 works here because both CF and NL use 5 X acres
            if (Acre_Index > 24) //Works for NL too, since the dock is located in the 5th Y acre row.
                Acre_Index = 0;

            if (saveType == SaveType.City_Folk)
            {
                Name = ItemData.CF_Building_Names[id];
                Exists = Acre_X > 0 && Acre_Y > 0;
            }
            else if (saveType == SaveType.New_Leaf)
            {
                Name = ItemData.NL_Building_Names[id];
                Exists = ID != 0xF8;
            }
            else
            {
                Name = ItemData.WA_Building_Names[id];
                Exists = ID != 0xFC;
            }
        }
    }

    //Expand this for use with CF/NL??
    public class Structure
    {
        public ushort[] Structure_World_Data;
        public string Structure_Name;
        public int Structure_Width;

        public Structure(string Name, int Width, ushort[] World_Data)
        {
            Structure_Name = Name;
            Structure_Width = Width;
            Structure_World_Data = World_Data;
        }
    }
}
