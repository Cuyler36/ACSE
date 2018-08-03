namespace ACSE
{
    /// <summary>
    /// Island class for Doubutsu no Mori e+
    /// </summary>
    class Island
    {
        #region Island Offsets

        private readonly int IslandName = 0x00;
        private readonly int TownNameOffset = 0x06;
        private readonly int IslandId = 0x0C;
        private readonly int TownIdOffset = 0x0E;
        private readonly int WorldData = 0x10;
        private readonly int CottageData = 0x418;
        private readonly int FlagData = 0xD00;
        private readonly int IslanderData = 0xF20;
        private readonly int BuriedData = 0x15A0;
        private readonly int IslandLeftAcreData = 0x15E0;
        private readonly int IslandRightAcreData = 0x15E1;
        private readonly int IslandInfoFlag = 0x15FB;

        #endregion

        public class Cottage
        {

            public Room MainRoom;

            public Cottage(int Offset, Save SaveData)
            {
                MainRoom = new Room
                {
                    Offset = Offset,
                    Name = "Cabana",
                    Layers = new Layer[4],

                    Carpet = new Item((ushort)(0x2600 | SaveData.ReadByte(Offset + 0x8A0))),
                    Wallpaper = new Item((ushort)(0x2700 | SaveData.ReadByte(Offset + 0x8A1)))
                };

                for (int x = 0; x < 4; x++)
                {
                    int LayerOffset = Offset + 0x228 * x;
                    var Layer = new Layer
                    {
                        Offset = LayerOffset,
                        Index = x,
                        Items = new Furniture[256],
                        Parent = MainRoom
                    };

                    // Load furniture for the layer
                    for (int f = 0; f < 256; f++)
                    {
                        int FurnitureOffset = LayerOffset + f * 2;
                        Layer.Items[f] = new Furniture(SaveData.ReadUInt16(FurnitureOffset, SaveData.IsBigEndian));
                    }

                    MainRoom.Layers[x] = Layer;
                }
            }

            public void Write()
            {
                MainRoom.Write();
            }
        }

        private Save SaveFile;
        private readonly int Offset;
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

        public Island(int Offset, Player[] Players, Save SaveFile)
        {
            this.SaveFile = SaveFile;
            this.Offset = Offset;

            Name = new ACSE.Classes.Utilities.ACString(SaveFile.ReadByteArray(Offset + IslandName, 6), SaveFile.SaveType).Trim();
            Id = SaveFile.ReadUInt16(Offset + IslandId, true);

            TownName = new ACSE.Classes.Utilities.ACString(SaveFile.ReadByteArray(Offset + TownNameOffset, 6), SaveFile.SaveType).Trim();
            TownId = SaveFile.ReadUInt16(Offset + TownIdOffset, true);

            ushort Identifier = SaveFile.ReadUInt16(Offset - 0x2214, true);
            foreach (Player Player in Players)
            {
                if (Player != null && Player.Data.Identifier == Identifier)
                {
                    Owner = Player;
                }
            }

            BuriedDataArray = SaveFile.ReadByteArray(Offset + BuriedData, 0x40, false);

            Items = new WorldItem[2][];
            for (int Acre = 0; Acre < 2; Acre++)
            {
                Items[Acre] = new WorldItem[0x100];
                int i = 0;
                foreach (ushort ItemId in SaveFile.ReadUInt16Array(Offset + WorldData + Acre * 0x200, 0x100, true))
                {
                    Items[Acre][i] = new WorldItem(ItemId, i % 256);
                    SetBuried(Items[Acre][i], Acre, BuriedDataArray, SaveFile.SaveType);
                    i++;
                }
            }

            Cabana = new Cottage(Offset + CottageData, SaveFile);
            FlagPattern = new Pattern(Offset + FlagData, 0, SaveFile);
            Islander = new Villager(Offset + IslanderData, 0, SaveFile);

            IslandLeftAcreIndex = SaveFile.ReadByte(Offset + IslandLeftAcreData);
            IslandRightAcreIndex = SaveFile.ReadByte(Offset + IslandRightAcreData);
        }

        private ushort IslandAcreIndexToIslandAcreId(byte Side, byte Index)
        {
            // Left side
            if (Side == 0)
            {
                switch (Index)
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
            else
            {
                switch (Index)
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
        }

        private int GetBuriedDataLocation(WorldItem item, int acre, SaveType saveType)
        {
            if (item != null)
            {
                int worldPosition = 0;
                if (saveType == SaveType.AnimalCrossing || saveType == SaveType.DoubutsuNoMoriEPlus || saveType == SaveType.CityFolk)
                    worldPosition = (acre * 256) + (15 - item.Location.X) + item.Location.Y * 16; //15 - item.Location.X because it's stored as a ushort in memory w/ reversed endianess
                else if (saveType == SaveType.WildWorld)
                    worldPosition = (acre * 256) + item.Index;
                return worldPosition / 8;
            }
            return -1;
        }

        private void SetBuried(WorldItem item, int acre, byte[] burriedItemData, SaveType saveType)
        {
            int burriedDataOffset = GetBuriedDataLocation(item, acre, saveType);
            if (burriedDataOffset > -1 && burriedDataOffset < burriedItemData.Length)
                item.Buried = DataConverter.ToBit(burriedItemData[burriedDataOffset], item.Location.X % 8) == 1;
        }

        // TODO: Make a toggle to enable/disable the island.
        private bool IsPurchased() => (SaveFile.ReadByte(Offset + IslandInfoFlag) & 0x80) == 0x80;

        public void SetBuriedInMemory(WorldItem item, int acre, byte[] burriedItemData, bool buried, SaveType saveType)
        {
            if (saveType != SaveType.NewLeaf && saveType != SaveType.WelcomeAmiibo)
            {
                int buriedLocation = GetBuriedDataLocation(item, acre, saveType);
                if (buriedLocation > -1)
                {
                    DataConverter.SetBit(ref burriedItemData[buriedLocation], item.Location.X % 8, buried);
                    item.Buried = DataConverter.ToBit(burriedItemData[buriedLocation], item.Location.X % 8) == 1;
                }
                else
                    item.Buried = false;
            }
        }

        public ushort[] GetAcreIds()
        {
            return new ushort[2] { IslandAcreIndexToIslandAcreId(0, IslandLeftAcreIndex), IslandAcreIndexToIslandAcreId(1, IslandRightAcreIndex) };
        }

        public void Write()
        {
            if (Owner != null)
            {
                SaveFile.Write(Offset + 0x00, ACSE.Classes.Utilities.ACString.GetBytes(Name, 6));
                SaveFile.Write(Offset + 0x06, ACSE.Classes.Utilities.ACString.GetBytes(TownName, 6));
                SaveFile.Write(Offset + 0x0C, Id, true);
                SaveFile.Write(Offset + 0x0E, TownId, true);
            }

            // Save World Items
            for (int Acre = 0; Acre < 2; Acre++)
            {
                for (int Item = 0; Item < 0x100; Item++)
                {
                    SaveFile.Write(Offset + WorldData + Acre * 0x200 + Item * 2, Items[Acre][Item].ItemId, true);
                }
            }

            // Save Cottage
            Cabana.Write();

            // TODO: Save Islander

            // Save Buried Data
            SaveFile.Write(Offset + BuriedData, BuriedDataArray);
        }
    }
}
