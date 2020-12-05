using System;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.AliceSoft;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.AliceSoft
{
    class DohnaDohna : AliceSoftBase
    {
        public DohnaDohna(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            string dirPath = Path.GetDirectoryName(thDataWork.SPath);
            return Path.GetExtension(thDataWork.SPath) == ".exe"
                && new DirectoryInfo(dirPath).HasAnyFiles("*.ain")
                ;
        }

        internal override string Name()
        {
            return "Dohna Dohna - Let's do bad things together";
        }

        internal override bool Open()
        {
            return PackUnpack() && OpenSaveFilesBase(Properties.Settings.Default.THProjectWorkDir, new AINTXT(thDataWork), "*.ain.txt");
        }

        private bool PackUnpack()
        {
            var alice = THSettingsData.AliceToolsExePath();

            Properties.Settings.Default.THProjectWorkDir = Path.Combine(THSettingsData.WorkDirPath(), Name());

            foreach (var ain in Directory.GetFiles(Path.GetDirectoryName(thDataWork.SPath), "*.ain"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.THProjectWorkDir);

                var args = "  ain dump -t -o \"" + Path.Combine(Properties.Settings.Default.THProjectWorkDir, Path.GetFileName(ain) + ".txt") + "\" \"" + ain + "\"";

                FunctionsProcess.RunProcess(alice, args);
            }

            return new DirectoryInfo(Properties.Settings.Default.THProjectWorkDir).HasAnyFiles("*.txt");
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
