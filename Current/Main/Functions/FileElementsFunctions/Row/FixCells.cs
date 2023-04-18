using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class FixCells : RowBase
    {
        public FixCells()
        {
        }

        protected override bool Apply()
        {
            return CellFixes();
        }

        private bool CellFixes()
        {
            try
            {
                string cvalue = Translation;
                //не трогать строку перевода, если она пустая
                if (cvalue.Length > 0)
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
                           _log.LogToFile("FixCells: Invalid regex:" + rule + "\r\nError:\r\n" + ex);
                            AppData.Main.ProgressInfo(true, "Invalid regex found. See " + THSettings.ApplicationLogName);
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
                                MessageBox.Show(T._("Error in") + " TranslationHelperCellFixesRegexRules.txt" + Environment.NewLine + "Regex: " + rule);
                            }
                        }
                    }

                    //идея с извлечением строк как при переводе, только перед фиксом ячейки, чтобы обрабатывало только извлеченное
                    //здесь функция возвращения извлеченного
                    //cvalue = RestoreExtracted(cvalue, row[cind - 1] as string);

                    if (!Equals(Translation, cvalue))
                    {
                        //ProjectData.THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = cvalue;
                        Translation = cvalue;
                        return true;
                    }
                }
            }
            catch (System.InvalidOperationException) // in case of collection was changed exception when rules was changed in time of iteration
            {
                // retry fixes
                return CellFixes();
            }
            return false;
        }
    }
}
