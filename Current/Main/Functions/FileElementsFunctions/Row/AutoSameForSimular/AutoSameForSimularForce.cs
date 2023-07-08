namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    class AutoSameForSimularForce : AutoSameForSimularBase
    {
        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return !string.IsNullOrEmpty(rowData.Original);
        }

        protected override bool IsForce => true; // force set values
    }
}
