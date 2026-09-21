using System.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// A file table together with its index within the project content DataSet.
    /// <para>
    /// Both are carried together because a row operation needs the table to work on and the index to
    /// report coordinates with, and deriving one from the other is a DataSet lookup nobody should
    /// have to repeat.
    /// </para>
    /// </summary>
    public class TableData
    {
        public TableData(DataTable selectedTable, int selectedTableIndex)
        {
            SelectedTable = selectedTable;
            SelectedTableIndex = selectedTableIndex;
        }

        public DataTable SelectedTable { get; }
        public int SelectedTableIndex { get; }
    }
}
