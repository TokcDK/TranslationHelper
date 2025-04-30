using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsLoadTranslationDB
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public FunctionsLoadTranslationDB()
        {

        }

        //internal static void THLoadDBCompare(DataSet THTempDS)
        //{
        //    if (!AppSettings.IsFullComprasionDBloadEnabled && FunctionsTable.IsDataSetsElementsCountIdentical(AppData.CurrentProject.FilesContent, THTempDS))
        //    {
        //        CompareLiteIfIdentical(THTempDS);
        //        return;
        //    }

        //    //using (DataSet THTempDS = new DataSet())
        //    //{
        //    //    //LogToFile("cleaning THTempDS and refreshing dgv", true);
        //    //    THTempDS.Reset();//очистка временной таблицы
        //    //}

        //    //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath); // write lastsavedpath

        //    //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
        //    int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
        //    if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
        //    {
        //        return;
        //    }

        //    //LogToFile("ocol=" + ocol);
        //    //оптимизация. Не искать колонку перевода, если она по стандарту первая

        //    int ttranscol = THTempDS.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
        //    if (ttranscol == 0 || ttranscol == -1)
        //    {
        //        return;
        //    }

        //    //Оптимизация. Стартовые значения номера таблицы и строки для таблицы с загруженным переводом
        //    int ttablestartindex = 0;
        //    int trowstartindex = 0;

        //    int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
        //    string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
        //    //проход по всем таблицам рабочего dataset
        //    for (int t = 0; t < tcount; t++)
        //    {
        //        //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
        //        bool b = ResetDGVDataSource(t);

        //        using (var table = AppData.CurrentProject.FilesContent.Tables[t])
        //        {
        //            //skip table if there is no untranslated lines
        //            if (FunctionsTable.IsTableColumnCellsAll(table))
        //                continue;

        //            string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
        //            Logger.Info(tableprogressinfo);

        //            int rcount = table.Rows.Count;
        //            //проход по всем строкам таблицы рабочего dataset
        //            for (int r = 0; r < rcount; r++)
        //            {
        //                //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
        //                var row = table.Rows[r];
        //                var CellTranslation = row.Field<string>(otranscol);
        //                if (string.IsNullOrEmpty(CellTranslation))
        //                {
        //                    bool translationWasSet = false;

        //                    var dbTablesCount = THTempDS.Tables.Count;
        //                    //проход по всем таблицам dataset с переводом
        //                    for (int t1 = ttablestartindex; t1 < dbTablesCount; t1++)
        //                    {
        //                        using (var dbTable = THTempDS.Tables[t1])
        //                        {
        //                            if (dbTable.Columns.Count > 1)
        //                            {
        //                                var dbTableRowsCount = THTempDS.Tables[t1].Rows.Count;
        //                                //проход по всем строкам таблицы dataset с переводом
        //                                for (int r1 = trowstartindex; r1 < dbTableRowsCount; r1++)
        //                                {
        //                                    var dbRow = dbTable.Rows[r1];
        //                                    var dbCellTranslation = dbRow.Field<string>(ttranscol);
        //                                    if (dbCellTranslation != null && !string.IsNullOrEmpty(dbCellTranslation))
        //                                    {
        //                                        try
        //                                        {
        //                                            if (Equals(row[0], dbRow[0]))
        //                                            {
        //                                                AppData.CurrentProject.FilesContent.Tables[t].Rows[r][otranscol] = dbCellTranslation;
        //                                                translationWasSet = true;

        //                                                trowstartindex = AppSettings.IsFullComprasionDBloadEnabled ? 0 : r1;//запоминание последнего индекса строки, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
        //                                                break;
        //                                            }
        //                                        }
        //                                        catch
        //                                        {
        //                                        }
        //                                    }
        //                                }
        //                                if (translationWasSet)//если перевод был присвоен, выйти из цикла таблицы с переводом
        //                                {
        //                                    ttablestartindex = AppSettings.IsFullComprasionDBloadEnabled ? 0 : t1;//запоминание последнего индекса таблицы, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
        //                                    break;
        //                                }
        //                                else
        //                                {
        //                                    //сбрасывать индекс на ноль, когда все строки были пройдены и таблица меняется, строка должна быть сброшена на ноль
        //                                    trowstartindex = 0;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if(b) ResetDGVDataSource(-1, false, table);
        //        }
        //    }

        //    
        //}

        //private static void CompareLiteIfIdentical(DataSet tHTempDS)
        //{
        //    int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
        //    string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
        //    //проход по всем таблицам рабочего dataset
        //    for (int t = 0; t < tcount; t++)
        //    {
        //        //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
        //        bool b = ResetDGVDataSource(t);

        //        using (var table = AppData.CurrentProject.FilesContent.Tables[t])
        //        {
        //            //skip table if there is no untranslated lines
        //            if (FunctionsTable.IsTableColumnCellsAll(table))
        //                continue;

        //            string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
        //            Logger.Info(tableprogressinfo);

        //            int rcount = table.Rows.Count;
        //            //проход по всем строкам таблицы рабочего dataset
        //            for (int r = 0; r < rcount; r++)
        //            {
        //                //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");

        //                var TranslationRow = table.Rows[r];
        //                var TranslationCell = TranslationRow.Field<string>(1);
        //                if (TranslationCell == null || string.IsNullOrEmpty(TranslationCell))
        //                {
        //                    var DBRow = tHTempDS.Tables[t].Rows[r];
        //                    if (Equals(TranslationRow[0], DBRow[0]))
        //                    {
        //                        AppData.CurrentProject.FilesContent.Tables[t].Rows[r][1] = DBRow[1];
        //                    }
        //                }
        //            }

        //            if(b) ResetDGVDataSource(-1, false, table);
        //        }
        //    }
        //    
        //}

        ///// <summary>
        ///// load translation from dictionary to dataset tables
        ///// </summary>
        ///// <param name="db"></param>
        ///// <param name="forced"></param>
        //internal static void THLoadDBCompareFromDictionary(Dictionary<string, string> db, bool forced = false)
        //{
        //    //Stopwatch timer = new Stopwatch();
        //    //timer.Start();

        //    //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
        //    int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
        //    if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
        //    {
        //        return;
        //    }

        //    //int RecordsCounter = 1;
        //    int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
        //    string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
        //    //проход по всем таблицам рабочего dataset
        //    for (int t = 0; t < tcount; t++)
        //    {
        //        //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
        //        bool b = ResetDGVDataSource(t);

        //        using (var table = AppData.CurrentProject.FilesContent.Tables[t])
        //        {
        //            //skip table if there is no untranslated lines
        //            if (!forced && FunctionsTable.IsTableColumnCellsAll(table))
        //                continue;

        //            string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
        //            Logger.Info(tableprogressinfo);

        //            int rcount = table.Rows.Count;
        //            //проход по всем строкам таблицы рабочего dataset
        //            for (int r = 0; r < rcount; r++)
        //            {
        //                //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
        //                var Row = table.Rows[r];
        //                var CellTranslation = Row.Field<string>(otranscol);
        //                if (forced || string.IsNullOrEmpty(CellTranslation))
        //                {
        //                    var origCellValue = Row.Field<string>(0);
        //                    if (db.ContainsKey(origCellValue) && db[origCellValue].Length > 0)
        //                    {
        //                        //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = db[origCellValue];
        //                        Row[otranscol] = db[origCellValue];
        //                    }
        //                    else if (AppSettings.DBTryToCheckLinesOfEachMultilineValue)
        //                    {
        //                        if (origCellValue.IsMultiline())
        //                        {
        //                            bool IsAllLinesTranslated = true;
        //                            List<string> mergedlines = new List<string>();
        //                            foreach (var line in origCellValue.SplitToLines())
        //                            {
        //                                if (line.HaveMostOfRomajiOtherChars())
        //                                {
        //                                    mergedlines.Add(line);
        //                                }
        //                                else if (db.TryGetValue(line, out string value) && value.Length > 0)
        //                                {
        //                                    mergedlines.Add(value);
        //                                }
        //                                else
        //                                {
        //                                    IsAllLinesTranslated = false;
        //                                }
        //                            }
        //                            if (IsAllLinesTranslated && mergedlines.Count > 0)
        //                            {
        //                                Row[otranscol] = string.Join(Environment.NewLine, mergedlines);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (b)
        //            {
        //                ResetDGVDataSource(-1, false, table);
        //            }
        //        }
        //    }

        //    //0.051
        //    //timer.Stop();
        //    //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
        //    //MessageBox.Show(difference.ToString());
        //    
        //    System.Media.SystemSounds.Beep.Play();
        //}

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal static void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string, string> db, bool forced = false)
        {
            int translationColumnIndex = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;

            if (translationColumnIndex <= 0)
            {
                return;
            }

            var workTableDatagridview = AppData.Main.THFileElementsDataGridView;
            var filesList = AppData.THFilesList;

            Parallel.ForEach(AppData.CurrentProject.FilesContent.Tables.Cast<DataTable>(), (table, state, tableIndex) =>
            {
                if (!forced && FunctionsTable.IsTableColumnCellsAll(table))
                {
                    return;
                }

                bool resetDGV = ResetDGVDataSource(tableIndex, filesList, workTableDatagridview);

                string tableProgressInfo = string.Format("{0} {1}: {2}>{3}/{4}", T._("Load"), T._(THSettings.TranslationColumnName), table.TableName, tableIndex, AppData.CurrentProject.FilesContent.Tables.Count);
                Logger.Info(tableProgressInfo);

                bool dbTryToCheckLinesOfEachMultilineValue = AppSettings.DBTryToCheckLinesOfEachMultilineValue;

                foreach (DataRow row in table.Rows)
                {
                    var translation = row.Field<string>(translationColumnIndex);
                    if (!forced && !string.IsNullOrEmpty(translation))
                    {
                        continue;
                    }

                    var originalCellValue = row.Field<string>(0);
                    var isRN = originalCellValue.IndexOf("\r\n") != -1;
                    string translatedValue = null;

                    if (db.TryGetValue(originalCellValue, out translatedValue) || db.TryGetValue(originalCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n"), out translatedValue))
                    {
                        row.SetValue(translationColumnIndex, translatedValue);
                    }
                    else if (dbTryToCheckLinesOfEachMultilineValue && originalCellValue.IsMultiline())
                    {
                        var mergedLines = new List<string>();
                        bool isAllLinesTranslated = true;

                        foreach (var line in originalCellValue.SplitToLines())
                        {
                            if (line.HaveMostOfRomajiOtherChars())
                            {
                                mergedLines.Add(line);
                            }
                            else if (db.TryGetValue(line, out translatedValue))
                            {
                                if (string.IsNullOrEmpty(translatedValue))
                                {
                                    isAllLinesTranslated = false;
                                    break;
                                }

                                mergedLines.Add(translatedValue);
                            }
                            else
                            {
                                isAllLinesTranslated = false;
                                break;
                            }
                        }

                        if (isAllLinesTranslated && mergedLines.Count > 0)
                        {
                            row.SetValue(translationColumnIndex, string.Join(Environment.NewLine, mergedLines));
                        }
                    }
                }

                if (resetDGV)
                {
                    ResetDGVDataSource(-1, filesList, workTableDatagridview, false, table);
                }

            });

            
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forceOverwriteTranslations"></param>
        internal static void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db, bool forceOverwriteTranslations = false)
        {
            var tables = AppData.CurrentProject.FilesContent.Tables;
            var translationColIndex = tables[0].Columns[THSettings.TranslationColumnName].Ordinal;

            if (translationColIndex < 1) return;

            var progressMessage = $"{T._("Load")} {T._(THSettings.TranslationColumnName)}:";

            var workTableDatagridview = AppData.Main.THFileElementsDataGridView;
            var filesList = AppData.THFilesList;

            _ = Parallel.ForEach(tables.Cast<DataTable>(), (table, _, tableIndex) =>
            {
                var isTableReset = ResetDGVDataSource(tableIndex, filesList, workTableDatagridview);

                if (!forceOverwriteTranslations && FunctionsTable.IsTableColumnCellsAll(table))
                {
                    return;
                }

                var tableProgressMessage = $"{progressMessage} {table.TableName}>{tableIndex + 1}/{tables.Count}";
                Logger.Info(tableProgressMessage);

                var rows = table.Rows;
                var rowCount = rows.Count;

                for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    var row = rows[rowIndex];
                    var translationValue = row.Field<string>(translationColIndex);

                    if (!forceOverwriteTranslations && !string.IsNullOrEmpty(translationValue))
                    {
                        continue;
                    }

                    var origCellValue = row.Field<string>(0);
                    var dbFound = db.TryGetValue(origCellValue, out var dbFilesListByOriginal);
                    if (!dbFound)
                    {
                        var isRN = origCellValue.IndexOf("\r\n") != -1;
                        var altOrigCellValue = origCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n");
                        dbFound = db.TryGetValue(altOrigCellValue, out dbFilesListByOriginal);
                    }

                    if (dbFound)
                    {
                        foreach (var fileLinesListByRowIndex in dbFilesListByOriginal.Values)
                        {
                            if (fileLinesListByRowIndex.TryGetValue(rowIndex, out var dbTranslation))
                            {
                                row.SetValue(translationColIndex, dbTranslation);
                                break;
                            }
                            else if (fileLinesListByRowIndex.Values.FirstOrDefault() is string firstTranslation)
                            {
                                row.SetValue(translationColIndex, firstTranslation);
                                break;
                            }
                        }
                    }
                    else if (origCellValue.IsMultiline() && AppSettings.DBTryToCheckLinesOfEachMultilineValue)
                    {
                        var mergedLines = new List<string>();
                        var allLinesTranslated = true;

                        foreach (var line in origCellValue.SplitToLines())
                        {
                            if (line.HaveMostOfRomajiOtherChars())
                            {
                                mergedLines.Add(line);
                            }
                            else if (db.TryGetValue(line, out var tablesList))
                            {
                                if (tablesList.Values.FirstOrDefault()?.Values.FirstOrDefault() is string translation)
                                {
                                    mergedLines.Add(translation);
                                }
                                else
                                {
                                    allLinesTranslated = false;
                                    break;
                                }
                            }
                            else
                            {
                                allLinesTranslated = false;
                                break;
                            }
                        }

                        if (allLinesTranslated && mergedLines.Count > 0)
                        {
                            row.SetValue(translationColIndex, string.Join(Environment.NewLine, mergedLines));
                        }
                    }
                }

                if (isTableReset)
                {
                    ResetDGVDataSource(-1, filesList, workTableDatagridview, false, table);
                }

                Logger.Info(tableProgressMessage);
            });

            
            System.Media.SystemSounds.Beep.Play();
        }

        //internal static void THLoadDBCompareFromDictionaryParallelRows(Dictionary<string, string> db)
        //{
        //    //Stopwatch timer = new Stopwatch();
        //    //timer.Start();

        //    //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
        //    int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
        //    if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
        //    {
        //        return;
        //    }

        //    //int RecordsCounter = 1;
        //    int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
        //    string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
        //    //проход по всем таблицам рабочего dataset
        //    for (int t = 0; t < tcount; t++)
        //    {
        //        //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
        //        bool b = ResetDGVDataSource(t);
        //        //skip table if there is no untranslated lines
        //        if (!FunctionsTable.IsTableColumnCellsAll(AppData.CurrentProject.FilesContent.Tables[t]))
        //        {
        //            string tableprogressinfo = infomessage + AppData.CurrentProject.FilesContent.Tables[t].TableName + ">" + t + "/" + tcount;
        //            Logger.Info(tableprogressinfo);

        //            int rcount = AppData.CurrentProject.FilesContent.Tables[t].Rows.Count;
        //            //проход по всем строкам таблицы рабочего dataset
        //            Parallel.For(0, rcount, r =>
        //            {
        //                //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
        //                var Row = AppData.CurrentProject.FilesContent.Tables[t].Rows[r];
        //                var CellTranslation = Row.Field<string>(otranscol);
        //                if (CellTranslation == null || string.IsNullOrEmpty(CellTranslation))
        //                {
        //                    var origCellValue = Row.Field<string>(0);
        //                    string dbvalue;
        //                    if (db.ContainsKey(origCellValue) && (dbvalue = db[origCellValue]).Length > 0)
        //                    {
        //                        //ссылка о проблемах
        //                        //https://stackoverflow.com/questions/29541767/index-out-of-range-exception-in-using-parallelfor-loop
        //                        lock (Row)
        //                        {
        //                            //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = dbvalue;
        //                            Row[otranscol] = dbvalue;
        //                        }
        //                    }
        //                }
        //            });
        //        }
        //    };

        //    //0.051
        //    //timer.Stop();
        //    //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
        //    //MessageBox.Show(difference.ToString());
        //    
        //    System.Media.SystemSounds.Beep.Play();
        //}
        private static bool ResetDGVDataSource(long tableIndex, ListBox filesList, DataGridView dgv, bool isReset = true, DataTable table = null)
        {
            bool b = false;

            if (dgv.InvokeRequired)
            {
                dgv.Invoke(new Action(() =>
                {
                    b = ResetDGVDataSource(tableIndex, filesList, dgv, isReset, table);
                }));
            }
            else
            {
                if ((isReset && dgv.DataSource != null || !isReset && dgv.DataSource == null) && filesList.SelectedIndex == tableIndex)
                {
                    dgv.DataSource = isReset ? null : table;
                    dgv.Update();
                    dgv.Refresh();
                    b = true;
                }
            }

            return b;
        }

        ///// <summary>
        ///// loading dict db but preget table-row data from main tables
        ///// </summary>
        ///// <param name="db"></param>
        //internal static void THLoadDBCompareFromDictionary2(Dictionary<string, string> db)
        //{
        //    //Stopwatch timer = new Stopwatch();
        //    //timer.Start();

        //    Dictionary<string, string> tableData = AppData.CurrentProject.FilesContent.GetTableRowsDataToDictionary();

        //    //проход по всем таблицам рабочего dataset
        //    string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
        //    //int tableDataKeysCount = tableData.Keys.Count;
        //    //int cur/* = 0*/;
        //    Logger.Info(infomessage);
        //    foreach (var original in tableData.Keys)
        //    {
        //        //ProjectData.Main.ProgressInfo(true, infomessage + cur +"/"+ tableDataKeysCount);
        //        if (db.ContainsKey(original))
        //        {
        //            foreach (var TableRowPair in tableData[original].Split('|'))
        //            {
        //                AppData.CurrentProject.FilesContent.Tables[int.Parse(TableRowPair.Split('!')[0], CultureInfo.InvariantCulture)].Rows[int.Parse(TableRowPair.Split('!')[1], CultureInfo.InvariantCulture)][1] = db[original];
        //            }
        //        }
        //        //cur++;
        //    }
        //    //MessageBox.Show(DT1.Rows[0][0] as string);


        //    //00.1512865
        //    //timer.Stop();
        //    //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
        //    //MessageBox.Show(difference.ToString());
        //    
        //    System.Media.SystemSounds.Beep.Play();
        //}

        internal async static Task LoadTranslationIfNeed(bool forceLoad = false, bool askIfLoadDB = true, bool askIfLoadAllDB = true)
        {
            var dbPath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName() + FunctionsDBFile.GetDBCompressionExt());
            dbPath = FunctionsDBFile.SearchByAllDBFormatExtensions(dbPath);

            if (File.Exists(dbPath) && (!askIfLoadDB || (askIfLoadDB && MessageBox.Show(T._("Found translation DB. Load it?"), T._("Load translation DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)))
            {
                if (forceLoad) await new ClearCells().AllT().ConfigureAwait(true);

                await Task.Run(() => FunctionsDBFile.LoadTranslationFromDB(sPath: dbPath, UseAllDB: false, forced: true)).ConfigureAwait(false);
            }
            else if (askIfLoadAllDB)
            {
                var loadTranslationsFromAllDBQuestion = MessageBox.Show(T._("Try to find translations in all avalaible DB? (Can take some time)"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (loadTranslationsFromAllDBQuestion != DialogResult.Yes) return;

                await Task.Run(() => FunctionsDBFile.LoadTranslationFromDB(sPath: string.Empty, UseAllDB: true)).ConfigureAwait(false);
            }
        }
    }
}
