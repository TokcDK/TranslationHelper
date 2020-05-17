using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMMV
{
    class JSON : RPGMMVBase
    {
        public JSON(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ReadJson(Path.GetFileNameWithoutExtension(thDataWork.FilePath), thDataWork.FilePath);
        }

        private bool ReadJson(string Jsonname, string sPath)
        {
            bool success = true;
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
                AddTables(Jsonname);

                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(Properties.Settings.Default.THSelectedDir);
                //using (var reader = new StreamReader(Properties.Settings.Default.THSelectedDir+"\\"+ Jsonname+".json"))
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
            catch
            {
                success = false;
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

            if (success && CheckTablesContent(Jsonname))
            {
                return true;
            }
            else
            {
                return false;
            }
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
        private string curcode = string.Empty;
        //string cType;
        //private string cCode = string.Empty;
        private string propertyName = string.Empty;
        //private string cId = string.Empty;
        //private string OldcId = "none";
        bool IsWithMergedMessages = false;
        private void ParseJToken(JToken jsonToken, string JsonName/*, string propertyname = ""*/)
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

                if (IsWithMergedMessages && IsMessageCode(curcode))
                {
                    if (MessageMerged.ToString().Length > 0)
                    {
                        MessageMerged.AppendLine();
                    }
                    //LogToFile("code 401 adding value to merge=" + tokenvalue + ", curcode=" + curcode);
                    MessageMerged.Append(tokenvalue);
                }
                else
                {
                    if (IsWithMergedMessages)
                    {
                        AddMergedMessage(jsonToken, JsonName);
                    }
                    if (tokenvalue.Length == 0/* || GetAlreadyAddedInTable(Jsonname, tokenvalue)*/ || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(tokenvalue) /* очень медленная функция, лучше выполнить в фоне, вручную, после открытия || GetAnyFileWithTheNameExist(tokenvalue)*/)
                    {
                    }
                    else
                    {
                        string c;
                        AddData(JsonName, tokenvalue, "JsonPath: "
                            + Environment.NewLine
                            + jsonToken.Path
                            + (!string.IsNullOrWhiteSpace(c = GetCodeValueOfParent(jsonToken)) ? Environment.NewLine + "Code=" + c :
                            IsWithMergedMessages && !string.IsNullOrWhiteSpace(curcode) ? Environment.NewLine + "Code=" + curcode : string.Empty)
                            + (c.Length > 0 && (c == "402" || c == "102") ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                            );
                    }
                }
            }
            else if (jsonToken is JObject JsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    propertyName = property.Name;

                    if (IsWithMergedMessages)//asdfg skip code 108,408,356
                    {
                        var IsCode = propertyName.Length == 4 && propertyName == "code";
                        if (IsCode)
                        {
                            curcode = property.Value + string.Empty;
                        }
                        if (skipit)
                        {
                            if (IsExcludedCode(curcode))
                            {
                                if (!IsCode && propertyName == "parameters")//asdf
                                {
                                    skipit = false;
                                    continue;
                                }
                            }
                            else
                            {
                                skipit = false;
                            }
                        }
                        else
                        {
                            if (IsCode)
                            {
                                //string propertyValue = property.Value + string.Empty;
                                if (IsExcludedCode(curcode))
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    ParseJToken(property.Value, JsonName/*, property.Name*/);
                }
            }
            else if (jsonToken is JArray JsonArray)
            {
                int arrayCount = JsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ParseJToken(JsonArray[i], JsonName);
                }

                //если последний массив properties был пустой, то добавить записанное сообщение
                if (IsWithMergedMessages && propertyName == "parameters" && arrayCount == 0 && MessageMerged.Length > 0)
                {
                    AddMergedMessage(jsonToken, JsonName, true, false);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
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
            if (string.IsNullOrWhiteSpace(mergedstring = MessageMerged.ToString())/* || token.Path.Contains("switches") || token.Path.Contains("variables")*/)
            {
            }
            else
            {
                if (/*GetAlreadyAddedInTable(Jsonname, mergedstring) || token.Path.Contains(".json'].data[") ||*/ Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(mergedstring))
                {
                }
                else
                {
                    string c;
                    AddData(JsonName, mergedstring, "JsonPath: "
                        + Environment.NewLine
                        + JsonToken.Path
                        + (!string.IsNullOrWhiteSpace(c = GetCodeValueOfParent(JsonToken, CheckPreviousToken, IsValue)) ? Environment.NewLine + "Code=" + c :
                        IsWithMergedMessages && !string.IsNullOrWhiteSpace(curcode) ? Environment.NewLine + "Code=" + curcode : string.Empty)
                        + (c.Length > 0 && (c == "402" || c == "102") ? Environment.NewLine + "note: Choice. Only 1 line." : string.Empty)
                        );
                }
                MessageMerged.Clear();
            }
        }

        /// <summary>
        /// get jobject with code jproperty
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <param name="previous">true means get previous jobject before this</param>
        /// <returns></returns>
        private string GetCodeValueOfParent(JToken jsonToken, bool previous = false, bool IsValue = true)
        {
            if (previous)
            {
                if (jsonToken.Parent != null)
                {
                    if (jsonToken.Parent.Parent != null)
                    {
                        if (IsValue)
                        {
                            if (jsonToken.Parent.Parent.Parent != null && jsonToken.Parent.Parent.Parent is JObject obj)
                            {
                                if (obj.Previous != null)
                                {
                                    if (obj.Previous is JObject obj1)
                                    {
                                        if (obj1.ContainsKey("code"))
                                        {
                                            return obj1.Value<long>("code") + string.Empty;
                                        }
                                    }
                                }
                            }
                        }
                        else if (jsonToken.Parent.Parent is JObject obj)
                        {
                            if (obj.Previous != null)
                            {
                                if (obj.Previous is JObject obj1)
                                {
                                    if (obj1.ContainsKey("code"))
                                    {
                                        return obj1.Value<long>("code") + string.Empty;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (jsonToken.Parent != null)
            {
                if (jsonToken.Parent.Parent != null)
                {
                    if (IsValue)
                    {
                        if (jsonToken.Parent.Parent.Parent != null)
                        {
                            if (jsonToken.Parent.Parent.Parent is JObject obj)
                            {
                                if (obj.ContainsKey("code"))
                                {
                                    return obj.Value<long>("code") + string.Empty;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (jsonToken.Parent.Parent.Parent is JObject obj)
                        {
                            if (obj.ContainsKey("code"))
                            {
                                return obj.Value<long>("code") + string.Empty;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// String for this codes must not be appeared
        /// </summary>
        /// <param name="curcode"></param>
        /// <returns></returns>
        private bool IsExcludedCode(string curcode)
        {
            return curcode.Length == 3 && (curcode == "108" || curcode == "408"/* || curcode == "356"*/);
        }

        /// <summary>
        /// Multiline message
        /// </summary>
        /// <param name="curcode"></param>
        /// <returns></returns>
        private bool IsMessageCode(string curcode)
        {
            return (curcode == "401" || curcode == "405");
        }

        //List<string> TempList;
        //List<string> TempListInfo;

        private void AddData(string Jsonname, string Value, string Info)
        {
            AddRowData(Jsonname, Value, Info, true);
            //if (hashes.Contains(Value))
            //    return;

            //thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Add(Value);
            //TempList.Add(Value);//много быстрее
            //hashes.Add(Value);

            //thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(Info);
            //TempListInfo.Add(Info);//много быстрее
        }

        bool skipit = false;

        internal override bool Save()
        {
            return WriteJson(Path.GetFileNameWithoutExtension(thDataWork.FilePath), thDataWork.FilePath);
        }

        private bool WriteJson(string Jsonname, string sPath)
        {
            thDataWork.Main.ProgressInfo(true, T._("Writing") + ": " + Jsonname + ".json");
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
                    curcode = string.Empty;
                    propertyName = string.Empty;
                    skipit = false;

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
                    ParseJTokenWriteWithPreSplitlines(root, Jsonname);
                }
                else
                {
                    if(!thDataWork.THFilesElementsDataset.Tables.Contains(Jsonname))
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
            finally
            {
            }
            return true;

        }

        private bool IsContainsLinedMessages(string Jsonname)
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
        private string CorrectJsonFormatToRPGMMV(JToken root)
        {
            return root.ToString(Formatting.Indented);
            //return Regex.Replace(root.ToString(Formatting.None), @"^\[null,(.+)\]$", "[\r\nnull,\r\n$1\r\n]");//regex нужен только для Formatting.None
        }

        int StartingRow = 0;//оптимизация. начальная строка, когда идет поиск по файлу, чтобы не искало каждый раз сначала при нахождении перевода будет переприсваиваться начальная строка на последнюю
        private void ParseJTokenWrite(JToken JsonToken, string JsonName/*, string propertyname = ""*/)
        {
            if (JsonToken == null)
            {
                return;
            }

            if (JsonToken is JValue)
            {
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (JsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenvalue = JsonToken + string.Empty;
                if (tokenvalue.Length == 0 || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(tokenvalue))
                {
                    return;
                }

                string parameter0value = tokenvalue;
                if (parameter0value.Length == 0)
                {
                    return;
                }

                //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

                int rcount = thDataWork.THFilesElementsDataset.Tables[JsonName].Rows.Count;
                for (int r = StartingRow; r < rcount; r++)
                {
                    var row = thDataWork.THFilesElementsDataset.Tables[JsonName].Rows[r];
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
                            string transmerged = string.Empty;
                            if (transA.Length == origALength)//если количество строк в оригинале и переводе равно
                            {
                                //ничего не делать
                            }
                            else // если перевод вдруг был переведен так, что не равен количеством строк оригиналу, тогда поделить его на равные строки
                            {
                                if (transA.Length > 0) // но перед этим, если перевод больше одной строки
                                {
                                    foreach (string ts in transA)
                                    {
                                        transmerged += ts; // объединить все строки в одну
                                    }
                                }

                                transA = FunctionsString.SplitStringByEqualParts(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
                            }

                            //Подстраховочная проверка для некоторых значений из нескольких строк, полное сравнение перед построчной
                            if (tokenvalue == row[0] + string.Empty)
                            {
                                (JsonToken as JValue).Value = (row[1] + string.Empty).Replace("\r", string.Empty);//убирает \r, т.к. в json присутствует только \n
                                StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                break;
                            }

                            bool TranslationWasSet = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
                            for (int i2 = 0; i2 < origALength; i2++)
                            {
                                if (parameter0value == origA[i2]/*.Replace("\r", string.Empty)*/) //Replace здесь убирает \r из за которой строки считались неравными
                                {
                                    //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));

                                    (JsonToken as JValue).Value = transA[i2]/*.Replace("\r", string.Empty)*/; //Replace убирает \r

                                    StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                                    //LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
                                    TranslationWasSet = true;
                                    break;
                                }
                            }
                            if (TranslationWasSet) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
                            {
                                StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                break;
                            }
                        }
                        else
                        {
                            if (tokenvalue == row[0] + string.Empty)
                            {
                                (JsonToken as JValue).Value = row[1] + string.Empty;
                                StartingRow = r;//запоминание строки, чтобы не пробегало всё с нуля
                                break;
                            }
                        }
                    }
                }
            }
            else if (JsonToken is JObject JsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    propertyName = property.Name;
                    if (IsWithMergedMessages)//asdfg skip code 108,408,356
                    {
                        if (propertyName.Length == 4 && propertyName == "code")
                        {
                            curcode = property.Value + string.Empty;
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }
                        if (skipit)
                        {
                            if (IsExcludedCode(curcode))
                            {
                                if (property.Name == "parameters")//asdf
                                {
                                    skipit = false;
                                    continue;
                                }
                            }
                            else
                            {
                                skipit = false;
                            }
                        }
                        else
                        {
                            if (propertyName.Length == 4 && propertyName == "code")
                            {
                                string propertyValue = property.Value + string.Empty;
                                if (IsExcludedCode(propertyValue))
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    ParseJTokenWrite(property.Value, JsonName/*, property.Name*/);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                int arrayCount = JsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ParseJTokenWrite(JsonArray[i], JsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        HashSet<JObject> addedjobjects;
        private void ParseJTokenWriteWithPreSplitlines(JToken JsonToken, string JsonName/*, string propertyname = ""*/)
        {
            if (JsonToken == null)
            {
                return;
            }

            if (JsonToken is JValue)
            {
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (JsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenvalue = JsonToken + string.Empty;
                if (tokenvalue.Length == 0 || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(tokenvalue))
                {
                    return;
                }

                if (TablesLinesDict.ContainsKey(tokenvalue) && TablesLinesDict[tokenvalue].Length > 0)
                {
                    try
                    {
                        bool test = true;
                        if (test && JsonToken.Parent != null && JsonToken.Parent.Parent != null && JsonToken.Parent.Parent.Parent != null && (JsonToken.Parent.Parent.Parent is JObject thisCodeObject) && TablesLinesDict[tokenvalue].GetLinesCount() > tokenvalue.GetLinesCount())
                        {
                            int tokenvalueLinesCount = tokenvalue.GetLinesCount();
                            int ind = 0;
                            List<string> stringToWrite = new List<string>();
                            bool NotWrited = true;
                            bool IsTheJObjetAdded = false;
                            //JsonToken.Parent.Add(",\""+ ExtraLineValue + "\"");
                            //JsonToken.Parent.Parent.Add("{\"code\":401,\"indent\":0,\"parameters\":[\"" + ExtraLineValue +"\"]}");
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
                                        (JsonToken as JValue).Value = string.Join(Environment.NewLine, stringToWrite);
                                        NotWrited = false;
                                    }

                                    //добавить новый объект с экстра строкой сразу после текущего
                                    //new JObject("{\"code\":401,\"indent\":0,\"parameters\":[\"" + line + "\"]}")

                                    //if(thisCodeObject.ContainsKey("code"))
                                    //{
                                    //    //var t = thisCodeObject.Value<long>("code");
                                    //}
                                    var NewJObject = GetNewJObject(thisCodeObject, line);
                                    thisCodeObject.AddAfterSelf(NewJObject);
                                    if (!IsTheJObjetAdded && !addedjobjects.Contains(NewJObject))
                                    {
                                        IsTheJObjetAdded = true;
                                        addedjobjects.Add(NewJObject);
                                    }
                                    thisCodeObject = NewJObject;//делать добавленный JObject текущим, чтобы новый добавлялся после него
                                    //thisCodeObject.AddAfterSelf(
                                    //    new JObject
                                    //(
                                    //    new JProperty("code", new JValue(401)),
                                    //    new JProperty("indent", new JValue(0)),
                                    //    new JProperty("parameters", new JArray(new JValue(line)))
                                    //)
                                    //    );
                                    //var j = JsonToken.Parent;
                                    //add extra as child parameter value
                                    //(JsonToken.Parent as JProperty).Add(",\"" + line + "\"");
                                }
                            }

                        }
                        else
                        {
                            (JsonToken as JValue).Value = TablesLinesDict[tokenvalue];
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
            else if (JsonToken is JObject JsonObject)
            {
                if (addedjobjects.Contains(JsonObject))
                {
                    return;
                }

                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    propertyName = property.Name;
                    if (IsWithMergedMessages)//asdfg skip code 108,408,356
                    {
                        var IsCode = propertyName.Length == 4 && propertyName == "code";
                        if (IsCode)
                        {
                            curcode = property.Value + string.Empty;
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }
                        if (skipit)
                        {
                            if (IsExcludedCode(curcode))
                            {
                                if (!IsCode && propertyName == "parameters")//asdf
                                {
                                    skipit = false;
                                    continue;
                                }
                            }
                            else
                            {
                                skipit = false;
                            }
                        }
                        else
                        {
                            if (IsCode)
                            {
                                //string propertyValue = property.Value + string.Empty;
                                if (IsExcludedCode(curcode))
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    ParseJTokenWriteWithPreSplitlines(property.Value, JsonName/*, property.Name*/);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                for (int i = 0; i < JsonArray.Count/*(здесь должно быть без предварительного зпоминания количества, т.к. массив меняется)*/
                    ; i++)
                {
                    try
                    {
                        ParseJTokenWriteWithPreSplitlines(JsonArray[i], JsonName);
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

        private JObject GetNewJObject(JContainer thisCodeObject, string line)
        {
            JObject ret = new JObject();
            foreach (var JsonProperty in (thisCodeObject as JObject).Properties())
            {
                if (JsonProperty.Name == "parameters")
                {
                    if (JsonProperty.Value is JValue)
                    {
                        ret.Add(new JProperty(JsonProperty.Name, new JValue(line)));
                    }
                    else if (JsonProperty.Value is JObject)
                    {
                        ret.Add(new JProperty(JsonProperty.Name, new JObject("{" + line + "}")));
                    }
                    else if (JsonProperty.Value is JArray)
                    {
                        ret.Add(new JProperty(JsonProperty.Name, new JArray(new object[] { line })));
                    }
                }
                else
                {
                    if (JsonProperty.Value is JValue val)
                    {
                        ret.Add(new JProperty(JsonProperty.Name, new JValue(val)));
                    }
                    else if (JsonProperty.Value is JObject obj)
                    {
                        ret.Add(new JProperty(JsonProperty.Name, new JObject(obj)));
                    }
                    else if (JsonProperty.Value is JArray arr)
                    {
                        ret.Add(new JProperty(JsonProperty.Name, new JArray(arr)));
                    }
                }
            }

            return ret;

            //new JObject
            //                        (
            //                            new JProperty("code", new JValue(401)),
            //                            new JProperty("indent", new JValue(0)),
            //                            new JProperty("parameters", new JArray(new JValue(line)))
            //                        )
            //                            );
        }
    }
}
