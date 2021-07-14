using System;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    class CSV : Rajiin7Base
    {
        public CSV(ProjectData projectData) : base(projectData)
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
            lineNumber = 0;
            if (ParseData.tablename.StartsWith("fship")
                || ParseData.tablename.StartsWith("pnet")
                || ParseData.tablename.StartsWith("pson")
                || ParseData.tablename.StartsWith("wapon")
                )
            {
                variant = 1;
            }
            else if (ParseData.tablename.StartsWith("item"))
            {
                variant = 2;
            }
            else if (ParseData.tablename.StartsWith("spec_rate"))
            {
                variant = 3;
            }
        }

        int lineNumber;
        int variant;
        protected override int ParseStringFileLine()
        {
            //ParseData.TrimmedLine = ParseData.line;

            var ret = 1;

            if (variant > 0)
            {
                //commented or empty
                if (string.IsNullOrWhiteSpace(ParseData.line) || ParseData.line.TrimStart().StartsWith("//"))
                {
                    ret = 0;
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
            return ParseStringFile();
        }
    }
}
