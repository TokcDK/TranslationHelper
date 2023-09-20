using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class AllHardFixes : HardFixesBase
    {
        protected override StringChangerBase Changer => new AllHardFixesChanger();
    }
}
