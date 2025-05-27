using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV;
using TranslationHelper.Functions;

namespace TranslationHelper.Projects.RPGMMV
{
    class RPGMMVjson : ProjectBase
    {
        public RPGMMVjson()
        {
        }

        internal override bool IsValid()
        {
            if (Path.GetExtension(AppData.SelectedProjectFilePath).ToUpperInvariant() == ".JSON")
            {
                if (Path.GetFileName(Path.GetDirectoryName(AppData.SelectedProjectFilePath)) == "data")
                {
                    return true;
                }
            }

            return false;
        }

        public override string Name => "RPG Maker MV Json";

        internal override string ProjectDBFolderName => "RPGMakerMV";

        protected override bool TryOpen()
        {
            return ParseProjectFiles();
        }

        /// <summary>
        /// Parsing the Project files
        /// </summary>
        /// <param name="Write">Use Save() instead of Open()</param>
        /// <returns></returns>
        private bool ParseProjectFiles()
        {
            return this.OpenSaveFilesBase(new FileInfo(ProjectPath), typeof(JSON));
        }

        protected override bool TrySave()
        {
            return ParseProjectFiles();
        }

        public override bool BakCreate()
        {
            return false;
        }

        public override bool BakRestore()
        {
            return false;
        }
    }
}
