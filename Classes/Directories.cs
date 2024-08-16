using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DesktopBackupper {
    internal class Directories {

        public static string[] excludes = new string[] { };

        public static int errors = 0;

        static int progress = 0;
        static int total;


        public static void setTotal(string from) {
            total = new DirectoryInfo(from).GetDirectories().Length;
        }

        public static void archive(string sourceDir, string destinationZip, bool recursive) {
            Console.WriteLine($"Creating archive: {destinationZip}");

            using (var zip = ZipFile.Open(destinationZip, ZipArchiveMode.Create))
                AddDirectoryToZip(zip, sourceDir, recursive, sourceDir);
        }

        public static void copy(string sourceDir, string destinationDir, bool recursive) {

            Console.WriteLine($"Creating folder: {destinationDir}");

            copyRecursive(sourceDir, destinationDir, recursive);
        }

        static string GetRelativePath(string baseDir, string fullPath) {
            Uri baseUri = new Uri(baseDir.EndsWith(Path.DirectorySeparatorChar.ToString()) ? baseDir : baseDir + Path.DirectorySeparatorChar);
            Uri fullUri = new Uri(fullPath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        static void AddDirectoryToZip(ZipArchive zip, string sourceDir, bool recursive, string baseDir) {
            var dir = new DirectoryInfo(sourceDir);


            bool echo = Settings.All[Settings.Keys.Echo] == "true";
            bool skipped = Settings.All[Settings.Keys.skippedEcho] == "true";
            bool isSourceDir = sourceDir == Settings.All[Settings.Keys.BackupFrom];


            foreach (FileInfo file in dir.GetFiles()) {
                try {

                    string relativePath = GetRelativePath(baseDir, file.FullName);
                    zip.CreateEntryFromFile(file.FullName, relativePath, CompressionLevel.Optimal);

                    if (echo)
                        Console.WriteLine($"File added to arhive: {sourceDir}\\{file.Name}");
                }
                catch (Exception exception) {
                    Console.WriteLine($"Can't archive file: {exception.Message}");
                    errors++;
                }
            }

            if (recursive) {
                foreach (DirectoryInfo subDir in dir.GetDirectories()) {

                    try {
                        if (excludes.Contains(subDir.Name)) {

                            if (skipped)
                                Console.WriteLine($"Skipped: {subDir.FullName}");

                            continue;
                        }

                        AddDirectoryToZip(zip, subDir.FullName, true, baseDir);

                        if (echo)
                            Console.WriteLine($"Folder added to arhive: {subDir.FullName}");

                        if (isSourceDir)
                            Console.WriteLine(
                                $"Progress: {subDir.FullName} ({++progress}/{total}) folder archived.\n"
                            );
                    }
                    catch (Exception exception) {
                        Console.WriteLine($"Can't archive directory: {exception.Message}");
                        errors++;
                    }
                }
            }
        }

        static void copyRecursive(string sourceDir, string destinationDir, bool recursive) {
            {
                var dir = new DirectoryInfo(sourceDir);

                DirectoryInfo[] dirs = dir.GetDirectories();

                Directory.CreateDirectory(destinationDir);

                bool echo = Settings.All[Settings.Keys.Echo] == "true";
                bool skipped = Settings.All[Settings.Keys.skippedEcho] == "true";
                bool isSourceDir = sourceDir == Settings.All[Settings.Keys.BackupFrom];

                foreach (FileInfo file in dir.GetFiles()) {
                    try {
                        string targetFilePath = Path.Combine(destinationDir, file.Name);
                        file.CopyTo(targetFilePath);

                        if (echo)
                            Console.WriteLine($"Copied file: {sourceDir}\\{file.Name}");
                    }
                    catch (Exception exception) {
                        Console.WriteLine($"Can't copy file: {exception.Message}");
                        errors++;
                    }
                }

                if (recursive)
                    foreach (DirectoryInfo subDir in dirs) {
                        try {
                            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);

                            if (excludes.Contains(subDir.Name)) {

                                if (skipped)
                                    Console.WriteLine($"Skipped: {subDir.FullName}");

                                continue;
                            }

                            copy(subDir.FullName, newDestinationDir, true);

                            if (echo)
                                Console.WriteLine($"Copied directory: {subDir.FullName}");

                            if (isSourceDir)
                                Console.WriteLine(
                                $"Progress: {dirs[progress].FullName} ({++progress}/{total}) folder copied.\n"
                                );

                        }
                        catch (Exception exception) {
                            Console.WriteLine($"Can't copy directory: {exception.Message}");
                            errors++;
                        }
                    }
            }
        }

    }
}
