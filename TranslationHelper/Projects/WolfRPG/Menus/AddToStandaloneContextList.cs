using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Menus.ProjectMenus;

namespace TranslationHelper.Projects.WolfRPG.Menus
{
    class AddToStandaloneContextList : IGridItemMenu
    {
        public string Text => "Add standalone context";

        public string Description => "Adds entered context as standalone. Each string using the context path will be moved in separated text block in patch in time of open/save";

        public string Category => "";

        public void OnClick(object sender, EventArgs e)
        {
            var standaloneContextFilePath = Path.Combine(ProjectData.SelectedDir, "StandaloneContextList.thdata");
            var standaloneContextList = new List<string>();
            if (File.Exists(standaloneContextFilePath))
            {
                using(StreamReader reader = new StreamReader(standaloneContextFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        standaloneContextList.Add(line);
                    }
                }
            }

            var form = new AddToStandaloneContextListForm();

            var result = form.ShowDialog();
            if(result != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var addedContextLine = form.ContextLine;

            standaloneContextList.Add(addedContextLine);

            File.WriteAllLines(standaloneContextFilePath, standaloneContextList);
        }
    }
}
