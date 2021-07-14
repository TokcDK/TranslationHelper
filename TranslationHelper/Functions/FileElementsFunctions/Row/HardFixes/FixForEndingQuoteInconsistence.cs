using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForEndingQuoteInconsistence : HardFixesBase
    {
        public FixForEndingQuoteInconsistence(ProjectData projectData) : base(projectData)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";
            var original = SelectedRow[ColumnIndexOriginal] as string;
            if (translation.Length > 1 && translation[translation.Length - 1] == '"' && original.Length > 0 && original[original.Length - 1] != '"')
            {
                SelectedRow[ColumnIndexTranslation] = translation.Remove(translation.Length - 1, 1) + original[original.Length - 1];
                return true;
            }
            return false;
        }
    }
}
