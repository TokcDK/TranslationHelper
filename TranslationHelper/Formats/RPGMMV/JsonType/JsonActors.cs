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
                if (AddRowData(ref s, AppData.CurrentProject.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nbattlerName: \"{item.BattlerName}\"\r\nCharacterName: \"{item.CharacterName}\"\r\nNote: \"{item.Note}\"") && AppData.CurrentProject.SaveFileMode) item.Name = s;
                s = item.CharacterName;
                if (AddRowData(ref s, AppData.CurrentProject.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nbattlerName: \"{item.BattlerName}\"") && AppData.CurrentProject.SaveFileMode) item.CharacterName = s;
                s = item.Profile;
                if (AddRowData(ref s, AppData.CurrentProject.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nCharacterName: \"{item.CharacterName}\"\r\nbattlerName: \"{item.BattlerName}\"") && AppData.CurrentProject.SaveFileMode) item.Profile = s;
                s = item.Note;
                if (AddRowData(ref s, AppData.CurrentProject.SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nCharacterName: \"{item.CharacterName}\"\r\nbattlerName: \"{item.BattlerName}") && AppData.CurrentProject.SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
