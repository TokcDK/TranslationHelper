using RPGMVJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonCommonEvents : EventCommandParseBase
    {
        //internal override int ExtIdentifier()
        //{
        //    return Path.GetFileName(FilePath).ToLowerInvariant()=="commonevents.json";
        //}

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadCommonEvents(path);

            int eventsCount = data.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = data[e];
                if (@event == null) continue;

                int commandsCount = @event.Commands.Length;
                for (int c = 0; c < commandsCount; c++)
                {
                    ParseCommandStrings(@event.Commands, $"Event ID: {@event.Id}\r\nEvent name: {@event.Name}");
                }
            }

            return data;
        }
    }
}
