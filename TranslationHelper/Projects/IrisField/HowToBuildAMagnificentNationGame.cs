using System;
using TranslationHelper.Formats.IrisField;

namespace TranslationHelper.Projects.IrisField
{
    class HowToBuildAMagnificentNationGame : IrisFieldGameBase
    {
        internal override string Name()
        {
            return "How to build a magnificent nation";
        }

        protected override string GameExeName => "素晴らしき国家の築き方";

        protected override Type GameExeType => typeof(HowToBuildAMagnificentNationExe);
    }
}
