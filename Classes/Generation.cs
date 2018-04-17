using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    class Generation
    {
        private static byte[] DefaultTownStructure = new byte[70]
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

        private static Dictionary<byte, ushort[]> TownAcrePool = new Dictionary<byte, ushort[]>
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

        private static ushort GetRandomTownAcreFromPool(byte AcreType)
        {
            if (TownAcrePool.ContainsKey(AcreType) && TownAcrePool[AcreType].Length > 0)
            {
                int RandomlyChosenAcreIdx = new Random().Next(TownAcrePool[AcreType].Length);
                return TownAcrePool[AcreType][RandomlyChosenAcreIdx];
            }
            else
            {
                return 0x0284;
            }
        }

        private static void DecideRiverAlbuminCliff(ref byte[] Data)
        {
            var Rand = new Random();
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
                    bool Turn = new Random().Next(0, 2) == 1;
                    if (Turn)
                    {
                        bool TurnSuccessful = false;
                        
                    }
                }
            }
        }

        private static void SetUniqueRailBlock(ref byte[] Data)
        {
            var Rand = new Random();
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
                    byte[] Data = new byte[70];
                    Array.Copy(DefaultTownStructure, Data, 70);

                    DecideRiverAlbuminCliff(ref Data);
                    SetUniqueRailBlock(ref Data);

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
    }
}
