using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using ACSE.Checksums;
using ACSE.Utilities;

namespace ACSE
{
    public enum SaveType : byte
    {
        Unknown,
        DoubutsuNoMori,
        DoubutsuNoMoriPlus,
        AnimalCrossing,
        DoubutsuNoMoriEPlus,
        AnimalForestEPlus, // Fan translated version of Doubutsu no Mori e+
        AnimalForest, // iQue version
        WildWorld,
        CityFolk,
        NewLeaf,
        WelcomeAmiibo,
        ACSwitch
    }

    public enum SaveGeneration : byte
    {
        Unknown,
        N64,
        iQue,
        GCN,
        NDS,
        Wii,
        N3DS,
        Switch
    }

    public enum Region : byte
    {
        Unknown,
        Japan,
        NTSC,
        PAL,
        Australia,
        China
    }

    public class Save
    {
        public readonly SaveType SaveType;
        public readonly SaveGeneration SaveGeneration;
        public readonly SaveInfo SaveInfo;
        public readonly byte[] SaveData;
        public readonly int SaveDataStartOffset;
        public string FullSavePath;
        public string SavePath;
        public string SaveName;
        public string SaveExtension;
        public string SaveId;
        public readonly bool IsBigEndian = true;
        public bool ChangesMade;
        public readonly bool SuccessfullyLoaded = true;
        private FileStream _saveFile;
        private readonly BinaryReader _saveReader;
        private BinaryWriter _saveWriter;
        private readonly Backup _backup;
        private readonly bool _byteswap = false;

        public Save(string filePath)
        {
            if (File.Exists(filePath))
            {
                if (_saveFile != null)
                {
                    _saveReader.Close();
                    _saveFile.Close();
                }
                try { _saveFile = new FileStream(filePath, FileMode.Open); } catch { SuccessfullyLoaded = false; }
                if (_saveFile == null || !SuccessfullyLoaded || !_saveFile.CanWrite)
                {
                    MessageBox.Show(
                        $"Error: File {Path.GetFileName(filePath)} is being used by another process. Please close any process using it before editing!",
                        "File Opening Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    try
                    {
                        _saveFile?.Close();
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }

                _saveReader = new BinaryReader(_saveFile);
                SaveData = _saveReader.ReadBytes((int)_saveFile.Length);
                _byteswap = this.IsByteSwapped();
                if (_byteswap)
                {
                    SaveData = SaveDataManager.ByteSwap(SaveData); // Only byteswap the working data.
                }

                SaveType = this.GetSaveType();
                SaveGeneration = SaveDataManager.GetSaveGeneration(SaveType);
                FullSavePath = filePath;
                SaveName = Path.GetFileNameWithoutExtension(filePath);
                SavePath = Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar;
                SaveExtension = Path.GetExtension(filePath);
                SaveId = SaveDataManager.GetGameId(SaveType);
                SaveDataStartOffset = SaveDataManager.GetSaveDataOffset(SaveId.ToLower(), SaveExtension?.Replace(".", "").ToLower());
                SaveInfo = SaveDataManager.GetSaveInfo(SaveType);

                if (SaveType == SaveType.WildWorld || SaveGeneration == SaveGeneration.N3DS)
                    IsBigEndian = false;

                _saveReader.Close();
                _saveFile.Close();
                _saveReader.Dispose();
                _saveFile.Dispose();

                // Create a Backup
                if (Properties.Settings.Default.BackupFiles)
                    _backup = new Backup(this);
            }
            else
                MessageBox.Show("File doesn't exist!");
        }

        public void Flush()
        {
            var fullSaveName = SavePath + Path.DirectorySeparatorChar + SaveName + SaveExtension;
            _saveFile = new FileStream(fullSaveName, FileMode.OpenOrCreate);
            _saveWriter = new BinaryWriter(_saveFile);
            switch (SaveType)
            {
                case SaveType.DoubutsuNoMoriPlus:
                case SaveType.AnimalCrossing:
                case SaveType.DoubutsuNoMoriEPlus:
                case SaveType.AnimalForestEPlus:
                    Write(SaveDataStartOffset + SaveInfo.SaveOffsets.Checksum,
                        new UInt16BEChecksum().Calculate(
                            SaveData.Skip(SaveDataStartOffset).Take(SaveInfo.SaveOffsets.SaveSize).ToArray(),
                            (uint) SaveInfo.SaveOffsets.Checksum), IsBigEndian);

                    SaveData.Skip(SaveDataStartOffset).Take(SaveInfo.SaveOffsets.SaveSize).ToArray().CopyTo(
                        SaveData,
                        SaveDataStartOffset + SaveInfo.SaveOffsets.SaveSize);
                    break;
                case SaveType.WildWorld:
                    Write(SaveDataStartOffset + SaveInfo.SaveOffsets.Checksum,
                        new UInt16LEChecksum().Calculate(
                            SaveData.Skip(SaveDataStartOffset).Take(SaveInfo.SaveOffsets.SaveSize).ToArray(),
                            (uint) SaveInfo.SaveOffsets.Checksum), IsBigEndian);

                    SaveData.Skip(SaveDataStartOffset).Take(SaveInfo.SaveOffsets.SaveSize).ToArray().CopyTo(
                        SaveData,
                        SaveDataStartOffset + SaveInfo.SaveOffsets.SaveSize);
                    break;
                case SaveType.DoubutsuNoMori:
                case SaveType.AnimalForest:
                    Write(SaveDataStartOffset + SaveInfo.SaveOffsets.Checksum,
                        new UInt16BEChecksum().Calculate(
                            SaveData.Skip(SaveDataStartOffset).Take(0xF980).ToArray(),
                            (uint) SaveInfo.SaveOffsets.Checksum), IsBigEndian);

                    SaveData.Skip(SaveDataStartOffset).Take(SaveInfo.SaveOffsets.SaveSize).ToArray().CopyTo(
                        SaveData,
                        SaveDataStartOffset + SaveInfo.SaveOffsets.SaveSize);
                    break;
                case SaveType.CityFolk:
                    var crc32 = new CRC32();
                    for (var i = 0; i < 4; i++)
                    {
                        var playerDataOffset = SaveDataStartOffset + i * 0x86C0 + 0x1140;
                        var playerCrc32 =
                            crc32.Calculate(SaveData.Skip(playerDataOffset + 4).Take(0x759C).ToArray());
                        Write(playerDataOffset, playerCrc32, true);
                    }

                    Write(SaveDataStartOffset + 0x5EC60,
                        crc32.Calculate(SaveData.Skip(SaveDataStartOffset + 0x5EC64).Take(0x1497C).ToArray()),
                        true);
                    Write(SaveDataStartOffset + 0x5EB04,
                        crc32.Calculate(SaveData.Skip(SaveDataStartOffset + 0x5EB08).Take(0x152).ToArray(),
                            0x12141018), true);
                    Write(SaveDataStartOffset + 0x73600,
                        crc32.Calculate(SaveData.Skip(SaveDataStartOffset + 0x73604).Take(0x19BD1C).ToArray()),
                        true);
                    Write(SaveDataStartOffset,
                        crc32.Calculate(SaveData.Skip(SaveDataStartOffset + 4).Take(0x1C).ToArray()), true);
                    Write(SaveDataStartOffset + 0x20,
                        crc32.Calculate(SaveData.Skip(SaveDataStartOffset + 0x24).Take(0x111C).ToArray()), true);
                    break;
                case SaveType.NewLeaf:
                    var crc32Type1 = new NewLeafCRC32Type1();

                    Write(SaveDataStartOffset,
                        crc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 4).Take(0x1C).ToArray()));
                    for (var i = 0; i < 4; i++)
                    {
                        var dataOffset = SaveDataStartOffset + 0x20 + i * 0x9F10;
                        Write(dataOffset,
                            crc32Type1.Calculate(SaveData.Skip(dataOffset + 4).Take(0x6B64).ToArray()));

                        var dataOffset2 = SaveDataStartOffset + 0x20 + 0x6B68 + i * 0x9F10;
                        Write(dataOffset2,
                            crc32Type1.Calculate(SaveData.Skip(dataOffset2 + 4).Take(0x33A4).ToArray()));
                    }

                    Write(SaveDataStartOffset + 0x27C60,
                        crc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x27C60 + 4).Take(0x218B0)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x49520,
                        crc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x49520 + 4).Take(0x44B8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x4D9DC,
                        crc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x4D9DC + 4).Take(0x1E420)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x6BE00,
                        crc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x6BE00 + 4).Take(0x20)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x6BE24,
                        crc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x6BE24 + 4).Take(0x13AF8)
                            .ToArray()));
                    break;
                case SaveType.WelcomeAmiibo:
                    var waCrc32Type1 = new NewLeafCRC32Type1();
                    var waCrc32Type2 = new NewLeafCRC32Type2();

                    // CRC32 Implementation Type 1 Checksums
                    Write(SaveDataStartOffset,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 4).Take(0x1C).ToArray()));
                    for (var i = 0; i < 4; i++)
                    {
                        var dataOffset = SaveDataStartOffset + 0x20 + i * 0xA480;
                        Write(dataOffset,
                            waCrc32Type1.Calculate(SaveData.Skip(dataOffset + 4).Take(0x6B84).ToArray()));

                        var dataOffset2 = SaveDataStartOffset + 0x20 + 0x6B88 + i * 0xA480;
                        Write(dataOffset2,
                            waCrc32Type1.Calculate(SaveData.Skip(dataOffset2 + 4).Take(0x38F4).ToArray()));
                    }

                    Write(SaveDataStartOffset + 0x29220,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x29220 + 4).Take(0x22BC8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x4BE00,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x4BE00 + 4).Take(0x44B8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x533A4,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x533A4 + 4).Take(0x1E4D8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x71880,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x71880 + 4).Take(0x20)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x718A4,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x718A4 + 4).Take(0xBE4)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x738D4,
                        waCrc32Type1.Calculate(SaveData.Skip(SaveDataStartOffset + 0x738D4 + 4).Take(0x16188)
                            .ToArray()));

                    // CRC32 Implementation Type 2 Checksums
                    Write(SaveDataStartOffset + 0x502BC,
                        waCrc32Type2.Calculate(SaveData.Skip(SaveDataStartOffset + 0x502BC + 4).Take(0x28F0)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x52BB0,
                        waCrc32Type2.Calculate(SaveData.Skip(SaveDataStartOffset + 0x52BB0 + 4).Take(0x7F0)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x7248C,
                        waCrc32Type2.Calculate(SaveData.Skip(SaveDataStartOffset + 0x7248C + 4).Take(0x1444)
                            .ToArray()));
                    break;
            }

            _saveWriter.Write(SaveType == SaveType.DoubutsuNoMori && _byteswap
                ? SaveDataManager.ByteSwap(SaveData)
                : SaveData); // Doubutsu no Mori is dword byteswapped
            _saveWriter.Flush();
            _saveFile.Flush();

            _saveWriter.Close();
            _saveFile.Close();
            _saveWriter.Dispose();
            _saveFile.Dispose();
            ChangesMade = false;
        }

        public void Close(bool save)
        {
            if (save)
            {
                Flush();
            }

            _saveWriter?.Dispose();
            _saveReader?.Dispose();
            _saveFile?.Dispose();
        }

        public void Write(int offset, dynamic data, bool reversed = false, int stringLength = 0)
        {
            if (data == null) return;
            ChangesMade = true;
            Type dataType = data.GetType();
            MainForm.DebugManager.WriteLine(string.Format("Writing Data {2} of type {0} to offset 0x{1:X}", dataType.Name, offset, //recasting a value shows it as original type?
                dataType.IsArray ? "" : " with value 0x" + (data.ToString("X"))), DebugLevel.Debug);
            if (!dataType.IsArray)
            {
                if (dataType == typeof(byte))
                    SaveData[offset] = (byte)data;
                else if (dataType == typeof(string))
                {
                    var stringByteBuff = AcString.GetBytes((string)data, stringLength);
                    Buffer.BlockCopy(stringByteBuff, 0, SaveData, offset, stringByteBuff.Length);
                }
                else
                {
                    byte[] byteArray = BitConverter.GetBytes(data);
                    if (reversed)
                        Array.Reverse(byteArray);
                    Buffer.BlockCopy(byteArray, 0, SaveData, offset, byteArray.Length);
                }
            }
            else
            {
                if (dataType == typeof(byte[]))
                    for (var i = 0; i < data.Length; i++)
                        SaveData[offset + i] = data[i];
                else
                {
                    int dataSize = Marshal.SizeOf(data[0]);
                    for (var i = 0; i < data.Length; i++)
                    {
                        byte[] byteArray = BitConverter.GetBytes(data[i]);
                        if (reversed)
                            Array.Reverse(byteArray);
                        byteArray.CopyTo(SaveData, offset + i * dataSize);
                    }
                }
            }
        }

        public void FindAndReplaceByteArray(in byte[] oldarr, in byte[] newarr, int end = -1, bool reverse = false)
        {
            if (end < 0)
            {
                end = SaveData.Length;
            }

            for (var i = SaveDataStartOffset; i < SaveDataStartOffset + end - oldarr.Length; i += 2)
            {
                if (i + oldarr.Length >= SaveData.Length) break;

                if (ReadByteArray(i, oldarr.Length).SequenceEqual(oldarr))
                {
                    Write(i, newarr, reverse);
                }
            }
        }

        public byte ReadByte(int offset)
        {
            return SaveData[offset];
        }

        public byte[] ReadByteArray(int offset, int count, bool reversed = false)
        {
            var data = new byte[count];
            if (reversed)
            {
                for (int i = 0, idx = count - 1; i < count; i++, idx--)
                {
                    data[idx] = SaveData[offset + i];
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    data[i] = SaveData[offset + i];
                }
            }
            return data;
        }

        public ushort ReadUInt16(int offset, bool reversed = false)
        {
            var value = BitConverter.ToUInt16(SaveData, offset);
            if (reversed)
                value = value.Reverse();
            return value;
        }

        public ushort[] ReadUInt16Array(int offset, int count, bool reversed = false)
        {
            var returnedValues = new ushort[count];
            for (var i = 0; i < count; i++)
                returnedValues[i] = ReadUInt16(offset + i * 2, reversed);
            return returnedValues;
        }

        public uint ReadUInt32(int offset, bool reversed = false)
        {
            var value = BitConverter.ToUInt32(SaveData, offset);
            if (reversed)
                value = value.Reverse();
            return value;
        }

        public uint[] ReadUInt32Array(int offset, int count, bool reversed = false)
        {
            var returnedValues = new uint[count];
            for (var i = 0; i < count; i++)
                returnedValues[i] = ReadUInt32(offset + i * 4, reversed);
            return returnedValues;
        }

        public ulong ReadUInt64(int offset, bool reversed = false)
        {
            var value = BitConverter.ToUInt64(SaveData, offset);
            if (reversed)
                value = value.Reverse();
            return value;
        }

        public string ReadString(int offset, int length) =>
            new AcString(ReadByteArray(offset, length), SaveType).Trim();

        public string ReadAsciiString(int offset, int length = -1)
        {
            if (length >= 0) return Encoding.ASCII.GetString(SaveData, offset, length);

            var outString = "";
            while (offset < SaveData.Length)
            {
                var data = SaveData[offset++];
                if (data == 0) break;

                outString += (char) data;
            }

            return outString;
        }

        public string[] ReadStringArray(int offset, int length, int count)
        {
            var stringArray = new string[count];
            for (var i = 0; i < count; i++)
                stringArray[i] = ReadString(offset + i * length, length);
            return stringArray;
        }

        public string[] ReadStringArrayWithVariedLengths(int offset, int count, byte endCharByte, int maxLength = 10)
        {
            var stringArray = new string[count];
            var lastOffset = 0;
            for (var i = 0; i < count; i++)
            {
                byte lastChar = 0;
                var idx = 0;
                while (lastChar != endCharByte && idx < maxLength)
                {
                    lastChar = ReadByte(offset + lastOffset + idx);
                    idx++;
                }
                stringArray[i] = ReadString(offset + lastOffset, idx);
                lastOffset += idx;
            }
            return stringArray;
        }
    }
}
