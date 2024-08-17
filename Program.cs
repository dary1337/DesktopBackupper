using System;
using System.IO;

namespace DesktopBackupper {

    internal class Program {

        static void Main(string[] args) {
            Console.WriteLine("Author: yuri, Github: https://github.com/dary1337");

            Settings.setSettings();
            Settings.setExcludes();

            Console.WriteLine();
            backup();

            if (Directories.errors != 0) {
                Console.WriteLine($"Backup complete, errors: {Directories.errors}");
                return;
            }

            if (Settings.All[Settings.Keys.closeOnFinish] == "true") {
                Environment.Exit(0);
                return;
            }

            Console.WriteLine("No errors, u can close the program");
            Console.ReadKey();
        }

        static void backup() {
            Console.WriteLine($"Starting backup...");

            string backupFrom = Settings.All[Settings.Keys.BackupFrom];
            string backupTo = Settings.All[Settings.Keys.BackupTo];

            if (backupTo == "")
                Settings.changeBackupTo();

            if (backupFrom == backupTo) {
                Console.WriteLine("[backupFrom] and [backupTo] can't be the same!");
                Settings.changeBackupTo();

                backup();
                return;
            }

            if (!Directory.Exists(backupFrom)) {
                Console.WriteLine("[backupFrom] value contains not existed folder");
                Settings.changeBackupTo();
            }

            string dateName = DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss");

            Directories.setTotal(backupFrom);

            string folderName = new DirectoryInfo(backupFrom).Name;

            string path = $"{(backupTo + "\\").Replace(@"\\", "\\")}{folderName}_{dateName}";

            if (Settings.All[Settings.Keys.compressToArchive] == "true")
                Directories.archive(backupFrom,
                    $"{path}.zip",
                    true);
            else
                Directories.copy(
                    backupFrom,
                    path,
                    true
                );
        }


    }
}
