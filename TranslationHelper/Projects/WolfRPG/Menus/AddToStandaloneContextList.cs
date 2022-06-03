using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Menus.ProjectMenus;

namespace TranslationHelper.Projects.WolfRPG.Menus
{
    class AddToStandaloneContextList : IGridItemMenu
    {
        public string Text => "Add standalone context";

        public string Description => "Adds entered context as standalone. Each string using the context path will be moved in separated text block in patch in time of open/save";

        public string Category => "";

        public static string BeginStringMarker = "> BEGIN STRING\r\n";
        public static string EndStringMarker = "\r\n> END STRING\r\n";
        public static string StandaloneContextFilePath = Path.Combine(ProjectData.CurrentProject.SelectedGameDir, "StandaloneContextList.thdata");
        public void OnClick(object sender, EventArgs e)
        {
            var standaloneContextList = LoadList(StandaloneContextFilePath);

            using (var form = new AddToStandaloneContextListForm())
            {
                var result = form.ShowDialog();
                if (result != DialogResult.OK)
                {
                    return;
                }

                if (ProjectData.THFilesList.SelectedIndex == -1)
                {
                    return;
                }

                var addedContextLine = form.ContextLine;

                var selectedCells = ProjectData.Main.THFileElementsDataGridView.SelectedCells;
                foreach (DataGridViewCell selectedCell in selectedCells)
                {
                    var cellValue = ProjectData.Main.THFileElementsDataGridView.Rows[selectedCell.RowIndex].Cells[0].Value + "";
                    if (string.IsNullOrWhiteSpace(cellValue))
                    {
                        continue;
                    }

                    CleanContext(ref addedContextLine);

                    AddPair(standaloneContextList, cellValue, addedContextLine);
                }

                if (standaloneContextList.Count > 0)
                {
                    var str = new StringBuilder();
                    foreach (var val in standaloneContextList)
                    {
                        foreach (var context in val.Value)
                        {
                            str.AppendLine(context);
                        }
                        str.Append(BeginStringMarker + val.Key + EndStringMarker);
                    }

                    File.WriteAllText(StandaloneContextFilePath, str.ToString());
                }
            }
        }

        public static void CleanContext(ref string addedContextLine)
        {
            foreach (var mark in new[]
            {
                        '<',
                        '#'
                    })
            {
                var tag = " " + mark + " UNTRANSLATED";
                if (addedContextLine.EndsWith(tag))
                {
                    addedContextLine = addedContextLine.Replace(tag, string.Empty);
                }
            }
        }

        public static Dictionary<string, HashSet<string>> LoadList(string standaloneContextFilePath)
        {
            var standaloneContextList = new Dictionary<string, HashSet<string>>();
            if (File.Exists(standaloneContextFilePath))
            {
                // load exist
                var blocks = File.ReadAllText(standaloneContextFilePath).Split(new[] { EndStringMarker }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var block in blocks)
                {
                    var keyvalue = block.Split(new[] { "\r\n" + BeginStringMarker }, StringSplitOptions.None);
                    if (keyvalue.Length != 2) // dont load invalid
                    {
                        continue;
                    }
                    var context = keyvalue[0];
                    var originalString = keyvalue[1];
                    AddPair(standaloneContextList, originalString, context);
                }
            }

            return standaloneContextList;
        }

        private static void AddPair(Dictionary<string, HashSet<string>> standaloneContextList, string stringValue, string contextLine)
        {
            if (!standaloneContextList.ContainsKey(stringValue))
            {
                standaloneContextList.Add(stringValue, new HashSet<string>());
                standaloneContextList[stringValue].Add(contextLine);
            }
            else
            {
                if (!standaloneContextList[stringValue].Contains(contextLine)) // add only if context not in list
                {
                    standaloneContextList[stringValue].Add(contextLine);
                }
            }
        }
    }
}
