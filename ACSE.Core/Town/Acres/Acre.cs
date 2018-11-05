using System;
using System.IO;
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

        public WorldAcre(ushort acreId, int position, ushort[] items = null, byte[] buriedItemData = null,
            SaveType saveType = SaveType.AnimalCrossing, uint[] nlItems = null, int townPosition = -1) : base(acreId,
            position)
        {
            if (items != null)
            {
                for (var i = 0; i < 256; i++)
                {
                    AcreItems[i] = new WorldItem(items[i], i);
                    if (buriedItemData != null)
                        SetBuried(AcreItems[i], townPosition == -1 ? position : townPosition, buriedItemData, saveType);
                }
            }
            else if (nlItems != null)
            {
                for (var i = 0; i < 256; i++)
                {
                    AcreItems[i] = new WorldItem(nlItems[i], i);
                    //add buried logic
                }
            }
        }

        public WorldAcre(ushort acreId, int position) : base(acreId, position) { }

        public WorldAcre(ushort acreId, int position, uint[] items = null, byte[] buriedItemData = null, SaveType saveType = SaveType.AnimalCrossing)
            : this(acreId, position, null, null, saveType, items) { }

        public WorldAcre(ushort acreId, int position, WorldItem[] items, byte[] buriedItemData = null,
            SaveType saveType = SaveType.AnimalCrossing, int townPosition = -1) : base(acreId, position)
        {
            AcreItems = items;
            if (buriedItemData == null || townPosition <= -1) return;
            for (var i = 0; i < 256; i++)
                SetBuried(AcreItems[i], townPosition, buriedItemData, saveType);
        }

        //TODO: Change BuriedData from byte[] to ushort[] and use updated code
        private static int GetBuriedDataLocation(WorldItem item, int acre, SaveType saveType)
        {
            if (item == null) return -1;
            var worldPosition = 0;
            switch (saveType)
            {
                //15 - item.Location.X because it's stored as a ushort in memory w/ reversed endianess
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                case SaveType.CityFolk:
                    worldPosition = (acre * 256) + (15 - item.Location.X) + item.Location.Y * 16;
                    break;
                case SaveType.WildWorld:
                    worldPosition = (acre * 256) + item.Index;
                    break;
            }
            return worldPosition / 8;
        }

        public void SetBuriedInMemory(WorldItem item, int acre, byte[] buriedItemData, bool buried, SaveType saveType)
        {
            if (saveType == SaveType.NewLeaf || saveType == SaveType.WelcomeAmiibo) return;
            var buriedLocation = GetBuriedDataLocation(item, acre, saveType);
            if (buriedLocation > -1)
            {
                buriedItemData[buriedLocation].SetBit(item.Location.X % 8, buried);
                item.Buried = buriedItemData[buriedLocation].GetBit(item.Location.X % 8) == 1;
            }
            else
                item.Buried = false;
        }

        private static void SetBuried(WorldItem item, int acre, byte[] buriedItemData, SaveType saveType)
        {
            var burriedDataOffset = GetBuriedDataLocation(item, acre, saveType);
            if (burriedDataOffset > -1 && burriedDataOffset < buriedItemData.Length)
            {
                item.Buried = buriedItemData[burriedDataOffset].GetBit(item.Location.X % 8) == 1;
            }
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
