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
    internal class JsonStates:JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadStates(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var state = data[e];
                if (state == null) continue;

                var s = state.Name;
                if (AddRowData(ref s, $"\r\nID: {state.Id}\r\nNote: \"{state.Note}\"") && ProjectData.SaveFileMode) state.Name = s;
                s = state.Message1;
                if (AddRowData(ref s, $"\r\nID: {state.Id}\r\nName: {state.Name}\r\nNote: \"{state.Note}\"") && ProjectData.SaveFileMode) state.Message1 = s;
                s = state.Message2;
                if (AddRowData(ref s, $"\r\nID: {state.Id}\r\nName: {state.Name}\r\nNote: \"{state.Note}\"") && ProjectData.SaveFileMode) state.Message2 = s;
                s = state.Message3;
                if (AddRowData(ref s, $"\r\nID: {state.Id}\r\nName: {state.Name}\r\nNote: \"{state.Note}\"") && ProjectData.SaveFileMode) state.Message3 = s;
                s = state.Message4;
                if (AddRowData(ref s, $"\r\nID: {state.Id}\r\nName: {state.Name}\r\nNote: \"{state.Note}\"") && ProjectData.SaveFileMode) state.Message4 = s;
            }

            return data;
        }
    }
}
