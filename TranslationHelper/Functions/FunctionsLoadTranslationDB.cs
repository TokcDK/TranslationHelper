using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsLoadTranslationDB
    {
        readonly THDataWork thDataWork;

        public FunctionsLoadTranslationDB(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal void THLoadDBCompare(DataSet THTempDS)
        {
            if (!Properties.Settings.Default.IsFullComprasionDBloadEnabled && FunctionsTable.IsDataSetsElementsCountIdentical(thDataWork.THFilesElementsDataset, THTempDS))
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
            int otranscol = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            //LogToFile("ocol=" + ocol);
            //оптимизация. Не искать колонку перевода, если она по стандарту первая

            int ttranscol = THTempDS.Tables[0].Columns["Translation"].Ordinal;
            if (ttranscol == 0 || ttranscol == -1)
            {
                return;
            }

            //Оптимизация. Стартовые значения номера таблицы и строки для таблицы с загруженным переводом
            int ttablestartindex = 0;
            int trowstartindex = 0;

            int tcount = thDataWork.THFilesElementsDataset.Tables.Count;
            string infomessage = T._("loading translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                using (var Table = thDataWork.THFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableRowsCompleted(Table))
                        continue;

                    string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                    thDataWork.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = Table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //thDataWork.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = Table.Rows[r];
                        var CellTranslation = Row[otranscol];
                        if (CellTranslation == null || string.IsNullOrEmpty(CellTranslation as string))
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
                                            var DBCellTranslation = DBRow[ttranscol];
                                            if (DBCellTranslation != null && !string.IsNullOrEmpty(DBCellTranslation as string))
                                            {
                                                try
                                                {
                                                    if (Equals(Row[0], DBRow[0]))
                                                    {
                                                        thDataWork.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = DBCellTranslation;
                                                        TranslationWasSet = true;

                                                        trowstartindex = Properties.Settings.Default.IsFullComprasionDBloadEnabled ? 0 : r1;//запоминание последнего индекса строки, если включена медленная полная рекурсивная проверка IsFullComprasionDBloadEnabled, сканировать с нуля
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
        }

        private void CompareLiteIfIdentical(DataSet tHTempDS)
        {
            int tcount = thDataWork.THFilesElementsDataset.Tables.Count;
            string infomessage = T._("loading translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                using (DataTable DT = thDataWork.THFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableRowsCompleted(DT))
                        continue;

                    string tableprogressinfo = infomessage + DT.TableName + ">" + t + "/" + tcount;
                    thDataWork.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = DT.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        thDataWork.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");

                        var TranslationRow = DT.Rows[r];
                        var TranslationCell = TranslationRow[1];
                        if (TranslationCell == null || string.IsNullOrEmpty(TranslationCell as string))
                        {
                            var DBRow = tHTempDS.Tables[t].Rows[r];
                            if (Equals(TranslationRow[0], DBRow[0]))
                            {
                                thDataWork.THFilesElementsDataset.Tables[t].Rows[r][1] = DBRow[1];
                            }
                        }
                    }
                }
            }
        }

        internal void THLoadDBCompareFromDictionary(Dictionary<string, string> db)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            //Для оптимизации поиск оригинала в обеих таблицах перенесен в начало, чтобы не повторялся
            int otranscol = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            if (otranscol == 0 || otranscol == -1)//если вдруг колонка была только одна
            {
                return;
            }

            int RecordsCounter = 1;
            int tcount = thDataWork.THFilesElementsDataset.Tables.Count;
            string infomessage = T._("loading translation") + ":";
            //проход по всем таблицам рабочего dataset
            for (int t = 0; t < tcount; t++)
            {
                using (var Table = thDataWork.THFilesElementsDataset.Tables[t])
                {
                    //skip table if there is no untranslated lines
                    if (FunctionsTable.IsTableRowsCompleted(Table))
                        continue;

                    string tableprogressinfo = infomessage + Table.TableName + ">" + t + "/" + tcount;
                    thDataWork.Main.ProgressInfo(true, tableprogressinfo);

                    int rcount = Table.Rows.Count;
                    //проход по всем строкам таблицы рабочего dataset
                    for (int r = 0; r < rcount; r++)
                    {
                        //thDataWork.Main.ProgressInfo(true, tableprogressinfo + "[" + r + "/" + rcount + "]");
                        var Row = Table.Rows[r];
                        var CellTranslation = Row[otranscol];
                        if (CellTranslation == null || string.IsNullOrEmpty(CellTranslation as string))
                        {
                            var origCellValue = Row[0] as string;
                            if (db.ContainsKey(origCellValue) && db[origCellValue].Length>0)
                            {
                                thDataWork.THFilesElementsDataset.Tables[t].Rows[r][otranscol] = db[origCellValue];
                            }
                        }
                    }
                }
            }

            //0.051
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
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

            Dictionary<string, string> tableData = thDataWork.THFilesElementsDataset.GetTableRowsDataToDictionary();

            //проход по всем таблицам рабочего dataset
            string infomessage = T._("loading translation") + ":";
            int tableDataKeysCount = tableData.Keys.Count;
            int cur = 0;
            thDataWork.Main.ProgressInfo(true, infomessage);
            foreach (var original in tableData.Keys)
            {
                //thDataWork.Main.ProgressInfo(true, infomessage + cur +"/"+ tableDataKeysCount);
                if (db.ContainsKey(original))
                {
                    foreach(var TableRowPair in tableData[original].Split('|'))
                    {
                        thDataWork.THFilesElementsDataset.Tables[int.Parse(TableRowPair.Split('!')[0], CultureInfo.GetCultureInfo("en-US"))].Rows[int.Parse(TableRowPair.Split('!')[1], CultureInfo.GetCultureInfo("en-US"))][1] = db[original];
                    }
                }
                //cur++;
            }
            //MessageBox.Show(DT1.Rows[0][0] as string);


            //00.1512865
            //timer.Stop();
            //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
            //MessageBox.Show(difference.ToString());
            System.Media.SystemSounds.Beep.Play();
        }
    }
}
