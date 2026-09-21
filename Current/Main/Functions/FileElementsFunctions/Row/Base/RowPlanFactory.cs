using System;
using System.Data;
using System.Linq;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Turns a selection into the list of table/row scopes a run has to cover.
    /// <para>
    /// Pure by design: it reads the selection and the project content and returns a plan, and it
    /// executes nothing. That is what keeps "what is in scope" testable and separate from "how a
    /// scope is walked", which is <see cref="RowBase"/>'s job.
    /// </para>
    /// </summary>
    internal static class RowPlanFactory
    {
        /// <summary>
        /// The rows selected in the work table grid, grouped by the file table they belong to.
        /// <para>
        /// A selection that covers every row of the one table it touches is treated as the whole
        /// table, so a "select all, then run" ends up in whole-table mode.
        /// </para>
        /// </summary>
        /// <param name="selection">Where the selected rows come from.</param>
        /// <param name="isOkSelected">
        /// Policy of the operation, asked about the one table the selection touches. A rejected
        /// selection produces an empty plan, which the caller reports as "nothing was done".
        /// </param>
        /// <returns>
        /// The plan, or an empty one when there is nothing to do — either because nothing is selected
        /// or because the operation's policy rejected the selection. Both mean the same to the
        /// caller: no hooks, no sound, nothing changed.
        /// </returns>
        internal static TableRowPlan[] ForSelectedRows(ISelectionProvider selection, Func<TableData, bool> isOkSelected)
        {
            var plan = selection.GetSelectedRowsPlan();
            if (plan.Length == 0) return Array.Empty<TableRowPlan>();

            if (plan.Length == 1 && !isOkSelected(plan[0].Table)) return Array.Empty<TableRowPlan>();

            if (plan.Length == 1 && plan[0].RowIndexes != null && plan[0].RowIndexes.Length == plan[0].Table.SelectedTable.Rows.Count)
            {
                plan = new[] { new TableRowPlan(plan[0].Table, null) };
            }

            return plan;
        }

        /// <summary>
        /// Whole-table plan for the entries selected in the files list. The "[ALL]" entry stands for
        /// every file, so selecting it plans all of them.
        /// </summary>
        internal static TableRowPlan[] ForSelectedEntries(ISelectionProvider selection, DataSet filesContent)
        {
            return selection.GetSelectedTableIndexes()
                .Where(idx => idx >= 0 && idx < filesContent.Tables.Count)
                .Select(idx => new TableRowPlan(new TableData(filesContent.Tables[idx], idx), null))
                .ToArray();
        }

        /// <summary>
        /// Whole-table plan for every table of the project content.
        /// </summary>
        internal static TableRowPlan[] ForAllTables(DataSet filesContent)
        {
            return Enumerable.Range(0, filesContent.Tables.Count)
                .Select(idx => new TableRowPlan(new TableData(filesContent.Tables[idx], idx), null))
                .ToArray();
        }
    }
}
