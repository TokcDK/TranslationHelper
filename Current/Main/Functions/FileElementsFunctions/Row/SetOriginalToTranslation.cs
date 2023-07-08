using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SetOriginalToTranslation : RowBase
    {
        public SetOriginalToTranslation()
        {
        }
        protected override bool Apply(RowBaseRowData rowData)
        {
            try
            {
                rowData.Translation = rowData.Original;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
