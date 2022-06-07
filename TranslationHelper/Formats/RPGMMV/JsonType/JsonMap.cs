using RPGMVJsonParser;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonMap : EventCommandParseBase
    {
        protected override object ParseJson(string path)
        {
            if (string.Equals(Path.GetFileNameWithoutExtension(path), "MapInfos", System.StringComparison.InvariantCultureIgnoreCase)) return null;
            
            var data = Helper.LoadMap(path);
            if (data == null) return null;

            int eventsCount = data.Events.Length;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = data.Events[e];
                if (@event == null) continue;

                int pagesCount = @event.Pages.Length;
                for (int p = 0; p < pagesCount; p++)
                {
                    var page = @event.Pages[p];
                    ParseCommandStrings(page.Commands, $"Event ID: {@event.Id}\r\nEvent name: {@event.Name}\r\nPage number: {p}");
                }
            }

            return data;
        }
    }
}
