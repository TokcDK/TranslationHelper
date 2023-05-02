using System;
using System.Data;
using System.Linq;
using TranslationHelper.Data;

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
            string o = original;
            string t = translation;
            if (o == t || !o.IsMultiline() || (checklinescount && o.GetLinesCount() != t.GetLinesCount()))
                return false;

            var olines = o.SplitToLines().ToArray();
            var tlines = t.SplitToLines().ToArray();
            for (int i = 0; i < olines.Length; i++)
            {
                if (i < tlines.Length && olines[i] == tlines[i] && olines[i].IsValidForTranslation())
                    return true;
            }

            return false;
        }
    }
}
