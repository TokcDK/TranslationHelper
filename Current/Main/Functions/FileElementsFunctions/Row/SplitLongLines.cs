﻿using System.Data;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SplitLongLines : RowBase
    {
        public SplitLongLines()
        {
        }

        protected override bool IsOkTable(TableData tableData)
        {
            return !Project.LineSplitProjectSpecificSkipForTable(tableData.SelectedTable);
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            string origCellValue = rowData.Original;
            string transCellValue = rowData.Translation;
            if (!string.IsNullOrWhiteSpace(transCellValue)
                && transCellValue != origCellValue
                && FunctionsString.GetLongestLineLength(transCellValue) > AppSettings.THOptionLineCharLimit
                /*&& !FunctionsString.IsStringContainsSpecialSymbols(transCellValue)*/
                && !Project.LineSplitProjectSpecificSkipForLine(origCellValue, transCellValue, rowData.SelectedTableIndex, rowData.SelectedRowIndex))
            {
                rowData.Translation = transCellValue.SplitMultiLineIfBeyondOfLimit(AppSettings.THOptionLineCharLimit);
                //row[1] = transCellValue.Wrap(AppSettings.THOptionLineCharLimit);
                return true;
            }
            return false;
        }
    }
}
