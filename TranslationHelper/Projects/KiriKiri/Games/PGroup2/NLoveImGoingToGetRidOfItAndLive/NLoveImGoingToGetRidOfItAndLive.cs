﻿using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Projects.KiriKiri.Games.PGroup2;

namespace TranslationHelper.Projects.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive
{
    class NLoveImGoingToGetRidOfItAndLive : PGroup2Base
    {
        public NLoveImGoingToGetRidOfItAndLive() : base()
        {
            exeCRC = "a83b58570eee4fabfd0e91f3fc67beab";
        }

        internal override string Name()
        {
            return "Nラブ ―ネ取りネ取られ生きるのさ―";
        }

        internal override bool Check()
        {
            if (CheckKiriKiriBase())
            {
                if (exeCRC.Length > 0 && ProjectData.SelectedFilePath.GetMD5() == exeCRC)
                {
                    return true;
                }
            }
            return false;
        }

        protected override List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase> { new TranslationHelper.Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive.KS() };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks" };
        }

        //private bool SaveFilesOld()
        //{
        //    bool ret = false;

        //    ProjectData.CurrentProject.FillTablesLinesDict();

        //    //PatchDir
        //    //var PatchDir = Directory.CreateDirectory(Path.Combine(ProjectData.ProjectWorkDir, "_patch"));

        //    foreach (var file in Directory.EnumerateFileSystemEntries(ProjectData.ProjectWorkDir, "*.ks", SearchOption.AllDirectories))
        //    {
        //        if (!ProjectData.THFilesElementsDataset.Tables.Contains(Path.GetFileName(file)))
        //        {
        //            continue;
        //        }
        //        ProjectData.FilePath = file;
        //        ProjectData.Main.ProgressInfo(true, T._("opening file: ") + Path.GetFileName(file));
        //        try
        //        {
        //            if (new Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive.KS().Save())
        //            {
        //                ret = true;
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    ProjectData.TablesLinesDict.Clear();

        //    //PackTranslatedFilesInPatch();

        //    ProjectData.Main.ProgressInfo(false);

        //    return ret;
        //}
    }
}
