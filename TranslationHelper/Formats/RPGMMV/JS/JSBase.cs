using GetListOfSubClasses;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JsBase : RpgmmvBase, IUseJsLocationInfo, IUseJsonParser
    {
        protected JsBase()
        {
            JsonParser = new JsJsonParser();
        }

        internal override bool UseTableNameWithoutExtension => false;

        internal override string Ext()
        {
            return ".js";
        }

        internal override string Name()
        {
            return "RPGMakerMV plugin js file";
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<System.Type> GetListOfJsTypes()
        {
            return Inherited.GetListOfInheritedTypes(typeof(JsBase));
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<JsBase> GetListOfJs()
        {
            return Inherited.GetListOfinheritedSubClasses<JsBase>();
        }

        public abstract string JsName { get; }

        public virtual string JsSubfolder => "plugins";

        JsonParserBase _jParser;

        public JsonParserBase JsonParser { get => _jParser; set => _jParser = value; }

        protected bool IsValidToken(JValue value)
        {
            return value.Type == JTokenType.String
                && JsTokenValid(value)
                //&& (!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
                && !string.IsNullOrWhiteSpace(value + "")
                && !(ThSettings.SourceLanguageIsJapanese() && value.ToString().HaveMostOfRomajiOtherChars());
        }

        protected virtual bool JsTokenValid(JValue value)
        {
            return true;
        }

        //protected bool PluginsJsNameFound;

        //protected void GetStringsFromJToken(JToken token, string Jsonname)
        //{
        //    if (token == null)
        //    {
        //        return;
        //    }

        //    switch (token)
        //    {
        //        case JValue value:
        //            {
        //                var TokenValue = value + "";

        //                if (TokenValue.StartsWith("{") && TokenValue.EndsWith("}") || TokenValue.StartsWith("[\"") && TokenValue.EndsWith("\"]"))
        //                {
        //                    //parse subtoken
        //                    GetStringsFromJToken(JToken.Parse(TokenValue), Jsonname);
        //                }
        //                else
        //                {
        //                    if (!IsValidToken(value))
        //                        return;

        //                    AddRowData(Jsonname, TokenValue,
        //                        value.Path
        //                        + (IsPluginsJS && value.Path.StartsWith("parameters.", StringComparison.InvariantCultureIgnoreCase)
        //                        ? Environment.NewLine + T._("Warning") + ". " + T._("Parameter: translation of some parameters can break the game.")
        //                        : string.Empty)
        //                        , true);
        //                }

        //                break;
        //            }

        //        case JObject obj:
        //            {
        //                //LogToFile("JObject Properties: \r\n" + obj.Properties());
        //                foreach (var property in obj.Properties())
        //                {
        //                    if (!PluginsJsNameFound && IsPluginsJS && property.Name == "name")
        //                    {
        //                        PluginsJsNameFound = true; // switch off check of parse plugin's json data

        //                        if (obj.Last is JProperty lastObjectsProperty)
        //                        {
        //                            GetStringsFromJToken(lastObjectsProperty.Value, Jsonname); // parse only parameters
        //                            break; // skip rest of properties because last was parsed
        //                        }
        //                        else
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    GetStringsFromJToken(property.Value, Jsonname);
        //                }

        //                PluginsJsNameFound = false; // switch on check of parse plugin's json data
        //                break;
        //            }

        //        case JArray array:
        //            {
        //                var arrayCount = array.Count;
        //                for (int i = 0; i < arrayCount; i++)
        //                {
        //                    GetStringsFromJToken(array[i], Jsonname);
        //                }

        //                break;
        //            }
        //    }
        //}

        //protected int rowindex;
        //protected void WriteStringsToJToken(JToken token, string Jsonname)
        //{
        //    if (token == null)
        //    {
        //        return;
        //    }

        //    switch (token)
        //    {
        //        case JValue value:
        //            {
        //                var TokenValue = value + "";
        //                if (TokenValue.StartsWith("{") && TokenValue.EndsWith("}") || TokenValue.StartsWith("[\"") && TokenValue.EndsWith("\"]")) // if value is json value
        //                {
        //                    //parse subtoken
        //                    var root = JToken.Parse(TokenValue);
        //                    WriteStringsToJTokenWithPreSplitlines(root, Jsonname);
        //                    value.Value = root.ToString(Formatting.None);
        //                }
        //                else
        //                {
        //                    if (!IsValidToken(value))
        //                        return;

        //                    var row = ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows[rowindex];
        //                    if (Equals(value.ToString(), row[0]) && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
        //                    {
        //                        value.Value = row[1] as string;
        //                    }
        //                    rowindex++;
        //                }
        //                break;
        //            }

        //        case JObject obj:
        //            {
        //                //LogToFile("JObject Properties: \r\n" + obj.Properties());
        //                foreach (var property in obj.Properties())
        //                {
        //                    if (!PluginsJsNameFound && IsPluginsJS && property.Name == "name")
        //                    {
        //                        PluginsJsNameFound = true; // switch off check of parse plugin's json data

        //                        if (obj.Last is JProperty lastObjectsProperty)
        //                        {
        //                            GetStringsFromJToken(lastObjectsProperty.Value, Jsonname); // parse only parameters
        //                            break; // skip rest of properties because last was parsed
        //                        }
        //                        else
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                    WriteStringsToJToken(property.Value, Jsonname);
        //                }

        //                PluginsJsNameFound = false; // switch on check of parse plugin's json data
        //                break;
        //            }

        //        case JArray array:
        //            {
        //                for (int i = 0; i < array.Count; i++)
        //                {
        //                    WriteStringsToJToken(array[i], Jsonname);
        //                }

        //                break;
        //            }

        //        default:
        //            break;
        //    }
        //}

        //protected void WriteStringsToJTokenWithPreSplitlines(JToken token, string Jsonname)
        //{
        //    if (token == null)
        //    {
        //        return;
        //    }

        //    switch (token)
        //    {
        //        case JValue value:
        //            {
        //                var TokenValue = value + "";

        //                if (TokenValue.StartsWith("{") && TokenValue.EndsWith("}") || TokenValue.StartsWith("[\"") && TokenValue.EndsWith("\"]")) // if value is json value
        //                {
        //                    //parse subtoken
        //                    var root = JToken.Parse(TokenValue);
        //                    WriteStringsToJTokenWithPreSplitlines(root, Jsonname);
        //                    value.Value = root.ToString(Formatting.None);
        //                }
        //                else
        //                {
        //                    if (!IsValidToken(value))
        //                        return;

        //                    if (TablesLinesDict.ContainsKey(TokenValue)
        //                        && !string.IsNullOrEmpty(TablesLinesDict[TokenValue])
        //                        && TablesLinesDict[TokenValue] != TokenValue)
        //                    {
        //                        value.Value = TablesLinesDict[TokenValue];
        //                    }
        //                }

        //                break;
        //            }

        //        case JObject obj:
        //            {
        //                //LogToFile("JObject Properties: \r\n" + obj.Properties());
        //                foreach (var property in obj.Properties())
        //                {
        //                    if (!PluginsJsNameFound && Jsonname == "plugins.js" && property.Name == "name")
        //                    {
        //                        PluginsJsNameFound = true;
        //                        continue;
        //                    }
        //                    WriteStringsToJTokenWithPreSplitlines(property.Value, Jsonname);
        //                }

        //                break;
        //            }

        //        case JArray array:
        //            {
        //                for (int i = 0; i < array.Count; i++)
        //                {
        //                    WriteStringsToJTokenWithPreSplitlines(array[i], Jsonname);
        //                }

        //                break;
        //            }

        //        default:
        //            break;
        //    }
        //}


        //protected bool ParseJSVarInJson(string SvarIdentifier)
        //{
        //    string line;

        //    string tablename = Path.GetFileName(ProjectData.FilePath);

        //    AddTables(tablename);

        //    bool StartReadingSvar = false;
        //    bool IsComment = false;
        //    StringBuilder Svar = new StringBuilder();
        //    using (StreamReader reader = new StreamReader(ProjectData.FilePath))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            line = reader.ReadLine();
        //            if (StartReadingSvar)
        //            {
        //                if (line.TrimStart().StartsWith("};"))
        //                {
        //                    Svar.Append('}');

        //                    try
        //                    {
        //                        JSJsonParser.ParseString(Svar.ToString());
        //                    }
        //                    catch
        //                    {
        //                    }

        //                    break;
        //                }
        //                else
        //                {
        //                    Svar.AppendLine(line);
        //                }
        //            }
        //            else
        //            {
        //                //comments
        //                if (IsComment)
        //                {
        //                    if (line.Contains("*/"))
        //                    {
        //                        IsComment = false;
        //                    }
        //                    continue;
        //                }
        //                else if (line.TrimStart().StartsWith("//"))
        //                {
        //                    continue;
        //                }
        //                else if (line.TrimStart().StartsWith("/*"))
        //                {
        //                    if (!line.Contains("*/"))
        //                    {
        //                        IsComment = true;
        //                        continue;
        //                    }
        //                }//endcomments
        //                else if (line.TrimStart().StartsWith(SvarIdentifier))
        //                {
        //                    StartReadingSvar = true;
        //                    Svar.AppendLine("{");
        //                }
        //            }
        //        }
        //    }

        //    return CheckTablesContent(tablename);
        //}

        //protected bool ParseJSVarInJsonWrite(string SvarIdentifier)
        //{
        //    StringBuilder TranslatedResult = new StringBuilder();
        //    string line;
        //    rowindex = 0;

        //    string tablename = Path.GetFileName(ProjectData.FilePath);
        //    if (FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[tablename]))
        //    {
        //        return false;
        //    }

        //    bool StartReadingSvar = false;
        //    bool IsComment = false;
        //    StringBuilder Svar = new StringBuilder();
        //    using (StreamReader reader = new StreamReader(ProjectData.FilePath))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            line = reader.ReadLine();
        //            if (StartReadingSvar)
        //            {
        //                if (line.TrimStart().StartsWith("};"))
        //                {
        //                    Svar.Append('}');

        //                    JToken root = JToken.Parse(Svar.ToString());
        //                    try
        //                    {
        //                        SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
        //                        if (TablesLinesDict != null/* && TablesLinesDict.Count > 0*/)
        //                        {
        //                            WriteStringsToJTokenWithPreSplitlines(root, tablename);
        //                        }
        //                        else
        //                        {
        //                            WriteStringsToJToken(root, tablename);
        //                        }
        //                    }
        //                    catch
        //                    {
        //                    }

        //                    StartReadingSvar = false;
        //                    TranslatedResult.AppendLine(root.ToString(Formatting.Indented) + ";");
        //                    continue;
        //                }
        //                else
        //                {
        //                    Svar.AppendLine(line);
        //                }
        //            }
        //            else
        //            {
        //                //comments
        //                if (IsComment)
        //                {
        //                    if (line.Contains("*/"))
        //                    {
        //                        IsComment = false;
        //                    }
        //                    //continue;
        //                }
        //                else if (line.TrimStart().StartsWith("//"))
        //                {
        //                    //continue;
        //                }
        //                else if (line.TrimStart().StartsWith("/*"))
        //                {
        //                    if (!line.Contains("*/"))
        //                    {
        //                        IsComment = true;
        //                        //continue;
        //                    }
        //                }//endcomments
        //                else if (line.TrimStart().StartsWith(SvarIdentifier))
        //                {
        //                    StartReadingSvar = true;
        //                    TranslatedResult.Append(line.Remove(line.Length - 1, 1));
        //                    Svar.AppendLine("{");
        //                }
        //            }

        //            if (!StartReadingSvar)
        //            {
        //                TranslatedResult.AppendLine(line);
        //            }
        //        }
        //    }

        //    try
        //    {
        //        File.WriteAllText(ProjectData.FilePath, TranslatedResult.ToString());
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        protected bool ParseJsSingleLinesWithRegex(string regexPattern)
        {
            return ParseJsSingleLinesWithRegex(regexPattern, false);
        }

        protected bool ParseJsSingleLinesWithRegex(string regexPattern, bool iswrite = false)
        {
            return ParseJsSingleLinesWithRegex(regexPattern, "", true, "$1", "", iswrite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regexPattern"></param>
        /// <param name="linePartForSearch"></param>
        /// <param name="linePartForSearchContains">true=contains,false=startswith</param>
        /// <param name="regexPartOfPatternToSave"></param>
        /// <param name="sInfo"></param>
        /// <param name="iswrite"></param>
        /// <returns></returns>
        protected bool ParseJsSingleLinesWithRegex(string regexPattern, string linePartForSearch = "", bool linePartForSearchContains = true, string regexPartOfPatternToSave = "$1", string sInfo = "", bool iswrite = false)
        {
            if (ProjectData.FilePath.Length == 0 || !File.Exists(ProjectData.FilePath))
            {
                return false;
            }

            string line;

            string tablename = Path.GetFileName(ProjectData.FilePath);

            bool useDict = false;
            if (iswrite)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
                {
                    useDict = true;
                }
            }
            else
            {
                AddTables(tablename);
            }

            StringBuilder resultForWrite = new StringBuilder();
            int rowIndex = 0;
            bool isComment = false;
            try
            {
                bool searchText = linePartForSearch.Length > 0;
                using (StreamReader reader = new StreamReader(ProjectData.FilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (isComment)
                        {
                            if (line.Contains("*/"))
                            {
                                isComment = false;
                            }
                        }
                        else
                        {
                            if (line.Contains("/*"))
                            {
                                isComment = true;
                            }

                            if (!line.TrimStart().StartsWith("//")
                                &&
                                (
                                (searchText &&
                                    (linePartForSearchContains && line.Contains(linePartForSearch))
                                    ||
                                    (!linePartForSearchContains && line.StartsWith(linePartForSearch))
                                )
                                || (!searchText && Regex.IsMatch(line, regexPattern))
                                )
                                )
                            {
                                string stringToAdd = Regex.Replace(line, regexPattern, regexPartOfPatternToSave);
                                if (IsValidString(stringToAdd))
                                {
                                    if (iswrite)
                                    {
                                        if (useDict)
                                        {
                                            if (TablesLinesDict.ContainsKey(stringToAdd) && !string.IsNullOrEmpty(TablesLinesDict[stringToAdd]) && TablesLinesDict[stringToAdd] != stringToAdd)
                                            {
                                                line = line.Replace(stringToAdd, TablesLinesDict[stringToAdd]);
                                            }
                                        }
                                        else
                                        {

                                            var row = ProjectData.ThFilesElementsDataset.Tables[tablename].Rows[rowIndex];
                                            if (row[0] as string == stringToAdd && row[1] != null && !string.IsNullOrEmpty(row[1] as string))
                                            {
                                                line = line.Replace(stringToAdd, row[1] as string);
                                            }

                                            rowIndex++;
                                        }
                                    }
                                    else
                                    {
                                        AddRowData(tablename, stringToAdd, sInfo, true);
                                        //ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
                                        //ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
                                    }
                                }
                            }
                        }

                        if (iswrite)
                        {
                            resultForWrite.AppendLine(line);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            if (iswrite)
            {
                try
                {
                    File.WriteAllText(ProjectData.FilePath, resultForWrite.ToString());
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return CheckTablesContent(tablename);
            }
        }
    }
}
