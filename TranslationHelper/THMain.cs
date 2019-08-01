using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THMain : Form
    {
        string THLog;
        //public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");
        THSettingsForm Settings = new THSettingsForm();
        //public const string THStrDGTranslationColumnName = "Translation";
        //public const string THStrDGOriginalColumnName = "Original";
        private THLang LangF;

        public string apppath = Application.StartupPath.ToString();
        private string extractedpatchpath = "";

        private string FVariant = "";
        private BindingList<THRPGMTransPatchFile> THRPGMTransPatchFiles; //Все файлы
        DataTable fileslistdt = new DataTable();
        DataSet ds = new DataSet();
        //DataTable THFilesElementsDatatable;
        private BindingSource THBS = new BindingSource();

        private string THSelectedDir;
        private string THRPGMTransPatchver;
        private string THSelectedSourceType;

        //Language strings
        string THMainDGVOriginalColumnName;
        string THMainDGVTranslationColumnName;

        public THMain()
        {
            InitializeComponent();
            LangF = new THLang();
            Settings.GetSettings();

            //anguage strings setup
            THMainDGVOriginalColumnName = LangF.THStrDGOriginalColumnName;
            THMainDGVTranslationColumnName = LangF.THStrDGTranslationColumnName;
            fileToolStripMenuItem.Text = LangF.THStrfileToolStripMenuItemName;
            openToolStripMenuItem.Text = LangF.THStropenToolStripMenuItemName;
            saveToolStripMenuItem.Text = LangF.THStrsaveToolStripMenuItemName;
            saveAsToolStripMenuItem.Text = LangF.THStrsaveAsToolStripMenuItemName;
            editToolStripMenuItem.Text = LangF.THStreditToolStripMenuItemName;
            viewToolStripMenuItem.Text = LangF.THStrviewToolStripMenuItemName;
            optionsToolStripMenuItem.Text = LangF.THStroptionsToolStripMenuItemName;
            helpToolStripMenuItem.Text = LangF.THStrhelpToolStripMenuItemName;
            aboutToolStripMenuItem.Text = LangF.THStraboutToolStripMenuItemName;
            LangF.THReadLanguageFileToStrings();

            THRPGMTransPatchFiles = new BindingList<THRPGMTransPatchFile>();
            //dt = new DataTable();

            //THFileElementsDataGridView set doublebuffered to true
            SetDoublebuffered(true);

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        public static void StartLoadingForm()
        {
            try
            {
                Application.Run(new THLoadingForm());
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog THFOpen = new OpenFileDialog
            {
                Filter = "All compatible|*.exe;RPGMKTRANSPATCH;*.json|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe|All files (*.*)|*.*"
            };

            if (THFOpen.ShowDialog() == DialogResult.OK)
            {
                if (THFOpen.OpenFile() != null)
                {
                    THCleanupThings();
                    THSelectedSourceType = GetSourceType(THFOpen.FileName);
                }
            }
        }

        private void THCleanupThings()
        {
            //Cleaning
            THFilesListBox.Items.Clear();

            //Disable menus
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            viewToolStripMenuItem.Enabled = false;
        }

        private string GetSourceType(String sPath)
        {
            //MessageBox.Show("sPath=" + sPath);
            if (sPath.ToUpper().Contains("\\RPGMKTRANSPATCH"))
            {
                return RPGMTransPatchPrepare(sPath);
                //return "RPGMTransPatch";
            }
            else if (sPath.ToLower().Contains("\\game.exe"))
            {
                var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));

                if (File.Exists(dir.ToString() + "\\www\\data\\system.json"))
                {
                    //var MVJsonFIles = new List<string>();
                    var mvdatadir = new DirectoryInfo(Path.GetDirectoryName(dir.ToString() + "\\www\\data\\"));
                    foreach (FileInfo file in mvdatadir.GetFiles("*.json"))
                    {
                        //MessageBox.Show("file.FullName=" + file.FullName);
                        //MVJsonFIles.Add(file.FullName);

                        if (OpenRPGMakerMVjson(file.FullName))
                        {
                        }
                        else
                        {
                            return "";
                        }
                    }

                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        THFilesListBox.Items.Add(ds.Tables[i].TableName);
                    }
                    return "RPG Maker MV json";
                }
                else
                {

                    extractedpatchpath = "";
                    bool result = TryToExtractToRPGMakerTransPatch(sPath);
                    //MessageBox.Show("result=" + result);
                    //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                    if (result)
                    {
                        //Cleaning of the type
                        THRPGMTransPatchFiles.Clear();

                        //var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
                        //string patchver;
                        bool isv3 = Directory.Exists(extractedpatchpath + "\\patch");
                        //MessageBox.Show("isv3=" + isv3+ ", patchdir="+ extractedpatchpath+ ", extractedpatchpath="+ extractedpatchpath);
                        if (isv3) //если есть подпапка patch, тогда это версия патча 3
                        {
                            THRPGMTransPatchver = "3";
                            extractedpatchpath += "\\patch";
                            //MessageBox.Show("extractedpatchpath=" + extractedpatchpath);
                            dir = new DirectoryInfo(Path.GetDirectoryName(extractedpatchpath + "\\")); //Два слеша здесь в конце исправляют проблему возврата информации о неверной папке
                                                                                                       //MessageBox.Show("patchdir1=" + patchdir);
                        }
                        else //иначе это версия 2
                        {
                            THRPGMTransPatchver = "2.0";
                        }
                        //MessageBox.Show("patchdir2=" + patchdir);

                        var vRPGMTransPatchFiles = new List<string>();

                        foreach (FileInfo file in dir.GetFiles("*.txt"))
                        {
                            //MessageBox.Show("file.FullName=" + file.FullName);
                            vRPGMTransPatchFiles.Add(file.FullName);
                        }

                        //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

                        //THFilesDataGridView.Nodes.Add("main");
                        //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
                        //RPGMTransPatch.OpenTransFiles(files, patchver);
                        //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
                        if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchver))
                        {
                            THSelectedDir = extractedpatchpath.Replace("\\patch", "");
                            saveToolStripMenuItem.Enabled = true;
                            saveAsToolStripMenuItem.Enabled = true;
                            editToolStripMenuItem.Enabled = true;
                            viewToolStripMenuItem.Enabled = true;
                            MessageBox.Show(THSelectedSourceType + " loaded!");
                            return "RPG Maker game with RPGMTransPatch";
                        }
                    }
                }

            }
            else if (sPath.ToLower().Contains(".json"))
            {
                if (OpenRPGMakerMVjson(sPath))
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        THFilesListBox.Items.Add(ds.Tables[i].TableName); //add all dataset tables names to the ListBox
                    }
                    return "RPG Maker MV json";
                }
            }

            MessageBox.Show("Uncompatible source or problem while opening.");
            return "";
        }

        private string RPGMTransPatchPrepare(string sPath)
        {

            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            //MessageBox.Show("THFOpen.FileName=" + THFOpen.FileName);
            //MessageBox.Show("dir=" + dir);
            THSelectedDir = dir.ToString();
            //MessageBox.Show("THSelectedDir=" + THSelectedDir);

            //MessageBox.Show("sType=" + sType);

            //if (THSelectedSourceType == "RPGMTransPatch")
            //{
            //Cleaning of the type
            THRPGMTransPatchFiles.Clear();

            //string patchver;
            var patchdir = dir;
            StreamReader patchfile = new StreamReader(sPath);
            //MessageBox.Show(patchfile.ReadLine());
            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(THSelectedDir + "\\patch")) //если есть подпапка patch, тогда это версия патча 3
            {
                THRPGMTransPatchver = "3";
                patchdir = new DirectoryInfo(Path.GetDirectoryName(sPath) + "\\patch");
            }
            else //иначе это версия 2
            {
                THRPGMTransPatchver = "2.0";
            }
            patchfile.Close();

            var vRPGMTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.GetFiles("*.txt"))
            {
                //MessageBox.Show("file.FullName=" + file.FullName);
                vRPGMTransPatchFiles.Add(file.FullName);
            }

            //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

            //THFilesDataGridView.Nodes.Add("main");
            //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
            //RPGMTransPatch.OpenTransFiles(files, patchver);
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchver))
            {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                viewToolStripMenuItem.Enabled = true;
                MessageBox.Show(THSelectedSourceType + " loaded!");
                return "RPGMTransPatch";
            }
            //}
            return "";
        }

        private bool OpenRPGMakerMVjson(string sPath)
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
                if (ds.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }
                string jsondata = File.ReadAllText(sPath); // get json data

                ds.Tables.Add(Jsonname); // create table with json name
                ds.Tables[Jsonname].Columns.Add("Original"); //create Original column

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

                string jsonname = Jsonname.ToLower(); //set jsonname to lower registry
                if (jsonname == "items" || jsonname == "armors" || jsonname == "weapons")
                {
                    //("name", "description", "note")
                    name = true;
                    description = true;
                    note = true;
                }
                else if (jsonname == "skills")
                {
                    //("name", "description", "message1", "message2", "note")
                    name = true;
                    description = true;
                    message1 = true;
                    message2 = true;
                    note = true;
                }
                else if (jsonname == "states")
                {
                    //("name", "message1", "message2", "message3", "message4", "note")
                    name = true;
                    message1 = true;
                    message2 = true;
                    message3 = true;
                    message4 = true;
                    note = true;
                }
                else if (jsonname == "classes" || jsonname == "enemies" || jsonname == "tilesets")
                {
                    //("name", "note")
                    name = true;
                    note = true;
                }
                else if (jsonname == "animations" || jsonname == "mapinfos")
                {
                    //("name")
                    name = true;
                }
                else if (jsonname == "actors")
                {
                    //("name", "nickname", "note", "profile")
                    name = true;
                    nickname = true;
                    note = true;
                    profile = true;
                }
                else if (jsonname.StartsWith("map"))
                {
                    //['displayName'] / ['note'] / ['events'][$eIndex]['name'] / ['events'][$eIndex]['note']
                    //displayname = true;
                    //name = true;
                    //note = true;
                    maps = true;
                }
                else if (jsonname == "troops")
                {
                    //"name" / 
                    name = true;
                }
                else if (jsonname == "commonevents")
                {
                    //"name" / 
                    //name = true;
                    cmnevents = true;
                }
                else if (jsonname == "system")
                {
                    //"name" /
                    //name = true;
                    system = true;
                }

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

                //var result = JsonConvert.DeserializeObject<List<RPGMakerMVjson>>(File.ReadAllText(sPath));
                //var resultdescriptions = JsonConvert.DeserializeObject<List<RPGMakerMVjsonFileDescriptions>>(File.ReadAllText(sPath));
                //var resultparameters = JsonConvert.DeserializeObject<List<RPGMakerMVjsonFileParameters>>(File.ReadAllText(sPath));

                //THFileElementsDataGridView.DataSource = ds.Tables[0];
                //THFileElementsDataGridView.Columns[0].ReadOnly = true;

                if (ds.Tables[Jsonname].Rows.Count > 0)
                {
                    ds.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    ds.Tables.Remove(Jsonname); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        private bool FillDSTableWithJsonValues(string Jsonname,
                                               string jsondata,
                                               bool name = false,
                                               bool description = false,
                                               bool displayname = false,
                                               bool note = false,
                                               bool message1 = false,
                                               bool message2 = false,
                                               bool message3 = false,
                                               bool message4 = false,
                                               bool nickname = false,
                                               bool profile = false,
                                               bool maps = false,
                                               bool cmnevents = false,
                                               bool system = false)
        {
            try
            {
                if (name)
                {
                    foreach (RPGMakerMVjsonName Name in JsonConvert.DeserializeObject<List<RPGMakerMVjsonName>>(jsondata))
                    {
                        if (Name == null || string.IsNullOrEmpty(Name.Name) || SelectedLocalePercentFromStringIsNotValid(Name.Name) || GetAlreadyAddedInTable(Jsonname, Name.Name))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Name.Name);
                        }
                    }
                }
                if (description)
                {
                    foreach (RPGMakerMVjsonDescription Description in JsonConvert.DeserializeObject<List<RPGMakerMVjsonDescription>>(jsondata))
                    {
                        if (Description == null || string.IsNullOrEmpty(Description.Description) || SelectedLocalePercentFromStringIsNotValid(Description.Description) || GetAlreadyAddedInTable(Jsonname, Description.Description))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Description.Description);
                        }
                    }
                }
                if (displayname)
                {
                    foreach (RPGMakerMVjsonDisplayName DisplayName in JsonConvert.DeserializeObject<List<RPGMakerMVjsonDisplayName>>(jsondata))
                    {
                        if (DisplayName == null || string.IsNullOrEmpty(DisplayName.DisplayName) || SelectedLocalePercentFromStringIsNotValid(DisplayName.DisplayName) || GetAlreadyAddedInTable(Jsonname, DisplayName.DisplayName))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(DisplayName.DisplayName);
                        }
                    }
                }
                if (note)
                {
                    foreach (RPGMakerMVjsonNote Note in JsonConvert.DeserializeObject<List<RPGMakerMVjsonNote>>(jsondata))
                    {
                        if (Note == null || string.IsNullOrEmpty(Note.Note) || SelectedLocalePercentFromStringIsNotValid(Note.Note) || GetAlreadyAddedInTable(Jsonname, Note.Note))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Note.Note);
                        }
                    }
                }
                if (message1)
                {
                    foreach (RPGMakerMVjsonMessage1 Message1 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage1>>(jsondata))
                    {
                        if (Message1 == null || string.IsNullOrEmpty(Message1.Message1) || SelectedLocalePercentFromStringIsNotValid(Message1.Message1) || GetAlreadyAddedInTable(Jsonname, Message1.Message1))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Message1.Message1);
                        }
                    }
                }
                if (message2)
                {
                    foreach (RPGMakerMVjsonMessage2 Message2 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage2>>(jsondata))
                    {
                        if (Message2 == null || string.IsNullOrEmpty(Message2.Message2) || SelectedLocalePercentFromStringIsNotValid(Message2.Message2) || GetAlreadyAddedInTable(Jsonname, Message2.Message2))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Message2.Message2);
                        }
                    }
                }
                if (message3)
                {
                    foreach (RPGMakerMVjsonMessage3 Message3 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage3>>(jsondata))
                    {
                        if (Message3 == null || string.IsNullOrEmpty(Message3.Message3) || SelectedLocalePercentFromStringIsNotValid(Message3.Message3) || GetAlreadyAddedInTable(Jsonname, Message3.Message3))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Message3.Message3);
                        }
                    }
                }
                if (message4)
                {
                    foreach (RPGMakerMVjsonMessage4 Message4 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage4>>(jsondata))
                    {
                        if (Message4 == null || string.IsNullOrEmpty(Message4.Message4) || SelectedLocalePercentFromStringIsNotValid(Message4.Message4) || GetAlreadyAddedInTable(Jsonname, Message4.Message4))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Message4.Message4);
                        }
                    }
                }
                if (nickname)
                {
                    foreach (RPGMakerMVjsonNickname Nickname in JsonConvert.DeserializeObject<List<RPGMakerMVjsonNickname>>(jsondata))
                    {
                        if (Nickname == null || string.IsNullOrEmpty(Nickname.Nickname) || SelectedLocalePercentFromStringIsNotValid(Nickname.Nickname) || GetAlreadyAddedInTable(Jsonname, Nickname.Nickname))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Nickname.Nickname);
                        }
                    }
                }
                if (profile)
                {
                    foreach (RPGMakerMVjsonProfile Profile in JsonConvert.DeserializeObject<List<RPGMakerMVjsonProfile>>(jsondata))
                    {
                        if (Profile == null || string.IsNullOrEmpty(Profile.Profile) || SelectedLocalePercentFromStringIsNotValid(Profile.Profile) || GetAlreadyAddedInTable(Jsonname, Profile.Profile))
                        {
                        }
                        else
                        {
                            ds.Tables[Jsonname].Rows.Add(Profile.Profile);
                        }
                    }
                }

                if (cmnevents)
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
                        if (string.IsNullOrEmpty(eventname) || GetAlreadyAddedInTable(Jsonname, eventname))
                        {
                        }
                        else //if code not equal old code and newline is not empty
                        {
                            ds.Tables[Jsonname].Rows.Add(eventname); //add event name to new row
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
                                                THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted is true and newline is empty\r\n";
                                                textaddingstarted = false;
                                            }
                                        }
                                        else //if code not equal old code and newline is not empty
                                        {
                                            if (GetAlreadyAddedInTable(Jsonname, newline))
                                            {
                                                THLog += DateTime.Now + " >>: Code 401/405 newline already in table=\"" + newline + "\"\r\n";
                                                newline = ""; //clear text data
                                                if (textaddingstarted)
                                                {
                                                    textaddingstarted = false;
                                                }
                                            }
                                            else
                                            {
                                                THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted=true added newline=\"" + newline + "\"\r\n";
                                                ds.Tables[Jsonname].Rows.Add(newline); //Save text to new row
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
                                        THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline is empty\r\n";
                                        textaddingstarted = false;
                                    }
                                }
                                else //if code not equal old code and newline is not empty
                                {
                                    if (GetAlreadyAddedInTable(Jsonname, newline))
                                    {
                                        if (textaddingstarted)
                                        {
                                            THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline already in table=\"" + newline + "\"\r\n";
                                            textaddingstarted = false;
                                        }
                                    }
                                    else
                                    {
                                        THLog += DateTime.Now + " >>: Code 101/105 newline is not empty=\"" + newline + "\"\r\n";
                                        ds.Tables[Jsonname].Rows.Add(newline); //Save text to new row
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
                                            THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
                                            textaddingstarted = false;
                                        }
                                    }
                                    else //if code not equal old code and newline is not empty
                                    {
                                        if (GetAlreadyAddedInTable(Jsonname, schoice))
                                        {
                                            THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
                                            if (textaddingstarted)
                                            {
                                                THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
                                                textaddingstarted = false;
                                            }
                                        }
                                        else
                                        {
                                            THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
                                            ds.Tables[Jsonname].Rows.Add(schoice); //Save text to new row
                                            if (string.IsNullOrEmpty(newline))
                                            {
                                            }
                                            else
                                            {
                                                THLog += DateTime.Now + " >>: Code 102 added schoice and also newline is not empty, set empty\r\n";
                                                newline = ""; //clear text data
                                            }
                                            if (textaddingstarted)
                                            {
                                                THLog += DateTime.Now + " >>: Code 102 added schoice and also textaddingstarted is true , set false\r\n";
                                                textaddingstarted = false;
                                            }
                                        }

                                    }
                                }

                                /*
                                string schoice = commoneventsdata[i].list[c].Parameters[0].ToString();
                                if (string.IsNullOrEmpty(schoice))
                                {
                                    if (textaddingstarted)
                                    {
                                        THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
                                        textaddingstarted = false;
                                    }
                                }
                                else //if code not equal old code and newline is not empty
                                {
                                    if (GetAlreadyAddedInTable(Jsonname, schoice))
                                    {
                                        THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
                                        if (textaddingstarted)
                                        {
                                            THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
                                            textaddingstarted = false;
                                        }
                                    }
                                    else
                                    {
                                        THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
                                        ds.Tables[Jsonname].Rows.Add(schoice); //Save text to new row
                                        newline = ""; //clear text data
                                        textaddingstarted = false;
                                    }

                                }
                                */
                            }
                        }
                        /*
                        foreach (var command in commoneventsdata[i].list)
                        {
                            commandcode = command.Code;

                            if (commandcode == commandoldcode)
                            {
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(newline) || GetAlreadyAddedInTable(Jsonname, newline))
                                {
                                }
                                else //if code not equal old code and newline is not empty
                                {
                                    //THLog += DateTime.Now + " >>: added newline=\"" + newline + "\"\r\n";
                                    ds.Tables[Jsonname].Rows.Add(newline); //Save text to new row
                                    newline = ""; //clear text data
                                    textaddingstarted = false;
                                }
                            }

                            if (commandcode == 102) //Show choices
                            {
                                foreach (var choice in command.parameters) //Get all choices
                                {

                                    if (string.IsNullOrEmpty(newline) || GetAlreadyAddedInTable(Jsonname, newline))
                                    {
                                    }
                                    else //if code not equal old code and newline is not empty
                                    {
                                        //THLog += DateTime.Now + " >>: 102 added newline=\"" + newline + "\"\r\n";
                                        ds.Tables[Jsonname].Rows.Add(newline); //Save text to new row
                                        newline = ""; //clear text data
                                        textaddingstarted = false;
                                    }
                                }
                            }
                            else if (commandcode == 401) //Show text 401
                            {
                                if (textaddingstarted)
                                {
                                    newline += "\r\n";//add new line when multiline value in text.
                                }
                                newline += command.parameters[0]; //Add text to variable for case if the command will add more lines
                                textaddingstarted = true;
                            }
                            else if (commandcode == 405) //Show text 401 OR Show Scrolling Text 405
                            {
                                if (textaddingstarted)
                                {
                                    newline += "\r\n";//add new line when multiline value in text.
                                }
                                newline += command.parameters[0]; //Add text to variable for case if the command will add more lines
                                textaddingstarted = true;
                            }

                            commandoldcode = commandcode;//save current command code to commandoldcode variable
                            
                        }
                        */
                        FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
                        THLog = "";
                    }

                }
                if (maps)
                {
                    //bool eventsdone = false;
                    //bool geteventnamenotedone = false;

                    var map = JsonConvert.DeserializeObject<RPGMakerMVjsonMap>(jsondata);

                    if (map.Events.Length > 1) //first event is empty
                    {
                        //Map displayed name
                        if (string.IsNullOrEmpty(map.DisplayName) || SelectedLocalePercentFromStringIsNotValid(map.DisplayName) || GetAlreadyAddedInTable(Jsonname, map.DisplayName))
                        {
                        }
                        else
                        {
                            //MessageBox.Show("map.DisplayName:" + map.DisplayName);
                            ds.Tables[Jsonname].Rows.Add(map.DisplayName);
                        }
                        //Map note
                        if (string.IsNullOrEmpty(map.Note) || SelectedLocalePercentFromStringIsNotValid(map.Note) || GetAlreadyAddedInTable(Jsonname, map.Note))
                        {
                        }
                        else
                        {
                            //MessageBox.Show("map.Note:" + map.Note);
                            ds.Tables[Jsonname].Rows.Add(map.Note);
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
                                if (string.IsNullOrEmpty(ev.Name) || ev.Name.StartsWith("EV") || SelectedLocalePercentFromStringIsNotValid(ev.Name) || GetAlreadyAddedInTable(Jsonname, ev.Name))
                                {
                                }
                                else
                                {
                                    //MessageBox.Show("map.Events add name"+ ev.Name);
                                    ds.Tables[Jsonname].Rows.Add(ev.Name);
                                    //prevval = ev.Name;
                                }
                                //event note
                                if (string.IsNullOrEmpty(ev.Note) || SelectedLocalePercentFromStringIsNotValid(ev.Note) || GetAlreadyAddedInTable(Jsonname, ev.Note))
                                {
                                }
                                else
                                {
                                    //MessageBox.Show("map.Events add note:" + ev.Note);
                                    ds.Tables[Jsonname].Rows.Add(ev.Note);
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
                                                if (string.IsNullOrEmpty(pstring) || HasNOJPcharacters(pstring) || SelectedLocalePercentFromStringIsNotValid(pstring) || GetAlreadyAddedInTable(Jsonname, pstring))
                                                {

                                                }
                                                else
                                                {
                                                    ds.Tables[Jsonname].Rows.Add(pstring);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (system)
                {
                    //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
                    var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

                    //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

                    ds.Tables[Jsonname].Rows.Add(systemdata.GameTitle);

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
                                ds.Tables[Jsonname].Rows.Add(armortype);
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
                                ds.Tables[Jsonname].Rows.Add(element);
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
                                ds.Tables[Jsonname].Rows.Add(equipType);
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
                                ds.Tables[Jsonname].Rows.Add(skillType);
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
                                ds.Tables[Jsonname].Rows.Add(_switch);
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
                                ds.Tables[Jsonname].Rows.Add(_switch);
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
                                ds.Tables[Jsonname].Rows.Add(weaponType);
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
                                ds.Tables[Jsonname].Rows.Add(basic);
                            }
                        }
                        foreach (var command in systemdata.Terms.Commands)
                        {
                            if (string.IsNullOrEmpty(command))
                            {

                            }
                            else
                            {
                                ds.Tables[Jsonname].Rows.Add(command);
                            }
                        }

                        foreach (string param in systemdata.Terms.Params)
                        {
                            if (string.IsNullOrEmpty(param))
                            {

                            }
                            else
                            {
                                ds.Tables[Jsonname].Rows.Add(param);
                            }
                        }

                        foreach (var Message in systemdata.Terms.Messages)
                        {
                            ds.Tables[Jsonname].Rows.Add(Message.Value);
                        }
                    }
                    //FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
                    //THLog = "";
                }


                return true;
            }
            catch (Exception ex)
            {
                //if (string.IsNullOrEmpty(THLog))
                //{
                //}
                //else
                //{
                FileWriter.WriteData(apppath + "\\TranslationHelper.log", ex.Message, true);
                //}
                return false;
            }
        }

        public static IEnumerable<Type> GetAllSubclassOf(Type parent)
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetTypes())
                {
                    if (t.IsSubclassOf(parent)) yield return t;
                }
            }
        }

        private bool GetAlreadyAddedInTable(string tablename, string value)
        {
            //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = ds.Tables[tablename].Columns["Original"];
            ds.Tables[tablename].PrimaryKey = keyColumns;

            //очень быстрый способ поиска дубликата значения, два нижник в разы медленней, этот почти не заметен
            if (ds.Tables[tablename].Rows.Contains(value))
            {
                //MessageBox.Show("found! value=" + value);
                return true;
            }
            /*самый медленный способ, заметно медленней нижнего и непомерно критически медленней верхнего
            if (ds.Tables[tablename].Select("Original = '" + value.Replace("'", "''") + "'").Length > 0)
            {
                //MessageBox.Show("found! value=" + value);
                return true;
            }
            */
            /*довольно медленный способ, быстрее того, что перед этим с Select, но критически медленней верхнего первого
            for (int i=0; i < ds.Tables[tablename].Rows.Count; i++)
            {
                if (ds.Tables[tablename].Rows[i][0].ToString() == value)
                {
                    return true;
                }
            }
            */
            return false;
        }

        private bool HasNOJPcharacters(string str)
        {
            return GetLocaleLangCount(str, "kanji") < 1 && GetLocaleLangCount(str, "katakana") < 1 && GetLocaleLangCount(str, "hiragana") < 1;
        }

        private bool TryToExtractToRPGMakerTransPatch(string sPath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            string tempdir = apppath + "\\Temp";
            if (!Directory.Exists(tempdir))
            {
                Directory.CreateDirectory(tempdir);
            }
            //MessageBox.Show("tempdir=" + tempdir);
            string outdir = apppath + "\\Temp\\" + Path.GetFileNameWithoutExtension(Path.GetDirectoryName(sPath));

            extractedpatchpath = outdir + "_patch";
            bool ret;
            if (!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);

                Process RPGMakerTransPatch = new Process();
                //MessageBox.Show("outdir=" + outdir);
                RPGMakerTransPatch.StartInfo.FileName = apppath + @"\\Res\\rpgmakertrans\\rpgmt.exe";
                RPGMakerTransPatch.StartInfo.Arguments = "\"" + dir + "\" -p \"" + outdir + "_patch\"" + "\" -o \"" + outdir + "_translated\"";
                ret = RPGMakerTransPatch.Start();
                RPGMakerTransPatch.WaitForExit();
                /*MessageBox.Show(
                      "INFO: apppath=" + apppath
                    + "\r\nRPGMakerTransPatch.StartInfo.FileName=" + RPGMakerTransPatch.StartInfo.FileName
                    + "\r\nRPGMakerTransPatch.StartInfo.Arguments=" + RPGMakerTransPatch.StartInfo.Arguments
                    + "\r\nsPath=" + sPath
                               );*/
            }
            else
            {
                return true;
            }
            return ret;
        }

        private int invalidformat;

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles, string patchver = "2.0")
        {
            //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
            //MessageBox.Show("ListFiles=" + ListFiles);
            //MessageBox.Show("ListFiles[0]=" + ListFiles[0]);
            System.IO.StreamReader _file;   //Через что читаем
            string _context = "";           //Комментарий
            string _advice = "";            //Предел длины строки
            string _string;// = "";            //Переменная строки
            string _untrans = "";           //Непереведенный текст
            string _trans = "";             //Переведенный текст
            int _status = 0;             //Статус

            int verok = 0;                  //версия патча
            //THMain Main = new THMain();
            //var Main = (THMain)MainForm;
            //THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //THFilesDataGridView.Columns.Add("Filename", "Text");
            THSourceTextBox.Enabled = false;
            THTargetTextBox.Enabled = false;

            //прогрессбар
            //progressBar.Maximum = ListFiles.Count;
            //progressBar.Value = 0;
            //List<RPGMTransPatchFile> THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                _file = new StreamReader(ListFiles[i]); //Задаем файл
                THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), ""));    //Добaвляем файл

                if (patchver == "3")
                {
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        //Код для версии патча 3
                        if (_string.StartsWith("> BEGIN STRING"))
                        {
                            invalidformat = 2; //если нашло строку
                            _string = _file.ReadLine();

                            int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            //MessageBox.Show("1.0" + _string);
                            while (!_string.StartsWith("> CONTEXT:"))  //Ждем начало следующего блока
                            {
                                if (untranslines > 0)
                                    _untrans += "\r\n";
                                _untrans += _string;            //Пишем весь текст
                                _string = _file.ReadLine();
                                untranslines++;
                                //MessageBox.Show("1.1"+_string);
                            }
                            //MessageBox.Show("2.1"+_untrans);

                            int contextlines = 0;
                            while (_string.StartsWith("> CONTEXT:"))
                            {
                                if (contextlines > 0)
                                    _context += "\r\n";
                                _context += _string.Replace("> CONTEXT: ", "").Replace(" < UNTRANSLATED", "");// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий
                                //MessageBox.Show("_context ='" + _context + "'");
                                _string = _file.ReadLine();
                                contextlines++;

                                //MessageBox.Show("3"+_string);
                            }
                            //MessageBox.Show("4" + _context);

                            //MessageBox.Show("7.0" + _untrans);

                            int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            while (!_string.StartsWith("> END"))      //Ждем конец блока
                            {
                                if (translines > 0)
                                    _trans += "\r\n";
                                _trans += _string;
                                _string = _file.ReadLine();
                                translines++;
                                //MessageBox.Show("_string ='" + _string + "'");
                                //MessageBox.Show("5" + _string);
                            }
                            //MessageBox.Show("6" + _trans);

                            //MessageBox.Show("7.1" + _untrans);
                            //_string = _file.ReadLine();

                            /*С условием проверки длины строки просто не загружался перевод, где первая строка была пустая
                            if (_string.Length > 0)                    //Если текст есть, ищем перевод
                            {
                                int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                                while (!_string.StartsWith("> END"))      //Ждем конец блока
                                {
                                    if (translines > 0)
                                        _trans += "\r\n";
                                    _trans += _string;
                                    _string = _file.ReadLine();
                                    translines++;
                                    //MessageBox.Show("_string ='" + _string + "'");
                                    //MessageBox.Show("5" + _string);
                                }
                                //MessageBox.Show("6" + _trans);

                                //MessageBox.Show("7.1" + _untrans);
                                _string = _file.ReadLine();
                            }*/

                            if (_untrans != "\r\n")
                            {
                                THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));  //Пишем
                            }

                            _context = "";  //Чистим
                            _untrans = "";  //Чистим
                            _trans = "";    //Чистим
                        }
                    }
                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else if (patchver == "2.0")
                {
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        //Код для версии патча 2.0
                        if (_string.StartsWith("# CONTEXT"))               //Ждем начало блока
                        {
                            invalidformat = 2;//строка найдена, формат верен

                            if (_string.Split(' ')[3] != "itemAttr/UsageMessage")
                            {
                                _context = _string.Replace("# CONTEXT : ", ""); //Сохраняем коментарий

                                _string = _file.ReadLine();

                                if (_string.StartsWith("# ADVICE"))
                                {
                                    _advice = _string.Replace("# ADVICE : ", "");   //Вытаскиваем число предела

                                    _string = _file.ReadLine();
                                    while (!_string.StartsWith("# TRANSLATION"))  //Ждем начало следующего блока
                                    {
                                        _untrans = _untrans + _string + "\r\n";            //Пишем весь текст
                                        _string = _file.ReadLine();
                                    }
                                    if (_untrans.Length > 0)                    //Если текст есть, ищем перевод
                                    {
                                        _string = _file.ReadLine();
                                        while (!_string.StartsWith("# END"))      //Ждем конец блока
                                        {
                                            _trans = _trans + _string + "\r\n";
                                            _string = _file.ReadLine();
                                        }
                                        if (_untrans != "\r\n")
                                        {
                                            THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
                                        }
                                    }
                                    _untrans = "";  //Чистим
                                    _trans = "";    //Чистим
                                }
                            }
                        }
                    }
                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else
                {
                    MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    _file.Close();  //Закрываем файл
                    return false;
                }

                if (invalidformat == 1)
                {
                    MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    invalidformat = 0;
                    return false;
                }

                //progressBar.Value++;
            }

            //MessageBox.Show("111");

            if (verok == 1 & invalidformat != 1)
            {
                //Запись в dataGridVivwer
                for (int i = 0; i < ListFiles.Count; i++)
                {
                    //MessageBox.Show("ListFiles=" + ListFiles[i]);
                    THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesDataGridView.Rows.Add();
                    //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                    //dGFiles.Rows.Add();
                    //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                }
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                FVariant = " * RPG Maker Trans Patch " + patchver;

                ActiveForm.Text += FVariant;
            }
            else if (invalidformat == 1)
            {
                MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                return false;
            }

            //MessageBox.Show("111");
            //progressBar.Value = 0;
            return true;
        }

        private void SetDoublebuffered(bool value)
        {
            // Double buffering can make DGV slow in remote desktop
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = THFileElementsDataGridView.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(THFileElementsDataGridView, value, null);
            }
        }

        //int numberOfRows=500;
        private bool THFilesListBox_MouseClickBusy;
        private void THFilesListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (THFilesListBox_MouseClickBusy)
            {
                //return;
            }
            else
            {
                //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                //int index = THFilesListBox.SelectedIndex;
                //Thread actions = new Thread(new ParameterizedThreadStart((obj) => THFilesListBoxMouseClickEventActions(index)));
                //actions.Start();

                THFilesListBox_MouseClickBusy = true;

                //Пример с присваиванием Dataset. Они вроде быстро открываются, в отличие от List. Проверка не подтвердила ускорения, всё также
                //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application

                //THFileElementsDataGridView.DataSource = null;
                //THFileElementsDataGridView.RowCount = 100;

                //Пробовал также отсюда через BindingList
                //https://stackoverflow.com/questions/44433428/how-to-use-virtual-mode-for-large-data-in-datagridview
                //не помогает
                //var dataPopulateList = new BindingList<Block>(THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks);
                //THFileElementsDataGridView.DataSource = dataPopulateList;

                //Еще источник с рекомендацией ниже, но тоже от нее не заметил эффекта
                //https://stackoverflow.com/questions/3580237/best-way-to-fill-datagridview-with-large-amount-of-data
                //THFileElementsDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                //or even better .DisableResizing.
                //Most time consumption enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
                //THFileElementsDataGridView.RowHeadersVisible = false; // set it to false if not needed

                //https://www.codeproject.com/Questions/784355/How-to-solve-performance-issue-in-Datagridview-Min
                // Поменял List на BindingList и вроде чуть быстрее стало загружаться
                try
                {
                    if (THSelectedSourceType.Contains("RPGMTransPatch"))
                    {
                        //THFiltersDataGridView.Columns.Clear();

                        //сунул под try так как один раз здесь была ошибка о выходе за диапахон


                        //https://www.youtube.com/watch?v=wZ4BkPyZllY
                        //Thread t = new Thread(new ThreadStart(StartLoadingForm));
                        //t.Start();
                        //Thread.Sleep(100);

                        this.Cursor = Cursors.WaitCursor; // Поменять курсор на часики

                        //измерение времени выполнения
                        //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
                        System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
                        swatch.Start();

                        //https://stackoverflow.com/questions/778095/windows-forms-using-backgroundimage-slows-down-drawing-of-the-forms-controls
                        //THFileElementsDataGridView.SuspendDrawing();//используются оба SuspendDrawing и SuspendLayout для возможного ускорения
                        //THFileElementsDataGridView.SuspendLayout();//с этим вроде побыстрее чем с SuspendDrawing из ControlHelper

                        //THsplitContainerFilesElements.Panel2.Visible = false;//сделать невидимым родительский элемент на время

                        //Советы, после которых отображение ячеек стало во много раз быстрее, 
                        //https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/best-practices-for-scaling-the-windows-forms-datagridview-control
                        //Конкретно, поменял режим отображения строк(Rows) c AllCells на DisplayerCells, что ускорило отображение 3400к. строк в таблице в 100 раз, с 9с. до 0.09с. !

                        //THBS.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;
                        //THFileElementsDataGridView.DataSource = THBS;

                        //THFileElementsDataGridView.Invoke((Action)(() => THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks));
                        THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;//.GetRange(0, THRPGMTransPatchFilesFGetCellCount());


                        /*
                        //Virtual mode implementation
                        THFileElementsDataGridView.Rows.Clear();
                        THFileElementsDataGridView.Columns.Clear();
                        THFileElementsDataGridView.Columns.Add("Original", THMainDGVOriginalColumnName);
                        THFileElementsDataGridView.Columns.Add("Translation", THMainDGVTranslationColumnName);
                        if (THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count < numberOfRows)
                        {
                            THFileElementsDataGridView.RowCount = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count;
                        }
                        else
                        {
                            THFileElementsDataGridView.RowCount = numberOfRows;
                        }
                        */

                        //foreach (var sblock in THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks)
                        //{
                        //    THFileElementsDataGridView.Rows.Add(sblock.Original, sblock.Translation);
                        //}

                        //iGrid1.FillWithData(THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks);


                        //t.Abort();

                        //THsplitContainerFilesElements.Panel2.Visible = true;

                        THFileElementsDataGridView.Columns["Context"].Visible = false;
                        THFileElementsDataGridView.Columns["Status"].Visible = false;
                        THFiltersDataGridView.Enabled = true;

                        //THFileElementsDataGridView.ResumeLayout();
                        THFileElementsDataGridView.ResumeDrawing();

                        swatch.Stop();
                        string time = swatch.Elapsed.ToString();
                        FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>:" + THFilesListBox.SelectedItem.ToString() + "> Time:\"" + time + "\"\r\n", true);
                        //MessageBox.Show("Time: "+ time); // тут выводим результат в консоль


                        if (FVariant == " * RPG Maker Trans Patch 3")
                        {
                            THFileElementsDataGridView.Columns["Advice"].Visible = false;
                        }


                        this.Cursor = Cursors.Default; ;//Поменять курсор обратно на обычный

                        //MessageBox.Show("THFiltersDataGridView.Columns.Count=" + THFiltersDataGridView.Columns.Count
                        //    + "\r\nTHFileElementsDataGridView visible Columns Count=" + THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible));
                        if (THFiltersDataGridView.Columns.Count != THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible))
                        {
                            //int visibleindex = -1;
                            for (int cindx = 0; cindx < THFileElementsDataGridView.Columns.Count; cindx++)
                            {
                                if (THFileElementsDataGridView.Columns[cindx].Visible)
                                {
                                    //visibleindex += 1;
                                    //MessageBox.Show("THFileElementsDataGridView.Columns[cindx].Name="+ THFileElementsDataGridView.Columns[cindx].Name
                                    //    + "\r\nTHFileElementsDataGridView.Columns[cindx].HeaderText="+ THFileElementsDataGridView.Columns[cindx].HeaderText);
                                    THFiltersDataGridView.Columns.Add(THFileElementsDataGridView.Columns[cindx].Name, THFileElementsDataGridView.Columns[cindx].HeaderText);
                                    //THFiltersDataGridView.Columns[visibleindex].Width = THFileElementsDataGridView.Columns[cindx].Width;
                                }
                            }
                            THFiltersDataGridView.Rows.Add(1);
                            THFiltersDataGridView.CurrentRow.Selected = false;
                        }
                    }
                    else if (THSelectedSourceType.Contains("RPG Maker MV"))
                    {
                        THFileElementsDataGridView.DataSource = ds.Tables[THFilesListBox.SelectedIndex];
                    }


                    THFileElementsDataGridView.Columns["Original"].Name = THMainDGVOriginalColumnName;
                    THFileElementsDataGridView.Columns["Translation"].Name = THMainDGVTranslationColumnName;
                    THFileElementsDataGridView.Columns[THMainDGVOriginalColumnName].ReadOnly = true;
                    THSourceTextBox.Enabled = true;
                    THTargetTextBox.Enabled = true;
                    THTargetTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                }
                catch (Exception)
                {
                }

                //THFileElementsDataGridView.RowHeadersVisible = true; // set it to false if not needed

                THFilesListBox_MouseClickBusy = false;
            }
        }

        private void THFileElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Считывание значения ячейки в текстовое поле 1, вариант 2, для DataSet, ds.Tables[0]
                if (THSourceTextBox.Enabled && THFileElementsDataGridView.Rows.Count > 0 && e.RowIndex >= 0 && e.ColumnIndex >= 0) //Проверка на размер индексов, для избежания ошибки при попытке сортировки " должен быть положительным числом и его размер не должен превышать размер коллекции"
                {
                    THTargetTextBox.Clear();

                    if (!String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[THMainDGVOriginalColumnName].Value.ToString())) //проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                    {
                        THSourceTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells[THMainDGVOriginalColumnName].Value.ToString(); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                                                                                                                                                //https://github.com/caguiclajmg/WanaKanaSharp
                                                                                                                                                //if (GetLocaleLangCount(THSourceTextBox.Text, "hiragana") > 0)
                                                                                                                                                //{
                                                                                                                                                //    GetWords(THSourceTextBox.Text);
                                                                                                                                                //   var hepburnConverter = new HepburnConverter();
                                                                                                                                                //   WanaKana.ToRomaji(hepburnConverter, THSourceTextBox.Text); // hiragana
                                                                                                                                                //}
                                                                                                                                                //также по японо ыфуригане
                                                                                                                                                //https://docs.microsoft.com/en-us/uwp/api/windows.globalization.japanesephoneticanalyzer
                    }
                    if (!String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString())) //проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                    {
                        if (String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString()))
                        {
                            THTargetTextBox.Clear();
                        }

                        THTargetTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString(); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                    }

                    THInfoTextBox.Text = "";

                    if (!String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    {
                        //gem furigana
                        //https://github.com/helephant/Gem
                        //var furigana = new Furigana(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        //THInfoTextBox.Text += furigana.Reading + "\r\n";
                        //THInfoTextBox.Text += furigana.Expression + "\r\n";
                        //THInfoTextBox.Text += furigana.Hiragana + "\r\n";
                        //THInfoTextBox.Text += furigana.ReadingHtml + "\r\n";
                        THInfoTextBox.Text += THShowLangsOfString(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), "all"); //Show all detected languages count info
                    }
                }
                //--------Считывание значения ячейки в текстовое поле 1
            }
            catch
            {
            }
        }

        //Detect languages
        //source: https://stackoverflow.com/questions/15805859/detect-japanese-character-input-and-romajis-ascii
        private static IEnumerable<char> GetCharsInRange(string text, int min, int max)
        {
            //Usage:
            //var romaji = GetCharsInRange(searchKeyword, 0x0020, 0x007E);
            //var hiragana = GetCharsInRange(searchKeyword, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(searchKeyword, 0x30A0, 0x30FF);
            //var kanji = GetCharsInRange(searchKeyword, 0x4E00, 0x9FBF);
            return text.Where(e => e >= min && e <= max);
        }

        private string THShowLangsOfString(string target, string langlocale)
        {
            string ret = "";
            if (langlocale == "all")
            {
                var kanji = GetCharsInRange(target, 0x4E00, 0x9FBF);
                var romaji = GetCharsInRange(target, 0x0020, 0x007E);
                var hiragana = GetCharsInRange(target, 0x3040, 0x309F);
                var katakana = GetCharsInRange(target, 0x30A0, 0x30FF);

                ret += "Contains: \r\n";
                if (romaji.Any())
                {
                    ret += ("       romaji:" + GetLocaleLangCount(target, "romaji") + "\r\n");
                }
                if (kanji.Any())
                {
                    ret += ("       kanji:" + GetLocaleLangCount(target, "kanji") + "\r\n");
                }
                if (hiragana.Any())
                {
                    ret += ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + "\r\n");
                }
                if (katakana.Any())
                {
                    ret += ("       katakana:" + GetLocaleLangCount(target, "katakana") + "\r\n");
                }
                if (GetLocaleLangCount(target, "other") > 0)
                {
                    ret += ("       other:" + (GetLocaleLangCount(target, "other")) + "\r\n");
                }
            }
            else if (langlocale.ToLower() == "romaji")
            {
                return ("       romaji:" + GetLocaleLangCount(target, "romaji") + "\r\n");
            }
            else if (langlocale.ToLower() == "kanji")
            {
                return ("       kanji:" + GetLocaleLangCount(target, "kanji") + "\r\n");
            }
            else if (langlocale.ToLower() == "hiragana")
            {
                return ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + "\r\n");
            }
            else if (langlocale.ToLower() == "katakana")
            {
                return ("       katakana:" + GetLocaleLangCount(target, "katakana") + "\r\n");
            }
            else if (langlocale.ToLower() == "other")
            {
                return ("       other:" + (GetLocaleLangCount(target, "other")) + "\r\n");
            }

            return ret;
        }

        private int GetLocaleLangCount(string target, string langlocale)
        {
            //var romaji = GetCharsInRange(THSourceTextBox.Text, 0x0020, 0x007E);
            //var kanji = GetCharsInRange(THSourceTextBox.Text, 0x4E00, 0x9FBF);
            //var hiragana = GetCharsInRange(THSourceTextBox.Text, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(THSourceTextBox.Text, 0x30A0, 0x30FF);

            int romaji = (GetCharsInRange(target, 0x0020, 0x007E)).Count();
            int kanji = (GetCharsInRange(target, 0x4E00, 0x9FBF)).Count();
            int hiragana = (GetCharsInRange(target, 0x3040, 0x309F)).Count();
            int katakana = (GetCharsInRange(target, 0x30A0, 0x30FF)).Count();

            int all = (romaji + kanji + hiragana + katakana);
            if (langlocale.ToLower() == "all")
            {
                return all + (target.Length - all);
            }
            else if (langlocale.ToLower() == "romaji")
            {
                return (romaji);
            }
            else if (langlocale.ToLower() == "kanji")
            {
                return (kanji);
            }
            else if (langlocale.ToLower() == "hiragana")
            {
                return (hiragana);
            }
            else if (langlocale.ToLower() == "katakana")
            {
                return (katakana);
            }
            else if (langlocale.ToLower() == "other")
            {
                return (target.Length - all);
            }

            return all;
        }

        //Для функции перевода, чтобы не переводить, когда в тексте нет иероглифов
        private bool NoKanjiHiraganaKatakanaInTheString(string target)
        {
            if (GetLocaleLangCount(target, "kanji") == 0 && GetLocaleLangCount(target, "hiragana") == 0 && GetLocaleLangCount(target, "katakana") == 0)
            {
                return true;
            }
            return false;
        }

        private bool SelectedLocalePercentFromStringIsNotValid(string target, string langlocale = "romaji", float percent = 80)
        {
            if (langlocale == "romaji" && Settings.THOptionDontLoadStringIfRomajiPercentCheckBox.Checked)
            {
                return ((GetLocaleLangCount(target, langlocale) * 100) / GetLocaleLangCount(target, "all")) > int.Parse(Settings.THOptionDontLoadStringIfRomajiPercentTextBox.Text);
            }
            return false;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("THSelectedSourceType=" + THSelectedSourceType);
            if (THSelectedSourceType == "RPGMTransPatch" || THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
            {
                THInfoTextBox.Text = "Saving...";
                MessageBox.Show("THSelectedDir=" + THSelectedDir);
                SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver);
                THInfoTextBox.Text = "";
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog THSaveFolderBrowser = new FolderBrowserDialog
            {
                //MessageBox.Show(dirpath);
                SelectedPath = THSelectedDir //Установить начальный путь на тот, что был установлен при открытии.
            };

            if (THSaveFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                if (THSelectedSourceType == "RPGMTransPatch")
                {
                    if (SaveRPGMTransPatchFiles(THSaveFolderBrowser.SelectedPath, THRPGMTransPatchver))
                    {
                        THSelectedDir = THSaveFolderBrowser.SelectedPath;
                        MessageBox.Show("Сохранение завершено!");
                    }
                }
            }
        }

        public bool SaveRPGMTransPatchFiles(string SelectedDir, string patchver = "2.0")
        {
            string buffer;

            //Прогресс
            //pbstatuslabel.Visible = true;
            //pbstatuslabel.Text = "сохранение..";
            //progressBar.Maximum = 0;
            //for (int i = 0; i < ArrayTransFilses.Count; i++)
            //    for (int y = 0; y < ArrayTransFilses[i].blocks.Count; y++)
            //        progressBar.Maximum = progressBar.Maximum + ArrayTransFilses[i].blocks.Count;
            //MessageBox.Show(progressBar.Maximum.ToString());
            //progressBar.Value = 0;

            if (patchver == "3")
            {
                //запись в файл RPGMKTRANSPATCH строки > RPGMAKER TRANS PATCH V3
                //StreamWriter RPGMKTRANSPATCHwriter = new StreamWriter("RPGMKTRANSPATCH", true);
                //RPGMKTRANSPATCHwriter.WriteLine("> RPGMAKER TRANS PATCH V3");
                //RPGMKTRANSPATCHwriter.Close();

                for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                {
                    buffer = "> RPGMAKER TRANS PATCH FILE VERSION 3\r\n";
                    for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                    {
                        buffer += "> BEGIN STRING\r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Original + "\r\n";
                        //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                        //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                        string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                        //string str1 = "";
                        for (int g = 0; g < str.Count(); g++)
                        {
                            if (str.Count() > 1)
                            {
                                str[g] = str[g].Replace("\r", "");//очистка от знака переноса в отдельную переменную
                                buffer += "> CONTEXT: " + str[g] + "\r\n";
                            }
                            else
                            {
                                str[g] = str[g].Replace("\r", "");//очистка от знака переноса в отдельную переменную
                                if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == "\r\n")
                                {
                                    buffer += "> CONTEXT: " + str[g] + " < UNTRANSLATED\r\n";
                                }
                                else
                                {
                                    buffer += "> CONTEXT: " + str[g] + "\r\n";
                                }
                            }
                        }
                        //buffer += "\r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Translation + "\r\n";
                        buffer += "> END STRING\r\n\r\n";

                        //progressBar.Value++;
                        //MessageBox.Show(progressBar.Value.ToString());
                    }

                    if (!String.IsNullOrWhiteSpace(buffer))
                    {
                        if (!Directory.Exists(SelectedDir + "\\patch"))
                        {
                            Directory.CreateDirectory(SelectedDir + "\\patch");
                        }
                        String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                        File.WriteAllText(_path, buffer);
                        //buffer = "";
                    }
                }
            }
            else if (patchver == "2.0")
            {
                for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                {
                    buffer = "# RPGMAKER TRANS PATCH FILE VERSION 2.0\r\n";
                    for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                    {
                        buffer += "# TEXT STRING\r\n";
                        if (THRPGMTransPatchFiles[i].blocks[y].Translation == "\r\n")
                            buffer += "# UNTRANSLATED\r\n";
                        buffer += "# CONTEXT : " + THRPGMTransPatchFiles[i].blocks[y].Context + "\r\n";
                        buffer += "# ADVICE : " + THRPGMTransPatchFiles[i].blocks[y].Advice + "\r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Original;
                        buffer += "# TRANSLATION \r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Translation;
                        buffer += "# END STRING\r\n\r\n";
                    }
                    if (!String.IsNullOrWhiteSpace(buffer))
                    {
                        String _path = SelectedDir + "\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                        File.WriteAllText(_path, buffer);
                        //buffer = "";
                    }
                }
            }
            //pbstatuslabel.Visible = false;
            //pbstatuslabel.Text = string.Empty;
            return true;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THAboutForm AboutForm = new THAboutForm();
            AboutForm.Show();
        }

        private void THTargetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Ctrl+Del function
            //https://stackoverflow.com/questions/18543198/why-cant-i-press-ctrla-or-ctrlbackspace-in-my-textbox
            if (e.Control)
            {
                //if (e.KeyCode == Keys.A)
                //{
                //    THTargetTextBox.SelectAll();
                //}
                if (e.KeyCode == Keys.Back)
                {
                    e.SuppressKeyPress = true;
                    int selStart = THTargetTextBox.SelectionStart;
                    while (selStart > 0 && THTargetTextBox.Text.Substring(selStart - 1, 1) == " ")
                    {
                        selStart--;
                    }
                    int prevSpacePos = -1;
                    if (selStart != 0)
                    {
                        prevSpacePos = THTargetTextBox.Text.LastIndexOf(' ', selStart - 1);
                    }
                    THTargetTextBox.Select(prevSpacePos + 1, THTargetTextBox.SelectionStart - prevSpacePos - 1);
                    THTargetTextBox.SelectedText = "";
                }
            }
        }

        private void THFiltersDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            

            int cindx = e.ColumnIndex;
            //MessageBox.Show("e.ColumnIndex" + cindx);
            for (int i = 0; i < THFileElementsDataGridView.Rows.Count; i++) //сделать все видимыми
            {
                THFileElementsDataGridView.Rows[i].Visible = true;
            }

            bool allfiltersisempty = true;
            for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)//check if all filters is empty
            {
                if (THFiltersDataGridView.Rows[0].Cells[c].Value == null || string.IsNullOrEmpty(THFiltersDataGridView.Rows[0].Cells[c].Value.ToString()))
                {
                }
                else
                {
                    allfiltersisempty = false;
                    break;
                }
            }

            if (allfiltersisempty)//Возврат, если все фильтры пустые
            {
                return;
            }

            //http://www.cyberforum.ru/post5844571.html
            THFileElementsDataGridView.CurrentCell = null;
            for (int i = 0; i < THFileElementsDataGridView.Rows.Count; i++)
            {
                bool stringfound = false;//по умолчанию скрыть, не найдено
                for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
                {
                    if (THFiltersDataGridView.Rows[0].Cells[c].Value == null)
                    {
                    }
                    else
                    {
                        string THFilteringColumnValue = THFiltersDataGridView.Rows[0].Cells[c].Value.ToString();
                        if (string.IsNullOrEmpty(THFilteringColumnValue))
                        {
                        }
                        else
                        {
                            if (THFiltersDataGridView.Rows[0].Cells[c].Value == null)
                            {
                            }
                            else
                            {
                                foreach (DataGridViewColumn column in THFileElementsDataGridView.Columns)
                                {
                                    //MessageBox.Show("THFiltersDataGridView.Columns[cindx].Name=" + THFiltersDataGridView.Columns[e.ColumnIndex].Name
                                    //    + "\r\nTHFileElementsDataGridView.Columns[cindx].Name=" + THFileElementsDataGridView.Columns[cindx].Name);
                                    if (cindx < THFileElementsDataGridView.Columns.Count - 1 /*Контроль на превышение лимита колонок, на всякий*/ && THFiltersDataGridView.Columns[e.ColumnIndex].Name == THFileElementsDataGridView.Columns[cindx].Name)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        cindx += 1;
                                    }
                                }

                                if (THFileElementsDataGridView.Rows[i].Cells[cindx].Value == null)
                                {
                                    //THFileElementsDataGridView.Rows[i].Visible = false;
                                }
                                else if (THFileElementsDataGridView.Rows[i].Cells[cindx].Value.ToString().Contains(THFilteringColumnValue))
                                {
                                    stringfound = true; //строка найдена, показать
                                }
                                else
                                {
                                    //MessageBox.Show("THFileElementsDataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString()=" + THFileElementsDataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString());
                                    //THFileElementsDataGridView.Rows[i].Visible = false;
                                }
                            }
                        }
                    }
                }

                if (stringfound)
                {
                    THFileElementsDataGridView.Rows[i].Visible = true;
                }
                else
                {
                    THFileElementsDataGridView.Rows[i].Visible = false;
                }
            }
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THSettingsForm THSettings = new THSettingsForm();
            THSettings.Show();
        }

        private void THFileElementsDataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void THMain_Load(object sender, EventArgs e)
        {
        }

        //Пример виртуального режима
        //http://www.cyberforum.ru/post9306711.html
        private void THFileElementsDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //MessageBox.Show("THFileElementsDataGridView_CellValueNeeded");
            /*
            if (e.ColumnIndex == 0)
            {
                e.Value = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks[e.RowIndex].Original;
            }
            else
            {
                e.Value = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks[e.RowIndex].Translation;
            }
            */
        }

        private void THFileElementsDataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            //if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            //{
                //newRowNeeded = true;
                /*
                if (THFileElementsDataGridView.Rows.Count < THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count)
                {
                    THFileElementsDataGridView.Rows.Add();
                }
                */

                /*debug info
                //https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.forms.datagridview.scroll?view=netframework-4.8
                System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
                messageBoxCS.AppendFormat("{0} = {1}", "ScrollOrientation", e.ScrollOrientation);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "Type", e.Type);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "NewValue", e.NewValue);
                messageBoxCS.AppendLine();
                messageBoxCS.AppendFormat("{0} = {1}", "OldValue", e.OldValue);
                messageBoxCS.AppendLine();
                MessageBox.Show(messageBoxCS.ToString(), "Scroll Event");
                */


                /*
                if (THFileElementsDataGridView.Rows.Count+500 > THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count)
                {
                    THFileElementsDataGridView.RowCount = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks.Count;
                }
                else
                {
                    THFileElementsDataGridView.RowCount = THFileElementsDataGridView.Rows.Count + 500;
                }
                */
            //}


        }

        //bool newRowNeeded;
        private void THFileElementsDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            
            //if (e.RowIndex == numberOfRows /*newRowNeeded*/)
            //{
                //newRowNeeded = false;
                //numberOfRows = numberOfRows + 1;
                //THFileElementsDataGridView.Rows.Add();
            //}
            

        }

        private void THFileElementsDataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            //MessageBox.Show("hhhhhhhhhhhh");
            //newRowNeeded = true;
        }
    }

    //Материалы
    //Сортировка при виртуальном режиме DatagridView
    //http://qaru.site/questions/1486005/c-datagridview-virtual-mode-enable-sorting
    //c# - Поиск ячеек/строк по DataGridView
    //http://www.skillcoding.com/Default.aspx?id=151
    //Ошибка "Строку, связанную с положением CurrencyManager, нельзя сделать невидимой"
    //http://www.cyberforum.ru/csharp-beginners/thread757809.html
    //Виртуальный режим
    //https://stackoverflow.com/questions/31458197/how-to-sort-datagridview-data-when-virtual-mode-enable
}