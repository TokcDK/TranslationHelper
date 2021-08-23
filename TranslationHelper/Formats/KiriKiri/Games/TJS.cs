using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class TJS : KiriKiriBase
    {
        public TJS()
        {
        }

        internal override string Ext()
        {
            return ".tjs";
        }

        protected override void ReadLineMod()
        {
            ProjectData.CurrentProject.ReadLineMod(ref ParseData.Line);
        }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
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
                    var mc = Regex.Matches(ParseData.Line, @"\""([^\""\r\n\\]+(?:\\.[^\""\\]*)*)\""");//get all between '"' include '\"' link: https://stackoverflow.com/questions/2148587/finding-quoted-strings-with-escaped-quotes-in-c-sharp-using-a-regular-expression
                    if (mc.Count > 0)
                    {
                        for (int i = mc.Count - 1; i >= 0; i--)
                        {
                            var value = mc[i].Result("$1");
                            if (IsValidString(value))
                            {
                                if (ProjectData.OpenFileMode)
                                {
                                    AddRowData(value, ParseData.Line, CheckInput: false);
                                }
                                else
                                {
                                    if (ProjectData.TablesLinesDict.ContainsKey(value))
                                    {
                                        ParseData.Line = ParseData.Line
                                            .Remove(mc[i].Index, mc[i].Length)
                                            .Insert(mc[i].Index, "\"" + ProjectData.TablesLinesDict[value] + "\"");
                                    }
                                }
                            }
                        }
                    }
                }

                SaveModeAddLine();
            }

            return 0;
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
