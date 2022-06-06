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
    internal class JsonEnemies:JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadEnemies(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var enemy = data[e];
                if (enemy == null) continue;

                var s = enemy.Name;
                if (AddRowData(ref s, $"\r\nID: {enemy.Id}\r\nbattlerName: {enemy.BattlerName}") && ProjectData.SaveFileMode) enemy.Name = s;
            }

            return data;
        }
    }
}
