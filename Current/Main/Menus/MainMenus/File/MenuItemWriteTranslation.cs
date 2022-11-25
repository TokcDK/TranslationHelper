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
    internal class MenuItemWriteTranslation : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Write translation");

        public override string Description => T._("Write traslated strings back into source files");

        public override async void OnClick(object sender, EventArgs e)
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
        public override int Order => base.Order + 5;
    }
}
