using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class KiriKiriGameUtils
    {
        internal static string KiriKiriTranslationSuffix = ".translation";

        /// <summary>
        /// Get ks and tjs files from the folder
        /// </summary>
        /// <param name="targetSubFolder"></param>
        /// <returns></returns>
        internal static List<FileInfo> GetKiriKiriScriptPaths(DirectoryInfo targetSubFolder, string[] masks)
        {
            return targetSubFolder.GetFileInfosList(masks);
        }

        internal static void ReCreateFolder(DirectoryInfo targetSubFolder)
        {
            if (!targetSubFolder.Exists)
                return;

            targetSubFolder.Attributes = FileAttributes.Normal;
            targetSubFolder.Delete(true);
            targetSubFolder.Create();
        }
    }

    /// <summary>
    /// The patch XP3 files of one game.
    /// <para>
    /// The folder is supplied rather than read from the application, because there is one of these per
    /// open project and a version that asked for "the" game folder would describe whichever project
    /// happened to be selected.
    /// </para>
    /// </summary>
    class Xp3PatchInfos
    {
        internal List<Xp3Patch> Xp3PatchList;
        internal string Splitter = "";
        internal int MaxIndex = 0;
        internal string MaxIndexString { get => (MaxIndex > 0 ? MaxIndex + "" : ""); }

        /// <summary>
        /// The folder of the game whose patch files are described.
        /// </summary>
        private readonly string _gameDir;

        public Xp3PatchInfos(string gameDir)
        {
            _gameDir = gameDir;

            Xp3PatchList = new List<Xp3Patch>();
            Get();
        }

        /// <summary>
        /// get patch.xp3 files
        /// </summary>
        /// <returns></returns>
        internal void Get()
        {
            if (string.IsNullOrWhiteSpace(_gameDir)) return;

            foreach (var xp3 in new[] { "scripts", "scenario", "data" })
            {
                var path = new FileInfo(Path.Combine(_gameDir, xp3 + ".xp3"));
                if (!path.Exists) continue;

                var info = new Xp3Patch();
                info.FileInfo = path;

                if (!Xp3PatchList.Contains(info)) Xp3PatchList.Insert(0, info);
            }

            foreach (var xp3Patch in new DirectoryInfo(_gameDir).EnumerateFiles("patch*.xp3"))
            {
                if (File.Exists(xp3Patch + ".translation")) continue;

                // get splitter and number
                var patchMatch = Regex.Match(xp3Patch.Name, @"[Pp]atch([^0-9]*)([^0-9]*)\.xp3");
                if (!patchMatch.Success) continue;

                // set info
                var info = new Xp3Patch();
                info.FileInfo = xp3Patch;

                var splitter = patchMatch.Result("$1");
                if (!string.IsNullOrEmpty(splitter) && string.IsNullOrEmpty(Splitter)) Splitter = patchMatch.Result(splitter);

                info.IndexString = patchMatch.Result("$2");
                if (int.TryParse(info.IndexString, out int ind)) info.Index = ind;

                info.IsTranslation = File.Exists(xp3Patch + KiriKiriGameUtils.KiriKiriTranslationSuffix);
                MaxIndex = MaxIndex >= info.Index ? MaxIndex : info.Index;

                // add info
                if (!Xp3PatchList.Contains(info)) Xp3PatchList.Add(info);
            }
        }
    }
    class Xp3Patch
    {
        internal FileInfo FileInfo;
        internal int Index = 0;
        internal string IndexString = "";
        internal bool IsTranslation = false;
    }
}
