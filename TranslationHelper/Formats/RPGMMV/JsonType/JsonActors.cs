using RPGMVJsonParser;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonActors : JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadActors(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nbattlerName: \"{item.BattlerName}\"\r\nNote: \"{item.Note}\"") && AppData.SaveFileMode) item.Name = s;
                s = item.Profile;
                if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nbattlerName: \"{item.BattlerName}\"") && AppData.SaveFileMode) item.Profile = s;
                s = item.Note;
                if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nbattlerName: \"{item.BattlerName}") && AppData.SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
