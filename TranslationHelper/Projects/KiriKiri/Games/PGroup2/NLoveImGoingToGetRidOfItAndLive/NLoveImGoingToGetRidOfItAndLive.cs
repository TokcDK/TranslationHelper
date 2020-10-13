﻿using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Projects.KiriKiri.Games.PGroup2;

namespace TranslationHelper.Projects.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive
{
    class NLoveImGoingToGetRidOfItAndLive : PGroup2Base
    {
        public NLoveImGoingToGetRidOfItAndLive(THDataWork thDataWork) : base(thDataWork)
        {
            exeCRC = "a83b58570eee4fabfd0e91f3fc67beab";
        }

        internal override bool Check()
        {
            if (CheckKiriKiriBase())
            {
                if (exeCRC.Length > 0 && thDataWork.SPath.GetMD5() == exeCRC)
                {
                    return true;
                }
            }
            return false;
        }

        internal override bool Open()
        {
            return ExtractXP3Data()
                && OpenFiles(new System.Collections.Generic.List<Formats.FormatBase>
                {
                    new Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive.KS(thDataWork)
                }
                );
        }

        internal override string Name()
        {
            return "Nラブ ―ネ取りネ取られ生きるのさ―";
        }

        internal override bool Save()
        {
            //SaveFilesOld();
            return SaveFiles(new System.Collections.Generic.List<Formats.FormatBase>
            {
                new Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive.KS(thDataWork)
            }
            );
        }

        private bool SaveFilesOld()
        {
            bool ret = false;

            thDataWork.CurrentProject.FillTablesLinesDict();

            //PatchDir
            //var PatchDir = Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.THProjectWorkDir, "_patch"));

            foreach (var file in Directory.EnumerateFileSystemEntries(Properties.Settings.Default.THProjectWorkDir, "*.ks", SearchOption.AllDirectories))
            {
                if (!thDataWork.THFilesElementsDataset.Tables.Contains(Path.GetFileName(file)))
                {
                    continue;
                }
                thDataWork.FilePath = file;
                thDataWork.Main.ProgressInfo(true, T._("opening file: ") + Path.GetFileName(file));
                try
                {
                    if (new Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive.KS(thDataWork).Save())
                    {
                        ret = true;
                    }
                }
                catch
                {
                }
            }
            thDataWork.TablesLinesDict.Clear();

            //PackTranslatedFilesInPatch();

            thDataWork.Main.ProgressInfo(false);

            return ret;
        }
    }
}