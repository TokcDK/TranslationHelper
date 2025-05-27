using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.LiveMaker;
using TranslationHelper.Functions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.LiveMaker
{
    class LiveMakerGame : ProjectBase
    {
        public LiveMakerGame()
        {
        }

        internal override bool IsValid()
        {
            return ProjectTools.IsExe(AppData.SelectedProjectFilePath) && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "live.dll"));
        }

        public override string Name => "LiveMaker";

        internal override string ProjectDBFolderName => Name;

        protected override bool TryOpen()
        {
            return ExtractRes() && ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted"), Format(), Mask(), false);
        }
        protected List<System.Type> Format()
        {
            return new List<System.Type> {
                typeof(LSBCSV),
                typeof(LSBLNS)
            };
        }

        protected static string[] Mask()
        {
            return new[] { "*.csv", "*.lns" };
        }

        private bool ExtractRes()
        {
            //https://pylivemaker.readthedocs.io/en/latest/usage.html
            var GameDir = AppData.CurrentProject.SelectedGameDir;
            var WorkDir = (AppData.CurrentProject.ProjectWorkDir = AppData.CurrentProject.ProjectWorkDir.Length == 0 ? Path.Combine(THSettings.WorkDirPath, this.ProjectDBFolderName, Path.GetFileName(GameDir)) : AppData.CurrentProject.ProjectWorkDir);

            try
            {
                var gameresoutput = Path.Combine(WorkDir, "output");
                var gameextractcommand =
                    //THSettingsData.PyLiveMakerLMARExtractionToolsPath()
                    " x \""
                    + AppData.SelectedProjectFilePath
                    + "\" -o \""
                    + gameresoutput
                    + "\""
                    ;
                //Directory.CreateDirectory(gameresoutput);
                if (!File.Exists(Path.Combine(gameresoutput, "INSTALL.DAT")))
                {
                    Logger.Info(T._("Resource extraction") + "..");
                    FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMARExtractionToolsPath, gameextractcommand, "", true, false);
                }

                //MessageBox.Show("game extracted!");

                if (!File.Exists(Path.Combine(gameresoutput, "INSTALL.DAT")))
                {
                    return false;
                }

                //extract lsb to text and menu csv
                var csvdir = Path.Combine(WorkDir, "Extracted");
                if (/*(!Extensions.ExtensionsFileFolder.ContainsFiles(csvdir, "*.csv") || !Extensions.ExtensionsFileFolder.ContainsFiles(csvdir, "*.lns", true, true))
                    &&*/ Extensions.ExtensionsFileFolder.ContainsFiles(gameresoutput, "*.lsb"))
                {
                    Directory.CreateDirectory(csvdir);
                    foreach (var lsb in Directory.EnumerateFiles(gameresoutput, "00*.lsb"))
                    {
                        var name = Path.GetFileNameWithoutExtension(lsb);

                        Logger.Info(T._("Proceed") + " " + name + ".lsb" + "..");

                        if (File.Exists(lsb + ".bak"))
                        {
                            File.Move(lsb, lsb + ".old");
                            File.Move(lsb + ".bak", lsb);
                            File.Delete(lsb + ".old");
                        }

                        //extract text
                        var textcsvname = name + ".text.csv";
                        var textcsv = Path.Combine(csvdir, textcsvname);
                        if (!File.Exists(textcsv) && !File.Exists(textcsv + ".skip"))
                        {
                            Logger.Info(name + ".lsb: " + T._("text extraction in csv") + "..");

                            var textcommand = "extractcsv -e utf-8-sig --overwrite \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + textcsvname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, textcommand, gameresoutput, true, false);
                            if (!File.Exists(textcsv))
                            {
                                File.WriteAllText(textcsv + ".skip", "");
                            }
                        }


                        //extract menu
                        var menucsvname = name + ".menu.csv";
                        var menucsv = Path.Combine(csvdir, name + ".menu.csv");
                        if (!File.Exists(menucsv) && !File.Exists(menucsv + ".skip"))
                        {
                            Logger.Info(name + ".lsb: " + T._("menu text extraction in csv") + "..");

                            var menucommand = "extractmenu -e utf-8-sig --overwrite \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + menucsvname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, menucommand, gameresoutput, true, false);
                            if (!File.Exists(menucsv))
                            {
                                File.WriteAllText(menucsv + ".skip", "");
                            }
                        }

                        //extract lns data
                        var lnsname = name;
                        var lns = Path.Combine(csvdir, "LNS", name);
                        if (!Directory.Exists(lns) && !Directory.Exists(lns + ".skip"))
                        {
                            Logger.Info(name + ".lsb: " + T._("scripts extraction in lns") + "..");

                            var lnscommand = "extract \"" + ".\\" + name + ".lsb\" -o \"" + "..\\Extracted\\LNS\\" + lnsname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, lnscommand, gameresoutput, true, false);
                            if (!Directory.Exists(lns) || !File.Exists(Path.Combine(lns, lnsname + ".lsbref")))
                            {
                                Directory.CreateDirectory(lns + ".skip");
                            }
                        }

                        //var lns = Path.Combine(csvdir, "LNS", Path.GetFileName(lsb) + "menu.csv");
                        //var lnscommand = "extract \"" + lsb + "\" -o \"" + lns + "\"";
                        //FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), menucommand);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while Livemaker project extraction. error:\r\n" + ex);
            }

            return false;
        }

        protected override bool TrySaveProject()
        {
            return ProjectToolsOpenSave.OpenSaveFilesBase(this, Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted"), Format(), Mask())
                && WriteTranslation();
        }

        private bool WriteTranslation()
        {
            try
            {
                var csvdir = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted");
                var gameresoutput = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "output");

                if (Extensions.ExtensionsFileFolder.ContainsFiles(csvdir, "*.csv")
                    && Extensions.ExtensionsFileFolder.ContainsFiles(gameresoutput, "*.lsb"))
                {
                    var lsblist = Directory.GetFiles(gameresoutput, "00*.lsb");
                    var lsblistmax = lsblist.Length;
                    for (int i = 0; i < lsblistmax; i++)
                    {
                        string lsb = lsblist[i];
                        var name = Path.GetFileNameWithoutExtension(lsb);

                        var progress = i + "/" + lsblistmax + ":";

                        Logger.Info(progress + T._("Proceed") + " " + name + ".lsb" + "..");

                        //insert text translations
                        var textcsvname = name + ".text.csv";
                        var textcsv = Path.Combine(csvdir, textcsvname);
                        if (File.Exists(textcsv))
                        {
                            Logger.Info(progress + name + ".lsb: " + T._("write text"));

                            var textcommand = "insertcsv -e utf-8-sig \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + textcsvname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, textcommand, gameresoutput, true, false);
                        }

                        //insert menu translations
                        var menucsvname = name + ".menu.csv";
                        var menucsv = Path.Combine(csvdir, name + ".menu.csv");
                        if (File.Exists(menucsv))
                        {
                            Logger.Info(progress + name + ".lsb: " + T._("write menus"));

                            var menucommand = "insertmenu -e utf-8-sig \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + menucsvname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, menucommand, gameresoutput, true, false);
                        }

                        //insert LNS
                        var lnsname = name;
                        var lns = Path.Combine(csvdir, "LNS", name);
                        if (Directory.Exists(lns))
                        {
                            Logger.Info(progress + name + ".lsb: " + T._("write scripts"));

                            var lnscommand = "batchinsert \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\LNS\\" + lnsname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, lnscommand, gameresoutput, true, false);
                        }
                    }
                }

                //copy lsb for insert
                Logger.Info(T._("Copy lsb for insertion"));
                var insertDir = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "insert");
                Directory.CreateDirectory(insertDir);
                foreach (var lsb in Directory.EnumerateFiles(gameresoutput, "00*.lsb"))
                {
                    if (File.Exists(Path.Combine(insertDir, Path.GetFileName(lsb))))
                    {
                        File.Delete(Path.Combine(insertDir, Path.GetFileName(lsb)));
                    }

                    File.Copy(lsb, Path.Combine(insertDir, Path.GetFileName(lsb)));
                }

                //check lsb for errors
                //var Bat = Path.Combine(insertDir, "validate.bat");
                //foreach (var lsb in Directory.EnumerateFiles(insertDir, "00*.lsb"))
                //{
                //    var name = Path.GetFileName(lsb);
                //    ProjectData.Main.ProgressInfo(T._("Check lsb for errors") + ":" + name);
                //    var ValidateBatContent = "\"" + THSettingsData.PyLiveMakerLMLSBExtractionToolPath() + "\" validate " + name + " >" + name + ".validate.txt"
                //        ;

                //    File.WriteAllText(Bat, ValidateBatContent);

                //    var b = FunctionsProcess.RunProcess(Bat, "", insertDir, true, false);

                //    var text = File.ReadAllText(lsb + ".validate.txt");
                //    if (text.ToUpperInvariant().Contains("ERROR") || text.ToUpperInvariant().Contains("FAILED") || text.ToUpperInvariant().Contains("WRONG"))
                //    {
                //        MessageBox.Show("Errors found!");
                //    }
                //}
                //File.Delete(Bat);

                //temporary rename backup because lmlsb will not write lsb when backup file exists
                if (File.Exists(AppData.SelectedProjectFilePath + ".bak"))
                {
                    File.Move(AppData.SelectedProjectFilePath + ".bak", AppData.SelectedProjectFilePath + ".bak1");
                }

                //insert lsb to exe from insert dir
                Logger.Info(T._("Inserting lsb in game") + "...");
                var command = "-r \"" + AppData.SelectedProjectFilePath + "\" \"" + "..\\insert\"";
                FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMPATCHExtractionToolPath, command, gameresoutput, true, false);

                //delete old bak if new was made by lmlsb
                if (File.Exists(AppData.SelectedProjectFilePath + ".bak"))
                {
                    File.Delete(AppData.SelectedProjectFilePath + ".bak1");
                }

                

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occured while Livemaker project packing. error:\r\n" + ex);
            }

            return false;
        }

        public override bool BakCreate()
        {
            var bakData = Directory.GetFiles(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "output"), "*.lsb");

            bakData = bakData.Union(new[] { AppData.SelectedProjectFilePath }).ToArray();

            if (Directory.Exists(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted"), "*.csv")).ToArray();
            }

            if (Directory.Exists(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted", "LNS")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted", "LNS"), "*.lns", SearchOption.AllDirectories)).ToArray();
            }

            return ProjectToolsBackup.BackupRestorePaths(bakData);
        }

        public override bool BakRestore()
        {
            string[] bakData = new string[1];

            if(Directory.Exists(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "output")))
            {
                bakData = Directory.GetFiles(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "output"), "*.lsb");
            }

            bakData = bakData.Union(new[] { AppData.SelectedProjectFilePath }).ToArray();

            if (Directory.Exists(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted"), "*.csv")).ToArray();
            }

            if (Directory.Exists(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted", "LNS")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(AppData.CurrentProject.ProjectWorkDir, "Extracted", "LNS"), "*.lns", SearchOption.AllDirectories)).ToArray();
            }

            return ProjectToolsBackup.BackupRestorePaths(bakData, false);
        }

        internal override string OnlineTranslationProjectSpecificPretranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            //removemarkup where many of them and impossible to extract
            o = Regex.Replace(o, @"<STYLE ID\=\""([0-9])\"">", "")
                .Replace("</STYLE>", "")
                ;
            return base.OnlineTranslationProjectSpecificPretranslationAction(o, t, tind, rind);
        }
    }
}
