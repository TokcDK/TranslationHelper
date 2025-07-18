﻿using TranslationHelper.Projects;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    class ExportedCSV : FormatStringBase
    {
        public ExportedCSV(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".csv";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!string.IsNullOrWhiteSpace(ParseData.Line) && !ParseData.Line.Contains("DO NOT EDIT"))//skip info lines
            {
                var data = ParseData.Line.Split('\t');
                if (OpenFileMode)
                {
                    AddRowData(data[0], "", isCheckInput: false);
                }
                else
                {
                    var trans = data[0];
                    if (SetTranslation(ref trans))
                    {
                        data[1] = trans.Replace("\t", " ");//replace tabspace with common space because tab is splitter
                    }

                    ParseData.Line = string.Join("\t", data);
                }
            }

            SaveModeAddLine();

            return 0;
        }
    }
}
