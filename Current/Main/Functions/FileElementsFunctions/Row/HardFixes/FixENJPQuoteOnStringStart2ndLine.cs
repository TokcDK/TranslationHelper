using System;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixEnjpQuoteOnStringStart2NdLine : HardFixesBase
    {
        protected override StringChangerBase Changer => new FixEnjpQuoteOnStringStart2NdLineChanger();
    }
}
