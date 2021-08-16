using System.IO;

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
            get => Properties.Settings.Default.RPGMTransPatchVersion;
            set => Properties.Settings.Default.RPGMTransPatchVersion = value;
        }

        public static string THSelectedSourceType
        {
            get => Properties.Settings.Default.THSelectedSourceType;
            set => Properties.Settings.Default.THSelectedSourceType = value;
        }
    }
}
