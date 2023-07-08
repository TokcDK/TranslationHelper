using System.Data;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class ProjectSpecificFixes : HardFixesBase
    {
        public ProjectSpecificFixes()
        {
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            var translation = rowData.Translation;
            var newtranslation = AppData.CurrentProject.HardcodedFixes(rowData.Original, translation);
            if (newtranslation != translation)
            {
                rowData.Translation = newtranslation;
                return true;
            }
            return false;
        }

        protected override void ActionsFinalize()
        {
            base.ActionsFinalize();
        }
    }
}
