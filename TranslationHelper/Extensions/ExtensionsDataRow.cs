using System.Data;
using System.Linq;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsDataRow
    {
        /// <summary>
        /// true when original and translation lines count is equal and any orig line valid and equal same transl line
        /// </summary>
        /// <param name="selectedRow"></param>
        /// <returns></returns>
        internal static bool HasAnyTranslationLineValidAndEqualSameOrigLine(this DataRow selectedRow, bool checklinescount = true)
        {
            string o = (selectedRow[0] + "");
            string t = (selectedRow[1] + "");
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
