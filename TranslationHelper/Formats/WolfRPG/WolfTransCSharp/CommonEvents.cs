using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WT = WolfTrans.Net;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.CommonEvents;
using WolfTrans.Net.Parsers.Events;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class CommonEvents: CommandUserBase
    {
        protected override void FileOpen()
        {
            Data = new WT.Parsers.CommonEvents.ParserCommonEvents();
            Data.Read(FilePath);

            var events = ((WT.Parsers.CommonEvents.ParserCommonEvents)Data).Events;
            var eventsCount = events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = events[e];

                ParseCommandStrings(@event.Commands, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}");
            }
        }
    }
}
