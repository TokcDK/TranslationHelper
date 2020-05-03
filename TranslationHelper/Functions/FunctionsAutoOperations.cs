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
    class FunctionsAutoOperations
    {
        public static string THExtractTextForTranslation(THDataWork thDataWork, string input)
        {
            string returnValue = input;

            foreach (var PatternReplacementPair in thDataWork.TranslationRegexRules)
            {
                if (Regex.IsMatch(returnValue, PatternReplacementPair.Key))
                {
                    returnValue = Regex.Replace(returnValue, PatternReplacementPair.Key, PatternReplacementPair.Value);
                }
            }

            //если файл с правилами существует
            //if (File.Exists(Path.Combine(Application.StartupPath, "TranslationHelperTranslationRegexRules.txt")))
            //{
            //    //читать файл с правилами
            //    using (StreamReader rules = new StreamReader(Path.Combine(Application.StartupPath, "TranslationHelperTranslationRegexRules.txt")))
            //    {
            //        //regex правило и результат из файла
            //        string regexPattern = string.Empty;
            //        string regexReplacement = string.Empty;
            //        bool ReadRule = true;
            //        while (!rules.EndOfStream)
            //        {
            //            try
            //            {
            //                //читать правило и результат
            //                if (ReadRule)
            //                {
            //                    regexPattern = rules.ReadLine();
            //                    if (string.IsNullOrWhiteSpace(regexPattern) || regexPattern.TrimStart().StartsWith(";"))//игнорировать комментарии
            //                    {
            //                        continue;
            //                    }
            //                    ReadRule = !ReadRule;
            //                    continue;
            //                }
            //                else
            //                {
            //                    regexReplacement = rules.ReadLine();
            //                    if (string.IsNullOrWhiteSpace(regexPattern) || regexReplacement.TrimStart().StartsWith(";") || !FunctionsString.IsStringAContainsStringB(regexReplacement, "$"))//игнорировать комментарии
            //                    {
            //                        continue;
            //                    }
            //                    ReadRule = !ReadRule;
            //                }

            //                //if (returnValue == input)
            //                //{
            //                //}
            //                //else
            //                //{
            //                //    break;
            //                //}
            //            }
            //            catch
            //            {

            //            }
            //        }
            //    }
            //}

            return returnValue;
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
        /// <param name="Method"></param>
        /// <param name="cind"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <param name="selectedonly"></param>
        public static void THFixCells(THDataWork thDataWork, string Method, int cind, int tind, int rind = 0, bool forceApply = false)//cind - индекс столбца перевода, задан до старта потока
        {
            try
            {
                if (thDataWork.CellFixesRegexRules.Count == 0)
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

                if (Method == "s")
                {
                    //cind = THFileElementsDataGridView.Columns["Translation"].Index;//-поле untrans                            
                    initialtableindex = tind;// THFilesListBox.SelectedIndex;//установить индекс таблицы на выбранную в listbox
                    selcellscnt = FunctionsTable.GetDGVRowIndexsesInDataSetTable(thDataWork);
                }
                else if (Method == "t")
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
                if (Method == "a")
                {
                    tablescount = thDataWork.THFilesElementsDataset.Tables.Count;//все таблицы в dataset
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
                    if (Method == "a" || Method == "t")
                    {
                        //все строки в выбранной таблице
                        rowscount = thDataWork.THFilesElementsDataset.Tables[t].Rows.Count;
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
                        if (Method == "s")
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
                        var row = thDataWork.THFilesElementsDataset.Tables[t].Rows[rowindex];
                        string cvalue = row[cind] + string.Empty;
                        //не трогать строку перевода, если она пустая
                        if (cvalue.Length > 0 && (forceApply || cvalue != row[cind - 1] as string))
                        {
                            //Hardcoded rules
                            cvalue = FunctionsStringFixes.ApplyHardFixes(row[0] + string.Empty, row[1] + string.Empty);
                            //cvalue = FunctionsStringFixes.FixENJPQuoteOnStringStart2ndLine(row[0] + string.Empty, row[1] + string.Empty);
                            //cvalue = FunctionsString.FixForRPGMAkerQuotationInSomeStrings(row);
                            //cvalue = FunctionsString.FixBrokeNameVar(cvalue);
                            //cvalue = FunctionsString.FixENJPQuoteOnStringStart1stLine(row[0] + string.Empty, row[1] + string.Empty);

                            //LogToFile("6 THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());

                            string rule;
                            string result;

                            foreach (var PatternReplacementPair in thDataWork.CellFixesRegexRules)
                            {
                                //читать правило и результат
                                rule = PatternReplacementPair.Key;
                                result = PatternReplacementPair.Value;

                                //задать правило
                                Regex regexrule = new Regex(rule);

                                //найти совпадение с заданным правилом в выбранной ячейке
                                MatchCollection mc = regexrule.Matches(cvalue);
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

                            if (!Equals(row[cind], cvalue))
                            {
                                //thDataWork.THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = cvalue;
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

        static bool THAutoSetSameTranslationForSimularIsBusy = false;
        static bool forcevalue;
        static int iTableIndex;
        static int iRowIndex;
        static int iColumnIndex;
        static string TRC;
        static Dictionary<string, bool> THAutoSetSameTranslationForSimularData = new Dictionary<string, bool>();
        public static void THAutoSetSameTranslationForSimular(THDataWork thDataWork, int InputTableIndex, int InputRowIndex, int InputColumnIndex, bool ForceSetValue = false)
        {
            if (!Properties.Settings.Default.ProjectIsOpened)
                return;

            if (InputTableIndex == -1
                || InputRowIndex == -1
                || InputColumnIndex == -1
                || thDataWork.THFilesElementsDataset == null
                || InputTableIndex > thDataWork.THFilesElementsDataset.Tables.Count - 1
                || InputRowIndex > thDataWork.THFilesElementsDataset.Tables[InputTableIndex].Rows.Count - 1
                || (thDataWork.THFilesElementsDataset.Tables[InputTableIndex].Rows[InputRowIndex][InputColumnIndex + 1] + string.Empty).Length == 0)
            {
                return;
            }
            //if (Properties.Settings.Default.THAutoSetSameTranslationForSimularIsBusy)
            //{
            //    return;
            //}
            if (!THAutoSetSameTranslationForSimularData.ContainsKey(InputTableIndex + "|" + InputRowIndex + "|" + InputColumnIndex))
            {
                THAutoSetSameTranslationForSimularData.Add(InputTableIndex + "|" + InputRowIndex + "|" + InputColumnIndex, ForceSetValue);
            }

            while (!THAutoSetSameTranslationForSimularIsBusy && THAutoSetSameTranslationForSimularData.Count > 0)
            {
                THAutoSetSameTranslationForSimularIsBusy = true;

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
                    TRC = THAutoSetSameTranslationForSimularData.ElementAt(0).Key;
                    iTableIndex = int.Parse(TRC.Split('|')[0], CultureInfo.GetCultureInfo("en-US"));
                    iRowIndex = int.Parse(TRC.Split('|')[1], CultureInfo.GetCultureInfo("en-US"));
                    iColumnIndex = int.Parse(TRC.Split('|')[2], CultureInfo.GetCultureInfo("en-US"));
                    forcevalue = THAutoSetSameTranslationForSimularData[TRC];

                    var InputTableRow = thDataWork.THFilesElementsDataset.Tables[iTableIndex].Rows[iRowIndex];
                    var InputTableOriginalCell = InputTableRow[iColumnIndex];
                    int TranslationColumnIndex = iColumnIndex + 1;
                    var InputTableTranslationCell = InputTableRow[TranslationColumnIndex];
                    if (InputTableTranslationCell == null || string.IsNullOrEmpty(InputTableTranslationCell as string))
                    {
                    }
                    else//Запускать сравнение только если ячейка имеет значение
                    {
                        //LogToFile("THFilesElementsDataset.Tables[tableind].Rows[rind][transcind]="+ THFilesElementsDataset.Tables[tableind].Rows[rind][transcind].ToString());

                        string regexPattern = GetStringSimularityRegexPattern();

                        Regex reg = new Regex(regexPattern); //reg равняется любым цифрам
                        string InputOrigCellValue = FunctionsRomajiKana.THFixDigits(InputTableOriginalCell as string);
                        string InputTransCellValue = FunctionsRomajiKana.THFixDigits(InputTableTranslationCell as string);

                        //Было исключение OutOfRangeException когда в оригинале присутствовали совпадения для regex, а входной перевод был пустой или равен \r\n, тогда попытка получить индекс совпадения из оригинала заканчивалась исключением, т.к. никаких совпадений не было. Похоже на неверный перевод от онлайн сервиса
                        if (string.IsNullOrWhiteSpace(InputTransCellValue) || InputTransCellValue == Environment.NewLine)
                            return;


                        //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
                        if (Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese" && Regex.Matches(InputTransCellValue, @"\d+").Count != Regex.Matches(InputOrigCellValue, @"\d+").Count)
                        {
                            Properties.Settings.Default.THAutoSetSameTranslationForSimularIsBusy = false;
                            return;
                        }

                        MatchCollection mc = reg.Matches(InputOrigCellValue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
                        int mccount = mc.Count;

                        int TableCount = thDataWork.THFilesElementsDataset.Tables.Count;
                        for (int Tindx = 0; Tindx < TableCount; Tindx++) //количество файлов
                        {
                            var Table = thDataWork.THFilesElementsDataset.Tables[Tindx];
                            var RowsCount = Table.Rows.Count;
                            //LogToFile("Table "+Tindx+" proceed");
                            for (int Rindx = 0; Rindx < RowsCount; Rindx++) //количество строк в каждом файле
                            {
                                //если приложение закрылось
                                if (Properties.Settings.Default.IsTranslationHelperWasClosed)
                                {
                                    return;
                                }

                                var TRow = Table.Rows[Rindx];
                                var TargetOriginallCell = TRow[iColumnIndex];
                                var TargetTranslationCell = TRow[TranslationColumnIndex];
                                string TargetOriginallCellString = TargetOriginallCell as string;
                                string TargetTranslationCellString = TargetTranslationCell + string.Empty;
                                if ((forcevalue && Rindx != iRowIndex && /*только если оригинал и перевод целевой ячейки не равны-*/TargetTranslationCellString != TargetOriginallCellString) || string.IsNullOrEmpty(TargetTranslationCellString)) //Проверять только для пустых ячеек перевода
                                {
                                    //LogToFile("THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString()=" + THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString());
                                    //если количество совпадений в mc больше нуля, т.е. цифры были в поле untrans выбранной только что переведенной ячейки
                                    //также проверить, если оригиналы с цифрами не равны, иначе присваивать по обычному
                                    if (mccount > 0 && !Equals(InputTableOriginalCell, TargetOriginallCell))
                                    {
                                        //string TargetOriginallCellStringFixed = RomajiKana.THFixDigits(TargetOriginallCellString);
                                        MatchCollection mc0 = reg.Matches(TargetOriginallCellString); //mc0 равно значениям цифр ячейки под номером y в файле i
                                        int mc0Count = mc0.Count;
                                        if (mc0Count > 0) //если количество совпадений в mc0 больше нуля, т.е. цифры были в поле untrans проверяемой на совпадение ячейки
                                        {
                                            string TargetTransCellValueWithRemovedPatternMatches = Regex.Replace(TargetTranslationCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                                            string InputTransCellValueWithRemovedPatternMatches = Regex.Replace(InputTransCellValue, regexPattern, string.Empty, RegexOptions.Compiled);

                                            //Если значение ячеек перевода без паттернов равны, идти дальше
                                            if (TargetTransCellValueWithRemovedPatternMatches == InputTransCellValueWithRemovedPatternMatches)
                                            {
                                                continue;
                                            }

                                            string TargetOrigCellValueWithRemovedPatternMatches = Regex.Replace(TargetOriginallCellString, regexPattern, string.Empty, RegexOptions.Compiled);
                                            string InputOrigCellValueWithRemovedPatternMatches = Regex.Replace(InputOrigCellValue, regexPattern, string.Empty, RegexOptions.Compiled);

                                            //LogToFile("checkingorigcellvalue=\r\n" + checkingorigcellvalue + "\r\ninputorigcellvalue=\r\n" + inputorigcellvalue);
                                            //если поле перевода равно только что измененному во входной, без учета цифр
                                            if ((TargetOrigCellValueWithRemovedPatternMatches == InputOrigCellValueWithRemovedPatternMatches) && mccount == mc0Count && IsAllMatchesInIdenticalPlaces(mc, mc0))
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

                                                MatchCollection tm = reg.Matches(InputTransCellValue);

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
                                                    InputTransCellValue = InputTransCellValue.Remove(startindex, stringlength).Insert(startindex, mc0[m].Value);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."

                                                    //stringoverallength0 += targetOrigMatches[m].Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены
                                                    stringoverallength0 += mc0[m].Value.Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены

                                                }
                                                //только если ячейка пустая
                                                TargetTranslationCell = thDataWork.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex];
                                                if (!failed && (forcevalue || TargetTranslationCell == null || string.IsNullOrEmpty(TargetTranslationCell as string)))
                                                {
                                                    //thDataWork.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTransCellValue;
                                                    TRow[TranslationColumnIndex] = InputTransCellValue;
                                                }
                                            }
                                        }
                                    }
                                    else //иначе, если в поле оригинала не было цифр, сравнить как обычно, два поля между собой 
                                    {
                                        if (Equals(TRow[iColumnIndex], InputTableOriginalCell)) //если поле Untrans елемента равно только что измененному
                                        {
                                            //thDataWork.THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationColumnIndex] = InputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                                            TRow[TranslationColumnIndex] = InputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
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

                if (THAutoSetSameTranslationForSimularData.ContainsKey(TRC))
                {
                    THAutoSetSameTranslationForSimularData.Remove(TRC);
                }
                THAutoSetSameTranslationForSimularIsBusy = false;
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
