namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    class AutoSameForSimularForce : AutoSameForSimularBase
    {
        protected override bool IsValidRow(RowData rowData)
        {
            return !string.IsNullOrEmpty(Original);
        }

        protected override bool IsForce => true; // force set values
    }
}
