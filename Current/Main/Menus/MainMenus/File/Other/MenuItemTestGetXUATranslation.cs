using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.MainMenus.File.Other;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File
{
    internal class MenuItemTestGetXUATranslation : MainMenuFileSubItemOtherBase, IProjectMenuItem
    {
        public override string Text => "test get xua translation";

        public override void OnClick(object sender, EventArgs e)
        {
            //get static XUA translation
            using (var wc = new WebClient())
            {
                try
                {
                    var dict = new Dictionary<string, string>();
                    FunctionsUI.ProgressInfo(true, T._("Loading") + ": " + T._("Static translations") + "-" + "XUA");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;//tls12 for github
                    var xuaStatic = wc.DownloadString(new Uri("https://github.com/bbepis/XUnity.AutoTranslator/raw/master/src/XUnity.AutoTranslator.Plugin.Core/Translations/StaticTranslations.txt"));
                    foreach (var line in xuaStatic.SplitToLines())
                    {
                        if (line.Length == 0 || !line.Contains("=")) continue;

                        var data = line.Split('=');

                        if (dict.ContainsKey(data[0])) continue;

                        dict.Add(data[0], data[1]);
                    }

                    FunctionsDBFile.WriteDictToXMLDB(dict, Path.Combine(THSettings.DBDirPath, "XUA.cmx"));
                }
                catch { }
            }
            FunctionsUI.ProgressInfo(false);
        }
    }
}
