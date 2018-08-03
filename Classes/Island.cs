using System.Collections.Generic;

namespace ACSE
{
    /// <summary>
    /// Island class for Doubutsu no Mori e+
    /// </summary>
    class Island
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
        private const int IslandInfoFlag = 0x15FB;

        #endregion

        public class Cottage
        {

            public Room MainRoom;

            public Cottage(int offset, Save saveData)
            {
                MainRoom = new Room
                {
                    Offset = offset,
                    Name = "Cabana",
                    Layers = new Layer[4],

                    Carpet = new Item((ushort)(0x2600 | saveData.ReadByte(offset + 0x8A0))),
                    Wallpaper = new Item((ushort)(0x2700 | saveData.ReadByte(offset + 0x8A1)))
                };

                for (var x = 0; x < 4; x++)
                {
                    var layerOffset = offset + 0x228 * x;
                    var layer = new Layer
                    {
                        Offset = layerOffset,
                        Index = x,
                        Items = new Furniture[256],
                        Parent = MainRoom
                    };

                    // Load furniture for the layer
                    for (var f = 0; f < 256; f++)
                    {
                        var furnitureOffset = layerOffset + f * 2;
                        layer.Items[f] = new Furniture(saveData.ReadUInt16(furnitureOffset, saveData.IsBigEndian));
                    }

                    MainRoom.Layers[x] = layer;
                }
            }

            public void Write()
            {
                MainRoom.Write();
            }
        }

        private readonly Save _saveFile;
        private readonly int _offset;
        public string Name;
        public ushort Id;
        public string TownName;
        public ushort TownId;
        public Player Owner;
        public WorldItem[][] Items;
        public Cottage Cabana;
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

            Items = new WorldItem[2][];
            for (var acre = 0; acre < 2; acre++)
            {
                Items[acre] = new WorldItem[0x100];
                var i = 0;
                foreach (var itemId in saveFile.ReadUInt16Array(offset + WorldData + acre * 0x200, 0x100, true))
                {
                    Items[acre][i] = new WorldItem(itemId, i % 256);
                    SetBuried(Items[acre][i], acre, BuriedDataArray, saveFile.SaveType);
                    i++;
                }
            }

            Cabana = new Cottage(offset + CottageData, saveFile);
            FlagPattern = new Pattern(offset + FlagData, 0, saveFile);
            Islander = new Villager(offset + IslanderData, 0, saveFile);
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

        private static int GetBuriedDataLocation(WorldItem item, int acre, SaveType saveType)
        {
            if (item == null) return -1;
            var worldPosition = 0;
            switch (saveType)
            {
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.CityFolk:
                    worldPosition = (acre * 256) + (15 - item.Location.X) + item.Location.Y * 16; //15 - item.Location.X because it's stored as a ushort in memory w/ reversed endianess
                    break;
                case SaveType.WildWorld:
                    worldPosition = (acre * 256) + item.Index;
                    break;
            }
            return worldPosition / 8;
        }

        private static void SetBuried(WorldItem item, int acre, IReadOnlyList<byte> burriedItemData, SaveType saveType)
        {
            var burriedDataOffset = GetBuriedDataLocation(item, acre, saveType);
            if (burriedDataOffset > -1 && burriedDataOffset < burriedItemData.Count)
                item.Buried = DataConverter.ToBit(burriedItemData[burriedDataOffset], item.Location.X % 8) == 1;
        }

        // TODO: Make a toggle to enable/disable the island.
        private bool IsPurchased()
            => (_saveFile.ReadByte(_offset + IslandInfoFlag) & 0x80) == 0x80;

        public void SetBuriedInMemory(WorldItem item, int acre, byte[] burriedItemData, bool buried, SaveType saveType)
        {
            if (saveType == SaveType.NewLeaf || saveType == SaveType.WelcomeAmiibo) return;
            var buriedLocation = GetBuriedDataLocation(item, acre, saveType);
            if (buriedLocation > -1)
            {
                DataConverter.SetBit(ref burriedItemData[buriedLocation], item.Location.X % 8, buried);
                item.Buried = DataConverter.ToBit(burriedItemData[buriedLocation], item.Location.X % 8) == 1;
            }
            else
                item.Buried = false;
        }

        public ushort[] GetAcreIds()
        {
            return new[] { IslandAcreIndexToIslandAcreId(0, IslandLeftAcreIndex), IslandAcreIndexToIslandAcreId(1, IslandRightAcreIndex) };
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
                    _saveFile.Write(_offset + WorldData + acre * 0x200 + item * 2, Items[acre][item].ItemId, true);
                }
            }

            // Save Cottage
            Cabana.Write();

            // TODO: Save Islander

            // Save Buried Data
            _saveFile.Write(_offset + BuriedData, BuriedDataArray);
        }
    }
}
