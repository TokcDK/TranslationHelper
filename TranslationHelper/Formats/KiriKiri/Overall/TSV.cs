using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri
{
    class TSV : FormatBase
    {
        public TSV()
        {
        }

        internal override string Ext()
        {
            return ".tsv";
        }

        internal override bool ExtIdentifier()
        {
            return true;
        }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.TrimmedLine.StartsWith(";") || !ParseData.Line.Contains("	"))
            {
            }
            else
            {
                var lineArray = ParseData.Line.Split(new[] { '	' });

                if (lineArray.Length != 2)
                {
                }
                else
                {
                    var data = lineArray[1];
                    bool dataIsArray;
                    if (dataIsArray = data.IndexOf(',') != -1)
                    {
                        var dataArray = data.Split(new[] { ',' });

                        for (int i = dataArray.Length - 1; i >= 0; i--)
                        {
                            var dataSubValue = dataArray[i];

                            if (string.IsNullOrWhiteSpace(dataSubValue))
                            {
                                continue;
                            }

                            bool dataIsWithSubValue;
                            if (dataIsWithSubValue = dataSubValue.IndexOf(':') != -1)
                            {
                                var dataSubValueArray = dataSubValue.Split(new[] { ':' });
                                if (dataSubValueArray.Length != 2)
                                {
                                    continue;
                                }

                                if (!IsValidString(dataSubValueArray[1]))
                                {
                                    continue;
                                }

                                if (ProjectData.OpenFileMode)
                                {
                                    AddRowData(dataSubValueArray[1], "Parent array:" + lineArray[0] + ", Member name:" + dataSubValueArray[0], CheckInput: false);
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
                                if (IsValidString(dataSubValue))
                                {
                                    if (ProjectData.OpenFileMode)
                                    {
                                        AddRowData(dataSubValue, "Parent array:" + lineArray[0], CheckInput: false);
                                    }
                                    else
                                    {
                                        var translation = dataSubValue;
                                        if (SetTranslation(ref translation))
                                        {
                                            dataArray[i] = translation;
                                        }
                                    }
                                }
                            }
                        }

                        if (ProjectData.SaveFileMode)
                        {
                            lineArray[1] = string.Join(",", dataArray); // merge all value members
                        }
                    }

                    if (!dataIsArray)
                    {
                        if (IsValidString(lineArray[1]))
                        {
                            if (ProjectData.OpenFileMode)
                            {
                                AddRowData(lineArray[1], "Varname: " + lineArray[0], CheckInput: false);
                            }
                            else
                            {
                                var translation = lineArray[1];
                                if (SetTranslation(ref translation))
                                {
                                    ParseData.Line = lineArray[0] + "	" + translation;
                                }
                            }
                        }
                    }
                }
            }

            SaveModeAddLine();

            return 0;
        }
    }
}
