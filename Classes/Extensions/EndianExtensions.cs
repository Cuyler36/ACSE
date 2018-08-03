namespace System
{
    public static class UInt16Extension
    {
        public static ushort Reverse(this ushort value)
        {
            return (ushort)((value << 8) | (value >> 8));
        }
    }

    public static class Int16Extension
    {
        public static short Reverse(this short value)
        {
            return (short)((value << 8) | (value >> 8));
        }
    }

    public static class UInt32Extension
    {
        public static uint Reverse(this uint value)
        {
            return ((value << 24) | ((value >> 24) & 0xFF) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00));
        }
    }

    public static class Int32Extension
    {
        public static int Reverse(this int value)
        {
            return ((value << 24) | ((value >> 24) & 0xFF) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00));
        }
    }

    public static class UInt64Extension
    {
        public static ulong Reverse(this ulong value)
        {
            return (((value & 0xFF00000000000000) >> 56) | ((value & 0xFF000000000000) >> 40) | ((value & 0xFF0000000000) >> 24) | ((value & 0xFF00000000) >> 8)
                | ((value & 0xFF000000) << 8) | ((value & 0xFF0000) << 24) | ((value & 0xFF00) << 40) | ((value & 0xFF) << 56));
        }
    }

    public static class Int64Extension
    {
        public static long Reverse(this long value)
        {
            return (((value >> 56) & 0xFF) | ((value & 0xFF000000000000) >> 40) | ((value & 0xFF0000000000) >> 24) | ((value & 0xFF00000000) >> 8)
                | ((value & 0xFF000000) << 8) | ((value & 0xFF0000) << 24) | ((value & 0xFF00) << 40) | ((value & 0xFF) << 56));
        }
    }
}
