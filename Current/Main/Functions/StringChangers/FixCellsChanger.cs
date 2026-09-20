using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.StringChangers
{
    class FixCellsChanger : StringChangerBase
    {
        public FixCellsChanger()
        {
        }

        /// <summary>
        /// Rules, keyed by their pattern. A pattern is an immutable string and the rule collection is
        /// replaced as a whole when it is reloaded, so a cached entry can never become stale. The cache
        /// exists because this method used to build a new <see cref="Regex"/> for every rule of every
        /// cell.
        /// </summary>
        static readonly ConcurrentDictionary<string, Regex> RegexCache = new ConcurrentDictionary<string, Regex>();

        /// <summary>
        /// Maximum number of attempts when the rules collection changes while it is being iterated.
        /// </summary>
        const int CellFixesMaxAttempts = 3;

        internal override string Description => $"{nameof(FixCellsChanger)}";


        internal override string Change(string inputString, object extraData)
        {
            return CellFixes(extraData as string, inputString);
        }

        private string CellFixes(string original, string translation, int attempt = 0)
        {
            //не трогать строку перевода, если она пустая
            if (string.IsNullOrEmpty(translation))
            {
                return translation;
            }

            try
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

                string cvalue = translation;
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
                        regexrule = RegexCache.GetOrAdd(rule, r => new Regex(r, RegexOptions.Compiled));

                        //найти совпадение с заданным правилом в выбранной ячейке
                        mc = regexrule.Matches(cvalue);
                    }
                    catch (ArgumentException ex)
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
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Failed to apply the cell fix rule '{0}'", rule);
                            MessageBox.Show(T._("Error in") + " TranslationHelperCellFixesRegexRules.txt" + Environment.NewLine + "Regex: " + rule);
                        }
                    }
                }

                //идея с извлечением строк как при переводе, только перед фиксом ячейки, чтобы обрабатывало только извлеченное
                //здесь функция возвращения извлеченного
                //cvalue = RestoreExtracted(cvalue, row[cind - 1] as string);

                if (!string.Equals(translation, cvalue))
                {
                    //ProjectData.THFilesElementsDataset.Tables[t].Rows[rowindex][cind] = cvalue;
                    return cvalue;
                }
                return translation;
            }
            catch (InvalidOperationException) // in case of collection was changed exception when rules was changed in time of iteration
            {
                // retry fixes, but only a bounded number of times: the previous unbounded retry
                // recursed until the stack overflowed and killed the process
                if (attempt + 1 >= CellFixesMaxAttempts) return translation;

                return CellFixes(original, translation, attempt + 1);
            }
        }
    }
}
