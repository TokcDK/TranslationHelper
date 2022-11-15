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

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    internal class Marketing_json : IdolManagerModBase
    {
        public override string Ext => ".json";
        protected override void ParseFileContent()
        {
            var infoContent = JsonConvert.DeserializeObject<List<Marketing_json_c>>(ParseData.Reader.ReadToEnd());

            bool ret = false;
            foreach (var info in infoContent)
            {
                string str = info.Title;
                if (IsParsed(ref str, nameof(info.Title))) { ret = true; if (SaveFileMode) info.Title = str; }
                str = info.Description;
                if (IsParsed(ref str, nameof(info.Description))) { ret = true; if (SaveFileMode) info.Description = str; }
            }

            if (SaveFileMode && ret && ParseData.Ret) 
                ParseData.ResultForWrite.Append(JsonConvert.SerializeObject(infoContent, Formatting.Indented));
        }

        private bool IsParsed(ref string stringToTranslate, string info="")
        {
            if (string.IsNullOrEmpty(stringToTranslate)) return false;

            var ot = new[] { stringToTranslate, "" };
            if (AddRowData(ref ot, rowInfo: info, isCheckInput: false))
            {
                if (SaveFileMode) stringToTranslate = ot[0]; // set to translated

                return true;
            };

            return false;
        }
    }
}
