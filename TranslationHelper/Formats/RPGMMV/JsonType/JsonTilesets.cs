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
    internal class JsonTilesets:JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadTilesets(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"\r\nID: {item.Id}") && AppData.SaveFileMode) item.Name = s;
            }

            return data;
        }
    }
}
