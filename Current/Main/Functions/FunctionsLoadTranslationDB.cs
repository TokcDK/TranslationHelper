using System;
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

        internal void THLoadDBCompare(DataSet THTempDS)
        {
            if (!AppSettings.IsFullComprasionDBloadEnabled && FunctionsTable.IsDataSetsElementsCountIdentical(AppData.CurrentProject.FilesContent, THTempDS))
            {
                CompareLiteIfIdentical(THTempDS);
                return;
            }

            //using (DataSet THTempDS = new DataSet())
            //{
            //    //LogToFile("cleaning THTempDS and refreshing dgv", true);
            //    THTempDS.Reset();//очистка временной таблицы
            //}

            //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath); // write lastsavedpath

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //LogToFile("ocol=" + ocol);
            //оптимизация. Не искать колонку перевода, если она по стандарту первая

            int ttranscol = THTempDS.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
            if (ttranscol == 0 || ttranscol == -1)
            {
                return;
            }

            //Оптимизация. Стартовые значения номера таблицы и строки для таблицы с загруженным переводом
            int ttablestartindex = 0;
            int trowstartindex = 0;

            int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                if (AppData.Main.THFileElementsDataGridView.DataSource != null && AppData.Main.THFilesList.GetSelectedIndex() == t)
                {
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var Table = AppData.CurrentProject.FilesContent.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableColumnCellsAll(Table))
                        continue;

                    string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                    AppData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = Table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = Table.Rows[r];
                        var CellTranslation = Row.Field<string>(otranscol);
                        if (string.IsNullOrEmpty(CellTranslation))
                        {
                            bool TranslationWasSet = false;

                            var DBTablesCount = THTempDS.Tables.Count;
                            //проход по всем таблицам dataset с переводом
                            for (int t1 = ttablestartindex; t1 < DBTablesCount; t1++)
                            {
                                using (var DBTable = THTempDS.Tables[t1])
                                {
                                    if (DBTable.Columns.Count > 1)
                                    {
                                        var DBTableRowsCount = THTempDS.Tables[t1].Rows.Count;
                                        //проход по всем строкам таблицы dataset с переводом
                                        for (int r1 = trowstartindex; r1 < DBTableRowsCount; r1++)
                                        {
                                            var DBRow = DBTable.Rows[r1];
                                            var DBCellTranslation = DBRow.Field<string>(ttranscol);
                                            if (DBCellTranslation != null && !string.IsNullOrEmpty(DBCellTranslation))
                                            {
                                                try
                                                {
                                                    if (Equals(Row[0], DBRow[0]))
                                                    {
                                                        AppData.CurrentProject.FilesContent.Tables[t].Rows[r][otranscol] = DBCellTranslation;
                                                        TranslationWasSet = true;

                                                        trowstartindex = AppSettings.IsFullComprasionDBloadEnabled ? 0 : r1;//запоминание последнего индекса строки, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
                                                        break;
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                        if (TranslationWasSet)//если перевод был присвоен, выйти из цикла таблицы с переводом
                                        {
                                            ttablestartindex = AppSettings.IsFullComprasionDBloadEnabled ? 0 : t1;//запоминание последнего индекса таблицы, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
                                            break;
                                        }
                                        else
                                        {
                                            //сбрасывать индекс на ноль, когда все строки были пройдены и таблица меняется, строка должна быть сброшена на ноль
                                            trowstartindex = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            AppData.Main.ProgressInfo(false);
        }

        private void CompareLiteIfIdentical(DataSet tHTempDS)
        {
            int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                if (AppData.Main.THFileElementsDataGridView.DataSource != null && AppData.Main.THFilesList.GetSelectedIndex() == t)
                {
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var DT = AppData.CurrentProject.FilesContent.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableColumnCellsAll(DT))
                        continue;

                    string tableprogressinfo = infomessage + DT.TableName + ">" + t + "/" + tcount;
                    AppData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = DT.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");

                        var TranslationRow = DT.Rows[r];
                        var TranslationCell = TranslationRow.Field<string>(1);
                        if (TranslationCell == null || string.IsNullOrEmpty(TranslationCell))
                        {
                            var DBRow = tHTempDS.Tables[t].Rows[r];
                            if (Equals(TranslationRow[0], DBRow[0]))
                            {
                                AppData.CurrentProject.FilesContent.Tables[t].Rows[r][1] = DBRow[1];
                            }
                        }
                    }
                }
            }
            AppData.Main.ProgressInfo(false);
        }

        /// <summary>
        /// load translation from dictionary to dataset tables
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal void THLoadDBCompareFromDictionary(Dictionary<string, string> db, bool forced = false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource != null && AppData.Main.THFilesList.GetSelectedIndex() == t));
                if (b)
                {
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var Table = AppData.CurrentProject.FilesContent.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (!forced && FunctionsTable.IsTableColumnCellsAll(Table))
                        continue;

                    string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                    AppData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = Table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = Table.Rows[r];
                        var CellTranslation = Row.Field<string>(otranscol);
                        if (forced || string.IsNullOrEmpty(CellTranslation))
                        {
                            var origCellValue = Row.Field<string>(0);
                            if (db.ContainsKey(origCellValue) && db[origCellValue].Length > 0)
                            {
                                //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = db[origCellValue];
                                Row[otranscol] = db[origCellValue];
                            }
                            else if (AppSettings.DBTryToCheckLinesOfEachMultilineValue)
                            {
                                if (origCellValue.IsMultiline())
                                {
                                    bool IsAllLinesTranslated = true;
                                    List<string> mergedlines = new List<string>();
                                    foreach (var line in origCellValue.SplitToLines())
                                    {
                                        if (line.HaveMostOfRomajiOtherChars())
                                        {
                                            mergedlines.Add(line);
                                        }
                                        else if (db.TryGetValue(line, out string value) && value.Length > 0)
                                        {
                                            mergedlines.Add(value);
                                        }
                                        else
                                        {
                                            IsAllLinesTranslated = false;
                                        }
                                    }
                                    if (IsAllLinesTranslated && mergedlines.Count > 0)
                                    {
                                        Row[otranscol] = string.Join(Environment.NewLine, mergedlines);
                                    }
                                }
                            }
                        }
                    }
                }

                if (b)
                {
                    AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource == null && AppData.Main.THFilesList.GetSelectedIndex() == -1));
                    if (b)
                    {
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = AppData.CurrentProject.FilesContent.Tables[t]));
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                    }
                }
            }

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
        /// <param name="forced"></param>
        internal void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string, string> db, bool forced = false)
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

            Parallel.For(0, tcount, t =>
            //for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource != null && AppData.Main.THFilesList.GetSelectedIndex() == t));
                if (b)
                {
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                }

                var Table = AppData.CurrentProject.FilesContent.Tables[t];

                //skip table if there is no untranslated lines
                if (!forced && FunctionsTable.IsTableColumnCellsAll(Table)) return;

                string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                AppData.Main.ProgressInfo(true, tableprogressinfo);

                int rcount = Table.Rows.Count;
                //проход по всем строкам таблицы рабочего dataset
                for (int r = 0; r < rcount; r++)
                {
                    //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                    var Row = Table.Rows[r];
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

                Table.Dispose();

                if (b)
                {
                    AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource == null && AppData.Main.THFilesList.GetSelectedIndex() == -1));
                    if (b)
                    {
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = AppData.CurrentProject.FilesContent.Tables[t]));
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                    }
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
        /// <param name="forced"></param>
        internal void THLoadDBCompareFromDictionaryParallellTables(Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db, bool forced = false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset

            Parallel.For(0, tcount, t =>
            //for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource != null && AppData.Main.THFilesList.GetSelectedIndex() == t));
                if (b)
                {
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var Table = AppData.CurrentProject.FilesContent.Tables[t])
                {
                    var RowIndexShift = 0;

                    //skip table if there is no untranslated lines
                    if (!forced && FunctionsTable.IsTableColumnCellsAll(Table))
                        return;

                    string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                    AppData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = Table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = Table.Rows[r];
                        var CellTranslation = Row.Field<string>(otranscol);
                        if (forced || CellTranslation == null || string.IsNullOrEmpty(CellTranslation))
                        {
                            var origCellValue = Row.Field<string>(0);
                            var found = false;
                            bool isRN = origCellValue.IndexOf("\r\n") != -1;
                            if (db.ContainsKey(origCellValue) || db.ContainsKey(origCellValue = origCellValue.Replace(isRN ? "\r\n" : "\n", isRN ? "\n" : "\r\n")))
                            {
                                var dbo = db[origCellValue];
                                if (dbo.ContainsKey(Table.TableName))
                                {
                                    var RowIndexWithShift = r + RowIndexShift;
                                    var dbt = dbo[Table.TableName];
                                    if (dbt.ContainsKey(r) /*&& !string.IsNullOrEmpty(db[origCellValue][Table.TableName][RowIndexWithShift])*/)
                                    {
                                        Row[otranscol] = dbt[RowIndexWithShift];
                                        found = true;
                                    }
                                    else
                                    {
                                        //for (int s = 0; s < rcount; s++)
                                        //{
                                        //    var sshift = RowIndexShift + s;

                                        //    if (sshift >= rcount)
                                        //    {
                                        //        break;
                                        //    }

                                        //    if (dbt.ContainsKey(RowIndexShift))
                                        //    {
                                        //        RowIndexShift = sshift;
                                        //        Row[otranscol] = dbt[sshift];
                                        //        found = true;
                                        //    }
                                        //}
                                        //if (!found)
                                        //{
                                        //    foreach (var tr in dbt.Values)
                                        //    {
                                        //        Row[otranscol] = tr;
                                        //        found = true;
                                        //        break;
                                        //    }
                                        //}
                                        foreach (var tr in dbt.Values)
                                        {
                                            Row[otranscol] = tr;
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
                                            Row[otranscol] = tr;
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
                                    List<string> mergedlines = new List<string>();
                                    foreach (var line in origCellValue.SplitToLines())
                                    {
                                        if (line.HaveMostOfRomajiOtherChars())
                                        {
                                            mergedlines.Add(line);
                                        }
                                        else if (db.ContainsKey(line) /*&& db[line].Length > 0*/)
                                        {
                                            foreach (var tn in db[line].Values)
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
                                        Row[otranscol] = string.Join(Environment.NewLine, mergedlines);
                                    }
                                }
                            }
                        }
                    }
                }

                if (b)
                {
                    AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource == null && AppData.Main.THFilesList.GetSelectedIndex() == -1));
                    if (b)
                    {
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = AppData.CurrentProject.FilesContent.Tables[t]));
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                        AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                    }
                }
            });

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            AppData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        internal void THLoadDBCompareFromDictionaryParallelRows(Dictionary<string, string> db)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = AppData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = AppData.CurrentProject.FilesContent.Tables.Count;
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                AppData.Main.Invoke((Action)(() => b = AppData.Main.THFileElementsDataGridView.DataSource != null && AppData.Main.THFilesList.GetSelectedIndex() == t));
                if (b)
                {
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.DataSource = null));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Update()));
                    AppData.Main.Invoke((Action)(() => AppData.Main.THFileElementsDataGridView.Refresh()));
                }
                //skip table if there is no untranslated lines
                if (!FunctionsTable.IsTableColumnCellsAll(AppData.CurrentProject.FilesContent.Tables[t]))
                {
                    string tableprogressinfo = infomessage + AppData.CurrentProject.FilesContent.Tables[t].TableName + ">" + t + "/" + tcount;
                    AppData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = AppData.CurrentProject.FilesContent.Tables[t].Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    Parallel.For(0, rcount, r =>
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = AppData.CurrentProject.FilesContent.Tables[t].Rows[r];
                        var CellTranslation = Row.Field<string>(otranscol);
                        if (CellTranslation == null || string.IsNullOrEmpty(CellTranslation))
                        {
                            var origCellValue = Row.Field<string>(0);
                            string dbvalue;
                            if (db.ContainsKey(origCellValue) && (dbvalue = db[origCellValue]).Length > 0)
                            {
                                //ссылка о проблемах
                                //https://stackoverflow.com/questions/29541767/index-out-of-range-exception-in-using-parallelfor-loop
                                lock (Row)
                                {
                                    //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = dbvalue;
                                    Row[otranscol] = dbvalue;
                                }
                            }
                        }
                    });
                }
            };

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            AppData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// loading dict db but preget table-row data from main tables
        /// </summary>
        /// <param name="db"></param>
        internal void THLoadDBCompareFromDictionary2(Dictionary<string, string> db)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            Dictionary<string, string> tableData = AppData.CurrentProject.FilesContent.GetTableRowsDataToDictionary();

            //проход по всем таблицам рабочего dataset
            string infomessage = T._("Load") + " " + T._(THSettings.TranslationColumnName) + ":";
            //int tableDataKeysCount = tableData.Keys.Count;
            //int cur/* = 0*/;
            AppData.Main.ProgressInfo(true, infomessage);
            foreach (var original in tableData.Keys)
            {
                //ProjectData.Main.ProgressInfo(true, infomessage + cur +"/"+ tableDataKeysCount);
                if (db.ContainsKey(original))
                {
                    foreach (var TableRowPair in tableData[original].Split('|'))
                    {
                        AppData.CurrentProject.FilesContent.Tables[int.Parse(TableRowPair.Split('!')[0], CultureInfo.InvariantCulture)].Rows[int.Parse(TableRowPair.Split('!')[1], CultureInfo.InvariantCulture)][1] = db[original];
                    }
                }
                //cur++;
            }
            //MessageBox.Show(DT1.Rows[0][0] as string);


            //00.1512865
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            AppData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        internal async static void LoadTranslationIfNeed()
        {
            var DBPath = Path.Combine(FunctionsDBFile.GetProjectDBFolder(), FunctionsDBFile.GetDBFileName() + FunctionsDBFile.GetDBCompressionExt());
            if (!File.Exists(DBPath))
            {
                FunctionsDBFile.SearchByAllDBFormatExtensions(ref DBPath);
            }
            if (File.Exists(DBPath))
            {
                var LoadFoundDBQuestion = MessageBox.Show(T._("Found translation DB. Load it?"), T._("Load translation DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (LoadFoundDBQuestion == DialogResult.Yes)
                {
                    await Task.Run(() => AppData.Main.LoadTranslationFromDB(DBPath, false, true)).ConfigureAwait(false);
                }
                else
                {
                    var LoadTranslationsFromAllDBQuestion = MessageBox.Show(T._("Try to find translations in all avalaible DB? (Can take some time)"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (LoadTranslationsFromAllDBQuestion == DialogResult.Yes)
                    {
                        await Task.Run(() => AppData.Main.LoadTranslationFromDB(string.Empty, true)).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
