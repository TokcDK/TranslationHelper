using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SetOriginalToTranslation : RowBase
    {
        public SetOriginalToTranslation()
        {
        }
        protected override bool Apply(RowData rowData)
        {
            try
            {
                Translation = Original;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
