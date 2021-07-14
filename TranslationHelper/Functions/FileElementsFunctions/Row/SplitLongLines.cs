using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SplitLongLines : RowBase
    {
        public SplitLongLines(ProjectData projectData) : base(projectData)
        {
        }

        protected override bool IsValidTable(DataTable table)
        {
            return !projectData.CurrentProject.LineSplitProjectSpecificSkipForTable(table);
        }

        protected override bool Apply()
        {
            string origCellValue = SelectedRow[ColumnIndexOriginal] as string;
            string transCellValue = SelectedRow[ColumnIndexTranslation] + string.Empty;
            if (!string.IsNullOrWhiteSpace(transCellValue)
                && transCellValue != origCellValue
                && FunctionsString.GetLongestLineLength(transCellValue) > Properties.Settings.Default.THOptionLineCharLimit
                /*&& !FunctionsString.IsStringContainsSpecialSymbols(transCellValue)*/
                && !projectData.CurrentProject.LineSplitProjectSpecificSkipForLine(origCellValue, transCellValue, SelectedTableIndex, SelectedRowIndex))
            {
                SelectedRow[ColumnIndexTranslation] = transCellValue.SplitMultiLineIfBeyondOfLimit(Properties.Settings.Default.THOptionLineCharLimit);
                //row[1] = transCellValue.Wrap(Properties.Settings.Default.THOptionLineCharLimit);
                return true;
            }
            return false;
        }
    }
}
