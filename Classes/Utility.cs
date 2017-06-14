using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSE
{
    public static class Utility
    {
        public static void Scan_For_NL_Int32()
        {
            if (NewMainForm.Save_File != null && (NewMainForm.Save_File.Save_Type == SaveType.New_Leaf || NewMainForm.Save_File.Save_Type == SaveType.Welcome_Amiibo))
            {
                for (int i = 0; i < NewMainForm.Save_File.Working_Save_Data.Length; i += 4)
                {
                    NL_Int32 Possible_NL_Int32 = new NL_Int32(NewMainForm.Save_File.ReadUInt32(i), NewMainForm.Save_File.ReadUInt32(i + 4));
                    if (Possible_NL_Int32.Valid)
                        System.Windows.Forms.MessageBox.Show(string.Format("Found Valid NL_Int32 at offset 0x{0} | Value: {1}", i.ToString("X"), Possible_NL_Int32.Value));
                }
            }
        }
    }
}
