using System;
using System.Collections.Generic;

namespace ACSE.Core.Generators
{
    public sealed class GCNGenerator : IGenerator
    {
        private int _randomSeed = Environment.TickCount;

        private enum CliffSide : ushort
        {
            Up = 0,
            Down = 1,
            Any = 2
        }

        private enum RiverSide : ushort
        {
            Left = 0,
            Right = 1,
            Any = 2
        }

        private struct data_combi
        {
            public ushort BlockType;
            public ushort MatchingBlockType;
            public byte ValidCombiCount; // Unsure about this name.
            public byte Padding;
        };

        private const int data_combi_table_number = 0x170; // This is only valid for Animal Crossing. Animal Forest+ has one less, and Animal Forest e+ has one more.

        private static readonly data_combi[] data_combi_table = new data_combi[368]
        {
            new data_combi { BlockType = 0x0124, MatchingBlockType = 0x00CB, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x003C, MatchingBlockType = 0x0000, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x00CB, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0004, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0005, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x000C, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0009, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x000B, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00F4, MatchingBlockType = 0x000D, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x000E, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x00CB, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x000F, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0010, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0011, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0012, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0013, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0014, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0015, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00F2, MatchingBlockType = 0x0016, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0017, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x001B, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00F6, MatchingBlockType = 0x001A, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x001B, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0069, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00CB, MatchingBlockType = 0x004E, ValidCombiCount = 0x33, Padding = 0x00 },
            new data_combi { BlockType = 0x00F8, MatchingBlockType = 0x0022, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0027, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0033, MatchingBlockType = 0x002D, ValidCombiCount = 0x1A, Padding = 0x00 },
            new data_combi { BlockType = 0x00DA, MatchingBlockType = 0x002E, ValidCombiCount = 0x0D, Padding = 0x00 },
            new data_combi { BlockType = 0x010D, MatchingBlockType = 0x0001, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x010E, MatchingBlockType = 0x0001, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x010F, MatchingBlockType = 0x0001, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0110, MatchingBlockType = 0x0001, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0006, MatchingBlockType = 0x0029, ValidCombiCount = 0x16, Padding = 0x00 },
            new data_combi { BlockType = 0x000F, MatchingBlockType = 0x002A, ValidCombiCount = 0x36, Padding = 0x00 },
            new data_combi { BlockType = 0x0016, MatchingBlockType = 0x002B, ValidCombiCount = 0x17, Padding = 0x00 },
            new data_combi { BlockType = 0x0018, MatchingBlockType = 0x002C, ValidCombiCount = 0x1E, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x002F, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x005B, MatchingBlockType = 0x0030, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x0001, MatchingBlockType = 0x0031, ValidCombiCount = 0x0F, Padding = 0x00 },
            new data_combi { BlockType = 0x0009, MatchingBlockType = 0x0032, ValidCombiCount = 0x1D, Padding = 0x00 },
            new data_combi { BlockType = 0x000C, MatchingBlockType = 0x0033, ValidCombiCount = 0x22, Padding = 0x00 },
            new data_combi { BlockType = 0x0013, MatchingBlockType = 0x0034, ValidCombiCount = 0x10, Padding = 0x00 },
            new data_combi { BlockType = 0x001D, MatchingBlockType = 0x0035, ValidCombiCount = 0x11, Padding = 0x00 },
            new data_combi { BlockType = 0x0020, MatchingBlockType = 0x0036, ValidCombiCount = 0x18, Padding = 0x00 },
            new data_combi { BlockType = 0x0025, MatchingBlockType = 0x0037, ValidCombiCount = 0x12, Padding = 0x00 },
            new data_combi { BlockType = 0x0028, MatchingBlockType = 0x0038, ValidCombiCount = 0x19, Padding = 0x00 },
            new data_combi { BlockType = 0x002A, MatchingBlockType = 0x0039, ValidCombiCount = 0x20, Padding = 0x00 },
            new data_combi { BlockType = 0x0030, MatchingBlockType = 0x003A, ValidCombiCount = 0x13, Padding = 0x00 },
            new data_combi { BlockType = 0x0035, MatchingBlockType = 0x003B, ValidCombiCount = 0x21, Padding = 0x00 },
            new data_combi { BlockType = 0x0037, MatchingBlockType = 0x003C, ValidCombiCount = 0x24, Padding = 0x00 },
            new data_combi { BlockType = 0x003B, MatchingBlockType = 0x003D, ValidCombiCount = 0x14, Padding = 0x00 },
            new data_combi { BlockType = 0x003E, MatchingBlockType = 0x003E, ValidCombiCount = 0x1B, Padding = 0x00 },
            new data_combi { BlockType = 0x0042, MatchingBlockType = 0x003F, ValidCombiCount = 0x15, Padding = 0x00 },
            new data_combi { BlockType = 0x00AA, MatchingBlockType = 0x0040, ValidCombiCount = 0x28, Padding = 0x00 },
            new data_combi { BlockType = 0x00B2, MatchingBlockType = 0x0041, ValidCombiCount = 0x29, Padding = 0x00 },
            new data_combi { BlockType = 0x00BA, MatchingBlockType = 0x0042, ValidCombiCount = 0x2A, Padding = 0x00 },
            new data_combi { BlockType = 0x00C2, MatchingBlockType = 0x0043, ValidCombiCount = 0x2B, Padding = 0x00 },
            new data_combi { BlockType = 0x00C8, MatchingBlockType = 0x0044, ValidCombiCount = 0x2C, Padding = 0x00 },
            new data_combi { BlockType = 0x00CE, MatchingBlockType = 0x0045, ValidCombiCount = 0x2D, Padding = 0x00 },
            new data_combi { BlockType = 0x00D4, MatchingBlockType = 0x0046, ValidCombiCount = 0x2E, Padding = 0x00 },
            new data_combi { BlockType = 0x0022, MatchingBlockType = 0x0047, ValidCombiCount = 0x1F, Padding = 0x00 },
            new data_combi { BlockType = 0x0047, MatchingBlockType = 0x0048, ValidCombiCount = 0x26, Padding = 0x00 },
            new data_combi { BlockType = 0x0040, MatchingBlockType = 0x0049, ValidCombiCount = 0x25, Padding = 0x00 },
            new data_combi { BlockType = 0x00AE, MatchingBlockType = 0x004A, ValidCombiCount = 0x2F, Padding = 0x00 },
            new data_combi { BlockType = 0x00B6, MatchingBlockType = 0x004B, ValidCombiCount = 0x30, Padding = 0x00 },
            new data_combi { BlockType = 0x00BE, MatchingBlockType = 0x004C, ValidCombiCount = 0x31, Padding = 0x00 },
            new data_combi { BlockType = 0x00C5, MatchingBlockType = 0x004D, ValidCombiCount = 0x32, Padding = 0x00 },
            new data_combi { BlockType = 0x00D1, MatchingBlockType = 0x004F, ValidCombiCount = 0x34, Padding = 0x00 },
            new data_combi { BlockType = 0x00D7, MatchingBlockType = 0x0050, ValidCombiCount = 0x35, Padding = 0x00 },
            new data_combi { BlockType = 0x00DB, MatchingBlockType = 0x0051, ValidCombiCount = 0x0C, Padding = 0x00 },
            new data_combi { BlockType = 0x0010, MatchingBlockType = 0x0052, ValidCombiCount = 0x36, Padding = 0x00 },
            new data_combi { BlockType = 0x001A, MatchingBlockType = 0x0053, ValidCombiCount = 0x37, Padding = 0x00 },
            new data_combi { BlockType = 0x0024, MatchingBlockType = 0x0054, ValidCombiCount = 0x38, Padding = 0x00 },
            new data_combi { BlockType = 0x002E, MatchingBlockType = 0x0055, ValidCombiCount = 0x39, Padding = 0x00 },
            new data_combi { BlockType = 0x0039, MatchingBlockType = 0x0056, ValidCombiCount = 0x3A, Padding = 0x00 },
            new data_combi { BlockType = 0x0041, MatchingBlockType = 0x0057, ValidCombiCount = 0x3B, Padding = 0x00 },
            new data_combi { BlockType = 0x0049, MatchingBlockType = 0x0058, ValidCombiCount = 0x3C, Padding = 0x00 },
            new data_combi { BlockType = 0x0045, MatchingBlockType = 0x005A, ValidCombiCount = 0x1C, Padding = 0x00 },
            new data_combi { BlockType = 0x002C, MatchingBlockType = 0x0059, ValidCombiCount = 0x23, Padding = 0x00 },
            new data_combi { BlockType = 0x0115, MatchingBlockType = 0x0028, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x010B, MatchingBlockType = 0x00C8, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0105, MatchingBlockType = 0x00C9, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0105, MatchingBlockType = 0x00CA, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0006, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00EF, MatchingBlockType = 0x00CC, ValidCombiCount = 0x0B, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x00CD, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0002, MatchingBlockType = 0x00C0, ValidCombiCount = 0x0F, Padding = 0x00 },
            new data_combi { BlockType = 0x0003, MatchingBlockType = 0x006B, ValidCombiCount = 0x0F, Padding = 0x00 },
            new data_combi { BlockType = 0x0004, MatchingBlockType = 0x006C, ValidCombiCount = 0x0F, Padding = 0x00 },
            new data_combi { BlockType = 0x0005, MatchingBlockType = 0x00BF, ValidCombiCount = 0x0F, Padding = 0x00 },
            new data_combi { BlockType = 0x0014, MatchingBlockType = 0x0072, ValidCombiCount = 0x10, Padding = 0x00 },
            new data_combi { BlockType = 0x00AB, MatchingBlockType = 0x0090, ValidCombiCount = 0x28, Padding = 0x00 },
            new data_combi { BlockType = 0x00AC, MatchingBlockType = 0x0091, ValidCombiCount = 0x28, Padding = 0x00 },
            new data_combi { BlockType = 0x00C3, MatchingBlockType = 0x009E, ValidCombiCount = 0x2B, Padding = 0x00 },
            new data_combi { BlockType = 0x00C9, MatchingBlockType = 0x00A1, ValidCombiCount = 0x2C, Padding = 0x00 },
            new data_combi { BlockType = 0x00D5, MatchingBlockType = 0x00A7, ValidCombiCount = 0x2E, Padding = 0x00 },
            new data_combi { BlockType = 0x00D6, MatchingBlockType = 0x00A8, ValidCombiCount = 0x2E, Padding = 0x00 },
            new data_combi { BlockType = 0x001B, MatchingBlockType = 0x0074, ValidCombiCount = 0x37, Padding = 0x00 },
            new data_combi { BlockType = 0x0011, MatchingBlockType = 0x0071, ValidCombiCount = 0x36, Padding = 0x00 },
            new data_combi { BlockType = 0x002F, MatchingBlockType = 0x007D, ValidCombiCount = 0x39, Padding = 0x00 },
            new data_combi { BlockType = 0x003A, MatchingBlockType = 0x0081, ValidCombiCount = 0x3A, Padding = 0x00 },
            new data_combi { BlockType = 0x003F, MatchingBlockType = 0x0084, ValidCombiCount = 0x1B, Padding = 0x00 },
            new data_combi { BlockType = 0x0021, MatchingBlockType = 0x0077, ValidCombiCount = 0x18, Padding = 0x00 },
            new data_combi { BlockType = 0x001E, MatchingBlockType = 0x0075, ValidCombiCount = 0x11, Padding = 0x00 },
            new data_combi { BlockType = 0x0044, MatchingBlockType = 0x0086, ValidCombiCount = 0x15, Padding = 0x00 },
            new data_combi { BlockType = 0x000A, MatchingBlockType = 0x006D, ValidCombiCount = 0x1D, Padding = 0x00 },
            new data_combi { BlockType = 0x000D, MatchingBlockType = 0x006F, ValidCombiCount = 0x22, Padding = 0x00 },
            new data_combi { BlockType = 0x0026, MatchingBlockType = 0x0078, ValidCombiCount = 0x12, Padding = 0x00 },
            new data_combi { BlockType = 0x0031, MatchingBlockType = 0x007E, ValidCombiCount = 0x13, Padding = 0x00 },
            new data_combi { BlockType = 0x003C, MatchingBlockType = 0x0082, ValidCombiCount = 0x14, Padding = 0x00 },
            new data_combi { BlockType = 0x00B3, MatchingBlockType = 0x0095, ValidCombiCount = 0x29, Padding = 0x00 },
            new data_combi { BlockType = 0x00BB, MatchingBlockType = 0x0099, ValidCombiCount = 0x2A, Padding = 0x00 },
            new data_combi { BlockType = 0x0029, MatchingBlockType = 0x007A, ValidCombiCount = 0x19, Padding = 0x00 },
            new data_combi { BlockType = 0x002B, MatchingBlockType = 0x007B, ValidCombiCount = 0x20, Padding = 0x00 },
            new data_combi { BlockType = 0x0046, MatchingBlockType = 0x0087, ValidCombiCount = 0x1C, Padding = 0x00 },
            new data_combi { BlockType = 0x00CF, MatchingBlockType = 0x00A4, ValidCombiCount = 0x2D, Padding = 0x00 },
            new data_combi { BlockType = 0x0036, MatchingBlockType = 0x00B2, ValidCombiCount = 0x21, Padding = 0x00 },
            new data_combi { BlockType = 0x002D, MatchingBlockType = 0x007C, ValidCombiCount = 0x23, Padding = 0x00 },
            new data_combi { BlockType = 0x00B4, MatchingBlockType = 0x0096, ValidCombiCount = 0x29, Padding = 0x00 },
            new data_combi { BlockType = 0x00BC, MatchingBlockType = 0x009A, ValidCombiCount = 0x2A, Padding = 0x00 },
            new data_combi { BlockType = 0x0038, MatchingBlockType = 0x0080, ValidCombiCount = 0x24, Padding = 0x00 },
            new data_combi { BlockType = 0x0032, MatchingBlockType = 0x007F, ValidCombiCount = 0x13, Padding = 0x00 },
            new data_combi { BlockType = 0x0027, MatchingBlockType = 0x0079, ValidCombiCount = 0x12, Padding = 0x00 },
            new data_combi { BlockType = 0x001F, MatchingBlockType = 0x0076, ValidCombiCount = 0x11, Padding = 0x00 },
            new data_combi { BlockType = 0x0015, MatchingBlockType = 0x0073, ValidCombiCount = 0x10, Padding = 0x00 },
            new data_combi { BlockType = 0x010C, MatchingBlockType = 0x00CE, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00B1, MatchingBlockType = 0x00C1, ValidCombiCount = 0x45, Padding = 0x00 },
            new data_combi { BlockType = 0x0007, MatchingBlockType = 0x005B, ValidCombiCount = 0x16, Padding = 0x00 },
            new data_combi { BlockType = 0x0008, MatchingBlockType = 0x005C, ValidCombiCount = 0x16, Padding = 0x00 },
            new data_combi { BlockType = 0x000B, MatchingBlockType = 0x006E, ValidCombiCount = 0x1D, Padding = 0x00 },
            new data_combi { BlockType = 0x000E, MatchingBlockType = 0x0070, ValidCombiCount = 0x22, Padding = 0x00 },
            new data_combi { BlockType = 0x0017, MatchingBlockType = 0x005E, ValidCombiCount = 0x17, Padding = 0x00 },
            new data_combi { BlockType = 0x0034, MatchingBlockType = 0x005D, ValidCombiCount = 0x1A, Padding = 0x00 },
            new data_combi { BlockType = 0x003D, MatchingBlockType = 0x0083, ValidCombiCount = 0x14, Padding = 0x00 },
            new data_combi { BlockType = 0x0043, MatchingBlockType = 0x0085, ValidCombiCount = 0x15, Padding = 0x00 },
            new data_combi { BlockType = 0x00AD, MatchingBlockType = 0x0092, ValidCombiCount = 0x28, Padding = 0x00 },
            new data_combi { BlockType = 0x00B5, MatchingBlockType = 0x00B3, ValidCombiCount = 0x29, Padding = 0x00 },
            new data_combi { BlockType = 0x00BD, MatchingBlockType = 0x009B, ValidCombiCount = 0x2A, Padding = 0x00 },
            new data_combi { BlockType = 0x00C4, MatchingBlockType = 0x009F, ValidCombiCount = 0x2B, Padding = 0x00 },
            new data_combi { BlockType = 0x00CA, MatchingBlockType = 0x00A2, ValidCombiCount = 0x2C, Padding = 0x00 },
            new data_combi { BlockType = 0x00D0, MatchingBlockType = 0x00A5, ValidCombiCount = 0x2D, Padding = 0x00 },
            new data_combi { BlockType = 0x00E4, MatchingBlockType = 0x0067, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E4, MatchingBlockType = 0x0066, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0068, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0019, MatchingBlockType = 0x005F, ValidCombiCount = 0x1E, Padding = 0x00 },
            new data_combi { BlockType = 0x0023, MatchingBlockType = 0x0060, ValidCombiCount = 0x1F, Padding = 0x00 },
            new data_combi { BlockType = 0x00AF, MatchingBlockType = 0x0093, ValidCombiCount = 0x2F, Padding = 0x00 },
            new data_combi { BlockType = 0x00B7, MatchingBlockType = 0x0097, ValidCombiCount = 0x30, Padding = 0x00 },
            new data_combi { BlockType = 0x00C6, MatchingBlockType = 0x00A0, ValidCombiCount = 0x32, Padding = 0x00 },
            new data_combi { BlockType = 0x00BF, MatchingBlockType = 0x009C, ValidCombiCount = 0x31, Padding = 0x00 },
            new data_combi { BlockType = 0x00CC, MatchingBlockType = 0x00A3, ValidCombiCount = 0x33, Padding = 0x00 },
            new data_combi { BlockType = 0x00D2, MatchingBlockType = 0x00A6, ValidCombiCount = 0x34, Padding = 0x00 },
            new data_combi { BlockType = 0x00D8, MatchingBlockType = 0x00A9, ValidCombiCount = 0x35, Padding = 0x00 },
            new data_combi { BlockType = 0x00B0, MatchingBlockType = 0x0094, ValidCombiCount = 0x2F, Padding = 0x00 },
            new data_combi { BlockType = 0x00B8, MatchingBlockType = 0x0098, ValidCombiCount = 0x30, Padding = 0x00 },
            new data_combi { BlockType = 0x00C0, MatchingBlockType = 0x009D, ValidCombiCount = 0x31, Padding = 0x00 },
            new data_combi { BlockType = 0x005C, MatchingBlockType = 0x0088, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x005D, MatchingBlockType = 0x0089, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x005E, MatchingBlockType = 0x008A, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x005F, MatchingBlockType = 0x008B, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x0060, MatchingBlockType = 0x008C, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x0061, MatchingBlockType = 0x008D, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x0062, MatchingBlockType = 0x008E, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x0063, MatchingBlockType = 0x008F, ValidCombiCount = 0x27, Padding = 0x00 },
            new data_combi { BlockType = 0x00DC, MatchingBlockType = 0x00AA, ValidCombiCount = 0x0C, Padding = 0x00 },
            new data_combi { BlockType = 0x00DD, MatchingBlockType = 0x00B6, ValidCombiCount = 0x0C, Padding = 0x00 },
            new data_combi { BlockType = 0x00DE, MatchingBlockType = 0x00B7, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00DF, MatchingBlockType = 0x00AB, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E0, MatchingBlockType = 0x00AE, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E1, MatchingBlockType = 0x00B8, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E2, MatchingBlockType = 0x00AF, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E3, MatchingBlockType = 0x00B9, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E4, MatchingBlockType = 0x00AD, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00E8, MatchingBlockType = 0x00B0, ValidCombiCount = 0x0D, Padding = 0x00 },
            new data_combi { BlockType = 0x00E9, MatchingBlockType = 0x00BA, ValidCombiCount = 0x0D, Padding = 0x00 },
            new data_combi { BlockType = 0x00EA, MatchingBlockType = 0x00BB, ValidCombiCount = 0x0D, Padding = 0x00 },
            new data_combi { BlockType = 0x00EB, MatchingBlockType = 0x00B1, ValidCombiCount = 0x0D, Padding = 0x00 },
            new data_combi { BlockType = 0x00B9, MatchingBlockType = 0x00B4, ValidCombiCount = 0x46, Padding = 0x00 },
            new data_combi { BlockType = 0x00C1, MatchingBlockType = 0x00C2, ValidCombiCount = 0x47, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x006A, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0061, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0062, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0063, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0064, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00AA, MatchingBlockType = 0x0065, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00D3, MatchingBlockType = 0x00B5, ValidCombiCount = 0x4A, Padding = 0x00 },
            new data_combi { BlockType = 0x00D9, MatchingBlockType = 0x00AC, ValidCombiCount = 0x4B, Padding = 0x00 },
            new data_combi { BlockType = 0x00F0, MatchingBlockType = 0x00BC, ValidCombiCount = 0x0B, Padding = 0x00 },
            new data_combi { BlockType = 0x00F1, MatchingBlockType = 0x00BD, ValidCombiCount = 0x0B, Padding = 0x00 },
            new data_combi { BlockType = 0x00C7, MatchingBlockType = 0x00C3, ValidCombiCount = 0x48, Padding = 0x00 },
            new data_combi { BlockType = 0x00CD, MatchingBlockType = 0x00BE, ValidCombiCount = 0x49, Padding = 0x00 },
            new data_combi { BlockType = 0x00FF, MatchingBlockType = 0x00CF, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x010A, MatchingBlockType = 0x00D0, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0103, MatchingBlockType = 0x0016, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0104, MatchingBlockType = 0x0016, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0106, MatchingBlockType = 0x00CB, ValidCombiCount = 0x4C, Padding = 0x00 },
            new data_combi { BlockType = 0x0107, MatchingBlockType = 0x00CB, ValidCombiCount = 0x4D, Padding = 0x00 },
            new data_combi { BlockType = 0x0108, MatchingBlockType = 0x00CB, ValidCombiCount = 0x4E, Padding = 0x00 },
            new data_combi { BlockType = 0x0109, MatchingBlockType = 0x00CB, ValidCombiCount = 0x4F, Padding = 0x00 },
            new data_combi { BlockType = 0x0012, MatchingBlockType = 0x00E8, ValidCombiCount = 0x36, Padding = 0x00 },
            new data_combi { BlockType = 0x004C, MatchingBlockType = 0x00CB, ValidCombiCount = 0x00, Padding = 0x00 },
            new data_combi { BlockType = 0x004D, MatchingBlockType = 0x00CB, ValidCombiCount = 0x01, Padding = 0x00 },
            new data_combi { BlockType = 0x004E, MatchingBlockType = 0x00CB, ValidCombiCount = 0x02, Padding = 0x00 },
            new data_combi { BlockType = 0x004F, MatchingBlockType = 0x00CB, ValidCombiCount = 0x3D, Padding = 0x00 },
            new data_combi { BlockType = 0x0052, MatchingBlockType = 0x00CB, ValidCombiCount = 0x09, Padding = 0x00 },
            new data_combi { BlockType = 0x0053, MatchingBlockType = 0x00CB, ValidCombiCount = 0x04, Padding = 0x00 },
            new data_combi { BlockType = 0x0054, MatchingBlockType = 0x00CB, ValidCombiCount = 0x3E, Padding = 0x00 },
            new data_combi { BlockType = 0x0057, MatchingBlockType = 0x00CB, ValidCombiCount = 0x0A, Padding = 0x00 },
            new data_combi { BlockType = 0x0058, MatchingBlockType = 0x00CB, ValidCombiCount = 0x05, Padding = 0x00 },
            new data_combi { BlockType = 0x0059, MatchingBlockType = 0x00CB, ValidCombiCount = 0x08, Padding = 0x00 },
            new data_combi { BlockType = 0x0064, MatchingBlockType = 0x0068, ValidCombiCount = 0x44, Padding = 0x00 },
            new data_combi { BlockType = 0x0065, MatchingBlockType = 0x00DC, ValidCombiCount = 0x44, Padding = 0x00 },
            new data_combi { BlockType = 0x0066, MatchingBlockType = 0x00E4, ValidCombiCount = 0x44, Padding = 0x00 },
            new data_combi { BlockType = 0x0067, MatchingBlockType = 0x0069, ValidCombiCount = 0x0E, Padding = 0x00 },
            new data_combi { BlockType = 0x0068, MatchingBlockType = 0x00E2, ValidCombiCount = 0x0E, Padding = 0x00 },
            new data_combi { BlockType = 0x0069, MatchingBlockType = 0x00E3, ValidCombiCount = 0x0E, Padding = 0x00 },
            new data_combi { BlockType = 0x006D, MatchingBlockType = 0x006A, ValidCombiCount = 0x42, Padding = 0x00 },
            new data_combi { BlockType = 0x006E, MatchingBlockType = 0x00E0, ValidCombiCount = 0x42, Padding = 0x00 },
            new data_combi { BlockType = 0x006F, MatchingBlockType = 0x00E1, ValidCombiCount = 0x42, Padding = 0x00 },
            new data_combi { BlockType = 0x00E5, MatchingBlockType = 0x00DD, ValidCombiCount = 0x43, Padding = 0x00 },
            new data_combi { BlockType = 0x00EC, MatchingBlockType = 0x00E5, ValidCombiCount = 0x41, Padding = 0x00 },
            new data_combi { BlockType = 0x00ED, MatchingBlockType = 0x00E6, ValidCombiCount = 0x41, Padding = 0x00 },
            new data_combi { BlockType = 0x00EE, MatchingBlockType = 0x00E7, ValidCombiCount = 0x41, Padding = 0x00 },
            new data_combi { BlockType = 0x00E6, MatchingBlockType = 0x00DE, ValidCombiCount = 0x43, Padding = 0x00 },
            new data_combi { BlockType = 0x00E7, MatchingBlockType = 0x00DF, ValidCombiCount = 0x43, Padding = 0x00 },
            new data_combi { BlockType = 0x005D, MatchingBlockType = 0x0002, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x0023, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0100, MatchingBlockType = 0x0025, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0101, MatchingBlockType = 0x0026, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0102, MatchingBlockType = 0x0024, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0000, MatchingBlockType = 0x00D1, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0078, MatchingBlockType = 0x0061, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x0079, MatchingBlockType = 0x0062, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x0078, MatchingBlockType = 0x0061, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0079, MatchingBlockType = 0x0062, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0082, MatchingBlockType = 0x0065, ValidCombiCount = 0x40, Padding = 0x00 },
            new data_combi { BlockType = 0x0050, MatchingBlockType = 0x00CB, ValidCombiCount = 0x50, Padding = 0x00 },
            new data_combi { BlockType = 0x0055, MatchingBlockType = 0x00CB, ValidCombiCount = 0x51, Padding = 0x00 },
            new data_combi { BlockType = 0x00F9, MatchingBlockType = 0x0151, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0083, MatchingBlockType = 0x0152, ValidCombiCount = 0x40, Padding = 0x00 },
            new data_combi { BlockType = 0x0084, MatchingBlockType = 0x0153, ValidCombiCount = 0x40, Padding = 0x00 },
            new data_combi { BlockType = 0x0085, MatchingBlockType = 0x0154, ValidCombiCount = 0x40, Padding = 0x00 },
            new data_combi { BlockType = 0x0086, MatchingBlockType = 0x0155, ValidCombiCount = 0x40, Padding = 0x00 },
            new data_combi { BlockType = 0x0087, MatchingBlockType = 0x0156, ValidCombiCount = 0x52, Padding = 0x00 },
            new data_combi { BlockType = 0x0088, MatchingBlockType = 0x0157, ValidCombiCount = 0x52, Padding = 0x00 },
            new data_combi { BlockType = 0x0089, MatchingBlockType = 0x0158, ValidCombiCount = 0x52, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x007A, MatchingBlockType = 0x0063, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x007B, MatchingBlockType = 0x0064, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x007C, MatchingBlockType = 0x0159, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x007D, MatchingBlockType = 0x015A, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x007E, MatchingBlockType = 0x015B, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x007F, MatchingBlockType = 0x015C, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x0080, MatchingBlockType = 0x015D, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x0081, MatchingBlockType = 0x015E, ValidCombiCount = 0x3F, Padding = 0x00 },
            new data_combi { BlockType = 0x001C, MatchingBlockType = 0x0160, ValidCombiCount = 0x37, Padding = 0x00 },
            new data_combi { BlockType = 0x0048, MatchingBlockType = 0x0162, ValidCombiCount = 0x26, Padding = 0x00 },
            new data_combi { BlockType = 0x004A, MatchingBlockType = 0x015F, ValidCombiCount = 0x3C, Padding = 0x00 },
            new data_combi { BlockType = 0x004B, MatchingBlockType = 0x0161, ValidCombiCount = 0x3C, Padding = 0x00 },
            new data_combi { BlockType = 0x00B1, MatchingBlockType = 0x0007, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x004B, MatchingBlockType = 0x0011, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0174, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0013, MatchingBlockType = 0x0175, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0030, MatchingBlockType = 0x0176, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0088, MatchingBlockType = 0x0177, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00CE, MatchingBlockType = 0x0178, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00BA, MatchingBlockType = 0x0179, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x000F, MatchingBlockType = 0x017A, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x007A, MatchingBlockType = 0x017B, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00AE, MatchingBlockType = 0x017C, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0030, MatchingBlockType = 0x017D, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0042, MatchingBlockType = 0x017E, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0008, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0116, MatchingBlockType = 0x017F, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0118, MatchingBlockType = 0x0180, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0117, MatchingBlockType = 0x0181, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x011A, MatchingBlockType = 0x0182, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0119, MatchingBlockType = 0x0183, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0184, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x005A, MatchingBlockType = 0x0185, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0006, MatchingBlockType = 0x0020, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x00AB, MatchingBlockType = 0x0021, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0122, MatchingBlockType = 0x0186, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x006A, MatchingBlockType = 0x0187, ValidCombiCount = 0x54, Padding = 0x00 },
            new data_combi { BlockType = 0x006B, MatchingBlockType = 0x0188, ValidCombiCount = 0x54, Padding = 0x00 },
            new data_combi { BlockType = 0x006C, MatchingBlockType = 0x0189, ValidCombiCount = 0x54, Padding = 0x00 },
            new data_combi { BlockType = 0x008A, MatchingBlockType = 0x018A, ValidCombiCount = 0x55, Padding = 0x00 },
            new data_combi { BlockType = 0x008B, MatchingBlockType = 0x018B, ValidCombiCount = 0x55, Padding = 0x00 },
            new data_combi { BlockType = 0x008C, MatchingBlockType = 0x018C, ValidCombiCount = 0x55, Padding = 0x00 },
            new data_combi { BlockType = 0x008D, MatchingBlockType = 0x018D, ValidCombiCount = 0x64, Padding = 0x00 },
            new data_combi { BlockType = 0x0090, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5E, Padding = 0x00 },
            new data_combi { BlockType = 0x0070, MatchingBlockType = 0x018F, ValidCombiCount = 0x63, Padding = 0x00 },
            new data_combi { BlockType = 0x0074, MatchingBlockType = 0x018E, ValidCombiCount = 0x62, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x00FE, MatchingBlockType = 0x00CB, ValidCombiCount = 0x53, Padding = 0x00 },
            new data_combi { BlockType = 0x0090, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5E, Padding = 0x00 },
            new data_combi { BlockType = 0x0090, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5E, Padding = 0x00 },
            new data_combi { BlockType = 0x0091, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5F, Padding = 0x00 },
            new data_combi { BlockType = 0x0091, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5F, Padding = 0x00 },
            new data_combi { BlockType = 0x0091, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5F, Padding = 0x00 },
            new data_combi { BlockType = 0x0091, MatchingBlockType = 0x00CB, ValidCombiCount = 0x5F, Padding = 0x00 },
            new data_combi { BlockType = 0x0092, MatchingBlockType = 0x00CB, ValidCombiCount = 0x60, Padding = 0x00 },
            new data_combi { BlockType = 0x0092, MatchingBlockType = 0x00CB, ValidCombiCount = 0x60, Padding = 0x00 },
            new data_combi { BlockType = 0x0092, MatchingBlockType = 0x00CB, ValidCombiCount = 0x60, Padding = 0x00 },
            new data_combi { BlockType = 0x0093, MatchingBlockType = 0x00CB, ValidCombiCount = 0x61, Padding = 0x00 },
            new data_combi { BlockType = 0x0093, MatchingBlockType = 0x00CB, ValidCombiCount = 0x61, Padding = 0x00 },
            new data_combi { BlockType = 0x0093, MatchingBlockType = 0x00CB, ValidCombiCount = 0x61, Padding = 0x00 },
            new data_combi { BlockType = 0x0093, MatchingBlockType = 0x00CB, ValidCombiCount = 0x61, Padding = 0x00 },
            new data_combi { BlockType = 0x0093, MatchingBlockType = 0x00CB, ValidCombiCount = 0x61, Padding = 0x00 },
            new data_combi { BlockType = 0x0051, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0056, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0090, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0091, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0092, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0093, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0094, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0095, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0096, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0097, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0098, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x0099, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x009C, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x009D, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x009E, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x009F, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A0, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A1, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A2, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A3, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A4, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A5, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A6, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A7, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x009A, MatchingBlockType = 0x00CB, ValidCombiCount = 0x66, Padding = 0x00 },
            new data_combi { BlockType = 0x009A, MatchingBlockType = 0x00CB, ValidCombiCount = 0x66, Padding = 0x00 },
            new data_combi { BlockType = 0x009B, MatchingBlockType = 0x00CB, ValidCombiCount = 0x67, Padding = 0x00 },
            new data_combi { BlockType = 0x009B, MatchingBlockType = 0x00CB, ValidCombiCount = 0x67, Padding = 0x00 },
            new data_combi { BlockType = 0x009B, MatchingBlockType = 0x00CB, ValidCombiCount = 0x67, Padding = 0x00 },
            new data_combi { BlockType = 0x009B, MatchingBlockType = 0x00CB, ValidCombiCount = 0x67, Padding = 0x00 },
            new data_combi { BlockType = 0x0123, MatchingBlockType = 0x0190, ValidCombiCount = 0xFF, Padding = 0x00 },
            new data_combi { BlockType = 0x0071, MatchingBlockType = 0x0192, ValidCombiCount = 0x63, Padding = 0x00 },
            new data_combi { BlockType = 0x0075, MatchingBlockType = 0x0191, ValidCombiCount = 0x62, Padding = 0x00 },
            new data_combi { BlockType = 0x0072, MatchingBlockType = 0x0194, ValidCombiCount = 0x63, Padding = 0x00 },
            new data_combi { BlockType = 0x0076, MatchingBlockType = 0x0193, ValidCombiCount = 0x62, Padding = 0x00 },
            new data_combi { BlockType = 0x0073, MatchingBlockType = 0x0196, ValidCombiCount = 0x63, Padding = 0x00 },
            new data_combi { BlockType = 0x0077, MatchingBlockType = 0x0195, ValidCombiCount = 0x62, Padding = 0x00 },
            new data_combi { BlockType = 0x008E, MatchingBlockType = 0x0197, ValidCombiCount = 0x64, Padding = 0x00 },
            new data_combi { BlockType = 0x008F, MatchingBlockType = 0x0198, ValidCombiCount = 0x64, Padding = 0x00 },
            new data_combi { BlockType = 0x00A8, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00A9, MatchingBlockType = 0x00CB, ValidCombiCount = 0x68, Padding = 0x00 },
            new data_combi { BlockType = 0x00F3, MatchingBlockType = 0x0199, ValidCombiCount = 0xFF, Padding = 0x00 }
        };

        // Generation Common Things
        private static readonly int[] blockGroup_428 = new int[16]
        {
            0x0F, 0x15, 0x28, 0x2E, 0x2F, 0x35, 0x36, 0x3C,
            0x16, 0x26, 0x16, 0x1C, 0x1D, 0x21, 0x22, 0x26
        };

        private static readonly int[] x_offset_409 = new int[4] { 0, -1, 0, 1 }; // Directions: North, West, South, East
        private static readonly int[] z_offset_410 = new int[4] { -1, 0, 1, 0 };

        private static readonly int[] system_block_info = new int[108]
        {
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000001,
            0x00000002, 0x00000004, 0x00000008, 0x00000010,
            0x00000020, 0x00000040, 0x00000001, 0x00000002,
            0x00000004, 0x00000008, 0x00000010, 0x00000020,
            0x00000040, 0x00000001, 0x00000002, 0x00000004,
            0x00000008, 0x00000010, 0x00000001, 0x00000008,
            0x00000010, 0x00000020, 0x00000040, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000001, 0x00000002,
            0x00000004, 0x00000008, 0x00000010, 0x00000020,
            0x00000040, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000001,
            0x00000004, 0x00000008, 0x00000008, 0x00000010,
            0x00000020, 0x00000040, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000001, 0x00000008, 0x00000010
        };

        private static readonly ushort[] exceptional_table = new ushort[54]
        {
            0x0050, 0x0051, 0x0055, 0x0056, 0x0078, 0x0090, 0x0079, 0x0091,
            0x007A, 0x0092, 0x007B, 0x0093, 0x007C, 0x0094, 0x007D, 0x0095,
            0x007E, 0x0096, 0x007F, 0x0097, 0x0080, 0x0098, 0x0081, 0x0099,
            0x0082, 0x009C, 0x0083, 0x009D, 0x0084, 0x009E, 0x0085, 0x009F,
            0x0086, 0x00A0, 0x0087, 0x00A1, 0x0088, 0x00A2, 0x0089, 0x00A3,
            0x008A, 0x00A4, 0x008B, 0x00A5, 0x008C, 0x00A6, 0x008D, 0x00A7,
            0x008E, 0x00A8, 0x008F, 0x00A9, 0x0125, 0x0125
        };

        // Two Layered Town Base Layout
        private static readonly byte[] DefaultTownStructure = new byte[70]
        {
        //   00    01    02    03    04    05    06
            0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, // 0
            0x09, 0x0C, 0x0C, 0x0B, 0x0C, 0x0C, 0x0A, // A
            0x02, 0x27, 0x27, 0x0E, 0x27, 0x27, 0x04, // B
            0x02, 0x27, 0x27, 0x27, 0x27, 0x27, 0x04, // C
            0x02, 0x27, 0x27, 0x27, 0x27, 0x27, 0x04, // D
            0x02, 0x27, 0x27, 0x27, 0x27, 0x27, 0x04, // E
            0x50, 0x27, 0x27, 0x27, 0x27, 0x27, 0x51, // F
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65, // G
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66, // H
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67  // I
        };

        // Two Layered Town Variables

        #region Cliff Variables
        // Cliff Variables
        private static readonly byte[] cliff_startA_factor = new byte[2] { 0x0F, 0x13 };
        private static readonly byte[] cliff_startB_factor = new byte[3] { 0x0F, 0x10, 0x13 };
        private static readonly byte[] cliff_startC_factor = new byte[2] { 0x0F, 0x10 };

        private static readonly byte[][] cliff_start_table = new byte[4][]
        {
            cliff_startA_factor,
            cliff_startA_factor,
            cliff_startB_factor,
            cliff_startC_factor
        };

        private static readonly byte[] cliff1_next = new byte[3] { 0x0F, 0x10, 0x13 };
        private static readonly byte[] cliff2_next = new byte[2] { 0x11, 0x12 };
        private static readonly byte[] cliff3_next = new byte[2] { 0x11, 0x12 };
        private static readonly byte[] cliff4_next = new byte[3] { 0x0F, 0x10, 0x13 };
        private static readonly byte[] cliff5_next = new byte[2] { 0x14, 0x15 };
        private static readonly byte[] cliff6_next = new byte[2] { 0x14, 0x15 };
        private static readonly byte[] cliff7_next = new byte[3] { 0x0F, 0x10, 0x13 };

        private static readonly byte[][] cliff_next_data = new byte[7][]
        {
            cliff1_next, cliff2_next, cliff3_next, cliff4_next,
            cliff5_next, cliff6_next, cliff7_next
        };

        private static readonly int[] cliff_info = new int[7] { 1, 2, 4, 8, 0x10, 0x20, 0x40 };

        /// <summary>
        /// The Direction of the next cliff section (0 = North, 1 = East? 2 = South, 3 = West?)
        /// </summary>
        private static readonly byte[] cliff_next_direct = new byte[8] // I think this is only 7 bytes long
        {
            3, 0, 0, 3, 2, 2, 3, 0
        };

        #endregion

        #region River Variables
        // River Variables
        private static readonly byte[] river1_album_data = new byte[7] { 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C };
        private static readonly byte[] river2_album_data = new byte[7] { 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0xFF, 0xFF };
        private static readonly byte[] river3_album_data = new byte[7] { 0x22, 0xFF, 0xFF, 0x23, 0x24, 0x25, 0x26 };
        private static readonly byte[] river_no_album_data = new byte[7] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        private static readonly byte[][] river_cliff_album_data = new byte[7][]
        {
            river1_album_data, river2_album_data, river3_album_data, river_no_album_data,
            river_no_album_data, river_no_album_data, river_no_album_data
        };

        /// <summary>
        /// Valid river start X-Acres
        /// </summary>
        private static readonly int[] startX_table = new int[4] { 1, 2, 4, 5 };

        /// <summary>
        /// River & Cliff Acres
        /// </summary>
        private static readonly byte[] cross_data = new byte[7] { 0x16, 0x17, 0x1A, 0x1E, 0x1F, 0x25, 0x26 };

        private static readonly byte[] river1_next = new byte[3] { 0x28, 0x2B, 0x2D };
        private static readonly byte[] river2_next = new byte[2] { 0x29, 0x2C };
        private static readonly byte[] river3_next = new byte[2] { 0x2A, 0x2E };
        private static readonly byte[] river4_next = new byte[2] { 0x29, 0x2C };
        private static readonly byte[] river5_next = new byte[3] { 0x28, 0x2B, 0x2D };
        private static readonly byte[] river6_next = new byte[2] { 0x2A, 0x2E };
        private static readonly byte[] river7_next = new byte[3] { 0x28, 0x2B, 0x2D };

        private static readonly byte[][] river_next_data = new byte[7][]
        {
            river1_next, river2_next, river3_next, river4_next,
            river5_next, river6_next, river7_next
        };

        /// <summary>
        /// The Direction of the next river section (0 = North, 1 = East? 2 = South, 3 = West?)
        /// </summary>
        private static readonly byte[] river_next_direct = new byte[7] { 2, 3, 1, 3, 2, 1, 2 };

        #endregion


        // Three Layered Town Layouts
        private static readonly byte[] step3_blocks3 = new byte[70]
        {
            0x05, 0x01, 0x00, 0x00, 0x00, 0x00, 0x08,
            0x09, 0x0D, 0x0C, 0x0B, 0x0C, 0x0C, 0x0A,
            0x02, 0x2B, 0x2C, 0x0E, 0x12, 0x0F, 0x3E,
            0x3D, 0x0F, 0x16, 0x0F, 0x10, 0x12, 0x3E,
            0x3D, 0x0F, 0x1A, 0x27, 0x27, 0x11, 0x04,
            0x02, 0x27, 0x1C, 0x0F, 0x0F, 0x10, 0x04,
            0x50, 0x27, 0x28, 0x27, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocks7 = new byte[70]
        {
            0x05, 0x01, 0x00, 0x00, 0x00, 0x00, 0x08,
            0x09, 0x0D, 0x0C, 0x0B, 0x0C, 0x0C, 0x0A,
            0x02, 0x2B, 0x2C, 0x0E, 0x27, 0x12, 0x3E,
            0x3D, 0x0F, 0x16, 0x0F, 0x0F, 0x10, 0x04,
            0x02, 0x27, 0x28, 0x12, 0x0F, 0x0F, 0x3E,
            0x3D, 0x0F, 0x16, 0x10, 0x27, 0x27, 0x04,
            0x50, 0x27, 0x28, 0x27, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocks7R = new byte[70]
        {
            0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x08,
            0x09, 0x0C, 0x0C, 0x0B, 0x0C, 0x0D, 0x0A,
            0x3D, 0x13, 0x27, 0x0E, 0x2E, 0x2D, 0x04,
            0x02, 0x15, 0x0F, 0x0F, 0x16, 0x0F, 0x3E,
            0x3D, 0x0F, 0x0F, 0x13, 0x28, 0x27, 0x04,
            0x02, 0x27, 0x27, 0x15, 0x16, 0x0F, 0x3E,
            0x50, 0x27, 0x27, 0x27, 0x28, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocks8 = new byte[70]
        {
            0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x08,
            0x09, 0x0C, 0x0C, 0x0B, 0x0C, 0x0D, 0x0A,
            0x3D, 0x0F, 0x13, 0x0E, 0x2E, 0x2D, 0x04,
            0x02, 0x27, 0x15, 0x0F, 0x1A, 0x12, 0x3E,
            0x3D, 0x0F, 0x0F, 0x13, 0x1C, 0x10, 0x04,
            0x02, 0x27, 0x27, 0x15, 0x16, 0x0F, 0x3E,
            0x50, 0x27, 0x27, 0x27, 0x28, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocksB = new byte[70]
        {
            0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x08,
            0x09, 0x0C, 0x0C, 0x0B, 0x0D, 0x0C, 0x0A,
            0x3D, 0x0F, 0x13, 0x0E, 0x28, 0x12, 0x3E,
            0x02, 0x27, 0x15, 0x0F, 0x16, 0x10, 0x04,
            0x3D, 0x0F, 0x13, 0x2E, 0x2D, 0x27, 0x04,
            0x02, 0x27, 0x15, 0x16, 0x0F, 0x0F, 0x3E,
            0x50, 0x27, 0x27, 0x28, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocksBR = new byte[70]
        {
            0x05, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08,
            0x09, 0x0C, 0x0D, 0x0B, 0x0C, 0x0C, 0x0A,
            0x3D, 0x13, 0x28, 0x0E, 0x12, 0x0F, 0x3E,
            0x02, 0x15, 0x16, 0x0F, 0x10, 0x27, 0x04,
            0x02, 0x27, 0x2B, 0x2C, 0x12, 0x0F, 0x3E,
            0x3D, 0x0F, 0x0F, 0x16, 0x10, 0x27, 0x04,
            0x50, 0x27, 0x27, 0x28, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocksE = new byte[70]
        {
            0x05, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08,
            0x09, 0x0C, 0x0D, 0x0B, 0x0C, 0x0C, 0x0A,
            0x3D, 0x13, 0x28, 0x0E, 0x12, 0x0F, 0x3E,
            0x02, 0x15, 0x16, 0x0F, 0x10, 0x27, 0x04,
            0x02, 0x27, 0x2B, 0x2C, 0x12, 0x0F, 0x3E,
            0x3D, 0x0F, 0x0F, 0x16, 0x10, 0x27, 0x04,
            0x50, 0x27, 0x27, 0x28, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocksER = new byte[70]
        {
            0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x08,
            0x09, 0x0C, 0x0C, 0x0B, 0x0D, 0x0C, 0x0A,
            0x3D, 0x0F, 0x13, 0x0E, 0x28, 0x12, 0x3E,
            0x02, 0x27, 0x15, 0x0F, 0x16, 0x10, 0x04,
            0x3D, 0x0F, 0x13, 0x2E, 0x2D, 0x27, 0x04,
            0x02, 0x27, 0x15, 0x16, 0x0F, 0x0F, 0x3E,
            0x50, 0x27, 0x27, 0x28, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocksF = new byte[70]
        {
            0x05, 0x00, 0x01, 0x00, 0x00, 0x00, 0x08,
            0x09, 0x0C, 0x0D, 0x0B, 0x0C, 0x0C, 0x0A,
            0x02, 0x27, 0x28, 0x0E, 0x12, 0x0F, 0x3E,
            0x3D, 0x0F, 0x16, 0x0F, 0x10, 0x27, 0x04,
            0x02, 0x27, 0x2B, 0x29, 0x2C, 0x27, 0x04,
            0x3D, 0x0F, 0x0F, 0x0F, 0x16, 0x0F, 0x3E,
            0x50, 0x27, 0x27, 0x27, 0x28, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[] step3_blocksFR = new byte[70]
        {
            0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x08,
            0x09, 0x0C, 0x0C, 0x0B, 0x0D, 0x0C, 0x0A,
            0x3D, 0x0F, 0x13, 0x0E, 0x28, 0x27, 0x04,
            0x02, 0x27, 0x15, 0x0F, 0x16, 0x0F, 0x3E,
            0x02, 0x27, 0x2E, 0x2A, 0x2D, 0x27, 0x04,
            0x3D, 0x0F, 0x16, 0x0F, 0x0F, 0x0F, 0x3E,
            0x50, 0x27, 0x28, 0x27, 0x27, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        // Unused 3-layer town layout found in DnM+. It was removed in the transition to Animal Crossing.
        private static readonly byte[] step3_blocksD = new byte[70]
        {
            0x05, 0x00, 0x00, 0x00, 0x00, 0x01, 0x08,
            0x09, 0x0c, 0x0c, 0x0b, 0x0c, 0x0d, 0x0a,
            0x3d, 0x0f, 0x13, 0x0e, 0x2e, 0x2d, 0x04,
            0x02, 0x27, 0x15, 0x0f, 0x16, 0x0f, 0x3e,
            0x3d, 0x0f, 0x0f, 0x0f, 0x1a, 0x27, 0x04,
            0x02, 0x27, 0x27, 0x27, 0x1c, 0x0f, 0x3e,
            0x50, 0x27, 0x27, 0x27, 0x28, 0x27, 0x51,
            0x65, 0x65, 0x65, 0x65, 0x65, 0x65, 0x65,
            0x53, 0x53, 0x53, 0x66, 0x62, 0x63, 0x66,
            0x53, 0x53, 0x53, 0x67, 0x67, 0x67, 0x67
        };

        private static readonly byte[][] step3_blockss = new byte[10][]
        {
            step3_blocks3, step3_blocks7, step3_blocks7R, step3_blocks8,
            step3_blocksB, step3_blocksBR, step3_blocksE, step3_blocksER,
            step3_blocksF, step3_blocksFR
        };

        private static readonly Dictionary<byte, ushort[]> TownAcrePool = new Dictionary<byte, ushort[]>
        {
            { 0x00, new ushort[] {0x0324} }, // Upper Border Cliff
            { 0x01, new ushort[] {0x0328} }, // Upper Border Cliff w/ River
            { 0x02, new ushort[] {0x032C} }, // Left Border Cliff
            // 0x03?
            { 0x04, new ushort[] {0x0338} }, // Right Border Cliff
            { 0x05, new ushort[] {0x0344} }, // Left Border Corner Cliff
            // 0x06?
            // 0x07?
            { 0x08, new ushort[] {0x0348} }, // Right Border Corner Cliff
            { 0x09, new ushort[] {0x0334} }, // Left Border Cliff w/ Tunnel
            { 0x0A, new ushort[] {0x0340} }, // Right Border Cliff w/ Tunnel
            { 0x0B, new ushort[] {0x0154, 0x02F0, 0x02F4} }, // Train Stations
            { 0x0C, new ushort[] {0x0118, 0x0294, 0x0298} }, // Dump Acres
            { 0x0D, new ushort[] {0x0070, 0x02B8, 0x02BC, 0x02C0, 0x02C4} }, // Rivers w/ Train Track
            { 0x0E, new ushort[] {0x0358, 0x035C, 0x0360} }, // Player House Acres
            { 0x0F, new ushort[] {0x009C, 0x015C, 0x0160, 0x0164, 0x0168} }, // Horizontal Cliffs
            { 0x10, new ushort[] {0x00A8, 0x016C, 0x01F4} }, // Left Corner Cliffs
            { 0x11, new ushort[] {0x00AC, 0x01A0, 0x01F0} }, // Left Side Cliffs
            { 0x12, new ushort[] {0x00B4, 0x01B0, 0x01EC} }, // Left Side Inverted Cliffs
            { 0x13, new ushort[] {0x00C0, 0x01B4, 0x01E8} }, // Right Side Inverted Cliffs
            { 0x14, new ushort[] {0x00CC, 0x01B8, 0x0218} }, // Right Side Cliffs
            { 0x15, new ushort[] {0x00D4, 0x01A4, 0x021C} }, // Right Side Corner Cliffs
            { 0x16, new ushort[] {0x0084, 0x0200, 0x0204} }, // South Flowing Waterfall w/ Horizontal Cliff
            { 0x17, new ushort[] {0x008C, 0x0210} }, // South Flowing Waterfall w/ Left Corner Cliff
            { 0x18, new ushort[] {0x00B0, 0x019C} }, // South Flowing River (Upper) w/ Left Cliff
            { 0x19, new ushort[] {0x00B8, 0x01C4} }, // South Flowing River (Upper) w/ Left Inverted Corner Cliff
            { 0x1A, new ushort[] {0x006C, 0x0214} }, // South Flowing Waterfall w/ Right Inverted Corner Cliff
            { 0x1B, new ushort[] {0x00D0, 0x0198} }, // South Flowing River (Lower) w/ Right Cliff
            { 0x1C, new ushort[] {0x0138, 0x01CC} }, // South Flowing River (Lower) w/ Right Corner Cliff
            { 0x1D, new ushort[] {0x00A0, 0x01A8, 0x0208} }, // East Flowing River (Upper) w/ Horizontal Cliff
            { 0x1E, new ushort[] {0x0090, 0x0244} }, // East Flowing Waterfall w/ Left Corner Cliff
            { 0x1F, new ushort[] {0x00F4, 0x0248} }, // East Flowing Waterfall w/ Left Cliff
            { 0x20, new ushort[] {0x00BC, 0x01C8} }, // East Flowing River (Upper) w/ Inverted Left Corner Cliff
            { 0x21, new ushort[] {0x00C4, 0x01D4} }, // East Flowing River (Upper) w/ Inverted Right Corner Cliff
            { 0x22, new ushort[] {0x00A4, 0x01AC, 0x020C} }, // West Flowing River (Upper) w/ Horizontal Cliff
            { 0x23, new ushort[] {0x013C, 0x01D8} }, // West Flowing River (Upper) w/ Inverted Left Corner Cliff
            { 0x24, new ushort[] {0x00C8, 0x01E4} }, // West Flowing River (Upper) w/ Inverted Right Corner Cliff
            { 0x25, new ushort[] {0x00FC} }, // West Flowing Waterfall w/ Right Cliff
            { 0x26, new ushort[] {0x00F8, 0x0414} }, // West Flowing Waterfall w/ Right Corner Cliff
            { 0x27, new ushort[] {0x0094, 0x0098, 0x0274, 0x0278, 0x027C, 0x0280, 0x0284, 0x0288, 0x028C, 0x0290} }, // Grass acres
            { 0x28, new ushort[] {0x00D8, 0x0170, 0x0174, 0x0220} }, // River south
            { 0x29, new ushort[] {0x00DC, 0x01BC, 0x01DC, 0x0224} }, // River east
            { 0x2A, new ushort[] {0x00E0, 0x01C0, 0x01E0, 0x0228} }, // River west
            { 0x2B, new ushort[] {0x00E4, 0x0178, 0x022C} }, // River south > east
            { 0x2C, new ushort[] {0x00E8, 0x017C, 0x0230} }, // River east > south
            { 0x2D, new ushort[] {0x00EC, 0x01D0, 0x0234} }, // River south > west
            { 0x2E, new ushort[] {0x00F0, 0x0180, 0x0184} }, // River west > south
            { 0x2F, new ushort[] {0x0100, 0x024C, 0x0268} }, // River south w/ bridge
            { 0x30, new ushort[] {0x0104, 0x0250, 0x026C} }, // River east w/ brdige
            { 0x31, new ushort[] {0x0108, 0x0258, 0x0270} }, // River west w/ bridge
            { 0x32, new ushort[] {0x010C, 0x0254} }, // River south > east w/ bridge
            { 0x33, new ushort[] {0x0060, 0x025C} }, // River east > south w/ bridge
            { 0x34, new ushort[] {0x0110, 0x0260} }, // River south > west w/ bridge
            { 0x35, new ushort[] {0x0114, 0x0264} }, // River west > south w/ bridge
            { 0x36, new ushort[] {0x0088, 0x011C, 0x018C, 0x0320} }, // Ramp south
            { 0x37, new ushort[] {0x0120, 0x0188, 0x0410} },
            { 0x38, new ushort[] {0x0124} },
            { 0x39, new ushort[] {0x0128, 0x0190} },
            { 0x3A, new ushort[] {0x012C, 0x0194} },
            { 0x3B, new ushort[] {0x0130} },
            { 0x3C, new ushort[] {0x0134, 0x0418, 0x041C} },
            { 0x3D, new ushort[] {0x0330} },
            { 0x3E, new ushort[] {0x033C} },
            { 0x3F, new ushort[] {0x03A0, 0x03A4, 0x03F0, 0x03F4, 0x03F8, 0x03FC, 0x0400, 0x0404, 0x0408, 0x040C} },
            { 0x40, new ushort[] {0x03B0, 0x03C0, 0x03C4, 0x03C8, 0x03CC} },
            { 0x41, new ushort[] {0x0374, 0x0378, 0x037C} }, // Nook's Acres
            { 0x42, new ushort[] {0x0364, 0x0368, 0x036C} },
            { 0x43, new ushort[] {0x0370, 0x0380, 0x0384} }, // Post Office Acres
            { 0x44, new ushort[] {0x034C, 0x0350, 0x0354} },
            { 0x45, new ushort[] {0x01FC} },
            { 0x46, new ushort[] {0x02C8} },
            { 0x47, new ushort[] {0x02CC} },
            { 0x48, new ushort[] {0x02F8} },
            { 0x49, new ushort[] {0x02FC} },
            { 0x4A, new ushort[] {0x02E8} },
            { 0x4B, new ushort[] {0x02EC} },
            { 0x4C, new ushort[] {0x0310} },
            { 0x4D, new ushort[] {0x0314} },
            { 0x4E, new ushort[] {0x0318} },
            { 0x4F, new ushort[] {0x031C} },
            { 0x50, new ushort[] {0x03B4} },
            { 0x51, new ushort[] {0x03B8} },
            { 0x52, new ushort[] {0x03D0, 0x03D4, 0x03D8} },
            { 0x53, new ushort[] {0x03DC, 0x03E0, 0x03E4, 0x03E8, 0x03EC, 0x04A8, 0x04AC, 0x04B0, 0x04B4, 0x04B8, 0x04BC, 0x04C0, 0x04C4, 0x04C8, 0x04CC, 0x04D0, 0x04D4, 0x04D8, 0x04DC} },
            { 0x54, new ushort[] {0x0480, 0x0484, 0x0488} },
            { 0x55, new ushort[] {0x048C, 0x0490, 0x0494} },
            // 0x56?
            // 0x57?
            // 0x58?
            // 0x59?
            // 0x5A?
            // 0x5B?
            // 0x5C?
            // 0x5D?
            { 0x5E, new ushort[] {0x049C, 0x04E0, 0x04E4} },
            { 0x5F, new ushort[] {0x04E8, 0x04EC, 0x04F0, 0x04F4} },
            { 0x60, new ushort[] {0x04F8, 0x04FC, 0x0500} },
            { 0x61, new ushort[] {0x0504, 0x0508, 0x050C, 0x0510, 0x0514} },
            { 0x62, new ushort[] {0x04A4, 0x0598, 0x05A0, 0x05A8} },
            { 0x63, new ushort[] {0x04A0, 0x0594, 0x059C, 0x05A4} },
            { 0x64, new ushort[] {0x0498, 0x05AC, 0x05B0} },
            // 0x65? these are handled slighly differently by the game.
            { 0x66, new ushort[] {0x0578, 0x057C} },
            { 0x67, new ushort[] {0x0580, 0x0584, 0x0588, 0x058C} },
            { 0x68, new ushort[] {0x0518, 0x051C, 0x0520, 0x0524, 0x0528, 0x052C, 0x0530, 0x0534, 0x0538, 0x053C, 0x0540, 0x0544, 0x0548, 0x054C, 0x0550, 0x0554, 0x0558, 0x055C, 0x0560, 0x0564, 0x0568, 0x056C, 0x0570, 0x0574, 0x05B4, 0x05B8} },
            // 69 - 6B
        };

        private int D2toD1(int AcreX, int AcreY)
        {
            return AcreY * 7 + AcreX;
        }

        private void D1toD2(int Index, out int X, out int Y)
        {
            X = Index % 7;
            Y = Index / 7;
        }

        private int GetXYCoordinateForBlockType(byte[] Data, int BlockType, out int X, out int Y)
        {
            int Index = -1;
            X = Y = -1;

            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] == BlockType)
                {
                    Index = i;
                    X = i % 7;
                    Y = i / 7;
                }
            }

            return Index;
        }

        /// <summary>
        /// Returns a randomly generated float between 0.0f and 1.0f. The inclusivity is [0.0f - 1.0f).
        /// </summary>
        /// <param name="seed">The optional seed used to generate the random number.</param>
        /// <returns>A random float [0.0f - 1.0f).</returns>
        private float fqrand(int? seed = null) // NOTE: seed isn't a parameter in the actual function.
        {
            var startSeed = seed ?? _randomSeed;
            startSeed *= 0x19660D;
            startSeed += 0x3C6EF35F;
            _randomSeed = startSeed;
            startSeed = 0x3F800000 | ((startSeed >> 9) & 0x7FFFFF);
            return BitConverter.ToSingle(BitConverter.GetBytes(startSeed), 0) - 1.0f;
        }

        /// <summary>
        /// Returns a random integer between [0 - <param name="maxValue"/>).
        /// </summary>
        /// <param name="maxValue">The exclusive upper limit of the random number to be generated.</param>
        /// <param name="seed">The optional seed parameter used to geenrate the random number.</param>
        /// <returns></returns>
        private int GetRandom(int maxValue, int? seed = null) // NOTE: seed isn't a parameter in the actual function.
            => (int)(fqrand(seed) * maxValue);

        /// <summary>
        /// Selects the "step" mode, or layer count, of your town. If 0, it's a 2 layered town. If 1, it's a 3 layered town.
        /// </summary>
        /// <returns>Step Mode</returns>
        private int GetRandomStepMode()
        {
            int RNG = GetRandom(100);
            int Temp = 0xF ^ RNG;
            int ShiftedValue = Temp >> 1;
            Temp &= 0xF;
            Temp = ShiftedValue - Temp;
            return (Temp >> 31) & 1;
        }

        /// <summary>
        /// Returns 0x1FF
        /// </summary>
        /// <returns>0x1FF</returns>
        private int MakePerfectBit()
        {
            int Perfect = 0;
            for (int i = 0; i < 9; i++)
                Perfect |= 1 << i;
            return Perfect;
        }

        /// <summary>
        /// Takes the current X & Y acre and a direction, and returns the next X & Y acre for that direction
        /// </summary>
        /// <param name="X">The next X acre</param>
        /// <param name="Y">The next Y acre</param>
        /// <param name="AcreX">The current X acre</param>
        /// <param name="AcreY">The current Y acre</param>
        /// <param name="Direction">The direction of the current feature</param>
        private void Direct2BlockNo(out int X, out int Y, int AcreX, int AcreY, int Direction)
        {
            X = AcreX + x_offset_409[Direction & 3];
            Y = AcreY + z_offset_410[Direction & 3];
        }

        private bool CheckBlockGroup(int BlockType, int AcreTypeSet)
        {
            if (AcreTypeSet != 8)
            {
                int MinType = blockGroup_428[AcreTypeSet * 2];
                int MaxType = blockGroup_428[AcreTypeSet * 2 + 1];

                return BlockType >= MinType && BlockType <= MaxType;
            }
            else
            {
                if (BlockType >= blockGroup_428[0] && BlockType <= blockGroup_428[1])
                {
                    return true;
                }
                else if (BlockType >= blockGroup_428[6] && BlockType <= blockGroup_428[7])
                {
                    return true;
                }
                else if (BlockType >= blockGroup_428[8] && BlockType <= blockGroup_428[9])
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks that both acre X & Y are within their valid parameters.
        /// </summary>
        /// <param name="AcreX">X-Acre</param>
        /// <param name="AcreY">Y-Acre</param>
        /// <param name="AcreXMin">Minimum X-Acre value</param>
        /// <param name="AcreXMax">Maximum X-Acre value</param>
        /// <param name="AcreYMin">Minimum Y-Acre value</param>
        /// <param name="AcreYMax">Maximum Y-Acre value</param>
        /// <returns>bool AcresAreValid</returns>
        private bool CheckCorrectBlockNo(int AcreX, int AcreY, int AcreXMin, int AcreXMax, int AcreYMin, int AcreYMax)
            => (AcreX >= AcreXMin && AcreX <= AcreXMax && AcreY >= AcreYMin && AcreY <= AcreYMax);

        private int GetSystemBlockInfo(int BlockType)
            => system_block_info[BlockType & 0xFF];

        // Cliff code
        private bool DecideBaseCliff(ref byte[] AcreData)
        {
            int CliffStartTableIndex = GetRandom(4);
            byte[] CliffStartTable = cliff_start_table[CliffStartTableIndex];
            byte CliffStartAcreType = CliffStartTable[GetRandom(CliffStartTable.Length)];

            // Set the first in-town cliff acre
            int CliffStartAcre = D2toD1(1, CliffStartTableIndex + 2);
            AcreData[CliffStartAcre] = CliffStartAcreType;

            // Set the border acre
            int CliffBorderStartAcre = D2toD1(0, CliffStartTableIndex + 2);
            AcreData[CliffBorderStartAcre] = 0x3D;

            // Trace Cliff
            TraceCliffBlock(ref AcreData, 1, CliffStartTableIndex + 2);

            // Set Cliff End Acre
            SetEndCliffBlock(ref AcreData);

            // Check Cliff is valid
            return LastCheckCliff(AcreData, 1, CliffStartTableIndex + 2);
        }

        private bool TraceCliffBlock(ref byte[] AcreData, int AcreX, int AcreY)
        {
            byte CurrentCliffAcreType = AcreData[D2toD1(AcreX, AcreY)];
            int TraceState = 0;
            byte CliffSubtractionValue = (byte)(CurrentCliffAcreType - 0xF);
            int CliffSubtractionValueShifted = CliffSubtractionValue << 2;
            int X = 0, Y = 0;

            while (TraceState == 0)
            {
                byte[] CliffNext = cliff_next_data[CliffSubtractionValue];
                byte CliffAcreType = CliffNext[GetRandom(CliffNext.Length)];
                byte CliffAdjustValue = cliff_next_direct[CliffSubtractionValue];

                Direct2BlockNo(out X, out Y, AcreX, AcreY, CliffAdjustValue);

                CliffAdjustValue = cliff_next_direct[(byte)(CliffAcreType - 0xF)];
                Direct2BlockNo(out int X2, out int Y2, X, Y, CliffAdjustValue);

                bool BlockCheck1 = CheckCorrectBlockNo(X, Y, 1, 5, 2, 5);
                bool BlockCheck2 = CheckCorrectBlockNo(X2, Y2, 1, 6, 2, 5);
                if (BlockCheck1 && BlockCheck2)
                {
                    int CliffReplaceAcre1 = D2toD1(X2, Y2);
                    if (AcreData[CliffReplaceAcre1] == 0xE)
                    {
                        Console.WriteLine("\nHouse acre is the next acre! Cannot continue.");
                        return false;
                    }

                    int CliffReplaceAcre2 = D2toD1(X, Y);
                    if (AcreData[CliffReplaceAcre2] != 0x27)
                    {
                        Console.WriteLine("\nCurrently selected acre isn't a grass block! Cannot continue.");
                        return false;
                    }

                    AcreData[CliffReplaceAcre2] = CliffAcreType;
                    if (X == 5)
                    {
                        TraceState = 2;
                    }
                    else
                    {
                        TraceState = 1;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("\nBlock number was incorrect! Cannot continue. Block Values: X1: {2} Y1: {3} Check1: {0}\n X2: {4} Y2: {5} Check2: {1}",
                        BlockCheck1.ToString(), BlockCheck2.ToString(), X, Y, X2, Y2));
                    return false;
                }
            }

            if (TraceState == 1)
            {
                return TraceCliffBlock(ref AcreData, X, Y);
            }
            else
            {
                return TraceState == 2;
            }
        }

        private void SetEndCliffBlock(ref byte[] AcreData) // This doesn't always work. Double check it at some point.
        {
            int AcreY = 0;
            int DirectType = 0;

            for (int Y = 0; Y < 6; Y++)
            {
                byte AcreType = AcreData[D2toD1(5, Y)];
                if (AcreType >= 0xF && AcreType <= 0x15)
                {
                    AcreY = Y;
                    DirectType = (byte)(AcreType - 0xF);
                }
            }

            byte CliffNextDirectValue = cliff_next_direct[DirectType];
            if (CliffNextDirectValue == 3) // Cliff is going west, end immediately
            {
                AcreData[D2toD1(6, AcreY)] = 0x3E;
            }
            else
            {
                byte[] CliffNextSet = cliff_next_data[DirectType];
                if (CliffNextSet.Length > 0)
                {
                    for (int i = 0; i < CliffNextSet.Length; i++)
                    {
                        byte CliffDirectValue = cliff_next_direct[CliffNextSet[i] - 0xF];
                        if (CliffDirectValue == 3)
                        {
                            Direct2BlockNo(out int X2, out int Y2, 5, AcreY, CliffNextDirectValue);
                            AcreData[D2toD1(X2, Y2)] = CliffNextSet[i];
                            AcreData[D2toD1(X2 + 1, Y2)] = 0x3E;
                        }
                    }
                }
            }
        }

        private bool LastCheckCliff(byte[] AcreData, int AcreX, int AcreY)
        {
            byte CliffAcreType = AcreData[D2toD1(AcreX, AcreY)];
            int Y = AcreY;
            while (CheckBlockGroup(CliffAcreType, 0) == true)
            {
                byte CliffDirectValue = cliff_next_direct[CliffAcreType - 0xF];
                Direct2BlockNo(out AcreX, out AcreY, AcreX, AcreY, CliffDirectValue);
                CliffAcreType = AcreData[D2toD1(AcreX, AcreY)];
            }

            return AcreX > 5 && AcreY != Y; // Might be AcreY >= Y or AcreY == Y
        }

        // River Generation Code

        /// <summary>
        /// Gets the direction of the next river section.
        /// </summary>
        /// <param name="RiverIdx">The current river block index</param>
        /// <returns>The direction of the next river section</returns>
        private int RiverIdx2NextDirect(int RiverIdx)
        {
            if (RiverIdx > -1 && RiverIdx < 7)
            {
                return river_next_direct[RiverIdx];
            }
            return 2; // Return south by default
        }

        private byte RiverAlbuminCliff(byte BlockType, byte AlbuminType)
        {
            if (CheckBlockGroup(BlockType, 0) && CheckBlockGroup(AlbuminType, 1))
            {
                sbyte AdjustType = (sbyte)(BlockType - 0xF);
                sbyte AlbuminAdjustType = (sbyte)(AlbuminType - 0x28);

                if (AlbuminAdjustType < 7 && AdjustType > -1 && AdjustType < 7)
                {
                    return river_cliff_album_data[AlbuminAdjustType][AdjustType];
                }
            }
            return 0xFF;
        }

        /// <summary>
        /// Checks if the river crosses the centerpoint of the map.
        /// </summary>
        /// <param name="AcreData">The current acre data</param>
        /// <returns>int NumberOfTimesCrossed</returns>
        private int GetCenterCrossZoneRiverCount(byte[] AcreData)
        {
            int AcreY = 2;
            int CrossZoneCount = 0;
            while (AcreY <= 5)
            {
                if (CheckBlockGroup(AcreData[D2toD1(3, AcreY)], 1) == true)
                {
                    CrossZoneCount++;
                }
                AcreY++;
            }
            return CrossZoneCount;
        }

        private bool TraceRiverPart1(ref byte[] AcreData, out int AcreX, out int AcreY)
        {
            int RiverTraceState = 0;
            AcreX = 0;
            AcreY = 0;
            while (RiverTraceState == 0)
            {
                int RiverXStartAcre = startX_table[GetRandom(4)];
                byte[] RiverStartData = river_next_data[0];
                byte RiverStartType = RiverStartData[GetRandom(RiverStartData.Length)];

                Direct2BlockNo(out int X, out int Y, RiverXStartAcre, 1, 2);
                int RiverAbsoluteStartAcre = D2toD1(X, Y);
                int NextRiverSectionDirection = RiverIdx2NextDirect((byte)(RiverStartType - 0x28));

                Direct2BlockNo(out int X2, out int Y2, X, Y, NextRiverSectionDirection);
                int NextRiverAbsoluteAcre = D2toD1(X2, Y2);

                if (CheckCorrectBlockNo(X, Y, 1, 5, 1, 6) && AcreData[RiverAbsoluteStartAcre] != 0xE)
                {
                    if (CheckBlockGroup(AcreData[RiverAbsoluteStartAcre], 0))
                    {
                        byte RiverAlbum = RiverAlbuminCliff(AcreData[RiverAbsoluteStartAcre], 0x28);
                        if (RiverAlbum == 0xFF)
                        {
                            Console.WriteLine("River Album in Cliff was invalid!");
                            return false;
                        }
                        else
                        {
                            int RiverAlbumAbsoluteAcre = D2toD1(X, Y + 1);
                            if (AcreData[RiverAlbumAbsoluteAcre] != 0xE)
                            {
                                RiverTraceState = 1;
                                AcreData[RiverAbsoluteStartAcre] = 0x28;
                                AcreX = X;
                                AcreY = Y;
                            }
                        }
                    }
                    else if (AcreData[NextRiverAbsoluteAcre] != 0xE)
                    {
                        AcreData[RiverAbsoluteStartAcre] = RiverStartType;
                        RiverTraceState = 1;
                        AcreX = X;
                        AcreY = Y;
                    }
                }

                if (RiverTraceState == 1)
                {
                    // Set River Border Acre & River Train Track Bridge Acre
                    int RiverBorderAcre = D2toD1(RiverXStartAcre, 0);
                    int RiverTrainTrackAcre = D2toD1(RiverXStartAcre, 1);
                    AcreData[RiverBorderAcre] = 1;
                    AcreData[RiverTrainTrackAcre] = 0xD;
                }
            }
            return true;
        }

        private bool TraceRiverPart2(ref byte[] AcreData, ref byte[] UnchangedAcreData, int AcreX, int AcreY, byte[] challenge_flag)
        {
            int RiverStartAcre = D2toD1(AcreX, AcreY);
            byte RiverStartAcreType = AcreData[RiverStartAcre];
            int RiverDirection = RiverIdx2NextDirect((byte)(RiverStartAcreType - 0x28));
            int X = 0, Y = 0;

            int RiverTraceState = 0;
            while (RiverTraceState == 0)
            {
                byte[] river_next_set = river_next_data[RiverStartAcreType - 0x28];
                byte NextRiverType = river_next_set[GetRandom(river_next_set.Length)];
                int NextRiverDirection = RiverIdx2NextDirect((byte)(NextRiverType - 0x28));

                Direct2BlockNo(out X, out Y, AcreX, AcreY, RiverDirection);
                if (Y == 6)
                {
                    NextRiverType = 0x28;
                    NextRiverDirection = RiverIdx2NextDirect(0);
                }

                Direct2BlockNo(out int X2, out int Y2, X, Y, NextRiverDirection);
                if (CheckCorrectBlockNo(X, Y, 1, 5, 1, 6))
                {
                    if (CheckCorrectBlockNo(X2, Y2, 1, 5, 1, 7))
                    {
                        int RiverPlacementAcre = D2toD1(X, Y);
                        int NextRiverPlacementAcre = D2toD1(X2, Y2);
                        if (UnchangedAcreData[NextRiverPlacementAcre] != 0xE)
                        {
                            bool PlacedBlock = false;
                            if (CheckBlockGroup(UnchangedAcreData[RiverPlacementAcre], 0))
                            {
                                byte RiverAlbum = RiverAlbuminCliff(UnchangedAcreData[RiverPlacementAcre], river_next_set[0]); // Check river_next_set
                                if (RiverAlbum != 0xFF)
                                {
                                    AcreData[RiverPlacementAcre] = river_next_set[0];
                                    PlacedBlock = true;
                                }
                                else
                                {
                                    Console.WriteLine("River Album was invalid!");
                                    return false;
                                }
                            }
                            else
                            {
                                AcreData[RiverPlacementAcre] = NextRiverType;
                                PlacedBlock = true;
                            }

                            if (PlacedBlock)
                            {
                                if (Y2 == 7)
                                {
                                    RiverTraceState = 2;
                                }
                                else
                                {
                                    RiverTraceState = 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("River destination block was invalid!");
                    return false;
                }
            }

            if (RiverTraceState == 1)
            {
                return TraceRiverPart2(ref AcreData, ref UnchangedAcreData, X, Y, challenge_flag);
            }
            else if(RiverTraceState == 2)
            {

                if (GetCenterCrossZoneRiverCount(AcreData) != 0)
                {
                    if (X == 1 || X == 5)
                    {
                        Console.WriteLine("River X value is invalid: " + X);
                    }
                    return X != 1 && X != 5;    
                }
                else
                {
                    Console.WriteLine("CrossZoneRiver Count was zero!");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("RiverTraceState wasn't valid!");
                return false;
            }
        }

        private bool LastCheckRiver(byte[] AcreData, int AcreX, int AcreY)
        {
            byte CurrentAcreType = AcreData[D2toD1(AcreX, AcreY)];
            while (CheckBlockGroup(CurrentAcreType, 1) == true)
            {
                int NextRiverDirection = RiverIdx2NextDirect(CurrentAcreType - 0x28);
                Direct2BlockNo(out int X, out int Y, AcreX, AcreY, NextRiverDirection);
                CurrentAcreType = AcreData[D2toD1(X, Y)];
                AcreX = X;
                AcreY = Y;
            }

            int Valid = AcreY ^ 6;
            Valid = (Valid >> 1) - (Valid & AcreY);
            return ((Valid >> 31) & 1) == 1; // This can be simplified as AcreY >= 7. I'm sticking to the code, though.
        }

        private bool DecideRiverAlbuminCliff(ref byte[] CliffData, ref byte[] RiverCliffData)
        {
            for (int Y = 0; Y < 8; Y++)
            {
                for (int X = 0; X < 7; X++)
                {
                    int AbsoluteAcre = D2toD1(X, Y);
                    byte RiverBlock = RiverCliffData[AbsoluteAcre];
                    byte SelectedAcreType = RiverAlbuminCliff(CliffData[AbsoluteAcre], RiverBlock);

                    if (SelectedAcreType == 0xFF)
                    {
                        // Check if the current block is a river block. If so, copy it to the "main" block data.
                        if (CheckBlockGroup(RiverBlock, 1) == true || RiverBlock == 1 || RiverBlock == 0xD)
                        {
                            CliffData[AbsoluteAcre] = RiverCliffData[AbsoluteAcre];
                        }
                    }
                    else
                    {
                        CliffData[AbsoluteAcre] = SelectedAcreType; // Set the block to a waterfall type.
                    }
                }
            }
            return true;
        }

        private bool DecideBaseRiver(ref byte[] AcreData, out byte[] RiverData)
        {
            byte[] UnchangedAcreData = new byte[AcreData.Length];
            Array.Copy(AcreData, UnchangedAcreData, AcreData.Length);
            RiverData = AcreData;
            AcreData = UnchangedAcreData;
            if (TraceRiverPart1(ref RiverData, out int AcreX, out int AcreY))
            {
                if (TraceRiverPart2(ref RiverData, ref AcreData, AcreX, AcreY, new byte[0x38]))
                {
                    return LastCheckRiver(RiverData, AcreX, AcreY);
                }
            }
            return false;
        }

        private bool SetRandomBlockData(ref byte[] AcreData, out byte[] RiverData) // Technically takes two copies of AcreData
        {
            RiverData = null;
            if (DecideBaseCliff(ref AcreData))
            {
                return DecideBaseRiver(ref AcreData, out RiverData);
            }
            return false;
        }

        private byte[] MakeBaseLandformStep2()
        {
            byte[] AcreData = new byte[70];
            byte[] RiverData = null;
            Array.Copy(DefaultTownStructure, AcreData, 70);
            while (SetRandomBlockData(ref AcreData, out RiverData) == false)
            {
                Array.Copy(DefaultTownStructure, AcreData, 70);
            }

            DecideRiverAlbuminCliff(ref AcreData, ref RiverData);
            return AcreData;
        }

        private byte[] MakeBaseLandformStep3()
        {
            return step3_blockss[GetRandom(10)].Copy();
        }

        private byte[] MakeBaseLandform(int StepMode)
        {
            if (StepMode == 1)
            {
                return MakeBaseLandformStep3();
            }
            else
            {
                return MakeBaseLandformStep2();
            }
        }

        // Grass Blocks
        /// <summary>
        /// Creates a map of North & South acres for cliffs, & East & West acres for rivers.
        /// </summary>
        /// <param name="AcreData">Current accre data</param>
        /// <param name="river_left_right_info">River right & left acre map</param>
        /// <param name="cliff_up_down_info">Cliff up & down acre map</param>
        private void MakeFlatPlaceInformation(byte[] AcreData, out ushort[] river_left_right_info, out ushort[] cliff_up_down_info)
        {
            river_left_right_info = new ushort[70];
            cliff_up_down_info = new ushort[70];

            for (int i = 0; i < 70; i++)
            {
                river_left_right_info[i] = 2;
                cliff_up_down_info[i] = 2;
            }

            // Cliff Check
            for (int X = 1; X < 6; X++)
            {
                ushort StoreValue = 0;
                for (int Y = 1; Y < 9; Y++)
                {
                    if (StoreValue == 0 && CheckBlockGroup(AcreData[D2toD1(X, Y)], 8) == true)
                    {
                        StoreValue = 1;
                    }
                    cliff_up_down_info[D2toD1(X, Y)] = StoreValue;
                }
            }

            // River Check
            for (int Y = 1; Y < 9; Y++)
            {
                ushort StoreValue = 0;
                for (int X = 1; X < 6; X++)
                {
                    if (StoreValue == 0 && ((CheckBlockGroup(AcreData[D2toD1(X, Y)], 1) == true || CheckBlockGroup(AcreData[D2toD1(X, Y)], 4) == true)))
                    {
                        StoreValue = 1;
                    }
                    river_left_right_info[D2toD1(X, Y)] = StoreValue;
                }
            }
        }

        // Oceanfront Blocks
        private void SetMarinBlock(ref byte[] AcreData)
        {
            for (int X = 1; X < 6; X++)
            {
                int AcreIndex = D2toD1(X, 6);
                if (AcreData[AcreIndex] == 0x27)
                {
                    AcreData[AcreIndex] = 0x3F;
                }
                else if (AcreData[AcreIndex] == 0x28)
                {
                    AcreData[AcreIndex] = 0x40;
                }
            }

            // Set border blocks
            AcreData[D2toD1(0, 6)] = 0x50;
            AcreData[D2toD1(6, 6)] = 0x51;
        }

        // Bridges & Slopes
        /// <summary>
        /// Counts the number of times the river & cliffs cross paths.
        /// </summary>
        /// <param name="AcreX">The X-Acre of the first cliff-river crossing.</param>
        /// <param name="AcreY">The Y-Acre of the first cliff-river crossing.</param>
        /// <param name="AcreData">The current acre data.</param>
        /// <returns>The amount of times the river & cliffs cross.</returns>
        public int GetRiverCrossCliffInfo(out int AcreX, out int AcreY, byte[] AcreData)
        {
            int AcreIdx = 0;
            int Count = 0;

            AcreX = AcreY = 0;

            for (int i = 0; i < 0x38; i++)
            {
                int Y = i / 7;
                int X = i % 7;

                for (int x = 0; x < 7; x++)
                {
                    if (AcreData[AcreIdx] == cross_data[x])
                    {
                        if (Count == 0)
                        {
                            AcreX = X;
                            AcreY = Y;
                        }
                        Count++;
                    }
                }

                AcreIdx++;
            }

            return Count;
        }

        private int SetBridgeBlock(ref byte[] AcreData, bool ThreeLayeredTown)
        {
            bool PlaceUpperBridge = (GetRandom(10) & 1) == 1;
            GetRiverCrossCliffInfo(out int AcreX, out int AcreY, AcreData);
            int AcreIdx = 0;
            int ValidBridgePlaceUpper = 0;
            int ValidBridgePlaceLower = 0;
            int SetBridgeBlockBit = 0;

            for (int Y = 0; Y < 8; Y++)
            {
                for (int X = 0; X < 7; X++)
                {
                    if (CheckBlockGroup(AcreData[AcreIdx], 1) == true)
                    {
                        if (Y >= AcreY)
                        {
                            ValidBridgePlaceUpper++;
                        }
                        else
                        {
                            ValidBridgePlaceLower++;
                        }
                    }
                    AcreIdx++;
                }
            }

            // Lower area first
            if (ValidBridgePlaceLower != 0)
            {
                int BridgeLowerLocation = GetRandom(ValidBridgePlaceLower);
                AcreIdx = 0;
                int Count = 0;

                for (int Y = 0; Y < 8; Y++)
                {
                    for (int X = 0; X < 7; X++)
                    {
                        if (CheckBlockGroup(AcreData[AcreIdx], 1) == true)
                        {
                            if (Y < AcreY)
                            {
                                if (Count == BridgeLowerLocation)
                                {
                                    SetBridgeBlockBit |= (1 << 2); // 4
                                    AcreData[AcreIdx] = (byte)(AcreData[AcreIdx] + 7);
                                }
                                Count++;
                            }
                        }
                        AcreIdx++;
                    }
                }
            }

            // Upper area next
            if (ValidBridgePlaceUpper != 0 && ThreeLayeredTown == false && PlaceUpperBridge == true)
            {
                int BridgeUpperLocation = GetRandom(ValidBridgePlaceUpper);
                AcreIdx = 0;
                int Count = 0;

                for (int Y = 0; Y < 8; Y++)
                {
                    for (int X = 0; X < 7; X++)
                    {
                        if (CheckBlockGroup(AcreData[AcreIdx], 1) == true)
                        {
                            if (Y > AcreY)
                            {
                                if (Count == BridgeUpperLocation)
                                {
                                    SetBridgeBlockBit |= (1 << 3); // 8
                                    AcreData[AcreIdx] = (byte)(AcreData[AcreIdx] + 7);
                                }
                                Count++;
                            }
                        }
                        AcreIdx++;
                    }
                }
            }

            return SetBridgeBlockBit;
        }

        // Slope Code
        private int BlockType2CliffIndex(int CliffIdx)
        {
            int CliffBlockInfo = GetSystemBlockInfo(CliffIdx);
            for (int i = 0; i < 7; i++)
            {
                if ((CliffBlockInfo & cliff_info[i]) == cliff_info[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private int CountDirectedInfoCliff(byte[] AcreData, int AcreX, int AcreY, int DesiredRiverSide)
        {
            AcreX += 1;
            byte CurrentBlockType = AcreData[D2toD1(AcreX, AcreY)];
            int RiverSide = 0;
            int ValidCliffBlocks = 0;

            while (CurrentBlockType != 0x3E)
            {
                int BlockCliffIndex = BlockType2CliffIndex(CurrentBlockType);
                if (CheckBlockGroup(CurrentBlockType, 4) == true) // Check if the current block is a waterfall acre
                {
                    RiverSide = 1;
                }
                else
                {
                    if (DesiredRiverSide == RiverSide)
                    {
                        ValidCliffBlocks++;
                    }
                }

                byte CliffDirection = cliff_next_direct[BlockCliffIndex];
                Direct2BlockNo(out int X, out int Y, AcreX, AcreY, CliffDirection);
                CurrentBlockType = AcreData[D2toD1(X, Y)];
                AcreX = X;
                AcreY = Y;
            }

            return ValidCliffBlocks;
        }

        private bool SetSlopeDirectedInfoCliff(ref byte[] AcreData, int AcreX, int AcreY, int ValidBlockType, int WriteIndex)
        {
            AcreX += 1;
            byte CurrentBlockType = AcreData[D2toD1(AcreX, AcreY)];
            int Unknown = 0;
            int Count = 0;

            while (CurrentBlockType != 0x3E)
            {
                int BlockCliffIndex = BlockType2CliffIndex(CurrentBlockType);
                if (CheckBlockGroup(CurrentBlockType, 4) == true)
                {
                    Unknown = 1;
                }
                else
                {
                    if (ValidBlockType == Unknown)
                    {
                        if (Count == WriteIndex)
                        {
                            AcreData[D2toD1(AcreX, AcreY)] = (byte)(BlockCliffIndex + 0x36);
                            return true;
                        }
                        else
                        {
                            Count++;
                        }
                    }
                }

                byte CliffDirection = cliff_next_direct[BlockCliffIndex];
                Direct2BlockNo(out int X, out int Y, AcreX, AcreY, CliffDirection);
                CurrentBlockType = AcreData[D2toD1(X, Y)];
                AcreX = X;
                AcreY = Y;
            }

            return false;
        }

        private int SetSlopeBlock(ref byte[] AcreData)
        {
            int SlopeBit = 0;
            for (int Y = 0; Y < 8; Y++)
            {
                byte BlockType = AcreData[D2toD1(0, Y)];
                if (BlockType == 0x3D)
                {
                    int Count = CountDirectedInfoCliff(AcreData, 0, Y, 0);
                    if (Count > 0)
                    {
                        int SlopeIndex = GetRandom(Count);
                        if (SetSlopeDirectedInfoCliff(ref AcreData, 0, Y, 0, SlopeIndex))
                        {
                            SlopeBit |= (1 << 0); // 1
                        }
                    }

                    Count = CountDirectedInfoCliff(AcreData, 0, Y, 1);
                    if (Count > 0)
                    {
                        int SlopeIndex = GetRandom(Count);
                        if (SetSlopeDirectedInfoCliff(ref AcreData, 0, Y, 1, SlopeIndex))
                        {
                            SlopeBit |= (1 << 1); // 2
                        }
                    }
                }
            }

            return SlopeBit;
        }

        private int SetBridgeAndSlopeBlock(ref byte[] AcreData, bool IsThreeLayeredTown)
        {
            return SetBridgeBlock(ref AcreData, IsThreeLayeredTown) | SetSlopeBlock(ref AcreData);
        }

        private int SetNeedleworkAndWharfBlock(ref byte[] AcreData)
        {
            int WorkBit = 0;
            int CurrentNeedleworkCheckIdx = 0;
            int NeedleworkXAcre = GetRandom(3);
            int WharfBlockIdx = D2toD1(5, 6);
            if (AcreData[WharfBlockIdx] == 0x3F)
            {
                AcreData[WharfBlockIdx] = 0x64;
                for (int X = 1; X < 6; X++)
                {
                    if (AcreData[D2toD1(X, 6)] == 0x3F && NeedleworkXAcre == CurrentNeedleworkCheckIdx)
                    {
                        AcreData[D2toD1(X, 6)] = 0x55;
                        WorkBit |= (1 << 8); // 0x100
                    }
                    CurrentNeedleworkCheckIdx++;
                }
            }

            return WorkBit;
        }

        // Museum, Wishing Well, & Police Station Code

        // Man the devs really really made this terrible
        private bool JudgeFlatBlock(byte[] AcreData, int Index, RiverSide RiverDirection, CliffSide CliffDirection, ushort[] cliff_up_down_info, ushort[] river_left_right_info)
        {
            if ((int)RiverDirection > -1 && (int)RiverDirection < 3 && (int)CliffDirection > -1 && (int)CliffDirection < 3)
            {
                if (Index > 0 && Index < 0x38)
                {
                    if (AcreData[Index] == 0x27)
                    {
                        if (CliffDirection == CliffSide.Any)
                        {
                            if (RiverDirection == RiverSide.Any)
                            {
                                if ((CliffSide)cliff_up_down_info[Index] == CliffDirection)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if ((RiverSide)river_left_right_info[Index] == RiverDirection)
                                {
                                    if ((CliffSide)cliff_up_down_info[Index] == CliffDirection)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (RiverDirection == RiverSide.Any)
                            {
                                if ((CliffSide)cliff_up_down_info[Index] == CliffDirection)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if ((RiverSide)river_left_right_info[Index] == RiverDirection)
                                {
                                    if ((CliffSide)cliff_up_down_info[Index] == CliffDirection)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private int CountFlatBlock(byte[] AcreData, RiverSide RiverDirection, CliffSide CliffDirection, ushort[] cliff_up_down_info, ushort[] river_left_right_info)
        {
            int FlatBlocks = 0;
            for (int i = 0; i < 0x38; i++)
            {
                if (JudgeFlatBlock(AcreData, i, RiverDirection, CliffDirection, cliff_up_down_info, river_left_right_info) == true)
                {
                    FlatBlocks++;
                }
            }
            return FlatBlocks;
        }

        private int RewriteFlatType(ref byte[] AcreData, int FlatBlockIndex, byte NewFlatBlockType, RiverSide RiverDirection, CliffSide CliffDirection,
            ushort[] cliff_up_down_info, ushort[] river_left_right_info)
        {
            int FlatBlock = 0;
            for (int i = 0; i < 0x38; i++)
            {
                if (JudgeFlatBlock(AcreData, i, RiverDirection, CliffDirection, cliff_up_down_info, river_left_right_info) == true)
                {
                    if (FlatBlock == FlatBlockIndex)
                    {
                        AcreData[i] = NewFlatBlockType;
                        return i;
                    }
                    FlatBlock++;
                }
            }

            return -1;
        }

        private bool FlatBlock2Unique(ref byte[] AcreData, byte NewFlatBlockType, RiverSide RiverDirection, CliffSide CliffDirection,
            ushort[] cliff_up_down_info, ushort[] river_left_right_info)
        {
            int FlatBlocks = CountFlatBlock(AcreData, RiverDirection, CliffDirection, cliff_up_down_info, river_left_right_info);
            if (FlatBlocks > 0)
            {
                if (RewriteFlatType(ref AcreData, GetRandom(FlatBlocks), NewFlatBlockType, RiverDirection, CliffDirection, cliff_up_down_info, river_left_right_info) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        private int SetUniqueFlatBlock(ref byte[] AcreData, ushort[] cliff_up_down_info, ushort[] river_left_right_info)
        {
            int FlatBit = 0;

            var RiverSide = (RiverSide) (GetRandom(100) & 1);
            var OppositeRiverSide = (RiverSide)(((int)RiverSide ^ 1) & 1);
            if (FlatBlock2Unique(ref AcreData, 0x42, RiverSide, CliffSide.Down, cliff_up_down_info, river_left_right_info) == true)
            {
                FlatBit |= 0x10;
            }
            else if (FlatBlock2Unique(ref AcreData, 0x42, OppositeRiverSide, CliffSide.Down, cliff_up_down_info, river_left_right_info) == true)
            {
                FlatBit |= 0x10;
            }

            if (FlatBlock2Unique(ref AcreData, 0x44, OppositeRiverSide, CliffSide.Down, cliff_up_down_info, river_left_right_info) == true)
            {
                FlatBit |= 0x20;
            }
            else if (FlatBlock2Unique(ref AcreData, 0x44, RiverSide, CliffSide.Down, cliff_up_down_info, river_left_right_info) == true)
            {
                FlatBit |= 0x20;
            }

            if (FlatBlock2Unique(ref AcreData, 0x54, RiverSide.Any, CliffSide.Down, cliff_up_down_info, river_left_right_info) == true)
            {
                FlatBit |= 0x40;
            }

            return FlatBit;
        }

        // Lake Code
        private int CountPureRiver(byte[] AcreData)
        {
            int RiverAcres = 0;
            for (int i = 0; i < 0x38; i++)
            {
                if (AcreData[i] == 0x28 || (byte)(AcreData[i] - 0x29) <= 4 || AcreData[i] == 0x29)
                {
                    RiverAcres++;
                }
            }

            return RiverAcres;
        }

        private bool SetPoolDirectedRiverBlock(ref byte[] AcreData, int LakeRiverIndex)
        {
            int RiverAcre = 0;
            for (int i = 0; i < 0x38; i++)
            {
                if (AcreData[i] == 0x28 || (byte)(AcreData[i] - 0x29) <= 4 || AcreData[i] == 0x29)
                {
                    if (RiverAcre == LakeRiverIndex)
                    {
                        AcreData[i] += 0x1D;
                        return true;
                    }
                    else
                    {
                        RiverAcre++;
                    }
                }
            }
            return false;
        }

        private int SetPoolBlock(ref byte[] AcreData)
        {
            int RiverAcres = CountPureRiver(AcreData);
            if (RiverAcres > 0)
            {
                int LakeAcre = GetRandom(RiverAcres);
                if (SetPoolDirectedRiverBlock(ref AcreData, LakeAcre) == true)
                {
                    return 0x80;
                }
            }
            return 0;
        }

        // Oceanfront Bridge
        private int SetSeaBlockWithBridgeRiver(ref byte[] AcreData, int CurrentGenerationBit)
        {
            if ((CurrentGenerationBit & 8) == 0) // Make sure the lower bridge wasn't placed already
            {
                for (int i = 0; i < 0x38; i++)
                {
                    if (AcreData[i] == 0x40)
                    {
                        AcreData[i] = 0x52;
                        return 8;
                    }
                }
            }
            return 0;
        }

        private ushort GetExceptionalSeaBgDownBgName(ushort BgType)
        {
            ushort CurrentValue = 0;
            ushort CurrentIdx = 0;
            do
            {
                CurrentValue = exceptional_table[CurrentIdx];
                if (CurrentValue != 0x125)
                {
                    if (CurrentValue == BgType)
                    {
                        return exceptional_table[CurrentIdx + 1];
                    }
                }
                CurrentIdx += 2;
            } while (CurrentValue != 0x125);
            return BgType;
        }

        private ushort BgName2RandomConbiNo(ushort ExceptionalValue)
        {
            int Matches = 0;
            for (int i = 0; i < data_combi_table_number; i++)
            {
                if (ExceptionalValue == data_combi_table[i].BlockType && data_combi_table[i].ValidCombiCount != 0xFF)
                {
                    Matches++;
                }
            }

            if (Matches > 0)
            {
                int CurrentMatch = 0;
                int RandomlySelectedMatch = GetRandom(0); // Silly Animal Crossing Devs. This isn't random.
                for (int i = 0; i < data_combi_table_number; i++)
                {
                    if (data_combi_table[i].BlockType == ExceptionalValue && data_combi_table[i].ValidCombiCount != 0xFF)
                    {
                        if (CurrentMatch == RandomlySelectedMatch)
                        {
                            return (ushort)i;
                        }
                        else
                        {
                            CurrentMatch++;
                        }
                    }
                }
            }

            return data_combi_table_number;
        }

        private ushort GetRandomTownAcreFromPool(byte AcreType)
        {
            if (TownAcrePool.ContainsKey(AcreType) && TownAcrePool[AcreType].Length > 0)
            {
                int RandomlyChosenAcreIdx = GetRandom(TownAcrePool[AcreType].Length);
                return TownAcrePool[AcreType][RandomlyChosenAcreIdx];
            }
            else
            {
                return 0x0284;
            }
        }

        private void SetUniqueRailBlock(ref byte[] Data)
        {
            byte A = 0, B = 0;
            bool ASet = false, BSet = false;

            if ((GetRandom(1000) & 1) == 0)
            {
                A = 0x43;
                B = 0x41;
            }
            else
            {
                A = 0x41;
                B = 0x43;
            }

            // Set A
            while (!ASet)
            {
                int ALocation = GetRandom(2);
                if (Data[8 + ALocation] == 0x0C)
                {
                    Data[8 + ALocation] = A;
                    ASet = true;
                }
            }

            // Set B
            while (!BSet)
            {
                int BLocation = GetRandom(2);
                if (Data[11 + BLocation] == 0x0C)
                {
                    Data[11 + BLocation] = B;
                    BSet = true;
                }
            }
        }

        private void ReportRandomFieldBitResult(int RandomFieldBit, int PerfectBit)
        {
            Console.WriteLine(string.Format("RandomField Bit: {0} | Perfect Bit: {1}", RandomFieldBit.ToString("X2"), PerfectBit.ToString("X2")));
        }

        // Acre Height Code
        private byte[] InitBlockBase()
            => new byte[70];

        private void GetBlockBase(ref byte[] HeightTable, byte[] AcreData)
        {
            for (int X = 0; X < 7; X++)
            {
                byte CurrentHeight = 0;
                for (int Y = 9; Y > -1; Y--)
                {
                    byte CurrentBlock = AcreData[D2toD1(X, Y)];
                    HeightTable[D2toD1(X, Y)] = CurrentHeight;
                    if ((GetSystemBlockInfo(CurrentBlock) & 1) == 1 || (GetSystemBlockInfo(CurrentBlock) & 8) == 8 || (GetSystemBlockInfo(CurrentBlock) & 0x10) == 0x10
                        || CurrentBlock == 0x3D || CurrentBlock == 0x3E)
                    {
                        CurrentHeight++;
                    }
                }
            }
        }

        private byte[] MakeBaseHeightTable(byte[] AcreData)
        {
            byte[] HeightTable = InitBlockBase();
            GetBlockBase(ref HeightTable, AcreData);
            return HeightTable;
        }

        private Tuple<byte[], byte[]> MakeRandomField_ovl()
        {
            int StepMode = GetRandomStepMode();
            int PerfectBit = MakePerfectBit();
            int Bit = 0;
            byte[] AcreData = null;
            byte[] HeightTable = null;
            Console.WriteLine("StepMode: " + StepMode);

            while ((PerfectBit & Bit) != PerfectBit)
            {
                AcreData = HeightTable = null;
                AcreData = MakeBaseLandform(StepMode);
                MakeFlatPlaceInformation(AcreData, out ushort[] river_left_right_info, out ushort[] cliff_up_down_info);
                SetMarinBlock(ref AcreData);
                Bit = SetBridgeAndSlopeBlock(ref AcreData, StepMode == 1);
                Bit |= SetNeedleworkAndWharfBlock(ref AcreData);
                Bit |= SetUniqueFlatBlock(ref AcreData, cliff_up_down_info, river_left_right_info);
                SetUniqueRailBlock(ref AcreData);
                Bit |= SetPoolBlock(ref AcreData);
                Bit |= SetSeaBlockWithBridgeRiver(ref AcreData, Bit);
                HeightTable = MakeBaseHeightTable(AcreData);
                ReportRandomFieldBitResult(Bit, PerfectBit);
            }
            return new Tuple<byte[], byte[]>(AcreData, HeightTable);
        }

        public ushort[] Generate(int? Seed = null)
        {
            _randomSeed = Seed ?? Environment.TickCount;

            var RandomFieldData = MakeRandomField_ovl();
            byte[] Data = RandomFieldData.Item1;
            byte[] HeightData = RandomFieldData.Item2;

            ushort[] AcreData = new ushort[70];
            for (int i = 0; i < 70; i++)
            {
                ushort BlockId = 0;
                if (Data[i] == 0x65)
                {
                    ushort AboveAcreId = AcreData[D2toD1(i % 7, (i / 7) - 1)];
                    int AcreBlockId = AboveAcreId >> 2;
                    ushort OceanId = data_combi_table[AcreBlockId].BlockType;
                    ushort ExceptionalValue = GetExceptionalSeaBgDownBgName(OceanId);
                    ushort CorrectBgValue = BgName2RandomConbiNo(ExceptionalValue);
                    BlockId = (ushort)(CorrectBgValue << 2);
                }
                else
                {
                    BlockId = GetRandomTownAcreFromPool(Data[i]);
                    if (i > 6 && i < 50 && i % 7 > 0 && i % 7 < 6)
                    {
                        int Count = 0;
                        while (IsUniqueBlock(AcreData, BlockId) == false && Count < 10)
                        {
                            BlockId = GetRandomTownAcreFromPool(Data[i]);
                            Count++;
                        }
                    }
                }
                AcreData[i] = (ushort)(BlockId | HeightData[i]);
            }
            return AcreData;
        }

        private bool IsUniqueBlock(ushort[] AcreData, ushort BlockId)
        {
            BlockId &= 0xFFFC; // Remove the height data
            foreach (var Block in AcreData)
                if ((Block & 0xFFFC) == BlockId)
                    return false;
            return true;
        }
    }
}
