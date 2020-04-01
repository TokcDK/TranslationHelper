using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.HowToMakeTrueSlavesRiseofaDarkEmpire
{
    class HowToMakeTrueSlavesRiseofaDarkEmpire : ProjectBase
    {
        public HowToMakeTrueSlavesRiseofaDarkEmpire(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string ProjectTitle()
        {
            return "How To Make True Slaves -Rise of a Dark Empire-";
        }

        internal override bool OpenDetect()
        {
            return Path.GetExtension(thDataWork.SPath) == ".exe"
                &&
                Path.GetFileNameWithoutExtension(thDataWork.SPath) == "正しい性奴隷の使い方"
                &&
                Directory.Exists(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "data", "Script"));
        }

        internal override bool Open()
        {
            return OpenFiles();
        }

        private bool OpenFiles()
        {
            foreach(string txt in Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "data"), "*.txt", SearchOption.AllDirectories))
            {
                thDataWork.FilePath = txt;
                thDataWork.Main.ProgressInfo(true, Path.GetFileName(txt));
                new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.TXT(thDataWork).Open();
            }

            thDataWork.Main.ProgressInfo(false, string.Empty);
            return thDataWork.THFilesElementsDataset.Tables.Count > 0;
        }

        internal override bool Save()
        {
            return SaveFiles();
        }

        private bool SaveFiles()
        {
            foreach (string txt in Directory.EnumerateFiles(Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "data"), "*.txt", SearchOption.AllDirectories))
            {
                thDataWork.FilePath = txt;
                thDataWork.Main.ProgressInfo(true, Path.GetFileName(txt));
                new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.TXT(thDataWork).Save();
            }

            thDataWork.Main.ProgressInfo(false, string.Empty);
            return true;
        }
    }
}
