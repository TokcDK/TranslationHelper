using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    abstract class KiriKiriBase : FormatBase
    {
        /// <summary>
        /// overall kirikiri environment for all formats
        /// </summary>
        /// <param name="thDataWork"></param>
        protected KiriKiriBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseStringFile();
        }

        internal override bool Save()
        {
            return ParseStringFile();
        }

        protected Encoding encoding = Encoding.Unicode;
        /// <summary>
        /// encoding for read write file
        /// </summary>
        /// <returns></returns>
        internal virtual Encoding FileEncoding()
        {
            //using (var fs = new FileStream(thDataWork.FilePath, FileMode.Open, FileAccess.Read))
            //    return FunctionsFileFolder.GetEncoding(fs);
            return FunctionsFileFolder.GetEncoding(thDataWork.FilePath);
            //return FunctionsFileFolder.GetEncoding(thDataWork.FilePath);
            //if (Enc == null && !string.IsNullOrEmpty(thDataWork.FilePath))
            //{
            //    Enc = FunctionsFileFolder.GetEncoding(thDataWork.FilePath);
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
        protected override void ParseStringFilePreOpenExtra()
        {
            ParseData.IsComment = false;
        }

        protected const string PatchDirName = "_patch";

        protected override string GetFilePath()
        {
            //write translated files to patch dir
            return Path.Combine(Properties.Settings.Default.THProjectWorkDir, PatchDirName, Path.GetFileName(thDataWork.FilePath));
        }
        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
