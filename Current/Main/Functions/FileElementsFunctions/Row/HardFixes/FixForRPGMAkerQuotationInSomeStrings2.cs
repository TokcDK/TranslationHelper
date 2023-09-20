using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;

using TranslationHelper.Functions.StringChangers.HardFixes;
namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForRpgmAkerQuotationInSomeStrings2 : HardFixesBase
    {
        protected override StringChangerBase Changer => new FixForRpgmAkerQuotationInSomeStrings2Changer();
    }
}
