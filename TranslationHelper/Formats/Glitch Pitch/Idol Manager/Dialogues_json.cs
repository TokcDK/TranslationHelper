using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using TranslationHelper.Formats.RPGMMV;

namespace TranslationHelper.Formats.Glitch_Pitch.Idol_Manager
{
    internal class Dialogues_json :FormatStringBase
    {
        public override string Ext => ".json";
        protected override void ParseFileContent()
        {
            var infoContent = JsonConvert.DeserializeObject<List<Dialogues_json_s>>(ParseData.Reader.ReadToEnd());

            bool ret = false;
            foreach (var info in infoContent) 
            {
                if (info.Scripts == null) continue;

                foreach (var script in info.Scripts) if (ReadScriptStrings(script)) ret = true; 
            }
        }

        private bool ReadScriptStrings(Script script)
        {
            if (script == null) return false;

            bool ret = false;
            foreach (var s in script.Scripts) if (ReadScriptStrings(s)) ret = true;

            if (!string.Equals(script.Type, "message", StringComparison.InvariantCultureIgnoreCase) 
                && !string.Equals(script.Type, "choice", StringComparison.InvariantCultureIgnoreCase)
                ) return ret;

            if (string.IsNullOrWhiteSpace(script.Val)) return ret;

            var ot = new[] { script.Val, "" };
            if (AddRowData(ref ot, rowInfo: script.Type, isCheckInput: false)) ret = true;

            return ret;
        }
    }
}
