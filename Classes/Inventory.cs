using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ACSE
{
    public class Inventory
    {
        public enum ACItemFlag
        {
            None = 0,
            Present = 1,
            Quest = 2
        };

        public Item[] Items;

        public Inventory(ushort[] inventoryData)
        {
            Items = new Item[inventoryData.Length];
            for(int i = 0; i < inventoryData.Length; i++)
            {
                Item item = new Item(inventoryData[i]);
                Items[i] = item;
            }
        }

        public Inventory(uint[] inventoryData)
        {
            Items = new Item[inventoryData.Length];
            for (int i = 0; i < inventoryData.Length; i++)
            {
                Item item = new Item(inventoryData[i]);
                Items[i] = item;
            }
        }

        public static Image GetItemPic(int itemsize, int itemsPerRow, Item[] items, SaveType Save_Type, int width = -1, int height = -1)
        {
            width = width == -1 ? itemsize * itemsPerRow : width;
            height = height == -1 ? itemsize * items.Length / itemsPerRow : height;
            height = height < 1 ? width : height;
            byte[] bmpData = new byte[4 * ((width) * (height))];
            for (int i = 0; i < items.Length; i++)
            {
                int X = i % itemsPerRow;
                int Y = i / itemsPerRow;
                Item item = items[i] ?? new Item(0);
                uint itemColor = ItemData.GetItemColor(ItemData.GetItemType(item.ItemID, Save_Type));

                for (int x = 0; x < itemsize * itemsize; x++)
                {
                    int dataPosition = (Y * itemsize + x % itemsize) * width * 4 + (X * itemsize + x / itemsize) * 4;
                    if (dataPosition >= bmpData.Length)
                    {
                        System.Windows.Forms.MessageBox.Show("Item Bitmap generation received more items than allocated space. Skipping " + (items.Length - i).ToString() + " Item(s).");
                        break;
                    }
                    Buffer.BlockCopy(BitConverter.GetBytes(itemColor), 0, bmpData, dataPosition, 4);
                }
            }

            // Don't draw a border if there's only one item
            if (items.Length > 1)
            {
                byte[] borderColorData = BitConverter.GetBytes(0x41000000);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x == 0 || y == 0 || x == width - 1 || y == height - 1 || x % itemsize == 0 || y % itemsize == 0)
                        {
                            Buffer.BlockCopy(borderColorData, 0, bmpData,
                                (y * width * 4 + x * 4), 4);
                        }
                    }
                }
            }

            Bitmap b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bData = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bmpData, 0, bData.Scan0, bmpData.Length);
            b.UnlockBits(bData);
            return b;
        }

        public static Image GetItemPic(int itemsize, int itemsPerRow, Item[] items, SaveType Save_Type, Size pictureBoxSize)
        {
            return GetItemPic(itemsize, itemsPerRow, items, Save_Type, pictureBoxSize.Width, pictureBoxSize.Height);
        }

        public static Image GetItemPic(int itemsize, Item item, SaveType Save_Type)
        {
            int width = 16;
            int height = 16;
            byte[] bmpData = new byte[1024];
            byte[] itemColor = item == null ? new byte[4] { 0xFF, 0xFF, 0x00, 0x00 } : BitConverter.GetBytes(ItemData.GetItemColor(ItemData.GetItemType(item.ItemID, Save_Type)));

            for (int i = 0; i < 1024; i+=4)
                itemColor.CopyTo(bmpData, i);

            Bitmap b = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bData = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(bmpData, 0, bData.Scan0, bmpData.Length);
            b.UnlockBits(bData);
            return b;
        }

        public ushort[] GetItemIDs()
        {
            ushort[] ids = new ushort[15];
            for (int i = 0; i < 15; i++)
                ids[i] = Items[i].ItemID;
            return ids;
        }

        public static ACItemFlag GetItemFlag(Save SaveFile, Player Player, int InventoryIdx)
        {
            switch (SaveFile.Save_Type)
            {
                case SaveType.Animal_Crossing:
                    return (ACItemFlag)(SaveFile.ReadUInt32(Player.Offset + 0x88, SaveFile.Is_Big_Endian) >> (InventoryIdx << 1) & 3);
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    return (ACItemFlag)(SaveFile.ReadUInt32(Player.Offset + 0x84, SaveFile.Is_Big_Endian) >> (InventoryIdx << 1) & 3);
                default:
                    return ACItemFlag.None;
            }
        }

        public static void SetItemFlag(Save SaveFile, Player Player, ACItemFlag Flag, int InventoryIdx)
        {
            int FlagIdx = InventoryIdx << 1;
            uint FlagValue = (uint)Flag & 3;
            switch (SaveFile.Save_Type)
            {
                case SaveType.Animal_Crossing:
                    SaveFile.Write(Player.Offset + 0x88, (SaveFile.ReadUInt32(Player.Offset + 0x88, SaveFile.Is_Big_Endian) & ~(3 << FlagIdx)) | (FlagValue << FlagIdx));
                    break;
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    SaveFile.Write(Player.Offset + 0x84, (SaveFile.ReadUInt32(Player.Offset + 0x84, SaveFile.Is_Big_Endian) & ~(3 << FlagIdx)) | (FlagValue << FlagIdx));
                    break;
                default:
                    break;
            }
        }
    }

    public class InventorySlot
    {
        public Item Item;
        public int SlotID;

        public InventorySlot(Item item, int slotId)
        {
            Item = item;
            SlotID = slotId;
        }
    }
}
