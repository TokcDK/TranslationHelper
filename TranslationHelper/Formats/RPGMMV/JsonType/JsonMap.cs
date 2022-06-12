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

            var s = data.DisplayName;
            if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"Map DisplayName \r\nMap Note: \"{data.Note}\"") && AppData.SaveFileMode) data.DisplayName = s;

            s = data.Note;
            if (AddRowData(ref s, AppData.SaveFileMode ? "" : $"Map Note") && AppData.SaveFileMode) data.Note = s;

            int eventsCount = data.Events.Length;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = data.Events[e];
                if (@event == null) continue;

                int pagesCount = @event.Pages.Length;
                for (int p = 0; p < pagesCount; p++)
                {
                    var page = @event.Pages[p];
                    ParseCommandStrings(page.Commands, AppData.SaveFileMode ? "" : $"Map DisplayName: \"{data.DisplayName}\"\r\nMap Note: \"{data.Note}\"\r\nEvent ID: {@event.Id}\r\nEvent name: \"{@event.Name}\"\r\nPage number: {p}");
                }
            }

            return data;
        }
    }
}
