namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class TrimEndSpace : RowBase
    {
        protected override bool Apply(RowBaseRowData rowData)
        {
            //Uses the translation accessor like every other row function: it resolves the column from
            //the project instead of assuming column 1, and it goes through the UI updater.
            rowData.Translation = (rowData.Translation + string.Empty).TrimEnd();

            return true;
        }
    }
}
