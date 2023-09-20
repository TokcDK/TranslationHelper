using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class ProjectSpecificFixes : HardFixesBase
    {
        protected override StringChangerBase Changer => new ProjectSpecificFixesChanger();
    }
}
