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

        protected override void ReadLineMod()
        {
            thDataWork.CurrentProject.ReadLineMod(ref ParseData.line);
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
                    if (!ParseData.TrimmedLine.EndsWith("*/"))
                    {
                        ParseData.IsComment = true;
                    }
                }
                else if (ParseData.TrimmedLine.StartsWith("//")) //comment
                {
                }
                else
                {
                    var mc = Regex.Matches(ParseData.line, @"\""([^\""\r\n\\]+(?:\\.[^\""\\]*)*)\""");//get all between '"' include '\"' link: https://stackoverflow.com/questions/2148587/finding-quoted-strings-with-escaped-quotes-in-c-sharp-using-a-regular-expression
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
                                    if (thDataWork.TablesLinesDict.ContainsKey(value))
                                    {
                                        ParseData.line = ParseData.line
                                            .Remove(mc[i].Index, mc[i].Length)
                                            .Insert(mc[i].Index, "\"" + thDataWork.TablesLinesDict[value] + "\"");
                                    }
                                }
                            }
                        }
                    }
                }

                if (thDataWork.SaveFileMode)
                {
                    ParseData.ResultForWrite.AppendLine(ParseData.line);
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
