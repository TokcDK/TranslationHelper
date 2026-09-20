using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class ProjectSpecificFixesChanger : StringChangerBase
    {
        internal override string Description => "Project specific hard fixes";

        public ProjectSpecificFixesChanger()
        {
        }

        internal override string Change(string inputString, object extraData)
        {
            //extraData is the original cell value (see HardFixesBase.Apply). The previous code passed
            //the translation as the original, so the project specific fixes never saw the original
            //text they are supposed to inspect.
            var original = extraData as string;
            if (original == null) return inputString;

            var newStr = AppData.CurrentProject.HardcodedFixes(original, inputString);
            return newStr != inputString ? newStr : inputString;
        }
    }
}
