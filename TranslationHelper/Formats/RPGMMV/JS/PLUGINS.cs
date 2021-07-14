﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PLUGINS : JSBase
    {
        public PLUGINS() : base()
        {
            IsPluginsJS = true;
        }

        internal override bool ExtIdentifier()
        {
            return Path.GetFileName(ProjectData.SPath).ToUpperInvariant() == "PLUGINS.JS" && Path.GetFileName(Path.GetDirectoryName(ProjectData.SPath)).ToUpperInvariant() == "JS";
        }

        internal override bool Open()
        {
            return ParseJSArrayOfJsons();
        }

        private bool ParseJSArrayOfJsons()
        {
            string line;

            string tablename = Path.GetFileName(ProjectData.FilePath);

            AddTables(tablename);

            using (StreamReader reader = new StreamReader(ProjectData.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line.TrimStart().StartsWith("{\"name\":"))
                    {
                        try
                        {
                            if (line.EndsWith(","))
                            {
                                line = line.Remove(line.Length - 1, 1);
                            }

                            JToken root;
                            root = JToken.Parse(line);

                            PluginsJSnameFound = false;

                            GetStringsFromJToken(root, tablename);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return CheckTablesContent(tablename);
        }

        internal override bool Save()
        {
            return ParseJSArrayOfJsonsWrite();
        }

        StringBuilder TranslatedResult;
        private bool ParseJSArrayOfJsonsWrite()
        {
            TranslatedResult = new StringBuilder();
            string line;
            rowindex = 0;
            string tablename = Path.GetFileName(ProjectData.FilePath);
            if (FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[tablename]))
            {
                return false;
            }

            ProjectData.Main.ProgressInfo(true, T._("Writing") + ": " + "plugins.js");

            using (StreamReader reader = new StreamReader(ProjectData.FilePath))
            {
                bool isJsonNotLast = false;
                bool IsNotFirstLine = false;
                while (!reader.EndOfStream)
                {
                    if (IsNotFirstLine)
                    {
                        TranslatedResult.AppendLine();
                    }

                    line = reader.ReadLine();

                    if (line.TrimStart().StartsWith("{\"name\":"))
                    {
                        try
                        {
                            if (line.EndsWith(","))
                            {
                                isJsonNotLast = true;
                                line = line.Remove(line.Length - 1, 1);
                            }
                            else
                            {
                                isJsonNotLast = false;
                            }

                            JToken root;
                            root = JToken.Parse(line);

                            PluginsJSnameFound = false;

                            SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                            if (TablesLinesDict != null/* && TablesLinesDict.Count > 0*/)
                            {
                                WriteStringsToJTokenWithPreSplitlines(root, tablename);
                            }
                            else
                            {
                                WriteStringsToJToken(root, tablename);
                            }

                            line = root.ToString(Formatting.None) + (isJsonNotLast ? "," : string.Empty);
                        }
                        catch
                        {
                        }
                        isJsonNotLast = !isJsonNotLast;
                    }

                    IsNotFirstLine = true;
                    TranslatedResult.Append(line);
                }
            }

            File.WriteAllText(ProjectData.FilePath, TranslatedResult.ToString().Replace("\r\n", "\n")/*js using only \n*/);
            return true;
        }

        internal override string JSName => "plugins.js";
        internal override string JSSubfolder => string.Empty;
    }
}
