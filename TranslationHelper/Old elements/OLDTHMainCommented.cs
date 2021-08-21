//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TranslationHelper.OldElements
//{
//    class OLDTHMainCommented
//    {

//        private void THMakeRPGMakerMVWorkProjectDir(string sPath)
//        {
//            string outdir = Path.Combine(Application.StartupPath, "Work", "RPGMakerMV", Path.GetFileNameWithoutExtension(Path.GetDirectoryName(sPath)));

//            if (!Directory.Exists(outdir))
//            {
//                Directory.CreateDirectory(outdir);


//                foreach (var d in Directory.GetDirectories(THSelectedDir, "*"))
//                {
//                    if (d.Contains("\\www"))
//                    {
//                        continue;
//                    }
//                    if (Directory.Exists(Path.Combine(outdir, Path.GetFileNameWithoutExtension(d))))
//                    {
//                    }
//                    else
//                    {
//                        THCreateSymlink.Folder(d, Path.Combine(outdir, Path.GetFileNameWithoutExtension(d)));
//                    }
//                }
//                Directory.CreateDirectory(Path.Combine(outdir, "www"));
//                foreach (var d in Directory.GetDirectories(Path.Combine(THSelectedDir, "www"), "*"))
//                {
//                    if (d.Contains("www\\data"))
//                    {
//                        continue;
//                    }
//                    if (Directory.Exists(Path.Combine(outdir, "www", Path.GetFileNameWithoutExtension(d))))
//                    {
//                    }
//                    else
//                    {
//                        THCreateSymlink.Folder(d, Path.Combine(outdir, "www", Path.GetFileNameWithoutExtension(d)));
//                    }
//                }
//                foreach (var f in Directory.GetFiles(THSelectedDir, "*.*"))
//                {
//                    if (File.Exists(Path.Combine(outdir, Path.GetFileName(f))))
//                    {
//                    }
//                    else
//                    {
//                        THCreateSymlink.File(f, Path.Combine(outdir, Path.GetFileName(f)));
//                    }
//                }

//                CopyFolder.Copy(Path.Combine(THSelectedDir, "www", "data"), Path.Combine(outdir, "www", "data"));
//            }
//            THWorkProjectDir = outdir;
//        }


//        private void GetDataFromRPGMakerMVjsonOfType(string Jsonname, string JsonElement)
//        {
//            if (string.IsNullOrEmpty(JsonElement) || SelectedLocalePercentFromStringIsNotValid(JsonElement) || GetAlreadyAddedInTable(Jsonname, JsonElement))
//            {
//            }
//            else
//            {
//                THFilesElementsDataset.Tables[Jsonname].Rows.Add(JsonElement);
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonItemsArmorsWeapons(string Jsonname, string jsondata)
//        {
//            try
//            {
//                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonItemsArmorsWeapons>>(jsondata))
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonSkills(string Jsonname, string jsondata)
//        {
//            try
//            {
//                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonSkills>>(jsondata))
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message1);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message2);
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonStates(string Jsonname, string jsondata)
//        {
//            try
//            {
//                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonStates>>(jsondata))
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message1);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message2);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message3);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message4);
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonClassesEnemiesTilesets(string Jsonname, string jsondata)
//        {
//            try
//            {
//                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonClassesEnemiesTilesets>>(jsondata))
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(string Jsonname, string jsondata)
//        {
//            try
//            {
//                foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonAnimationsMapInfosTroops>>(jsondata))
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonActors(string Jsonname, string jsondata)
//        {
//            try
//            {
//                var actors = RPGMakerMVjsonActors.FromJson(jsondata);//JsonConvert.DeserializeObject<List<RPGMakerMVjsonActors>>(jsondata)
//                foreach (var JsonElement in actors)
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Nickname);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                        GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Profile);
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonCommonEvents(string Jsonname, string jsondata)
//        {
//            try
//            {
//                //info RPG Maker MV Event codes
//                //https://forums.rpgmakerweb.com/index.php?threads/extract-events-to-text-file.17444/
//                //https://forums.rpgmakerweb.com/index.php?threads/cross-reference-tool.72563/
//                //https://pastebin.com/JyRTdq0b
//                //https://pastebin.com/eJx0EvXB
//                //    case 401 : return 'Show Text';              break;
//                //    case 102 : return 'Show Choices';           break;
//                //    case 103 : return 'Input Number';           break;
//                //    case 104 : return 'Select Item';            break;
//                //    case 405 : return 'Show Scrolling Text';    break;
//                //    case 111 : return 'Conditional Branch';     break;
//                //    case 119 : return 'Common Event';           break;
//                //    case 121 : return 'Control Switches';       break;
//                //    case 122 : return 'Control Variables';      break;
//                //    case 125 : return 'Change Gold';            break;
//                //    case 126 : return 'Change Items';           break;
//                //    case 127 : return 'Change Weapons';         break;
//                //    case 128 : return 'Change Armors';          break;
//                //    case 129 : return 'Change Party Member';    break;
//                //    case 201 : return 'Transfer Player';        break;
//                //    case 202 : return 'Set Vehicle Location';   break;
//                //    case 203 : return 'Set Event Location';     break;
//                //    case 505 : return 'Set Movement Route';     break;
//                //    case 212 : return 'Show Animation';         break;
//                //    case 231 : return 'Show Picture';           break;
//                //    case 232 : return 'Move Picture';           break;
//                //    case 285 : return 'Get Location Info';      break;
//                //    case 301 : return 'Battle Processing';      break;
//                //    case 302 :
//                //    case 605 : return 'Shop Processing';        break;
//                //    case 303 : return 'Name Input Processing';  break;
//                //    case 311 : return 'Change HP';              break;
//                //    case 312 : return 'Change MP';              break;
//                //    case 326 : return 'Change TP';              break;
//                //    case 313 : return 'Change State';           break;
//                //    case 314 : return 'Recover All';            break;
//                //    case 315 : return 'Change EXP';             break;
//                //    case 316 : return 'Change Level';           break;
//                //    case 317 : return 'Change Parameter';       break;
//                //    case 318 : return 'Change Skill';           break;
//                //    case 319 : return 'Change Equipment';       break;
//                //    case 320 : return 'Change Name';            break;
//                //    case 321 : return 'Change Class';           break;
//                //    case 322 : return 'Change Actor Images';    break;
//                //    case 324 : return 'Change Nickname';        break;
//                //    case 325 : return 'Change Profile';         break;
//                //    case 331 : return 'Change Enemy HP';        break;
//                //    case 332 : return 'Change Enemy MP';        break;
//                //    case 342 : return 'Change Enemy TP';        break;
//                //    case 333 : return 'Change Enemy State';     break;
//                //    case 336 : return 'Enemy Transform';        break;
//                //    case 337 : return 'Show Battle Animation';  break;
//                //    case 339 : return 'Force Action';           break;
//                //
//                //Will be handled:
//                //401 - Show text (mergeable)
//                //102 - Show choices (Choices list)
//                //402 - Choice for choices - ignore because already in 102
//                //405 - Show Scrolling Text (mergeable)
//                //108 and 408 - Comment - can be ignored because it is for dev suppose
//                //normal example about command values adding: https://galvs-scripts.com/galvs-party-select/


//                //var commoneventsdata = JsonConvert.DeserializeObject<List<RPGMakerMVjsonCommonEvents>>(jsondata);
//                var commoneventsdata = RpgMakerMVjsonCommonEvents.FromJson(jsondata);

//                for (int i = 1; i < commoneventsdata.Count; i++)
//                {
//                    //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

//                    //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
//                    //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

//                    string eventname = commoneventsdata[i].Name;
//                    if (string.IsNullOrEmpty(eventname) || GetAlreadyAddedInTable(Jsonname, eventname))
//                    {
//                    }
//                    else //if code not equal old code and newline is not empty
//                    {
//                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(eventname); //add event name to new row
//                    }

//                    string newline = string.Empty;
//                    //int commandcode;
//                    //int commandoldcode = 999999;
//                    bool textaddingstarted = false;


//                    int CommandsCount = commoneventsdata[i].List.Count;
//                    for (int c = 0; c < CommandsCount; c++)
//                    {
//                        if (textaddingstarted)
//                        {
//                            if (commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
//                            {
//                                newline += commoneventsdata[i].List[c].Parameters[0].String;

//                                if (c < CommandsCount - 1 && commoneventsdata[i].List[c].Code == commoneventsdata[i].List[c + 1].Code)
//                                {
//                                    newline += "\r\n";
//                                }
//                                else
//                                {
//                                    if (string.IsNullOrEmpty(newline))
//                                    {
//                                        if (textaddingstarted)
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted is true and newline is empty\r\n";
//                                            textaddingstarted = false;
//                                        }
//                                    }
//                                    else //if code not equal old code and newline is not empty
//                                    {
//                                        if (GetAlreadyAddedInTable(Jsonname, newline))
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 401/405 newline already in table=\"" + newline + "\"\r\n";
//                                            newline = string.Empty; //clear text data
//                                            if (textaddingstarted)
//                                            {
//                                                textaddingstarted = false;
//                                            }
//                                        }
//                                        else
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted=true added newline=\"" + newline + "\"\r\n";
//                                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
//                                            newline = string.Empty; //clear text data
//                                            textaddingstarted = false;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        else if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105)
//                        {
//                            if (string.IsNullOrEmpty(newline))
//                            {
//                                if (textaddingstarted)
//                                {
//                                    //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline is empty\r\n";
//                                    textaddingstarted = false;
//                                }
//                            }
//                            else //if code not equal old code and newline is not empty
//                            {
//                                if (GetAlreadyAddedInTable(Jsonname, newline))
//                                {
//                                    if (textaddingstarted)
//                                    {
//                                        //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline already in table=\"" + newline + "\"\r\n";
//                                        textaddingstarted = false;
//                                    }
//                                }
//                                else
//                                {
//                                    //THLog += DateTime.Now + " >>: Code 101/105 newline is not empty=\"" + newline + "\"\r\n";
//                                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
//                                    newline = string.Empty; //clear text data
//                                    textaddingstarted = false;
//                                }
//                            }

//                            textaddingstarted = true;
//                        }
//                        else if (commoneventsdata[i].List[c].Code == 102)
//                        {
//                            for (int i1 = 0; i1 < commoneventsdata[i].List[c].Parameters[0].AnythingArray.Count; i1++)
//                            {
//                                if (string.IsNullOrEmpty(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String))
//                                {
//                                    if (textaddingstarted)
//                                    {
//                                        //THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
//                                        textaddingstarted = false;
//                                    }
//                                }
//                                else //if code not equal old code and newline is not empty
//                                {
//                                    if (GetAlreadyAddedInTable(Jsonname, commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String))
//                                    {
//                                        //THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
//                                        if (textaddingstarted)
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
//                                            textaddingstarted = false;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        //THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
//                                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String); //Save text to new row
//                                        if (string.IsNullOrEmpty(newline))
//                                        {
//                                        }
//                                        else
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 102 added schoice and also newline is not empty, set empty\r\n";
//                                            newline = string.Empty; //clear text data
//                                        }
//                                        if (textaddingstarted)
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 102 added schoice and also textaddingstarted is true , set false\r\n";
//                                            textaddingstarted = false;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonMap(string Jsonname, string jsondata)
//        {
//            try
//            {
//                //bool eventsdone = false;
//                //bool geteventnamenotedone = false;

//                var map = JsonConvert.DeserializeObject<RPGMakerMVjsonMap>(jsondata);

//                if (map.Events.Length > 1) //first event is empty
//                {
//                    //Map displayed name
//                    if (string.IsNullOrEmpty(map.DisplayName) || SelectedLocalePercentFromStringIsNotValid(map.DisplayName) || GetAlreadyAddedInTable(Jsonname, map.DisplayName))
//                    {
//                    }
//                    else
//                    {
//                        //MessageBox.Show("map.DisplayName:" + map.DisplayName);
//                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.DisplayName);
//                    }
//                    //Map note
//                    if (string.IsNullOrEmpty(map.Note) || SelectedLocalePercentFromStringIsNotValid(map.Note) || GetAlreadyAddedInTable(Jsonname, map.Note))
//                    {
//                    }
//                    else
//                    {
//                        //MessageBox.Show("map.Note:" + map.Note);
//                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.Note);
//                    }

//                    //string prevval = string.Empty;
//                    foreach (RPGMakerMVjsonMapEvent ev in map.Events)
//                    {
//                        if (ev == null)
//                        {
//                        }
//                        else
//                        {
//                            //event name
//                            if (string.IsNullOrEmpty(ev.Name) || ev.Name.StartsWith("EV") || SelectedLocalePercentFromStringIsNotValid(ev.Name) || GetAlreadyAddedInTable(Jsonname, ev.Name))
//                            {
//                            }
//                            else
//                            {
//                                //MessageBox.Show("map.Events add name"+ ev.Name);
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Name);
//                                //prevval = ev.Name;
//                            }
//                            //event note
//                            if (string.IsNullOrEmpty(ev.Note) || SelectedLocalePercentFromStringIsNotValid(ev.Note) || GetAlreadyAddedInTable(Jsonname, ev.Note))
//                            {
//                            }
//                            else
//                            {
//                                //MessageBox.Show("map.Events add note:" + ev.Note);
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Note);
//                            }

//                            //event parameters
//                            foreach (RPGMakerMVjsonMapPage page in ev.Pages)
//                            {
//                                foreach (RPGMakerMVjsonMapPageList lst in page.List)
//                                {
//                                    foreach (var parameter in lst.Parameters)
//                                    {
//                                        if (parameter == null)
//                                        {

//                                        }
//                                        else if (parameter.GetType().Name == "String")
//                                        {
//                                            string pstring = parameter.ToString();
//                                            if (string.IsNullOrEmpty(pstring) || HasNOJPcharacters(pstring) || SelectedLocalePercentFromStringIsNotValid(pstring) || GetAlreadyAddedInTable(Jsonname, pstring))
//                                            {

//                                            }
//                                            else
//                                            {
//                                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(pstring);
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool GetDataFromRPGMakerMVjsonSystem(string Jsonname, string jsondata)
//        {
//            try
//            {
//                //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
//                var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

//                //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

//                if (systemdata.GameTitle == null || string.IsNullOrEmpty(systemdata.GameTitle))
//                {
//                }
//                else
//                {
//                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);
//                }

//                if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
//                {
//                }
//                else
//                {
//                    foreach (string armortype in systemdata.ArmorTypes)
//                    {
//                        if (string.IsNullOrEmpty(armortype))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
//                        }
//                    }
//                }
//                if (systemdata.Elements == null || systemdata.Elements.Length < 1)
//                {

//                }
//                else
//                {
//                    foreach (string element in systemdata.Elements)
//                    {
//                        if (string.IsNullOrEmpty(element))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
//                        }
//                    }
//                }
//                if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
//                {

//                }
//                else
//                {
//                    foreach (string equipType in systemdata.EquipTypes)
//                    {
//                        if (string.IsNullOrEmpty(equipType))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
//                        }
//                    }
//                }
//                if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
//                {

//                }
//                else
//                {
//                    foreach (string skillType in systemdata.skillTypes)
//                    {
//                        if (string.IsNullOrEmpty(skillType))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
//                        }
//                    }
//                }
//                if (systemdata.Switches == null || systemdata.Switches.Length < 1)
//                {

//                }
//                else
//                {
//                    foreach (string _switch in systemdata.Switches)
//                    {
//                        if (string.IsNullOrEmpty(_switch))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
//                        }
//                    }
//                }
//                if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
//                {

//                }
//                else
//                {
//                    foreach (string weaponType in systemdata.WeaponTypes)
//                    {
//                        if (string.IsNullOrEmpty(weaponType))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
//                        }
//                    }
//                }
//                if (systemdata.Terms == null)
//                {

//                }
//                else
//                {
//                    foreach (var basic in systemdata.Terms.Basic)
//                    {
//                        if (string.IsNullOrEmpty(basic))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
//                        }
//                    }
//                    foreach (var command in systemdata.Terms.Commands)
//                    {
//                        if (string.IsNullOrEmpty(command))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
//                        }
//                    }

//                    foreach (string param in systemdata.Terms.Params)
//                    {
//                        if (string.IsNullOrEmpty(param))
//                        {

//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
//                        }
//                    }

//                    foreach (var Message in systemdata.Terms.Messages)
//                    {
//                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
//                    }
//                }
//                //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", THLog, true);
//                //THLog = string.Empty;

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool FillDSTableWithJsonValues(string Jsonname, string jsondata, bool name = false, bool description = false, bool displayname = false, bool note = false, bool message1 = false, bool message2 = false, bool message3 = false, bool message4 = false, bool nickname = false, bool profile = false, bool maps = false, bool cmnevents = false, bool system = false)
//        {
//            try
//            {
//                if (name)
//                {
//                    foreach (RPGMakerMVjsonName Name in JsonConvert.DeserializeObject<List<RPGMakerMVjsonName>>(jsondata))
//                    {
//                        if (Name == null || string.IsNullOrEmpty(Name.Name) || SelectedLocalePercentFromStringIsNotValid(Name.Name) || GetAlreadyAddedInTable(Jsonname, Name.Name))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Name.Name);
//                        }
//                    }
//                }
//                if (description)
//                {
//                    foreach (RPGMakerMVjsonDescription Description in JsonConvert.DeserializeObject<List<RPGMakerMVjsonDescription>>(jsondata))
//                    {
//                        if (Description == null || string.IsNullOrEmpty(Description.Description) || SelectedLocalePercentFromStringIsNotValid(Description.Description) || GetAlreadyAddedInTable(Jsonname, Description.Description))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Description.Description);
//                        }
//                    }
//                }
//                if (displayname)
//                {
//                    foreach (RPGMakerMVjsonDisplayName DisplayName in JsonConvert.DeserializeObject<List<RPGMakerMVjsonDisplayName>>(jsondata))
//                    {
//                        if (DisplayName == null || string.IsNullOrEmpty(DisplayName.DisplayName) || SelectedLocalePercentFromStringIsNotValid(DisplayName.DisplayName) || GetAlreadyAddedInTable(Jsonname, DisplayName.DisplayName))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(DisplayName.DisplayName);
//                        }
//                    }
//                }
//                if (note)
//                {
//                    foreach (RPGMakerMVjsonNote Note in JsonConvert.DeserializeObject<List<RPGMakerMVjsonNote>>(jsondata))
//                    {
//                        if (Note == null || string.IsNullOrEmpty(Note.Note) || SelectedLocalePercentFromStringIsNotValid(Note.Note) || GetAlreadyAddedInTable(Jsonname, Note.Note))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Note.Note);
//                        }
//                    }
//                }
//                if (message1)
//                {
//                    foreach (RPGMakerMVjsonMessage1 Message1 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage1>>(jsondata))
//                    {
//                        if (Message1 == null || string.IsNullOrEmpty(Message1.Message1) || SelectedLocalePercentFromStringIsNotValid(Message1.Message1) || GetAlreadyAddedInTable(Jsonname, Message1.Message1))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message1.Message1);
//                        }
//                    }
//                }
//                if (message2)
//                {
//                    foreach (RPGMakerMVjsonMessage2 Message2 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage2>>(jsondata))
//                    {
//                        if (Message2 == null || string.IsNullOrEmpty(Message2.Message2) || SelectedLocalePercentFromStringIsNotValid(Message2.Message2) || GetAlreadyAddedInTable(Jsonname, Message2.Message2))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message2.Message2);
//                        }
//                    }
//                }
//                if (message3)
//                {
//                    foreach (RPGMakerMVjsonMessage3 Message3 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage3>>(jsondata))
//                    {
//                        if (Message3 == null || string.IsNullOrEmpty(Message3.Message3) || SelectedLocalePercentFromStringIsNotValid(Message3.Message3) || GetAlreadyAddedInTable(Jsonname, Message3.Message3))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message3.Message3);
//                        }
//                    }
//                }
//                if (message4)
//                {
//                    foreach (RPGMakerMVjsonMessage4 Message4 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage4>>(jsondata))
//                    {
//                        if (Message4 == null || string.IsNullOrEmpty(Message4.Message4) || SelectedLocalePercentFromStringIsNotValid(Message4.Message4) || GetAlreadyAddedInTable(Jsonname, Message4.Message4))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message4.Message4);
//                        }
//                    }
//                }
//                if (nickname)
//                {
//                    foreach (RPGMakerMVjsonNickname Nickname in JsonConvert.DeserializeObject<List<RPGMakerMVjsonNickname>>(jsondata))
//                    {
//                        if (Nickname == null || string.IsNullOrEmpty(Nickname.Nickname) || SelectedLocalePercentFromStringIsNotValid(Nickname.Nickname) || GetAlreadyAddedInTable(Jsonname, Nickname.Nickname))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Nickname.Nickname);
//                        }
//                    }
//                }
//                if (profile)
//                {
//                    foreach (RPGMakerMVjsonProfile Profile in JsonConvert.DeserializeObject<List<RPGMakerMVjsonProfile>>(jsondata))
//                    {
//                        if (Profile == null || string.IsNullOrEmpty(Profile.Profile) || SelectedLocalePercentFromStringIsNotValid(Profile.Profile) || GetAlreadyAddedInTable(Jsonname, Profile.Profile))
//                        {
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Profile.Profile);
//                        }
//                    }
//                }

//                if (cmnevents)
//                {
//                    //info RPG Maker MV Event codes
//                    //https://forums.rpgmakerweb.com/index.php?threads/extract-events-to-text-file.17444/
//                    //https://forums.rpgmakerweb.com/index.php?threads/cross-reference-tool.72563/
//                    //https://pastebin.com/JyRTdq0b
//                    //https://pastebin.com/eJx0EvXB
//                    //    case 401 : return 'Show Text';              break;
//                    //    case 102 : return 'Show Choices';           break;
//                    //    case 103 : return 'Input Number';           break;
//                    //    case 104 : return 'Select Item';            break;
//                    //    case 405 : return 'Show Scrolling Text';    break;
//                    //    case 111 : return 'Conditional Branch';     break;
//                    //    case 119 : return 'Common Event';           break;
//                    //    case 121 : return 'Control Switches';       break;
//                    //    case 122 : return 'Control Variables';      break;
//                    //    case 125 : return 'Change Gold';            break;
//                    //    case 126 : return 'Change Items';           break;
//                    //    case 127 : return 'Change Weapons';         break;
//                    //    case 128 : return 'Change Armors';          break;
//                    //    case 129 : return 'Change Party Member';    break;
//                    //    case 201 : return 'Transfer Player';        break;
//                    //    case 202 : return 'Set Vehicle Location';   break;
//                    //    case 203 : return 'Set Event Location';     break;
//                    //    case 505 : return 'Set Movement Route';     break;
//                    //    case 212 : return 'Show Animation';         break;
//                    //    case 231 : return 'Show Picture';           break;
//                    //    case 232 : return 'Move Picture';           break;
//                    //    case 285 : return 'Get Location Info';      break;
//                    //    case 301 : return 'Battle Processing';      break;
//                    //    case 302 :
//                    //    case 605 : return 'Shop Processing';        break;
//                    //    case 303 : return 'Name Input Processing';  break;
//                    //    case 311 : return 'Change HP';              break;
//                    //    case 312 : return 'Change MP';              break;
//                    //    case 326 : return 'Change TP';              break;
//                    //    case 313 : return 'Change State';           break;
//                    //    case 314 : return 'Recover All';            break;
//                    //    case 315 : return 'Change EXP';             break;
//                    //    case 316 : return 'Change Level';           break;
//                    //    case 317 : return 'Change Parameter';       break;
//                    //    case 318 : return 'Change Skill';           break;
//                    //    case 319 : return 'Change Equipment';       break;
//                    //    case 320 : return 'Change Name';            break;
//                    //    case 321 : return 'Change Class';           break;
//                    //    case 322 : return 'Change Actor Images';    break;
//                    //    case 324 : return 'Change Nickname';        break;
//                    //    case 325 : return 'Change Profile';         break;
//                    //    case 331 : return 'Change Enemy HP';        break;
//                    //    case 332 : return 'Change Enemy MP';        break;
//                    //    case 342 : return 'Change Enemy TP';        break;
//                    //    case 333 : return 'Change Enemy State';     break;
//                    //    case 336 : return 'Enemy Transform';        break;
//                    //    case 337 : return 'Show Battle Animation';  break;
//                    //    case 339 : return 'Force Action';           break;
//                    //
//                    //Will be handled:
//                    //401 - Show text (mergeable)
//                    //102 - Show choices (Choices list)
//                    //402 - Choice for choices - ignore because already in 102
//                    //405 - Show Scrolling Text (mergeable)
//                    //108 and 408 - Comment - can be ignored because it is for dev suppose
//                    //normal example about command values adding: https://galvs-scripts.com/galvs-party-select/


//                    //var commoneventsdata = JsonConvert.DeserializeObject<List<RPGMakerMVjsonCommonEvents>>(jsondata);
//                    var commoneventsdata = RpgMakerMVjsonCommonEvents.FromJson(jsondata);

//                    for (int i = 1; i < commoneventsdata.Count; i++)
//                    {
//                        //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

//                        //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
//                        //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

//                        string eventname = commoneventsdata[i].Name;
//                        if (string.IsNullOrEmpty(eventname) || GetAlreadyAddedInTable(Jsonname, eventname))
//                        {
//                        }
//                        else //if code not equal old code and newline is not empty
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(eventname); //add event name to new row
//                        }

//                        string newline = string.Empty;
//                        //int commandcode;
//                        //int commandoldcode = 999999;
//                        bool textaddingstarted = false;


//                        int CommandsCount = commoneventsdata[i].List.Count;
//                        for (int c = 0; c < CommandsCount; c++)
//                        {
//                            if (textaddingstarted)
//                            {
//                                if (commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
//                                {
//                                    newline += commoneventsdata[i].List[c].Parameters[0];

//                                    if (c < CommandsCount - 1 && commoneventsdata[i].List[c].Code == commoneventsdata[i].List[c + 1].Code)
//                                    {
//                                        newline += "\r\n";
//                                    }
//                                    else
//                                    {
//                                        if (string.IsNullOrEmpty(newline))
//                                        {
//                                            if (textaddingstarted)
//                                            {
//                                                //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted is true and newline is empty\r\n";
//                                                textaddingstarted = false;
//                                            }
//                                        }
//                                        else //if code not equal old code and newline is not empty
//                                        {
//                                            if (GetAlreadyAddedInTable(Jsonname, newline))
//                                            {
//                                                //THLog += DateTime.Now + " >>: Code 401/405 newline already in table=\"" + newline + "\"\r\n";
//                                                newline = string.Empty; //clear text data
//                                                if (textaddingstarted)
//                                                {
//                                                    textaddingstarted = false;
//                                                }
//                                            }
//                                            else
//                                            {
//                                                //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted=true added newline=\"" + newline + "\"\r\n";
//                                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
//                                                newline = string.Empty; //clear text data
//                                                textaddingstarted = false;
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                            else if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105)
//                            {
//                                if (string.IsNullOrEmpty(newline))
//                                {
//                                    if (textaddingstarted)
//                                    {
//                                        //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline is empty\r\n";
//                                        textaddingstarted = false;
//                                    }
//                                }
//                                else //if code not equal old code and newline is not empty
//                                {
//                                    if (GetAlreadyAddedInTable(Jsonname, newline))
//                                    {
//                                        if (textaddingstarted)
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline already in table=\"" + newline + "\"\r\n";
//                                            textaddingstarted = false;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        //THLog += DateTime.Now + " >>: Code 101/105 newline is not empty=\"" + newline + "\"\r\n";
//                                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
//                                        newline = string.Empty; //clear text data
//                                        textaddingstarted = false;
//                                    }
//                                }

//                                textaddingstarted = true;
//                            }
//                            else if (commoneventsdata[i].List[c].Code == 102)
//                            {
//                                JArray choices = JArray.Parse(commoneventsdata[i].List[c].Parameters[0].ToString());

//                                foreach (var choice in choices)
//                                {
//                                    string schoice = choice.ToString();
//                                    if (string.IsNullOrEmpty(schoice))
//                                    {
//                                        if (textaddingstarted)
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
//                                            textaddingstarted = false;
//                                        }
//                                    }
//                                    else //if code not equal old code and newline is not empty
//                                    {
//                                        if (GetAlreadyAddedInTable(Jsonname, schoice))
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
//                                            if (textaddingstarted)
//                                            {
//                                                //THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
//                                                textaddingstarted = false;
//                                            }
//                                        }
//                                        else
//                                        {
//                                            //THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
//                                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(schoice); //Save text to new row
//                                            if (string.IsNullOrEmpty(newline))
//                                            {
//                                            }
//                                            else
//                                            {
//                                                //THLog += DateTime.Now + " >>: Code 102 added schoice and also newline is not empty, set empty\r\n";
//                                                newline = string.Empty; //clear text data
//                                            }
//                                            if (textaddingstarted)
//                                            {
//                                                //THLog += DateTime.Now + " >>: Code 102 added schoice and also textaddingstarted is true , set false\r\n";
//                                                textaddingstarted = false;
//                                            }
//                                        }

//                                    }
//                                }
//                            }
//                        }
//                        //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", THLog, true);
//                        //THLog = string.Empty;
//                    }

//                }
//                if (maps)
//                {
//                    //bool eventsdone = false;
//                    //bool geteventnamenotedone = false;

//                    var map = JsonConvert.DeserializeObject<RPGMakerMVjsonMap>(jsondata);

//                    if (map.Events.Length > 1) //first event is empty
//                    {
//                        //Map displayed name
//                        if (string.IsNullOrEmpty(map.DisplayName) || SelectedLocalePercentFromStringIsNotValid(map.DisplayName) || GetAlreadyAddedInTable(Jsonname, map.DisplayName))
//                        {
//                        }
//                        else
//                        {
//                            //MessageBox.Show("map.DisplayName:" + map.DisplayName);
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.DisplayName);
//                        }
//                        //Map note
//                        if (string.IsNullOrEmpty(map.Note) || SelectedLocalePercentFromStringIsNotValid(map.Note) || GetAlreadyAddedInTable(Jsonname, map.Note))
//                        {
//                        }
//                        else
//                        {
//                            //MessageBox.Show("map.Note:" + map.Note);
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.Note);
//                        }

//                        //string prevval = string.Empty;
//                        foreach (RPGMakerMVjsonMapEvent ev in map.Events)
//                        {
//                            if (ev == null)
//                            {
//                            }
//                            else
//                            {
//                                //event name
//                                if (string.IsNullOrEmpty(ev.Name) || ev.Name.StartsWith("EV") || SelectedLocalePercentFromStringIsNotValid(ev.Name) || GetAlreadyAddedInTable(Jsonname, ev.Name))
//                                {
//                                }
//                                else
//                                {
//                                    //MessageBox.Show("map.Events add name"+ ev.Name);
//                                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Name);
//                                    //prevval = ev.Name;
//                                }
//                                //event note
//                                if (string.IsNullOrEmpty(ev.Note) || SelectedLocalePercentFromStringIsNotValid(ev.Note) || GetAlreadyAddedInTable(Jsonname, ev.Note))
//                                {
//                                }
//                                else
//                                {
//                                    //MessageBox.Show("map.Events add note:" + ev.Note);
//                                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Note);
//                                }

//                                //event parameters
//                                foreach (RPGMakerMVjsonMapPage page in ev.Pages)
//                                {
//                                    foreach (RPGMakerMVjsonMapPageList lst in page.List)
//                                    {
//                                        foreach (var parameter in lst.Parameters)
//                                        {
//                                            if (parameter == null)
//                                            {

//                                            }
//                                            else if (parameter.GetType().Name == "String")
//                                            {
//                                                string pstring = parameter.ToString();
//                                                if (string.IsNullOrEmpty(pstring) || HasNOJPcharacters(pstring) || SelectedLocalePercentFromStringIsNotValid(pstring) || GetAlreadyAddedInTable(Jsonname, pstring))
//                                                {

//                                                }
//                                                else
//                                                {
//                                                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(pstring);
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                if (system)
//                {
//                    //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
//                    var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

//                    //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

//                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);

//                    if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string armortype in systemdata.ArmorTypes)
//                        {
//                            if (string.IsNullOrEmpty(armortype))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
//                            }
//                        }
//                    }
//                    if (systemdata.Elements == null || systemdata.Elements.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string element in systemdata.Elements)
//                        {
//                            if (string.IsNullOrEmpty(element))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
//                            }
//                        }
//                    }
//                    if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string equipType in systemdata.EquipTypes)
//                        {
//                            if (string.IsNullOrEmpty(equipType))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
//                            }
//                        }
//                    }
//                    if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string skillType in systemdata.skillTypes)
//                        {
//                            if (string.IsNullOrEmpty(skillType))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
//                            }
//                        }
//                    }
//                    if (systemdata.Switches == null || systemdata.Switches.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string _switch in systemdata.Switches)
//                        {
//                            if (string.IsNullOrEmpty(_switch))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
//                            }
//                        }
//                    }
//                    if (systemdata.Switches == null || systemdata.Switches.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string _switch in systemdata.Switches)
//                        {
//                            if (string.IsNullOrEmpty(_switch))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
//                            }
//                        }
//                    }
//                    if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
//                    {

//                    }
//                    else
//                    {
//                        foreach (string weaponType in systemdata.WeaponTypes)
//                        {
//                            if (string.IsNullOrEmpty(weaponType))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
//                            }
//                        }
//                    }
//                    if (systemdata.Terms == null)
//                    {

//                    }
//                    else
//                    {
//                        foreach (var basic in systemdata.Terms.Basic)
//                        {
//                            if (string.IsNullOrEmpty(basic))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
//                            }
//                        }
//                        foreach (var command in systemdata.Terms.Commands)
//                        {
//                            if (string.IsNullOrEmpty(command))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
//                            }
//                        }

//                        foreach (string param in systemdata.Terms.Params)
//                        {
//                            if (string.IsNullOrEmpty(param))
//                            {

//                            }
//                            else
//                            {
//                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
//                            }
//                        }

//                        foreach (var Message in systemdata.Terms.Messages)
//                        {
//                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
//                        }
//                    }
//                    //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", THLog, true);
//                    //THLog = string.Empty;
//                }


//                return true;
//            }
//            catch (Exception ex)
//            {
//                FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", ex.Message, true);
//                return false;
//            }
//        }

//        private string TestSaveGetDataFromRPGMakerMVjsonOfType(string Jsonname, string JsonElement)
//        {
//            if (string.IsNullOrEmpty(JsonElement) || SelectedLocalePercentFromStringIsNotValid(JsonElement))
//            {
//            }
//            else
//            {
//                for (int i = 0; i < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i++)
//                {
//                    if (THFilesElementsDataset.Tables[Jsonname].Rows[i][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString()))
//                    {

//                    }
//                    else
//                    {
//                        if (JsonElement == THFilesElementsDataset.Tables[Jsonname].Rows[i][0].ToString())
//                        {
//                            return THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString();
//                        }
//                    }
//                }
//                //THFilesElementsDataset.Tables[Jsonname].Rows[i][1];
//                //THFilesElementsDataset.Tables[Jsonname].Rows.Add(JsonElement);
//            }
//            return string.Empty;
//        }

//        private bool TestSaveGetDataFromRPGMakerMVjsonItemsArmorsWeapons(string Jsonname, string jsondata)
//        {
//            try
//            {
//                var testjsonforwrite = JsonConvert.DeserializeObject<List<RPGMakerMVjsonItemsArmorsWeapons>>(jsondata);
//                foreach (var JsonElement in testjsonforwrite)
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        string Name = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        if (string.IsNullOrEmpty(Name))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Name = Name;
//                        }
//                        string Description = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
//                        if (string.IsNullOrEmpty(Description))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Description = Description;
//                        }
//                        string Note = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                        if (string.IsNullOrEmpty(Note))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Note = Note;
//                        }
//                    }
//                }

//                string s = JsonConvert.SerializeObject(testjsonforwrite);
//                File.WriteAllText(@"C:\\000 test RPGMaker MV data\\Armors1.json", s);
//                MessageBox.Show("test write finished");
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool TestSaveGetDataFromRPGMakerMVjsonSystem(string Jsonname, string jsondata)
//        {
//            try
//            {
//                //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
//                var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

//                //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

//                if (systemdata.GameTitle == null || string.IsNullOrEmpty(systemdata.GameTitle))
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i++)
//                    {
//                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString()))
//                        {
//                        }
//                        else
//                        {
//                            if (systemdata.GameTitle == THFilesElementsDataset.Tables[Jsonname].Rows[i][0].ToString())
//                            {
//                                systemdata.GameTitle = THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString();
//                                break;
//                            }
//                        }
//                    }
//                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);
//                }

//                if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.ArmorTypes.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.ArmorTypes[i]))
//                        {

//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.ArmorTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.ArmorTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
//                        }
//                    }
//                }
//                if (systemdata.Elements == null || systemdata.Elements.Length < 1)
//                {

//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.Elements.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.Elements[i]))
//                        {

//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.Elements[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.Elements[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
//                        }
//                    }
//                }
//                if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.EquipTypes.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.EquipTypes[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.EquipTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.EquipTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
//                        }
//                    }
//                }
//                if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.skillTypes.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.skillTypes[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.skillTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.skillTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
//                        }
//                    }
//                }
//                if (systemdata.Switches == null || systemdata.Switches.Length < 1)
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.Switches.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.Switches[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.Switches[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.Switches[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
//                        }
//                    }
//                }
//                if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.WeaponTypes.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.WeaponTypes[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.WeaponTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.WeaponTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
//                        }
//                    }
//                }
//                if (systemdata.Terms == null)
//                {
//                }
//                else
//                {
//                    for (int i = 0; i < systemdata.Terms.Basic.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.Terms.Basic[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.Terms.Basic[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.Terms.Basic[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
//                        }
//                    }
//                    for (int i = 0; i < systemdata.Terms.Commands.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.Terms.Commands[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.Terms.Commands[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.Terms.Commands[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
//                        }
//                    }

//                    for (int i = 0; i < systemdata.Terms.Params.Length; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.Terms.Params[i]))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.Terms.Params[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.Terms.Params[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
//                        }
//                    }

//                    //http://www.cyberforum.ru/csharp-beginners/thread785914.html
//                    for (int i = 0; i < systemdata.Terms.Messages.Count; i++)
//                    {
//                        if (string.IsNullOrEmpty(systemdata.Terms.Messages.ElementAt(i).Value))
//                        {
//                        }
//                        else
//                        {
//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (systemdata.Terms.Messages.ElementAt(i).Value == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                    {
//                                        systemdata.Terms.Messages[systemdata.Terms.Messages.ElementAt(i).Key] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                        //THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
//                    }
//                }
//                //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", THLog, true);
//                //THLog = string.Empty;

//                //var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);
//                string s = RPGMakerMVjsonSystemTo.ToJson(systemdata);
//                File.WriteAllText(@"C:\\000 test RPGMaker MV data\\System1.json", s);
//                MessageBox.Show("test write finished");
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool TestSaveGetDataFromRPGMakerMVjsonActors(string Jsonname, string jsondata)
//        {
//            try
//            {
//                var actors = RPGMakerMVjsonActors.FromJson(jsondata);//JsonConvert.DeserializeObject<List<RPGMakerMVjsonActors>>(jsondata)
//                foreach (var JsonElement in actors)
//                {
//                    if (JsonElement == null)
//                    {
//                    }
//                    else
//                    {
//                        string Name = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
//                        if (string.IsNullOrEmpty(Name))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Name = Name;
//                        }
//                        string Nickname = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Nickname);
//                        if (string.IsNullOrEmpty(Nickname))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Nickname = Nickname;
//                        }
//                        string Note = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
//                        if (string.IsNullOrEmpty(Note))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Note = Note;
//                        }
//                        string Profile = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Profile);
//                        if (string.IsNullOrEmpty(Profile))
//                        {
//                        }
//                        else
//                        {
//                            JsonElement.Profile = Profile;
//                        }
//                    }
//                }

//                string s = RPGMakerMVjsonActorsTo.ToJson(actors);
//                File.WriteAllText(@"C:\\000 test RPGMaker MV data\\Actors1.json", s);
//                MessageBox.Show("test write finished");

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        private bool TestSaveGetDataFromRPGMakerMVjsonCommonEvents(string Jsonname, string jsondata)
//        {
//            //info RPG Maker MV Event codes
//            //https://forums.rpgmakerweb.com/index.php?threads/extract-events-to-text-file.17444/
//            //https://forums.rpgmakerweb.com/index.php?threads/cross-reference-tool.72563/
//            //https://pastebin.com/JyRTdq0b
//            //https://pastebin.com/eJx0EvXB
//            //    case 401 : return 'Show Text';              break;
//            //    case 102 : return 'Show Choices';           break;
//            //    case 103 : return 'Input Number';           break;
//            //    case 104 : return 'Select Item';            break;
//            //    case 405 : return 'Show Scrolling Text';    break;
//            //    case 111 : return 'Conditional Branch';     break;
//            //    case 119 : return 'Common Event';           break;
//            //    case 121 : return 'Control Switches';       break;
//            //    case 122 : return 'Control Variables';      break;
//            //    case 125 : return 'Change Gold';            break;
//            //    case 126 : return 'Change Items';           break;
//            //    case 127 : return 'Change Weapons';         break;
//            //    case 128 : return 'Change Armors';          break;
//            //    case 129 : return 'Change Party Member';    break;
//            //    case 201 : return 'Transfer Player';        break;
//            //    case 202 : return 'Set Vehicle Location';   break;
//            //    case 203 : return 'Set Event Location';     break;
//            //    case 505 : return 'Set Movement Route';     break;
//            //    case 212 : return 'Show Animation';         break;
//            //    case 231 : return 'Show Picture';           break;
//            //    case 232 : return 'Move Picture';           break;
//            //    case 285 : return 'Get Location Info';      break;
//            //    case 301 : return 'Battle Processing';      break;
//            //    case 302 :
//            //    case 605 : return 'Shop Processing';        break;
//            //    case 303 : return 'Name Input Processing';  break;
//            //    case 311 : return 'Change HP';              break;
//            //    case 312 : return 'Change MP';              break;
//            //    case 326 : return 'Change TP';              break;
//            //    case 313 : return 'Change State';           break;
//            //    case 314 : return 'Recover All';            break;
//            //    case 315 : return 'Change EXP';             break;
//            //    case 316 : return 'Change Level';           break;
//            //    case 317 : return 'Change Parameter';       break;
//            //    case 318 : return 'Change Skill';           break;
//            //    case 319 : return 'Change Equipment';       break;
//            //    case 320 : return 'Change Name';            break;
//            //    case 321 : return 'Change Class';           break;
//            //    case 322 : return 'Change Actor Images';    break;
//            //    case 324 : return 'Change Nickname';        break;
//            //    case 325 : return 'Change Profile';         break;
//            //    case 331 : return 'Change Enemy HP';        break;
//            //    case 332 : return 'Change Enemy MP';        break;
//            //    case 342 : return 'Change Enemy TP';        break;
//            //    case 333 : return 'Change Enemy State';     break;
//            //    case 336 : return 'Enemy Transform';        break;
//            //    case 337 : return 'Show Battle Animation';  break;
//            //    case 339 : return 'Force Action';           break;
//            //
//            //Will be handled:
//            //401 - Show text (mergeable)
//            //102 - Show choices (Choices list)
//            //402 - Choice for choices - ignore because already in 102
//            //405 - Show Scrolling Text (mergeable)
//            //108 and 408 - Comment - can be ignored because it is for dev suppose
//            //normal example about command values adding: https://galvs-scripts.com/galvs-party-select/


//            //var commoneventsdata = JsonConvert.DeserializeObject<List<RPGMakerMVjsonCommonEvents>>(jsondata);
//            var commoneventsdata = RpgMakerMVjsonCommonEvents.FromJson(jsondata);

//            for (int i = 1; i < commoneventsdata.Count; i++)
//            {
//                //FileWriter.WriteData(Application.StartupPath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

//                //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
//                //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

//                //string eventname = commoneventsdata[i].Name;
//                if (string.IsNullOrEmpty(commoneventsdata[i].Name))
//                {
//                }
//                else //if code not equal old code and newline is not empty
//                {
//                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                    {
//                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                        {
//                        }
//                        else
//                        {
//                            if (commoneventsdata[i].Name == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                            {
//                                commoneventsdata[i].Name = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                break;
//                            }
//                        }
//                    }
//                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].Name); //add event name to new row
//                }

//                for (int c = 0; c < commoneventsdata[i].List.Count; c++)
//                {
//                    if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105 || commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
//                    {
//                        string parameter0value = commoneventsdata[i].List[c].Parameters[0].String;
//                        if (string.IsNullOrEmpty(parameter0value))
//                        {
//                        }
//                        else //if code not equal old code and newline is not empty
//                        {
//                            //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

//                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
//                                    //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp
//                                    string[] origA = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString().Split('\n').Where(emptyvalues => !string.IsNullOrEmpty(emptyvalues.Replace("\r", string.Empty))).ToArray();//Все строки, кроме пустых, чтобы потом исключить из проверки
//                                    int origALength = origA.Length;
//                                    if (origALength == 0)
//                                    {
//                                        origA = new string[1];
//                                        origA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString();
//                                        //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
//                                    }

//                                    if (origALength > 0)
//                                    {
//                                        string[] transA = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString().Split('\n').Where(emptyvalues => !string.IsNullOrEmpty(emptyvalues.Replace("\r", string.Empty))).ToArray();//Все строки, кроме пустых
//                                        if (transA.Length == 0)
//                                        {
//                                            transA = new string[1];
//                                            transA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                            //LogToFile("(transA.Length == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString());
//                                        }
//                                        string transmerged = string.Empty;
//                                        if (transA.Length == origALength)//если количество строк в оригинале и переводе равно
//                                        {
//                                            //ничего не делать
//                                        }
//                                        else // если перевод вдруг был переведен так, что не равен количеством строк оригиналу, тогда поделить его на равные строки
//                                        {
//                                            if (transA.Length > 0) // но перед этим, если перевод больше одной строки
//                                            {
//                                                foreach (string ts in transA)
//                                                {
//                                                    transmerged += ts; // объединить все строки в одну
//                                                }
//                                            }

//                                            //Это заменил расширением Where, что выше при задании массива, пустые строки будут исключены сразу
//                                            //Проверить, есть ли в массиве хоть один пустой элемент
//                                            //https://stackoverflow.com/questions/44405411/how-can-i-check-wether-an-array-contains-any-item-or-is-completely-empty
//                                            //if (orig.Any(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)) ) )
//                                            //А это считает количество пустых элементов в массиве
//                                            //https://stackoverflow.com/questions/2391743/how-many-elements-of-array-are-not-null
//                                            //int ymptyelementscnt = orig.Count(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", string.Empty)));

//                                            transA = THSplit(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
//                                        }
//                                        bool br = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
//                                        for (int i2 = 0; i2 < origALength; i2++)
//                                        {
//                                            //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));
//                                            if (parameter0value == origA[i2].Replace("\r", string.Empty)) //Replace здесь убирает \r из за которой строки считались неравными
//                                            {
//                                                //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", string.Empty) + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", string.Empty)));

//                                                commoneventsdata[i].List[c].Parameters[0] = transA[i2].Replace("\r", string.Empty); //Replace убирает \r

//                                                //LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
//                                                br = true;
//                                                break;
//                                            }
//                                        }
//                                        if (br) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
//                                        {
//                                            break;
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (commoneventsdata[i].List[c].Parameters[0].String == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
//                                        {
//                                            commoneventsdata[i].List[c].Parameters[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
//                                            break;
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else if (commoneventsdata[i].List[c].Code == 102)
//                    {
//                        for (int i1 = 0; i1 < commoneventsdata[i].List[c].Parameters[0].AnythingArray.Count; i1++)
//                        {
//                            string parameter0value = commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String;
//                            if (string.IsNullOrEmpty(parameter0value))
//                            {
//                            }
//                            else //if code not equal old code and newline is not empty
//                            {
//                                for (int i2 = 0; i2 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i2++)
//                                {
//                                    if (THFilesElementsDataset.Tables[Jsonname].Rows[i2][1] == null)
//                                    {
//                                    }
//                                    else
//                                    {
//                                        string orig = THFilesElementsDataset.Tables[Jsonname].Rows[i2][0].ToString();
//                                        string trans = THFilesElementsDataset.Tables[Jsonname].Rows[i2][1].ToString();
//                                        if (string.IsNullOrEmpty(trans) || orig == trans)
//                                        {
//                                        }
//                                        else
//                                        {
//                                            //LogToFile("parameter0value=" + parameter0value+ ", orig=" + orig + ", (parameter0value == orig) is " + (parameter0value == orig));
//                                            if (parameter0value == orig)
//                                            {
//                                                //LogToFile("parameter0value=" + parameter0value + ", orig=" + orig + ", (parameter0value == orig) is " + (parameter0value == orig));

//                                                commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1] = trans;
//                                                //LogToFile("commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1]=" + commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String+ ", trans"+ trans);
//                                                break;
//                                            }
//                                        }
//                                    }
//                                }
//                                //THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String); //Save text to new row
//                            }
//                        }
//                    }
//                    else if (commoneventsdata[i].List[c].Code == 402)
//                    {
//                        if (string.IsNullOrEmpty(commoneventsdata[i].List[c].Parameters[1].String))
//                        {
//                        }
//                        else //if code not equal old code and newline is not empty
//                        {
//                            for (int i2 = 0; i2 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i2++)
//                            {
//                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i2][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i2][1].ToString()))
//                                {
//                                }
//                                else
//                                {
//                                    if (commoneventsdata[i].List[c].Parameters[1].String == THFilesElementsDataset.Tables[Jsonname].Rows[i2][0].ToString())
//                                    {
//                                        commoneventsdata[i].List[c].Parameters[1] = THFilesElementsDataset.Tables[Jsonname].Rows[i2][1].ToString();
//                                        break;
//                                    }
//                                }
//                            }
//                            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String); //Save text to new row
//                        }
//                    }
//                }
//            }
//            //LogToFile(string.Empty, true);
//            string s = RpgMakerMVjsonCommonEventsTo.ToJson(commoneventsdata);
//            File.WriteAllText(@"C:\\000 test RPGMaker MV data\\" + Jsonname + "1.json", s);
//            MessageBox.Show("test write finished");

//            return true;
//        }

//        split string to several with equal lenght //работает криво, выдает результат "te" и "st" вместо "test1" и "test2" для "test1test2"
//        https://stackoverflow.com/questions/1450774/splitting-a-string-into-chunks-of-a-certain-size
//        /*
//        static IEnumerable<string> THSplit(string str, int chunkSize)
//        {
//            return Enumerable.Range(0, str.Length / chunkSize)
//                .Select(i => str.Substring(i * chunkSize, chunkSize));
//        }
//        */



//        private void THOnlineTranslateByBigBlocks(int cind, int tableindex, int[] selindexes, string method = "a")
//        {
//            int rowscount = 0;
//            int googletextmaxsize = 2000;
//            int googletextcurrentsize = 0;
//            using (DataTable inputtextarrayData = new DataTable())
//            {
//                inputtextarrayData.Columns.Add(THSettings.OriginalColumnName());
//                StringBuilder inputtextarrayDataSB = new StringBuilder();
//                using (DataTable inputtextarrayInfo = new DataTable())
//                {
//                    //LogToFile("1111");
//                    inputtextarrayInfo.Columns.Add("Inputstring Table Index");
//                    inputtextarrayInfo.Columns.Add("Inputstring Row Index");
//                    inputtextarrayInfo.Columns.Add("Inputstring Extracted Value");

//                    string[] splitter = new string[] { "{THSPLIT}" };

//                    //отображение кнопки отмены операции перевода
//                    this.Invoke((Action)(() => translationInteruptToolStripMenuItem.Visible = true));
//                    this.Invoke((Action)(() => translationInteruptToolStripMenuItem1.Visible = true));
//                    try
//                    {
//                        //проход по таблицам
//                        for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
//                        {
//                            //проход по строкам таблицы
//                            for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
//                            {
//                                //Прерывание потока, если отменено нажатием кнопки отмены
//                                if (InteruptTranslation)
//                                {
//                                    //translationInteruptToolStripMenuItem.Visible = false;
//                                    //translationInteruptToolStripMenuItem1.Visible = false;
//                                    this.Invoke((Action)(() => translationInteruptToolStripMenuItem.Visible = false));
//                                    this.Invoke((Action)(() => translationInteruptToolStripMenuItem1.Visible = false));
//                                    InteruptTranslation = false;
//                                    return;
//                                }

//                                //если поле перевода выбранной строки r в таблице t пустое
//                                if ((THFilesElementsDataset.Tables[t].Rows[r][1] + string.Empty).Length == 0)
//                                {
//                                    //LogToFile("2222");
//                                    //если жто последняя таблица и последняя строка в ней
//                                    if (t + 1 == THFilesElementsDataset.Tables.Count && r + 1 == THFilesElementsDataset.Tables[t].Rows.Count)
//                                    {
//                                        string translation = GoogleAPI.Translate(THFilesElementsDataset.Tables[t].Rows[r][0] + string.Empty);

//                                        //если перевод не пустой
//                                        if (translation.Length == 0)
//                                        {
//                                        }
//                                        else
//                                        {
//                                            //если строка перевода не пустая, доп. проверка на случай, если пользователь ввел в неё значение
//                                            if ((THFilesElementsDataset.Tables[t].Rows[r][1] + string.Empty).Length == 0)
//                                            {
//                                                //присвоить строке значение перевода
//                                                THFilesElementsDataset.Tables[t].Rows[r][1] = translation;

//                                                //автоприсвоение того же значения всем похожим
//                                                THAutoSetValueForSameCells(t, r, 0);
//                                            }
//                                        }

//                                        if (inputtextarrayInfo.Rows.Count > 0)
//                                        {
//                                            //LogToFile("1 ENCODED=\r\n"+ GoogleAPI.UrlEncodeForTranslation(inputtextarrayDataSB.ToString()));
//                                            googletextcurrentsize = 0;
//                                            TranslateArrayAndSetValues(inputtextarrayDataSB, inputtextarrayData, inputtextarrayInfo);
//                                        }
//                                    }
//                                    else
//                                    {
//                                        //получение длины текста оригинала
//                                        googletextcurrentsize += (THFilesElementsDataset.Tables[t].Rows[r][0] + string.Empty).Length + 5;//+5 здесь для учета символа разделителя DNFTT

//                                        //LogToFile("googletextcurrentsize="+ googletextcurrentsize);
//                                        //если текущий общий размер больше максимального для запроса
//                                        if (googletextcurrentsize > googletextmaxsize)
//                                        {
//                                            //LogToFile("inputtextarrayData.Rows.Count=" + inputtextarrayData.Rows.Count);
//                                            //сброс текущего размера
//                                            googletextcurrentsize = 0;

//                                            //обработка объединенной строки
//                                            //LogToFile("2 ENCODED=\r\n" + GoogleAPI.UrlEncodeForTranslation(inputtextarrayDataSB.ToString()));
//                                            TranslateArrayAndSetValues(inputtextarrayDataSB, inputtextarrayData, inputtextarrayInfo);

//                                            string translation = GoogleAPI.Translate(THFilesElementsDataset.Tables[t].Rows[r][0] + string.Empty);

//                                            //перевод последней, с которой размер стал больше
//                                            if ((THFilesElementsDataset.Tables[t].Rows[r][1] + string.Empty).Length == 0)
//                                            {
//                                                THFilesElementsDataset.Tables[t].Rows[r][1] = translation;

//                                                //автоприсвоение того же значения всем похожим
//                                                THAutoSetValueForSameCells(t, r, 0);
//                                            }
//                                        }
//                                        else //если текущий размер не больше максимального
//                                        {
//                                            //LogToFile("add line="+ THFilesElementsDataset.Tables[t].Rows[r][0].ToString());
//                                            //добавление строки в таблицувходных данных и индекса таблицы и строки в таблицу информации о добавленной строке, для дальнейшего разбора
//                                            //inputtextarrayData.Rows.Add(THFilesElementsDataset.Tables[t].Rows[r][0].ToString());
//                                            //замена переноса символа новой строки в конце на DNFTT
//                                            //string addstring = Regex.Replace(THFilesElementsDataset.Tables[t].Rows[r][0].ToString(), @"\r\n$|\r$|\n$","\r\nDNFTT\r\n");

//                                            //if (addstring.Contains("DNFTT"))
//                                            //{
//                                            //}
//                                            //else
//                                            //{
//                                            //    addstring += "DNFTT";
//                                            //}

//                                            string bufferedvalue = Regex.Replace(THFilesElementsDataset.Tables[t].Rows[r][0] + string.Empty, @"\r\n$", "{THSPLIT}");
//                                            //LogToFile("translation fixed=" + translation);
//                                            string[] bufferedvaluearray = bufferedvalue.Split(splitter, StringSplitOptions.None);

//                                            if (bufferedvaluearray.Length > 1)
//                                            {
//                                                for (int b = 0; b < bufferedvaluearray.Length; b++)
//                                                {
//                                                    string addstring = bufferedvaluearray[b];
//                                                    //LogToFile("addstring=" + addstring + ", table=" + t + ", row=" + r);

//                                                    if (addstring.Length == 0)
//                                                    {
//                                                    }
//                                                    else
//                                                    {
//                                                        string extractedvalue = THExtractTextForTranslation(addstring);

//                                                        if (extractedvalue.Length == 0)
//                                                        {
//                                                            bool hasmatch = false;
//                                                            for (int v = 0; v < inputtextarrayData.Rows.Count; v++)
//                                                            {
//                                                                //для последующего сравнения без учета цифр проверить, есть ли уже в добавленных такие же значения с указанными условиями
//                                                                if (Regex.Replace(addstring, @"\d+", string.Empty).Trim() == Regex.Replace(inputtextarrayData.Rows[v][0] + string.Empty, @"\d+", string.Empty).Trim())
//                                                                {
//                                                                    hasmatch = true;
//                                                                    break;
//                                                                }

//                                                            }

//                                                            if (hasmatch)
//                                                            {
//                                                            }
//                                                            else
//                                                            {
//                                                                //for (int s=0,s<addstring.Replace)
//                                                                //добавление строки в кучу для перевода
//                                                                inputtextarrayDataSB.Append(addstring + Environment.NewLine);
//                                                                inputtextarrayData.Rows.Add(addstring);
//                                                                //добавление информации о строке
//                                                                inputtextarrayInfo.Rows.Add(t, r, null);
//                                                            }
//                                                        }
//                                                        else
//                                                        {
//                                                            bool hasmatch = false;
//                                                            for (int v = 0; v < inputtextarrayData.Rows.Count; v++)
//                                                            {
//                                                                //для последующего сравнения без учета цифр проверить, есть ли уже в добавленных такие же значения с указанными условиями
//                                                                if (Regex.Replace(extractedvalue, @"\d+", string.Empty).Trim() == Regex.Replace(inputtextarrayData.Rows[v][0] + string.Empty, @"\d+", string.Empty).Trim())
//                                                                {
//                                                                    hasmatch = true;
//                                                                    break;
//                                                                }

//                                                            }

//                                                            if (hasmatch)
//                                                            {
//                                                            }
//                                                            else
//                                                            {
//                                                                //for (int s=0,s<addstring.Replace)
//                                                                //добавление строки в кучу для перевода
//                                                                inputtextarrayDataSB.Append(extractedvalue + Environment.NewLine);
//                                                                inputtextarrayData.Rows.Add(extractedvalue);
//                                                                //добавление информации о строке
//                                                                inputtextarrayInfo.Rows.Add(t, r, extractedvalue);
//                                                            }
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                            else
//                                            {
//                                                string addstring = THFilesElementsDataset.Tables[t].Rows[r][0] + string.Empty;
//                                                //LogToFile("addstring=" + addstring + ", table=" + t + ", row=" + r);

//                                                if (addstring.Length == 0)
//                                                {
//                                                }
//                                                else
//                                                {
//                                                    string extractedvalue = THExtractTextForTranslation(addstring);

//                                                    if (extractedvalue.Length == 0)
//                                                    {
//                                                        bool hasmatch = false;
//                                                        for (int v = 0; v < inputtextarrayData.Rows.Count; v++)
//                                                        {
//                                                            //для последующего сравнения без учета цифр проверить, есть ли уже в добавленных такие же значения с указанными условиями
//                                                            if (Regex.Replace(addstring, @"\d+", string.Empty).Trim() == Regex.Replace(inputtextarrayData.Rows[v][0] + string.Empty, @"\d+", string.Empty).Trim())
//                                                            {
//                                                                hasmatch = true;
//                                                                break;
//                                                            }

//                                                        }

//                                                        if (hasmatch)
//                                                        {
//                                                        }
//                                                        else
//                                                        {
//                                                            //for (int s=0,s<addstring.Replace)
//                                                            //добавление строки в кучу для перевода
//                                                            inputtextarrayDataSB.Append(addstring + Environment.NewLine);
//                                                            inputtextarrayData.Rows.Add(addstring);
//                                                            //добавление информации о строке
//                                                            inputtextarrayInfo.Rows.Add(t, r, null);
//                                                        }
//                                                    }
//                                                    else
//                                                    {
//                                                        bool hasmatch = false;
//                                                        for (int v = 0; v < inputtextarrayData.Rows.Count; v++)
//                                                        {
//                                                            //для последующего сравнения без учета цифр проверить, есть ли уже в добавленных такие же значения с указанными условиями
//                                                            if (Regex.Replace(extractedvalue, @"\d+", string.Empty).Trim() == Regex.Replace(inputtextarrayData.Rows[v][0] + string.Empty, @"\d+", string.Empty).Trim())
//                                                            {
//                                                                hasmatch = true;
//                                                                break;
//                                                            }

//                                                        }

//                                                        if (hasmatch)
//                                                        {
//                                                        }
//                                                        else
//                                                        {
//                                                            //for (int s=0,s<addstring.Replace)
//                                                            //добавление строки в кучу для перевода
//                                                            inputtextarrayDataSB.Append(extractedvalue + Environment.NewLine);
//                                                            inputtextarrayData.Rows.Add(extractedvalue);
//                                                            //добавление информации о строке
//                                                            inputtextarrayInfo.Rows.Add(t, r, extractedvalue);
//                                                        }
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }

//                        if (inputtextarrayInfo.Rows.Count > 0)
//                        {
//                            //LogToFile("1 ENCODED=\r\n"+ GoogleAPI.UrlEncodeForTranslation(inputtextarrayDataSB.ToString()));
//                            googletextcurrentsize = 0;
//                            TranslateArrayAndSetValues(inputtextarrayDataSB, inputtextarrayData, inputtextarrayInfo);
//                        }
//                    }
//                    catch
//                    {
//                        //LogToFile("Error: "+ex,true);
//                    }
//                    rowscount = inputtextarrayInfo.Rows.Count;
//                }
//            }
//            //LogToFile("rowscount="+ rowscount, true);
//            IsTranslating = false;
//            ProgressInfo(false);
//        }

//        private void TranslateArrayAndSetValues(StringBuilder inputtextarrayDataSB, DataTable inputtextarrayData, DataTable inputtextarrayInfo)
//        {
//            string[] input = new string[inputtextarrayData.Rows.Count];
//            for (int i = 0; i < inputtextarrayData.Rows.Count; i++)
//            {
//                input[i] = inputtextarrayData.Rows[i][0] + string.Empty;
//            }

//            string[] translationarray = GoogleAPI.TranslateMultiple(input, "ja", "en");

//            if (translationarray.Length > 0)
//            {
//                for (int resultindex = 0; resultindex < translationarray.Length; resultindex++)
//                {
//                    int targettableindex = int.Parse(inputtextarrayInfo.Rows[resultindex][0] + string.Empty);
//                    int targetrowindex = int.Parse(inputtextarrayInfo.Rows[resultindex][1] + string.Empty);

//                    if (targetrowindex + 1 < inputtextarrayInfo.Rows.Count && targetrowindex == int.Parse(inputtextarrayInfo.Rows[resultindex + 1][1] + string.Empty))
//                    {
//                        string[] targetcellarray = (THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][0] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);

//                        int linenum = resultindex;
//                        for (int line = 0; line < targetcellarray.Length; line++)
//                        {
//                            if (inputtextarrayInfo.Rows[resultindex][2] == null)
//                            {
//                                if (targetcellarray[line] == inputtextarrayData.Rows[resultindex][0] + string.Empty)
//                                {
//                                    THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] = translationarray[resultindex];

//                                    if (int.Parse(inputtextarrayInfo.Rows[resultindex][1] + string.Empty) + 1 < inputtextarrayInfo.Rows.Count && int.Parse(inputtextarrayInfo.Rows[resultindex][1] + string.Empty) == int.Parse(inputtextarrayInfo.Rows[resultindex + 1][1] + string.Empty))
//                                    {
//                                        resultindex++;
//                                    }
//                                    else
//                                    {
//                                        ////автоприсвоение того же значения всем похожим
//                                        //THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//                                    }
//                                }
//                                ////автоприсвоение того же значения всем похожим
//                                //THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//                            }
//                            else
//                            {
//                                if (targetcellarray[line].Contains(inputtextarrayInfo.Rows[resultindex][2] + string.Empty))
//                                {
//                                    THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] = THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][0] + string.Empty
//                                       .Replace(inputtextarrayInfo.Rows[resultindex][2] + string.Empty, translationarray[resultindex]);

//                                    if (int.Parse(inputtextarrayInfo.Rows[resultindex][1] + string.Empty) + 1 < inputtextarrayInfo.Rows.Count && int.Parse(inputtextarrayInfo.Rows[resultindex][1] + string.Empty) == int.Parse(inputtextarrayInfo.Rows[resultindex + 1][1] + string.Empty))
//                                    {
//                                        resultindex++;
//                                    }
//                                    else
//                                    {
//                                        ////автоприсвоение того же значения всем похожим
//                                        //THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//                                    }

//                                }
//                            }
//                        }
//                        //автоприсвоение того же значения всем похожим
//                        THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//                    }
//                    else
//                    {
//                        if (inputtextarrayInfo.Rows[resultindex][2] == null)
//                        {
//                            THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] = translationarray[resultindex];
//                            //автоприсвоение того же значения всем похожим
//                            THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//                        }
//                        else
//                        {
//                            THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] = THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][0] + string.Empty
//                               .Replace(inputtextarrayInfo.Rows[resultindex][2] + string.Empty, translationarray[resultindex]);
//                            //автоприсвоение того же значения всем похожим
//                            THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//                        }
//                    }

//                }
//            }
//            inputtextarrayInfo.Rows.Clear();
//            inputtextarrayData.Rows.Clear();

//            //LogToFile("TranslateArrayAndSetValues executed!",true);
//            //по таблице в массив
//            //https://stackoverflow.com/questions/37827308/convert-datatable-into-an-array-in-c-sharp
//            //string[] outputarray = GoogleAPI.TranslateMultiple(inputtextarrayData.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray());
//            //inputtextarrayData.Rows.Clear();
//            //googletextcurrentsize = 0;
//            //LogToFile("input SB=\r\n" + inputtextarrayDataSB.ToString());

//            //string translation = GoogleAPI.Translate(inputtextarrayDataSB.ToString());

//            //LogToFile("translation=\r\n" + translation);
//            //if (string.IsNullOrEmpty(translation))
//            //{
//            //}
//            //else
//            //{
//            //    //https://stackoverflow.com/questions/3989816/reading-a-string-line-per-line-in-c-sharp
//            //    string[] translationarray = translation.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
//            //    //LogToFile("\r\ntranslationarray count="+ translationarray.Length+ "\r\ninputtextarrayInfo.Rows.Count="+ inputtextarrayInfo.Rows.Count);
//            //    for (int i = 0; i < translationarray.Length; i++)
//            //    {
//            //        //LogToFile("translationarray["+ i+ "]=" + translationarray[i]);
//            //        int targettableindex = int.Parse(inputtextarrayInfo.Rows[i][0].ToString());
//            //        int targetrowindex = int.Parse(inputtextarrayInfo.Rows[i][1].ToString());
//            //        if (THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1].ToString()))
//            //        {
//            //            string[] targetcellarray = THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][0].ToString().Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);

//            //            //LogToFile("targetcellarray.Length=" + targetcellarray.Length+",value="+ THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][0].ToString());
//            //            if (targetcellarray.Length > 1)
//            //            {
//            //                for (int c=i;c< targetcellarray.Length+i;c++)
//            //                {
//            //                    int targettableindexSUB = int.Parse(inputtextarrayInfo.Rows[i][0].ToString());
//            //                    int targetrowindexSUB = int.Parse(inputtextarrayInfo.Rows[i][1].ToString());

//            //                    if (THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][1]==null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][1].ToString()))
//            //                    {
//            //                        if (THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][0].ToString().Contains(inputtextarrayData.Rows[c][0].ToString()))
//            //                        {
//            //                            if (inputtextarrayInfo.Rows[c][2] == null)
//            //                            {
//            //                                THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][1] = THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][0].ToString().Replace(inputtextarrayData.Rows[c][0].ToString(), translationarray[c]);
//            //                                //LogToFile("Result set value (raw)=" + THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][1]);
//            //                            }
//            //                            else
//            //                            {
//            //                                THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][1] = THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][0].ToString().Replace(inputtextarrayInfo.Rows[c][2].ToString(), translationarray[c]);
//            //                                //LogToFile("Result set value (extracted)=" + THFilesElementsDataset.Tables[targettableindexSUB].Rows[targetrowindexSUB][1]);
//            //                            }
//            //                        }
//            //                    }
//            //                    i = c;
//            //                }

//            //                //string celllinevalue = string.Empty;
//            //                //for (int c = 0; c < targetcellarray.Length; c++)
//            //                //{
//            //                //    i += c;
//            //                //    if (inputtextarrayInfo.Rows[r][2].ToString() == null)
//            //                //    {
//            //                //        celllinevalue += targetcellarray[c];
//            //                //    }
//            //                //    else
//            //                //    {
//            //                //        celllinevalue += targetcellarray[c].Replace(inputtextarrayInfo.Rows[r][2].ToString(), translationarray[i]) + Environment.NewLine;
//            //                //    }
//            //                //}
//            //                //THFilesElementsDataset.Tables[targettableindex].Rows[r][1] = celllinevalue;
//            //            }
//            //            else
//            //            {
//            //                THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] = THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][0].ToString().Replace(inputtextarrayData.Rows[i][0].ToString(), translationarray[i]);
//            //                //LogToFile("Result set value="+ THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1]);
//            //                //автоприсвоение того же значения всем похожим
//            //                THAutoSetValueForSameCells(targettableindex, targetrowindex, 0);
//            //            }
//            //        }
//            //    }

//            //LogToFile("translation="+ translation);
//            ////http://qaru.site/questions/31364/how-do-i-split-a-string-by-a-multi-character-delimiter-in-c
//            ////задание разделителя
//            ////\r\n{{BBC}}\r\n
//            //string[] splitter = new string[] { "THBBC" };
//            ////обрезание пробелов вокруг разделителя, образовавшихся при переводе
//            ////translation = translation.Replace("D NFTT", "DNFTT").Replace("DN FTT", "DNFTT").Replace("DNF TT", "DNFTT").Replace("DNFT T", "DNFTT").Replace(" DNFTT ", "DNFTT").Replace(" DNFTT", "DNFTT").Replace("DNFTT ", "DNFTT").Replace("\r\nDNFTT\r\n", "DNFTT\r\n");
//            //translation = FixBBC(translation);
//            ////создание массима по разделителю
//            //LogToFile("translation fixed=" + translation);
//            //string[] result = translation.Split(splitter, StringSplitOptions.None);
//            //LogToFile("result=\r\n" + result);
//            //for (int o = 0; o < result.Length; o++)
//            //{
//            //    if (string.IsNullOrEmpty(result[o]))
//            //    {
//            //    }
//            //    else
//            //    {
//            //        int targettableindex = int.Parse(inputtextarrayInfo.Rows[o][0].ToString());
//            //        int targetrowindex = int.Parse(inputtextarrayInfo.Rows[o][1].ToString());

//            //        if (THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1].ToString()))
//            //        {
//            //            //LogToFile("result[o]=" + result[o]);
//            //            THFilesElementsDataset.Tables[targettableindex].Rows[targetrowindex][1] = result[o];
//            //        }
//            //    }
//            //}
//            //}
//        }

//        private string FixBBC(string input)
//        {
//            return input
//                .Replace("T HBBC", "THBBC")
//                .Replace("TH BBC", "THBBC")
//                .Replace("THB BC", "THBBC")
//                .Replace("THBB C", "THBBC")
//                .Replace("\r\nTHBBC\r\n", "THBBC")
//                ;
//        }



//    }
//}
