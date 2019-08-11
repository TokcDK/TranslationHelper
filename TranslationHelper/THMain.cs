using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THMain : Form
    {
        //string THLog;
        //public IniFile THConfigINI = new IniFile("TranslationHelperConfig.ini");
        public THSettingsForm Settings;
        //public const string THStrDGTranslationColumnName = "Translation";
        //public const string THStrDGOriginalColumnName = "Original";
        private readonly THLang LangF;

        public static string apppath = Application.StartupPath.ToString();
        private string extractedpatchpath = "";

        private string FVariant = "";
        //private BindingList<THRPGMTransPatchFile> THRPGMTransPatchFiles; //Все файлы
        //DataTable fileslistdt = new DataTable();
        public DataSet THFilesElementsDataset;
        public DataSet THFilesElementsDatasetInfo;
        //DataTable THFilesElementsDatatable;
        //private BindingSource THBS = new BindingSource();

        public string THSelectedDir;
        public string THRPGMTransPatchver;
        public string THSelectedSourceType;

        //Language strings
        public string THMainDGVOriginalColumnName;
        public string THMainDGVTranslationColumnName;

        //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
        DataColumn[] keyColumns = new DataColumn[1];

        //Translation cache
        //DataSet THTranslationCache;
        public static string THTranslationCachePath;

        public THMain()
        {
            InitializeComponent();
            LangF = new THLang();
            Settings = new THSettingsForm();

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

            THFilesElementsDataset = new DataSet();
            THFilesElementsDatasetInfo = new DataSet();

            //DataSet THTranslationCache; THTranslationCache = new DataSet();
            THTranslationCachePath = apppath + "\\DB\\THTranslationCache.cmx";

            //THRPGMTransPatchFiles = new BindingList<THRPGMTransPatchFile>();
            //dt = new DataTable();

            //THFileElementsDataGridView set doublebuffered to true
            SetDoublebuffered(true);
            if (File.Exists(apppath + "\\TranslationHelper.log"))
            {
                File.Delete(apppath + "\\TranslationHelper.log");
            }

            //Test Проверка ключа Git для планируемой функции использования Git
            //string GitPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\GitForWindows", "InstallPath", null).ToString();
        }

        bool THdebug = true;
        StringBuilder THsbLog = new StringBuilder();
        public void LogToFile(string s, bool w = false)
        {
            if (THdebug)
            {
                if (w)
                {
                    if (THsbLog.Length == 0)
                    {
                        FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>" + s + "\r\n", true);
                    }
                    else
                    {
                        FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>" + THsbLog + "\r\n", true);
                        THsbLog.Clear();
                    }
                }
                else
                {
                    THsbLog.Append(DateTime.Now + " >>" + s + "\r\n");
                }
            }
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

        bool IsOpeningInProcess = false;
        private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsOpeningInProcess)//Do nothing if user will try to use Open menu before previous will be finished
            {
            }
            else
            {
                IsOpeningInProcess = true;

                //об сообщении Освобождаемый объект никогда не освобождается и почему using здесь
                //https://stackoverflow.com/questions/2926869/do-you-need-to-dispose-of-objects-and-set-them-to-null
                using (OpenFileDialog THFOpen = new OpenFileDialog())
                {
                    THFOpen.Filter = "All compatible|*.exe;RPGMKTRANSPATCH;*.json|RPGMakerTrans patch|RPGMKTRANSPATCH|RPG maker execute(*.exe)|*.exe";

                    if (THFOpen.ShowDialog() == DialogResult.OK)
                    {
                        if (THFOpen.OpenFile() != null)
                        {
                            //THActionProgressBar.Visible = true;
                            ProgressInfo(true, "opening..");

                            THCleanupThings();

                            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                            //Thread open = new Thread(new ParameterizedThreadStart((obj) => GetSourceType(THFOpen.FileName)));
                            //open.Start();

                            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
                            await Task.Run(() => THSelectedSourceType = GetSourceType(THFOpen.FileName));

                            //THSelectedSourceType = GetSourceType(THFOpen.FileName);

                            //THActionProgressBar.Visible = false;
                            ProgressInfo(false, "");

                            if (string.IsNullOrEmpty(THSelectedSourceType))
                            {
                                THMsg.Show("Still can't open the source or was error in open. Try to report about this to devs.");
                            }
                            else
                            {
                                THMsg.Show(THSelectedSourceType + " loaded!");

                                editToolStripMenuItem.Enabled = true;
                                viewToolStripMenuItem.Enabled = true;
                                loadTranslationToolStripMenuItem.Enabled = true;
                                loadTrasnlationAsToolStripMenuItem.Enabled = true;

                                ActiveForm.Text += FVariant;
                            }

                        }
                    }
                }

                IsOpeningInProcess = false;
            }
        }

        private void THCleanupThings()
        {
            try
            {
                //Reset strings
                ActiveForm.Text = "Translation Helper by DenisK";
                THInfoTextBox.Text = string.Empty;
                THSourceTextBox.Text = string.Empty;
                THTargetTextBox.Text = string.Empty;

                //Clean data
                THFilesListBox.Items.Clear();
                THFilesElementsDataset.Reset();
                THFilesElementsDatasetInfo.Reset();
                THFileElementsDataGridView.Columns.Clear();
                //THFileElementsDataGridView.Rows.Clear();

                //Disable items
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                editToolStripMenuItem.Enabled = false;
                viewToolStripMenuItem.Enabled = false;
                saveTranslationToolStripMenuItem.Enabled = false;
                writeTranslationInGameToolStripMenuItem.Enabled = false;
                loadTranslationToolStripMenuItem.Enabled = false;
                loadTrasnlationAsToolStripMenuItem.Enabled = false;
                THSourceTextBox.Enabled = false;
                THTargetTextBox.Enabled = false;
            }
            catch
            {

            }
        }

        public DirectoryInfo mvdatadir;
        private string GetSourceType(String sPath)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            THSelectedDir = dir.ToString();
            //MessageBox.Show("sPath=" + sPath);
            if (sPath.ToUpper().Contains("\\RPGMKTRANSPATCH"))
            {
                return RPGMTransPatchPrepare(sPath);
                //return "RPGMTransPatch";
            }
            else if (sPath.ToLower().Contains("\\game.exe"))
            {
                if (File.Exists(THSelectedDir + "\\www\\data\\system.json"))
                {
                    THSelectedDir += "\\www\\data";
                    //var MVJsonFIles = new List<string>();
                    mvdatadir = new DirectoryInfo(Path.GetDirectoryName(THSelectedDir+ "\\"));
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

                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);
                        THFilesListBox.Invoke((Action)(() => THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                    }
                    return "RPG Maker MV";
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
                        //THRPGMTransPatchFiles.Clear();
                        //THFilesElementsDataset.Clear();

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
                            THRPGMTransPatchver = "2";
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
                            //MessageBox.Show(THSelectedSourceType + " loaded!");
                            //ProgressInfo(false, "");
                            return "RPG Maker game with RPGMTransPatch";
                        }
                    }
                }

            }
            else if (sPath.ToLower().Contains(".json"))
            {
                if (OpenRPGMakerMVjson(sPath))
                {
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        THFilesListBox.Invoke((Action)(() => THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName)));//add all dataset tables names to the ListBox

                    }
                    return "RPG Maker MV json";
                }
            }

            //MessageBox.Show("Uncompatible source or problem with opening.");
            return string.Empty;
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
            //THRPGMTransPatchFiles.Clear();
            //THFilesElementsDataset.Clear();

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
                THRPGMTransPatchver = "2";
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
                //MessageBox.Show(THSelectedSourceType + " loaded!");
                //THShowMessage(THSelectedSourceType + " loaded!");
                //ProgressInfo(false, "");
                //LogToFile("", true);
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
                if (THFilesElementsDataset.Tables.Contains(Jsonname))
                {
                    //MessageBox.Show("true!");
                    return true;
                }
                ProgressInfo(true, "opening file: " + Jsonname + ".json");
                string jsondata = File.ReadAllText(sPath); // get json data

                THFilesElementsDataset.Tables.Add(Jsonname); // create table with json name
                THFilesElementsDataset.Tables[Jsonname].Columns.Add("Original"); //create Original column
                THFilesElementsDatasetInfo.Tables.Add(Jsonname); // create table with json name
                THFilesElementsDatasetInfo.Tables[Jsonname].Columns.Add("Original"); //create Original column
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

                if (THFilesElementsDataset.Tables[Jsonname].Rows.Count > 0)
                {
                    THFilesElementsDataset.Tables[Jsonname].Columns.Add("Translation");
                }
                else
                {
                    THFilesElementsDataset.Tables.Remove(Jsonname); // remove table if was no items added
                    THFilesElementsDatasetInfo.Tables.Remove(Jsonname); // remove table if was no items added
                }

                return ret;
            }
            catch
            {
                return false;
            }

        }

        //private void GetDataFromRPGMakerMVjsonOfType(string Jsonname, string JsonElement)
        //{
        //    if (string.IsNullOrEmpty(JsonElement) || SelectedLocalePercentFromStringIsNotValid(JsonElement) || GetAlreadyAddedInTable(Jsonname, JsonElement))
        //    {
        //    }
        //    else
        //    {
        //        THFilesElementsDataset.Tables[Jsonname].Rows.Add(JsonElement);
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonItemsArmorsWeapons(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonItemsArmorsWeapons>>(jsondata))
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonSkills(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonSkills>>(jsondata))
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message1);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message2);
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonStates(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonStates>>(jsondata))
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message1);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message2);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message3);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Message4);
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonClassesEnemiesTilesets(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonClassesEnemiesTilesets>>(jsondata))
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonAnimationsMapInfosTroops(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        foreach (var JsonElement in JsonConvert.DeserializeObject<List<RPGMakerMVjsonAnimationsMapInfosTroops>>(jsondata))
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonActors(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        var actors = RPGMakerMVjsonActors.FromJson(jsondata);//JsonConvert.DeserializeObject<List<RPGMakerMVjsonActors>>(jsondata)
        //        foreach (var JsonElement in actors)
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Nickname);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //                GetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Profile);
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonCommonEvents(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        //info RPG Maker MV Event codes
        //        //https://forums.rpgmakerweb.com/index.php?threads/extract-events-to-text-file.17444/
        //        //https://forums.rpgmakerweb.com/index.php?threads/cross-reference-tool.72563/
        //        //https://pastebin.com/JyRTdq0b
        //        //https://pastebin.com/eJx0EvXB
        //        //    case 401 : return 'Show Text';              break;
        //        //    case 102 : return 'Show Choices';           break;
        //        //    case 103 : return 'Input Number';           break;
        //        //    case 104 : return 'Select Item';            break;
        //        //    case 405 : return 'Show Scrolling Text';    break;
        //        //    case 111 : return 'Conditional Branch';     break;
        //        //    case 119 : return 'Common Event';           break;
        //        //    case 121 : return 'Control Switches';       break;
        //        //    case 122 : return 'Control Variables';      break;
        //        //    case 125 : return 'Change Gold';            break;
        //        //    case 126 : return 'Change Items';           break;
        //        //    case 127 : return 'Change Weapons';         break;
        //        //    case 128 : return 'Change Armors';          break;
        //        //    case 129 : return 'Change Party Member';    break;
        //        //    case 201 : return 'Transfer Player';        break;
        //        //    case 202 : return 'Set Vehicle Location';   break;
        //        //    case 203 : return 'Set Event Location';     break;
        //        //    case 505 : return 'Set Movement Route';     break;
        //        //    case 212 : return 'Show Animation';         break;
        //        //    case 231 : return 'Show Picture';           break;
        //        //    case 232 : return 'Move Picture';           break;
        //        //    case 285 : return 'Get Location Info';      break;
        //        //    case 301 : return 'Battle Processing';      break;
        //        //    case 302 :
        //        //    case 605 : return 'Shop Processing';        break;
        //        //    case 303 : return 'Name Input Processing';  break;
        //        //    case 311 : return 'Change HP';              break;
        //        //    case 312 : return 'Change MP';              break;
        //        //    case 326 : return 'Change TP';              break;
        //        //    case 313 : return 'Change State';           break;
        //        //    case 314 : return 'Recover All';            break;
        //        //    case 315 : return 'Change EXP';             break;
        //        //    case 316 : return 'Change Level';           break;
        //        //    case 317 : return 'Change Parameter';       break;
        //        //    case 318 : return 'Change Skill';           break;
        //        //    case 319 : return 'Change Equipment';       break;
        //        //    case 320 : return 'Change Name';            break;
        //        //    case 321 : return 'Change Class';           break;
        //        //    case 322 : return 'Change Actor Images';    break;
        //        //    case 324 : return 'Change Nickname';        break;
        //        //    case 325 : return 'Change Profile';         break;
        //        //    case 331 : return 'Change Enemy HP';        break;
        //        //    case 332 : return 'Change Enemy MP';        break;
        //        //    case 342 : return 'Change Enemy TP';        break;
        //        //    case 333 : return 'Change Enemy State';     break;
        //        //    case 336 : return 'Enemy Transform';        break;
        //        //    case 337 : return 'Show Battle Animation';  break;
        //        //    case 339 : return 'Force Action';           break;
        //        //
        //        //Will be handled:
        //        //401 - Show text (mergeable)
        //        //102 - Show choices (Choices list)
        //        //402 - Choice for choices - ignore because already in 102
        //        //405 - Show Scrolling Text (mergeable)
        //        //108 and 408 - Comment - can be ignored because it is for dev suppose
        //        //normal example about command values adding: https://galvs-scripts.com/galvs-party-select/


        //        //var commoneventsdata = JsonConvert.DeserializeObject<List<RPGMakerMVjsonCommonEvents>>(jsondata);
        //        var commoneventsdata = RpgMakerMVjsonCommonEvents.FromJson(jsondata);

        //        for (int i = 1; i < commoneventsdata.Count; i++)
        //        {
        //            //FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

        //            //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
        //            //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

        //            string eventname = commoneventsdata[i].Name;
        //            if (string.IsNullOrEmpty(eventname) || GetAlreadyAddedInTable(Jsonname, eventname))
        //            {
        //            }
        //            else //if code not equal old code and newline is not empty
        //            {
        //                THFilesElementsDataset.Tables[Jsonname].Rows.Add(eventname); //add event name to new row
        //            }

        //            string newline = "";
        //            //int commandcode;
        //            //int commandoldcode = 999999;
        //            bool textaddingstarted = false;


        //            int CommandsCount = commoneventsdata[i].List.Count;
        //            for (int c = 0; c < CommandsCount; c++)
        //            {
        //                if (textaddingstarted)
        //                {
        //                    if (commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
        //                    {
        //                        newline += commoneventsdata[i].List[c].Parameters[0].String;

        //                        if (c < CommandsCount - 1 && commoneventsdata[i].List[c].Code == commoneventsdata[i].List[c + 1].Code)
        //                        {
        //                            newline += "\r\n";
        //                        }
        //                        else
        //                        {
        //                            if (string.IsNullOrEmpty(newline))
        //                            {
        //                                if (textaddingstarted)
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted is true and newline is empty\r\n";
        //                                    textaddingstarted = false;
        //                                }
        //                            }
        //                            else //if code not equal old code and newline is not empty
        //                            {
        //                                if (GetAlreadyAddedInTable(Jsonname, newline))
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 401/405 newline already in table=\"" + newline + "\"\r\n";
        //                                    newline = ""; //clear text data
        //                                    if (textaddingstarted)
        //                                    {
        //                                        textaddingstarted = false;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted=true added newline=\"" + newline + "\"\r\n";
        //                                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
        //                                    newline = ""; //clear text data
        //                                    textaddingstarted = false;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                else if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105)
        //                {
        //                    if (string.IsNullOrEmpty(newline))
        //                    {
        //                        if (textaddingstarted)
        //                        {
        //                            //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline is empty\r\n";
        //                            textaddingstarted = false;
        //                        }
        //                    }
        //                    else //if code not equal old code and newline is not empty
        //                    {
        //                        if (GetAlreadyAddedInTable(Jsonname, newline))
        //                        {
        //                            if (textaddingstarted)
        //                            {
        //                                //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline already in table=\"" + newline + "\"\r\n";
        //                                textaddingstarted = false;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //THLog += DateTime.Now + " >>: Code 101/105 newline is not empty=\"" + newline + "\"\r\n";
        //                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
        //                            newline = ""; //clear text data
        //                            textaddingstarted = false;
        //                        }
        //                    }

        //                    textaddingstarted = true;
        //                }
        //                else if (commoneventsdata[i].List[c].Code == 102)
        //                {
        //                    for (int i1 = 0; i1 < commoneventsdata[i].List[c].Parameters[0].AnythingArray.Count; i1++)
        //                    {
        //                        if (string.IsNullOrEmpty(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String))
        //                        {
        //                            if (textaddingstarted)
        //                            {
        //                                //THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
        //                                textaddingstarted = false;
        //                            }
        //                        }
        //                        else //if code not equal old code and newline is not empty
        //                        {
        //                            if (GetAlreadyAddedInTable(Jsonname, commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String))
        //                            {
        //                                //THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
        //                                if (textaddingstarted)
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
        //                                    textaddingstarted = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
        //                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String); //Save text to new row
        //                                if (string.IsNullOrEmpty(newline))
        //                                {
        //                                }
        //                                else
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 102 added schoice and also newline is not empty, set empty\r\n";
        //                                    newline = ""; //clear text data
        //                                }
        //                                if (textaddingstarted)
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 102 added schoice and also textaddingstarted is true , set false\r\n";
        //                                    textaddingstarted = false;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonMap(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        //bool eventsdone = false;
        //        //bool geteventnamenotedone = false;

        //        var map = JsonConvert.DeserializeObject<RPGMakerMVjsonMap>(jsondata);

        //        if (map.Events.Length > 1) //first event is empty
        //        {
        //            //Map displayed name
        //            if (string.IsNullOrEmpty(map.DisplayName) || SelectedLocalePercentFromStringIsNotValid(map.DisplayName) || GetAlreadyAddedInTable(Jsonname, map.DisplayName))
        //            {
        //            }
        //            else
        //            {
        //                //MessageBox.Show("map.DisplayName:" + map.DisplayName);
        //                THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.DisplayName);
        //            }
        //            //Map note
        //            if (string.IsNullOrEmpty(map.Note) || SelectedLocalePercentFromStringIsNotValid(map.Note) || GetAlreadyAddedInTable(Jsonname, map.Note))
        //            {
        //            }
        //            else
        //            {
        //                //MessageBox.Show("map.Note:" + map.Note);
        //                THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.Note);
        //            }

        //            //string prevval = "";
        //            foreach (RPGMakerMVjsonMapEvent ev in map.Events)
        //            {
        //                if (ev == null)
        //                {
        //                }
        //                else
        //                {
        //                    //event name
        //                    if (string.IsNullOrEmpty(ev.Name) || ev.Name.StartsWith("EV") || SelectedLocalePercentFromStringIsNotValid(ev.Name) || GetAlreadyAddedInTable(Jsonname, ev.Name))
        //                    {
        //                    }
        //                    else
        //                    {
        //                        //MessageBox.Show("map.Events add name"+ ev.Name);
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Name);
        //                        //prevval = ev.Name;
        //                    }
        //                    //event note
        //                    if (string.IsNullOrEmpty(ev.Note) || SelectedLocalePercentFromStringIsNotValid(ev.Note) || GetAlreadyAddedInTable(Jsonname, ev.Note))
        //                    {
        //                    }
        //                    else
        //                    {
        //                        //MessageBox.Show("map.Events add note:" + ev.Note);
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Note);
        //                    }

        //                    //event parameters
        //                    foreach (RPGMakerMVjsonMapPage page in ev.Pages)
        //                    {
        //                        foreach (RPGMakerMVjsonMapPageList lst in page.List)
        //                        {
        //                            foreach (var parameter in lst.Parameters)
        //                            {
        //                                if (parameter == null)
        //                                {

        //                                }
        //                                else if (parameter.GetType().Name == "String")
        //                                {
        //                                    string pstring = parameter.ToString();
        //                                    if (string.IsNullOrEmpty(pstring) || HasNOJPcharacters(pstring) || SelectedLocalePercentFromStringIsNotValid(pstring) || GetAlreadyAddedInTable(Jsonname, pstring))
        //                                    {

        //                                    }
        //                                    else
        //                                    {
        //                                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(pstring);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool GetDataFromRPGMakerMVjsonSystem(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
        //        var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

        //        //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

        //        if (systemdata.GameTitle == null || string.IsNullOrEmpty(systemdata.GameTitle))
        //        {
        //        }
        //        else
        //        {
        //            THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);
        //        }

        //        if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
        //        {
        //        }
        //        else
        //        {
        //            foreach (string armortype in systemdata.ArmorTypes)
        //            {
        //                if (string.IsNullOrEmpty(armortype))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
        //                }
        //            }
        //        }
        //        if (systemdata.Elements == null || systemdata.Elements.Length < 1)
        //        {

        //        }
        //        else
        //        {
        //            foreach (string element in systemdata.Elements)
        //            {
        //                if (string.IsNullOrEmpty(element))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
        //                }
        //            }
        //        }
        //        if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
        //        {

        //        }
        //        else
        //        {
        //            foreach (string equipType in systemdata.EquipTypes)
        //            {
        //                if (string.IsNullOrEmpty(equipType))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
        //                }
        //            }
        //        }
        //        if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
        //        {

        //        }
        //        else
        //        {
        //            foreach (string skillType in systemdata.skillTypes)
        //            {
        //                if (string.IsNullOrEmpty(skillType))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
        //                }
        //            }
        //        }
        //        if (systemdata.Switches == null || systemdata.Switches.Length < 1)
        //        {

        //        }
        //        else
        //        {
        //            foreach (string _switch in systemdata.Switches)
        //            {
        //                if (string.IsNullOrEmpty(_switch))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
        //                }
        //            }
        //        }
        //        if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
        //        {

        //        }
        //        else
        //        {
        //            foreach (string weaponType in systemdata.WeaponTypes)
        //            {
        //                if (string.IsNullOrEmpty(weaponType))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
        //                }
        //            }
        //        }
        //        if (systemdata.Terms == null)
        //        {

        //        }
        //        else
        //        {
        //            foreach (var basic in systemdata.Terms.Basic)
        //            {
        //                if (string.IsNullOrEmpty(basic))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
        //                }
        //            }
        //            foreach (var command in systemdata.Terms.Commands)
        //            {
        //                if (string.IsNullOrEmpty(command))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
        //                }
        //            }

        //            foreach (string param in systemdata.Terms.Params)
        //            {
        //                if (string.IsNullOrEmpty(param))
        //                {

        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
        //                }
        //            }

        //            foreach (var Message in systemdata.Terms.Messages)
        //            {
        //                THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
        //            }
        //        }
        //        //FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
        //        //THLog = "";

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool FillDSTableWithJsonValues(string Jsonname, string jsondata, bool name = false, bool description = false, bool displayname = false, bool note = false, bool message1 = false, bool message2 = false, bool message3 = false, bool message4 = false, bool nickname = false, bool profile = false, bool maps = false, bool cmnevents = false, bool system = false)
        //{
        //    try
        //    {
        //        if (name)
        //        {
        //            foreach (RPGMakerMVjsonName Name in JsonConvert.DeserializeObject<List<RPGMakerMVjsonName>>(jsondata))
        //            {
        //                if (Name == null || string.IsNullOrEmpty(Name.Name) || SelectedLocalePercentFromStringIsNotValid(Name.Name) || GetAlreadyAddedInTable(Jsonname, Name.Name))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Name.Name);
        //                }
        //            }
        //        }
        //        if (description)
        //        {
        //            foreach (RPGMakerMVjsonDescription Description in JsonConvert.DeserializeObject<List<RPGMakerMVjsonDescription>>(jsondata))
        //            {
        //                if (Description == null || string.IsNullOrEmpty(Description.Description) || SelectedLocalePercentFromStringIsNotValid(Description.Description) || GetAlreadyAddedInTable(Jsonname, Description.Description))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Description.Description);
        //                }
        //            }
        //        }
        //        if (displayname)
        //        {
        //            foreach (RPGMakerMVjsonDisplayName DisplayName in JsonConvert.DeserializeObject<List<RPGMakerMVjsonDisplayName>>(jsondata))
        //            {
        //                if (DisplayName == null || string.IsNullOrEmpty(DisplayName.DisplayName) || SelectedLocalePercentFromStringIsNotValid(DisplayName.DisplayName) || GetAlreadyAddedInTable(Jsonname, DisplayName.DisplayName))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(DisplayName.DisplayName);
        //                }
        //            }
        //        }
        //        if (note)
        //        {
        //            foreach (RPGMakerMVjsonNote Note in JsonConvert.DeserializeObject<List<RPGMakerMVjsonNote>>(jsondata))
        //            {
        //                if (Note == null || string.IsNullOrEmpty(Note.Note) || SelectedLocalePercentFromStringIsNotValid(Note.Note) || GetAlreadyAddedInTable(Jsonname, Note.Note))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Note.Note);
        //                }
        //            }
        //        }
        //        if (message1)
        //        {
        //            foreach (RPGMakerMVjsonMessage1 Message1 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage1>>(jsondata))
        //            {
        //                if (Message1 == null || string.IsNullOrEmpty(Message1.Message1) || SelectedLocalePercentFromStringIsNotValid(Message1.Message1) || GetAlreadyAddedInTable(Jsonname, Message1.Message1))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message1.Message1);
        //                }
        //            }
        //        }
        //        if (message2)
        //        {
        //            foreach (RPGMakerMVjsonMessage2 Message2 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage2>>(jsondata))
        //            {
        //                if (Message2 == null || string.IsNullOrEmpty(Message2.Message2) || SelectedLocalePercentFromStringIsNotValid(Message2.Message2) || GetAlreadyAddedInTable(Jsonname, Message2.Message2))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message2.Message2);
        //                }
        //            }
        //        }
        //        if (message3)
        //        {
        //            foreach (RPGMakerMVjsonMessage3 Message3 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage3>>(jsondata))
        //            {
        //                if (Message3 == null || string.IsNullOrEmpty(Message3.Message3) || SelectedLocalePercentFromStringIsNotValid(Message3.Message3) || GetAlreadyAddedInTable(Jsonname, Message3.Message3))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message3.Message3);
        //                }
        //            }
        //        }
        //        if (message4)
        //        {
        //            foreach (RPGMakerMVjsonMessage4 Message4 in JsonConvert.DeserializeObject<List<RPGMakerMVjsonMessage4>>(jsondata))
        //            {
        //                if (Message4 == null || string.IsNullOrEmpty(Message4.Message4) || SelectedLocalePercentFromStringIsNotValid(Message4.Message4) || GetAlreadyAddedInTable(Jsonname, Message4.Message4))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message4.Message4);
        //                }
        //            }
        //        }
        //        if (nickname)
        //        {
        //            foreach (RPGMakerMVjsonNickname Nickname in JsonConvert.DeserializeObject<List<RPGMakerMVjsonNickname>>(jsondata))
        //            {
        //                if (Nickname == null || string.IsNullOrEmpty(Nickname.Nickname) || SelectedLocalePercentFromStringIsNotValid(Nickname.Nickname) || GetAlreadyAddedInTable(Jsonname, Nickname.Nickname))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Nickname.Nickname);
        //                }
        //            }
        //        }
        //        if (profile)
        //        {
        //            foreach (RPGMakerMVjsonProfile Profile in JsonConvert.DeserializeObject<List<RPGMakerMVjsonProfile>>(jsondata))
        //            {
        //                if (Profile == null || string.IsNullOrEmpty(Profile.Profile) || SelectedLocalePercentFromStringIsNotValid(Profile.Profile) || GetAlreadyAddedInTable(Jsonname, Profile.Profile))
        //                {
        //                }
        //                else
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Profile.Profile);
        //                }
        //            }
        //        }

        //        if (cmnevents)
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
        //                //FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

        //                //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
        //                //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

        //                string eventname = commoneventsdata[i].Name;
        //                if (string.IsNullOrEmpty(eventname) || GetAlreadyAddedInTable(Jsonname, eventname))
        //                {
        //                }
        //                else //if code not equal old code and newline is not empty
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(eventname); //add event name to new row
        //                }

        //                string newline = "";
        //                //int commandcode;
        //                //int commandoldcode = 999999;
        //                bool textaddingstarted = false;


        //                int CommandsCount = commoneventsdata[i].List.Count;
        //                for (int c = 0; c < CommandsCount; c++)
        //                {
        //                    if (textaddingstarted)
        //                    {
        //                        if (commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
        //                        {
        //                            newline += commoneventsdata[i].List[c].Parameters[0];

        //                            if (c < CommandsCount - 1 && commoneventsdata[i].List[c].Code == commoneventsdata[i].List[c + 1].Code)
        //                            {
        //                                newline += "\r\n";
        //                            }
        //                            else
        //                            {
        //                                if (string.IsNullOrEmpty(newline))
        //                                {
        //                                    if (textaddingstarted)
        //                                    {
        //                                        //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted is true and newline is empty\r\n";
        //                                        textaddingstarted = false;
        //                                    }
        //                                }
        //                                else //if code not equal old code and newline is not empty
        //                                {
        //                                    if (GetAlreadyAddedInTable(Jsonname, newline))
        //                                    {
        //                                        //THLog += DateTime.Now + " >>: Code 401/405 newline already in table=\"" + newline + "\"\r\n";
        //                                        newline = ""; //clear text data
        //                                        if (textaddingstarted)
        //                                        {
        //                                            textaddingstarted = false;
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        //THLog += DateTime.Now + " >>: Code 401/405 textaddingstarted=true added newline=\"" + newline + "\"\r\n";
        //                                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
        //                                        newline = ""; //clear text data
        //                                        textaddingstarted = false;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105)
        //                    {
        //                        if (string.IsNullOrEmpty(newline))
        //                        {
        //                            if (textaddingstarted)
        //                            {
        //                                //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline is empty\r\n";
        //                                textaddingstarted = false;
        //                            }
        //                        }
        //                        else //if code not equal old code and newline is not empty
        //                        {
        //                            if (GetAlreadyAddedInTable(Jsonname, newline))
        //                            {
        //                                if (textaddingstarted)
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 101/105 textaddingstarted is true and newline already in table=\"" + newline + "\"\r\n";
        //                                    textaddingstarted = false;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //THLog += DateTime.Now + " >>: Code 101/105 newline is not empty=\"" + newline + "\"\r\n";
        //                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(newline); //Save text to new row
        //                                newline = ""; //clear text data
        //                                textaddingstarted = false;
        //                            }
        //                        }

        //                        textaddingstarted = true;
        //                    }
        //                    else if (commoneventsdata[i].List[c].Code == 102)
        //                    {
        //                        JArray choices = JArray.Parse(commoneventsdata[i].List[c].Parameters[0].ToString());

        //                        foreach (var choice in choices)
        //                        {
        //                            string schoice = choice.ToString();
        //                            if (string.IsNullOrEmpty(schoice))
        //                            {
        //                                if (textaddingstarted)
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 102 textaddingstarted is true and schoice is empty\r\n";
        //                                    textaddingstarted = false;
        //                                }
        //                            }
        //                            else //if code not equal old code and newline is not empty
        //                            {
        //                                if (GetAlreadyAddedInTable(Jsonname, schoice))
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 102 newline already in table=\"" + newline + "\"\r\n";
        //                                    if (textaddingstarted)
        //                                    {
        //                                        //THLog += DateTime.Now + " >>: Code 102 newline already in table and also textaddingstarted is true , set false\r\n";
        //                                        textaddingstarted = false;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    //THLog += DateTime.Now + " >>: Code 102 added schoice=\"" + schoice + "\"\r\n";
        //                                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(schoice); //Save text to new row
        //                                    if (string.IsNullOrEmpty(newline))
        //                                    {
        //                                    }
        //                                    else
        //                                    {
        //                                        //THLog += DateTime.Now + " >>: Code 102 added schoice and also newline is not empty, set empty\r\n";
        //                                        newline = ""; //clear text data
        //                                    }
        //                                    if (textaddingstarted)
        //                                    {
        //                                        //THLog += DateTime.Now + " >>: Code 102 added schoice and also textaddingstarted is true , set false\r\n";
        //                                        textaddingstarted = false;
        //                                    }
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //                //FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
        //                //THLog = "";
        //            }

        //        }
        //        if (maps)
        //        {
        //            //bool eventsdone = false;
        //            //bool geteventnamenotedone = false;

        //            var map = JsonConvert.DeserializeObject<RPGMakerMVjsonMap>(jsondata);

        //            if (map.Events.Length > 1) //first event is empty
        //            {
        //                //Map displayed name
        //                if (string.IsNullOrEmpty(map.DisplayName) || SelectedLocalePercentFromStringIsNotValid(map.DisplayName) || GetAlreadyAddedInTable(Jsonname, map.DisplayName))
        //                {
        //                }
        //                else
        //                {
        //                    //MessageBox.Show("map.DisplayName:" + map.DisplayName);
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.DisplayName);
        //                }
        //                //Map note
        //                if (string.IsNullOrEmpty(map.Note) || SelectedLocalePercentFromStringIsNotValid(map.Note) || GetAlreadyAddedInTable(Jsonname, map.Note))
        //                {
        //                }
        //                else
        //                {
        //                    //MessageBox.Show("map.Note:" + map.Note);
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(map.Note);
        //                }

        //                //string prevval = "";
        //                foreach (RPGMakerMVjsonMapEvent ev in map.Events)
        //                {
        //                    if (ev == null)
        //                    {
        //                    }
        //                    else
        //                    {
        //                        //event name
        //                        if (string.IsNullOrEmpty(ev.Name) || ev.Name.StartsWith("EV") || SelectedLocalePercentFromStringIsNotValid(ev.Name) || GetAlreadyAddedInTable(Jsonname, ev.Name))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            //MessageBox.Show("map.Events add name"+ ev.Name);
        //                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Name);
        //                            //prevval = ev.Name;
        //                        }
        //                        //event note
        //                        if (string.IsNullOrEmpty(ev.Note) || SelectedLocalePercentFromStringIsNotValid(ev.Note) || GetAlreadyAddedInTable(Jsonname, ev.Note))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            //MessageBox.Show("map.Events add note:" + ev.Note);
        //                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(ev.Note);
        //                        }

        //                        //event parameters
        //                        foreach (RPGMakerMVjsonMapPage page in ev.Pages)
        //                        {
        //                            foreach (RPGMakerMVjsonMapPageList lst in page.List)
        //                            {
        //                                foreach (var parameter in lst.Parameters)
        //                                {
        //                                    if (parameter == null)
        //                                    {

        //                                    }
        //                                    else if (parameter.GetType().Name == "String")
        //                                    {
        //                                        string pstring = parameter.ToString();
        //                                        if (string.IsNullOrEmpty(pstring) || HasNOJPcharacters(pstring) || SelectedLocalePercentFromStringIsNotValid(pstring) || GetAlreadyAddedInTable(Jsonname, pstring))
        //                                        {

        //                                        }
        //                                        else
        //                                        {
        //                                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(pstring);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (system)
        //        {
        //            //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
        //            var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

        //            //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

        //            THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);

        //            if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string armortype in systemdata.ArmorTypes)
        //                {
        //                    if (string.IsNullOrEmpty(armortype))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
        //                    }
        //                }
        //            }
        //            if (systemdata.Elements == null || systemdata.Elements.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string element in systemdata.Elements)
        //                {
        //                    if (string.IsNullOrEmpty(element))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
        //                    }
        //                }
        //            }
        //            if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string equipType in systemdata.EquipTypes)
        //                {
        //                    if (string.IsNullOrEmpty(equipType))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
        //                    }
        //                }
        //            }
        //            if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string skillType in systemdata.skillTypes)
        //                {
        //                    if (string.IsNullOrEmpty(skillType))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
        //                    }
        //                }
        //            }
        //            if (systemdata.Switches == null || systemdata.Switches.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string _switch in systemdata.Switches)
        //                {
        //                    if (string.IsNullOrEmpty(_switch))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
        //                    }
        //                }
        //            }
        //            if (systemdata.Switches == null || systemdata.Switches.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string _switch in systemdata.Switches)
        //                {
        //                    if (string.IsNullOrEmpty(_switch))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
        //                    }
        //                }
        //            }
        //            if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
        //            {

        //            }
        //            else
        //            {
        //                foreach (string weaponType in systemdata.WeaponTypes)
        //                {
        //                    if (string.IsNullOrEmpty(weaponType))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
        //                    }
        //                }
        //            }
        //            if (systemdata.Terms == null)
        //            {

        //            }
        //            else
        //            {
        //                foreach (var basic in systemdata.Terms.Basic)
        //                {
        //                    if (string.IsNullOrEmpty(basic))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
        //                    }
        //                }
        //                foreach (var command in systemdata.Terms.Commands)
        //                {
        //                    if (string.IsNullOrEmpty(command))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
        //                    }
        //                }

        //                foreach (string param in systemdata.Terms.Params)
        //                {
        //                    if (string.IsNullOrEmpty(param))
        //                    {

        //                    }
        //                    else
        //                    {
        //                        THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
        //                    }
        //                }

        //                foreach (var Message in systemdata.Terms.Messages)
        //                {
        //                    THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
        //                }
        //            }
        //            //FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
        //            //THLog = "";
        //        }


        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        FileWriter.WriteData(apppath + "\\TranslationHelper.log", ex.Message, true);
        //        return false;
        //    }
        //}


        public bool GetAlreadyAddedInTable(string tablename, string value)
        {
            //про primary key взял отсюда: https://stackoverflow.com/questions/3567552/table-doesnt-have-a-primary-key
            //DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = THFilesElementsDataset.Tables[tablename].Columns["Original"];
            THFilesElementsDataset.Tables[tablename].PrimaryKey = keyColumns;

            //очень быстрый способ поиска дубликата значения, два нижник в разы медленней, этот почти не заметен
            if (THFilesElementsDataset.Tables[tablename].Rows.Contains(value))
            {
                //LogToFile("Value already in table: \r\n"+ value);
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
            //LogToFile("Value still not in table: \r\n" + value);
            return false;
        }

        public bool HasNOJPcharacters(string str)
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

                using (Process RPGMakerTransPatch = new Process())
                {
                    //MessageBox.Show("outdir=" + outdir);
                    RPGMakerTransPatch.StartInfo.FileName = apppath + @"\\Res\\rpgmakertrans\\rpgmt.exe";
                    RPGMakerTransPatch.StartInfo.Arguments = "\"" + dir + "\" -p \"" + outdir + "_patch\"" + "\" -o \"" + outdir + "_translated\"";
                    ret = RPGMakerTransPatch.Start();
                    RPGMakerTransPatch.WaitForExit();
                }
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

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles, string patchver = "2")
        {
            //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
            //MessageBox.Show("ListFiles=" + ListFiles);
            //MessageBox.Show("ListFiles[0]=" + ListFiles[0]);

            StreamReader _file;   //Через что читаем
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
            //THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //THFilesDataGridView.Columns.Add("Filename", "Text");

            //прогрессбар
            //progressBar.Maximum = ListFiles.Count;
            //progressBar.Value = 0;
            //List<RPGMTransPatchFile> THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                //измерение времени выполнения
                //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
                //System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
                //swatch.Start();

                string fname = Path.GetFileNameWithoutExtension(ListFiles[i]);
                ProgressInfo(true, "opening file: "+ fname+".txt");
                _file = new StreamReader(ListFiles[i]); //Задаем файл
                //THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), ""));    //Добaвляем файл
                THFilesElementsDataset.Tables.Add(fname);
                THFilesElementsDataset.Tables[i].Columns.Add("Context");
                THFilesElementsDataset.Tables[i].Columns.Add("Advice");
                THFilesElementsDataset.Tables[i].Columns.Add("Original");
                THFilesElementsDataset.Tables[i].Columns.Add("Translation");
                THFilesElementsDataset.Tables[i].Columns.Add("Status");
                THFilesElementsDatasetInfo.Tables.Add(fname);
                THFilesElementsDatasetInfo.Tables[i].Columns.Add("Original");

                //swatch.Stop();
                //string time = swatch.Elapsed.ToString();
                //LogToFile("time=" + time);//asdf
                //THMsg.Show("time=" + time);

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
                                //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));  //Пишем
                                THFilesElementsDataset.Tables[i].Rows.Add(_context, _advice, _untrans, _trans, _status);
                                THFilesElementsDatasetInfo.Tables[i].Rows.Add(_context);
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
                else if (patchver == "2")
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
                                            //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
                                            THFilesElementsDataset.Tables[i].Rows.Add(_context, _advice, _untrans, _trans, _status);
                                            THFilesElementsDatasetInfo.Tables[i].Rows.Add(_context);
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
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    //THMsg.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    _file.Close();  //Закрываем файл
                    return false;
                }

                if (invalidformat == 1)
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
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
                    //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName);//asdf
                    THFilesListBox.Invoke((Action)(() => THFilesListBox.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
                    //THFilesDataGridView.Rows.Add();
                    //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                    //dGFiles.Rows.Add();
                    //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                }
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                FVariant = " * RPG Maker Trans Patch " + patchver;
            }
            else if (invalidformat == 1)
            {
                //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
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
                    ProgressInfo(true);

                    /*
                    if (THSelectedSourceType.Contains("RPGMTransPatch"))
                    {
                    }
                    else if (THSelectedSourceType.Contains("RPG Maker MV"))
                    {
                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex];

                        //https://10tec.com/articles/why-datagridview-slow.aspx
                        //ds.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = string.Format("Text LIKE '%{0}%'", "FIltering string");
                        
                    }
                    */

                    //THFiltersDataGridView.Columns.Clear();

                    //сунул под try так как один раз здесь была ошибка о выходе за диапахон


                    //https://www.youtube.com/watch?v=wZ4BkPyZllY
                    //Thread t = new Thread(new ThreadStart(StartLoadingForm));
                    //t.Start();
                    //Thread.Sleep(100);

                    //this.Cursor = Cursors.WaitCursor; // Поменять курсор на часики

                    //измерение времени выполнения
                    //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
                    //System.Diagnostics.Stopwatch swatch = new System.Diagnostics.Stopwatch();
                    //swatch.Start();

                    //https://stackoverflow.com/questions/778095/windows-forms-using-backgroundimage-slows-down-drawing-of-the-forms-controls
                    //THFileElementsDataGridView.SuspendDrawing();//используются оба SuspendDrawing и SuspendLayout для возможного ускорения
                    //THFileElementsDataGridView.SuspendLayout();//с этим вроде побыстрее чем с SuspendDrawing из ControlHelper

                    //THsplitContainerFilesElements.Panel2.Visible = false;//сделать невидимым родительский элемент на время

                    //Советы, после которых отображение ячеек стало во много раз быстрее, 
                    //https://docs.microsoft.com/en-us/dotnet/framework/winforms/controls/best-practices-for-scaling-the-windows-forms-datagridview-control
                    //Конкретно, поменял режим отображения строк(Rows) c AllCells на DisplayerCells, что ускорило отображение 3400к. строк в таблице в 100 раз, с 9с. до 0.09с. !

                    //THBS.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;
                    //THFileElementsDataGridView.DataSource = THBS;
                    
                    //THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    //THFileElementsDataGridView.Invoke((Action)(() => THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks));
                    //THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;//.GetRange(0, THRPGMTransPatchFilesFGetCellCount());
                    if (THFilesListBox.SelectedIndex >= 0)//предотвращает исключение "Невозможно найти таблицу -1"
                    {
                        THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex];//.GetRange(0, THRPGMTransPatchFilesFGetCellCount());
                    }


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

                    //swatch.Stop();
                    //string time = swatch.Elapsed.ToString();
                    //FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>:" + THFilesListBox.SelectedItem.ToString() + "> Time:\"" + time + "\"\r\n", true);
                    //MessageBox.Show("Time: "+ time); // тут выводим результат в консоль

                    if (THSelectedSourceType.Contains("RPGMTransPatch")) //Additional tweaks for RPGMTransPatch table
                    {
                        THFileElementsDataGridView.Columns["Context"].Visible = false;
                        THFileElementsDataGridView.Columns["Status"].Visible = false;
                        if (FVariant == " * RPG Maker Trans Patch 3")
                        {
                            THFileElementsDataGridView.Columns["Advice"].Visible = false;
                        }
                    }

                    //this.Cursor = Cursors.Default; ;//Поменять курсор обратно на обычный

                    //THFileElementsDataGridView.ResumeLayout();
                    //THFileElementsDataGridView.ResumeDrawing();

                    SetFilterDGV(); //Init Filters datagridview

                    SetOnTHFileElementsDataGridViewWasLoaded(); //Additional actions when elements of file was loaded in datagridview

                    CheckFilterDGV(); //Apply filters if they is not empty

                    ProgressInfo(false, "");
                }
                catch (Exception)
                {
                }

                //THFileElementsDataGridView.RowHeadersVisible = true; // set it to false if not needed

                THFilesListBox_MouseClickBusy = false;
            }
        }

        private void SetOnTHFileElementsDataGridViewWasLoaded()
        {
            THFileElementsDataGridView.Columns["Original"].HeaderText = "Оригинал";//THMainDGVOriginalColumnName;
            THFileElementsDataGridView.Columns["Translation"].HeaderText = "Перевод";//THMainDGVTranslationColumnName;
            THFileElementsDataGridView.Columns[THMainDGVOriginalColumnName].ReadOnly = true;
            THFiltersDataGridView.Enabled = true;
            THSourceTextBox.Enabled = true;
            THTargetTextBox.Enabled = true;
            THTargetTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            //saveToolStripMenuItem.Enabled = true; //эти активируются при внесении изменений
            //saveAsToolStripMenuItem.Enabled = true;
            //saveTranslationToolStripMenuItem.Enabled = true;
            //editToolStripMenuItem.Enabled = true;//а эти активируются сразу после успешного открытия файлов
            //viewToolStripMenuItem.Enabled = true;
            //loadTranslationToolStripMenuItem.Enabled = true;
            //loadTrasnlationAsToolStripMenuItem.Enabled = true;
        }

        private void SetFilterDGV()
        {
            //MessageBox.Show("THFiltersDataGridView.Columns.Count=" + THFiltersDataGridView.Columns.Count
            //    + "\r\nTHFileElementsDataGridView visible Columns Count=" + THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible));
            if (THFiltersDataGridView.Columns.Count != THFileElementsDataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible))
            {
                THFiltersDataGridView.Columns.Clear();
                THFiltersDataGridView.Rows.Clear();
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
                        //wrap words fix: https://stackoverflow.com/questions/1751371/how-to-use-n-in-a-textbox
                        THSourceTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells[THMainDGVOriginalColumnName].Value.ToString().Replace("\n", Environment.NewLine); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
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

                        THTargetTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString().Replace("\n", Environment.NewLine); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
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
                        THInfoTextBox.Text += "\r\n";
                        THInfoTextBox.Text += "rowinfo:\r\n"+THFilesElementsDatasetInfo.Tables[THFilesListBox.SelectedIndex].Rows[e.RowIndex][0];
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

                ret += "contains: \r\n";
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

        public bool SelectedLocalePercentFromStringIsNotValid(string target, string langlocale = "romaji")
        {
            if (Settings.THOptionDontLoadStringIfRomajiPercentCheckBoxChecked)
            {
                return ((GetLocaleLangCount(target, langlocale) * 100) / GetLocaleLangCount(target, "all")) > Settings.THOptionDontLoadStringIfRomajiPercentValue;
            }
            return false;
        }

        bool SaveInAction = false;
        bool FIleDataWasChanged = false;
        private void WriteTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveInAction)
            {
                //MessageBox.Show("Saving still in progress. Please wait a little.");
            }
            else if (FIleDataWasChanged)
            {
                SaveInAction = true;
                FIleDataWasChanged = false;
                //MessageBox.Show("THSelectedSourceType=" + THSelectedSourceType);
                if (THSelectedSourceType == "RPGMTransPatch" || THSelectedSourceType == "RPG Maker game with RPGMTransPatch")
                {
                    //THActionProgressBar.Visible = true;
                    //THInfolabel.Visible = true;
                    //THInfolabel.Text = "saving..";
                    ProgressInfo(true);

                    //THInfoTextBox.Text = "Saving...";

                    //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
                    Thread save = new Thread(new ParameterizedThreadStart((obj) => SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver)));
                    save.Start();

                    //MessageBox.Show("THSelectedDir=" + THSelectedDir);
                    //SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver);

                    //THInfoTextBox.Text = "";

                    //THActionProgressBar.Visible = false;
                }
                else if (THSelectedSourceType == "RPG Maker MV json")
                {
                    //THMsg.Show(THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                    WriteJson(THFilesListBox.Items[0].ToString(), THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                }
                else if (THSelectedSourceType == "RPG Maker MV")
                {
                    for (int f=0;f < THFilesListBox.Items.Count; f++)
                    {
                        //глянуть здесь насчет поиска значения строки в колонки. Для функции поиска, например.
                        //https://stackoverflow.com/questions/633819/find-a-value-in-datatable

                        bool changed = false;
                        for (int r = 0; r < THFilesElementsDataset.Tables[f].Rows.Count; r++)
                        {
                            if (THFilesElementsDataset.Tables[f].Rows[r]["Translation"] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[f].Rows[r]["Translation"].ToString()))
                            {
                            }
                            else
                            {
                                changed = true;
                                break;
                            }
                        }
                        //THMsg.Show(THSelectedDir + "\\" + THFilesListBox.Items[0].ToString() + ".json");
                        if (changed)
                        {

                            //THMsg.Show("start writing");
                            WriteJson(THFilesListBox.Items[f].ToString(), THSelectedDir + "\\" + THFilesListBox.Items[f].ToString() + ".json");
                        }
                    }
                    THMsg.Show("finished");
                }
            }
        }

        private void ProgressInfo(bool status, string statustext = "working..")
        {
            THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Visible = status));
            THInfolabel.Invoke((Action)(() => THInfolabel.Visible = status));
            THInfolabel.Invoke((Action)(() => THInfolabel.Text = statustext));
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog THSaveFolderBrowser = new FolderBrowserDialog())
            {
                //MessageBox.Show(dirpath);
                THSaveFolderBrowser.SelectedPath = THSelectedDir; //Установить начальный путь на тот, что был установлен при открытии.

                if (THSaveFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (THSelectedSourceType == "RPGMTransPatch")
                    {
                        if (SaveRPGMTransPatchFiles(THSaveFolderBrowser.SelectedPath, THRPGMTransPatchver))
                        {
                            THSelectedDir = THSaveFolderBrowser.SelectedPath;
                            //MessageBox.Show("Сохранение завершено!");
                            THMsg.Show("Сохранение завершено!");
                        }
                    }
                }
            }
        }

        public bool SaveRPGMTransPatchFiles(string SelectedDir, string patchver = "2")
        {
            try
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

                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        buffer = "> RPGMAKER TRANS PATCH FILE VERSION 3.2\r\n";
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < THFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            buffer += "> BEGIN STRING\r\n";
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + "\r\n";
                            buffer += THFilesElementsDataset.Tables[i].Rows[y][2] + "\r\n";
                            //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                            //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                            //string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                            string[] str = THFilesElementsDataset.Tables[i].Rows[y][0].ToString().Split('\n');
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
                                                                      //if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == "\r\n")
                                    if (String.IsNullOrEmpty(THFilesElementsDataset.Tables[i].Rows[y][3].ToString())) //if (ArrayTransFilses[i].blocks[y].Trans == "\r\n")
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
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation + "\r\n";
                            buffer += THFilesElementsDataset.Tables[i].Rows[y][3] + "\r\n";
                            buffer += "> END STRING\r\n\r\n";

                            //progressBar.Value++;
                            //MessageBox.Show(progressBar.Value.ToString());
                        }

                        if (String.IsNullOrWhiteSpace(buffer))
                        {
                        }
                        else
                        {
                            if (Directory.Exists(SelectedDir + "\\patch"))
                            {
                            }
                            else
                            {
                                Directory.CreateDirectory(SelectedDir + "\\patch");
                            }
                            //String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            String _path = SelectedDir + "\\patch\\" + THFilesElementsDataset.Tables[i].TableName + ".txt";
                            File.WriteAllText(_path, buffer);
                            //buffer = "";
                        }
                    }
                }
                else if (patchver == "2")
                {
                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                    {
                        buffer = "# RPGMAKER TRANS PATCH FILE VERSION 2.0\r\n";
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < THFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            buffer += "# TEXT STRING\r\n";
                            //if (THRPGMTransPatchFiles[i].blocks[y].Translation == "\r\n")
                            if (THFilesElementsDataset.Tables[i].Rows[y][3].ToString() == "\r\n")
                                buffer += "# UNTRANSLATED\r\n";
                            //buffer += "# CONTEXT : " + THRPGMTransPatchFiles[i].blocks[y].Context + "\r\n";
                            buffer += "# CONTEXT : " + THFilesElementsDataset.Tables[i].Rows[y][0].ToString() + "\r\n";
                            //buffer += "# ADVICE : " + THRPGMTransPatchFiles[i].blocks[y].Advice + "\r\n";
                            buffer += "# ADVICE : " + THFilesElementsDataset.Tables[i].Rows[y][1].ToString() + "\r\n";
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original;
                            buffer += THFilesElementsDataset.Tables[i].Rows[y][2].ToString();
                            buffer += "# TRANSLATION \r\n";
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation;
                            buffer += THFilesElementsDataset.Tables[i].Rows[y][3].ToString();
                            buffer += "# END STRING\r\n\r\n";
                        }
                        if (!String.IsNullOrWhiteSpace(buffer))
                        {
                            //String _path = SelectedDir + "\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            String _path = SelectedDir + "\\" + THFilesElementsDataset.Tables[i].TableName + ".txt";
                            File.WriteAllText(_path, buffer);
                            //buffer = "";
                        }
                    }
                }
                //pbstatuslabel.Visible = false;
                //pbstatuslabel.Text = string.Empty;
            }
            catch
            {
                //THInfoTextBox.Text = "";
                //THActionProgressBar.Visible = false;
                //THInfolabel.Invoke((Action)(() => THInfolabel.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Text = ""));
                ProgressInfo(false, "");
                SaveInAction = false;
                return false;
            }
            finally
            {
                //THInfoTextBox.Text = "";
                //THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Text = ""));
                ProgressInfo(false, "");
            }

            SaveInAction = false;
            return true;

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (THAboutForm AboutForm = new THAboutForm())
            {
                AboutForm.Show();
            }
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
            CheckFilterDGV();
            /*
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
                                    if (cindx < THFileElementsDataGridView.Columns.Count - 1 Контроль на превышение лимита колонок, на всякий && THFiltersDataGridView.Columns[e.ColumnIndex].Name == THFileElementsDataGridView.Columns[cindx].Name)
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
            */
        }

        private void CheckFilterDGV()
        {
            //private void DGVFilter()
            string OverallFilter = string.Empty;
            for (int c = 0; c < THFiltersDataGridView.Columns.Count; c++)
            {
                if (THFiltersDataGridView.Rows[0].Cells[c].Value == null || string.IsNullOrEmpty(THFiltersDataGridView.Rows[0].Cells[c].Value.ToString()))
                {

                }
                else
                {
                    if (string.IsNullOrEmpty(OverallFilter))
                    {
                        OverallFilter += THFiltersDataGridView.Columns[c].Name + " Like '%" + THFiltersDataGridView.Rows[0].Cells[c].Value + "%'";
                    }
                    else
                    {
                        OverallFilter += " AND ";
                        OverallFilter += THFiltersDataGridView.Columns[c].Name + " Like '%" + THFiltersDataGridView.Rows[0].Cells[c].Value + "%'";
                    }
                }
            }
            //also about sort:https://docs.microsoft.com/ru-ru/dotnet/api/system.data.dataview.rowfilter?view=netframework-4.8
            //THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.Sort = String.Empty;
            //THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = String.Empty;
            //MessageBox.Show("\""+OverallFilter+ "\"");
            //MessageBox.Show(string.Format("" + THFiltersDataGridView.Columns[e.ColumnIndex].Name + " LIKE '%{0}%'", THFiltersDataGridView.Rows[0].Cells[e.ColumnIndex].Value));
            //https://10tec.com/articles/why-datagridview-slow.aspx
            //THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = string.Format("" + THFiltersDataGridView.Columns[e.ColumnIndex].Name + " LIKE '%{0}%'", THFiltersDataGridView.Rows[0].Cells[e.ColumnIndex].Value);
            THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex].DefaultView.RowFilter = OverallFilter;
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
#pragma warning disable IDE0067 // Ликвидировать объекты перед потерей области //игнор, т.к. закрывает сразу второе окно
            THSettingsForm THSettings = new THSettingsForm();
#pragma warning restore IDE0067 // Ликвидировать объекты перед потерей области
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

        private void THFileElementsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                FIleDataWasChanged = true;
            }
        }

        string dbpath;
        string lastautosavepath;
        private void SaveTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbpath = apppath + "\\DB";
            string dbfilename = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");
            lastautosavepath = dbpath + "\\Auto\\Auto" + dbfilename + GetDBCompressionExt();

            ProgressInfo(true);

            WriteDBFile(THFilesElementsDataset, lastautosavepath);
            //THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

            Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath);

            ProgressInfo(false);
        }

        private void LoadTranslationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTranslationFromDB();
        }

        bool LoadTranslationToolStripMenuItem_ClickIsBusy = false;
        private async void LoadTranslationFromDB(string sPath = "")
        {
            if (LoadTranslationToolStripMenuItem_ClickIsBusy)
            {
                return;
            }
            LoadTranslationToolStripMenuItem_ClickIsBusy = true;

            //dbpath = apppath + "\\DB";
            //string dbfilename = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");

            ProgressInfo(true);

            //lastautosavepath = dbpath + "\\Auto\\Auto" + dbfilename + GetDBCompressionExt();

            //WriteDBFile(THFilesElementsDataset, lastautosavepath);
            //THFilesElementsDataset.WriteXml(lastautosavepath); // make buckup of previous data

            //THFilesElementsDataset.Reset();
            //THFilesListBox.Items.Clear();


            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
            await Task.Run(() => THLoadDBCompare(sPath));

            THFileElementsDataGridView.Refresh();
            /*
            THFileElementsDataGridView.DataSource = THFilesElementsDataset;

            foreach (DataTable t in THFilesElementsDataset.Tables)
            {
                THFilesListBox.Items.Add(t.TableName);
                //FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>: \"" + t.TableName + "\"\r\n", true);
            }
            //THFileElementsDataGridView.Refresh();
            if (THFilesListBox.SelectedIndex > -1)
            {
                THFileElementsDataGridView.DataSource = THFilesElementsDataset.Tables[THFilesListBox.SelectedIndex];
            }            
            */

            ProgressInfo(false);

            LoadTranslationToolStripMenuItem_ClickIsBusy = false;
        }

        private void THLoadDBCompare(string sPath)
        {
            using (DataSet THTempDS = new DataSet())
            {
                if (string.IsNullOrEmpty(sPath))
                {
                    //THFilesElementsDataset.ReadXml(Settings.THConfigINI.ReadINI("Paths", "LastAutoSavePath")); //load new data
                    ReadDBFile(THTempDS, Settings.THConfigINI.ReadINI("Paths", "LastAutoSavePath")); //load new data
                }
                else
                {
                    ReadDBFile(THTempDS, sPath); //load new data
                }

                //Settings.THConfigINI.WriteINI("Paths", "LastAutoSavePath", lastautosavepath); // write lastsavedpath

                for (int t = 0; t < THFilesElementsDataset.Tables.Count; t++)
                {
                    int ocol = THFilesElementsDataset.Tables[t].Columns.Count - 1; //если вдруг колонка была только одна
                    //LogToFile("ocol=" + ocol);
                    if (ocol == 0)
                    {
                        //LogToFile("ocol=0! creating second");
                        THFilesElementsDataset.Tables[t].Columns.Add("Translation");
                        continue;
                    }

                    while (ocol > 1) //на будущее, если колонок будет больше одной
                    {
                        if (THFilesElementsDataset.Tables[t].Columns[ocol].ColumnName == "Original")
                        {
                            break;
                        }
                        ocol -= 1;
                    }
                    //LogToFile("THFilesElementsDataset.Tables[t].Columns[ocol].ColumnName=" + THFilesElementsDataset.Tables[t].Columns[ocol].ColumnName);

                    for (int r = 0; r < THFilesElementsDataset.Tables[t].Rows.Count; r++)
                    {
                        if (String.IsNullOrEmpty(THFilesElementsDataset.Tables[t].Rows[r][ocol].ToString()) || THFilesElementsDataset.Tables[t].Rows[r][0] == THFilesElementsDataset.Tables[t].Rows[r][ocol])
                        {
                            //LogToFile("THFilesElementsDataset.Tables[" + t + "].Rows[" + r + "][" + ocol + "].ToString()=" + THFilesElementsDataset.Tables[t].Rows[r][ocol].ToString());
                            bool transwasset = false;
                            for (int t1 = 0; t1 < THTempDS.Tables.Count; t1++)
                            {
                                if (THTempDS.Tables[t1].Columns.Count > 1)
                                {
                                    for (int r1 = 0; r1 < THTempDS.Tables[t1].Rows.Count; r1++)
                                    {
                                        int tcol = THTempDS.Tables[t1].Columns.Count - 1;
                                        //LogToFile("tcol=" + tcol);
                                        while (tcol > 1) //поиск колонки оригинал
                                        {
                                            if (THTempDS.Tables[t1].Columns[tcol].ColumnName == "Original")
                                            {
                                                break;
                                            }
                                            tcol -= 1;
                                        }

                                        if (THTempDS.Tables[t1].Rows[r1][tcol] == null || string.IsNullOrEmpty(THTempDS.Tables[t1].Rows[r1][tcol].ToString()))
                                        {
                                        }
                                        else
                                        {
                                            //LogToFile("THFilesElementsDataset.Tables[" + t + "].Rows[" + r + "][0].ToString()=" + THFilesElementsDataset.Tables[t].Rows[r][0].ToString());
                                            //LogToFile("THTempDS.Tables[" + t1 + "].Rows[" + r1 + "][0].ToString()=" + THTempDS.Tables[t1].Rows[r1][0].ToString());
                                            //LogToFile("THFilesElementsDataset.Tables[" + t + "].Rows[" + r + "][0]=" + THFilesElementsDataset.Tables[t].Rows[r][0]);
                                            //LogToFile("THTempDS.Tables[" + t1 + "].Rows[" + r1 + "][0]=" + THTempDS.Tables[t1].Rows[r1][0]);
                                            //LogToFile("THTempDS.Tables[" + t1 + "].Rows[" + r1 + "][" + tcol + "].ToString()=" + THTempDS.Tables[t1].Rows[r1][tcol].ToString());
                                            if (THFilesElementsDataset.Tables[t].Rows[r][0].ToString() == THTempDS.Tables[t1].Rows[r1][0].ToString())
                                            {
                                                THFilesElementsDataset.Tables[t].Rows[r][ocol] = THTempDS.Tables[t1].Rows[r1][tcol];
                                                transwasset = true;
                                                //LogToFile("value set. transwasset=" + transwasset.ToString());
                                                break;
                                            }
                                        }
                                    }
                                    if (transwasset)
                                    {
                                        //LogToFile("value set. transwasset=" + transwasset.ToString());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                //LogToFile("cleaning THTempDS and refreshing dgv", true);
                THTempDS.Reset();//очистка временной таблицы
            }            
        }

        private string GetDBCompressionExt()
        {
            //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked"));
            if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompressionCheckBox.Checked") == "True")
            {
                //MessageBox.Show(Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression"));
                if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "XML (none)")
                {
                    //MessageBox.Show(".xml");
                    return ".xml";
                }
                else if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Gzip (cmx)")
                {
                    //MessageBox.Show(".cmx");
                    return ".cmx";
                }
                else if (Settings.THConfigINI.ReadINI("Optimizations", "THOptionDBCompression") == "Deflate (cmz)")
                {
                    //MessageBox.Show(".cmz");
                    return ".cmz";
                }

            }
            //MessageBox.Show("Default .xml");
            return ".xml";
        }

        //https://stackoverflow.com/questions/223738/net-stream-dataset-of-xml-data-to-zip-file
        //http://madprops.org/blog/saving-datasets-locally-with-compression/
        public static void ReadDBFile(DataSet DS, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                Stream s;
                if (Path.GetExtension(fileName) == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Decompress);
                }
                else if (Path.GetExtension(fileName) == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Decompress);
                }
                else
                {
                    s = fs;
                }
                DS.ReadXml(s);
                s.Close();
            }
        }

        public static void WriteDBFile(DataSet DS, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                Stream s;
                if (Path.GetExtension(fileName) == ".cmx")
                {
                    s = new GZipStream(fs, CompressionMode.Compress);
                }
                else if (Path.GetExtension(fileName) == ".cmz")
                {
                    s = new DeflateStream(fs, CompressionMode.Compress);
                }
                else
                {
                    s = fs;
                }
                DS.WriteXml(s);
                s.Close();
            }
        }

        private void ToXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void XmlToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void LoadTrasnlationAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog THFOpenBD = new OpenFileDialog())
            {
                THFOpenBD.Filter = "DB file|*.xml;*.cmx;*.cmz|XML-file|*.xml|Gzip compressed DB (*.cmx)|*.cmx|Deflate compressed DB (*.cmz)|*.cmz";
                THFOpenBD.InitialDirectory = apppath + "\\DB";

                if (THFOpenBD.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(THFOpenBD.FileName))
                    {
                    }
                    else
                    {
                        //string spath = THFOpenBD.FileName;
                        //THFOpenBD.OpenFile().Close();
                        //MessageBox.Show(THFOpenBD.FileName);
                        LoadTranslationFromDB(THFOpenBD.FileName);
                    }
                }
            }
        }

        private void TestWriteJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\Armors.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonItemsArmorsWeapons("Armors", jsondata);
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\System.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonSystem("System", jsondata);
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\Actors.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonActors("Actors", jsondata);
            //string jsondata = File.ReadAllText(@"C:\\000 test RPGMaker MV data\\CommonEvents.json"); // get json data
            //TestSaveGetDataFromRPGMakerMVjsonCommonEvents("CommonEvents", jsondata);
            //WriteJson("CommonEvents", @"C:\\000 test RPGMaker MV data\\CommonEvents.json");
        }

        //private string TestSaveGetDataFromRPGMakerMVjsonOfType(string Jsonname, string JsonElement)
        //{
        //    if (string.IsNullOrEmpty(JsonElement) || SelectedLocalePercentFromStringIsNotValid(JsonElement))
        //    {
        //    }
        //    else
        //    {
        //        for (int i = 0; i < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i++)
        //        {
        //            if (THFilesElementsDataset.Tables[Jsonname].Rows[i][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString()))
        //            {

        //            }
        //            else
        //            {
        //                if (JsonElement == THFilesElementsDataset.Tables[Jsonname].Rows[i][0].ToString())
        //                {
        //                    return THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString();
        //                }
        //            }
        //        }
        //        //THFilesElementsDataset.Tables[Jsonname].Rows[i][1];
        //        //THFilesElementsDataset.Tables[Jsonname].Rows.Add(JsonElement);
        //    }
        //    return string.Empty;
        //}

        //private bool TestSaveGetDataFromRPGMakerMVjsonItemsArmorsWeapons(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        var testjsonforwrite = JsonConvert.DeserializeObject<List<RPGMakerMVjsonItemsArmorsWeapons>>(jsondata);
        //        foreach (var JsonElement in testjsonforwrite)
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                string Name = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                if (string.IsNullOrEmpty(Name))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Name = Name;
        //                }
        //                string Description = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Description);
        //                if (string.IsNullOrEmpty(Description))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Description = Description;
        //                }
        //                string Note = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //                if (string.IsNullOrEmpty(Note))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Note = Note;
        //                }
        //            }
        //        }

        //        string s = JsonConvert.SerializeObject(testjsonforwrite);
        //        File.WriteAllText(@"C:\\000 test RPGMaker MV data\\Armors1.json", s);
        //        MessageBox.Show("test write finished");
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool TestSaveGetDataFromRPGMakerMVjsonSystem(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        //новые классы сгенерированы через этот сервис: https://app.quicktype.io/#l=cs&r=json2csharp
        //        var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);

        //        //var systemdata = JsonConvert.DeserializeObject<RPGMakerMVjsonSystem>(jsondata);

        //        if (systemdata.GameTitle == null || string.IsNullOrEmpty(systemdata.GameTitle))
        //        {
        //        }
        //        else
        //        {
        //            for (int i = 0; i < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i++)
        //            {
        //                if (THFilesElementsDataset.Tables[Jsonname].Rows[i][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString()))
        //                {
        //                }
        //                else
        //                {
        //                    if (systemdata.GameTitle == THFilesElementsDataset.Tables[Jsonname].Rows[i][0].ToString())
        //                    {
        //                        systemdata.GameTitle = THFilesElementsDataset.Tables[Jsonname].Rows[i][1].ToString();
        //                        break;
        //                    }
        //                }
        //            }
        //            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(systemdata.GameTitle);
        //        }

        //        if (systemdata.ArmorTypes == null || systemdata.ArmorTypes.Length < 1)
        //        {
        //        }
        //        else
        //        {
        //            for (int i = 0; i < systemdata.ArmorTypes.Length; i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.ArmorTypes[i]))
        //                {

        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.ArmorTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.ArmorTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(armortype);
        //                }
        //            }
        //        }
        //        if (systemdata.Elements == null || systemdata.Elements.Length < 1)
        //        {

        //        }
        //        else
        //        {
        //            for (int i=0;i< systemdata.Elements.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.Elements[i]))
        //                {

        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.Elements[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.Elements[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(element);
        //                }
        //            }
        //        }
        //        if (systemdata.EquipTypes == null || systemdata.EquipTypes.Length < 1)
        //        {
        //        }
        //        else
        //        {
        //            for (int i=0;i< systemdata.EquipTypes.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.EquipTypes[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.EquipTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.EquipTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(equipType);
        //                }
        //            }
        //        }
        //        if (systemdata.skillTypes == null || systemdata.skillTypes.Length < 1)
        //        {
        //        }
        //        else
        //        {
        //            for (int i=0; i< systemdata.skillTypes.Length; i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.skillTypes[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.skillTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.skillTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(skillType);
        //                }
        //            }
        //        }
        //        if (systemdata.Switches == null || systemdata.Switches.Length < 1)
        //        {
        //        }
        //        else
        //        {
        //            for (int i=0;i< systemdata.Switches.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.Switches[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.Switches[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.Switches[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(_switch);
        //                }
        //            }
        //        }
        //        if (systemdata.WeaponTypes == null || systemdata.WeaponTypes.Length < 1)
        //        {
        //        }
        //        else
        //        {
        //            for (int i=0;i< systemdata.WeaponTypes.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.WeaponTypes[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.WeaponTypes[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.WeaponTypes[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(weaponType);
        //                }
        //            }
        //        }
        //        if (systemdata.Terms == null)
        //        {
        //        }
        //        else
        //        {
        //            for (int i=0;i< systemdata.Terms.Basic.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.Terms.Basic[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.Terms.Basic[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.Terms.Basic[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(basic);
        //                }
        //            }
        //            for (int i=0;i< systemdata.Terms.Commands.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.Terms.Commands[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.Terms.Commands[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.Terms.Commands[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(command);
        //                }
        //            }

        //            for (int i=0;i < systemdata.Terms.Params.Length;i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.Terms.Params[i]))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.Terms.Params[i] == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.Terms.Params[i] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(param);
        //                }
        //            }

        //            //http://www.cyberforum.ru/csharp-beginners/thread785914.html
        //            for (int i = 0; i < systemdata.Terms.Messages.Count; i++)
        //            {
        //                if (string.IsNullOrEmpty(systemdata.Terms.Messages.ElementAt(i).Value))
        //                {
        //                }
        //                else
        //                {
        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (systemdata.Terms.Messages.ElementAt(i).Value == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                            {
        //                                systemdata.Terms.Messages[systemdata.Terms.Messages.ElementAt(i).Key] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //                //THFilesElementsDataset.Tables[Jsonname].Rows.Add(Message.Value);
        //            }
        //        }
        //        //FileWriter.WriteData(apppath + "\\TranslationHelper.log", THLog, true);
        //        //THLog = "";

        //        //var systemdata = RPGMakerMVjsonSystem.FromJson(jsondata);
        //        string s = RPGMakerMVjsonSystemTo.ToJson(systemdata);
        //        File.WriteAllText(@"C:\\000 test RPGMaker MV data\\System1.json", s);
        //        MessageBox.Show("test write finished");
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool TestSaveGetDataFromRPGMakerMVjsonActors(string Jsonname, string jsondata)
        //{
        //    try
        //    {
        //        var actors = RPGMakerMVjsonActors.FromJson(jsondata);//JsonConvert.DeserializeObject<List<RPGMakerMVjsonActors>>(jsondata)
        //        foreach (var JsonElement in actors)
        //        {
        //            if (JsonElement == null)
        //            {
        //            }
        //            else
        //            {
        //                string Name = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Name);
        //                if (string.IsNullOrEmpty(Name))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Name = Name;
        //                }
        //                string Nickname = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Nickname);
        //                if (string.IsNullOrEmpty(Nickname))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Nickname = Nickname;
        //                }
        //                string Note = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Note);
        //                if (string.IsNullOrEmpty(Note))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Note = Note;
        //                }
        //                string Profile = TestSaveGetDataFromRPGMakerMVjsonOfType(Jsonname, JsonElement.Profile);
        //                if (string.IsNullOrEmpty(Profile))
        //                {
        //                }
        //                else
        //                {
        //                    JsonElement.Profile = Profile;
        //                }
        //            }
        //        }

        //        string s = RPGMakerMVjsonActorsTo.ToJson(actors);
        //        File.WriteAllText(@"C:\\000 test RPGMaker MV data\\Actors1.json", s);
        //        MessageBox.Show("test write finished");

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //private bool TestSaveGetDataFromRPGMakerMVjsonCommonEvents(string Jsonname, string jsondata)
        //{
        //    //info RPG Maker MV Event codes
        //    //https://forums.rpgmakerweb.com/index.php?threads/extract-events-to-text-file.17444/
        //    //https://forums.rpgmakerweb.com/index.php?threads/cross-reference-tool.72563/
        //    //https://pastebin.com/JyRTdq0b
        //    //https://pastebin.com/eJx0EvXB
        //    //    case 401 : return 'Show Text';              break;
        //    //    case 102 : return 'Show Choices';           break;
        //    //    case 103 : return 'Input Number';           break;
        //    //    case 104 : return 'Select Item';            break;
        //    //    case 405 : return 'Show Scrolling Text';    break;
        //    //    case 111 : return 'Conditional Branch';     break;
        //    //    case 119 : return 'Common Event';           break;
        //    //    case 121 : return 'Control Switches';       break;
        //    //    case 122 : return 'Control Variables';      break;
        //    //    case 125 : return 'Change Gold';            break;
        //    //    case 126 : return 'Change Items';           break;
        //    //    case 127 : return 'Change Weapons';         break;
        //    //    case 128 : return 'Change Armors';          break;
        //    //    case 129 : return 'Change Party Member';    break;
        //    //    case 201 : return 'Transfer Player';        break;
        //    //    case 202 : return 'Set Vehicle Location';   break;
        //    //    case 203 : return 'Set Event Location';     break;
        //    //    case 505 : return 'Set Movement Route';     break;
        //    //    case 212 : return 'Show Animation';         break;
        //    //    case 231 : return 'Show Picture';           break;
        //    //    case 232 : return 'Move Picture';           break;
        //    //    case 285 : return 'Get Location Info';      break;
        //    //    case 301 : return 'Battle Processing';      break;
        //    //    case 302 :
        //    //    case 605 : return 'Shop Processing';        break;
        //    //    case 303 : return 'Name Input Processing';  break;
        //    //    case 311 : return 'Change HP';              break;
        //    //    case 312 : return 'Change MP';              break;
        //    //    case 326 : return 'Change TP';              break;
        //    //    case 313 : return 'Change State';           break;
        //    //    case 314 : return 'Recover All';            break;
        //    //    case 315 : return 'Change EXP';             break;
        //    //    case 316 : return 'Change Level';           break;
        //    //    case 317 : return 'Change Parameter';       break;
        //    //    case 318 : return 'Change Skill';           break;
        //    //    case 319 : return 'Change Equipment';       break;
        //    //    case 320 : return 'Change Name';            break;
        //    //    case 321 : return 'Change Class';           break;
        //    //    case 322 : return 'Change Actor Images';    break;
        //    //    case 324 : return 'Change Nickname';        break;
        //    //    case 325 : return 'Change Profile';         break;
        //    //    case 331 : return 'Change Enemy HP';        break;
        //    //    case 332 : return 'Change Enemy MP';        break;
        //    //    case 342 : return 'Change Enemy TP';        break;
        //    //    case 333 : return 'Change Enemy State';     break;
        //    //    case 336 : return 'Enemy Transform';        break;
        //    //    case 337 : return 'Show Battle Animation';  break;
        //    //    case 339 : return 'Force Action';           break;
        //    //
        //    //Will be handled:
        //    //401 - Show text (mergeable)
        //    //102 - Show choices (Choices list)
        //    //402 - Choice for choices - ignore because already in 102
        //    //405 - Show Scrolling Text (mergeable)
        //    //108 and 408 - Comment - can be ignored because it is for dev suppose
        //    //normal example about command values adding: https://galvs-scripts.com/galvs-party-select/


        //    //var commoneventsdata = JsonConvert.DeserializeObject<List<RPGMakerMVjsonCommonEvents>>(jsondata);
        //    var commoneventsdata = RpgMakerMVjsonCommonEvents.FromJson(jsondata);

        //    for (int i = 1; i < commoneventsdata.Count; i++)
        //    {
        //        //FileWriter.WriteData(apppath + "\\TranslationHelper.log", DateTime.Now + " >>: p=\"" + p + "\"\r\n", true);

        //        //THLog += DateTime.Now + " >>: event id=\"" + commoneventsdata[i].Id + "\"\r\n";
        //        //THLog += DateTime.Now + " >>: added event name=\"" + commoneventsdata[i].Name + "\"\r\n";

        //        //string eventname = commoneventsdata[i].Name;
        //        if (string.IsNullOrEmpty(commoneventsdata[i].Name))
        //        {
        //        }
        //        else //if code not equal old code and newline is not empty
        //        {
        //            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //            {
        //                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                {
        //                }
        //                else
        //                {
        //                    if (commoneventsdata[i].Name == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                    {
        //                        commoneventsdata[i].Name = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                        break;
        //                    }
        //                }
        //            }
        //            //THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].Name); //add event name to new row
        //        }

        //        for (int c = 0; c < commoneventsdata[i].List.Count; c++)
        //        {
        //            if (commoneventsdata[i].List[c].Code == 101 || commoneventsdata[i].List[c].Code == 105 || commoneventsdata[i].List[c].Code == 401 || commoneventsdata[i].List[c].Code == 405)
        //            {
        //                string parameter0value = commoneventsdata[i].List[c].Parameters[0].String;
        //                if (string.IsNullOrEmpty(parameter0value))
        //                {
        //                }
        //                else //if code not equal old code and newline is not empty
        //                {
        //                    //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

        //                    for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
        //                            //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp
        //                            string[] origA = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString().Split('\n').Where(emptyvalues => !string.IsNullOrEmpty(emptyvalues.Replace("\r", ""))).ToArray();//Все строки, кроме пустых, чтобы потом исключить из проверки
        //                            int origALength = origA.Length;
        //                            if (origALength == 0)
        //                            {
        //                                origA = new string[1];
        //                                origA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString();
        //                                LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
        //                            }

        //                            if (origALength > 0)
        //                            {
        //                                string[] transA = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString().Split('\n').Where(emptyvalues => !string.IsNullOrEmpty(emptyvalues.Replace("\r", ""))).ToArray();//Все строки, кроме пустых
        //                                if (transA.Length == 0)
        //                                {
        //                                    transA = new string[1];
        //                                    transA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                    LogToFile("(transA.Length == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString());
        //                                }
        //                                string transmerged = string.Empty;
        //                                if (transA.Length == origALength)//если количество строк в оригинале и переводе равно
        //                                {
        //                                    //ничего не делать
        //                                }
        //                                else // если перевод вдруг был переведен так, что не равен количеством строк оригиналу, тогда поделить его на равные строки
        //                                {
        //                                    if (transA.Length > 0) // но перед этим, если перевод больше одной строки
        //                                    {
        //                                        foreach (string ts in transA)
        //                                        {
        //                                            transmerged += ts; // объединить все строки в одну
        //                                        }
        //                                    }

        //                                    //Это заменил расширением Where, что выше при задании массива, пустые строки будут исключены сразу
        //                                    //Проверить, есть ли в массиве хоть один пустой элемент
        //                                    //https://stackoverflow.com/questions/44405411/how-can-i-check-wether-an-array-contains-any-item-or-is-completely-empty
        //                                    //if (orig.Any(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", "")) ) )
        //                                    //А это считает количество пустых элементов в массиве
        //                                    //https://stackoverflow.com/questions/2391743/how-many-elements-of-array-are-not-null
        //                                    //int ymptyelementscnt = orig.Count(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", "")));
                                            
        //                                    transA = THSplit(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
        //                                }
        //                                bool br = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
        //                                for (int i2 = 0; i2 < origALength; i2++)
        //                                {
        //                                    //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", "") + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", "")));
        //                                    if (parameter0value == origA[i2].Replace("\r", "")) //Replace здесь убирает \r из за которой строки считались неравными
        //                                    {
        //                                        LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", "") + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", "")));

        //                                        commoneventsdata[i].List[c].Parameters[0] = transA[i2].Replace("\r", ""); //Replace убирает \r

        //                                        LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
        //                                        br = true;
        //                                        break;
        //                                    }
        //                                }
        //                                if (br) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
        //                                {
        //                                    break;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (commoneventsdata[i].List[c].Parameters[0].String == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
        //                                {
        //                                    commoneventsdata[i].List[c].Parameters[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else if (commoneventsdata[i].List[c].Code == 102)
        //            {
        //                for (int i1 = 0; i1 < commoneventsdata[i].List[c].Parameters[0].AnythingArray.Count; i1++)
        //                {
        //                    string parameter0value = commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String;
        //                    if (string.IsNullOrEmpty(parameter0value))
        //                    {
        //                    }
        //                    else //if code not equal old code and newline is not empty
        //                    {
        //                        for (int i2 = 0; i2 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i2++)
        //                        {
        //                            if (THFilesElementsDataset.Tables[Jsonname].Rows[i2][1] == null)
        //                            {
        //                            }
        //                            else
        //                            {
        //                                string orig = THFilesElementsDataset.Tables[Jsonname].Rows[i2][0].ToString();
        //                                string trans = THFilesElementsDataset.Tables[Jsonname].Rows[i2][1].ToString();
        //                                if (string.IsNullOrEmpty(trans) || orig == trans)
        //                                {
        //                                }
        //                                else
        //                                {
        //                                    //LogToFile("parameter0value=" + parameter0value+ ", orig=" + orig + ", (parameter0value == orig) is " + (parameter0value == orig));
        //                                    if (parameter0value == orig)
        //                                    {
        //                                        LogToFile("parameter0value=" + parameter0value + ", orig=" + orig + ", (parameter0value == orig) is " + (parameter0value == orig));

        //                                        commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1] = trans;
        //                                        LogToFile("commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1]=" + commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String+ ", trans"+ trans);
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        //THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String); //Save text to new row
        //                    }
        //                }
        //            }
        //            else if (commoneventsdata[i].List[c].Code == 402)
        //            {
        //                if (string.IsNullOrEmpty(commoneventsdata[i].List[c].Parameters[1].String))
        //                {
        //                }
        //                else //if code not equal old code and newline is not empty
        //                {
        //                    for (int i2 = 0; i2 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i2++)
        //                    {
        //                        if (THFilesElementsDataset.Tables[Jsonname].Rows[i2][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i2][1].ToString()))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (commoneventsdata[i].List[c].Parameters[1].String == THFilesElementsDataset.Tables[Jsonname].Rows[i2][0].ToString())
        //                            {
        //                                commoneventsdata[i].List[c].Parameters[1] = THFilesElementsDataset.Tables[Jsonname].Rows[i2][1].ToString();
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    //THFilesElementsDataset.Tables[Jsonname].Rows.Add(commoneventsdata[i].List[c].Parameters[0].AnythingArray[i1].String); //Save text to new row
        //                }
        //            }
        //        }
        //    }
        //    LogToFile("", true);
        //    string s = RpgMakerMVjsonCommonEventsTo.ToJson(commoneventsdata);
        //    File.WriteAllText(@"C:\\000 test RPGMaker MV data\\" + Jsonname + "1.json", s);
        //    MessageBox.Show("test write finished");

        //    return true;
        //}

        //split string to several with equal lenght //работает криво, выдает результат "te" и "st" вместо "test1" и "test2" для "test1test2"
        //https://stackoverflow.com/questions/1450774/splitting-a-string-into-chunks-of-a-certain-size
        /*
        static IEnumerable<string> THSplit(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        */

        //моя функция деления строки на равные части с остатком и запись их в строковый массив
        public static string[] THSplit(string str, int chunkSize)
        {
            string[] parts = new string[chunkSize];

            int ind = 0;
            int strLength = str.Length;
            //памятка о приведении типов
            //https://www.aubrett.com/article/information-technology/web-development/net-framework/csharp/csharp-division-floating-point
            //THMsg.Show("strLength=" + strLength + ",str=" + str + ",f=" + f);
            int substrLength = (int)Math.Ceiling((double)strLength / chunkSize);//округление числа символов в части в большую сторону
            //THMsg.Show("f="+f+", substrLength=" + substrLength);
            int partsLength = parts.Length;
            for (int i = 0;i< partsLength; i++)
            {
                if (i == partsLength - 1)
                {
                    parts[i] = str.Substring(ind, strLength-ind);
                }
                else
                {
                    parts[i] = str.Substring(ind, substrLength);
                    ind += substrLength;
                }
            }

            return parts;
        }

        private void TestSplitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //string[] s = THSplit("test1test2ddd", 3);
            //string infoabouts = "";
            //for (int i = 0;i< s.Length; i++)
            //{
            //    infoabouts += ", s[" + i + "]=" + s[i];
            //}

            //THMsg.Show("s.Length=" + s.Length + infoabouts);
        }

        private bool GetAnyFileWithTheNameExist(string name)
        {
            //исключение имен с недопустимыми именами для файла или папки
            //http://www.cyberforum.ru/post5599483.html
            if (name.Intersect(Path.GetInvalidFileNameChars()).Any())
            {
                //MessageBox.Show("GetAnyFileWithTheNameExist return false because invalid! name=" + name);
                return false;
            }
            //MessageBox.Show("THSelectedDir=" + THSelectedDir.Replace("\\www\\data", "\\www") + "\r\n, name=" + name + "\r\n, Count=" + Directory.EnumerateFiles(THSelectedDir, name + ".*", SearchOption.AllDirectories).Count());
            if (Directory.EnumerateFiles(THSelectedDir.Replace("\\www\\data", "\\www"), name + ".*", SearchOption.AllDirectories).Count() > 0)
            {
                //MessageBox.Show("GetAnyFileWithTheNameExist Returns True! name="+ name);
                return true;
            }
            //MessageBox.Show("GetAnyFileWithTheNameExist Returns False! name=" + name);
            return false;
        }

        private bool ReadJson(string Jsonname, string sPath)
        {
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(THSelectedDir);
                //using (var reader = new StreamReader(THSelectedDir+"\\"+ Jsonname+".json"))
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

                ProceedJToken(root, Jsonname);

                //treeView1.ExpandAll();
            }
            catch
            {
                //LogToFile("", true);
                return false;
            }
            finally
            {
                //LogToFile("", true);
                //MessageBox.Show("sss");
                //ds.Tables[Jsonname].Columns.Add("Translation");
                //ds.Tables[Jsonname].Columns["Original"].ReadOnly = true;
                //DGV.DataSource = ds.Tables[0];
                //treeView1.EndUpdate();
            }
            return true;

        }

        StringBuilder textsb = new StringBuilder();
        private string curcode = "";
        private string cType = "";
        //private string cCode = "";
        private string cName = "";
        //private string cId = "";
        private void ProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (propertyname == "code")
                {
                    curcode = token.ToString();
                    //cCode = "Code" + curcode;
                    //MessageBox.Show("propertyname="+ propertyname+",value="+ token.ToString());
                }
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (token.Type == JTokenType.String)
                {
                    string tokenvalue = token.ToString();
                    if (curcode != "401" && curcode != "405")
                    {
                        if (string.IsNullOrEmpty(textsb.ToString()))
                        {
                        }
                        else
                        {
                            string mergedstring = textsb.ToString();
                            if (GetAlreadyAddedInTable(Jsonname, mergedstring) || SelectedLocalePercentFromStringIsNotValid(mergedstring))
                            {
                            }
                            else
                            {
                                THFilesElementsDataset.Tables[Jsonname].Rows.Add(mergedstring);
                                THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add("JArray:Object");
                            }
                            textsb.Clear();
                        }
                        if (string.IsNullOrEmpty(tokenvalue) || GetAlreadyAddedInTable(Jsonname, tokenvalue) || SelectedLocalePercentFromStringIsNotValid(tokenvalue) || GetAnyFileWithTheNameExist(tokenvalue))
                        {
                        }
                        else
                        {
                            if (curcode == "102")
                            {
                                cName = "Choice";
                            }

                            //LogToFile("Jsonname=" + Jsonname);
                            //LogToFile("", true);
                            THFilesElementsDataset.Tables[Jsonname].Rows.Add(tokenvalue);
                            //dsinfo.Tables[0].Rows.Add(cType+"\\"+ cId + "\\" + cCode + "\\" + cName);
                            THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(cType + ":" + cName);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(textsb.ToString()))
                        {
                        }
                        else
                        {
                            textsb.Append("\r\n");
                        }
                        textsb.Append(tokenvalue);
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
                    cType = "JObject";
                    cName = property.Name;
                    ProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    //LogToFile("JArray=\r\n" + array[i]);
                    //var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    //childNode.Tag = array[i];
                    cType = "JArray";
                    ProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        private bool WriteJson(string Jsonname, string sPath)
        {
            //LogToFile("Jsonname = " + Jsonname);
            try
            {
                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                JToken root;
                //MessageBox.Show(THSelectedDir);
                //using (var reader = new StreamReader(THSelectedDir+"\\"+ Jsonname+".json"))
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

                WProceedJToken(root, Jsonname);

                Regex regex = new Regex(@"^\[null,(.+)\]$");//Корректировка формата записываемого json так, как в файлах RPGMaker MV
                File.WriteAllText(sPath, regex.Replace(root.ToString(Formatting.None),"[\r\nnull,\r\n$1\r\n]"));

                //treeView1.ExpandAll();
            }
            catch
            {
                //LogToFile("", true);
                return false;
            }
            finally
            {                
                //LogToFile("", true);
                //MessageBox.Show("sss");
                //ds.Tables[Jsonname].Columns.Add("Translation");
                //ds.Tables[Jsonname].Columns["Original"].ReadOnly = true;
                //DGV.DataSource = ds.Tables[0];
                //treeView1.EndUpdate();
            }
            return true;

        }

        private void WProceedJToken(JToken token, string Jsonname, string propertyname = "")
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                //LogToFile("JValue: " + propertyname + "=" + token.ToString());
                if (token.Type == JTokenType.String)
                {
                    string tokenvalue = token.ToString();
                    if (string.IsNullOrEmpty(tokenvalue) || SelectedLocalePercentFromStringIsNotValid(tokenvalue))
                    {
                    }
                    else
                    {

                        string parameter0value = tokenvalue;
                        if (string.IsNullOrEmpty(parameter0value))
                        {
                        }
                        else //if code not equal old code and newline is not empty
                        {
                            //ЕСЛИ ПОЗЖЕ СДЕЛАЮ ВТОРОЙ DATASET С ДАННЫМИ ID, CODE И TYPE (ДЛЯ ДОП. ИНФЫ В ТАБЛИЦЕ) , ТО МОЖНО БУДЕТ УСКОРИТЬ СОХРАНЕНИЕ ЗА СЧЕТ СЧИТЫВАНИЯ ЗНАЧЕНИЙ ТОЛЬКО ИЗ СООТВЕТСТВУЮЩИХ РАЗДЕЛОВ

                            for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
                            {
                                if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
                                {
                                }
                                else
                                {
                                    //Where здесь формирует новый массив из входного, из элементов входного, удовлетворяющих заданному условию
                                    //https://stackoverflow.com/questions/1912128/filter-an-array-in-c-sharp
                                    string[] origA = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString().Split('\n').Where(emptyvalues => !string.IsNullOrEmpty(emptyvalues.Replace("\r", ""))).ToArray();//Все строки, кроме пустых, чтобы потом исключить из проверки
                                    int origALength = origA.Length;
                                    if (origALength == 0)
                                    {
                                        origA = new string[1];
                                        origA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString();
                                        //LogToFile("(origALength == 0 : Set size to 1 and value0=" + THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString());
                                    }

                                    if (origALength > 0)
                                    {
                                        string[] transA = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString().Split('\n').Where(emptyvalues => !string.IsNullOrEmpty(emptyvalues.Replace("\r", ""))).ToArray();//Все строки, кроме пустых
                                        if (transA.Length == 0)
                                        {
                                            transA = new string[1];
                                            transA[0] = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
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
                                            //if (orig.Any(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", "")) ) )
                                            //А это считает количество пустых элементов в массиве
                                            //https://stackoverflow.com/questions/2391743/how-many-elements-of-array-are-not-null
                                            //int ymptyelementscnt = orig.Count(emptyvalue => string.IsNullOrEmpty(emptyvalue.Replace("\r", "")));

                                            transA = THSplit(transmerged, origALength); // и создать новый массив строк перевода поделенный на равные строки по кол.ву строк оригинала.
                                        }
                                        bool br = false; //это чтобы выйти потом из прохода по таблице и перейти к след. элементу json, если перевод был присвоен
                                        for (int i2 = 0; i2 < origALength; i2++)
                                        {
                                            //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", "") + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", "")));
                                            if (parameter0value == origA[i2].Replace("\r", "")) //Replace здесь убирает \r из за которой строки считались неравными
                                            {
                                                //LogToFile("parameter0value=" + parameter0value + ",orig[i2]=" + origA[i2].Replace("\r", "") + ", parameter0value == orig[i2] is " + (parameter0value == origA[i2].Replace("\r", "")));
                                                var t = token as JValue;
                                                t.Value = transA[i2].Replace("\r", ""); //Replace убирает \r

                                                //LogToFile("commoneventsdata[i].List[c].Parameters[0].String=" + commoneventsdata[i].List[c].Parameters[0].String + ",trans[i2]=" + transA[i2]);
                                                br = true;
                                                break;
                                            }
                                        }
                                        if (br) //выход из цикла прохода по всей таблице, если значение найдено для одной из строк оригинала, и переход к следующему элементу json
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (tokenvalue == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
                                        {
                                            var t = token as JValue;
                                            t.Value = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        /*
                        for (int i1 = 0; i1 < THFilesElementsDataset.Tables[Jsonname].Rows.Count; i1++)
                        {
                            if (THFilesElementsDataset.Tables[Jsonname].Rows[i1][1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString()))
                            {
                            }
                            else
                            {
                                if (tokenvalue == THFilesElementsDataset.Tables[Jsonname].Rows[i1][0].ToString())
                                {
                                    var t = token as JValue;
                                    t.Value = THFilesElementsDataset.Tables[Jsonname].Rows[i1][1].ToString();
                                    break;
                                }
                            }
                        }
                        */
                        //LogToFile("Jsonname=" + Jsonname);
                        //LogToFile("", true);
                        //THFilesElementsDataset.Tables[Jsonname].Rows.Add(tokenvalue);
                        //dsinfo.Tables[0].Rows.Add(cType+"\\"+ cId + "\\" + cCode + "\\" + cName);
                        //THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(cType + ":" + cName);
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
                    //cName = property.Name;
                    WProceedJToken(property.Value, Jsonname, property.Name);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
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

        bool savemenusNOTenabled = true;
        private void THFileElementsDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (FIleDataWasChanged && savemenusNOTenabled)
            {
                writeTranslationInGameToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                saveTranslationToolStripMenuItem.Enabled = true;
            }
        }

        private void TryToTranslateOnlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < THFileElementsDataGridView.SelectedCells.Count; i++)
            //{
            //    int rind = THFileElementsDataGridView.SelectedCells[i].RowIndex;
            //    int cind = 0;//2-поле untrans
            //    if (THFileElementsDataGridView.SelectedCells[i].Value == null)
            //    {

            //    }
            //    else
            //    {
            //        if (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString()))
            //        {
            //            try
            //            {
            //                THFileElementsDataGridView[cind + 1, rind].Value = GoogleAPI.Translate(THFileElementsDataGridView.SelectedCells[i].Value.ToString());//из исходников ESPTranslator

            //            }
            //            catch (Exception ex)
            //            {
            //                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //            }
            //            finally
            //            {
            //                //this._lblStatus.Text = string.Format("Translated in {0} mSec", (int)t.TranslationTime.TotalMilliseconds);
            //                this.Cursor = Cursors.Default;
            //            }
            //        }
            //    }
            //}


            //координаты стартовой строк, колонки оригинала и номера таблицы
            int cind = THFileElementsDataGridView.Columns["Original"].Index;//-поле untrans
            int tableindex = THFilesListBox.SelectedIndex;
            int[] selindexes = new int[THFileElementsDataGridView.SelectedCells.Count];
            for (int i = 0; i < selindexes.Length; i++)
            {
                selindexes[i] = THFileElementsDataGridView.SelectedCells[i].RowIndex;
            }

            THMsg.Show("selindexes[0]=" + selindexes[0] + "\r\ncind=" + cind + "\r\ntableindex=" + tableindex + "\r\nselected=" + selindexes.Length + ", lastselectedrowvalue=" + THFilesElementsDataset.Tables[tableindex].Rows[selindexes[0]][cind]);
            
            //https://ru.stackoverflow.com/questions/222414/%d0%9a%d0%b0%d0%ba-%d0%bf%d1%80%d0%b0%d0%b2%d0%b8%d0%bb%d1%8c%d0%bd%d0%be-%d0%b2%d1%8b%d0%bf%d0%be%d0%bb%d0%bd%d0%b8%d1%82%d1%8c-%d0%bc%d0%b5%d1%82%d0%be%d0%b4-%d0%b2-%d0%be%d1%82%d0%b4%d0%b5%d0%bb%d1%8c%d0%bd%d0%be%d0%bc-%d0%bf%d0%be%d1%82%d0%be%d0%ba%d0%b5 
            //почемуто так не переводит, переводчик кидает ошибку при заппуске в другом потоке
            //await Task.Run(() => OnlineTranslateSelected(cind, tableindex, selindexes));

            //http://www.sql.ru/forum/1149655/kak-peredat-parametr-s-metodom-delegatom
            Thread trans = new Thread(new ParameterizedThreadStart((obj) => OnlineTranslateSelected(cind, tableindex, selindexes)));
            //
            //..и фикс ошибки:
            //System.TypeInitializationException: Инициализатор типа "TranslationHelper.GoogleAPI" выдал исключение. ---> System.Threading.ThreadStateException: Создание экземпляра элемента управления ActiveX '8856f961-340a-11d0-a96b-00c04fd705a2' невозможно: текущий поток не находится в однопоточном контейнере
            //https://ru.stackoverflow.com/questions/412073/c-webbrowser-threadstateexception-%D0%9E%D0%B4%D0%BD%D0%BE%D0%BF%D0%BE%D1%82%D0%BE%D1%87%D0%BD%D1%8B%D0%B9-%D0%BA%D0%BE%D0%BD%D1%82%D0%B5%D0%B9%D0%BD%D0%B5%D1%80
            trans.SetApartmentState(ApartmentState.STA);
            //но при выборе только одной строчки почему-то кидает исключение
            trans.Start();

            //OnlineTranslateSelected(cind, tableindex, selindexes);
            //ProgressInfo(false);

        }

        //DataSet THTranslationCache = new DataSet();
        public static void TranslationCacheInit(DataSet DS)
        {
            DS.Reset();
            if (File.Exists(THTranslationCachePath))
            {
                ReadDBFile(DS, THTranslationCachePath);
            }
            else
            {
                DS.Tables.Add("TranslationCache");
                DS.Tables["TranslationCache"].Columns.Add("Original");
                DS.Tables["TranslationCache"].Columns.Add("Translation");
            }
            //MessageBox.Show("TranslationCache Rows.Count=" + THTranslationCache.Tables["TranslationCache"].Rows.Count+ "TranslationCache Columns.Count=" + THTranslationCache.Tables["TranslationCache"].Columns.Count);
        }

        public static string TranslationCacheFind(DataSet DS, string Input)
        {
            if (DS.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                {
                    //MessageBox.Show("Input=" + Input+"\r\nCache="+ THTranslationCache.Tables["TranslationCache"].Rows[i][0].ToString());
                    if (Input == DS.Tables[0].Rows[i][0].ToString())
                    {
                        return DS.Tables[0].Rows[i][1].ToString();
                    }
                }
            }
            return "";
        }

        public static void THTranslationCacheAdd(DataSet DS, string original, string translation)
        {
            //LogToFile("original=" + original+ ",translation=" + translation,true);
            DS.Tables[0].Rows.Add(original, translation);            
        }


        private void OnlineTranslateSelected(int cind, int tableindex, int[] selindexes)
        {
            try
            {
                using (DataSet THTranslationCache = new DataSet())
                {
                    TranslationCacheInit(THTranslationCache);

                    //проход по всем выбранным ячейкам
                    for (int i = 0; i < selindexes.Length; i++)
                    {
                        ProgressInfo(true, "Getting translation: " + (i + 1) + "/" + selindexes.Length);
                        //проверка пустого значения поля для перевода
                        //if (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString()))
                        if (THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind + 1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind + 1].ToString()))
                        {
                            //проверка наличия заданного процента romaji или other в оригинале
                            //if ( SelectedLocalePercentFromStringIsNotValid(THFileElementsDataGridView[cind, rind].Value.ToString()) || SelectedLocalePercentFromStringIsNotValid(THFileElementsDataGridView[cind, rind].Value.ToString(), "other"))
                            if (SelectedLocalePercentFromStringIsNotValid(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString()) || SelectedLocalePercentFromStringIsNotValid(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString(), "other"))
                            {
                            }
                            else
                            {
                                string onlinetranslation = TranslationCacheFind(THTranslationCache, THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString());

                                if (string.IsNullOrEmpty(onlinetranslation))
                                {
                                    LogToFile("Row value for translation=" + THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString(), true);

                                    //запрос перевода
                                    //string onlinetranslation = GoogleAPI.Translate(THFileElementsDataGridView.SelectedCells[i].Value.ToString());//из исходников ESPTranslator 
                                    onlinetranslation = GoogleAPI.Translate(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString());//из исходников ESPTranslator 
                                                                                                                                                            //string onlinetranslation = DEEPL.Translate(origvalue);//из исходников ESPTranslator 

                                    LogToFile("Result onlinetranslation=" + onlinetranslation, true);
                                    //проверка наличия результата и вторичная проверка пустого значения поля для перевода перед записью
                                    //if (!string.IsNullOrEmpty(onlinetranslation) && (THFileElementsDataGridView[cind + 1, rind].Value == null || string.IsNullOrEmpty(THFileElementsDataGridView[cind + 1, rind].Value.ToString())))
                                    if (!string.IsNullOrEmpty(onlinetranslation) && (THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind + 1] == null || string.IsNullOrEmpty(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind + 1].ToString())))
                                    {
                                        LogToFile("THTranslationCache Rows count="+ THTranslationCache.Tables[0].Rows.Count);
                                        THTranslationCache.Tables[0].Rows.Add(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString(), onlinetranslation);
                                        //THTranslationCacheAdd(THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind].ToString(), onlinetranslation);                                    

                                        //запись перевода
                                        //THFileElementsDataGridView[cind + 1, rind].Value = onlinetranslation;
                                        THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind + 1] = onlinetranslation;
                                    }
                                }
                                else
                                {
                                    THFilesElementsDataset.Tables[tableindex].Rows[selindexes[i]][cind + 1] = onlinetranslation;
                                }
                            }
                        }
                    }
                    if (THTranslationCache.Tables[0].Rows.Count > 0)
                    {
                        WriteDBFile(THTranslationCache, THTranslationCachePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogToFile("Error: "+ex,true);
            }

            ProgressInfo(false);
        }
        
        private void OpenInWebToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.THSettingsWebTransLinkTextBox.Text.Replace("{languagefrom}","auto").Replace("{languageto}","en").Replace("{text}", THFileElementsDataGridView.CurrentCell.Value.ToString()));
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
}