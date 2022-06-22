using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.TyranoBuilder.Extracted;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    abstract class KSOther : KSParserBase
    {
        public class LinePart
        {
            public string Value { get; }

            public bool IsString { get; }
            public LinePart(string value, bool isString)
            {
                Value = value;
                IsString = isString;
            }
        }

        readonly KSSyntax.Attribute Attr = new KSSyntax.Attribute();

        internal override string Ext => ".ks";

        bool IsScript = false;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (AppData.SaveFileMode) return base.ParseStringFileLine(); // temp

            if (IsScript)
            {
                // wait end of script
                if (ParseData.Line.Contains("[endscript]")) IsScript = false;
                Saveline();
                return KeywordActionAfter.Continue;
            }

            // skip comments and labels
            if (ParseData.Line.TrimStart().StartsWith(";")) { Saveline(); return KeywordActionAfter.Continue; }
            if (ParseData.Line.TrimStart().StartsWith("*")) { Saveline(); return KeywordActionAfter.Continue; }

            // skip script block
            IsScript = ParseData.Line.Contains("[iscript]");
            if (IsScript) return KeywordActionAfter.Continue;

            // get emb attribute value
            var matches = new Regex(Attr.StartsWith).Matches(ParseData.Line);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (!match.Value.StartsWith("emb=")) continue;

                    var s = match.Groups[1].Value;
                    AddRowData(RowData: s, RowInfo: ParseData.Line);

                    break;
                }

                return KeywordActionAfter.Continue; // skip if found attributes
            }

            // add full line when no tag bracket found
            if (ParseData.Line.IndexOf('[') == -1)
            {
                AddRowData(ParseData.Line);                
                Saveline();
                return KeywordActionAfter.Continue;
            }

            // get line parts and strings from it
            var morphedLine = new Regex(@"\[[phrl]+\]").Replace(ParseData.Line, string.Empty);
            morphedLine = new Regex(@"\[.\]").Replace(morphedLine, string.Empty);
            if (string.IsNullOrWhiteSpace(morphedLine)) { Saveline(); return KeywordActionAfter.Continue; }
            if (morphedLine.Contains("[")) { Saveline(); return KeywordActionAfter.Continue; }

            var regex = new Regex(@"\[[^\]]+\]");
            List<LinePart> lineParts = new List<LinePart>();
            int stringIndex = 0;
            matches = regex.Matches(ParseData.Line);
            foreach (Match m in matches)
            {
                if (m.Index > stringIndex)
                {
                    string s = ParseData.Line.Substring(stringIndex, m.Index - stringIndex);
                    lineParts.Add(new LinePart(s, true));
                }

                lineParts.Add(new LinePart(m.Value, false));
                stringIndex = (m.Index + m.Length); // move position right after match
            }
            if(stringIndex!= ParseData.Line.Length)
            {
                // add string in last part of line
                string s = ParseData.Line.Substring(stringIndex);
                lineParts.Add(new LinePart(s, true));
            }
            int index = lineParts.Count;
            for (int i = index - 1; i >= 0; i--)
            {
                if (!lineParts[i].IsString) continue;

                AddRowData(lineParts[i].Value, ParseData.Line, CheckInput: false);
            }

            Saveline();
            return KeywordActionAfter.Continue;
        }
        void Saveline()
        {
            SaveModeAddLine(newline: Environment.NewLine);//using \n as new line
        }
    }
}
