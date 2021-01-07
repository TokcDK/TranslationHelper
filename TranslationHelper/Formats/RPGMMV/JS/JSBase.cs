using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSBase : RPGMMVBase
    {
        public JSBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        /// <summary>
        /// Get all inherited classes of an abstract class
        /// </summary>
        /// <returns></returns>
        internal static List<JSBase> GetListOfJS(THDataWork thDataWork)
        {
            //https://stackoverflow.com/a/5411981
            //Get all inherited classes of an abstract class
            IEnumerable<JSBase> SubclassesOfJSBase = typeof(JSBase)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(JSBase)) && !t.IsAbstract)
            .Select(t => (JSBase)Activator.CreateInstance(t, thDataWork));

            return (from JSBase SubClass in SubclassesOfJSBase
                    select SubClass).ToList();
        }

        internal abstract string JSName { get; }

        internal virtual string JSSubfolder => "plugins";

        protected static bool IsPluginsJS;//for some specific to plugins.js operations

        protected static bool IsValidToken(JToken token)
        {
            return token.Type == JTokenType.String
                //&& (!IsPluginsJS || (IsPluginsJS && !token.Path.StartsWith("parameters.",StringComparison.InvariantCultureIgnoreCase)))//translation of some parameters can break game
                && !string.IsNullOrWhiteSpace(token.ToString())
                && !(Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && token.ToString().HaveMostOfRomajiOtherChars());
        }

        protected bool PluginsJSnameFound;
        protected void GetStringsFromJToken(JToken token, string Jsonname)
        {
            if (token == null)
            {
                return;
            }


            if (token is JValue)
            {
                var TokenValue = token + "";

                if ((TokenValue.StartsWith("{") && TokenValue.EndsWith("}")) || (TokenValue.StartsWith("[\"") && TokenValue.EndsWith("\"]")))
                {
                    //parse subtoken
                    GetStringsFromJToken(JToken.Parse(TokenValue), Jsonname);
                }
                else
                {
                    if (!IsValidToken(token))
                        return;

                    AddRowData(Jsonname, TokenValue,
                        token.Path
                        + ((IsPluginsJS && token.Path.StartsWith("parameters.", StringComparison.InvariantCultureIgnoreCase))
                        ? Environment.NewLine + T._("Warning") + ". " + T._("Parameter: translation of some parameters can break the game.")
                        : string.Empty)
                        , true);
                }
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    if (!PluginsJSnameFound && IsPluginsJS && property.Name == "name")
                    {
                        PluginsJSnameFound = true;
                        continue;
                    }
                    GetStringsFromJToken(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                var arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    GetStringsFromJToken(array[i], Jsonname);
                }
            }
            else
            {
            }
        }

        protected int rowindex;
        protected void WriteStringsToJToken(JToken token, string Jsonname)
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (!IsValidToken(token))
                    return;

                var row = thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[rowindex];
                if (Equals(token.ToString(), row[0]) && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                {
                    (token as JValue).Value = row[1] as string;
                }
                rowindex++;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    if (!PluginsJSnameFound && Jsonname == "plugins.js" && property.Name == "name")
                    {
                        PluginsJSnameFound = true;
                        continue;
                    }
                    WriteStringsToJToken(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    WriteStringsToJToken(array[i], Jsonname);
                }
            }
            else
            {
            }
        }

        protected void WriteStringsToJTokenWithPreSplitlines(JToken token, string Jsonname)
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                var TokenValue = token + "";

                if ((TokenValue.StartsWith("{") && TokenValue.EndsWith("}")) || (TokenValue.StartsWith("[\"") && TokenValue.EndsWith("\"]")))
                {
                    //parse subtoken
                    var root = JToken.Parse(TokenValue);
                    WriteStringsToJTokenWithPreSplitlines(root, Jsonname);
                    (token as JValue).Value = root.ToString(Formatting.None);
                }
                else
                {
                    if (!IsValidToken(token))
                        return;

                    if (TablesLinesDict.ContainsKey(TokenValue)
                        && !string.IsNullOrEmpty(TablesLinesDict[TokenValue])
                        && TablesLinesDict[TokenValue] != TokenValue)
                    {
                        (token as JValue).Value = TablesLinesDict[TokenValue];
                    }
                }
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    if (!PluginsJSnameFound && Jsonname == "plugins.js" && property.Name == "name")
                    {
                        PluginsJSnameFound = true;
                        continue;
                    }
                    WriteStringsToJTokenWithPreSplitlines(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    WriteStringsToJTokenWithPreSplitlines(array[i], Jsonname);
                }
            }
            else
            {
            }
        }

        protected bool ParseJSVarInJson(string SvarIdentifier)
        {
            string line;

            string tablename = Path.GetFileName(thDataWork.FilePath);

            AddTables(tablename);

            bool StartReadingSvar = false;
            bool IsComment = false;
            StringBuilder Svar = new StringBuilder();
            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (StartReadingSvar)
                    {
                        if (line.TrimStart().StartsWith("};"))
                        {
                            Svar.Append('}');

                            try
                            {
                                JToken root;
                                root = JToken.Parse(Svar.ToString());

                                GetStringsFromJToken(root, tablename);
                            }
                            catch
                            {
                            }

                            break;
                        }
                        else
                        {
                            Svar.AppendLine(line);
                        }
                    }
                    else
                    {
                        //comments
                        if (IsComment)
                        {
                            if (line.Contains("*/"))
                            {
                                IsComment = false;
                            }
                            continue;
                        }
                        else if (line.TrimStart().StartsWith("//"))
                        {
                            continue;
                        }
                        else if (line.TrimStart().StartsWith("/*"))
                        {
                            if (!line.Contains("*/"))
                            {
                                IsComment = true;
                                continue;
                            }
                        }//endcomments
                        else if (line.TrimStart().StartsWith(SvarIdentifier))
                        {
                            StartReadingSvar = true;
                            Svar.AppendLine("{");
                        }
                    }
                }
            }

            return CheckTablesContent(tablename);
        }

        protected bool ParseJSVarInJsonWrite(string SvarIdentifier)
        {
            StringBuilder TranslatedResult = new StringBuilder();
            string line;
            rowindex = 0;

            string tablename = Path.GetFileName(thDataWork.FilePath);
            if (FunctionsTable.IsTableRowsAllEmpty(thDataWork.THFilesElementsDataset.Tables[tablename]))
            {
                return false;
            }

            bool StartReadingSvar = false;
            bool IsComment = false;
            StringBuilder Svar = new StringBuilder();
            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (StartReadingSvar)
                    {
                        if (line.TrimStart().StartsWith("};"))
                        {
                            Svar.Append('}');

                            JToken root = JToken.Parse(Svar.ToString());
                            try
                            {
                                SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                                if (TablesLinesDict != null/* && TablesLinesDict.Count > 0*/)
                                {
                                    WriteStringsToJTokenWithPreSplitlines(root, tablename);
                                }
                                else
                                {
                                    WriteStringsToJToken(root, tablename);
                                }
                            }
                            catch
                            {
                            }

                            StartReadingSvar = false;
                            TranslatedResult.AppendLine(root.ToString(Formatting.Indented) + ";");
                            continue;
                        }
                        else
                        {
                            Svar.AppendLine(line);
                        }
                    }
                    else
                    {
                        //comments
                        if (IsComment)
                        {
                            if (line.Contains("*/"))
                            {
                                IsComment = false;
                            }
                            //continue;
                        }
                        else if (line.TrimStart().StartsWith("//"))
                        {
                            //continue;
                        }
                        else if (line.TrimStart().StartsWith("/*"))
                        {
                            if (!line.Contains("*/"))
                            {
                                IsComment = true;
                                //continue;
                            }
                        }//endcomments
                        else if (!IsComment && line.TrimStart().StartsWith(SvarIdentifier))
                        {
                            StartReadingSvar = true;
                            TranslatedResult.Append(line.Remove(line.Length - 1, 1));
                            Svar.AppendLine("{");
                        }
                    }

                    if (!StartReadingSvar)
                    {
                        TranslatedResult.AppendLine(line);
                    }
                }
            }

            try
            {
                File.WriteAllText(thDataWork.FilePath, TranslatedResult.ToString());
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected bool ParseJSSingleLinesWithRegex(string RegexPattern)
        {
            return ParseJSSingleLinesWithRegex(RegexPattern, false);
        }

        protected bool ParseJSSingleLinesWithRegex(string RegexPattern, bool Iswrite = false)
        {
            return ParseJSSingleLinesWithRegex(RegexPattern, "", true, "$1", "", Iswrite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegexPattern"></param>
        /// <param name="LinePartForSearch"></param>
        /// <param name="LinePartForSearchContains">true=contains,false=startswith</param>
        /// <param name="RegexPartOfPatternToSave"></param>
        /// <param name="SInfo"></param>
        /// <param name="Iswrite"></param>
        /// <returns></returns>
        protected bool ParseJSSingleLinesWithRegex(string RegexPattern, string LinePartForSearch = "", bool LinePartForSearchContains = true, string RegexPartOfPatternToSave = "$1", string SInfo = "", bool Iswrite = false)
        {
            if (thDataWork.FilePath.Length == 0 || !File.Exists(thDataWork.FilePath))
            {
                return false;
            }

            string line;

            string tablename = Path.GetFileName(thDataWork.FilePath);

            bool UseDict = false;
            if (Iswrite)
            {
                SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
                {
                    UseDict = true;
                }
            }
            else
            {
                AddTables(tablename);
            }

            StringBuilder ResultForWrite = new StringBuilder();
            int RowIndex = 0;
            bool IsComment = false;
            try
            {
                bool SearchText = LinePartForSearch.Length > 0;
                using (StreamReader reader = new StreamReader(thDataWork.FilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();

                        if (IsComment)
                        {
                            if (line.Contains("*/"))
                            {
                                IsComment = false;
                            }
                        }
                        else
                        {
                            if (line.Contains("/*"))
                            {
                                IsComment = true;
                            }

                            if (!line.TrimStart().StartsWith("//")
                                &&
                                (
                                (SearchText &&
                                    (LinePartForSearchContains && line.Contains(LinePartForSearch))
                                    ||
                                    (!LinePartForSearchContains && line.StartsWith(LinePartForSearch))
                                )
                                || (!SearchText && Regex.IsMatch(line, RegexPattern))
                                )
                                )
                            {
                                string StringToAdd = Regex.Replace(line, RegexPattern, RegexPartOfPatternToSave);
                                if (IsValidString(StringToAdd))
                                {
                                    if (Iswrite)
                                    {
                                        if (UseDict)
                                        {
                                            if (TablesLinesDict.ContainsKey(StringToAdd) && !string.IsNullOrEmpty(TablesLinesDict[StringToAdd]) && TablesLinesDict[StringToAdd] != StringToAdd)
                                            {
                                                line = line.Replace(StringToAdd, TablesLinesDict[StringToAdd]);
                                            }
                                        }
                                        else
                                        {

                                            var row = thDataWork.THFilesElementsDataset.Tables[tablename].Rows[RowIndex];
                                            if (row[0] as string == StringToAdd && row[1] != null && !string.IsNullOrEmpty(row[1] as string))
                                            {
                                                line = line.Replace(StringToAdd, row[1] as string);
                                            }

                                            RowIndex++;
                                        }
                                    }
                                    else
                                    {
                                        AddRowData(tablename, StringToAdd, SInfo, true);
                                        //thDataWork.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
                                        //thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
                                    }
                                }
                            }
                        }

                        if (Iswrite)
                        {
                            ResultForWrite.AppendLine(line);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            if (Iswrite)
            {
                try
                {
                    File.WriteAllText(thDataWork.FilePath, ResultForWrite.ToString());
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
