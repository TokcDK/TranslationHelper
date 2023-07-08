using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using TranslationHelper.Data;
using TranslationHelper.Formats.RimWorld;
using TranslationHelper.Formats.zzzOtherFormat;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus;
using TranslationHelper.Menus.FilesListMenus;
using TranslationHelper.Menus.MainMenus.Edit;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class MainMenuTryReadEnglishTranslations : MainMenuEditSubItemBase, IProjectSpecifiedMenuItem
    {
        public override string Text => "[" + AppData.CurrentProject.Name+ "]" + T._("Try read English language strings into translations");

        public override string Description => T._("Try read translation from same elements of English language if present");

        public async override void OnClick(object sender, EventArgs e)
        {
            await new FileRowTryReadEnglishTranslations().AllT();
        }
    }

    class FileRowTryReadEnglishTranslations : RowBase
    {
        protected override bool IsOkAll()
        {
            var languagesDirMatch = Regex.Match(AppData.CurrentProject.SelectedDir, @"^(.+\\Languages)\\([a-zA-Z -_]+)\\");
            if (!languagesDirMatch.Success) return false;

            if (string.Equals(languagesDirMatch.Groups[2].Value, "English", StringComparison.InvariantCultureIgnoreCase)) return false; // dont need if selected English

            _languagesDir = Path.Combine(languagesDirMatch.Groups[1].Value, "English");

            FillDict();

            if(dict.Count==0) return false;

            return true;
        }

        string _languagesDir = "";

        protected override bool Apply(RowData rowData)
        {
            try
            {
                var o = Original;
                if (dict.TryGetValue(o, out string value)) Translation = value;
            }
            catch
            {
                return false;
            }
            return true;
        }

        readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        bool FillDict()
        {
            var ret = false;

            foreach (var xml in Directory.GetFiles(_languagesDir, "*.xml", SearchOption.AllDirectories))
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    FileStream fs = new FileStream(xml, FileMode.Open, FileAccess.Read);
                    xmldoc.Load(fs);
                    var languageDataNode = xmldoc.GetElementsByTagName("LanguageData");
                    foreach (XmlNode subnode in languageDataNode[0].ChildNodes)
                    {
                        if (!(subnode is XmlElement el)) continue;

                        if (string.IsNullOrWhiteSpace(el.Name)) continue;
                        if (string.IsNullOrWhiteSpace(el.InnerText)) continue;
                        if(dict.ContainsKey(el.Name)) continue;

                        dict.Add(el.Name, el.InnerText);
                    }
                }
                catch { }
            }
            return ret;
        }
    }
}
