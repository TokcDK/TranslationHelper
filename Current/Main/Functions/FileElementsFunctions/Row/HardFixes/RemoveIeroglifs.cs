using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class RemoveIeroglifs : HardFixesBase
    {
        protected override StringChangerBase Changer => new RemoveIeroglifsChanger();
    }
}
