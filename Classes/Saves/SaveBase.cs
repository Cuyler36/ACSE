using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACSE.Classes.Saves
{
    /// <summary>
    /// The base class for all Save generations' Save Files.
    /// </summary>
    abstract class SaveBase
    {
        private int SaveDataStartOffset;
        private bool BigEndian;

        public SaveType SaveType;
        public SaveGeneration Generation;
        public Region Region;

        public Player[] Players;
        public WorldAcre[] Acres;
        public WorldAcre[] TownAcres;
        public NewVillager[] Villagers;

        public string FileName;
        public string FileExtension;
        public string FilePath;

        protected byte[] OriginalData;
        protected byte[] Data;

        public bool Modified { get; protected set; } = false;
        public bool Loaded { get; protected set; } = false;
        public bool ChangesMade { get; protected set; } = false;

        public SaveBase(string savePath)
        {
            FileName = Path.GetFileNameWithoutExtension(savePath);
            FileExtension = Path.GetExtension(savePath);
            FilePath = Path.GetDirectoryName(savePath);

            bool SuccessfullyLoadedFile = false;

            try
            {
                OriginalData = File.ReadAllBytes(savePath);
                SuccessfullyLoadedFile = true;
            }
            catch
            {
                MessageBox.Show("Unable to load the file [{0}]!\nIt may be in use by another program.", "File Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (SuccessfullyLoadedFile)
                {
                    Data = OriginalData.Clone() as byte[];
                }
            }
        }

        /// <summary>
        /// Parses and loads the save file's data
        /// </summary>
        /// <returns>bool: Loaded successfully</returns>
        protected abstract bool Load();

        #region Read Methods

        /// <summary>
        /// Reads a byte at the requested offset
        /// </summary>
        /// <param name="offset">The offset to read the byte from</param>
        /// <returns>The byte at the specified offset</returns>
        public virtual byte ReadByte(int offset)
        {
            return Data[SaveDataStartOffset + offset];
        }

        /// <summary>
        /// Reads an array of bytes from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="count">The amount of bytes to read</param>
        /// <returns>The array of bytes at the specified offset</returns>
        public virtual byte[] ReadByteArray(int offset, int count)
        {
            byte[] Buffer = new byte[count];
            for (int i = 0; i < count; i++)
                Buffer[i] = ReadByte(offset + i);

            return Buffer;
        }

        /// <summary>
        /// Reads an ushort from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="bigEndian">Read as big endian</param>
        /// <returns>The ushort at the specified offset</returns>
        public virtual ushort ReadUInt16(int offset, bool? bigEndian = null)
        {
            offset += SaveDataStartOffset;
            if (!bigEndian.HasValue)
            {
                return BigEndian ? (ushort)((Data[offset] << 8) | Data[offset + 1]) : (ushort)((Data[offset + 1] << 8) | Data[offset]);
            }
            else
            {
                return bigEndian.Value ? (ushort)((Data[offset] << 8) | Data[offset + 1]) : (ushort)((Data[offset + 1] << 8) | Data[offset]);
            }
        }

        /// <summary>
        /// Reads an array of ushorts from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="count">The amount of ushorts to read</param>
        /// <param name="bigEndian">Read as big endian</param>
        /// <returns></returns>
        public virtual ushort[] ReadUInt16Array(int offset, int count, bool? bigEndian = null)
        {
            ushort[] Buffer = new ushort[count];
            for (int i = 0; i < count; i++)
                Buffer[i] = ReadUInt16(offset + i * 2, bigEndian);

            return Buffer;
        }

        /// <summary>
        /// Reads an uint from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="bigEndian">Read as big endian</param>
        /// <returns></returns>
        public virtual uint ReadUInt32(int offset, bool? bigEndian = null)
        {
            offset += SaveDataStartOffset;
            uint Value = (uint)((Data[offset + 3] << 24) | (Data[offset + 2] << 16) | (Data[offset + 1] << 8) | Data[offset]);

            if ((!bigEndian.HasValue && BigEndian) || (bigEndian.HasValue && bigEndian.Value))
                Value = Value.Reverse();

            return Value;
        }

        /// <summary>
        /// Reads an array of uints from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="count">The amount of uints to read</param>
        /// <param name="bigEndian">Read as big endian</param>
        /// <returns></returns>
        public virtual uint[] ReadUInt32Array(int offset, int count, bool? bigEndian = null)
        {
            uint[] Buffer = new uint[count];
            for (int i = 0; i < count; i++)
                Buffer[i] = ReadUInt16(offset + i * 4, bigEndian);

            return Buffer;
        }

        /// <summary>
        /// Reads an ulong from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="bigEndian">Read as big endian</param>
        /// <returns></returns>
        public virtual ulong ReadUInt64(int offset, bool? bigEndian = null)
        {
            offset += SaveDataStartOffset;
            ulong Value = ((ulong)Data[offset + 7] << 56) | ((ulong)Data[offset + 6] << 48) | ((ulong)Data[offset + 5] << 40)
                | ((ulong)Data[offset + 4] << 32) | ((ulong)Data[offset + 3] << 24) | ((ulong)Data[offset + 2] << 16) | ((ulong)Data[offset + 1] << 8)
                | (Data[offset]);

            if ((!bigEndian.HasValue && BigEndian) || (bigEndian.HasValue && bigEndian.Value))
                Value = Value.Reverse();

            return Value;
        }

        /// <summary>
        /// Reads an array of ulongs from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="count">The amount of ulongs to read</param>
        /// <param name="bigEndian">Read as big endian</param>
        /// <returns></returns>
        public virtual ulong[] ReadUInt64Array(int offset, int count, bool? bigEndian = null)
        {
            ulong[] Buffer = new ulong[count];
            for (int i = 0; i < count; i++)
                Buffer[i] = ReadUInt64(offset + i * 8, bigEndian);

            return Buffer;
        }

        /// <summary>
        /// Reads a char from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <returns></returns>
        public virtual char ReadChar(int offset)
        {
            return (char)Data[SaveDataStartOffset + offset];
        }

        /// <summary>
        /// Reads an array of chars from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="count">The amount of chars to read</param>
        /// <returns></returns>
        public virtual char[] ReadCharArray(int offset, int count)
        {
            char[] Buffer = new char[count];
            for (int i = 0; i < count; i++)
                Buffer[i] = ReadChar(offset + i);

            return Buffer;
        }

        /// <summary>
        /// Reads a string from an offset
        /// </summary>
        /// <param name="offset">The offset to read from</param>
        /// <param name="length">The length of the string</param>
        /// <returns></returns>
        public virtual string ReadString(int offset, int length)
        {
            return new Utilities.ACString(ReadByteArray(offset, length), SaveType).Trim();
        }

        #endregion

        /// <summary>
        /// Writes any data type to an offset
        /// </summary>
        /// <param name="offset">The offset to write to</param>
        /// <param name="data">The data to be written</param>
        /// <param name="bigEndian">Write as big endian</param>
        /// <param name="stringLength">The length of the string to be written, only used if data is a string</param>
        public virtual void Write(int offset, dynamic data, bool? bigEndian = null, int stringLength = 0)
        {
            bool reversed = false;
            if (bigEndian.HasValue)
                reversed = bigEndian.Value;

            ChangesMade = true;
            Type Data_Type = data.GetType();

            if (!Data_Type.IsArray)
            {
                if (Data_Type == typeof(byte))
                    Data[offset] = (byte)data;
                else if (Data_Type == typeof(string))
                {
                    byte[] String_Byte_Buff = Utilities.ACString.GetBytes((string)data, stringLength);
                    Buffer.BlockCopy(String_Byte_Buff, 0, Data, offset, String_Byte_Buff.Length);
                }
                else
                {
                    byte[] Byte_Array = BitConverter.GetBytes(data);
                    if (reversed)
                        Array.Reverse(Byte_Array);
                    Buffer.BlockCopy(Byte_Array, 0, Data, offset, Byte_Array.Length);
                }
            }
            else
            {
                if (Data_Type == typeof(byte[]))
                    for (int i = 0; i < data.Length; i++)
                        Data[offset + i] = data[i];
                else
                {
                    int Data_Size = System.Runtime.InteropServices.Marshal.SizeOf(data[0]);
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte[] Byte_Array = BitConverter.GetBytes(data[i]);
                        if (reversed)
                            Array.Reverse(Byte_Array);
                        Byte_Array.CopyTo(Data, offset + i * Data_Size);
                    }
                }
            }
        }
    }
}
