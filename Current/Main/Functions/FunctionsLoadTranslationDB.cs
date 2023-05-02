﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsLoadTranslationDB
    {
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
        //            AppData.Main.ProgressInfo(true, tableprogressinfo);

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

        //    AppData.Main.ProgressInfo(false);
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
        //            AppData.Main.ProgressInfo(true, tableprogressinfo);

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
        //    AppData.Main.ProgressInfo(false);
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
        //            AppData.Main.ProgressInfo(true, tableprogressinfo);

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
        //    AppData.Main.ProgressInfo(false);
        //    System.Media.SystemSounds.Beep.Play();
        //}

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal static void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string, string> db, bool forced = false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;

            //если вдруг колонка была только одна
            if (otranscol == 0 || otranscol == -1) return;

            //int RecordsCounter = 1;
            int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset

            bool DBTryToCheckLinesOfEachMultilineValue = AppSettings.DBTryToCheckLinesOfEachMultilineValue;

            Parallel.For(0, tcount, tableIndex =>
            //for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = ResetDGVDataSource(tableIndex);

                var table = AppData.CurrentProject.FilesContent.Tables[tableIndex];

                //skip table if there is no untranslated lines
                if (!forced && FunctionsTable.IsTableColumnCellsAll(table)) return;

                string tableprogressinfo = infomessage + table.TableName + ">" + tableIndex + "/" + tcount;
                AppData.Main.ProgressInfo(true, tableprogressinfo);

                int rcount = table.Rows.Count;
                //проход по всем строкам таблицы рабочего dataset
                for (int r = 0; r < rcount; r++)
                {
                    //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                    var Row = table.Rows[r];
                    var CellTranslation = Row.Field<string>(otranscol);
                    if (!forced && CellTranslation != null && !string.IsNullOrEmpty(CellTranslation)) continue;

                    var origCellValue = Row.Field<string>(0);
                    var isRN = origCellValue.IndexOf("\r\n") != -1;
                    if ((db.ContainsKey(origCellValue) && db[origCellValue].Length > 0) || (db.ContainsKey(origCellValue = origCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n")) && db[origCellValue].Length > 0))
                    {
                        //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = db[origCellValue];
                        Row[otranscol] = db[origCellValue];
                    }
                    else if (DBTryToCheckLinesOfEachMultilineValue && origCellValue.IsMultiline())
                    {
                        bool IsAllLinesTranslated = true;
                        var mergedlines = new List<string>();
                        foreach (var line in origCellValue.SplitToLines())
                        {
                            if (line.HaveMostOfRomajiOtherChars())
                            {
                                mergedlines.Add(line);
                            }
                            else if (db.ContainsKey(line) && db[line].Length > 0)
                            {
                                mergedlines.Add(db[line]);
                            }
                            else
                            {
                                IsAllLinesTranslated = false;
                            }
                        }

                        if (IsAllLinesTranslated && mergedlines.Count > 0) Row[otranscol] = string.Join(Environment.NewLine, mergedlines);
                    }
                }

                table.Dispose();

                if (b)
                {
                    ResetDGVDataSource(-1, false, table);
                }
            });

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            AppData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forceOverwriteTranslations"></param>
        internal static void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db, bool forceOverwriteTranslations = false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            var tables = AppData.CurrentProject.FilesContent.Tables;

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tablesCount = tables.Count;
            string infoMessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset

            Parallel.For(0, tablesCount, tableIndex =>
            //for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = ResetDGVDataSource(tableIndex);

                using (var table = tables[tableIndex])
                {
                    //skip table if there is no untranslated lines
                    if (!forceOverwriteTranslations && FunctionsTable.IsTableColumnCellsAll(table))
                        return;

                    string tableProgressInfo = infoMessage + table.TableName + ">" + tableIndex + "/" + tablesCount;
                    AppData.Main.ProgressInfo(true, tableProgressInfo);

                    var rows = table.Rows;
                    int rowsCount = rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
                    {
                        var row = rows[rowIndex];
                        var translationValue = row.Field<string>(otranscol);
                        if (forceOverwriteTranslations || string.IsNullOrEmpty(translationValue))
                        {
                            var origCellValue = row.Field<string>(0);
                            var found = false;
                            bool isRN = origCellValue.IndexOf("\r\n") != -1;
                            bool isFoundByOriginal = db.TryGetValue(origCellValue, out var dbFilesListByOriginal);
                            if (!isFoundByOriginal) origCellValue = origCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n");
                            bool isFoundByOriginalAltNewlineSymbols = db.TryGetValue(origCellValue, out var dbFilesListByOriginalAltNewLine);
                            if (isFoundByOriginal || isFoundByOriginalAltNewlineSymbols)
                            {
                                var dbFoundFilesListByOriginal = isFoundByOriginal ? dbFilesListByOriginal : dbFilesListByOriginalAltNewLine;
                                if (dbFoundFilesListByOriginal.TryGetValue(table.TableName, out var dbFileLinesListByRowIndex))
                                {
                                    var dbTranslations = dbFileLinesListByRowIndex;
                                    if (dbTranslations.TryGetValue(rowIndex, out string dbTranslation))
                                    {
                                        if(!row.Field<string>(otranscol).Equals(dbTranslation))
                                        {
                                            row.SetField(otranscol, dbTranslation);
                                        }

                                        found = true;
                                    }
                                    else
                                    {
                                        // get first translation from list
                                        foreach (var tr in dbTranslations.Values)
                                        {
                                            if (!row.Field<string>(otranscol).Equals(tr))
                                            {
                                                row.SetField(otranscol, tr);
                                            }
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var tn in db[origCellValue].Values)
                                    {
                                        foreach (var tr in tn.Values)
                                        {
                                            if (!row.Field<string>(otranscol).Equals(tr))
                                            {
                                                row.SetField(otranscol, tr);
                                            }
                                            found = true;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }

                            if (!found && AppSettings.DBTryToCheckLinesOfEachMultilineValue)
                            {
                                if (origCellValue.IsMultiline())
                                {
                                    bool IsAllLinesTranslated = true;
                                    var mergedlines = new List<string>();
                                    foreach (var line in origCellValue.SplitToLines())
                                    {
                                        if (line.HaveMostOfRomajiOtherChars())
                                        {
                                            mergedlines.Add(line);
                                        }
                                        else if (db.TryGetValue(line, out var tablesList))
                                        {
                                            foreach (var tn in tablesList.Values)
                                            {
                                                foreach (var tr in tn.Values)
                                                {
                                                    mergedlines.Add(tr);
                                                    break;
                                                }
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            IsAllLinesTranslated = false;
                                        }
                                    }
                                    if (IsAllLinesTranslated && mergedlines.Count > 0)
                                    {
                                        var mergedTranslation = string.Join(Environment.NewLine, mergedlines);
                                        if (!row.Field<string>(otranscol).Equals(mergedTranslation))
                                        {
                                            row.SetField(otranscol, mergedTranslation);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (b)
                {
                    ResetDGVDataSource(-1, false, tables[tableIndex]);
                }
            });

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            AppData.Main.ProgressInfo(false);
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
        //            AppData.Main.ProgressInfo(true, tableprogressinfo);

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
        //    AppData.Main.ProgressInfo(false);
        //    System.Media.SystemSounds.Beep.Play();
        //}

        private static bool ResetDGVDataSource(int tableIndex, bool isReset = true, DataTable table = null)
        {
            bool b = false;
            var dgv = AppData.Main.THFileElementsDataGridView;
            AppData.Main.Invoke((Action)(() => b = (isReset 
            ? dgv.DataSource != null 
            : dgv.DataSource == null) 
            && AppData.Main.THFilesList.GetSelectedIndex() == tableIndex));
            if (b)
            {
                AppData.Main.Invoke((Action)(() => 
                dgv.DataSource = table));
                AppData.Main.Invoke((Action)(() => dgv.Update()));
                AppData.Main.Invoke((Action)(() => dgv.Refresh()));
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
        //    AppData.Main.ProgressInfo(true, infomessage);
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
        //    AppData.Main.ProgressInfo(false);
        //    System.Media.SystemSounds.Beep.Play();
        //}

        internal async static void LoadTranslationIfNeed()
        {
            var dbPath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName() + FunctionsDBFile.GetDBCompressionExt());
            if (!File.Exists(dbPath))
            {
                FunctionsDBFile.SearchByAllDBFormatExtensions(ref dbPath);
            }
            if (File.Exists(dbPath))
            {
                var loadFoundDBQuestion = MessageBox.Show(T._("Found translation DB. Load it?"), T._("Load translation DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (loadFoundDBQuestion == DialogResult.Yes)
                {
                    await Task.Run(() => AppData.Main.LoadTranslationFromDB(dbPath, false, true)).ConfigureAwait(false);
                }
                else
                {
                    var loadTranslationsFromAllDBQuestion = MessageBox.Show(T._("Try to find translations in all avalaible DB? (Can take some time)"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (loadTranslationsFromAllDBQuestion == DialogResult.Yes)
                    {
                        await Task.Run(() => AppData.Main.LoadTranslationFromDB(string.Empty, true)).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
