using RPGMVJsonParser;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonActors : JsonTypeBase
    {
        public JsonActors(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadActors(path);

            int count = data.Count;
            for (int e = 0; e < count; e++)
            {
                var item = data[e];
                if (item == null) continue;

                var s = item.Name;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nbattlerName: \"{item.BattlerName}\"\r\nCharacterName: \"{item.CharacterName}\"\r\nNote: \"{item.Note}\"") && SaveFileMode) item.Name = s;
                s = item.Nickname;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nbattlerName: \"{item.BattlerName}\"") && SaveFileMode) item.Nickname = s;
                s = item.CharacterName;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nbattlerName: \"{item.BattlerName}\"") && SaveFileMode) item.CharacterName = s;
                s = item.Profile;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nCharacterName: \"{item.CharacterName}\"\r\nbattlerName: \"{item.BattlerName}\"") && SaveFileMode) item.Profile = s;
                s = item.Note;
                if (AddRowData(ref s, SaveFileMode ? "" : $"\r\nID: {item.Id}\r\nName: \"{item.Name}\"\r\nCharacterName: \"{item.CharacterName}\"\r\nbattlerName: \"{item.BattlerName}") && SaveFileMode) item.Note = s;
            }

            return data;
        }
    }
}
