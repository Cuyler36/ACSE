using System.Drawing;

namespace ACSE
{
    public class Item
    {
        public Inventory.ACItemFlag ItemFlag;
        public ushort ItemId;
        public byte Flag1;
        public byte Flag2;
        public string Name;

        public Item()
        {
            var saveType = MainForm.Save_File == null ? SaveType.AnimalCrossing : MainForm.Save_File.SaveType;
            switch (saveType)
            {
                case SaveType.WildWorld:
                case SaveType.CityFolk:
                    ItemId = 0xFFF1;
                    break;
                case SaveType.NewLeaf:
                case SaveType.WelcomeAmiibo:
                    ItemId = 0x7FFE;
                    break;
            }
            Name = ItemData.GetItemName(ItemId);
        }

        public Item(ushort itemId)
        {
            ItemId = itemId;
            Name = ItemData.GetItemName(ItemId);
        }

        public Item(uint itemId)
        {
            ItemId = (ushort)itemId;
            Flag1 = (byte)(itemId >> 24);
            Flag2 = (byte)(itemId >> 16);
            Name = ItemData.GetItemName(ItemId);
        }

        public Item(Item cloningItem)
        {
            ItemId = cloningItem.ItemId;
            Flag1 = cloningItem.Flag1;
            Flag2 = cloningItem.Flag2;
            Name = cloningItem.Name;
        }

        public Item(ushort itemId, byte flag1, byte flag2)
        {
            ItemId = itemId;
            Flag1 = flag1;
            Flag2 = flag2;
            Name = ItemData.GetItemName(ItemId);
        }

        public uint ToUInt32()
        {
            return (uint)((Flag1 << 24) + (Flag2 << 16) + ItemId);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Item _:
                    var comparingItem = obj as Item;
                    return comparingItem != null && (comparingItem.ItemId == ItemId && comparingItem.Flag1 == Flag1 && comparingItem.Flag2 == Flag2);
                case ushort _:
                    return (ushort)obj == ItemId;
            }

            return false;
        }

        public static bool operator ==(Item obj1, Item obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(Item obj1, Item obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class WorldItem : Item
    {
        public Point Location;
        public int Index;
        public bool Buried;
        public bool Watered;

        public WorldItem(ushort itemId, int position) : base(itemId)
        {
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(ushort itemId, byte flag1, byte flag2, int position) : base(itemId)
        {
            Flag1 = flag1;
            Flag2 = flag2;
            Buried = Flag1 == 0x80;
            Watered = Flag1 == 0x40;
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(int position)
        {
            Location = new Point(position % 16, position / 16);
            Index = position;
        }

        public WorldItem(WorldItem cloningItem)
        {
            ItemId = cloningItem.ItemId;
            Flag1 = cloningItem.Flag1;
            Flag2 = cloningItem.Flag2;
            Name = cloningItem.Name;
            Index = cloningItem.Index;
            Location = new Point(Index % 16, Index / 16);
        }

        public WorldItem(uint itemId, int position) : base(itemId)
        {
            Index = position;
            Location = new Point(position % 16, position / 16);
            ItemId = (ushort)itemId;
            Flag1 = (byte)(itemId >> 24);
            Flag2 = (byte)(itemId >> 16);
            Name = ItemData.GetItemName(ItemId);
            Buried = Flag1 == 0x80;
            Watered = Flag1 == 0x40;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case WorldItem _:
                    var comparingItem = obj as WorldItem;
                    return comparingItem != null && (comparingItem.ItemId == ItemId && comparingItem.Flag1 == Flag1 && comparingItem.Flag2 == Flag2);
                case ushort _:
                    return (ushort)obj == ItemId;
            }

            return false;
        }

        public static bool operator ==(WorldItem obj1, WorldItem obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(WorldItem obj1, WorldItem obj2)
        {
            return !Equals(obj1, obj2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Furniture : Item
    {
        public ushort BaseItemId;
        public int Rotation;

        public Furniture(ushort itemId) : base(itemId)
        {
            if (MainForm.Save_File.SaveGeneration == SaveGeneration.N3DS)
            {
                BaseItemId = itemId;
                Rotation = 0;
            }
            else
            {
                BaseItemId = (ushort)(ItemId & 0xFFFC);
                if (ItemData.GetItemType(itemId, MainForm.Save_File.SaveType) == "Furniture" || ItemData.GetItemType(itemId, MainForm.Save_File.SaveType).Equals("Gyroids"))
                {
                    Rotation = (ItemId % 4) * 90;
                }
            }
        }

        public Furniture(uint item) : base(item)
        {
            BaseItemId = ItemId;
            Rotation = ((Flag1 >> 4) / 4) * 90;
        }

        public Furniture(ushort item, byte flag1, byte flag2) : base(item, flag1, flag2)
        {
            if (MainForm.Save_File.SaveGeneration == SaveGeneration.N3DS)
            {
                BaseItemId = ItemId;
                Rotation = ((Flag1 >> 4) / 4) * 90;
            }
            else
            {
                BaseItemId = (ushort)(ItemId & 0xFFFC);
                if (ItemData.GetItemType(ItemId, MainForm.Save_File.SaveType) == "Furniture" ||
                    ItemData.GetItemType(ItemId, MainForm.Save_File.SaveType).Equals("Gyroids"))
                {
                    Rotation = (ItemId % 4) * 90;
                }
            }
        }

        public Furniture(Item item)
        {
            ItemId = item.ItemId;
            Name = item.Name;
            Flag1 = item.Flag1;
            Flag2 = item.Flag2;

            if (MainForm.Save_File.SaveGeneration == SaveGeneration.N3DS)
            {
                BaseItemId = ItemId;
                Rotation = ((Flag1 >> 4) / 4) * 90;
            }
            else
            {
                BaseItemId = (ushort)(ItemId & 0xFFFC);
                if (ItemData.GetItemType(ItemId, MainForm.Save_File.SaveType) == "Furniture" ||
                    ItemData.GetItemType(ItemId, MainForm.Save_File.SaveType).Equals("Gyroids"))
                {
                    Rotation = (ItemId % 4) * 90;
                }
            }
        }

        public void SetRotation(int degrees)
        {
            if (degrees % 90 != 0) return;
            Rotation = degrees;
            ItemId = (ushort)(BaseItemId + (degrees / 90));
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Furniture _:
                    var comparingItem = obj as Furniture;
                    if (MainForm.Save_File.SaveGeneration == SaveGeneration.N3DS)
                        return comparingItem != null && (comparingItem.ItemId == ItemId && comparingItem.Flag1 == Flag1 && comparingItem.Flag2 == Flag2);
                    else
                        return comparingItem != null && comparingItem.BaseItemId == BaseItemId;
                case ushort _:
                    return (ushort)obj == ItemId;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public sealed class GyroidItem : Item
    {
        public uint Price;
        public byte SaleType;
        public bool Free;

        public GyroidItem(ushort itemId, uint price, byte sellType) : base(itemId)
        {
            Price = price;
            SaleType = sellType;
            Free = sellType == 0;
        }
    }

    public sealed class Building
    {
        public byte Id;
        public bool Exists;
        public byte AcreIndex;
        public byte XPos;
        public byte YPos;
        public byte AcreX;
        public byte AcreY;
        public string Name;

        public Building(byte id, byte x, byte y, SaveType saveType)
        {
            Id = id;
            //Despite what previous editors assume, I'm fairly certain that the X & Y location bytes are structured like this:
            //Upper Nibble = Acre
            //Lower Nibble = Position in Acre
            //I say this, as a town hall in New Leaf with location bytes of X = 0x28, Y = 0x19 is positioned on the third X acre and second Y acre at 0x8, 0x9.
            SaveDataManager.GetNibbles(x, out XPos, out AcreX);
            SaveDataManager.GetNibbles(y, out YPos, out AcreY);
            AcreIndex = (byte)((AcreY - 1) * 5 + (AcreX - 1)); // * 5 works here because both CF and NL use 5 X acres
            if (AcreIndex > 24) //Works for NL too, since the dock is located in the 5th Y acre row.
                AcreIndex = 0;

            switch (saveType)
            {
                case SaveType.CityFolk:
                    Name = ItemData.CfBuildingNames[id];
                    Exists = AcreX > 0 && AcreY > 0;
                    break;
                case SaveType.NewLeaf:
                    Name = ItemData.NlBuildingNames[id];
                    Exists = Id != 0xF8;
                    break;
                default:
                    Name = ItemData.WaBuildingNames[id];
                    Exists = Id != 0xFC;
                    break;
            }
        }
    }
}
