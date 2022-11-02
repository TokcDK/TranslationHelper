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

        internal override bool IsValid()
        {
            return CheckKiriKiriBase();
        }

        public override string Name => Path.GetFileName(AppData.CurrentProject.ProjectWorkDir) + "(" + AppData.SelectedFilePath + ")";

        protected override List<System.Type> FormatType()
        {
            return new List<System.Type>
            {
                typeof(Formats.KiriKiri.Games.KS),
                typeof(Formats.KiriKiri.Games.CSV.CSV)
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.csv" };
        }
    }
}
