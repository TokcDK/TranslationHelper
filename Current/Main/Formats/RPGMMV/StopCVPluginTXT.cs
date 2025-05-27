using System.Collections.Generic;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV
{
    internal class StopCVPluginTXT : FormatStringBase
    {
        public StopCVPluginTXT(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".txt";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!IsValid(ParseData.Line))
            {
                SaveModeAddLine();
                return KeywordActionAfter.Continue; // comment
            }

            var line = ParseData.Line;

            var extraLines = string.Join("\r\n", GetLines());
            if (!string.IsNullOrEmpty(extraLines)) extraLines = "\r\n" + extraLines;

            var t = $"{line}{extraLines}";
            if (AddRowData(ref t) && SaveFileMode)
            {
                ParseData.Line = t + "\r\n" + ParseData.Line;

                SaveModeAddLine();
            }


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
