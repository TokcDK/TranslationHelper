using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class ProjectSpecificFixes : HardFixesBase
    {
        public ProjectSpecificFixes()
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";
            var newtranslation = AppData.CurrentProject.HardcodedFixes(SelectedRow[ColumnIndexOriginal] as string, translation);
            if (newtranslation != translation)
            {
                SelectedRow[ColumnIndexTranslation] = newtranslation;
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
