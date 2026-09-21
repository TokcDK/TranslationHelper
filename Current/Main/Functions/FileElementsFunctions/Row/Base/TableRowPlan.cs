namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// The tables an operation has to be applied to and, for each of them, the rows to apply it to.
    /// A null <see cref="RowIndexes"/> means every row of the table.
    /// <para>
    /// A list of these is how an operation expresses its scope, because the entry selected in the
    /// files list is not necessarily one table: the "[ALL]" entry is several files at once.
    /// </para>
    /// </summary>
    public sealed class TableRowPlan
    {
        public TableRowPlan(TableData table, int[] rowIndexes)
        {
            Table = table;
            RowIndexes = rowIndexes;
        }

        /// <summary>
        /// The table to apply the operation to.
        /// </summary>
        public TableData Table { get; }

        /// <summary>
        /// The rows of that table to apply it to, or null for every row of it.
        /// </summary>
        public int[] RowIndexes { get; }
    }
}
