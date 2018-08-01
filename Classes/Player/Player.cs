using System;
using System.Reflection;
using ACSE.Classes.Utilities;
using ACSE.Messages.Mail;

namespace ACSE
{
    public sealed class Player
    {
        public readonly PlayerSaveInfo Offsets;
        public PlayerData Data;
        public House House;
        public readonly int Index;
        public readonly int Offset;
        public bool Exists;
        private readonly Save _saveData;

        public Player(int offset, int idx, Save save)
        {
            _saveData = save;
            Index = idx;
            Offset = offset;
            Offsets = PlayerInfo.GetPlayerInfo(save.Save_Type);
            Exists = _saveData.ReadByte(offset + Offsets.Identifier) != 0 && _saveData.ReadByte(offset + Offsets.Identifier) != 0xFF;
            if (!Exists) return;

            var playerDataType = typeof(PlayerData);
            var playerSaveInfoType = typeof(PlayerSaveInfo);
            object boxedData = new PlayerData();
            foreach (var field in playerSaveInfoType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                if (field.GetValue(Offsets) != null && !field.Name.Contains("Count") && !field.Name.Contains("Size"))
                    if (playerDataType.GetField(field.Name) != null)
                    {
                        if (field.FieldType != typeof(int) || (int) field.GetValue(Offsets) == -1) continue;

                        var currentField = playerDataType.GetField(field.Name);
                        var fieldType = currentField.FieldType;
                        var dataOffset = Offset + (int)field.GetValue(Offsets);

                        switch (field.Name)
                        {
                            case "TownPassCardImage" when save.Save_Generation == SaveGeneration.N3DS:
                                playerDataType.GetField("TownPassCardData").SetValue(boxedData, _saveData.ReadByteArray(dataOffset, 0x1400));
                                currentField.SetValue(boxedData,
                                    ImageGeneration.GetTPCImage((byte[])playerDataType.GetField("TownPassCardData").GetValue(boxedData)));
                                break;
                            case "Reset" when save.Save_Generation == SaveGeneration.GCN:
                                currentField.SetValue(boxedData, _saveData.ReadUInt32(dataOffset, _saveData.Is_Big_Endian) != 0);
                                break;
                            default:
                                if (fieldType == typeof(byte))
                                    currentField.SetValue(boxedData, _saveData.ReadByte(dataOffset));
                                else if (fieldType == typeof(byte[]) && playerSaveInfoType.GetField(field.Name + "Count") != null)
                                    currentField.SetValue(boxedData, _saveData.ReadByteArray(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets)));
                                else if (fieldType == typeof(ushort))
                                    currentField.SetValue(boxedData, _saveData.ReadUInt16(dataOffset, _saveData.Is_Big_Endian));
                                else if (fieldType == typeof(ushort[]))
                                    currentField.SetValue(boxedData, _saveData.ReadUInt16Array(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets), _saveData.Is_Big_Endian));
                                else if (fieldType == typeof(uint))
                                    currentField.SetValue(boxedData, _saveData.ReadUInt32(dataOffset, _saveData.Is_Big_Endian));
                                else if (fieldType == typeof(string))
                                    currentField.SetValue(boxedData, new ACString(_saveData.ReadByteArray(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Size").GetValue(Offsets)), _saveData.Save_Type).Trim());
                                else if (fieldType == typeof(Inventory))
                                    if (save.Save_Generation == SaveGeneration.N3DS)
                                        currentField.SetValue(boxedData, new Inventory(_saveData.ReadUInt32Array(dataOffset,
                                            (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets), false), save, this));
                                    else
                                        currentField.SetValue(boxedData, new Inventory(_saveData.ReadUInt16Array(dataOffset,
                                            (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets), _saveData.Is_Big_Endian), save, this));
                                else if (fieldType == typeof(Item))
                                    if (save.Save_Generation == SaveGeneration.N3DS)
                                        currentField.SetValue(boxedData, new Item(_saveData.ReadUInt32(dataOffset, false)));
                                    else
                                        currentField.SetValue(boxedData, new Item(_saveData.ReadUInt16(dataOffset, _saveData.Is_Big_Endian)));
                                else if (fieldType == typeof(Item[]))
                                {
                                    if (field.Name.Equals("Dressers"))
                                    {
                                        switch (_saveData.Save_Generation)
                                        {
                                            case SaveGeneration.NDS:
                                                dataOffset = 0x15430 + 0xB4 * Index; // Terrible hack
                                                break;
                                            case SaveGeneration.Wii:
                                                dataOffset = 0x1F3038 + 0x140 * Index;
                                                break;
                                        }
                                    }

                                    var itemArray = new Item[(int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets)];
                                    for (var i = 0; i < itemArray.Length; i++)
                                    {
                                        if (save.Save_Generation == SaveGeneration.N3DS)
                                            itemArray[i] = new Item(_saveData.ReadUInt32(dataOffset + i * 4, false));
                                        else
                                            itemArray[i] = new Item(_saveData.ReadUInt16(dataOffset + i * 2, _saveData.Is_Big_Endian));
                                    }
                                    currentField.SetValue(boxedData, itemArray);
                                }
                                else if (fieldType == typeof(NL_Int32))
                                {
                                    var intData = _saveData.ReadUInt32Array(dataOffset, 2);
                                    currentField.SetValue(boxedData, new NL_Int32(intData[0], intData[1]));
                                }
                                else if (fieldType == typeof(ACDate) && dataOffset > 0)
                                {
                                    currentField.SetValue(boxedData, new ACDate(_saveData.ReadByteArray(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Size").GetValue(Offsets))));
                                }
                                break;
                        }
                    }
            Data = (PlayerData)boxedData;
            switch (save.Save_Type)
            {
                case SaveType.Wild_World:
                    var condensedData = Data.HairColor;
                    Data.HairColor = (byte)(condensedData & 0x0F);
                    Data.Tan = (byte)((condensedData & 0xF0) >> 4); //Has to be 0 - 3
                    condensedData = Data.HairType;
                    Data.FaceType = (byte)(condensedData & 0x0F);
                    Data.HairType = (byte)((condensedData & 0xF0) >> 4);

                    if (Data.Tan > 3)
                        Data.Tan = 0;
                    if (Data.HairColor > 7)
                        Data.HairColor = 0;
                    break;
                case SaveType.City_Folk:
                    Data.Tan = (byte)(Data.Tan >> 1); //Not 100% sure about this, but this is what ACToolkit does
                    if (Data.Tan > 7)
                        Data.Tan = 0;
                    if (Data.HairType > 0x19)
                        Data.HairType = 0x19;
                    Data.FaceType = (byte)(Data.FaceType & 0x0F);
                    Data.EyeColor = (byte)(Data.FaceType & 0xF0); //Not actually eye color, just there to hold the upper nibble
                    break;
            }
            if (Offsets.Patterns > -1)
            {
                Data.Patterns = new Pattern[Offsets.PatternCount];
                for (var i = 0; i < Data.Patterns.Length; i++)
                    Data.Patterns[i] = new Pattern(offset + Offsets.Patterns + Offsets.PatternSize * i, i, save);
            }

            if (_saveData.Save_Type == SaveType.City_Folk)
            {
                Data.Reset = (_saveData.ReadByte(Offset + 0x8670) & 0x02) == 0x02;
            }
            else if (_saveData.Save_Type == SaveType.New_Leaf)
            {
                Data.Reset = (_saveData.ReadByte(Offset + 0x5702) & 0x02) == 0x02;
            }
            else if (_saveData.Save_Type == SaveType.Welcome_Amiibo)
            {
                Data.Reset = (_saveData.ReadByte(Offset + 0x570A) & 0x02) == 0x02;
            }

            // Get the Player's House
            House = HouseInfo.GetHouse(this, save.Save_Type);
            if (House != null)
            {
                if (House.Data.Bed != null)
                {
                    Data.Bed = House.Data.Bed;
                }
            }

            Console.WriteLine($"Player {Index}'s house = {House}");

            // Mail Test
            if (_saveData.Save_Generation != SaveGeneration.GCN) return;
            {
                for (var i = 0; i < 10; i++)
                {
                    var mail = new GCNPlayerMail(_saveData, this, i);
                    //System.Windows.Forms.MessageBox.Show(Mail.GetFormattedMailString());
                }
            }
        }

        public void Write()
        {
            // Set City Folk Bed first
            if (_saveData.Save_Generation == SaveGeneration.Wii && House != null && Data.Bed != null)
            {
                House.Data.Bed = Data.Bed;
                Data.Bed = null;
            }

            var playerSaveInfoType = typeof(PlayerSaveInfo);
            var playerDataType = typeof(PlayerData);
            foreach (var field in playerSaveInfoType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.GetValue(Offsets) == null || field.Name.Contains("Count") ||
                    field.Name.Contains("Size")) continue;
                if (playerDataType.GetField(field.Name) == null) continue;
                if (field.FieldType != typeof(int) || (int) field.GetValue(Offsets) == -1) continue;

                var fieldType = typeof(PlayerData).GetField(field.Name).FieldType;
                var dataOffset = Offset + (int)field.GetValue(Offsets);
                switch (field.Name)
                {
                    case "TownPassCardImage" when _saveData.Save_Generation == SaveGeneration.N3DS:
                        _saveData.Write(dataOffset, Data.TownPassCardData);
                        break;
                    case "Reset" when _saveData.Save_Generation == SaveGeneration.GCN:
                        _saveData.Write(dataOffset, Data.Reset ? (uint)0x250C : (uint)0 ,_saveData.Is_Big_Endian);
                        break;
                    default:
                        if (fieldType == typeof(string))
                        {
                            _saveData.Write(dataOffset, ACString.GetBytes((string)playerDataType.GetField(field.Name).GetValue(Data),
                                (int)playerSaveInfoType.GetField(field.Name + "Size").GetValue(Offsets)));
                        }
                        else if (fieldType == typeof(byte))
                        {
                            if (_saveData.Save_Type == SaveType.Wild_World)
                            {
                                switch (field.Name)
                                {
                                    case "HairColor":
                                    {
                                        var condensedData = (byte)(Data.HairColor & 0x0F); //Remove upper nibble just incase
                                        condensedData += (byte)((Data.Tan & 0x0F) << 4); //Add in tan to the byte
                                        _saveData.Write(dataOffset, condensedData);
                                        break;
                                    }
                                    case "HairType":
                                    {
                                        var condensedData = (byte)(Data.FaceType & 0x0F);
                                        condensedData += (byte)((Data.HairType & 0x0F) << 4);
                                        _saveData.Write(dataOffset, condensedData);
                                        break;
                                    }
                                    default:
                                        _saveData.Write(dataOffset, (byte)playerDataType.GetField(field.Name).GetValue(Data));
                                        break;
                                }
                            }
                            else if (_saveData.Save_Type == SaveType.City_Folk)
                            {
                                switch (field.Name)
                                {
                                    case "Tan":
                                        var shiftedData = (byte)(Data.Tan << 1); //ACToolkit does this
                                        _saveData.Write(dataOffset, shiftedData);
                                        break;
                                    case "FaceType":
                                        _saveData.Write(dataOffset, (byte)(Data.EyeColor + Data.FaceType));
                                        break;
                                    default:
                                        _saveData.Write(dataOffset, (byte)playerDataType.GetField(field.Name).GetValue(Data));
                                        break;
                                }
                            }
                            else
                            {
                                _saveData.Write(dataOffset, (byte)playerDataType.GetField(field.Name).GetValue(Data));
                            }
                        }
                        else if (fieldType == typeof(ushort) || fieldType == typeof(uint))
                        {
                            _saveData.Write(dataOffset, (dynamic)playerDataType.GetField(field.Name).GetValue(Data), _saveData.Is_Big_Endian);
                        }
                        else if (fieldType == typeof(Inventory))
                        {
                            if (_saveData.Save_Generation == SaveGeneration.N3DS)
                            {
                                var items = new uint[Offsets.PocketsCount];
                                for (var i = 0; i < items.Length; i++)
                                    items[i] = ItemData.EncodeItem(Data.Pockets.Items[i]);
                                _saveData.Write(dataOffset, items);
                            }
                            else
                            {
                                var items = new ushort[Offsets.PocketsCount];
                                for (var i = 0; i < items.Length; i++)
                                    items[i] = Data.Pockets.Items[i].ItemID;
                                _saveData.Write(dataOffset, items, _saveData.Is_Big_Endian);
                            }
                        }
                        else if (fieldType == typeof(Item))
                        {
                            var item = (Item)playerDataType.GetField(field.Name).GetValue(Data);
                            if (_saveData.Save_Generation == SaveGeneration.N3DS)
                            {
                                _saveData.Write(dataOffset, item.ToUInt32());
                            }
                            else
                            {
                                if (field.Name.Equals("Shirt") &&
                                    (_saveData.Save_Generation == SaveGeneration.N64 || _saveData.Save_Generation == SaveGeneration.GCN ||
                                     _saveData.Save_Generation == SaveGeneration.iQue)) // For some reason, the shirt lower byte is also stored before the actual item id.
                                {
                                    _saveData.Write(dataOffset - 1, new[] { (byte)item.ItemID, (byte)(item.ItemID >> 8), (byte)item.ItemID }, _saveData.Is_Big_Endian);
                                }
                                else
                                {
                                    _saveData.Write(dataOffset, item.ItemID, _saveData.Is_Big_Endian);
                                }
                            }
                        }
                        else if (fieldType == typeof(Item[]))
                        {
                            var itemArray = (Item[])playerDataType.GetField(field.Name).GetValue(Data);
                            if (field.Name.Equals("Dressers"))
                            {
                                switch (_saveData.Save_Generation)
                                {
                                    case SaveGeneration.NDS:
                                        dataOffset = 0x15430 + 0xB4 * Index; // Terrible hack
                                        break;
                                    case SaveGeneration.Wii:
                                        dataOffset = 0x1F3038 + 0x140 * Index;
                                        break;
                                }
                            }

                            for (var i = 0; i < itemArray.Length; i++)
                            {
                                if (_saveData.Save_Generation == SaveGeneration.N3DS)
                                    _saveData.Write(dataOffset + i * 4, itemArray[i].ToUInt32());
                                else
                                    _saveData.Write(dataOffset + i * 2, itemArray[i].ItemID, _saveData.Is_Big_Endian);
                            }
                        }
                        else if (fieldType == typeof(NL_Int32))
                        {
                            if (_saveData.Save_Generation == SaveGeneration.NDS)
                            {
                                var encryptedInt = (NL_Int32)playerDataType.GetField(field.Name).GetValue(Data);
                                _saveData.Write(dataOffset, encryptedInt.Int_1);
                                _saveData.Write(dataOffset + 4, encryptedInt.Int_2);
                            }
                        }
                        else if (fieldType == typeof(ACDate) && (_saveData.Save_Generation == SaveGeneration.GCN || _saveData.Save_Generation == SaveGeneration.NDS))
                        {
                            if (field.Name.Equals("Birthday"))
                            {
                                _saveData.Write(dataOffset, ((ACDate)playerDataType.GetField(field.Name).GetValue(Data)).ToMonthDayDateData());
                            }
                        }

                        break;
                }
            }
        }
    }
}
