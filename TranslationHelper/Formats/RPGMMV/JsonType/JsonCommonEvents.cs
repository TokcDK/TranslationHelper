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
            if (data == null) return null;

            int eventsCount = data.Count;
            for (int e = 0; e < eventsCount; e++)
            {
                var @event = data[e];
                if (@event == null) continue;

                ParseCommandStrings(@event.Commands, $"Event ID: {@event.Id}\r\nEvent name: {@event.Name}");
            }

            return data;
        }
    }
}
