using System.Collections.Generic;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.LOFI.Kenshi
{
    internal class KenshoModStrings : FormatStringBase
    {
        public KenshoModStrings(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".kenshi-mod-strings";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!OpenFileMode && ParseData.Line.StartsWith("> BEGIN STRING"))
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
