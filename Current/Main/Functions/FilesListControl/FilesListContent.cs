using System;
using System.Collections.Generic;
using System.Data;

namespace TranslationHelper.Functions.FilesListControl
{
    /// <summary>
    /// Relates the entries of the files list to the content they present.
    /// <para>
    /// The list holds one entry per file of the opened project, preceded by the "[ALL]" entry that
    /// presents every file at once. An <em>entry index</em> is a position in that list; a
    /// <em>table index</em> is a position in <see cref="Projects.ProjectBase.FilesContent"/>. The two
    /// are different numbers, and before this class existed every caller used the entry index as if it
    /// were a table index, which is what made adding an entry at the top of the list impossible. This
    /// class is the single place that converts one into the other.
    /// </para>
    /// <para>
    /// The aggregate content is reachable only through the entry index 0, and only while
    /// <see cref="Initialize"/> has been called. Until then every entry index is a table index, so the
    /// code paths that fill the list themselves keep working exactly as they did.
    /// </para>
    /// </summary>
    internal sealed class FilesListContent : IDisposable
    {
        /// <summary>
        /// Name of the aggregate entry, which is always the first entry of the list.
        /// </summary>
        internal const string AllEntryName = AllFilesContentTable.TableName;

        /// <summary>
        /// Provides the content of the project that is open right now, or null when none is open.
        /// </summary>
        private readonly Func<DataSet> _filesContentProvider;

        /// <summary>
        /// The aggregate content the "[ALL]" entry presents.
        /// </summary>
        private readonly AllFilesContentTable _all;

        /// <summary>
        /// True while the "[ALL]" entry is part of the list.
        /// </summary>
        private bool _hasAllEntry;

        internal FilesListContent(Func<DataSet> filesContentProvider)
        {
            _filesContentProvider = filesContentProvider ?? throw new ArgumentNullException(nameof(filesContentProvider));
            _all = new AllFilesContentTable(filesContentProvider);
        }

        /// <summary>
        /// The content of the project that is open right now, or null when no project is open.
        /// </summary>
        private DataSet FilesContent => _filesContentProvider();

        /// <summary>
        /// Number of entries the list holds.
        /// </summary>
        internal int EntriesCount => (FilesContent?.Tables.Count ?? 0) + (_hasAllEntry ? 1 : 0);

        /// <summary>
        /// Build the aggregate content and make the "[ALL]" entry the first entry of the list. Has to
        /// be called after the project's file tables are complete and before the list is filled.
        /// </summary>
        internal void Initialize()
        {
            _hasAllEntry = (FilesContent?.Tables.Count ?? 0) > 0;
            _all.Rebuild();
        }

        /// <summary>
        /// Drop the aggregate content and the "[ALL]" entry. The list returns to holding one entry per
        /// file, which is its state before a project is opened.
        /// </summary>
        internal void Reset()
        {
            _all.DetachFromFiles();
            _all.Clear();
            _hasAllEntry = false;
        }

        /// <summary>
        /// Names of the entries, in list order.
        /// </summary>
        internal IEnumerable<string> GetEntryNames()
        {
            if (_hasAllEntry) yield return AllEntryName;

            var tables = FilesContent?.Tables;
            if (tables == null) yield break;

            foreach (DataTable table in tables)
            {
                yield return table.TableName;
            }
        }

        /// <summary>
        /// True when the entry at <paramref name="listIndex"/> is the aggregate entry.
        /// </summary>
        internal bool IsAllEntry(int listIndex) => _hasAllEntry && listIndex == 0;

        /// <summary>
        /// Table index the entry presents, or -1 when the entry is the aggregate one or the index is
        /// out of range.
        /// </summary>
        internal int GetTableIndex(int listIndex)
        {
            if (listIndex < 0) return -1;

            if (_hasAllEntry)
            {
                if (listIndex == 0) return -1;
                listIndex--;
            }

            var tables = FilesContent?.Tables;
            return tables != null && listIndex < tables.Count ? listIndex : -1;
        }

        /// <summary>
        /// Entry index that presents the table at <paramref name="tableIndex"/>, or -1 when there is
        /// no such table.
        /// </summary>
        internal int GetListIndex(int tableIndex)
        {
            var tables = FilesContent?.Tables;
            if (tables == null || tableIndex < 0 || tableIndex >= tables.Count) return -1;

            return tableIndex + (_hasAllEntry ? 1 : 0);
        }

        /// <summary>
        /// The table to bind for the entry: the aggregate content for the "[ALL]" entry, the file's
        /// own table otherwise. Null when the index is out of range.
        /// </summary>
        internal DataTable GetTable(int listIndex)
        {
            if (IsAllEntry(listIndex)) return _all.Table;

            int tableIndex = GetTableIndex(listIndex);
            if (tableIndex < 0) return null;

            var tables = FilesContent?.Tables;
            return tables == null ? null : tables[tableIndex];
        }

        /// <summary>
        /// Table indexes the entry covers, in order: every file for the aggregate entry, the one file
        /// otherwise.
        /// </summary>
        internal int[] GetTableIndexes(int listIndex)
        {
            var tables = FilesContent?.Tables;
            if (tables == null) return Array.Empty<int>();

            if (IsAllEntry(listIndex))
            {
                int tablesCount = tables.Count;
                var all = new int[tablesCount];
                for (int i = 0; i < tablesCount; i++) all[i] = i;

                return all;
            }

            int tableIndex = GetTableIndex(listIndex);
            return tableIndex < 0 ? Array.Empty<int>() : new[] { tableIndex };
        }

        /// <summary>
        /// Report which file table and row a row of the entry's table presents. For a file entry that
        /// is the row itself; for the aggregate entry it is the file row the row was copied from.
        /// </summary>
        /// <param name="listIndex">Index of the entry in the files list.</param>
        /// <param name="rowIndex">
        /// Index of the row in the entry's table — not a grid row index. A caller holding a grid row
        /// index has to resolve it first, because the grid may be showing the table sorted or filtered.
        /// </param>
        /// <param name="tableIndex">Index of the file table in the project content.</param>
        /// <param name="sourceRowIndex">Index of the row in that file table.</param>
        /// <returns>True when the row was resolved to a file row.</returns>
        internal bool TryResolveRow(int listIndex, int rowIndex, out int tableIndex, out int sourceRowIndex)
        {
            tableIndex = -1;
            sourceRowIndex = -1;

            if (rowIndex < 0) return false;

            if (IsAllEntry(listIndex)) return _all.TryGetOrigin(rowIndex, out tableIndex, out sourceRowIndex);

            tableIndex = GetTableIndex(listIndex);
            if (tableIndex < 0) return false;

            var tables = FilesContent?.Tables;
            if (tables == null || rowIndex >= tables[tableIndex].Rows.Count) return false;

            sourceRowIndex = rowIndex;
            return true;
        }

        /// <summary>
        /// Rebuild the aggregate content from the files as they are now. Called when the "[ALL]" entry
        /// becomes the displayed one, so it never shows what the files held the last time it was shown.
        /// </summary>
        internal void Refresh() => _all.Rebuild();

        /// <summary>
        /// Keep the aggregate content in step with the file tables while it is displayed.
        /// </summary>
        internal void AttachToFiles() => _all.AttachToFiles();

        /// <summary>
        /// Stop keeping the aggregate content in step with the file tables.
        /// </summary>
        internal void DetachFromFiles() => _all.DetachFromFiles();

        public void Dispose() => _all.Dispose();
    }
}
