using System.Text;
using System.Text.RegularExpressions;

namespace TranslationHelper.Formats.Other
{
    internal class PO : FormatStringBase
    {
        internal override string Ext()
        {
            return ".po";
        }

        bool FindOriginal = true;
        string original;
        StringBuilder Info = new StringBuilder();
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (FindOriginal)
            {
                if (ParseData.Line.StartsWith("#"))
                {
                    Info.AppendLine(ParseData.Line);
                }
                else if (ParseData.Line.StartsWith("msgid"))
                {
                    original = Regex.Match(ParseData.Line, @"^msgid \""(.*)\""$").Groups[1].Value;
                    if (!string.IsNullOrEmpty(original))
                    {
                        FindOriginal = false;
                    }
                }
            }
            else
            {
                if (ParseData.Line.StartsWith("msgstr"))
                {
                    FindOriginal = true;
                    var rowData = new[] { original, Regex.Match(ParseData.Line, @"^msgstr \""(.*)\""$").Groups[1].Value };
                    if (AddRowData(ref rowData, Info.ToString(), false))
                    {
                        ParseData.Ret = true;
                    }
                }
            }

            return KeywordActionAfter.Continue;
        }
    }
}
