using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace TranslationHelper.Functions.FilesListControl
{
    /// <summary>
    /// The content the "[ALL]" entry of the files list presents: the rows of every file of the opened
    /// project in one table, in file order.
    /// <para>
    /// A <see cref="DataTable"/> cannot hold another table's <see cref="DataRow"/> objects, so this
    /// class keeps its own rows and remembers, for each of them, the file row it was copied from. Both
    /// directions of that relation are kept live:
    /// </para>
    /// <list type="bullet">
    /// <item>a value written to a cell of this table is written to the same cell of the file row
    /// immediately, which is what makes an edit in the "[ALL]" entry change the file;</item>
    /// <item>a value written to a file row is copied into this table, so the "[ALL]" entry does not
    /// keep showing a value that an operation working on the files directly has already replaced.</item>
    /// </list>
    /// <para>
    /// The relation is held as <see cref="DataRow"/> references rather than as row indexes, because an
    /// index goes stale the moment a row is inserted or removed while a reference does not. A row index
    /// is worked out from the reference only when a caller asks for one, so it is always the index the
    /// row has right now.
    /// </para>
    /// <para>
    /// This table is a view. It is deliberately not a member of
    /// <see cref="Projects.ProjectBase.FilesContent"/>, so it is never written to a file and never
    /// counted as one.
    /// </para>
    /// </summary>
    internal sealed class AllFilesContentTable : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Name of this table and of the files list entry that presents it.
        /// </summary>
        internal const string TableName = "[ALL]";

        /// <summary>
        /// Provides the content of the project that is open right now. Read on every use rather than
        /// captured once, so one instance of this class survives opening one project after another.
        /// </summary>
        private readonly Func<DataSet> _filesContentProvider;

        /// <summary>
        /// Guards the two maps and <see cref="_attached"/> against the row operations that run on
        /// worker threads.
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// The file tables whose changes are currently copied into this table.
        /// </summary>
        private readonly List<DataTable> _attached = new List<DataTable>();

        /// <summary>
        /// The file row each row of this table was copied from. Keyed by reference: a
        /// <see cref="DataRow"/> is compared by identity, so two file rows holding equal values stay
        /// two separate keys.
        /// </summary>
        private readonly Dictionary<DataRow, DataRow> _sourceOf = new Dictionary<DataRow, DataRow>();

        /// <summary>
        /// The reverse of <see cref="_sourceOf"/>: the row of this table that mirrors a file row.
        /// </summary>
        private readonly Dictionary<DataRow, DataRow> _copyOf = new Dictionary<DataRow, DataRow>();

        /// <summary>
        /// True while <see cref="Rebuild"/> or <see cref="Clear"/> fills the table. Filling is not
        /// editing, so neither direction of the synchronisation may act then.
        /// </summary>
        private bool _isRebuilding;

        /// <summary>
        /// True on the thread that is currently copying a value from one table to the other. It is
        /// thread-static because a row operation can run on several threads at once, and one thread's
        /// copy must not silence another thread's.
        /// </summary>
        [ThreadStatic]
        private static bool _isForwarding;

        internal AllFilesContentTable(Func<DataSet> filesContentProvider)
        {
            _filesContentProvider = filesContentProvider ?? throw new ArgumentNullException(nameof(filesContentProvider));

            Table = new DataTable(TableName);
            Table.ColumnChanging += OnTableColumnChanging;
            Table.RowDeleted += OnTableRowDeleted;
        }

        /// <summary>
        /// The table a grid is bound to while the "[ALL]" entry is selected.
        /// </summary>
        internal DataTable Table { get; }

        /// <summary>
        /// Number of rows of this table, which is the number of rows of all files together.
        /// </summary>
        internal int RowCount => Table.Rows.Count;

        /// <summary>
        /// Rebuild the rows from the files as they are now. The columns become the union of the
        /// columns of the files, so a project whose files carry extra columns shows them too.
        /// </summary>
        internal void Rebuild()
        {
            lock (_locker)
            {
                _isRebuilding = true;
                try
                {
                    Table.Rows.Clear();
                    _sourceOf.Clear();
                    _copyOf.Clear();

                    BuildColumns();

                    var tables = FilesContent?.Tables;
                    if (tables == null) return;

                    int tablesCount = tables.Count;
                    for (int tableIndex = 0; tableIndex < tablesCount; tableIndex++)
                    {
                        var source = tables[tableIndex];
                        if (!IsFileTable(source)) continue;

                        foreach (DataRow sourceRow in source.Rows)
                        {
                            // A removed row stays in the collection until the deletion is accepted, and
                            // it has no values left to copy.
                            if (sourceRow.RowState == DataRowState.Deleted) continue;

                            var copy = Table.NewRow();
                            CopyRowValues(sourceRow, copy);
                            Table.Rows.Add(copy);

                            _sourceOf[copy] = sourceRow;
                            _copyOf[sourceRow] = copy;
                        }
                    }

                    // The rows are a copy of settled file rows, not pending edits of this table.
                    Table.AcceptChanges();
                }
                finally
                {
                    _isRebuilding = false;
                }
            }
        }

        /// <summary>
        /// Drop every row and column. Used when a project is closed, so the next project does not
        /// inherit the columns of the previous one.
        /// </summary>
        internal void Clear()
        {
            lock (_locker)
            {
                _isRebuilding = true;
                try
                {
                    _sourceOf.Clear();
                    _copyOf.Clear();
                    Table.Rows.Clear();
                    Table.Columns.Clear();
                }
                finally
                {
                    _isRebuilding = false;
                }
            }
        }

        /// <summary>
        /// Report the file table and row that <paramref name="rowIndex"/> of this table presents.
        /// </summary>
        /// <param name="rowIndex">Index of a row of this table, not a grid row index.</param>
        /// <param name="tableIndex">Index of the file table in the project content.</param>
        /// <param name="sourceRowIndex">Index of the row in that file table.</param>
        /// <returns>True when the row was taken from a file table.</returns>
        internal bool TryGetOrigin(int rowIndex, out int tableIndex, out int sourceRowIndex)
        {
            tableIndex = -1;
            sourceRowIndex = -1;

            DataRow source;
            lock (_locker)
            {
                var rows = Table.Rows;
                if (rowIndex < 0 || rowIndex >= rows.Count) return false;

                var copy = rows[rowIndex];
                if (copy.RowState == DataRowState.Deleted || copy.RowState == DataRowState.Detached) return false;
                if (!_sourceOf.TryGetValue(copy, out source)) return false;
            }

            return TryIndexOf(source, out tableIndex, out sourceRowIndex);
        }

        /// <summary>
        /// Start copying changes of the file tables into this table. Called while the "[ALL]" entry is
        /// the displayed one, so the display follows the operations that work on the files directly
        /// instead of going stale until the entry is selected again.
        /// </summary>
        internal void AttachToFiles()
        {
            lock (_locker)
            {
                if (_attached.Count > 0) return;

                var tables = FilesContent?.Tables;
                if (tables == null) return;

                int tablesCount = tables.Count;
                for (int tableIndex = 0; tableIndex < tablesCount; tableIndex++)
                {
                    var source = tables[tableIndex];
                    if (!IsFileTable(source)) continue;

                    source.ColumnChanged += OnFileColumnChanged;
                    _attached.Add(source);
                }
            }
        }

        /// <summary>
        /// Stop copying changes of the file tables into this table.
        /// </summary>
        internal void DetachFromFiles()
        {
            lock (_locker)
            {
                int attachedCount = _attached.Count;
                for (int i = 0; i < attachedCount; i++)
                {
                    _attached[i].ColumnChanged -= OnFileColumnChanged;
                }

                _attached.Clear();
            }
        }

        /// <summary>
        /// The content of the project that is open right now, or null when no project is open.
        /// </summary>
        private DataSet FilesContent => _filesContentProvider();

        /// <summary>
        /// True for a table that holds file content, which excludes this table and the empty slots a
        /// <see cref="DataSet"/> may have.
        /// </summary>
        private bool IsFileTable(DataTable table)
        {
            return table != null && !ReferenceEquals(table, Table);
        }

        /// <summary>
        /// Give this table the columns of every file, in the order in which the files present them.
        /// </summary>
        private void BuildColumns()
        {
            var tables = FilesContent?.Tables;
            if (tables == null) return;

            int tablesCount = tables.Count;
            for (int tableIndex = 0; tableIndex < tablesCount; tableIndex++)
            {
                var source = tables[tableIndex];
                if (!IsFileTable(source)) continue;

                foreach (DataColumn column in source.Columns)
                {
                    if (Table.Columns.Contains(column.ColumnName)) continue;

                    Table.Columns.Add(column.ColumnName, column.DataType);
                }
            }
        }

        /// <summary>
        /// Copy every value of <paramref name="source"/> that this table has a column for.
        /// </summary>
        private static void CopyRowValues(DataRow source, DataRow target)
        {
            var sourceColumns = source.Table.Columns;
            foreach (DataColumn targetColumn in target.Table.Columns)
            {
                if (!sourceColumns.Contains(targetColumn.ColumnName)) continue;

                target[targetColumn.ColumnName] = source[targetColumn.ColumnName];
            }
        }

        /// <summary>
        /// A cell of this table is being edited: write the same value into the file row it came from.
        /// </summary>
        private void OnTableColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            if (_isRebuilding || _isForwarding) return;

            // Only the lookup needs the lock. Once the file row is in hand, writing to it is safe: the
            // row object is the row, whatever its index becomes.
            DataRow sourceRow;
            lock (_locker)
            {
                if (!_sourceOf.TryGetValue(e.Row, out sourceRow)) return;
            }

            if (sourceRow.RowState == DataRowState.Deleted || sourceRow.RowState == DataRowState.Detached) return;
            if (!sourceRow.Table.Columns.Contains(e.Column.ColumnName)) return;

            _isForwarding = true;
            try
            {
                sourceRow[e.Column.ColumnName] = e.ProposedValue;
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to write an edited value of \"{0}\" back to {1}. Error: {2}", TableName, sourceRow.Table.TableName, ex);
            }
            finally
            {
                _isForwarding = false;
            }
        }

        /// <summary>
        /// A row of this table is being removed: forget where it came from.
        /// </summary>
        private void OnTableRowDeleted(object sender, DataRowChangeEventArgs e)
        {
            if (_isRebuilding) return;

            lock (_locker)
            {
                if (!_sourceOf.TryGetValue(e.Row, out var source)) return;

                _sourceOf.Remove(e.Row);
                _copyOf.Remove(source);
            }
        }

        /// <summary>
        /// A cell of a file row changed: show the new value here as well.
        /// </summary>
        private void OnFileColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (_isRebuilding || _isForwarding) return;

            DataRow copy;
            lock (_locker)
            {
                if (!_copyOf.TryGetValue(e.Row, out copy)) return;
            }

            if (copy.RowState == DataRowState.Deleted || copy.RowState == DataRowState.Detached) return;
            if (!copy.Table.Columns.Contains(e.Column.ColumnName)) return;

            _isForwarding = true;
            try
            {
                copy[e.Column.ColumnName] = e.ProposedValue;
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to show a change of {0} in \"{1}\". Error: {2}", e.Row.Table.TableName, TableName, ex);
            }
            finally
            {
                _isForwarding = false;
            }
        }

        /// <summary>
        /// Report where <paramref name="source"/> sits now: which file table holds it, and at which
        /// index of that table. Both are worked out on the spot, so neither can be out of date.
        /// </summary>
        private bool TryIndexOf(DataRow source, out int tableIndex, out int sourceRowIndex)
        {
            tableIndex = -1;
            sourceRowIndex = -1;

            if (source == null) return false;

            var tables = FilesContent?.Tables;
            if (tables == null) return false;

            tableIndex = tables.IndexOf(source.Table);
            if (tableIndex < 0) return false;

            sourceRowIndex = source.Table.Rows.IndexOf(source);
            return sourceRowIndex >= 0;
        }

        /// <summary>
        /// Stop copying changes and release the subscriptions this table holds.
        /// </summary>
        public void Dispose()
        {
            DetachFromFiles();

            lock (_locker)
            {
                _sourceOf.Clear();
                _copyOf.Clear();
            }

            Table.ColumnChanging -= OnTableColumnChanging;
            Table.RowDeleted -= OnTableRowDeleted;
        }
    }
}
