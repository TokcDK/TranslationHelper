
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsTable
    {
        /// <summary>
        /// Will return array with real indexes of rows in dataset which currently selected in datagridview
        /// </summary>
        /// <param name="selectedRowsCount"></param>
        /// <returns></returns>
        internal static int[] GetDGVRowIndexsesInDataSetTable()
        {
            //int[] selindexes = new int[ProjectData.Main.THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount()];

            var tableIndex = 0;
            AppData.Main.Invoke((Action)(() => tableIndex = AppData.Main.THFilesList.GetSelectedIndex()));


            int[] selindexes = GetRowIndexesOfSelectedDGVCells(AppData.Main.THFileElementsDataGridView.SelectedCells);
            var selindexesLength = selindexes.Length;
            for (int i = 0; i < selindexesLength; i++)
            {
                //по нахождению верного индекса строки
                //https://stackoverflow.com/questions/50999121/displaying-original-rowindex-after-filter-in-datagridview
                //https://stackoverflow.com/questions/27125494/get-index-of-selected-row-in-filtered-datagrid
                //DataRow r = ((DataRowView)BindingContext[THFileElementsDataGridView.DataSource].).Row;
                //selindexes[i] = r.Table.Rows.IndexOf(r); //находит верный но только длявыбранной ячейки
                //
                //DataGridViewRow to DataRow: https://stackoverflow.com/questions/1822314/how-do-i-get-a-datarow-from-a-row-in-a-datagridview
                //DataRow row = ((DataRowView)THFileElementsDataGridView.SelectedCells[i].OwningRow.DataBoundItem).Row;
                //int index = THFilesElementsDataset.Tables[tableindex].Rows.IndexOf(row);
                selindexes[i] = GetDGVSelectedRowIndexInDatatable(tableIndex, selindexes[i]);

                //selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
            }

            Array.Sort(selindexes);//сортировка номеров строк, для порядка

            return selindexes;
        }

        internal static int[] GetRowIndexesOfSelectedDGVCells(DataGridViewSelectedCellCollection selectedCells)
        {
            var rowindexes = new List<int>();
            foreach (DataGridViewCell cell in selectedCells)
            {
                var rowIndex = cell.RowIndex;
                if (!rowindexes.Contains(rowIndex))
                {
                    rowindexes.Add(rowIndex);
                }
            }
            return rowindexes.ToArray();
        }

        /// <summary>
        /// shows first row where translation cell is empty
        /// </summary>
        /// <param name="projectData"></param>
        internal static void ShowFirstRowWithEmptyTranslation()
        {
            int TCount = AppData.CurrentProject.FilesContent.Tables.Count;
            for (int t = 0; t < TCount; t++)
            {
                var table = AppData.CurrentProject.FilesContent.Tables[t];

                int RCount = table.Rows.Count;
                for (int r = 0; r < RCount; r++)
                {
                    var cellValue = table.Rows[r].Field<string>(1);
                    if (string.IsNullOrEmpty(cellValue))
                    {
                        ShowSelectedRow(t, THSettings.TranslationColumnName, r);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// shows selected row in selected table
        /// </summary>
        /// <param name="projectData"></param>
        /// <param name="tableIndex"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        internal static void ShowSelectedRow(int tableIndex, string columnName, int rowIndex)
        {
            if (tableIndex == -1 || tableIndex > AppData.CurrentProject.FilesContent.Tables.Count - 1 || string.IsNullOrEmpty(columnName) || !AppData.CurrentProject.FilesContent.Tables[tableIndex].Columns.Contains(columnName))
            {
                return;
            }

            int RCount = 0;//for debug purposes
            try
            {
                RCount = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows.Count;
                if (tableIndex == AppData.Main.THFilesList.GetSelectedIndex() && RCount > 0 && AppData.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    AppData.Main.THFilesList.SetSelectedIndex(tableIndex);
                    AppData.Main.THFileElementsDataGridView.DataSource = AppData.CurrentProject.FilesContent.Tables[tableIndex];
                }

                AppData.Main.THFileElementsDataGridView.CurrentCell = AppData.Main.THFileElementsDataGridView[columnName, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                AppData.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;

                AppData.Main.UpdateTextboxes();
            }
            catch (Exception ex)
            {
                string error = "Error:" + Environment.NewLine + ex + Environment.NewLine + "rowIndex=" + rowIndex + Environment.NewLine + "tableIndex=" + tableIndex + Environment.NewLine + "table rows count=" + RCount;
                FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName), error + Environment.NewLine + Environment.NewLine);
                MessageBox.Show(error);
            }
        }

        /// <summary>
        /// shows selected row in selected table
        /// </summary>
        /// <param name="projectData"></param>
        /// <param name="tableIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        internal static void ShowSelectedRow(int tableIndex, int columnIndex, int rowIndex)
        {
            if (tableIndex == -1 || tableIndex > AppData.CurrentProject.FilesContent.Tables.Count - 1 || columnIndex == -1 || columnIndex > AppData.CurrentProject.FilesContent.Tables[tableIndex].Columns.Count - 1)
            {
                return;
            }

            int RCount = 0;//for debug purposes
            try
            {
                RCount = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows.Count;
                if (tableIndex == AppData.Main.THFilesList.GetSelectedIndex() && RCount > 0 && AppData.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    AppData.Main.THFilesList.SetSelectedIndex(tableIndex);
                    AppData.Main.THFileElementsDataGridView.DataSource = AppData.CurrentProject.FilesContent.Tables[tableIndex];

                }

                AppData.Main.THFileElementsDataGridView.CurrentCell = AppData.Main.THFileElementsDataGridView[columnIndex, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                AppData.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch (Exception ex)
            {
                string error = "Error:" + Environment.NewLine + ex + Environment.NewLine + "rowIndex=" + rowIndex + Environment.NewLine + "tableIndex=" + tableIndex + Environment.NewLine + "table rows count=" + RCount;
                FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName), error + Environment.NewLine + Environment.NewLine);
                MessageBox.Show(error);
            }
        }

        /// <summary>
        /// When table not exists it will create table with table ProjectData.TempPath name and Original Column<br>
        /// When table is exists it will remove table if no rows there else will create Translation column
        /// </summary>
        /// <param name="projectData"></param>
        /// <returns></returns>
        public static bool SetTableAndColumns(string filePath, bool add = true)
        {
            if (filePath.Length == 0)
                return false;

            string fileName = Path.GetFileName(filePath);

            if (add && !AppData.CurrentProject.FilesContent.Tables.Contains(fileName))
            {
                _ = AppData.CurrentProject.FilesContent.Tables.Add(fileName);
                _ = AppData.CurrentProject.FilesContent.Tables[fileName].Columns.Add(THSettings.OriginalColumnName);
                _ = AppData.CurrentProject.FilesContentInfo.Tables.Add(fileName);
                _ = AppData.CurrentProject.FilesContentInfo.Tables[fileName].Columns.Add(THSettings.OriginalColumnName);

                return true;
            }
            else
            {
                if (AppData.CurrentProject.FilesContent.Tables[fileName].Rows.Count == 0)
                {
                    AppData.CurrentProject.FilesContent.Tables.Remove(fileName);
                    AppData.CurrentProject.FilesContentInfo.Tables.Remove(fileName);
                    return false;
                }
                else
                {
                    _ = AppData.CurrentProject.FilesContent.Tables[fileName].Columns.Add(THSettings.TranslationColumnName);
                    return true;
                }
            }
        }

        /// <summary>
        /// True if Translation cell of any row has value
        /// </summary>
        /// <param name="DS"></param>
        /// <returns></returns>
        public static bool TheDataSetIsNotEmpty(DataSet DS)
        {
            if (DS == null)
            {
                return false;
            }

            try
            {
                int DSTablesCount = DS.Tables.Count;
                for (int t = 0; t < DSTablesCount; t++)
                {
                    var table = DS.Tables[t];
                    int rowscount = table.Rows.Count;
                    for (int r = 0; r < rowscount; r++)
                    {
                        var cell = table.Rows[r].Field<string>(1);
                        if (!string.IsNullOrEmpty(cell))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        public static string TranslationCacheFind(DataSet dataSet, string inputString)
        {
            if (!AppSettings.EnableTranslationCache)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(inputString) || dataSet == null)
            {
                return string.Empty;
            }
            using (var table = dataSet.Tables[0])
            {
                if (!GetAlreadyAddedInTableAndTableHasRowsColumns(table, inputString))
                {
                    return string.Empty;
                }
                var rowsCount = table.Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    if (Equals(inputString, table.Rows[i].Field<string>(0)))
                    {
                        return table.Rows[i].Field<string>(1);
                    }
                }
                return string.Empty;
            }
        }

        public static DataTable RemoveAllRowsDuplicatesWithRepeatingOriginals(DataTable table)
        {
            //using (DataTable tempTable = new DataTable())
            {
                DataTable tempTable = new DataTable();
                foreach (DataColumn column in table.Columns)
                {
                    tempTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in table.Rows)
                {
                    if (!GetAlreadyAddedInTableAndTableHasRowsColumns_Slower(tempTable, row.Field<string>(0)))
                    {
                        tempTable.ImportRow(row);
                    }
                }
                return tempTable;
            }
        }

        private static bool GetAlreadyAddedInTableAndTableHasRowsColumns_Slower(DataTable table, string value)
        {
            int tableRowsCount = table.Rows.Count;
            int originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
            for (int i = 0; i < tableRowsCount; i++)
            {
                if (table.Rows[i].Field<string>(originalColumnIndex).Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetAlreadyAddedInTableAndTableHasRowsColumns(DataTable table, string value)
        {
            if (string.IsNullOrEmpty(value) || table == null || table.Rows.Count == 0 || table.Columns.Count == 0)
            {
                return false;
            }

            //было отдельно снаружи метода, нужно проверить для чего и будет ли исключение
            //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
            DataColumn[] keyColumns = new DataColumn[1];

            //показать неуникальную строчку из таблицы, если есть. делал, когда была ошибка об неуникальности значения.
            //for (int i = 0; i < table.Rows.Count; i++)
            //{
            //    var value1 = table.Rows[i][0];
            //    for (int i2 = 0; i2 < table.Rows.Count; i2++)
            //    {
            //        var value2 = table.Rows[i2][0];
            //        if (i != i2 && Equals(value1, value2))
            //        {
            //            MessageBox.Show(value2 as string);
            //        }
            //    }
            //}

            keyColumns[0] = table.Columns[0];

            try
            {
                //здесь ошибки, когда кеш ломается из за ошибки с автозаменой и в него попадают неуникальные строчки, нужно тогда удалить из таблицы все неуникальные строки
                table.PrimaryKey = keyColumns;
            }
            catch (System.ArgumentException)
            {
                table.Rows.Clear();//очистка, если не смогло восстановить строки
                return false;//возврат false, после стирания строк


                ////очистка таблицы от дубликатов, если задание ключа выдало исключение
                //table = RemoveAllRowsDuplicatesWithRepeatingOriginals(table);

                //try
                //{
                //    //перезадание ключа
                //    keyColumns[0] = table.Columns[0];
                //    table.PrimaryKey = keyColumns;
                //}
                //catch
                //{
                //    table.Rows.Clear();//очистка, если не смогло восстановить строки
                //    return false;//возврат false, после стирания строк
                //}
            }

            //очень быстрый способ поиска дубликата значения, два нижник в разы медленней, этот почти не заметен
            if (table.Rows.Contains(value))
            {
                //LogToFile("Value already in table: \r\n"+ value);
                //MessageBox.Show("found! value=" + value);
                return true;
            }
            /*самый медленный способ, заметно медленней нижнего и непомерно критически медленней верхнего
            if (ds.Tables[tablename].Select("Original = '" + value.Replace("'", "''") + "'").Length > 0)
            {
                //MessageBox.Show("found! value=" + value);
                return true;
            }
            */
            /*довольно медленный способ, быстрее того, что перед этим с Select, но критически медленней верхнего первого
            for (int i=0; i < ds.Tables[tablename].Rows.Count; i++)
            {
                if (ds.Tables[tablename].Rows[i][0].ToString() == value)
                {
                    return true;
                }
            }
            */
            //LogToFile("Value still not in table: \r\n" + value);
            return false;
        }

        //DataSet THTranslationCache = new DataSet();
        public static void TranslationCacheInit(DataSet dataSet)
        {
            if (dataSet == null)
            {
                return;
            }

            dataSet.Reset();
            if (File.Exists(AppSettings.THTranslationCachePath))
            {
                FunctionsDBFile.ReadDBFile(dataSet, AppSettings.THTranslationCachePath);
            }
            else
            {
                dataSet.Tables.Add("TranslationCache");
                dataSet.Tables["TranslationCache"].Columns.Add(THSettings.OriginalColumnName);
                dataSet.Tables["TranslationCache"].Columns.Add(THSettings.TranslationColumnName);
            }
            //MessageBox.Show("TranslationCache Rows.Count=" + THTranslationCache.Tables["TranslationCache"].Rows.Count+ "TranslationCache Columns.Count=" + THTranslationCache.Tables["TranslationCache"].Columns.Count);
        }

        public static void THTranslationCacheAdd(DataSet dataSet, string original, string translation)
        {
            if (dataSet != null)
            {
                //LogToFile("original=" + original+ ",translation=" + translation,true);
                dataSet.Tables[0].Rows.Add(original, translation);
            }
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev cell
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetDGVSelectedRowIndexInDatatable(int tableIndex, int rowIndex)
        {
            try
            {
                var table = AppData.CurrentProject.FilesContent.Tables[tableIndex];
                if (string.IsNullOrEmpty(table.DefaultView.Sort) && string.IsNullOrEmpty(table.DefaultView.RowFilter))
                {
                    return rowIndex;
                }

                var dataRow = AppData.Main.THFileElementsDataGridView.Rows[rowIndex];
                var dataBoundItem = (DataRowView)dataRow.DataBoundItem;
                var dataBoundItemRow = dataBoundItem.Row;
                var realIndex = table.Rows.IndexOf(dataBoundItemRow);
                return realIndex;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// get Hashes of row indexes for selected/visible rows
        /// </summary>
        /// <param name="tableindex"></param>
        /// <param name="isVisible">set to true if need to search in visible rows</param>
        /// <returns></returns>
        internal static HashSet<int> GetDGVRowsIndexesHashesInDT(int tableindex, bool isVisible = false)
        {
            DataGridView dgv = null;
            AppData.Main.Invoke((Action)(() => dgv = AppData.Main.THFileElementsDataGridView));

            var selected = new HashSet<int>();
            if (isVisible)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.Visible)
                    {
                        continue;
                    }

                    selected.Add(GetDGVSelectedRowIndexInDatatable(tableindex, row.Index));
                }
            }
            else
            {
                for (int i = 0; i < dgv.SelectedCells.Count; i++)
                {
                    var rowindex = dgv.SelectedCells[i].RowIndex;
                    if (!selected.Contains(rowindex))
                    {
                        selected.Add(GetDGVSelectedRowIndexInDatatable(tableindex, rowindex));
                    }
                }
            }

            return selected;
        }

        /// <summary>
        /// Returns Dataset with tables with only non empty rows
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static DataSet GetDataSetWithoutEmptyTableRows(DataSet dataSet)
        {
            var retDS = new DataSet();
            int translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;
            int tablesCount = dataSet.Tables.Count;
            for (int t = 0; t < tablesCount; t++)
            {
                var table = dataSet.Tables[t];
                string tname = table.TableName;
                var retDStable = table.Clone();
                retDStable.TableName = tname;
                foreach(DataRow row in table.Rows)
                {
                    var cellTranslation = row.Field<string>(translationColumnIndex);
                    if (!string.IsNullOrEmpty(cellTranslation))
                    {
                        retDStable.ImportRow(row);
                    }
                }
                if (retDStable.Rows.Count == 0)
                {
                    retDS.Tables.Remove(retDStable);
                }
            }

            return retDS;
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
        /// Checks if all rows in column <paramref name="columnName"/> of <paramref name="dataTabe"/> if they are <paramref name="complete"/> or not
        /// </summary>
        /// <param name="dataTabe">dataset which to check</param>
        /// <param name="columnName">column name for check, default is translation</param>
        /// <param name="complete">true - all is empty, false - all have values</param>
        /// <returns>True when <paramref name="complete"/> is true and all values of <paramref name="columnName"/> is NOT empty. True when <paramref name="complete"/> is false and all values of <paramref name="columnName"/> is empty.</returns>
        public static bool IsTableColumnCellsAll(this DataTable dataTabe, string columnName = null, bool complete = true)
        {
            if (dataTabe == null)
            {
                return false;
            }
            int DTRowsCount = dataTabe.Rows.Count;
            columnName = columnName ?? THSettings.TranslationColumnName;
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = dataTabe.Rows[r]?.Field<string>(columnName);

                if ((!complete && (!string.IsNullOrEmpty(cell))) || (complete && (string.IsNullOrEmpty(cell))))
                {
                    return false;
                }
            }
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
        /// <param name="dataSet"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetDatasetNonEmptyRowsCount(DataSet dataSet, string columnName = null)
        {
            if (dataSet == null)
            {
                return 0;
            }

            int NonEmptyRowsCount = 0;
            columnName = columnName ?? THSettings.TranslationColumnName;
            int DTTablesCount = dataSet.Tables.Count;
            for (int t = 0; t < DTTablesCount; t++)
            {
                NonEmptyRowsCount += GetTableNonEmptyRowsCount(dataSet.Tables[t], columnName);
            }

            return NonEmptyRowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Table
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetTableNonEmptyRowsCount(DataTable dataTable, string columnName = null)
        {
            if (dataTable == null)
            {
                return 0;
            }

            int NonEmptyRowsCount = 0;
            int DTRowsCount = dataTable.Rows.Count;
            columnName = columnName ?? THSettings.TranslationColumnName;
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = dataTable.Rows[r].Field<string>(columnName);

                if (string.IsNullOrEmpty(cell))
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

        internal static string FixDataTableFilterStringValue(string stringValue)
        {
            return stringValue
                .Replace("'", "''")
                .Replace("*", "[*]")
                .Replace("%", "[%]")
                .Replace("[", "-QB[BQ-")
                .Replace("]", "[]]")
                //-QB[BQ- - для исбежания проблем с заменой в операторе .Replace("]", "[]]"), после этого
                .Replace("-QB[BQ-", "[[]");
        }
    }
}
