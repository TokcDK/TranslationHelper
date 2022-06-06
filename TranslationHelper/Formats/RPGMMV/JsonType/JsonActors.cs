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
                var actor = data[e];
                if (actor == null) continue;

                var s = actor.Name;
                if (AddRowData(ref s, $"\r\nID: {actor.Id}\r\nbattlerName: {actor.BattlerName}") && ProjectData.SaveFileMode) actor.Name = s;
                s = actor.Profile;
                if (AddRowData(ref s, $"\r\nID: {actor.Id}\r\nName: {actor.Name}\r\nbattlerName: {actor.BattlerName}") && ProjectData.SaveFileMode) actor.Profile = s;
            }

            return data;
        }
    }
}
