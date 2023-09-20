using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForEndingQuoteInconsistence : HardFixesBase
    {
        protected override StringChangerBase Changer => new FixForEndingQuoteInconsistenceChanger();
    }
}
