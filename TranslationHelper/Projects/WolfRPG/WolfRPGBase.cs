using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Menus.ProjectMenus;
using TranslationHelper.Projects.WolfRPG.Menus;

namespace TranslationHelper.Projects.WolfRPG
{
    abstract class WolfRPGBase : ProjectBase
    {
        protected WolfRPGBase()
        {
            HideVarsBase = new Dictionary<string, string>
            {
                {"\\cself[", @"「?\\\\cself\[[0-9]{1,12}\]」?"},
                {"\\img[", @"\\\\img\[[^\]]+\]"},
                {"\\cdb[", @"\\\\cdb\[[^:]+:[^:]+:[^:\]]+\]"},
                {"\\udb[", @"\\\\udb\[[^:]+:[^:]+:[^:\]]+\]"}
            };
        }
        internal override bool IsValid()
        {
            string d;
            return ProjectTools.IsExe(AppData.SelectedFilePath)
                && (File.Exists(Path.Combine(d = Path.GetDirectoryName(AppData.SelectedFilePath), "DATA.wolf"))
                || FunctionsFileFolder.IsInDirExistsAnyFile(d = Path.GetDirectoryName(AppData.SelectedFilePath), "*.wolf", recursive: true)
                || (Directory.Exists(d = Path.Combine(d, "Data")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.wolf", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "MapData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.mps", recursive: true))
                || (Directory.Exists(d = Path.Combine(d, "BasicData")) && FunctionsFileFolder.IsInDirExistsAnyFile(d, "*.dat", recursive: true))
                );
        }

        protected bool ExtractWolfFiles()
        {
            var ret = false;

            try
            {
                var progressMessageTitle = "Wolf archive" + " " + (OpenFileMode ? T._("Create patch") : T._("Write patch")) + ".";

                var dataPath = Path.Combine(AppData.CurrentProject.SelectedGameDir, "Data");

                //decode wolf files
                if (OpenFileMode)
                {
                    //var wolfextractor = THSettings.WolfRPGExtractorExePath();
                    var wolfextractor = THSettings.WolfdecExePath;
                    foreach (var wolfFile in Directory.EnumerateFiles(AppData.CurrentProject.SelectedGameDir, "*.wolf", SearchOption.AllDirectories))
                    {
                        var nameNoExt = Path.GetFileNameWithoutExtension(wolfFile).ToLowerInvariant();
                        if (nameNoExt.Contains("cg")
                            || nameNoExt.Contains("anime")
                            || nameNoExt.Contains("bgm")
                            || nameNoExt.Contains("se")
                            || nameNoExt.Contains("me")
                            || nameNoExt.Contains("voice")
                            || nameNoExt.Contains("music")
                            || nameNoExt.Contains("picture")
                            || nameNoExt.Contains("graphic")
                            || nameNoExt.Contains("effect")
                            ) continue;

                        var extractedDirPath = new DirectoryInfo(Path.Combine(dataPath, Path.GetFileNameWithoutExtension(wolfFile)));
                        if (extractedDirPath.Exists && !extractedDirPath.IsEmpty()) continue;

                        //.mps, .dat, .project
                        AppData.Main.ProgressInfo(true, progressMessageTitle + T._("Extract") + " " + Path.GetFileName(wolfFile));
                        if (FunctionsProcess.RunProcess(wolfextractor, "\"" + wolfFile + "\""))
                        {
                            ret = true;
                            File.Move(wolfFile, wolfFile + ".bak");
                        }
                    }

                    if (!Directory.Exists(dataPath)) return false;
                }
            }
            catch { }

            return ret;
        }

        internal override string ProjectDBFolderName => "WolfRPG";
        internal override bool TablesLinesDictAddEqual => true;

        internal override string OnlineTranslationProjectSpecificPosttranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            t = HardcodedFixes(o, t);

            return RestoreVARS(t);
        }
        //\\\\r\[[^\,]+\,[^\]]+\]
        internal override string HardcodedFixes(string original, string translation)
        {
            //fix escape sequences 
            if (Regex.IsMatch(translation, @"(?<!\\)\\[^sntr><#\\]"))
            {
                var sequences = new char[] { 'S', 'N', 'T', 'R' };
                var mc = Regex.Matches(translation, @"(?<!\\)\\[^sntr><#\\]");
                for (int i = mc.Count - 1; i >= 0; i--)
                {
                    foreach (var schar in sequences)
                    {
                        if (mc[i].Value == "\\" + schar)
                        {
                            translation = translation.Remove(mc[i].Index, mc[i].Length).Insert(mc[i].Index, ("\\" + schar).ToLower());
                        }
                    }
                }
            }
            return translation;
        }

        internal override List<IMenuItem> GridItemMenusList()
        {
            return new List<IMenuItem>() { new AddToStandaloneContextList() };
        }
    }
}
