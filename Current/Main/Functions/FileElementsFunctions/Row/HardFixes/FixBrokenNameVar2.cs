using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixBrokenNameVar2 : HardFixesBase
    {
        protected override StringChangerBase Changer => new FixBrokenNameVar2Changer();
    }
}
