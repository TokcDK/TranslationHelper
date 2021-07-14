using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class ProjectSpecificFixes : HardFixesBase
    {
        public ProjectSpecificFixes(ProjectData projectData) : base(projectData)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";
            var newtranslation = projectData.CurrentProject.HardcodedFixes(SelectedRow[ColumnIndexOriginal] as string, translation);
            if (newtranslation != translation)
            {
                SelectedRow[ColumnIndexTranslation] = newtranslation;
                return true;
            }
            return false;
        }
    }
}
