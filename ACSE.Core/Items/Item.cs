using System;
using ACSE.Core.Saves;

namespace ACSE.Core.Items
{
    public class Item : IEquatable<Item>, IEquatable<ushort>
    {
        /// <summary>
        /// The currently selected <see cref="Item"/>.
        /// </summary>
        public static Item SelectedItem;

        public readonly ItemType Type;

        public Inventory.AcItemFlag ItemFlag;
        public readonly ushort ItemId;
        public byte Flag1;
        public byte Flag2;
        public readonly string Name;

        public Item()
        {
            var saveType = Save.SaveInstance == null ? SaveType.AnimalCrossing : Save.SaveInstance.SaveType;
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
            Type = ItemData.GetItemType(ItemId, Save.SaveInstance?.SaveType ?? SaveType.AnimalCrossing);
        }

        public Item(ushort itemId)
        {
            ItemId = itemId;
            Name = ItemData.GetItemName(ItemId);
            Type = ItemData.GetItemType(ItemId, Save.SaveInstance?.SaveType ?? SaveType.AnimalCrossing);
        }

        public Item(uint itemId)
        {
            ItemId = (ushort)itemId;
            Flag1 = (byte)(itemId >> 24);
            Flag2 = (byte)(itemId >> 16);
            Name = ItemData.GetItemName(ItemId);
            Type = ItemData.GetItemType(ItemId, Save.SaveInstance?.SaveType ?? SaveType.AnimalCrossing);
        }

        public Item(Item cloningItem)
        {
            ItemId = cloningItem.ItemId;
            Flag1 = cloningItem.Flag1;
            Flag2 = cloningItem.Flag2;
            Name = cloningItem.Name;
            Type = cloningItem.Type;
        }

        public Item(ushort itemId, byte flag1, byte flag2)
        {
            ItemId = itemId;
            Flag1 = flag1;
            Flag2 = flag2;
            Name = ItemData.GetItemName(ItemId);
            Type = ItemData.GetItemType(ItemId, Save.SaveInstance?.SaveType ?? SaveType.AnimalCrossing);
        }

        public override string ToString() => Name;

        public uint ToUInt32()
        {
            return (uint)((Flag1 << 24) + (Flag2 << 16) + ItemId);
        }

        public bool Equals(Item other)
        {
            return ReferenceEquals(other, this);
        }

        public bool Equals(ushort other) => ItemId == other;

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Item _:
                    return Equals((Item) obj);
                case ushort _:
                    return Equals((ushort) obj);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ ItemId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Item obj1, Item obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(Item obj1, Item obj2)
        {
            return !Equals(obj1, obj2);
        }
    }

    public class Furniture : Item, IEquatable<Furniture>
    {
        public readonly ushort BaseItemId;
        public readonly int Rotation;

        public Furniture(ushort itemId) : base(itemId)
        {
            if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
            {
                BaseItemId = itemId;
                Rotation = 0;
            }
            else
            {
                BaseItemId = (ushort)(ItemId & 0xFFFC);
                if (ItemData.GetItemType(itemId, Save.SaveInstance.SaveType) == ItemType.Furniture ||
                    ItemData.GetItemType(itemId, Save.SaveInstance.SaveType) == ItemType.Gyroid)
                {
                    Rotation = ItemId & 3;
                }
            }
        }

        public Furniture(uint item) : base(item)
        {
            BaseItemId = ItemId;
            Rotation = (Flag1 >> 4) / 4;
        }

        public Furniture(ushort item, byte flag1, byte flag2) : base(item, flag1, flag2)
        {
            if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
            {
                BaseItemId = ItemId;
                Rotation = (Flag1 >> 4) / 4;
            }
            else
            {
                BaseItemId = (ushort)(ItemId & 0xFFFC);
                if (ItemData.GetItemType(ItemId, Save.SaveInstance.SaveType) == ItemType.Furniture ||
                    ItemData.GetItemType(ItemId, Save.SaveInstance.SaveType) == ItemType.Gyroid)
                {
                    Rotation = ItemId & 3;
                }
            }
        }

        public Furniture(Item item) : base (item)
        {
            if (Save.SaveInstance.SaveGeneration == SaveGeneration.N3DS)
            {
                BaseItemId = ItemId;
                Rotation = (Flag1 >> 4) / 4;
            }
            else
            {
                BaseItemId = (ushort)(ItemId & 0xFFFC);
                if (ItemData.GetItemType(ItemId, Save.SaveInstance.SaveType) == ItemType.Furniture ||
                    ItemData.GetItemType(ItemId, Save.SaveInstance.SaveType) == ItemType.Gyroid)
                {
                    Rotation = ItemId & 3;
                }
            }
        }

        public bool Equals(Furniture other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Rotation == other.Rotation && base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Furniture _:
                    return Equals((Furniture) obj);
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
}
