using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class Plugins : JsBase
    {
        public Plugins()
        {
        }

        internal override bool ExtIdentifier()
        {
            return Path.GetFileName(ProjectData.SelectedFilePath).ToUpperInvariant() == "PLUGINS.JS" && Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath)).ToUpperInvariant() == "JS";
        }

        //protected static bool IsPluginsJS = false; //for some specific to plugins.js operations

        protected override bool JsTokenValid(JValue value)
        {
            return value.Path != "Modelname";
        }

        //internal override bool Open()
        //{
        //    return ParseJSArrayOfJsons();
        //}

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (ParseData.TrimmedLine.TrimStart().StartsWith("{\"name\":"))
            {
                try
                {
                    if (ParseData.Line.EndsWith(","))
                    {
                        _isJsonNotLast = true;
                        ParseData.Line = ParseData.Line.Remove(ParseData.Line.Length - 1, 1);
                    }
                    else if (ProjectData.SaveFileMode)
                    {
                        _isJsonNotLast = false;
                    }

                    JsonParser.ParseString(ParseData.Line);

                    //PluginsJsNameFound = false;

                    if (ProjectData.SaveFileMode)
                    {
                        ParseData.Line = JsonParser.JsonRoot.ToString(Formatting.None) + (_isJsonNotLast ? "," : string.Empty);
                        ParseData.Ret = true;
                    }
                }
                catch
                {
                }

                _isJsonNotLast = !_isJsonNotLast;
            }

            SaveModeAddLine("\n");

            return ParseStringFileLineReturnState.Continue;
        }

        //private bool ParseJSArrayOfJsons()
        //{
        //    string line;

        //    string tablename = Path.GetFileName(ProjectData.FilePath);

        //    AddTables(tablename);

        //    using (StreamReader reader = new StreamReader(ProjectData.FilePath))
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
        private bool _isJsonNotLast;

        //private bool ParseJSArrayOfJsonsWrite()
        //{
        //    TranslatedResult = new StringBuilder();
        //    string line;
        //    rowindex = 0;
        //    string tablename = Path.GetFileName(ProjectData.FilePath);
        //    if (FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[tablename]))
        //    {
        //        return false;
        //    }

        //    ProjectData.Main.ProgressInfo(true, T._("Writing") + ": " + "plugins.js");

        //    using (StreamReader reader = new StreamReader(ProjectData.FilePath))
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

        //    File.WriteAllText(ProjectData.FilePath, TranslatedResult.ToString().Replace("\r\n", "\n")/*js using only \n*/);
        //    return true;
        //}

        public override string JsName => "plugins.js";
        public override string JsSubfolder => string.Empty;
    }
}
