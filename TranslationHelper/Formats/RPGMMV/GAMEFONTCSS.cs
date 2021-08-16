using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV
{
    class Gamefontcss : RpgmmvBase
    {
        public Gamefontcss()
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
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
                Match r;
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
                else if ((r = Regex.Match(ParseData.Line, @"src: url\(\""([^\""]+)\""\)")).Success)
                {
                    if (ProjectData.OpenFileMode)
                    {
                        ParseData.Ret = AddRowData(r.Result("$1"), T._("GameFont.\r\nFont must be installed in system or file placed in folder %GAME%\\www\\fonts\\ \r\n or use absolute path. \r\n Change font to smaller is more preferable than line split function\r\nexample: c:/windows/fonts/browa.ttf"), true, false);

                        return ParseStringFileLineReturnState.Break;
                    }
                    else
                    {
                        string str = r.Result("$1");
                        var trans = str;
                        if (SetTranslation(ref trans))
                        {
                            int i;
                            ParseData.Line = ParseData.Line
                                .Remove(r.Index, r.Value.Length)
                                .Insert(r.Index,
                                r.Value.Remove(i = r.Value.IndexOf(str), str.Length).Insert(i, trans)
                                );
                            //ParseData.ResultForWrite.AppendLine(str);
                            ParseData.Ret = true;
                        }
                    }
                }
            }

            SaveModeAddLine();

            return ParseStringFileLineReturnState.ReadToEnd;
        }

        internal override bool Save()
        {
            ProjectData.SaveFileMode = true;
            return ParseStringFile();
        }

    }
}
