using RPGMVJsonParser;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonClasses : JsonTypeBase
    {
        protected override object ParseJson(string path)
        {
            var data = Helper.LoadClasses(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"\r\nID: {item.Id}") && AppData.SaveFileMode) item.Name = s;

                s = item.Note;
                if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"\r\nID: {item.Id}") && AppData.SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
