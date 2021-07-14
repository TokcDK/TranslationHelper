using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVOLD
    {
        
        public RPGMMVOLD()
        {
            
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
                if (ProjectData.THFilesElementsDataset.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }
                ProjectData.Main.ProgressInfo(true, T._("opening file: ") + Jsonname + ".json");
                string jsondata = File.ReadAllText(sPath); // get json data

                ProjectData.THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                ProjectData.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                ProjectData.THFilesElementsDatasetInfo.Tables.Add(Jsonname); // create table with json name
                ProjectData.THFilesElementsDatasetInfo.Tables[Jsonname].Columns.Add("Original"); //create Original column
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

                if (ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows.Count > 0)
                {
                    ProjectData.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    ProjectData.THFilesElementsDataset.Tables.Remove(Jsonname); // remove table if was no items added
                    ProjectData.THFilesElementsDatasetInfo.Tables.Remove(Jsonname); // remove table if was no items added
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
        private readonly StringBuilder textsb = new StringBuilder();
        private string curcode = string.Empty;
        //string cType;
        //private string cCode = string.Empty;
        private string cName = string.Empty;
        //private string cId = string.Empty;
        //private string OldcId = "none";
        bool IsCommonEvents;
        private void ProceedJToken(JToken token, string Jsonname/*, string propertyname = ""*/)
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
                        if (string.IsNullOrWhiteSpace(textsb.ToString())/* || token.Path.Contains("switches") || token.Path.Contains("variables")*/)
                        {
                        }
                        else
                        {
                            string mergedstring = textsb.ToString();
                            if (/*GetAlreadyAddedInTable(Jsonname, mergedstring) || token.Path.Contains(".json'].data[") ||*/ mergedstring.ForJPLangHaveMostOfRomajiOtherChars())
                            {
                            }
                            else
                            {
                                //LogToFile("textsb is not empty. add. value=" + mergedstring + ", curcode=" + curcode);

                                ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows.Add(mergedstring);
                                //TempList.Add(mergedstring);//много быстрее

                                ProjectData.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JsonPath: " + token.Path);
                                //TempListInfo.Add("JsonPath: " + token.Path);//много быстрее
                            }
                            textsb.Clear();
                        }
                    }
                    //if (token.Type == JTokenType.String)
                    //{
                    if (tokenvalue.Length == 0/* || GetAlreadyAddedInTable(Jsonname, tokenvalue)*/ || FunctionsRomajiKana.LocalePercentIsNotValid(tokenvalue) /* очень медленная функция, лучше выполнить в фоне, вручную, после открытия || GetAnyFileWithTheNameExist(tokenvalue)*/)
                    {
                    }
                    else
                    {

                        ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows.Add(tokenvalue);

                        ProjectData.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JsonPath: " + token.Path);
                        //TempListInfo.Add("JsonPath: " + token.Path);//много быстрее
                    }
                    //}
                }
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    cName = property.Name;

                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (cName.Length == 4 && cName == "code")
                        {
                            curcode = property.Value + string.Empty;
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
                    ProceedJToken(property.Value, Jsonname/*, property.Name*/);
                }
            }
            else if (token is JArray array)
            {
                int arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }
        bool skipit;

        internal bool WriteJson(string Jsonname, string sPath)
        {
            ProjectData.Main.ProgressInfo(true, T._("Writing: ") + Jsonname + ".json");

            //skip file if table with same name has translation cells in all lines empty
            if (FunctionsTable.IsTableRowsAllEmpty(ProjectData.THFilesElementsDataset.Tables[Jsonname]))
                return true;

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
            }
            catch
            {
                //LogToFile(string.Empty, true);
                ProjectData.Main.ProgressInfo(false);
                return false;
            }
            finally
            {
            }
            //LogToFile(string.Empty, true);
            ProjectData.Main.ProgressInfo(false);
            return true;

        }

        int startingrow;//оптимизация. начальная строка, когда идет поиск по файлу, чтобы не искало каждый раз сначала при нахождении перевода будет переприсваиваться начальная строка на последнюю
        private void WProceedJToken(JToken token, string Jsonname/*, string propertyname = ""*/)
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
                if (tokenvalue.Length == 0 || FunctionsRomajiKana.LocalePercentIsNotValid(tokenvalue))
                {
                }
                else
                {
                    string parameter0value = tokenvalue;
                    if (parameter0value.Length == 0)
                    {
                    }
                    else //if code not equal old code and newline is not empty
                    {
                        //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

                        for (int i1 = startingrow; i1 < ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
                        {
                            var row = ProjectData.THFilesElementsDataset.Tables[Jsonname].Rows[i1];
                            if ((row[1] + string.Empty).Length == 0)
                            {
                            }
                            else
                            {
                                //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
                                //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp
                                string[] origA = (row[0] + string.Empty)
                                    .Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
                                    .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
                                    .ToArray();//Все строки, кроме пустых, чтобы потом исключить из проверки
                                int origALength = origA.Length;
                                if (origALength == 0)
                                {
                                    origA = new string[1];
                                    origA[0] = row[0] + string.Empty;
                                    //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                }

                                if (origALength > 0)
                                {
                                    string[] transA = (row[1] + string.Empty)
                                        .Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/)
                                        .Where(emptyvalues => (emptyvalues/*.Replace("\r", string.Empty)*/).Length != 0)
                                        .ToArray();//Все строки, кроме пустых
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

                                        //Это заменил расширением Where, что выше при задании массива, пустые строки будут исключены сразу
                                        //Проверить, есть ли в массиве хоть один пустой элемент
                                        //https://stackoverflow.com/questions/44405411/how-can-i-check-wether-an-array-contains-any-item-or-is-completely-empty
                                        //if (orig.Any(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)) ) )
                                        //А это считает количество пустых элементов в массиве
                                        //https://stackoverflow.com/questions/2391743/how-many-elements-of-array-are-not-null
                                        //int ymptyelementscnt = orig.Count(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)));

                                        transA = FunctionsString.SplitStringByEqualParts(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
                                    }

                                    //Подстраховочная проверка для некоторых значений из нескольких строк, полное сравнение перед построчной
                                    if (tokenvalue == row[0] + string.Empty)
                                    {
                                        //var t = token as JValue;
                                        (token as JValue).Value = (row[1] + string.Empty).Replace("\r", string.Empty);//убирает \r, т.к. в json присутствует только \n
                                        startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                        break;
                                    }

                                    bool br = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
                                    for (int i2 = 0; i2 < origALength; i2++)
                                    {
                                        //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                        if (parameter0value == origA[i2]/*.Replace("\r", string.Empty)*/) //Replace здесь убирает \r из за которой строки считались неравными
                                        {
                                            //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
                                            //var t = token as JValue;
                                            (token as JValue).Value = transA[i2]/*.Replace("\r", string.Empty)*/; //Replace убирает \r

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
                                    if (tokenvalue == row[0] + string.Empty)
                                    {
                                        //var t = token as JValue;
                                        (token as JValue).Value = row[1] + string.Empty;
                                        startingrow = i1;//запоминание строки, чтобы не пробегало всё с нуля
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    cName = property.Name;
                    if (IsCommonEvents)//asdfg skip code 108,408,356
                    {
                        if (cName.Length == 4 && cName == "code")
                        {
                            curcode = property.Value + string.Empty;
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
                    WProceedJToken(property.Value, Jsonname/*, property.Name*/);
                }
            }
            else if (token is JArray array)
            {
                int arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
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
