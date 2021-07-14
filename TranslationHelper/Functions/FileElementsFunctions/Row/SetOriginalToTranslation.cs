using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SetOriginalToTranslation : RowBase
    {
        public SetOriginalToTranslation() : base()
        {
        }
        protected override bool Apply()
        {
            try
            {
                SelectedRow[ColumnIndexTranslation] = SelectedRow[ColumnIndexOriginal];
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
