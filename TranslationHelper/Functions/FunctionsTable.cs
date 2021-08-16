
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
        internal static int[] GetDgvRowIndexsesInDataSetTable()
        {
            //int[] selindexes = new int[ProjectData.Main.THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount()];

            var tableIndex = 0;
            ProjectData.Main.Invoke((Action)(()=>tableIndex=ProjectData.Main.THFilesList.SelectedIndex));


            int[] selindexes = GetRowIndexesOfSelectedDgvCells(ProjectData.Main.THFileElementsDataGridView.SelectedCells);
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
                selindexes[i] = GetDgvSelectedRowIndexInDatatable(tableIndex, selindexes[i]);

                //selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
            }

            Array.Sort(selindexes);//сортировка номеров строк, для порядка

            return selindexes;
        }

        internal static int[] GetRowIndexesOfSelectedDgvCells(DataGridViewSelectedCellCollection selectedCells)
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
            int count = ProjectData.ThFilesElementsDataset.Tables.Count;
            for (int t = 0; t < count; t++)
            {
                var table = ProjectData.ThFilesElementsDataset.Tables[t];

                int rCount = table.Rows.Count;
                for (int r = 0; r < rCount; r++)
                {
                    var cell = table.Rows[r][1];
                    if (cell == null || string.IsNullOrEmpty(cell as string))
                    {
                        ShowSelectedRow(t, "Translation", r);
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
            if (tableIndex == -1 || tableIndex > ProjectData.ThFilesElementsDataset.Tables.Count - 1 || string.IsNullOrEmpty(columnName) || !ProjectData.ThFilesElementsDataset.Tables[tableIndex].Columns.Contains(columnName))
            {
                return;
            }

            int rCount = 0;//for debug purposes
            try
            {
                rCount = ProjectData.ThFilesElementsDataset.Tables[tableIndex].Rows.Count;
                if (tableIndex == ProjectData.Main.THFilesList.SelectedIndex && rCount > 0 && ProjectData.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    ProjectData.Main.THFilesList.SelectedIndex = tableIndex;
                    ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[tableIndex];

                }

                ProjectData.Main.THFileElementsDataGridView.CurrentCell = ProjectData.Main.THFileElementsDataGridView[columnName, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                ProjectData.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;

                ProjectData.Main.UpdateTextboxes();
            }
            catch (Exception ex)
            {
                string error = "Error:" + Environment.NewLine + ex + Environment.NewLine + "rowIndex=" + rowIndex + Environment.NewLine + "tableIndex=" + tableIndex + Environment.NewLine + "table rows count=" + rCount;
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
            if (tableIndex == -1 || tableIndex > ProjectData.ThFilesElementsDataset.Tables.Count - 1 || columnIndex == -1 || columnIndex > ProjectData.ThFilesElementsDataset.Tables[tableIndex].Columns.Count - 1)
            {
                return;
            }

            int rCount = 0;//for debug purposes
            try
            {
                rCount = ProjectData.ThFilesElementsDataset.Tables[tableIndex].Rows.Count;
                if (tableIndex == ProjectData.Main.THFilesList.SelectedIndex && rCount > 0 && ProjectData.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    ProjectData.Main.THFilesList.SelectedIndex = tableIndex;
                    ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[tableIndex];

                }

                ProjectData.Main.THFileElementsDataGridView.CurrentCell = ProjectData.Main.THFileElementsDataGridView[columnIndex, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                ProjectData.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch (Exception ex)
            {
                string error = "Error:" + Environment.NewLine + ex + Environment.NewLine + "rowIndex=" + rowIndex + Environment.NewLine + "tableIndex=" + tableIndex + Environment.NewLine + "table rows count=" + rCount;
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
        public static bool SetTableAndColumns(bool add = true)
        {
            if (ProjectData.FilePath.Length == 0)
                return false;

            string fileName = Path.GetFileName(ProjectData.FilePath);

            if (add && !ProjectData.ThFilesElementsDataset.Tables.Contains(fileName))
            {
                _ = ProjectData.ThFilesElementsDataset.Tables.Add(fileName);
                _ = ProjectData.ThFilesElementsDataset.Tables[fileName].Columns.Add("Original");
                _ = ProjectData.ThFilesElementsDatasetInfo.Tables.Add(fileName);
                _ = ProjectData.ThFilesElementsDatasetInfo.Tables[fileName].Columns.Add("Original");

                return true;
            }
            else
            {
                if (ProjectData.ThFilesElementsDataset.Tables[fileName].Rows.Count == 0)
                {
                    ProjectData.ThFilesElementsDataset.Tables.Remove(fileName);
                    ProjectData.ThFilesElementsDatasetInfo.Tables.Remove(fileName);
                    return false;
                }
                else
                {
                    _ = ProjectData.ThFilesElementsDataset.Tables[fileName].Columns.Add("Translation");
                    return true;
                }
            }
        }

        /// <summary>
        /// True if Translation cell of any row has value
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static bool TheDataSetIsNotEmpty(DataSet ds)
        {
            if (ds == null)
            {
                return false;
            }

            try
            {
                int dsTablesCount = ds.Tables.Count;
                for (int t = 0; t < dsTablesCount; t++)
                {
                    var table = ds.Tables[t];
                    int rowscount = table.Rows.Count;
                    for (int r = 0; r < rowscount; r++)
                    {
                        var cell = table.Rows[r][1];
                        if (cell == null || string.IsNullOrEmpty(cell as string))
                        {
                        }
                        else
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

        public static string TranslationCacheFind(DataSet ds, string input)
        {
            if (Properties.Settings.Default.EnableTranslationCache)
            {
                if (!string.IsNullOrEmpty(input) && ds != null)
                {
                    using (var table = ds.Tables[0])
                    {
                        if (FunctionsTable.GetAlreadyAddedInTableAndTableHasRowsColumns(table, input))
                        {
                            var rowsCount = table.Rows.Count;
                            for (int i = 0; i < rowsCount; i++)
                            {
                                //MessageBox.Show("Input=" + Input+"\r\nCache="+ THTranslationCache.Tables["TranslationCache"].Rows[i][0].ToString());
                                if (Equals(input, table.Rows[i][0]))
                                {
                                    return table.Rows[i][1] as string;
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
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
                    if (!GetAlreadyAddedInTableAndTableHasRowsColumns_Slower(tempTable, row[0] as string))
                    {
                        tempTable.ImportRow(row);
                    }
                }
                return tempTable;
            }
        }

        private static bool GetAlreadyAddedInTableAndTableHasRowsColumns_Slower(DataTable dt, string value)
        {
            int dtRowsCount = dt.Rows.Count;
            for (int i = 0; i < dtRowsCount; i++)
            {
                if (Equals(dt.Rows[i][0], value))
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
        public static void TranslationCacheInit(DataSet ds)
        {
            if (ds == null)
            {
                return;
            }

            ds.Reset();
            if (File.Exists(Properties.Settings.Default.THTranslationCachePath))
            {
                FunctionsDbFile.ReadDbFile(ds, Properties.Settings.Default.THTranslationCachePath);
            }
            else
            {
                ds.Tables.Add("TranslationCache");
                ds.Tables["TranslationCache"].Columns.Add("Original");
                ds.Tables["TranslationCache"].Columns.Add("Translation");
            }
            //MessageBox.Show("TranslationCache Rows.Count=" + THTranslationCache.Tables["TranslationCache"].Rows.Count+ "TranslationCache Columns.Count=" + THTranslationCache.Tables["TranslationCache"].Columns.Count);
        }

        public static void ThTranslationCacheAdd(DataSet ds, string original, string translation)
        {
            if (ds != null)
            {
                //LogToFile("original=" + original+ ",translation=" + translation,true);
                ds.Tables[0].Rows.Add(original, translation);
            }
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev cell
        /// </summary>
        /// <param name="TargetDataSet"></param>
        /// <param name="InputDataGridView"></param>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetDgvSelectedRowIndexInDatatable(int tableIndex, int rowIndex)
        {
            var table = ProjectData.ThFilesElementsDataset.Tables[tableIndex];
            if (string.IsNullOrEmpty(table.DefaultView.Sort) && string.IsNullOrEmpty(table.DefaultView.RowFilter))
            {
                return rowIndex;
            }

            return table.Rows
                .IndexOf(
                ((DataRowView)ProjectData.Main.THFileElementsDataGridView.Rows[rowIndex].DataBoundItem).Row
                        );
        }

        /// <summary>
        /// get Hashes of row indexes for selected/visible rows
        /// </summary>
        /// <param name="projectData"></param>
        /// <param name="tableindex"></param>
        /// <param name="isVisible">set to true if need to search in visible rows</param>
        /// <returns></returns>
        internal static HashSet<int> GetDgvRowsIndexesHashesInDt(int tableindex, bool isVisible = false)
        {
            DataGridView dgv = null;
            ProjectData.Main.Invoke((Action)(() => dgv = ProjectData.Main.THFileElementsDataGridView));

            var selected = new HashSet<int>();
            if (isVisible)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (!row.Visible)
                    {
                        continue;
                    }

                    selected.Add(GetDgvSelectedRowIndexInDatatable(tableindex, row.Index));
                }
            }
            else
            {
                for (int i = 0; i < dgv.SelectedCells.Count; i++)
                {
                    var rowindex = dgv.SelectedCells[i].RowIndex;
                    if (!selected.Contains(rowindex))
                    {
                        selected.Add(GetDgvSelectedRowIndexInDatatable(tableindex, rowindex));
                    }
                }
            }

            return selected;
        }

        /// <summary>
        /// Returns Dataset with tables with only non empty rows
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static DataSet FillTempDb(DataSet ds)
        {
            DataSet retds = new DataSet();
            int tablesCount = ds.Tables.Count;
            for (int t = 0; t < tablesCount; t++)
            {
                var table = ds.Tables[t];
                string tname = table.TableName;
                retds.Tables.Add(tname);
                retds.Tables[tname].Columns.Add("Original");
                retds.Tables[tname].Columns.Add("Translation");
                int rowsCount = table.Rows.Count;
                for (int r = 0; r < rowsCount; r++)
                {
                    var row = table.Rows[r];
                    var cellTranslation = row[1];
                    if (cellTranslation == null || string.IsNullOrEmpty(cellTranslation as string))
                    {
                    }
                    else
                    {
                        retds.Tables[tname].ImportRow(row);
                    }
                }
                if (retds.Tables[tname].Rows.Count == 0)
                {
                    retds.Tables.Remove(tname);
                }
            }

            return retds;
        }

        /// <summary>
        /// true if 2 datasets tables and table rows count is identical
        /// </summary>
        /// <param name="ds1"></param>
        /// <param name="ds2"></param>
        /// <returns></returns>
        public static bool IsDataSetsElementsCountIdentical(DataSet ds1, DataSet ds2)
        {
            if (ds1.Tables.Count == ds2.Tables.Count)
            {
                for (int t = 0; t < ds1.Tables.Count; t++)
                {
                    if (ds1.Tables[t].Rows.Count != ds2.Tables[t].Rows.Count)
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
        /// Returns true if translation cells in all rows have values
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsTableRowsCompleted(DataTable dt, string column = "Translation")
        {
            if (dt == null)
            {
                return false;
            }
            int dtRowsCount = dt.Rows.Count;
            for (int r = 0; r < dtRowsCount; r++)
            {
                var cell = dt.Rows[r]?[column];

                if (cell == null || string.IsNullOrEmpty(cell as string))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if translation cells in all Datatable rows is empty
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsTableRowsAllEmpty(DataTable dt, string column = "Translation")
        {
            if (dt == null)
            {
                return true;
            }
            int dtRowsCount = dt.Rows.Count;
            for (int r = 0; r < dtRowsCount; r++)
            {
                var cell = dt.Rows[r][column];

                if (cell != null && !string.IsNullOrEmpty(cell as string))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get rows count from all Dataset tables
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static int GetDatasetRowsCount(DataSet ds)
        {
            if (ds == null)
            {
                return 0;
            }

            int rowsCount = 0;

            int dtTablesCount = ds.Tables.Count;
            for (int t = 0; t < dtTablesCount; t++)
            {
                rowsCount += ds.Tables[t].Rows.Count;
            }

            return rowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Dataset tables
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetDatasetNonEmptyRowsCount(DataSet ds, string column = "Translation")
        {
            if (ds == null)
            {
                return 0;
            }

            int nonEmptyRowsCount = 0;

            int dtTablesCount = ds.Tables.Count;
            for (int t = 0; t < dtTablesCount; t++)
            {
                nonEmptyRowsCount += GetTableNonEmptyRowsCount(ds.Tables[t], column);
            }

            return nonEmptyRowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Table
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetTableNonEmptyRowsCount(DataTable dt, string column = "Translation")
        {
            if (dt == null)
            {
                return 0;
            }

            int nonEmptyRowsCount = 0;
            int dtRowsCount = dt.Rows.Count;
            for (int r = 0; r < dtRowsCount; r++)
            {
                var cell = dt.Rows[r][column];

                if (cell == null || string.IsNullOrEmpty(cell as string))
                {
                    //LogToFile("\r\nIsTableRowsCompleted=false");
                }
                else
                {
                    nonEmptyRowsCount++;
                }
            }

            return nonEmptyRowsCount;
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
