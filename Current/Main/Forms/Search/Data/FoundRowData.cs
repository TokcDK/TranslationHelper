using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Forms.Search.Data
{
    public class FoundRowData
    {
        private static readonly int _originalColumnIndex = AppData.CurrentProject.OriginalColumnIndex;
        private static readonly int _translationColumnIndex = AppData.CurrentProject.TranslationColumnIndex;

        public FoundRowData(DataRow row)
        {
            Row = row;
            TableIndex = AppData.CurrentProject.FilesContent.Tables.IndexOf(row.Table);
            RowIndex = row.Table.Rows.IndexOf(row);
        }

        [Browsable(false)]
        public DataRow Row { get; }
        public string Original => Row.Field<string>(_originalColumnIndex);
        public string Translation
        {
            get => Row.Field<string>(_translationColumnIndex);
            set => Row.SetField(_translationColumnIndex, value);
        }

        [Browsable(false)]
        public int TableIndex { get; }
        [Browsable(false)]
        public int RowIndex { get; }
    }
}
