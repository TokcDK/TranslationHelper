using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsString
    {
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
                            if (Lines[LasLineIndex].Length > 0 && char.IsLetterOrDigit(Convert.ToChar(Lines[LasLineIndex].Substring(Lines[LasLineIndex].Length - 1), CultureInfo.InvariantCulture)))
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

        internal static string GetSplittedLine(this string Line, int Limit, THDataWork thDataWork = null)
        {
            string Trigger/* = string.Empty*/;
            string newLine = (Trigger = Regex.Match(Line, @"(if|en)\([^\r\n]+\)$").Value).Length > 0 ? Line.Replace(Trigger, string.Empty) : Line;



            //попытка сделать удаление и возвращение на место спецсимволов

            //MatchCollection SpecSymbols = Regex.Matches(newLine, @"(\\\\[A-Za-z]\[[0-9]{1,3}\])|(\\[A-Za-z]\[[0-9]{1,3}\])");

            if (newLine.Length == 0 /*|| newLine.Length + SpecSymbols.Count <= Limit*/)
            {
                return Line;
            }
            //return string.Join(Properties.Settings.Default.ProjectNewLineSymbol
            //    , SplitLineIfBeyondOfLimit(Trigger.Length > 0 ? newLine : Line, Limit)
            //    ) + Trigger;
            //newLine = string.Join(Properties.Settings.Default.ProjectNewLineSymbol
            //    , SplitLineIfBeyondOfLimit(newLine, Limit)
            //    ) + Trigger;
            //return string.Join(Properties.Settings.Default.ProjectNewLineSymbol
            //    , newLine.SplitLineIfBeyondOfLimit(Limit)
            //    ) + Trigger;
            //var newLineBefore = newLine;
            return string.Join(Properties.Settings.Default.ProjectNewLineSymbol
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
        /// https://stackoverflow.com/a/4398419
        /// Wraps words of lines which is beyond limit
        /// </summary>
        /// <param name="text"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        internal static string Wrap(this string text, int max)
        {
            string[] words = text.Split(' ');
            //var parts = new Dictionary<int, string>();
            var parts = new List<string>();//2) смена на List еще снизило время с 0.058 до 0.052 на 100000 итераций
            string part = string.Empty;
            //int partCounter;// = 0;
            bool IsNewLine;
            foreach (var word in words)
            {
                IsNewLine = false;
                foreach (var subword in word.Split('\n'))
                {
                    if (!IsNewLine && part.Length + subword.Length + 1 <= max)
                    {
                        if (string.IsNullOrEmpty(part))//1) это снизило время выполнения с 0.067 до 0.058 на 100000 итераций
                        {
                            part += subword;
                        }
                        else
                        {
                            part += " " + subword;
                        }
                        //part += string.IsNullOrEmpty(part) ? word : " " + word;
                    }
                    else
                    {
                        //parts.Add(partCounter, part);
                        if (IsNewLine)
                        {
                            parts.Add(part.Replace("\r", string.Empty));
                        }
                        else
                        {
                            parts.Add(part);
                        }

                        part = subword;
                        //partCounter++;
                    }
                    IsNewLine = true;
                }
            }
            //parts.Add(partCounter, part);
            parts.Add(part);
            //foreach (var item in parts)
            //{
            //    Console.WriteLine("Part {0} (length = {2}): {1}", item.Key, item.Value, item.Value.Length);
            //}
            //Console.ReadLine();

            //return parts.Values.ToArray();
            return string.Join(Environment.NewLine, parts);
        }

        /// <summary>
        /// true if in string only digits 0-9
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static bool IsDigitsOnly(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            str = str.Replace(".", string.Empty);
            //https://stackoverflow.com/questions/7461080/fastest-way-to-check-if-string-contains-only-digits
            //наибыстрейший метод 
            int strLength = str.Length;//и моя оптимизация, ускоряющая с 2.19 до 1.62 при 100млн. итераций
            for (int i = 0; i < strLength; i++)
            {
                if ((str[i] ^ '0') > 9)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// true when language is japanese and most od symbols are romaji or other
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool ForJPLangHaveMostOfRomajiOtherChars(this string inputString)
        {
            return THSettingsData.SourceLanguageIsJapanese() && inputString.HaveMostOfRomajiOtherChars();
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
                || s== "『"
                || s== "「"
                || s== "」"
                || s== "』"
                || s== "　"
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
            return THSettingsData.SourceLanguageIsJapanese() && inputString.HaveMostOfRomajiOtherChars();
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
            //inputString="line1\r\n"
            //00.00066x100000
            //var count = 0;
            //var index = -1;

            //do
            //{
            //    if (++count > 1)
            //    {
            //        return true;
            //    }

            //    index = inputString.IndexOf('\n', index + 1);
            //}
            //while (index != -1);

            //return false;

            //inputString="line1\r\n"
            //00.00055x100000
            return input.IndexOf('\n', 0) > -1;

            //inputString="line1\r\n"
            //00.0021x100000
            //return inputString.Contains("\n");

            ///old
            {
                //0.0035x100000
                //if (input != null)
                //{
                //    using (System.IO.StringReader reader = new System.IO.StringReader(input))
                //    {
                //        int i = 0;
                //        while (reader.ReadLine() != null)
                //        {
                //            i++;
                //            if (i > 1)
                //            {
                //                return true;
                //            }
                //        }
                //    }
                //}

                //return false;
            }
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

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
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
            return !string.IsNullOrWhiteSpace(inputString) && !inputString.IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther() && inputString.HasLetters() && !inputString.IsSoundsText();
        }

        /// <summary>
        /// will trim all except lettern or digits
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        internal static string TrimAllExceptLettersOrDigits(this string keyValue)
        {
            var trimit = new List<char>();
            var unique = new HashSet<char>();
            foreach (var c in keyValue)
            {
                if (!char.IsLetterOrDigit(c) && !unique.Contains(c))
                {
                    trimit.Add(c);
                    unique.Add(c);
                }
            }

            return keyValue.Trim(trimit.ToArray());
        }

        internal static bool IsSoundsText(this string str)
        {
            var regexed = Regex.Replace(str, THSettingsData.SoundsTextRegexPattern(), "");
            var trimmed = regexed.TrimAllExceptLettersOrDigits();
            return trimmed.Length == 0;
        }
    }
}
