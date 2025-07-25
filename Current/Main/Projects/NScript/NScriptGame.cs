﻿using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.NScriptGame.nscript.dat;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects.NScript
{
    class NScriptGame : NScriptBase
    {
        [Obsolete]
        public NScriptGame()
        {
        }

        internal override bool IsValid()
        {
            return Path.GetExtension(AppData.SelectedProjectFilePath).ToUpperInvariant() == ".EXE"
                && File.Exists(Path.Combine(Path.GetDirectoryName(AppData.SelectedProjectFilePath), "nscript.dat"));
        }

        public override string Name => "NScript";

        protected override bool TryOpen()
        {
            return /*ExtractNScriptDAT() &&*/ OpenSaveNScript();
        }

        private bool OpenSaveNScript()
        {
            var filePath = Path.Combine(AppData.CurrentProject.SelectedGameDir, "nscript.dat");

            if (OpenFileMode)
            {
                if (!File.Exists(filePath + ".orig"))//backup for manual restore
                {
                    File.Copy(filePath, filePath + ".orig");
                }
            }
            
            return this.OpenSaveFilesBase(new FileInfo(filePath), typeof(NSCRIPT));
        }

        private static bool ExtractNScriptDAT()
        {
            var ret = false;

            try
            {
                var nscriptdat = Path.Combine(AppData.CurrentProject.SelectedDir, "nscript.dat");
                //var nsdecingame = Path.Combine(ProjectData.CurrentProject.SelectedDir, THSettingsData.NSDECexeName());
                //if (!File.Exists(nsdecingame))
                //{
                //    File.Copy(THSettingsData.NSDECexePath(), nsdecingame);
                //}

                //var ssss = encryptDecrypt(File.ReadAllText(nscriptdat, Encoding.GetEncoding(932)));
                //var ssss = EncryptOrDecrypt(File.ReadAllText(nscriptdat, Encoding.GetEncoding(932)),"84");

                var targetnscripttxt = Path.Combine(AppData.CurrentProject.ProjectWorkDir, "nscript.txt");

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

                    Directory.CreateDirectory(AppData.CurrentProject.ProjectWorkDir);
                    File.WriteAllText(targetnscripttxt, decryptedStr, Encoding.GetEncoding(932));
                    ret = true;
                }

                //var nscripttxt = Path.Combine(Path.GetDirectoryName(ProjectData.SPath), "nscript.txt");
                //ProjectData.CurrentProject.ProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), ProjecFolderName(), Path.GetFileName(Path.GetDirectoryName(ProjectData.SPath)));
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
                //    //var projectdir = ProjectData.CurrentProject.ProjectWorkDir;
                //    Directory.CreateDirectory(ProjectData.CurrentProject.ProjectWorkDir);

                //    File.Move(nscripttxt, targetnscripttxt);
                //}
            }
            catch
            {

            }

            return ret;
        }

        protected override bool TrySave()
        {
            return OpenSaveNScript();
        }

        public override bool BakCreate()
        {
            return ProjectToolsBackup.BackupFile(Path.Combine(AppData.CurrentProject.SelectedGameDir, "nscript.dat"));
        }

        public override bool BakRestore()
        {
            return ProjectToolsBackup.RestoreFile(Path.Combine(AppData.CurrentProject.SelectedGameDir, "nscript.dat"));
        }
    }
}
