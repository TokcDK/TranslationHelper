
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
            ProjectData.Main.Invoke((Action)(() => tableIndex = ProjectData.Main.THFilesList.GetSelectedIndex()));


            int[] selindexes = GetRowIndexesOfSelectedDGVCells(ProjectData.Main.THFileElementsDataGridView.SelectedCells);
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
            int TCount = ProjectData.THFilesElementsDataset.Tables.Count;
            for (int t = 0; t < TCount; t++)
            {
                var table = ProjectData.THFilesElementsDataset.Tables[t];

                int RCount = table.Rows.Count;
                for (int r = 0; r < RCount; r++)
                {
                    var cell = table.Rows[r][1];
                    if (cell == null || string.IsNullOrEmpty(cell as string))
                    {
                        ShowSelectedRow(t, THSettings.TranslationColumnName(), r);
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
            if (tableIndex == -1 || tableIndex > ProjectData.THFilesElementsDataset.Tables.Count - 1 || string.IsNullOrEmpty(columnName) || !ProjectData.THFilesElementsDataset.Tables[tableIndex].Columns.Contains(columnName))
            {
                return;
            }

            int RCount = 0;//for debug purposes
            try
            {
                RCount = ProjectData.THFilesElementsDataset.Tables[tableIndex].Rows.Count;
                if (tableIndex == ProjectData.Main.THFilesList.GetSelectedIndex() && RCount > 0 && ProjectData.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    ProjectData.Main.THFilesList.SetSelectedIndex(tableIndex);
                    ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.THFilesElementsDataset.Tables[tableIndex];
                }

                ProjectData.Main.THFileElementsDataGridView.CurrentCell = ProjectData.Main.THFileElementsDataGridView[columnName, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                ProjectData.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;

                ProjectData.Main.UpdateTextboxes();
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
            if (tableIndex == -1 || tableIndex > ProjectData.THFilesElementsDataset.Tables.Count - 1 || columnIndex == -1 || columnIndex > ProjectData.THFilesElementsDataset.Tables[tableIndex].Columns.Count - 1)
            {
                return;
            }

            int RCount = 0;//for debug purposes
            try
            {
                RCount = ProjectData.THFilesElementsDataset.Tables[tableIndex].Rows.Count;
                if (tableIndex == ProjectData.Main.THFilesList.GetSelectedIndex() && RCount > 0 && ProjectData.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    ProjectData.Main.THFilesList.SetSelectedIndex(tableIndex);
                    ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.THFilesElementsDataset.Tables[tableIndex];

                }

                ProjectData.Main.THFileElementsDataGridView.CurrentCell = ProjectData.Main.THFileElementsDataGridView[columnIndex, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                ProjectData.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
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
        public static bool SetTableAndColumns(bool add = true)
        {
            if (ProjectData.FilePath.Length == 0)
                return false;

            string fileName = Path.GetFileName(ProjectData.FilePath);

            if (add && !ProjectData.THFilesElementsDataset.Tables.Contains(fileName))
            {
                _ = ProjectData.THFilesElementsDataset.Tables.Add(fileName);
                _ = ProjectData.THFilesElementsDataset.Tables[fileName].Columns.Add(THSettings.OriginalColumnName());
                _ = ProjectData.THFilesElementsDatasetInfo.Tables.Add(fileName);
                _ = ProjectData.THFilesElementsDatasetInfo.Tables[fileName].Columns.Add(THSettings.OriginalColumnName());

                return true;
            }
            else
            {
                if (ProjectData.THFilesElementsDataset.Tables[fileName].Rows.Count == 0)
                {
                    ProjectData.THFilesElementsDataset.Tables.Remove(fileName);
                    ProjectData.THFilesElementsDatasetInfo.Tables.Remove(fileName);
                    return false;
                }
                else
                {
                    _ = ProjectData.THFilesElementsDataset.Tables[fileName].Columns.Add(THSettings.TranslationColumnName());
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

        public static string TranslationCacheFind(DataSet DS, string Input)
        {
            if (Properties.Settings.Default.EnableTranslationCache)
            {
                if (!string.IsNullOrEmpty(Input) && DS != null)
                {
                    using (var Table = DS.Tables[0])
                    {
                        if (FunctionsTable.GetAlreadyAddedInTableAndTableHasRowsColumns(Table, Input))
                        {
                            var RowsCount = Table.Rows.Count;
                            for (int i = 0; i < RowsCount; i++)
                            {
                                //MessageBox.Show("Input=" + Input+"\r\nCache="+ THTranslationCache.Tables["TranslationCache"].Rows[i][0].ToString());
                                if (Equals(Input, Table.Rows[i][0]))
                                {
                                    return Table.Rows[i][1] as string;
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

        private static bool GetAlreadyAddedInTableAndTableHasRowsColumns_Slower(DataTable DT, string value)
        {
            int DTRowsCount = DT.Rows.Count;
            for (int i = 0; i < DTRowsCount; i++)
            {
                if (Equals(DT.Rows[i][0], value))
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
        public static void TranslationCacheInit(DataSet DS)
        {
            if (DS == null)
            {
                return;
            }

            DS.Reset();
            if (File.Exists(Properties.Settings.Default.THTranslationCachePath))
            {
                FunctionsDBFile.ReadDBFile(DS, Properties.Settings.Default.THTranslationCachePath);
            }
            else
            {
                DS.Tables.Add("TranslationCache");
                DS.Tables["TranslationCache"].Columns.Add(THSettings.OriginalColumnName());
                DS.Tables["TranslationCache"].Columns.Add(THSettings.TranslationColumnName());
            }
            //MessageBox.Show("TranslationCache Rows.Count=" + THTranslationCache.Tables["TranslationCache"].Rows.Count+ "TranslationCache Columns.Count=" + THTranslationCache.Tables["TranslationCache"].Columns.Count);
        }

        public static void THTranslationCacheAdd(DataSet DS, string original, string translation)
        {
            if (DS != null)
            {
                //LogToFile("original=" + original+ ",translation=" + translation,true);
                DS.Tables[0].Rows.Add(original, translation);
            }
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev cell
        /// </summary>
        /// <param name="TableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetDGVSelectedRowIndexInDatatable(int TableIndex, int rowIndex)
        {
            try
            {
                var table = ProjectData.THFilesElementsDataset.Tables[TableIndex];
                if (string.IsNullOrEmpty(table.DefaultView.Sort) && string.IsNullOrEmpty(table.DefaultView.RowFilter))
                {
                    return rowIndex;
                }

                var dataRow = ProjectData.Main.THFileElementsDataGridView.Rows[rowIndex];
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
        /// <param name="projectData"></param>
        /// <param name="tableindex"></param>
        /// <param name="IsVisible">set to true if need to search in visible rows</param>
        /// <returns></returns>
        internal static HashSet<int> GetDGVRowsIndexesHashesInDT(int tableindex, bool IsVisible = false)
        {
            DataGridView DGV = null;
            ProjectData.Main.Invoke((Action)(() => DGV = ProjectData.Main.THFileElementsDataGridView));

            var selected = new HashSet<int>();
            if (IsVisible)
            {
                foreach (DataGridViewRow row in DGV.Rows)
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
                for (int i = 0; i < DGV.SelectedCells.Count; i++)
                {
                    var rowindex = DGV.SelectedCells[i].RowIndex;
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
                RETDS.Tables[tname].Columns.Add(THSettings.OriginalColumnName());
                RETDS.Tables[tname].Columns.Add(THSettings.TranslationColumnName());
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
        /// Checks if all rows in column <paramref name="columnName"/> of <paramref name="dataSet"/> if they are <paramref name="complete"/> or not
        /// </summary>
        /// <param name="dataSet">dataset which to check</param>
        /// <param name="columnName">column name for check, default is translation</param>
        /// <param name="complete">true - all is empty, false - all have values</param>
        /// <returns>True when <paramref name="complete"/> is true and all values of <paramref name="columnName"/> is NOT empty. True when <paramref name="complete"/> is false and all values of <paramref name="columnName"/> is empty.</returns>
        public static bool IsTableRowsAll(DataTable dataSet, string columnName = null, bool complete = true)
        {
            if (dataSet == null)
            {
                return false;
            }
            int DTRowsCount = dataSet.Rows.Count;
            columnName = columnName ?? THSettings.TranslationColumnName();
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = dataSet.Rows[r]?[columnName];

                if ((!complete && (cell != null && !string.IsNullOrEmpty(cell as string))) || (complete && (cell == null || string.IsNullOrEmpty(cell as string))))
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
            columnName = columnName ?? THSettings.TranslationColumnName();
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
            columnName = columnName ?? THSettings.TranslationColumnName();
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = dataTable.Rows[r][columnName];

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
