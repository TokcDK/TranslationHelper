using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SplitLongLines : RowBase
    {
        public SplitLongLines()
        {
        }

        protected override bool IsValidTable(DataTable table)
        {
            return !AppData.CurrentProject.LineSplitProjectSpecificSkipForTable(table);
        }

        protected override bool Apply()
        {
            string origCellValue = Original;
            string transCellValue = Translation;
            if (!string.IsNullOrWhiteSpace(transCellValue)
                && transCellValue != origCellValue
                && FunctionsString.GetLongestLineLength(transCellValue) > AppSettings.THOptionLineCharLimit
                /*&& !FunctionsString.IsStringContainsSpecialSymbols(transCellValue)*/
                && !AppData.CurrentProject.LineSplitProjectSpecificSkipForLine(origCellValue, transCellValue, SelectedTableIndex, SelectedRowIndex))
            {
                Translation = transCellValue.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit);
                //row[1] = transCellValue.Wrap(AppSettings.THOptionLineCharLimit);
                return true;
            }
            return false;
        }
    }
}
