using RPGMVJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonTroops : EventCommandParseBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadTroops(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, AppData.CurrentProject.SaveFileMode ? "" : $"\r\nID: {item.Id}") && AppData.CurrentProject.SaveFileMode) item.Name = s;

                int pagesCount = item.Pages.Length;
                for (int p = 0;p< pagesCount; p++)
                {
                    var page = item.Pages[p];
                    ParseCommandStrings(page.Commands, AppData.CurrentProject.SaveFileMode ? "" : $"Troop name: \"{item.Name}\"\r\nPage number: {p}");
                }
            }

            return data;
        }
    }
}
