using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PLUGINS : JSBase
    {
        public PLUGINS()
        {
        }

        internal override bool ExtIdentifier()
        {
            return Path.GetFileName(ProjectData.SelectedFilePath).ToUpperInvariant() == "PLUGINS.JS" && Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath)).ToUpperInvariant() == "JS";
        }

        //protected static bool IsPluginsJS = false; //for some specific to plugins.js operations

        protected override bool JSTokenValid(JValue value)
        {
            return value.Path != "Modelname";
        }

        //internal override bool Open()
        //{
        //    return ParseJSArrayOfJsons();
        //}

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.TrimmedLine.TrimStart().StartsWith("{\"name\":"))
            {
                try
                {
                    if (ParseData.Line.EndsWith(","))
                    {
                        IsJsonNotLast = true;
                        ParseData.Line = ParseData.Line.Remove(ParseData.Line.Length - 1, 1);
                    }
                    else if (ProjectData.SaveFileMode)
                    {
                        IsJsonNotLast = false;
                    }

                    JsonParser.ParseString(ParseData.Line);

                    //PluginsJsNameFound = false;

                    if (ProjectData.SaveFileMode)
                    {
                        ParseData.Line = JsonParser.JsonRoot.ToString(Formatting.None) + (IsJsonNotLast ? "," : string.Empty);
                        ParseData.Ret = true;
                    }
                }
                catch
                {
                }

                IsJsonNotLast = !IsJsonNotLast;
            }

            SaveModeAddLine(newline: "\n");

            return KeywordActionAfter.Continue;
        }

        //private bool ParseJSArrayOfJsons()
        //{
        //    string line;

        //    string tablename = Path.GetFileName(FilePath);

        //    AddTables(tablename);

        //    using (StreamReader reader = new StreamReader(FilePath))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            line = reader.ReadLine();

        //            if (line.TrimStart().StartsWith("{\"name\":"))
        //            {
        //                try
        //                {
        //                    if (line.EndsWith(","))
        //                    {
        //                        line = line.Remove(line.Length - 1, 1);
        //                    }

        //                    JToken root;
        //                    root = JToken.Parse(line);

        //                    PluginsJsNameFound = false;

        //                    GetStringsFromJToken(root, tablename);
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }
        //    }

        //    return CheckTablesContent(tablename);
        //}

        //internal override bool Save()
        //{
        //    return ParseJSArrayOfJsonsWrite();
        //}

        //StringBuilder TranslatedResult;
        private bool IsJsonNotLast;

        //private bool ParseJSArrayOfJsonsWrite()
        //{
        //    TranslatedResult = new StringBuilder();
        //    string line;
        //    rowindex = 0;
        //    string tablename = Path.GetFileName(FilePath);
        //    if (FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[tablename]))
        //    {
        //        return false;
        //    }

        //    ProjectData.Main.ProgressInfo(true, T._("Writing") + ": " + "plugins.js");

        //    using (StreamReader reader = new StreamReader(FilePath))
        //    {
        //        bool isJsonNotLast = false;
        //        bool IsNotFirstLine = false;
        //        while (!reader.EndOfStream)
        //        {
        //            if (IsNotFirstLine)
        //            {
        //                TranslatedResult.AppendLine();
        //            }

        //            line = reader.ReadLine();

        //            if (line.TrimStart().StartsWith("{\"name\":"))
        //            {
        //                try
        //                {
        //                    if (line.EndsWith(","))
        //                    {
        //                        isJsonNotLast = true;
        //                        line = line.Remove(line.Length - 1, 1);
        //                    }
        //                    else
        //                    {
        //                        isJsonNotLast = false;
        //                    }

        //                    JToken root;
        //                    root = JToken.Parse(line);

        //                    PluginsJsNameFound = false;

        //                    SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
        //                    if (TablesLinesDict != null/* && TablesLinesDict.Count > 0*/)
        //                    {
        //                        WriteStringsToJTokenWithPreSplitlines(root, tablename);
        //                    }
        //                    else
        //                    {
        //                        WriteStringsToJToken(root, tablename);
        //                    }

        //                    line = root.ToString(Formatting.None) + (isJsonNotLast ? "," : string.Empty);
        //                }
        //                catch
        //                {
        //                }
        //                isJsonNotLast = !isJsonNotLast;
        //            }

        //            IsNotFirstLine = true;
        //            TranslatedResult.Append(line);
        //        }
        //    }

        //    File.WriteAllText(FilePath, TranslatedResult.ToString().Replace("\r\n", "\n")/*js using only \n*/);
        //    return true;
        //}

        public override string JSName => "plugins.js";
        public override string JSSubfolder => string.Empty;
    }
}
