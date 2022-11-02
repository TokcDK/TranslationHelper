namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class TrimEndSpace : RowBase
    {
        protected override bool Apply()
        {
            SelectedRow[1] = (SelectedRow[1] + "").TrimEnd();

            return true;
        }
    }
}
