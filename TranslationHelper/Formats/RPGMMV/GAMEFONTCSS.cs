﻿using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV
{
    class GAMEFONTCSS : RPGMMVBase
    {
        public GAMEFONTCSS() : base()
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
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
                else if ((r = Regex.Match(ParseData.line, @"src: url\(\""([^\""]+)\""\)")).Success)
                {
                    if (ProjectData.OpenFileMode)
                    {
                        ParseData.Ret = AddRowData(r.Result("$1"), T._("GameFont.\r\nFont must be installed in system or file placed in folder %GAME%\\www\\fonts\\ \r\n or use absolute path. \r\n Change font to smaller is more preferable than line split function\r\nexample: c:/windows/fonts/browa.ttf"), true, false);

                        return -1;
                    }
                    else
                    {
                        string str = r.Result("$1");
                        var trans = str;
                        if (SetTranslation(ref trans))
                        {
                            int i;
                            ParseData.line = ParseData.line
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

            return 1;
        }

        internal override bool Save()
        {
            ProjectData.SaveFileMode = true;
            return ParseStringFile();
        }

    }
}
