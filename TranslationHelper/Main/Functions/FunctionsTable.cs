
using System.Data;
using System.Windows.Forms;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsTable
    {
        public static int GetDGVSelectedRowIndexInDatatable(DataSet TargetDataSet, DataGridView InputDataGridView,  int TableIndex, int rowIndex)
        {
            return TargetDataSet.Tables[TableIndex].Rows
                .IndexOf(
                ((DataRowView)InputDataGridView.Rows[rowIndex].DataBoundItem).Row
                        );
        }

        /// <summary>
        /// Returns Dataset with tables with only non empty rows
        /// </summary>
        /// <param name="DS"></param>
        /// <returns></returns>
        public static DataSet FillTempDB(DataSet DS)
        {
            DataSet RETDS = new DataSet();
            int TablesCount = DS.Tables.Count;
            for (int t = 0; t < TablesCount; t++)
            {
                var Table = DS.Tables[t];
                string tname = Table.TableName;
                RETDS.Tables.Add(tname);
                RETDS.Tables[tname].Columns.Add("Original");
                RETDS.Tables[tname].Columns.Add("Translation");
                int RowsCount = Table.Rows.Count;
                for (int r = 0; r < RowsCount; r++)
                {
                    var Row = Table.Rows[r];
                    var CellTranslation = Row[1];
                    if (CellTranslation == null || string.IsNullOrEmpty(CellTranslation as string))
                    {
                    }
                    else
                    {
                        RETDS.Tables[tname].ImportRow(Row);
                    }
                }
                if (RETDS.Tables[tname].Rows.Count == 0)
                {
                    RETDS.Tables.Remove(tname);
                }
            }

            return RETDS;
        }

        /// <summary>
        /// true if 2 datasets tables and table rows count is identical
        /// </summary>
        /// <param name="DS1"></param>
        /// <param name="DS2"></param>
        /// <returns></returns>
        public static bool IsDataSetsElementsCountIdentical(DataSet DS1, DataSet DS2)
        {
            if (DS1.Tables.Count == DS2.Tables.Count)
            {
                for (int t = 0; t < DS1.Tables.Count; t++)
                {
                    if (DS1.Tables[t].Rows.Count != DS2.Tables[t].Rows.Count)
                    {
                        return false;
                    }
                }
                return true; ;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns false if any of Datatable row have value
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsTableRowsCompleted(DataTable DT, string column = "Translation")
        {
            if (DT == null)
            {
                return false;
            }
            int DTRowsCount = DT.Rows.Count;
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = DT.Rows[r][column];
                //LogToFile("\r\nIsTableRowsCompleted Value =\"" + DT.Rows[r][column] +"\", Length=" + ((DT.Rows[r][column] + string.Empty).Length));
                if (cell == null || string.IsNullOrEmpty(cell as string))
                {
                    //LogToFile("\r\nIsTableRowsCompleted=false");
                    return false;
                }
            }
            //THFilesElementsDataset.Tables[e.Index].AsEnumerable().All(dr => !string.IsNullOrEmpty(dr["Translation"] + string.Empty))
            //if (DT.AsEnumerable().All(datarow => !string.IsNullOrEmpty(datarow[column]+string.Empty)))
            //{
            //    return true;
            //}
            //LogToFile("\r\nIsTableRowsCompleted=true");
            return true;
        }

        /// <summary>
        /// Get rows count from all Dataset tables
        /// </summary>
        /// <param name="DS"></param>
        /// <returns></returns>
        public static int GetDatasetRowsCount(DataSet DS)
        {
            if (DS == null)
            {
                return 0;
            }

            int RowsCount = 0;

            int DTTablesCount = DS.Tables.Count;
            for (int t = 0; t < DTTablesCount; t++)
            {
                RowsCount += DS.Tables[t].Rows.Count;
            }

            return RowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Dataset tables
        /// </summary>
        /// <param name="DS"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetDatasetNonEmptyRowsCount(DataSet DS, string column = "Translation")
        {
            if (DS == null)
            {
                return 0;
            }

            int NonEmptyRowsCount = 0;

            int DTTablesCount = DS.Tables.Count;
            for (int t = 0; t < DTTablesCount; t++)
            {
                NonEmptyRowsCount += GetTableNonEmptyRowsCount(DS.Tables[t], column);
            }

            return NonEmptyRowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Table
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetTableNonEmptyRowsCount(DataTable DT, string column = "Translation")
        {
            if (DT == null)
            {
                return 0;
            }

            int NonEmptyRowsCount = 0;
            int DTRowsCount = DT.Rows.Count;
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = DT.Rows[r][column];

                if (cell == null || string.IsNullOrEmpty(cell as string))
                {
                    //LogToFile("\r\nIsTableRowsCompleted=false");
                }
                else
                {
                    NonEmptyRowsCount++;
                }
            }

            return NonEmptyRowsCount;
        }
    }
}
