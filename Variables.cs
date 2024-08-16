using System;
using System.IO;
using System.Reflection;

namespace DesktopBackupper {
    internal class Variables {

        public static readonly string programLocation = Path.GetDirectoryName(
            Assembly.GetEntryAssembly().Location
        );
        public static readonly string desktopLocation = Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop
        );

        public static readonly string excludesFile = "excludes.txt";
        public static readonly string configFile = "config.ini";
    }
}
