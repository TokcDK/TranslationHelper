using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.RowParsersParallel
{
    public abstract class RowParallelParserBase
    {
        #region Shared

        /// <summary>
        /// Parse all rows in all tables
        /// </summary>
        public void All()
        {
            ParseSelectedTables(AppData.CurrentProject.FilesContent.Tables.AsParallel().OfType<DataTable>());
        }
        /// <summary>
        /// Parse all rows in selected table[s]
        /// </summary>
        public void Table()
        {
            var tables = AppData.CurrentProject.FilesContent.Tables;
            ParseSelectedTables(AppData.FilesListControl.GetSelectedIndexes().Select(i=> tables[i]));
        }
        /// <summary>
        /// Parse selected rows
        /// </summary>
        public void Row()
        {

        }

        #endregion Shared

        #region Tables
        void ParseSelectedTables(IEnumerable<DataTable> tables)
        {
            _rowsLeftToProcess = tables.Select(t => t.Rows.Count).Sum();

            Parallel.ForEach(tables, table => 
            {
                if (!IsValidTable(table)) return;

                Parse(table); 
            });
        }

        private bool IsValidTable(DataTable table)
        {
            return true;
        }

        void Parse(DataTable table)
        {
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
                IsLastRow = --_rowsLeftToProcess == 0;

                if (!IsValidRow(row)) return;

                Parse(row); 
            });
        }

        protected virtual bool IsValidRow(DataRow row)
        {
            return true;
        }

        protected abstract bool Parse(DataRow row);

        #endregion Rows
    }
}
