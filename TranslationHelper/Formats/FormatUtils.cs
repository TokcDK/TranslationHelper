using TranslationHelper.Data;

namespace TranslationHelper.Formats
{
    class FormatUtils
    {
        /// <summary>
        /// Add table to work dataset
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="extraColumns"></param>
        internal static void AddTables(string tablename, string[] extraColumns = null)
        {
            if (!ProjectData.THFilesElementsDataset.Tables.Contains(tablename))
            {
                ProjectData.THFilesElementsDataset.Tables.Add(tablename);
                ProjectData.THFilesElementsDataset.Tables[tablename].Columns.Add(THSettings.OriginalColumnName());
                ProjectData.THFilesElementsDataset.Tables[tablename].Columns.Add(THSettings.TranslationColumnName());

                if (extraColumns != null && extraColumns.Length > 0)
                {
                    foreach (var columnName in extraColumns)
                    {
                        ProjectData.THFilesElementsDataset.Tables[tablename].Columns.Add(columnName);
                    }
                }
            }
            if (!ProjectData.THFilesElementsDatasetInfo.Tables.Contains(tablename))
            {
                ProjectData.THFilesElementsDatasetInfo.Tables.Add(tablename);
                ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Columns.Add("Info");
            }
        }
    }
}
