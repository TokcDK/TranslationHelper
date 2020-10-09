using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7.eve
{
    class TXT : Rajiin7Base
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseFile();
        }

        string Value;
        protected override int ParseFileLine()
        {
            ParseData.TrimmedLine = ParseData.line;

            var ret = 1;

            //commented or empty
            if (string.IsNullOrWhiteSpace(ParseData.line) || ParseData.TrimmedLine.StartsWith("//"))
            {
                ret = 0;
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
                if (thDataWork.OpenFileMode)
                {
                    AddRowData(Values[1], "", true);
                    AddRowData(RestOfText, "", true);
                }
                else
                {
                    if (IsValid(Values[1]))
                    {
                        ParseData.Ret = true;
                        Values[1] = FixInvalidSymbols(thDataWork.TablesLinesDict[Values[1]]);
                    }

                    if (IsValid(RestOfText))
                    {
                        ParseData.Ret = true;
                        RestOfText = FixInvalidSymbols(thDataWork.TablesLinesDict[RestOfText]);
                    }

                    ParseData.line = string.Join(",", Values) + Environment.NewLine + RestOfText;

                }

                ret = -1;
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

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
            return ParseFile();
        }
    }
}
