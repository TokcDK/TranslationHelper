using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;

namespace TranslationHelper.Projects.RPGMMV
{
    class RpgmmVjson : ProjectBase
    {
        public RpgmmVjson()
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

        string _parseFileMessage;
        /// <summary>
        /// Parsing the Project files
        /// </summary>
        /// <param name="write">Use Save() instead of Open()</param>
        /// <returns></returns>
        private bool ParseProjectFiles(bool write = false)
        {
            if (!write)
            {
                BakRestore();
            }

            _parseFileMessage = write ? T._("write file: ") : T._("opening file: ");
            try
            {
                if (ParseRpgMakerMVjson(ProjectData.SelectedFilePath, write))
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

        private bool ParseRpgMakerMVjson(string filePath, bool write = false)
        {
            try
            {
                string jsonname = Path.GetFileNameWithoutExtension(filePath); // get json file name

                ProjectData.Main.ProgressInfo(true, _parseFileMessage + jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                ProjectData.FilePath = filePath;
                //ret = ReadJson(Jsonname, sPath);

                ret = write ? new Json().Save() : new Json().Open();

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
