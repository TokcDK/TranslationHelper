using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixInstancesOfName : HardFixesBase
    {
        protected override StringChangerBase Changer => new FixInstancesOfNameChanger();
    }
}
