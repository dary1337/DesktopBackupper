using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesktopBackupper {
    internal class Settings {

        public static class Keys {

            public const string _BackupFrom = "# If you want, you can use any other folder";
            public const string BackupFrom = "backupFrom";

            public const string BackupTo = "backupTo";

            public const string _compressToArchive = "# Compress backup to .zip instead of copying folder";
            public const string compressToArchive = "compressToArchive";

            public const string _SkippedEcho = "# See every skipped folder path";
            public const string skippedEcho = "skippedEcho";

            public const string _echoEveryCopied = "# See every copied file and folder";
            public const string Echo = "echo";
            
            public const string _closeOnFinish = "# Close the program if there were no errors during backup?";
            public const string closeOnFinish = "closeOnFinish";
        }


        public static readonly Dictionary<string, string> All = new Dictionary<string, string> {
            [Keys._BackupFrom] = "",
            [Keys.BackupFrom] = Variables.desktopLocation,
            [Keys.BackupTo] = "",
            [Keys._compressToArchive] = "",
            [Keys.compressToArchive] = "true",
            [Keys._SkippedEcho] = "",
            [Keys.skippedEcho] = "true",
            [Keys._echoEveryCopied] = "",
            [Keys.Echo] = "false",
            [Keys._closeOnFinish] = "",
            [Keys.closeOnFinish] = "true",
        };

        public static void changeExcludes(StreamWriter stream) {

            while (true) {
                Console.WriteLine($"Exclude folder name from backup (just press Enter if u end):");
                string excludeFolder = Console.ReadLine().Trim();

                if (excludeFolder == "")
                    break;

                stream.WriteLine(excludeFolder);
                Directories.excludes.Append(excludeFolder);
                Console.WriteLine($"Created exclude: {excludeFolder}");
            }
        }

        public static void setExcludes() {

            Console.WriteLine();

            string fileName = $"{Variables.programLocation}/{Variables.excludesFile}";

            if (!File.Exists(fileName)) {

                using (var stream = File.CreateText(fileName)) {
                    Console.WriteLine($"Creating: {Variables.excludesFile}");

                    changeExcludes(stream);
                }
                Directories.excludes = new string[] { };
                return;
            }

            Directories.excludes = File.ReadAllLines(fileName);
            if (Directories.excludes.Length == 0) {
                Console.WriteLine("Excludes is empty. Backups will be created for each folder");
                return;
            }

            Console.WriteLine("Loaded excludes:");
            foreach (string exclude in Directories.excludes)
                Console.WriteLine(exclude);
        }

        public static void changeBackupTo() {
            string path = "";

            while (path == "" && !Directory.Exists(path)) {
                Console.WriteLine("[backupTo] path:");
                path = Console.ReadLine().Replace("\"", "").Trim();
            }

            All[Keys.BackupTo] = path;
            corruptedConfig($"Deleting {Variables.configFile}");
        }

        public static void setSettings() {
            Console.WriteLine();

            string fileName = $"{Variables.programLocation}/{Variables.configFile}";

            var defaultKeys = All.Keys.ToList();

            if (!File.Exists(fileName)) {
                using (var stream = File.CreateText(fileName)) {
                    Console.WriteLine($"Creating: {Variables.configFile}");

                    foreach (var key in defaultKeys) {
                        if (key.StartsWith("#")) {
                            stream.WriteLine(key);
                            continue;
                        }

                        string value = All[key];

                        stream.WriteLine($"{key}={value}");
                        All[key] = value;
                        Console.WriteLine($"Created setting: {key}={value}");
                    }
                }
                return;
            }

            var keys = All.Keys.ToList();
            int loadedValues = 0;

            foreach (var line in File.ReadAllLines(fileName)) {
                if (line.StartsWith("#")) {
                    loadedValues++;
                    continue;
                }

                foreach (var key in keys) {
                    if (line.StartsWith($"{key}=")) {
                        string value = line.Replace($"{key}=", "");

                        if (value == "")
                            throw new Exception("Setting can't be empty");

                        All[key] = value;
                        loadedValues++;
                        Console.WriteLine($"Loaded setting: {key}={value}");
                    }
                }
            }

            if (keys.Count != loadedValues)
                corruptedConfig($"Not all keys were found. Recreating {Variables.configFile}");
        }

        static void corruptedConfig(string whatsWrong) {
            Console.WriteLine(whatsWrong);
            File.Delete($"{Variables.programLocation}/{Variables.configFile}");
            setSettings();
        }
    }
}
