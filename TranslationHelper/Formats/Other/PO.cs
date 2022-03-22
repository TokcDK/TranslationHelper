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
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (FindOriginal)
            {
                if (ParseData.Line.StartsWith("msgid"))
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
                    AddRowData(ref original, Regex.Match(ParseData.Line, @"^msgstr \""(.*)\""$").Groups[1].Value, false);
                    ParseData.Ret = true;
                }
            }

            return KeywordActionAfter.Continue;
        }
    }
}
