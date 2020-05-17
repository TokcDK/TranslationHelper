using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive
{
    class NLoveImGoingToGetRidOfItAndLive : KiriKiriGameBase
    {
        public NLoveImGoingToGetRidOfItAndLive(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            ExtractXP3Data();

            return OpenFiles();
        }

        private bool OpenFiles()
        {
            bool ret = false;
            foreach (var file in Directory.EnumerateFileSystemEntries(Properties.Settings.Default.THProjectWorkDir, "*.ks", SearchOption.AllDirectories))
            {
                thDataWork.FilePath = file;
                thDataWork.Main.ProgressInfo(true, T._("opening file: ") + Path.GetFileName(file));
                try
                {
                    if (new Formats.KiriKiri.Games.NLoveImGoingToGetRidOfItAndLive.KS(thDataWork).Open())
                    {
                        ret = true;
                    }
                }
                catch
                {
                }
            }



            thDataWork.Main.ProgressInfo(false);
            return ret;
        }

        internal override bool OpenDetect()
        {
            if (DetectBaseFiles())
            {
                if (thDataWork.SPath.GetMD5() == "a83b58570eee4fabfd0e91f3fc67beab")
                {
                    return true;
                }
            }
            return false;
        }

        internal override string ProjectTitle()
        {
            return "Nラブ ―ネ取りネ取られ生きるのさ―";
        }

        internal override bool Save()
        {
            bool ret = false;

            thDataWork.CurrentProject.FillTHFilesElementsDictionary();

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
            thDataWork.THFilesElementsDictionary.Clear();

            //PackTranslatedFilesInPatch();

            thDataWork.Main.ProgressInfo(false);

            return ret;
        }
    }
}
