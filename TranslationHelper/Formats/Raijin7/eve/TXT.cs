using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7.eve
{
    class TXT : Rajiin7Base
    {
        public TXT()
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            //ParseData.TrimmedLine = ParseData.line;

            var ret = ParseStringFileLineReturnState.ReadToEnd;

            //commented or empty
            if (string.IsNullOrWhiteSpace(ParseData.line) || ParseData.TrimmedLine.StartsWith("//"))
            {
                ret = ParseStringFileLineReturnState.Continue;
            }
            else if (ParseData.TrimmedLine.StartsWith("set_run_msg")
                || ParseData.TrimmedLine.StartsWith("zin_old_msg")
                || ParseData.TrimmedLine.StartsWith("zin_reg_msg")
                || ParseData.TrimmedLine.StartsWith("zin_reg_event")
                || ParseData.TrimmedLine.StartsWith("zin_reg_qnar")
                || ParseData.TrimmedLine.StartsWith("zin_reg_title")
                || ParseData.TrimmedLine.StartsWith("zin_reg_nar")
                )
            {
                SetValue(1);
            }
            else if (ParseData.TrimmedLine.StartsWith("zin_reg_2sel")
                || ParseData.TrimmedLine.StartsWith("zin_reg_3sel")
                || ParseData.TrimmedLine.StartsWith("zin_regs_msg")
                )
            {
                SetValue(1,2);
            }
            else if (ParseData.TrimmedLine.StartsWith("chg_pname"))
            {
                SetValue(2);
            }
            else if (ParseData.TrimmedLine.StartsWith("get_item"))
            {
                SetValue(3);
            }
            else if (ParseData.TrimmedLine.StartsWith("set_fort"))
            {
                SetValue(5);
            }
            else if (ParseData.tablename.StartsWith("sn"))
            {
                var Values = ParseData.line.Split(',');
                var RestOfText = ParseData.reader.ReadToEnd();
                if (ProjectData.OpenFileMode)
                {
                    AddRowData(Values[1], "", true);
                    AddRowData(RestOfText, "", true);
                }
                else
                {
                    var trans = "";
                    if (IsValid(Values[1], ref trans))
                    {
                        ParseData.Ret = true;
                        Values[1] = FixInvalidSymbols(ProjectData.TablesLinesDict[Values[1]]);
                    }

                    if (IsValid(RestOfText, ref trans))
                    {
                        ParseData.Ret = true;
                        RestOfText = FixInvalidSymbols(ProjectData.TablesLinesDict[RestOfText]);
                    }

                    ParseData.line = string.Join(",", Values) + Environment.NewLine + RestOfText;

                }

                ret = ParseStringFileLineReturnState.Break;
            }

            SaveModeAddLine();

            return ret;
        }


        protected override string FixInvalidSymbols(string str)
        {
            return str
                .Replace(", ", "、")
                .Replace(',', '、')
                ;
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }
    }
}
