using System;
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
        private static readonly string Log_File_Name = "ACSE_Log";
        private FileStream Log_File;
        private StreamWriter Log_Writer;
        private readonly int MaxLogSize = 5000000; // 5MB Max Size
        public bool Enabled = false;

        public DebugManager()
        {
            CheckAndDeleteLogFile();

            if (Properties.Settings.Default.DebugLevel > 0)
            {
                InitiateDebugLogWriter();
                Enabled = true;
                WriteLine("========== Debug Log Initiated ==========");
            }
            else
            {
                Enabled = false;
            }
        }

        public void CloseDebugLogWriter()
        {
            if (Log_Writer != null)
            {
                Log_Writer.Close();
                Log_Writer.Dispose();
            }

            if (Log_File != null)
            {
                Log_File.Close();
                Log_File.Dispose();
            }

            Enabled = false;
        }

        public void InitiateDebugLogWriter()
        {
            CloseDebugLogWriter();

            try
            {
                Log_File = new FileStream(Get_Log_File_Path(), FileMode.OpenOrCreate);
                Log_Writer = new StreamWriter(Log_File);
                Log_Writer.BaseStream.Seek(0, SeekOrigin.End);
            }
            catch
            {
                Enabled = false;
                Console.WriteLine("Unable to open or create the debug log file!");
            }
        }

        private bool CheckLogSizeOK()
        {
            var Info = new FileInfo(Get_Log_File_Path());
            return Info.Length <= MaxLogSize;
        }

        public void DeleteLogFile(string FilePath)
        {
            try
            {
                File.Delete(FilePath);
                Console.WriteLine("Log file exceeded maximum file length and was deleted.");
            }
            catch { Console.WriteLine("Unable to delete log file!"); }
        }

        private void CheckAndDeleteLogFile()
        {
            var FilePath = Get_Log_File_Path();
            if (File.Exists(FilePath) && !CheckLogSizeOK())
            {
                DeleteLogFile(FilePath);
            }
        }

        public bool Log_File_Exists()
        {
            return File.Exists(Get_Log_File_Path());
        }

        public string Get_Log_File_Path()
        {
            return MainForm.Assembly_Location + string.Format("\\{0}.txt", Log_File_Name);
        }

        public void WriteLine(string Contents, DebugLevel Level = DebugLevel.Info)
        {
            if (Log_Writer != null && Level <= Properties.Settings.Default.DebugLevel)
            {
                if (!CheckLogSizeOK())
                {
                    CloseDebugLogWriter();
                    DeleteLogFile(Get_Log_File_Path());
                    InitiateDebugLogWriter();
                }
                Log_Writer.WriteLine(string.Format("[{0}] - ({1}) - {2} => {3}", Level, MainForm.Save_File != null
                    ? MainForm.Save_File.SaveType.ToString().Replace("_", " ") : "No Save", DateTime.Now, Contents));
                Log_Writer.Flush();
            }
        }
    }
}
