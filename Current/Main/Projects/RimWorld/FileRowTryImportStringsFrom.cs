using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using TranslationHelper.Data;
using TranslationHelper.Functions.FileElementsFunctions.Row;
using TranslationHelper.Menus;
using TranslationHelper.Menus.MainMenus.Edit;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class MainMenuTryImportStringsFrom : MainMenuEditSubItemBase, IProjectSpecifiedMenuItem
    {
        public override string Text => "[" + AppData.CurrentProject.Name + "] " + T._("Try import translations from..");

        public override string Description => T._("Try import translations from other language dir");

        public override void OnClick(object sender, EventArgs e)
        {
            new FileRowTryImportStringsFrom().All();
        }
    }

    class FileRowTryImportStringsFrom : RowBase
    {
        protected override bool IsOkAll()
        {
            var p = "";
            var languagesDirMatch = Regex.Match(AppData.CurrentProject.SelectedDir, @"^(.+\\Languages)\\([^\\]+)\\");
            if (languagesDirMatch.Success)
            {
                p = languagesDirMatch.Groups[1].Value;
            }

            using (OpenFileDialog openFIle = new OpenFileDialog())
            {
                if (p != "") openFIle.InitialDirectory = p;
                openFIle.Filter = T._("Xml files") + "|*.xml";
                openFIle.Title = T._("Select an xml file in targett language folder where from need to import");

                if (openFIle.ShowDialog() != DialogResult.OK) return false;

                languagesDirMatch = Regex.Match(openFIle.FileName, @"^(.+\\Languages\\([^\\]+))\\");
                if (!languagesDirMatch.Success) return false;

                _languagesDir = languagesDirMatch.Groups[1].Value;
            }

            FillDict();

            if (dict.Count == 0) return false;

            return true;
        }

        string _languagesDir = "";

        protected override bool Apply()
        {
            try
            {
                string oinfo = AppData.CurrentProject.FilesContentInfo.Tables[SelectedTable.TableName].Rows[SelectedRowIndex].Field<string>(0);
                var match = Regex.Match(oinfo, @"Element: \""([^\""]+)\""");
                if (!match.Success) return false;
                var k = match.Groups[1].Value;

                if (dict.TryGetValue(k, out string value)) Translation = value;
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
                        if (dict.ContainsKey(el.Name)) continue;

                        dict.Add(el.Name, el.InnerText);
                    }
                }
                catch { }
            }
            return ret;
        }
    }
}
