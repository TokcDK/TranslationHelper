using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT = WolfTrans.Net;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.CommonEvents;
using WolfTrans.Net.Parsers.Shared;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class CommonEvents: CommandUserBase
    {
        protected override void FileOpen()
        {
            Data = new CommonEventsParser();
            Data.Read(FilePath);

            var events = ((CommonEventsParser)Data).Events;
            var eventsCount = events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = events[e];

                ParseCommandStrings(@event.Commands, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}");
            }
        }
    }
}
