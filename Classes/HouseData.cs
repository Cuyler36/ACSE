using System;
using System.Reflection;
using ACSE.Classes.Utilities;

namespace ACSE
{
    public struct HouseOffsets
    {
        public int Room_Start;
        public int Room_Count;
        public int Room_Size;
        public int Layer_Size;
        public int Layer_Count;
        public int House_Upgrade_Size;
        public int Owning_Player_ID;
        public int Owning_Player_Name;
        public int Owning_Player_NameSize;
        public int Town_ID;
        public int Town_Name;
        public int Town_NameSize;
        public int Design;
        public int Design_Size;
        public int Room_Wallpaper;
        public int Room_Carpet;
        public int Bed; //CF Exclusive
        public int Roof_Color;
        public int Customization_Start; //NL Exclusive
        public int Mailbox;
        public int Mail_Size;
        public int Mail_Count;
        public int Gyroid_Items; //AC Exclusive
        public int Gryoid_Message; //AC Exclusive
    }

    public struct HouseData
    {
        public Room[] Rooms;
        public byte House_Upgrade_Size;
        public ushort Owning_Player_ID;
        public string Owning_Player_Name;
        public ushort Town_ID;
        public string Town_Name;
        public Pattern Design;
        public Item Bed;
        public byte Roof_Color;
        public Item[] Customizations;
        //public Mail[] Mailbox;
        public Gyroid_Item[] Gyroid_Items;
        public string Gyroid_Message;
    }

    public class Layer
    {
        public int Index = -1;
        public int Offset;
        public Furniture[] Items;
        public Room Parent;

        public void Write()
        {
            if (Items != null && Index > -1)
            {
                var SaveFile = NewMainForm.Save_File;
                if (SaveFile.Game_System == SaveGeneration.N3DS)
                {
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (Items[i] != null)
                        {
                            SaveFile.Write(Offset + i * 4, Items[i].ToUInt32());
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (Items[i] != null)
                        {
                            SaveFile.Write(Offset + i * 2, Items[i].ItemID, SaveFile.Is_Big_Endian);
                        }
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
            var SaveFile = NewMainForm.Save_File;

            foreach (Layer l in Layers)
            {
                l.Write();
            }

            var Offsets = HouseInfo.GetHouseOffsets(SaveFile.Save_Type);

            if (Offsets.Room_Carpet != -1)
                if (SaveFile.Game_System == SaveGeneration.N64 || SaveFile.Game_System == SaveGeneration.GCN) // TODO: Non-Original titles
                    SaveFile.Write(Offset + Offsets.Room_Carpet, (byte)(Carpet.ItemID));

            if (Offsets.Room_Wallpaper != -1)
                if (SaveFile.Game_System == SaveGeneration.N64 || SaveFile.Game_System == SaveGeneration.GCN) // TODO: Non-Original titles
                    SaveFile.Write(Offset + Offsets.Room_Wallpaper, (byte)(Wallpaper.ItemID));

            // TODO: Room_Song
        }
    }

    public class House
    {
        public int Index;
        public int Offset;
        public HouseData Data;
        public NewPlayer Owner;

        public House(int Index, int Offset)
        {
            this.Index = Index;
            this.Offset = Offset;

            int HouseSize = HouseInfo.GetHouseSize(Offset, NewMainForm.Save_File.Save_Type);
            bool Basement = false;
            //Console.WriteLine("House Index: " + Index);
            //Console.WriteLine("House Offset: 0x" + Offset.ToString("X"));
            //Console.WriteLine("House Size: " + HouseSize.ToString());
            if (NewMainForm.Save_File.Game_System == SaveGeneration.N64 || NewMainForm.Save_File.Game_System == SaveGeneration.GCN)
            {
                Basement = HouseInfo.HasBasement(Offset, NewMainForm.Save_File.Save_Type);
                //Console.WriteLine("Basement: " + Basement.ToString());
            }

            // Load House Data
            var Offsets = HouseInfo.GetHouseOffsets(NewMainForm.Save_File.Save_Type);
            var SaveData = NewMainForm.Save_File;
            Type PlayerDataType = typeof(HouseData);
            Type PlayerSaveInfoType = typeof(HouseOffsets);
            object BoxedData = new HouseData();
            foreach (var Field in PlayerSaveInfoType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                {
                    if (PlayerDataType.GetField(Field.Name) != null)
                    {
                        if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                        {
                            var Current_Field = PlayerDataType.GetField(Field.Name);
                            Type FieldType = Current_Field.FieldType;
                            int DataOffset = Offset + (int)Field.GetValue(Offsets);

                            if (!Field.Name.Equals("Room_Carpet") && !Field.Name.Equals("Room_Wallpaper") && !Field.Name.Equals("Room_Song"))
                            {
                                if (FieldType == typeof(byte))
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
                                else if (FieldType == typeof(Item))
                                    if (SaveData.Save_Type == SaveType.New_Leaf || SaveData.Save_Type == SaveType.Welcome_Amiibo)
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
                    }
                }
            }
            Data = (HouseData)BoxedData;

            // Load Rooms/Layers
            int ItemDataSize = NewMainForm.Save_File.Game_System == SaveGeneration.N3DS ? 4 : 2;
            int ItemsPerLayer = 256; //Offsets.Layer_Size / ItemDataSize;
            Data.Rooms = new Room[Offsets.Room_Count];
            var RoomNames = HouseInfo.GetRoomNames(SaveData.Game_System);

            for (int i = 0; i < Offsets.Room_Count; i++)
            {
                int RoomOffset = Offset + Offsets.Room_Start + i * Offsets.Room_Size;
                var Room = new Room
                {
                    Index = i,
                    Offset = RoomOffset,
                    Name = RoomNames[i],
                    Layers = new Layer[Offsets.Layer_Count]
                };

                if (SaveData.Game_System == SaveGeneration.N64 || SaveData.Game_System == SaveGeneration.GCN)
                {
                    Room.Carpet = new Item((ushort)(0x2600 | SaveData.ReadByte(RoomOffset + Offsets.Room_Carpet)));
                    Room.Wallpaper = new Item((ushort)(0x2700 | SaveData.ReadByte(RoomOffset + Offsets.Room_Wallpaper)));
                }

                for (int x = 0; x < Offsets.Layer_Count; x++)
                {
                    int LayerOffset = RoomOffset + Offsets.Layer_Size * x;
                    var Layer = new Layer
                    {
                        Offset = LayerOffset,
                        Index = x,
                        Items = new Furniture[ItemsPerLayer],
                        Parent = Room
                    };

                    // Load furniture for the layer
                    for (int f = 0; f < ItemsPerLayer; f++)
                    {
                        int FurnitureOffset = LayerOffset + f * ItemDataSize;
                        if (ItemDataSize == 4)
                        {
                            Layer.Items[f] = new Furniture(SaveData.ReadUInt32(FurnitureOffset));
                        }
                        else
                        {
                            Layer.Items[f] = new Furniture(SaveData.ReadUInt16(FurnitureOffset, SaveData.Is_Big_Endian));
                        }
                    }

                    Room.Layers[x] = Layer;
                }
                Data.Rooms[i] = Room;
            }
        }

        public void Write()
        {
            var SaveData = NewMainForm.Save_File;
            var Offsets = HouseInfo.GetHouseOffsets(SaveData.Save_Type);

            // Set House TownID & Name
            if (Offsets.Owning_Player_Name != -1 && Owner != null && Offsets.Town_ID != -1)
            {
                Data.Town_ID = SaveData.ReadUInt16(SaveData.Save_Data_Start_Offset + NewMainForm.Current_Save_Info.Save_Offsets.Town_ID, SaveData.Is_Big_Endian); // Might not be UInt16 in all games
            }
            if (Offsets.Owning_Player_Name != -1 && Owner != null && Offsets.Town_Name != -1)
            {
                Data.Town_Name = SaveData.ReadString(SaveData.Save_Data_Start_Offset + NewMainForm.Current_Save_Info.Save_Offsets.Town_Name,
                    NewMainForm.Current_Save_Info.Save_Offsets.Town_NameSize);
            }
            if (Offsets.Owning_Player_Name != -1 && Owner != null)
            {
                Data.Owning_Player_Name = Owner.Data.Name;
            }
            if (Offsets.Owning_Player_ID != -1 && Owner != null)
            {
                Data.Owning_Player_ID = Owner.Data.Identifier;
            }
            
            Type HouseOffsetData = typeof(HouseOffsets);
            Type HouseDataType = typeof(HouseData);
            foreach (var Field in HouseOffsetData.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Field.GetValue(Offsets) != null && !Field.Name.Contains("Count") && !Field.Name.Contains("Size"))
                {
                    if (HouseDataType.GetField(Field.Name) != null)
                    {
                        if (Field.FieldType == typeof(int) && (int)Field.GetValue(Offsets) != -1)
                        {
                            Type FieldType = HouseDataType.GetField(Field.Name).FieldType;
                            int DataOffset = Offset + (int)Field.GetValue(Offsets);
                            //MessageBox.Show("Field Name: " + Field.Name + " | Data Offset: " + DataOffset.ToString("X"));
                            if (FieldType == typeof(string))
                            {
                                SaveData.Write(DataOffset, ACString.GetBytes((string)HouseDataType.GetField(Field.Name).GetValue(Data),
                                    (int)HouseOffsetData.GetField(Field.Name + "Size").GetValue(Offsets)));
                            }
                            else if (FieldType == typeof(byte))
                            {
                                SaveData.Write(DataOffset, (byte)HouseDataType.GetField(Field.Name).GetValue(Data));
                            }
                            else if (FieldType == typeof(byte[]))
                            {
                                SaveData.Write(DataOffset, (byte[])HouseDataType.GetField(Field.Name).GetValue(Data));
                            }
                            else if (FieldType == typeof(ushort))
                            {
                                SaveData.Write(DataOffset, (ushort)HouseDataType.GetField(Field.Name).GetValue(Data), SaveData.Is_Big_Endian);
                            }
                            else if (FieldType == typeof(Item))
                            {
                                if (SaveData.Game_System == SaveGeneration.N3DS)
                                {
                                    SaveData.Write(DataOffset, ItemData.EncodeItem((Item)HouseDataType.GetField(Field.Name).GetValue(Data)), SaveData.Is_Big_Endian);
                                }
                                else
                                {
                                    SaveData.Write(DataOffset, ((Item)HouseDataType.GetField(Field.Name).GetValue(Data)).ItemID, SaveData.Is_Big_Endian);
                                }
                            }
                        }
                    }
                }
            }

            foreach (Room r in Data.Rooms)
                r.Write();
        }
    }

    public static class HouseInfo
    {
        private static  House[] Houses;

        public static HouseOffsets Animal_Crossing_House_Offsets = new HouseOffsets
        {
            // House Upgrade date: 0x26 (byte) 0x27 (byte) 0x28 (ushort) | Day, Month, Year
            Owning_Player_Name = 0,
            Owning_Player_NameSize = 8,
            Town_Name = 8,
            Town_NameSize = 8,
            Owning_Player_ID = 0x10,
            Town_ID = 0x12,
            House_Upgrade_Size = 0x2A,
            Roof_Color = 0x2C, // 0x2D = Roof color on next house upgrade?
            Room_Start = 0x38,
            Room_Count = 3,
            Room_Size = 0x8A8,
            Layer_Count = 4,
            Layer_Size = 0x228,
            Room_Carpet = 0x8A0,
            Room_Wallpaper = 0x8A1,
        };

        public static HouseOffsets Doubutsu_no_Mori_e_Plus_Offsets = new HouseOffsets
        {
            // House Upgrade date: 0x22 (byte) 0x23 (byte) 0x24 (ushort) | Day, Month, Year
            Owning_Player_Name = 0,
            Owning_Player_NameSize = 6,
            Town_Name = 6,
            Town_NameSize = 6,
            Owning_Player_ID = 0xC,
            Town_ID = 0xE,
            House_Upgrade_Size = 0x26, // Island is included in house upgrade size (size of 4)
            Roof_Color = 0x28, // 0x29 = Roof color on next house upgrade?
            Room_Start = 0x30,
            Room_Count = 3,
            Room_Size = 0x8A8,
            Layer_Count = 4,
            Layer_Size = 0x228,
            Room_Carpet = 0x8A0,
            Room_Wallpaper = 0x8A1,
            // 0x20 & 0x10 >> 4 == 1 is basement purchased
            // 0x26 bytes in = house upgrade process byte? & with 0xE0? (In AC, it's 0x2A & 0x2B)
        };

        public static HouseOffsets Wild_World_Offsets = new HouseOffsets
        {
            Room_Count = 5,
            Room_Size = 0x450,
            Room_Start = 0,
            Layer_Count = 2,
            Layer_Size = 0x200,
            Room_Carpet = 0x448,
            Room_Wallpaper = 0x44A,
            // Room_Song = 0x44C,
            Owning_Player_Name = -1,
            Town_Name = -1,
            Owning_Player_ID = -1,
            Town_ID = -1
        };

        public static HouseOffsets City_Folk_Offsets = new HouseOffsets
        {
            Room_Start = 0x8AC,
            Room_Count = 3,
            Room_Size = 0x458,
            Layer_Size = 0x200, //16 * 16 DWORDs
            Layer_Count = 2,
            House_Upgrade_Size = 0x15B4, //Also at 0x15B5

        };

        public static HouseOffsets New_Leaf_Offsets = new HouseOffsets //HouseData is duplicated starting at 0x9 (0x0 - 0x8)
        {
            House_Upgrade_Size = 0,
            Customization_Start = 1,
            //Style = 1,
            //DoorShape = 2,
            //Walls = 3,
            //Roof = 4,
            //Door = 5,
            //Fence = 6,
            //Pavement = 7,
            //Mailbox = 8,
            Room_Start = 0x44, //0x76,
            Room_Count = 6,
            Room_Size = 0x302,
            Layer_Size = 0x150,
            Layer_Count = 2
        };

        public static string[] AC_Room_Names = new string[3]
        {
            "First Floor", "Second Floor", "Basement"
        };

        public static string[] WW_Room_Names = new string[5]
        {
            "Entry Room", "Back Wing", "Right Wing", "Left Wing", "Second Floor"
        };

        public static string[] NL_Room_Names = new string[6]
        {
            "Entry Room", "Second Floor", "Basement", "Right Wing", "Left Wing", "Back Wing"
        };

        public static string[] AC_Roof_Colors = new string[12]
        {
            "Red", "Orange", "Yellow", "Pale Green", "Green", "Sky Blue", "Blue", "Purple", "Pink", "Black", "White", "Brown"
        };

        public static HouseOffsets GetHouseOffsets(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return Animal_Crossing_House_Offsets;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return Doubutsu_no_Mori_e_Plus_Offsets;
                case SaveType.Wild_World:
                    return Wild_World_Offsets;
                case SaveType.New_Leaf:
                case SaveType.Welcome_Amiibo:
                    return New_Leaf_Offsets;
                default:
                    return Animal_Crossing_House_Offsets;
            }
        }

        public static string[] GetRoofColors(SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return AC_Roof_Colors;

                default:
                    return new string[0];
            }
        }

        public static string[] GetRoomNames(SaveGeneration Generation)
        {
            switch (Generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.Wii:
                    return AC_Room_Names;

                case SaveGeneration.NDS:
                default:
                    return WW_Room_Names;

                case SaveGeneration.N3DS:
                    return NL_Room_Names;
            }
        }

        public static House GetHouse(NewPlayer Player, SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori:
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    HouseOffsets Current_Offsets = GetHouseOffsets(Save_Type);
                    for (int i = 0; i < 4; i++)
                    {
                        if (Houses[i].Owner == null && Houses[i].Data.Owning_Player_ID == Player.Data.Identifier && Houses[i].Data.Town_ID == Player.Data.TownIdentifier)
                        {
                            Houses[i].Owner = Player;
                            return Houses[i];
                        }
                    }
                    return null;
                default:
                    return null;
            }
        }

        public static int GetHouseSize(int Offset, SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing: // NOTE: N64 & GameCube titles don't include Basement in the size
                    return (NewMainForm.Save_File.Working_Save_Data[Offset + 0x2A] >> 5) & 7;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (NewMainForm.Save_File.Working_Save_Data[Offset + 0x26] >> 5) & 7;
                case SaveType.Wild_World:
                    return NewMainForm.Save_File.ReadByte(0xFAF8) & 7; // Not sure about this
                default:
                    return 0;
            }
        }

        public static int GetHouseUpgradeSize(int Offset, SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (NewMainForm.Save_File.Working_Save_Data[Offset + 0x26] >> 2) & 7;
                default:
                    return 0;
            }
        }

        public static int GetRoomSize(int Offset) // NL/WA only
        {
            return Math.Min(8, NewMainForm.Save_File.ReadByte(Offset - 0x44) * 2);
        }

        public static bool HasBasement(int Offset, SaveType Save_Type)
        {
            switch (Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return (NewMainForm.Save_File.Working_Save_Data[Offset + 0x24] & 0x10) == 0x10;
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (NewMainForm.Save_File.Working_Save_Data[Offset + 0x20] & 0x10) == 0x10;
                default:
                    return false;
            }
        }

        public static void SetHasBasement(bool Enabled, House SelectedHouse)
        {
            var SaveFile = NewMainForm.Save_File;
            if (SaveFile.Game_System == SaveGeneration.N64 || SaveFile.Game_System == SaveGeneration.GCN)
            {
                int BasementFlagOffset = SelectedHouse.Offset;
                switch (SaveFile.Save_Type)
                {
                    case SaveType.Animal_Crossing:
                        BasementFlagOffset += 0x24;
                        break;
                    case SaveType.Doubutsu_no_Mori_e_Plus:
                        BasementFlagOffset += 0x20;
                        break;
                }

                if (Enabled)
                {
                    SaveFile.Write(BasementFlagOffset, SaveFile.ReadByte(BasementFlagOffset) | 0x10);
                }
                else
                {
                    SaveFile.Write(BasementFlagOffset, SaveFile.ReadByte(BasementFlagOffset) & 0xEF);
                }
            }
        }

        public static House[] LoadHouses(Save SaveFile)
        {
            int HouseCount = SaveFile.Game_System == SaveGeneration.NDS ? 1 : 4;
            Houses = new House[HouseCount];

            for (int i = 0; i < HouseCount; i++)
            {
                Houses[i] = new House(i, SaveFile.Save_Data_Start_Offset + SaveFile.Save_Info.Save_Offsets.House_Data + i * SaveFile.Save_Info.Save_Offsets.House_Data_Size);
            }
            return Houses;
        }
    }
}
