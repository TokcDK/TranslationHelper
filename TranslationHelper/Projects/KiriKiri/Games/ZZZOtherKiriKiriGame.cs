using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class ZZZOtherKiriKiriGame : KiriKiriGameBase
    {
        public ZZZOtherKiriKiriGame()
        {
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase();
        }

        internal override string Name()
        {
            return Path.GetFileName(ProjectData.ProjectWorkDir) + "(" + ProjectData.SelectedFilePath + ")";
        }

        protected override List<System.Type> FormatType()
        {
            return new List<System.Type>
            {
                typeof(Formats.KiriKiri.KSParser.KS),
                typeof(Formats.KiriKiri.KSParser.CSV.CSV)
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.csv" };
        }
    }
}
