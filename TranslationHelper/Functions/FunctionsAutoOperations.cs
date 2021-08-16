using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsAutoOperations
    {
        public static string ThExtractTextForTranslation(string input)
        {
            foreach (var patternReplacementPair in ProjectData.TranslationRegexRules)
            {
                if (Regex.IsMatch(input, patternReplacementPair.Key))
                {
                    //new FunctionsLogs().LogToFile("applied translation rule: "+ PatternReplacementPair.Key);
                    return Regex.Replace(input, patternReplacementPair.Key, patternReplacementPair.Value);
                }
            }

            return input;
        }

        /// <summary>
        /// Extract all values for $1-$99 matches
        /// Work in progress
        /// </summary>
        /// <param name="projectData"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] ThExtractTextForTranslationSplit(string input)
        {
            foreach (var patternReplacementPair in ProjectData.TranslationRegexRules)
            {
                if (Regex.IsMatch(input, patternReplacementPair.Key))
                {
                    MatchCollection results = Regex.Matches(patternReplacementPair.Value, @"\$([0-9]{1,2}|(\{.+\}))");
                    if (results.Count > 0)
                    {
                        List<string> ret = new List<string>
                        {
                            patternReplacementPair.Key,
                            patternReplacementPair.Value
                        };
                        Dictionary<string, string> foundValues = new Dictionary<string, string>();
                        foreach (Match result in results)
                        {
                            if (foundValues.ContainsKey(result.Value))
                            {
                                continue;
                            }
                            var candidate = Regex.Replace(input, patternReplacementPair.Key, result.Value);
                            if (!string.IsNullOrEmpty(candidate))
                            {
                                foundValues.Add(result.Value, candidate);
                            }
                        }
                        foreach (var val in foundValues.Values)
                        {
                            ret.Add(val);
                        }
                        return ret.ToArray();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Fixes selected value
        /// </summary>
        /// <param name="projectData"></param>
        /// <param name="cvalue"></param>
        /// <returns></returns>
        public static string ThFixCells(this string cvalue)
        {
            string rule;
            string result;

            foreach (var patternReplacementPair in ProjectData.CellFixesRegexRules)
            {
                //читать правило и результат
                rule = patternReplacementPair.Key;
                result = patternReplacementPair.Value;

                try
                {
                    //задать правило
                    var regexrule = new Regex(rule);

                    //найти совпадение с заданным правилом в выбранной ячейке
                    var mc = regexrule.Matches(cvalue);
                    //перебрать все айденные совпадения
                    foreach (Match m in mc)
                    {
                        //LogToFile("match=" + m.ToString() + ", result=" + regexrule.Replace(m.Value.ToString(), result), true);

                        //исправить значения по найденным совпадениям в выбранной ячейке
                        cvalue = cvalue.Replace(m.Value + string.Empty, regexrule.Replace(m.Value + string.Empty, result));
                        //THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = Regex.Replace(THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString(), m.Value.ToString(), result);

                        //LogToFile("7 Result THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());
                    }
                }
                catch (ArgumentException ex)
                {
                    var message = T._("Error in") + " TranslationHelperCellFixesRegexRules.txt" + Environment.NewLine + "Regex: " + rule + Environment.NewLine + "Error:" + Environment.NewLine + ex;
                    new FunctionsLogs().LogToFile(message);
                    MessageBox.Show(message);
                }
            }

            return cvalue;
        }

        /// <summary>
        /// Исправления формата спецсимволов в заданной ячейке перевода
        /// <br/>Для выбранных ячеек, таблицы или для всех значений задать:
        /// <br/>method:
        /// <br/>"a" - All
        /// <br/>"t" - Table
        /// <br/>"s" - Selected
        /// <br/>..а также cind - индекс колонки, где ячейки перевода и tind - индекс таблицы, для вариантов Table и Selected
        /// <br/>Для одной выбранной ячейки, когда, например, определенная обрабатывается в коде, <br/>задать tind, cind и rind, а также true для onselectedonly
        /// </summary>
        /// <param name="method"></param>
        /// <param name="cind"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <param name="selectedonly"></param>
        public static void ThFixCells(string method, int cind, int tind, int rind = 0, bool forceApply = false)//cind - индекс столбца перевода, задан до старта потока
        {
            try
            {
                if (ProjectData.CellFixesRegexRules.Count == 0)
                {
                    return;
                }

                //индекс столбца перевода, таблицы и массив индексов для варианта с несколькими выбранными ячейками
                //int cind = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Columns["Translation"].Ordinal;//-поле untrans;//-поле untrans
                int initialtableindex = 0;
                int[] selcellscnt;

                //method
                //a - All
                //t - Table
                //s - Selected

                if (method == "s")
                {
                    //cind = THFileElementsDataGridView.Columns["Translation"].Index;//-поле untrans                            
                    initialtableindex = tind;// THFilesListBox.SelectedIndex;//установить индекс таблицы на выбранную в listbox
                    selcellscnt = FunctionsTable.GetDgvRowIndexsesInDataSetTable();
                }
                else if (method == "t")
                {
                    initialtableindex = tind;// THFilesListBox.SelectedIndex;//установить индекс таблицы на выбранную в listbox
                                             //cind = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Columns["Translation"].Ordinal;
                    selcellscnt = new int[1];//не будет использоваться с этим вариантом
                }
                else
                {
                    selcellscnt = new int[1];//не будет использоваться с этим вариантом
                }

                //количество таблиц, строк и индекс троки для использования в переборе строк и таблиц
                int tablescount;
                int rowscount;
                int rowindex;

                //LogToFile("1 rule=" + rule + ",tableindex=" + initialtableindex);
                if (method == "a")
                {
                    tablescount = ProjectData.ThFilesElementsDataset.Tables.Count;//все таблицы в dataset
                }
                else
                {
                    tablescount = initialtableindex + 1;//одна таблица с индексом на один больше индекса стартовой
                }

                //LogToFile("2 tablescount=" + tablescount);
                //перебор таблиц dataset
                for (int t = initialtableindex; t < tablescount; t++)
                {
                    //LogToFile("3 selected table index=" + t);
                    if (method == "a" || method == "t")
                    {
                        //все строки в выбранной таблице
                        rowscount = ProjectData.ThFilesElementsDataset.Tables[t].Rows.Count;
                    }
                    else
                    {
                        //все выделенные строки в выбранной таблице
                        rowscount = selcellscnt.Length;
                    }

                    //LogToFile("4 rowscount=" + rowscount);
                    //перебор строк таблицы
                    for (int r = 0; r < rowscount; r++)
                    {
                        if (method == "s")
                        {
                            //индекс = первому из заданного списка выбранных индексов
                            rowindex = selcellscnt[r];
                        }
                        else
                        {
                            //индекс с нуля и до последней строки
                            rowindex = r;
                        }

                        //LogToFile("5 selected i row index=" + i + ", value of THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "]=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind]);
                        var row = ProjectData.ThFilesElementsDataset.Tables[t].Rows[rowindex];
                        string cvalue = row[cind] + string.Empty;
                        //не трогать строку перевода, если она пустая
                        if (cvalue.Length > 0 && (forceApply || cvalue != row[cind - 1] as string))
                        {
                            //Hardcoded rules
                            //cvalue = FunctionsStringFixes.ApplyHardFixes(row[0] + string.Empty, row[1] + string.Empty);
                            //cvalue = FunctionsStringFixes.FixENJPQuoteOnStringStart2ndLine(row[0] + string.Empty, row[1] + string.Empty);
                            //cvalue = FunctionsString.FixForRPGMAkerQuotationInSomeStrings(row);
                            //cvalue = FunctionsString.FixBrokeNameVar(cvalue);
                            //cvalue = FunctionsString.FixENJPQuoteOnStringStart1stLine(row[0] + string.Empty, row[1] + string.Empty);

                            //LogToFile("6 THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());

                            //идея с извлечением строк как при переводе, только перед фиксом ячейки, чтобы обрабатывало только извлеченное
                            //здесь функция извлечения
                            //cvalue = ExtractLines(cvalue);

                            string rule;
                            string result;
                            foreach (var patternReplacementPair in ProjectData.CellFixesRegexRules)
                            {
                                //читать правило и результат
                                rule = patternReplacementPair.Key;
                                result = patternReplacementPair.Value;

                                //задать правило
                                var regexrule = new Regex(rule);

                                //найти совпадение с заданным правилом в выбранной ячейке
                                var mc = regexrule.Matches(cvalue);
                                //перебрать все айденные совпадения
                                foreach (Match m in mc)
                                {
                                    try//если будет корявый регекс и выдаст исключение
                                    {
                                        //LogToFile("match=" + m.ToString() + ", result=" + regexrule.Replace(m.Value.ToString(), result), true);

                                        //исправить значения по найденным совпадениям в выбранной ячейке
                                        cvalue = cvalue.Replace(m.Value + string.Empty, regexrule.Replace(m.Value + string.Empty, result));
                                        //THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = Regex.Replace(THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString(), m.Value.ToString(), result);

                                        //LogToFile("7 Result THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());
                                    }
                                    catch
                                    {
                                        MessageBox.Show(T._("Error in") + " TranslationHelperCellFixesRegexRules.txt" + Environment.NewLine + "Regex: " + rule);
                                    }
                                }
                            }

                            //идея с извлечением строк как при переводе, только перед фиксом ячейки, чтобы обрабатывало только извлеченное
                            //здесь функция возвращения извлеченного
                            //cvalue = RestoreExtracted(cvalue, row[cind - 1] as string);

                            if (!Equals(row[cind], cvalue))
                            {
                                //ProjectData.THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = cvalue;
                                row[cind] = cvalue;
                            }
                        }
                    }
                }
                //LogToFile(string.Empty,true);
            }
            catch
            {
            }
        }

        private static string RestoreExtracted(string cvalue, string v)
        {
            //FunctionsOnlineTranslation.PasteTranslationBackIfExtracted(cvalue, row[cind - 1] as string, cvalue);
            throw new NotImplementedException();
        }

        private static string ExtractLines(string cvalue)
        {
            throw new NotImplementedException();
        }

        public static string GetStringSimularityRegexPattern()
        {
            //http://www.cyberforum.ru/csharp-beginners/thread244709.html
            string regexPatternQuotationMark = "\"";
            //string regexPatternLatinSymbols = @"\d|\!|\?|\.|[|]";
            string regexPatternLatinSymbolsStart = @"\d|\!|\?|\.|\[";
            string regexPatternLatinSymbolsEnd = @"\d|\!|\?|\.|\]";
            //string regexPatternJapanesSymbols = @"\！|\？|\。|\【|\】|\「|\」|\『|\』|\〝|\〟";
            string regexPatternJapanesSymbolsStart = @"！|？|。|【|「|『|〝";
            string regexPatternJapanesSymbolsEnd = @"！|？|。|】|」|』|〟";
            string regexPatternDigitsInAnyPlace = @"\d+";
            string regexPatternDigitsOrSymbolsInStartOfLine = "(^(" + regexPatternJapanesSymbolsStart + "|" + regexPatternQuotationMark + "|" + regexPatternLatinSymbolsStart + ")+)";
            string regexPatternDigitsOrSymbolsInEndOfLine = "((" + regexPatternLatinSymbolsEnd + "|" + regexPatternQuotationMark + "|" + regexPatternJapanesSymbolsEnd + ")+$)";

            return regexPatternDigitsOrSymbolsInStartOfLine + "|" + regexPatternDigitsInAnyPlace + "|" + regexPatternDigitsOrSymbolsInEndOfLine;
        }

        static bool _thAutoSetSameTranslationForSimularIsBusy;
        static bool _forcevalue;
        static int _iTableIndex;
        static int _iRowIndex;
        static int _iColumnIndex;
        static string _trc;
        static readonly Dictionary<string, bool> ThAutoSetSameTranslationForSimularData = new Dictionary<string, bool>();
        public static void ThAutoSetSameTranslationForSimular(int inputTableIndex, int inputRowIndex, int inputColumnIndex, bool forceSetValue = false)
        {
            if (!Properties.Settings.Default.ProjectIsOpened)
                return;

            if (inputTableIndex == -1
                || inputRowIndex == -1
                || inputColumnIndex == -1
                || ProjectData.ThFilesElementsDataset == null
                || inputTableIndex > ProjectData.ThFilesElementsDataset.Tables.Count - 1
                || inputRowIndex > ProjectData.ThFilesElementsDataset.Tables[inputTableIndex].Rows.Count - 1
                || (ProjectData.ThFilesElementsDataset.Tables[inputTableIndex].Rows[inputRowIndex][inputColumnIndex + 1] + string.Empty).Length == 0)
            {
                return;
            }
            //if (Properties.Settings.Default.THAutoSetSameTranslationForSimularIsBusy)
            //{
            //    return;
            //}
            if (!ThAutoSetSameTranslationForSimularData.ContainsKey(inputTableIndex + "|" + inputRowIndex + "|" + inputColumnIndex))
            {
                ThAutoSetSameTranslationForSimularData.Add(inputTableIndex + "|" + inputRowIndex + "|" + inputColumnIndex, forceSetValue);
            }

            while (!_thAutoSetSameTranslationForSimularIsBusy && ThAutoSetSameTranslationForSimularData.Count > 0)
            {
                _thAutoSetSameTranslationForSimularIsBusy = true;

                try
                {
                    {
                        //re-set input variables to prevent break of work while concurent execution
                        //forcevalue = ForceSetValue;
                        //iTableIndex = InputTableIndex;
                        //iRowIndex = InputRowIndex;
                        //iCellIndex = InputCellIndex;
                    }

                    //присвоить значения для обработки
                    _trc = ThAutoSetSameTranslationForSimularData.ElementAt(0).Key;
                    _iTableIndex = int.Parse(_trc.Split('|')[0], CultureInfo.InvariantCulture);
                    _iRowIndex = int.Parse(_trc.Split('|')[1], CultureInfo.InvariantCulture);
                    _iColumnIndex = int.Parse(_trc.Split('|')[2], CultureInfo.InvariantCulture);
                    _forcevalue = ThAutoSetSameTranslationForSimularData[_trc];

                    var inputTable = ProjectData.ThFilesElementsDataset.Tables[_iTableIndex];
                    var inputTableRow = inputTable.Rows[_iRowIndex];
                    var inputTableOriginalCell = inputTableRow[_iColumnIndex];
                    int translationColumnIndex = _iColumnIndex + 1;
                    var inputTableTranslationCell = inputTableRow[translationColumnIndex];
                    if (inputTableTranslationCell == null || string.IsNullOrEmpty(inputTableTranslationCell as string))
                    {
                    }
                    else//Запускать сравнение только если ячейка имеет значение
                    {
                        //LogToFile("THFilesElementsDataset.Tables[tableind].Rows[rind][transcind]="+ THFilesElementsDataset.Tables[tableind].Rows[rind][transcind].ToString());

                        string regexPattern = GetStringSimularityRegexPattern();

                        Regex reg = new Regex(regexPattern); //reg равняется любым цифрам
                        string inputOrigCellValue = FunctionsRomajiKana.ThFixDigits(inputTableOriginalCell as string);
                        string inputTransCellValue = FunctionsRomajiKana.ThFixDigits(inputTableTranslationCell as string);

                        //Было исключение OutOfRangeException когда в оригинале присутствовали совпадения для regex, а входной перевод был пустой или равен \r\n, тогда попытка получить индекс совпадения из оригинала заканчивалась исключением, т.к. никаких совпадений не было. Похоже на неверный перевод от онлайн сервиса
                        if (string.IsNullOrWhiteSpace(inputTransCellValue) || inputTransCellValue == Environment.NewLine)
                            return;

                        //set same value for duplicates
                        if (!Properties.Settings.Default.DontLoadDuplicates && ProjectData.OriginalsTableRowCoordinats!=null && ProjectData.OriginalsTableRowCoordinats[inputOrigCellValue].Values.Count > 1)
                        {
                            var tableName = inputTable.TableName;
                            foreach (var t in ProjectData.OriginalsTableRowCoordinats[inputOrigCellValue])
                            {
                                var table = ProjectData.ThFilesElementsDataset.Tables[t.Key];
                                foreach (var rind in t.Value)
                                {
                                    var row = table.Rows[rind];

                                    //skip if same table\row as input or row translation is not empty
                                    if ((t.Key == tableName && rind == _iRowIndex) || (row[1] + "").Length > 0)
                                    {
                                        continue;
                                    }

                                    row[1] = inputTransCellValue;
                                }
                            }
                        }

                        //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
                        if (Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese" && Regex.Matches(inputTransCellValue, @"\d+").Count != Regex.Matches(inputOrigCellValue, @"\d+").Count)
                        {
                            Properties.Settings.Default.THAutoSetSameTranslationForSimularIsBusy = false;
                            return;
                        }

                        MatchCollection mc = reg.Matches(inputOrigCellValue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
                        int mccount = mc.Count;

                        int tableCount = ProjectData.ThFilesElementsDataset.Tables.Count;
                        for (int tindx = 0; tindx < tableCount; tindx++) //количество файлов
                        {
                            var table = ProjectData.ThFilesElementsDataset.Tables[tindx];
                            var rowsCount = table.Rows.Count;
                            //LogToFile("Table "+Tindx+" proceed");
                            for (int rindx = 0; rindx < rowsCount; rindx++) //количество строк в каждом файле
                            {
                                //если приложение закрылось
                                if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                                {
                                    return;
                                }

                                var row = table.Rows[rindx];
                                var targetOriginallCell = row[_iColumnIndex];
                                var targetTranslationCell = row[translationColumnIndex];
                                string targetOriginallCellString = targetOriginallCell as string;
                                string targetTranslationCellString = targetTranslationCell + string.Empty;
                                if ((_forcevalue && rindx != _iRowIndex && /*только если оригинал и перевод целевой ячейки не равны-*/targetTranslationCellString != targetOriginallCellString) || string.IsNullOrEmpty(targetTranslationCellString)) //Проверять только для пустых ячеек перевода
                                {
                                    //LogToFile("THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString()=" + THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString());
                                    //если количество совпадений в mc больше нуля, т.е. цифры были в поле untrans выбранной только что переведенной ячейки
                                    //также проверить, если оригиналы с цифрами не равны, иначе присваивать по обычному
                                    if (mccount > 0 && !Equals(inputTableOriginalCell, targetOriginallCell))
                                    {
                                        //string TargetOriginallCellStringFixed = RomajiKana.THFixDigits(TargetOriginallCellString);
                                        MatchCollection mc0 = reg.Matches(targetOriginallCellString); //mc0 равно значениям цифр ячейки под номером y в файле i
                                        int mc0Count = mc0.Count;
                                        if (mc0Count > 0) //если количество совпадений в mc0 больше нуля, т.е. цифры были в поле untrans проверяемой на совпадение ячейки
                                        {
                                            string targetTransCellValueWithRemovedPatternMatches = Regex.Replace(targetTranslationCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                                            string inputTransCellValueWithRemovedPatternMatches = Regex.Replace(inputTransCellValue, regexPattern, string.Empty, RegexOptions.Compiled);

                                            //Если значение ячеек перевода без паттернов равны, идти дальше
                                            if (targetTransCellValueWithRemovedPatternMatches == inputTransCellValueWithRemovedPatternMatches)
                                            {
                                                continue;
                                            }

                                            string targetOrigCellValueWithRemovedPatternMatches = Regex.Replace(targetOriginallCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                                            string inputOrigCellValueWithRemovedPatternMatches = Regex.Replace(inputOrigCellValue, regexPattern, string.Empty, RegexOptions.Compiled);

                                            //LogToFile("checkingorigcellvalue=\r\n" + checkingorigcellvalue + "\r\ninputorigcellvalue=\r\n" + inputorigcellvalue);
                                            //если поле перевода равно только что измененному во входной, без учета цифр
                                            if ((targetOrigCellValueWithRemovedPatternMatches == inputOrigCellValueWithRemovedPatternMatches) && mccount == mc0Count && IsAllMatchesInIdenticalPlaces(mc, mc0))
                                            {
                                                {
                                                    ////инициализация основных целевого и входного массивов
                                                    //string[] inputOrigMatches = new string[mccount];
                                                    //string[] targetOrigMatches = new string[mccount];
                                                    ////присваивание цифр из совпадений в массивы, в основной входного и во временный целевого
                                                    //for (int r = 0; r < mccount; r++)
                                                    //{
                                                    //    inputOrigMatches[r] = RomajiKana.THFixDigits(mc[r].Value/*.Replace(mc[r].Value, mc[r].Value)*/);
                                                    //    targetOrigMatches[r] = RomajiKana.THFixDigits(mc0[r].Value/*.Replace(mc0[r].Value, mc0[r].Value)*/);
                                                    //}
                                                    //также инфо о другом способе:
                                                    //http://qaru.site/questions/41136/how-to-convert-matchcollection-to-string-array
                                                    //там же все тесты и for, как у здесь меня - наиболее быстрый вариант

                                                    //string inputresult = Regex.Replace(inputtranscellvalue, pattern, "{{$1}}");//оборачивание цифры в {{}}, чтобы избежать ошибочных замен например замены 5 на 6 в значении, где есть 5 50
                                                    //переименовано и закомментировано, т.к. было убрано оборачивание в цифры. string inputtranscellvalue = inputtranscellvalue;//оборачивание цифры в {{}}, чтобы избежать ошибочных замен например замены 5 на 6 в значении, где есть 5 50
                                                }

                                                MatchCollection tm = reg.Matches(inputTransCellValue);

                                                //количество совпадений должно быть равное для избежания исключений и прочих неверных замен
                                                if (tm.Count != mc.Count)
                                                    return;

                                                int startindex;
                                                int stringoverallength = 0;
                                                int stringlength;
                                                int stringoverallength0 = 0;
                                                bool failed = false;
                                                //LogToFile("arraysize=" + arraysize + ", wrapped inputresult" + inputresult);
                                                for (int m = 0; m < mccount; m++)
                                                {
                                                    //проверка, ЧТОБЫ СОВПАДЕНИЯ ОТЛИЧАЛИСЬ, Т.Е. НЕ МЕНЯТЬ ! НА ! И ? НА ?, ТОЛЬКО ! НА ? И 1 НА 2
                                                    if (mc[m].Value == mc0[m].Value)
                                                    {
                                                        continue;
                                                    }

                                                    //замена символа путем удаления на позиции и вставки нового:https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
                                                    startindex = tm[m].Index - stringoverallength + stringoverallength0;//отнять предыдущее число и заменить новым числом, для корректировки индекса

                                                    stringlength = tm[m].Value.Length;
                                                    stringoverallength += stringlength;//запомнить общую длину заменяемых символов, для коррекции индекса позиции для замены

                                                    //InputTransCellValue = InputTransCellValue.Remove(startindex, stringlength).Insert(startindex, targetOrigMatches[m]);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."
                                                    inputTransCellValue = inputTransCellValue.Remove(startindex, stringlength).Insert(startindex, mc0[m].Value);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."

                                                    //stringoverallength0 += targetOrigMatches[m].Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены
                                                    stringoverallength0 += mc0[m].Value.Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены

                                                }
                                                //только если ячейка пустая
                                                targetTranslationCell = ProjectData.ThFilesElementsDataset.Tables[tindx].Rows[rindx][translationColumnIndex];
                                                if (!failed && (_forcevalue || targetTranslationCell == null || string.IsNullOrEmpty(targetTranslationCell as string)))
                                                {
                                                    //ProjectData.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTransCellValue;
                                                    row[translationColumnIndex] = inputTransCellValue;
                                                }
                                            }
                                        }
                                    }
                                    else //иначе, если в поле оригинала не было цифр, сравнить как обычно, два поля между собой 
                                    {
                                        if (Equals(row[_iColumnIndex], inputTableOriginalCell)) //если поле Untrans елемента равно только что измененному
                                        {
                                            //ProjectData.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                                            row[translationColumnIndex] = inputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }

                if (ThAutoSetSameTranslationForSimularData.ContainsKey(_trc))
                {
                    ThAutoSetSameTranslationForSimularData.Remove(_trc);
                }
                _thAutoSetSameTranslationForSimularIsBusy = false;
            }
        }

        public static bool IsAllMatchesInIdenticalPlaces(MatchCollection mc, MatchCollection mc0)
        {
            try
            {
                int overallength = 0;
                int overallength0 = 0;
                int mcCount = mc.Count;
                for (int m = 0; m < mcCount; m++)
                {
                    if (mc[m].Index - overallength == mc0[m].Index - overallength0)
                    {
                        overallength += mc[m].Length;
                        overallength0 += mc0[m].Length;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
