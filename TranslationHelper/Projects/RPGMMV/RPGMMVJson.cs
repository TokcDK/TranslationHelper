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
            if (Path.GetExtension(ProjectData.SelectedFilePath).ToUpperInvariant() == ".JSON")
            {
                if (Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath)) == "data")
                {
                    return true;
                }
            }

            return false;
        }

        internal override string Name()
        {
            return "RPG Maker MV Json";
        }

        internal override string ProjectFolderName()
        {
            return "RPGMakerMV";
        }

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
                if (ParseRPGMakerMVjson(ProjectData.SelectedFilePath, Write))
                {
                    return true;
                }
            }
            catch
            {
            }

            ProjectData.Main.ProgressInfo(false);
            return false;
        }

        private bool ParseRPGMakerMVjson(string FilePath, bool Write = false)
        {
            try
            {
                string Jsonname = Path.GetFileNameWithoutExtension(FilePath); // get json file name

                ProjectData.Main.ProgressInfo(true, ParseFileMessage + Jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                ProjectData.FilePath = FilePath;
                //ret = ReadJson(Jsonname, sPath);

                ret = Write ? new JSON().Save() : new JSON().Open();

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
