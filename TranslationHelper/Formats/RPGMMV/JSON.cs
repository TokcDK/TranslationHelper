using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
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
                IsCommonEvents = (Jsonname == "CommonEvents");

                MessageMerged = new StringBuilder();

                ProceedJToken(root, Jsonname);
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

        bool MessageParsing = false;
        /// <summary>
        /// For Any Json
        /// </summary>
        /// <param name="JsonToken"></param>
        /// <param name="JsonName"></param>
        private void ProceedJTokenAny(JToken JsonToken, string JsonName)
        {
            if (JsonToken == null)
            {
                return;
            }

            if (JsonToken is JValue JsonValue)
            {
                if (JsonToken.Type != JTokenType.String)
                {
                    return;
                }

                AddRowData(JsonName, JsonValue.Value as string, JsonValue.Path);
            }
            else if (JsonToken is JObject JsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    ProceedJTokenAny(property.Value, JsonName);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                foreach (var Element in JsonArray)
                {
                    ProceedJTokenAny(Element, JsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        /// <summary>
        /// (WIP) for RPG Maker MV CommonEvents.json
        /// </summary>
        /// <param name="JsonToken"></param>
        /// <param name="JsonName"></param>
        private void ProceedJTokenCommonEvents(JToken JsonToken, string JsonName)
        {
            if (JsonToken == null)
            {
                return;
            }

            if (JsonToken is JValue JsonValue)
            {
                var t1 = JsonValue.Value;
                var t2 = JsonValue.Type;
                var t3 = JsonValue.Path;

                if (JsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenvalue = JsonValue.Value as string;
            }
            else if (JsonToken is JObject JsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    if (MessageParsing)
                    {
                    }
                    propertyName = property.Name;
                    if (!MessageParsing && propertyName == "code" && IsMessageCode(property.Value + string.Empty))
                    {
                        MessageParsing = true;

                    }
                    ProceedJTokenCommonEvents(property.Value, JsonName);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                foreach (var Element in JsonArray)
                {
                    ProceedJTokenCommonEvents(Element, JsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        private StringBuilder MessageMerged;
        private string curcode = string.Empty;
        //string cType;
        //private string cCode = string.Empty;
        private string propertyName = string.Empty;
        //private string cId = string.Empty;
        //private string OldcId = "none";
        bool IsCommonEvents = false;
        private void ProceedJToken(JToken JsonToken, string JsonName/*, string propertyname = ""*/)
        {
            if (JsonToken == null)
            {
                return;
            }

            if (JsonToken is JValue)
            {
                //научить запоминать параметры шифрования файлов, если научу их дешифровать
                //"hasEncryptedImages":true,"hasEncryptedAudio":true,"encryptionKey":"d41d8cd98f00b204e9800998ecf8427e"}

                if (JsonToken.Type != JTokenType.String)
                {
                    return;
                }

                string tokenvalue = JsonToken.ToString();

                if (IsCommonEvents && IsMessageCode(curcode))
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
                    if (IsCommonEvents)
                    {
                        if (string.IsNullOrWhiteSpace(MessageMerged.ToString())/* || token.Path.Contains("switches") || token.Path.Contains("variables")*/)
                        {
                        }
                        else
                        {
                            string mergedstring = MessageMerged.ToString();
                            if (/*GetAlreadyAddedInTable(Jsonname, mergedstring) || token.Path.Contains(".json'].data[") ||*/ Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(mergedstring))
                            {
                            }
                            else
                            {
                                AddData(JsonName, mergedstring, "JsonPath: "
                                    + Environment.NewLine
                                    + JsonToken.Path
                                    + (IsCommonEvents ? Environment.NewLine + "Code=" + curcode : string.Empty)
                                    );
                            }
                            MessageMerged.Clear();
                        }
                    }
                    if (tokenvalue.Length == 0/* || GetAlreadyAddedInTable(Jsonname, tokenvalue)*/ || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(tokenvalue) /* очень медленная функция, лучше выполнить в фоне, вручную, после открытия || GetAnyFileWithTheNameExist(tokenvalue)*/)
                    {
                    }
                    else
                    {
                        AddData(JsonName, tokenvalue, "JsonPath: "
                                    + Environment.NewLine
                                    + JsonToken.Path
                                    + (IsCommonEvents ? Environment.NewLine + "Code=" + curcode : string.Empty)
                                    );
                    }
                }
            }
            else if (JsonToken is JObject JsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    propertyName = property.Name;

                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (propertyName.Length == 4 && propertyName == "code")
                        {
                            curcode = property.Value + string.Empty;
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
                    ProceedJToken(property.Value, JsonName/*, property.Name*/);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                int arrayCount = JsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ProceedJToken(JsonArray[i], JsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
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
            thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Add(Value);
            //TempList.Add(Value);//много быстрее

            thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(Info);
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
                IsCommonEvents = (Jsonname == "CommonEvents");
                if (IsCommonEvents)
                {
                    //сброс значений для CommonEvents
                    curcode = string.Empty;
                    propertyName = string.Empty;
                    skipit = false;
                }

                SplitTableCellValuesToDictionaryLines(Jsonname);
                if (TableLines != null && TableLines.Count > 0)
                {
                    WProceedJTokenWithPreSplitlines(root, Jsonname);
                }
                else
                {
                    WProceedJToken(root, Jsonname);
                }

                Regex regex = new Regex(@"^\[null,(.+)\]$");//Корректировка формата записываемого json так, как в файлах RPGMaker MV
                File.WriteAllText(sPath, regex.Replace(root.ToString(Formatting.None), "[\r\nnull,\r\n$1\r\n]"));

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

        int StartingRow = 0;//оптимизация. начальная строка, когда идет поиск по файлу, чтобы не искало каждый раз сначала при нахождении перевода будет переприсваиваться начальная строка на последнюю
        private void WProceedJToken(JToken JsonToken, string JsonName/*, string propertyname = ""*/)
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
                        string[] origA = FunctionsString.GetAllNonEmptyLines(row[0] + string.Empty);//Все строки, кроме пустых, чтобы потом исключить из проверки
                        int origALength = origA.Length;
                        if (origALength == 0)
                        {
                            origA = new string[1];
                            origA[0] = row[0] + string.Empty;
                            //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                        }

                        if (origALength > 0)
                        {
                            string[] transA = FunctionsString.GetAllNonEmptyLines(row[1] + string.Empty);

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
                    if (IsCommonEvents)//asdfg skip code 108,408,356
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
                    WProceedJToken(property.Value, JsonName/*, property.Name*/);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                int arrayCount = JsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    WProceedJToken(JsonArray[i], JsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        private void WProceedJTokenWithPreSplitlines(JToken JsonToken, string JsonName/*, string propertyname = ""*/)
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

                if (TableLines.ContainsKey(tokenvalue) && TableLines[tokenvalue].Length > 0)
                {
                    (JsonToken as JValue).Value = TableLines[tokenvalue];
                }
                else
                {
                    return;
                }
            }
            else if (JsonToken is JObject JsonObject)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in JsonObject.Properties())
                {
                    propertyName = property.Name;
                    if (IsCommonEvents)//asdfg skip code 108,408,356
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
                    WProceedJTokenWithPreSplitlines(property.Value, JsonName/*, property.Name*/);
                }
            }
            else if (JsonToken is JArray JsonArray)
            {
                int arrayCount = JsonArray.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    WProceedJTokenWithPreSplitlines(JsonArray[i], JsonName);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }
    }
}
