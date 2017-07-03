using System;

namespace ACSE
{
    /*
     * Animal Crossing: New Leaf (and Welcome Amiibo) Interger Encryption Documentation
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

    public class NL_Int32
    {
        public uint Int_1;
        public uint Int_2;
        public uint Value;
        public bool Valid = false;

        public NL_Int32()
        {
            Int_1 = 0;
            Int_2 = 0;

            Value = 0;
        }

        public NL_Int32(uint Int_A, uint Int_B)
        {
            Int_1 = Int_A;
            Int_2 = Int_B;

            ushort Adjust_Value = (ushort)(Int_B);
            byte Shift_Value = (byte)(Int_B >> 16);
            byte Int_Checksum = (byte)(Int_B >> 24);
            byte Computed_Checksum = GetChecksum(Int_A);

            if (Int_Checksum != Computed_Checksum)
            {
                NewMainForm.Debug_Manager.WriteLine("Encrypted Int had an invalid Checksum!", DebugLevel.Error);
                Int_1 = 0;
                Int_2 = 0;
                Value = 0;
                Valid = false;
            }
            else
            {
                byte Left_Shift_Count = (byte)(0x1C - Shift_Value);
                byte Right_Shift_Count = (byte)(0x20 - Left_Shift_Count);

                if (Left_Shift_Count < 0x20)
                {
                    Value = ((Int_A << Left_Shift_Count) + (Int_A >> Right_Shift_Count)) - (Adjust_Value + 0x8F187432);
                    Valid = true;
                }
                else
                {
                    NewMainForm.Debug_Manager.WriteLine("Invalid Shift Count was detected!", DebugLevel.Error);
                    Value = (Int_A << Right_Shift_Count) - (Adjust_Value + 0x8F187432);
                    Valid = true; // Is this right?
                }
            }
            //System.Windows.Forms.MessageBox.Show("Value = " + Value.ToString());
        }

        public NL_Int32(uint Unencrypted_Int)
        {
            Random Generator = new Random();
            ushort Adjust_Value = (ushort)(Generator.Next(0x10000));
            byte Shift_Value = (byte)(Generator.Next(0x1A));

            uint Encrypted_Int = Unencrypted_Int + Adjust_Value + 0x8F187432;
            Encrypted_Int = (Encrypted_Int >> (0x1C - Shift_Value)) + (Encrypted_Int << (Shift_Value + 4));
            uint Encryption_Data = (uint)(Adjust_Value + (Shift_Value << 16) + (GetChecksum(Encrypted_Int) << 24));

            //System.Windows.Forms.MessageBox.Show("Encryped Int is decrypted properly: " + (new NL_Int32(Encrypted_Int, Encryption_Data).Value == Unencrypted_Int).ToString());
            Int_1 = Encrypted_Int;
            Int_2 = Encryption_Data;
            Value = Unencrypted_Int;
        }

        public static byte GetChecksum(uint Encrypted_Int)
        {
            return (byte)((byte)(Encrypted_Int) + (byte)(Encrypted_Int >> 8) + (byte)(Encrypted_Int >> 16) + (byte)(Encrypted_Int >> 24) + 0xBA);
        }
    }
}
