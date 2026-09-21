using NLog;
using System;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class FixCells : RowBase
    {
        public FixCells()
        {
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            return CellFixes(rowData);
        }

        /// <summary>
        /// Maximum number of attempts when the rules collection changes while it is being iterated.
        /// </summary>
        private const int CellFixesMaxAttempts = 3;

        private bool CellFixes(RowBaseRowData rowData, int attempt = 0)
        {
            try
            {
                string cvalue = rowData.Translation;
                //не трогать строку перевода, если она пустая
                if (!string.IsNullOrEmpty(cvalue))
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
                    foreach (var patternReplacementPair in AppData.CellFixesRegexRules)
                    {
                        //читать правило и результат
                        rule = patternReplacementPair.Key;
                        result = patternReplacementPair.Value;


                        Regex regexrule;
                        MatchCollection mc;
                        try
                        {
                            //задать правило
                            regexrule = new Regex(rule);

                            //найти совпадение с заданным правилом в выбранной ячейке
                            mc = regexrule.Matches(cvalue);
                        }
                        catch (System.ArgumentException ex)
                        {
                            Logger.Warn("FixCells: Invalid regex:" + rule + "\r\nError:\r\n" + ex);
                            Logger.Warn("Invalid regex found. See " + THSettings.ApplicationLogName);
                            continue;
                        }

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
                                UiUpdater.ShowMessage(T._("Error in") + " TranslationHelperCellFixesRegexRules.txt" + Environment.NewLine + "Regex: " + rule);
                            }
                        }
                    }

                    //идея с извлечением строк как при переводе, только перед фиксом ячейки, чтобы обрабатывало только извлеченное
                    //здесь функция возвращения извлеченного
                    //cvalue = RestoreExtracted(cvalue, row[cind - 1] as string);

                    if (!Equals(rowData.Translation, cvalue))
                    {
                        //ProjectData.THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = cvalue;
                        rowData.Translation = cvalue;
                        return true;
                    }
                }
            }
            catch (System.InvalidOperationException) // in case of collection was changed exception when rules was changed in time of iteration
            {
                // retry fixes, but only a bounded number of times: the previous unbounded retry
                // recursed until the stack overflowed and killed the process
                if (attempt + 1 >= CellFixesMaxAttempts) return false;

                return CellFixes(rowData, attempt + 1);
            }
            return false;
        }
    }
}
