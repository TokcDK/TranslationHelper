using System.Collections.Concurrent;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict
{
    abstract class FillEmptyTablesLinesDictBase : RowBase
    {
        protected override bool IsOkSelected()
        {
            return false;
        }

        protected override bool IsOkTable()
        {
            return IsOkSelected();
        }

        /// <summary>
        /// Run force even if not save mode and donotloadduplicates is false
        /// </summary>
        protected virtual bool ForceRun => false;

        bool _checked;
        protected override bool IsOkAll()
        {
            if (!_checked && Translations == null && ProjectData.CurrentProject.TablesLinesDict != null && ProjectData.CurrentProject.TablesLinesDict.Count > 0)
            {
                _checked= true;
                Translations = ProjectData.CurrentProject.TablesLinesDict;
                return false;
            }

            return (!ForceRun || (ProjectData.SaveFileMode && Properties.Settings.Default.DontLoadDuplicates)) && (Translations == null || Translations.Count == 0);
        }

        protected override bool IsValidRow()
        {
            return IsAll && base.IsValidRow() && !string.IsNullOrWhiteSpace(SelectedRow[0].ToString()) && !string.IsNullOrWhiteSpace(SelectedRow[1] + string.Empty);
        }

        internal ConcurrentDictionary<string, string> Translations;

        protected FillEmptyTablesLinesDictBase()
        {
            Translations = new ConcurrentDictionary<string, string>();
        }

        protected override bool Apply()
        {
            Translations.TryAdd(SelectedRow[0] + "", SelectedRow[1] + "");

            return true;
        }
    }
}
