using RPGMVJsonParser;
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
    internal class JsonEventCommandsList : EventCommandParseBase
    {
        public JsonEventCommandsList(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override object ParseJson(string path)
        {
            var data = Helper.LoadCommandsList(path);
            if (data == null) return null;

            ParseCommandStrings(data, "");

            return data;
        }
    }
}
