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
    internal class JsonSkills :JsonTypeBase
    {
        public JsonSkills(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadSkills(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nNote: \"{item.Note}\"") && SaveFileMode) item.Name = s;
                s = item.Description;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: {item.Name}\r\nNote: \"{item.Note}\"") && SaveFileMode) item.Description = s;
                s = item.Message1;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: {item.Name}\r\nNote: \"{item.Note}\"") && SaveFileMode) item.Message1 = s;
                s = item.Message2;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: {item.Name}\r\nNote: \"{item.Note}\"") && SaveFileMode) item.Message2 = s;
                s = item.Note;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: {item.Name}\r\nNote") && SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
