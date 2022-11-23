using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions
{
    internal class FunctionRules
    {
        internal static void ReloadTranslationRegexRules()
        {
            // make temp dict
            var ProjectDataTranslationRegexRules = new Dictionary<string, string>();

            //если файл с правилами существует
            if (System.IO.File.Exists(THSettings.TranslationRegexRulesFilePath))
            {
                //читать файл с правилами
                using (var rules = new StreamReader(THSettings.TranslationRegexRulesFilePath))
                {
                    //regex правило и результат из файла
                    var regexPattern = string.Empty;
                    var regexReplacement = string.Empty;
                    var ReadRule = true;
                    while (!rules.EndOfStream)
                    {
                        try
                        {
                            //читать правило и результат
                            if (ReadRule)
                            {
                                regexPattern = rules.ReadLine();
                                if (string.IsNullOrWhiteSpace(regexPattern) || regexPattern.TrimStart().StartsWith(";"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                                continue;
                            }
                            else
                            {
                                regexReplacement = rules.ReadLine();
                                if (string.IsNullOrWhiteSpace(regexPattern) || regexReplacement.TrimStart().StartsWith(";") || !FunctionsString.IsStringAContainsStringB(regexReplacement, "$"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                            }

                            ProjectDataTranslationRegexRules.TryAdd(regexPattern, regexReplacement);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            // re:Set rules
            AppData.TranslationRegexRules = ProjectDataTranslationRegexRules;
        }

        internal static void ReloadCellFixesRegexRules()
        {
            var ProjectDataCellFixesRegexRules = new Dictionary<string, string>();

            //если файл с правилами существует
            if (System.IO.File.Exists(THSettings.CellFixesRegexRulesFilePath))
            {
                //читать файл с правилами
                using (var rules = new StreamReader(THSettings.CellFixesRegexRulesFilePath))
                {
                    //regex правило и результат из файла
                    var regexPattern = string.Empty;
                    var regexReplacement = string.Empty;
                    var ReadRule = true;
                    while (!rules.EndOfStream)
                    {
                        try
                        {
                            //читать правило и результат
                            if (ReadRule)
                            {
                                regexPattern = rules.ReadLine();
                                if (string.IsNullOrEmpty(regexPattern) || regexPattern.TrimStart().StartsWith(";"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                                continue;
                            }
                            else
                            {
                                regexReplacement = rules.ReadLine();
                                if (string.IsNullOrEmpty(regexPattern) || regexReplacement.TrimStart().StartsWith(";"))//игнорировать комментарии
                                {
                                    continue;
                                }
                                ReadRule = !ReadRule;
                            }

                            ProjectDataCellFixesRegexRules.TryAdd(regexPattern, regexReplacement);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            // re:Set rules
            AppData.CellFixesRegexRules = ProjectDataCellFixesRegexRules;
        }
    }
}
