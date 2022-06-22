using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.KiriKiri.Games.CSV
{
    class CSV : KiriKiriBase
    {
        public CSV()
        {
        }

        internal override string Ext => ".csv";

        /// <summary>
        /// Determines when need or not need to set info line
        /// </summary>
        bool InfoLineFound;
        /// <summary>
        /// Global info line of value data names
        /// </summary>
        string[] InfoLine;

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!ParseData.TrimmedLine.StartsWith(";") && !string.IsNullOrWhiteSpace(ParseData.Line)) // skip comments and empty lines
            {
                var valuesArray = ParseData.Line.Split(new[] { '\t' }, System.StringSplitOptions.None);

                if (InfoLineFound)
                {
                    ParseValues(valuesArray);
                }
                else
                {
                    // set info line
                    InfoLineFound = true;
                    InfoLine = valuesArray;
                }
            }

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }

        /// <summary>
        /// Parse values array
        /// </summary>
        /// <param name="valuesArray"></param>
        private void ParseValues(string[] valuesArray)
        {
            for (int valueIndex = 0; valueIndex < valuesArray.Length; valueIndex++)
            {
                ParseValue(ref valuesArray[valueIndex], InfoLine[valueIndex]);
            }

            if (AppData.CurrentProject.SaveFileMode)
            {
                ParseData.Line = string.Join("\t", valuesArray);
            }
        }

        /// <summary>
        /// Any value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="info"></param>
        private void ParseValue(ref string value, string info)
        {
            if (IsArray(value))
            {
                ParseArray(ref value, info);
            }
            else
            if (IsWithValue(value))
            {
                ParseWithValue(ref value, info);
            }
            else if (value != "true" && value != "false" && !value.IsDigitsOnly()) // Usual value to add
            {
                AddRowData(ref value, info);
            }
        }

        /// <summary>
        /// Determine if the <paramref name="value"/> contains value ":" split symbol
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsWithValue(string value)
        {
            return value.IndexOf(':') != -1;
        }

        /// <summary>
        /// Determine if the <paramref name="value"/> contains array "," split symbol
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsArray(string value)
        {
            return value.IndexOf(',') != -1;
        }

        /// <summary>
        /// Array of values "like aaa,bbb,ccc,ddd"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="info"></param>
        private void ParseArray(ref string value, string info)
        {
            var valueArray = value.Split(',');

            for (int valueArrayIndex = 0; valueArrayIndex < valueArray.Length; valueArrayIndex++)
            {
                ParseValue(ref valueArray[valueArrayIndex], info);
            }

            if (AppData.CurrentProject.SaveFileMode)
            {
                value = string.Join(",", valueArray);
            }
        }

        /// <summary>
        /// value like "Value:10"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="info"></param>
        private void ParseWithValue(ref string value, string info)
        {
            var subvalueArray = value.Split(':');

            AddRowData(ref subvalueArray[0], info);

            if (AppData.CurrentProject.SaveFileMode)
            {
                value = string.Join(":", subvalueArray);
            }
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
