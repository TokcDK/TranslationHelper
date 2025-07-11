﻿using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.Other
{
    internal class PO : FormatStringBase
    {
        public override string Extension => ".po";

        readonly StringBuilder Info = new StringBuilder();

        public PO(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (OpenFileMode && ParseData.Line.StartsWith("#"))
            {
                Info.AppendLine(ParseData.Line); // save string info
                return KeywordActionAfter.Continue;
            }
            else if (!ParseData.Line.StartsWith("msgid"))
            {
                if (OpenFileMode) Info.Clear();
                return Continue();
            }

            // get original value
            var o = Regex.Match(ParseData.Line, @"^msgid \""(.*)\""$").Groups[1].Value;
            while ((ParseData.Line = ReadLine()) != null && ParseData.Line.StartsWith("\""))
            {
                o += Regex.Match(ParseData.Line, @"^\""(.*)\""$").Groups[1].Value;
            }

            // skip when empty original
            if (string.IsNullOrWhiteSpace(o)) return Continue();

            // skip if logically translation value is empty or not starts with 'msgstr'
            if (string.IsNullOrEmpty(ParseData.Line)) return Continue();
            if (!ParseData.Line.StartsWith("msgstr")) return Continue();

            // get translation value
            var t = Regex.Match(ParseData.Line, @"^msgstr \""(.*)\""$").Groups[1].Value;
            while ((ParseData.Line = ReadLine()) != null && ParseData.Line.StartsWith("\""))
            {
                t += Regex.Match(ParseData.Line, @"^\""(.*)\""$").Groups[1].Value;
            }
            //if (string.IsNullOrWhiteSpace(t)) return KeywordActionAfter.Continue;
            //if (t == o) return KeywordActionAfter.Continue;

            // add row data or set translation
            var rowData = new[] { o, t };
            if (!AddRowData(ref rowData, OpenFileMode ? Info.ToString() : "", false)) return Continue();

            // set new line value istead of old
            if (SaveFileMode) ParseData.Line = "msgid \"" + o + "\"\n" + "msgstr \"" + rowData[0] + "\"\n" + ParseData.Line;
            if (OpenFileMode) Info.Clear();
            ParseData.Ret = true; // must autoset to true when any translation set of line added but in any case

            return Continue();
        }

        KeywordActionAfter Continue()
        {
            SaveModeAddLine(newline: "\n");
            return KeywordActionAfter.Continue;
        }
    }
}
