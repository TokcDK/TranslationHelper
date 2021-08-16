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
    class FunctionsLoadTranslationDb
    {
        

        public FunctionsLoadTranslationDb()
        {
            
        }

        internal void ThLoadDbCompare(DataSet thTempDs)
        {
            if (!Properties.Settings.Default.IsFullComprasionDBloadEnabled && FunctionsTable.IsDataSetsElementsCountIdentical(ProjectData.ThFilesElementsDataset, thTempDs))
            {
                CompareLiteIfIdentical(thTempDs);
                return;
            }

            //using (DataSet THTempDS = new DataSet())
            //{
            //    //LogToFile("cleaning THTempDS and refreshing dgv", true);
            //    THTempDS.Reset();//очистка временной таблицы
            //}

            //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath); // write lastsavedpath

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //LogToFile("ocol=" + ocol);
            //оптимизация. Не искать колонку перевода, если она по стандарту первая

            int ttranscol = thTempDs.Tables[0].Columns["Translation"].Ordinal;
            if (ttranscol == 0 || ttranscol == -1)
            {
                return;
            }

            //Оптимизация. Стартовые значения номера таблицы и строки для таблицы с загруженным переводом
            int ttablestartindex = 0;
            int trowstartindex = 0;

            int tcount = ProjectData.ThFilesElementsDataset.Tables.Count;
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                if (ProjectData.Main.THFileElementsDataGridView.DataSource != null && ProjectData.Main.THFilesList.SelectedIndex == t)
                {
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var table = ProjectData.ThFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableRowsCompleted(table))
                        continue;

                    string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
                    ProjectData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var row = table.Rows[r];
                        var cellTranslation = row[otranscol];
                        if (cellTranslation == null || string.IsNullOrEmpty(cellTranslation as string))
                        {
                            bool translationWasSet = false;

                            var dbTablesCount = thTempDs.Tables.Count;
                            //проход по всем таблицам dataset с переводом
                            for (int t1 = ttablestartindex; t1 < dbTablesCount; t1++)
                            {
                                using (var dbTable = thTempDs.Tables[t1])
                                {
                                    if (dbTable.Columns.Count > 1)
                                    {
                                        var dbTableRowsCount = thTempDs.Tables[t1].Rows.Count;
                                        //проход по всем строкам таблицы dataset с переводом
                                        for (int r1 = trowstartindex; r1 < dbTableRowsCount; r1++)
                                        {
                                            var dbRow = dbTable.Rows[r1];
                                            var dbCellTranslation = dbRow[ttranscol];
                                            if (dbCellTranslation != null && !string.IsNullOrEmpty(dbCellTranslation as string))
                                            {
                                                try
                                                {
                                                    if (Equals(row[0], dbRow[0]))
                                                    {
                                                        ProjectData.ThFilesElementsDataset.Tables[t].Rows[r][otranscol] = dbCellTranslation;
                                                        translationWasSet = true;

                                                        trowstartindex = Properties.Settings.Default.IsFullComprasionDBloadEnabled ? 0 : r1;//запоминание последнего индекса строки, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
                                                        break;
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }
                                        if (translationWasSet)//если перевод был присвоен, выйти из цикла таблицы с переводом
                                        {
                                            ttablestartindex = Properties.Settings.Default.IsFullComprasionDBloadEnabled ? 0 : t1;//запоминание последнего индекса таблицы, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
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

            ProjectData.Main.ProgressInfo(false);
        }

        private void CompareLiteIfIdentical(DataSet tHTempDs)
        {
            int tcount = ProjectData.ThFilesElementsDataset.Tables.Count;
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                if (ProjectData.Main.THFileElementsDataGridView.DataSource != null && ProjectData.Main.THFilesList.SelectedIndex == t)
                {
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var dt = ProjectData.ThFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableRowsCompleted(dt))
                        continue;

                    string tableprogressinfo = infomessage + dt.TableName + ">" + t + "/" + tcount;
                    ProjectData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = dt.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");

                        var translationRow = dt.Rows[r];
                        var translationCell = translationRow[1];
                        if (translationCell == null || string.IsNullOrEmpty(translationCell as string))
                        {
                            var dbRow = tHTempDs.Tables[t].Rows[r];
                            if (Equals(translationRow[0], dbRow[0]))
                            {
                                ProjectData.ThFilesElementsDataset.Tables[t].Rows[r][1] = dbRow[1];
                            }
                        }
                    }
                }
            }
            ProjectData.Main.ProgressInfo(false);
        }

        /// <summary>
        /// load translation from dictionary to dataset tables
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal void ThLoadDbCompareFromDictionary(Dictionary<string, string> db, bool forced=false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = ProjectData.ThFilesElementsDataset.Tables.Count;
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource != null && ProjectData.Main.THFilesList.SelectedIndex == t));
                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var table = ProjectData.ThFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (!forced && FunctionsTable.IsTableRowsCompleted(table))
                        continue;

                    string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
                    ProjectData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var row = table.Rows[r];
                        var cellTranslation = row[otranscol];
                        if (forced || cellTranslation == null || string.IsNullOrEmpty(cellTranslation as string))
                        {
                            var origCellValue = row[0] as string;
                            if (db.ContainsKey(origCellValue) && db[origCellValue].Length > 0)
                            {
                                //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = db[origCellValue];
                                row[otranscol] = db[origCellValue];
                            }
                            else if (Properties.Settings.Default.DBTryToCheckLinesOfEachMultilineValue)
                            {
                                if (origCellValue.IsMultiline())
                                {
                                    bool isAllLinesTranslated = true;
                                    List<string> mergedlines = new List<string>();
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
                                            isAllLinesTranslated = false;
                                        }
                                    }
                                    if (isAllLinesTranslated && mergedlines.Count > 0)
                                    {
                                        row[otranscol] = string.Join(Environment.NewLine, mergedlines);
                                    }
                                }
                            }
                        }
                    }
                }

                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource == null && ProjectData.Main.THFilesList.SelectedIndex == -1));
                    if (b)
                    {
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[t]));
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                    }
                }
            }

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            ProjectData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal void ThLoadDbCompareFromDictionaryParallellTables(Dictionary<string, string> db, bool forced=false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = ProjectData.ThFilesElementsDataset.Tables.Count;
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //проход по всем таблицам рабочего dataset

            Parallel.For(0, tcount, t =>
            //for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource != null && ProjectData.Main.THFilesList.SelectedIndex == t));
                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var table = ProjectData.ThFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (!forced && FunctionsTable.IsTableRowsCompleted(table))
                        return;

                    string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
                    ProjectData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var row = table.Rows[r];
                        var cellTranslation = row[otranscol];
                        if (forced || cellTranslation == null || string.IsNullOrEmpty(cellTranslation as string))
                        {
                            var origCellValue = row[0] as string;
                            if (db.ContainsKey(origCellValue) && db[origCellValue].Length > 0)
                            {
                                //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = db[origCellValue];
                                row[otranscol] = db[origCellValue];
                            }
                            else if (Properties.Settings.Default.DBTryToCheckLinesOfEachMultilineValue)
                            {
                                if (origCellValue.IsMultiline())
                                {
                                    bool isAllLinesTranslated = true;
                                    List<string> mergedlines = new List<string>();
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
                                            isAllLinesTranslated = false;
                                        }
                                    }
                                    if (isAllLinesTranslated && mergedlines.Count > 0)
                                    {
                                        row[otranscol] = string.Join(Environment.NewLine, mergedlines);
                                    }
                                }
                            }
                        }
                    }
                }

                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource == null && ProjectData.Main.THFilesList.SelectedIndex == -1));
                    if (b)
                    {
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[t]));
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                    }
                }
            });

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            ProjectData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// load translation from dictionary to dataset tables (Parallell tables variant)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="forced"></param>
        internal void ThLoadDbCompareFromDictionaryParallellTables(Dictionary<string/*original*/, Dictionary<string/*table name*/, Dictionary<int/*row index*/, string/*translation*/>>> db, bool forced = false)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = ProjectData.ThFilesElementsDataset.Tables.Count;
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //проход по всем таблицам рабочего dataset

            Parallel.For(0, tcount, t =>
            //for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource != null && ProjectData.Main.THFilesList.SelectedIndex == t));
                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                }

                using (var table = ProjectData.ThFilesElementsDataset.Tables[t])
                {
                    var rowIndexShift = 0;

                    //skip table if there is no untranslated lines
                    if (!forced && FunctionsTable.IsTableRowsCompleted(table))
                        return;

                    string tableprogressinfo = infomessage + table.TableName + ">" + t + "/" + tcount;
                    ProjectData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var row = table.Rows[r];
                        var cellTranslation = row[otranscol];
                        if (forced || cellTranslation == null || string.IsNullOrEmpty(cellTranslation as string))
                        {
                            var origCellValue = row[0] as string;
                            var found = false;
                            if (db.ContainsKey(origCellValue))
                            {
                                var dbo = db[origCellValue];
                                if (dbo.ContainsKey(table.TableName))
                                {
                                    var rowIndexWithShift = r + rowIndexShift;
                                    var dbt = dbo[table.TableName];
                                    if (dbt.ContainsKey(r) /*&& !string.IsNullOrEmpty(db[origCellValue][Table.TableName][RowIndexWithShift])*/)
                                    {
                                        row[otranscol] = dbt[rowIndexWithShift];
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
                                            row[otranscol] = tr;
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach(var tn in db[origCellValue].Values)
                                    {
                                        foreach (var tr in tn.Values)
                                        {
                                            row[otranscol] = tr;
                                            found = true;
                                            break;
                                        }
                                        break;
                                    }
                                }
                            }

                            if (!found && Properties.Settings.Default.DBTryToCheckLinesOfEachMultilineValue)
                            {
                                if (origCellValue.IsMultiline())
                                {
                                    bool isAllLinesTranslated = true;
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
                                            isAllLinesTranslated = false;
                                        }
                                    }
                                    if (isAllLinesTranslated && mergedlines.Count > 0)
                                    {
                                        row[otranscol] = string.Join(Environment.NewLine, mergedlines);
                                    }
                                }
                            }
                        }
                    }
                }

                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource == null && ProjectData.Main.THFilesList.SelectedIndex == -1));
                    if (b)
                    {
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = ProjectData.ThFilesElementsDataset.Tables[t]));
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                        ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                    }
                }
            });

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            ProjectData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        internal void ThLoadDbCompareFromDictionaryParallelRows(Dictionary<string, string> db)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //int RecordsCounter = 1;
            int tcount = ProjectData.ThFilesElementsDataset.Tables.Count;
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                //выключение таблицы, если она была открыта, для предотвращения тормозов из за прорисовки
                bool b = false;
                ProjectData.Main.Invoke((Action)(() => b = ProjectData.Main.THFileElementsDataGridView.DataSource != null && ProjectData.Main.THFilesList.SelectedIndex == t));
                if (b)
                {
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.DataSource = null));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Update()));
                    ProjectData.Main.Invoke((Action)(() => ProjectData.Main.THFileElementsDataGridView.Refresh()));
                }
                //skip table if there is no untranslated lines
                if (!FunctionsTable.IsTableRowsCompleted(ProjectData.ThFilesElementsDataset.Tables[t]))
                {
                    string tableprogressinfo = infomessage + ProjectData.ThFilesElementsDataset.Tables[t].TableName + ">" + t + "/" + tcount;
                    ProjectData.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = ProjectData.ThFilesElementsDataset.Tables[t].Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    Parallel.For(0, rcount, r =>
                    {
                        //ProjectData.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var row = ProjectData.ThFilesElementsDataset.Tables[t].Rows[r];
                        var cellTranslation = row[otranscol];
                        if (cellTranslation == null || string.IsNullOrEmpty(cellTranslation as string))
                        {
                            var origCellValue = row[0] as string;
                            string dbvalue;
                            if (db.ContainsKey(origCellValue) && (dbvalue = db[origCellValue]).Length > 0)
                            {
                                //ссылка о проблемах
                                //https://stackoverflow.com/questions/29541767/index-out-of-range-exception-in-using-parallelfor-loop
                                lock (row)
                                {
                                    //ProjectData.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = dbvalue;
                                    row[otranscol] = dbvalue;
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
            ProjectData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// loading dict db but preget table-row data from main tables
        /// </summary>
        /// <param name="db"></param>
        internal void ThLoadDbCompareFromDictionary2(Dictionary<string, string> db)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            Dictionary<string, string> tableData = ProjectData.ThFilesElementsDataset.GetTableRowsDataToDictionary();

            //проход по всем таблицам рабочего dataset
            string infomessage = T._("Load") + " " + T._("translation") + ":";
            //int tableDataKeysCount = tableData.Keys.Count;
            //int cur/* = 0*/;
            ProjectData.Main.ProgressInfo(true, infomessage);
            foreach (var original in tableData.Keys)
            {
                //ProjectData.Main.ProgressInfo(true, infomessage + cur +"/"+ tableDataKeysCount);
                if (db.ContainsKey(original))
                {
                    foreach (var tableRowPair in tableData[original].Split('|'))
                    {
                        ProjectData.ThFilesElementsDataset.Tables[int.Parse(tableRowPair.Split('!')[0], CultureInfo.InvariantCulture)].Rows[int.Parse(tableRowPair.Split('!')[1], CultureInfo.InvariantCulture)][1] = db[original];
                    }
                }
                //cur++;
            }
            //MessageBox.Show(DT1.Rows[0][0] as string);


            //00.1512865
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            ProjectData.Main.ProgressInfo(false);
            System.Media.SystemSounds.Beep.Play();
        }

        internal async static void LoadTranslationIfNeed()
        {
            var dbPath = Path.Combine(FunctionsDbFile.GetProjectDbFolder(), FunctionsDbFile.GetDbFileName() + FunctionsDbFile.GetDbCompressionExt());
            if (!File.Exists(dbPath))
            {
                FunctionsDbFile.SearchByAllDbFormatExtensions(ref dbPath);
            }
            if (File.Exists(dbPath))
            {
                var loadFoundDbQuestion = MessageBox.Show(T._("Found translation DB. Load it?"), T._("Load translation DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (loadFoundDbQuestion == DialogResult.Yes)
                {
                    await Task.Run(()=>ProjectData.Main.LoadTranslationFromDb(dbPath, false, true)).ConfigureAwait(false);
                }
                else
                {
                    var loadTranslationsFromAllDbQuestion = MessageBox.Show(T._("Try to find translations in all avalaible DB? (Can take some time)"), T._("Load all DB"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (loadTranslationsFromAllDbQuestion == DialogResult.Yes)
                    {
                        await Task.Run(() => ProjectData.Main.LoadTranslationFromDb(string.Empty, true)).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
