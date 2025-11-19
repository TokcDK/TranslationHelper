
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsTable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                selindexes[i] = GetRealRowIndex(tableIndex, selindexes[i]);

                //selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
            }

            Array.Sort(selindexes);//сортировка номеров строк, для порядка

            return selindexes;
        }

        internal static int[] GetRowIndexesOfSelectedDGVCells(DataGridViewSelectedCellCollection selectedCells)
        {
            var rowIndexes = new List<int>();
            foreach (DataGridViewCell cell in selectedCells)
            {
                var rowIndex = cell.RowIndex;
                if (!rowIndexes.Contains(rowIndex))
                {
                    rowIndexes.Add(rowIndex);
                }
            }
            return rowIndexes.ToArray();
        }

        /// <summary>
        /// shows first row where translation cell is empty
        /// </summary>
        internal static void ShowFirstRowWithEmptyTranslation()
        {
            var tables = AppData.CurrentProject.FilesContent.Tables;
            int tablesCount = tables.Count;
            string translationColumnName = THSettings.TranslationColumnName;
            for (int t = 0; t < tablesCount; t++)
            {
                var table = tables[t];

                int rowsCount = table.Rows.Count;
                for (int r = 0; r < rowsCount; r++)
                {
                    var row = table.Rows[r];
                    var cellValue = row.Field<string>(translationColumnName);
                    if (string.IsNullOrEmpty(cellValue))
                    {
                        int columnIndex = table.Columns.IndexOf(translationColumnName);
                        ShowSelectedRowCurrentCell(t, columnIndex, r, AppData.Main.THFileElementsDataGridView);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// shows selected row in selected table
        /// </summary>
        /// <param name="projectData"></param>s
        /// <param name="tableIndex"></param>
        /// <param name="selectedCellColumnIndex"></param>
        /// <param name="rowIndex"></param>
        internal static void ShowSelectedRowCurrentCell(int tableIndex, int selectedCellColumnIndex, int rowIndex, DataGridView dataGridView)
        {
            if (tableIndex == -1 
                || tableIndex > AppData.CurrentProject.FilesContent.Tables.Count - 1 
                || selectedCellColumnIndex == -1 
                || selectedCellColumnIndex > AppData.CurrentProject.FilesContent.Tables[tableIndex].Columns.Count - 1)
            {
                return;
            }

            // info: when using function Show first untranslated row it s calling the function twice.
            // added lock to avoid problems with it

            int RCount = 0;//for debug purposes
            try
            {
                // reset any filters
                var table = AppData.CurrentProject.FilesContent.Tables[tableIndex];
                if (!string.IsNullOrEmpty(table.DefaultView.RowFilter))
                {
                    AppData.Main.THFileElementsDataGridView.CleanFilter();
                    table.DefaultView.RowFilter = string.Empty;
                    table.DefaultView.Sort = string.Empty;
                    AppData.Main.THFileElementsDataGridView.Refresh();
                }

                RCount = AppData.CurrentProject.FilesContent.Tables[tableIndex].Rows.Count;
                if (tableIndex != AppData.Main.THFilesList.GetSelectedIndex() || RCount == 0 || AppData.Main.THFileElementsDataGridView.DataSource != table)
                {
                    AppData.Main.THFilesList.SetSelectedIndex(tableIndex);
                    dataGridView.DataSource = AppData.CurrentProject.FilesContent.Tables[tableIndex];
                }

                dataGridView.CurrentCell = dataGridView[selectedCellColumnIndex, rowIndex];

                if (dataGridView.Rows.Count > rowIndex && rowIndex >= 0)
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
                }

                FunctionsUI.UpdateTextboxes();
            }
            catch (Exception ex)
            {
                string error = "Error:" + Environment.NewLine + ex + Environment.NewLine + "rowIndex=" + rowIndex + Environment.NewLine + "tableIndex=" + tableIndex + Environment.NewLine + "table rows count=" + RCount;
                Logger.Error(error);
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
            catch (Exception ex)
            {
                Logger.Debug("Error in {0}. Error {1}", nameof(TheDataSetIsNotEmpty), ex);
            }

            return false;
        }

        /// <summary>
        /// Return real row index in Datatable for Datagridviev cell
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public static int GetRealRowIndex(int tableIndex, int rowIndex)
        {
            return AppData.CurrentProject.FilesContent.Tables[tableIndex].GetRealRowIndex(rowIndex);
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

                    selected.Add(GetRealRowIndex(tableindex, row.Index));
                }
            }
            else
            {
                for (int i = 0; i < dgv.SelectedCells.Count; i++)
                {
                    var rowindex = dgv.SelectedCells[i].RowIndex;
                    if (!selected.Contains(rowindex))
                    {
                        selected.Add(GetRealRowIndex(tableindex, rowindex));
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
                foreach (DataRow row in table.Rows)
                {
                    var cellTranslation = row.Field<string>(translationColumnIndex);
                    if (!string.IsNullOrEmpty(cellTranslation))
                    {
                        retDStable.ImportRow(row);
                    }
                }
                if (retDStable.Rows.Count > 0)
                {
                    retDS.Tables.Add(retDStable);
                }
            }

            return retDS;
        }

        /// <summary>
        /// Checks if all rows in column <paramref name="columnName"/> of <paramref name="dataTable"/> if they are <paramref name="complete"/> or not
        /// </summary>
        /// <param name="dataTable">dataset which to check</param>
        /// <param name="columnName">column name for check, default is translation</param>
        /// <param name="complete">true - all is empty, false - all have values</param>
        /// <returns>True when <paramref name="complete"/> is true and all values of <paramref name="columnName"/> is NOT empty. True when <paramref name="complete"/> is false and all values of <paramref name="columnName"/> is empty.</returns>
        public static bool IsTableColumnCellsAll(this DataTable dataTable, string columnName = null, bool complete = true)
        {
            if (dataTable == null) return false;

            int tableRowsCount = dataTable.Rows.Count;
            columnName = columnName ?? THSettings.TranslationColumnName;
            for (int r = 0; r < tableRowsCount; r++)
            {
                var cell = dataTable.Rows[r]?.Field<string>(columnName);

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
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static int GetDatasetRowsCount(DataSet dataSet)
        {
            if (dataSet == null) return 0;

            int rowsCount = 0;

            var tables = dataSet.Tables;
            int tablesCount = tables.Count;
            for (int t = 0; t < tablesCount; t++)
            {
                rowsCount += tables[t].Rows.Count;
            }

            return rowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Dataset tables
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetDatasetNonEmptyRowsCount(DataSet dataSet, string columnName = null)
        {
            if (dataSet == null) return 0;

            int nonEmptyRowsCount = 0;
            columnName = columnName ?? THSettings.TranslationColumnName;
            var tables = dataSet.Tables;
            int tablesCount = tables.Count;
            for (int t = 0; t < tablesCount; t++)
            {
                nonEmptyRowsCount += GetTableNonEmptyRowsCount(tables[t], columnName);
            }

            return nonEmptyRowsCount;
        }

        /// <summary>
        /// Get count of non empty rows in the Table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetTableNonEmptyRowsCount(DataTable table, string columnName = null)
        {
            if (table == null) return 0;

            int nonEmptyRowsCount = 0;
            int tableRowsCount = table.Rows.Count;
            columnName = columnName ?? THSettings.TranslationColumnName;
            for (int r = 0; r < tableRowsCount; r++)
            {
                var cell = table.Rows[r].Field<string>(columnName);

                if (string.IsNullOrEmpty(cell))
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

        public static int SelectedRowRealIndex = -1;

        internal static void ReselectCellSelectedBeforeSorting(ListBox thFilesList, DataGridView thFileElementsDataGridView)
        {
            if (SelectedRowRealIndex == -1) return;

            int fileIndex = AppSettings.THFilesListSelectedIndex;
            foreach (DataGridViewRow row in thFileElementsDataGridView.Rows)
            {
                int realrowindex = FunctionsTable.GetRealRowIndex(fileIndex, row.Index);
                if (SelectedRowRealIndex != realrowindex) continue;

                int rowindex;
                AppSettings.DGVSelectedRowIndex = rowindex = row.Index;
                AppSettings.DGVSelectedRowRealIndex = realrowindex;
                FunctionsTable.ShowSelectedRowCurrentCell(thFilesList.GetSelectedIndex(), AppSettings.DGVSelectedColumnIndex, rowindex, AppData.Main.THFileElementsDataGridView);
                SelectedRowRealIndex = realrowindex;
                break;
            }
        }

        internal static void RememberLastCellSelection(ListBox thFilesList, DataGridView thFileElementsDataGridView)
        {
            if (thFileElementsDataGridView.SelectedCells.Count > 0)
            {
                SelectedRowRealIndex = FunctionsTable.GetRealRowIndex
                    (
                    thFilesList.GetSelectedIndex(),
                    thFileElementsDataGridView.SelectedCells[0].RowIndex
                    );
            }
        }

        internal static void CellMouseDown(DataGridView thFileElementsDataGridView, ListBox thFilesList, DataGridViewCellMouseEventArgs e, ContextMenuStrip rowMenus)
        {
            //использован код отсюда:https://stackoverflow.com/a/22912594
            //но модифицирован для ситуации когда выбрана только ячейка, а не строка полностью
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewRow clickedRow = thFileElementsDataGridView.Rows[e.RowIndex];
                    if (!clickedRow.Cells[e.ColumnIndex].Selected && !clickedRow.Selected)//вот это модифицировано
                    {
                        thFileElementsDataGridView.CurrentCell = clickedRow.Cells[e.ColumnIndex];
                    }

                    if (!clickedRow.Cells[e.ColumnIndex].IsInEditMode)//не вызывать меню, когда ячейка в режиме редактирования
                    {
                        var mousePosition = thFileElementsDataGridView.PointToClient(Cursor.Position);

                        rowMenus.Show(thFileElementsDataGridView, mousePosition);
                    }
                }
            }
            if (e.RowIndex == -1)
            {
                RememberLastCellSelection(thFilesList, thFileElementsDataGridView);
            }
        }

        internal static void PaintDigitInFrontOfRow(object sender, DataGridViewRowPostPaintEventArgs e, Font font)
        {
            var grid = sender as DataGridView;
            //var rowIdx = (e.RowIndex + 1).ToString();

            using (var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString("F", font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }
        }
    }
}
