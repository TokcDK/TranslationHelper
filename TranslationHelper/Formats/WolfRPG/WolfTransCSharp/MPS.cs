using System;
using System.Linq;
using TranslationHelper.Data;
using WolfTrans.Net.Parsers;
using WolfTrans.Net.Parsers.Shared;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal class MPS : CommandUserBase
    {
        MapParser Data = null;
        protected override void FileOpen()
        {
            Data = new MapParser();
            Data.Read(FilePath);

            var eventsCount = Data.Events.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = Data.Events[e];
                var pagesCount = @event.Pages.Count;
                for (int p = 0; p < pagesCount; p++)
                {
                    var page = @event.Pages[p];
                    ParseCommandStrings(page.Commands, $"Event ID: {@event.ID}\r\nEvent name: {@event.Name}\r\nPage index: {p}");
                }
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
