using System.Linq;
using System.Text;
using TranslationHelper.Extensions;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.KiriKiri.Games.CSV
{
    class CSVDisplayName : KiriKiriBase
    {
        public CSVDisplayName(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Extension => ".csv";

        /// <summary>
        /// Determines when need or not need to set info line
        /// </summary>
        bool _isInfoLineFound;
        /// <summary>
        /// Global info line of value data names
        /// </summary>
        string[] _infoLine;
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (!ParseData.TrimmedLine.StartsWith(";") && !string.IsNullOrWhiteSpace(ParseData.Line)) // skip comments and empty lines
            {
                var valuesArray = ParseData.Line.Split(new[] { '\t' }, System.StringSplitOptions.None);

                if (!_isInfoLineFound)
                {
                    _isInfoLineFound = true;
                    _infoLine = valuesArray;

                    if (SaveFileMode)
                    {
                        ParseData.Line = string.Join("\t", valuesArray.Concat(new[] { "displayName" }));
                    }
                }
                else
                {
                    var name = "";
                    bool changed = false;
                    for (int i = 0; i < valuesArray.Length; i++)
                    {
                        var info = _infoLine[i];
                        bool isName = info.Equals("name", System.StringComparison.OrdinalIgnoreCase);
                        if (!isName
                            && !info.Equals("detail", System.StringComparison.OrdinalIgnoreCase)
                            ) continue;

                        var value = valuesArray[i];

                        if(AddRowData(ref value, _infoLine[i]) && SaveFileMode && !isName)
                        {
                            changed = true;

                            valuesArray[i] = value;
                        }

                        if (isName) name = value;
                    }

                    if (changed && SaveFileMode)
                    {
                        ParseData.Line = string.Join("\t", valuesArray.Concat(new[] { name }));
                    }
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
                ParseValue(ref valuesArray[valueIndex], _infoLine[valueIndex]);
            }

            if (SaveFileMode)
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

            if (SaveFileMode)
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

            if (SaveFileMode)
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
