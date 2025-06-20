﻿using RPGMVJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JsonType
{
    internal class JsonCommonEvents : EventCommandParseBase
    {
        public JsonCommonEvents(ProjectBase parentProject) : base(parentProject)
        {
        }

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
                var item = data[e];
                if (item == null) continue;

                ParseCommandStrings(item.Commands, SaveFileMode ? "" : $"Event ID: {item.Id}\r\nEvent name: \"{item.Name}\"");
            }

            return data;
        }
    }
}
