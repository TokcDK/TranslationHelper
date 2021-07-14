using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class ZZZOtherKiriKiriGame : KiriKiriGameBase
    {
        public ZZZOtherKiriKiriGame(ProjectData projectData) : base(projectData)
        {
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase();
        }

        internal override string Name()
        {
            return Path.GetFileName(Properties.Settings.Default.THProjectWorkDir) + "(" + projectData.SPath + ")";
        }

        protected override List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase>
            {
                new Formats.KiriKiri.Games.KS(projectData),
                new Formats.KiriKiri.Games.CSV.CSV(projectData)
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.csv" };
        }
    }
}
