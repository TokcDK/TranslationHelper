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
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.Glitch_Pitch.IdolManager.Mod
{
    internal class Trivia_json : IdolManagerModBase
    {
        public Trivia_json(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".json";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            var match = Regex.Match(ParseData.Line, @"^\s*ID:\s*([0-9]+),$");
            if (!match.Success) return KeywordActionAfter.Continue;

            var id = match.Groups[1].Value;

            match = Regex.Match(ReadLine(), @"^\s*Type:\s*\""([^\""]+)\"",$");
            if (!match.Success) return KeywordActionAfter.Continue;

            var type = match.Groups[1].Value;

            match = Regex.Match(ReadLine(), @"^\s*Text:\s*\""([^\""]+)\""$");
            if (!match.Success) return KeywordActionAfter.Continue;

            var text = match.Groups[1].Value;

            if (AddRowData(ref text, $"ID:{id}\r\nType:\"{type}\"") && SaveFileMode) { };

            return base.ParseStringFileLine();
        }

        internal override bool IsValidString(string inputString) { return true; }
    }
}
