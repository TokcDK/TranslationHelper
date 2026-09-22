using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Functions;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemWriteTranslation : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Write translation");

        public override string Description => T._("Write traslated strings back into source files");

        public override int Order => base.Order + 5;

        public override async void OnClick(object sender, EventArgs e)
        {
            //The project the menu acts on, taken once: the write runs in the background and must not
            //follow the user to another project half way through.
            var project = AppData.CurrentProject;
            if (project == null) return;

            if (project.DontLoadDuplicates
                && project.TablesLinesDict != null
                && !project.TablesLinesDict.IsEmpty)
            {
                project.TablesLinesDict.Clear();
            }

            //The files list holds one entry per file, preceded by the "[ALL]" entry, so what is
            //selected are entry indexes while the save works in table indexes. Selecting "[ALL]" means
            //every file, which is the same as not restricting the save at all.
            HashSet<int> fileIndexesToWrite = null;
            if (WriteSelected)
            {
                var selectedEntries = AppData.ActiveWorkspace?.FilesList?.GetSelectedIndexes() ?? Array.Empty<int>();
                if (selectedEntries.Length == 0)
                {
                    Logger.Debug("No files selected for writing translation.");
                    return;
                }

                if (!selectedEntries.Any(project.FilesListContent.IsAllEntry))
                {
                    fileIndexesToWrite = selectedEntries
                        .Select(project.FilesListContent.GetTableIndex)
                        .Where(tableIndex => tableIndex >= 0)
                        .ToHashSet();
                }
            }

            await Task.Run(() => FunctionsSave.PrepareToWrite(fileIndexesToWrite)).ConfigureAwait(true);
            project.AfterTranslationWriteActions();

            if (project.DontLoadDuplicates) project.TablesLinesDict = null;
        }
        protected virtual bool WriteSelected { get; } = false;
    }
}
