using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ACSE.Core.Debug;
using ACSE.Core.Saves;
using ACSE.Core.Utilities;

namespace ACSE.WinForms.Backups
{
    public sealed class Backup
    {
        private readonly Save _save;

        public static string[] GetBackups()
        {
            var backupsDirectory = GetBackupDirectory();
            return backupsDirectory.Exists ? backupsDirectory.GetFiles().Select(x => x.FullName).ToArray() : new string[0];
        }

        private static string GetBackupLocation()
        {
            // TODO: Should we check if the path is on an invalid drive?
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.BackupLocation))
            {
                Properties.Settings.Default.BackupLocation =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ACSE",
                        "Backups");
                Properties.Settings.Default.Save();
            }

            // Attempt to create the full path to the backup directory. Reset to default path if failed.
            // TODO: Should this be switched to throw an exception and disable backups? User would need to be notified.
            try
            {
                Directory.CreateDirectory(Properties.Settings.Default.BackupLocation);
            }
            catch
            {
                DebugUtility.DebugManagerInstance.WriteLine(
                    $"The requested backup path {Properties.Settings.Default.BackupLocation} couldn't be accessed!\n" +
                    "The backup folder was reset to the default in ApplicationData/ACSE/Backups/!");
                Properties.Settings.Default.BackupLocation = "";
                Properties.Settings.Default.Save();
                return GetBackupLocation();
            }

            return Properties.Settings.Default.BackupLocation;
        }

        private static DirectoryInfo GetBackupDirectory()
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

        public bool CreateBackup()
        {
            // TODO: Do we want to store the date-time in the backup name?
            var backupsDirectory = GetBackupDirectory();
            if (!backupsDirectory.Exists) return false;
            var backupLocation = GetBackupFileName(_save);
            try
            {
                var (saveLocation, fileExists) = GetBackupFileName(_save);
                if (fileExists) return false;

                using (var backupFile = File.Create(saveLocation))
                {
                    backupFile.Write(_save.SaveData, 0, _save.SaveData.Length);
                    DebugUtility.DebugManagerInstance.WriteLine(
                        $"Save File {_save.SaveName} was backuped to {backupLocation}");
                }
            }
            catch
            {
                DebugUtility.DebugManagerInstance.WriteLine(
                    $"Failed to create backup for save {_save.SaveName} at {backupLocation}", DebugLevel.Error);
                return false;
            }

            return true;
        }

        public Backup(Save saveFile)
        {
            _save = saveFile;

            // Attempt to create an initial backup
            CreateBackup();
        }
    }
}
