using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using ACSE.Core.Players;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.Core.Items
{
    public class Inventory
    {
        public enum AcItemFlag
        {
            None = 0,
            Present = 1,
            Quest = 2
        };

        public Item[] Items;

        public Inventory(IReadOnlyList<ushort> inventoryData, Player player)
        {
            Items = new Item[inventoryData.Count];
            for(var i = 0; i < inventoryData.Count; i++)
            {
                var item = new Item(inventoryData[i]);

                if (Save.SaveInstance.SaveGeneration == SaveGeneration.GCN)
                {
                    item.ItemFlag = GetItemFlag(player, i);
                }

                Items[i] = item;
            }
        }

        public Inventory(IReadOnlyList<uint> inventoryData)
        {
            Items = new Item[inventoryData.Count];
            for (var i = 0; i < inventoryData.Count; i++)
            {
                var item = new Item(inventoryData[i]);
                Items[i] = item;
            }
        }

        public static Image GetItemPic(int itemsize, int itemsPerRow, Item[] items, SaveType saveType, int width = -1, int height = -1)
        {
            width = width == -1 ? itemsize * itemsPerRow : width;
            height = height == -1 ? itemsize * items.Length / itemsPerRow : height;
            height = height < 1 ? width : height;
            var bmpData = new byte[4 * ((width) * (height))];
            for (var i = 0; i < items.Length; i++)
            {
                var x = i % itemsPerRow;
                var y = i / itemsPerRow;
                var item = items[i] ?? new Item(0);
                var itemColor = ItemData.GetItemColor(ItemData.GetItemType(item.ItemId, saveType));

                for (var x2 = 0; x2 < itemsize * itemsize; x2++)
                {
                    var dataPosition = (y * itemsize + x2 % itemsize) * width * 4 + (x * itemsize + x2 / itemsize) * 4;
                    if (dataPosition >= bmpData.Length)
                    {
                        DebugUtility.DebugManagerInstance.WriteLine(
                            $"Item Bitmap generation received more items than allocated space. Skipping {items.Length - i} item(s).");
                        break;
                    }
                    Buffer.BlockCopy(BitConverter.GetBytes(itemColor), 0, bmpData, dataPosition, 4);
                }
            }

            // Don't draw a border if there's only one item
            if (items.Length > 1)
            {
                var borderColorData = BitConverter.GetBytes(0x41000000);
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || x % itemsize == 0 || y % itemsize == 0)
                        {
                            Buffer.BlockCopy(borderColorData, 0, bmpData,
                                (y * width * 4 + x * 4), 4);
                        }
                    }
                }
            }

            var b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bData = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bmpData, 0, bData.Scan0, bmpData.Length);
            b.UnlockBits(bData);
            return b;
        }

        public static Image GetItemPic(int itemsize, int itemsPerRow, Item[] items, SaveType saveType, Size pictureBoxSize)
        {
            return GetItemPic(itemsize, itemsPerRow, items, saveType, pictureBoxSize.Width, pictureBoxSize.Height);
        }

        public static Image GetItemPic(int itemsize, Item item, SaveType saveType)
        {
            const int width = 16;
            const int height = 16;
            var bmpData = new byte[1024];
            var itemColor = item == null
                ? new byte[] {0xFF, 0xFF, 0x00, 0x00}
                : BitConverter.GetBytes(ItemData.GetItemColor(ItemData.GetItemType(item.ItemId, saveType)));

            for (var i = 0; i < 1024; i+=4)
                itemColor.CopyTo(bmpData, i);

            var b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bData = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bmpData, 0, bData.Scan0, bmpData.Length);
            b.UnlockBits(bData);
            return b;
        }

        public ushort[] GetItemIDs()
        {
            var ids = new ushort[15];
            for (var i = 0; i < 15; i++)
                ids[i] = Items[i].ItemId;
            return ids;
        }

        public static AcItemFlag GetItemFlag(Player player, int inventoryIdx)
        {
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.AnimalCrossing:
                    return (AcItemFlag)(Save.SaveInstance.ReadUInt32(player.Offset + 0x88, Save.SaveInstance.IsBigEndian) >> (inventoryIdx << 1) & 3);
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    return (AcItemFlag)(Save.SaveInstance.ReadUInt32(player.Offset + 0x84, Save.SaveInstance.IsBigEndian) >> (inventoryIdx << 1) & 3);
                default:
                    return AcItemFlag.None;
            }
        }

        public static void SetItemFlag(Player player, AcItemFlag flag, int inventoryIdx)
        {
            var flagIdx = inventoryIdx << 1;
            var flagValue = (uint)flag & 3;
            switch (Save.SaveInstance.SaveType)
            {
                case SaveType.AnimalCrossing:
                    Save.SaveInstance.Write(player.Offset + 0x88, (Save.SaveInstance.ReadUInt32(player.Offset + 0x88, Save.SaveInstance.IsBigEndian) & ~(3 << flagIdx)) | (flagValue << flagIdx));
                    break;
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    Save.SaveInstance.Write(player.Offset + 0x84, (Save.SaveInstance.ReadUInt32(player.Offset + 0x84, Save.SaveInstance.IsBigEndian) & ~(3 << flagIdx)) | (flagValue << flagIdx));
                    break;
            }
        }
    }
}
