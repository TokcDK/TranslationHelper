using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions
{
    class FunctionsOnlineTranslation
    {
        readonly THDataWork thDataWork;
        readonly GoogleAPI Translator;

        public FunctionsOnlineTranslation(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
            Translator = new GoogleAPI();
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
                    Translator.ResetCache();

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

                                    if (InputValue.IsMultiline())
                                    {
                                        ResultValue = TranslateMultilineValue(InputValue.SplitToLines().ToArray()/*, THTranslationCache*/);
                                    }
                                    else
                                    {
                                        string ExtractedValue = thDataWork.Main.THExtractTextForTranslation(InputValue);
                                        //LogToFile("extractedvalue="+ extractedvalue,true);
                                        if (ExtractedValue.Length == 0 || ExtractedValue == InputValue)
                                        {
                                            ResultValue = Translator.Translate(InputValue);

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
                                                string OnlineValue = Translator.Translate(ExtractedValue);//из исходников ESPTranslator 

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
                            Result = Translator.Translate(OriginalLine);
                        }
                        FunctionsTable.AddToTranslationCacheIfValid(thDataWork, OriginalLine, Result);
                    }
                    else
                    {
                        string ExtractedTranslation = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(ExtractedOriginal);
                        if (ExtractedTranslation.Length == 0 || ExtractedTranslation == ExtractedOriginal)
                        {
                            ExtractedTranslation = Translator.Translate(ExtractedOriginal);
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

        private string PasteTranslationBackIfExtracted(string Translation, string Original, string Extracted)
        {
            if (Translation.Length == 0 || Original.Length == 0 || Extracted.Length == 0 || Equals(Original, Extracted))
            {
                return Properties.Settings.Default.ApplyFixesOnTranslation ?
                    FunctionsStringFixes.ApplyHardFixes(Original, Translation.THFixCells(thDataWork)) : Translation;
            }
            //переделано через удаление и вставку строки, чтобы точно вставлялась нужная
            //строка в нужное место и с рассчетом на будущее, когда возможно строки будут выдираться из исходной
            //, а потом вставляться обратно
            int IndexOfTheString = Original.IndexOf(Extracted);
            if (IndexOfTheString > -1)
            {
                return Original
                    .Remove(IndexOfTheString, Extracted.Length)
                    .Insert(IndexOfTheString,
                    Properties.Settings.Default.ApplyFixesOnTranslation ?
                    FunctionsStringFixes.ApplyHardFixes(Original, Translation.THFixCells(thDataWork)) : Translation
                    );
            }
            else
            {
                return Properties.Settings.Default.ApplyFixesOnTranslation ?
                    FunctionsStringFixes.ApplyHardFixes(Original, Translation.THFixCells(thDataWork)) : Translation;
            }
        }

        private static string PasteTranslationBackIfExtractedSplit(string Translation, string Original, string Extracted)
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

                    List<string> InputLines = new List<string>();
                    List<InputLinesInfoData> InputLinesInfo = new List<InputLinesInfoData>();

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

                        Task.Run(() => thDataWork.Main.ProgressInfo(true, T._("getting translation") + t + "/" + TableMaxIndex + ": " + Table.TableName + " ")).ConfigureAwait(false);

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

                            //string progressinfo;
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
                            if (string.IsNullOrWhiteSpace(InputOriginalLine) || InputOriginalLine.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
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
                                    bool IsExtracted = false;
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
                                                if (linevalue.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                {
                                                    Translation = linevalue;//если большинством ромаджи или прочее
                                                }
                                                else
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
                                                        if (extractedvalue.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                        {
                                                            Translation = linevalue;//если извлеченное значение большинством ромаджи или прочее
                                                        }
                                                        else
                                                        {
                                                            IsExtracted = extractedvalue.Length > 0 && extractedvalue != linevalue;
                                                            InputLines.Add(extractedvalue.Length == 0 ? linevalue : extractedvalue);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //original, translation(when exists in cache), table number, row number
                                        InputLinesInfo.Add(new InputLinesInfoData(linevalue, Translation, t, CurrentRowIndex, currentLineNumber == LinesCount, IsExtracted));
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
                    if (InputLines.Count > 0)
                    {
                        TranslateItNow(InputLines, InputLinesInfo);
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

        /// <summary>
        /// Translate rows by big blocks with selected method
        /// Variant with using multiextracted value
        /// Work in progress
        /// </summary>
        /// <param name="OrigColIndex">Original column index</param>
        /// <param name="TableIndex">Start/Current table index</param>
        /// <param name="SelectedIndexes">Selected rows indexes array</param>
        /// <param name="Method">Selected method: a- all tables; t - selected table; s - selected rows</param>
        internal void THOnlineTranslateByBigBlocks2NewExtract(int OrigColIndex = 0, int TableIndex = 0, int[] SelectedIndexes = null, string Method = "a"/*ats*/)
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

                    List<string> InputLines = new List<string>();
                    List<InputLinesInfoDataMultiExtracted> InputLinesInfo = new List<InputLinesInfoDataMultiExtracted>();

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

                        Task.Run(() => thDataWork.Main.ProgressInfo(true, T._("getting translation") + t + "/" + TableMaxIndex + ": " + Table.TableName + " ")).ConfigureAwait(false);

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

                            //string progressinfo;
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
                            if (string.IsNullOrWhiteSpace(InputOriginalLine) || InputOriginalLine.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
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

                                    string[] extractedvalue;
                                    string cache;

                                    int LinesCount = InputOriginalLine.GetLinesCount();
                                    int currentLineNumber = 0;
                                    bool IsExtracted = false;
                                    foreach (var linevalue in InputOriginalLine.SplitToLines())
                                    {
                                        currentLineNumber++;
                                        //string linevalue = Lines[s];

                                        string Translation = string.Empty;
                                        List<string> Translations = new List<string>();

                                        //cache = FunctionsTable.TranslationCacheFind(THTranslationCache, linevalue);//поиск подстроки в кеше
                                        cache = thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(linevalue);//поиск подстроки в кеше
                                        if (cache.Length > 0)
                                        {
                                            Translation = cache;//если нашло в кеше, присвоить как перевод
                                            Translations.Add(Translation);
                                        }
                                        else
                                        {
                                            if (linevalue.Length > 0)
                                            {
                                                if (linevalue.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                {
                                                    Translation = linevalue;//если большинством ромаджи или прочее
                                                    Translations.Add(Translation);
                                                }
                                                else
                                                {
                                                    extractedvalue = thDataWork.Main.THExtractTextForTranslationSplit(linevalue);//извлечение подстроки
                                                    if (IsExtracted = IsExtractedOnlyOneAndNotEqualInput(linevalue, extractedvalue))
                                                    {
                                                        // только если извлеченное значение отличается от оригинальной строки
                                                        //cache = extractedvalue == linevalue ? string.Empty : FunctionsTable.TranslationCacheFind(THTranslationCache, extractedvalue);//поиск извлеченной подстроки в кеше
                                                        cache = extractedvalue[2] == linevalue ? string.Empty : thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(extractedvalue[2]);//поиск извлеченной подстроки в кеше
                                                        if (cache.Length > 0)
                                                        {
                                                            Translation = PasteTranslationBackIfExtracted(cache, linevalue, extractedvalue[2]);
                                                        }
                                                        else
                                                        {
                                                            if (extractedvalue[2].IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                            {
                                                                Translation = linevalue;//если извлеченное значение большинством ромаджи или прочее
                                                            }
                                                            else
                                                            {
                                                                //IsExtracted = extractedvalue.Length > 0 && extractedvalue != linevalue;
                                                                InputLines.Add(extractedvalue[2].Length == 0 ? linevalue : extractedvalue[2]);
                                                            }
                                                        }
                                                    }
                                                    else if (IsExtracted = (extractedvalue != null && extractedvalue.Length > 3))
                                                    {
                                                        for (int e = 2; e < extractedvalue.Length; e++)
                                                        {
                                                            cache = extractedvalue[e] == linevalue ? string.Empty : thDataWork.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(extractedvalue[e]);//поиск извлеченной подстроки в кеше
                                                            if (cache.Length > 0)
                                                            {
                                                                Translation = PasteTranslationBackIfExtracted(cache, linevalue, extractedvalue[e]);
                                                                Translations.Add(Translation);
                                                            }
                                                            else
                                                            {
                                                                if (extractedvalue[e].IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                                {
                                                                    Translation = linevalue;//если извлеченное значение большинством ромаджи или прочее
                                                                    Translations.Add(Translation);
                                                                }
                                                                else
                                                                {
                                                                    //IsExtracted = extractedvalue.Length > 0 && extractedvalue != linevalue;
                                                                    InputLines.Add(extractedvalue[2].Length == 0 ? linevalue : extractedvalue[2]);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //original, translation(when exists in cache), table number, row number
                                        InputLinesInfo.Add(new InputLinesInfoDataMultiExtracted(linevalue, Translations, t, CurrentRowIndex, currentLineNumber == LinesCount, IsExtracted));
                                    }
                                }
                            }

                            if (TranslateIt)
                            {
                                CurrentCharsCount = 0;

                                TranslateItNowMulti(InputLines, InputLinesInfo);
                            }
                        }
                    }

                    //перевести, если после прохода по всем таблицам добавленные строки так и не были переведены
                    if (InputLines.Count > 0)
                    {
                        CurrentCharsCount = 0;

                        TranslateItNowMulti(InputLines, InputLinesInfo);
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

        /// <summary>
        /// if single extracted value equals input value
        /// </summary>
        /// <param name="linevalue"></param>
        /// <param name="extractedvalue"></param>
        /// <returns></returns>
        private bool IsExtractedOnlyOneAndNotEqualInput(string linevalue, string[] extractedvalue)
        {
            if (extractedvalue != null && extractedvalue.Length == 3)
            {
                if (extractedvalue[2] != linevalue)
                {
                    return true;
                }
            }
            return false;
        }

        private void TranslateItNowMulti(List<string> InputLines, List<InputLinesInfoDataMultiExtracted> InputLinesInfo)
        {
            if (InputLines.Count > 0)
            {
                TranslateLinesAndSetTranslationMulti(InputLines, InputLinesInfo/*, THTranslationCache*/);
            }
            else if (InputLinesInfo.Count > 0)
            {
                int PreviousTableIndex = -1;
                int PreviousRowIndex = -1;
                int NewTableIndex;
                int NewRowIndex;
                int rowscount = InputLinesInfo.Count;
                StringBuilder ResultValue = new StringBuilder(rowscount);
                for (int i = 0; i < rowscount; i++)
                {
                    var rowInfo = InputLinesInfo[i];
                    string CachedTranslation = BuildExtractedToTranslated(rowInfo.GetCachedTranslation);
                    NewTableIndex = rowInfo.GetTableIndex;
                    NewRowIndex = rowInfo.GetRowIndex;
                    if (string.IsNullOrEmpty(rowInfo.GetOriginal))
                    {
                        ResultValue.Append(Environment.NewLine);
                    }
                    else if (!string.IsNullOrEmpty(CachedTranslation))
                    {
                        ResultValue.Append(CachedTranslation);
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

            InputLines.Clear();
            InputLinesInfo.Clear();
        }

        private string BuildExtractedToTranslated(List<string> getCachedTranslation)
        {
            var result = string.Empty;
            foreach (var str in getCachedTranslation)
            {

            }

            return result;
        }

        private void TranslateItNow(List<string> InputLines, List<InputLinesInfoData> InputLinesInfo)
        {
            if (InputLines.Count > 0)
            {
                TranslateLinesAndSetTranslation(InputLines, InputLinesInfo/*, THTranslationCache*/);
            }
            else if (InputLinesInfo.Count > 0)
            {
                int PreviousTableIndex = -1;
                int PreviousRowIndex = -1;
                int NewTableIndex;
                int NewRowIndex;
                int rowscount = InputLinesInfo.Count;
                StringBuilder ResultValue = new StringBuilder(rowscount);
                for (int i = 0; i < rowscount; i++)
                {
                    var rowInfo = InputLinesInfo[i];
                    NewTableIndex = rowInfo.GetTableIndex;
                    NewRowIndex = rowInfo.GetRowIndex;
                    if (string.IsNullOrEmpty(rowInfo.GetOriginal))
                    {
                        ResultValue.Append(Environment.NewLine);
                    }
                    else if (!string.IsNullOrEmpty(rowInfo.GetCachedTranslation))
                    {
                        ResultValue.Append(rowInfo.GetCachedTranslation);
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

            InputLines.Clear();
            InputLinesInfo.Clear();
        }

        private void TranslateLinesAndSetTranslationMulti(List<string> InputLines, List<InputLinesInfoDataMultiExtracted> InputLinesInfo)
        {
            //https://www.codeproject.com/Questions/722877/DataTable-to-string-array
            //add table rows to string array
            string[] OriginalLines = InputLines.ToArray();

            //сброс кеша в GoogleAPI
            Translator.ResetCache();

            //send string array to translation for multiline
            string[] TranslatedLines = Translator.Translate(OriginalLines);

            //int infoCount = InputLinesInfo.Rows.Count;
            //int TranslatedCount = TranslatedLines.Length-1; // -1 - отсекание последнего пустого элемента

            if (TranslatedLines != null && TranslatedLines.Length > 0 && InputLines.Count == TranslatedLines.Length)
            {
                StringBuilder ResultValue = new StringBuilder();
                int TranslatedLinesIndex = 0;
                InputLinesInfoDataMultiExtracted InfoRow = null;
                for (int InfoIndex = 0; InfoIndex < InputLinesInfo.Count; InfoIndex++)
                {
                    InfoRow = InputLinesInfo[InfoIndex];

                    string newLine;
                    if (!InfoRow.GetIsLastLineInString)
                    {
                        newLine = Environment.NewLine;
                    }
                    else
                    {
                        newLine = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(InfoRow.GetCachedTranslation[0]))//перевод найден в кеше
                    {
                        ResultValue.Append(InfoRow.GetCachedTranslation[0]);
                    }
                    else if (string.IsNullOrEmpty(InfoRow.GetOriginal))//пустая строка в оригинале
                    {
                    }
                    else
                    {
                        if (InputLines[TranslatedLinesIndex] == InfoRow.GetOriginal)
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
                                InfoRow.GetOriginal
                            ,
                                InputLines[TranslatedLinesIndex]
                            ));
                        }
                        TranslatedLinesIndex++;
                    }
                    ResultValue.Append(newLine);

                    if (newLine.Length == 0)
                    {
                        SetTranslationResultToCellIfEmpty(
                            InfoRow.GetTableIndex
                            ,
                            InfoRow.GetRowIndex
                            ,
                            ResultValue
                            );
                        ResultValue.Clear();
                    }
                }
                if (ResultValue.Length > 0 && InfoRow != null)
                {
                    SetTranslationResultToCellIfEmpty(
                        InfoRow.GetTableIndex
                        ,
                        InfoRow.GetRowIndex
                        ,
                        ResultValue
                        );
                    ResultValue.Clear();
                }
            }
        }

        private void TranslateLinesAndSetTranslation(List<string> InputLines, List<InputLinesInfoData> InputLinesInfo)
        {
            //https://www.codeproject.com/Questions/722877/DataTable-to-string-array
            //add table rows to string array
            string[] OriginalLines = InputLines.ToArray();

            //сброс кеша в GoogleAPI
            Translator.ResetCache();

            //send string array to translation for multiline
            string[] TranslatedLines = Translator.Translate(OriginalLines);

            //int infoCount = InputLinesInfo.Rows.Count;
            //int TranslatedCount = TranslatedLines.Length-1; // -1 - отсекание последнего пустого элемента

            if (TranslatedLines != null && TranslatedLines.Length > 0 && InputLines.Count == TranslatedLines.Length)
            {
                StringBuilder ResultValue = new StringBuilder();
                int TranslatedLinesIndex = 0;
                InputLinesInfoData InfoRow = null;
                for (int InfoIndex = 0; InfoIndex < InputLinesInfo.Count; InfoIndex++)
                {
                    InfoRow = InputLinesInfo[InfoIndex];

                    string newLine;
                    if (!InfoRow.GetIsLastLineInString)
                    {
                        newLine = Environment.NewLine;
                    }
                    else
                    {
                        newLine = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(InfoRow.GetCachedTranslation))//перевод найден в кеше
                    {
                        ResultValue.Append(
                            Properties.Settings.Default.ApplyFixesOnTranslation ?
                                FunctionsStringFixes.ApplyHardFixes(TranslatedLines[TranslatedLinesIndex], InfoRow.GetCachedTranslation.THFixCells(thDataWork))
                                : InfoRow.GetCachedTranslation
                            );
                    }
                    else if (string.IsNullOrEmpty(InfoRow.GetOriginal))//пустая строка в оригинале
                    {
                    }
                    else
                    {
                        if (InputLines[TranslatedLinesIndex] == InfoRow.GetOriginal)
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
                                InfoRow.GetOriginal
                            ,
                                InputLines[TranslatedLinesIndex]
                            ));
                        }
                        TranslatedLinesIndex++;
                    }
                    ResultValue.Append(newLine);

                    if (newLine.Length == 0)
                    {
                        SetTranslationResultToCellIfEmpty(
                            InfoRow.GetTableIndex
                            ,
                            InfoRow.GetRowIndex
                            ,
                            ResultValue
                            );
                        ResultValue.Clear();
                    }
                }
                if (ResultValue.Length > 0 && InfoRow != null)
                {
                    SetTranslationResultToCellIfEmpty(
                        InfoRow.GetTableIndex
                        ,
                        InfoRow.GetRowIndex
                        ,
                        ResultValue
                        );
                    ResultValue.Clear();
                }
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
                    s = Translator.Translate(Cell as string);
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

    /// <summary>
    /// for translation by big blocks
    /// </summary>
    internal class InputLinesInfoData
    {
        internal string GetOriginal { get; }
        internal string GetCachedTranslation { get; }
        internal int GetTableIndex { get; }
        internal int GetRowIndex { get; }
        internal bool GetIsLastLineInString { get; }
        internal bool GetIsExtracted { get; }

        internal InputLinesInfoData(string Original, string CachedTranslation, int TableIndex, int RowIndex, bool IsLastLineInString, bool IsExtracted)
        {
            GetOriginal = Original;
            GetCachedTranslation = CachedTranslation;
            GetTableIndex = TableIndex;
            GetRowIndex = RowIndex;
            GetIsLastLineInString = IsLastLineInString;
            GetIsExtracted = IsExtracted;
        }
    }

    /// <summary>
    /// for translation by big blocks for multiextraction
    /// WIP
    /// </summary>
    internal class InputLinesInfoDataMultiExtracted
    {
        internal string GetOriginal { get; }
        internal List<string> GetCachedTranslation { get; }
        internal int GetTableIndex { get; }
        internal int GetRowIndex { get; }
        internal bool GetIsLastLineInString { get; }
        internal bool GetIsExtracted { get; }

        internal InputLinesInfoDataMultiExtracted(string Original, List<string> CachedTranslation, int TableIndex, int RowIndex, bool IsLastLineInString, bool IsExtracted)
        {
            GetOriginal = Original;
            GetCachedTranslation = CachedTranslation;
            GetTableIndex = TableIndex;
            GetRowIndex = RowIndex;
            GetIsLastLineInString = IsLastLineInString;
            GetIsExtracted = IsExtracted;
        }
    }
}
