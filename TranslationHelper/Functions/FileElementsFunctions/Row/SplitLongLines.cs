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
            string origCellValue = SelectedRow[ColumnIndexOriginal] as string;
            string transCellValue = SelectedRow[ColumnIndexTranslation] + string.Empty;
            if (!string.IsNullOrWhiteSpace(transCellValue)
                && transCellValue != origCellValue
                && FunctionsString.GetLongestLineLength(transCellValue) > AppSettings.THOptionLineCharLimit
                /*&& !FunctionsString.IsStringContainsSpecialSymbols(transCellValue)*/
                && !AppData.CurrentProject.LineSplitProjectSpecificSkipForLine(origCellValue, transCellValue, SelectedTableIndex, SelectedRowIndex))
            {
                SelectedRow[ColumnIndexTranslation] = transCellValue.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit);
                //row[1] = transCellValue.Wrap(AppSettings.THOptionLineCharLimit);
                return true;
            }
            return false;
        }
    }
}
