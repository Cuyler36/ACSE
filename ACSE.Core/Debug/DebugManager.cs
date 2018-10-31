using System;
using System.IO;
using ACSE.Core.Saves;

namespace ACSE.Core.Debug
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
        private const string LogFileName = "ACSE_Log";
        private FileStream _logFile;
        private StreamWriter _logWriter;
        private const int MaxLogSize = 5000000; // 5MB Max Size
        public Save SaveFile;

        public bool Enabled;
        public DebugLevel Level;

        public DebugManager(Save saveFile, DebugLevel level = DebugLevel.Info)
        {
            SaveFile = saveFile;
            Level = level;
            CheckAndDeleteLogFile();

            if (level > 0)
            {
                InitializeDebugLogWriter();
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
            _logWriter?.Close();
            _logFile?.Close();
            Enabled = false;
        }

        public void InitializeDebugLogWriter()
        {
            CloseDebugLogWriter();

            try
            {
                _logFile = new FileStream(GetLogFilePath(), FileMode.OpenOrCreate);
                _logWriter = new StreamWriter(_logFile);
                _logWriter.BaseStream.Seek(0, SeekOrigin.End);
            }
            catch
            {
                Enabled = false;
                Console.WriteLine("Unable to open or create the debug log file!");
            }
        }

        private static bool CheckLogSizeOk() => new FileInfo(GetLogFilePath()).Length <= MaxLogSize;

        public void DeleteLogFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                Console.WriteLine("Log file exceeded maximum file length and was deleted.");
            }
            catch { Console.WriteLine("Unable to delete log file!"); }
        }

        private void CheckAndDeleteLogFile()
        {
            var filePath = GetLogFilePath();
            if (File.Exists(filePath) && !CheckLogSizeOk())
            {
                DeleteLogFile(filePath);
            }
        }

        public static string GetLogFilePath() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ACSE",
                $"{LogFileName}.txt");

        public void WriteLine(string contents, DebugLevel level = DebugLevel.Info)
        {
            if (_logWriter == null || level > Level) return;
            if (!CheckLogSizeOk())
            {
                CloseDebugLogWriter();
                DeleteLogFile(GetLogFilePath());
                InitializeDebugLogWriter();
            }

            _logWriter.WriteLine(
                $"[{level}] - ({(SaveFile != null ? SaveFile.SaveType.ToString().Replace("_", " ") : "No Save")}) - {DateTime.Now} => {contents}");
            _logWriter.Flush();
        }
    }
}
