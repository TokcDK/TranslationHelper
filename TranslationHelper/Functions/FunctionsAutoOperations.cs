﻿using System;
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
