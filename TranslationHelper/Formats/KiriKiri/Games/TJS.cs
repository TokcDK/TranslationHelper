using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class TJS : KiriKiriBase
    {
        public TJS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override int ParseStringFileLine()
        {
            if (ParseData.IsComment)
            {
                if (ParseData.TrimmedLine.EndsWith("*/")) //comment section end
                {
                    ParseData.IsComment = false;
                }
            }
            else
            {
                if (ParseData.TrimmedLine.StartsWith("/*")) //comment section start
                {
                    ParseData.IsComment = true;
                }
                else if (ParseData.TrimmedLine.StartsWith("//")) //comment
                {
                }
                else
                {
                    var mc = Regex.Matches(ParseData.line, @"\""([^\""\\]+(?:\\.[^\""\\]*)*)\""");//get all between '"' include '\"' link: https://stackoverflow.com/questions/2148587/finding-quoted-strings-with-escaped-quotes-in-c-sharp-using-a-regular-expression
                    if (mc.Count > 0)
                    {
                        for (int i = mc.Count - 1; i >= 0; i--)
                        {
                            var value = mc[i].Result("$1");
                            if (IsValidString(value))
                            {
                                if (thDataWork.OpenFileMode)
                                {
                                    AddRowData(value, ParseData.line, true, false);
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

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
