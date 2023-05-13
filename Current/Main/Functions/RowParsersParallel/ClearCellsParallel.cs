using System;
using System.Data;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.RowParsersParallel
{
    class ClearCellsParallel : RowParallelParserBase
    {
        protected override bool Process(DataRowData row)
        {
            row.Translation = null;

            return true;
        }
    }
}
