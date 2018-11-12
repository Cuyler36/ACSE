using System;
using System.IO;
using System.Linq;
using ACSE.Core.Debug;
using ACSE.Core.Items;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Town.Acres
{
    public class Acre
    {
        public ushort AcreId;
        public ushort BaseAcreId; // Used in N64/GCN games as a shortcut
        public int Index;
        public string Name = "";

        public Acre(ushort acreId, int position)
        {
            AcreId = acreId;
            BaseAcreId = (ushort)(acreId & 0xFFFC);
            Index = position;
        }

        public Acre(byte acreId, int position)
        {
            AcreId = acreId;
            Index = position;
        }
    }

    public class WorldAcre : Acre
    {
        public WorldItem[] AcreItems = new WorldItem[16 * 16];
        public int TownIndex;

        public WorldAcre(ushort acreId, int position, ushort[] items = null, uint[] nlItems = null,
            int townPosition = -1) : base(acreId, position)
        {
            TownIndex = townPosition;

            if (items != null)
            {
                for (var i = 0; i < 256; i++)
                {
                    AcreItems[i] = new WorldItem(items[i], i);
                    AcreItems[i].Buried = IsItemBuried(AcreItems[i], Save.SaveInstance.SaveGeneration);
                }
            }
            else if (nlItems != null)
            {
                for (var i = 0; i < 256; i++)
                {
                    AcreItems[i] = new WorldItem(nlItems[i], i);
                }
            }
        }

        public WorldAcre(ushort acreId, int position) : base(acreId, position) { }

        public WorldAcre(ushort acreId, int position, uint[] items = null) : this(acreId, position, null, items)
        { }

        public WorldAcre(ushort acreId, int position, WorldItem[] items, int townPosition = -1) : base(acreId, position)
        {
            AcreItems = items;
            if (townPosition <= -1) return;
            for (var i = 0; i < 256; i++)
            {
                AcreItems[i].Buried = IsItemBuried(AcreItems[i], Save.SaveInstance.SaveGeneration);
            }
        }

        public bool IsItemBuried(Item item, SaveGeneration generation)
        {
            if (item == null || !AcreItems.Contains(item)) return false;

            var itemIdx = Array.IndexOf(AcreItems, item);
            if (itemIdx < 0 || itemIdx > 255) return false;

            int offset;
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                case SaveGeneration.Wii:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (TownIndex * 16 + itemIdx / 16) * 2;
                    return Save.SaveInstance.ReadUInt16(offset, true, true).GetBit(itemIdx % 16) == 1;

                case SaveGeneration.NDS:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (Index * 256 + itemIdx) / 8;
                    return Save.SaveInstance.ReadByte(offset, true).GetBit(itemIdx % 8) == 1;

                case SaveGeneration.N3DS:
                    return (item.Flag1 & 0x80) == 0x80;
            }

            return false;
        }

        public bool SetItemBuried(Item item, bool buried, SaveGeneration generation)
        {
            if (item == null || !AcreItems.Contains(item)) return false;

            var itemIdx = Array.IndexOf(AcreItems, item);
            if (itemIdx < 0 || itemIdx > 255) return false;

            int offset;
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                case SaveGeneration.Wii:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (TownIndex * 16 + itemIdx / 16) * 2;
                    Save.SaveInstance.Write(offset,
                        Save.SaveInstance.ReadUInt16(offset, true, true).SetBit(itemIdx % 16, buried), true, true);
                    break;

                case SaveGeneration.NDS:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (Index * 256 + itemIdx) / 8;
                    Save.SaveInstance.Write(offset,
                        Save.SaveInstance.ReadByte(offset, true).SetBit(itemIdx % 8, buried), true);
                    break;

                case SaveGeneration.N3DS:
                    item.Flag1 &= 0x7F;
                    break;
            }

            return IsItemBuried(item, generation);
        }

        /// <summary>
        /// Loads the Acre's default <see cref="Item"/>s from a file.
        /// </summary>
        /// <param name="saveFile">The current save file.</param>
        /// <returns>bool ItemsWereLoaded</returns>
        public bool LoadDefaultItems(Save saveFile)
        {
            var defaultAcreDataFolder = Path.Combine(PathUtility.GetExeDirectory(), "Resources", "Default Acre Items");

            switch (saveFile.SaveGeneration)
            {
                case SaveGeneration.GCN:
                    defaultAcreDataFolder += Path.DirectorySeparatorChar + "GCN" + Path.DirectorySeparatorChar;
                    break;
            }

            if (!Directory.Exists(defaultAcreDataFolder)) return false;
            var filePath = defaultAcreDataFolder + BaseAcreId.ToString("X4") + ".bin";
            if (!File.Exists(filePath)) return false;
            try
            {
                using (var fStream = new FileStream(filePath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(fStream))
                    {
                        for (var i = 0; i < fStream.Length / 2; i++)
                        {
                            if (i >= 0x100) // Don't read past the maximum item slot.
                                break;
                            AcreItems[i] = new WorldItem(reader.ReadUInt16().Reverse(), i);
                        }
                    }
                }
                return true;
            }
            catch
            {
                DebugUtility.DebugManagerInstance.WriteLine(
                    $"Unable to open default acre data for Acre Id 0x{BaseAcreId:X4}", DebugLevel.Error);
            }

            return false;
        }
    }
}
