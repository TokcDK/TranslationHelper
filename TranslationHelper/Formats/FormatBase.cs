using TranslationHelper.Data;

namespace TranslationHelper.Formats
{
    abstract class FormatBase
    {
        protected THDataWork thDataWork;

        protected FormatBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal abstract bool Open();

        internal abstract bool Save();

        protected void AddTables(string tablename, string[] extraColumns = null)
        {
            if (!thDataWork.THFilesElementsDataset.Tables.Contains(tablename))
            {
                thDataWork.THFilesElementsDataset.Tables.Add(tablename);
                thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add("Original");
                thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add("Translation");

                if (extraColumns != null && extraColumns.Length > 0)
                {
                    foreach (var columnName in extraColumns)
                    {
                        thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add(columnName);
                    }
                }
            }
            if (!thDataWork.THFilesElementsDatasetInfo.Tables.Contains(tablename))
            {
                thDataWork.THFilesElementsDatasetInfo.Tables.Add(tablename);
                thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Columns.Add("Info");
            }
        }

        protected bool CheckTablesContent(string tablename)
        {
            if (thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                if (thDataWork.THFilesElementsDataset.Tables.Contains(tablename))
                {
                    thDataWork.THFilesElementsDataset.Tables.Remove(tablename); // remove table if was no items added
                }

                if (thDataWork.THFilesElementsDatasetInfo.Tables.Contains(tablename))
                {
                    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(tablename); // remove table if was no items added
                }

                return false;
            }
        }
    }
}
