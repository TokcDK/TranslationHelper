using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVjson : ProjectBase
    {
        public RPGMMVjson()
        {
        }

        internal override bool Check()
        {
            if (Path.GetExtension(AppData.SelectedFilePath).ToUpperInvariant() == ".JSON")
            {
                if (Path.GetFileName(Path.GetDirectoryName(AppData.SelectedFilePath)) == "data")
                {
                    return true;
                }
            }

            return false;
        }

        internal override string Name => "RPG Maker MV Json";

        internal override string ProjectFolderName => "RPGMakerMV";

        internal override bool Open()
        {
            return ParseProjectFiles();
        }

        string ParseFileMessage;
        /// <summary>
        /// Parsing the Project files
        /// </summary>
        /// <param name="Write">Use Save() instead of Open()</param>
        /// <returns></returns>
        private bool ParseProjectFiles(bool Write = false)
        {
            if (!Write)
            {
                BakRestore();
            }

            ParseFileMessage = Write ? T._("write file: ") : T._("opening file: ");
            try
            {
                if (ParseRPGMakerMVjson(AppData.SelectedFilePath, Write))
                {
                    return true;
                }
            }
            catch
            {
            }

            AppData.Main.ProgressInfo(false);
            return false;
        }

        private bool ParseRPGMakerMVjson(string filePath, bool Write = false)
        {
            try
            {
                string Jsonname = Path.GetFileNameWithoutExtension(filePath); // get json file name

                AppData.Main.ProgressInfo(true, ParseFileMessage + Jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                //ret = ReadJson(Jsonname, sPath);
                var format = new JSON
                {
                    FilePath = filePath
                };

                ret = Write ? format.Save() : format.Open();

                return ret;
            }
            catch
            {
                return false;
            }
        }

        internal override bool Save()
        {
            return ParseProjectFiles(true);
        }

        internal override bool BakCreate()
        {
            return false;
        }

        internal override bool BakRestore()
        {
            return false;
        }
    }
}
