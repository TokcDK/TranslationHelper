using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using TranslationHelper.Formats.Glitch_Pitch.Idol_Manager.Mod;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    internal class Dialogues_json : IdolManagerModBase
    {
        public Dialogues_json(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".json";
        protected override void ParseFileContent()
        {
            var infoContent = JsonConvert.DeserializeObject<List<Dialogues_json_c>>(ParseData.Reader.ReadToEnd());

            bool ret = false;
            foreach (var info in infoContent) 
            {
                if (info.Scripts == null) continue;

                foreach (var script in info.Scripts) if (ReadScriptStrings(script)) ret = true;
            }

            if (SaveFileMode && ret && ParseData.Ret) 
                ParseData.ResultForWrite.Append(JsonConvert.SerializeObject(infoContent, Formatting.Indented));
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
            if (AddRowData(ref ot, info: script.Type, isCheckInput: false)) 
            {
                ret = true;

                if (SaveFileMode) script.Val = ot[0]; // set to translated
            };

            return ret;
        }
    }
}
