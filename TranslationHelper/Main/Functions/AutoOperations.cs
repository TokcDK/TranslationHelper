using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper.Main.Functions
{
    class AutoOperations
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
                            result = rules.ReadLine();

                            //проверить, есть ли правило и результат, если вдруг файле будет нечетное количество строк, по ошибке юзера
                            if (rule.Length == 0 || !result.Contains("$"))
                            {
                            }
                            else
                            {
                                ret = Regex.Replace(ret, rule, result);
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
                using (StreamReader rules = new StreamReader(Path.Combine(Application.StartupPath, "TranslationHelperCellFixesRegexRules.txt")))
                {
                    try
                    {
                        if (selectedonly)
                        {
                            //regex правило и результат из файла
                            string rule;
                            string result = string.Empty;
                            while (!rules.EndOfStream)
                            {
                                //читать правило и результат
                                rule = rules.ReadLine();
                                result = rules.ReadLine();

                                //проверить, есть ли правило и результат, если вдруг файле будет нечетное количество строк, по ошибке юзера
                                if (rule.Length == 0)
                                {
                                }
                                else
                                {
                                    //задать правило
                                    Regex regexrule = new Regex(rule);

                                    //найти совпадение с заданным правилом в выбранной ячейке
                                    MatchCollection mc = regexrule.Matches(THFilesElementsDataset.Tables[tind].Rows[rind][cind] + string.Empty);

                                    //перебрать все найденные совпадения
                                    foreach (Match m in mc)
                                    {
                                        //исправить значения по найденным совпадениям в выбранной ячейке
                                        THFilesElementsDataset.Tables[tind].Rows[rind][cind] = (THFilesElementsDataset.Tables[tind].Rows[rind][cind] + string.Empty).Replace(m.Value + string.Empty, regexrule.Replace(m.Value + string.Empty, result));
                                        //THFilesElementsDataset.Tables[tind].Rows[rind][cind] = Regex.Replace(THFilesElementsDataset.Tables[tind].Rows[rind][cind].ToString(),m.Value.ToString(), result);
                                    }
                                }
                            }
                        }
                        else
                        {
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
                                    selcellscnt[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
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

                            //regex правило и результат из файла
                            string rule;
                            string result = string.Empty;
                            while (!rules.EndOfStream)
                            {
                                //читать правило и результат
                                rule = rules.ReadLine();
                                result = rules.ReadLine();

                                //проверить, есть ли правило и результат, если вдруг файле будет нечетное количество строк, по ошибке юзера
                                if (rule.Length == 0)
                                {
                                }
                                else
                                {
                                    //задать правило
                                    Regex regexrule = new Regex(rule);

                                    if (selectedonly)
                                    {
                                        //LogToFile("6 THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());

                                        //найти совпадение с заданным правилом в выбранной ячейке
                                        MatchCollection mc = regexrule.Matches(THFilesElementsDataset.Tables[tind].Rows[rind][cind] + string.Empty);
                                        //перебрать все айденные совпадения
                                        foreach (Match m in mc)
                                        {
                                            //LogToFile("match=" + m.ToString() + ", result=" + regexrule.Replace(m.Value.ToString(), result), true);

                                            //исправить значения по найденным совпадениям в выбранной ячейке
                                            THFilesElementsDataset.Tables[tind].Rows[rind][cind] = (THFilesElementsDataset.Tables[tind].Rows[rind][cind] + string.Empty).Replace(m.Value + string.Empty, regexrule.Replace(m.Value + string.Empty, result));

                                            //LogToFile("7 Result THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());
                                        }
                                    }
                                    else
                                    {

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
                                            for (int i = 0; i < rowscount; i++)
                                            {
                                                if (method == "s")
                                                {
                                                    //индекс = первому из заданного списка выбранных индексов
                                                    rowindex = selcellscnt[i];
                                                }
                                                else
                                                {
                                                    //индекс с нуля и до последней строки
                                                    rowindex = i;
                                                }

                                                //LogToFile("5 selected i row index=" + i + ", value of THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "]=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind]);
                                                //не трогать строку перевода, если она пустая
                                                if ((THFilesElementsDataset.Tables[t].Rows[rowindex][cind] + string.Empty).Length == 0)
                                                {
                                                }
                                                else
                                                {
                                                    //LogToFile("6 THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());

                                                    //найти совпадение с заданным правилом в выбранной ячейке
                                                    MatchCollection mc = regexrule.Matches(THFilesElementsDataset.Tables[t].Rows[rowindex][cind] + string.Empty);
                                                    //перебрать все айденные совпадения
                                                    foreach (Match m in mc)
                                                    {
                                                        //LogToFile("match=" + m.ToString() + ", result=" + regexrule.Replace(m.Value.ToString(), result), true);

                                                        //исправить значения по найденным совпадениям в выбранной ячейке
                                                        THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = (THFilesElementsDataset.Tables[t].Rows[rowindex][cind] + string.Empty).Replace(m.Value + string.Empty, regexrule.Replace(m.Value + string.Empty, result));
                                                        //THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = Regex.Replace(THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString(), m.Value.ToString(), result);

                                                        //LogToFile("7 Result THFilesElementsDataset.Tables[" + t + "].Rows[" + rowindex + "][" + cind + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[rowindex][cind].ToString());
                                                    }
                                                }
                                            }
                                        }
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
        }

        public static void THAutoSetSameTranslationForSimular(DataSet THFilesElementsDataset, int InputTableIndex, int InputRowIndex, int InputCellIndex, bool forcerun = true, bool forcevalue = false)
        {
            int TranslationCellIndex = InputCellIndex + 1;
            var InputTableRow = THFilesElementsDataset.Tables[InputTableIndex].Rows[InputRowIndex];
            var InputTableOriginalCell = InputTableRow[InputCellIndex];
            var InputTableTranslationCell = InputTableRow[TranslationCellIndex];
            if (InputTableTranslationCell == null || string.IsNullOrEmpty(InputTableTranslationCell as string))
            {
            }
            else//Запускать сравнение только если ячейка имеет значение
            {
                //LogToFile("THFilesElementsDataset.Tables[tableind].Rows[rind][transcind]="+ THFilesElementsDataset.Tables[tableind].Rows[rind][transcind].ToString());
                //http://www.cyberforum.ru/csharp-beginners/thread244709.html
                string quote = "\"";
                string japanessymbols = "【|】|「|」";
                string pattern = @"((\d|\!|\?|\.|[|]|" + quote + "|" + japanessymbols + ")+)";
                Regex reg = new Regex(pattern); //reg равняется любым цифрам
                string inputorigcellvalue = RomajiKana.THFixDigits(InputTableOriginalCell as string);
                string inputtranscellvalue = RomajiKana.THFixDigits(InputTableTranslationCell as string);
                MatchCollection mc = reg.Matches(inputorigcellvalue); //присвоить mc совпадения в выбранной ячейке, заданные в reg, т.е. все цифры в поле untrans выбранной строки, если они есть.
                int mccount = mc.Count;

                int TableCount = THFilesElementsDataset.Tables.Count;
                for (int Tindx = 0; Tindx < TableCount; Tindx++) //количество файлов
                {
                    var Table = THFilesElementsDataset.Tables[Tindx];
                    var RowsCount = Table.Rows.Count;
                    //LogToFile("Table "+Tindx+" proceed");
                    for (int Rindx = 0; Rindx < RowsCount; Rindx++) //количество строк в каждом файле
                    {
                        var TRow = Table.Rows[Rindx];
                        var TCell = TRow[TranslationCellIndex];
                        if ((forcevalue && Rindx != InputRowIndex) || TCell == null || string.IsNullOrEmpty(TCell as string)) //Проверять только для пустых ячеек перевода
                        {
                            //LogToFile("THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString()=" + THFilesElementsDataset.Tables[i].Rows[y][transcind].ToString());
                            if (mccount > 0) //если количество совпадений в mc больше нуля, т.е. цифры были в поле untrans выбранной только что переведенной ячейки
                            {
                                string TempCell = TRow[InputCellIndex] + string.Empty;
                                string checkingorigcellvalue = RomajiKana.THFixDigits(TempCell);
                                MatchCollection mc0 = reg.Matches(TempCell); //mc0 равно значениям цифр ячейки под номером y в файле i
                                int mc0Count = mc0.Count;
                                if (mc0Count > 0) //если количество совпадений в mc0 больше нуля, т.е. цифры были в поле untrans проверяемой на совпадение ячейки
                                {
                                    string checkingorigcellvalueNoDigits = Regex.Replace(checkingorigcellvalue, pattern, string.Empty);
                                    string inputorigcellvalueNoDigits = Regex.Replace(inputorigcellvalue, pattern, string.Empty);

                                    //LogToFile("checkingorigcellvalue=\r\n" + checkingorigcellvalue + "\r\ninputorigcellvalue=\r\n" + inputorigcellvalue);
                                    //если поле перевода равно только что измененному во входной, без учета цифр
                                    if (Equals(checkingorigcellvalueNoDigits, inputorigcellvalueNoDigits) && mccount == mc0Count && IsAllMatchesInIdenticalPlaces(mc, mc0))
                                    {
                                        //инициализация основных целевого и входного массивов
                                        string[] inputorigmatches = new string[mccount];
                                        string[] targetorigmatches = new string[mccount];
                                        //присваивание цифр из совпадений в массивы, в основной входного и во временный целевого
                                        for (int r = 0; r < mccount; r++)
                                        {
                                            inputorigmatches[r] = RomajiKana.THFixDigits(mc[r].Value/*.Replace(mc[r].Value, mc[r].Value)*/);
                                            targetorigmatches[r] = RomajiKana.THFixDigits(mc0[r].Value/*.Replace(mc0[r].Value, mc0[r].Value)*/);
                                        }
                                        //также инфо о другом способе:
                                        //http://qaru.site/questions/41136/how-to-convert-matchcollection-to-string-array
                                        //там же че тести и for, ак у здесь меня - наиболее быстрый вариант

                                        //проверка для предотвращения ситуации с ошибкой, когда, например, строка "\{\V[11] \}万円手に入れた！" с японского будет переведена как "\ {\ V [11] \} You got 10,000 yen!" и число совпадений по числам поменется, т.к. 万 [man] переводится как 10000.
                                        if (reg.Matches(inputtranscellvalue).Count == mccount)
                                        {
                                            //string inputresult = Regex.Replace(inputtranscellvalue, pattern, "{{$1}}");//оборачивание цифры в {{}}, чтобы избежать ошибочных замен например замены 5 на 6 в значении, где есть 5 50
                                            string inputresult = inputtranscellvalue;//оборачивание цифры в {{}}, чтобы избежать ошибочных замен например замены 5 на 6 в значении, где есть 5 50

                                            MatchCollection tm = reg.Matches(inputresult);
                                            int startindex;
                                            int stringoverallength = 0;
                                            int stringlength;
                                            int stringoverallength0 = 0;
                                            //LogToFile("arraysize=" + arraysize + ", wrapped inputresult" + inputresult);
                                            for (int m = 0; m < mccount; m++)
                                            {
                                                //LogToFile("inputorigmatches[" + m + "]=" + inputorigmatches[m] + ", targetorigmatches[" + m + "]=" + targetorigmatches[m] + ", pre result[" + m + "]=" + inputresult);
                                                //inputresult = inputresult.Replace("{{" + inputorigmatches[m] + "}}", targetorigmatches[m]);

                                                //замена символа путем удаления на позиции и вставки нового:https://stackoverflow.com/questions/5015593/how-to-replace-part-of-string-by-position
                                                startindex = tm[m].Index - stringoverallength + stringoverallength0;//отнять предыдущее число и заменить новым числом, для корректировки индекса
                                                stringlength = inputorigmatches[m].Length;
                                                stringoverallength += stringlength;//запомнить общую длину заменяемых символов, для коррекции индекса позиции для замены
                                                inputresult = inputresult.Remove(startindex, stringlength).Insert(startindex, targetorigmatches[m]);
                                                stringoverallength0 += targetorigmatches[m].Length;//запомнить общую длину заменяющих символов, для коррекции индекса позиции для замены
                                                                                                   //inputresult = inputresult.Replace("{{"+ mc[m].Value + "}}", mc0[m].Value);
                                                                                                   //LogToFile("result[" + m + "]=" + inputresult);
                                            }
                                            //только если ячейка пустая
                                            TCell = THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationCellIndex];
                                            if (forcevalue || TCell == null || string.IsNullOrEmpty(TCell as string))
                                            {
                                                THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationCellIndex] = inputresult;
                                            }
                                        }
                                    }
                                }
                            }
                            else //иначе, если в поле оригинала не было цифр, сравнить как обычно, два поля между собой 
                            {
                                if (Equals(TRow[InputCellIndex], InputTableOriginalCell)) //если поле Untrans елемента равно только что измененному
                                {
                                    THFilesElementsDataset.Tables[Tindx].Rows[Rindx][TranslationCellIndex] = InputTableTranslationCell; //Присвоить полю Trans элемента значение только что измененного элемента, учитываются цифры при замене перевода      
                                }
                            }
                        }
                    }
                }
            }
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
        public static void StringCaseMorph(DataGridView THFileElementsDataGridView, int variant)
        {
            int THFileElementsDataGridViewSelectedCellsCount = THFileElementsDataGridView.SelectedCells.Count;
            if (THFileElementsDataGridViewSelectedCellsCount > 0)
            {
                try
                {
                    for (int i = 0; i < THFileElementsDataGridViewSelectedCellsCount; i++)
                    {
                        int rind = THFileElementsDataGridView.SelectedCells[i].RowIndex;
                        //int corigind = THFileElementsDataGridView.Columns["Original"].Index;//2-поле untrans
                        int ctransind = THFileElementsDataGridView.Columns["Translation"].Index;//2-поле untrans
                        if (THFileElementsDataGridView.SelectedCells[i].Value == null)
                        {
                        }
                        else
                        {
                            if (variant == 0)//lowercase
                            {
                                THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value = (THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value + string.Empty).ToLowerInvariant();
                            }
                            else if (variant == 1)//Uppercase
                            {
                                //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                                string s = THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value as string;
                                THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value = char.ToUpper(s[0]) + s.Substring(1);
                            }
                            else if (variant == 2)//UPPERCASE
                            {
                                THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value = (THFileElementsDataGridView.Rows[rind].Cells[ctransind].Value as string).ToUpperInvariant();
                            }
                            else
                            {
                                break;
                            }

                        }

                    }
                }
                catch
                {
                }
            }

        }

    }
}
