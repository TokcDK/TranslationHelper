using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVjson : ProjectBase
    {
        public RPGMMVjson(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            if (Path.GetExtension(thDataWork.SPath).ToUpperInvariant() == ".JSON")
            {
                if (Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath)) == "data")
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
                if (ParseRPGMakerMVjson(thDataWork.SPath, Write))
                {
                    return true;
                }
            }
            catch
            {
            }

            thDataWork.Main.ProgressInfo(false);
            return false;
        }

        private bool ParseRPGMakerMVjson(string FilePath, bool Write = false)
        {
            try
            {
                string Jsonname = Path.GetFileNameWithoutExtension(FilePath); // get json file name

                thDataWork.Main.ProgressInfo(true, ParseFileMessage + Jsonname + ".json");

                //string jsondata = File.ReadAllText(FilePath); // get json data

                bool ret = true;

                thDataWork.FilePath = FilePath;
                //ret = ReadJson(Jsonname, sPath);

                ret = Write ? new JSON(thDataWork).Save() : new JSON(thDataWork).Open();

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
