using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    abstract class KiriKiriBase : FormatStringBase
    {
        /// <summary>
        /// overall kirikiri environment for all formats
        /// </summary>
        /// <param name="projectData"></param>
        protected KiriKiriBase()
        {
        }

        internal override bool Open()
        {
            return ParseFile();
        }

        internal override bool Save()
        {
            return ParseFile();
        }

        protected Encoding encoding = Encoding.Unicode;
        /// <summary>
        /// encoding for read write file
        /// </summary>
        /// <returns></returns>
        internal virtual Encoding FileEncoding()
        {
            //using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            //    return FunctionsFileFolder.GetEncoding(fs);
            return FunctionsFileFolder.GetEncoding(GetOpenFilePath());
            //return FunctionsFileFolder.GetEncoding(FilePath);
            //if (Enc == null && !string.IsNullOrEmpty(FilePath))
            //{
            //    Enc = FunctionsFileFolder.GetEncoding(FilePath);
            //}
            //else
            //{
            //    Enc = Encoding.Unicode;
            //    //new UTF8Encoding(false) //UTF8 with no BOM
            //}
            //return Encoding.Unicode;
            //return Enc;
            //return new UTF8Encoding(false);
        }
        protected override void PreOpenExtraActions()
        {
            ParseData.IsComment = false;
        }

        protected const string PatchDirName = "_patch";

        protected override string GetSaveFilePath()
        {
            //write translated files to patch dir
            return Path.Combine(ProjectData.ProjectWorkDir, PatchDirName, Path.GetFileName(FilePath));
        }
        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
