using System;
using System.IO;

namespace ACSE
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

        public WorldAcre(ushort acreId, int position, ushort[] items = null, byte[] buriedItemData = null, SaveType saveType = SaveType.Animal_Crossing, uint[] nlItems = null, int townPosition = -1) : base(acreId, position)
        {
            if (items != null)
            {
                for (var i = 0; i < 256; i++)
                {
                    AcreItems[i] = new WorldItem(items[i], i);
                    if (buriedItemData != null)
                        SetBuried(AcreItems[i], townPosition == -1 ? position : townPosition, buriedItemData, saveType); //Broken in original save editor lol.. needs a position - 1 to function properly
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

        public WorldAcre(ushort acreId, int position, uint[] items = null, byte[] burriedItemData = null, SaveType saveType = SaveType.Animal_Crossing)
            : this(acreId, position, null, null, saveType, items) { }

        public WorldAcre(ushort acreId, int position, WorldItem[] items, byte[] buriedItemData = null, SaveType saveType = SaveType.Animal_Crossing, int townPosition = -1) : base(acreId, position)
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
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                case SaveType.City_Folk:
                    worldPosition = (acre * 256) + (15 - item.Location.X) + item.Location.Y * 16; //15 - item.Location.X because it's stored as a ushort in memory w/ reversed endianess
                    break;
                case SaveType.Wild_World:
                    worldPosition = (acre * 256) + item.Index;
                    break;
            }
            return worldPosition / 8;
        }

        public void SetBuriedInMemory(WorldItem item, int acre, byte[] burriedItemData, bool buried, SaveType saveType)
        {
            if (saveType == SaveType.New_Leaf || saveType == SaveType.Welcome_Amiibo) return;
            var buriedLocation = GetBuriedDataLocation(item, acre, saveType);
            if (buriedLocation > -1)
            {
                DataConverter.SetBit(ref burriedItemData[buriedLocation], item.Location.X % 8, buried);
                item.Buried = DataConverter.ToBit(burriedItemData[buriedLocation], item.Location.X % 8) == 1;
            }
            else
                item.Buried = false;
        }
        //Correct decoding/setting of buried items. Fixes the hacky SaveType case for AC/CF. (Don't forget to implement this!)
        private void SetBuriedInMemoryFixed(WorldItem item, int acre, ushort[] buriedItemData, bool buried, SaveType saveType)
        {
            if (saveType == SaveType.New_Leaf || saveType == SaveType.Welcome_Amiibo) return;
            var buriedLocation = (acre * 256 + item.Index) / 16;
            if (buriedLocation > -1)
            {
                var buriedRowBytes = BitConverter.GetBytes(buriedItemData[buriedLocation]);
                DataConverter.SetBit(ref buriedRowBytes[item.Location.X / 8], item.Location.X % 8, buried); //Should probably rewrite bit editing functions to take any data type
                item.Buried = DataConverter.ToBit(buriedRowBytes[item.Location.X / 8], item.Location.X % 8) == 1;
                buriedItemData[buriedLocation] = BitConverter.ToUInt16(buriedRowBytes, 0);
            }
            else
                item.Buried = false;
        }

        private static void SetBuried(WorldItem item, int acre, byte[] burriedItemData, SaveType saveType)
        {
            var burriedDataOffset = GetBuriedDataLocation(item, acre, saveType);
            if (burriedDataOffset > -1 && burriedDataOffset < burriedItemData.Length)
                item.Buried = DataConverter.ToBit(burriedItemData[burriedDataOffset], item.Location.X % 8) == 1;
        }

        /// <summary>
        /// Loads the Acre's default <see cref="Item"/>s from a file.
        /// </summary>
        /// <param name="saveFile">The current save file.</param>
        /// <returns>bool ItemsWereLoaded</returns>
        public bool LoadDefaultItems(Save saveFile)
        {
            var defaultAcreDataFolder = MainForm.Assembly_Location + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "Default Acre Items";

            switch (saveFile.Save_Generation)
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
                MainForm.Debug_Manager.WriteLine(
                    $"Unable to open default acre data for Acre Id 0x{BaseAcreId:X4}", DebugLevel.Error);
            }

            return false;
        }
    }
}
