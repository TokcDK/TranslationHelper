using System.Data;

namespace TranslationHelper.Extensions
{
    internal static class ExtensionsDataRow
    {
        /// <summary>
        /// true when original and translation lines count is equal and any orig line valid and equal same transl line
        /// </summary>
        /// <param name="selectedRow"></param>
        /// <returns></returns>
        internal static bool HasAnyTranslationLineValidAndEqualSameOrigLine(this DataRow selectedRow, bool checklinescount=true)
        {
            if (checklinescount && (selectedRow[1] + "").GetLinesCount() != (selectedRow[0] + "").GetLinesCount())
                return false;

            foreach (var oline in (selectedRow[0] + "").SplitToLines())
            {
                foreach (var tline in (selectedRow[1] + "").SplitToLines())
                {
                    if (oline.IsValidForTranslation() && oline == tline)
                        return true;
                }
            }

            return false;
        }
    }
}
