using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.RJ297684GoblinsCave
{
    class RJ297684GoblinsCave : PGroup1Base
    {
        public RJ297684GoblinsCave(THDataWork thDataWork) : base(thDataWork)
        {
            exeCRC = "7c2bfd95";
        }

        internal override string Name()
        {
            return "[RJ297684]Goblins Cave";
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase() && exeCRC.Length > 0 && thDataWork.SPath.GetCrc32() == exeCRC;
        }

        protected override Formats.FormatBase Format()
        {
            return new Formats.KiriKiri.Games.FGroup1.RJ297684GoblinsCave.KS(thDataWork);
        }
    }
}
