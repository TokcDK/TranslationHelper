using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.FileRowMenus;
using TranslationHelper.Menus.MainMenus.File;
using TranslationHelper.Menus.MainMenus.View;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemOpenProjectsDir : OpenPathByExplorerMainMenuViewSubItemBase, IProjectMenuItem
    {
        public override string Name => T._("Selected projects dir");

        public override string DirPath => "";

        public override void OnClick(object sender, EventArgs e)
        {
            FunctionsProcess.OpenProjectsDir();
        }
    }
}
