using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ACSE
{
    public class Backup
    {
        public static string[] GetBackups()
        {
            var backupsDirectory = GetBackupDirectory();
            return backupsDirectory.Exists ? backupsDirectory.GetFiles().Select(x => x.FullName).ToArray() : new string[0];
        }

        internal static string GetBackupLocation()
            => MainForm.AssemblyLocation + Path.DirectorySeparatorChar + "ACSE Backups";

        internal static DirectoryInfo GetBackupDirectory()
            => Directory.CreateDirectory(GetBackupLocation());

        private static string GetSaveHash(in byte[] data)
        {
            using (var hash = SHA256.Create())
            {
                var hashData = hash.ComputeHash(data);

                var stringBuilder = new StringBuilder();
                foreach (var b in hashData)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        private static string GetHashFromFileName(string name)
        {
            var hashStartIndex = name?.LastIndexOf("_hash_");
            if (hashStartIndex.HasValue && hashStartIndex.Value > -1)
            {
                return name.Substring(hashStartIndex.Value + 6);
            }

            return "";
        }

        internal static (string, bool) GetBackupFileName(Save saveFile)
        {
            var saveFileName = GetBackupLocation() + Path.DirectorySeparatorChar + saveFile.SaveName + "_Backup" +
                               $"_hash_{GetSaveHash(saveFile.SaveData)}" + saveFile.SaveExtension;
            return (saveFileName, File.Exists(saveFileName));
        }

        public Backup(Save saveFile)
        {
            var backupsDirectory = GetBackupDirectory();
            if (!backupsDirectory.Exists) return;
            var backupLocation = GetBackupFileName(saveFile);
            try
            {
                var (saveLocation, fileExists) = GetBackupFileName(saveFile);
                if (fileExists) return;

                using (var backupFile = File.Create(saveLocation))
                {
                    backupFile.Write(saveFile.SaveData, 0, saveFile.SaveData.Length);
                    MainForm.DebugManager.WriteLine(
                        $"Save File {saveFile.SaveName} was backuped to {backupLocation}");
                }
            }
            catch
            {
                MainForm.DebugManager.WriteLine(
                    $"Failed to create backup for save {saveFile.SaveName} at {backupLocation}", DebugLevel.Error);
            }
        }
    }
}
