using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ACSE
{
    public enum DebugLevel
    {
        None,
        Error,
        Info,
        Debug
    }

    public class DebugManager
    {
        private static string Log_File_Name = "ACSE_Log";
        private FileStream Log_File;
        private StreamWriter Log_Writer;
        public bool Enabled = false;

        public DebugManager()
        {
            if (Properties.Settings.Default.DebugLevel > 0)
            {
                Log_File = new FileStream(Get_Log_File_Path(), FileMode.OpenOrCreate);
                Log_Writer = new StreamWriter(Log_File);
                Log_Writer.BaseStream.Seek(0, SeekOrigin.End);
                Enabled = true;
                WriteLine("========== Debug Log Initiated ==========");
            }
            else
            {
                Enabled = false;
            }
        }

        public bool Log_File_Exists()
        {
            return File.Exists(Get_Log_File_Path());
        }

        public string Get_Log_File_Path()
        {
            return NewMainForm.Assembly_Location + string.Format("\\{0}.txt", Log_File_Name);
        }

        public void WriteLine(string Contents, DebugLevel Level = DebugLevel.Info)
        {
            if (Log_Writer != null && Level <= Properties.Settings.Default.DebugLevel)
            {
                Log_Writer.WriteLine(string.Format("[{0}] - ({1}) - {2} => {3}", Level, NewMainForm.Save_File != null
                    ? NewMainForm.Save_File.Save_Type.ToString().Replace("_", " ") : "No Save", DateTime.Now, Contents));
                Log_Writer.Flush();
            }
        }
    }
}
