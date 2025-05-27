using RPGMVJsonParser;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonClasses : JsonTypeBase
    {
        public JsonClasses(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadClasses(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (!string.IsNullOrWhiteSpace(s) && AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}", isCheckInput: false ) && SaveFileMode) item.Name = s;

                s = item.Note;
                if (!string.IsNullOrWhiteSpace(s) && AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}", isCheckInput: false ) && SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
