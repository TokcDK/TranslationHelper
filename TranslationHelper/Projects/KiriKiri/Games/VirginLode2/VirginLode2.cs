using System;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.VirginLode2
{
    class VirginLode2 : KiriKiriGameBase
    {
        public VirginLode2(THDataWork thDataWork) : base(thDataWork)
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
                    if (new Formats.KiriKiri.Games.VirginLode2.KS(thDataWork).Open())
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
                if (thDataWork.SPath.GetMD5() == "dacf4898da60741356cc5c254774e5cb")
                {
                    return true;
                }
            }
            return false;
        }

        internal override string ProjectTitle()
        {
            return "Virgin Lode 2";
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
