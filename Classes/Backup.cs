using System.IO;
using System.Linq;

namespace ACSE
{
    public class Backup
    {
        public static string[] GetBackups()
        {
            var BackupsDirectory = GetBackupDirectory();
            if (BackupsDirectory.Exists)
            {
                return BackupsDirectory.GetFiles().Select(x => x.FullName).ToArray();
            }
            return new string[0];
        }

        internal static string GetBackupLocation()
            => MainForm.Assembly_Location + Path.DirectorySeparatorChar + "ACSE Backups";

        internal static DirectoryInfo GetBackupDirectory()
            => Directory.CreateDirectory(GetBackupLocation());

        internal string GetBackupFileName(Save SaveFile)
        {
            string SaveFileName = SaveFile.Save_Name + "_Backup_";
            string BackupsLocation = GetBackupLocation();
            int BackupNumber = 0;
            while (File.Exists(BackupsLocation + Path.DirectorySeparatorChar + SaveFileName + BackupNumber + SaveFile.Save_Extension))
                BackupNumber++;

            return BackupsLocation + Path.DirectorySeparatorChar + SaveFileName + BackupNumber + SaveFile.Save_Extension;
        }

        public Backup(Save SaveFile)
        {
            var BackupsDirectory = GetBackupDirectory();
            if (BackupsDirectory.Exists)
            {
                string BackupLocation = GetBackupFileName(SaveFile);
                try
                {
                    using (var BackupFile = File.Create(BackupLocation))
                    {
                        BackupFile.Write(SaveFile.Original_Save_Data, 0, SaveFile.Original_Save_Data.Length);
                        MainForm.Debug_Manager.WriteLine(string.Format("Save File {0} was backuped to {1}", SaveFile.Save_Name, BackupLocation), DebugLevel.Info);
                    }
                }
                catch
                {
                    MainForm.Debug_Manager.WriteLine(string.Format("Failed to create backup for save {0} at {1}", SaveFile.Save_Name, BackupLocation), DebugLevel.Error);
                }
            }
        }
    }
}
