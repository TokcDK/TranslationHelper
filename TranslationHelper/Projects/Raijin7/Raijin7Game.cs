using System;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.Raijin7;
using TranslationHelper.Formats.Raijin7.eve;

namespace TranslationHelper.Projects
{
    class Raijin7Game : ProjectBase
    {
        public Raijin7Game(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool OpenDetect()
        {
            string dirPath = Path.GetDirectoryName(thDataWork.SPath);
            if (!string.IsNullOrEmpty(thDataWork.SPath) && Path.GetExtension(thDataWork.SPath) == ".exe" && Directory.Exists(Path.Combine(dirPath, "eve")) && Directory.Exists(Path.Combine(dirPath, "csv")))
            {
                return true;
            }

            return false;
        }

        internal override string ProjectTitle()
        {
            return "Raijin 7";
        }

        internal override bool Open()
        {
            string dirPath = Path.GetDirectoryName(thDataWork.SPath);

            string targetDir = Path.Combine(dirPath, "eve");
            if (Directory.Exists(targetDir))
            {
                foreach (var EventTxt in Directory.GetFiles(targetDir, "*.txt"))
                {
                    thDataWork.FilePath = EventTxt;
                    new TXT(thDataWork).Open();
                }
            }

            targetDir = Path.Combine(dirPath, "csv");
            if (Directory.Exists(targetDir))
            {
                foreach (var csv in Directory.GetFiles(targetDir, "*.csv"))
                {
                    thDataWork.FilePath = csv;
                    new CSV(thDataWork).Open();
                }
            }

            return thDataWork.THFilesElementsDataset.Tables.Count > 0;
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
