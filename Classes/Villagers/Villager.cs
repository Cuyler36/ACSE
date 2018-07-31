using System;
using System.Linq;
using System.Reflection;
using System.Text;
using ACSE.Classes.Utilities;

namespace ACSE
{
    public class Villager
    {
        public VillagerOffsets Offsets;
        public VillagerDataStruct Data;
        public PlayerRelation[] PlayerRelations;
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
            Offsets = VillagerInfo.GetVillagerInfo(save.Save_Type);

            var structType = typeof(VillagerDataStruct);
            var offsetType = typeof(VillagerOffsets);

            if (save.Save_Type == SaveType.Wild_World)
                Exists = _saveData.ReadByte(offset + Offsets.VillagerId) != 0 && _saveData.ReadByte(offset + Offsets.VillagerId) != 0xFF;
            else
                Exists = _saveData.ReadUInt16(offset + Offsets.VillagerId, save.Is_Big_Endian) != 0 && _saveData.ReadUInt16(offset + Offsets.VillagerId, save.Is_Big_Endian) != 0xFFFF;
            object boxedData = new VillagerDataStruct();
            foreach (var field in offsetType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (field.GetValue(Offsets) == null || field.Name.Contains("Count") ||
                    field.Name.Contains("Size")) continue;
                if (structType.GetField(field.Name) == null) continue;
                if (field.FieldType != typeof(int) || (int)field.GetValue(Offsets) == -1) continue;

                var fieldType = structType.GetField(field.Name).FieldType;
                var dataOffset = Offset + (int)field.GetValue(Offsets);

                if (field.Name == "Villager_ID" && save.Save_Type == SaveType.Wild_World) // Villager IDs are only a byte in WW
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadByte(dataOffset));
                else if (fieldType == typeof(byte))
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadByte(dataOffset));
                else if (fieldType == typeof(byte[]) && offsetType.GetField(field.Name + "Count") != null)
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadByteArray(dataOffset,
                        (int)offsetType.GetField(field.Name + "Count").GetValue(Offsets)));
                else if (fieldType == typeof(ushort))
                    structType.GetField(field.Name).SetValue(boxedData,
                        _saveData.ReadUInt16(dataOffset, _saveData.Is_Big_Endian));
                else if (fieldType == typeof(ushort[]))
                    structType.GetField(field.Name).SetValue(boxedData, _saveData.ReadUInt16Array(dataOffset,
                        (int)offsetType.GetField(field.Name + "Count").GetValue(Offsets),
                        _saveData.Is_Big_Endian));
                else if (fieldType == typeof(uint))
                    structType.GetField(field.Name).SetValue(boxedData,
                        _saveData.ReadUInt32(dataOffset, _saveData.Is_Big_Endian));
                else if (fieldType == typeof(string))
                    structType.GetField(field.Name).SetValue(boxedData, new ACString(_saveData.ReadByteArray(
                                dataOffset,
                                (int)offsetType.GetField(field.Name + "Size").GetValue(Offsets)),
                            _saveData.Save_Type)
                        .Trim());
                else if (fieldType == typeof(Item))
                    structType.GetField(field.Name).SetValue(boxedData,
                        save.Save_Generation == SaveGeneration.N3DS
                            ? new Item(_saveData.ReadUInt32(dataOffset))
                            : new Item(_saveData.ReadUInt16(dataOffset, _saveData.Is_Big_Endian)));
                else if (fieldType == typeof(Item[]))
                {
                    var collection =
                        new Item[(int)offsetType.GetField(field.Name + "Count").GetValue(Offsets)];
                    for (var i = 0; i < collection.Length; i++)
                    {
                        if (save.Save_Generation == SaveGeneration.N3DS)
                        {
                            collection[i] = new Item(_saveData.ReadUInt32(dataOffset + i * 4));
                        }
                        else
                        {
                            collection[i] =
                                new Item(_saveData.ReadUInt16(dataOffset + i * 2, _saveData.Is_Big_Endian));
                        }
                    }

                    structType.GetField(field.Name).SetValue(boxedData, collection);
                }
            }

            Data = (VillagerDataStruct)boxedData;

            // Create Player Relations;
            if (save.Save_Type != SaveType.Animal_Crossing) return;
            {
                PlayerRelations = new PlayerRelation[7];
                for (var i = 0; i < 7; i++)
                {
                    PlayerRelations[i] = new ACPlayerRelation(save, this, Offset + 0x10 + i * 0x138);
                }
            }
        }

        public bool Boxed()
        {
            if (_saveData.Save_Generation == SaveGeneration.N3DS)
            {
                return (Data.Status & 1) == 1;
            }

            return false;
        }

        public override string ToString()
        {
            return Name ?? "Unknown";
        }

        public PlayerRelation GetPlayerRelation(Player player)
        {
            return PlayerRelations?.First(o => o.PlayerId == player.Data.Identifier && o.PlayerName.Equals(player.Data.Name));
        }

        public void ImportDlcVillager(byte[] dlcData, int dlcIndex)
        {
            if (_saveData.Save_Type != SaveType.Doubutsu_no_Mori_e_Plus || dlcData.Length < 0x10 ||
                Encoding.ASCII.GetString(dlcData, 0, 4) != "Yaz0") return;

            var offset = _saveData.Save_Data_Start_Offset + 0x24494 + dlcIndex * 0x749;
            _saveData.Write(offset, dlcData);
        }

        public void Write()
        {
            // Set Villager TownID & Name when villager exists.
            if (Exists)
            {
                if (Offsets.TownId != -1)
                {
                    Data.TownId = _saveData.ReadUInt16(_saveData.Save_Data_Start_Offset + MainForm.Current_Save_Info.Save_Offsets.Town_ID, _saveData.Is_Big_Endian); // Might not be UInt16 in all games
                }
                if (Offsets.TownName != -1)
                {
                    Data.TownName = _saveData.ReadString(_saveData.Save_Data_Start_Offset + MainForm.Current_Save_Info.Save_Offsets.Town_Name,
                        MainForm.Current_Save_Info.Save_Offsets.Town_NameSize);
                }
            }

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
                    case "Villager_ID" when _saveData.Save_Type == SaveType.Wild_World:
                        _saveData.Write(dataOffset, Convert.ToByte(dataObject));
                        break;
                    //Might not encompass City Folk
                    case "Villager_ID":
                        _saveData.Write(dataOffset, dataObject, _saveData.Is_Big_Endian);
                        break;
                    default:
                        if (fieldType == typeof(string))
                        {
                            _saveData.Write(dataOffset, ACString.GetBytes(dataObject, (int)villagerOffsetData.GetField(field.Name + "Size").GetValue(Offsets)));
                        }
                        else if (fieldType == typeof(byte[]))
                        {
                            _saveData.Write(dataOffset, dataObject, false);
                        }
                        else if (fieldType == typeof(Item))
                        {
                            switch (_saveData.Save_Generation)
                            {
                                case SaveGeneration.N3DS:
                                    _saveData.Write(dataOffset, ItemData.EncodeItem((Item)dataObject));
                                    break;
                                default:
                                    _saveData.Write(dataOffset, ((Item)dataObject).ItemID, _saveData.Is_Big_Endian);
                                    break;
                            }
                        }
                        else if (!fieldType.IsClass) // Don't write classes. If we're here, the that means that we haven't handled saving the class.
                        {
                            _saveData.Write(dataOffset, dataObject, _saveData.Is_Big_Endian);
                        }

                        break;
                }
            }

            // Write PlayerRelations
            if (Exists && PlayerRelations != null)
            {
                foreach (var relation in PlayerRelations)
                {
                    if (relation.Exists)
                    {
                        relation.Write();
                    }
                }
            }

            // Write the NameId for N64/GC/iQue games. TODO: Check if Wild World also has this.
            if (_saveData.Save_Generation != SaveGeneration.N64 && _saveData.Save_Generation != SaveGeneration.GCN &&
                _saveData.Save_Generation != SaveGeneration.iQue) return;

            switch (_saveData.Save_Type)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                    _saveData.Write(Offset + Offsets.NameId, Index == 15 ? (byte)0xFF : (byte)Data.VillagerId);
                    break;
                case SaveType.Doubutsu_no_Mori_e_Plus: // e+ doesn't set this byte, as they changed the SetNameID function
                    break;
                default:
                    _saveData.Write(Offset + Offsets.NameId, (byte)Data.VillagerId);
                    break;
            }
        }
    }
}
