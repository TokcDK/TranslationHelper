using System;
using System.Diagnostics;
using TranslationHelper.Data;
using TranslationHelper.Menus.ProjectMenus;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class SkipJSFileOpen : IFileListItemMenu
    {
        public string Text => "[" + AppData.CurrentProject.Name() + "]" + T._("Skip JS") + "-->" + T._("Open");

        public string Description => "Open Skip.js file in text editor";

        public string Category => "";

        public void OnClick(object sender, EventArgs e)
        {
            Process.Start(THSettings.RPGMakerMVSkipjsRulesFilePath());
        }
    }
}
