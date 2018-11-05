using ACSE.Core.Items;
using ACSE.Core.Saves;

namespace ACSE.Core.Town.Shops
{
    public sealed class FurnitureShop : Shop
    {
        public uint VisitorBellsSum;
        public byte Size;

        public FurnitureShop(Save saveFile, int offset) : base(saveFile, offset)
        {
            Size = GetSize(saveFile.SaveGeneration);
            Name = ShopInfo.GetShopName(saveFile.SaveGeneration, ShopType.FurnitureShop, Size);
            BellsSumOffset = ShopOffsets.FurnitureShopBells;

            var itemCount = 0;

            switch (saveFile.SaveGeneration)
            {
                case SaveGeneration.GCN when saveFile.SaveType == SaveType.DoubutsuNoMoriEPlus ||
                                             saveFile.SaveType == SaveType.AnimalForestEPlus:
                    switch (Size)
                    {
                        case 0:
                            itemCount = 10;
                            break;

                        case 1:
                            itemCount = 15;
                            break;

                        case 2:
                            itemCount = 21;
                            break;

                        case 3:
                            itemCount = 32;
                            break;
                    }

                    break;

                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    if (Size == 0)
                        itemCount = 11;
                    else if (Size == 1)
                        itemCount = 16;
                    else if (Size == 2)
                        itemCount = 22;
                    else
                        itemCount = 33;
                    break;
                case SaveGeneration.N3DS:
                    break;
            }

            var items = new Item[itemCount];
            for (var i = 0; i < itemCount; i++)
            {
                if (SaveFile.SaveGeneration == SaveGeneration.N3DS)
                {
                    items[i] = new Item(SaveFile.ReadUInt32(Offset + ShopOffsets.FurnitureShop + i * 4));
                }
                else
                {
                    items[i] = new Item(SaveFile.ReadUInt16(Offset + ShopOffsets.FurnitureShop + i * 2, SaveFile.IsBigEndian));
                }
            }

            Stock = items;
        }

        public byte GetSize(SaveGeneration generation)
        {
            var saveFile = Save.SaveInstance;
            var shopOffsets = ShopInfo.GetShopOffsets(saveFile.SaveType);
            if (shopOffsets == null) return 0;
            switch (generation)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    return (byte)((saveFile.ReadByte(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade) >> 6) & 3);
                case SaveGeneration.N3DS:
                    return saveFile.ReadByte(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade);
                default:
                    return 0;
            }
        }

        public void SetSize(byte size)
        {
            var saveFile = Save.SaveInstance;
            var shopOffsets = ShopInfo.GetShopOffsets(saveFile.SaveType);
            if (shopOffsets == null) return;
            switch (saveFile.SaveGeneration)
            {
                case SaveGeneration.N64:
                case SaveGeneration.GCN:
                    saveFile.Write(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade,
                        (byte)((saveFile.ReadByte(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade) & 0x3F) | ((size & 3) << 6)));
                    break;
                case SaveGeneration.N3DS:
                    saveFile.Write(saveFile.SaveDataStartOffset + shopOffsets.FurnitureShopUpgrade, size);
                    break;
            }
        }

        public override void Write()
        {
            throw new System.NotImplementedException();
        }
    }
}
