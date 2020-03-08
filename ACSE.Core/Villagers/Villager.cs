using System;
using System.Linq;
using System.Reflection;
using System.Text;
using ACSE.Core.Items;
using ACSE.Core.Players;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;
using ACSE.Core.Villagers.AnimalMemories;

namespace ACSE.Core.Villagers
{
    public class Villager
    {
        public VillagerOffsets Offsets;
        public VillagerDataStruct Data;
        public AnimalMemory[] AnimalMemories;
        public readonly int Index;
        public readonly int Offset;
        public string Name;
        public bool Exists;
        private readonly Save _saveData;

        public Villager(int offset, int idx, Save save)
        {
            _saveData = save;
            Index = idx;
            Offset = offset;
            Offsets = VillagerInfo.GetVillagerInfo(save.SaveType);

            var structType = typeof(VillagerDataStruct);
            var offsetType = typeof(VillagerOffsets);

            if (save.SaveType == SaveType.WildWorld)
                Exists = _saveData.ReadByte(offset + Offsets.VillagerId) != 0 && _saveData.ReadByte(offset + Offsets.VillagerId) != 0xFF;
            else if (save.SaveType == SaveType.CityFolk)
                Exists = _saveData.ReadByte(offset) != 0;
            else
                Exists = _saveData.ReadUInt16(offset + Offsets.VillagerId, save.IsBigEndian) != 0 &&
                         _saveData.ReadUInt16(offset + Offsets.VillagerId, save.IsBigEndian) != 0xFFFF;
            object boxedData = new VillagerDataStruct();
            foreach (var field in offsetType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.GetValue(Offsets) == null || field.Name.Contains("Count") ||
                    field.Name.Contains("Size")) continue;
                if (structType.GetField(field.Name) == null) continue;
                if (field.FieldType != typeof(int) || (int)field.GetValue(Offsets) == -1) continue;

                var fieldType = structType.GetField(field.Name).FieldType;
                var dataOffset = Offset + (int)field.GetValue(Offsets);

                if (field.Name == "VillagerId" && save.SaveType == SaveType.WildWorld) // Villager IDs are only a byte in WW
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadByte(dataOffset));
                else if (fieldType == typeof(byte))
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadByte(dataOffset));
                else if (fieldType == typeof(byte[]) && offsetType.GetField(field.Name + "Count") != null)
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadByteArray(dataOffset,
                        (int)offsetType.GetField(field.Name + "Count").GetValue(Offsets)));
                else if (fieldType == typeof(ushort))
                    structType.GetField(field.Name).SetValue(boxedData,
                        _saveData.ReadUInt16(dataOffset, _saveData.IsBigEndian));
                else if (fieldType == typeof(ushort[]))
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadUInt16Array(dataOffset,
                        (int)offsetType.GetField(field.Name + "Count").GetValue(Offsets),
                        _saveData.IsBigEndian));
                else if (fieldType == typeof(uint))
                    structType.GetField(field.Name).SetValue(boxedData,
                        _saveData.ReadUInt32(dataOffset, _saveData.IsBigEndian));
                else if (fieldType == typeof(string))
                    structType.GetField(field.Name).SetValue(boxedData, new AcString(_saveData.ReadByteArray(
                                dataOffset,
                                (int)offsetType.GetField(field.Name + "Size").GetValue(Offsets)),
                            _saveData.SaveType)
                        .Trim());
                else if (fieldType == typeof(Item))
                    structType.GetField(field.Name).SetValue(boxedData,
                        save.SaveGeneration == SaveGeneration.N3DS
                            ? new Item(_saveData.ReadUInt32(dataOffset))
                            : new Item(_saveData.ReadUInt16(dataOffset, _saveData.IsBigEndian)));
                else if (fieldType == typeof(Item[]))
                {
                    var collection =
                        new Item[(int)offsetType.GetField(field.Name + "Count").GetValue(Offsets)];
                    for (var i = 0; i < collection.Length; i++)
                    {
                        if (save.SaveGeneration == SaveGeneration.N3DS)
                        {
                            collection[i] = new Item(_saveData.ReadUInt32(dataOffset + i * 4));
                        }
                        else
                        {
                            collection[i] =
                                new Item(_saveData.ReadUInt16(dataOffset + i * 2, _saveData.IsBigEndian));
                        }
                    }

                    structType.GetField(field.Name).SetValue(boxedData, collection);
                }
            }

            Data = (VillagerDataStruct)boxedData;

            // Set Villager Name for e+ TODO: Separate translated e+ & untranslated e+.
            if (_saveData.SaveType == SaveType.DoubutsuNoMoriEPlus)
            {
                Name = _saveData.ReadString(Offset + 0xC, 6);
            }
            else if (_saveData.SaveType == SaveType.AnimalForestEPlus)
            {
                Name = _saveData.ReadString(Offset + 0xC, 8);
            }

            // Create Player Relations;
            if (save.SaveType != SaveType.AnimalCrossing) return;
            {
                AnimalMemories = new AnimalMemory[7];
                for (var i = 0; i < 7; i++)
                {
                    AnimalMemories[i] = new AnimalCrossingAnimalMemory(save, this, Offset + 0x10 + i * 0x138);
                }
            }
        }

        public bool Boxed()
        {
            if (_saveData.SaveGeneration == SaveGeneration.N3DS)
            {
                return (Data.Status & 1) == 1;
            }

            return false;
        }

        public override string ToString()
        {
            return Name ?? "Unknown";
        }

        public AnimalMemory GetAnimalMemory(Player player)
        {
            return AnimalMemories?.First(o => o.PlayerId == player.Data.Identifier && o.PlayerName.Equals(player.Data.Name));
        }

        public void SetDlcVillager(int dlcIndex)
        {
            _saveData.Write(_saveData.SaveDataStartOffset + 0x24490,
                _saveData.ReadUInt32(_saveData.SaveDataStartOffset + 0x24490, _saveData.IsBigEndian) | (uint)(1 << dlcIndex),
                _saveData.IsBigEndian);
        }

        private void ClearDlcVillager(int dlcIndex)
        {
            _saveData.Write(_saveData.SaveDataStartOffset + 0x24490,
                _saveData.ReadUInt32(_saveData.SaveDataStartOffset + 0x24490) & (uint)~(1 << dlcIndex));
        }

        public void ImportDlcVillager(byte[] dlcData, int dlcIndex)
        {
            if ((_saveData.SaveType != SaveType.DoubutsuNoMoriEPlus && _saveData.SaveType != SaveType.AnimalForestEPlus) ||
                dlcData.Length < 0x10 || dlcData.Length > 0x73B || Encoding.ASCII.GetString(dlcData, 0, 4) != "Yaz0") return;

            SetDlcVillager((Data.VillagerId & 0xFF) - 0xEC);
            var offset = _saveData.SaveDataStartOffset + 0x24494 + dlcIndex * 0x749;
            _saveData.Write(offset, dlcData);
        }

        public void Write(string townName)
        {
            // Set Villager TownID & Name when villager exists.
            if (Exists && _saveData.SaveGeneration != SaveGeneration.iQue) // TODO: Once iQue text is implemented, remove this.
            {
                if (Offsets.TownId != -1)
                {
                    Data.TownId =
                        _saveData.ReadUInt16(
                            _saveData.SaveDataStartOffset + Save.SaveInstance.SaveInfo.SaveOffsets.TownId,
                            _saveData.IsBigEndian); // Might not be UInt16 in all games
                }
                if (!string.IsNullOrWhiteSpace(townName) && Offsets.TownName != -1)
                {
                    Data.TownName = townName;
                }
            }

            if (!Exists) return; // TODO: Do I need this? it overwrites important stuff in Animal Forest iQue if I don't have it.

            var villagerOffsetData = typeof(VillagerOffsets);
            var villagerDataType = typeof(VillagerDataStruct);
            foreach (var field in villagerOffsetData.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.GetValue(Offsets) == null || field.Name.Contains("Count") ||
                    field.Name.Contains("Size")) continue;
                if (villagerDataType.GetField(field.Name) == null) continue;
                if (field.FieldType != typeof(int) || (int)field.GetValue(Offsets) == -1) continue;

                var fieldType = villagerDataType.GetField(field.Name).FieldType;
                var dataOffset = Offset + (int)field.GetValue(Offsets);
                dynamic dataObject = villagerDataType.GetField(field.Name).GetValue(Data);

                switch (field.Name)
                {
                    case "VillagerId" when _saveData.SaveType == SaveType.WildWorld:
                        _saveData.Write(dataOffset, Convert.ToByte(dataObject));
                        break;
                    //Might not encompass City Folk
                    case "VillagerId":
                        _saveData.Write(dataOffset, dataObject, _saveData.IsBigEndian);
                        break;
                    default:
                        if (fieldType == typeof(string))
                        {
                            _saveData.Write(dataOffset,
                                AcString.GetBytes(dataObject,
                                    (int) villagerOffsetData.GetField(field.Name + "Size").GetValue(Offsets)));
                        }
                        else if (fieldType == typeof(byte))
                        {
                            _saveData.Write(dataOffset, (byte)dataObject);
                        }
                        else if (fieldType == typeof(ushort))
                        {
                            _saveData.Write(dataOffset, (ushort)dataObject, _saveData.IsBigEndian);
                        }
                        else if (fieldType == typeof(uint))
                        {
                            _saveData.Write(dataOffset, (uint)dataObject, _saveData.IsBigEndian);
                        }
                        else if (fieldType == typeof(byte[]))
                        {
                            _saveData.Write(dataOffset, dataObject, false);
                        }
                        else if (fieldType == typeof(Item))
                        {
                            switch (_saveData.SaveGeneration)
                            {
                                case SaveGeneration.N3DS:
                                    _saveData.Write(dataOffset, ItemData.EncodeItem((Item)dataObject));
                                    break;
                                default:
                                    _saveData.Write(dataOffset, ((Item)dataObject).ItemId, _saveData.IsBigEndian);
                                    break;
                            }
                        }
                        else if (fieldType == typeof(Item[]))
                        {
                            if (dataObject is Item[] collection)
                                for (var i = 0; i < collection.Length; i++)
                                {
                                    switch (_saveData.SaveGeneration)
                                    {
                                        case SaveGeneration.N3DS:
                                            _saveData.Write(dataOffset + i * 4, ItemData.EncodeItem(collection[i]));
                                            break;
                                        default:
                                            _saveData.Write(dataOffset + i * 2, collection[i].ItemId,
                                                _saveData.IsBigEndian);
                                            break;
                                    }
                                }
                        }
                        else if (!fieldType.IsClass) // Don't write classes. If we're here, the that means that we haven't handled saving the class.
                        {
                            _saveData.Write(dataOffset, dataObject, _saveData.IsBigEndian);
                        }

                        break;
                }
            }

            // Write AnimalMemories
            if (Exists && AnimalMemories != null)
            {
                foreach (var relation in AnimalMemories)
                {
                    if (relation.Exists)
                    {
                        relation.Write();
                    }
                }
            }

            // Write the NameId for N64/GC/iQue games, & name for e+. TODO: Check if Wild World also has this.
            if (_saveData.SaveGeneration != SaveGeneration.N64 && _saveData.SaveGeneration != SaveGeneration.GCN &&
                _saveData.SaveGeneration != SaveGeneration.iQue) return;

            switch (_saveData.SaveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                    _saveData.Write(Offset + Offsets.NameId, Index == 15 ? (byte)0xFF : (byte)Data.VillagerId);
                    break;
                case SaveType.DoubutsuNoMoriEPlus: // e+ doesn't set this byte, as they changed the SetNameID function
                    _saveData.Write(Offset + 0xC, AcString.GetBytes(Name, 6), false);
                    break;
                case SaveType.AnimalForestEPlus:
                    _saveData.Write(Offset + 0xC, AcString.GetBytes(Name, 8), false);
                    break;
                default:
                    _saveData.Write(Offset + Offsets.NameId, (byte)Data.VillagerId);
                    break;
            }
        }
    }
}
