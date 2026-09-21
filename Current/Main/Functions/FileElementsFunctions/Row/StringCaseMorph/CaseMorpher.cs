using System;
using System.Text.RegularExpressions;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    /// <summary>
    /// Which case a value has to be changed to.
    /// <para>
    /// The member names are the ones the operations used while the variants were declared inside
    /// <see cref="StringCaseMorphBase"/>. They were kept unchanged so this extraction stayed purely
    /// mechanical.
    /// </para>
    /// </summary>
    internal enum CaseMorphVariant
    {
        /// <summary>
        /// all chars to lower case
        /// </summary>
        lower = 0,
        /// <summary>
        /// 1st char to Upper case
        /// </summary>
        Upper = 1,
        /// <summary>
        /// all chars to upper case
        /// </summary>
        UPPER = 2,
        /// <summary>
        /// 1st char to lower
        /// </summary>
        lower1st = 3,
        /// <summary>
        /// 1st char to Upper case in all lines
        /// </summary>
        UpperAllLines = 4,
    }

    /// <summary>
    /// Changes the letter case of a translation the way the row operations need it.
    /// <para>
    /// This is a pure text transformation: no row, no table, no UI, no project. It lives apart from
    /// <see cref="StringCaseMorphBase"/> because the base's job is to feed the values extracted from a
    /// row through this, and the transformation itself is worth reading and checking on its own.
    /// </para>
    /// </summary>
    internal static class CaseMorpher
    {
        /// <summary>
        /// Applies <paramref name="variant"/> to <paramref name="translationString"/>.
        /// </summary>
        /// <param name="originalString">
        /// The original value. It is compared with the translation so a character that is already the
        /// same in both is left alone.
        /// </param>
        /// <param name="translationString">The value to change.</param>
        /// <param name="variant">Which case to change to.</param>
        /// <param name="isExtracted">
        /// True when the value came out of the extraction rules. The all-lines variant only splits a
        /// whole value into lines; an extracted value is treated as a single one.
        /// </param>
        internal static string Change(string originalString, string translationString, CaseMorphVariant variant, bool isExtracted)
        {
            switch (variant)
            {
                case CaseMorphVariant.lower:
                    //lowercase
                    return translationString.ToLowerInvariant();
                case CaseMorphVariant.UpperAllLines:
                    return ToUpperAllLines(originalString, translationString, isExtracted);
                case CaseMorphVariant.Upper:
                    //Uppercase
                    //https://www.c-sharpcorner.com/blogs/first-letter-in-uppercase-in-c-sharp1
                    return StringToUpper(translationString, originalString);
                case CaseMorphVariant.UPPER:
                    //UPPERCASE
                    return translationString.ToUpperInvariant();
                case CaseMorphVariant.lower1st:
                    //UPPERCASE
                    return StringToUpper(translationString, originalString, isReverse: true);
                default:
                    return translationString;
            }
        }

        private static string ToUpperAllLines(string originalString, string translationString, bool isExtracted)
        {
            if (isExtracted) return StringToUpper(translationString, originalString);

            var newLineSymbolIndex = translationString.IndexOf("\n");
            if (newLineSymbolIndex == -1)
            {
                // standart ToUpper when single line
                return StringToUpper(translationString, originalString);
            }

            string newLineSymbol = newLineSymbolIndex > 0 && translationString[newLineSymbolIndex - 1].Equals('\r') ? "\r\n" : "\n";

            string[] linesOfOriginal = originalString.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
            string[] linesOfTranslation = translationString.Split(new[] { newLineSymbol }, StringSplitOptions.None);

            if (linesOfOriginal.Length != linesOfTranslation.Length) return StringToUpper(translationString, originalString);

            for (int i = 0; i < linesOfTranslation.Length; i++)
            {
                var line = linesOfTranslation[i];

                if (string.IsNullOrWhiteSpace(line)) continue;

                // original line 1st char is equal to translation, skip
                if (line.Equals(linesOfOriginal[i])) continue;

                linesOfTranslation[i] = StringToUpper(line, originalString);
            }

            return string.Join(newLineSymbol, linesOfTranslation);
        }

        /// <summary>
        /// Upper cases the first letter that is worth changing.
        /// </summary>
        /// <param name="inputString">The value to change.</param>
        /// <param name="original">The original value, used to skip a character that is already the same.</param>
        /// <param name="isReverse">1st char change to lower instead of Upper</param>
        internal static string StringToUpper(string inputString, string original, bool isReverse = false)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return inputString;

            //original can be null, so every access to it below is bounds checked instead of relying on
            //the caller.
            original = original ?? string.Empty;

            if (char.IsLetter(inputString[0]))
            {
                //no original char to compare with means the case change applies
                if (original.Length == 0 || original[0] != inputString[0]) // skip if char in original equals char in translation with same index
                {
                    inputString = (isReverse ? char.ToLowerInvariant(inputString[0]) : char.ToUpperInvariant(inputString[0])) + inputString.Substring(1);
                }
            }
            else
            {
                int dsTransCellLength = inputString.Length;
                for (int c = 0; c < dsTransCellLength; c++)
                {
                    char @char = inputString[c];
                    if (IsCustomSymbol(@char) || char.IsWhiteSpace(@char) || char.IsPunctuation(@char)) continue;

                    if ((c > 0 && (@char == 's' && inputString[c - 1] == '\'' || inputString[c - 1] == '\\')) // 's or \s
                        ||
                        original.Length > c && original[c] == inputString[c]) // skip if char in original equals char in translation with same index
                    { }
                    //Substring(c + 1) already returns an empty string for the last character.
                    else inputString = inputString.Substring(0, c) + (isReverse ? char.ToLowerInvariant(inputString[c]) : char.ToUpperInvariant(inputString[c])) + inputString.Substring(c + 1);

                    break;
                }
            }

            if (inputString.StartsWith("[") && inputString.IsMultiline())
            {
                int lineCnt = 0;
                string resultLine = string.Empty;
                foreach (var line in inputString.SplitToLines())
                {
                    if (lineCnt == 0)
                    {
                        resultLine += line;
                    }
                    else
                    {
                        resultLine += Environment.NewLine;
                        if (lineCnt == 1)
                        {
                            resultLine += StringToUpper(line, original, isReverse);
                        }
                        else
                        {
                            resultLine += line;
                        }
                    }
                    lineCnt++;
                }
                inputString = resultLine;
            }

            // upper case of first letter after jp bracket
            foreach (Match m in Regex.Matches(inputString, "[「『][a-z]")) inputString = inputString.Remove(m.Index, 2).Insert(m.Index, m.Value.ToUpperInvariant());

            return inputString;
        }

        private static bool IsCustomSymbol(char @char)
        {
            return @char == '「' || @char == '『' || @char == '"';
        }
    }
}
