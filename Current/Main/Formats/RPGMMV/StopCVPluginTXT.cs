using System;
using System.Collections.Generic;
using System.Linq;

namespace TranslationHelper.Formats.RPGMMV
{
    internal class StopCVPluginTXT : FormatStringBase
    {
        public override string Extension => ".txt";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!IsValid(ParseData.Line))
            {
                SaveModeAddLine();
                return KeywordActionAfter.Continue; // comment
            }

            var t = $"{ParseData.Line}\r\n{string.Join("\r\n", GetLines())}";
            AddRowData(ref t);

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }

        private IEnumerable<string> GetLines()
        {
            while (IsValid(ReadLine()))
                yield return ParseData.Line;
        }

        bool IsValid(string s)
        {
            if (ParseData.Line == null) return false; // comment
            if (ParseData.Line.StartsWith("#")) return false; // comment
            if (ParseData.Line.StartsWith("@")) return false; // command
            if (string.IsNullOrEmpty(ParseData.Line)) return false; // empty

            return true;
        }
    }
}
