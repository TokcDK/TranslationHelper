using System;
using System.Data;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsDataRow
    {
        /// <summary>
        /// Set value to selected row.
        /// Use invoke in debug
        /// </summary>
        /// <param name="inputRow"></param>
        /// <param name="columnIndex"></param>
        /// <param name="value"></param>
        internal static void SetValue(this DataRow inputRow, int columnIndex, object value)
        {
#if DEBUG
            AppData.Main.Invoke((Action)(() => inputRow.SetField(columnIndex, value)));
#else
            inputRow.SetField(columnIndex, value);
#endif
        }
        /// <summary>
        /// Set value to selected row.
        /// Use invoke in debug
        /// </summary>
        /// <param name="inputRow"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        internal static void SetValue(this DataRow inputRow, string columnName, object value)
        {
#if DEBUG
            AppData.Main.Invoke((Action)(() => inputRow.SetField(columnName, value)));
#else
            inputRow.SetField(columnName, value);
#endif
        }

        /// <summary>
        /// true when original and translation lines count is equal and any orig line valid and equal same transl line
        /// </summary>
        /// <param name="selectedRow"></param>
        /// <returns></returns>
        internal static bool HasAnyTranslationLineValidAndEqualSameOrigLine(this string original, string translation, bool checklinescount = true)
        {
            if (string.IsNullOrEmpty(original)) return true;

            int originalLinesCount = original.GetLinesCount();
            int translatonLinesCount = translation.GetLinesCount();
            if (original == translation || (checklinescount && originalLinesCount > 1 && originalLinesCount != translatonLinesCount))
                return false;

            int i = -1;
            var translationLines = translation.SplitToLines().ToArray();
            foreach (var originalLine in original.SplitToLines())
            {
                if (++i == translatonLinesCount) return false;

                if (originalLine == translationLines[i]
                    && originalLine.IsValidForTranslation())
                    return true;

                var extractData = new ExtractRegexInfo(originalLine);
                if (extractData != null
                    && extractData.ExtractedValuesList.Count > 0
                    && extractData.ExtractedValuesList.Any(v => !string.IsNullOrWhiteSpace(v.Original) && v.Original.IsValidForTranslation()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
