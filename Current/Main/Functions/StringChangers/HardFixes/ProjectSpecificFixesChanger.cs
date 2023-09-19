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
            var str = inputString;
            var newStr = AppData.CurrentProject.HardcodedFixes(inputString as string, str);
            if (newStr != str)
            {
                return newStr;
            }
            else return str;
        }
    }
}
