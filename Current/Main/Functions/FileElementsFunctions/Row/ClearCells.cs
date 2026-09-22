using System.Threading.Tasks;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Clears the translation of every row in scope.
    /// <para>
    /// It rewrites whole tables, so it asks the view to stop following the table while it runs: a grid
    /// bound to a table being rewritten row by row is repainted once per row, which for a large file is
    /// slower than the work itself. Which control that is, and whether one shows the table at all, is
    /// the view's business — this class only says that the table is about to change wholesale.
    /// </para>
    /// </summary>
    class ClearCells : RowBase
    {
        public ClearCells()
        {
        }

        protected override bool IsParallelRows => true;

        bool _dataSourceClear = false;

        protected override Task ActionsPreTableApply(TableData tableData)
        {
            if (IsAll || IsTables || IsTable)
            {
                _dataSourceClear = true;
                UiUpdater.BeginBulkChange(tableData.SelectedTable);
            }

            return Task.CompletedTask;
        }

        protected override Task ActionsPostTableApply(TableData tableData)
        {
            if ((IsAll || IsTables || IsTable) && _dataSourceClear)
            {
                _dataSourceClear = false;
                UiUpdater.EndBulkChange(tableData.SelectedTable);
            }

            return Task.CompletedTask;
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return true; //clear any rows
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            rowData.Translation = null;

            return true;
        }
    }
}
