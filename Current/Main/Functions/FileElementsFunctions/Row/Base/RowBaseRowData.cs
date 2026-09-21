using System;
using System.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// One row of one file table, as handed to a row operation: the row itself, where it comes from,
    /// and the accessors for its original and translation cells.
    /// <para>
    /// This is the whole contract between the engine and an implementation: an operation reads
    /// <see cref="Original"/>, decides, and writes <see cref="Translation"/>. It never sees the work
    /// table grid, the selection or the project DataSet.
    /// </para>
    /// </summary>
    public class RowBaseRowData
    {
        ProjectBase Project { get; }

        IUiUpdater UiUpdater { get; }

        public RowBaseRowData(ProjectBase project, DataRow row, int rowIndex, TableData table, IUiUpdater uiUpdater)
        {
            Project = project ?? throw new ArgumentNullException(nameof(project));
            UiUpdater = uiUpdater ?? throw new ArgumentNullException(nameof(uiUpdater));

            SelectedRow = row;
            SelectedRowIndex = rowIndex;
            TableData = table;
        }

        /// <summary>
        /// The table this row belongs to, and its index in the project content.
        /// </summary>
        public TableData TableData { get; }

        public DataRow SelectedRow { get; }
        public int SelectedRowIndex { get; }

        /// <summary>
        /// True while this is the last row of the table being processed. Operations that accumulate
        /// something per table use it to act once, when the table is done.
        /// </summary>
        public bool IsLastRow { get; set; }

        public int ColumnIndexOriginal => Project.OriginalColumnIndex;
        public int ColumnIndexTranslation => Project.TranslationColumnIndex;

        public string Original => SelectedRow.Field<string>(ColumnIndexOriginal);

        public string Translation
        {
            get => SelectedRow.Field<string>(ColumnIndexTranslation);
            set => UiUpdater.SetTranslation(SelectedRow, ColumnIndexTranslation, value);
        }

        public DataTable SelectedTable => TableData.SelectedTable;
        public int SelectedTableIndex => TableData.SelectedTableIndex;
    }
}
