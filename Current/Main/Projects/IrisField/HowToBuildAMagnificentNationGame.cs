using System;
using TranslationHelper.Formats.IrisField;

namespace TranslationHelper.Projects.IrisField
{
    class HowToBuildAMagnificentNationGame : IrisFieldGameBase
    {
        public override string Name => "How to build a magnificent nation";

        protected override string GameExeName => "素晴らしき国家の築き方";

        protected override Type GameExeFormatType => typeof(HowToBuildAMagnificentNationExe);

        internal override int MaxLineLength => 80;
    }
}
