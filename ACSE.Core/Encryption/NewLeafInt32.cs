using System;
using ACSE.Core.Utilities;

namespace ACSE.Core.Encryption
{
    /*
     * Animal Crossing: New Leaf (and Welcome Amiibo) Integer Encryption Documentation
     * 
     * Animal Crossing: New Leaf uses a combination of methods to obfuscate their integer values in the save file.
     * The encryption consists of two integer values. The first integer is the encrypted integer. The second integer contains the checksum of the encryption and
     * the data needed to decrypt it.
     * 
     * The lower two bytes are a short which has a randomized "adjustment value"
     * This is likely to throw off anyone looking for a static value that is added/removed from it.
     * 
     * The second byte in the int contains the "Shift Value" which is a random number between 0x00 and 0x1A. This number is then subtracted from a static value of 0x1C
     * to determine the left bitshift count.
     * 
     * The left bitshift count is then subtracted from a static value of 0x20 to determine the right bitshift count.
     * 
     * The uppermost byte in the second integer is the Checksum of all the bytes in the Encrypted Integer + a statc value of 0xBA.
     * 
     * Once we have the left & right bitshift counts, we can decode the Encrypted Integer.
     * 
     * The formula is this:
     *  uint Unencrypted_Int = ((Encrypted_Integer << Left_Shift_Count) + (Encrypted_Integer >> Right_Shift_Count)) - (Random_Adjustment_Value + Static_Int 0x8F187432);
     */

    public class NewLeafInt32
    {
        public uint Int1;
        public uint Int2;
        public uint Value;
        public bool Valid;

        public NewLeafInt32()
        {
            Int1 = 0;
            Int2 = 0;

            Value = 0;
        }

        public NewLeafInt32(uint intA, uint intB)
        {
            Int1 = intA;
            Int2 = intB;

            var adjustValue = (ushort)(intB);
            var shiftValue = (byte)(intB >> 16);
            var intChecksum = (byte)(intB >> 24);
            var computedChecksum = GetChecksum(intA);

            if (intChecksum != computedChecksum)
            {
                DebugUtility.DebugManagerInstance.WriteLine(
                    $"Encrypted Int had an invalid Checksum! Checksum: 0x{intChecksum:X2} | Calculated Checksum: 0x{computedChecksum:X2}");
                Int1 = 0;
                Int2 = 0;
                Value = 0;
                Valid = false;
            }
            else
            {
                var leftShiftCount = (byte)(0x1C - shiftValue);
                var rightShiftCount = (byte)(0x20 - leftShiftCount);

                if (leftShiftCount < 0x20)
                {
                    Value = ((intA << leftShiftCount) + (intA >> rightShiftCount)) - (adjustValue + 0x8F187432);
                    Valid = true;
                }
                else
                {
                    DebugUtility.DebugManagerInstance.WriteLine("Invalid Shift Count was detected!");
                    Value = (intA << rightShiftCount) - (adjustValue + 0x8F187432);
                    Valid = true; // Is this right?
                }
            }
        }

        public NewLeafInt32(ulong encryptedData) : this((uint)(encryptedData >> 32), (uint)(encryptedData)) { }

        public NewLeafInt32(uint unencryptedInt)
        {
            var generator = new Random();
            var adjustValue = (ushort)(generator.Next(0x10000));
            var shiftValue = (byte)(generator.Next(0x1A));

            var encryptedInt = unencryptedInt + adjustValue + 0x8F187432;
            encryptedInt = (encryptedInt >> (0x1C - shiftValue)) + (encryptedInt << (shiftValue + 4));
            var encryptionData = (uint)(adjustValue + (shiftValue << 16) + (GetChecksum(encryptedInt) << 24));

            Int1 = encryptedInt;
            Int2 = encryptionData;
            Value = unencryptedInt;
        }

        public static byte GetChecksum(uint encryptedInt)
        {
            return (byte)((byte)(encryptedInt) + (byte)(encryptedInt >> 8) + (byte)(encryptedInt >> 16) + (byte)(encryptedInt >> 24) + 0xBA);
        }
    }
}
