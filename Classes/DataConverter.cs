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

        public static Dictionary<Type, Delegate> typeMap = new Dictionary<Type, Delegate>
        {
            {typeof(byte[]), new Func<byte[], byte>
                (b => {
                    return ConvertByte(new BitArray(b));
                })
            },
            {typeof(BitArray), new Func<BitArray, byte>
                (b => {
                    return ConvertByte(b);
                })
            }
        };

        public static byte[] ToBits(byte[] Byte_Buffer, bool Reverse = false)
        {
            byte[] Bits = new byte[8 * Byte_Buffer.Length];
            for (int i = 0; i < Byte_Buffer.Length; i++)
            {
                BitArray Bit_Array = new BitArray(new byte[] { Byte_Buffer[i] });
                for (int x = 0; x < Bit_Array.Length; x++)
                    Bits[i * 8 + (Reverse ? 7 - x : x)] = Convert.ToByte(Bit_Array[x]);
            }
            return Bits;
        }

        public static byte ToBit(byte Bit_Byte, int Bit_Index, bool Reverse = false)
        {
            return (byte)((Reverse ? Bit_Byte >> (7 - Bit_Index) : Bit_Byte >> Bit_Index) & 1);
        }

        public static void SetBit(ref byte Bit_Byte, int Bit_Index, bool Set, bool Reverse = false)
        {
            int Mask = 1 << (Reverse ? 7 - Bit_Index : Bit_Index);
            if (Set)
                Bit_Byte = Bit_Byte |= (byte)Mask;
            else
                Bit_Byte = Bit_Byte &= (byte)~Mask;
        }

        public static byte ToByte(object Variant)
        {
            return (byte)typeMap[Variant.GetType()].DynamicInvoke(Variant);
        }

        public static byte GetByte(byte[] Bits)
        {
            byte Value = 0;
            for (int i = 0; i < 8; i++)
                Value |= (byte)((Bits[i] & 1) << i);
            return Value;
        }

        public static byte[] GetBits(byte Value)
        {
            byte[] Bits = new byte[8];
            for (int i = 0; i < 8; i++)
                Bits[i] = (byte)((Value >> i) & 1);
            return Bits;
        }
    }
}
