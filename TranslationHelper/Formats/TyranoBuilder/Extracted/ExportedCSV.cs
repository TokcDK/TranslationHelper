using TranslationHelper.Data;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    class ExportedCSV : FormatBase
    {
        public ExportedCSV(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".csv";
        }

        protected override int ParseStringFileLine()
        {
            if (!string.IsNullOrWhiteSpace(ParseData.line) && !ParseData.line.Contains("DO NOT EDIT"))//skip info lines
            {
                var data = ParseData.line.Split('\t');
                if (thDataWork.OpenFileMode)
                {
                    AddRowData(data[0], "", true, false);
                }
                else
                {
                    AddTranslation(ref data[1], data[0]);
                    data[1] = data[1].Replace("\t"," ");//replace tabspace with common space because tab is splitter
                    ParseData.line = string.Join("\t", data);
                }
            }

            SaveModeAddLine();

            return 0;
        }
    }
}
