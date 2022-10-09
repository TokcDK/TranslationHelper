using System;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV
{
    class GAMEFONTCSS : RPGMMVBase
    {
        public GAMEFONTCSS()
        {
        }

        protected override KeywordActionAfter ParseStringFileLine()
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
                    if (OpenFileMode)
                    {
                        ParseData.Ret = AddRowData(r.Result("$1"), T._("GameFont.\r\nFont must be installed in system or file placed in folder %GAME%\\www\\fonts\\ \r\n or use absolute path. \r\n Change font to smaller is more preferable than line split function\r\nexamples:\r\nWin7:c:/windows/fonts/browa.ttf\r\nWin10: %LocalAppData%/Microsoft/Windows/Fonts/browa.ttf"), isCheckInput: false);

                        return KeywordActionAfter.Break;
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
                                r.Value.Remove(i = r.Value.IndexOf(str), str.Length).Insert(i, Environment.ExpandEnvironmentVariables(trans).Replace("\\", "/"))
                                );
                            //ParseData.ResultForWrite.AppendLine(str);
                            ParseData.Ret = true;
                        }
                    }
                }
            }

            SaveModeAddLine();

            return KeywordActionAfter.ReadToEnd;
        }

        protected override bool TrySave()
        {
            SaveFileMode = true;
            return ParseFile();
        }

    }
}
