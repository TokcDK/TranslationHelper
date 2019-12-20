using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TranslationHelper.Main.Functions
{
    class FunctionsAutoOperations
    {
        public static string THExtractTextForTranslation(string input)
        {
            string ret = input;
            //если файл с правилами существует
            if (File.Exists(Path.Combine(Application.StartupPath, "TranslationHelperTranslationRegexRules.txt")))
            {
                //читать файл с правилами
                using (StreamReader rules = new StreamReader(Path.Combine(Application.StartupPath, "TranslationHelperTranslationRegexRules.txt")))
                {
                    try
                    {
                        //regex правило и результат из файла
                        string rule;
                        string result = string.Empty;
                        while (!rules.EndOfStream)
                        {
                            //читать правило и результат
                            rule = rules.ReadLine();
                            if (rule.Length == 0 || rule.StartsWith(";"))//игнорировать комментарии
                            {
                                continue;
                            }
                            result = rules.ReadLine();

                            //проверить, есть ли правило и результат, если вдруг файле будет нечетное количество строк, по ошибке юзера
                            if (result.Contains("$"))
                            {
                                ret = Regex.Replace(ret, rule, result);
                                //ret = Regex.Replace(ret, rule, Regex.Replace(result, @"(\$\d+)","$1{{AND}}"));//new for regex split rules

                                //задать правило
                                //Regex regexrule = new Regex(rule);

                                ////найти совпадение с заданным правилом в выбранной ячейке
                                //MatchCollection mc = regexrule.Matches(input);

                                ////перебрать все найденные совпадения
                                //foreach (Match m in mc)
                                //{
                                //    //исправить значения по найденным совпадениям в выбранной ячейке
                                //    //ret = ret.Replace(m.Value.ToString(), regexrule.Replace(m.Value.ToString(), result));
                                //    ret = Regex.Replace(ret, rule, result);
                                //}
                                if (ret == input)
                                {
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //LogToFile(string.Empty,true);
                    }
                    catch
                    {
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Исправления формата спецсимволов в заданной ячейке перевода
        /// Для выбранных ячеек, таблицы или для всех значений задать:
        /// method:
        /// "a" - All
        /// "t" - Table
        /// "s" - Selected
        /// ..а также cind - индекс колонки, где ячейки перевода и tind - индекс таблицы, для вариантов Table и Selected
        /// Для одной выбранной ячейки, когда, например, определенная обрабатывается в коде, задать tind, cind и rind, а также true для onselectedonly
        /// </summary>
        /// <param name="method"></param>
        /// <param name="cind"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <param name="selectedonly"></param>
        public static void THFixCells(DataSet THFilesElementsDataset, DataGridView THFileElementsDataGridView, string method, int cind, int tind, int rind = 0, bool selectedonly = false)//cind - индекс столбца перевода, задан до старта потока
        {
            //если файл с правилами существует
            if (File.Exists(Path.Combine(Application.StartupPath, "TranslationHelperCellFixesRegexRules.txt")))
            {
                //читать файл с правилами
                string[] rules = File.ReadAllLines(Path.Combine(Application.StartupPath, "TranslationHelperCellFixesRegexRules.txt"));
                try
                {
                    var rulesLength = rules.Length;
                    if (rulesLength < 2)
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
                        selcellscnt = new int[THFileElementsDataGridView.SelectedCells.Count];//создать массив длинной числом выбранных ячеек
                        for (int i = 0; i < selcellscnt.Length; i++) //записать индексы всех выбранных ячеек
                        {
                            selcellscnt[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesElementsDataset, THFileElementsDataGridView, tind, THFileElementsDataGridView.SelectedCells[i].RowIndex);
                        }
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
                        tablescount = THFilesElementsDataset.Tables.Count;//все таблицы в dataset
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
                            rowscount = THFilesElementsDataset.Tables[t].Rows.Count;
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
                            var row = THFilesElementsDataset.Tables[t].Rows[rowindex];
                            string cvalue = row[cind] + string.Empty;
                            //не трогать строку перевода, если она пустая
                            if (cvalue.Length > 0 && cvalue != row[cind - 1] as string)
                            {
                                //Hardcoded rules
                                cvalue = FunctionsString.FixForRPGMAkerQuotationInSomeStrings(row);

                                //LogToFile("6 THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());

                                //regex правило и результат из файла
                                string rule;
                                string result;
                                for (int i = 0; i < rulesLength; i++)
                                {
                                    //игнорировать комментарии и пустые строки
                                    while (rules[i].Length == 0 || rules[i].StartsWith(";"))
                                    {
                                        i++;
                                    }

                                    //читать правило и результат
                                    rule = rules[i];
                                    i++;
                                    //проверка, если строки закончились
                                    if (i == rulesLength)
                                    {
                                        break;
                                    }
                                    result = rules[i];

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

                                if (!Equals(THFilesElementsDataset.Tables[t].Rows[rowindex][cind], cvalue))
                                {
                                    THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = cvalue;
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
        public static void THAutoSetSameTranslationForSimular(DataSet THFilesElementsDataset, int InputTableIndex, int InputRowIndex, int InputCellIndex, bool ForceSetValue = false)
        {
            if (THAutoSetSameTranslationForSimularIsBusy)
            {
                return;
            }
            THAutoSetSameTranslationForSimularIsBusy = true;

            //re-set input variables to prevent break of work while concurent execution
            bool forcevalue = ForceSetValue;
            int iTableIndex = InputTableIndex;
            int iRowIndex = InputRowIndex;
            int iCellIndex = InputCellIndex;

            var InputTableRow = THFilesElementsDataset.Tables[iTableIndex].Rows[iRowIndex];
            var InputTableOriginalCell = InputTableRow[iCellIndex];
            int TranslationCellIndex = iCellIndex + 1;
            var InputTableTranslationCell = InputTableRow[TranslationCellIndex];
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

                //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменяется, т.к. 万 [man] переводится как 10000.
                if (Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese" && Regex.Matches(InputTransCellValue, @"\d+").Count != Regex.Matches(InputOrigCellValue, @"\d+").Count)
                {
                    THAutoSetSameTranslationForSimularIsBusy = false;
                    return;
                }

                MatchCollection mc = reg.Matches(InputOrigCellValue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
                int mccount = mc.Count;

                int TableCount = THFilesElementsDataset.Tables.Count;
                for (int Tindx = 0; Tindx < TableCount; Tindx++) //количество файлов
                {
                    var Table = THFilesElementsDataset.Tables[Tindx];
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
                        var TargetOriginallCell = TRow[iCellIndex];
                        var TargetTranslationCell = TRow[TranslationCellIndex];
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
                                    string TargetTransCellValueWithRemovedPatternMatches = Regex.Replace(TargetTranslationCellString, regexPattern, string.Empty);
                                    string InputTransCellValueWithRemovedPatternMatches = Regex.Replace(InputTransCellValue, regexPattern, string.Empty);

                                    //Если значение ячеек перевода без паттернов равны, идти дальше
                                    if (TargetTransCellValueWithRemovedPatternMatches == InputTransCellValueWithRemovedPatternMatches)
                                    {
                                        continue;
                                    }

                                    string TargetOrigCellValueWithRemovedPatternMatches = Regex.Replace(TargetOriginallCellString, regexPattern, string.Empty);
                                    string InputOrigCellValueWithRemovedPatternMatches = Regex.Replace(InputOrigCellValue, regexPattern, string.Empty);

                                    //LogToFile("checkingorigcellvalue=\r\n" + checkingorigcellvalue + "\r\ninputorigcellvalue=\r\n" + inputorigcellvalue);
                                    //если поле перевода равно только что измененному во входной, без учета цифр
                                    if ((TargetOrigCellValueWithRemovedPatternMatches == InputOrigCellValueWithRemovedPatternMatches) && mccount == mc0Count && IsAllMatchesInIdenticalPlaces(mc, mc0))
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

                                        MatchCollection tm = reg.Matches(InputTransCellValue);
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

                                            //LogToFile("inputorigmatches[" + m + "]=" + inputorigmatches[m] + ", targetorigmatches[" + m + "]=" + targetorigmatches[m] + ", pre result[" + m + "]=" + inputresult);
                                            //inputresult = inputresult.Replace("{{" + inputorigmatches[m] + "}}", targetorigmatches[m]);
                                            try
                                            {
                                                //замена символа путем удаления на позиции и вставки нового:https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
                                                startindex = tm[m].Index - stringoverallength + stringoverallength0;//отнять предыдущее число и заменить новым числом, для корректировки индекса
                                                                                                                    //if (startindex > -1 ??)
                                                                                                                    //{
                                                                                                                    //}
                                                                                                                    //else
                                                                                                                    //{
                                                                                                                    //    //была ошибка когда индекс был -1. Добавил проверку индекса и
                                                                                                                    //    failed = true;
                                                                                                                    //}
                                                                                                                    //string g1 = tm[m].Value;//i
                                                                                                                    //string g2 = targetOrigMatches[m];//t
                                                                                                                    //string g3 = inputOrigMatches[m];//i
                                                                                                                    //string g4 = mc[m].Value;//i
                                                                                                                    //string g5 = mc0[m].Value;//t
                                                                                                                    //stringlength = inputOrigMatches[m].Length;
                                                stringlength = tm[m].Value.Length;
                                                stringoverallength += stringlength;//запомнить общую длину заменяемых символов, для коррекции индекса позиции для замены

                                                //InputTransCellValue = InputTransCellValue.Remove(startindex, stringlength).Insert(startindex, targetOrigMatches[m]);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."
                                                InputTransCellValue = InputTransCellValue.Remove(startindex, stringlength).Insert(startindex, mc0[m].Value);//Исключение - startindex = [Данные недоступны. Доступные данные IntelliTrace см. в окне "Локальные переменные"] "Индекс и показание счетчика должны указывать на позицию в строке."

                                                //stringoverallength0 += targetOrigMatches[m].Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены
                                                stringoverallength0 += mc0[m].Value.Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены
                                                                                           //inputresult = inputresult.Replace("{{"+ mc[m].Value + "}}", mc0[m].Value);
                                                                                           //LogToFile("result[" + m + "]=" + inputresult);
                                            }
                                            catch (Exception ex)
                                            {
                                                //была ошибка с startindex, добавлено для поимки исключения
                                                string ggg = ex.ToString()
                                                    + Environment.NewLine + "m=" + m
                                                    + Environment.NewLine + "tm[m].Index=" + tm[m].Index
                                                    + Environment.NewLine + "stringoverallength=" + stringoverallength
                                                    + Environment.NewLine + "stringoverallength0=" + stringoverallength0;
                                                FileWriter.WriteData(Path.Combine(Application.StartupPath, "Error.log"), ggg);
                                                MessageBox.Show("AutoSameValueMethod ERROR: " + ggg);
                                                failed = true;
                                                break;
                                            }
                                        }
                                        //только если ячейка пустая
                                        TargetTranslationCell = THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationCellIndex];
                                        if (!failed && (forcevalue || TargetTranslationCell == null || string.IsNullOrEmpty(TargetTranslationCell as string)))
                                        {
                                            THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationCellIndex] = InputTransCellValue;
                                        }
                                    }
                                }
                            }
                            else //иначе, если в поле оригинала не было цифр, сравнить как обычно, два поля между собой 
                            {
                                if (Equals(TRow[iCellIndex], InputTableOriginalCell)) //если поле Untrans елемента равно только что измененному
                                {
                                    THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationCellIndex] = InputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                                }
                            }
                        }
                    }
                }
            }
            THAutoSetSameTranslationForSimularIsBusy = false;
        }

        public static bool IsAllMatchesInIdenticalPlaces(MatchCollection mc, MatchCollection mc0)
        {
            try
            {
                //int startindex;
                int overallength = 0;
                //int startindex0;
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
                    //string mvalue = mc[m].Value;
                    //string mvalue0 = mc0[m].Value;
                    //int i = first.IndexOf(mvalue, startindex);
                    //int i0 = second.IndexOf(mvalue0, startindex0);
                    //if (i - overallength == i0 - overallength0)
                    //{
                    //    int l = mvalue.Length;
                    //    startindex = i+ l;
                    //    overallength+= l;
                    //    int l0 = mvalue0.Length;
                    //    startindex0 = i0 + l0;
                    //    overallength0 += l0;
                    //}
                    //else
                    //{
                    //    return false;
                    //}
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Changing string to uppercase(first char or all) or lowercase.
        /// variant 0 - lowercase / 
        /// variant 1 = Uppercase / 
        /// variant 2 = UPPERCASE / 
        /// </summary>
        /// <param name="THFileElementsDataGridView"></param>
        /// <param name="variant"></param>
        public static void StringCaseMorph(DataSet THFilesElementsDataset, int TableIndex, DataGridView THFileElementsDataGridView, int variant, bool All = false)
        {
            if (THFilesElementsDataset == null || variant > 2 || (!All && (TableIndex == -1 || THFileElementsDataGridView == null)))
            {
                return;
            }

            int ctransind = THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
            int corigind = THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
            if (!All)
            {
                int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.SelectedCells.Count;
                if (THFileElementsDataGridViewSelectedCellsCount > 0)
                {
                    try
                    {
                        int[] indexes = new int[THFileElementsDataGridViewSelectedCellsCount];
                        for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                        {
                            int rind = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                            indexes[i] = FunctionsTable.GetDGVSelectedRowIndexInDatatable(THFilesElementsDataset, THFileElementsDataGridView, TableIndex, rind);
                        }
                        foreach (var rindex in indexes)
                        {
                            var DSOrigCell = THFilesElementsDataset.Tables[TableIndex].Rows[rindex][corigind] + string.Empty;
                            var DSTransCell = THFilesElementsDataset.Tables[TableIndex].Rows[rindex][ctransind] + string.Empty;
                            if (!string.IsNullOrWhiteSpace(DSTransCell) && DSTransCell != DSOrigCell)
                            {
                                THFilesElementsDataset.Tables[TableIndex].Rows[rindex][ctransind] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                int THFilesElementsDatasetTablesCount = THFilesElementsDataset.Tables.Count;
                for (int tindex = 0; tindex < THFilesElementsDatasetTablesCount; tindex++)
                {
                    int THFilesElementsDatasetTableRowsCount = THFilesElementsDataset.Tables[tindex].Rows.Count;
                    for (int rindex = 0; rindex < THFilesElementsDatasetTableRowsCount; rindex++)
                    {
                        var DSOrigCell = THFilesElementsDataset.Tables[tindex].Rows[rindex][corigind] + string.Empty;
                        var DSTransCell = THFilesElementsDataset.Tables[tindex].Rows[rindex][ctransind] + string.Empty;
                        if (!string.IsNullOrWhiteSpace(DSTransCell) && DSTransCell != DSOrigCell)
                        {
                            THFilesElementsDataset.Tables[tindex].Rows[rindex][ctransind] = ChangeRegistryCaseForTheCell(DSTransCell, variant);
                        }

                    }
                }
            }
        }

        private static string ChangeRegistryCaseForTheCell(string DSTransCell, int variant)
        {
            switch (variant)
            {
                case 0:
                    //lowercase
#pragma warning disable CA1308 // Нормализуйте строки до прописных букв
                    return DSTransCell.ToLowerInvariant();
#pragma warning restore CA1308 // Нормализуйте строки до прописных букв
                case 1:
                    //Uppercase
                    //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                    return stringToUpper(DSTransCell);
                case 2:
                    //UPPERCASE
                    return DSTransCell.ToUpperInvariant();
                default:
                    return DSTransCell;
            }
        }

        private static string stringToUpper(string DSTransCell)
        {
            if (char.IsLetter(DSTransCell[0]))
            {
                return char.ToUpper(DSTransCell[0], CultureInfo.InvariantCulture) + DSTransCell.Substring(1);
            }
            else
            {
                int DSTransCellLength = DSTransCell.Length;
                for (int c = 0; c < DSTransCellLength; c++)
                {
                    char Char = DSTransCell[c];
                    if (char.IsWhiteSpace(Char) || char.IsPunctuation(Char))
                    {
                    }
                    else
                    {
                        return DSTransCell.Substring(0, c) + char.ToUpper(DSTransCell[c], CultureInfo.InvariantCulture) + (c == DSTransCellLength - 1 ? string.Empty : DSTransCell.Substring(c + 1));
                    }
                }

                return DSTransCell;
            }
        }

        /// <summary>
        /// Returns length of longest line in string
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public static int GetLongestLineLength(string Line)
        {
            int ReturnLength = 0;
            string[] sublines = Line.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
            int sublinesLength = sublines.Length;
            if (sublinesLength > 1)
            {
                for (int N = 0; N < sublinesLength; N++)
                {
                    int sublinesNLength = sublines[N].Length;
                    if (sublinesNLength > 0 && sublinesNLength > ReturnLength)
                    {
                        ReturnLength = sublinesNLength;
                    }
                }
            }
            else
            {
                ReturnLength = Line.TrimEnd().Length;
            }
            return ReturnLength;
        }

        public static string SplitMultiLineIfBeyondOfLimit(string Line, int Limit)
        {
            //StringBuilder ReturnLine = new StringBuilder();
            string[] sublines = Line.Split(new string[2] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            int sublinesLength = sublines.Length;
            List<string> Lines = new List<string>();
            if (sublinesLength > 1)
            {
                bool lastLineWasSplitted = false;
                for (int N = 0; N < sublinesLength; N++)
                {
                    string subline = sublines[N];
                    if (lastLineWasSplitted && subline.Length > 0)
                    {
                        //lastLineWasSplitted значит, что соединять только когда предыдущая была разделена, если нет, не нужно трогать порядок переноса из оригинала
                        //когда не первая строка, соединяем эту строку с последним элементом в Lines и удаляем последний элемент, обрабатываем такую новую строку
                        int LasLineIndex = Lines.Count - 1;
                        if (Lines[LasLineIndex].Length > 0)
                        {
                            subline = Lines[LasLineIndex] + subline;
                            Lines.RemoveAt(LasLineIndex);
                        }
                    }
                    int i = 0;
                    lastLineWasSplitted = false;
                    foreach (var line in GetSplittedLine(subline, Limit).SplitToLines())
                    {
                        i++;
                        Lines.Add(line);
                    }

                    if (i > 1)
                    {
                        //если i>1 значит строка была разделена
                        lastLineWasSplitted = true;
                    }
                    //if (N == sublinesLength - 1)
                    //{
                    //    foreach(var line in GetSplittedLine(subline, Limit).SplitToLines())
                    //    {
                    //        Lines.Add(line);
                    //    }
                    //    ReturnLine.Append(string.Join(Environment.NewLine, Lines));
                    //}
                    //else
                    //{
                    //    ReturnLine.AppendLine(GetSplittedLine(subline, Limit));
                    //}
                }
            }
            else
            {
                return GetSplittedLine(Line, Limit);
            }

            return string.Join(Environment.NewLine, Lines); //ReturnLine.ToString();
        }

        private static string GetSplittedLine(string Line, int Limit)
        {
            if (Line.Length == 0 || Line.Length <= Limit)
            {
                return Line;
            }
            return string.Join(FunctionsString.IsStringAContainsStringB(Properties.Settings.Default.THSelectedSourceType, "RPG Maker MV") ? "\\n " : Environment.NewLine, SplitLineIfBeyondOfLimit(Line, Limit));
        }

        public static string[] SplitLineIfBeyondOfLimit(string text, int max)
        {
            //https://ru.stackoverflow.com/questions/707937/c-%D0%BF%D0%B5%D1%80%D0%B5%D0%BD%D0%BE%D1%81-%D1%81%D0%BB%D0%BE%D0%B2-%D0%B2-%D1%81%D1%82%D1%80%D0%BE%D0%BA%D0%B5-%D1%81-%D1%80%D0%B0%D0%B7%D0%B1%D0%B8%D0%B2%D0%BA%D0%BE%D0%B9-%D0%BD%D0%B0-%D0%BE%D0%BF%D1%80%D0%B5%D0%B4%D0%B5%D0%BB%D0%B5%D0%BD%D0%BD%D1%83%D1%8E-%D0%B4%D0%BB%D0%B8%D0%BD%D1%83
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }
    }
}
