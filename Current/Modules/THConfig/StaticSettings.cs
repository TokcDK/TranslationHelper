using System.IO;

namespace THConfig
{
    public class StaticSettings
    {
        public static string ApplicationExeName;
        public static string ApplicationStartupPath;
        public static string ApplicationIniPath { get => Path.Combine(ApplicationStartupPath, ApplicationExeName + ".ini"); }
    }
}
