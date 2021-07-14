using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.WolfRPG.WolfTrans
{
    class TXT : WolfRPGBase
    {
        public TXT(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        internal override bool Save()
        {
            projectData.SaveFileMode = true;
            return ParseStringFile();
        }

        protected override int ParseStringFileLine()
        {
            return CheckAndParse();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && projectData.SaveFileMode && ParseData.ResultForWrite.Length > 0)
                {
                    File.WriteAllText(filePath.Length > 0 ? filePath : projectData.FilePath, ParseData.ResultForWrite.ToString().Replace(Properties.Settings.Default.NewLine, "\n"), FunctionsFileFolder.GetEncoding(projectData.FilePath));
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}
