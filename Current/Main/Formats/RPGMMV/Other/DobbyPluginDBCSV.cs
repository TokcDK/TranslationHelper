using System;
using System.Web.UI.WebControls.WebParts;

namespace TranslationHelper.Formats.RPGMMV.Other
{
    internal class DobbyPluginDBCSV : FormatStringBase
    {
        public override string Extension => ".csv";

        protected override void ParseFileContent()
        {
            var lines = ParseData.Reader.ReadToEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var linesCount = lines.Length;
            bool readHeader = true;
            string[] header = null;
            int headerMaxIndex = -1;
            for (int i = 0; i < linesCount; i++)
            {
                if (readHeader)
                {
                    // first line is info about line parts
                    readHeader = false;
                    header = lines[i].Split(',');
                    headerMaxIndex = header.Length - 1;

                    continue;
                }

                var line = lines[i];

                var parts = line.Split(',');

                bool changed = false;
                for (int i1 = 0; i1 < parts.Length; i1++)
                {
                    string part = parts[i1];

                    if (AddRowData(ref part, $"Index:{i1}{(i1 <= headerMaxIndex ? ">" + header[i1] : "")}") 
                        && SaveFileMode)
                    {
                        changed = true;
                        parts[i1] = part.Replace(",", string.Empty);
                    }
                }

                if (SaveFileMode && changed) lines[i] = string.Join(",", parts);
            }

            if (SaveFileMode) ParseData.ResultForWrite.Append(string.Join("\r\n", lines));
        }

        internal override bool IsValidString(string inputString)
        {
            if (!base.IsValidString(inputString)) return false;
            if (string.IsNullOrEmpty(inputString)) return false;
            if (inputString.Equals("-")) return false;
            if (inputString.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return false;
            if (inputString.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return false;
            if (inputString.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return false;
            if (inputString.StartsWith("icon_", StringComparison.OrdinalIgnoreCase)) return false;
            if (int.TryParse(inputString, out _)) return false;

            return true;
        }
    }
}
