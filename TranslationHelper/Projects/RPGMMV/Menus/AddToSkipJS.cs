using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Menus.ProjectMenus;

namespace TranslationHelper.Projects.RPGMMV.Menus
{
    class AddToSkipJS : IFileListItemMenu
    {
        public string Text => "[" + AppData.CurrentProject.Name+ "]" + T._("Skip JS");

        public string Description => T._("Add selected item to Skip.js file");

        public string CategoryName => "";

        public void OnClick(object sender, EventArgs e)
        {
            //read and check the name
            var names = AppData.THFilesList.CopySelectedNames();
            if (string.IsNullOrWhiteSpace(names) || names.ToUpperInvariant().IndexOf(".JS") == -1)
            {
                return;
            }

            //read only js names
            var SkipJSList = new HashSet<string>();
            SetSkipJSList(SkipJSList, THSettings.RPGMakerMVSkipjsRulesFilePath);

            //read all list
            List<string> SkipJSOveralList;
            if (SkipJSList.Count == 0 || !File.Exists(THSettings.RPGMakerMVSkipjsRulesFilePath))
            {
                SkipJSOveralList = new List<string>();
            }
            else
            {
                SkipJSOveralList = File.ReadAllLines(THSettings.RPGMakerMVSkipjsRulesFilePath).ToList();
            }

            bool changed = false;
            foreach (var jsname in names.SplitToLines())
            {
                if (string.IsNullOrWhiteSpace(jsname) || !string.Equals(Path.GetExtension(jsname), ".js", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                //check if name already exists in list
                if (!SkipJSList.Contains(names) && !SkipJSOveralList.Contains(names))
                {
                    changed = true;
                    SkipJSOveralList.Add(names);
                }
            }

            if (!changed)
            {
                return;
            }

            //write list
            File.WriteAllLines(THSettings.RPGMakerMVSkipjsRulesFilePath, SkipJSOveralList);
        }

        /// <summary>
        /// read js file names from <paramref name="skipjsfilePath"/> in <paramref name="SkipJSList"/>
        /// </summary>
        /// <param name="SkipJSList"></param>
        /// <param name="skipjsfilePath"></param>
        private static void SetSkipJSList(HashSet<string> SkipJSList, string skipjsfilePath)
        {
            if (!File.Exists(skipjsfilePath))
            {
                return;
            }

            var skipjs = File.ReadAllLines(skipjsfilePath);
            foreach (var line in skipjs)
            {
                var jsfile = line.Trim();
                if (jsfile.Length == 0 || jsfile[0] == ';' || SkipJSList.Contains(jsfile))
                {
                    continue;
                }
                SkipJSList.Add(jsfile);
            }
        }
    }
}
