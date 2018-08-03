using System.IO;
using System.Linq;

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

        internal string GetBackupFileName(Save saveFile)
        {
            var saveFileName = saveFile.SaveName + "_Backup_";
            var backupsLocation = GetBackupLocation();
            var backupNumber = 0;
            while (File.Exists(backupsLocation + Path.DirectorySeparatorChar + saveFileName + backupNumber + saveFile.SaveExtension))
                backupNumber++;

            return backupsLocation + Path.DirectorySeparatorChar + saveFileName + backupNumber + saveFile.SaveExtension;
        }

        public Backup(Save saveFile)
        {
            var backupsDirectory = GetBackupDirectory();
            if (!backupsDirectory.Exists) return;
            var backupLocation = GetBackupFileName(saveFile);
            try
            {
                using (var backupFile = File.Create(backupLocation))
                {
                    backupFile.Write(saveFile.OriginalSaveData, 0, saveFile.OriginalSaveData.Length);
                    MainForm.DebugManager.WriteLine($"Save File {saveFile.SaveName} was backuped to {backupLocation}");
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
