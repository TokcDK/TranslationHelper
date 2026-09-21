using System.Linq;
using System.Threading;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Progress of one run: how many rows are in scope, how many are left and how many have been
    /// changed.
    /// <para>
    /// It is separate from the engine so the execution loop only reads and advances counters, and the
    /// counters can be handed to a progress report without handing out the engine.
    /// </para>
    /// </summary>
    internal class ProcessingState
    {
        /// <summary>
        /// Rows <see cref="RowBase"/> reports as changed. Counted with <see cref="IncrementParsed"/>.
        /// </summary>
        public int ParsedCount;

        /// <summary>
        /// Rows of the table currently being processed.
        /// </summary>
        public int SelectedRowsCount { get; set; }

        /// <summary>
        /// Rows of that table still to process. Zero means the current row is the last one.
        /// </summary>
        public int SelectedRowsCountRest { get; set; }

        /// <summary>
        /// Sizes the counters for a new run over <paramref name="plan"/>.
        /// </summary>
        public void Reset(TableRowPlan[] plan)
        {
            ParsedCount = 0;
            SelectedRowsCount = plan.Sum(entry => entry.RowIndexes?.Length ?? entry.Table.SelectedTable.Rows.Count);
            SelectedRowsCountRest = SelectedRowsCount;
        }

        /// <summary>
        /// Counts one changed row. Thread-safe, because rows can be processed in parallel.
        /// </summary>
        public void IncrementParsed() => Interlocked.Increment(ref ParsedCount);
    }
}
