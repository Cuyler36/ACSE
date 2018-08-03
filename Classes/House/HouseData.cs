using System;
using System.Reflection;
using ACSE.Classes.Utilities;

namespace ACSE
{
    public struct HouseOffsets
    {
        public int RoomStart;
        public int RoomCount;
        public int RoomSize;
        public int LayerSize;
        public int LayerCount;
        public int HouseUpgradeSize;
        public int OwningPlayerId;
        public int OwningPlayerName;
        public int OwningPlayerNameSize;
        public int TownId;
        public int TownName;
        public int TownNameSize;
        public int Design;
        public int DesignSize;
        public int RoomWallpaper;
        public int RoomCarpet;
        public int Bed; //CF Exclusive
        public int RoofColor;
        // NL Exclusives
        public int Style;
        public int DoorShape;
        public int ExteriorType;
        public int RoofType;
        public int DoorType;
        public int FenceType;
        public int PavementType;
        public int MailboxType;
        // End NL Exclusives
        public int Mailbox;
        public int MailSize;
        public int MailCount;
        public int GyroidItems; //AC Exclusive
        public int GryoidMessage; //AC Exclusive
    }

    public struct HouseData
    {
        public Room[] Rooms;
        public byte HouseUpgradeSize;
        public ushort OwningPlayerId;
        public string OwningPlayerName;
        public ushort TownId;
        public string TownName;
        public Pattern Design;
        public Item Bed;
        public byte RoofColor;
        public Item[] Customizations;
        //public Mail[] Mailbox;
        public GyroidItem[] GyroidItems;
        public string GyroidMessage;
    }

    public class Layer
    {
        public int Index = -1;
        public int Offset;
        public Furniture[] Items;
        public Room Parent;

        public void Write()
        {
            if (Items == null || Index <= -1) return;
            var saveFile = MainForm.Save_File;
            if (saveFile.Save_Generation == SaveGeneration.N3DS)
            {
                for (var i = 0; i < Items.Length; i++)
                {
                    if (Items[i] != null)
                    {
                        saveFile.Write(Offset + i * 4, Items[i].ToUInt32());
                    }
                }
            }
            else
            {
                for (var i = 0; i < Items.Length; i++)
                {
                    if (Items[i] != null)
                    {
                        saveFile.Write(Offset + i * 2, Items[i].ItemId, saveFile.Is_Big_Endian);
                    }
                }
            }
        }
    }

    public class Room
    {
        public int Index;
        public int Offset;
        public string Name;
        public Layer[] Layers;
        public Item Wallpaper;
        public Item Carpet;
        public Item Song;

        public void Write()
        {
            var saveFile = MainForm.Save_File;

            foreach (var l in Layers)
            {
                l.Write();
            }

            var offsets = HouseInfo.GetHouseOffsets(saveFile.Save_Type);

            if (offsets.RoomCarpet != -1)
                if (saveFile.Save_Generation == SaveGeneration.N64 || saveFile.Save_Generation == SaveGeneration.GCN) // TODO: Non-Original titles
                    saveFile.Write(Offset + offsets.RoomCarpet, (byte)(Carpet.ItemId));

            if (offsets.RoomWallpaper == -1) return;
            if (saveFile.Save_Generation == SaveGeneration.N64 || saveFile.Save_Generation == SaveGeneration.GCN) // TODO: Non-Original titles
                saveFile.Write(Offset + offsets.RoomWallpaper, (byte)(Wallpaper.ItemId));

            // TODO: Room_Song
        }
    }

    public static class HouseInfo
    {
        private static House[] _houses;

        public static readonly HouseOffsets DoubutsuNoMoriHouseOffsets = new HouseOffsets
        {
            OwningPlayerName = 0,
            OwningPlayerNameSize = 6,
            TownName = 6,
            TownNameSize = 6,
            OwningPlayerId = 0xC,
            TownId = 0xE,
            RoomStart = 0x38,
            RoomSize = 0x440,
            RoomCount = 1,
            LayerSize = 0x220,
            LayerCount = 2,
            RoomCarpet = -0x24, // Relative to the start of the house object
            RoomWallpaper = -0x23,
            Bed = -1,
            RoofColor = -1,
        };

        public static readonly HouseOffsets DoubutsuNoMoriPlusOffsets = new HouseOffsets
        {
            OwningPlayerName = 0,
            OwningPlayerNameSize = 6,
            TownName = 6,
            TownNameSize = 6,
            OwningPlayerId = 0xC,
            TownId = 0xE,
            RoomStart = 0x30,
            RoomSize = 0x8A8,
            RoomCount = 3,
            LayerSize = 0x228,
            LayerCount = 4,
            RoomCarpet = 0x8A0,
            RoomWallpaper = 0x8A1,
            RoofColor = 0x28,
            Bed = -1,
        };

        public static readonly HouseOffsets AnimalCrossingHouseOffsets = new HouseOffsets
        {
            // House Upgrade date: 0x26 (byte) 0x27 (byte) 0x28 (ushort) | Day, Month, Year
            OwningPlayerName = 0,
            OwningPlayerNameSize = 8,
            TownName = 8,
            TownNameSize = 8,
            OwningPlayerId = 0x10,
            TownId = 0x12,
            HouseUpgradeSize = 0x2A,
            RoofColor = 0x2C, // 0x2D = Roof color on next house upgrade?
            RoomStart = 0x38,
            RoomCount = 3,
            RoomSize = 0x8A8,
            LayerCount = 4,
            LayerSize = 0x228,
            RoomCarpet = 0x8A0,
            RoomWallpaper = 0x8A1,
            Bed = -1,
        };

        public static readonly HouseOffsets DoubutsuNoMoriEPlusOffsets = new HouseOffsets
        {
            // House Upgrade date: 0x22 (byte) 0x23 (byte) 0x24 (ushort) | Day, Month, Year
            OwningPlayerName = 0,
            OwningPlayerNameSize = 6,
            TownName = 6,
            TownNameSize = 6,
            OwningPlayerId = 0xC,
            TownId = 0xE,
            // 0x20 = Basement
            HouseUpgradeSize = 0x26, // Island is included in house upgrade size (size of 4)
            RoofColor = 0x28, // 0x29 = Roof color on next house upgrade?
            RoomStart = 0x30,
            RoomCount = 3,
            RoomSize = 0x8A8,
            LayerCount = 4,
            LayerSize = 0x228,
            RoomCarpet = 0x8A0,
            RoomWallpaper = 0x8A1,
            Bed = -1,
            // 0x20 & 0x10 >> 4 == 1 is basement purchased
            // 0x26 bytes in = house upgrade process byte? & with 0xE0? (In AC, it's 0x2A & 0x2B)
        };

        public static readonly HouseOffsets WildWorldOffsets = new HouseOffsets
        {
            RoomCount = 5,
            RoomSize = 0x450,
            RoomStart = 0,
            LayerCount = 2,
            LayerSize = 0x200,
            RoomCarpet = 0x448,
            RoomWallpaper = 0x44A,
            // Room_Song = 0x44C,
            OwningPlayerName = -1,
            TownName = -1,
            OwningPlayerId = -1,
            TownId = -1,
            Bed = -1,
            RoofColor = -1 // 
        };

        public static readonly HouseOffsets CityFolkOffsets = new HouseOffsets
        {
            // 0 - 0x870 = Flag Pattern
            TownId = 0x880,
            TownName = 0x882,
            TownNameSize = 16,
            OwningPlayerId = 0x896,
            OwningPlayerName = 0x898,
            OwningPlayerNameSize = 16,
            RoomStart = 0x8AC,
            RoomCount = 3,
            RoomSize = 0x458,
            LayerSize = 0x200, //16 * 16 DWORDs
            LayerCount = 2,
            RoomWallpaper = 0x44E,
            RoomCarpet = 0x450,
            HouseUpgradeSize = 0x15B4, //Also at 0x15B5
            Bed = 0x15B8,
            RoofColor = -1 //
        };

        public static readonly HouseOffsets NewLeafOffsets = new HouseOffsets //HouseData is duplicated starting at 0x9 (0x0 - 0x8)
        {
            HouseUpgradeSize = 0,
            Style = 1,
            DoorShape = 2,
            ExteriorType = 3,
            RoofType = 4,
            DoorType = 5,
            FenceType = 6,
            PavementType = 7,
            MailboxType = 8,
            RoomStart = 0x44, //0x76,
            RoomCount = 6,
            RoomSize = 0x302,
            LayerSize = 0x150,
            LayerCount = 2,
            Bed = -1,
            RoofColor = -1
        };

        #region Room Names

        private static readonly string[] DnMHouseNames = {
            "Main Room"
        };

        private static readonly string[] AcRoomNames = {
            "First Floor", "Second Floor", "Basement"
        };

        private static readonly string[] WwRoomNames = {
            "Entry Room", "Back Wing", "Right Wing", "Left Wing", "Second Floor"
        };

        private static readonly string[] NlRoomNames = {
            "Entry Room", "Second Floor", "Basement", "Right Wing", "Left Wing", "Back Wing"
        };

        #endregion

        #region House Sizes

        private static readonly string[] DnMHouseSizes = {
            "Small Room", "Medium Room", "Large Room"
        };

        private static readonly string[] AcHouseSizes = {
            "Small Room", "Medium Room", "Large Room", "Basement", "Second Floor", "Second Floor w/o Statue" // The basement is a separate flag
        };

        #endregion

        private static readonly string[] AcRoofColors = {
            "Red", "Orange", "Yellow", "Pale Green", "Green", "Sky Blue", "Blue", "Purple", "Pink", "Black", "White", "Brown"
        };

        public static HouseOffsets GetHouseOffsets(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori:
                    return DoubutsuNoMoriHouseOffsets;
                case SaveType.Doubutsu_no_Mori_Plus:
                    return DoubutsuNoMoriPlusOffsets;
                case SaveType.Animal_Crossing:
                    return AnimalCrossingHouseOffsets;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return DoubutsuNoMoriEPlusOffsets;
                case SaveType.Animal_Forest:
                    return DoubutsuNoMoriHouseOffsets; // TEMP
                case SaveType.Wild_World:
                    return WildWorldOffsets;
                case SaveType.City_Folk:
                    return CityFolkOffsets;
                case SaveType.New_Leaf:
                case SaveType.Welcome_Amiibo:
                    return NewLeafOffsets;
                default:
                    return AnimalCrossingHouseOffsets;
            }
        }

        public static string[] GetRoofColors(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                case SaveType.Animal_Forest:
                    return AcRoofColors;

                default:
                    return new string[0];
            }
        }

        public static string[] GetRoomNames(SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.iQue:
                    return DnMHouseNames;

                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    return AcRoomNames;

                case SaveGeneration.NDS:
                    return WwRoomNames;

                case SaveGeneration.N3DS:
                    return NlRoomNames;

                default:
                    return new string[0];
            }
        }

        public static string[] GetHouseSizes(SaveGeneration generation)
        {
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.iQue:
                    return DnMHouseSizes;
                case SaveGeneration.GCN:
                    return AcHouseSizes;
                default:
                    return new string[0];
            }
        }

        public static House GetHouse(Player player, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                case SaveType.Animal_Forest:
                case SaveType.City_Folk:
                    for (var i = 0; i < 4; i++)
                    {
                        if (_houses[i].Owner != null || _houses[i].Data.OwningPlayerId != player.Data.Identifier ||
                            _houses[i].Data.TownId != player.Data.TownIdentifier) continue;
                        _houses[i].Owner = player;
                        return _houses[i];
                    }
                    return null;
                default:
                    return null;
            }
        }

        public static int GetHouseSize(int offset, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Animal_Crossing: // NOTE: N64 & GameCube titles don't include Basement in the size
                    return (MainForm.Save_File.Working_Save_Data[offset + 0x2A] >> 5) & 7;
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (MainForm.Save_File.Working_Save_Data[offset + 0x26] >> 5) & 7;
                case SaveType.Wild_World:
                    return MainForm.Save_File.ReadByte(0xFAF8) & 7; // Not sure about this
                default:
                    return 0;
            }
        }

        public static void SetHouseSize(int offset, SaveType saveType, int value)
        {
            switch (saveType)
            {
                case SaveType.Animal_Crossing: // NOTE: N64 & GameCube titles don't include Basement in the size
                    MainForm.Save_File.Write(offset + 0x2A, (byte)(MainForm.Save_File.ReadByte(offset + 0x2A) & ~(7 << 5) | ((value & 7) << 5)));
                    break;
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    MainForm.Save_File.Write(offset + 0x26, (byte)(MainForm.Save_File.ReadByte(offset + 0x26) & ~(7 << 5) | ((value & 7) << 5)));
                    break;
                case SaveType.Wild_World:
                    MainForm.Save_File.Write(0xFAF8, (byte)((MainForm.Save_File.ReadByte(0xFAF8) & ~7) | (value & 7))); // Not sure about this
                    break;
            }
        }

        public static bool HasStatue(int offset, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                    return ((MainForm.Save_File.ReadByte(offset + 0x26) >> 2) & 7) == 4;
                case SaveType.Animal_Crossing:
                    return ((MainForm.Save_File.ReadByte(offset + 0x2A) >> 2) & 7) == 4;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return ((MainForm.Save_File.ReadByte(offset + 0x26) >> 2) & 7) == 5;
                default:
                    return false;
            }
        }

        public static void SetStatueEnabled(int offset, SaveType saveType, bool enabled)
        {
            int writeValue;
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                    writeValue = enabled ? (4 << 2) : (5 << 2);
                    MainForm.Save_File.Write(offset + 0x26, (byte)((MainForm.Save_File.ReadByte(offset + 0x26) & ~(7 << 2) | writeValue)));
                    break;
                case SaveType.Animal_Crossing:
                    writeValue = enabled ? (4 << 2) : (5 << 2);
                    MainForm.Save_File.Write(offset + 0x2A, (byte)((MainForm.Save_File.ReadByte(offset + 0x2A) & ~(7 << 2) | writeValue)));
                    break;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    writeValue = enabled ? (5 << 2) : (6 << 2);
                    MainForm.Save_File.Write(offset + 0x26, (byte)((MainForm.Save_File.ReadByte(offset + 0x26) & ~(7 << 2) | writeValue)));
                    break;
            }
        }

        public static int GetHouseUpgradeSize(int offset, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (MainForm.Save_File.Working_Save_Data[offset + 0x26] >> 2) & 7;
                default:
                    return 0;
            }
        }

        public static int GetRoomSize(int offset) // NL/WA only
        {
            return Math.Min(8, MainForm.Save_File.ReadByte(offset - 0x44) * 2);
        }

        public static bool HasBasement(int offset, SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Animal_Crossing:
                    return (MainForm.Save_File.Working_Save_Data[offset + 0x24] & 0x10) == 0x10;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (MainForm.Save_File.Working_Save_Data[offset + 0x20] & 0x10) == 0x10;
                default:
                    return false;
            }
        }

        public static void SetHasBasement(bool enabled, House selectedHouse)
        {
            var saveFile = MainForm.Save_File;
            if (saveFile.Save_Generation != SaveGeneration.N64 && saveFile.Save_Generation != SaveGeneration.GCN &&
                saveFile.Save_Generation != SaveGeneration.iQue) return;
            var basementFlagOffset = selectedHouse.Offset;
            switch (saveFile.Save_Type)
            {
                case SaveType.Animal_Crossing:
                    basementFlagOffset += 0x24;
                    break;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    basementFlagOffset += 0x20;
                    break;
            }

            if (enabled)
            {
                saveFile.Write(basementFlagOffset, saveFile.ReadByte(basementFlagOffset) | 0x10);
            }
            else
            {
                saveFile.Write(basementFlagOffset, saveFile.ReadByte(basementFlagOffset) & 0xEF);
            }
        }

        public static House[] LoadHouses(Save saveFile)
        {
            var houseCount = saveFile.Save_Generation == SaveGeneration.NDS ? 1 : 4;
            _houses = new House[houseCount];

            for (var i = 0; i < houseCount; i++)
            {
                _houses[i] = new House(i, saveFile.Save_Data_Start_Offset + saveFile.Save_Info.Save_Offsets.House_Data + i * saveFile.Save_Info.Save_Offsets.House_Data_Size);
            }
            return _houses;
        }
    }
}
