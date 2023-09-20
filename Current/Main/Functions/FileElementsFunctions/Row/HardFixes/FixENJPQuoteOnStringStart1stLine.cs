using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixEnjpQuoteOnStringStart1StLine : HardFixesBase
    {
        protected override StringChangerBase Changer => new FixEnjpQuoteOnStringStart1StLineChanger();
    }
}
