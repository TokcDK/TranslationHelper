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
    internal class JsonTroops:JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadTroops(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var troop = data[e];
                if (troop == null) continue;

                var s = troop.Name;
                if (AddRowData(ref s, $"\r\nID: {troop.Id}") && ProjectData.SaveFileMode) troop.Name = s;
            }

            return data;
        }
    }
}
