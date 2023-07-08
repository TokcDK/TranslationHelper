namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class TrimEndSpace : RowBase
    {
        protected override bool Apply(RowBaseRowData rowData)
        {
            rowData.SelectedRow[1] = (rowData.SelectedRow[1] + "").TrimEnd();

            return true;
        }
    }
}
