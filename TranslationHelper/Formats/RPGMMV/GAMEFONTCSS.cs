using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV
{
    class GAMEFONTCSS : RPGMMVBase
    {
        public GAMEFONTCSS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseFile();
        }

        protected override int ParseFileLine()
        {
            Match r;
            if ((r = Regex.Match(ParseData.line, @"src: url\(\""([^\""]+)\""\)")).Success)
            {
                if (thDataWork.OpenFileMode)
                {
                    ParseData.Ret = AddRowData(r.Result("$1"), T._("GameFont.\r\nFont must be installed in system or file placed in folder %GAME%\\www\\fonts\\ \r\n Change font to smaller is more preferable than line split function"), true, false);

                    return -1;
                }
                else
                {
                    string str;
                    if (thDataWork.TablesLinesDict.ContainsKey(str = r.Result("$1")))
                    {
                        int i;
                        ParseData.line = ParseData.line
                            .Remove(r.Index, r.Value.Length)
                            .Insert(r.Index,
                            r.Value.Remove(i = r.Value.IndexOf(str), str.Length).Insert(i, thDataWork.TablesLinesDict[str])
                            );
                        //ParseData.ResultForWrite.AppendLine(str);
                        ParseData.Ret = true;
                    }
                }
            }
            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

            return 1;
        }

        internal override bool Save()
        {
            thDataWork.SaveFileMode = true;
            return ParseFile();
        }

    }
}
