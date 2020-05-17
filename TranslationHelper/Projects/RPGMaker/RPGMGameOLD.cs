using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMTrans;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.RPGMaker
{
    class RPGMGameOLD
    {
        readonly THDataWork thDataWork;
        public RPGMGameOLD(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal bool TryToExtractToRPGMakerTransPatch(string sPath, string extractdir = "Work")
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            string workdir = Path.Combine(Application.StartupPath, extractdir, "RPGMakerTrans");
            if (!Directory.Exists(workdir))
            {
                Directory.CreateDirectory(workdir);
            }
            //MessageBox.Show("tempdir=" + tempdir);
            string outdir = Path.Combine(workdir, Path.GetFileNameWithoutExtension(Path.GetDirectoryName(sPath)));


            if (extractdir == "Work")
            {
                thDataWork.Main.extractedpatchpath = outdir + "_patch";// Распаковывать в Work\ProjectDir\
            }

            //if (!Directory.Exists(outdir))
            //{
            //    Directory.CreateDirectory(outdir);

            //    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

            //}
            if (Directory.Exists(outdir + "_patch") && FunctionsFileFolder.IsInDirExistsAnyFile(outdir + "_patch", "RPGMKTRANSPATCH", true, true))
            {
                DialogResult result = MessageBox.Show(T._("Found already extracted files in work dir. Continue with them?"), T._("Found extracted files"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    //чистка и пересоздание папки
                    RPGMTransOther.CleanInvalidRPGMakerTransPatchFolders(outdir);
                    Directory.CreateDirectory(outdir);

                    //ret = CreateRPGMakerTransPatch(dir.FullName, outdir);

                }
            }

            return RPGMTransOther.CreateRPGMakerTransPatch(dir.FullName, outdir);
        }
    }
}
