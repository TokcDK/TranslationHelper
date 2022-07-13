using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Data
{
    public static class AppSettings
    {
        public static string ApplicationStartupPath { get; set; }
        public static bool AutotranslationForSimular { get; set; } = true;
        public static bool IsFullComprasionDBloadEnabled { get; set; } = false;
        public static bool DontLoadStringIfRomajiPercentForOpen { get; set; } = true;
        public static bool DontLoadStringIfRomajiPercentForTranslation { get; set; } = true;
        public static bool DontLoadStringIfRomajiPercent { get; set; } = false;
        public static int DontLoadStringIfRomajiPercentNumber { get; set; } = 90;
    }
}
