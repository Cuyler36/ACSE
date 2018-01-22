namespace System
{
    public static class UInt16Extension
    {
        public static UInt16 Reverse(this UInt16 Value)
        {
            return (ushort)((Value << 8) | (Value >> 8));
        }
    }

    public static class Int16Extension
    {
        public static Int16 Reverse(this Int16 Value)
        {
            return (short)((Value << 8) | (Value >> 8));
        }
    }

    public static class UInt32Extension
    {
        public static UInt32 Reverse(this UInt32 Value)
        {
            return ((Value << 24) | ((Value >> 24) & 0xFF) | ((Value & 0xFF00) << 8) | ((Value >> 8) & 0xFF00));
        }
    }

    public static class Int32Extension
    {
        public static Int32 Reverse(this Int32 Value)
        {
            return ((Value << 24) | ((Value >> 24) & 0xFF) | ((Value & 0xFF00) << 8) | ((Value >> 8) & 0xFF00));
        }
    }

    public static class UInt64Extension
    {
        public static UInt64 Reverse(this UInt64 Value)
        {
            return (((Value & 0xFF00000000000000) >> 56) | ((Value & 0xFF000000000000) >> 40) | ((Value & 0xFF0000000000) >> 24) | ((Value & 0xFF00000000) >> 8)
                | ((Value & 0xFF000000) << 8) | ((Value & 0xFF0000) << 24) | ((Value & 0xFF00) << 40) | ((Value & 0xFF) << 56));
        }
    }

    public static class Int64Extension
    {
        public static Int64 Reverse(this Int64 Value)
        {
            return (((Value >> 56) & 0xFF) | ((Value & 0xFF000000000000) >> 40) | ((Value & 0xFF0000000000) >> 24) | ((Value & 0xFF00000000) >> 8)
                | ((Value & 0xFF000000) << 8) | ((Value & 0xFF0000) << 24) | ((Value & 0xFF00) << 40) | ((Value & 0xFF) << 56));
        }
    }
}
