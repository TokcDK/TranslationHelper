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
    internal class JsonEnemies:JsonTypeBase
    {
        public JsonEnemies(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadEnemies(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nbattlerName: \"{item.BattlerName}\"\r\nNote: \"{item.Note}\"") && SaveFileMode) item.Name = s;
                s = item.Note;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nbattlerName: \"{item.BattlerName}\"") && SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
