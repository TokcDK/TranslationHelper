namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class TrimEndSpace : RowBase
    {
        protected override bool Apply(RowData rowData)
        {
            SelectedRow[1] = (SelectedRow[1] + "").TrimEnd();

            return true;
        }
    }
}
