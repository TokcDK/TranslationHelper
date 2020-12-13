using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.AliceSoft
{
    class AINTXT : AliceSoftBase
    {
        public AINTXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        string lastgroupname = "";
        readonly string ainstringpattern = @"^;?[sm]\[[0-9]{1,10}\] \= \""(.+)\""$";
        protected override int ParseStringFileLine()
        {
            var m = Regex.Match(ParseData.line, ainstringpattern);
            if (m.Success)
            {
                var str = m.Result("$1");

                if (IsValidString(str))
                {
                    if (thDataWork.OpenFileMode)
                    {
                        AddRowData(str, T._("Last group") + ": " + lastgroupname, true, false);
                    }
                    else
                    {
                        if (thDataWork.TablesLinesDict.ContainsKey(str))
                        {
                            //set translation and replace in orig line
                            var ind = ParseData.line.IndexOf(str);
                            int lngth = str.Length;
                            str = thDataWork.TablesLinesDict[str];
                            str = str.Replace("\"", "'")/*fix syntax errors */.Replace("\u200B", "")/*fix invalid bytes sequence*/;
                            ParseData.line = ParseData.line.Remove(ind, lngth).Insert(ind, str);

                            if (ParseData.line.StartsWith(";"))
                            {
                                ParseData.line = ParseData.line.Remove(0, 1);
                            }
                            ParseData.Ret = true;
                        }
                    }
                }
            }
            else
            {
                lastgroupname = ParseData.line;
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

            return 0;
        }
    }
}
