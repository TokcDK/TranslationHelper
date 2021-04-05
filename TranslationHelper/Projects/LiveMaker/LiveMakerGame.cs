﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Formats.LiveMaker;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.LiveMaker
{
    class LiveMakerGame : ProjectBase
    {
        public LiveMakerGame(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            return IsExe() && File.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "live.dll"));
        }

        internal override string Name()
        {
            return "LiveMaker";
        }

        internal override string ProjectFolderName()
        {
            return Name();
        }

        internal override bool Open()
        {
            return ExtractRes() && OpenSaveFilesBase(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted"), Format(), Mask(), false);
        }
        protected List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase> {
                new LSBCSV(thDataWork),
                new LSBLNS(thDataWork)
            };
        }

        protected static string[] Mask()
        {
            return new[] { "*.csv", "*.lns" };
        }

        private bool ExtractRes()
        {
            //https://pylivemaker.readthedocs.io/en/latest/usage.html
            var GameDir = Properties.Settings.Default.THSelectedGameDir;
            var WorkDir = (Properties.Settings.Default.THProjectWorkDir = Properties.Settings.Default.THProjectWorkDir.Length == 0 ? Path.Combine(THSettingsData.WorkDirPath(), this.ProjectFolderName(), Path.GetFileName(GameDir)) : Properties.Settings.Default.THProjectWorkDir);

            try
            {
                var gameresoutput = Path.Combine(WorkDir, "output");
                var gameextractcommand =
                    //THSettingsData.PyLiveMakerLMARExtractionToolsPath()
                    " x \""
                    + thDataWork.SPath
                    + "\" -o \""
                    + gameresoutput
                    + "\""
                    ;
                //Directory.CreateDirectory(gameresoutput);
                if (!File.Exists(Path.Combine(gameresoutput, "INSTALL.DAT")))
                {
                    thDataWork.Main.ProgressInfo(T._("Resource extraction") + "..");
                    FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMARExtractionToolsPath(), gameextractcommand, "", true, false);
                }

                thDataWork.Main.ProgressInfo();

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

                        thDataWork.Main.ProgressInfo(T._("Proceed") + " " + name + ".lsb" + "..");

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
                            thDataWork.Main.ProgressInfo(name + ".lsb: " + T._("text extraction in csv") + "..");

                            var textcommand = "extractcsv -e utf-8-sig --overwrite \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + textcsvname + "\"";
                            FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), textcommand, gameresoutput, true, false);
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
                            thDataWork.Main.ProgressInfo(name + ".lsb: " + T._("menu text extraction in csv") + "..");

                            var menucommand = "extractmenu -e utf-8-sig --overwrite \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + menucsvname + "\"";
                            FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), menucommand, gameresoutput, true, false);
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
                            thDataWork.Main.ProgressInfo(name + ".lsb: " + T._("scripts extraction in lns") + "..");

                            var lnscommand = "extract \"" + ".\\" + name + ".lsb\" -o \"" + "..\\Extracted\\LNS\\" + lnsname + "\"";
                            FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), lnscommand, gameresoutput, true, false);
                            if (!Directory.Exists(lns) || !File.Exists(Path.Combine(lns, lnsname + ".lsbref")))
                            {
                                Directory.CreateDirectory(lns + ".skip");
                            }
                        }

                        //var lns = Path.Combine(csvdir, "LNS", Path.GetFileName(lsb) + "menu.csv");
                        //var lnscommand = "extract \"" + lsb + "\" -o \"" + lns + "\"";
                        //FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), menucommand);

                        thDataWork.Main.ProgressInfo();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("An error occured while Livemaker project extraction. error:\r\n" + ex);
            }

            return false;
        }

        internal override bool Save()
        {
            return OpenSaveFilesBase(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted"), Format(), Mask())
                && WriteTranslation();
        }

        private bool WriteTranslation()
        {
            try
            {
                var csvdir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted");
                var gameresoutput = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "output");

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

                        thDataWork.Main.ProgressInfo(progress + T._("Proceed") + " " + name + ".lsb" + "..");
                        thDataWork.Main.ProgressInfo(i, lsblistmax);

                        //insert text translations
                        var textcsvname = name + ".text.csv";
                        var textcsv = Path.Combine(csvdir, textcsvname);
                        if (File.Exists(textcsv))
                        {
                            thDataWork.Main.ProgressInfo(progress + name + ".lsb: " + T._("write text"));

                            var textcommand = "insertcsv -e utf-8-sig \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + textcsvname + "\"";
                            FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), textcommand, gameresoutput, true, false);
                        }

                        //insert menu translations
                        var menucsvname = name + ".menu.csv";
                        var menucsv = Path.Combine(csvdir, name + ".menu.csv");
                        if (File.Exists(menucsv))
                        {
                            thDataWork.Main.ProgressInfo(progress + name + ".lsb: " + T._("write menus"));

                            var menucommand = "insertmenu -e utf-8-sig \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + menucsvname + "\"";
                            FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), menucommand, gameresoutput, true, false);
                        }

                        //insert LNS
                        var lnsname = name;
                        var lns = Path.Combine(csvdir, "LNS", name);
                        if (Directory.Exists(lns))
                        {
                            thDataWork.Main.ProgressInfo(progress + name + ".lsb: " + T._("write scripts"));

                            var lnscommand = "batchinsert \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\LNS\\" + lnsname + "\"";
                            FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMLSBExtractionToolPath(), lnscommand, gameresoutput, true, false);
                        }
                    }
                }

                thDataWork.Main.ProgressInfo();

                //copy lsb for insert
                thDataWork.Main.ProgressInfo(T._("Copy lsb for insertion"));
                var insertDir = Path.Combine(Properties.Settings.Default.THProjectWorkDir, "insert");
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
                //    thDataWork.Main.ProgressInfo(T._("Check lsb for errors") + ":" + name);
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
                if (File.Exists(thDataWork.SPath + ".bak"))
                {
                    File.Move(thDataWork.SPath + ".bak", thDataWork.SPath + ".bak1");
                }

                //insert lsb to exe from insert dir
                thDataWork.Main.ProgressInfo(T._("Inserting lsb in game") + "...");
                var command = "-r \"" + thDataWork.SPath + "\" \"" + "..\\insert\"";
                FunctionsProcess.RunProcess(THSettingsData.PyLiveMakerLMPATCHExtractionToolPath(), command, gameresoutput, true, false);

                //delete old bak if new was made by lmlsb
                if (File.Exists(thDataWork.SPath + ".bak"))
                {
                    File.Delete(thDataWork.SPath + ".bak1");
                }

                thDataWork.Main.ProgressInfo();

                return true;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("An error occured while Livemaker project packing. error:\r\n" + ex);
            }

            return false;
        }

        internal override bool BakCreate()
        {
            var bakData = Directory.GetFiles(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "output"), "*.lsb");

            bakData = bakData.Union(new[] { thDataWork.SPath }).ToArray();

            if (Directory.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted"), "*.csv")).ToArray();
            }

            if (Directory.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted", "LNS")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted", "LNS"), "*.lns", SearchOption.AllDirectories)).ToArray();
            }

            return BackupRestorePaths(bakData);
        }

        internal override bool BakRestore()
        {
            string[] bakData = new string[1];

            if(Directory.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "output")))
            {
                bakData = Directory.GetFiles(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "output"), "*.lsb");
            }

            bakData = bakData.Union(new[] { thDataWork.SPath }).ToArray();

            if (Directory.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted"), "*.csv")).ToArray();
            }

            if (Directory.Exists(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted", "LNS")))
            {
                bakData = bakData.Union(Directory.GetFiles(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "Extracted", "LNS"), "*.lns", SearchOption.AllDirectories)).ToArray();
            }

            return BackupRestorePaths(bakData, false);
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