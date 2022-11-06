using System.IO;
using THConfig.Interfaces;

namespace THConfig
{
    public class StaticSettings
    {
        public static IAppSettings Settings = null;

        public static string ApplicationProductName;
        public static string ApplicationStartupPath;

        internal static string AppIniPath = "";
        public static string ApplicationIniPath { get => string.IsNullOrWhiteSpace(AppIniPath) ? Path.Combine(ApplicationStartupPath, ApplicationProductName + ".ini") : AppIniPath; }
    }
}
