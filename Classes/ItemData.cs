using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ACSE
{
    public class ItemData
    {
        //TODO: Add Nook's shops, Player Houses, Island House
        //Possibily add: Suspension Bridges, Boards, Villager Houses, Train Station
        public static Structure[] World_Structures = new Structure[7]
        {
            new Structure("Post Office", 4, new ushort[16]
            {
                0, 1, 1, 0,
                1, 1, 2, 1,
                1, 1, 1, 1,
                1, 1, 1, 0
            }),
            new Structure("Dump", 6, new ushort[36]
            {
                1, 1, 1, 1, 1, 1,
                1, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 1,
                1, 1, 0, 0, 1, 2
            }),
            new Structure("Museum", 7, new ushort[42]
            {
                1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 2, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1,
                0, 0, 0, 1, 0, 0, 0
            }),
            new Structure("Tailor's Shop", 4, new ushort[16]
            {
                0, 1, 1, 0,
                1, 1, 2, 1,
                1, 1, 1, 1,
                1, 1, 1, 0
            }),
            new Structure("Police Station", 3, new ushort[9]
            {
                1, 1, 1,
                1, 2, 1,
                1, 1, 1
            }),
            new Structure("Wishing Well", 2, new ushort[8]
            {
                1, 1,
                1, 1,
                2, 1,
                1, 1
            }),
            new Structure("Lighthouse", 2, new ushort[4]
            {
                1, 1,
                1, 2
            })
        };

        public static string[] CF_Building_Names = new string[]
        {
            "Player House (A)",
            "Player House (B)",
            "Player House (C)",
            "Player House (D)",
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

        //Furniture items are 0x4 offset.
        //Their rotation is calculated by this equation: (Item ID % 4) * 90
        //So, a piece of furniture with an id of 0x1022 would have a rotational value of 180 degrees.
        public static ushort[] Furniture_IDs = new ushort[1102]
        {
            0x1C68, 0x1C6C, 0x1C70, 0x1C74, 0x1C78, 0x1C7C, 0x1C80, 0x1C84, 0x1C88, 0x1C8C, 0x1C90, 0x1C94, 0x1C98, 0x1C9C, 0x1CA0, 0x1CA4, 0x1CA8, 0x1CAC, 0x1CB0, 0x1CB4, 0x1CB8, 0x1CBC, 0x1CC0, 0x1CC4, 0x1CC8, 0x1CCC, 0x1CD0, 0x1CD4, 0x1CD8, 0x1CDC, 0x1CE0, 0x1CE4, 0x1CE8, 0x1CEC, 0x1CF0, 0x1CF4, 0x1CF8, 0x1CFC, 0x1D00, 0x1D04, 0x1EEC, 0x1EF0, 0x1EF4, 0x1EF8, 0x1EFC, 0x1F00, 0x1F04, 0x1F08, 0x1F0C, 0x1F10, 0x1F14, 0x1F18, 0x1F1C, 0x1F20, 0x1F24, 0x1F28, 0x1F2C, 0x1F30, 0x1F34, 0x1F38, 0x1F3C, 0x1F40, 0x1F44, 0x1F48, 0x1F4C, 0x1350, 0x1354, 0x1358, 0x135C, 0x1360, 0x1364, 0x1368, 0x136C, 0x1370, 0x1374, 0x1378, 0x137C, 0x1380, 0x1384, 0x1388, 0x138C, 0x1390, 0x1394, 0x1398, 0x139C, 0x13A0, 0x13A4, 0x13A8, 0x13AC, 0x13B0, 0x13B4, 0x13B8, 0x13BC, 0x13C0, 0x13C4, 0x13C8, 0x13CC, 0x13D0, 0x13D4, 0x13D8, 0x13DC, 0x13E0, 0x13E4, 0x13E8, 0x13EC, 0x13F0, 0x13F4, 0x13F8, 0x13FC, 0x1400, 0x1404, 0x1408, 0x140C, 0x1410, 0x1414, 0x1418, 0x141C, 0x1420, 0x1424, 0x1428, 0x142C, 0x1430, 0x1434, 0x1438, 0x143C, 0x1440, 0x1444, 0x1448, 0x144C, 0x1450, 0x1454, 0x1458, 0x145C, 0x1460, 0x1464, 0x1468, 0x146C, 0x1470, 0x1474, 0x1478, 0x147C, 0x1480, 0x1484, 0x1488, 0x148C, 0x1490, 0x1494, 0x1498, 0x149C, 0x14A0, 0x14A4, 0x14A8, 0x14AC, 0x14B0, 0x14B4, 0x14B8, 0x14BC, 0x14C0, 0x14C4, 0x14C8, 0x14CC, 0x14D0, 0x14D4, 0x14D8, 0x14DC, 0x14E0, 0x14E4, 0x14E8, 0x14EC, 0x14F0, 0x14F4, 0x14F8, 0x14FC, 0x1500, 0x1504, 0x1508, 0x150C, 0x1510, 0x1514, 0x1518, 0x151C, 0x1520, 0x1524, 0x1528, 0x152C, 0x1530, 0x1534, 0x1538, 0x153C, 0x1540, 0x1544, 0x1548, 0x154C, 0x1550, 0x1554, 0x1558, 0x155C, 0x1560, 0x1564, 0x1568, 0x156C, 0x1570, 0x1574, 0x1578, 0x157C, 0x1580, 0x1584, 0x1588, 0x158C, 0x1590, 0x1594, 0x1598, 0x159C, 0x15A0, 0x15A4, 0x15A8, 0x15AC, 0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028, 0x102C, 0x1030, 0x1034, 0x1038, 0x103C, 0x1040, 0x1044, 0x1048, 0x104C, 0x1050, 0x1054, 0x1058, 0x105C, 0x1060, 0x1064, 0x1068, 0x106C, 0x1070, 0x1074, 0x1078, 0x107C, 0x1080, 0x1084, 0x1088, 0x108C, 0x1090, 0x1094, 0x1098, 0x109C, 0x10A0, 0x10A4, 0x10A8, 0x10AC, 0x10B0, 0x10B4, 0x10B8, 0x10BC, 0x10C0, 0x10C4, 0x10C8, 0x10CC, 0x10D0, 0x10D4, 0x10D8, 0x10DC, 0x10E0, 0x10E4, 0x10E8, 0x10EC, 0x10F0, 0x10F4, 0x10F8, 0x10FC, 0x1100, 0x1104, 0x1108, 0x110C, 0x1110, 0x1114, 0x1118, 0x111C, 0x1120, 0x1124, 0x1128, 0x112C, 0x1130, 0x1134, 0x1138, 0x113C, 0x1140, 0x1144, 0x1148, 0x114C, 0x1150, 0x1154, 0x1158, 0x115C, 0x1160, 0x1164, 0x1168, 0x116C, 0x1170, 0x1174, 0x1178, 0x117C, 0x1180, 0x1184, 0x1188, 0x118C, 0x1190, 0x1194, 0x1198, 0x119C, 0x11A0, 0x11A4, 0x11A8, 0x11AC, 0x11B0, 0x11B4, 0x11B8, 0x11BC, 0x11C0, 0x11C4, 0x11C8, 0x11CC, 0x11D0, 0x11D4, 0x11D8, 0x11DC, 0x11E0, 0x11E4, 0x11E8, 0x11EC, 0x11F0, 0x11F4, 0x11F8, 0x11FC, 0x1200, 0x1204, 0x1208, 0x120C, 0x1210, 0x1214, 0x1218, 0x121C, 0x1220, 0x1224, 0x1228, 0x122C, 0x1230, 0x1234, 0x1238, 0x123C, 0x1240, 0x1244, 0x1248, 0x124C, 0x1250, 0x1254, 0x1258, 0x125C, 0x1260, 0x1264, 0x1268, 0x126C, 0x1270, 0x1274, 0x1278, 0x127C, 0x1280, 0x1284, 0x1288, 0x128C, 0x1290, 0x1294, 0x1298, 0x129C, 0x12A0, 0x12A4, 0x12A8, 0x12AC, 0x12B0, 0x12B4, 0x12B8, 0x12BC, 0x12C0, 0x12C4, 0x12C8, 0x12CC, 0x12D0, 0x12D4, 0x12D8, 0x12DC, 0x12E0, 0x12E4, 0x12E8, 0x12EC, 0x12F0, 0x12F4, 0x12F8, 0x12FC, 0x1300, 0x1304, 0x1308, 0x130C, 0x1310, 0x1314, 0x1318, 0x131C, 0x1320, 0x1324, 0x1328, 0x132C, 0x1330, 0x1334, 0x1338, 0x133C, 0x1340, 0x1344, 0x1348, 0x134C, 0x1DA8, 0x1DAC, 0x1DB0, 0x1DB4, 0x1DB8, 0x1DBC, 0x1DC0, 0x1DC4, 0x1DC8, 0x1DCC, 0x1DD0, 0x1DD4, 0x1DD8, 0x1DDC, 0x1DE0, 0x1DE4, 0x1DE8, 0x1DEC, 0x1DF0, 0x1DF4, 0x1DF8, 0x1DFC, 0x1E00, 0x1E04, 0x1E08, 0x1E0C, 0x1E10, 0x1E14, 0x1E18, 0x1E1C, 0x1E20, 0x1E24, 0x1E28, 0x1E2C, 0x1E30, 0x1E34, 0x1E38, 0x1E3C, 0x1E40, 0x1E44, 0x1E48, 0x1E4C, 0x1E50, 0x1E54, 0x1E58, 0x1E5C, 0x1E60, 0x1E64, 0x1E68, 0x1E6C, 0x1E70, 0x1E74, 0x1E78, 0x1E7C, 0x1E80, 0x1E84, 0x1E88, 0x1E8C, 0x1E90, 0x1E94, 0x1E98, 0x1E9C, 0x1EA0, 0x1EA4, 0x1EA8, 0x1EAC, 0x1EB0, 0x1EB4, 0x1EB8, 0x1EBC, 0x1EC0, 0x1EC4, 0x1EC8, 0x1ECC, 0x1ED0, 0x1ED4, 0x1ED8, 0x1EDC, 0x1EE0, 0x1EE4, 0x1EE8, 0x1F50, 0x1F54, 0x1F58, 0x1F5C, 0x1F60, 0x1F64, 0x1F68, 0x1F6C, 0x1F70, 0x1F74, 0x1F78, 0x1F7C, 0x1F80, 0x1F84, 0x1F88, 0x1F8C, 0x1F90, 0x1F94, 0x1F98, 0x1F9C, 0x1FA0, 0x1FA4, 0x1FA8, 0x1FAC, 0x1FB0, 0x1FB4, 0x1FB8, 0x1FBC, 0x1FC0, 0x1FC4, 0x1FC8, 0x1FCC, 0x1FD0, 0x1FD4, 0x1FD8, 0x1FDC, 0x1FE0, 0x1FE4, 0x1FE8, 0x1FEC, 0x1FF0, 0x1FF4, 0x1FF8, 0x1FFC, 0x3000, 0x3004, 0x3008, 0x300C, 0x3010, 0x3014, 0x3018, 0x301C, 0x3020, 0x3024, 0x3028, 0x302C, 0x3030, 0x3034, 0x3038, 0x303C, 0x3040, 0x3044, 0x3048, 0x304C, 0x3050, 0x3054, 0x3058, 0x305C, 0x3060, 0x3064, 0x3068, 0x306C, 0x3070, 0x3074, 0x3078, 0x307C, 0x3080, 0x3084, 0x3088, 0x308C, 0x3090, 0x3094, 0x3098, 0x309C, 0x30A0, 0x30A4, 0x30A8, 0x30AC, 0x30B0, 0x30B4, 0x30B8, 0x30BC, 0x30C0, 0x30C4, 0x30C8, 0x30CC, 0x30D0, 0x30D4, 0x30D8, 0x30DC, 0x30E0, 0x30E4, 0x30E8, 0x30EC, 0x30F0, 0x30F4, 0x319C, 0x31A0, 0x31A4, 0x31A8, 0x31AC, 0x31B0, 0x31B4, 0x31B8, 0x31BC, 0x31C0, 0x31C4, 0x31C8, 0x31CC, 0x31D0, 0x31D4, 0x31D8, 0x31DC, 0x31E0, 0x31E4, 0x31E8, 0x31EC, 0x31F0, 0x31F4, 0x31F8, 0x31FC, 0x3200, 0x3204, 0x3208, 0x320C, 0x3210, 0x3214, 0x3218, 0x321C, 0x3220, 0x3224, 0x3228, 0x322C, 0x3230, 0x3234, 0x3238, 0x323C, 0x3240, 0x3244, 0x3248, 0x324C, 0x3250, 0x3254, 0x3258, 0x325C, 0x3260, 0x3264, 0x3268, 0x326C, 0x3270, 0x3274, 0x3278, 0x327C, 0x3280, 0x3284, 0x3288, 0x328C, 0x3290, 0x3294, 0x3298, 0x329C, 0x32A0, 0x32A4, 0x32A8, 0x32AC, 0x32B0, 0x32B4, 0x32B8, 0x32BC, 0x32C0, 0x32C4, 0x32C8, 0x32CC, 0x32D0, 0x32D4, 0x32D8, 0x32DC, 0x32E0, 0x32E4, 0x32E8, 0x32EC, 0x32F0, 0x32F4, 0x32F8, 0x32FC, 0x3300, 0x3304, 0x3308, 0x330C, 0x3310, 0x3314, 0x3318, 0x331C, 0x3320, 0x3324, 0x3328, 0x332C, 0x3330, 0x3334, 0x3338, 0x333C, 0x3340, 0x3344, 0x3348, 0x334C, 0x3350, 0x3354, 0x3358, 0x335C, 0x3360, 0x3364, 0x3368, 0x336C, 0x3370, 0x3374, 0x3378, 0x337C, 0x3380, 0x3384, 0x3388, 0x338C, 0x3390, 0x3394, 0x3398, 0x339C, 0x33A0, 0x33A4, 0x33A8, 0x33AC, 0x33B0, 0x33B4, 0x33B8, 0x33BC, 0x33C0, 0x33C4, 0x1BC8, 0x1BCC, 0x1BD0, 0x1BD4, 0x1BD8, 0x1BDC, 0x1BE0, 0x1BE4, 0x1BE8, 0x1BEC, 0x1BF0, 0x1BF4, 0x1BF8, 0x1BFC, 0x1C00, 0x1C04, 0x1C08, 0x1C0C, 0x1C10, 0x1C14, 0x1C18, 0x1C1C, 0x1C20, 0x1C24, 0x1C28, 0x1C2C, 0x1C30, 0x1C34, 0x1C38, 0x1C3C, 0x1C40, 0x1C44, 0x1C48, 0x1C4C, 0x1C50, 0x1C54, 0x1C58, 0x1C5C, 0x1C60, 0x1C64, 0x313C, 0x3140, 0x3144, 0x3148, 0x314C, 0x3150, 0x3154, 0x3158, 0x315C, 0x3160, 0x3164, 0x3168, 0x316C, 0x3170, 0x3174, 0x3178, 0x317C, 0x3180, 0x3184, 0x3188, 0x318C, 0x3190, 0x3194, 0x3198, 0x30F8, 0x30FC, 0x3100, 0x3104, 0x3108, 0x310C, 0x3110, 0x3114, 0x3118, 0x311C, 0x3120, 0x3124, 0x3128, 0x312C, 0x3130, 0x3134, 0x3138, 0xFE0F, 0xFE16, 0x17AC, 0x17B0, 0x17B4, 0x17B8, 0x17BC, 0x17C0, 0x17C4, 0x17C8, 0x17CC, 0x17D0, 0x17D4, 0x17D8, 0x17DC, 0x17E0, 0x17E4, 0x17E8, 0x17EC, 0x17F0, 0x17F4, 0x17F8, 0x17FC, 0x1800, 0x1804, 0x1808, 0x180C, 0x1810, 0x1814, 0x1818, 0x181C, 0x1820, 0x1824, 0x1828, 0x182C, 0x1830, 0x1834, 0x1838, 0x183C, 0x1840, 0x1844, 0x1848, 0x184C, 0x1850, 0x1854, 0x1858, 0x185C, 0x1860, 0x1864, 0x1868, 0x186C, 0x1870, 0x1874, 0x1878, 0x187C, 0x1880, 0x1884, 0x1888, 0x188C, 0x1890, 0x1894, 0x1898, 0x189C, 0x18A0, 0x18A4, 0x18A8, 0x18AC, 0x18B0, 0x18B4, 0x18B8, 0x18BC, 0x18C0, 0x18C4, 0x18C8, 0x18CC, 0x18D0, 0x18D4, 0x18D8, 0x18DC, 0x18E0, 0x18E4, 0x18E8, 0x18EC, 0x18F0, 0x18F4, 0x18F8, 0x18FC, 0x1900, 0x1904, 0x1908, 0x190C, 0x1910, 0x1914, 0x1918, 0x191C, 0x1920, 0x1924, 0x1928, 0x192C, 0x1930, 0x1934, 0x1938, 0x193C, 0x1940, 0x1944, 0x1948, 0x194C, 0x1950, 0x1954, 0x1958, 0x195C, 0x1960, 0x1964, 0x1968, 0x196C, 0x1970, 0x1974, 0x1978, 0x197C, 0x1980, 0x1984, 0x1988, 0x198C, 0x1990, 0x1994, 0x1998, 0x199C, 0x19A0, 0x19A4, 0x19A8, 0x19AC, 0x19B0, 0x19B4, 0x19B8, 0x19BC, 0x19C0, 0x19C4, 0x19C8, 0x19CC, 0x19D0, 0x19D4, 0x19D8, 0x19DC, 0x19E0, 0x19E4, 0x19E8, 0x19EC, 0x19F0, 0x19F4, 0x19F8, 0x19FC, 0x1A00, 0x1A04, 0x1A08, 0x1A0C, 0x1A10, 0x1A14, 0x1A18, 0x1A1C, 0x1A20, 0x1A24, 0x1A28, 0x1A2C, 0x1A30, 0x1A34, 0x1A38, 0x1A3C, 0x1A40, 0x1A44, 0x1A48, 0x1A4C, 0x1A50, 0x1A54, 0x1A58, 0x1A5C, 0x1A60, 0x1A64, 0x1A68, 0x1A6C, 0x1A70, 0x1A74, 0x1A78, 0x1A7C, 0x1A80, 0x1A84, 0x1A88, 0x1A8C, 0x1A90, 0x1A94, 0x1A98, 0x1A9C, 0x1AA0, 0x1AA4, 0x1AA8, 0x1AAC, 0x1AB0, 0x1AB4, 0x1AB8, 0x1ABC, 0x1AC0, 0x1AC4, 0x1AC8, 0x1ACC, 0x1AD0, 0x1AD4, 0x1AD8, 0x1ADC, 0x1AE0, 0x1AE4, 0x1AE8, 0x1AEC, 0x1AF0, 0x1AF4, 0x1AF8, 0x1AFC, 0x1B00, 0x1B04, 0x1B08, 0x1B0C, 0x1B10, 0x1B14, 0x1B18, 0x1B1C, 0x1B20, 0x1B24, 0x1B28, 0x1B2C, 0x1B30, 0x1B34, 0x1B38, 0x1B3C, 0x1B40, 0x1B44, 0x1B48, 0x1B4C, 0x1B50, 0x1B54, 0x1B58, 0x1B5C, 0x1B60, 0x1B64, 0x1B68, 0x1B6C, 0x1B70, 0x1B74, 0x1B78, 0x1B7C, 0x1B80, 0x1B84, 0x1B88, 0x1B8C, 0x1B90, 0x1B94, 0x1B98, 0x1B9C, 0x1BA0, 0x1BA4, 0x1BA8, 0x1BAC, 0x1BB0, 0x1BB4, 0x1BB8, 0x1BBC, 0x1BC0, 0x1BC4, 0xFE10
        };
        public static string[] Furniture_Names = new string[1102]
        {
            "Crucia Carp (Furniture)", "Brook Trout (Furniture)", "Carp (Furniture)", "Koi (Furniture)", "Catfish (Furniture)", "Small Bass (Furniture)", "Bass (Furniture)", "Large Bass (Furniture)", "Bluegill (Furniture)", "Giant Catfish (Furniture)", "Giant Snakehead (Furniture)", "Barbel Steed (Furniture)", "Dace (Furniture)", "Pale Chub (Furniture)", "Bitterling (Furniture)", "Loach (Furniture)", "Pond Smelt (Furniture)", "Sweetfish (Furniture)", "Cherry Salmon (Furniture)", "Large Char (Furniture)", "Rainbow Trout (Furniture)", "Stringfish (Furniture)", "Salmon (Furniture)", "Goldfish (Furniture)", "Piranha (Furniture)", "Arowana (Furniture)", "Eel (Furniture)", "Freshwater Goby (Furniture)", "Angelfish (Furniture)", "Guppy (Furniture)", "Popeyed Goldfish (Furniture)", "Coelacanth (Furniture)", "Crawfish (Furniture)", "Frog (Furniture)", "Killifish (Furniture)", "Jellyfish (Furniture)", "Sea Bass (Furniture)", "Red Snapper (Furniture)", "Barred Knifejaw (Furniture)", "Arapaima (Furniture)", "Tricera Skull", "Tricera Tail", "Tricera Torso", "T-rex Skull", "T-rex Tail", "T-rex Torso", "Apato Skull", "Apato Tail", "Apato Torso", "Stego Skull", "Stego Tail", "Stego Torso", "Ptera Skull", "Ptera Right Wing", "Ptera Left Wing", "Plesio Skull", "Plesio Neck", "Plesio Torso", "Mammoth Skull", "Mammoth Torso", "Amber", "Dinosaur Track", "Ammonite", "Dinosaur Egg", "Trilobite", "Modern Sofa", "Modern Table", "Blue Bed", "Blue Bench", "Blue Chair", "Blue Dresser", "Blue Bookcase", "Blue Table", "Green Bed", "Green Bench", "Green Chair", "Green Pantry", "Green Counter", "Green Lamp", "Green Table", "Cabin Bed", "Cabin Couch", "Cabin Armchair", "Cabin Bookcase", "Cabin Low Table", "Aloe", "Bromeliaceae", "Coconut Palm", "Snake Plant", "Dracaena", "Rubber Tree", "Pothos", "Fan Palm", "Grapefruit Table", "Lime Chair", "Weeping Fig", "Corn Plant", "Croton", "Pachira", "Cactus", "Metronome", "Deer Scare", "Pine Bonsai", "Mugho Bonsai", "Barber's Pole", "Ponderosa Bonsai", "Plum Bonsai", "Giant Dharma", "Dharma", "Mini-Dharma", "Quince Bonsai", "Azalea Bonsai", "Jasmine Bonsai", "Executive Toy", "Traffic Cone", "Striped Cone", "Orange Cone", "Cola Machine", "Maple Bonsai", "Hawthorne Bonsai", "Holly Bonsai", "Barricade", "Fence", "Plastic Fence", "Fence and Sign", "Soda Machine", "Manhole Cover", "Pop Machine", "Brown Drum", "Green Drum", "Red Drum", "Juice Machine", "Iron Frame", "Trash Can", "Grabage Pail", "Watermelon Chair", "Melon Chair", "Watermelon Table", "Robotic Flagman", "Garbage Can", "Trash Bin", "Violin", "Bass", "Cello", "Ebony Piano", "Zen Basin", "Handcart", "Wash Basin", "Jack-o'-Lantern", "Warning Sign", "Detour Arrow", "Garden Stone", "Standing Stone", "Route Sign", "Men Working Sign", "Caution Sign", "Temple Basin", "Spooky Bed", "Jack-in-the-Box", "Spooky Chair", "Unused Chair", "Spooky Bookcase", "Spooky Sofa", "Spooky Table", "Lunar Lander", "Satellite", "Mossy Stone", "Leaning Stone", "Dark Stone", "Flying Saucer", "Stone Couple", "Garden Pond", "Rocket", "Spaceman Sam", "Spooky Clock", "Spooky Lamp", "Exotic Bed", "Exotic End Table", "Asteroid", "Cabana Lamp", "Cabana Table", "Bucket", "Scale", "Faucet", "Spa Chair", "Cabana Screen", "Cabana Vanity", "Cabana Chair", "Cabana Bookcase", "Arwing", "Lunar Rover", "Massage Chair", "Bath Mat", "Spa Tub", "Blue Clock", "Mochi Pestle", "Spooky Vanity", "Green Desk", "Clerk's Booth", "Modern Chair", "Modern End Table", "Space Station", "Spa Screen", "Cabin Chair", "Regal Bed", "Space Shuttle", "Regal Vanity", "Regal Sofa", "Regal Lamp", "Cabin Table", "Bath Locker", "Milk Fridge", "Tea Set", "Nook's Portrait", "Gerbera", "Sunflower", "Daffodil", "Spooky Wardrobe", "Classic Wardrobe", "Blue Wardrobe", "Office Locker", "Jingle Wardrobe", "Regal Armoire", "Cabana Wardrobe", "Cabin Wardrobe", "Lovely Armoire", "Green Wardrobe", "Pear Wardrobe", "Ranch Wardrobe", "Blue Cabinet", "Modern Wardrobe", "Exotic Wardrobe", "Jingle Dresser", "Regal Dresser", "Cabana Dresser", "Cabin Dresser", "Lovely Dresser", "Spooky Dresser", "Green Dresser", "Pear Dresser", "Ranch Dresser", "Classic Vanity", "Blue Bureau", "Modern Dresser", "Exotic Bureau", "Kiddie Dresser", "Kiddie Bureau", "Kiddie Wardrobe", "Dresser", "Tansu", "Sewing Box", "Fan", "Paper Lantern", "Tea Table", "Shogi Board", "Screen", "Zabuton", "Bus Stop", "Froggy Chair", "Lilly-pad Table", "Refridgerator", "Chest", "Rack", "Red Sofa", "Red Armchair", "Hibachi", "Stove", "Cream Sofa", "Tea Tansu", "Pink Kotatsu", "Blue Kotatsu", "Folk Guitar", "Country Guitar", "Rock Guitar", "Hinaningyo", "Papa Bear", "Mama Bear", "Baby Bear", "Classic Hutch", "Classic Chair", "Classic Desk", "Classic Table", "Classic Cabinet", "Rocking Chair", "Regal Cupboard", "Writing Desk", "Keiko Figurine", "Yuki Figurine", "Yoko Figurine", "Emi Figurine", "Maki Figurine", "Naomi Figurine", "Globe", "Regal Chair", "Regal Table", "Retro TV", "Eagle Pole", "Raven Pole", "Bear Pole", "Frog Woman Pole", "Taiko Drum", "Space Heater", "Retro Stereo", "Cabana Armchair", "Classic Sofa", "Lovely End Table", "Lovely Armchair", "Ivory Piano", "Lovely Lamp", "Lovely Kitchen", "Lovely Chair", "Lovely Bed", "Classic Clock", "Cabana Bed", "Green Golf Bag", "White Golf Bag", "Blue Golf Bag", "Regal Bookcase", "Writing Chair", "Ranch Couch", "Ranch Armchair", "Ranch Tea Table", "Ranch Hutch", "Ranch Bookcase", "Ranch Chair", "Ranch Bed", "Ranch Table", "Computer", "Office Desk", "Master Sword", "N Logo", "Vibraphone", "Biwa Lute", "Conga Drum", "Extinguisher", "Ruby Econo-chair", "Gold Econo-chair", "Jade Econo-chair", "Gold Stereo", "Folding Chair", "Lovely Vanity", "Birdcage", "Timpano Drum", "Nice Speaker", "Birthday Cake", "School Desk", "Graffiti Desk", "Towel Desk", "Tall Cactus", "Round Cactus", "Classic Bed", "Wide-screen TV", "Lovely Table", "Kadomatsu", "Kagamimochi", "Low Lantern", "Tall Lantern", "Pond Lantern", "Office Chair", "Cubby Hole", "Letter Cubby", "Heavy Chair", "School Chair", "Towel Chair", "Science Table", "Stepstool", "Shrine Lantern", "Barrel", "Keg", "Vaulting Horse", "Glass-top Table", "Alarm Clock", "Tulip Table", "Daffodil Table", "Iris Table", "Blue Vase", "Tulip Chair", "Daffodil Chair", "Iris Chair", "Elephant Slide", "Toilet", "Super Toilet", "Pine Table", "Pine Chair", "Tea Vase", "Red Vase", "Sewing Machine", "Billiard Table", "Famous Painting", "Basic Painting", "Scarying Painting", "Moving Painting", "Flowery Painting", "Common Painting", "Quaint Painting", "Dainty Painting", "Amazing Painting", "Strange Painting", "Rare Painting", "Classic Painting", "Perfect Painting", "Fine Painting", "Worthy Painting", "Pineapple Bed", "Orange Chair", "Unused Dresser", "Lemon Table", "Apple TV", "Table Tennis", "Harp", "Cabin Clock", "Train Set", "Water Bird", "Wobbelina", "Unused Monkey", "Slot Machine", "Exotic Bench", "Exotic Chair", "Exotic Chest", "Exotic Lamp", "Caladium", "Lady Palm", "Exotic Screen", "Exotic Table", "Djimbe Drum", "Modern Bed", "Modern Den Chair", "Modern Cabinet", "Modern Desk", "Clu Clu Land", "Balloon Fight", "Donkey Kong", "DK Jr MATH", "Pinball", "Tennis", "Golf", "Punchout", "Baseball", "Clu Clu Land D", "Donkey Kong 3", "Donky Kong Jr", "Soccer", "Excitebike", "Wario's Woods", "Ice Climber", "Mario Bros", "Super Mario Bros", "Legend of Zelda", "NES", "Phonograph", "Turntable", "Jukebox", "Red Boom Box", "White Boom Box", "High-end Stereo", "Hi-fi Stereo", "Lovely Stereo", "Jingle Lamp", "Jingle Chair", "Jingle Shelves", "Jingle Sofa", "Jingle Bed", "Jingle Clock", "Jingle Table", "Jingle Piano", "Aiko Figurine", "Robo-Stereo", "Dice Stereo", "Apple Clock", "Robo-clock", "Kitschy Clock", "Antique Clock", "Reel-to-reel", "Tape Deck", "CD Player", "Glow Clock", "Odd Clock", "Red Clock", "Cube Clock", "Owl Clock", "Lucky Cat", "Lucky Black Cat", "Samurai Suit", "Racoon Obje", "Lucky Frog", "Big Festive Tree", "White Rook", "Black Rook", "White Queen", "Black Queen", "White Bishop", "Black Bishop", "White King", "Black King", "White Knight", "Black Knight", "White Pawn", "Black Pawn", "Festive Tree", "Kiddie Clock", "Kiddie Bed", "Kiddie Table", "Kiddie Couch", "Kiddie Stereo", "Kiddie Chair", "Kiddie Bookcase", "Alcove", "Hearth", "Chalk Board", "Mop", "Modern Lamp", "Snowman Fridge", "Snowman Table", "Snowman Bed", "Snowman Chair", "Snowman Lamp", "Snowman Sofa", "Snowman TV", "Snowman Dresser", "Snowman Wardrobe", "Snowman Clock", "Tricera D", "T-rex D", "Bronto D", "Ptera D", "HUTABAD", "Mammoth D", "Stego D", "Stego D2", "Fossil (Furniture)", "Shogi Piece", "Chocolates", "Post Box", "Piggy Bank", "Tissue", "Tribal Mask", "Matryoshka", "Legend of Zelda", "Bottled Shop", "Tiger Bobblehead", "Moai Statue", "Aerobics Radio", "Pagoda", "Fishing Bear", "Mouth of Truth", "Chinese Lioness", "Tower of Pisa", "Merlion", "Manekin Pis", "Tokyo Tower", "Red Balloon", "Yellow Balloon", "Blue Balloon", "Green Balloon", "Purple Balloon", "Bunny P. Balloon", "Bunny B. Balloon", "Bunny O. Balloon", "Lady Liberty", "Arc De Triomphe", "Stone Coin", "Mermaid Statue", "Post Model", "House Model", "Manor Model", "Police Model", "Museum Model", "Plate Armor", "Moon Dumpling", "Bean Set", "Osechi", "Lovely Phone", "Market Model", "Katrina's Tent", "Chinese Lion", "Tanabata Palm", "Spring Medal", "Fall Medal", "Shop Model", "Compass", "Long-life Noodle", "Bass Boat", "Lighthouse Model", "Life Ring", "Tree Model", "Pink Tree Model", "Weed Model", "Tailor Model", "Dump Model", "Mortar Ball", "Snowman", "Miniature Car", "Big Catch Flag", "Moon", "Locomotive Model", "Dolly", "Station Model 1", "Station Model 2", "Station Model 3", "Station Model 4", "Station Model 5", "Station Model 6", "Station Model 7", "Station Model 8", "Station Model 9", "Station Model 10", "Station Model 11", "Station Model 12", "Station Model 13", "Station Model 14", "Station Model 15", "Well Model", "Grass Model", "Track Model", "Dirt Model", "Train Car Model", "Crab Stew", "Fireplace", "Igloo Model", "Snowy Tree Model", "Snowcone Machine", "Treasure Chest", "Beach Chair", "Beach Table", "Hibachi Grill", "Surfboard", "Snowboard", "Wave Breaker", "Ukulele", "Diver Dan", "Snow Bunny", "Scary Painting", "Novel Painting", "Sleigh", "Nintendo Bench", "G Logo", "Merge Sign", "Bottle Rocket", "Wet Roadway Sign", "Detour Sign", "Men at Work Sign", "Lefty Desk", "Righty Desk", "School Desk", "Flagman Sign", "Fishing Trophy", "Jersey Barrier", "Speed Sign", "Golf Trophy", "Teacher's Desk", "Haz-mat Barrel", "Tennis Trophy", "Saw Horse", "Kart Trophy", "Bug Zapper", "Telescope", "Coffee Machine", "Bird Bath", "Barbecue", "Radiator", "Lawn Chair", "Chess Table", "Candy Machine", "Backyard Pool", "Cement Mixer", "Jackhammer", "Tiki Torch", "Birdhouse", "Potbelly Stove", "Bus Stop", "Hamster Cage", "Flip-top Desk", "Festive Flag", "Super Tortimer", "Bird Feeder", "Teacher's Chair", "Steam roller", "Mr. Flamingo", "Mailbox", "Festive Candle", "Hammock", "Garden Gnome", "Mrs. Flamingo", "Spring Medal (again?)", "Autumn Medal", "Tumbleweed", "Cow Skull", "Oil Drum", "Saddle Fence", "Western Fence", "Watering Trough", "Luigi Trophy", "Mario Trophy", "Harvest Lamp", "Covered Wagon", "Storefront", "Picnic Table", "Harvest Table", "Harvest TV", "Harvest Bed", "Harvest Chair", "Harvest Clock", "Harvest Sofa", "Green Pipe", "Brick Block", "Harvest Bureau", "Flagpole", "Harvest Dresser", "Super Mushroom", "Harvest Mirror", "Coin", "? Block", "Starman", "Koopa Shell", "Cannon", "Desert Cactus", "Fire Flower", "Wagon Wheel", "Well", "Boxing Barricade", "Neutral Corner", "Red Corner", "Blue Corner", "Boxing Mat", "Ringside Table", "Speed Bag", "Sandbag", "Weight Bench", "Campfire", "Bonfire", "Kayak", "Sprinkler", "Tent Model", "Backpack", "Angler Trophy", "Pansy Model 1", "Pansy Model 2", "Pansy Model 3", "Cosmos Model 1", "Cosmos Model 2", "Cosmos Model 3", "Tulip Model 1", "Tulip Model 2", "Tulip Model 3", "Lantern", "Lawn Mower", "Cooler", "Mountain Bike", "Sleeping Bag", "Propane Stove", "Cornucopia", "Judge's Bell", "Noisemaker", "Chowder", "DUMMY", "Common Butterfly (Furniture)", "Yellow Butterfly (Furniture)", "Tiger Butterfly (Furniture)", "Purple Butterfly (Furniture)", "Robust Cicada (Furniture)", "Walker Cicada (Furniture)", "Evening Cicada (Furniture)", "Brown Cicada (Furniture)", "Bee (Furniture)", "Common Dragonfly (Furniture)", "Red Dragonfly (Furniture)", "Darner Dragonfly (Furniture)", "Banded Dragonfly (Furniture)", "Long Locust (Furniture)", "Migratory Locust (Furniture)", "Cricket (Furniture)", "Grasshopper (Furniture)", "Bell Cricket (Furniture)", "Pine Cricket (Furniture)", "Drone Beetle (Furniture)", "Dynastid Beetle (Furniture)", "Flat Stag Beetle (Furniture)", "Jewel Beetle (Furniture)", "Longhorn Beetle (Furniture)", "Ladybug (Furniture)", "Spotted Ladybug (Furniture)", "Mantis (Furniture)", "Firefly (Furniture)", "Cockroach (Furniture)", "Saw Stag Beetle (Furniture)", "Mountain Beetle (Furniture)", "Giant Beetle (Furniture)", "Snail (Furniture)", "Mole Cricket (Furniture)", "Pond Skater (Furniture)", "Bagworm (Furniture)", "Pill Bug (Furniture)", "Spider (Furniture)", "Ant (Furniture)", "Mosquito (Furniture)", "Golden Net", "Golden Axe (Furniture)", "Golden Shovel (Furniture)", "Golden Rod (Furniture)", "Bluebell Fan (Furniture)", "Plum Fan (Furniture)", "Bamboo Fan (Furniture)", "Cloud Fan (Furniture)", "Maple Fan (Furniture)", "Fan Fan (Furniture)", "Flower Fan (Furniture)", "Leaf Fan (Furniture)", "Yellow Pinwheel (Furniture)", "Red Pinwheel (Furniture)", "Tiger Pinwheel (Furniture)", "Green Pinwheel (Furniture)", "Pink Pinwheel (Furniture)", "Striped Pinwheel (Furniture)", "Flower Pinwheel (Furniture)", "Fancy Pinwheel (Furniture)", "Net (Furniture)", "Axe (Furniture)", "Shovel (Furniture)", "Fishing Rod (Furniture)", "Orange Box", "College Rule (Furniture)", "School Pad (Furniture)", "Organizer (Furniture)", "Diary (Furniture)", "Journal (Furniture)", "Pink Diary (Furniture)", "Captain's Log (Furniture)", "Blue Diary (Furniture)", "French Notebook (Furniture)", "Scroll (Furniture)", "Pink Plaid Pad (Furniture)", "Blue Polka Pad (Furniture)", "Green Plaid Pad (Furniture)", "Red Polka Pad (Furniture)", "Yellow Plaid Pad (Furniture)", "Calligraphy Pad (Furniture)", "Empty Manniquin", "Sold Out Sign (2)", "Flame Shirt (Model)", "Paw Shirt (Model)", "Wavy Pink Shirt (Model)", "Furture Shirt (Model)", "Bold Check Shirt (Model)", "Mint Gingham (Model)", "Bad Plaid Shirt (Model)", "Speedway Shirt (Model)", "Folk Shirt (Model)", "Daisy Shirt (Model)", "Wavy Tan Shirt (Model)", "Optical Shirt (Model)", "Rugby Shirt (Model)", "Sherbet Gingham (Model)", "Yellow Tartan (Model)", "Gelato Shirt (Model)", "Work Uniform (Model)", "Patched Shirt (Model)", "Plum Kimono (Model)", "Somber Robe (Model)", "Red Sweatsuit (Model)", "Blue Sweatsuit (Model)", "Red Puffy Vest (Model)", "Blue Puffy Vest (Model)", "Summer Robe (Model)", "Bamboo Robe (Model)", "Red Aloha Shirt (Model)", "Blue Aloha Shirt (Model)", "Dark Polka Shirt (Model)", "Lite Polka Shirt (Model)", "Lovely Shirt (Model)", "Citrus Shirt (Model)", "Kiwi Shirt (Model)", "Watermelon Shirt (Model)", "Strawberry Shirt (Model)", "Grape Shirt (Model)", "Melon Shirt (Model)", "Jingle Shirt (Model)", "Blossom Shirt (Model)", "Icy Shirt (Model)", "Crewel Shirt (Model)", "Tropical Shirt (Model)", "Ribbon Shirt (Model)", "Fall Plaid Shirt (Model)", "Fiendish Shirt (Model)", "Chevron Shirt (Model)", "Ladybug Shirt (Model)", "Botanical Shirt (Model)", "Anju's Shirt (Model)", "Kaffe's Shirt (Model)", "Lavender Robe (Model)", "Blue Grid Shirt (Model)", "Butterfly Shirt (Model)", "Blue Tartan (Model)", "Gracie's Top (Model)", "Orange Tie-Die (Model)", "Purple Tie-Die (Model)", "Green Tie-Die (Model)", "Blue Tie-Die (Model)", "Red Tie-Die (Model)", "One-Ball Shirt (Model)", "Two-Ball Shirt (Model)", "Three-Ball Shirt (Model)", "Four-Ball Shirt (Model)", "Five-Ball Shirt (Model)", "Six-Ball Shirt (Model)", "Seven-Ball Shirt (Model)", "Eight-Ball Shirt (Model)", "Nine-Ball Shirt (Model)", "Arctic Camo (Model)", "Jungle Camo (Model)", "Desert Camo (Model)", "Rally Shirt (Model)", "Racer Shirt (Model)", "Racer 6 Shirt (Model)", "Fish Bone Shirt (Model)", "Spiderweb Shirt (Model)", "Zipper Shirt (Model)", "Bubble Shirt (Model)", "Yellow Bolero (Model)", "Nebula Shirt (Model)", "Neo-Classic Knit (Model)", "Noble Shirt (Model)", "Turnip Top (Model)", "Oft-Seen Print (Model)", "Ski Sweater (Model)", "Circus Shirt (Model)", "Patchwork Top (Model)", "Mod Top (Model)", "Hippie Shirt (Model)", "Rickrack Shirt (Model)", "Diner Uniform (Model)", "Shirt Circuit (Model)", "U R Here Shirt (Model)", "Yodel Shirt (Model)", "Pulse Shirt (Model)", "Prism Shirt (Model)", "Star Shirt (Model)", "Straw Shirt (Model)", "Noodle Shirt (Model)", "Dice Shirt (Model)", "Kiddie Shirt (Model)", "Frog Shirt (Model)", "Moody Blue Shirt (Model)", "Cloudy Shirt (Model)", "Fortune Shirt (Model)", "Skull Shirt (Model)", "Desert Shirt (Model)", "Aurora Knit (Model)", "Winter Sweater (Model)", "Go-Go Shirt (Model)", "Jade Check Print (Model)", "Blue Check Print (Model)", "Red Grid Shirt (Model)", "Flicker Shirt (Model)", "Floral Knit (Model)", "Rose Shirt (Model)", "Sunset Top (Model)", "Chain-Gang Shirt (Model)", "Spring Shirt (Model)", "Bear Shirt (Model)", "MVP Shirt (Model)", "Silk Bloom Shirt (Model)", "Pop Bloom Shirt (Model)", "Loud Bloom Shirt (Model)", "Hot Spring Shirt (Model)", "New Spring Shirt (Model)", "Deep Blue Tee (Model)", "Snowcone Shirt (Model)", "Ugly Shirt (Model)", "Sharp Outfit (Model)", "Painter's Smock (Model)", "Spade Shirt (Model)", "Blossoming Shirt (Model)", "Peachy Shirt (Model)", "Static Shirt (Model)", "Rainbow Shirt (Model)", "Groovy Shirt (Model)", "Loud Line Shirt (Model)", "Dazed Shirt (Model)", "Red Bar Shirt (Model)", "Blue Stripe Knit (Model)", "Earthy Knit (Model)", "Spunky Knit (Model)", "Deer Shirt (Model)", "Blue Check Shirt (Model)", "Light Line Shirt (Model)", "Blue Pinestripe (Model)", "Diamond Shirt (Model)", "Lime Line Shirt (Model)", "Big Bro's Shirt (Model)", "Green Bar Shirt (Model)", "Yellow Bar Shirt (Model)", "Monkey Shirt (Model)", "Polar Fleece (Model)", "Ancient Knit (Model)", "Fish Knit (Model)", "Vertigo Shirt (Model)", "Misty Shirt (Model)", "Stormy Shirt (Model)", "Red Scale Shirt (Model)", "Blue Scale Shirt (Model)", "Heart Shirt (Model)", "Yellow Pinstripe (Model)", "Club Shirt (Model)", "Li'l Bro's Shirt (Model)", "Argyle Knit (Model)", "Caveman Tunic (Model)", "Café Shirt (Model)", "Tiki Shirt (Model)", "A Shirt (Model)", "Checkered Shirt (Model)", "No. 1 Shirt (Model)", "No. 2 Shirt (Model)", "No. 3 Shirt (Model)", "No. 4 Shirt (Model)", "No. 5 Shirt (Model)", "No. 23 Shirt (Model)", "No. 67 Shirt (Model)", "BB Shirt (Model)", "Beatnik Shirt (Model)", "Moldy Shirt (Model)", "Houndstooth Tee (Model)", "Big Star Shirt (Model)", "Orange Pinstripe (Model)", "Twinkle Shirt (Model)", "Funky Dot Shirt (Model)", "Crossing Shirt (Model)", "Splendid Shirt (Model)", "Jagged Shirt (Model)", "Denim Shirt (Model)", "Cherry Shirt (Model)", "Gumdrop Shirt (Model)", "Barber Shirt (Model)", "Concierge Shirt (Model)", "Fresh Shirt (Model)", "Far-Out Shirt (Model)", "Dawn Shirt (Model)", "Striking Outfit (Model)", "Red Check Shirt (Model)", "Berry Gingham (Model)", "Lemon Gingham (Model)", "Dragon Suit (Model)", "G Logo Shirt (Model)", "Tin Shirt (Model)", "Jester Shirt (Model)", "Pink Tartan (Model)", "Waffle Shirt (Model)", "Gray Tartan (Model)", "Windsock Shirt (Model)", "Trendy Top (Model)", "Green Ring Shirt (Model)", "White Ring Shirt (Model)", "Snappy Print (Model)", "Chichi Print (Model)", "Wave Print (Model)", "Checkerboard Tee (Model)", "Subdued Print (Model)", "Airy Shirt (Model)", "Coral Shirt (Model)", "Leather Jerkin (Model)", "Zebra Print (Model)", "Tiger Print (Model)", "Cow Print (Model)", "Leopard Print (Model)", "Danger Shirt (Model)", "Big Dot Shirt (Model)", "Puzzling Shirt (Model)", "Exotic Shirt (Model)", "Houndstooth Knit (Model)", "Uncommon Shirt (Model)", "Dapper Shirt (Model)", "Gaudy Sweater (Model)", "Cozy Sweater (Model)", "Comfy Sweater (Model)", "Classic Top (Model)", "Vogue Top (Model)", "Laced Shirt (Model)", "Natty Shirt (Model)", "Citrus Gingham (Model)", "Cool Shirt (Model)", "Dreamy Shirt (Model)", "Flowery Shirt (Model)", "Caterpillar Tee (Model)", "Shortcake Shirt (Model)", "Whirly Shirt (Model)", "Thunder Shirt (Model)", "Giraffe Print (Model)", "Swell Shirt (Model)", "Toad Print (Model)", "Grass Shirt (Model)", "Mosaic Shirt (Model)", "Fetching Outfit (Model)", "Snow Shirt (Model)", "Melon Gingham (Model)", "My Mannenquin 1 (Model)", "My Mannenquin 2 (Model)", "My Mannenquin 3 (Model)", "My Mannenquin 4 (Model)", "My Mannenquin 5 (Model)", "My Mannenquin 6 (Model)", "My Mannenquin 7 (Model)", "My Mannenquin 8 (Model)", "Sold Out Sign (1)"
        };

        //Villager Houses are 50XX (Where XX is the Villager Identification Byte) Ex: Mitzi > 5002
        //0x580A = Train (Front) < Becomes invisible/is removed
        //0x580B = Train (Caboose) < Causes game to crash
        //0x5853+ = Player (1?) Model. Resets game after loading model.

        public static List<ushort> acreItemIDs = new List<ushort> {
            0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 0x0008, 0x0009, 0x000A, 0x000B, 0x000C, 0x000D, 0x000E, 0x000F, 0x0010,
            0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017, 0x0018, 0x0019, 0x001A, 0x001B, 0x001C, 0x001D, 0x001E, 0x001F, 0x0020,
            0x0028, 0x002A, 0x0030, 0x0038, 0x0040, 0x0048, 0x0050, 0x005C, 0x005E, 0x005F, 0x0060, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067,
            0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077,
            0x0078, 0x0079, 0x007A, 0x007B, 0x007C, 0x007D, 0x007E, 0x007F, 0x0080, 0x0081, 0x0082, 0x0083, 0x0084, 0x0085, 0x0086, 0x0087,
            0x0088, 0x00BF, 0x0800, 0x0802, 0x0804, 0x0805, 0x0806, 0x0807, 0x0808, 0x080C, 0x0814, 0x081C, 0x0824, 0x082C, 0x082D, 0x0831,
            0x0834, 0x0836, 0x083B, 0x0845, 0x0846, 0x0847, 0x0848, 0x0849, 0x084A, 0x084B, 0x084C, 0x084D, 0x084E, 0x085B, 0x0861, 0x0862,
            0x0867, 0x0868, 0x0900, 0x0901, 0x0902, 0x0903, 0x0904, 0x0905, 0x0906, 0x0907, 0x0908, 0x0909, 0x090A, 0x090B, 0x090C, 0x090D,
            0x090E, 0x090F, 0x0910, 0x0911, 0x0912, 0x0913, 0x0914, 0x0915, 0x0916, 0x0917, 0x0918, 0x0919, 0x091A, 0x091B, 0x091C, 0x091D,
            0x091E, 0x091F, 0x0920, 0x5800, 0x5801, 0x5802, 0x5803, 0x5804, 0x5805, 0x5806, 0x5807, 0x5808, 0x5809, 0x580C, 0x580D, 0x580E,
            0x580F, 0x5810, 0x5811, 0x5812, 0x5813, 0x5814, 0x5815, 0x5816, 0x5817, 0x5818, 0x5819, 0x581A, 0x581B, 0x581C, 0x581D, 0x581E,
            0x581F, 0x5820, 0x5821, 0x5822, 0x5823, 0x5824, 0x5825, 0x5826, 0x5827, 0x5828, 0x5829, 0x582A, 0x582B, 0x582C, 0x582D, 0x582E,
            0x582F, 0x5830, 0x5831, 0x5832, 0x5833, 0x5834, 0x5835, 0x5836, 0x5837, 0x5838, 0x5839, 0x583A, 0x583B, 0x5841, 0x5842, 0x5843,
            0x5844, 0x5845, 0x5846, 0x5847, 0x5848, 0x5849, 0x584A, 0x584B, 0x584C, 0x584D, 0x584E, 0x584F, 0x5850, 0x5851, 0x5852, 0xA000,
            0xA001, 0xA002, 0xA003, 0xA004, 0xA005, 0xA006, 0xA007, 0xA011, 0xA012, 0xFE1D, 0xFE1E, 0xFFFF,
        };
        public static List<string> acreItemNames = new List<string>
        {
            "Tree Stump (Small)", "Tree Stump (Medium)", "Tree Stump (Large)", "Tree Stump (Fully Grown)", "Fence (Type 1)", "Fence (Type 2)", "Message Board (B)", "Weed", "Weed", "Weed", "Message Board (A)", "Map Board (B)", "Map Board (A)", "Music Board (B)", "Music Board (A)", "Wooden Fence",
            "Hole", "Hole (Angled Down)", "Hole (Angled Up)", "Hole (Angled Right)", "Hole (Angled Left)", "Hole (Angled Down & Right)", "Hole (Angled Down & Left)", "Hole (Angled Up & Right)", "Hole (Angled Up & Left)", "Hole (Right)", "Hole (Left)", "Hole (Right)", "Hole (Left)", "Hole (Right)", "Hole (Left)", "Hole (Bent & Angled Right)",
            "Hole (Bent & Angled Down)", "Buried Pitfall", "Buried Pitfall (Angled Down-Right)", "Buried Pitfall (Angled)", "Buried Pitfall (Bent Left)", "Buried Pitfall (Bent Down-Left)", "Buried Pitfall (Bent Left & Slightly Down)", "Glowing Spot", "Tree (Bees)", "Tree (Furniture)", "Tree (Festive Lights)", "Rock (Type 1)", "Rock (Type 2)", "Rock (Type 3)", "Rock (Type 4)", "Rock (Type 5)",
            "Rock (Unused)", "Tree (Bells)", "Red Rock (Type 1)", "Red Rock (Type 2)", "Red Rock (Type 3)", "Red Rock (Type 4)", "Red Rock (Type 5)", "Red Rock (Unused)", "Chopped Palm Tree (Small)", "Chopped Palm Tree (Medium)", "Chopped Palm Tree (Large)", "Chopped Palm Tree (Full)", "Chopped Cedar Tree (Small)", "Chopped Cedar Tree (Medium)", "Chopped Cedar Tree (Large)", "Chopped Cedar Tree (Full)",
            "Cedar Tree (Bells)", "Cedar Tree (Furniture)", "Cedar Tree (Bees)", "Chopped Tree (Small)", "Chopped Tree (Medium)", "Chopped Tree (Large)", "Chopped Tree (Full)", "Golden Tree w/ 100 Bells", "Golden Tree w/ Furniture", "Golden Tree w/ Bees", "Cedar Tree (Festive Lights)", "Sapling (Doesn't Grow)", "Small Tree (^)", "Medium Tree (^)", "Large Tree (^)", "Fully Grown Tree (^)",
            "Sapling (^)", "Flower Base", "Sapling", "Tree (Growing)", "Tree", "Apple Tree Sapling", "Small Apple Tree", "Medium Apple Tree", "Large Apple Tree", "Apple Tree (Fruit)", "Orange Tree (Fruit)", "Peach Tree (Fruit)", "Pear Tree (Fruit)", "Cherry Tree(Fruit)", "Sapling", "Money Tree (1,000 Bells)",
            "Medium Tree", "Money Tree (10,000) Bells", "Money Tree (30,000 Bells)", "White Pansies", "Purple Pansies", "Yellow Pansies", "Yellow Cosmos", "Purple Cosmos", "Blue Cosmos", "Red Tulips", "White Tulips", "Yellow Tulips", "Dead Sapling (1)", "Palm Tree (Fruit)", "Cedar Tree", "Dead Sapling (2)",
            "Golden Tree (Golden Shovel)", "Golden Tree","Placed Signboard w/ Player 1's Pattern #1", "Placed Signboard w/ Player 1's Pattern #2", "Placed Signboard w/ Player 1's Pattern #3", "Placed Signboard w/ Player 1's Pattern #4", "Placed Signboard w/ Player 1's Pattern #5", "Placed Signboard w/ Player 1's Pattern #6", "Placed Signboard w/ Player 1's Pattern #7", "Placed Signboard w/ Player 1's Pattern #8", "Placed Signboard w/ Player 2's Pattern #1", "Placed Signboard w/ Player 2's Pattern #2", "Placed Signboard w/ Player 2's Pattern #3", "Placed Signboard w/ Player 2's Pattern #4", "Placed Signboard w/ Player 2's Pattern #5", "Placed Signboard w/ Player 2's Pattern #6",
            "Placed Signboard w/ Player 2's Pattern #7", "Placed Signboard w/ Player 2's Pattern #8", "Placed Signboard w/ Player 3's Pattern #1", "Placed Signboard w/ Player 3's Pattern #2", "Placed Signboard w/ Player 3's Pattern #3", "Placed Signboard w/ Player 3's Pattern #4", "Placed Signboard w/ Player 3's Pattern #5", "Placed Signboard w/ Player 3's Pattern #6", "Placed Signboard w/ Player 3's Pattern #7", "Placed Signboard w/ Player 3's Pattern #8", "Placed Signboard w/ Player 4's Pattern #1", "Placed Signboard w/ Player 4's Pattern #2", "Placed Signboard w/ Player 4's Pattern #3", "Placed Signboard w/ Player 4's Pattern #4", "Placed Signboard w/ Player 4's Pattern #5", "Placed Signboard w/ Player 4's Pattern #6",
            "Placed Signboard w/ Player 4's Pattern #7", "Placed Signboard w/ Player 4's Pattern #8", "Placed Signboard",
            "Upper Left Player House", "Upper Right Player House", "Lower Left Player House", "Lower Right Player House", "Nook's Cranny", "Nook 'n' Go", "Nookway", "Nookington's", "Post Office", "Train Station (Right)", "Police Station", "Waterfall", "Waterfall (Right)", "Waterfall (Left)",
            "Signboard (1)", "Signboard (2)", "Signboard (3)", "Signboard (4)", "Signboard (5)", "Signboard (6)", "Signboard (7)", "Signboard (8)", "Signboard (9)", "Signboard (10)", "Signboard (11)", "Signboard (12)", "Signboard (13)", "Signboard (14)", "Signboard (15)", "Signboard (16)",
            "Signboard (17)", "Signboard (18)", "Signboard (19)", "Signboard (20)", "Signboard (21)", "Wishing Well", "Crazy Redd's Tent", "Katrina's Tent", "Gracie's Car", "Igloo", "Cherry Festival Table #1", "Cherry Festival Table #2", "Aerobics Radio", "Redd's Stall (Right)", "Redd's Stall (Left)", "Katrina's Shrine (Right)",
            "Katrina's Shrine (Left)", "Katrina's New Years Table", "New Years Clock (Part 1)", "New Years Clock (Part 2)", "Red Sports Fair Balls", "White Sports Fair Balls", "Red Sports Fair Basket", "White Sports Fair Basket", "Fish Check Stand (Right)", "Fish Check Stand (Left)", "Fish Windsock", "Dump", "Lily Pads", "K.K. Slider's Box", "Train Station (Left)", "Lighthouse",
            "Tortimer's Stand (Groundhog Day)", "Cherry Blossom Festival Table #1", "Cherry Blossom Festival Table #2", "Harvest Festival Table", "Camping Tent", "Museum", "Suspension Bridge (/)", "Suspension Bridge (\\)", "Tailor's Shop", "Island Flag", "Kapp'n w/ Boat", "Player's Island House", "Islander's House", "Dock Signboard", "Upper Left House's Mailbox", "Upper Right House's Mailbox",
            "Lower Left House's Mailbox", "Lower Right House's Mailbox", "Upper Left House's Gyroid", "Upper Right House's Gyroid", "Lower Left House's Gyroid", "Lower Right House's Gyroid", "Train Caboose Door", "Villager Signboard", "Possible Bridge Location (/)", "Possible Bridge Location (\\)", "Occupied/Unavailable",
        };

        public static ushort[] Shirt_IDs = new ushort[255] { 0x2400, 0x2401, 0x2402, 0x2403, 0x2404, 0x2405, 0x2406, 0x2407, 0x2408, 0x2409, 0x240A, 0x240B, 0x240C, 0x240D, 0x240E, 0x240F, 0x2410, 0x2411, 0x2412, 0x2413, 0x2414, 0x2415, 0x2416, 0x2417, 0x2418, 0x2419, 0x241A, 0x241B, 0x241C, 0x241D, 0x241E, 0x241F, 0x2420, 0x2421, 0x2422, 0x2423, 0x2424, 0x2425, 0x2426, 0x2427, 0x2428, 0x2429, 0x242A, 0x242B, 0x242C, 0x242D, 0x242E, 0x242F, 0x2430, 0x2431, 0x2432, 0x2433, 0x2434, 0x2435, 0x2436, 0x2437, 0x2438, 0x2439, 0x243A, 0x243B, 0x243C, 0x243D, 0x243E, 0x243F, 0x2440, 0x2441, 0x2442, 0x2443, 0x2444, 0x2445, 0x2446, 0x2447, 0x2448, 0x2449, 0x244A, 0x244B, 0x244C, 0x244D, 0x244E, 0x244F, 0x2450, 0x2451, 0x2452, 0x2453, 0x2454, 0x2455, 0x2456, 0x2457, 0x2458, 0x2459, 0x245A, 0x245B, 0x245C, 0x245D, 0x245E, 0x245F, 0x2460, 0x2461, 0x2462, 0x2463, 0x2464, 0x2465, 0x2466, 0x2467, 0x2468, 0x2469, 0x246A, 0x246B, 0x246C, 0x246D, 0x246E, 0x246F, 0x2470, 0x2471, 0x2472, 0x2473, 0x2474, 0x2475, 0x2476, 0x2477, 0x2478, 0x2479, 0x247A, 0x247B, 0x247C, 0x247D, 0x247E, 0x247F, 0x2480, 0x2481, 0x2482, 0x2483, 0x2484, 0x2485, 0x2486, 0x2487, 0x2488, 0x2489, 0x248A, 0x248B, 0x248C, 0x248D, 0x248E, 0x248F, 0x2490, 0x2491, 0x2492, 0x2493, 0x2494, 0x2495, 0x2496, 0x2497, 0x2498, 0x2499, 0x249A, 0x249B, 0x249C, 0x249D, 0x249E, 0x249F, 0x24A0, 0x24A1, 0x24A2, 0x24A3, 0x24A4, 0x24A5, 0x24A6, 0x24A7, 0x24A8, 0x24A9, 0x24AA, 0x24AB, 0x24AC, 0x24AD, 0x24AE, 0x24AF, 0x24B0, 0x24B1, 0x24B2, 0x24B3, 0x24B4, 0x24B5, 0x24B6, 0x24B7, 0x24B8, 0x24B9, 0x24BA, 0x24BB, 0x24BC, 0x24BD, 0x24BE, 0x24BF, 0x24C0, 0x24C1, 0x24C2, 0x24C3, 0x24C4, 0x24C5, 0x24C6, 0x24C7, 0x24C8, 0x24C9, 0x24CA, 0x24CB, 0x24CC, 0x24CD, 0x24CE, 0x24CF, 0x24D0, 0x24D1, 0x24D2, 0x24D3, 0x24D4, 0x24D5, 0x24D6, 0x24D7, 0x24D8, 0x24D9, 0x24DA, 0x24DB, 0x24DC, 0x24DD, 0x24DE, 0x24DF, 0x24E0, 0x24E1, 0x24E2, 0x24E3, 0x24E4, 0x24E5, 0x24E6, 0x24E7, 0x24E8, 0x24E9, 0x24EA, 0x24EB, 0x24EC, 0x24ED, 0x24EE, 0x24EF, 0x24F0, 0x24F1, 0x24F2, 0x24F3, 0x24F4, 0x24F5, 0x24F6, 0x24F7, 0x24F8, 0x24F9, 0x24FA, 0x24FB, 0x24FC, 0x24FD, 0x24FE};
        public static string[] Shirt_Names = new string[255] { "Flame Shirt", "Paw Shirt", "Wavy Pink Shirt", "Furture Shirt", "Bold Check Shirt", "Mint Gingham", "Bad Plaid Shirt", "Speedway Shirt", "Folk Shirt", "Daisy Shirt", "Wavy Tan Shirt", "Optical Shirt", "Rugby Shirt", "Sherbet Gingham", "Yellow Tartan", "Gelato Shirt", "Work Uniform", "Patched Shirt", "Plum Kimono", "Somber Robe", "Red Sweatsuit", "Blue Sweatsuit", "Red Puffy Vest", "Blue Puffy Vest", "Summer Robe", "Bamboo Robe", "Red Aloha Shirt", "Blue Aloha Shirt", "Dark Polka Shirt", "Lite Polka Shirt", "Lovely Shirt", "Citrus Shirt", "Kiwi Shirt", "Watermelon Shirt", "Strawberry Shirt", "Grape Shirt", "Melon Shirt", "Jingle Shirt", "Blossom Shirt", "Icy Shirt", "Crewel Shirt", "Tropical Shirt", "Ribbon Shirt", "Fall Plaid Shirt", "Fiendish Shirt", "Chevron Shirt", "Ladybug Shirt", "Botanical Shirt", "Anju's Shirt", "Kaffe's Shirt", "Lavender Robe", "Blue Grid Shirt", "Butterfly Shirt", "Blue Tartan", "Gracie's Top", "Orange Tie-Die", "Purple Tie-Die", "Green Tie-Die", "Blue Tie-Die", "Red Tie-Die", "One-Ball Shirt", "Two-Ball Shirt", "Three-Ball Shirt", "Four-Ball Shirt", "Five-Ball Shirt", "Six-Ball Shirt", "Seven-Ball Shirt", "Eight-Ball Shirt", "Nine-Ball Shirt", "Arctic Camo", "Jungle Camo", "Desert Camo", "Rally Shirt", "Racer Shirt", "Racer 6 Shirt", "Fish Bone Shirt", "Spiderweb Shirt", "Zipper Shirt", "Bubble Shirt", "Yellow Bolero", "Nebula Shirt", "Neo-Classic Knit", "Noble Shirt", "Turnip Top", "Oft-Seen Print", "Ski Sweater", "Circus Shirt", "Patchwork Top", "Mod Top", "Hippie Shirt", "Rickrack Shirt", "Diner Uniform", "Shirt Circuit", "U R Here Shirt", "Yodel Shirt", "Pulse Shirt", "Prism Shirt", "Star Shirt", "Straw Shirt", "Noodle Shirt", "Dice Shirt", "Kiddie Shirt", "Frog Shirt", "Moody Blue Shirt", "Cloudy Shirt", "Fortune Shirt", "Skull Shirt", "Desert Shirt", "Aurora Knit", "Winter Sweater", "Go-Go Shirt", "Jade Check Print", "Blue Check Print", "Red Grid Shirt", "Flicker Shirt", "Floral Knit", "Rose Shirt", "Sunset Top", "Chain-Gang Shirt", "Spring Shirt", "Bear Shirt", "MVP Shirt", "Silk Bloom Shirt", "Pop Bloom Shirt", "Loud Bloom Shirt", "Hot Spring Shirt", "New Spring Shirt", "Deep Blue Tee", "Snowcone Shirt", "Ugly Shirt", "Sharp Outfit", "Painter's Smock", "Spade Shirt", "Blossoming Shirt", "Peachy Shirt", "Static Shirt", "Rainbow Shirt", "Groovy Shirt", "Loud Line Shirt", "Dazed Shirt", "Red Bar Shirt", "Blue Stripe Knit", "Earthy Knit", "Spunky Knit", "Deer Shirt", "Blue Check Shirt", "Light Line Shirt", "Blue Pinestripe", "Diamond Shirt", "Lime Line Shirt", "Big Bro's Shirt", "Green Bar Shirt", "Yellow Bar Shirt", "Monkey Shirt", "Polar Fleece", "Ancient Knit", "Fish Knit", "Vertigo Shirt", "Misty Shirt", "Stormy Shirt", "Red Scale Shirt", "Blue Scale Shirt", "Heart Shirt", "Yellow Pinstripe", "Club Shirt", "Li'l Bro's Shirt", "Argyle Knit", "Caveman Tunic", "Café Shirt", "Tiki Shirt", "A Shirt", "Checkered Shirt", "No. 1 Shirt", "No. 2 Shirt", "No. 3 Shirt", "No. 4 Shirt", "No. 5 Shirt", "No. 23 Shirt", "No. 67 Shirt", "BB Shirt", "Beatnik Shirt", "Moldy Shirt", "Houndstooth Tee", "Big Star Shirt", "Orange Pinstripe", "Twinkle Shirt", "Funky Dot Shirt", "Crossing Shirt", "Splendid Shirt", "Jagged Shirt", "Denim Shirt", "Cherry Shirt", "Gumdrop Shirt", "Barber Shirt", "Concierge Shirt", "Fresh Shirt", "Far-Out Shirt", "Dawn Shirt", "Striking Outfit", "Red Check Shirt", "Berry Gingham", "Lemon Gingham", "Dragon Suit", "G Logo Shirt", "Tin Shirt", "Jester Shirt", "Pink Tartan", "Waffle Shirt", "Gray Tartan", "Windsock Shirt", "Trendy Top", "Green Ring Shirt", "White Ring Shirt", "Snappy Print", "Chichi Print", "Wave Print", "Checkerboard Tee", "Subdued Print", "Airy Shirt", "Coral Shirt", "Leather Jerkin", "Zebra Print", "Tiger Print", "Cow Print", "Leopard Print", "Danger Shirt", "Big Dot Shirt", "Puzzling Shirt", "Exotic Shirt", "Houndstooth Knit", "Uncommon Shirt", "Dapper Shirt", "Gaudy Sweater", "Cozy Sweater", "Comfy Sweater", "Classic Top", "Vogue Top", "Laced Shirt", "Natty Shirt", "Citrus Gingham", "Cool Shirt", "Dreamy Shirt", "Flowery Shirt", "Caterpillar Tee", "Shortcake Shirt", "Whirly Shirt", "Thunder Shirt", "Giraffe Print", "Swell Shirt", "Toad Print", "Grass Shirt", "Mosaic Shirt", "Fetching Outfit", "Snow Shirt", "Melon Gingham" };

        static ushort[] Wallpaper_IDs = new ushort[67] { 0x2700, 0x2701, 0x2702, 0x2703, 0x2704, 0x2705, 0x2706, 0x2707, 0x2708, 0x2709, 0x270A, 0x270B, 0x270C, 0x270D, 0x270E, 0x270F, 0x2710, 0x2711, 0x2712, 0x2713, 0x2714, 0x2715, 0x2716, 0x2717, 0x2718, 0x2719, 0x271A, 0x271B, 0x271C, 0x271D, 0x271E, 0x271F, 0x2720, 0x2721, 0x2722, 0x2723, 0x2724, 0x2725, 0x2726, 0x2727, 0x2728, 0x2729, 0x272A, 0x272B, 0x272C, 0x272D, 0x272E, 0x272F, 0x2730, 0x2731, 0x2732, 0x2733, 0x2734, 0x2735, 0x2736, 0x2737, 0x2738, 0x2739, 0x273A, 0x273B, 0x273C, 0x273D, 0x273E, 0x273F, 0x2740, 0x2741, 0x2742 };
        static string[] Wallpaper_Names = new string[67] { "Chic Wall", "Classic Wall", "Parlor Wall", "Stone Wall", "Blue-Trim Wall", "Plaster Wall", "Classroom Wall", "Lovely Wall", "Exotic Wall", "Mortar Wall", "Gold Screen Wall", "Tea Room Wall", "Citrus Wall", "Cabin Wall", "Blue Tarp", "Lunar Horizon", "Garden Wall", "Spooky Wall", "Western Vista", "Green Wall", "Blue Wall", "Regal Wall", "Ranch Wall", "Modern Wall", "Cabana Wall", "Snowman Wall", "Backyard Fence", "Music Room Wall", "Plaza Wall", "Lattice Wall", "Ornate Wall", "Modern Screen", "Bamboo Wall", "Kitchen Wall", "Old Brick Wall", "Stately Wall", "Imperial Wall", "Manor Wall", "Ivy Wall", "Mod Wall", "Rose Wall", "Wood Paneling", "Concrete Wall", "Office Wall", "Ancient Wall", "Exquisite Wall", "Sandlot Wall", "Jingle Wall", "Meadow Vista", "Tree-Lined Wall", "Mosaic Wall", "Arched Window", "Basement Wall", "Backgammon Wall", "Kiddie Wall", "Shanty Wall", "Industrial Wall", "Desert Vista", "Library Wall", "Floral Wall", "Tropical Vista", "Playroom Wall", "Kitschy Wall", "Groovy Wall", "Mushroom Mural", "Ringside Seating", "Harvest Wall"  };

        static ushort[] Carpet_IDs = new ushort[67] { 0x2600, 0x2601, 0x2602, 0x2603, 0x2604, 0x2605, 0x2606, 0x2607, 0x2608, 0x2609, 0x260A, 0x260B, 0x260C, 0x260D, 0x260E, 0x260F, 0x2610, 0x2611, 0x2612, 0x2613, 0x2614, 0x2615, 0x2616, 0x2617, 0x2618, 0x2619, 0x261A, 0x261B, 0x261C, 0x261D, 0x261E, 0x261F, 0x2620, 0x2621, 0x2622, 0x2623, 0x2624, 0x2625, 0x2626, 0x2627, 0x2628, 0x2629, 0x262A, 0x262B, 0x262C, 0x262D, 0x262E, 0x262F, 0x2630, 0x2631, 0x2632, 0x2633, 0x2634, 0x2635, 0x2636, 0x2637, 0x2638, 0x2639, 0x263A, 0x263B, 0x263C, 0x263D, 0x263E, 0x263F, 0x2640, 0x2641, 0x2642 };
        static string[] Carpet_Names = new string[67] { "Plush Carpet", "Classic Carpet", "Checkered Tile", "Old Flooring", "Red Tile", "Birch Flooring", "Classroom Floor", "Lovely Carpet", "Exotic Rug", "Mossy Carpet", "18 Mat Tatami", "8 Mat Tatami", "Citrus Carpet", "Cabin Rug", "Closed Road", "Lunar Surface", "Sand Garden", "Spooky Carpet", "Western Desert", "Green Rug", "Blue Flooring", "Regal Carpet", "Ranch Flooring", "Modern Tile", "Cabana Flooring", "Snowman Carpet", "Backyard Lawn", "Music Room Floor", "Plaza Tile", "Kitchen Tile", "Ornate Rug", "Tatami Floor", "Bamboo Flooring", "Kitchen Flooring", "Charcoal Tile", "Stone Tile", "Imperial Tile", "Opulent Rug", "Slate Flooring", "Ceramic Tile", "Fancy Carpet", "Cowhide Rug", "Steel Flooring", "Office Flooring", "Ancient Tile", "Exquisite Rug", "Sandlot", "Jingle Carpet", "Daisy Meadow", "Sidewalk", "Mosaic Tile", "Parquet Floor", "Basement Floor", "Chessboard Rug", "Kiddie Carpet", "Shanty Mat", "Concrete Floor", "Saharah's Desert", "Tartan Rug", "Palace Tile", "Tropical Floor", "Playroom Rug", "Kitschy Tile", "Diner Tile", "Block Flooring", "Boxing Ring Mat", "Harvest Rug" };

        static ushort[] Item_IDs = new ushort[336]
        {
            0x2503, 0x2504, 0x2505, 0x2506, 0x2507, 0x2508, 0x2509, 0x250A, 0x250B, 0x250C, 0x2103, 0x2100, 0x2101, 0x2102, 0x250E, 0x250F, 0x2510, 0x2511, 0x2512, 0x2514, 0x2515, 0x2516, 0x2517, 0x2518, 0x2519, 0x251A, 0x251B, 0x251C, 0x251E, 0x251F, 0x2520, 0x2521, 0x2522, 0x2523, 0x2524, 0x2525, 0x2526, 0x2527, 0x2528, 0x2529, 0x252A, 0x252B, 0x252C, 0x252D, 0x252E, 0x252F, 0x2530, 0x2B00, 0x2B01, 0x2B02, 0x2B03, 0x2B04, 0x2B05, 0x2B06, 0x2B07, 0x2B08, 0x2B09, 0x2B0A, 0x2B0B, 0x2B0C, 0x2B0D, 0x2B0E, 0x2B0F, 0x2800, 0x2801, 0x2802, 0x2803, 0x2804, 0x2805, 0x2806, 0x2807, 0x2900, 0x2901, 0x2902, 0x2903, 0x2904, 0x2905, 0x2906, 0x2907, 0x2908, 0x2909, 0x290A, 0x2C00, 0x2C01, 0x2C02, 0x2C03, 0x2C04, 0x2C05, 0x2C06, 0x2C07, 0x2C08, 0x2C09, 0x2C0A, 0x2C0B, 0x2C0C, 0x2C0D, 0x2C0E, 0x2C0F, 0x2C10, 0x2C11, 0x2C12, 0x2C13, 0x2C14, 0x2C15, 0x2C16, 0x2C17, 0x2C18, 0x2C19, 0x2C1A, 0x2C1B, 0x2C1C, 0x2C1D, 0x2C1E, 0x2C1F, 0x2C20, 0x2C21, 0x2C22, 0x2C23, 0x2C24, 0x2C25, 0x2C26, 0x2C27, 0x2C28, 0x2C29, 0x2C2A, 0x2C2B, 0x2C2C, 0x2C2D, 0x2C2E, 0x2C2F, 0x2C30, 0x2C31, 0x2C32, 0x2C33, 0x2C34, 0x2C35, 0x2C36, 0x2C37, 0x2C38, 0x2C39, 0x2C3A, 0x2C3B, 0x2C3C, 0x2C3D, 0x2C3E, 0x2C3F, 0x2C40, 0x2C41, 0x2C42, 0x2C43, 0x2C44, 0x2C45, 0x2C46, 0x2C47, 0x2C48, 0x2C49, 0x2C4A, 0x2C4B, 0x2C4C, 0x2C4D, 0x2C4E, 0x2C4F, 0x2C50, 0x2C51, 0x2C52, 0x2C53, 0x2C54, 0x2C55, 0x2C56, 0x2C57, 0x2C58, 0x2C59, 0x2C5A, 0x2C5B, 0x2C5C, 0x2C5D, 0x2C5E, 0x2C5F, 0x2A00, 0x2A01, 0x2A02, 0x2A03, 0x2A04, 0x2A05, 0x2A06, 0x2A07, 0x2A08, 0x2A09, 0x2A0A, 0x2A0B, 0x2A0C, 0x2A0D, 0x2A0E, 0x2A0F, 0x2A10, 0x2A11, 0x2A12, 0x2A13, 0x2A14, 0x2A15, 0x2A16, 0x2A17, 0x2A18, 0x2A19, 0x2A1A, 0x2A1B, 0x2A1C, 0x2A1D, 0x2A1E, 0x2A1F, 0x2A20, 0x2A21, 0x2A22, 0x2A23, 0x2A24, 0x2A25, 0x2A26, 0x2A27, 0x2A28, 0x2A29, 0x2A2A, 0x2A2B, 0x2A2C, 0x2A2D, 0x2A2E, 0x2A2F, 0x2A30, 0x2A31, 0x2A32, 0x2A33, 0x2A34, 0x2A35, 0x2A36, 0x2200, 0x2201, 0x2202, 0x2203, 0x2204, 0x2205, 0x2206, 0x2207, 0x2208, 0x2209, 0x220A, 0x220B, 0x220C, 0x220D, 0x220E, 0x220F, 0x2210, 0x2211, 0x2212, 0x2213, 0x2214, 0x2215, 0x2216, 0x2217, 0x2218, 0x2219, 0x221A, 0x221B, 0x221C, 0x221D, 0x221E, 0x221F, 0x2220, 0x2221, 0x2222, 0x2223, 0x2224, 0x2225, 0x2226, 0x2227, 0x2228, 0x2229, 0x222A, 0x222B, 0x222C, 0x222D, 0x222E, 0x222F, 0x2230, 0x2231, 0x2232, 0x2233, 0x2234, 0x2235, 0x2236, 0x2237, 0x2238, 0x2239, 0x223A, 0x223B, 0x223C, 0x223D, 0x223E, 0x223F, 0x2240, 0x2241, 0x2242, 0x2243, 0x2244, 0x2245, 0x2246, 0x2247, 0x2248, 0x2249, 0x224A, 0x224B, 0x224C, 0x224D, 0x224E, 0x224F, 0x2250, 0x2251, 0x2252, 0x2253, 0x2254, 0x2255, 0x2256, 0x2257, 0x2258, 0x2259, 0x225A, 0x225B, 0x2D28, 0x2D29, 0x2D2A, 0x2D2B, 0x2D2C, 0x2E00, 0x2E01, 0x2F00, 0x2F01, 0x2F02, 0x2F03
        };
        static string[] Item_Names = new string[336]
        {
            "Videotape", "Organizer", "Pokémon Pikachu", "Comic Book", "Picture Book", "Game Boy", "Camera", "Watch", "Handkerchief", "Glasses Case", "100 Bells", "1,000 Bells", "10,000 Bells", "30,000 Bells", "Empty Can", "Boot", "Old Tire", "Fossil", "Pitfall", "Lion's Paw", "Wentletrap", "Venus Comb", "Porceletta", "Sand Dollar", "White Scallop", "Conch", "Coral", "Present (Drop to Open)(Chess Table)", "Signboard (Placeable)", "Present (Drop to Open)(Golden Net)", "Present (Drop to Open)(Golden Axe)", "Present (Drop to Open)(Golden Shovel)", "Present (Drop to Open)(Golden Rod)", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Exercise Card", "Knife and Fork", "College Rule", "School Pad", "Organizer", "Diary", "Journal", "Pink Diary", "Captain's Log", "Blue Diary", "French Notebook", "Scroll", "Pink Plaid Pad", "Blue Polka Pad", "Green Plaid Pad", "Red Polka Pad", "Yellow Plaid Pad", "Calligraphy Pad", "Apple", "Cherry Shirt", "Pear", "Peach", "Orange", "Mushroom", "Candy", "Coconut", "Sapling", "Cedar Sapling", "White Pansy Bag", "Purple Pansy Bag", "Yellow Pansy Bag", "White Cosmos Bag", "Pink Cosmos Bag", "Blue Cosmos Bag", "Red Tulip Bag", "White Tulip Bag", "Yellow Tulip Bag", "January Ticket (1)", "January Ticket (2)", "January Ticket (3)", "January Ticket (4)", "January Ticket (5)", "January Ticket (1) (2)", "January Ticket (1) (3)", "January Ticket (1) (4)", "February Ticket (1)", "February Ticket (2)", "February Ticket (3)", "February Ticket (4)", "February Ticket (5)", "February Ticket (1) (2)", "February Ticket (1) (3)", "February Ticket (1) (4)", "March Ticket (1)", "March Ticket (2)", "March Ticket (3)", "March Ticket (4)", "March Ticket (5)", "March Ticket (1) (2)", "March Ticket (1) (3)", "March Ticket (1) (4)", "April Ticket (1)", "April Ticket (2)", "April Ticket (3)", "April Ticket (4)", "April Ticket (5)", "April Ticket (1) (2)", "April Ticket (1) (3)", "April Ticket (1) (4)", "May Ticket (1)", "May Ticket (2)", "May Ticket (3)", "May Ticket (4)", "May Ticket (5)", "May Ticket (1) (2)", "May Ticket (1) (3)", "May Ticket (1) (4)", "June Ticket (1)", "June Ticket (2)", "June Ticket (3)", "June Ticket (4)", "June Ticket (5)", "June Ticket (1) (2)", "June Ticket (1) (3)", "June Ticket (1) (4)", "July Ticket (1)", "July Ticket (2)", "July Ticket (3)", "July Ticket (4)", "July Ticket (5)", "July Ticket (1) (2)", "July Ticket (1) (3)", "July Ticket (1) (4)", "August Ticket (1)", "August Ticket (2)", "August Ticket (3)", "August Ticket (4)", "August Ticket (5)", "August Ticket (1) (2)", "August Ticket (1) (3)", "August Ticket (1) (4)", "September Ticket (1)", "September Ticket (2)", "September Ticket (3)", "September Ticket (4)", "September Ticket (5)", "September Ticket (1) (2)", "September Ticket (1) (3)", "September Ticket (1) (4)", "October Ticket (1)", "October Ticket (2)", "October Ticket (3)", "October Ticket (4)", "October Ticket (5)", "October Ticket (1) (2)", "October Ticket (1) (3)", "October Ticket (1) (4)", "November Ticket (1)", "November Ticket (2)", "November Ticket (3)", "November Ticket (4)", "November Ticket (5)", "November Ticket (1) (2)", "November Ticket (1) (3)", "November Ticket (1) (4)", "December Ticket (1)", "December Ticket (2)", "December Ticket (3)", "December Ticket (4)", "December Ticket (5)", "December Ticket (1) (2)", "December Ticket (1) (3)", "December Ticket (1) (4)", "K.K. Chorale", "K.K. March", "K.K. Waltz", "K.K. Swing", "K.K. Jazz", "K.K. Fusion", "K.K. Etude", "K.K. Lullaby", "K.K. Aria", "K.K. Samba", "K.K. Bossa", "K.K. Calypso", "K.K. Salsa", "K.K. Mambo", "K.K. Reggae", "K.K. Ska", "K.K. Tango", "K.K. Faire", "Aloha K.K.", "Lucky K.K.", "K.K. Condor", "K.K. Steppe", "Imperial K.K.", "K.K. Casbah", "K.K. Safari", "K.K. Folk", "K.K. Rock", "Rockin' K.K.", "K.K. Ragtime", "K.K. Gumbo", "The K. Funk", "K.K. Blues", "Soulful K.K.", "K.K. Soul", "K.K. Cruisin'", "K.K. Love Song", "K.K. D & B", "K.K. Technopop", "DJ K.K.", "Only Me", "K.K. Country", "Surfin' K.K.", "K.K. Ballad", "Comrade K.K.", "K.K. Lament", "Go K.K. Rider!", "K.K. Dirge", "K.K. Western", "Mr. K.K.", "Café K.K.", "K.K. Parade", "Señor K.K.", "K.K. Song", "I Love You", "Two Days Ago", "Net", "Axe", "Shovel", "Fishing Rod", "Gelato Umbrella", "Daffodil Parasol", "Berry Umbrella", "Orange Umbrella", "Mod Umbrella", "Petal Parasol", "Ribbon Parasol", "Gingham Parasol", "Plaid Parasol", "Lacy Parasol", "Elegant Umbrella", "Dainty Parasol", "Classic Umbrella", "Nintendo Parasol", "Bumbershoot", "Sunny Parasol", "Batbrella", "Checked Umbrella", "Yellow Umbrella", "Leaf Umbrella", "Lotus Parasol", "Paper Parasol", "Polka Parasol", "Sharp Umbrella", "Twig Parasol", "Noodle Parasol", "Hypno Parasol", "Pastel Parasol", "Retro Umbrella", "Icy Umbrella", "Blue Umbrella", "Flame Umbrella", "Pattern #1 (Umbrella)", "Pattern #2 (Umbrella)", "Pattern #3 (Umbrella)", "Pattern #4 (Umbrella)", "Pattern #5 (Umbrella)", "Pattern #6 (Umbrella)", "Pattern #7 (Umbrella)", "Pattern #8 (Umbrella)", "Sickle", "Red Paint", "Orange Paint", "Yellow Paint", "Pale Green Paint", "Green Paint", "Sky Blue Paint", "Blue Paint", "Purple Paint", "Pink Paint", "Black Paint", "White Paint", "Brown Paint", "Golden Net", "Golden Axe", "Golden Shovel", "Golden Rod", "Axe (Use #1)", "Axe (Use #2)", "Axe (Use #3)", "Axe (Use #4)", "Axe (Use #5)", "Axe (Use #6)", "Axe (Use #7)", "Red Balloon", "Yellow Balloon", "Blue Balloon", "Green Balloon", "Purple Balloon", "Bunny P. Balloon", "Bunny B. Balloon", "Bunny O. Balloon", "Yellow Pinwheel", "Red Pinwheel", "Tiger Pinwheel", "Green Pinwheel", "Pink Pinwheel", "Striped Pinwheel", "Flower Pinwheel", "Fancy Pinwheel", "Bluebell Fan", "Plum Fan", "Bamboo Fan", "Cloud Fan", "Maple Fan", "Fan Fan", "Flower Fan", "Leaf Fan", "Spirit (1)", "Spirit (2)", "Spirit (3)", "Spirit (4)", "Spirit (5)", "Grab Bag (1)", "Grab Bag (2)", "10 Turnips", "50 Turnips", "100 Turnips", "Spoiled Turnips",
        };

        static ushort[] Fish_IDs = new ushort[40]
        {
            0x2300, 0x2301, 0x2302, 0x2303, 0x2304, 0x2305, 0x2306, 0x2307, 0x2308, 0x2309, 0x230A, 0x230B, 0x230C, 0x230D, 0x230E, 0x230F, 0x2310, 0x2311, 0x2312, 0x2313, 0x2314, 0x2315, 0x2316, 0x2317, 0x2318, 0x2319, 0x231A, 0x231B, 0x231C, 0x231D, 0x231E, 0x231F, 0x2320, 0x2321, 0x2322, 0x2323, 0x2324, 0x2325, 0x2326, 0x2327
        };

        static string[] Fish_Names = new string[40]
        {
            "Crucian Carp", "Brook Trout", "Carp", "Koi", "Catfish", "Small Bass", "Bass", "Large Bass", "Bluegill", "Giant Catfish", "Giant Snakehead", "Barbel Steed", "Dace", "Pale Chub", "Bitterling", "Loach", "Pond Smelt", "Sweetfish", "Cherry Salmon", "Large Char", "Rainbow Trout", "Stringfish", "Salmon", "Goldfish", "Piranha", "Arowana", "Eel", "Freshwater Goby", "Angelfish", "Guppy", "Popeyed Goldfish", "Coelacanth", "Crawfish", "Frog", "Killifish", "Jellyfish", "Sea Bass", "Red Snapper", "Barred Knifejaw", "Arapaima"
        };

        static ushort[] Insect_IDs = new ushort[40]
        {
            0x2D00, 0x2D01, 0x2D02, 0x2D03, 0x2D04, 0x2D05, 0x2D06, 0x2D07, 0x2D08, 0x2D09, 0x2D0A, 0x2D0B, 0x2D0C, 0x2D0D, 0x2D0E, 0x2D0F, 0x2D10, 0x2D11, 0x2D12, 0x2D13, 0x2D14, 0x2D15, 0x2D16, 0x2D17, 0x2D18, 0x2D19, 0x2D1A, 0x2D1B, 0x2D1C, 0x2D1D, 0x2D1E, 0x2D1F, 0x2D20, 0x2D21, 0x2D22, 0x2D23, 0x2D24, 0x2D25, 0x2D26, 0x2D27
        };

        static string[] Insect_Names = new string[40]
        {
            "Common Butterfly", "Yellow Butterfly", "Tiger Butterfly", "Purple Butterfly", "Robust Cicada", "Walker Cicada", "Evening Cicada", "Brown Cicada", "Bee", "Common Dragonfly", "Red Dragonfly", "Darner Dragonfly", "Banded Dragonfly", "Long Locust", "Migratory Locust", "Cricket", "Grasshopper", "Bell Cricket", "Pine Cricket", "Drone Beetle", "Dynastid Beetle", "Flat Stag Beetle", "Jewel Beetle", "Longhorn Beetle", "Ladybug", "Spotted Ladybug", "Mantis", "Firefly", "Cockroach", "Saw Stag Beetle", "Mountain Beetle", "Giant Beetle", "Snail", "Mole Cricket", "Pond Skater", "Bagworm", "Pill Bug", "Spider", "Ant", "Mosquito"
        };

        static ushort[] Gyroid_IDs = new ushort[127]
        {
            0x15B0, 0x15B4, 0x15B8, 0x15BC, 0x15C0, 0x15C4, 0x15C8, 0x15CC, 0x15D0, 0x15D4, 0x15D8, 0x15DC, 0x15E0, 0x15E4, 0x15E8,
            0x15EC, 0x15F0, 0x15F4, 0x15F8, 0x15FC, 0x1600, 0x1604, 0x1608, 0x160C, 0x1610, 0x1614, 0x1618, 0x161C, 0x1620, 0x1624,
            0x1628, 0x162C, 0x1630, 0x1634, 0x1638, 0x163C, 0x1640, 0x1644, 0x1648, 0x164C, 0x1650, 0x1654, 0x1658, 0x165C, 0x1660,
            0x1664, 0x1668, 0x166C, 0x1670, 0x1674, 0x1678, 0x167C, 0x1680, 0x1684, 0x1688, 0x168C, 0x1690, 0x1694, 0x1698, 0x169C,
            0x16A0, 0x16A4, 0x16A8, 0x16AC, 0x16B0, 0x16B4, 0x16B8, 0x16BC, 0x16C0, 0x16C4, 0x16C8, 0x16CC, 0x16D0, 0x16D4, 0x16D8,
            0x16DC, 0x16E0, 0x16E4, 0x16E8, 0x16EC, 0x16F0, 0x16F4, 0x16F8, 0x16FC, 0x1700, 0x1704, 0x1708, 0x170C, 0x1710, 0x1714,
            0x1718, 0x171C, 0x1720, 0x1724, 0x1728, 0x172C, 0x1730, 0x1734, 0x1738, 0x173C, 0x1740, 0x1744, 0x1748, 0x174C, 0x1750,
            0x1754, 0x1758, 0x175C, 0x1760, 0x1764, 0x1768, 0x176C, 0x1770, 0x1774, 0x1778, 0x177C, 0x1780, 0x1784, 0x1788, 0x178C,
            0x1790, 0x1794, 0x1798, 0x179C, 0x17A0, 0x17A4, 0x17A8
        };

        static string[] Gyroid_Names = new string[127]
        {
            "Tall Gongoid", "Mega Gongoid", "Mini Gongoid", "Gongoid", "Mini Oombloid", "Oombloid", "Mega Oombloid", "Tall Oombloid", "Mega Echoid", "Mini Echoid", "Tall Echoid", "Mini Sputnoid", "Sputnoid", "Mega Sputnoid", "Tall Sputnoid",
            "Mini Dinkoid", "Mini Fizzoid", "Mega Fizzoid", "Mega Dinkoid", "Mini Gargloid", "Gargloid", "Tall Gargloid", "Mega Buzzoid", "Tall Buzzoid", "Buzzoid", "Mini Buzzoid", "Sproid", "Mini Sproid", "Mega Sproid", "Tall Sproid",
            "Tootoid", "Mini Tootoid", "Mega Tootoid", "Tall Droploid", "Mega Bovoid", "Tall Bovoid", "Mini Metatoid", "Metatoid", "Mini Bowtoid", "Bowtoid", "Mega Bowtoid", "Tall Bowtoid", "Mega Lamentoid", "Tall Lamentoid", "Lamentoid",
            "Mini Lamentoid", "Mini Timpanoid", "Timpanoid", "Mega Timpanoid", "Quazoid", "Mega Quazoid", "Mega Dekkoid", "Dekkoid", "Mini Dekkoid", "Mega Alloid", "Tall Alloid", "Mini Alloid", "Mini Freakoid", "Mega Feakoid", "Tall Quazoid",
            "Mini Quazoid", "Squat Dingloid", "Mega Dingloid", "Tall Dingloid", "Dingloid", "Mini Dingloid", "Wee Dingloid", "Mega Clankoid", "Clankoid", "Tall Clankoid", "Mini Clankoid", "Croakoid", "Mega Croakoid", "Tall Croakoid", "Mini Croakoid",
            "Mega Poltergoid", "Tall Poltergoid", "Poltergoid", "Mini Poltergoid", "Tall Warbloid", "Warbloid", "Mini Warbloid", "Mega Rustoid", "Rustoid", "Mini Rustoid", "Mega Percoloid", "Tall Percoloid", "Mega Puffoid", "Mini Puffoid", "Tall Puffoid",
            "Rhythmoid", "Mini Rythmoid", "Slim Quazoid", "Mega Oboid", "Oboid", "Tall Oboid", "Tall Timpanoid", "Mini Howloid", "Howloid", "Mega Howloid", "Mega Harmonoid", "Harmonoid", "Tall Harmonoid", "Mini Harmonoid", "Tall Strumboid",
            "Mega Strumboid", "Strumboid", "Mini Strumboid", "Mega Lullaboid", "Tall Lullaboid", "Lullaboid", "Mini Lullaboid", "Mega Drilloid", "Drilloid", "Mini Drilloid", "Mega Nebuloid", "Nebuloid", "Squat Nebuloid", "Tall Nebuloid", "Mini Nebuloid",
            "Slim Nebuloid", "Mega Plinkoid", "Plinkoid", "Mini Plinkoid", "Squelchoid", "Mega Squelchoid", "Mini Squelchoid"
        };

        static ushort[] Stationery_IDs = new ushort[256]
        {
            0x2000, 0x2001, 0x2002, 0x2003, 0x2004, 0x2005, 0x2006, 0x2007, 0x2008, 0x2009, 0x200A, 0x200B, 0x200C, 0x200D, 0x200E, 0x200F, 0x2010, 0x2011, 0x2012, 0x2013, 0x2014, 0x2015, 0x2016, 0x2017, 0x2018, 0x2019, 0x201A, 0x201B, 0x201C, 0x201D, 0x201E, 0x201F, 0x2020, 0x2021, 0x2022, 0x2023, 0x2024, 0x2025, 0x2026, 0x2027, 0x2028, 0x2029, 0x202A, 0x202B, 0x202C, 0x202D, 0x202E, 0x202F, 0x2030, 0x2031, 0x2032, 0x2033, 0x2034, 0x2035, 0x2036, 0x2037, 0x2038, 0x2039, 0x203A, 0x203B, 0x203C, 0x203D, 0x203E, 0x203F, 0x2040, 0x2041, 0x2042, 0x2043, 0x2044, 0x2045, 0x2046, 0x2047, 0x2048, 0x2049, 0x204A, 0x204B, 0x204C, 0x204D, 0x204E, 0x204F, 0x2050, 0x2051, 0x2052, 0x2053, 0x2054, 0x2055, 0x2056, 0x2057, 0x2058, 0x2059, 0x205A, 0x205B, 0x205C, 0x205D, 0x205E, 0x205F, 0x2060, 0x2061, 0x2062, 0x2063, 0x2064, 0x2065, 0x2066, 0x2067, 0x2068, 0x2069, 0x206A, 0x206B, 0x206C, 0x206D, 0x206E, 0x206F, 0x2070, 0x2071, 0x2072, 0x2073, 0x2074, 0x2075, 0x2076, 0x2077, 0x2078, 0x2079, 0x207A, 0x207B, 0x207C, 0x207D, 0x207E, 0x207F, 0x2080, 0x2081, 0x2082, 0x2083, 0x2084, 0x2085, 0x2086, 0x2087, 0x2088, 0x2089, 0x208A, 0x208B, 0x208C, 0x208D, 0x208E, 0x208F, 0x2090, 0x2091, 0x2092, 0x2093, 0x2094, 0x2095, 0x2096, 0x2097, 0x2098, 0x2099, 0x209A, 0x209B, 0x209C, 0x209D, 0x209E, 0x209F, 0x20A0, 0x20A1, 0x20A2, 0x20A3, 0x20A4, 0x20A5, 0x20A6, 0x20A7, 0x20A8, 0x20A9, 0x20AA, 0x20AB, 0x20AC, 0x20AD, 0x20AE, 0x20AF, 0x20B0, 0x20B1, 0x20B2, 0x20B3, 0x20B4, 0x20B5, 0x20B6, 0x20B7, 0x20B8, 0x20B9, 0x20BA, 0x20BB, 0x20BC, 0x20BD, 0x20BE, 0x20BF, 0x20C0, 0x20C1, 0x20C2, 0x20C3, 0x20C4, 0x20C5, 0x20C6, 0x20C7, 0x20C8, 0x20C9, 0x20CA, 0x20CB, 0x20CC, 0x20CD, 0x20CE, 0x20CF, 0x20D0, 0x20D1, 0x20D2, 0x20D3, 0x20D4, 0x20D5, 0x20D6, 0x20D7, 0x20D8, 0x20D9, 0x20DA, 0x20DB, 0x20DC, 0x20DD, 0x20DE, 0x20DF, 0x20E0, 0x20E1, 0x20E2, 0x20E3, 0x20E4, 0x20E5, 0x20E6, 0x20E7, 0x20E8, 0x20E9, 0x20EA, 0x20EB, 0x20EC, 0x20ED, 0x20EE, 0x20EF, 0x20F0, 0x20F1, 0x20F2, 0x20F3, 0x20F4, 0x20F5, 0x20F6, 0x20F7, 0x20F8, 0x20F9, 0x20FA, 0x20FB, 0x20FC, 0x20FD, 0x20FE, 0x20FF
        };

        static string[] Stationery_Names = new string[256]
        {
            "Airmail Paper (1)", "Sparkly Paper (1)", "Bamboo Paper (1)", "Orange Paper (1)", "Essay Paper (1)", "Panda Paper (1)", "Ranch Paper (1)", "Steel Paper (1)", "Blossom Paper (1)", "Vine Paper (1)", "Cloudy Paper (1)", "Petal Paper (1)", "Snowy Paper (1)", "Rainy Day Paper (1)", "Watermelon Paper (1)", "Deep Sea Paper (1)", "Starry Sky Paper (1)", "Daisy Paper (1)", "Bluebell Paper (1)", "Maple Leaf Paper (1)", "Woodcut Paper (1)", "Octopus Paper (1)", "Festive Paper (1)", "Skyline Paper (1)", "Museum Paper (1)", "Fortune Paper (1)", "Stageshow Paper (1)", "Thick Paper (1)", "Spooky Paper (1)", "Noodle Paper (1)", "Neat Paper (1)", "Horsetail Paper (1)", "Felt Paper (1)", "Parchment (1)", "Cool Paper (1)", "Elegant Paper (1)", "Lacy Paper (1)", "Polka-Dot Paper (1)", "Dizzy Paper (1)", "Rainbow Paper (1)", "Hot Neon Paper (1)", "Cool Neon Paper (1)", "Aloha Paper (1)", "Ribbon Paper (1)", "Fantasy Paper (1)", "Woodland Paper (1)", "Gingko Paper (1)", "Fireworks Paper (1)", "Winter Paper (1)", "Gyroid Paper (1)", "Ivy Paper (1)", "Wing Paper (1)", "Dragon Paper (1)", "Tile Paper (1)", "Misty Paper (1)", "Simple Paper (1)", "Honeybee Paper (1)", "Mystic Paper (1)", "Sunset Paper (1)", "Lattice Paper (1)", "Dainty Paper (1)", "Butterfly Paper (1)", "New Year's Card (1)", "Inky Paper (1)", "Airmail Paper (2)", "Sparkly Paper (2)", "Bamboo Paper (2)", "Orange Paper (2)", "Essay Paper (2)", "Panda Paper (2)", "Ranch Paper (2)", "Steel Paper (2)", "Blossom Paper (2)", "Vine Paper (2)", "Cloudy Paper (2)", "Petal Paper (2)", "Snowy Paper (2)", "Rainy Day Paper (2)", "Watermelon Paper (2)", "Deep Sea Paper (2)", "Starry Sky Paper (2)", "Daisy Paper (2)", "Bluebell Paper (2)", "Maple Leaf Paper (2)", "Woodcut Paper (2)", "Octopus Paper (2)", "Festive Paper (2)", "Skyline Paper (2)", "Museum Paper (2)", "Fortune Paper (2)", "Stageshow Paper (2)", "Thick Paper (2)", "Spooky Paper (2)", "Noodle Paper (2)", "Neat Paper (2)", "Horsetail Paper (2)", "Felt Paper (2)", "Parchment (2)", "Cool Paper (2)", "Elegant Paper (2)", "Lacy Paper (2)", "Polka-Dot Paper (2)", "Dizzy Paper (2)", "Rainbow Paper (2)", "Hot Neon Paper (2)", "Cool Neon Paper (2)", "Aloha Paper (2)", "Ribbon Paper (2)", "Fantasy Paper (2)", "Woodland Paper (2)", "Gingko Paper (2)", "Fireworks Paper (2)", "Winter Paper (2)", "Gyroid Paper (2)", "Ivy Paper (2)", "Wing Paper (2)", "Dragon Paper (2)", "Tile Paper (2)", "Misty Paper (2)", "Simple Paper (2)", "Honeybee Paper (2)", "Mystic Paper (2)", "Sunset Paper (2)", "Lattice Paper (2)", "Dainty Paper (2)", "Butterfly Paper (2)", "New Year's Card (2)", "Inky Paper (2)", "Airmail Paper (3)", "Sparkly Paper (3)", "Bamboo Paper (3)", "Orange Paper (3)", "Essay Paper (3)", "Panda Paper (3)", "Ranch Paper (3)", "Steel Paper (3)", "Blossom Paper (3)", "Vine Paper (3)", "Cloudy Paper (3)", "Petal Paper (3)", "Snowy Paper (3)", "Rainy Day Paper (3)", "Watermelon Paper (3)", "Deep Sea Paper (3)", "Starry Sky Paper (3)", "Daisy Paper (3)", "Bluebell Paper (3)", "Maple Leaf Paper (3)", "Woodcut Paper (3)", "Octopus Paper (3)", "Festive Paper (3)", "Skyline Paper (3)", "Museum Paper (3)", "Fortune Paper (3)", "Stageshow Paper (3)", "Thick Paper (3)", "Spooky Paper (3)", "Noodle Paper (3)", "Neat Paper (3)", "Horsetail Paper (3)", "Felt Paper (3)", "Parchment (3)", "Cool Paper (3)", "Elegant Paper (3)", "Lacy Paper (3)", "Polka-Dot Paper (3)", "Dizzy Paper (3)", "Rainbow Paper (3)", "Hot Neon Paper (3)", "Cool Neon Paper (3)", "Aloha Paper (3)", "Ribbon Paper (3)", "Fantasy Paper (3)", "Woodland Paper (3)", "Gingko Paper (3)", "Fireworks Paper (3)", "Winter Paper (3)", "Gyroid Paper (3)", "Ivy Paper (3)", "Wing Paper (3)", "Dragon Paper (3)", "Tile Paper (3)", "Misty Paper (3)", "Simple Paper (3)", "Honeybee Paper (3)", "Mystic Paper (3)", "Sunset Paper (3)", "Lattice Paper (3)", "Dainty Paper (3)", "Butterfly Paper (3)", "New Year's Card (3)", "Inky Paper (3)", "Airmail Paper (4)", "Sparkly Paper (4)", "Bamboo Paper (4)", "Orange Paper (4)", "Essay Paper (4)", "Panda Paper (4)", "Ranch Paper (4)", "Steel Paper (4)", "Blossom Paper (4)", "Vine Paper (4)", "Cloudy Paper (4)", "Petal Paper (4)", "Snowy Paper (4)", "Rainy Day Paper (4)", "Watermelon Paper (4)", "Deep Sea Paper (4)", "Starry Sky Paper (4)", "Daisy Paper (4)", "Bluebell Paper (4)", "Maple Leaf Paper (4)", "Woodcut Paper (4)", "Octopus Paper (4)", "Festive Paper (4)", "Skyline Paper (4)", "Museum Paper (4)", "Fortune Paper (4)", "Stageshow Paper (4)", "Thick Paper (4)", "Spooky Paper (4)", "Noodle Paper (4)", "Neat Paper (4)", "Horsetail Paper (4)", "Felt Paper (4)", "Parchment (4)", "Cool Paper (4)", "Elegant Paper (4)", "Lacy Paper (4)", "Polka-Dot Paper (4)", "Dizzy Paper (4)", "Rainbow Paper (4)", "Hot Neon Paper (4)", "Cool Neon Paper (4)", "Aloha Paper (4)", "Ribbon Paper (4)", "Fantasy Paper (4)", "Woodland Paper (4)", "Gingko Paper (4)", "Fireworks Paper (4)", "Winter Paper (4)", "Gyroid Paper (4)", "Ivy Paper (4)", "Wing Paper (4)", "Dragon Paper (4)", "Tile Paper (4)", "Misty Paper (4)", "Simple Paper (4)", "Honeybee Paper (4)", "Mystic Paper (4)", "Sunset Paper (4)", "Lattice Paper (4)", "Dainty Paper (4)", "Butterfly Paper (4)", "New Year's Card (4)", "Inky Paper (4)"
        };

        public static List<KeyValuePair<ushort, string>> ItemDatabase = new List<KeyValuePair<ushort, string>>();

        public static void SetupItemDictionary()
        {
            for (int i = 0; i < Furniture_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Furniture_IDs[i], Furniture_Names[i]));

            for (int i = 0; i < Shirt_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Shirt_IDs[i], Shirt_Names[i]));

            for (int i = 0; i < Gyroid_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Gyroid_IDs[i], Gyroid_Names[i]));

            for (int i = 0; i < Carpet_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Carpet_IDs[i], Carpet_Names[i]));

            for (int i = 0; i < Wallpaper_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Wallpaper_IDs[i], Wallpaper_Names[i]));

            for (int i = 0; i < Item_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Item_IDs[i], Item_Names[i]));

            for (int i = 0; i < acreItemIDs.Count; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(acreItemIDs[i], acreItemNames[i]));

            for (int i = 0; i < Fish_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Fish_IDs[i], Fish_Names[i]));

            for (int i = 0; i < Insect_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Insect_IDs[i], Insect_Names[i]));

            for (int i = 0; i < Stationery_IDs.Length; i++)
                ItemDatabase.Add(new KeyValuePair<ushort, string>(Stationery_IDs[i], Stationery_Names[i]));

            ItemDatabase.Add(new KeyValuePair<ushort, string>(0, "Empty")); //Empty Case
            ItemDatabase.Sort((x, y) => x.Key.CompareTo(y.Key));
        }

        public static string GetItemType(ushort ID, SaveType Save_Type = SaveType.Animal_Crossing)
        {
            if (Save_Type == SaveType.Animal_Crossing)
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
                else if (ID >= 0x2400 && ID <= 0x24FE)
                    return "Clothes";
                else if (ID >= 0x15B0 && ID <= 0x17A8)
                    return "Gyroids";
                else if (ID == 0x2511)
                    return "Fossil";
                else if ((ID >= 0x2200 && ID <= 0x225B) || ID == 0x251E)
                    return "Tool";  //0x251E = Signboard (not a 'tool', but it's still classified as one)
                else if ((ID >= 0x1 && ID <= 0x4) || (ID >= 0x005E && ID <= 0x0060) || ID == 0x69 || (ID >= 0x0070 && ID <= 0x0082) || (ID >= 0x0800 && ID <= 0x0868))
                    return "Tree";
                else if ((ID >= 0x5 && ID <= 0x7) || (ID >= 0xB && ID <= 0x10) || (ID >= 0x5000 && ID <= 0xB000) || (ID == 0xFE1D || ID == 0xFE1E))
                    return "Building";
                else if ((ID >= 0x1000 && ID <= 0x15AC) || (ID >= 0x17AC && ID <= 0x1FFC) || (ID >= 0x3000 && ID <= 0x33C4))
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
                    return "Signboard";
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
                else if (ID >= 0x11A8 && ID <= 0x12A7)
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
                    return "Gyroid";
                else if (ID >= 0x5000 && ID <= 0x5021)
                    return "Building";
                else
                    return "Unknown";
            else if (Save_Type == SaveType.City_Folk)
            {
                if (ID == 0xFFF1)
                    return "Empty";
                else if ((ID >= 0xB710 && ID <= 0xCE50) || (ID >= 0x93F0 && ID <= 0x9414))
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
                //0x74 - 0x93 = Patterns
                //0x94 = Hole
                //0x95 - 0x9C = Turnip
                else if (ID >= 0x009E && ID <= 0x00DC)
                    return "Flower";
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
                else if (ID == 0xD000 || ID == 0x7003)
                    return "Building";
                else
                    return "Unknown";
            }
            else if (Save_Type == SaveType.New_Leaf) //perhaps add WA to this as well
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
                    return 0xC8868686;
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
                case "Unknown":
                    return 0xC8FF0000;
                default:
                    return 0xC8FF0000;
            }
        }

        public static string GetItemFlag1Type(Item Item, byte ItemFlag)
        {
            string ItemType = GetItemType(Item.ItemID, NewMainForm.Save_File.Save_Type);
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

        public static void AddVillagerHouses()
        {
            foreach (KeyValuePair<ushort, string> villager in VillagerData.Villagers)
                if (villager.Key >= 0xE000 && villager.Key <= 0xE0EB)
                {
                    ushort houseId = BitConverter.ToUInt16(new byte[2] { (byte)(villager.Key & 0xFF), 0x50 }, 0);
                    acreItemIDs.Add(houseId);
                    acreItemNames.Add(villager.Value + "'s House");
                }
        }

        public static ushort GetItemID(string itemName)
        {
            return (Array.IndexOf(Item_Names, itemName) < 0 ? (ushort)0 : Item_IDs[Array.IndexOf(Item_Names, itemName)]);
        }

        public static string GetItemName(ushort itemID)
        {
            var Found = ItemDatabase.Where(o => o.Key == itemID).Select(o => new { Key = o.Key, Value = o.Value }).FirstOrDefault();
            if (Found != null)
                return Found.Value;
            else
            {
                ushort BaseID = (ushort)(itemID - (itemID % 4));
                var FoundBase = ItemDatabase.Where(o => o.Key == BaseID).Select(o => new { Key = o.Key, Value = o.Value }).FirstOrDefault();
                if (FoundBase != null)
                    return FoundBase.Value;
                else
                    return "Unknown";
            }
        }

        //Position should be where the main item id goes
        public static void Place_Structure(string Struct_Name, int Position, ref WorldItem[] Acre_Data)
        {
            Structure Struct_to_Place = null;
            foreach (Structure Struct in World_Structures)
                if (Struct.Structure_Name == Struct_Name)
                {
                    Struct_to_Place = Struct;
                    break;
                }
            if (Struct_to_Place != null)
            {
                int Struct_ID_Position = Array.IndexOf(Struct_to_Place.Structure_World_Data, (ushort)2);
                int Struct_ID_X_Pos = Struct_ID_Position % Struct_to_Place.Structure_Width;
                int Struct_Rows = Struct_to_Place.Structure_World_Data.Length / Struct_to_Place.Structure_Width;
                int World_X_Position = Position % 16;
                if (World_X_Position <= 16 - Struct_ID_X_Pos && World_X_Position >= (Struct_to_Place.Structure_Width - Struct_ID_X_Pos)) //Fix Bounds checking
                {
                    int Starting_Position = Position - Struct_ID_X_Pos - 16 * (Struct_ID_Position / Struct_to_Place.Structure_Width); //Total Position - X-Pos - Y Pos
                    int x = 0;
                    for (int i = 0; i < Struct_Rows; i++)
                    {
                        for (int y = 0; y < Struct_to_Place.Structure_Width; y++)
                        {
                            int World_Pos = Starting_Position + y + i * 16;
                            Acre_Data[World_Pos] = Struct_to_Place.Structure_World_Data[x] == 1 ? new WorldItem(0xFFFF, World_Pos) : Acre_Data[World_Pos];
                            x++;
                        }
                    }
                }
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
                        int DataOffset = save.Save_Data_Start_Offset + NewMainForm.Current_Save_Info.Save_Offsets.Buildings + i * 4;
                        Buildings.Add(new Building(save.ReadByte(DataOffset), save.ReadByte(DataOffset + 2), save.ReadByte(DataOffset + 3), save.Save_Type));
                        //Technically, Building IDs are shorts, but since they only use the lower byte, we'll just ignore that
                    }
                else
                    for (int i = 0; i < 2; i++)
                    {
                        int DataOffset = save.Save_Data_Start_Offset + NewMainForm.Current_Save_Info.Save_Offsets.Island_Buildings + i * 4;
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
            SaveType saveType = NewMainForm.Save_File.Save_Type;
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

        public Item (ushort itemId, byte flag1, byte flag2)
        {
            ItemID = itemId;
            Flag1 = flag1;
            Flag2 = flag2;
            Name = ItemData.GetItemName(ItemID);
        }
    }

    public class WorldItem : Item
    {
        public Point Location;
        public int Index = 0;
        public bool Burried = false;
        public bool Watered = false;

        public WorldItem(ushort itemId, int position) : base(itemId)
        {
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(ushort itemId, byte flag1, byte flag2, int position) : this(itemId, position)
        {
            Flag1 = flag1;
            Flag2 = flag2;
            Burried = Flag1 == 0x80;
            Watered = Flag1 == 0x40;

        }

        public WorldItem(int position) : base()
        {
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(uint itemId, int position) : base(itemId)
        {
            Index = position;
            Location = new Point(position % 16, position / 16);
            ItemID = (ushort)itemId;
            Flag1 = (byte)(itemId >> 24);
            Flag2 = (byte)(itemId >> 16);
            Name = ItemData.GetItemName(ItemID);
            Burried = Flag1 == 0x80;
            Watered = Flag1 == 0x40;
        }
    }

    public class Furniture : Item
    {
        public ushort BaseItemID = 0;
        public int Rotation = 0;
        public bool IsFurniture = false;

        public Furniture(ushort itemId) : base(itemId)
        {
            BaseItemID = (ushort)(ItemID - (ItemID % 4));
            IsFurniture = ItemData.Furniture_IDs.Contains(BaseItemID);
            if (ItemData.Furniture_IDs.Contains(BaseItemID))
            {
                Rotation = (ItemID % 4) * 90;
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
