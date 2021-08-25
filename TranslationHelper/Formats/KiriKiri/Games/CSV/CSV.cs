using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.KSParser.CSV
{
    class CSV : KiriKiriBase
    {
        public CSV()
        {
        }

        internal override string Ext()
        {
            return ".csv";
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            var values = Regex.Matches(ParseData.Line, @"[^\t\r\n]+");
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
                            if (ProjectData.OpenFileMode)
                            {
                                AddRowData(subvalues[s], "", CheckInput: true);
                            }
                            else
                            {
                                if (IsValidString(subvalues[s]) && ProjectData.CurrentProject.TablesLinesDict.ContainsKey(subvalues[s]))
                                {
                                    subvalues[s] = ProjectData.CurrentProject.TablesLinesDict[subvalues[s]];
                                    subvalueschanged = true;
                                }
                            }
                        }

                        if (ProjectData.SaveFileMode && subvalueschanged)
                        {
                            ParseData.Line = ParseData.Line
                                .Remove(values[i].Index, values[i].Length)
                                .Insert(values[i].Index, string.Join(",", subvalues));
                            ParseData.Ret = true;
                        }
                    }
                    else
                    {
                        if (ProjectData.OpenFileMode)
                        {
                            AddRowData(values[i].Value, "", CheckInput: true);
                        }
                        else
                        {
                            var value = values[i].Value;
                            if (IsValidString(value) && ProjectData.CurrentProject.TablesLinesDict.ContainsKey(value))
                            {
                                ParseData.Line = ParseData.Line
                                    .Remove(values[i].Index, values[i].Length)
                                    .Insert(values[i].Index, ProjectData.CurrentProject.TablesLinesDict[value]);
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
