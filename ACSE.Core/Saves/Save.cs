using System;
using System.Linq;
using System.IO;
using System.Text;
using ACSE.Core.Debug;
using ACSE.Core.Housing;
using ACSE.Core.Items;
using ACSE.Core.Players;
using ACSE.Core.Saves.Checksums;
using ACSE.Core.Town.Acres;
using ACSE.Core.Utilities;

namespace ACSE.Core.Saves
{
    public enum SaveType : byte
    {
        Unknown,
        DoubutsuNoMori,
        DoubutsuNoMoriPlus,
        AnimalCrossing,
        DoubutsuNoMoriEPlus,
        AnimalForestEPlus, // Fan translated version of Doubutsu no Mori e+
        DongwuSenlin, // iQue version
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

    public sealed class Save
    {
        public static Save SaveInstance { get; private set; }

        // Publics

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

        // Public events

        /// <summary>
        /// Fires when the currently selected <see cref="Item"/> is changed.
        /// </summary>
        public event EventHandler<ItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Fires when the currently selected <see cref="Player"/> is changed. The <see cref="EventHandler"/> contains a reference to the previously selected <see cref="Player"/>.
        /// </summary>
        public event EventHandler<Player> SelectedPlayerChanged;

        /// <summary>
        /// Fires when the currently selected <see cref="House"/> is changed. The <see cref="EventHandler"/> contains a reference to the previously selected <see cref="House"/>.
        /// </summary>
        public event EventHandler<House> SelectedHouseChanged;

        /// <summary>
        /// Fires when the currently selected <see cref="Acre"/> is changed. The <see cref="EventHandler"/> contains a reference to the previously selected <see cref="Acre"/>.
        /// </summary>
        public event EventHandler<Acre> SelectedAcreChanged;

        // Public Game-related Info
        public Player[] Players { get; private set; }
        public Acre[] Acres { get; private set; }
        public WorldAcre[] WorldAcres { get; private set; }
        public House[] Houses { get; private set; }


        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SelectedItemChanged?.Invoke(this, new ItemChangedEventArgs{ PreviousItem = _selectedItem, NewItem = value});
                _selectedItem = value;
            }
        }

        private Player _selectedPlayer;
        public Player SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                SelectedPlayerChanged?.Invoke(this, _selectedPlayer);
                _selectedPlayer = value;
            }
        }

        private House _selectedHouse;
        public House SelectedHouse
        {
            get => _selectedHouse;
            set
            {
                SelectedHouseChanged?.Invoke(this, _selectedHouse);
                _selectedHouse = value;
            }
        }

        private Acre _selectedAcre;
        public Acre SelectedAcre
        {
            get => _selectedAcre;
            set
            {
                SelectedAcreChanged?.Invoke(this, _selectedAcre);
                _selectedAcre = value;
            }
        }

        // Privates

        private FileStream _saveFile;
        private readonly BinaryReader _saveReader;
        private BinaryWriter _saveWriter;
        private readonly bool _byteswap = false;

        public Save(string filePath, bool createBackup = false)
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
                    DebugUtility.DebugManagerInstance.WriteLine(
                        $"Error: File {Path.GetFileName(filePath)} is being used by another process. Please close any process using it before editing!",
                        DebugLevel.Error);
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

                SaveInstance = this;
            }
            else
            {
                DebugUtility.DebugManagerInstance.WriteLine("File doesn't exist!", DebugLevel.Error);
            }
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
                case SaveType.DongwuSenlin:
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
                    var crc32Reflected = new NewLeafCRC32Reflected();

                    Write(SaveDataStartOffset,
                        crc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 4).Take(0x1C).ToArray()));
                    for (var i = 0; i < 4; i++)
                    {
                        var dataOffset = SaveDataStartOffset + 0x20 + i * 0x9F10;
                        Write(dataOffset,
                            crc32Reflected.Calculate(SaveData.Skip(dataOffset + 4).Take(0x6B64).ToArray()));

                        var dataOffset2 = SaveDataStartOffset + 0x20 + 0x6B68 + i * 0x9F10;
                        Write(dataOffset2,
                            crc32Reflected.Calculate(SaveData.Skip(dataOffset2 + 4).Take(0x33A4).ToArray()));
                    }

                    Write(SaveDataStartOffset + 0x27C60,
                        crc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x27C60 + 4).Take(0x218B0)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x49520,
                        crc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x49520 + 4).Take(0x44B8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x4D9DC,
                        crc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x4D9DC + 4).Take(0x1E420)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x6BE00,
                        crc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x6BE00 + 4).Take(0x20)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x6BE24,
                        crc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x6BE24 + 4).Take(0x13AF8)
                            .ToArray()));
                    break;
                case SaveType.WelcomeAmiibo:
                    var wCrc32Reflected = new NewLeafCRC32Reflected();
                    var wCrc32Normal = new NewLeafCRC32Normal();

                    // Reflected CRC32 Implementation Checksums
                    Write(SaveDataStartOffset,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 4).Take(0x1C).ToArray()));
                    for (var i = 0; i < 4; i++)
                    {
                        var dataOffset = SaveDataStartOffset + 0x20 + i * 0xA480;
                        Write(dataOffset,
                            wCrc32Reflected.Calculate(SaveData.Skip(dataOffset + 4).Take(0x6B84).ToArray()));

                        var dataOffset2 = SaveDataStartOffset + 0x20 + 0x6B88 + i * 0xA480;
                        Write(dataOffset2,
                            wCrc32Reflected.Calculate(SaveData.Skip(dataOffset2 + 4).Take(0x38F4).ToArray()));
                    }

                    Write(SaveDataStartOffset + 0x29220,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x29220 + 4).Take(0x22BC8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x4BE00,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x4BE00 + 4).Take(0x44B8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x533A4,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x533A4 + 4).Take(0x1E4D8)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x71880,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x71880 + 4).Take(0x20)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x718A4,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x718A4 + 4).Take(0xBE4)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x738D4,
                        wCrc32Reflected.Calculate(SaveData.Skip(SaveDataStartOffset + 0x738D4 + 4).Take(0x16188)
                            .ToArray()));

                    // Normal CRC32 Implementation Checksums
                    Write(SaveDataStartOffset + 0x502BC,
                        wCrc32Normal.Calculate(SaveData.Skip(SaveDataStartOffset + 0x502BC + 4).Take(0x28F0)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x52BB0,
                        wCrc32Normal.Calculate(SaveData.Skip(SaveDataStartOffset + 0x52BB0 + 4).Take(0x7F0)
                            .ToArray()));
                    Write(SaveDataStartOffset + 0x7248C,
                        wCrc32Normal.Calculate(SaveData.Skip(SaveDataStartOffset + 0x7248C + 4).Take(0x1444)
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

        // Data methods

        public void Write(int offset, byte value, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            SaveData[offset] = value;
        }

        public void Write(int offset, sbyte value, bool includeStartOffset = false) =>
            Write(offset, (byte) value, includeStartOffset);

        public void Write(int offset, byte[] value, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            Buffer.BlockCopy(value, 0, SaveData, offset, value.Length);
        }

        public void Write(int offset, sbyte[] value, bool includeStartOffset = false) =>
            Write(offset, (byte[])(Array) value, includeStartOffset);

        public void Write(int offset, ushort value, bool? reversed = false, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            if (reversed == null && IsBigEndian || reversed != null && reversed.Value) value = value.Reverse();

            Write(offset, BitConverter.GetBytes(value));
        }

        public void Write(int offset, short value, bool? reversed = false, bool includeStartOffset = false) =>
            Write(offset, (ushort) value, reversed, includeStartOffset);

        public void Write(int offset, uint value, bool? reversed = false, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            if (reversed == null && IsBigEndian || reversed != null && reversed.Value) value = value.Reverse();

            Write(offset, BitConverter.GetBytes(value));
        }

        public void Write(int offset, int value, bool? reversed = false, bool includeStartOffset = false) =>
            Write(offset, (uint) value, reversed, includeStartOffset);

        public void Write(int offset, ulong value, bool? reversed = false, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            if (reversed == null && IsBigEndian || reversed != null && reversed.Value) value = value.Reverse();

            Write(offset, BitConverter.GetBytes(value));
        }

        public void Write(int offset, long value, bool? reversed = false, bool includeStartOffset = false) =>
            Write(offset, (ulong)value, reversed, includeStartOffset);

        public void Write(int offset, ushort[] value, bool? reversed = false, bool includeStartOffset = false)
        {
            foreach (var data in value)
            {
                Write(offset, data, reversed, includeStartOffset);
                offset += 2;
            }
        }

        public void Write(int offset, short[] value, bool? reversed = false, bool includeStartOffset = false) =>
            Write(offset, (ushort[]) (Array) value, reversed, includeStartOffset);

        public void Write(int offset, uint[] value, bool? reversed = false, bool includeStartOffset = false)
        {
            foreach (var data in value)
            {
                Write(offset, data, reversed, includeStartOffset);
                offset += 4;
            }
        }

        public void Write(int offset, int[] value, bool? reversed = false, bool includeStartOffset = false) =>
            Write(offset, (uint[])(Array)value, reversed, includeStartOffset);

        public void Write(int offset, ulong[] value, bool? reversed = false, bool includeStartOffset = false)
        {
            foreach (var data in value)
            {
                Write(offset, data, reversed, includeStartOffset);
                offset += 8;
            }
        }

        public void Write(int offset, long[] value, bool? reversed = false, bool includeStartOffset = false) =>
            Write(offset, (ulong[])(Array)value, reversed, includeStartOffset);

        public void Write(int offset, string value, int maxLength = 0, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

            var stringByteBuff = AcString.GetBytes(value, maxLength);
            Buffer.BlockCopy(stringByteBuff, 0, SaveData, offset, stringByteBuff.Length);
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

        public byte ReadByte(int offset, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            return SaveData[offset];
        }

        public byte[] ReadByteArray(int offset, int count, bool reversed = false,
            bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

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

        public ushort ReadUInt16(int offset, bool reversed = false, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

            var value = BitConverter.ToUInt16(SaveData, offset);
            if (reversed)
                value = value.Reverse();
            return value;
        }

        public ushort[] ReadUInt16Array(int offset, int count, bool reversed = false,
            bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

            var returnedValues = new ushort[count];
            for (var i = 0; i < count; i++)
                returnedValues[i] = ReadUInt16(offset + i * 2, reversed);
            return returnedValues;
        }

        public uint ReadUInt32(int offset, bool reversed = false, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

            var value = BitConverter.ToUInt32(SaveData, offset);
            if (reversed)
                value = value.Reverse();
            return value;
        }

        public uint[] ReadUInt32Array(int offset, int count, bool reversed = false,
            bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

            var returnedValues = new uint[count];
            for (var i = 0; i < count; i++)
                returnedValues[i] = ReadUInt32(offset + i * 4, reversed);
            return returnedValues;
        }

        public ulong ReadUInt64(int offset, bool reversed = false, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;

            var value = BitConverter.ToUInt64(SaveData, offset);
            if (reversed)
                value = value.Reverse();
            return value;
        }

        public string ReadString(int offset, int length, bool includeStartOffset = false) =>
            new AcString(ReadByteArray(offset, length, false, includeStartOffset), SaveType).Trim();

        public string ReadAsciiString(int offset, int length = -1, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
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

        public string[] ReadStringArray(int offset, int length, int count, bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
            var stringArray = new string[count];
            for (var i = 0; i < count; i++)
                stringArray[i] = ReadString(offset + i * length, length);
            return stringArray;
        }

        public string[] ReadStringArrayWithVariedLengths(int offset, int count, byte endCharByte, int maxLength = 10,
            bool includeStartOffset = false)
        {
            if (includeStartOffset) offset += SaveDataStartOffset;
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
