using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsString
    {
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
        /// True if procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// when string mostly contains japanese chars
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool HaveMostOfRomajiOtherChars(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return true;

            return FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(inputString)
                || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(inputString, "other")
                || (FunctionsRomajiKana.GetLocaleLangCount(inputString, "romaji") + FunctionsRomajiKana.GetLocaleLangCount(inputString, "other")) == inputString.Length
                || FunctionsRomajiKana.GetLocaleLangCount(inputString, "romaji") == inputString.Length
                || FunctionsRomajiKana.GetLocaleLangCount(inputString, "other") == inputString.Length;
        }

        /// <summary>
        /// True if language is Japanese and procent of Romaji or Other characters in input string is lesser of set value in Settings
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal static bool IsSourceLangJapaneseAndTheStringMostlyRomajiOrOther(this string inputString)
        {
            return Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && inputString.HaveMostOfRomajiOtherChars();
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
            if (input != null)
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(input))
                {
                    int i = 0;
                    while (reader.ReadLine() != null)
                    {
                        i++;
                        if (i > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
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

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
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
    }
}
