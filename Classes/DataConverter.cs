using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

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

        public static void WriteByte(int offset, byte data)
        {
            //MainForm.SaveBuffer[offset] = data;
        }

        public static void WriteByteArray(int offset, byte[] data, bool reverse = true)
        {
            if (reverse)
                Array.Reverse(data);
            //data.CopyTo(MainForm.SaveBuffer, offset);
        }

        public static void Write(int offset, object data, bool reverse = true)
        {
            Type type = data.GetType();
            if (type.IsArray && typeof(byte[]) != type)
            {
                object[] arr = ((Array)data).Cast<object>().ToArray();
                int objSize = Marshal.SizeOf(arr[0]);
                Type t = type.GetElementType();
                for (int i = 0; i < arr.Length; i++)
                    WriteByteArray(offset + i * objSize,
                        (byte[])typeof(BitConverter).GetMethod("GetBytes", new Type[] { type.GetElementType() }).Invoke(null, new object[] { arr[i] }),
                        reverse);
            }
            else if (type.IsArray && typeof(byte[]) == type)
                WriteByteArray(offset, (byte[])data, reverse);
            else if (typeof(byte) == type)
                WriteByte(offset, (byte)data);
            else
            {
                var convertedData = Convert.ChangeType(data, type);
                WriteByteArray(offset, (byte[])typeof(BitConverter)
                        .GetMethod("GetBytes", new Type[] { convertedData.GetType() })
                        .Invoke(null, new object[] { convertedData }), reverse);
            }
        }

        public static byte ReadByte(int offset)
        {
            return 0; //MainForm.SaveBuffer[offset];
        }

        public static byte[] ReadData(int offset, int size)
        {
            byte[] data = new byte[size];
            //Buffer.BlockCopy(MainForm.SaveBuffer, offset, data, 0, size);
            Array.Reverse(data);
            return data;
        }

        public static byte[] ReadDataRaw(int offset, int size)
        {
            byte[] data = new byte[size];
            //Buffer.BlockCopy(MainForm.SaveBuffer, offset, data, 0, size);
            return data;
        }

        public static ushort ReadUShort(int offset)
        {
            return BitConverter.ToUInt16(ReadData(offset, 2), 0);
        }

        public static ushort[] ReadUShortArray(int offset, int numUshorts)
        {
            ushort[] ushortArray = new ushort[numUshorts];
            for (int i = 0; i < numUshorts; i++)
                ushortArray[i] = ReadUShort(offset + i * 2);
            return ushortArray;
        }

        public static uint ReadUInt(int offset)
        {
            return BitConverter.ToUInt32(ReadData(offset, 4), 0);
        }

        public static uint[] ReadUIntArray(int offset, int numInts)
        {
            uint[] uintArray = new uint[numInts];
            for (int i = 0; i < numInts; i++)
                uintArray[i] = ReadUInt(offset + i * 4);
            return uintArray;
        }

        public static ACString ReadString(int offset, int maxSize)
        {
            byte[] data = new byte[maxSize];
            //Buffer.BlockCopy(MainForm.SaveBuffer, offset, data, 0, maxSize);
            return new ACString(data);
        }

        public static void WriteString(int offset, string str, int maxSize)
        {
            byte[] strBytes = new byte[maxSize];
            byte[] ACStringBytes = ACString.GetBytes(str, maxSize);
            if (ACStringBytes.Length <= maxSize)
            {
                ACStringBytes.CopyTo(strBytes, 0);
                if (str.Length < maxSize)
                    for (int i = (str.Length); i <= maxSize - 1; i++)
                        strBytes[i] = 0x20;
                //strBytes.CopyTo(MainForm.SaveBuffer, offset);
            }
        }

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
    }
}
