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
            if (AppData.CurrentProject.DontLoadDuplicates
                && AppData.CurrentProject.TablesLinesDict != null
                && !AppData.CurrentProject.TablesLinesDict.IsEmpty)
            {
                AppData.CurrentProject.TablesLinesDict.Clear();
            }

            if (WriteSelected && AppData.THFilesList.SelectedIndices.Count == 0)
            {
                Logger.Debug("No files selected for writing translation.");
                return;
            }

            HashSet<int> fileIndexesToWrite = WriteSelected ? 
                new HashSet<int>(AppData.THFilesList.SelectedIndices.Cast<int>()) : 
                null;

            await Task.Run(() => FunctionsSave.PrepareToWrite(fileIndexesToWrite)).ConfigureAwait(true);
            AppData.CurrentProject.AfterTranslationWriteActions();

            if (AppData.CurrentProject.DontLoadDuplicates) AppData.CurrentProject.TablesLinesDict = null;
        }
        protected virtual bool WriteSelected { get; } = false;
    }
}
