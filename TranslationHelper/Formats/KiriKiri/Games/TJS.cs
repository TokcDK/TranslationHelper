using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class TJS : KiriKiriFormatBase
    {
        public TJS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        bool IsComment = false;
        protected override int ParseStringFileLine()
        {
            if (IsComment)
            {
                if (ParseData.TrimmedLine.EndsWith("*/")) //comment section end
                {
                    IsComment = false;
                }
            }
            else
            {
                if (ParseData.TrimmedLine.StartsWith("/*")) //comment section start
                {
                    IsComment = true;
                }
                else if (ParseData.TrimmedLine.StartsWith("//")) //comment
                {
                }
                else
                {
                    var mc = Regex.Matches(ParseData.line,@"\""([^\""]+)\""");
                    if (mc.Count > 0)
                    {
                        for (int i = mc.Count - 1; i >= 0; i--)
                        {
                            var value = mc[i].Result("$1");
                            if (IsValidString(value))
                            {
                                if (thDataWork.OpenFileMode)
                                {
                                    AddRowData(value);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }
    }
}
