using System;
using System.Reflection;
using ACSE.Core.Encryption;
using ACSE.Core.Housing;
using ACSE.Core.Imaging;
using ACSE.Core.Items;
using ACSE.Core.Messages.Mail;
using ACSE.Core.Patterns;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Players
{
    public sealed class Player
    {
        public readonly PlayerSaveInfo Offsets;
        public PlayerData Data;
        public House House;
        public readonly int Index;
        public readonly int Offset;
        public bool Exists;

        public Player(int index)
        {
            Index = index;
        }

        public Player(int offset, int idx)
        {
            Index = idx;
            Offset = offset;
            Offsets = PlayerInfo.GetPlayerInfo(Save.SaveInstance.SaveType);
            Exists = Save.SaveInstance.ReadByte(offset + Offsets.Identifier) != 0 && Save.SaveInstance.ReadByte(offset + Offsets.Identifier) != 0xFF;
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
                            case "TownPassCardImage" when Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS:
                                playerDataType.GetField("TownPassCardData").SetValue(boxedData, Save.SaveInstance.ReadByteArray(dataOffset, 0x1400));
                                currentField.SetValue(boxedData,
                                    ImageGeneration.GetTpcImage((byte[])playerDataType.GetField("TownPassCardData").GetValue(boxedData)));
                                break;
                            case "Reset" when Save.SaveInstance.SaveGeneration == SaveGeneration.GCN:
                                currentField.SetValue(boxedData, Save.SaveInstance.ReadUInt32(dataOffset, Save.SaveInstance.IsBigEndian) != 0);
                                break;
                            default:
                                if (fieldType == typeof(byte))
                                    currentField.SetValue(boxedData, Save.SaveInstance.ReadByte(dataOffset));
                                else if (fieldType == typeof(byte[]) && playerSaveInfoType.GetField(field.Name + "Count") != null)
                                    currentField.SetValue(boxedData, Save.SaveInstance.ReadByteArray(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets)));
                                else if (fieldType == typeof(ushort))
                                    currentField.SetValue(boxedData, Save.SaveInstance.ReadUInt16(dataOffset, Save.SaveInstance.IsBigEndian));
                                else if (fieldType == typeof(ushort[]))
                                    currentField.SetValue(boxedData, Save.SaveInstance.ReadUInt16Array(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets), Save.SaveInstance.IsBigEndian));
                                else if (fieldType == typeof(uint))
                                    currentField.SetValue(boxedData, Save.SaveInstance.ReadUInt32(dataOffset, Save.SaveInstance.IsBigEndian));
                                else if (fieldType == typeof(string))
                                    currentField.SetValue(boxedData, new AcString(Save.SaveInstance.ReadByteArray(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Size").GetValue(Offsets)), Save.SaveInstance.SaveType).Trim());
                                else if (fieldType == typeof(Inventory))
                                    if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                                        currentField.SetValue(boxedData, new Inventory(Save.SaveInstance.ReadUInt32Array(dataOffset,
                                            (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets), false)));
                                    else
                                        currentField.SetValue(boxedData, new Inventory(Save.SaveInstance.ReadUInt16Array(dataOffset,
                                            (int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets), Save.SaveInstance.IsBigEndian), this));
                                else if (fieldType == typeof(Item))
                                    if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                                        currentField.SetValue(boxedData, new Item(Save.SaveInstance.ReadUInt32(dataOffset, false)));
                                    else
                                        currentField.SetValue(boxedData, new Item(Save.SaveInstance.ReadUInt16(dataOffset, Save.SaveInstance.IsBigEndian)));
                                else if (fieldType == typeof(Item[]))
                                {
                                    if (field.Name.Equals("Dressers"))
                                    {
                                        switch (Save.SaveInstance.SaveGeneration)
                                        {
                                            case SaveGeneration.NDS:
                                                dataOffset = Save.SaveInstance.SaveDataStartOffset + 0x15430 + 0xB4 * Index; // Terrible hack
                                                break;
                                            case SaveGeneration.Wii:
                                                dataOffset = Save.SaveInstance.SaveDataStartOffset + 0x1F3038 + 0x140 * Index;
                                                break;
                                        }
                                    }

                                    var itemArray = new Item[(int)playerSaveInfoType.GetField(field.Name + "Count").GetValue(Offsets)];
                                    for (var i = 0; i < itemArray.Length; i++)
                                    {
                                        if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                                            itemArray[i] = new Item(Save.SaveInstance.ReadUInt32(dataOffset + i * 4, false));
                                        else
                                            itemArray[i] = new Item(Save.SaveInstance.ReadUInt16(dataOffset + i * 2, Save.SaveInstance.IsBigEndian));
                                    }
                                    currentField.SetValue(boxedData, itemArray);
                                }
                                else if (fieldType == typeof(NewLeafInt32))
                                {
                                    var intData = Save.SaveInstance.ReadUInt32Array(dataOffset, 2);
                                    currentField.SetValue(boxedData, new NewLeafInt32(intData[0], intData[1]));
                                }
                                else if (fieldType == typeof(AcDate) && dataOffset > 0)
                                {
                                    currentField.SetValue(boxedData, new AcDate(Save.SaveInstance.ReadByteArray(dataOffset,
                                        (int)playerSaveInfoType.GetField(field.Name + "Size").GetValue(Offsets))));
                                }
                                break;
                        }
                    }
            Data = (PlayerData)boxedData;
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.WildWorld:
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
                case SaveType.CityFolk:
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
                    Data.Patterns[i] = new Pattern(offset + Offsets.Patterns + Offsets.PatternSize * i, i);
            }

            if (Save.SaveInstance.SaveType == SaveType.CityFolk)
            {
                Data.Reset = (Save.SaveInstance.ReadByte(Offset + 0x8670) & 0x02) == 0x02;
            }
            else if (Save.SaveInstance.SaveType == SaveType.NewLeaf)
            {
                Data.Reset = (Save.SaveInstance.ReadByte(Offset + 0x5702) & 0x02) == 0x02;
            }
            else if (Save.SaveInstance.SaveType == SaveType.WelcomeAmiibo)
            {
                Data.Reset = (Save.SaveInstance.ReadByte(Offset + 0x570A) & 0x02) == 0x02;
            }

            // Get the Player's House
            House = HouseInfo.GetHouse(this, Save.SaveInstance.SaveType);
            if (House != null)
            {
                if (House.Data.Bed != null)
                {
                    Data.Bed = House.Data.Bed;
                }
            }

            Console.WriteLine($"Player {Index}'s house = {House}");

            // Mail Test
            if (Save.SaveInstance.SaveGeneration != SaveGeneration.GCN) return;
            {
                for (var i = 0; i < 10; i++)
                {
                    //var mail = new GcnPlayerMail(Save.SaveInstance, this, i);
                    //System.Windows.Forms.MessageBox.Show(Mail.GetFormattedMailString());
                }
            }
        }

        public void Write()
        {
            // Set City Folk Bed first
            if (Save.SaveInstance.SaveGeneration == SaveGeneration.Wii && House != null && Data.Bed != null)
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
                    case "TownPassCardImage" when Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS:
                        Save.SaveInstance.Write(dataOffset, Data.TownPassCardData);
                        break;
                    case "Reset" when Save.SaveInstance.SaveGeneration == SaveGeneration.GCN:
                        Save.SaveInstance.Write(dataOffset, Data.Reset ? (uint)0x250C : (uint)0 ,Save.SaveInstance.IsBigEndian);
                        break;
                    default:
                        if (fieldType == typeof(string))
                        {
                            Save.SaveInstance.Write(dataOffset, AcString.GetBytes((string)playerDataType.GetField(field.Name).GetValue(Data),
                                (int)playerSaveInfoType.GetField(field.Name + "Size").GetValue(Offsets)));
                        }
                        else if (fieldType == typeof(byte))
                        {
                            if (Save.SaveInstance.SaveType == SaveType.WildWorld)
                            {
                                switch (field.Name)
                                {
                                    case "HairColor":
                                    {
                                        var condensedData = (byte)(Data.HairColor & 0x0F); //Remove upper nibble just incase
                                        condensedData += (byte)((Data.Tan & 0x0F) << 4); //Add in tan to the byte
                                        Save.SaveInstance.Write(dataOffset, condensedData);
                                        break;
                                    }
                                    case "HairType":
                                    {
                                        var condensedData = (byte)(Data.FaceType & 0x0F);
                                        condensedData += (byte)((Data.HairType & 0x0F) << 4);
                                        Save.SaveInstance.Write(dataOffset, condensedData);
                                        break;
                                    }
                                    default:
                                        Save.SaveInstance.Write(dataOffset, (byte)playerDataType.GetField(field.Name).GetValue(Data));
                                        break;
                                }
                            }
                            else if (Save.SaveInstance.SaveType == SaveType.CityFolk)
                            {
                                switch (field.Name)
                                {
                                    case "Tan":
                                        var shiftedData = (byte)(Data.Tan << 1); //ACToolkit does this
                                        Save.SaveInstance.Write(dataOffset, shiftedData);
                                        break;
                                    case "FaceType":
                                        Save.SaveInstance.Write(dataOffset, (byte)(Data.EyeColor + Data.FaceType));
                                        break;
                                    default:
                                        Save.SaveInstance.Write(dataOffset, (byte)playerDataType.GetField(field.Name).GetValue(Data));
                                        break;
                                }
                            }
                            else
                            {
                                Save.SaveInstance.Write(dataOffset, (byte)playerDataType.GetField(field.Name).GetValue(Data));
                            }
                        }
                        else if (fieldType == typeof(ushort))
                        {
                            Save.SaveInstance.Write(dataOffset, (ushort)playerDataType.GetField(field.Name).GetValue(Data), Save.SaveInstance.IsBigEndian);
                        }
                        else if (fieldType == typeof(uint))
                        {
                            Save.SaveInstance.Write(dataOffset, (uint)playerDataType.GetField(field.Name).GetValue(Data), Save.SaveInstance.IsBigEndian);
                        }
                        else if (fieldType == typeof(Inventory))
                        {
                            if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                            {
                                var items = new uint[Offsets.PocketsCount];
                                for (var i = 0; i < items.Length; i++)
                                    items[i] = ItemData.EncodeItem(Data.Pockets.Items[i]);
                                Save.SaveInstance.Write(dataOffset, items);
                            }
                            else
                            {
                                var items = new ushort[Offsets.PocketsCount];
                                for (var i = 0; i < items.Length; i++)
                                    items[i] = Data.Pockets.Items[i].ItemId;
                                Save.SaveInstance.Write(dataOffset, items, Save.SaveInstance.IsBigEndian);
                            }
                        }
                        else if (fieldType == typeof(Item))
                        {
                            var item = (Item)playerDataType.GetField(field.Name).GetValue(Data);
                            if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                            {
                                Save.SaveInstance.Write(dataOffset, item.ToUInt32());
                            }
                            else
                            {
                                if (field.Name.Equals("Shirt") &&
                                    (Save.SaveInstance.SaveGeneration == SaveGeneration.N64 || Save.SaveInstance.SaveGeneration == SaveGeneration.GCN ||
                                     Save.SaveInstance.SaveGeneration == SaveGeneration.iQue)) // For some reason, the shirt lower byte is also stored before the actual item id.
                                {
                                    Save.SaveInstance.Write(dataOffset - 1, new[] { (byte)item.ItemId, (byte)(item.ItemId >> 8), (byte)item.ItemId }, Save.SaveInstance.IsBigEndian);
                                }
                                else
                                {
                                    Save.SaveInstance.Write(dataOffset, item.ItemId, Save.SaveInstance.IsBigEndian);
                                }
                            }
                        }
                        else if (fieldType == typeof(Item[]))
                        {
                            var itemArray = (Item[])playerDataType.GetField(field.Name).GetValue(Data);
                            if (field.Name.Equals("Dressers"))
                            {
                                switch (Save.SaveInstance.SaveGeneration)
                                {
                                    case SaveGeneration.NDS:
                                        dataOffset = Save.SaveInstance.SaveDataStartOffset + 0x15430 + 0xB4 * Index; // Terrible hack
                                        break;
                                    case SaveGeneration.Wii:
                                        dataOffset = Save.SaveInstance.SaveDataStartOffset + 0x1F3038 + 0x140 * Index;
                                        break;
                                }
                            }

                            for (var i = 0; i < itemArray.Length; i++)
                            {
                                if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
                                    Save.SaveInstance.Write(dataOffset + i * 4, itemArray[i].ToUInt32());
                                else
                                    Save.SaveInstance.Write(dataOffset + i * 2, itemArray[i].ItemId, Save.SaveInstance.IsBigEndian);
                            }
                        }
                        else if (fieldType == typeof(NewLeafInt32))
                        {
                            if (Save.SaveInstance.SaveGeneration == SaveGeneration.NDS)
                            {
                                var encryptedInt = (NewLeafInt32)playerDataType.GetField(field.Name).GetValue(Data);
                                Save.SaveInstance.Write(dataOffset, encryptedInt.Int1);
                                Save.SaveInstance.Write(dataOffset + 4, encryptedInt.Int2);
                            }
                        }
                        else if (fieldType == typeof(AcDate) && (Save.SaveInstance.SaveGeneration == SaveGeneration.GCN ||
                                                                 Save.SaveInstance.SaveGeneration == SaveGeneration.NDS ||
                                                                 Save.SaveInstance.SaveGeneration == SaveGeneration.Wii))
                        {
                            if (field.Name.Equals("Birthday"))
                            {
                                Save.SaveInstance.Write(dataOffset, ((AcDate)playerDataType.GetField(field.Name).GetValue(Data)).ToMonthDayDateData());
                            }
                        }

                        break;
                }
            }
        }
    }
}
