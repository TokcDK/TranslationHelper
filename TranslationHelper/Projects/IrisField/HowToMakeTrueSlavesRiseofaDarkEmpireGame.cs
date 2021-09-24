using System;
using TranslationHelper.Formats.IrisField;

namespace TranslationHelper.Projects.IrisField
{
    class HowToMakeTrueSlavesRiseofaDarkEmpireGame : IrisFieldGameBase
    {
        internal override string Name()
        {
            return "How to make true slaves -Rise of a Dark Empire-";
        }

        protected override string GameExeName => "正しい性奴隷の使い方";

        protected override Type GameExeFormatType => typeof(HowToMakeTrueSlavesRiseofaDarkEmpireExe);
    }
}
