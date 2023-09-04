using System;
using System.Text.RegularExpressions;

namespace TranslationHelper.Formats.RPGMMV
{
    internal class ValueLocationPresetTxt : FormatStringBase
    {
        public override string Description => "RPGMV www/valueLocationPreset.txt";
        public override string Extension => ".txt";

        public override bool Check()
        {
            return string.Equals(FileName, "valueLocationPreset.txt", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!ParseData.Line.StartsWith("\"") || !ParseData.Line.EndsWith("\"")) return KeywordActionAfter.Continue;

            var strings = ParseData.Line.Trim('"').Split(new[] { "\\n" }, StringSplitOptions.None);

            bool changed = false;
            int stringsCount = strings.Length;
            for (int i = 0; i < stringsCount; i++)
            {
                var match = Regex.Match(strings[i], @"^(\[([0-9]+)\])(.+)$");

                if (!match.Success) continue;

                var s = match.Groups[3].Value;

                if (AddRowData(ref s, "#" + match.Groups[2].Value) && SaveFileMode)
                {
                    changed = true;

                    strings[i] = match.Groups[1].Value + s;
                }
            }

            if (SaveFileMode && changed)
            {
                ParseData.Line = "\"" + string.Join("\\n", strings) + $"{(strings[stringsCount - 1].EndsWith("\\n") ? "" : "\\n")}\"";
            }

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }
    }
}
