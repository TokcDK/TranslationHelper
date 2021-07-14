using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.NScript
{
    class NScriptGame : NScriptBase
    {
        public NScriptGame() : base()
        {
        }

        internal override bool Check()
        {
            return Path.GetExtension(ProjectData.SPath).ToUpperInvariant() == ".EXE"
                && File.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "nscript.dat"));
        }

        internal override string Name()
        {
            return "NScript";
        }

        internal override bool Open()
        {
            return /*ExtractNScriptDAT() &&*/ OpenSaveNScript();
        }

        private bool OpenSaveNScript()
        {
            ProjectData.Main.ProgressInfo(true, (ProjectData.OpenFileMode ? T._("Opening") : T._("Saving")) + ": nscript.dat");
            ProjectData.FilePath = Path.Combine(ProjectData.SelectedGameDir, "nscript.dat");
            bool ret;
            if (ProjectData.OpenFileMode)
            {
                if (!File.Exists(ProjectData.FilePath + ".orig"))//backup for manual restore
                {
                    File.Copy(ProjectData.FilePath, ProjectData.FilePath + ".orig");
                }

                ret = new Formats.NScriptGame.nscript.dat.NSCRIPT().Open();
            }
            else
            {
                ret = new Formats.NScriptGame.nscript.dat.NSCRIPT().Save();
            }

            ProjectData.Main.ProgressInfo(false);
            return ret;
        }

        private static bool ExtractNScriptDAT()
        {
            var ret = false;

            try
            {
                var nscriptdat = Path.Combine(ProjectData.SelectedDir, "nscript.dat");
                //var nsdecingame = Path.Combine(ProjectData.SelectedDir, THSettingsData.NSDECexeName());
                //if (!File.Exists(nsdecingame))
                //{
                //    File.Copy(THSettingsData.NSDECexePath(), nsdecingame);
                //}

                //var ssss = encryptDecrypt(File.ReadAllText(nscriptdat, Encoding.GetEncoding(932)));
                //var ssss = EncryptOrDecrypt(File.ReadAllText(nscriptdat, Encoding.GetEncoding(932)),"84");

                var targetnscripttxt = Path.Combine(ProjectData.ProjectWorkDir, "nscript.txt");

                using (var s = new FileStream(nscriptdat, FileMode.Open, FileAccess.Read))
                using (var br = new BinaryReader(s, Encoding.GetEncoding(932)))
                {
                    //var input = System.Console.ReadLine();
                    //var inputBytes = Encoding.Unicode.GetBytes(input);

                    var inputKey = "84";
                    var key = Encoding.GetEncoding(932).GetBytes(inputKey);
                    key = new byte[] { 0x84 };

                    //byte[] key = { 0, 0 }; if key is 0, encryption will not happen

                    //byte[] encryptedBytes = EncryptOrDecrypt(inputBytes, key);
                    //string encryptedStr = Encoding.Unicode.GetString(encryptedBytes);

                    //byte[] decryptedBytes = EncryptOrDecrypt(encryptedBytes, key);
                    //string decryptedStr = Encoding.Unicode.GetString(decryptedBytes);

                    //var ovrl = Encoding.GetEncoding(932).GetString(XorUnxor(br.ReadBytes((int)s.Length))).Replace("\n", Environment.NewLine);

                    byte[] decryptedBytes = br.ReadBytes((int)s.Length).XorUnxor(key);

                    string decryptedStr = Encoding.GetEncoding(932).GetString(decryptedBytes).Replace("\n", Environment.NewLine);

                    Directory.CreateDirectory(ProjectData.ProjectWorkDir);
                    File.WriteAllText(targetnscripttxt, decryptedStr, Encoding.GetEncoding(932));
                    ret = true;
                }

                //var nscripttxt = Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "nscript.txt");
                //ProjectData.ProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjecFolderName(), Path.GetFileName(Path.GetDirectoryName(ProjectData.SPath)));
                //if (!File.Exists(nscripttxt) && !(ret=File.Exists(targetnscripttxt)))
                //{
                //    var exe = THSettingsData.ArcConvExePath();
                //    var args = "--mod xor 84 \"" + nscriptdat + "\" \"" + nscripttxt + "\"";
                //    FunctionsProcess.RunProcess(exe, args);
                //    args = "--mod texteol 2 \"" + nscripttxt + "\"";
                //    FunctionsProcess.RunProcess(exe, args);
                //}
                //if (File.Exists(nscripttxt) && !File.Exists(targetnscripttxt))
                //{
                //    ret = true;
                //    //var projectdir = ProjectData.ProjectWorkDir;
                //    Directory.CreateDirectory(ProjectData.ProjectWorkDir);

                //    File.Move(nscripttxt, targetnscripttxt);
                //}
            }
            catch
            {

            }

            return ret;
        }

        internal override bool Save()
        {
            return OpenSaveNScript();
        }

        internal override bool BakCreate()
        {
            return BackupFile(Path.Combine(ProjectData.SelectedGameDir, "nscript.dat"));
        }

        internal override bool BakRestore()
        {
            return RestoreFile(Path.Combine(ProjectData.SelectedGameDir, "nscript.dat"));
        }
    }
}
