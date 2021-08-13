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
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMMV
{
    class JSON : RPGMMVBase
    {
        public JSON() : base()
        {
        }

        internal override bool Open()
        {
            return ReadJson(Path.GetFileNameWithoutExtension(ProjectData.FilePath), ProjectData.FilePath);
        }

        internal override string Ext()
        {
            return ".json";
        }

        private bool ReadJson(string jsonName, string sPath)
        {
            //LogToFile("jsonName = " + jsonName);
            try
            {
                AddTables(jsonName);

                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(ProjectData.SelectedDir);
                //using (var reader = new StreamReader(ProjectData.SelectedDir+"\\"+ jsonName+".json"))
                using (StreamReader reader = new StreamReader(sPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, jsonName);
                    }
                }

                //ds.Tables.Add(jsonName); // create table with json name
                //ds.Tables[jsonName].Columns.Add("Original"); //create Original column


                //treeView1.BeginUpdate();
                // treeView1.Nodes.Clear();
                //var tNode = treeView1.Nodes[treeView1.Nodes.Add(new TreeNode(rootName))];
                //tNode.Tag = root;

                //Stopwatch timer = new Stopwatch();

                //timer.Start();

                //TempList = new List<string>();
                //TempListInfo = new List<string>();
                IsWithMergedMessages = IsContainsLinedMessages(jsonName);//оказалось, что сообщения есть не только в CommonEvents, но и в maps,troops и возможно других файлах

                MessageMerged = new StringBuilder();
                AddedJObjects = new HashSet<JObject>();

                ParseJToken(root, jsonName);
                //ProceedJTokenAny(root, jsonName);

                MessageMerged = null;
                //занесение в список
                //int TempListCount = TempList.Count;
                //for (int i = 0; i < TempListCount; i++)
                //{
                //    THFilesElementsDataset.Tables[jsonName].Rows.Add(TempList[i]);
                //    THFilesElementsDatasetInfo.Tables[jsonName].Rows.Add(TempListInfo[i]);
                //}

                //TempList = null;
                //TempListInfo = null;

                //timer.Stop();
                //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
                //FileWriter.WriteData(Path.Combine(Application.StartupPath, "Timers 4 Table.log"), jsonName + ": " + difference + Environment.NewLine);
                //MessageBox.Show(difference.ToString());

                //treeView1.ExpandAll();
            }
            catch (JsonReaderException ex)
            {
                ProjectData.AppLog.LogToFile("Error occured while json read (json is empty or corrupted): \r\n" + ex);
            }
            catch (Exception ex)
            {
                ProjectData.AppLog.LogToFile("Error occured while json parse: \r\n" + ex);
                //LogToFile(string.Empty, true);
            }
            finally
            {
                //LogToFile(string.Empty, true);
                //MessageBox.Show("sss");
                //ds.Tables[jsonName].Columns.Add("Translation");
                //ds.Tables[jsonName].Columns["Original"].ReadOnly = true;
                //DGV.DataSource = ds.Tables[0];
                //treeView1.EndUpdate();
            }
            //LogToFile(string.Empty, true);

            return CheckTablesContent(jsonName);
        }

        //bool MessageParsing = false;
        ///// <summary>
        ///// For Any Json
        ///// </summary>
        ///// <param name="JsonToken"></param>
        ///// <param name="JsonName"></param>
        //private void ParseJTokenAny(JToken JsonToken, string JsonName)
        //{
        //    if (JsonToken == null)
        //    {
        //        return;
        //    }

        //    if (JsonToken is JValue JsonValue)
        //    {
        //        if (JsonToken.Type != JTokenType.String)
        //        {
        //            return;
        //        }

        //        AddRowData(JsonName, JsonValue.Value as string, JsonValue.Path);
        //    }
        //    else if (JsonToken is JObject JsonObject)
        //    {
        //        //LogToFile("JObject Properties: \r\n" + obj.Properties());
        //        foreach (var property in JsonObject.Properties())
        //        {
        //            ParseJTokenAny(property.Value, JsonName);
        //        }
        //    }
        //    else if (JsonToken is JArray JsonArray)
        //    {
        //        foreach (var Element in JsonArray)
        //        {
        //            ParseJTokenAny(Element, JsonName);
        //        }
        //    }
        //    else
        //    {
        //        //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
        //    }
        //}

        ///// <summary>
        ///// (WIP) for RPG Maker MV CommonEvents.json
        ///// </summary>
        ///// <param name="JsonToken"></param>
        ///// <param name="JsonName"></param>
        //private void ParseJTokenCommonEvents(JToken JsonToken, string JsonName)
        //{
        //    if (JsonToken == null)
        //    {
        //        return;
        //    }

        //    if (JsonToken is JValue JsonValue)
        //    {
        //        if (JsonToken.Type != JTokenType.String)
        //        {
        //            return;
        //        }

        //        string tokenvalue = JsonValue.Value as string;
        //    }
        //    else if (JsonToken is JObject JsonObject)
        //    {
        //        //LogToFile("JObject Properties: \r\n" + obj.Properties());
        //        foreach (var property in JsonObject.Properties())
        //        {
        //            if (MessageParsing)
        //            {
        //            }
        //            propertyName = property.Name;
        //            if (!MessageParsing && propertyName == "code" && IsMessageCode(property.Value + string.Empty))
        //            {
        //                MessageParsing = true;

        //            }
        //            ParseJTokenCommonEvents(property.Value, JsonName);
        //        }
        //    }
        //    else if (JsonToken is JArray JsonArray)
        //    {
        //        foreach (var Element in JsonArray)
        //        {
        //            ParseJTokenCommonEvents(Element, JsonName);
        //        }
        //    }
        //    else
        //    {
        //        //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
        //    }
        //}

        private StringBuilder MessageMerged;
        private int CurrentEventCode = -1;
        //string cType;
        //private string cCode = string.Empty;
        private string propertyName = string.Empty;
        //private string cId = string.Empty;
        //private string OldcId = "none";
        bool IsWithMergedMessages;
        private void ParseJToken(JToken jsonToken, string jsonName/*, string propertyname = ""*/)
        {
            if (jsonToken == null)
            {
                return;
            }

            switch (jsonToken)
            {
                case JValue value:
                    {
                        //научить запоминать параметры шифрования файлов, если научу их дешифровать
                        //"hasEncryptedImages":true,"hasEncryptedAudio":true,"encryptionKey":"d41d8cd98f00b204e9800998ecf8427e"}

                        if (value.Type != JTokenType.String)
                        {
                            return;
                        }

                        string tokenValue = jsonToken + "";

                        // commented because replaced by other way to parse merged messages
                        //if (IsWithMergedMessages && IsMessageCode(CurrentEventCode))
                        //{
                        //    if (MessageMerged.ToString().Length > 0)
                        //    {
                        //        MessageMerged.Append('\n');//add \n between lines in merged message
                        //    }
                        //    //LogToFile("code 401 adding value to merge=" + tokenvalue + ", currentEventCode=" + currentEventCode);
                        //    MessageMerged.Append(tokenValue);
                        //    AddToStats();
                        //}
                        //else
                        {
                            // commented because replaced by other way to parse merged messages
                            //if (IsWithMergedMessages)
                            //{
                            //    AddMergedMessage(jsonToken, jsonName);
                            //}

                            if (!IsValidString(value, tokenValue))
                            {
                                AddToStats(false);
                                return;
                            }

                            AddToStats();

                            bool HasCurCode = CurrentEventCode > -1;
                            AddRowData(jsonName, tokenValue, "JsonPath: "
                                + Environment.NewLine
                                + jsonToken.Path
                                + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                                IsWithMergedMessages && HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                                //+ (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                                , true, false);
                        }

                        break;
                    }

                case JObject jsonObject:
                    {
                        if (AddedJObjects.Contains(jsonObject))
                        {
                            return;
                        }

                        //LogToFile("JObject Properties: \r\n" + obj.Properties());
                        //JToken lastProperty = null; // commented because replaced by other way to parse merged messages
                        foreach (var property in jsonObject.Properties())
                        {
                            //lastProperty = property; // commented because replaced by other way to parse merged messages
                            propertyName = property.Name;

                            if (IsWithMergedMessages)//asdfg skip code 108,408,356
                            {
                                bool IsCode = IsInteger(property.Value.Type) && propertyName == "code";
                                if (IsCode)
                                {
                                    CurrentEventCode = (int)property.Value;
                                }

                                if (IsMessageCode(CurrentEventCode)) // идея при записи брать сразу всё сообщение, брать перевод для него и переводить, потом пропускать объекты с частями переведенного сообщения
                                {
                                    var messageparts = GetNextTokensWithSameCode(jsonObject);
                                    var fullmessage = GetMessageLinesFrom(messageparts);

                                    bool HasCurCode = true; // message code parse
                                    AddRowData(tablename: jsonName, RowData: fullmessage, RowInfo: "JsonPath: "
                                        + Environment.NewLine
                                        + jsonToken.Path
                                        + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                                        IsWithMergedMessages && HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                                        //+ (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                                        , CheckAddHashes: true, CheckInput: true);

                                    AddToStats();

                                    //TranslateMessages(messageparts, fullmessage);
                                    break;
                                }
                                else
                                if (IsExcludedOrParsed(jsonObject, CurrentEventCode, jsonName))
                                {
                                    break; // skip all rest properties parse
                                }

                                // replaced by IsExcludedCode above
                                //if (skipIt)
                                //{
                                //    if (IsExcludedCode(CurrentEventCode))
                                //    {
                                //        if (!IsCode && propertyName == "parameters")//asdf
                                //        {
                                //            skipIt = false;
                                //            continue;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        skipIt = false;
                                //    }
                                //}
                                //else
                                //{
                                //    if (IsCode)
                                //    {
                                //        //string propertyValue = property.Value + string.Empty;
                                //        if (IsExcludedCode(CurrentEventCode))
                                //        {
                                //            skipIt = true;
                                //            continue;
                                //        }
                                //    }
                                //}
                            }
                            ParseJToken(property.Value, jsonName/*, property.Name*/);
                        }

                        // commented because replaced by other way to parse merged messages
                        // добавление объединенных сообщений для события текстового сообщения 401 ил 405, если новых строк больше нет
                        //if (IsMessageCode(CurrentEventCode) && MessageMerged.Length > 0)
                        //{
                        //    if (IsNextTokenNotWithSameCode(jsonToken))
                        //    {
                        //        AddMergedMessage(lastProperty, jsonName, true, false);
                        //    }
                        //}

                        ResetCurrentCode();
                        break;
                    }

                case JArray jsonArray:
                    {
                        int arrayCount = jsonArray.Count;
                        for (int i = 0; i < arrayCount; i++)
                        {
                            ParseJToken(jsonArray[i], jsonName);
                        }

                        break;

                        //заменил записью выше
                        //если последний массив properties был пустой, то добавить записанное сообщение
                        //if (IsWithMergedMessages
                        //    && propertyName == "parameters"
                        //    && arrayCount == 0 /**/
                        //    &&
                        //    MessageMerged.Length > 0) //добавить объединенное сообщение, если оно не пустое
                        //{
                        //    AddMergedMessage(jsonToken, JsonName, true, false);
                        //}
                    }

                default:
                    break;
            }
        }

        /// <summary>
        /// Translate originalMergedMessage
        /// </summary>
        /// <param name="originalMessageJTokensList"></param>
        /// <param name="originalMergedMessage"></param>
        private void TranslateMessages(List<JToken> originalMessageJTokensList, string originalMergedMessage)
        {
            if (!IsValidString(originalMergedMessage))
            {
                return;
            }

            if (!ProjectData.TablesLinesDict.ContainsKey(originalMergedMessage))
            {
                return;
            }

            var translated = ProjectData.TablesLinesDict[originalMergedMessage].SplitToLines().ToArray();

            var origLength = originalMessageJTokensList.Count;
            JToken last = null;
            for (int i = 0; i < translated.Length; i++)
            {
                if (i < origLength)
                {
                    JObject obj = originalMessageJTokensList[i] as JObject;
                    JProperty prop = obj.Last as JProperty;
                    JArray array = prop.Value as JArray;
                    array[0] = translated[i];

                    AddedJObjects.AddTry(obj);
                    last = obj;
                }
                else
                {
                    var newJObject = GetNewJObject(last as JObject, translated[i]);
                    last.AddAfterSelf(newJObject);
                    AddedJObjects.AddTry(newJObject);

                    last = last.Next;
                }
            }

        }

        /// <summary>
        /// get all string lines of message from input <paramref name="messagePartsList"/>
        /// </summary>
        /// <param name="messagePartsList"></param>
        /// <returns></returns>
        private static string GetMessageLinesFrom(List<JToken> messagePartsList)
        {
            List<string> lines = new List<string>();
            foreach (JToken token in messagePartsList)
            {
                JProperty prop = (token as JObject).Last as JProperty;
                JArray array = prop.Value as JArray;
                foreach (string value in array)
                {
                    lines.Add(value);
                }
            }

            return string.Join("\n", lines);
        }

        /// <summary>
        /// Add all next JTokens with same message code to list
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <returns></returns>
        private List<JToken> GetNextTokensWithSameCode(JToken jsonToken)
        {
            List<JToken> list = new List<JToken>
            {
                jsonToken
            };

            var next = jsonToken.Next;
            while (next is JObject obj
                && obj.First is JProperty prop
                && prop.Name == "code"
                && (int)prop.Value == CurrentEventCode
                )
            {
                AddedJObjects.AddTry(obj);
                list.Add(next);
                next = next.Next;
            }

            return list;
        }

        /// <summary>
        /// Checks if next token is exists and it with same event code
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <returns></returns>
        private bool IsNextTokenNotWithSameCode(JToken jsonToken)
        {
            return jsonToken.Next == null // next JProperty is not exists
                                || !(jsonToken.Next is JObject obj) // or next property is not a JObject
                                || !(obj.First is JProperty prop) // or next JObject's first element is not JProperty
                                || prop.Name != "code" // or first JProperty has not "code" name
                                || (int)prop.Value != CurrentEventCode // or the JProperty's code not equal current
                                ;
        }

        private bool IsCodeWithStringInParameters(int currentEventCode)
        {
            return //IsMessageCode(currentEventCode) || 
                currentEventCode == 102
                || currentEventCode == 402
                || currentEventCode == 118
                || currentEventCode == 119;
        }

        /// <summary>
        /// for statistics and json open\save improvement purpose.
        /// </summary>
        /// <param name="add">for added strings else for skipped</param>
        private void AddToStats(bool add = true)
        {
            var dict = add ? ProjectData.RpgMVAddedCodesStat : ProjectData.RpgMVSkippedCodesStat;
            if (CurrentEventCode > -1)
            {
                if (!dict.ContainsKey(CurrentEventCode))
                {
                    dict.Add(CurrentEventCode, 1);
                }
                else
                {
                    dict[CurrentEventCode]++;
                }
            }
        }

        private void ParseJTokenNew(JToken jsonToken, string jsonName)
        {
            if (jsonToken == null)
            {
                return;
            }

            if (jsonToken is JValue)
            {
                if (jsonToken.Type != JTokenType.String)
                {
                    return;
                }
            }
            else if (jsonToken is JObject jsonObject)
            {
                foreach (var property in jsonObject.Properties())
                {
                    bool IsCode = IsInteger(property.Value.Type) && property.Name == "code";
                    if (IsCode)
                    {
                        CurrentEventCode = (int)property.Value;

                        if (IsMessageCode(CurrentEventCode))
                        {
                            ParseJTokenNew(jsonObject.Last, jsonName);
                        }
                    }

                    ParseJTokenNew(property.Value, jsonName);
                }
            }
            else if (jsonToken is JArray jsonArray)
            {
                int arrayCount = jsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ParseJTokenNew(jsonArray[i], jsonName);
                }
            }
        }

        /// <summary>
        /// true if type is JTokenType.Integer
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsInteger(JTokenType type)
        {
            return type == JTokenType.Integer;
        }

        /// <summary>
        /// reset current code if it is was exist
        /// </summary>
        private void ResetCurrentCode()
        {
            if (CurrentEventCode != -1)
            {
                CurrentEventCode = -1;
            }
        }

        /// <summary>
        /// Save merged message
        /// </summary>
        /// <param name="jsonToken">Token where get code</param>
        /// <param name="jsonName">Json name</param>
        /// <param name="checkPreviousToken">Check previous token when get code info</param>
        /// <param name="isValue">True when the method called from JValue section</param>
        private void AddMergedMessage(JToken jsonToken, string jsonName, bool checkPreviousToken = true, bool isValue = true)
        {
            string mergedstring;
            if (string.IsNullOrWhiteSpace(mergedstring = MessageMerged.ToString()))
            {
                return;
            }

            MessageMerged.Clear();

            if (!IsValidString(mergedstring))
            {
                return;
            }

            bool HasCurCode = CurrentEventCode > -1;
            AddData(jsonName, mergedstring, "JsonPath: "
                + Environment.NewLine
                + jsonToken.Path
                + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                (IsWithMergedMessages && HasCurCode) ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                //+ (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                );
        }

        /// <summary>
        /// returns event name if found
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private string GetCodeName(int currentEventCode)
        {
            if (EventCodes.ContainsKey(currentEventCode))
            {
                var eventName = EventCodes[currentEventCode];
                if (eventName.Length > 0)
                {
                    return "\r\nEvent=\"" + eventName + "\"";
                }
            }

            return string.Empty;
        }

        //obsolete
        //currentEventCode must be set and reseted correctly
        ///// <summary>
        ///// get jobject with code jproperty
        ///// </summary>
        ///// <param name="jsonToken"></param>
        ///// <param name="previous">true means get previous jobject before this</param>
        ///// <returns></returns>
        //private static string GetCodeValueOfParent(JToken jsonToken, bool previous = false, bool IsValue = true)
        //{
        //    if (previous)
        //    {
        //        if (jsonToken.Parent != null)
        //        {
        //            if (jsonToken.Parent.Parent != null)
        //            {
        //                if (IsValue)
        //                {
        //                    if (jsonToken.Parent.Parent.Parent != null && jsonToken.Parent.Parent.Parent is JObject obj)
        //                    {
        //                        if (obj.Previous != null)
        //                        {
        //                            if (obj.Previous is JObject obj1)
        //                            {
        //                                if (obj1.ContainsKey("code"))
        //                                {
        //                                    return obj1.Value<long>("code") + string.Empty;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (jsonToken.Parent.Parent is JObject obj)
        //                {
        //                    if (obj.Previous != null)
        //                    {
        //                        if (obj.Previous is JObject obj1)
        //                        {
        //                            if (obj1.ContainsKey("code"))
        //                            {
        //                                return obj1.Value<long>("code") + string.Empty;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (jsonToken.Parent != null)
        //    {
        //        if (jsonToken.Parent.Parent != null)
        //        {
        //            if (IsValue)
        //            {
        //                if (jsonToken.Parent.Parent.Parent != null)
        //                {
        //                    if (jsonToken.Parent.Parent.Parent is JObject obj)
        //                    {
        //                        if (obj.ContainsKey("code"))
        //                        {
        //                            return obj.Value<long>("code") + string.Empty;
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (jsonToken.Parent.Parent.Parent is JObject obj)
        //                {
        //                    if (obj.ContainsKey("code"))
        //                    {
        //                        return obj.Value<long>("code") + string.Empty;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return string.Empty;
        //}

        /// <summary>
        /// String for this codes must not be appeared
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private static bool IsExcludedCode(int currentEventCode)
        {
            return ExcludedCodes.ContainsKey(currentEventCode); /*currentEventCode.Length == 3 && (currentEventCode == 108 || currentEventCode == 408 || currentEventCode == 356)*/;
        }

        /// <summary>
        /// Multiline message
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        private static bool IsMessageCode(int currentEventCode)
        {
            return (currentEventCode == 401 || currentEventCode == 405);
        }

        //List<string> TempList;
        //List<string> TempListInfo;

        private void AddData(string jsonName, string value, string info)
        {
            AddRowData(jsonName, value, info, true);
            //if (hashes.Contains(Value))
            //    return;

            //ProjectData.THFilesElementsDataset.Tables[jsonName].Rows.Add(Value);
            //TempList.Add(Value);//много быстрее
            //hashes.Add(Value);

            //ProjectData.THFilesElementsDatasetInfo.Tables[jsonName].Rows.Add(Info);
            //TempListInfo.Add(Info);//много быстрее
        }

        bool skipIt;

        internal override bool Save()
        {
            return WriteJson(Path.GetFileNameWithoutExtension(ProjectData.FilePath), ProjectData.FilePath);
        }

        private bool WriteJson(string jsonName, string sPath)
        {
            if (ProjectData.THFilesElementsDataset.Tables.Contains(jsonName) && FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[jsonName]))
            {
                return false;
            }

            ProjectData.Main.ProgressInfo(true, T._("Writing") + ": " + jsonName + ".json");
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                using (StreamReader reader = new StreamReader(sPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, jsonName);
                    }
                }

                StartingRow = 0;//сброс начальной строки поиска в табице
                IsWithMergedMessages = IsContainsLinedMessages(jsonName);//разбитые на строки сообщения есть и в других файлах, вроде maps,troops, может еще где
                if (IsWithMergedMessages)
                {
                    //сброс значений для CommonEvents
                    //currentEventCode = string.Empty; // commented, see ResetCurrentCode()
                    propertyName = string.Empty;
                    skipIt = false;

                    //только в CommonEvents была сборка строк в сообщение
                    //SplitTableCellValuesAndTheirLinesToDictionary(jsonName, false, false);
                }
                else
                {
                    //SplitTableCellValuesToDictionaryLines(jsonName);
                }
                SplitTableCellValuesAndTheirLinesToDictionary(jsonName, false, false);

                AddedJObjects = new HashSet<JObject>();
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
                {
                    //GetTranslatableRows(jsonName);
                    ParseJTokenWriteWithPreSplitlines(root, jsonName);
                }
                else
                {
                    if (!ProjectData.THFilesElementsDataset.Tables.Contains(jsonName))
                    {
                        return false;
                    }
                    ParseJTokenWrite(root, jsonName);
                }

                File.WriteAllText(sPath, CorrectJsonFormatToRPGMMV(root));

            }
            catch
            {
                return false;
            }

            return true;
        }

        ///// <summary>
        ///// list of identifiers if row have translation
        ///// </summary>
        //Dictionary<string, bool> IsTranslatableRow;
        ///// <summary>
        ///// IsTranslatableRow[original] will be true if original have translation
        ///// tried to add check if row have translation but here also issue because no displayed rows will be translated
        ///// </summary>
        ///// <param name="jsonName"></param>
        //private void GetTranslatableRows(string jsonName)
        //{
        //    //IsTranslatableRow = new Dictionary<string, bool>();
        //    //foreach (System.Data.DataRow row in ProjectData.THFilesElementsDataset.Tables[jsonName].Rows)
        //    //{
        //    //    IsTranslatableRow.Add(row[0] as string,
        //    //        !(row[1] == null || string.IsNullOrEmpty(row[1] + "") || Equals(row[1], row[0]))
        //    //        );
        //    //}
        //}

        private static bool IsContainsLinedMessages(string jsonName)
        {
            return true;
            //return jsonName == "CommonEvents";
            //return jsonName == "CommonEvents" || jsonName.StartsWith("Map") || jsonName == "Troops";
        }

        /// <summary>
        /// Корректировка формата записываемого json так, как в файлах RPGMaker MV
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static string CorrectJsonFormatToRPGMMV(JToken root)
        {
            return root.ToString(Formatting.Indented);
            //return Regex.Replace(root.ToString(Formatting.None), @"^\[null,(.+)\]$", "[\r\nnull,\r\n$1\r\n]");//regex нужен только для Formatting.None
        }

        int StartingRow;//оптимизация. начальная строка, когда идет поиск по файлу, чтобы не искало каждый раз сначала при нахождении перевода будет переприсваиваться начальная строка на последнюю
        private void ParseJTokenWrite(JToken jsonToken, string jsonName/*, string propertyname = ""*/)
        {
            if (jsonToken == null)
            {
                return;
            }

            switch (jsonToken)
            {
                case JValue value:
                    {
                        //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                        if (value.Type != JTokenType.String)
                        {
                            return;
                        }

                        string tokenValue = jsonToken + string.Empty;

                        if (!IsValidString(value, tokenValue))
                        {
                            return;
                        }

                        string parameter0Value = tokenValue;

                        //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

                        int rcount = ProjectData.THFilesElementsDataset.Tables[jsonName].Rows.Count;
                        for (int r = StartingRow; r < rcount; r++)
                        {
                            var row = ProjectData.THFilesElementsDataset.Tables[jsonName].Rows[r];
                            if ((row[1] + string.Empty).Length == 0)
                            {
                                continue;
                            }

                            string[] origA = FunctionsString.GetAllNonEmptyLinesToArray(row[0] + string.Empty);//Все строки, кроме пустых, чтобы потом исключить из проверки
                            int origALength = origA.Length;
                            if (origALength == 0)
                            {
                                origA = new string[1];
                                origA[0] = row[0] + string.Empty;
                                //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[jsonName].Rows[i1][0].ToString());
                            }

                            if (origALength > 0)
                            {
                                string[] transA = FunctionsString.GetAllNonEmptyLinesToArray(row[1] + string.Empty);

                                if (transA.Length == 0)
                                {
                                    transA = new string[1];
                                    transA[0] = row[1] + string.Empty;
                                    //LogToFile("(transA.Length == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[jsonName].Rows[i1][1].ToString());
                                }
                                string transMerged = string.Empty;
                                if (transA.Length == origALength)//если количество строк в оригинале и переводе равно
                                {
                                    //ничего не делать
                                }
                                else // если перевод вдруг был переведен так, что не равен количеством строк оригиналу, тогда поделить его на равные строки
                                {
                                    //if (transA.Length > 0) // но перед этим, если перевод больше одной строки
                                    {
                                        foreach (string ts in transA)
                                        {
                                            transMerged += ts; // объединить все строки в одну
                                        }
                                    }

                                    transA = FunctionsString.SplitStringByEqualParts(transMerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
                                }

                                //Подстраховочная проверка для некоторых значений из нескольких строк, полное сравнение перед построчной
                                if (tokenValue == row[0] + string.Empty)
                                {
                                    (jsonToken as JValue).Value = (row[1] + string.Empty).Replace("\r", string.Empty);//убирает \r, т.к. в json присутствует только \n
                                    StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                    break;
                                }

                                bool translationWasSet = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
                                for (int i2 = 0; i2 < origALength; i2++)
                                {
                                    if (parameter0Value == origA[i2]/*.Replace("\r", string.Empty)*/) //Replace здесь убирает \r из за которой строки считались неравными
                                    {
                                        //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));

                                        value.Value = transA[i2]/*.Replace("\r", string.Empty)*/; //Replace убирает \r

                                        StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                                        //LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
                                        translationWasSet = true;
                                        break;
                                    }
                                }
                                if (translationWasSet) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
                                {
                                    StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                    break;
                                }
                            }
                            else
                            {
                                if (tokenValue == row[0] + string.Empty)
                                {
                                    value.Value = row[1] + string.Empty;
                                    StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                    break;
                                }
                            }
                        }

                        break;
                    }

                case JObject jsonObject:
                    {
                        //LogToFile("JObject Properties: \r\n" + obj.Properties());
                        foreach (var property in jsonObject.Properties())
                        {
                            propertyName = property.Name;
                            if (IsWithMergedMessages)//asdfg skip code 108,408,356
                            {
                                if (IsInteger(property.Value.Type) && propertyName == "code")
                                {
                                    CurrentEventCode = (int)property.Value;
                                    //cCode = "Code" + currentEventCode;
                                    //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                                }

                                if (IsExcludedOrParsed(jsonObject, CurrentEventCode, jsonName))
                                {
                                    break;
                                }

                                // replaced by IsExcludedCode above
                                //if (skipIt)
                                //{
                                //    if (IsExcludedCode(CurrentEventCode))
                                //    {
                                //        if (property.Name == "parameters")//asdf
                                //        {
                                //            skipIt = false;
                                //            continue;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        skipIt = false;
                                //    }
                                //}
                                //else
                                //{
                                //    if (IsInteger(property.Value.Type) && propertyName == "code")
                                //    {
                                //        if (IsExcludedCode((int)property.Value))
                                //        {
                                //            skipIt = true;
                                //            continue;
                                //        }
                                //    }
                                //}
                            }
                            ParseJTokenWrite(property.Value, jsonName/*, property.Name*/);
                        }

                        ResetCurrentCode();
                        break;
                    }

                case JArray jsonArray:
                    {
                        int arrayCount = jsonArray.Count;
                        for (int i = 0; i < arrayCount; i++)
                        {
                            ParseJTokenWrite(jsonArray[i], jsonName);
                        }

                        break;
                    }

                default:
                    break;
            }
        }

        bool IsValidString(JToken token, string value)
        {
            string path;
            return IsValidString(value)
                && !((path = token.Path).Contains("].name") && Regex.IsMatch(path, @"events\[[0-9]+\]\.name")) // skip event names
                && !(path.Contains("].image") && Regex.IsMatch(path, @"\[[0-9]+\]\.image")) // skip image names
                && !(path.Contains("gm.name") && Regex.IsMatch(token.Path, @"[Bb]gm\.name")); // skip bgm
        }

        protected override bool IsValidString(string inputString)
        {
            int commentIndex = inputString.IndexOf("//");
            var tokenvalueNoComment = commentIndex > -1 ? inputString.Substring(0, commentIndex) : inputString;
            return base.IsValidString(tokenvalueNoComment);
        }

        /// <summary>
        /// already parsed objects of message code 401 or 405 which was parsed
        /// </summary>
        HashSet<JObject> AddedJObjects = new HashSet<JObject>();
        private void ParseJTokenWriteWithPreSplitlines(JToken jsonToken, string jsonName/*, string propertyname = ""*/)
        {
            if (jsonToken == null)
            {
                return;
            }

            switch (jsonToken)
            {
                case JValue value:
                    {
                        //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                        if (value.Type != JTokenType.String)
                        {
                            return;
                        }

                        string tokenValue = value + string.Empty;

                        if (!IsValidString(value, tokenValue))
                        {
                            return;
                        }

                        //предполагается, что ячейки из таблицы были разбиты на строки и добавлены в словарь
                        if (/*IsTranslatableRow.ContainsKey(tokenvalue) && IsTranslatableRow[tokenvalue] tried to add check if row have translation but here also issue because no displaed rows will be translated &&*/
                            !TablesLinesDict.ContainsKey(tokenValue)
                            || TablesLinesDict[tokenValue].Length == 0
                            )
                        {
                            return;
                        }

                        WriteTranslation(jsonToken, value, tokenValue);

                        break;
                    }

                case JObject jsonObject:
                    {
                        //skip new added json object
                        if (AddedJObjects.Contains(jsonObject))
                        {
                            return;
                        }

                        //LogToFile("JObject Properties: \r\n" + obj.Properties());
                        foreach (var property in jsonObject.Properties())
                        {
                            propertyName = property.Name;
                            if (IsWithMergedMessages)//asdfg skip code 108,408,356
                            {
                                var isCode = IsInteger(property.Value.Type) && propertyName == "code";
                                if (isCode)
                                {
                                    CurrentEventCode = (int)property.Value;
                                    //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                                }

                                if (IsMessageCode(CurrentEventCode)) // идея при записи брать сразу всё сообщение, брать перевод для него и переводить, потом пропускать объекты с частями переведенного сообщения
                                {
                                    var messageparts = GetNextTokensWithSameCode(jsonObject);
                                    var fullmessage = GetMessageLinesFrom(messageparts);

                                    //bool HasCurCode = true; // message code parse
                                    //AddRowData(tablename: jsonName, RowData: fullmessage, RowInfo: "JsonPath: "
                                    //    + Environment.NewLine
                                    //    + jsonToken.Path
                                    //    + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                                    //    IsWithMergedMessages && HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                                    //    //+ (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                                    //    , CheckAddHashes: true, CheckInput: false);

                                    TranslateMessages(messageparts, fullmessage);
                                    break;
                                }
                                else
                                if (IsExcludedOrParsed(jsonObject, CurrentEventCode, jsonName))
                                {
                                    break;
                                }

                                // replaced by IsExcludedCode above
                                //if (skipIt)
                                //{
                                //    if (IsExcludedCode(CurrentEventCode))
                                //    {
                                //        if (!isCode && propertyName == "parameters")//asdf
                                //        {
                                //            skipIt = false;
                                //            continue;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        skipIt = false;
                                //    }
                                //}
                                //else
                                //{
                                //    if (isCode)
                                //    {
                                //        //string propertyValue = property.Value + string.Empty;
                                //        if (IsExcludedCode(CurrentEventCode))
                                //        {
                                //            skipIt = true;
                                //            continue;
                                //        }
                                //    }
                                //}
                            }
                            ParseJTokenWriteWithPreSplitlines(property.Value, jsonName/*, property.Name*/);
                        }

                        ResetCurrentCode();
                        break;
                    }

                case JArray jsonArray:
                    {
                        for (int i = 0; i < jsonArray.Count/*(здесь должно быть без предварительного зпоминания количества, т.к. массив меняется)*/
                            ; i++)
                        {
                            try
                            {
                                ParseJTokenWriteWithPreSplitlines(jsonArray[i], jsonName);
                            }
                            catch
                            {
                            }
                        }

                        break;
                    }

                default:
                    break;
            }
        }

        private void WriteTranslation(JToken jsonToken, JValue value, string tokenValue)
        {
            try
            {
                int tokenvalueLinesCount;
                if (TablesLinesDict[tokenValue].GetLinesCount() == (tokenvalueLinesCount = tokenValue.GetLinesCount())
                    || jsonToken.Parent == null
                    || jsonToken.Parent.Parent == null
                    || jsonToken.Parent.Parent.Parent == null
                    || !(jsonToken.Parent.Parent.Parent is JObject currentJObject)

                    )
                {
                    value.Value = TablesLinesDict[tokenValue];
                }
                else
                {
                    int ind = 0;
                    List<string> stringToWrite = new List<string>();
                    bool NotWrited = true;
                    bool IsTheJObjetAdded = false;

                    foreach (var line in TablesLinesDict[tokenValue].SplitToLines())
                    {
                        if (ind < tokenvalueLinesCount)
                        {
                            stringToWrite.Add(line);
                            ind++;
                        }
                        else
                        {
                            if (NotWrited)
                            {
                                value.Value = string.Join(Environment.NewLine, stringToWrite);
                                NotWrited = false;
                            }

                            var newJObject = GetNewJObject(currentJObject, line);
                            currentJObject.AddAfterSelf(newJObject);
                            if (!IsTheJObjetAdded && !AddedJObjects.Contains(newJObject))
                            {
                                IsTheJObjetAdded = true;
                                AddedJObjects.Add(newJObject);
                            }
                            currentJObject = newJObject;//делать добавленный JObject текущим, чтобы новый добавлялся после него
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private bool IsExcludedOrParsed(JObject jsonObject, int currentEventCode, string jsonName)
        {
            if (IsExcludedCode(currentEventCode)) // code always located in the objects and dont need do as code below
            {
                AddToStats(false);
                return true;
            }
            else
            if (IsCodeWithStringInParameters(currentEventCode) && jsonObject.Last is JProperty lastObjectsProperty) // in message codes need only last property's "parameters" value
            {
                ParseJToken(lastObjectsProperty.Value, jsonName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// copy all objects from input jContainer in new JObject and replace parameters value to new
        /// </summary>
        /// <param name="jContainer">input JContainer</param>
        /// <param name="line">line to which replace parameters value</param>
        /// <returns></returns>
        private static JObject GetNewJObject(JContainer jContainer, string line)
        {
            JObject newJObject = new JObject();
            foreach (var jProperty in (jContainer as JObject).Properties())
            {
                switch (jProperty.Name)
                {
                    case "parameters":
                        if (jProperty.Value is JValue)
                        {
                            newJObject.Add(new JProperty(jProperty.Name, new JValue(line)));
                        }
                        else if (jProperty.Value is JObject)
                        {
                            newJObject.Add(new JProperty(jProperty.Name, new JObject("{" + line + "}")));
                        }
                        else if (jProperty.Value is JArray)
                        {
                            newJObject.Add(new JProperty(jProperty.Name, new JArray(new object[] { line })));
                        }
                        break;
                    default:
                        {
                            if (jProperty.Value is JValue jValue)
                            {
                                newJObject.Add(new JProperty(jProperty.Name, new JValue(jValue)));
                            }
                            else if (jProperty.Value is JObject jObject)
                            {
                                newJObject.Add(new JProperty(jProperty.Name, new JObject(jObject)));
                            }
                            else if (jProperty.Value is JArray jArray)
                            {
                                newJObject.Add(new JProperty(jProperty.Name, new JArray(jArray)));
                            }

                            break;
                        }
                }
            }

            return newJObject;
        }
    }
}
