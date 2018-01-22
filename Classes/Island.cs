using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    /// <summary>
    /// Island class for Doubutsu no Mori e+
    /// </summary>
    class Island
    {
        #region Island Offsets

        int PlayerName = 0x00;
        int TownName = 0x06;
        int PlayerId = 0x0C;
        int TownId = 0x0E;
        int WorldData = 0x10;
        int CottageData = 0x418;
        int IslanderData = 0xF20;
        int BuriedData = -1; // Research

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
                        Layer.Items[f] = new Furniture(SaveData.ReadUInt16(FurnitureOffset, SaveData.Is_Big_Endian));
                    }

                    MainRoom.Layers[x] = Layer;
                }
            }
        }

        public NewPlayer Owner;
        public WorldItem[][] Items;
        public Cottage Cabana;
        public NewVillager Islander;
        Pattern FlagPattern;

        public Island(int Offset, NewPlayer[] Players, Save SaveFile)
        {
            ushort Identifier = SaveFile.ReadUInt16(Offset + PlayerId, true);
            foreach (NewPlayer Player in Players)
            {
                if (Player != null && Player.Data.Identifier == Identifier)
                {
                    Owner = Player;
                }
            }

            Items = new WorldItem[2][];
            for (int Acre = 0; Acre < 2; Acre++)
            {
                Items[Acre] = new WorldItem[0x100];
                int i = 0;
                foreach (ushort ItemId in SaveFile.ReadUInt16Array(Offset + WorldData + Acre * 0x200, 0x100, true))
                {
                    Items[Acre][i] = new WorldItem(ItemId, i % 256);
                    i++;
                }
            }

            Cabana = new Cottage(Offset + CottageData, SaveFile);

            Islander = new NewVillager(Offset + IslanderData, 0, SaveFile);
        }
    }
}
