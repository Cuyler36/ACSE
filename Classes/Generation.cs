using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    class Generation
    {
        private static ushort[] DefaultTownBorderStructure = new ushort[70]
        {
            0x0344, 0x0324, 0x0324, 0x0324, 0x0324, 0x0324, 0x0348,
            0x0334, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0340,
            0x032C, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0338,
            0x032C, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0338,
            0x032C, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0338,
            0x032C, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0338,
            0x03B4, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x03B8,
            0x0518, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x051C,
            0x03E0, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x057C,
            0x04D8, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0580,
        };

        /// <summary>
        /// Returns a randomly generated layer count between 0 and 3.
        /// </summary>
        /// <param name="AcreBuffer">The acre buffer array</param>
        /// <returns>Layer Count</returns>
        private byte GetTownLevelCount(ref ushort[] AcreBuffer)
        {
            var LevelGenerator = new Random();
            byte Level = (byte)LevelGenerator.Next(0, 4);
            for (int Y = 0; Y < 10; Y++)
            {
                for (int X = 0; X < 7; X++)
                {
                    if (Y == 0 || X == 0 || X == 6)
                    {
                        int Index = Y * 10 + X;
                        AcreBuffer[Index] = (ushort)(AcreBuffer[Index] + Level);
                    }
                }
            }
            return Level;
        }

        private void GenerateCliffs(ref ushort[] AcreBuffer, byte LevelCount)
        {
            if (LevelCount > 0 && LevelCount < 4)
            {
                var Generator = new Random();
                bool CanBend = LevelCount < 3;
                byte PreviousAcreRowStart = 3;

                for (int i = 0; i < LevelCount; i++)
                {
                    byte CliffStart = (byte)Generator.Next(PreviousAcreRowStart, 6 - (LevelCount - i));
                    if (CanBend)
                        CanBend = CliffStart < 5;
                    // TODO: Cliff logic
                    PreviousAcreRowStart = CliffStart;
                }
            }
        }

        private byte DecideRiverStartAcre(ref ushort[] AcreBuffer, byte LevelCount)
        {
            byte Acre = (byte)(new Random()).Next(1, 7);
            if (Acre == 3)
            {
                return DecideRiverStartAcre(ref AcreBuffer, LevelCount);
            }
            else
            {
                AcreBuffer[Acre] = (ushort)(0x0328 + LevelCount);
                return Acre;
            }
        }

        private void SelectRiverAcre(ref ushort[] AcreBuffer, byte PreviousRiverAcreIndex, byte PreviousRiverDirection)
        {
            // TODO: Get all river acres and sort them for use in generation. 
            // Needs:
            //  Determine if river should be a waterfall (possibly place waterfall object correctly?)
            //  Determine if river should be an ocean outlet
            //  Place two bridges (perhaps allow user to pick a number?)
            //  Place on correct height
        }

        private byte GetRiverBendDirection(byte PreviousRiverAcreIndex, byte PreviousRiverDirection)
        {
            byte Direction = (byte)(new Random().Next(0, 3));
            if (Direction == 1)
            {
                if (PreviousRiverDirection == 2 || PreviousRiverAcreIndex == 5)
                {
                    return GetRiverBendDirection(PreviousRiverAcreIndex, PreviousRiverDirection);
                }
            }
            else if (Direction == 2)
            {
                if (PreviousRiverDirection == 1 || PreviousRiverAcreIndex == 1)
                {
                    return GetRiverBendDirection(PreviousRiverAcreIndex, PreviousRiverDirection);
                }
            }
            return Direction;
        }

        private byte DecideRiverBend(ref ushort[] AcreBuffer, ref byte PreviousRiverAcreIndex, byte PreviousRiverDirection)
        {
            var Generator = new Random();
            int ThisRiverAcreIndex = PreviousRiverAcreIndex;

            switch (PreviousRiverDirection)
            {
                default: // South
                case 0:
                    ThisRiverAcreIndex = PreviousRiverAcreIndex + 10;
                    break;
                case 1: // East
                    ThisRiverAcreIndex = PreviousRiverAcreIndex++;
                    break;
                case 2: // West
                    ThisRiverAcreIndex = PreviousRiverAcreIndex--;
                    break;
            }

            if (Generator.Next(0, 256) > 0xBF) // Approximately 25% chance of a bend
            {
                var NewDirection = GetRiverBendDirection(PreviousRiverAcreIndex, PreviousRiverDirection);
                
                // TODO: Check for cliff acres (for waterfall & rivers running along it) and place river based upon the previous & new flow directions

                return 0; // TEMP
            }
            else
            {

                return PreviousRiverDirection;
            }
        }
    }
}
