﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    internal class Params_json : FormatStringBase
    {
        public override string Ext => ".json";
        protected override void ParseFileContent()
        {
            var infoContent = JsonConvert.DeserializeObject<Params_json_c>(ParseData.Reader.ReadToEnd(), new JsonSerializerSettings()
            { 
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            bool ret = false;

            string str = null;

            str = infoContent.FirstName;
            if (IsParsed(ref str, nameof(infoContent.FirstName))) { ret = true; if (SaveFileMode) infoContent.FirstName = str; }
            str = infoContent.LastName;
            if (IsParsed(ref str, nameof(infoContent.LastName))) { ret = true; if (SaveFileMode) infoContent.LastName = str; }
            str = infoContent.IntroMessage;
            if (IsParsed(ref str, nameof(infoContent.IntroMessage))) { ret = true; if (SaveFileMode) infoContent.IntroMessage = str; }

            if (SaveFileMode && ret && ParseData.Ret)
                ParseData.ResultForWrite.Append(JsonConvert.SerializeObject(infoContent, Formatting.Indented));
        }
        private bool IsParsed(ref string stringToTranslate, string info = "")
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
