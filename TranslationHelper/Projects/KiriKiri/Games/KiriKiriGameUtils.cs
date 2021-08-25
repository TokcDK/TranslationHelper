using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

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
        internal static List<FileInfo> GetPaths(DirectoryInfo targetSubFolder)
        {
            List<FileInfo> filePaths = new List<FileInfo>();
            string[] extensions = new string[2] { "*.ks", "*.tjs" };
            Parallel.ForEach(extensions, mask =>
            {
                Parallel.ForEach(targetSubFolder.EnumerateFiles(mask, SearchOption.AllDirectories), filePath =>
                {
                    filePaths.Add(filePath);
                });
            });

            return filePaths;
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

    class Xp3PatchInfos
    {
        internal List<Xp3Patch> Xp3PatchList;
        internal string Splitter = "";
        internal int MaxIndex = 0;
        internal string MaxIndexString { get => (MaxIndex > 0 ? MaxIndex + "" : ""); }

        public Xp3PatchInfos()
        {
            Xp3PatchList = new List<Xp3Patch>();
            Get();
        }

        /// <summary>
        /// get patch.xp3 files
        /// </summary>
        /// <returns></returns>
        internal void Get()
        {
            //var data = new FileInfo(Path.Combine(ProjectData.SelectedGameDir, "data.xp3"));
            //if (data.Exists)
            //{
            //    // set data info
            //    var info = new Xp3Patch
            //    {
            //        FileInfo = data
            //    };

            //    Xp3PatchList.Add(info);
            //}

            foreach (var xp3Patch in new DirectoryInfo(ProjectData.SelectedGameDir).EnumerateFiles("patch*.xp3"))
            {
                // get splitter and number
                var patchMatch = Regex.Match(xp3Patch.Name, @"[Pp]atch([^0-9]*)([^0-9]*)\.xp3");
                if (!patchMatch.Success)
                {
                    continue;
                }

                // set info
                var info = new Xp3Patch();
                info.FileInfo = xp3Patch;
                var splitter = patchMatch.Result("$1");
                if (!string.IsNullOrEmpty(splitter) && string.IsNullOrEmpty(Splitter))
                {
                    Splitter = patchMatch.Result(splitter);
                }
                info.IndexString = patchMatch.Result("$2");
                if (int.TryParse(info.IndexString, out int ind))
                {
                    info.Index = ind;
                }
                info.IsTranslation = File.Exists(xp3Patch + KiriKiriGameUtils.KiriKiriTranslationSuffix);
                MaxIndex = MaxIndex >= info.Index ? MaxIndex : info.Index;

                // add info
                if (!Xp3PatchList.Contains(info))
                {
                    Xp3PatchList.Add(info);
                }
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
