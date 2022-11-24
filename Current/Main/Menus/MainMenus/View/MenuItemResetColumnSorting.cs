using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Menus.MenuTypes;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemResetColumnSorting : MainMenuViewSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Reset column sorting");

        public override string Description => Text;

        public override void OnClick(object sender, EventArgs e)
        {
            AppData.CurrentProject.FilesContent.Tables[AppData.Main.THFilesList.GetSelectedIndex()].DefaultView.Sort = string.Empty;
        }
    }
}
