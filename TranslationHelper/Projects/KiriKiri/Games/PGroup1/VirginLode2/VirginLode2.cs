using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.VirginLode2
{
    class VirginLode2 : PGroup1Base
    {
        public VirginLode2(THDataWork thDataWork) : base(thDataWork)
        {
            exeCRC = "dacf4898da60741356cc5c254774e5cb";
        }

        internal override string Name()
        {
            return "Virgin Lode 2";
        }

        internal override bool Check()
        {
            if (CheckKiriKiriBase())
            {
                if (exeCRC.Length > 0 && thDataWork.SPath.GetMD5() == exeCRC)
                {
                    return true;
                }
            }
            return false;
        }

        protected override Formats.FormatBase Format()
        {
            return new TranslationHelper.Formats.KiriKiri.Games.FGroup1.VirginLode2.KS(thDataWork);
        }
    }
}
