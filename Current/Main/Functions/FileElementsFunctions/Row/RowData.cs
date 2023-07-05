using System.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class RowData
    {
        public bool IsLastRow { get; set; }
        public DataRow SelectedRow { get; set; }
        public DataTable SelectedTable { get; set; }
        public string Original { get; set; }
        public string Translation { get; set; }

        // Add any other properties related to a row here

        public RowData(DataRow row)
        {
            SelectedRow = row;
            // Initialize other properties here
        }
    }
}
