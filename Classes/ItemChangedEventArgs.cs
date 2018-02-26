namespace ACSE
{
    public class ItemChangedEventArgs
    {
        public Item PreviousItem;
        public Item NewItem;
    }

    public class IndexedItemChangedEventArgs : ItemChangedEventArgs
    {
        public int Index;
    }
}
