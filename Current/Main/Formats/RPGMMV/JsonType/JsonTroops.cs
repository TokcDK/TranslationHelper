using RPGMVJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonTroops : EventCommandParseBase
    {
        public JsonTroops(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadTroops(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}") && SaveFileMode) item.Name = s;

                int pagesCount = item.Pages.Length;
                for (int p = 0;p< pagesCount; p++)
                {
                    var page = item.Pages[p];
                    ParseCommandStrings(page.Commands, SaveFileMode ? "" : $"Troop name: \"{item.Name}\"\r\nPage number: {p}");
                }
            }

            return data;
        }
    }
}
