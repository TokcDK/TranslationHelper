using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMaker.Functions
{
    public static class RPGMFunctions
    {
        public static string GetRPGMakerArc(string inputDir)
        {
            string path = Path.Combine(inputDir, "Game.rgss3a");
            if (File.Exists(path))
            {
                return path;
            }
            return string.Empty;
        }

        public static string RPGMTransPatchVersion
        {
            get => AppSettings.RPGMTransPatchVersion;
            set => AppSettings.RPGMTransPatchVersion = value;
        }

        public static string THSelectedSourceType
        {
            get => AppSettings.THSelectedSourceType;
            set => AppSettings.THSelectedSourceType = value;
        }
    }
}
