using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    class CSV : Rajiin7Base
    {
        public CSV()
        {
        }

        internal override string Ext()
        {
            return ".csv";
        }

        internal override bool Open()
        {
            return ParseFile();
        }
        protected override void PreOpenExtraActions()
        {
            lineNumber = 0;
            if (TableName().StartsWith("fship")
                || TableName().StartsWith("pnet")
                || TableName().StartsWith("pson")
                || TableName().StartsWith("wapon")
                )
            {
                variant = 1;
            }
            else if (TableName().StartsWith("item"))
            {
                variant = 2;
            }
            else if (TableName().StartsWith("spec_rate"))
            {
                variant = 3;
            }
        }

        int lineNumber;
        int variant;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            //ParseData.TrimmedLine = ParseData.line;

            var ret = KeywordActionAfter.ReadToEnd;

            if (variant > 0)
            {
                //commented or empty
                if (string.IsNullOrWhiteSpace(ParseData.Line) || ParseData.Line.TrimStart().StartsWith("//"))
                {
                    ret = KeywordActionAfter.Continue;
                }
                else if (variant == 1)
                {
                    SetValue(1);
                }
                else if (variant == 2)
                {
                    SetValue(0,1);
                }
                else if (variant == 3)
                {
                    if (lineNumber > 0)
                    {
                        SetValue(0);
                    }
                    else
                    {
                        SetValue(999);
                    }
                }
                lineNumber++;
            }

            SaveModeAddLine();

            return ret;
        }

        internal override bool Save()
        {
            return ParseFile();
        }
    }
}
