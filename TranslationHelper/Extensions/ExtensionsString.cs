using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsString
    {
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
    }
}
