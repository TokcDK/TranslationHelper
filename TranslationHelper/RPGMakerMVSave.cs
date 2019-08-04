using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper
{
    class RPGMakerMVSave
    {
        THMain Main = new THMain();

        private bool SaveRPGMakerMVjson(string sPath)
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
                if (Main.THFilesElementsDataset.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }
                string jsondata = File.ReadAllText(sPath); // get json data

                Main.THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                Main.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                
                bool ret = true;
                string jsonname = Jsonname.ToLower(); //set jsonname to lower registry
                if (jsonname == "items" || jsonname == "armors" || jsonname == "weapons")
                {
                    //("name", "description", "note")
                    //name = true;
                    //description = true;
                    //note = true;
                    ret = GetDataFromRPGMakerMVjsonItemsArmorsWeapons(jsonname, jsondata);
                }
                else if (jsonname == "skills")
                {
                    //("name", "description", "message1", "message2", "note")
                    //name = true;
                    //description = true;
                    //message1 = true;
                    //message2 = true;
                    //note = true;
                    ret = GetDataFromRPGMakerMVjsonSkills(jsonname, jsondata);
                }
                else if (jsonname == "states")
                {
                    //("name", "message1", "message2", "message3", "message4", "note")
                    /*
                    name = true;
                    message1 = true;
                    message2 = true;
                    message3 = true;
                    message4 = true;
                    note = true;
                    */
                    ret = GetDataFromRPGMakerMVjsonStates(jsonname, jsondata);
                }
                else if (jsonname == "classes" || jsonname == "enemies" || jsonname == "tilesets")
                {
                    //("name", "note")
                    /*
                    name = true;
                    note = true;
                    */
                    ret = GetDataFromRPGMakerMVjsonClassesEnemiesTilesets(jsonname, jsondata);
                }
                else if (jsonname == "animations" || jsonname == "mapinfos" || jsonname == "troops")
                {
                    //("name")
                    //name = true;
                    ret = GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(jsonname, jsondata);
                }
                else if (jsonname == "actors")
                {
                    //("name", "nickname", "note", "profile")
                    /*
                    name = true;
                    nickname = true;
                    note = true;
                    profile = true;
                    */
                    ret = GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(jsonname, jsondata);
                }
                else if (jsonname.StartsWith("map"))
                {
                    //['displayName'] / ['note'] / ['events'][$eIndex]['name'] / ['events'][$eIndex]['note']
                    //displayname = true;
                    //name = true;
                    //note = true;
                    //maps = true;
                    ret = GetDataFromRPGMakerMVjsonMap(jsonname, jsondata);
                }
                else if (jsonname == "commonevents")
                {
                    //"name" / 
                    //name = true;
                    //cmnevents = true;
                    ret = GetDataFromRPGMakerMVjsonCommonEvents(jsonname, jsondata);
                }
                else if (jsonname == "system")
                {
                    //"name" /
                    //name = true;
                    //system = true;
                    ret = GetDataFromRPGMakerMVjsonSystem(jsonname, jsondata);
                }

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

                if (Main.THFilesElementsDataset.Tables[Jsonname].Rows.Count > 0)
                {
                    Main.THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    Main.THFilesElementsDataset.Tables.Remove(Jsonname); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        private void GetDataFromRPGMakerMVjsonOfType(string Jsonname, string JsonElement)
        {
            if (string.IsNullOrEmpty(JsonElement) || Main.SelectedLocalePercentFromStringIsNotValid(JsonElement))
            {
            }
            else
            {
                for (int i = 0; i < Main.THFilesElementsDataset.Tables[Jsonname].Rows.Count; i++)
                {
                    if (JsonElement == Main.THFilesElementsDataset.Tables[Jsonname].Rows[i][0].ToString())
                    {
                        JsonElement = Main.THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString();
                        break;
                    }
                }
                Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(JsonElement);
            }
        }

        private bool GetDataFromRPGMakerMVjsonItemsArmorsWeapons(string Jsonname, string jsondata)
        {
            try
            {
                var jsonobj = JsonConvert.DeserializeObject<List<RPGMakerMVjsonItemsArmorsWeapons>>(jsondata);
                foreach (var JsonElement in jsonobj)
                {
                    if (JsonElement == null)
                    {
                    }
                    else
                    {
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
                    }
                }

                JsonConvert.SerializeObject(jsonobj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonSkills(string Jsonname, string jsondata)
        {
            try
            {
                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonSkills>>(jsondata))
                {
                    if (JsonElement == null)
                    {
                    }
                    else
                    {
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message1);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message2);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonStates(string Jsonname, string jsondata)
        {
            try
            {
                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonStates>>(jsondata))
                {
                    if (JsonElement == null)
                    {
                    }
                    else
                    {
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message1);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message2);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message3);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message4);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonClassesEnemiesTilesets(string Jsonname, string jsondata)
        {
            try
            {
                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonClassesEnemiesTilesets>>(jsondata))
                {
                    if (JsonElement == null)
                    {
                    }
                    else
                    {
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(string Jsonname, string jsondata)
        {
            try
            {
                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonAnimationsMapInfosTroops>>(jsondata))
                {
                    if (JsonElement == null)
                    {
                    }
                    else
                    {
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonActors(string Jsonname, string jsondata)
        {
            try
            {
                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonActors>>(jsondata))
                {
                    if (JsonElement == null)
                    {
                    }
                    else
                    {
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Nickname);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Profile);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonCommonEvents(string Jsonname, string jsondata)
        {
            try
            {
                //info RPG Maker MV Event codes
                //https://forums.rpgmakerweb.com/index.php?threads/extract-events-to-text-file.17444/
                //https://forums.rpgmakerweb.com/index.php?threads/cross-reference-tool.72563/
                //https://pastebin.com/JyRTdq0b
                //https://pastebin.com/eJx0EvXB
                //    case 401 : return 'Show Text';              break;
                //    case 102 : return 'Show Choices';           break;
                //    case 103 : return 'Input Number';           break;
                //    case 104 : return 'Select Item';            break;
                //    case 405 : return 'Show Scrolling Text';    break;
                //    case 111 : return 'Conditional Branch';     break;
                //    case 119 : return 'Common Event';           break;
                //    case 121 : return 'Control Switches';       break;
                //    case 122 : return 'Control Variables';      break;
                //    case 125 : return 'Change Gold';            break;
                //    case 126 : return 'Change Items';           break;
                //    case 127 : return 'Change Weapons';         break;
                //    case 128 : return 'Change Armors';          break;
                //    case 129 : return 'Change Party Member';    break;
                //    case 201 : return 'Transfer Player';        break;
                //    case 202 : return 'Set Vehicle Location';   break;
                //    case 203 : return 'Set Event Location';     break;
                //    case 505 : return 'Set Movement Route';     break;
                //    case 212 : return 'Show Animation';         break;
                //    case 231 : return 'Show Picture';           break;
                //    case 232 : return 'Move Picture';           break;
                //    case 285 : return 'Get Location Info';      break;
                //    case 301 : return 'Battle Processing';      break;
                //    case 302 :
                //    case 605 : return 'Shop Processing';        break;
                //    case 303 : return 'Name Input Processing';  break;
                //    case 311 : return 'Change HP';              break;
                //    case 312 : return 'Change MP';              break;
                //    case 326 : return 'Change TP';              break;
                //    case 313 : return 'Change State';           break;
                //    case 314 : return 'Recover All';            break;
                //    case 315 : return 'Change EXP';             break;
                //    case 316 : return 'Change Level';           break;
                //    case 317 : return 'Change Parameter';       break;
                //    case 318 : return 'Change Skill';           break;
                //    case 319 : return 'Change Equipment';       break;
                //    case 320 : return 'Change Name';            break;
                //    case 321 : return 'Change Class';           break;
                //    case 322 : return 'Change Actor Images';    break;
                //    case 324 : return 'Change Nickname';        break;
                //    case 325 : return 'Change Profile';         break;
                //    case 331 : return 'Change Enemy HP';        break;
                //    case 332 : return 'Change Enemy MP';        break;
                //    case 342 : return 'Change Enemy TP';        break;
                //    case 333 : return 'Change Enemy State';     break;
                //    case 336 : return 'Enemy Transform';        break;
                //    case 337 : return 'Show Battle Animation';  break;
                //    case 339 : return 'Force Action';           break;
                //
                //Will be handled:
                //401 - Show text (mergeable)
                //102 - Show choices (Choices list)
                //402 - Choice for choices - ignore because already in 102
                //405 - Show Scrolling Text (mergeable)
                //108 and 408 - Comment - can be ignored because it is for dev suppose
                //normal example about command values adding: https://galvs-scripts.com/galvs-party-select/


                var commoneventsdata = JsonConvert.DeserializeObject<List<RPGMakerMVjsonCommonEvents>>(jsondata);

                for (int i = 1; i < commoneventsdata.Count; i++)
                {
                    //FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

                    //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
                    //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

                    string eventname = commoneventsdata[i].Name;
                    if (string.IsNullOrEmpty(eventname) || Main.GetAlreadyAddedInTable(Jsonname, eventname))
                    {
                    }
                    else //if code not equal old code and newline is not empty
                    {
                        Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(eventname); //add event name to new row
                    }

                    string newline = "";
                    //int commandcode;
                    //int commandoldcode = 999999;
                    bool textaddingstarted = false;


                    int CommandsCount = commoneventsdata[i].List.Length;
                    for (int c = 0; c < CommandsCount; c++)
                    {
                        if (textaddingstarted)
                        {
                            if (commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
                            {
                                newline += commoneventsdata[i].List[c].Parameters[0];

                                if (c < CommandsCount - 1 && commoneventsdata[i].List[c].Code == commoneventsdata[i].List[c + 1].Code)
                                {
                                    newline += "\r\n";
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(newline))
                                    {
                                        if (textaddingstarted)
                                        {
                                            //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted is true and newline is empty\r\n";
                                            textaddingstarted = false;
                                        }
                                    }
                                    else //if code not equal old code and newline is not empty
                                    {
                                        if (Main.GetAlreadyAddedInTable(Jsonname, newline))
                                        {
                                            //THLog += DateTime.Now + " >>: Code 401/405 newline already in table=\"" + newline + "\"\r\n";
                                            newline = ""; //clear text data
                                            if (textaddingstarted)
                                            {
                                                textaddingstarted = false;
                                            }
                                        }
                                        else
                                        {
                                            //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted=true added newline=\"" + newline + "\"\r\n";
                                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
                                            newline = ""; //clear text data
                                            textaddingstarted = false;
                                        }
                                    }
                                }
                            }
                        }
                        else if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105)
                        {
                            if (string.IsNullOrEmpty(newline))
                            {
                                if (textaddingstarted)
                                {
                                    //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline is empty\r\n";
                                    textaddingstarted = false;
                                }
                            }
                            else //if code not equal old code and newline is not empty
                            {
                                if (Main.GetAlreadyAddedInTable(Jsonname, newline))
                                {
                                    if (textaddingstarted)
                                    {
                                        //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline already in table=\"" + newline + "\"\r\n";
                                        textaddingstarted = false;
                                    }
                                }
                                else
                                {
                                    //THLog += DateTime.Now + " >>: Code 101/105 newline is not empty=\"" + newline + "\"\r\n";
                                    Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
                                    newline = ""; //clear text data
                                    textaddingstarted = false;
                                }
                            }

                            textaddingstarted = true;
                        }
                        else if (commoneventsdata[i].List[c].Code == 102)
                        {
                            JArray choices = JArray.Parse(commoneventsdata[i].List[c].Parameters[0].ToString());

                            foreach (var choice in choices)
                            {
                                string schoice = choice.ToString();
                                if (string.IsNullOrEmpty(schoice))
                                {
                                    if (textaddingstarted)
                                    {
                                        //THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
                                        textaddingstarted = false;
                                    }
                                }
                                else //if code not equal old code and newline is not empty
                                {
                                    if (Main.GetAlreadyAddedInTable(Jsonname, schoice))
                                    {
                                        //THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
                                        if (textaddingstarted)
                                        {
                                            //THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
                                            textaddingstarted = false;
                                        }
                                    }
                                    else
                                    {
                                        //THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
                                        Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(schoice); //Save text to new row
                                        if (string.IsNullOrEmpty(newline))
                                        {
                                        }
                                        else
                                        {
                                            //THLog += DateTime.Now + " >>: Code 102 added schoice and also newline is not empty, set empty\r\n";
                                            newline = ""; //clear text data
                                        }
                                        if (textaddingstarted)
                                        {
                                            //THLog += DateTime.Now + " >>: Code 102 added schoice and also textaddingstarted is true , set false\r\n";
                                            textaddingstarted = false;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonMap(string Jsonname, string jsondata)
        {
            try
            {
                //bool eventsdone = false;
                //bool geteventnamenotedone = false;

                var map = JsonConvert.DeserializeObject<RPGMakerMVjsonMap>(jsondata);

                if (map.Events.Length > 1) //first event is empty
                {
                    //Map displayed name
                    if (string.IsNullOrEmpty(map.DisplayName) || Main.SelectedLocalePercentFromStringIsNotValid(map.DisplayName) || Main.GetAlreadyAddedInTable(Jsonname, map.DisplayName))
                    {
                    }
                    else
                    {
                        //MessageBox.Show("map.DisplayName:" + map.DisplayName);
                        Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.DisplayName);
                    }
                    //Map note
                    if (string.IsNullOrEmpty(map.Note) || Main.SelectedLocalePercentFromStringIsNotValid(map.Note) || Main.GetAlreadyAddedInTable(Jsonname, map.Note))
                    {
                    }
                    else
                    {
                        //MessageBox.Show("map.Note:" + map.Note);
                        Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.Note);
                    }

                    //string prevval = "";
                    foreach (Event ev in map.Events)
                    {
                        if (ev == null)
                        {
                        }
                        else
                        {
                            //event name
                            if (string.IsNullOrEmpty(ev.Name) || ev.Name.StartsWith("EV") || Main.SelectedLocalePercentFromStringIsNotValid(ev.Name) || Main.GetAlreadyAddedInTable(Jsonname, ev.Name))
                            {
                            }
                            else
                            {
                                //MessageBox.Show("map.Events add name"+ ev.Name);
                                Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Name);
                                //prevval = ev.Name;
                            }
                            //event note
                            if (string.IsNullOrEmpty(ev.Note) || Main.SelectedLocalePercentFromStringIsNotValid(ev.Note) || Main.GetAlreadyAddedInTable(Jsonname, ev.Note))
                            {
                            }
                            else
                            {
                                //MessageBox.Show("map.Events add note:" + ev.Note);
                                Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Note);
                            }

                            //event parameters
                            foreach (Page page in ev.Pages)
                            {
                                foreach (PageList lst in page.List)
                                {
                                    foreach (var parameter in lst.Parameters)
                                    {
                                        if (parameter == null)
                                        {

                                        }
                                        else if (parameter.GetType().Name == "String")
                                        {
                                            string pstring = parameter.ToString();
                                            if (string.IsNullOrEmpty(pstring) || Main.HasNOJPcharacters(pstring) || Main.SelectedLocalePercentFromStringIsNotValid(pstring) || Main.GetAlreadyAddedInTable(Jsonname, pstring))
                                            {

                                            }
                                            else
                                            {
                                                Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(pstring);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetDataFromRPGMakerMVjsonSystem(string Jsonname, string jsondata)
        {
            try
            {
                //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
                var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

                //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

                if (systemdata.GameTitle == null || string.IsNullOrEmpty(systemdata.GameTitle))
                {
                }
                else
                {
                    Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);
                }

                if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
                {
                }
                else
                {
                    foreach (string armortype in systemdata.ArmorTypes)
                    {
                        if (string.IsNullOrEmpty(armortype))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
                        }
                    }
                }
                if (systemdata.Elements == null || systemdata.Elements.Length < 1)
                {

                }
                else
                {
                    foreach (string element in systemdata.Elements)
                    {
                        if (string.IsNullOrEmpty(element))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
                        }
                    }
                }
                if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
                {

                }
                else
                {
                    foreach (string equipType in systemdata.EquipTypes)
                    {
                        if (string.IsNullOrEmpty(equipType))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
                        }
                    }
                }
                if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
                {

                }
                else
                {
                    foreach (string skillType in systemdata.skillTypes)
                    {
                        if (string.IsNullOrEmpty(skillType))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
                        }
                    }
                }
                if (systemdata.Switches == null || systemdata.Switches.Length < 1)
                {

                }
                else
                {
                    foreach (string _switch in systemdata.Switches)
                    {
                        if (string.IsNullOrEmpty(_switch))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
                        }
                    }
                }
                if (systemdata.Switches == null || systemdata.Switches.Length < 1)
                {

                }
                else
                {
                    foreach (string _switch in systemdata.Switches)
                    {
                        if (string.IsNullOrEmpty(_switch))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
                        }
                    }
                }
                if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
                {

                }
                else
                {
                    foreach (string weaponType in systemdata.WeaponTypes)
                    {
                        if (string.IsNullOrEmpty(weaponType))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
                        }
                    }
                }
                if (systemdata.Terms == null)
                {

                }
                else
                {
                    foreach (var basic in systemdata.Terms.Basic)
                    {
                        if (string.IsNullOrEmpty(basic))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
                        }
                    }
                    foreach (var command in systemdata.Terms.Commands)
                    {
                        if (string.IsNullOrEmpty(command))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
                        }
                    }

                    foreach (string param in systemdata.Terms.Params)
                    {
                        if (string.IsNullOrEmpty(param))
                        {

                        }
                        else
                        {
                            Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
                        }
                    }

                    foreach (var Message in systemdata.Terms.Messages)
                    {
                        Main.THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
                    }
                }
                //FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
                //THLog = "";

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
