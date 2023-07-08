using System.Data;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class ProjectSpecificFixes : HardFixesBase
    {
        public ProjectSpecificFixes()
        {
        }

        protected override bool Apply(RowData rowData)
        {
            var translation = Translation;
            var newtranslation = AppData.CurrentProject.HardcodedFixes(Original, translation);
            if (newtranslation != translation)
            {
                Translation = newtranslation;
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
