using System;
using System.Collections.Generic;
using System.Linq;
using ACSE.Core.Saves;

namespace ACSE.Core.Town.Buildings
{
    /// <summary>
    /// Building class for editing buildings in City Folk & New Leaf.
    /// </summary>
    public sealed class Building
    {
        /// <summary>
        /// The currently selected <see cref="Building"/>.
        /// </summary>
        public static Building SelectedBuilding;

        /// <summary>
        /// The current list of buildings in town.
        /// </summary>
        public static Building[] Buildings { get; private set; }

        /// <summary>
        /// The current list of buildings in the island.
        /// </summary>
        public static Building[] IslandBuildings { get; private set; }

        #region Building Names

        public static readonly string[] CityFolkBuildingNames = {
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

        public static readonly Dictionary<byte, string> NewLeafBuildingNames = new Dictionary<byte, string>
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

        public static readonly Dictionary<byte, string> WelcomeAmiiboBuildingNames = new Dictionary<byte, string>
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

        #endregion

        public byte Id;
        public bool Exists;
        public byte AcreIndex;
        public byte XPos;
        public byte YPos;
        public byte AcreX;
        public byte AcreY;
        public string Name;

        public Building(byte id, byte x, byte y, SaveType saveType)
        {
            Id = id;
            //Despite what previous editors assume, I'm fairly certain that the X & Y location bytes are structured like this:
            //Upper Nibble = Acre
            //Lower Nibble = Position in Acre
            //I say this, as a town hall in New Leaf with location bytes of X = 0x28, Y = 0x19 is positioned on the third X acre and second Y acre at 0x8, 0x9.
            var (xpos, acrex) = x.GetNibbles();
            var (ypos, acrey) = y.GetNibbles();
            XPos = xpos;
            AcreX = acrex;
            YPos = ypos;
            AcreY = acrey;

            AcreIndex = (byte)((AcreY - 1) * 5 + (AcreX - 1)); // * 5 works here because both CF and NL use 5 X acres
            if (AcreIndex > 24) //Works for NL too, since the dock is located in the 5th Y acre row.
                AcreIndex = 0;

            switch (saveType)
            {
                case SaveType.CityFolk:
                    Name = CityFolkBuildingNames[id];
                    Exists = AcreX > 0 && AcreY > 0;
                    break;
                case SaveType.NewLeaf:
                    Name = NewLeafBuildingNames[id];
                    Exists = Id != 0xF8;
                    break;
                case SaveType.WelcomeAmiibo:
                    Name = WelcomeAmiiboBuildingNames[id];
                    Exists = Id != 0xFC;
                    break;
            }
        }

        /// <summary>
        /// Loads any buildings that exist in a specific <see cref="SaveGeneration"/>.
        /// </summary>
        /// <param name="save">The current <see cref="Save"/> file.</param>
        /// <param name="islandBuildings">Load regular buildings or island buildings?</param>
        /// <returns></returns>
        public static Building[] LoadBuildings(Save save, bool islandBuildings = false)
        {
            var buildings = new List<Building>();
            switch (save.SaveGeneration)
            {
                case SaveGeneration.Wii:
                    IslandBuildings = null;

                    for (var i = 0; i < 33; i++)
                    {
                        var dataOffset = save.SaveDataStartOffset + SaveDataManager.CityFolkOffsets.Buildings + i * 2;
                        buildings.Add(new Building((byte) i, save.ReadByte(dataOffset), save.ReadByte(dataOffset + 1),
                            SaveType.CityFolk));
                    }

                    //Add Pave's Table
                    var paveOffset = save.SaveDataStartOffset + 0x5EB90;
                    var paveTable = new Building(0x21, save.ReadByte(paveOffset), save.ReadByte(paveOffset + 1),
                        SaveType.CityFolk);
                    buildings.Add(paveTable);

                    //Add Bus Stop
                    var busStopOffset = save.SaveDataStartOffset + 0x5EB8A;
                    var busStop = new Building(0x22, save.ReadByte(busStopOffset), save.ReadByte(busStopOffset + 1),
                        SaveType.CityFolk);
                    buildings.Add(busStop);

                    //Add Signs
                    for (var i = 0; i < 100; i++)
                    {
                        var dataOffset = save.SaveDataStartOffset + 0x5EB92 + i * 2;
                        var sign = new Building(0x23, save.ReadByte(dataOffset), save.ReadByte(dataOffset + 1),
                            SaveType.CityFolk);
                        buildings.Add(sign);
                    }

                    break;

                case SaveGeneration.N3DS:
                    if (islandBuildings == false)
                    {
                        for (var i = 0; i < 58; i++)
                        {
                            var dataOffset = save.SaveDataStartOffset + Save.SaveInstance.SaveInfo.SaveOffsets.Buildings +
                                             i * 4;
                            buildings.Add(new Building(save.ReadByte(dataOffset), save.ReadByte(dataOffset + 2),
                                save.ReadByte(dataOffset + 3), save.SaveType));
                            //Technically, Building IDs are shorts, but since they only use the lower byte, we'll just ignore that
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            var dataOffset = save.SaveDataStartOffset +
                                             Save.SaveInstance.SaveInfo.SaveOffsets.IslandBuildings + i * 4;
                            buildings.Add(new Building(save.ReadByte(dataOffset), save.ReadByte(dataOffset + 2),
                                save.ReadByte(dataOffset + 3), save.SaveType));
                        }
                    }

                    break;

                default:
                    IslandBuildings = null;
                    return Buildings = null;
            }

            if (islandBuildings)
            {
                return IslandBuildings = buildings.ToArray();
            }

            return Buildings = buildings.ToArray();
        }

        /// <summary>
        /// Queries whether or not a <see cref="Building"/> is at the specified acre and location.
        /// </summary>
        /// <param name="acre">The acre to check.</param>
        /// <param name="x">The desired x-coordinate to check.</param>
        /// <param name="y">The desired y-coordinate to check.</param>
        /// <param name="islandAcre">Is the acre an island acre, or a regular town acre? Defaults to false.</param>
        /// <returns>Building buildingFound (can be null)</returns>
        public static Building IsBuildingHere(int acre, int x, int y, bool islandAcre = false) => islandAcre
            ? IslandBuildings?.FirstOrNull(b => b.Exists && b.AcreIndex == acre && b.XPos == x && b.YPos == y)
            : Buildings?.FirstOrNull(b => b.Exists && b.AcreIndex == acre && b.XPos == x && b.YPos == y);
    }
}
