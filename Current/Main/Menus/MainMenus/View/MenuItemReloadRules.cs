﻿using System;
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
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.Edit
{
    internal class MenuItemReloadRules : MainMenuViewSubItemBase
    {
        public override string Text => T._("ReloadRules");

        public override string Description => Text;

        public override void OnClick(object sender, EventArgs e)
        {
            FunctionRules.ReloadTranslationRegexRules();
            FunctionRules.ReloadCellFixesRegexRules();
            FunctionsSounds.LoadDBCompleted();
        }
    }
}
