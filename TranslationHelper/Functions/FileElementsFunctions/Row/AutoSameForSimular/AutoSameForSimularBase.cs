using TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class AutoSameForSimularBase : RowBase
    {
        protected override bool IsValidRow()
        {
            return !string.IsNullOrEmpty(SelectedRow[1] + ""); // not empty original translation
        }

        protected virtual bool IsForce => false;
        protected override bool Apply()
        {
            try
            {
                AutoSameForSimularUtils.Set(inputTableIndex: SelectedTableIndex, inputRowIndex: SelectedRowIndex, inputForceSetValue: IsForce);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
