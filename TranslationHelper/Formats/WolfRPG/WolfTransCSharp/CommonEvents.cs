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
        CommonEventsParser Data = null;

        protected override void FileOpen()
        {
            Data = new CommonEventsParser();
            Data.Read(FilePath);

            var eventsCount = Data.Events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = Data.Events[e];

                ParseCommandStrings(@event.Commands, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}");
            }
        }

        protected override bool WriteFileData(string filePath = "")
        {
            if (!ParseData.Ret) return false;

            try { Data.Write(); } catch { return false; }
            return true;
        }
    }
}
