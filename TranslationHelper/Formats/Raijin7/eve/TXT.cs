using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7.eve
{
    class TXT : Rajiin7Base
    {
        public TXT()
        {
        }

        internal override string Ext => ".txt";

        internal override bool TryOpen()
        {
            return ParseFile();
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            //ParseData.TrimmedLine = ParseData.line;

            var ret = KeywordActionAfter.ReadToEnd;

            //commented or empty
            if (string.IsNullOrWhiteSpace(ParseData.Line) || ParseData.TrimmedLine.StartsWith("//"))
            {
                ret = KeywordActionAfter.Continue;
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
            else if (FileName.StartsWith("sn"))
            {
                var Values = ParseData.Line.Split(',');
                var RestOfText = ParseData.Reader.ReadToEnd();
                if (AppData.CurrentProject.OpenFileMode)
                {
                    AddRowData(Values[1], "", CheckInput: true);
                    AddRowData(RestOfText, "", CheckInput: true);
                }
                else
                {
                    var trans = "";
                    if (IsValid(Values[1], ref trans))
                    {
                        ParseData.Ret = true;
                        Values[1] = FixInvalidSymbols(AppData.CurrentProject.TablesLinesDict[Values[1]]);
                    }

                    if (IsValid(RestOfText, ref trans))
                    {
                        ParseData.Ret = true;
                        RestOfText = FixInvalidSymbols(AppData.CurrentProject.TablesLinesDict[RestOfText]);
                    }

                    ParseData.Line = string.Join(",", Values) + Environment.NewLine + RestOfText;

                }

                ret = KeywordActionAfter.Break;
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

        internal override bool TrySave()
        {
            return ParseFile();
        }
    }
}
