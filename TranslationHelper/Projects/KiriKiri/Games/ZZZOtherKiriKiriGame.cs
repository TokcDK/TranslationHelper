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

        internal override bool Open()
        {
            return ExtractXP3Data()
                && OpenFiles(new System.Collections.Generic.List<Formats.FormatBase>
                {
                    new Formats.KiriKiri.Games.KS(thDataWork)
                }
                );
        }

        internal override bool Save()
        {
            return SaveFiles(new System.Collections.Generic.List<Formats.FormatBase>
            {
                new Formats.KiriKiri.Games.KS(thDataWork)
            }
            );
        }
    }
}
