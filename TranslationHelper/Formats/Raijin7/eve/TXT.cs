using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7.eve
{
    class Txt : Rajiin7Base
    {
        public Txt()
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
            if (string.IsNullOrWhiteSpace(ParseData.Line) || ParseData.TrimmedLine.StartsWith("//"))
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
            else if (ParseData.Tablename.StartsWith("sn"))
            {
                var values = ParseData.Line.Split(',');
                var restOfText = ParseData.Reader.ReadToEnd();
                if (ProjectData.OpenFileMode)
                {
                    AddRowData(values[1], "", true);
                    AddRowData(restOfText, "", true);
                }
                else
                {
                    var trans = "";
                    if (IsValid(values[1], ref trans))
                    {
                        ParseData.Ret = true;
                        values[1] = FixInvalidSymbols(ProjectData.TablesLinesDict[values[1]]);
                    }

                    if (IsValid(restOfText, ref trans))
                    {
                        ParseData.Ret = true;
                        restOfText = FixInvalidSymbols(ProjectData.TablesLinesDict[restOfText]);
                    }

                    ParseData.Line = string.Join(",", values) + Environment.NewLine + restOfText;

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
