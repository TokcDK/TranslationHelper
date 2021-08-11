using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        private bool ReadJson(string Jsonname, string sPath)
        {
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
                AddTables(Jsonname);

                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(ProjectData.SelectedDir);
                //using (var reader = new StreamReader(ProjectData.SelectedDir+"\\"+ Jsonname+".json"))
                using (StreamReader reader = new StreamReader(sPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, Jsonname);
                    }
                }

                //ds.Tables.Add(Jsonname); // create table with json name
                //ds.Tables[Jsonname].Columns.Add("Original"); //create Original column


                //treeView1.BeginUpdate();
                // treeView1.Nodes.Clear();
                //var tNode = treeView1.Nodes[treeView1.Nodes.Add(new TreeNode(rootName))];
                //tNode.Tag = root;

                //Stopwatch timer = new Stopwatch();

                //timer.Start();

                //TempList = new List<string>();
                //TempListInfo = new List<string>();
                IsWithMergedMessages = IsContainsLinedMessages(Jsonname);//отказалось, что сообщения есть не только в CommonEvents, но и в maps,troops и возможно других файлах

                MessageMerged = new StringBuilder();

                ParseJToken(root, Jsonname);
                //ProceedJTokenAny(root, Jsonname);

                MessageMerged = null;
                //занесение в список
                //int TempListCount = TempList.Count;
                //for (int i = 0; i < TempListCount; i++)
                //{
                //    THFilesElementsDataset.Tables[Jsonname].Rows.Add(TempList[i]);
                //    THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(TempListInfo[i]);
                //}

                //TempList = null;
                //TempListInfo = null;

                //timer.Stop();
                //TimeSpan difference = new TimeSpan(timer.ElapsedTicks);
                //FileWriter.WriteData(Path.Combine(Application.StartupPath, "Timers 4 Table.log"), Jsonname + ": " + difference + Environment.NewLine);
                //MessageBox.Show(difference.ToString());

                //treeView1.ExpandAll();
            }
            catch (JsonReaderException ex)
            {
                new FunctionsLogs().LogToFile("Error occured while json read (json is empty or corrupted): \r\n" + ex);
            }
            catch (Exception ex)
            {
                new FunctionsLogs().LogToFile("Error occured while json parse: \r\n" + ex);
                //LogToFile(string.Empty, true);
            }
            finally
            {
                //LogToFile(string.Empty, true);
                //MessageBox.Show("sss");
                //ds.Tables[Jsonname].Columns.Add("Translation");
                //ds.Tables[Jsonname].Columns["Original"].ReadOnly = true;
                //DGV.DataSource = ds.Tables[0];
                //treeView1.EndUpdate();
            }
            //LogToFile(string.Empty, true);

            return CheckTablesContent(Jsonname);
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

            if (jsonToken is JValue)
            {
                //научить запоминать параметры шифрования файлов, если научу их дешифровать
                //"hasEncryptedImages":true,"hasEncryptedAudio":true,"encryptionKey":"d41d8cd98f00b204e9800998ecf8427e"}

                if (jsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenvalue = jsonToken.ToString();

                if (IsWithMergedMessages && IsMessageCode(CurrentEventCode))
                {
                    if (MessageMerged.ToString().Length > 0)
                    {
                        MessageMerged.Append('\n');//add \n between lines in merged message
                    }
                    //LogToFile("code 401 adding value to merge=" + tokenvalue + ", curcode=" + curcode);
                    MessageMerged.Append(tokenvalue);
                    AddToStats();
                }
                else
                {
                    if (IsWithMergedMessages)
                    {
                        AddMergedMessage(jsonToken, jsonName);
                    }

                    int commentIndex = tokenvalue.IndexOf("//");
                    var tokenvalueNoComment = commentIndex > -1 ? tokenvalue.Substring(0, commentIndex) : tokenvalue;
                    if (IsValidString(tokenvalueNoComment))
                    {
                        AddToStats();

                        bool HasCurCode = CurrentEventCode > -1;
                        AddRowData(jsonName, tokenvalue, "JsonPath: "
                            + Environment.NewLine
                            + jsonToken.Path
                            + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                            (IsWithMergedMessages && HasCurCode) ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                            + (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                            , true, false);
                    }
                    else
                    {
                        AddToStats(false);
                    }
                }
            }
            else if (jsonToken is JObject jsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                JToken lastProperty = null;
                foreach (var property in jsonObject.Properties())
                {
                    lastProperty = property;
                    propertyName = property.Name;

                    if (IsWithMergedMessages)//asdfg skip code 108,408,356
                    {
                        bool IsCode = IsInteger(property.Value.Type) && propertyName == "code";
                        if (IsCode)
                        {
                            CurrentEventCode = (int)property.Value;
                        }
                        if (IsExcludedCode(CurrentEventCode)) // code always located in the objects and dont need do as code below
                        {
                            AddToStats(false);
                            continue;
                        }
                        else
                        if (IsCodeWithStringInParameters(CurrentEventCode) && jsonObject.Last is JProperty lastObjectsProperty) // in message codes need only last property's "parameters" value
                        {
                            ParseJToken(lastObjectsProperty.Value, jsonName);
                            continue;
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

                // добавление объединенных сообщений для события текстового сообщения 401 ил 405, если новых строк больше нет
                if (IsMessageCode(CurrentEventCode) && MessageMerged.Length > 0)
                {
                    if (jsonToken.Next == null // next JProperty is not exists
                        || !(jsonToken.Next is JObject obj) // or next property is not a JObject
                        || !(obj.First is JProperty prop) // or next JObject's first element is not JProperty
                        || prop.Name != "code" // or first JProperty has not "code" name
                        || (int)prop.Value != CurrentEventCode // or the JProperty's code not equal current
                        )
                    {
                        AddMergedMessage(lastProperty, jsonName, true, false);
                    }
                }

                ResetCurrentCode();
            }
            else if (jsonToken is JArray jsonArray)
            {
                int arrayCount = jsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ParseJToken(jsonArray[i], jsonName);
                }

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
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        private bool IsCodeWithStringInParameters(int currentEventCode)
        {
            return IsMessageCode(currentEventCode)
                || currentEventCode == 102
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
        /// <param name="JsonToken">Token where get code</param>
        /// <param name="JsonName">Json name</param>
        /// <param name="CheckPreviousToken">Check previous token when get code info</param>
        /// <param name="IsValue">True when the method called from JValue section</param>
        private void AddMergedMessage(JToken JsonToken, string JsonName, bool CheckPreviousToken = true, bool IsValue = true)
        {
            string mergedstring;
            if (string.IsNullOrWhiteSpace(mergedstring = MessageMerged.ToString()))
            {
                return;
            }

            MessageMerged.Clear();

            if (mergedstring.ForJPLangHaveMostOfRomajiOtherChars())
            {
                return;
            }

            bool HasCurCode = CurrentEventCode > -1;
            AddData(JsonName, mergedstring, "JsonPath: "
                + Environment.NewLine
                + JsonToken.Path
                + (HasCurCode ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) :
                (IsWithMergedMessages && HasCurCode) ? Environment.NewLine + "Code=" + CurrentEventCode + GetCodeName(CurrentEventCode) : string.Empty)
                + (HasCurCode && (CurrentEventCode == 402 || CurrentEventCode == 102) ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                );
        }

        /// <summary>
        /// returns event name if found
        /// </summary>
        /// <param name="curcode"></param>
        /// <returns></returns>
        private string GetCodeName(int curcode)
        {
            if (EventCodes.ContainsKey(curcode))
            {
                var EventName = EventCodes[curcode];
                if (EventName.Length > 0)
                {
                    return "\r\nEvent=\"" + EventName + "\"";
                }
            }

            return string.Empty;
        }

        //obsolete
        //curcode must be set and reseted correctly
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
        /// <param name="curcode"></param>
        /// <returns></returns>
        private static bool IsExcludedCode(int curcode)
        {
            return ExcludedCodes.ContainsKey(curcode); /*curcode.Length == 3 && (curcode == 108 || curcode == 408 || curcode == 356)*/;
        }

        /// <summary>
        /// Multiline message
        /// </summary>
        /// <param name="curcode"></param>
        /// <returns></returns>
        private static bool IsMessageCode(int curcode)
        {
            return (curcode == 401 || curcode == 405);
        }

        //List<string> TempList;
        //List<string> TempListInfo;

        private void AddData(string Jsonname, string Value, string Info)
        {
            AddRowData(Jsonname, Value, Info, true);
            //if (hashes.Contains(Value))
            //    return;

            //ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows.Add(Value);
            //TempList.Add(Value);//много быстрее
            //hashes.Add(Value);

            //ProjectData.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(Info);
            //TempListInfo.Add(Info);//много быстрее
        }

        bool skipIt;

        internal override bool Save()
        {
            return WriteJson(Path.GetFileNameWithoutExtension(ProjectData.FilePath), ProjectData.FilePath);
        }

        private bool WriteJson(string Jsonname, string sPath)
        {
            if (ProjectData.THFilesElementsDataset.Tables.Contains(Jsonname) && FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[Jsonname]))
            {
                return false;
            }

            ProjectData.Main.ProgressInfo(true, T._("Writing") + ": " + Jsonname + ".json");
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                using (StreamReader reader = new StreamReader(sPath))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        root = JToken.Load(jsonReader);

                        //ReadJson(root, Jsonname);
                    }
                }

                StartingRow = 0;//сброс начальной строки поиска в табице
                IsWithMergedMessages = IsContainsLinedMessages(Jsonname);//разбитые на строки сообщения есть и в других файлах, вроде maps,troops, может еще где
                if (IsWithMergedMessages)
                {
                    //сброс значений для CommonEvents
                    //curcode = string.Empty; // commented, see ResetCurrentCode()
                    propertyName = string.Empty;
                    skipIt = false;

                    //только в CommonEvents была сборка строк в сообщение
                    //SplitTableCellValuesAndTheirLinesToDictionary(Jsonname, false, false);
                }
                else
                {
                    //SplitTableCellValuesToDictionaryLines(Jsonname);
                }
                SplitTableCellValuesAndTheirLinesToDictionary(Jsonname, false, false);

                addedjobjects = new HashSet<JObject>();
                if (TablesLinesDict != null && TablesLinesDict.Count > 0)
                {
                    //GetTranslatableRows(Jsonname);
                    ParseJTokenWriteWithPreSplitlines(root, Jsonname);
                }
                else
                {
                    if (!ProjectData.THFilesElementsDataset.Tables.Contains(Jsonname))
                    {
                        return false;
                    }
                    ParseJTokenWrite(root, Jsonname);
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
        ///// <param name="Jsonname"></param>
        //private void GetTranslatableRows(string Jsonname)
        //{
        //    //IsTranslatableRow = new Dictionary<string, bool>();
        //    //foreach (System.Data.DataRow row in ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows)
        //    //{
        //    //    IsTranslatableRow.Add(row[0] as string,
        //    //        !(row[1] == null || string.IsNullOrEmpty(row[1] + "") || Equals(row[1], row[0]))
        //    //        );
        //    //}
        //}

        private static bool IsContainsLinedMessages(string Jsonname)
        {
            return true;
            //return Jsonname == "CommonEvents";
            //return Jsonname == "CommonEvents" || Jsonname.StartsWith("Map") || Jsonname == "Troops";
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

            if (jsonToken is JValue)
            {
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (jsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenValue = jsonToken + string.Empty;
                if (tokenValue.Length == 0 || FunctionsRomajiKana.LocalePercentIsNotValid(tokenValue))
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
                    }
                    else
                    {
                        string[] origA = FunctionsString.GetAllNonEmptyLinesToArray(row[0] + string.Empty);//Все строки, кроме пустых, чтобы потом исключить из проверки
                        int origALength = origA.Length;
                        if (origALength == 0)
                        {
                            origA = new string[1];
                            origA[0] = row[0] + string.Empty;
                            //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                        }

                        if (origALength > 0)
                        {
                            string[] transA = FunctionsString.GetAllNonEmptyLinesToArray(row[1] + string.Empty);

                            if (transA.Length == 0)
                            {
                                transA = new string[1];
                                transA[0] = row[1] + string.Empty;
                                //LogToFile("(transA.Length == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString());
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

                                    (jsonToken as JValue).Value = transA[i2]/*.Replace("\r", string.Empty)*/; //Replace убирает \r

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
                                (jsonToken as JValue).Value = row[1] + string.Empty;
                                StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                break;
                            }
                        }
                    }
                }
            }
            else if (jsonToken is JObject jsonObject)
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
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }

                        if (IsCodeWithStringInParameters(CurrentEventCode)) // in message codes need only last object value
                        {
                            ParseJTokenNew(jsonObject.Last, jsonName);
                        }
                        else if (IsExcludedCode(CurrentEventCode)) // code always located in the objects and dont need do as code below
                        {
                            AddToStats(false);
                            continue;
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
            }
            else if (jsonToken is JArray jsonArray)
            {
                int arrayCount = jsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ParseJTokenWrite(jsonArray[i], jsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        HashSet<JObject> addedjobjects;
        private void ParseJTokenWriteWithPreSplitlines(JToken jsonToken, string jsonName/*, string propertyname = ""*/)
        {
            if (jsonToken == null)
            {
                return;
            }

            if (jsonToken is JValue)
            {
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (jsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenvalue = jsonToken + string.Empty;

                if (tokenvalue.Length == 0 || FunctionsRomajiKana.LocalePercentIsNotValid(tokenvalue))
                {
                    return;
                }

                //предполагается, что ячейки из таблицы были разбиты на строки и добавлены в словарь
                if (/*IsTranslatableRow.ContainsKey(tokenvalue) && IsTranslatableRow[tokenvalue] tried to add check if row have translation but here also issue because no displaed rows will be translated &&*/
                    TablesLinesDict.ContainsKey(tokenvalue)
                    && TablesLinesDict[tokenvalue].Length > 0)
                {
                    try
                    {
                        int tokenvalueLinesCount;
                        if (jsonToken.Parent != null
                            && jsonToken.Parent.Parent != null
                            && jsonToken.Parent.Parent.Parent != null
                            && (jsonToken.Parent.Parent.Parent is JObject currentJObject)
                            && TablesLinesDict[tokenvalue].GetLinesCount() > (tokenvalueLinesCount = tokenvalue.GetLinesCount()))
                        {
                            int ind = 0;
                            List<string> stringToWrite = new List<string>();
                            bool NotWrited = true;
                            bool IsTheJObjetAdded = false;

                            foreach (var line in TablesLinesDict[tokenvalue].SplitToLines())
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
                                        (jsonToken as JValue).Value = string.Join(Environment.NewLine, stringToWrite);
                                        NotWrited = false;
                                    }

                                    var newJObject = GetNewJObject(currentJObject, line);
                                    currentJObject.AddAfterSelf(newJObject);
                                    if (!IsTheJObjetAdded && !addedjobjects.Contains(newJObject))
                                    {
                                        IsTheJObjetAdded = true;
                                        addedjobjects.Add(newJObject);
                                    }
                                    currentJObject = newJObject;//делать добавленный JObject текущим, чтобы новый добавлялся после него
                                }
                            }

                        }
                        else
                        {
                            (jsonToken as JValue).Value = TablesLinesDict[tokenvalue];
                        }
                    }
                    catch
                    {

                    }
                }
                else
                {
                    return;
                }
            }
            else if (jsonToken is JObject jsonObject)
            {
                //skip new added json object
                if (addedjobjects.Contains(jsonObject))
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

                        if (IsCodeWithStringInParameters(CurrentEventCode)) // in message codes need only last object value
                        {
                            ParseJTokenNew(jsonObject.Last, jsonName);
                        }
                        else if (IsExcludedCode(CurrentEventCode)) // code always located in the objects and dont need do as code below
                        {
                            AddToStats(false);
                            continue;
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
            }
            else if (jsonToken is JArray jsonArray)
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
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        /// <summary>
        /// copy all objects from input jContainer in new JObject and replace parameters value to new
        /// </summary>
        /// <param name="jContainer">input JContainer</param>
        /// <param name="line">line to which replace parameters value</param>
        /// <returns></returns>
        private static JObject GetNewJObject(JContainer jContainer, string line)
        {
            JObject ret = new JObject();
            foreach (var jProperty in (jContainer as JObject).Properties())
            {
                if (jProperty.Name == "parameters")
                {
                    if (jProperty.Value is JValue)
                    {
                        ret.Add(new JProperty(jProperty.Name, new JValue(line)));
                    }
                    else if (jProperty.Value is JObject)
                    {
                        ret.Add(new JProperty(jProperty.Name, new JObject("{" + line + "}")));
                    }
                    else if (jProperty.Value is JArray)
                    {
                        ret.Add(new JProperty(jProperty.Name, new JArray(new object[] { line })));
                    }
                }
                else
                {
                    if (jProperty.Value is JValue jValue)
                    {
                        ret.Add(new JProperty(jProperty.Name, new JValue(jValue)));
                    }
                    else if (jProperty.Value is JObject jObject)
                    {
                        ret.Add(new JProperty(jProperty.Name, new JObject(jObject)));
                    }
                    else if (jProperty.Value is JArray jArray)
                    {
                        ret.Add(new JProperty(jProperty.Name, new JArray(jArray)));
                    }
                }
            }

            return ret;
        }
    }
}
