using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions
{
    internal class FunctionRules
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static void ReloadTranslationRegexRules()
        {
            // re:Set rules
            AppData.TranslationRegexRules = LoadRegexRules(THSettings.TranslationRegexRulesFilePath, requireDollarPlaceholder: true);
        }

        internal static void ReloadCellFixesRegexRules()
        {
            // re:Set rules
            AppData.CellFixesRegexRules = LoadRegexRules(THSettings.CellFixesRegexRulesFilePath, requireDollarPlaceholder: false);
        }

        /// <summary>
        /// Reads a rules file where every rule occupies two consecutive lines: the regex pattern
        /// and its replacement.
        /// </summary>
        /// <param name="rulesFilePath">File to read. When it does not exist an empty rule set is returned.</param>
        /// <param name="requireDollarPlaceholder">
        /// When true a replacement line is only accepted when it contains a '$' placeholder.
        /// </param>
        private static Dictionary<string, string> LoadRegexRules(string rulesFilePath, bool requireDollarPlaceholder)
        {
            var rulesDictionary = new Dictionary<string, string>();

            //если файл с правилами существует
            if (!File.Exists(rulesFilePath))
            {
                return rulesDictionary;
            }

            //читать файл с правилами
            using (var rules = new StreamReader(rulesFilePath))
            {
                //regex правило и результат из файла
                var regexPattern = string.Empty;
                var readPattern = true;
                while (!rules.EndOfStream)
                {
                    try
                    {
                        //читать правило
                        if (readPattern)
                        {
                            regexPattern = rules.ReadLine();
                            if (string.IsNullOrWhiteSpace(regexPattern) || regexPattern.TrimStart().StartsWith(";"))//игнорировать комментарии
                            {
                                continue;
                            }

                            readPattern = false;
                            continue;
                        }

                        //читать результат
                        //(the old code also tested regexPattern here, but it is always non blank at
                        //this point, so that test could never be true)
                        var regexReplacement = rules.ReadLine();
                        if (regexReplacement.TrimStart().StartsWith(";")//игнорировать комментарии
                            || (requireDollarPlaceholder && !FunctionsString.IsStringAContainsStringB(regexReplacement, "$")))
                        {
                            continue;
                        }

                        readPattern = true;
                        rulesDictionary.TryAdd(regexPattern, regexReplacement);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("Failed to read a rule from {0}. Error: {1}", rulesFilePath, ex);
                    }
                }
            }

            return rulesDictionary;
        }
    }
}
