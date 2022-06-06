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
    internal class JsonSkills :JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadSkills(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var skill = data[e];
                if (skill == null) continue;

                var s = skill.Name;
                if (AddRowData(ref s, $"\r\nID: {skill.Id}\r\nNote: \"{skill.Note}\"") && AppData.SaveFileMode) skill.Name = s;
                s = skill.Message1;
                if (AddRowData(ref s, $"\r\nID: {skill.Id}\r\nName: {skill.Name}\r\nNote: \"{skill.Note}\"") && AppData.SaveFileMode) skill.Message1 = s;
                s = skill.Message2;
                if (AddRowData(ref s, $"\r\nID: {skill.Id}\r\nName: {skill.Name}\r\nNote: \"{skill.Note}\"") && AppData.SaveFileMode) skill.Message2 = s;
            }

            return data;
        }
    }
}
