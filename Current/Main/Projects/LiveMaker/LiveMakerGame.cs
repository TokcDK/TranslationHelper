using System;
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
        public LiveMakerGame()
        {
        }

        internal override bool IsValid()
        {
            return ProjectTools.IsExe(AppData.SelectedProjectFilePath) && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "live.dll"));
        }

        public override string Name => "LiveMaker";

        internal override string ProjectDBFolderName => Name;

        public override bool Open()
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
                    " x \""
                    + AppData.SelectedProjectFilePath
                    + "\" -o \""
                    + gameresoutput
                    + "\""
                    ;
                if (!Directory.Exists(gameresoutput))
                {
                    Directory.CreateDirectory(gameresoutput);
                }
                if (!File.Exists(Path.Combine(gameresoutput, "INSTALL.DAT")))
                {
                    AppData.Main.ProgressInfo(T._("Resource extraction") + "..");
                    FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMARExtractionToolsPath, gameextractcommand, "", true, false);
                }

                AppData.Main.ProgressInfo();

                if (!File.Exists(Path.Combine(gameresoutput, "INSTALL.DAT")))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("An error occured while Livemaker project extraction. error:\r\n" + ex);
            }

            return false;
        }

        public override bool Save()
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

                if (!Directory.Exists(csvdir))
                {
                    Directory.CreateDirectory(csvdir);
                }

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

                        AppData.Main.ProgressInfo(progress + T._("Proceed") + " " + name + ".lsb" + "..");
                        AppData.Main.ProgressInfo(i, lsblistmax);

                        //insert text translations
                        var textcsvname = name + ".text.csv";
                        var textcsv = Path.Combine(csvdir, textcsvname);
                        if (File.Exists(textcsv))
                        {
                            AppData.Main.ProgressInfo(progress + name + ".lsb: " + T._("write text"));

                            var textcommand = "insertcsv -e utf-8-sig \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + textcsvname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, textcommand, gameresoutput, true, false);
                        }

                        //insert menu translations
                        var menucsvname = name + ".menu.csv";
                        var menucsv = Path.Combine(csvdir, name + ".menu.csv");
                        if (File.Exists(menucsv))
                        {
                            AppData.Main.ProgressInfo(progress + name + ".lsb: " + T._("write menus"));

                            var menucommand = "insertmenu -e utf-8-sig \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\" + menucsvname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, menucommand, gameresoutput, true, false);
                        }

                        //insert LNS
                        var lnsname = name;
                        var lns = Path.Combine(csvdir, "LNS", name);
                        if (Directory.Exists(lns))
                        {
                            AppData.Main.ProgressInfo(progress + name + ".lsb: " + T._("write scripts"));

                            var lnscommand = "batchinsert \"" + ".\\" + name + ".lsb\" \"" + "..\\Extracted\\LNS\\" + lnsname + "\"";
                            FunctionsProcess.RunProcess(THSettings.PyLiveMakerLMLSBExtractionToolPath, lnscommand, gameresoutput, true, false);
                        }
                    }
                }

                AppData.Main.ProgressInfo();

                return true;
            }
            catch (Exception ex)
            {
                new Functions.FunctionsLogs().LogToFile("An error occured while Livemaker project packing. error:\r\n" + ex);
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

