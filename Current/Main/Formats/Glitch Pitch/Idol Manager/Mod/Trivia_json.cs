using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using TranslationHelper.Data;
using TranslationHelper.Formats.Glitch_Pitch.Idol_Manager.Mod;

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    internal class Trivia_json : IdolManagerModBase
    {
        public override string Ext => ".json";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            var match = Regex.Match(ParseData.Line, @"^\t*ID:\t*([0-9]+),$");
            if (!match.Success) return KeywordActionAfter.Continue;

            var id = match.Groups[1].Value;

            match = Regex.Match(ReadLine(), @"^\t*Type:\t*\""([^\""]+)\"",$");
            if (!match.Success) return KeywordActionAfter.Continue;

            var type = match.Groups[1].Value;

            match = Regex.Match(ReadLine(), @"^\t*Text:\t*\""([^\""]+)\"",$");
            if (!match.Success) return KeywordActionAfter.Continue;

            var text = match.Groups[1].Value;

            if (AddRowData(ref text, $"ID:{id}\r\nType:\"{type}\"") && SaveFileMode) { };

            return base.ParseStringFileLine();
        }
    }
}
