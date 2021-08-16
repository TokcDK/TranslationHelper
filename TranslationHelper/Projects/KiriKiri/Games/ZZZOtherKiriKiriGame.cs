using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class ZzzOtherKiriKiriGame : KiriKiriGameBase
    {
        public ZzzOtherKiriKiriGame()
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
                typeof(Formats.KiriKiri.Games.Ks),
                typeof(Formats.KiriKiri.Games.CSV.Csv)
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.csv" };
        }
    }
}
