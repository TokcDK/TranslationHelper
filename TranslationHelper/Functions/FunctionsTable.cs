
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsTable
    {

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

        public static void AddToTranslationCacheIfValid(DataSet THTranslationCache, string Original, string Translation)
        {
            if (Properties.Settings.Default.IsTranslationCacheEnabled && !Properties.Settings.Default.IsTranslationHelperWasClosed)
            {
                if (string.CompareOrdinal(Original, Translation) == 0 || Original.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).Length != Translation.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None).Length || FunctionsTable.GetAlreadyAddedInTableAndTableHasRowsColumns(THTranslationCache.Tables[0], Original))
                {
                }
                else
                {
                    THTranslationCache.Tables[0].Rows.Add(Original, Translation);
                }
            }
        }

        public static string TranslationCacheFind(DataSet DS, string Input)
        {
            if (Properties.Settings.Default.IsTranslationCacheEnabled)
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
                foreach(DataColumn column in table.Columns)
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
                if (Equals(DT.Rows[i][0],value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GetAlreadyAddedInTableAndTableHasRowsColumns(DataTable table, string value)
        {
            if (string.IsNullOrEmpty(value) || table==null || table.Rows.Count == 0 || table.Columns.Count == 0)
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
        public static int GetDGVSelectedRowIndexInDatatable(DataSet TargetDataSet, DataGridView InputDataGridView, int TableIndex, int rowIndex)
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

        internal static void CleanTableCells(DataGridView THFileElementsDataGridView, DataSet THFilesElementsDataset, int Tindex)
        {
            int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.SelectedCells.Count;
            // Ensure that text is currently selected in the text box.    
            if (THFileElementsDataGridViewSelectedCellsCount > 0)
            {
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
                    int corigind = THFileElementsDataGridView.Columns["Original"].Index;//2-поле untrans
                    int ctransind = THFileElementsDataGridView.Columns["Translation"].Index;//2-поле untrans

                    bool TableHasNotDefaultRowsOrder = true;
                    if (THFileElementsDataGridViewSelectedCellsCount > 50)
                    {
                        //определение, имеет ли датагрид нестандартный порядок строк, как при сортировке или фильтрах
                        int equalIndexsesCounter = 0;
                        for (int i = 0; i < 5; i++)
                        {
                            var rind = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                            if (rind == FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesElementsDataset, THFileElementsDataGridView, Tindex, rind))
                            {
                                equalIndexsesCounter++;
                            }
                        }

                        TableHasNotDefaultRowsOrder = equalIndexsesCounter < 5;//имеет, если не все 5 индексов датагрида были равны индексам дататэйбл
                    }

                    for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                    {
                        int rind = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                        if ((THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value + string.Empty).Length > 0)
                        {
                            if (TableHasNotDefaultRowsOrder)
                            {
                                rindexes[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesElementsDataset, THFileElementsDataGridView, Tindex, rind);
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

                        var cell = THFilesElementsDataset.Tables[Tindex].Rows[rindexes[i]][ctransind];
                        if (cell != null && (cell as string).Length > 0)
                        {
                            THFilesElementsDataset.Tables[Tindex].Rows[rindexes[i]][ctransind] = string.Empty;
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
