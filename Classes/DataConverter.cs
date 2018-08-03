using System;
using System.Collections.Generic;
using System.Collections;

namespace ACSE
{
    public static class DataConverter
    {
        private static byte ConvertByte(BitArray b)
        {
            byte value = 0;
            for (byte i = 0; i < b.Count; i++)
                if (b[i])
                    value |= (byte)(1 << i);
            return value;
        }

        public static Dictionary<Type, Delegate> TypeMap = new Dictionary<Type, Delegate>
        {
            {typeof(byte[]), new Func<byte[], byte>
                (b => ConvertByte(new BitArray(b)))
            },
            {typeof(BitArray), new Func<BitArray, byte>
                (ConvertByte)
            }
        };

        public static byte[] ToBits(byte[] byteBuffer, bool reverse = false)
        {
            var bits = new byte[8 * byteBuffer.Length];
            for (var i = 0; i < byteBuffer.Length; i++)
            {
                var bitArray = new BitArray(new[] { byteBuffer[i] });
                for (var x = 0; x < bitArray.Length; x++)
                    bits[i * 8 + (reverse ? 7 - x : x)] = Convert.ToByte(bitArray[x]);
            }
            return bits;
        }

        public static byte ToBit(byte bitByte, int bitIndex, bool reverse = false)
        {
            return (byte)((reverse ? bitByte >> (7 - bitIndex) : bitByte >> bitIndex) & 1);
        }

        public static void SetBit(ref byte bitByte, int bitIndex, bool set, bool reverse = false)
        {
            var mask = 1 << (reverse ? 7 - bitIndex : bitIndex);
            if (set)
                bitByte |= (byte)mask;
            else
                bitByte &= (byte)~mask;
        }

        public static byte ToByte(object variant)
        {
            return (byte)TypeMap[variant.GetType()].DynamicInvoke(variant);
        }

        public static byte GetByte(byte[] bits)
        {
            byte value = 0;
            for (var i = 0; i < 8; i++)
                value |= (byte)((bits[i] & 1) << i);
            return value;
        }

        public static byte[] GetBits(byte value)
        {
            var bits = new byte[8];
            for (var i = 0; i < 8; i++)
                bits[i] = (byte)((value >> i) & 1);
            return bits;
        }
    }
}
