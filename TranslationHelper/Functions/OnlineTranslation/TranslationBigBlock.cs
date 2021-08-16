using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.OnlineTranslation
{
    class TranslationBigBlock : TranslationBase
    {
        public TranslationBigBlock()
        {
        }

        internal override void Get()
        {
            THOnlineTranslateByBigBlocks2();
        }

        /// <summary>
        /// Translate rows by big blocks with selected method
        /// </summary>
        /// <param name="origColIndex">Original column index</param>
        /// <param name="tableIndex">Start/Current table index</param>
        /// <param name="selectedIndexes">Selected rows indexes array</param>
        /// <param name="method">Selected method: a- all tables; t - selected table; s - selected rows</param>
        internal void THOnlineTranslateByBigBlocks2(int origColIndex = 0, int tableIndex = 0, int[] selectedIndexes = null, string method = "a"/*ats*/)
        {
            try
            {
                if (Properties.Settings.Default.UseAllDBFilesForOnlineTranslationForAll && method == "a")
                {
                    ProjectData.Main.ProgressInfo(true, "Get all databases");
                    FunctionsDbFile.MergeAllDBtoOne();
                }

                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.translationInteruptToolStripMenuItem.Visible = true));

                //using (DataSet THTranslationCache = new DataSet())
                {
                    //Init translation cache
                    //FunctionsTable.TranslationCacheInit(THTranslationCache);
                    CacheInitWhenNeed();

                    int maxchars = 1000; //большие значения ломаю ответ сервера, например отсутствует или ломается разделитель при значении 1000, потом надо будет подстроить идеальный максимум
                    int currentCharsCount = 0;
                    string inputOriginalLine;

                    List<string> inputLines = new List<string>();
                    List<InputLinesInfoData> inputLinesInfo = new List<InputLinesInfoData>();

                    //количество таблиц, строк и индекс троки для использования в переборе строк и таблиц
                    int tableMaxIndex;
                    int rowsCountInTable;
                    int currentRowIndex;
                    tableMaxIndex = (method == "a") ? ProjectData.ThFilesElementsDataset.Tables.Count : tableMaxIndex = tableIndex + 1;

                    //int tcount = ProjectData.THFilesElementsDataset.Tables.Count;
                    //for (int t = 0; t < tcount; t++)
                    for (int t = tableIndex; t < tableMaxIndex; t++)
                    {
                        var table = ProjectData.ThFilesElementsDataset.Tables[t];

                        if (FunctionsTable.IsTableRowsCompleted(table))
                        {
                            continue;
                        }

                        Task.Run(() => ProjectData.Main.ProgressInfo(true, T._("getting translation") + t + "/" + tableMaxIndex + ": " + table.TableName + " ")).ConfigureAwait(false);

                        rowsCountInTable = (method == "a" || method == "t") ? table.Rows.Count : selectedIndexes.Length;

                        //int rcount = Table.Rows.Count;
                        //for (int r = 0; r < rcount; r++)
                        for (int r = 0; r < rowsCountInTable; r++)
                        {
                            if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                            {
                                //Thread.CurrentThread.Abort();
                                return;
                            }
                            else if (ProjectData.Main.InteruptTranslation)
                            {
                                ProjectData.Main.Invoke((Action)(() => ProjectData.Main.translationInteruptToolStripMenuItem.Visible = false));
                                CacheUnloadWhenNeed();
                                ProjectData.Main.ProgressInfo(false);
                                //Thread.CurrentThread.Abort();
                                return;
                            }

                            //string progressinfo;
                            if (method == "s")
                            {
                                //progressinfo = T._("getting translation: ") + (r + 1) + "/" + RowsCountInTable;
                                //индекс = первому из заданного списка выбранных индексов
                                currentRowIndex = selectedIndexes[r];
                            }
                            else if (method == "t")
                            {
                                //progressinfo = T._("getting translation: ") + (r + 1) + "/" + RowsCountInTable;
                                //индекс с нуля и до последней строки
                                currentRowIndex = r;
                            }
                            else
                            {
                                //progressinfo = T._("getting translation: ") + t + "/" + TableMaxIndex + "::" + (r + 1) + "/" + RowsCountInTable;
                                //индекс с нуля и до последней строки
                                currentRowIndex = r;
                            }

                            var row = table.Rows[currentRowIndex];
                            //ProjectData.Main.ProgressInfo(true, progressinfo);
                            //ProjectData.Main.ProgressInfo(true, T._("translating") + ": " + t + "/" + tcount + " (" + r + "/" + rcount + ")");

                            inputOriginalLine = row[origColIndex] as string;
                            if (string.IsNullOrWhiteSpace(inputOriginalLine)/* || InputOriginalLine.Length >= maxchars*/ || inputOriginalLine.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                            {
                                continue;
                            }

                            bool translateIt = false;
                            translateIt = /*(CurrentCharsCount + InputOriginalLine.Length) >= maxchars ||*/ (t == tableMaxIndex/*tcount*/ - 1 && r == rowsCountInTable/*rcount*/ - 1);

                            var cell = row[origColIndex + 1];
                            if (cell == null || string.IsNullOrEmpty(cell as string))
                            {
                                //string InputOriginalLineFromCache = FunctionsTable.TranslationCacheFind(THTranslationCache, InputOriginalLine);//поиск оригинала в кеше
                                string inputOriginalLineFromCache = ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(inputOriginalLine);


                                if (inputOriginalLineFromCache.Length > 0)
                                {
                                    ProjectData.ThFilesElementsDataset.Tables[t].Rows[currentRowIndex][origColIndex + 1] = inputOriginalLineFromCache;
                                    continue;
                                }
                                else
                                {
                                    //CurrentCharsCount += InputOriginalLine.Length;

                                    string extractedvalue;
                                    string cache;

                                    int linesCount = inputOriginalLine.GetLinesCount();
                                    int currentLineNumber = 0;
                                    bool isExtracted = false;
                                    foreach (var linevalue in inputOriginalLine.SplitToLines())
                                    {
                                        currentLineNumber++;
                                        //string linevalue = Lines[s];

                                        string translation = string.Empty;
                                        //cache = FunctionsTable.TranslationCacheFind(THTranslationCache, linevalue);//поиск подстроки в кеше
                                        cache = ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(linevalue);//поиск подстроки в кеше
                                        if (cache.Length > 0)
                                        {
                                            translation = cache;//если нашло в кеше, присвоить как перевод
                                        }
                                        else
                                        {
                                            if (linevalue.Length > 0)
                                            {
                                                if (linevalue.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                {
                                                    translation = linevalue;//если большинством ромаджи или прочее
                                                }
                                                else
                                                {
                                                    extractedvalue = ProjectData.Main.ThExtractTextForTranslation(linevalue);//извлечение подстроки

                                                    // только если извлеченное значение отличается от оригинальной строки
                                                    //cache = extractedvalue == linevalue ? string.Empty : FunctionsTable.TranslationCacheFind(THTranslationCache, extractedvalue);//поиск извлеченной подстроки в кеше
                                                    cache = extractedvalue == linevalue ? string.Empty : ProjectData.OnlineTranslationCache.GetValueFromCacheOrReturnEmpty(extractedvalue);//поиск извлеченной подстроки в кеше
                                                    if (cache.Length > 0)
                                                    {
                                                        translation = PasteTranslationBackIfExtracted(cache, linevalue, extractedvalue);
                                                    }
                                                    else
                                                    {
                                                        if (extractedvalue.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                                                        {
                                                            translation = linevalue;//если извлеченное значение большинством ромаджи или прочее
                                                        }
                                                        else
                                                        {
                                                            currentCharsCount += extractedvalue.Length;
                                                            if (!translateIt)
                                                            {
                                                                translateIt = currentCharsCount >= maxchars;//считать в лимит по символам для отправки только те строки, что будут точно отправлены
                                                            }

                                                            isExtracted = extractedvalue.Length > 0 && extractedvalue != linevalue;
                                                            inputLines.Add(extractedvalue.Length == 0 ? linevalue : extractedvalue);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //original, translation(when exists in cache), table number, row number
                                        inputLinesInfo.Add(new InputLinesInfoData(linevalue, translation, t, currentRowIndex, currentLineNumber == linesCount, isExtracted));
                                    }
                                }
                            }

                            if (translateIt)
                            {
                                currentCharsCount = 0;

                                TranslateItNow(inputLines, inputLinesInfo);
                            }
                        }
                    }

                    //перевести, если после прохода по всем таблицам добавленные строки так и не были переведены
                    if (inputLines.Count > 0)
                    {
                        TranslateItNow(inputLines, inputLinesInfo);
                    }

                    //FunctionsDBFile.WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);
                    ProjectData.OnlineTranslationCache.Write();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            CacheUnloadWhenNeed();

            ProjectData.Main.ProgressInfo(false);
        }

        private static void CacheInitWhenNeed()
        {
            //if (!Properties.Settings.Default.IsTranslationCacheEnabled)
            //    return;

            Properties.Settings.Default.OnlineTranslationCacheUseCount++;
            if (ProjectData.OnlineTranslationCache == null)
            {
                ProjectData.OnlineTranslationCache = new FunctionsOnlineCache();
                ProjectData.OnlineTranslationCache.Read();
            }
        }

        private static void CacheUnloadWhenNeed()
        {
            Properties.Settings.Default.OnlineTranslationCacheUseCount--;
            if (Properties.Settings.Default.OnlineTranslationCacheUseCount == 0)
            {
                if (ProjectData.OnlineTranslationCache != null)
                {
                    ProjectData.OnlineTranslationCache = null;
                }
            }
        }

        internal static string PasteTranslationBackIfExtracted(string translation, string original, string extracted)
        {
            if (translation.Length == 0 || original.Length == 0 || extracted.Length == 0 || Equals(original, extracted))
            {
                return translation;
                //return Properties.Settings.Default.ApplyFixesOnTranslation ?
                //    FunctionsStringFixes.ApplyHardFixes(Extracted, Translation.THFixCells()) : Translation;
            }
            //переделано через удаление и вставку строки, чтобы точно вставлялась нужная
            //строка в нужное место и с рассчетом на будущее, когда возможно строки будут выдираться из исходной
            //, а потом вставляться обратно
            int indexOfTheString = original.IndexOf(extracted);
            if (indexOfTheString > -1)
            {
                return original
                    .Remove(indexOfTheString, extracted.Length)
                    .Insert(indexOfTheString,
                    translation
                    );
                //return Original
                //    .Remove(IndexOfTheString, Extracted.Length)
                //    .Insert(IndexOfTheString,
                //    Properties.Settings.Default.ApplyFixesOnTranslation ?
                //    FunctionsStringFixes.ApplyHardFixes(Extracted, Translation.THFixCells()) : Translation
                //    );
            }
            else
            {
                return translation;
                //return Properties.Settings.Default.ApplyFixesOnTranslation ?
                //    FunctionsStringFixes.ApplyHardFixes(Original, Translation.THFixCells()) : Translation;
            }
        }

        private void TranslateItNow(List<string> inputLines, List<InputLinesInfoData> inputLinesInfo)
        {
            if (inputLines.Count > 0)
            {
                TranslateLinesAndSetTranslation(inputLines, inputLinesInfo/*, THTranslationCache*/);
            }
            else if (inputLinesInfo.Count > 0)
            {
                int previousTableIndex = -1;
                int previousRowIndex = -1;
                int newTableIndex;
                int newRowIndex;
                int rowscount = inputLinesInfo.Count;
                StringBuilder resultValue = new StringBuilder(rowscount);
                for (int i = 0; i < rowscount; i++)
                {
                    var rowInfo = inputLinesInfo[i];
                    newTableIndex = rowInfo.GetTableIndex;
                    newRowIndex = rowInfo.GetRowIndex;
                    if (string.IsNullOrEmpty(rowInfo.GetOriginal))
                    {
                        resultValue.Append(Environment.NewLine);
                    }
                    else if (!string.IsNullOrEmpty(rowInfo.GetCachedTranslation))
                    {
                        resultValue.Append(rowInfo.GetCachedTranslation);
                    }
                    if (newRowIndex == previousRowIndex && i < rowscount - 1)
                    {
                        resultValue.Append(Environment.NewLine);
                    }
                    else
                    {
                        SetTranslationResultToCellIfEmpty(previousTableIndex, previousRowIndex, resultValue/*, THTranslationCache*/);
                    }
                    previousTableIndex = newTableIndex;
                    previousRowIndex = newRowIndex;
                }
            }


            //FunctionsDBFile.WriteTranslationCacheIfValid(THTranslationCache, THTranslationCachePath);//промежуточная запись кеша
            ProjectData.OnlineTranslationCache.Write();//промежуточная запись кеша

            inputLines.Clear();
            inputLinesInfo.Clear();
            _translator.OnTranslatorClosed();
        }
        private void TranslateLinesAndSetTranslation(List<string> inputLines, List<InputLinesInfoData> inputLinesInfo)
        {
            int debugInfoIndex = 0;
            int debugTranslatedLinesIndex = 0;
            try
            {
                //https://www.codeproject.com/Questions/722877/DataTable-to-string-array
                //add table rows to string array
                string[] originalLines = inputLines.ToArray();

                //сброс кеша в GoogleAPI
                //Translator.ResetCache();

                //send string array to translation for multiline
                string[] translatedLines = null;
                try
                {
                    var originalLinesPreapplied = ApplyProjectPretranslationAction(originalLines);
                    translatedLines = _translator.Translate(originalLinesPreapplied);
                    if (translatedLines == null || translatedLines.Length == 0 || inputLines.Count != translatedLines.Length)
                    {
                        return;
                    }
                    translatedLines = ApplyProjectPosttranslationAction(originalLines, translatedLines);
                }
                catch (Exception ex)
                {
                    new FunctionsLogs().LogToFile("TranslateLinesAndSetTranslation. Error while translation:"
                        + Environment.NewLine
                        + ex
                        + Environment.NewLine
                        + "OriginalLines="
                        + string.Join(Environment.NewLine + "</br>" + Environment.NewLine, originalLines));
                }

                //int infoCount = InputLinesInfo.Rows.Count;
                //int TranslatedCount = TranslatedLines.Length-1; // -1 - отсекание последнего пустого элемента

                {
                    StringBuilder resultValue = new StringBuilder();
                    int translatedLinesIndex = 0;
                    InputLinesInfoData infoRow = null;
                    for (int infoIndex = 0; infoIndex < inputLinesInfo.Count; infoIndex++)
                    {
                        debugInfoIndex = infoIndex;
                        debugTranslatedLinesIndex = translatedLinesIndex;

                        infoRow = inputLinesInfo[infoIndex];

                        string newLine;
                        if (!infoRow.GetIsLastLineInString)
                        {
                            newLine = Environment.NewLine;
                        }
                        else
                        {
                            newLine = string.Empty;
                        }

                        if (!string.IsNullOrEmpty(infoRow.GetCachedTranslation))//перевод найден в кеше
                        {
                            if (translatedLinesIndex < translatedLines.Length && infoRow.GetOriginal != infoRow.GetCachedTranslation && !infoRow.GetCachedTranslation.HaveMostOfRomajiOtherChars())
                            {
                                resultValue.Append(
                                Properties.Settings.Default.ApplyFixesOnTranslation ?
                                    FunctionsStringFixes.ApplyHardFixes(translatedLines[translatedLinesIndex], infoRow.GetCachedTranslation.ThFixCells())
                                    : infoRow.GetCachedTranslation
                                );
                            }
                            else
                            {
                                resultValue.Append(infoRow.GetCachedTranslation);
                            }
                        }
                        else if (string.IsNullOrEmpty(infoRow.GetOriginal))//пустая строка в оригинале
                        {
                        }
                        else
                        {
                            if (inputLines[translatedLinesIndex] == infoRow.GetOriginal)
                            {
                                resultValue.Append(translatedLines[translatedLinesIndex]);
                            }
                            else
                            {
                                resultValue.Append(
                                    PasteTranslationBackIfExtracted
                                (
                                    translatedLines[translatedLinesIndex]
                                ,
                                    infoRow.GetOriginal
                                ,
                                    inputLines[translatedLinesIndex]
                                ));
                            }
                            translatedLinesIndex++;
                        }
                        resultValue.Append(newLine);

                        if (newLine.Length == 0)
                        {
                            SetTranslationResultToCellIfEmpty(
                                infoRow.GetTableIndex
                                ,
                                infoRow.GetRowIndex
                                ,
                                resultValue
                                );
                            resultValue.Clear();
                        }
                    }
                    if (resultValue.Length > 0 && infoRow != null)
                    {
                        SetTranslationResultToCellIfEmpty(
                            infoRow.GetTableIndex
                            ,
                            infoRow.GetRowIndex
                            ,
                            resultValue
                            );
                        resultValue.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                new FunctionsLogs().LogToFile(
                    Environment.NewLine + "TranslateLinesAndSetTranslation error:" + Environment.NewLine + ex
                    + Environment.NewLine + "InfoIndex=" + debugInfoIndex
                    + Environment.NewLine + "TranslatedLinesIndex=" + debugTranslatedLinesIndex
                    + Environment.NewLine);
            }
        }

        readonly GoogleApiold _translator;
        private void SetTranslationResultToCellIfEmpty(int previousTableIndex, int previousRowIndex, StringBuilder resultValue/*, DataSet THTranslationCache*/)
        {
            if (resultValue.Length > 0 && previousTableIndex > -1 && previousRowIndex > -1)
            {
                string s; //иногда значения без перевода и равны оригиналу, но отдельным переводом выбранной ячейки получается нормально
                var row = ProjectData.ThFilesElementsDataset.Tables[previousTableIndex].Rows[previousRowIndex];
                var cell = row[0];
                if (Equals(cell, resultValue))
                {
                    s = _translator.Translate(cell as string);
                }
                else
                {
                    s = resultValue.ToString();
                }

                var translationCell = row[1];
                if (translationCell == null || string.IsNullOrEmpty(translationCell as string))
                {
                    try
                    {
                        //apply fixes for translation
                        if (Properties.Settings.Default.ApplyFixesOnTranslation)
                        {
                            s = s.ThFixCells();//cell fixes
                            s = FunctionsStringFixes.ApplyHardFixes(row[0] as string, s);//hardcoded fixes
                        }

                        row[1] = s;

                        //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, Cell as string, s);
                        FunctionsOnlineCache.AddToTranslationCacheIfValid(cell as string, s);

                        //закоментировано для повышения производительности
                        //THAutoSetSameTranslationForSimular(PreviousTableIndex, PreviousRowIndex, 0);
                    }
                    catch (ArgumentNullException ex)
                    {
                        new FunctionsLogs().LogToFile("Error occured:" + Environment.NewLine + ex);
                    }
                }
                resultValue.Clear();
            }
        }
        private string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            for (int i = 0; i < originalLines.Length; i++)
            {
                var s = ProjectData.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], string.Empty);
                if (!string.IsNullOrEmpty(s))
                {
                    originalLines[i] = s;
                }
            }
            return originalLines;
        }
        private string[] ApplyProjectPosttranslationAction(string[] originalLines, string[] translatedLines)
        {
            for (int i = 0; i < translatedLines.Length; i++)
            {
                var s = ProjectData.CurrentProject.OnlineTranslationProjectSpecificPosttranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(s) && s != translatedLines[i])
                {
                    translatedLines[i] = s;
                }
            }
            return translatedLines;
        }
    }
}
