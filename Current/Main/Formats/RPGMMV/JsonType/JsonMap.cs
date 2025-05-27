using RPGMVJsonParser;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonMap : EventCommandParseBase
    {
        public JsonMap(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            if (!Regex.IsMatch(Path.GetFileName(path).ToLowerInvariant(),@"map[0-9]+\.json")) return null;

            var data = Helper.LoadMap(path);
            if (data == null) return null;

            var s = data.DisplayName;
            if (AddRowData(ref s, SaveFileMode ? "" : $"Map DisplayName \r\nMap Note: \"{data.Note}\"") && SaveFileMode) data.DisplayName = s;

            s = data.Note;
            if (AddRowData(ref s, SaveFileMode ? "" : $"Map Note") && SaveFileMode) data.Note = s;

            int eventsCount = data.Events.Length;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = data.Events[e];
                if (@event == null) continue;

                s = @event.Note;
                if (AddRowData(ref s, SaveFileMode ? "" : $"Event Note") && SaveFileMode) @event.Note = s;

                int pagesCount = @event.Pages.Length;
                for (int p = 0; p < pagesCount; p++)
                {
                    var page = @event.Pages[p];
                    ParseCommandStrings(page.Commands, SaveFileMode ? "" : $"Map DisplayName: \"{data.DisplayName}\"\r\nMap Note: \"{data.Note}\"\r\nEvent ID: {@event.Id}\r\nEvent name: \"{@event.Name}\"\r\nPage number: {p}");
                }
            }

            return data;
        }
    }
}
