using System;
using System.Diagnostics;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Menus.ProjectMenus;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class OpenJsonSkipCodesList : IFileListItemMenu
    {
        public string Text => "[" + AppData.CurrentProject.Name+ "]" + T._(" Open SkipCodesList");

        public string Description => "Open skipcodes.txt file in project dir to add codes which need to be skipped for the project";

        public string Category => "";

        public void OnClick(object sender, EventArgs e)
        {
            var filePath = THSettings.RPGMakerMVSkipCodesFilePath(); ;
            if (!File.Exists(filePath)) File.WriteAllText(filePath, "; All text after ; will be ignored. Enter here codes one per line. example: '408,comment' or '108'");

            Process.Start(filePath);
        }
    }
}
