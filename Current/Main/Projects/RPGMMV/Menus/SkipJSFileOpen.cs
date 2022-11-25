using System;
using System.Diagnostics;
using TranslationHelper.Data;
using TranslationHelper.Menus;
using TranslationHelper.Menus.FilesListMenus;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class SkipJSFileOpen : FileListMenuItemBase, IProjectSpecifiedMenuItem
    {
        public override string Text => "[" + AppData.CurrentProject.Name+ "]" + T._("Skip JS") + "-->" + T._("Open");

        public override string Description => "Open Skip.js file in text editor";

        public override void OnClick(object sender, EventArgs e)
        {
            Process.Start(THSettings.RPGMakerMVSkipjsRulesFilePath);
        }
    }
}
