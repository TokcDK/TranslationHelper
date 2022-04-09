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
        public static string THExtractTextForTranslation(string input)
        {
            foreach (var PatternReplacementPair in ProjectData.TranslationRegexRules)
            {
                if (Regex.IsMatch(input, PatternReplacementPair.Key))
                {
                    //new FunctionsLogs().LogToFile("applied translation rule: "+ PatternReplacementPair.Key);
                    return Regex.Replace(input, PatternReplacementPair.Key, PatternReplacementPair.Value);
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
        public static string[] THExtractTextForTranslationSplit(string input)
        {
            foreach (var PatternReplacementPair in ProjectData.TranslationRegexRules)
            {
                if (Regex.IsMatch(input, PatternReplacementPair.Key))
                {
                    MatchCollection Results = Regex.Matches(PatternReplacementPair.Value, @"\$([0-9]{1,2}|(\{.+\}))");
                    if (Results.Count > 0)
                    {
                        List<string> Ret = new List<string>
                        {
                            PatternReplacementPair.Key,
                            PatternReplacementPair.Value
                        };
                        Dictionary<string, string> FoundValues = new Dictionary<string, string>();
                        foreach (Match Result in Results)
                        {
                            if (FoundValues.ContainsKey(Result.Value))
                            {
                                continue;
                            }
                            var candidate = Regex.Replace(input, PatternReplacementPair.Key, Result.Value);
                            if (!string.IsNullOrEmpty(candidate))
                            {
                                FoundValues.Add(Result.Value, candidate);
                            }
                        }
                        foreach (var val in FoundValues.Values)
                        {
                            Ret.Add(val);
                        }
                        return Ret.ToArray();
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
        public static string THFixCells(this string cvalue)
        {
            string rule;
            string result;

            foreach (var PatternReplacementPair in ProjectData.CellFixesRegexRules)
            {
                //читать правило и результат
                rule = PatternReplacementPair.Key;
                result = PatternReplacementPair.Value;

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
        /// <param name="Method"></param>
        /// <param name="cind"></param>
        /// <param name="tind"></param>
        /// <param name="rind"></param>
        /// <param name="selectedonly"></param>
        public static void THFixCells(string Method, int cind, int tind, int rind = 0, bool forceApply = false)//cind - индекс столбца перевода, задан до старта потока
        {
            try
            {
                if (ProjectData.CellFixesRegexRules.Count == 0)
                {
                    return;
                }

                //индекс столбца перевода, таблицы и массив индексов для варианта с несколькими выбранными ячейками
                //int cind = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Columns[THSettings.TranslationColumnName()].Ordinal;//-поле untrans;//-поле untrans
                int initialtableindex = 0;
                int[] selcellscnt;

                //method
                //a - All
                //t - Table
                //s - Selected

                if (Method == "s")
                {
                    //cind = THFileElementsDataGridView.Columns[THSettings.TranslationColumnName()].Index;//-поле untrans                            
                    initialtableindex = tind;// THFilesListBox.SelectedIndex;//установить индекс таблицы на выбранную в listbox
                    selcellscnt = FunctionsTable.GetDGVRowIndexsesInDataSetTable();
                }
                else if (Method == "t")
                {
                    initialtableindex = tind;// THFilesListBox.SelectedIndex;//установить индекс таблицы на выбранную в listbox
                                             //cind = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].Columns[THSettings.TranslationColumnName()].Ordinal;
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
                    tablescount = ProjectData.FilesContent.Tables.Count;//все таблицы в dataset
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
                        rowscount = ProjectData.FilesContent.Tables[t].Rows.Count;
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
                        var row = ProjectData.FilesContent.Tables[t].Rows[rowindex];
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
                            foreach (var PatternReplacementPair in ProjectData.CellFixesRegexRules)
                            {
                                //читать правило и результат
                                rule = PatternReplacementPair.Key;
                                result = PatternReplacementPair.Value;

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
    }
}
