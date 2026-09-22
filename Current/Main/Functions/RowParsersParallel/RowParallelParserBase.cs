using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions.FilesListControl;
using TranslationHelper.Projects;
using TranslationHelper.Workspace;

namespace TranslationHelper.Functions.RowParsersParallel
{
    /// <summary>
    /// Base for row parsers that run over many rows in parallel.
    /// <para>
    /// <c>internal</c> like the rest of the row framework: it hands out <see cref="IProjectWorkspace"/>,
    /// which is an application-internal contract, so it cannot be part of a public surface.
    /// </para>
    /// </summary>
    internal abstract class RowParallelParserBase
    {
        public class DataRowData
        {
            /// <summary>
            /// </summary>
            /// <param name="row">The row being processed.</param>
            /// <param name="originalColumnIndex">Index of the Original column of the row's project.</param>
            /// <param name="translationColumnIndex">Index of the Translation column of the row's project.</param>
            public DataRowData(DataRow row, int originalColumnIndex, int translationColumnIndex)
            {
                Row = row;
                OriginalColumnIndex = originalColumnIndex;
                TranslationColumnIndex = translationColumnIndex;
            }

            int OriginalColumnIndex { get; }
            int TranslationColumnIndex { get; }

            public DataRow Row { get; }

            public string Original { get => Row.Field<string>(OriginalColumnIndex); }

            public string Translation
            {
                get => Row.Field<string>(TranslationColumnIndex);
                set => Row.SetValue(TranslationColumnIndex, value);
            }
        }

        #region General

        /// <summary>
        /// The project the parse works on, and its controls.
        /// <para>
        /// Captured when the parser is built rather than read from <see cref="AppData"/> on every use,
        /// so a parse covers the project it was started for even if the user selects another one while
        /// it runs. Reading "the" project and "the" files list on each use was equivalent only while
        /// there could not be more than one.
        /// </para>
        /// </summary>
        protected readonly IProjectWorkspace Workspace = AppData.ActiveWorkspace;

        /// <summary>
        /// The project the parse works on. Falls back to the selected project for a parser built while
        /// no project is on screen, which is what a caller that drives the parse itself expects.
        /// </summary>
        protected ProjectBase Project => Workspace?.Project ?? AppData.CurrentProject;

        /// <summary>
        /// The project's files list. The abstraction rather than its <see cref="ListBox"/>, because
        /// what the list selection *means* — including the offset the "[ALL]" entry introduces — is
        /// the abstraction's business.
        /// </summary>
        protected FilesListControlBase FilesList => Workspace?.FilesList;

        protected DataSet AllTables => Project?.FilesContent;

        protected DataGridView WorkTableDatagridView => Workspace?.ActiveFileWorkspace?.ElementsDataGridView;

        #endregion General

        #region Shared

        /// <summary>
        /// Parse all rows in all tables
        /// </summary>
        public async Task All()
        {
            await Task.Run(() => ParseSelectedTables(AllTables.Tables.AsParallel().OfType<DataTable>())).ConfigureAwait(false);
        }
        /// <summary>
        /// Parse all rows in selected table[s]
        /// </summary>
        public async Task Tables()
        {
            var filesList = FilesList;
            var filesListContent = Project?.FilesListContent;
            if (filesList == null || filesListContent == null) return;

            //GetSelectedIndexes returns indexes in the files list, which are not table indexes: the
            //list starts with the "[ALL]" entry. The mapping has to be asked for, otherwise the wrong
            //file is parsed and the last index is out of range.
            var tableIndexes = filesList
                .GetSelectedIndexes()
                .SelectMany(filesListContent.GetTableIndexes)
                .Distinct()
                .ToArray();

            var allTables = AllTables;
            if (allTables == null) return;

            await Task.Run(() => ParseSelectedTables(tableIndexes.Select(i => allTables.Tables[i]))).ConfigureAwait(false);
        }

        /// <summary>
        /// Parse selected rows
        /// </summary>
        public async Task Rows()
        {
            var grid = WorkTableDatagridView;
            if (grid == null) return;

            var selectedRowsIndexes = grid.GetSelectedRowsIndexes().OrderBy(i => i).ToArray();

            _rowsLeftToProcess = selectedRowsIndexes.Length;

            await Task.Run(() => ParseSelectedRows(grid.EnumerateSelectedRowsByRealRowIndexes(selectedRowsIndexes))).ConfigureAwait(false);
        }

        #endregion Shared

        #region Tables
        void ParseSelectedTables(IEnumerable<DataTable> tables)
        {
            _rowsLeftToProcess = tables.Select(t => t.Rows.Count).Sum();

            Parallel.ForEach(tables, table =>
            {
                Parse(table);
            });
        }

        protected virtual bool IsValidTable(DataTable table)
        {
            return true;
        }

        void Parse(DataTable table)
        {
            if (!IsValidTable(table)) return;

            ParseSelectedRows(table.Rows.AsParallel().OfType<DataRow>());
        }

        #endregion Tables

        #region Rows

        /// <summary>
        /// Determine if the row is last processing
        /// </summary>
        protected bool IsLastRow = false;
        int _rowsLeftToProcess = 0;

        void ParseSelectedRows(IEnumerable<DataRow> rows)
        {
            Parallel.ForEach(rows, row =>
            {
                Parse(row);
            });
        }

        protected virtual bool IsValidRow(DataRowData row)
        {
            return true;
        }

        bool Parse(DataRow row)
        {
            //Reached from Parallel.ForEach, so the read-modify-write has to be atomic.
            IsLastRow = Interlocked.Decrement(ref _rowsLeftToProcess) == 0;

            var project = Project;
            var rowData = new DataRowData(
                row,
                project?.OriginalColumnIndex ?? 0,
                project?.TranslationColumnIndex ?? 1);

            return IsValidRow(rowData) && Process(rowData);
        }

        protected abstract bool Process(DataRowData row);

        #endregion Rows
    }
}
