using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    class Generation
    {
        private readonly static Random Rand = new Random();

        // Generation Common Things
        private static readonly int[] blockGroup_428 = new int[16]
        {
            0x0F, 0x15, 0x28, 0x2E, 0x2F, 0x35, 0x36, 0x3C,
            0x16, 0x26, 0x16, 0x1C, 0x1D, 0x21, 0x22, 0x26
        };

        private static readonly int[] x_offset_409 = new int[4] { 0, -1, 0, 1 };
        private static readonly int[] z_offset_410 = new int[4] { -1, 0, 1, 0 };

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
        private static readonly byte[] river1_album_data   = new byte[7] { 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C };
        private static readonly byte[] river2_album_data   = new byte[7] { 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0xFF, 0xFF };
        private static readonly byte[] river3_album_data   = new byte[7] { 0x22, 0xFF, 0xFF, 0x23, 0x24, 0x25, 0x26 };
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

        private static readonly byte[][] step3_blockss = new byte[10][]
        {
            step3_blocks3, step3_blocks7, step3_blocks7R, step3_blocks8,
            step3_blocksB, step3_blocksBR, step3_blocksE, step3_blocksER,
            step3_blocksF, step3_blocksFR
        };

        private static readonly Dictionary<byte, ushort[]> TownAcrePool = new Dictionary<byte, ushort[]>
        {
            { 0x00, new ushort[] {0x0324} },
            { 0x01, new ushort[] {0x0328} },
            { 0x02, new ushort[] {0x032C} },
            // 0x03?
            { 0x04, new ushort[] {0x0338} },
            { 0x05, new ushort[] {0x0344} },
            // 0x06?
            // 0x07?
            { 0x08, new ushort[] {0x0348} },
            { 0x09, new ushort[] {0x0334} },
            { 0x0A, new ushort[] {0x0340} },
            { 0x0B, new ushort[] {0x0154, 0x02F0, 0x02F4} },
            { 0x0C, new ushort[] {0x0118, 0x0294, 0x0298} }, // Dump Acres
            { 0x0D, new ushort[] {0x0070, 0x02B8, 0x02BC, 0x02C0, 0x02C4} },
            { 0x0E, new ushort[] {0x0358, 0x035C, 0x0360} },
            { 0x0F, new ushort[] {0x009C, 0x015C, 0x0160, 0x0164, 0x0168} },
            { 0x10, new ushort[] {0x00A8, 0x016C, 0x01F4} },
            { 0x11, new ushort[] {0x00AC, 0x01A0, 0x01F0} },
            { 0x12, new ushort[] {0x00B4, 0x01B0, 0x01EC} },
            { 0x13, new ushort[] {0x00C0, 0x01B4, 0x01E8} },
            { 0x14, new ushort[] {0x00CC, 0x01B8, 0x0218} },
            { 0x15, new ushort[] {0x00D4, 0x01A4, 0x021C} },
            { 0x16, new ushort[] {0x0084, 0x0200, 0x0204} },
            { 0x17, new ushort[] {0x008C, 0x0210} },
            { 0x18, new ushort[] {0x00B0, 0x019C} },
            { 0x19, new ushort[] {0x00B8, 0x01C4} },
            { 0x1A, new ushort[] {0x006C, 0x0214} },
            { 0x1B, new ushort[] {0x00D0, 0x0198} },
            { 0x1C, new ushort[] {0x0138, 0x01CC} },
            { 0x1D, new ushort[] {0x00A0, 0x01A8, 0x0208} },
            { 0x1E, new ushort[] {0x0090, 0x0244} },
            { 0x1F, new ushort[] {0x00F4, 0x0248} },
            { 0x20, new ushort[] {0x00BC, 0x01C8} },
            { 0x21, new ushort[] {0x00C4, 0x01D4} },
            { 0x22, new ushort[] {0x00A4, 0x01AC, 0x020C} },
            { 0x23, new ushort[] {0x013C, 0x01D8} },
            { 0x24, new ushort[] {0x00C8, 0x01E4} },
            { 0x25, new ushort[] {0x00FC} },
            { 0x26, new ushort[] {0x00F8, 0x0414} },
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
            { 0x66, new ushort[] {0x0578, 0x057C} },
            { 0x67, new ushort[] {0x0580, 0x0584, 0x0588, 0x058C} },
            { 0x68, new ushort[] {0x0518, 0x051C, 0x0520, 0x0524, 0x0528, 0x052C, 0x0530, 0x0534, 0x0538, 0x053C, 0x0540, 0x0544, 0x0548, 0x054C, 0x0550, 0x0554, 0x0558, 0x055C, 0x0560, 0x0564, 0x0568, 0x056C, 0x0570, 0x0574, 0x05B4, 0x05B8} },
        };

        private static int D2toD1(int AcreX, int AcreY)
        {
            return AcreY * 7 + AcreX;
        }

        private static void D1toD2(int Index, out int X, out int Y)
        {
            X = Index % 7;
            Y = Index / 7;
        }

        private static int GetXYCoordinateForBlockType(byte[] Data, int BlockType, out int X, out int Y)
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
        /// Selects the "step" mode, or layer count, of your town. If 0, it's a 2 layered town. If 1, it's a 3 layered town.
        /// </summary>
        /// <returns>Step Mode</returns>
        private static int GetRandomStepMode()
        {
            int RNG = Rand.Next(0, 64);
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
        private static int MakePerfectBit()
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
        private static void Direct2BlockNo(out int X, out int Y, int AcreX, int AcreY, int Direction)
        {
            X = AcreX + x_offset_409[Direction & 3];
            Y = AcreY + z_offset_410[Direction & 3];
        }

        private static bool CheckBlockGroup(int BlockType, int AcreTypeSet)
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
        private static bool CheckCorrectBlockNo(int AcreX, int AcreY, int AcreXMin, int AcreXMax, int AcreYMin, int AcreYMax)
            => (AcreX >= AcreXMin && AcreX <= AcreXMax && AcreY >= AcreYMin && AcreY <= AcreYMax);

        private static bool DecideBaseCliff(ref byte[] AcreData)
        {
            int CliffStartTableIndex = Rand.Next(4);
            byte[] CliffStartTable = cliff_start_table[CliffStartTableIndex];
            byte CliffStartAcreType = CliffStartTable[Rand.Next(CliffStartTable.Length)];

            // Set the first in-town cliff acre
            int CliffStartAcre = D2toD1(1, CliffStartTableIndex + 2);
            AcreData[CliffStartAcre] = CliffStartAcreType;

            // Set the border acre
            int CliffBorderStartAcre = D2toD1(0, CliffStartTableIndex + 2);
            AcreData[CliffBorderStartAcre] = 0x3D;

            Console.WriteLine("\n===================================");
            Console.WriteLine("     Begin Cliff Tracing");
            Console.WriteLine("===================================\n");

            // Trace Cliff
            TraceCliffBlock(ref AcreData, 1, CliffStartTableIndex + 2);
            PrintAcreData(AcreData);

            Console.WriteLine("\n===================================");
            Console.WriteLine("     Set Cliff End");
            Console.WriteLine("===================================\n");

            // Set Cliff End Acre
            SetEndCliffBlock(ref AcreData);

            // Check Cliff is valid
            return LastCheckCliff(AcreData, 1, CliffStartTableIndex + 2);
        }

        private static bool TraceCliffBlock(ref byte[] AcreData, int AcreX, int AcreY)
        {
            byte CurrentCliffAcreType = AcreData[D2toD1(AcreX, AcreY)];
            int TraceState = 0;
            byte CliffSubtractionValue = (byte)(CurrentCliffAcreType - 0xF);
            int CliffSubtractionValueShifted = CliffSubtractionValue << 2;
            int X = 0, Y = 0;

            while (TraceState == 0)
            {
                PrintAcreData(AcreData);
                byte[] CliffNext = cliff_next_data[CliffSubtractionValue];
                byte CliffAcreType = CliffNext[Rand.Next(CliffNext.Length)];
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

        private static void SetEndCliffBlock(ref byte[] AcreData) // This doesn't always work. Double check it at some point.
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

            PrintAcreData(AcreData);
        }

        private static bool LastCheckCliff(byte[] AcreData, int AcreX, int AcreY)
        {
            byte CliffAcreType = AcreData[D2toD1(AcreX, AcreY)];
            int Y = AcreY;
            while (CheckBlockGroup(CliffAcreType, 0) == true)
            {
                byte CliffDirectValue = cliff_next_direct[CliffAcreType - 0xF];
                Direct2BlockNo(out AcreX, out AcreY, AcreX, AcreY, CliffDirectValue);
                CliffAcreType = AcreData[D2toD1(AcreX, AcreY)];
            }

            return AcreX > 5 && AcreY == Y; // Might be AcreY >= Y or AcreY != Y
        }

        // River Generation Code
        /// <summary>
        /// Gets the direction of the next river section.
        /// </summary>
        /// <param name="RiverIdx">The current river block index</param>
        /// <returns>The direction of the next river section</returns>
        private static int RiverIdx2NextDirect(int RiverIdx)
        {
            if (RiverIdx > -1 && RiverIdx < 7)
            {
                return river_next_direct[RiverIdx];
            }
            return 2; // Return south by default
        }

        private static byte RiverAlbuminCliff(byte BlockType, byte AlbuminType)
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
        private static int GetCenterCrossZoneRiverCount(byte[] AcreData)
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

        private static bool TraceRiverPart1(ref byte[] AcreData, out int AcreX, out int AcreY)
        {
            int RiverTraceState = 0;
            AcreX = 0;
            AcreY = 0;
            while (RiverTraceState == 0)
            {
                int RiverXStartAcre = startX_table[Rand.Next(4)];
                byte[] RiverStartData = river_next_data[0];
                byte RiverStartType = RiverStartData[Rand.Next(RiverStartData.Length)];

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

        private static bool TraceRiverPart2(ref byte[] AcreData, ref byte[] UnchangedAcreData, int AcreX, int AcreY, byte[] challenge_flag)
        {
            PrintAcreData(AcreData);
            int RiverStartAcre = D2toD1(AcreX, AcreY);
            byte RiverStartAcreType = AcreData[RiverStartAcre];
            int RiverDirection = RiverIdx2NextDirect((byte)(RiverStartAcreType - 0x28));
            int X = 0, Y = 0;

            int RiverTraceState = 0;
            while (RiverTraceState == 0)
            {
                byte[] river_next_set = river_next_data[RiverStartAcreType - 0x28];
                byte NextRiverType = river_next_set[Rand.Next(river_next_set.Length)];
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

        private static bool LastCheckRiver(byte[] AcreData, int AcreX, int AcreY)
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

        private static bool DecideBaseRiver(ref byte[] AcreData)
        {
            byte[] UnchangedAcreData = new byte[AcreData.Length];
            Array.Copy(AcreData, UnchangedAcreData, AcreData.Length);
            Console.WriteLine("============== River Generation ==============\n");
            if (TraceRiverPart1(ref AcreData, out int AcreX, out int AcreY))
            {
                if (TraceRiverPart2(ref AcreData, ref UnchangedAcreData, AcreX, AcreY, new byte[0x38]))
                {
                    PrintAcreData(AcreData);
                    Console.WriteLine("============== End River Generation ============== ");
                    return LastCheckRiver(AcreData, AcreX, AcreY);
                }
            }
            return false;
        }

        private static bool SetRandomBlockData(ref byte[] AcreData) // Technically takes two copies of AcreData
        {
            if (DecideBaseCliff(ref AcreData))
            {
                return DecideBaseRiver(ref AcreData);
            }
            return false;
        }

        private static byte[] MakeBaseLandformStep2()
        {
            byte[] AcreData = new byte[70];
            Array.Copy(DefaultTownStructure, AcreData, 70);
            while (SetRandomBlockData(ref AcreData) == false)
            {
                Array.Copy(DefaultTownStructure, AcreData, 70);
            }

            //DecideRiverAlbuminCliff(ref AcreData);
            return AcreData;
        }

        private static byte[] MakeBaseLandformStep3()
        {
            return step3_blockss[Rand.Next(10)];
        }

        private static byte[] MakeBaseLandform(int StepMode)
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

        private static byte[] MakeRandomField_ovl()
        {
            int StepMode = GetRandomStepMode();
            int PerfectBit = MakePerfectBit();
            int Bit = 0;

            Console.WriteLine("StepMode: " + StepMode);

            // TODO: Check for perfect bit vs current bit
            return MakeBaseLandform(StepMode);
        }

        private static ushort GetRandomTownAcreFromPool(byte AcreType)
        {
            if (TownAcrePool.ContainsKey(AcreType) && TownAcrePool[AcreType].Length > 0)
            {
                int RandomlyChosenAcreIdx = Rand.Next(TownAcrePool[AcreType].Length);
                return TownAcrePool[AcreType][RandomlyChosenAcreIdx];
            }
            else
            {
                return 0x0284;
            }
        }

        private static void DecideRiverAlbuminCliff(ref byte[] Data)
        {
            bool Set = false;
            while (!Set)
            {
                int XLocation = Rand.Next(1, 6);
                int Location = D2toD1(XLocation, 1);
                if (Data[Location] == 0x0C)
                {
                    Data[Location] = 0x0D; // Train Track River Bridge
                    Data[XLocation] = 0x01; // River Start Cliff (I think this is set in TraceRiver1)
                    Set = true;
                }
            }
        }

        private static int SearchForRiverStart(ref byte[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] == 0x0D)
                {
                    return i;
                }
            }

            return -1;
        }

        private static void TraceRiver(ref byte[] Data)
        {
            int TrainTrackRiverAcre = SearchForRiverStart(ref Data);
            if (TrainTrackRiverAcre > -1)
            {
                D1toD2(TrainTrackRiverAcre, out int TrainTrackRiverXAcre, out int TrainTrackRiverYAcre);
                int CurrentYAcre = TrainTrackRiverYAcre;
                int CurrentXAcre = TrainTrackRiverXAcre;

                while (CurrentYAcre < 7)
                {
                    bool Turn = Rand.Next(0, 2) == 1;
                    if (Turn)
                    {
                        bool TurnSuccessful = false;
                        
                    }
                }
            }
        }

        private static void SetUniqueRailBlock(ref byte[] Data)
        {
            byte A = 0, B = 0;
            bool ASet = false, BSet = false;

            if (Rand.Next(1000) >= 500) // This may not be accurate. I believe it's only true when it equals one.
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
                int ALocation = Rand.Next(2);
                if (Data[8 + ALocation] == 0x0C)
                {
                    Data[8 + ALocation] = A;
                    ASet = true;
                }
            }

            // Set B
            while (!BSet)
            {
                int BLocation = Rand.Next(2);
                if (Data[11 + BLocation] == 0x0C)
                {
                    Data[11 + BLocation] = B;
                    BSet = true;
                }
            }
        }

        public static ushort[] Generate(SaveType saveType)
        {
            switch (saveType)
            {
                case SaveType.Doubutsu_no_Mori_Plus:
                case SaveType.Animal_Crossing:
                case SaveType.Doubutsu_no_Mori_e_Plus:
                    /*byte[] Data = new byte[70];
                    Array.Copy(DefaultTownStructure, Data, 70);

                    DecideRiverAlbuminCliff(ref Data);
                    SetUniqueRailBlock(ref Data);*/

                    byte[] Data = MakeRandomField_ovl();
                    PrintAcreData(Data);

                    ushort[] AcreData = new ushort[70];
                    string s = "";
                    for (int i = 0; i < 70; i++)
                    {
                        AcreData[i] = GetRandomTownAcreFromPool(Data[i]);
                        if (i % 7 == 0)
                        {
                            Console.WriteLine(s);
                            s = "";
                        }

                        s += "0x" + AcreData[i].ToString("X4") + " ";
                    }

                    return null;
                default:
                    return null;
            }
        }

        // Debug
        private static void PrintAcreData(byte[] AcreData)
        {
            Console.Write("\n");
            for (int i = 0; i < AcreData.Length; i++)
            {
                if (i > 0 && i % 7 == 0)
                {
                    Console.Write("\n");
                }
                Console.Write(AcreData[i].ToString("X2") + " ");
            }
            Console.Write("\n");
        }
    }
}
