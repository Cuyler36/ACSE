using System;
using System.Collections.Generic;
using System.Linq;
using ACSE.Core.Housing;
using ACSE.Core.Items;
using ACSE.Core.Patterns;
using ACSE.Core.Players;
using ACSE.Core.Saves;
using ACSE.Core.Town.Acres;
using ACSE.Core.Villagers;

namespace ACSE.Core.Town.Island
{
    /// <summary>
    /// Island class for Doubutsu no Mori e+
    /// </summary>
    public sealed class Island
    {
        #region Island Offsets

        private const int IslandName = 0x00;
        private const int TownNameOffset = 0x06;
        private const int IslandId = 0x0C;
        private const int TownIdOffset = 0x0E;
        private const int WorldData = 0x10;
        private const int CottageData = 0x418;
        private const int FlagData = 0xD00;
        private const int IslanderData = 0xF20;
        private const int BuriedData = 0x15A0;
        private const int IslandLeftAcreData = 0x15E0;
        private const int IslandRightAcreData = 0x15E1;
        private const int IslandGrassType = 0x15F8;
        private const int IslandInfoFlag = 0x15FB;
        
        // Info:
        // 0x00000000 = Haven't spoken to islander yet.
        // Current Year, Month, Day = Spoke to islander, wll build house on next reload.
        // 0xFFFFFFFF = House has been built.
        // 0x80008080 = Reset islander by importing a new islander via the wishing well after house is built.
        private const int IslanderAppearYearMonthDay = 0x1600;

        #endregion

        private readonly Save _saveFile;
        private readonly int _offset;
        public string Name;
        public ushort Id;
        public string TownName;
        public ushort TownId;
        public Player Owner;
        public WorldAcre[] Acres;
        public House Cabana;
        public Villager Islander;
        public Pattern FlagPattern;
        public byte[] BuriedDataArray;
        public byte IslandLeftAcreIndex, IslandRightAcreIndex;
        public bool Purchased;

        public Island(int offset, IEnumerable<Player> players, Save saveFile)
        {
            _saveFile = saveFile;
            _offset = offset;

            Name = new Utilities.AcString(saveFile.ReadByteArray(offset + IslandName, 6), saveFile.SaveType).Trim();
            Id = saveFile.ReadUInt16(offset + IslandId, true);

            TownName = new Utilities.AcString(saveFile.ReadByteArray(offset + TownNameOffset, 6), saveFile.SaveType).Trim();
            TownId = saveFile.ReadUInt16(offset + TownIdOffset, true);

            var identifier = saveFile.ReadUInt16(offset - 0x2214, true);
            foreach (var player in players)
            {
                if (player != null && player.Data.Identifier == identifier)
                {
                    Owner = player;
                }
            }

            BuriedDataArray = saveFile.ReadByteArray(offset + BuriedData, 0x40);

            Acres = new WorldAcre[2];
            for (var acre = 0; acre < 2; acre++)
            {
                Acres[acre] = new WorldAcre(0, acre,
                    saveFile.ReadUInt16Array(offset + WorldData + acre * 0x200, 0x100, true), null, acre, true);
            }

            Cabana = new House(-1, offset + CottageData, 1, 0);
            Cabana.Data.Rooms[0].Name = "Cabana";
            
            FlagPattern = new Pattern(offset + FlagData, 0);
            Islander = new Villager(offset + IslanderData, 15, saveFile);
            Purchased = IsPurchased();

            IslandLeftAcreIndex = saveFile.ReadByte(offset + IslandLeftAcreData);
            IslandRightAcreIndex = saveFile.ReadByte(offset + IslandRightAcreData);
        }

        private static ushort IslandAcreIndexToIslandAcreId(byte side, byte index)
        {
            // Left side
            if (side == 0)
            {
                switch (index)
                {
                    case 0:
                        return 0x04A4;
                    case 1:
                        return 0x0598;
                    case 2:
                        return 0x05A0;
                    case 3:
                        return 0x05A8;
                    default:
                        return 0;
                }
            }

            switch (index)
            {
                case 0:
                    return 0x04A0;
                case 1:
                    return 0x0594;
                case 2:
                    return 0x059C;
                case 3:
                    return 0x05A4;
                default:
                    return 0;
            }
        }

        private bool IsPurchased()
            => (_saveFile.ReadByte(_offset + IslandInfoFlag) & 0x80) == 0x80;

        public void SetPurchased(bool purchased)
            => _saveFile.Write(_offset + IslandInfoFlag, _saveFile.ReadByte(_offset + IslandInfoFlag).SetBit(7, purchased));

        public ushort[] GetAcreIds()
        {
            return new[] { IslandAcreIndexToIslandAcreId(0, IslandLeftAcreIndex), IslandAcreIndexToIslandAcreId(1, IslandRightAcreIndex) };
        }

        public bool IsItemBuried(WorldAcre acre, Item item, SaveGeneration generation)
        {
            if (item == null || !acre.Items.Contains(item)) return false;

            var itemIdx = Array.IndexOf(acre.Items, item);
            if (itemIdx < 0 || itemIdx > 255) return false;

            int offset;
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                case SaveGeneration.Wii:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (acre.Index * 16 + itemIdx / 16) * 2;
                    return Save.SaveInstance.ReadUInt16(offset, true, true).GetBit(itemIdx % 16) == 1;

                case SaveGeneration.NDS:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (acre.Index * 256 + itemIdx) / 8;
                    return Save.SaveInstance.ReadByte(offset, true).GetBit(itemIdx % 8) == 1;

                case SaveGeneration.N3DS:
                    return (item.Flag1 & 0x80) == 0x80;
            }

            return false;
        }

        public bool IsItemBuried(WorldAcre acre, Item item) =>
            IsItemBuried(acre, item, Save.SaveInstance.SaveGeneration);

        public bool SetItemBuried(WorldAcre acre, Item item, bool buried, SaveGeneration generation)
        {
            if (item == null || !acre.Items.Contains(item)) return false;

            var itemIdx = Array.IndexOf(acre.Items, item);
            if (itemIdx < 0 || itemIdx > 255) return false;

            int offset;
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                case SaveGeneration.iQue:
                case SaveGeneration.Wii:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (acre.Index * 16 + itemIdx / 16) * 2;
                    Save.SaveInstance.Write(offset,
                        Save.SaveInstance.ReadUInt16(offset, true, true).SetBit(itemIdx % 16, buried), true, true);
                    break;

                case SaveGeneration.NDS:
                    offset = Save.SaveInstance.SaveInfo.SaveOffsets.BuriedData + (acre.Index * 256 + itemIdx) / 8;
                    Save.SaveInstance.Write(offset,
                        Save.SaveInstance.ReadByte(offset, true).SetBit(itemIdx % 8, buried), true);
                    break;

                case SaveGeneration.N3DS:
                    item.Flag1 &= 0x7F;
                    break;
            }

            return IsItemBuried(acre, item, generation);
        }

        private bool FindIslandItem(ushort itemId, out int acreX, out int x, out int y)
        {
            acreX = x = y = -1;
            for (var i = 0; i < 2; i++)
                if (Acres[i].FindItem(itemId, out x, out y))
                {
                    acreX = 4 + i;
                    return true;
                }

            return false;
        }

        // NOTE: The "castaway" islander will not spawn if the house coordinates are not set.
        public void ResetIslanderHasAppeared()
        {
            // Search for Islander House.
            if (FindIslandItem(0x5851, out var acreX, out var x, out var y))
                Acres[acreX - 4].Items[y * 16 + x] = new Item(0x5871); // Replace it with the islander sign.
            else if (FindIslandItem(0x5871, out acreX, out x, out y) == false) return; // If we don't find the sign then return immediately.

            Islander.Data.HouseCoordinates = new byte[] { (byte)acreX, 8, (byte)x, (byte)y }; // Update "House Coordinates", which point to the sign.
            _saveFile.Write(_offset + IslanderAppearYearMonthDay, 0u, true); // Reset the "appear date" which is actually the date the house is built.
        }

        public void Write()
        {
            if (Owner != null)
            {
                _saveFile.Write(_offset + 0x00, Utilities.AcString.GetBytes(Name, 6));
                _saveFile.Write(_offset + 0x06, Utilities.AcString.GetBytes(TownName, 6));
                _saveFile.Write(_offset + 0x0C, Id, true);
                _saveFile.Write(_offset + 0x0E, TownId, true);
            }

            // Save World Items
            for (var acre = 0; acre < 2; acre++)
            {
                for (var item = 0; item < 0x100; item++)
                {
                    _saveFile.Write(_offset + WorldData + acre * 0x200 + item * 2, Acres[acre].Items[item].ItemId, true);
                }
            }

            // Save Cottage
            Cabana.Data.Rooms[0].Write();

            //Save Islander
            Islander.Write(TownName);

            // Save Buried Data
            _saveFile.Write(_offset + BuriedData, BuriedDataArray);
        }
    }
}
