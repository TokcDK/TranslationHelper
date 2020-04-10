using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            OpenFilesSerial();
            return thDataWork.THFilesElementsDataset.Tables.Count > 0;
        }

        /// <summary>
        /// IsOpen=true = Open, else Save
        /// </summary>
        /// <param name="IsOpen"></param>
        private void OpenFilesSerial(bool IsOpen = true, string openPath="")
        {
            if (openPath.Length == 0)
            {
                openPath = Path.Combine(Path.GetDirectoryName(thDataWork.SPath), "data");
            }

            var txtFormat = new Formats.HowToMakeTrueSlavesRiseofaDarkEmpire.TXT(thDataWork);
            foreach (string txt in Directory.EnumerateFiles(openPath, "*.txt", SearchOption.AllDirectories))
            {
                thDataWork.FilePath = txt;
                thDataWork.Main.ProgressInfo(true, Path.GetFileName(txt));

                if (IsOpen)
                {
                    txtFormat.Open();
                }
                else
                {
                    txtFormat.Save();
                }
            }

            thDataWork.Main.ProgressInfo(false, string.Empty);
        }

        internal override bool Save()
        {
            return SaveFiles();
        }

        private bool SaveFiles()
        {
            OpenFilesSerial(false);
            return true;
        }
    }
}
