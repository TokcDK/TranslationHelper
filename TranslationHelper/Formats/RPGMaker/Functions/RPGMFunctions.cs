using System.IO;

namespace TranslationHelper.Formats.RPGMaker.Functions
{
    public static class RpgmFunctions
    {
        public static string GetRpgMakerArc(string inputDir)
        {
            string path = Path.Combine(inputDir, "Game.rgss3a");
            if (File.Exists(path))
            {
                return path;
            }
            return string.Empty;
        }

        public static string RpgmTransPatchVersion
        {
            get => Properties.Settings.Default.RPGMTransPatchVersion;
            set => Properties.Settings.Default.RPGMTransPatchVersion = value;
        }

        public static string ThSelectedSourceType
        {
            get => Properties.Settings.Default.THSelectedSourceType;
            set => Properties.Settings.Default.THSelectedSourceType = value;
        }
    }
}
