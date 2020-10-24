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

        protected override Formats.FormatBase Format()
        {
            return new Formats.KiriKiri.Games.KS(thDataWork);
        }
    }
}
