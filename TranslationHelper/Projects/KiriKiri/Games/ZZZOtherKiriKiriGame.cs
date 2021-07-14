using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class ZZZOtherKiriKiriGame : KiriKiriGameBase
    {
        public ZZZOtherKiriKiriGame() : base()
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

        protected override List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase>
            {
                new Formats.KiriKiri.Games.KS(),
                new Formats.KiriKiri.Games.CSV.CSV()
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.csv" };
        }
    }
}
