﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        protected static bool IsValidToken(JToken token)
        {
            return token.Type == JTokenType.String && !string.IsNullOrWhiteSpace(token.ToString()) && !(Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && token.ToString().HaveMostOfRomajiOtherChars());
        }

        protected bool PluginsJSnameFound = false;
        protected void GetStringsFromJToken(JToken token, string Jsonname)
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (!IsValidToken(token))
                    return;

                AddRowData(Jsonname, token.ToString(), token.Path, true);
                //thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Add(token.ToString());
                //thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(token.Path);
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
                if (!IsValidToken(token))
                    return;

                string TokenValue = token.ToString();
                if (TablesLinesDict.ContainsKey(TokenValue)
                    && !string.IsNullOrEmpty(TablesLinesDict[TokenValue])
                    && TablesLinesDict[TokenValue] != TokenValue)
                {
                    (token as JValue).Value = TablesLinesDict[TokenValue];
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
                            Svar.Append("}");

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
                            Svar.Append("}");

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
    }
}
