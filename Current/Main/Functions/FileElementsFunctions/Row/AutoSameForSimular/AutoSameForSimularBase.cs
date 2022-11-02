using System;
using System.Threading.Tasks;
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
            Set();

            return true;
        }

        private async void Set()
        {
            await Task.Run(() => AutoSameForSimularUtils.Set(inputTableIndex: SelectedTableIndex, inputRowIndex: SelectedRowIndex, inputForceSetValue: IsForce)).ConfigureAwait(false);
        }
    }
}
