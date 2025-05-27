using System;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.KiriKiri
{
    class TSV : FormatStringBase
    {
        public TSV(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".tsv";

        protected override KeywordActionAfter ParseStringFileLine()
        {
            Parse();

            SaveModeAddLine();

            return 0;
        }

        private void Parse()
        {
            if (ParseData.TrimmedLine.StartsWith(";") || !ParseData.Line.Contains("	")) return;


            var lineArray = ParseData.Line.Split(new[] { '	' });

            if (lineArray.Length != 2) return;

            var data = lineArray[1];
            bool dataIsArray;
            if (dataIsArray = data.IndexOf(',') != -1)
            {
                var dataArray = data.Split(new[] { ',' });

                for (int i = dataArray.Length - 1; i >= 0; i--)
                {
                    var dataSubValue = dataArray[i];

                    if (string.IsNullOrWhiteSpace(dataSubValue)) continue;

                    bool dataIsWithSubValue;
                    if (dataIsWithSubValue = dataSubValue.IndexOf(':') != -1)
                    {
                        var dataSubValueArray = dataSubValue.Split(new[] { ':' });
                        if (dataSubValueArray.Length != 2) continue;

                        if (OpenFileMode)
                        {
                            AddRowData(dataSubValueArray[1], "Parent array:" + lineArray[0] + ", Member name:" + dataSubValueArray[0], isCheckInput: true);
                        }
                        else
                        {
                            var translation = dataSubValueArray[1];
                            if (SetTranslation(ref translation))
                            {
                                dataSubValueArray[1] = translation;

                                dataArray[i] = dataSubValueArray[0] + ":" + dataSubValueArray[1];
                            }
                        }
                    }

                    if (!dataIsWithSubValue)
                    {
                        if (OpenFileMode)
                        {
                            AddRowData(dataSubValue, "Parent array:" + lineArray[0], isCheckInput: true);
                        }
                        else
                        {
                            var translation = dataSubValue;
                            if (SetTranslation(ref translation)) dataArray[i] = translation;
                        }
                    }
                }

                if (SaveFileMode) lineArray[1] = string.Join(",", dataArray); // merge all value members
            }

            if (dataIsArray) return;

            if (OpenFileMode)
            {
                AddRowData(lineArray[1], "Varname: " + lineArray[0], isCheckInput: false);
            }
            else
            {
                var translation = lineArray[1];
                if (SetTranslation(ref translation)) ParseData.Line = lineArray[0] + "	" + translation;
            }
        }
    }
}
