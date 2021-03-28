using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class ZZZOtherKiriKiriGame : KiriKiriGameBase
    {
        public ZZZOtherKiriKiriGame(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase();
        }

        internal override string Name()
        {
            return Path.GetFileName(Properties.Settings.Default.THProjectWorkDir) + "(" + thDataWork.SPath + ")";
        }

        protected override List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase>
            {
                new Formats.KiriKiri.Games.KS(thDataWork),
                new Formats.KiriKiri.Games.CSV.CSV(thDataWork)
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.csv" };
        }
    }
}
