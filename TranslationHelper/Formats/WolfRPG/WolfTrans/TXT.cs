using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.WolfRPG.WolfTrans
{
    class TXT : WolfRPGBase
    {
        public TXT()
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
            ProjectData.SaveFileMode = true;
            return ParseStringFile();
        }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            return CheckAndParse();
        }

        protected override bool WriteFileData(string filePath = "")
        {
            try
            {
                if (ParseData.Ret && ProjectData.SaveFileMode && ParseData.ResultForWrite.Length > 0)
                {
                    File.WriteAllText(filePath.Length > 0 ? filePath : ProjectData.FilePath, ParseData.ResultForWrite.ToString().Replace(Properties.Settings.Default.NewLine, "\n"), FunctionsFileFolder.GetEncoding(ProjectData.FilePath));
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
