using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Menus.MenuTypes;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemWriteTranslation : IMainMenuItem
    {
        public string ParentMenuName => T._("File");

        public string Text => T._("Write translation");

        public string Description => T._("Write traslated strings back into source files");

        public string CategoryName => "";

        public async void OnClick(object sender, EventArgs e)
        {
            if (AppData.CurrentProject.DontLoadDuplicates 
                && AppData.CurrentProject.TablesLinesDict != null 
                && !AppData.CurrentProject.TablesLinesDict.IsEmpty)
            {
                AppData.CurrentProject.TablesLinesDict.Clear();
            }
            await Task.Run(() => new FunctionsSave().PrepareToWrite()).ConfigureAwait(true);
            AppData.CurrentProject.AfterTranslationWriteActions();

            if (AppData.CurrentProject.DontLoadDuplicates) AppData.CurrentProject.TablesLinesDict = null;
        }
    }
}
