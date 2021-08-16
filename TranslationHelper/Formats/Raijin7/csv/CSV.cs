using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    class Csv : Rajiin7Base
    {
        public Csv()
        {
        }

        internal override string Ext()
        {
            return ".csv";
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }
        protected override void ParseStringFilePreOpenExtra()
        {
            _lineNumber = 0;
            if (ParseData.Tablename.StartsWith("fship")
                || ParseData.Tablename.StartsWith("pnet")
                || ParseData.Tablename.StartsWith("pson")
                || ParseData.Tablename.StartsWith("wapon")
                )
            {
                _variant = 1;
            }
            else if (ParseData.Tablename.StartsWith("item"))
            {
                _variant = 2;
            }
            else if (ParseData.Tablename.StartsWith("spec_rate"))
            {
                _variant = 3;
            }
        }

        int _lineNumber;
        int _variant;
        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            //ParseData.TrimmedLine = ParseData.line;

            var ret = ParseStringFileLineReturnState.ReadToEnd;

            if (_variant > 0)
            {
                //commented or empty
                if (string.IsNullOrWhiteSpace(ParseData.Line) || ParseData.Line.TrimStart().StartsWith("//"))
                {
                    ret = ParseStringFileLineReturnState.Continue;
                }
                else if (_variant == 1)
                {
                    SetValue(1);
                }
                else if (_variant == 2)
                {
                    SetValue(0,1);
                }
                else if (_variant == 3)
                {
                    if (_lineNumber > 0)
                    {
                        SetValue(0);
                    }
                    else
                    {
                        SetValue(999);
                    }
                }
                _lineNumber++;
            }

            SaveModeAddLine();

            return ret;
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }
    }
}
