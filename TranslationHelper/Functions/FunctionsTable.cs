
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
        internal static int[] GetDGVRowIndexsesInDataSetTable(THDataWork thDataWork)
        {
            //int[] selindexes = new int[thDataWork.Main.THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount()];

            int[] selindexes = GetRowIndexesOfSelectedDGVCells(thDataWork.Main.THFileElementsDataGridView.SelectedCells);
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
                selindexes[i] = GetDGVSelectedRowIndexInDatatable(thDataWork, thDataWork.Main.THFilesList.SelectedIndex, selindexes[i]);

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
        /// <param name="thDataWork"></param>
        internal static void ShowFirstRowWithEmptyTranslation(THDataWork thDataWork)
        {
            int TCount = thDataWork.THFilesElementsDataset.Tables.Count;
            for (int t = 0; t < TCount; t++)
            {
                var table = thDataWork.THFilesElementsDataset.Tables[t];

                int RCount = table.Rows.Count;
                for (int r = 0; r < RCount; r++)
                {
                    var cell = table.Rows[r][1];
                    if (cell == null || string.IsNullOrEmpty(cell as string))
                    {
                        ShowSelectedRow(thDataWork, t, "Translation", r);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// shows selected row in selected table
        /// </summary>
        /// <param name="thDataWork"></param>
        /// <param name="tableIndex"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        internal static void ShowSelectedRow(THDataWork thDataWork, int tableIndex, string columnName, int rowIndex)
        {
            if (tableIndex == -1 || tableIndex > thDataWork.THFilesElementsDataset.Tables.Count - 1 || string.IsNullOrEmpty(columnName) || !thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns.Contains(columnName))
            {
                return;
            }

            int RCount = 0;//for debug purposes
            try
            {
                RCount = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows.Count;
                if (tableIndex == thDataWork.Main.THFilesList.SelectedIndex && RCount > 0 && thDataWork.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    thDataWork.Main.THFilesList.SelectedIndex = tableIndex;
                    thDataWork.Main.THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[tableIndex];

                }

                thDataWork.Main.THFileElementsDataGridView.CurrentCell = thDataWork.Main.THFileElementsDataGridView[columnName, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                thDataWork.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;

                thDataWork.Main.UpdateTextboxes();
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
        /// <param name="thDataWork"></param>
        /// <param name="tableIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        internal static void ShowSelectedRow(THDataWork thDataWork, int tableIndex, int columnIndex, int rowIndex)
        {
            if (tableIndex == -1 || tableIndex > thDataWork.THFilesElementsDataset.Tables.Count - 1 || columnIndex == -1 || columnIndex > thDataWork.THFilesElementsDataset.Tables[tableIndex].Columns.Count - 1)
            {
                return;
            }

            int RCount = 0;//for debug purposes
            try
            {
                RCount = thDataWork.THFilesElementsDataset.Tables[tableIndex].Rows.Count;
                if (tableIndex == thDataWork.Main.THFilesList.SelectedIndex && RCount > 0 && thDataWork.Main.THFileElementsDataGridView.DataSource != null)
                {
                }
                else
                {
                    thDataWork.Main.THFilesList.SelectedIndex = tableIndex;
                    thDataWork.Main.THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[tableIndex];

                }

                thDataWork.Main.THFileElementsDataGridView.CurrentCell = thDataWork.Main.THFileElementsDataGridView[columnIndex, rowIndex];

                //scrool to selected cell
                //https://stackoverflow.com/a/51399750
                thDataWork.Main.THFileElementsDataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
            }
            catch (Exception ex)
            {
                string error = "Error:" + Environment.NewLine + ex + Environment.NewLine + "rowIndex=" + rowIndex + Environment.NewLine + "tableIndex=" + tableIndex + Environment.NewLine + "table rows count=" + RCount;
                FileWriter.WriteData(Path.Combine(Application.StartupPath, Application.ProductName), error + Environment.NewLine + Environment.NewLine);
                MessageBox.Show(error);
            }
        }

        /// <summary>
        /// When table not exists it will create table with table thDataWork.TempPath name and Original Column<br>
        /// When table is exists it will remove table if no rows there else will create Translation column
        /// </summary>
        /// <param name="thDataWork"></param>
        /// <returns></returns>
        public static bool SetTableAndColumns(THDataWork thDataWork, bool add = true)
        {
            if (thDataWork.FilePath.Length == 0)
                return false;

            string fileName = Path.GetFileName(thDataWork.FilePath);

            if (add && !thDataWork.THFilesElementsDataset.Tables.Contains(fileName))
            {
                _ = thDataWork.THFilesElementsDataset.Tables.Add(fileName);
                _ = thDataWork.THFilesElementsDataset.Tables[fileName].Columns.Add("Original");
                _ = thDataWork.THFilesElementsDatasetInfo.Tables.Add(fileName);
                _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Columns.Add("Original");

                return true;
            }
            else
            {
                if (thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Count == 0)
                {
                    thDataWork.THFilesElementsDataset.Tables.Remove(fileName);
                    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(fileName);
                    return false;
                }
                else
                {
                    _ = thDataWork.THFilesElementsDataset.Tables[fileName].Columns.Add("Translation");
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
                        if (cell == null && string.IsNullOrEmpty(cell as string))
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

        public static void AddToTranslationCacheIfValid(/*DataSet THTranslationCache*/THDataWork thDataWork, string Original, string Translation)
        {
            if (Properties.Settings.Default.EnableTranslationCache && !Properties.Settings.Default.IsTranslationHelperWasClosed)
            {
                if (string.CompareOrdinal(Original, Translation) == 0 || Original.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).Length != Translation.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).Length || thDataWork.OnlineTranslationCache.cache.ContainsKey(Original) || string.IsNullOrWhiteSpace(Translation) /*FunctionsTable.GetAlreadyAddedInTableAndTableHasRowsColumns(THTranslationCache.Tables[0], Original)*/)
                {
                }
                else
                {
                    //THTranslationCache.Tables[0].Rows.Add(Original, Translation);
                    thDataWork.OnlineTranslationCache.cache.Add(Original, Translation);
                }
            }
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
            using (DataTable tempTable = new DataTable())
            {
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
                DS.Tables["TranslationCache"].Columns.Add("Original");
                DS.Tables["TranslationCache"].Columns.Add("Translation");
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
        /// <param name="TargetDataSet"></param>
        /// <param name="InputDataGridView"></param>
        /// <param name="TableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetDGVSelectedRowIndexInDatatable(THDataWork thDataWork, int TableIndex, int rowIndex)
        {
            var table = thDataWork.THFilesElementsDataset.Tables[TableIndex];
            if (string.IsNullOrEmpty(table.DefaultView.Sort) && string.IsNullOrEmpty(table.DefaultView.RowFilter))
            {
                return rowIndex;
            }

            return table.Rows
                .IndexOf(
                ((DataRowView)thDataWork.Main.THFileElementsDataGridView.Rows[rowIndex].DataBoundItem).Row
                        );
        }

        /// <summary>
        /// get Hashes of row indexes for selected/visible rows
        /// </summary>
        /// <param name="thDataWork"></param>
        /// <param name="tableindex"></param>
        /// <param name="IsVisible">set to true if need to search in visible rows</param>
        /// <returns></returns>
        internal static HashSet<int> GetDGVRowsIndexesHashesInDT(THDataWork thDataWork, int tableindex, bool IsVisible = false)
        {
            DataGridView DGV = null;
            thDataWork.Main.Invoke((Action)(() => DGV = thDataWork.Main.THFileElementsDataGridView));

            var selected = new HashSet<int>();
            if (IsVisible)
            {
                foreach (DataGridViewRow row in DGV.Rows)
                {
                    if (!row.Visible)
                    {
                        continue;
                    }

                    selected.Add(GetDGVSelectedRowIndexInDatatable(thDataWork, tableindex, row.Index));
                }
            }
            else
            {
                for (int i = 0; i < DGV.SelectedCells.Count; i++)
                {
                    var rowindex = DGV.SelectedCells[i].RowIndex;
                    if (!selected.Contains(rowindex))
                    {
                        selected.Add(GetDGVSelectedRowIndexInDatatable(thDataWork, tableindex, rowindex));
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
        /// Returns true if translation cells in all rows have values
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
                var cell = DT.Rows[r]?[column];

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
        /// <param name="DT"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsTableRowsAllEmpty(DataTable DT, string column = "Translation")
        {
            if (DT == null)
            {
                return true;
            }
            int DTRowsCount = DT.Rows.Count;
            for (int r = 0; r < DTRowsCount; r++)
            {
                var cell = DT.Rows[r][column];

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

        internal static void CleanTableCells(THDataWork thDataWork, int Tindex)
        {
            int THFileElementsDataGridViewSelectedCellsCount = thDataWork.Main.THFileElementsDataGridView.GetCountOfRowsWithSelectedCellsCount();
            // Ensure that text is currently selected in the text box.    
            if (THFileElementsDataGridViewSelectedCellsCount == 0)
            {
                return;
            }

            //если количество выбранных строк равно числу строк в таблице, то 
            if (THFileElementsDataGridViewSelectedCellsCount == thDataWork.THFilesElementsDataset.Tables[Tindex].Rows.Count)
            {
                //DataColumn dc = thDataWork.THFilesElementsDataset.Tables[Tindex].Columns[1];

                //отключение датасорса для убирания тормозов с параллельной прорисовкой
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.DataSource = null));
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Update()));
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Refresh()));

                var table = thDataWork.THFilesElementsDataset.Tables[Tindex];

                foreach (DataRow row in table.Rows)
                {
                    if (row[1] == null || string.IsNullOrEmpty(row[1] as string))
                    {
                    }
                    else
                    {
                        row[1] = null;
                    }
                }

                //int rowsCount = table.Rows.Count;
                //Parallel.For(0, rowsCount,
                //    r =>
                //    {
                //        try
                //        {
                //            var row = table.Rows[r];
                //            if (row[1] == null || string.IsNullOrEmpty(row[1] as string))
                //            {
                //            }
                //            else
                //            {
                //                lock (row)
                //                {
                //                    row[1] = null;
                //                }
                //            }
                //        }
                //        catch
                //        {
                //        }
                //    });

                //for (int r = 0; r < rowsCount; r++)
                //{
                //    try
                //    {
                //        //типа многопоточная очистка ячеек
                //        Task.Run(() => thDataWork.Main.Invoke((Action)(() => thDataWork.THFilesElementsDataset.Tables[Tindex].Rows[r][1] = null))).ConfigureAwait(false);
                //    }
                //    catch
                //    {
                //    }

                //    //thDataWork.THFilesElementsDataset.Tables[Tindex].Rows[r][1] = null;
                //}

                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.ActionsOnTHFIlesListElementSelected()));
                //thDataWork.Main.THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset.Tables[Tindex];


                //https://stackoverflow.com/questions/15035219/how-do-you-clear-an-entire-datagridview-column
                //thDataWork.Main.THFileElementsDataGridView.ClearColumn(1);

                ////удалить колонку, потом добавить и переместить на позицию 1
                //thDataWork.THFilesElementsDataset.Tables[Tindex].Columns.Remove("Translation");//КИДАЕТ ИСКЛЮЧЕНИЕ dATAGRIDVIEW
                //thDataWork.THFilesElementsDataset.Tables[Tindex].Columns.Add("Translation");
                ////https://stackoverflow.com/questions/3757997/how-to-change-datatable-columns-order
                //thDataWork.THFilesElementsDataset.Tables[Tindex].Columns["Translation"].SetOrdinal(1);

                ////перерисовка датагрида
                ////https://stackoverflow.com/a/29453395 частично
                //thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.DataSource = null));
                //thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Update()));
                //thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Refresh()));
                ////thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.Parent.Refresh()));
                ////thDataWork.Main.Invoke((Action)(() => thDataWork.Main.THFileElementsDataGridView.DataSource = thDataWork.THFilesElementsDataset));
                //thDataWork.Main.Invoke((Action)(() => thDataWork.Main.ActionsOnTHFIlesListElementSelected()));
                return;
            }

            //Clear selected cells                
            //проверка, выполнять очистку только если выбранные ячейки не помечены Только лдя чтения
            //if (THFileElementsDataGridView.CurrentCell.ReadOnly)
            //{
            //}
            //else
            //{
            //    foreach (DataGridViewCell dgvCell in THFileElementsDataGridView.SelectedCells)
            //    {
            //        dgvCell.Value = string.Empty;
            //    }
            //}

            try
            {
                int[] rindexes = new int[THFileElementsDataGridViewSelectedCellsCount];
                int corigind = thDataWork.Main.THFileElementsDataGridView.Columns["Original"].Index;//2-поле untrans
                int ctransind = thDataWork.Main.THFileElementsDataGridView.Columns["Translation"].Index;//2-поле untrans

                bool TableHasNotDefaultRowsOrder = true;
                if (THFileElementsDataGridViewSelectedCellsCount > 50)
                {
                    //определение, имеет ли датагрид нестандартный порядок строк, как при сортировке или фильтрах
                    int equalIndexsesCounter = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        var rind = thDataWork.Main.THFileElementsDataGridView.SelectedCells[i].RowIndex;
                        if (rind == FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, Tindex, rind))
                        {
                            equalIndexsesCounter++;
                        }
                    }

                    TableHasNotDefaultRowsOrder = equalIndexsesCounter < 5;//имеет, если не все 5 индексов датагрида были равны индексам дататэйбл
                }

                for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                {
                    int rind = thDataWork.Main.THFileElementsDataGridView.SelectedCells[i].RowIndex;
                    if ((thDataWork.Main.THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value + string.Empty).Length > 0)
                    {
                        if (TableHasNotDefaultRowsOrder)
                        {
                            rindexes[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(thDataWork, Tindex, rind);
                        }
                        else
                        {
                            rindexes[i] = rind;
                        }
                    }

                }

                var rindexesLength = rindexes.Length;
                for (int i = 0; i < rindexesLength; i++)
                {
                    if (rindexes[i] == -1)
                    {
                        continue;
                    }

                    var row = thDataWork.THFilesElementsDataset.Tables[Tindex].Rows[rindexes[i]];
                    if (row[ctransind] != null && !string.IsNullOrEmpty(row[ctransind] as string))
                    {
                        row[ctransind] = string.Empty;
                    }
                }
            }
            catch
            {
            }
        }
    }
}
