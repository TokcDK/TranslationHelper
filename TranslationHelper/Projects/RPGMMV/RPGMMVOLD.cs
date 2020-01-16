using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVOLD
    {
        THDataWork thDataWork;
        public RPGMMVOLD(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal bool OpenRPGMakerMVjson(string sPath)
        {
            //StreamReader _file = new StreamReader(sPath);
            //while (!_file.EndOfStream)
            //{
            //string jsonline = _file.ReadLine();
            //string jsonstring = _file.ReadToEnd();
            //JObject o1 = JObject.Parse(File.ReadAllText(@"test.json"));
            //var parsedObject = JObject.Parse(o1);
            //THFileElementsDataGridView.DataSource = o1;
            //if (parsedObject.ToString().ToLower().Contains("name"))
            //{
            //RPGMakerMVjsonFile o = JsonConvert.DeserializeObject<RPGMakerMVjsonFile>(jsonline);
            //MessageBox.Show(o.Name);
            //}
            //}

            //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
            /*using (var reader = new StreamReader(sPath))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var root = JToken.Load(jsonReader);
                THFileElementsDataGridView.DataSource = root;
            }*/

            try
            {

                //Вроде прочитало в DGV
                //источник: https://stackoverflow.com/questions/23763446/how-to-display-the-json-data-in-datagridview-in-c-sharp-windows-application-from

                string Jsonname = Path.GetFileNameWithoutExtension(sPath); // get json file name
                if (thDataWork.THFilesElementsDataset.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }
                thDataWork.Main.ProgressInfo(true, T._("opening file: ") + Jsonname + ".json");
                string jsondata = File.ReadAllText(sPath); // get json data

                thDataWork.THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                thDataWork.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                thDataWork.THFilesElementsDatasetInfo.Tables.Add(Jsonname); // create table with json name
                thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Columns.Add("Original"); //create Original column
                //MessageBox.Show("Added table:"+Jsonname);

                /*
                bool name = false;
                bool description = false;
                bool displayname = false;
                bool note = false;
                bool message1 = false;
                bool message2 = false;
                bool message3 = false;
                bool message4 = false;
                bool nickname = false;
                bool profile = false;
                bool maps = false;
                bool cmnevents = false;
                bool system = false;
                */

                bool ret = true;

                ret = ReadJson(Jsonname, sPath);

                /*
                string jsonname = Jsonname.ToLower(); //set jsonname to lower registry
                if (jsonname == "items" || jsonname == "armors" || jsonname == "weapons")
                {
                    //("name", "description", "note")
                    //name = true;
                    //description = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonItemsArmorsWeapons(jsonname, jsondata);
                }
                else if (jsonname == "skills")
                {
                    //("name", "description", "message1", "message2", "note")
                    //name = true;
                    //description = true;
                    //message1 = true;
                    //message2 = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonSkills(jsonname, jsondata);
                }
                else if (jsonname == "states")
                {
                    //("name", "message1", "message2", "message3", "message4", "note")
                    //name = true;
                    //message1 = true;
                    //message2 = true;
                    //message3 = true;
                    //message4 = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonStates(jsonname, jsondata);
                }
                else if (jsonname == "classes" || jsonname == "enemies" || jsonname == "tilesets")
                {
                    //("name", "note")
                    //name = true;
                    //note = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonClassesEnemiesTilesets(jsonname, jsondata);
                }
                else if (jsonname == "animations" || jsonname == "mapinfos" || jsonname == "troops")
                {
                    //("name")
                    //name = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(jsonname, jsondata);
                }
                else if (jsonname == "actors")
                {
                    //("name", "nickname", "note", "profile")
                    //name = true;
                    //nickname = true;
                    //note = true;
                    //profile = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(jsonname, jsondata);
                }
                else if (jsonname.StartsWith("map"))
                {
                    //['displayName'] / ['note'] / ['events'][$eIndex]['name'] / ['events'][$eIndex]['note']
                    //displayname = true;
                    //name = true;
                    //note = true;
                    //maps = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonMap(jsonname, jsondata);
                }
                else if (jsonname == "commonevents")
                {
                    //"name" / 
                    //name = true;
                    //cmnevents = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonCommonEvents(jsonname, jsondata);
                }
                else if (jsonname == "system")
                {
                    //"name" /
                    //name = true;
                    //system = true;
                    ret = ReadJson(jsonname, jsondata);
                    //ret = GetDataFromRPGMakerMVjsonSystem(jsonname, jsondata);
                }
                */

                //MessageBox.Show("ret="+ret+ ",jsonname="+ jsonname);

                /*
                bool ret = FillDSTableWithJsonValues(
                    Jsonname,
                    jsondata,
                    name,
                    description,
                    displayname,
                    note, message1,
                    message2,
                    message3,
                    message4,
                    nickname,
                    profile,
                    maps,
                    cmnevents,
                    system);
                */

                //var result = JsonConvert.DeserializeObject<List<RPGMakerMVjson>>(File.ReadAllText(sPath));
                //var resultdescriptions = JsonConvert.DeserializeObject<List<RPGMakerMVjsonFileDescriptions>>(File.ReadAllText(sPath));
                //var resultparameters = JsonConvert.DeserializeObject<List<RPGMakerMVjsonFileParameters>>(File.ReadAllText(sPath));

                //THFileElementsDataGridView.DataSource = ds.Tables[0];
                //THFileElementsDataGridView.Columns[0].ReadOnly = true;

                if (thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Count > 0)
                {
                    thDataWork.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    thDataWork.THFilesElementsDataset.Tables.Remove(Jsonname); // remove table if was no items added
                    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(Jsonname); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        private bool ReadJson(string Jsonname, string sPath)
        {
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
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
                ProceedJToken(root, Jsonname);

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
                //LogToFile(string.Empty, true);
                return false;
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
            return true;

        }

        //List<string> TempList;
        //List<string> TempListInfo;
        private StringBuilder textsb = new StringBuilder();
        private string curcode = string.Empty;
        //string cType;
        //private string cCode = string.Empty;
        private string cName = string.Empty;
        //private string cId = string.Empty;
        //private string OldcId = "none";
        bool IsCommonEvents = false;
        private void ProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (token.Type != JTokenType.String)
                {
                    return;
                }

                //if (cName == "code")
                //{
                //    curcode = token.ToString();
                //    //cCode = "Code" + curcode;
                //    //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                //}
                //else if (propertyname == "id")
                //{
                //    if (cId != OldcId)
                //    {
                //        OldcId = cId;
                //        cId = "ID" + token.ToString() + ":";
                //    }
                //}
                //LogToFile("JValue: " + propertyname + "=" + token.ToString()+", token path="+token.Path);
                string tokenvalue = token.ToString();

                if (IsCommonEvents && (curcode == "401" || curcode == "405"))
                {
                    //if (token.Type == JTokenType.String)
                    //{
                        if (textsb.ToString().Length > 0)
                        {
                            textsb.AppendLine();
                        }
                        //LogToFile("code 401 adding valur to merge=" + tokenvalue + ", curcode=" + curcode);
                        textsb.Append(tokenvalue);
                    //}
                }
                else
                {
                    if (IsCommonEvents)
                    {
                        if (string.IsNullOrWhiteSpace(textsb.ToString()))
                        {
                        }
                        else
                        {
                            string mergedstring = textsb.ToString();
                            if (/*GetAlreadyAddedInTable(Jsonname, mergedstring) || token.Path.Contains(".json'].data[") ||*/ Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese" && FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(mergedstring))
                            {
                            }
                            else
                            {
                                //LogToFile("textsb is not empty. add. value=" + mergedstring + ", curcode=" + curcode);

                                thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Add(mergedstring);
                                //TempList.Add(mergedstring);//много быстрее

                                //JToken t = token;
                                //while (!string.IsNullOrEmpty(t.Parent.Path))
                                //{
                                //    t = t.Parent;
                                //    extra += "\\" + t.Path;
                                //}

                                thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JsonPath: " + token.Path);
                                //TempListInfo.Add("JsonPath: " + token.Path);//много быстрее
                            }
                            textsb.Clear();
                        }
                    }
                    //if (token.Type == JTokenType.String)
                    //{
                        if (tokenvalue.Length == 0/* || GetAlreadyAddedInTable(Jsonname, tokenvalue)*/ || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(tokenvalue) /* очень медленная функция, лучше выполнить в фоне, вручную, после открытия || GetAnyFileWithTheNameExist(tokenvalue)*/)
                        {
                        }
                        else
                        {
                            //if (IsCommonEvents && curcode == "102")
                            //{
                            //    cName = "Choice";
                            //}

                            //LogToFile("Jsonname=" + Jsonname+ ", tokenvalue=" + tokenvalue);
                            //LogToFile(string.Empty, true);

                            thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Add(tokenvalue);
                            //TempList.Add(tokenvalue);//много быстрее

                            //dsinfo.Tables[0].Rows.Add(cType+"\\"+ cId + "\\" + cCode + "\\" + cName);
                            //JToken t = token;
                            //while (!string.IsNullOrEmpty(t.Parent.Path))
                            //{
                            //    t = t.Parent;
                            //    extra += "\\"+t.Path;
                            //}

                            thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JsonPath: " + token.Path);
                            //TempListInfo.Add("JsonPath: " + token.Path);//много быстрее
                        }
                    //}
                }
                //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                //childNode.Tag = token;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    //cType = "JObject";
                    cName = property.Name;
                    //LogToFile("JObject propery: " + property.Name + "=" + property.Value+ ", token.Path=" + token.Path);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    //childNode.Tag = property;

                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (cName.Length == 4 && cName == "code")
                        {
                            curcode = property.Value + string.Empty;
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }
                        if (skipit)
                        {
                            if (curcode == "108" || curcode == "408" || curcode == "356")
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
                            if (cName.Length == 4 && cName == "code")
                            {
                                string propertyValue = property.Value + string.Empty;
                                if (propertyValue.Length == 3 && (propertyValue == "108" || propertyValue == "408" || propertyValue == "356"))
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    ProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                int arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    //LogToFile("JArray=\r\n" + array[i] + "\r\n, token.Path=" + token.Path);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    //childNode.Tag = array[i];
                    //cType = "JArray";
                    ProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }
        bool skipit = false;

        internal bool WriteJson(string Jsonname, string sPath)
        {
            thDataWork.Main.ProgressInfo(true, T._("Writing: ") + Jsonname + ".json");
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
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

                startingrow = 0;//сброс начальной строки поиска в табице
                IsCommonEvents = (Jsonname == "CommonEvents");
                if (IsCommonEvents)
                {
                    //сброс значений для CommonEvents
                    curcode = string.Empty;
                    cName = string.Empty;
                    skipit = false;
                }
                WProceedJToken(root, Jsonname);

                Regex regex = new Regex(@"^\[null,(.+)\]$");//Корректировка формата записываемого json так, как в файлах RPGMaker MV
                File.WriteAllText(sPath, regex.Replace(root.ToString(Formatting.None), "[\r\nnull,\r\n$1\r\n]"));

                //treeView1.ExpandAll();
            }
            catch
            {
                //LogToFile(string.Empty, true);
                thDataWork.Main.ProgressInfo(false);
                return false;
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
            thDataWork.Main.ProgressInfo(false);
            return true;

        }

        int startingrow = 0;//оптимизация. начальная строка, когда идет поиск по файлу, чтобы не искало каждый раз сначала при нахождении перевода будет переприсваиваться начальная строка на последнюю
        private void WProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (token.Type != JTokenType.String)
                {
                    return;
                }

                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                string tokenvalue = token + string.Empty;
                if (tokenvalue.Length == 0 || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(tokenvalue))
                {
                }
                else
                {
                    //if (Jsonname == "States" && tokenvalue.Contains("自動的に付加されます"))
                    //{
                    //    //LogToFile("tokenvalue=" + tokenvalue);
                    //}
                    string parameter0value = tokenvalue;
                    if (parameter0value.Length == 0)
                    {
                    }
                    else //if code not equal old code and newline is not empty
                    {
                        //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

                        for (int i1 = startingrow; i1 < thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
                        {
                            if ((thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty).Length == 0)
                            {
                            }
                            else
                            {
                                //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
                                //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp
                                string[] origA = (thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty)
                                    .Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
                                    .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
                                    .ToArray();//Все строки, кроме пустых, чтобы потом исключить из проверки
                                int origALength = origA.Length;
                                if (origALength == 0)
                                {
                                    origA = new string[1];
                                    origA[0] = thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty;
                                    //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                }

                                if (origALength > 0)
                                {
                                    string[] transA = (thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty)
                                        .Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
                                        .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
                                        .ToArray();//Все строки, кроме пустых
                                    if (transA.Length == 0)
                                    {
                                        transA = new string[1];
                                        transA[0] = thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty;
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

                                        //Это заменил расширением Where, что выше при задании массива, пустые строки будут исключены сразу
                                        //Проверить, есть ли в массиве хоть один пустой элемент
                                        //https://stackoverflow.com/questions/44405411/how-can-i-check-wether-an-array-contains-any-item-or-is-completely-empty
                                        //if (orig.Any(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)) ) )
                                        //А это считает количество пустых элементов в массиве
                                        //https://stackoverflow.com/questions/2391743/how-many-elements-of-array-are-not-null
                                        //int ymptyelementscnt = orig.Count(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)));

                                        transA = FunctionsString.THSplit(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
                                    }

                                    //LogToFile("parameter0value=" + parameter0value);
                                    //if (Jsonname == "States" && tokenvalue.Contains("自動的に付加されます"))
                                    //{
                                    //    //LogToFile("tokenvalue=" + tokenvalue + ", tablevalue=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                    //}
                                    //Подстраховочная проверка для некоторых значений из нескольких строк, полное сравнение перед построчной
                                    if (tokenvalue == thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty)
                                    {
                                        var t = token as JValue;
                                        t.Value = (thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty).Replace("\r", string.Empty);//убирает \r, т.к. в json присутствует только \n
                                        startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                        break;
                                    }

                                    bool br = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
                                    for (int i2 = 0; i2 < origALength; i2++)
                                    {
                                        //LogToFile("parameter0value=" + parameter0value);
                                        //if (Jsonname == "States" && parameter0value.Contains("自動的に付加されます"))
                                        //{
                                        //    //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                        //}

                                        //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                        if (parameter0value == origA[i2]/*.Replace("\r", string.Empty)*/) //Replace здесь убирает \r из за которой строки считались неравными
                                        {
                                            //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                            var t = token as JValue;
                                            t.Value = transA[i2]/*.Replace("\r", string.Empty)*/; //Replace убирает \r

                                            startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                                             //LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
                                            br = true;
                                            break;
                                        }
                                    }
                                    if (br) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
                                    {
                                        startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                        break;
                                    }
                                }
                                else
                                {
                                    //LogToFile("tokenvalue=" + tokenvalue);
                                    //if (Jsonname == "States" && tokenvalue.Contains("自動的に付加されます"))
                                    //{
                                    //    //LogToFile("tokenvalue=" + tokenvalue + ", tablevalue=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                    //}
                                    if (tokenvalue == thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][0] + string.Empty)
                                    {
                                        var t = token as JValue;
                                        t.Value = thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] + string.Empty;
                                        startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                //childNode.Tag = token;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    //LogToFile("JObject propery: " + property.Name + "=" + property.Value);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    //childNode.Tag = property;
                    //cType = "JObject";
                    cName = property.Name;
                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (cName.Length == 4 && cName == "code")
                        {
                            curcode = property.Value + string.Empty;
                            //cCode = "Code" + curcode;
                            //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                        }
                        if (skipit)
                        {
                            if (curcode.Length == 3 && curcode == "108" || curcode == "408" || curcode == "356")
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
                            if (cName.Length == 4 && cName == "code")
                            {
                                string propertyValue = property.Value + string.Empty;
                                if (propertyValue.Length == 3 && (propertyValue == "108" || propertyValue == "408" || propertyValue == "356"))
                                {
                                    skipit = true;
                                    continue;
                                }
                            }
                        }
                    }
                    WProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                int arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    //LogToFile("JArray=\r\n" + array[i]);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    //childNode.Tag = array[i];
                    //cType = "JArray";
                    WProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }
    }
}
