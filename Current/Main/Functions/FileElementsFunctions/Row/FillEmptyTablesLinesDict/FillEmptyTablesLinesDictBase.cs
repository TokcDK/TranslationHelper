using System.Collections.Concurrent;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict
{
    abstract class FillEmptyTablesLinesDictBase : RowBase
    {
        protected override bool IsOkSelected(TableData tableData)
        {
            return false;
        }

        protected override bool IsOkTable(TableData tableData)
        {
            return IsOkSelected(tableData);
        }

        /// <summary>
        /// Run force even if not save mode and donotloadduplicates is false
        /// </summary>
        protected virtual bool ForceRun => false;

        bool _checked;
        protected override bool IsOkAll()
        {
            if (!_checked && Translations == null && Project.TablesLinesDict != null && Project.TablesLinesDict.Count > 0)
            {
                _checked= true;
                Translations = Project.TablesLinesDict;
                return false;
            }

            return (!ForceRun || (Project.DontLoadDuplicates)) && (Translations == null || Translations.Count == 0);
        }

        protected override bool IsValidRow(RowBaseRowData rowData)
        {
            return IsAll && base.IsValidRow(rowData) && !string.IsNullOrWhiteSpace(rowData.SelectedRow[0].ToString()) && !string.IsNullOrWhiteSpace(rowData.SelectedRow[1] + string.Empty);
        }

        internal ConcurrentDictionary<string, string> Translations;

        protected FillEmptyTablesLinesDictBase()
        {
            Translations = new ConcurrentDictionary<string, string>();
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            Translations.TryAdd(rowData.SelectedRow[0] + "", rowData.SelectedRow[1] + "");

            return true;
        }
    }
}
