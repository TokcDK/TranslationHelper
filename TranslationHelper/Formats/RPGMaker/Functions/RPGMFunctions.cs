using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static string THRPGMTransPatchver
        {
            get => Properties.Settings.Default.THRPGMTransPatchver;
            set => Properties.Settings.Default.THRPGMTransPatchver = value;
        }

        public static string THSelectedSourceType
        {
            get => Properties.Settings.Default.THSelectedSourceType;
            set => Properties.Settings.Default.THSelectedSourceType = value;
        }
    }
}
