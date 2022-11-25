using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;
using TranslationHelper.Functions;
using TranslationHelper.Functions.FileElementsFunctions.Row.ExportFormats;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects.RPGMTrans;

namespace TranslationHelper.Menus.MainMenus.File.Import
{
    internal class MenuItemImportFromPOfile : MainMenuFileSubItemBase, IProjectMenuItem
    {
        public override string Text => T._("Import translation from PO file");

        public override string CategoryName => "Import";

        public override void OnClick(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                if (d.ShowDialog() != DialogResult.OK) return;

                if (string.IsNullOrWhiteSpace(d.FileName) || !System.IO.File.Exists(d.FileName)) return;

                Dictionary<string, string> pairs = new Dictionary<string, string>();

                using (StreamReader sr = new StreamReader(d.FileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.StartsWith("msgid")) continue;

                        var o = Regex.Match(line, @"^msgid \""(.*)\""$").Groups[1].Value;
                        while ((line = sr.ReadLine()) != null && line.StartsWith("\""))
                        {
                            o += Regex.Match(line, @"^\""(.*)\""$").Groups[1].Value;
                        }
                        if (string.IsNullOrWhiteSpace(o)) continue;

                        //if ((line = sr.ReadLine()) == null) continue;
                        if (string.IsNullOrEmpty(line)) continue;
                        if (!line.StartsWith("msgstr")) continue;

                        var t = Regex.Match(line, @"^msgstr \""(.*)\""$").Groups[1].Value;
                        while ((line = sr.ReadLine()) != null && line.StartsWith("\""))
                        {
                            t += Regex.Match(line, @"^\""(.*)\""$").Groups[1].Value;
                        }
                        if (string.IsNullOrWhiteSpace(t)) continue;
                        //if (t == o) continue;

                        pairs.Add(o, t);
                    }
                }

                foreach (DataTable table in AppData.CurrentProject.FilesContent.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var cellValue = row[THSettings.TranslationColumnName] + "";
                        if (cellValue.Length > 0) continue;

                        var cellOriginalValue = row[THSettings.OriginalColumnName] + "";
                        if (!pairs.ContainsKey(cellOriginalValue)) continue;

                        row[THSettings.TranslationColumnName] = pairs[cellOriginalValue];
                    }
                }
            }
        }
    }
}
