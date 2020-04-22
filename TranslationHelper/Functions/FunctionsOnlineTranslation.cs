using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsOnlineTranslation
    {
        readonly THDataWork thDataWork;

        public FunctionsOnlineTranslation(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        /// <summary>
        /// Translate rows one by one with selected method
        /// </summary>
        /// <param name="OrigColIndex">Original column index</param>
        /// <param name="TableIndex">Start/Current table index</param>
        /// <param name="SelectedIndexes">Selected rows indexes array</param>
        /// <param name="Method">Selected method: a- all tables; t - selected table; s - selected rows</param>
        internal void THOnlineTranslate(int OrigColIndex = 0, int TableIndex = 0, int[] SelectedIndexes = null, string Method = "s"/*ats*/)
        {
            thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem.Visible = true));
            thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem1.Visible = true));
            //translationInteruptToolStripMenuItem.Visible = true;
            //translationInteruptToolStripMenuItem1.Visible = true;

            try
            {
                using (DataSet THTranslationCache = new DataSet())
                {
                    //FunctionsTable.TranslationCacheInit(THTranslationCache);
                    CacheInitWhenNeed(thDataWork);

                    //количество таблиц, строк и индекс троки для использования в переборе строк и таблиц
                    int TableMaxIndex;
                    int RowsCountInTable;
                    int CurrentRowIndex;
                    TableMaxIndex = (Method == "a") ? thDataWork.THFilesElementsDataset.Tables.Count : TableMaxIndex = TableIndex + 1;
                    //if (method == "a")
                    //{
                    //    tablescount = THFilesElementsDataset.Tables.Count;//все таблицы в dataset
                    //}
                    //else
                    //{
                    //    tablescount = tableindex + 1;//одна таблица с индексом на один больше индекса стартовой
                    //}

                    //сброс кеша в GoogleAPI
                    GoogleAPI.ResetCache();

                    //перебор таблиц dataset
                    for (int t = TableIndex; t < TableMaxIndex; t++)
                    {
                        RowsCountInTable = (Method == "a" || Method == "t") ? thDataWork.THFilesElementsDataset.Tables[t].Rows.Count : SelectedIndexes.Length;
                        //if (method == "a" || method == "t")
                        //{
                        //    //все строки в выбранной таблице
                        //    rowscount = THFilesElementsDataset.Tables[t].Rows.Count;
                        //}
                        //else
                        //{
                        //    //все выделенные строки в выбранной таблице
                        //    rowscount = selindexes.Length;
                        //}

                        //перебор строк таблицы
                        for (int r = 0; r < RowsCountInTable; r++)
                        {
                            if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                            {
                                CacheUnloadWhenNeed(thDataWork);
                                Thread.CurrentThread.Abort();
                                return;
                            }
                            else if (thDataWork.Main.InteruptTranslation)
                            {
                                //translationInteruptToolStripMenuItem.Visible = false;
                                //translationInteruptToolStripMenuItem1.Visible = false;
                                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem.Visible = false));
                                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem1.Visible = false));
                                thDataWork.Main.InteruptTranslation = false;
                                Thread.CurrentThread.Abort();
                                return;
                            }

                            string progressinfo;
                            if (Method == "s")
                            {
                                progressinfo = T._("getting translation: ") + (r + 1) + "/" + RowsCountInTable;
                                //индекс = первому из заданного списка выбранных индексов
                                CurrentRowIndex = SelectedIndexes[r];
                            }
                            else if (Method == "t")
                            {
                                progressinfo = T._("getting translation: ") + (r + 1) + "/" + RowsCountInTable;
                                //индекс с нуля и до последней строки
                                CurrentRowIndex = r;
                            }
                            else
                            {
                                progressinfo = T._("getting translation: ") + t + "/" + TableMaxIndex + "::" + (r + 1) + "/" + RowsCountInTable;
                                //индекс с нуля и до последней строки
                                CurrentRowIndex = r;
                            }

                            thDataWork.Main.ProgressInfo(true, progressinfo);
                            //LogToFile("111=" + 111, true);
                            //проверка пустого значения поля для перевода
                            //if (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString()))
                            if ((thDataWork.THFilesElementsDataset.Tables[t].Rows[CurrentRowIndex][OrigColIndex + 1] + string.Empty).Length == 0)
                            {
                                var row = thDataWork.THFilesElementsDataset.Tables[t].Rows[CurrentRowIndex];
                                string InputValue = row[OrigColIndex] + string.Empty;
                                //LogToFile("1 inputvalue=" + inputvalue, true);
                                //проверка наличия заданного процента romaji или other в оригинале
                                //if ( SelectedLocalePercentFromStringIsNotValid(THFileElementsDataGridView[cind, rind].Value.ToString()) || SelectedLocalePercentFromStringIsNotValid(THFileElementsDataGridView[cind, rind].Value.ToString(), "other"))

                                //string ResultValue = FunctionsTable.TranslationCacheFind(THTranslationCache, InputValue);
                                string ResultValue = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(InputValue);

                                if (ResultValue.Length != 0)
                                {
                                    row[OrigColIndex + 1] = ResultValue;
                                    //THAutoSetValueForSameCells(t, rowindex, cind);
                                }
                                else
                                {
                                    //LogToFile("resultvalue from cache is empty. resultvalue=" + resultvalue, true);
                                    //string[] inputvaluearray = InputValue.Split(new string[2] { Environment.NewLine, @"\n" }, StringSplitOptions.None);

                                    if (FunctionsString.IsMultiline(InputValue))
                                    {
                                        ResultValue = TranslateMultilineValue(InputValue.SplitToLines().ToArray()/*, THTranslationCache*/);
                                    }
                                    else
                                    {
                                        string ExtractedValue = thDataWork.Main.THExtractTextForTranslation(InputValue);
                                        //LogToFile("extractedvalue="+ extractedvalue,true);
                                        if (ExtractedValue.Length == 0 || ExtractedValue == InputValue)
                                        {
                                            ResultValue = GoogleAPI.Translate(InputValue);

                                            //LogToFile("resultvalue=" + resultvalue, true);
                                            //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, InputValue, ResultValue);
                                            FunctionsTable.AddToTranslationCacheIfValid(thDataWork, InputValue, ResultValue);
                                        }
                                        else
                                        {
                                            //string CachedExtractedValue = FunctionsTable.TranslationCacheFind(THTranslationCache, ExtractedValue);
                                            string CachedExtractedValue = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(ExtractedValue);
                                            //LogToFile("cachedvalue=" + cachedvalue, true);
                                            if (CachedExtractedValue.Length == 0)
                                            {
                                                string OnlineValue = GoogleAPI.Translate(ExtractedValue);//из исходников ESPTranslator 

                                                if (Equals(ExtractedValue, OnlineValue))
                                                {
                                                }
                                                else
                                                {
                                                    //resultvalue = inputvalue.Replace(extractedvalue, onlinevalue);
                                                    ResultValue = PasteTranslationBackIfExtracted(OnlineValue, InputValue, ExtractedValue);

                                                    //LogToFile("resultvalue=" + resultvalue, true);
                                                    //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, InputValue, ResultValue);
                                                    FunctionsTable.AddToTranslationCacheIfValid(thDataWork, InputValue, ResultValue);
                                                }
                                            }
                                            else
                                            {
                                                //resultvalue = inputvalue.Replace(extractedvalue, cachedvalue);
                                                ResultValue = PasteTranslationBackIfExtracted(CachedExtractedValue, InputValue, ExtractedValue);
                                            }


                                        }
                                    }                                                                                            //string onlinetranslation = DEEPL.Translate(origvalue);//из исходников ESPTranslator 

                                    //LogToFile("Result onlinetranslation=" + onlinetranslation, true);
                                    //проверка наличия результата и вторичная проверка пустого значения поля для перевода перед записью
                                    //if (!string.IsNullOrEmpty(onlinetranslation) && (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString())))
                                    if (ResultValue.Length == 0)
                                    {
                                    }
                                    else
                                    {
                                        if ((row[OrigColIndex + 1] + string.Empty).Length == 0)
                                        {
                                            //LogToFile("THTranslationCache Rows count="+ THTranslationCache.Tables[0].Rows.Count);

                                            //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, InputValue, ResultValue);
                                            FunctionsTable.AddToTranslationCacheIfValid(thDataWork, InputValue, ResultValue);
                                            //THTranslationCacheAdd(inputvalue, onlinetranslation);                                    

                                            //запись перевода
                                            //THFileElementsDataGridView[cind + 1, rind].Value = onlinetranslation;
                                            row[OrigColIndex + 1] = ResultValue;
                                            //THAutoSetValueForSameCells(t, rowindex, cind);
                                        }
                                    }
                                }
                                thDataWork.Main.THAutoSetSameTranslationForSimular(t, CurrentRowIndex, OrigColIndex);
                            }
                        }
                    }
                    //FunctionsDBFile.WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);
                    thDataWork.OnlineTranslationCache.WriteCache();
                }
            }
            catch (System.ArgumentNullException)
            {
                //LogToFile("Error: "+ex,true);
            }
            thDataWork.Main.IsTranslating = false;
            CacheUnloadWhenNeed(thDataWork);
            thDataWork.Main.ProgressInfo(false);
        }

        private string TranslateMultilineValue(string[] InputLines/*, DataSet cacheDS*/)
        {
            //LogToFile("0 Started multiline array handling");
            //string ResultValue = string.Empty;
            int InputLinesLength = InputLines.Length;
            StringBuilder ResultValue = new StringBuilder(InputLinesLength);
            string OriginalLine;
            for (int a = 0; a < InputLinesLength; a++)
            {
                OriginalLine = InputLines[a];//.Replace("\r", string.Empty);//replace было нужно когда делил строку по знаку \n и оставался \r
                //LogToFile("1 inputlinevalue="+ inputlinevalue);
                if (OriginalLine.Length == 0)
                {
                    ResultValue.Append(OriginalLine);
                    //LogToFile("1.1 inputlinevalue is empty. resultvalue="+ resultvalue);
                }
                else
                {
                    string ExtractedOriginal = thDataWork.Main.THExtractTextForTranslation(OriginalLine);
                    //LogToFile("2 extractedvalue=" + extractedvalue);
                    string Result;
                    if (ExtractedOriginal.Length == 0 || ExtractedOriginal == OriginalLine)
                    {
                        //Result = TranslatorsFunctions.ReturnTranslatedOrCache(cacheDS, OriginalLine);
                        Result = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(OriginalLine);
                        //FunctionsTable.AddToTranslationCacheIfValid(cacheDS, OriginalLine, Result);
                        if (Result.Length == 0 || Result == OriginalLine)
                        {
                            Result = GoogleAPI.Translate(OriginalLine);
                        }
                        FunctionsTable.AddToTranslationCacheIfValid(thDataWork, OriginalLine, Result);
                    }
                    else
                    {
                        string ExtractedTranslation = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(ExtractedOriginal);
                        if (ExtractedTranslation.Length == 0 || ExtractedTranslation == ExtractedOriginal)
                        {
                            ExtractedTranslation = GoogleAPI.Translate(ExtractedOriginal);
                        }
                        Result = PasteTranslationBackIfExtracted(
                            //TranslatorsFunctions.ReturnTranslatedOrCache(cacheDS, ExtractedOriginal),
                            ExtractedTranslation,
                            OriginalLine,
                            ExtractedOriginal
                            );
                        //FunctionsTable.AddToTranslationCacheIfValid(cacheDS, ExtractedOriginal, Result);
                        FunctionsTable.AddToTranslationCacheIfValid(thDataWork, ExtractedOriginal, ExtractedTranslation);
                    }
                    ResultValue.Append(Result);

                }
                //добавление новой строки если последняя строка не последняя в массиве строк
                if (a + 1 < InputLinesLength)
                {
                    ResultValue.Append(Environment.NewLine);
                }
                //LogToFile("5 resultvalue=" + resultvalue);
            }
            //LogToFile(string.Empty,true);
            return ResultValue.ToString();
        }

        private static string PasteTranslationBackIfExtracted(string Translation, string Original, string Extracted)
        {
            if (Translation.Length == 0 || Original.Length == 0 || Extracted.Length == 0 || Equals(Original, Extracted))
            {
                return Translation;
            }
            //переделано через удаление и вставку строки, чтобы точно вставлялась нужная
            //строка в нужное место и с рассчетом на будущее, когда возможно строки будут выдираться из исходной
            //, а потом вставляться обратно
            int IndexOfTheString = Original.IndexOf(Extracted);
            if (IndexOfTheString > -1)
            {
                return Original.Remove(IndexOfTheString, Extracted.Length).Insert(IndexOfTheString, Translation);
            }
            else
            {
                return Translation;
            }
        }

        private static void CacheInitWhenNeed(THDataWork thDataWork)
        {
            //if (!Properties.Settings.Default.IsTranslationCacheEnabled)
            //    return;

            Properties.Settings.Default.OnlineTranslationCacheUseCount++;
            if (thDataWork.OnlineTranslationCache == null)
            {
                thDataWork.OnlineTranslationCache = new FunctionsOnlineCache();
                thDataWork.OnlineTranslationCache.ReadCache();
            }
        }

        private static void CacheUnloadWhenNeed(THDataWork thDataWork)
        {
            Properties.Settings.Default.OnlineTranslationCacheUseCount--;
            if (Properties.Settings.Default.OnlineTranslationCacheUseCount == 0)
            {
                if (thDataWork.OnlineTranslationCache != null)
                {
                    thDataWork.OnlineTranslationCache = null;
                }
            }
        }

        /// <summary>
        /// Translate rows by big blocks with selected method
        /// </summary>
        /// <param name="OrigColIndex">Original column index</param>
        /// <param name="TableIndex">Start/Current table index</param>
        /// <param name="SelectedIndexes">Selected rows indexes array</param>
        /// <param name="Method">Selected method: a- all tables; t - selected table; s - selected rows</param>
        internal void THOnlineTranslateByBigBlocks2(int OrigColIndex = 0, int TableIndex = 0, int[] SelectedIndexes = null, string Method = "a"/*ats*/)
        {
            try
            {
                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem.Visible = true));

                //using (DataSet THTranslationCache = new DataSet())
                {
                    //Init translation cache
                    //FunctionsTable.TranslationCacheInit(THTranslationCache);
                    CacheInitWhenNeed(thDataWork);

                    int maxchars = 1000; //большие значения ломаю ответ сервера, например отсутствует или ломается разделитель при значении 1000, потом надо будет подстроить идеальный максимум
                    int CurrentCharsCount = 0;
                    string InputOriginalLine;

                    using (DataTable InputLines = new DataTable())
                    {
                        using (DataTable InputLinesInfo = new DataTable())
                        {
                            InputLines.Columns.Add("Original");

                            InputLinesInfo.Columns.Add("Original");
                            InputLinesInfo.Columns.Add("Translation");
                            InputLinesInfo.Columns.Add("Table");
                            InputLinesInfo.Columns.Add("Row");
                            InputLinesInfo.Columns.Add("IsLastLineInCell");
                            InputLinesInfo.Columns.Add("IsExtracted");

                            //количество таблиц, строк и индекс троки для использования в переборе строк и таблиц
                            int TableMaxIndex;
                            int RowsCountInTable;
                            int CurrentRowIndex;
                            TableMaxIndex = (Method == "a") ? thDataWork.THFilesElementsDataset.Tables.Count : TableMaxIndex = TableIndex + 1;

                            //int tcount = thDataWork.THFilesElementsDataset.Tables.Count;
                            //for (int t = 0; t < tcount; t++)
                            for (int t = TableIndex; t < TableMaxIndex; t++)
                            {
                                var Table = thDataWork.THFilesElementsDataset.Tables[t];

                                if (FunctionsTable.IsTableRowsCompleted(Table))
                                {
                                    continue;
                                }

                                thDataWork.Main.ProgressInfo(true, T._("getting translation: ") + t+"/"+ TableMaxIndex);

                                RowsCountInTable = (Method == "a" || Method == "t") ? Table.Rows.Count : SelectedIndexes.Length;

                                //int rcount = Table.Rows.Count;
                                //for (int r = 0; r < rcount; r++)
                                for (int r = 0; r < RowsCountInTable; r++)
                                {
                                    if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                                    {
                                        Thread.CurrentThread.Abort();
                                        return;
                                    }
                                    else if (thDataWork.Main.InteruptTranslation)
                                    {
                                        thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem.Visible = false));
                                        CacheUnloadWhenNeed(thDataWork);
                                        thDataWork.Main.ProgressInfo(false);
                                        Thread.CurrentThread.Abort();
                                        return;
                                    }

                                    string progressinfo;
                                    if (Method == "s")
                                    {
                                        //progressinfo = T._("getting translation: ") + (r + 1) + "/" + RowsCountInTable;
                                        //индекс = первому из заданного списка выбранных индексов
                                        CurrentRowIndex = SelectedIndexes[r];
                                    }
                                    else if (Method == "t")
                                    {
                                        //progressinfo = T._("getting translation: ") + (r + 1) + "/" + RowsCountInTable;
                                        //индекс с нуля и до последней строки
                                        CurrentRowIndex = r;
                                    }
                                    else
                                    {
                                        //progressinfo = T._("getting translation: ") + t + "/" + TableMaxIndex + "::" + (r + 1) + "/" + RowsCountInTable;
                                        //индекс с нуля и до последней строки
                                        CurrentRowIndex = r;
                                    }

                                    var Row = Table.Rows[CurrentRowIndex];
                                    //thDataWork.Main.ProgressInfo(true, progressinfo);
                                    //thDataWork.Main.ProgressInfo(true, T._("translating") + ": " + t + "/" + tcount + " (" + r + "/" + rcount + ")");

                                    InputOriginalLine = Row[OrigColIndex] as string;
                                    if (string.IsNullOrWhiteSpace(InputOriginalLine))
                                    {
                                        continue;
                                    }

                                    bool TranslateIt = false;
                                    TranslateIt = (CurrentCharsCount + InputOriginalLine.Length) >= maxchars || (t == TableMaxIndex/*tcount*/ - 1 && r == RowsCountInTable/*rcount*/ - 1);

                                    var Cell = Row[OrigColIndex + 1];
                                    if (Cell == null || string.IsNullOrEmpty(Cell as string))
                                    {
                                        //string InputOriginalLineFromCache = FunctionsTable.TranslationCacheFind(THTranslationCache, InputOriginalLine);//поиск оригинала в кеше
                                        string InputOriginalLineFromCache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(InputOriginalLine);


                                        if (InputOriginalLineFromCache.Length > 0)
                                        {
                                            thDataWork.THFilesElementsDataset.Tables[t].Rows[CurrentRowIndex][OrigColIndex + 1] = InputOriginalLineFromCache;
                                            continue;
                                        }
                                        else
                                        {
                                            CurrentCharsCount += InputOriginalLine.Length;

                                            string extractedvalue;
                                            string cache;

                                            int LinesCount = InputOriginalLine.GetLinesCount();
                                            int currentLineNumber = 0;
                                            int IsExtracted = 0;
                                            foreach (var linevalue in InputOriginalLine.SplitToLines())
                                            {
                                                currentLineNumber++;
                                                //string linevalue = Lines[s];

                                                string Translation = string.Empty;
                                                //cache = FunctionsTable.TranslationCacheFind(THTranslationCache, linevalue);//поиск подстроки в кеше
                                                cache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(linevalue);//поиск подстроки в кеше
                                                if (cache.Length > 0)
                                                {
                                                    Translation = cache;//если нашло в кеше, присвоить как перевод
                                                }
                                                else
                                                {
                                                    if (linevalue.Length > 0)
                                                    {
                                                        extractedvalue = thDataWork.Main.THExtractTextForTranslation(linevalue);//извлечение подстроки

                                                        // только если извлеченное значение отличается от оригинальной строки
                                                        //cache = extractedvalue == linevalue ? string.Empty : FunctionsTable.TranslationCacheFind(THTranslationCache, extractedvalue);//поиск извлеченной подстроки в кеше
                                                        cache = extractedvalue == linevalue ? string.Empty : thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(extractedvalue);//поиск извлеченной подстроки в кеше
                                                        if (cache.Length > 0)
                                                        {
                                                            Translation = PasteTranslationBackIfExtracted(cache, linevalue, extractedvalue);
                                                        }
                                                        else
                                                        {
                                                            IsExtracted = extractedvalue.Length == 0 ? 0 : 1;
                                                            InputLines.Rows.Add(extractedvalue.Length == 0 ? linevalue : extractedvalue);
                                                        }
                                                    }
                                                }
                                                //original, translation(when exists in cache), table number, row number
                                                InputLinesInfo.Rows.Add(linevalue, Translation, t, CurrentRowIndex, (currentLineNumber == LinesCount ? 1 : 0), IsExtracted);
                                            }

                                            {
                                                //old code
                                                //string[] Lines = InputOriginalLine.Split(new string[2] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                                                //int LinesCount = Lines.Length;
                                                //if (LinesCount > 1) //если строк больше одной
                                                //{
                                                //    for (int s = 0; s < LinesCount; s++) //добавить все непустые строки по отдельности, пустые добавить в Info
                                                //    {
                                                //        string linevalue = Lines[s];

                                                //        string Translation = string.Empty;
                                                //        //cache = FunctionsTable.TranslationCacheFind(THTranslationCache, linevalue);//поиск подстроки в кеше
                                                //        cache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(linevalue);//поиск подстроки в кеше
                                                //        if (cache.Length > 0)
                                                //        {
                                                //            Translation = cache;
                                                //        }
                                                //        else
                                                //        {
                                                //            if (linevalue.Length > 0)
                                                //            {
                                                //                extractedvalue = thDataWork.Main.THExtractTextForTranslation(linevalue);//извлечение подстроки

                                                //                // только если извлеченное значение отличается от оригинальной строки
                                                //                //cache = extractedvalue == linevalue ? string.Empty : FunctionsTable.TranslationCacheFind(THTranslationCache, extractedvalue);//поиск извлеченной подстроки в кеше
                                                //                cache = extractedvalue == linevalue ? string.Empty : thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(extractedvalue);//поиск извлеченной подстроки в кеше
                                                //                if (cache.Length > 0)
                                                //                {
                                                //                    Translation = PasteTranslationBackIfExtracted(cache, linevalue, extractedvalue);
                                                //                }
                                                //                else
                                                //                {
                                                //                    InputLines.Rows.Add(extractedvalue.Length == 0 ? linevalue : extractedvalue);
                                                //                }
                                                //            }
                                                //        }
                                                //        InputLinesInfo.Rows.Add(linevalue, Translation, t, CurrentRowIndex);
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    //cache = FunctionsTable.TranslationCacheFind(THTranslationCache, InputOriginalLine);
                                                //    cache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(InputOriginalLine);
                                                //    if (cache.Length > 0)
                                                //    {
                                                //        thDataWork.THFilesElementsDataset.Tables[t].Rows[CurrentRowIndex][OrigColIndex + 1] = cache;
                                                //    }
                                                //    else
                                                //    {
                                                //        //с одной строкой просто добавить её в таблицы
                                                //        extractedvalue = thDataWork.Main.THExtractTextForTranslation(InputOriginalLine);
                                                //        InputLines.Rows.Add(extractedvalue.Length == 0 ? InputOriginalLine : extractedvalue);
                                                //        InputLinesInfo.Rows.Add(InputOriginalLine, string.Empty, t, CurrentRowIndex);
                                                //    }

                                                //}
                                            }

                                        }
                                    }

                                    if (TranslateIt)
                                    {
                                        CurrentCharsCount = 0;

                                        TranslateItNow(InputLines, InputLinesInfo);
                                    }
                                }
                            }

                            //перевести, если после прохода по всем таблицам добавленные строки так и не были переведены
                            if (InputLines.Rows.Count>0)
                            {
                                TranslateItNow(InputLines, InputLinesInfo);
                            }
                        }
                    }

                    //FunctionsDBFile.WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);
                    thDataWork.OnlineTranslationCache.WriteCache();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            CacheUnloadWhenNeed(thDataWork);

            thDataWork.Main.ProgressInfo(false);
        }

        private void TranslateItNow(DataTable InputLines, DataTable InputLinesInfo)
        {
            if (InputLines.Rows.Count > 0)
            {
                TranslateLinesAndSetTranslation(InputLines, InputLinesInfo/*, THTranslationCache*/);
            }
            else if (InputLinesInfo.Rows.Count > 0)
            {
                int PreviousTableIndex = -1;
                int PreviousRowIndex = -1;
                int NewTableIndex;
                int NewRowIndex;
                int rowscount = InputLinesInfo.Rows.Count;
                StringBuilder ResultValue = new StringBuilder(rowscount);
                for (int i = 0; i < rowscount; i++)
                {
                    var rowInfo = InputLinesInfo.Rows[i];
                    NewTableIndex = int.Parse(rowInfo[2] as string, CultureInfo.GetCultureInfo("en-US"));
                    NewRowIndex = int.Parse(rowInfo[3] as string, CultureInfo.GetCultureInfo("en-US"));
                    if (string.IsNullOrEmpty(rowInfo[0] as string))
                    {
                        ResultValue.Append(Environment.NewLine);
                    }
                    else if (!string.IsNullOrEmpty(rowInfo[1] as string))
                    {
                        ResultValue.Append(rowInfo[1]);
                    }
                    if (NewRowIndex == PreviousRowIndex && i < rowscount - 1)
                    {
                        ResultValue.Append(Environment.NewLine);
                    }
                    else
                    {
                        SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue/*, THTranslationCache*/);
                    }
                    PreviousTableIndex = NewTableIndex;
                    PreviousRowIndex = NewRowIndex;
                }
            }
            //FunctionsDBFile.WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);//промежуточная запись кеша
            thDataWork.OnlineTranslationCache.WriteCache();//промежуточная запись кеша

            InputLines.Rows.Clear();
            InputLinesInfo.Rows.Clear();
        }

        private void TranslateLinesAndSetTranslation(DataTable InputLines, DataTable InputLinesInfo/*, DataSet THTranslationCache*/)
        {
            //https://www.codeproject.com/Questions/722877/DataTable-to-string-array
            //add table rows to string array
            string[] OriginalLines = InputLines.Rows.OfType<DataRow>().Select(row => row[0].ToString()).ToArray();

            //сброс кеша в GoogleAPI
            GoogleAPI.ResetCache();

            //send string array to translation for multiline
            string[] TranslatedLines = GoogleAPI.TranslateMultiple(OriginalLines);

            //int infoCount = InputLinesInfo.Rows.Count;
            //int TranslatedCount = TranslatedLines.Length-1; // -1 - отсекание последнего пустого элемента

            if (TranslatedLines != null && TranslatedLines.Length > 0 && InputLines.Rows.Count == TranslatedLines.Length)
            {
                StringBuilder ResultValue = new StringBuilder();
                int TranslatedLinesIndex = 0;
                for (int InfoIndex = 0; InfoIndex < InputLinesInfo.Rows.Count; InfoIndex++)
                {
                    var InfoRow = InputLinesInfo.Rows[InfoIndex];

                    string newLine;
                    if (InfoRow[4] as string == "0")
                    {
                        newLine = Environment.NewLine;
                    }
                    else
                    {
                        newLine = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(InfoRow[1] + string.Empty))//перевод найден в кеше
                    {
                        ResultValue.Append(InfoRow[1] + string.Empty);
                    }
                    else if (string.IsNullOrEmpty(InfoRow[0] + string.Empty))//пустая строка в оригинале
                    {
                    }
                    else
                    {
                        if (Equals(InputLines.Rows[TranslatedLinesIndex][0], InfoRow[0]))
                        {
                            ResultValue.Append(TranslatedLines[TranslatedLinesIndex]);
                        }
                        else
                        {
                            ResultValue.Append(
                                PasteTranslationBackIfExtracted
                            (
                                TranslatedLines[TranslatedLinesIndex]
                            ,
                                InfoRow[0] as string
                            ,
                                InputLines.Rows[TranslatedLinesIndex][0] as string
                            ));
                        }
                        TranslatedLinesIndex++;
                    }
                    ResultValue.Append(newLine);

                    if (newLine.Length == 0)
                    {
                        SetTranslationResultToCellIfEmpty(
                            int.Parse(InfoRow[2] as string, CultureInfo.GetCultureInfo("en-US"))
                            ,
                            int.Parse(InfoRow[3] as string, CultureInfo.GetCultureInfo("en-US"))
                            ,
                            ResultValue
                            //,
                            //THTranslationCache
                            );
                        ResultValue.Clear();
                    }
                }
            }

            {
                //if (TranslatedLines != null && TranslatedLines.Length > 0)//check if returned lines from translator is valid
                //{
                //    StringBuilder ResultValue = new StringBuilder();

                //    int PreviousTableIndex = -1;
                //    int PreviousRowIndex = -1;
                //    int i2 = 0;

                //    int TableIndex = 0;
                //    int RowIndex = 0;

                //    int TranslatedLinesLength = TranslatedLines.Length;
                //    for (int i = 0; i < TranslatedLinesLength; i++)//iteration through translated lines
                //    {
                //        string TranslatedLine = TranslatedLines[i];//set translated line to string
                //        var InfoRow = InputLinesInfo.Rows[i2];//set info to datarow
                //        {
                //            {
                //                //string fdsfsdf = TranslatedLines[i];
                //                //string dfsgsdg1 = fdsfsdf;
                //                //--------------------------------
                //                //Блок считывания индеков таблицы и строки
                //                //добавление переноса, если строка той же ячейки, либо запись результата, если уже новая ячейка
                //                //string prelastvalue="";
                //                //string lastvalue = "";
                //                //try
                //                //{
                //                //    int inforowscount = InputLinesInfo.Rows.Count;
                //                //    if (inforowscount > 0)
                //                //    {
                //                //        if (inforowscount > 1)
                //                //        {
                //                //            prelastvalue = InputLinesInfo.Rows[InputLines.Rows.Count - 2][1] + string.Empty;

                //                //        }
                //                //        lastvalue = InputLinesInfo.Rows[InputLinesInfo.Rows.Count - 1][1] + string.Empty;

                //                //    }
                //            }
                //        }
                //        //set table index and row index from inforow to integers
                //        TableIndex = int.Parse(InfoRow[2] as string, CultureInfo.GetCultureInfo("en-US"));
                //        RowIndex = int.Parse(InfoRow[3] as string, CultureInfo.GetCultureInfo("en-US"));
                //        {
                //            {
                //                //}
                //                //catch(Exception ex)
                //                //{
                //                //    MessageBox.Show(ex.ToString());
                //                //}
                //                //string asdasd = prelastvalue;
                //                //string adsasdaaa = lastvalue;
                //            }
                //        }

                //        if (TableIndex == PreviousTableIndex && (RowIndex == PreviousRowIndex /*|| (i2 + 1 < InputLinesInfo.Rows.Count && TableIndex== int.Parse(InputLinesInfo.Rows[i2 + 1][2] as string)  && RowIndex == int.Parse(InputLinesInfo.Rows[i2 + 1][3] as string))*/))
                //        {
                //            ResultValue.Append(Environment.NewLine);
                //        }
                //        else
                //        {
                //            {
                //                //string originalValue = InfoRow[0] as string;
                //                //string extractedValue = InputLines.Rows[i][0] as string;
                //                //SetTranslationResultToCellIfEmpty(
                //                //    PreviousTableIndex == -1
                //                //    ?
                //                //    TableIndex
                //                //    : 
                //                //    PreviousTableIndex
                //                //    , 
                //                //    PreviousRowIndex == -1
                //                //    ? 
                //                //    RowIndex
                //                //    : 
                //                //    PreviousRowIndex
                //                //    , 
                //                //    PreviousRowIndex == -1 && ResultValue.ToString().Length == 0
                //                //    ? 
                //                //    ResultValue.Append(PasteTranslationBackIfExtracted(TranslatedLine, originalValue, extractedValue))
                //                //    : 
                //                //    ResultValue
                //                //    ,
                //                //    THTranslationCache
                //                //    );
                //            }

                //            SetTranslationResultToCellIfEmpty(
                //                PreviousTableIndex
                //                ,
                //                PreviousRowIndex
                //                ,
                //                ResultValue
                //                //,
                //                //THTranslationCache
                //                );
                //        }

                //        //--------------------------------
                //        //Блок записи пустой строки или кеша из информации, если они были и увеличение i2 на 1

                //        bool WritedFromInfo = true;
                //        while (WritedFromInfo)
                //        {
                //            string InfoRowOriginal = InfoRow[0] as string;
                //            string InfoRowCachedTranslation = InfoRow[1] as string;
                //            WritedFromInfo = false;
                //            if (string.IsNullOrEmpty(InfoRowOriginal))
                //            {
                //                //ResultValue.Append(string.Empty); //закоментировано для оптимизации, тот же эффект добавление пустой строки
                //                WritedFromInfo = true;
                //                i2++;
                //            }
                //            else if (!string.IsNullOrEmpty(InfoRowCachedTranslation))
                //            {
                //                ResultValue.Append(InfoRowCachedTranslation);
                //                WritedFromInfo = true;
                //                i2++;
                //            }

                //            if (WritedFromInfo)//контроль, когда было записано из Info , предотвращает запись лишнего переноса
                //            {
                //                PreviousRowIndex = RowIndex;
                //                PreviousTableIndex = TableIndex;
                //                //--------------------------------
                //                //Блок считывания индеков таблицы и строки
                //                //добавление переноса, если строка той же ячейки, либо запись результата, если уже новая ячейка
                //                InfoRow = InputLinesInfo.Rows[i2];//еще раз переприсваивание, т.к. i2 поменялось
                //                TableIndex = int.Parse(InfoRow[2] as string, CultureInfo.CurrentCulture);
                //                RowIndex = int.Parse(InfoRow[3] as string, CultureInfo.CurrentCulture);

                //                if (RowIndex == PreviousRowIndex && TableIndex == PreviousTableIndex)
                //                {
                //                    ResultValue.AppendLine();
                //                }
                //                else
                //                {
                //                    SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue/*, THTranslationCache*/);
                //                }

                //                //--------------------------------
                //            }
                //        }

                //        string originalLineValue = InfoRow[0] as string;
                //        string extractedLineValue = InputLines.Rows[i][0] as string;
                //        ResultValue.Append(PasteTranslationBackIfExtracted(TranslatedLine, originalLineValue, extractedLineValue));

                //        //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, (originalLineValue.Length == extractedLineValue.Length && originalLineValue == extractedLineValue) ? originalLineValue : extractedLineValue, TranslatedLine);
                //        FunctionsTable.AddToTranslationCacheIfValid(thDataWork, (originalLineValue.Length == extractedLineValue.Length && originalLineValue == extractedLineValue) ? originalLineValue : extractedLineValue, TranslatedLine);

                //        PreviousRowIndex = RowIndex;
                //        PreviousTableIndex = TableIndex;
                //        i2++;
                //    }

                //    //--------------------------------ВТОРОЙ БЛОК
                //    //-когда строки в переводчике закончились а InputLinesInfo.Rows еще нет
                //    int InputLinesInfoRowsCount = InputLinesInfo.Rows.Count;
                //    while (i2 < InputLinesInfoRowsCount)
                //    {
                //        var InfoRow = InputLinesInfo.Rows[i2];

                //        TableIndex = int.Parse(InfoRow[2] as string, CultureInfo.CurrentCulture);
                //        RowIndex = int.Parse(InfoRow[3] as string, CultureInfo.CurrentCulture);

                //        if (TableIndex == PreviousTableIndex && RowIndex == PreviousRowIndex)
                //        {
                //            ResultValue.AppendLine();
                //        }
                //        else
                //        {
                //            SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue/*, THTranslationCache*/);
                //        }

                //        string InfoOriginalLineValue = InfoRow[0] as string;
                //        string InfoCacheValue = InfoRow[1] as string;
                //        if (string.IsNullOrEmpty(InfoOriginalLineValue))
                //        {
                //            //ResultValue.Append(string.Empty); //закоментировано для оптимизации, тот же эффект добавление пустой строки                      
                //        }
                //        else if (!string.IsNullOrEmpty(InfoCacheValue))
                //        {
                //            ResultValue.Append(InfoCacheValue);
                //        }

                //        PreviousRowIndex = RowIndex;
                //        PreviousTableIndex = TableIndex;
                //        i2++;
                //    }
                //    SetTranslationResultToCellIfEmpty(PreviousTableIndex, PreviousRowIndex, ResultValue/*, THTranslationCache*/);
                //}
            }

        }

        private void SetTranslationResultToCellIfEmpty(int PreviousTableIndex, int PreviousRowIndex, StringBuilder ResultValue/*, DataSet THTranslationCache*/)
        {
            if (ResultValue.Length > 0 && PreviousTableIndex > -1 && PreviousRowIndex > -1)
            {
                string s; //иногда значения без перевода и равны оригиналу, но отдельным переводом выбранной ячейки получается нормально
                var Row = thDataWork.THFilesElementsDataset.Tables[PreviousTableIndex].Rows[PreviousRowIndex];
                var Cell = Row[0];
                if (Equals(Cell, ResultValue))
                {
                    s = GoogleAPI.Translate(Cell as string);
                }
                else
                {
                    s = ResultValue.ToString();
                }

                var TranslationCell = Row[1];
                if (TranslationCell == null || string.IsNullOrEmpty(TranslationCell as string))
                {
                    thDataWork.THFilesElementsDataset.Tables[PreviousTableIndex].Rows[PreviousRowIndex][1] = s;

                    //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, Cell as string, s);
                    FunctionsTable.AddToTranslationCacheIfValid(thDataWork, Cell as string, s);

                    //закоментировано для повышения производительности
                    //THAutoSetSameTranslationForSimular(PreviousTableIndex, PreviousRowIndex, 0);
                }
                ResultValue.Clear();
            }
        }
    }
}
