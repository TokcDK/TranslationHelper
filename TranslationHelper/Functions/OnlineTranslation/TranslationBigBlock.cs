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
        public TranslationBigBlock(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override void Get()
        {
            THOnlineTranslateByBigBlocks2();
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
                if (Properties.Settings.Default.UseAllDBFilesForOnlineTranslationForAll && Method == "a")
                {
                    thDataWork.Main.ProgressInfo(true, "Get all databases");
                    FunctionsDBFile.MergeAllDBtoOne(thDataWork);
                }

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
                                //Thread.CurrentThread.Abort();
                                return;
                            }
                            else if (thDataWork.Main.InteruptTranslation)
                            {
                                thDataWork.Main.Invoke((Action)(() => thDataWork.Main.translationInteruptToolStripMenuItem.Visible = false));
                                CacheUnloadWhenNeed(thDataWork);
                                thDataWork.Main.ProgressInfo(false);
                                //Thread.CurrentThread.Abort();
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
                            if (string.IsNullOrWhiteSpace(InputOriginalLine)/* || InputOriginalLine.Length >= maxchars*/ || InputOriginalLine.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther())
                            {
                                continue;
                            }

                            bool TranslateIt = false;
                            TranslateIt = /*(CurrentCharsCount + InputOriginalLine.Length) >= maxchars ||*/ (t == TableMaxIndex/*tcount*/ - 1 && r == RowsCountInTable/*rcount*/ - 1);

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
                                    //CurrentCharsCount += InputOriginalLine.Length;

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
                                                            CurrentCharsCount += extractedvalue.Length;
                                                            if (!TranslateIt)
                                                            {
                                                                TranslateIt = CurrentCharsCount >= maxchars;//считать в лимит по символам для отправки только те строки, что будут точно отправлены
                                                            }

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
                    thDataWork.OnlineTranslationCache.Write();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            CacheUnloadWhenNeed(thDataWork);

            thDataWork.Main.ProgressInfo(false);
        }

        private static void CacheInitWhenNeed(THDataWork thDataWork)
        {
            //if (!Properties.Settings.Default.IsTranslationCacheEnabled)
            //    return;

            Properties.Settings.Default.OnlineTranslationCacheUseCount++;
            if (thDataWork.OnlineTranslationCache == null)
            {
                thDataWork.OnlineTranslationCache = new FunctionsOnlineCache(thDataWork);
                thDataWork.OnlineTranslationCache.Read();
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

        internal static string PasteTranslationBackIfExtracted(string Translation, string Original, string Extracted)
        {
            if (Translation.Length == 0 || Original.Length == 0 || Extracted.Length == 0 || Equals(Original, Extracted))
            {
                return Translation;
                //return Properties.Settings.Default.ApplyFixesOnTranslation ?
                //    FunctionsStringFixes.ApplyHardFixes(Extracted, Translation.THFixCells(thDataWork), thDataWork) : Translation;
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
                    Translation
                    );
                //return Original
                //    .Remove(IndexOfTheString, Extracted.Length)
                //    .Insert(IndexOfTheString,
                //    Properties.Settings.Default.ApplyFixesOnTranslation ?
                //    FunctionsStringFixes.ApplyHardFixes(Extracted, Translation.THFixCells(thDataWork), thDataWork) : Translation
                //    );
            }
            else
            {
                return Translation;
                //return Properties.Settings.Default.ApplyFixesOnTranslation ?
                //    FunctionsStringFixes.ApplyHardFixes(Original, Translation.THFixCells(thDataWork), thDataWork) : Translation;
            }
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
            thDataWork.OnlineTranslationCache.Write();//промежуточная запись кеша

            InputLines.Clear();
            InputLinesInfo.Clear();
            Translator.OnTranslatorClosed();
        }
        private void TranslateLinesAndSetTranslation(List<string> InputLines, List<InputLinesInfoData> InputLinesInfo)
        {
            int DebugInfoIndex = 0;
            int DebugTranslatedLinesIndex = 0;
            try
            {
                //https://www.codeproject.com/Questions/722877/DataTable-to-string-array
                //add table rows to string array
                string[] OriginalLines = InputLines.ToArray();

                //сброс кеша в GoogleAPI
                //Translator.ResetCache();

                //send string array to translation for multiline
                string[] TranslatedLines = null;
                try
                {
                    var OriginalLinesPreapplied = ApplyProjectPretranslationAction(OriginalLines);
                    TranslatedLines = Translator.Translate(OriginalLinesPreapplied);
                    if (TranslatedLines == null || TranslatedLines.Length == 0 || InputLines.Count != TranslatedLines.Length)
                    {
                        return;
                    }
                    TranslatedLines = ApplyProjectPosttranslationAction(OriginalLines, TranslatedLines);
                }
                catch (Exception ex)
                {
                    new FunctionsLogs().LogToFile("TranslateLinesAndSetTranslation. Error while translation:"
                        + Environment.NewLine
                        + ex
                        + Environment.NewLine
                        + "OriginalLines="
                        + string.Join(Environment.NewLine + "</br>" + Environment.NewLine, OriginalLines));
                }

                //int infoCount = InputLinesInfo.Rows.Count;
                //int TranslatedCount = TranslatedLines.Length-1; // -1 - отсекание последнего пустого элемента

                {
                    StringBuilder ResultValue = new StringBuilder();
                    int TranslatedLinesIndex = 0;
                    InputLinesInfoData InfoRow = null;
                    for (int InfoIndex = 0; InfoIndex < InputLinesInfo.Count; InfoIndex++)
                    {
                        DebugInfoIndex = InfoIndex;
                        DebugTranslatedLinesIndex = TranslatedLinesIndex;

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
                            if (TranslatedLinesIndex < TranslatedLines.Length && InfoRow.GetOriginal != InfoRow.GetCachedTranslation && !InfoRow.GetCachedTranslation.HaveMostOfRomajiOtherChars())
                            {
                                ResultValue.Append(
                                Properties.Settings.Default.ApplyFixesOnTranslation ?
                                    FunctionsStringFixes.ApplyHardFixes(TranslatedLines[TranslatedLinesIndex], InfoRow.GetCachedTranslation.THFixCells(thDataWork), thDataWork)
                                    : InfoRow.GetCachedTranslation
                                );
                            }
                            else
                            {
                                ResultValue.Append(InfoRow.GetCachedTranslation);
                            }
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
            catch (Exception ex)
            {
                new FunctionsLogs().LogToFile(
                    Environment.NewLine + "TranslateLinesAndSetTranslation error:" + Environment.NewLine + ex
                    + Environment.NewLine + "InfoIndex=" + DebugInfoIndex
                    + Environment.NewLine + "TranslatedLinesIndex=" + DebugTranslatedLinesIndex
                    + Environment.NewLine);
            }
        }

        readonly GoogleAPIOLD Translator;
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
                    try
                    {
                        //apply fixes for translation
                        if (Properties.Settings.Default.ApplyFixesOnTranslation)
                        {
                            s = s.THFixCells(thDataWork);//cell fixes
                            s = FunctionsStringFixes.ApplyHardFixes(Row[0] as string, s, thDataWork);//hardcoded fixes
                        }

                        Row[1] = s;

                        //FunctionsTable.AddToTranslationCacheIfValid(THTranslationCache, Cell as string, s);
                        FunctionsOnlineCache.AddToTranslationCacheIfValid(thDataWork, Cell as string, s);

                        //закоментировано для повышения производительности
                        //THAutoSetSameTranslationForSimular(PreviousTableIndex, PreviousRowIndex, 0);
                    }
                    catch (ArgumentNullException ex)
                    {
                        new FunctionsLogs().LogToFile("Error occured:" + Environment.NewLine + ex);
                    }
                }
                ResultValue.Clear();
            }
        }
        private string[] ApplyProjectPretranslationAction(string[] originalLines)
        {
            for (int i = 0; i < originalLines.Length; i++)
            {
                var s = thDataWork.CurrentProject.OnlineTranslationProjectSpecificPretranslationAction(originalLines[i], string.Empty);
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
                var s = thDataWork.CurrentProject.OnlineTranslationProjectSpecificPosttranslationAction(originalLines[i], translatedLines[i]);
                if (!string.IsNullOrEmpty(s) && s != translatedLines[i])
                {
                    translatedLines[i] = s;
                }
            }
            return translatedLines;
        }
    }
}
