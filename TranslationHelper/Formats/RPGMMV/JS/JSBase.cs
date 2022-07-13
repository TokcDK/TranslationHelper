using GetListOfSubClasses;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSBase : RPGMMVBase, IUseJSLocationInfo, IUseJsonParser
    {
        protected JSBase()
        {
            JsonParser = new JSJsonParser(this);
        }

        internal override bool UseTableNameWithoutExtension => false;

        internal override string Ext => ".js";

        internal override string Name => "RPGMakerMV plugin js file";

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<System.Type> GetListOfJSTypes()
        {
            return Inherited.GetListOfInheritedTypes(typeof(JSBase));
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<JSBase> GetListOfJS()
        {
            return Inherited.GetListOfinheritedSubClasses<JSBase>();
        }

        public abstract string JSName { get; }

        public virtual string JSSubfolder => "plugins";

        JsonParserBase JParser;

        public JsonParserBase JsonParser { get => JParser; set => JParser = value; }

        protected bool IsValidToken(JValue value)
        {
            return value.Type == JTokenType.String
                && JSTokenValid(value)
                //&& (!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
                && !string.IsNullOrWhiteSpace(value + "")
                && !(THSettings.SourceLanguageIsJapanese && value.ToString().HaveMostOfRomajiOtherChars());
        }

        protected virtual bool JSTokenValid(JValue value)
        {
            return true;
        }

        // old code
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

        //    string tablename = Path.GetFileName(FilePath);

        //    AddTables(tablename);

        //    bool StartReadingSvar = false;
        //    bool IsComment = false;
        //    StringBuilder Svar = new StringBuilder();
        //    using (StreamReader reader = new StreamReader(FilePath))
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

        //    string tablename = Path.GetFileName(FilePath);
        //    if (FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[tablename]))
        //    {
        //        return false;
        //    }

        //    bool StartReadingSvar = false;
        //    bool IsComment = false;
        //    StringBuilder Svar = new StringBuilder();
        //    using (StreamReader reader = new StreamReader(FilePath))
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
        //        File.WriteAllText(FilePath, TranslatedResult.ToString());
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //protected bool ParseJSSingleLinesWithRegex(string RegexPattern)
        //{
        //    return ParseJSSingleLinesWithRegex(RegexPattern, "", true, "$1", "");
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="regexPattern"></param>
        ///// <param name="lineWithTheString"></param>
        ///// <param name="checkIfContains">true=contains,false=startswith</param>
        ///// <param name="regexGroup"></param>
        ///// <param name="stringInfo"></param>
        ///// <returns></returns>
        //protected bool ParseJSSingleLinesWithRegex(string regexPattern, string lineWithTheString = "", bool checkIfContains = true, string regexGroup = "$1", string stringInfo = "")
        //{
        //    if (FilePath.Length == 0 || !File.Exists(FilePath))
        //    {
        //        return false;
        //    }

        //    string line;

        //    string tablename = Path.GetFileName(FilePath);

        //    bool UseDict = false;
        //    if (ProjectData.SaveFileMode)
        //    {
        //        SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
        //        if (TablesLinesDict != null && TablesLinesDict.Count > 0)
        //        {
        //            UseDict = true;
        //        }
        //    }
        //    else
        //    {
        //        AddTables(tablename);
        //    }

        //    StringBuilder ResultForWrite = new StringBuilder();
        //    int RowIndex = 0;
        //    bool IsComment = false;
        //    try
        //    {
        //        bool SearchText = lineWithTheString.Length > 0;
        //        using (StreamReader reader = new StreamReader(FilePath))
        //        {
        //            while (!reader.EndOfStream)
        //            {
        //                line = reader.ReadLine();

        //                if (IsComment)
        //                {
        //                    if (line.Contains("*/"))
        //                    {
        //                        IsComment = false;
        //                    }
        //                }
        //                else
        //                {
        //                    if (line.Contains("/*"))
        //                    {
        //                        IsComment = true;
        //                    }

        //                    if (!line.TrimStart().StartsWith("//")
        //                        &&
        //                        (
        //                        (SearchText &&
        //                            (checkIfContains && line.Contains(lineWithTheString))
        //                            ||
        //                            (!checkIfContains && line.StartsWith(lineWithTheString))
        //                        )
        //                        || (!SearchText && Regex.IsMatch(line, regexPattern))
        //                        )
        //                        )
        //                    {
        //                        string StringToAdd = Regex.Replace(line, regexPattern, regexGroup);
        //                        if (!IsValidString(StringToAdd))
        //                        {
        //                            continue;
        //                        }

        //                        if (ProjectData.SaveFileMode)
        //                        {
        //                            if (UseDict)
        //                            {
        //                                if (TablesLinesDict.ContainsKey(StringToAdd) && !string.IsNullOrEmpty(TablesLinesDict[StringToAdd]) && TablesLinesDict[StringToAdd] != StringToAdd)
        //                                {
        //                                    line = line.Replace(StringToAdd, TablesLinesDict[StringToAdd]);
        //                                }
        //                            }
        //                            else
        //                            {

        //                                var row = ProjectData.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
        //                                if (row[0] as string == StringToAdd && row[1] != null && !string.IsNullOrEmpty(row[1] as string))
        //                                {
        //                                    line = line.Replace(StringToAdd, row[1] as string);
        //                                }

        //                                RowIndex++;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            AddRowData(tablename, StringToAdd, stringInfo, true);
        //                            //ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
        //                            //ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
        //                        }
        //                    }
        //                }

        //                if (ProjectData.SaveFileMode)
        //                {
        //                    ResultForWrite.AppendLine(line);
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    if (ProjectData.SaveFileMode)
        //    {
        //        try
        //        {
        //            File.WriteAllText(FilePath, ResultForWrite.ToString());
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return CheckTablesContent(tablename);
        //    }
        //}
    }
}
