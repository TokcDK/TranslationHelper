using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.CSV
{
    class CSV : KiriKiriBase
    {
        public CSV(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".csv";
        }

        protected override int ParseStringFileLine()
        {
            var values = Regex.Matches(ParseData.line, @"[^\t\r\n]+");
            var maxindex = values.Count - 1;
            for (int i = maxindex; i >= 0; i--)
            {
                if (!values[i].Value.TrimStart().StartsWith(";"))
                {
                    if (values[i].Value.Contains(","))
                    {
                        var subvalues = values[i].Value.Split(',');
                        var subvalueschanged = false;
                        for (int s=0; s< subvalues.Length;s++)
                        {
                            if (thDataWork.OpenFileMode)
                            {
                                AddRowData(subvalues[s], "", true);
                            }
                            else
                            {
                                if (IsValidString(subvalues[s]) && thDataWork.TablesLinesDict.ContainsKey(subvalues[s]))
                                {
                                    subvalues[s] = thDataWork.TablesLinesDict[subvalues[s]];
                                    subvalueschanged = true;
                                }
                            }
                        }

                        if (thDataWork.SaveFileMode && subvalueschanged)
                        {
                            ParseData.line = ParseData.line
                                .Remove(values[i].Index, values[i].Length)
                                .Insert(values[i].Index, string.Join(",", subvalues));
                            ParseData.Ret = true;
                        }
                    }
                    else
                    {
                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(values[i].Value, "", true);
                        }
                        else
                        {
                            var value = values[i].Value;
                            if (IsValidString(value) && thDataWork.TablesLinesDict.ContainsKey(value))
                            {
                                ParseData.line = ParseData.line
                                    .Remove(values[i].Index, values[i].Length)
                                    .Insert(values[i].Index, thDataWork.TablesLinesDict[value]);
                                ParseData.Ret = true;
                            }
                        }
                    }
                }
            }

            SaveModeAddLine();

            return 0;
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
