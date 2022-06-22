using System.Collections.Generic;

namespace TranslationHelper.Formats.LOFI.Kenshi
{
    internal class kensho_mod_strings : FormatStringBase
    {
        internal override string Ext => ".kenshi-mod-strings";

        bool reading = false;
        bool readingOriginal = true;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!reading && ParseData.Line.StartsWith("> BEGIN STRING"))
            {
                ReadLine();
                List<string> str = new List<string>();
                while (!ParseData.Line.StartsWith("> CONTEXT"))
                {
                    str.Add(ParseData.Line);
                    ReadLine();
                }
                var o = string.Join("\r\n", str);

                ReadLine();
                while (ParseData.Line.StartsWith("> CONTEXT"))
                {
                    ReadLine();
                }

                str = new List<string>();
                str.Add(ParseData.Line);
                while (!ParseData.Line.StartsWith("> END STRING"))
                {
                    str.Add(ParseData.Line);
                }
            }

            return base.ParseStringFileLine();
        }
    }
}
