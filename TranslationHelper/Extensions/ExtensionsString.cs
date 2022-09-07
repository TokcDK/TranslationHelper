using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsString
    {
        /// <summary>
        /// extract captured groups from string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineCoordinates"></param>
        /// <param name="lineNum"></param>
        /// <returns></returns>
        internal static ExtractRegexInfo ExtractMulty(this string line)
        {
            var log = new FunctionsLogs();
            var extractRegexData = new ExtractRegexInfo();
            try
            {
                foreach (var PatternReplacementPair in AppData.TranslationRegexRules)
                {
                    // check if any regex is match
                    Regex regex = null;
                    Match match = null;
                    try 
                    {
                        regex = new Regex(PatternReplacementPair.Key);
                        match = regex.Match(line);

                        if (!match.Success) continue; 
                    }
                    catch (System.ArgumentException ex)
                    {
                        log.LogToFile("ExtractMulty: Invalid regex:" + PatternReplacementPair.Key + "\r\nError:\r\n" + ex);
                        AppData.Main.ProgressInfo(true, "Invalid regex found. See " + THSettings.ApplicationLogName);
                        continue;
                    }

                    // add regex pattern and replacer
                    extractRegexData.Pattern = PatternReplacementPair.Key;
                    extractRegexData.Replacer = PatternReplacementPair.Value;

                    // add matched groups values
                    foreach (Group g in match.Groups)
                    {
                        if (!extractRegexData.Replacer.Contains("$" + g.Name)) continue; // skip if group is missing in replacer value

                        var valueData = extractRegexData.ValueDataList.ContainsKey(g.Value) ? extractRegexData.ValueDataList[g.Value] : new ExtractRegexValueInfo();

                        if (valueData.GroupIndexes.Contains(g.Index)) continue;

                        valueData.GroupIndexes.Add(g.Index);
                    }

                    break; // regex found skip other
                }
            }
            catch (InvalidOperationException) // in case of collection was changed exception when rules was changed in time of iteration
            {
                // retry extraction
                return line.ExtractMulty();
            }

            return extractRegexData;
        }

        /// <summary>
        /// extract captured groups from string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineCoordinates"></param>
        /// <param name="lineNum"></param>
        /// <returns></returns>
        internal static string[] ExtractMulty(this string line, string lineCoordinates, int lineNum, Dictionary<string/*table index,row index*/, Dictionary<int/*line number*/, Dictionary<string/*text from original*/, string/*text of translation*/>>> bufferExtracted = null)
        {
            if (bufferExtracted == null)
            {
                bufferExtracted = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
            }

            if (!bufferExtracted.ContainsKey(lineCoordinates))
            {
                //init data
                bufferExtracted.Add(lineCoordinates, new Dictionary<int, Dictionary<string, string>>());//add coordinates
            }
            if (!bufferExtracted[lineCoordinates].ContainsKey(lineNum))
            {
                //init data
                bufferExtracted[lineCoordinates].Add(lineNum, new Dictionary<string, string>());//add linenum
            }

            var log = new FunctionsLogs();
            var GroupValues = new List<string>();//list of values for captured groups which containing in PatternReplacementPair.Value
            try
            {
                foreach (var PatternReplacementPair in AppData.TranslationRegexRules)
                {
                    try
                    {
                        if (!Regex.IsMatch(line, PatternReplacementPair.Key))
                        {
                            continue;
                        }
                    }
                    catch (System.ArgumentException ex)
                    {
                        log.LogToFile("ExtractMulty: Invalid regex:" + PatternReplacementPair.Key + "\r\nError:\r\n" + ex);
                        AppData.Main.ProgressInfo(true, "Invalid regex found. See " + THSettings.ApplicationLogName);
                        continue;
                    }

                    foreach (Group g in Regex.Match(line, PatternReplacementPair.Key).Groups)
                    {
                        try
                        {

                            if (!bufferExtracted[lineCoordinates][lineNum].ContainsKey(PatternReplacementPair.Key))//add pattern-replacement data
                            {
                                bufferExtracted[lineCoordinates][lineNum].Add(PatternReplacementPair.Key, PatternReplacementPair.Value);
                            }

                            if (PatternReplacementPair.Value.Contains("$" + g.Name))//if replacement contains the group name ($1,$2,$3...$99)
                            {

                                if (!bufferExtracted[lineCoordinates][lineNum].ContainsKey("$" + g.Name))
                                {
                                    bufferExtracted[lineCoordinates][lineNum].Add("$" + g.Name, g.Value);//add group name with valueif it is not added and is in replacement

                                    if (!GroupValues.Contains(g.Value))
                                    {
                                        GroupValues.Add(g.Value);//add value for translation if valid
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    break;
                }
            }
            catch (System.InvalidOperationException) // in case of collection was changed exception when rules was changed in time of iteration
            {
                // retry extraction
                return line.ExtractMulty(lineCoordinates, lineNum, bufferExtracted);
            }

            if (GroupValues.Count > 0)
            {
                return GroupValues.ToArray();
            }
            else
            {
                return new string[1] { line };
            }
        }

        /// <summary>
        /// extract captured groups from string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineCoordinates"></param>
        /// <param name="lineNum"></param>
        /// <returns></returns>
        internal static string[] ExtractMulty(this string line, bool onlyOne = false, List<int> outIndexes = null)
        {
            var GroupValues = (onlyOne ? new List<string>(1) : new List<string>());//list of values for captured groups which containing in PatternReplacementPair.Value
            foreach (var PatternReplacementPair in AppData.TranslationRegexRules)
            {
                Match m = Regex.Match(line, PatternReplacementPair.Key);
                if (!m.Success)
                {
                    continue;
                }

                if (onlyOne && m.Groups.Count > 2) // 2 because 0 group is always will be in success match
                {
                    continue;
                }

                bool skipFirst = true;
                foreach (Group g in m.Groups)
                {
                    if (skipFirst) // first $0 group is equal line itself
                    {
                        skipFirst = false;
                        continue;
                    }

                    try
                    {

                        if (PatternReplacementPair.Value.Contains("$" + g.Name))//if replacement contains the group name ($1,$2,$3...$99)
                        {
                            if (!GroupValues.Contains(g.Value))
                            {
                                if (outIndexes != null)
                                {
                                    outIndexes.Add(g.Index); // add index of string
                                }

                                GroupValues.Add(g.Value);//add value for translation if valid
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                break;
            }

            if (GroupValues.Count > 0)
            {
                return GroupValues.ToArray();
            }
            else
            {
                return new string[1] { line };
            }
        }

        internal static string SplitMultiLineIfBeyondOfLimit(this string Line, int Limit, int linesCountLimit = -1)
        {
            if (string.IsNullOrWhiteSpace(Line)/* || !Line.IsMultiline()*/)
            {
                return Line;
            }

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
                            //когда последняя строка, к которой присоединится новая, кончается не на пробел или другой разделитель, добавить пробел перед добавляемой строкой
                            if (char.IsLetterOrDigit(Convert.ToChar(Lines[LasLineIndex].Substring(Lines[LasLineIndex].Length - 1), CultureInfo.InvariantCulture)))
                            {
                                string trimmedSubline = subline.TrimStart();
                                //сделать перву букву добавляемой строки в нижнем регистре
                                if (trimmedSubline.Length > 1 && char.IsUpper(Convert.ToChar(trimmedSubline.Substring(0, 1), CultureInfo.InvariantCulture)))
                                {
                                    trimmedSubline = trimmedSubline.Substring(0, 1).ToLowerInvariant() + trimmedSubline.Substring(1);
                                }

                                subline = " " + trimmedSubline;
                            }
                            subline = Lines[LasLineIndex] + subline;
                            Lines.RemoveAt(LasLineIndex);
                        }
                    }
                    int i = 0;
                    lastLineWasSplitted = false;
                    foreach (var line in subline.GetSplittedLine(Limit).SplitToLines())
                    {
                        i++;
                        Lines.Add(line);
                    }

                    if (i > 1)
                    {
                        //если i>1 значит строка была разделена, сверху больше одного прохода цикла с i++
                        lastLineWasSplitted = true;
                    }
                }
            }
            else
            {
                return Line.GetSplittedLine(Limit);
            }

            return string.Join(Environment.NewLine, Lines); //ReturnLine.ToString();
        }

        internal static string GetSplittedLine(this string Line, int Limit)
        {
            string Trigger/* = string.Empty*/;
            string newLine = (Trigger = Regex.Match(Line, @"(if|en)\([^\r\n]+\)$").Value).Length > 0 ? Line.Replace(Trigger, string.Empty) : Line;



            //попытка сделать удаление и возвращение на место спецсимволов

            //MatchCollection SpecSymbols = Regex.Matches(newLine, @"(\\\\[A-Za-z]\[[0-9]{1,3}\])|(\\[A-Za-z]\[[0-9]{1,3}\])");

            if (newLine.Length == 0 /*|| newLine.Length + SpecSymbols.Count <= Limit*/)
            {
                return Line;
            }
            //return string.Join(AppSettings.ProjectNewLineSymbol
            //    , SplitLineIfBeyondOfLimit(Trigger.Length > 0 ? newLine : Line, Limit)
            //    ) + Trigger;
            //newLine = string.Join(AppSettings.ProjectNewLineSymbol
            //    , SplitLineIfBeyondOfLimit(newLine, Limit)
            //    ) + Trigger;
            //return string.Join(AppSettings.ProjectNewLineSymbol
            //    , newLine.SplitLineIfBeyondOfLimit(Limit)
            //    ) + Trigger;
            //var newLineBefore = newLine;
            return string.Join(AppSettings.ProjectNewLineSymbol
                , newLine.Wrap(Limit)
                ) + Trigger;

            //MatchCollection newLineSymbols = Regex.Matches(newLine, Environment.NewLine);
            //var newLineSymbolsCount = newLineSymbols.Count;
            ////newLine = newLine.Replace(Environment.NewLine, " ");
            //for (int i = newLineSymbolsCount - 1; i > -1; i--)
            //{
            //    newLine = newLine.Remove(newLineSymbols[i].Index, newLineSymbols[i].Value.Length);
            //}

            //var ind = 0;
            //var IndShift = newLineBefore.Length - newLine.Length;//модификация сдвига, на случай, если строка после модификаций будет отличаться
            //for (int i = 0; i < newLineSymbolsCount; i++)
            //{
            //    if (ind >= SpecSymbolsCount)
            //    {
            //        newLine = newLine.Insert(newLineSymbols[i].Index + IndShift, newLineSymbols[i].Value);
            //        IndShift += newLineSymbols[i].Value.Length;
            //    }

            //    for (int si = ind; si < SpecSymbolsCount; si++)
            //    {
            //        if (ind >= SpecSymbolsCount)
            //        {
            //            break;
            //        }

            //        if (SpecSymbols[si].Index + IndShift == newLineSymbols[i].Index + IndShift)
            //        {
            //            newLine = newLine.Insert(SpecSymbols[si].Index + IndShift, SpecSymbols[si].Value);
            //            IndShift += SpecSymbols[si].Value.Length;
            //            newLine = newLine.Insert(newLineSymbols[i].Index + IndShift, newLineSymbols[i].Value);
            //            IndShift += newLineSymbols[si].Value.Length;
            //            ind = si;

            //            if (i < newLineSymbolsCount - 1)
            //            {
            //                break;
            //            }
            //        }
            //        else if (SpecSymbols[si].Index + IndShift < newLineSymbols[i].Index + IndShift)
            //        {
            //            newLine = newLine.Insert(SpecSymbols[si].Index + IndShift, SpecSymbols[si].Value);
            //            IndShift += SpecSymbols[si].Value.Length;
            //            ind = si;
            //        }
            //        else// if (SpecSymbols[si].Index + IndShift > newLineSymbols[i].Index + IndShift)
            //        {
            //            newLine = newLine.Insert(newLineSymbols[i].Index + IndShift, newLineSymbols[i].Value);
            //            IndShift += newLineSymbols[i].Value.Length;
            //            ind = si;
            //            if (i < newLineSymbolsCount - 1)
            //            {
            //                break;
            //            }
            //        }
            //    }

            //}

            //return newLine;
        }

        internal static string[] SplitLineIfBeyondOfLimit(this string text, int max)
        {
            //https://ru.stackoverflow.com/questions/707937/c-%D0%BF%D0%B5%D1%80%D0%B5%D0%BD%D0%BE%D1%81-%D1%81%D0%BB%D0%BE%D0%B2-%D0%B2-%D1%81%D1%82%D1%80%D0%BE%D0%BA%D0%B5-%D1%81-%D1%80%D0%B0%D0%B7%D0%B1%D0%B8%D0%B2%D0%BA%D0%BE%D0%B9-%D0%BD%D0%B0-%D0%BE%D0%BF%D1%80%D0%B5%D0%B4%D0%B5%D0%BB%D0%B5%D0%BD%D0%BD%D1%83%D1%8E-%D0%B4%D0%BB%D0%B8%D0%BD%D1%83
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }
        /// <summary>
        /// examples: https://stackoverflow.com/a/4398419
        /// Wraps words of lines in <paramref name="text"/> which is beyond <paramref name="max"/> limit
        /// </summary>
        /// <param name="text">inpyt text</param>
        /// <param name="max">max chars limit in one sentence</param>
        /// <returns></returns>
        internal static string Wrap(this string text, int max)
        {
            string[] words = text.Split(' ');
            List<string> lines = new List<string>();
            string line = string.Empty;
            bool isNewLine;
            int ll;
            foreach (var word in words)
            {
                isNewLine = false;
                foreach (var subword in word.Split('\n')) // добавлено для сохранения оригинальных переносов в тексте, где, например, сначала имя персонажа, а его речь идет со следующей строки или следующая строка начинаятся с символа табуляции
                {
                    if (!isNewLine && (ll = line.Length) + subword.Length + 1 <= max)
                    {
                        if (ll == 0)
                        {
                            line += subword;
                        }
                        else
                        {
                            line += " " + subword;
                        }
                    }
                    else
                    {
                        if (isNewLine) // add merged merged line in new lines
                        {
                            // убрать \r в конце, если есть
                            if ((ll = line.Length) > 0 && line[--ll] == '\r')
                            {
                                lines.Add(line.Remove(ll));
                            }
                            else
                            {
                                lines.Add(line);
                            }
                        }
                        else
                        {
                            lines.Add(line);
                        }

                        line = subword; // установить слово как начало новой строки
                    }
                    isNewLine = true; // обозначает, что если в слове был оригинальный символ новой строки, 
                }
            }

            lines.Add(line);

            return string.Join(Environment.NewLine, lines);// merge lines with newline symbol and return
        }

        /// <summary>
        /// true if in string only digits 0-9
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static bool IsDigitsOnly(this string str)
        {
            str = str
                .Replace(".", string.Empty) // remove dots
                .Replace(",", string.Empty);

            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            //https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits
            //наибыстрейший метод 
            int strLength = str.Length;//и моя оптимизация, ускоряющая с 2.19 до 1.62 при 100млн. итераций
            int startIndex = str[0] == '-' && strLength > 1 ? 1 : 0;
            for (int i = startIndex; i < strLength; i++)
            {
                if (!IsDigit(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Return if <paramref name="character"/> is digit
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        internal static bool IsDigit(this char character)
        {
            return !((character ^ '0') > 9);
        }

        /// <summary>
        /// true when language is japanese and most od symbols are romaji or other
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool ForJPLangHaveMostOfRomajiOtherChars(this string inputString)
        {
            return THSettings.SourceLanguageIsJapanese && inputString.HaveMostOfRomajiOtherChars();
        }

        /// <summary>
        /// True if procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// when string mostly contains japanese chars
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool HaveMostOfRomajiOtherChars(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return true;

            int romajicnt;
            int othercnt;
            var inputStringLength = inputString.Length;
            return FunctionsRomajiKana.LocalePercentIsNotValid(inputString)
                || (FunctionsRomajiKana.LocalePercentIsNotValid(inputString, "other"))
                || ((romajicnt = FunctionsRomajiKana.GetLocaleLangCount(inputString, "romaji")) + (othercnt = FunctionsRomajiKana.GetLocaleLangCount(inputString, "other"))) == inputStringLength
                || romajicnt == inputStringLength
                || othercnt == inputStringLength;
        }

        /// <summary>
        /// true if input char is symbol of Kanji/Hiragana/Katakana. Input only one char as string
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static bool IsJPChar(this string s)
        {
            if (
                FunctionsRomajiKana.GetCharsInRange(s, 0x4E00, 0x9FBF).Any()//kanji
                || FunctionsRomajiKana.GetCharsInRange(s, 0x3040, 0x309F).Any()//hiragana
                || FunctionsRomajiKana.GetCharsInRange(s, 0x30A0, 0x30FF).Any()//katakana
                || s == "『"
                || s == "「"
                || s == "」"
                || s == "』"
                || s == "　"
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// True if language is Japanese and procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther(this string inputString)
        {
            return THSettings.SourceLanguageIsJapanese && inputString.HaveMostOfRomajiOtherChars();
        }

        /// <summary>
        /// True if string not empty/null and for selected JP language have most of japanese chars
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool IsNotEmptyAndNotMostlyRomajiiOther(this string inputString)
        {
            return !string.IsNullOrEmpty(inputString) && !inputString.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther();
        }

        /// <summary>
        /// If string is multiline
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static bool IsMultiline(this string input)
        {
            return input.IndexOf('\n', 0) > -1;
        }

        //https://stackoverflow.com/a/2567623
        //В моем случае этот вариант самый быстрый
        /// <summary>
        /// Get count of lines in the inputString
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static int GetLinesCount(this string inputString)
        {
            int count = -1;
            int index = -1;

            do
            {
                count++;
                index = inputString.IndexOf('\n', index + 1);
            }
            while (index != -1);

            return count + 1;
        }

        /// <summary>
        /// Split string to lines
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static IEnumerable<string> SplitToLines(this string input)
        {
            //https://stackoverflow.com/a/23408020
            if (input == null)
            {
                yield break;
            }

            if (!input.IsMultiline())
            {
                yield return input;
                yield break;
            }

            //Fix of last newline \n symbol was not returned 
            bool EndsWithNewLine = input.EndsWith("\n");

            using (var reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
                if (EndsWithNewLine)//if string endswith \n then last line will be null
                {
                    yield return string.Empty;
                }
            }
        }

        /// <summary>
        /// gets length of string without several special symbols
        /// </summary>
        /// <param name="inputLine"></param>
        /// <returns></returns>
        internal static int LengthWithoutSpecSymbols(this string inputLine)
        {
            string newline = inputLine;

            newline = Regex.Replace(newline, @"^([\s\S]+)(if|en)\([\s\S]+\)$", "$1");
            newline = Regex.Replace(newline, @"\\\#\{\$game_actors\[.+\]\.name\}", "ActorName1");
            newline = Regex.Replace(newline, @"\\\#\{\$game_variables\[.+\]\}", "variable10");
            newline = Regex.Replace(newline, @"\\\\[A-Za-z]\[.+\]", string.Empty);

            return newline.Length;
        }

        /// <summary> 
        /// Get string lines to string array
        /// </summary>
        /// <param name="str"></param>
        /// <param name="OnlyNonEmpty">Add only non empty</param>
        /// <returns></returns>
        internal static string[] GetAllLinesToArray(this string str, bool OnlyNonEmpty = false)
        {
            List<string> lineslist = new List<string>();
            foreach (var line in str.SplitToLines())
            {
                if (OnlyNonEmpty)
                {
                    if (line.Length > 0)
                    {
                        lineslist.Add(line);
                    }
                }
                else
                {
                    lineslist.Add(line);
                }
            }

            return lineslist.ToArray();
        }

        /// <summary>
        /// return joined lines with newline symbol
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string Joined(this IEnumerable<string> value)
        {
            return string.Join(Environment.NewLine, value);
        }

        /// <summary>
        /// return joined lines with newline symbol
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string Joined(this string[] value)
        {
            return string.Join(Environment.NewLine, value);
        }

        /// <summary>
        /// true when string contains atleast one letter
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static bool HasLetters(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            foreach (var c in s)
            {
                if (char.IsLetter(c))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// true if input string is valid
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool IsValidForTranslation(this string inputString)
        {
            return !string.IsNullOrWhiteSpace(inputString) && AppData.CurrentProject.IsValidForTranslation(inputString) && !inputString.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther() && inputString.HasLetters() && !inputString.IsSoundsText();
        }

        /// <summary>
        /// will trim all except lettern or digits
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        internal static string TrimAllExceptLettersOrDigits(this string keyValue)
        {
            var trimit = new HashSet<char>(keyValue.Length);
            foreach (var c in keyValue)
            {
                if (!char.IsLetterOrDigit(c) && !trimit.Contains(c))
                {
                    trimit.Add(c);
                }
            }

            return keyValue.Trim(trimit.ToArray());
        }

        internal static bool IsSoundsText(this string str)
        {
            var regexed = Regex.Replace(str, THSettings.SoundsTextRegexPattern, "");
            var trimmed = regexed.TrimAllExceptLettersOrDigits();
            return trimmed.Length == 0;
        }

        /// <summary>
        /// Get closing bracket index for given opening bracket.
        /// https://www.geeksforgeeks.org/find-index-closing-bracket-given-opening-bracket-expression/
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="index"></param>

        public static int GetClosingBraketIndexFor(this string inputString, int index)
        {
            int i;

            // If index given is invalid and is
            // not an opening bracket.
            if (inputString[index] != '[')
            {
                //Console.Write(expression + ", "
                //        + index + ": -1\n");
                return -1;
            }

            // Stack to store opening brackets.
            var st = new Stack();

            // Traverse through string starting from
            // given index.
            for (i = index; i < inputString.Length; i++)
            {

                // If current character is an
                // opening bracket push it in stack.
                if (inputString[i] == '[')
                {
                    st.Push((int)inputString[i]);
                }
                // If current character is a closing
                // bracket, pop from stack. If stack
                // is empty, then this closing
                // bracket is required bracket.
                else if (inputString[i] == ']')
                {
                    st.Pop();
                    if (st.Count == 0)
                    {
                        //Console.Write(inputString + ", "
                        //        + index + ": " + i + "\n");
                        return i;
                    }
                }
            }

            // If no matching closing bracket
            // is found.
            //Console.Write(inputString + ", "
            //        + index + ": -1\n");
            return -1;
        }
    }
}
