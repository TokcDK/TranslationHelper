using System;
using System.Linq;
using TranslationHelper.Data;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class MPS : CommandUserBase
    {
        protected override void FileOpen()
        {
            Data = new MapParser();
            Data.Read(FilePath);

            var events = ((MapParser)Data).Events;
            var eventsCount = events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = events[e];
                var pagesCount = @event.Pages.Count;
                for (int p = 0; p < pagesCount; p++)
                {
                    var page = @event.Pages[p];
                    ParseCommandStrings(page.Commands, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}\r\nPage index: {p}");
                }
            }
        }
    }
}
