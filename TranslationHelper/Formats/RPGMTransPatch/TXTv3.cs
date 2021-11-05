using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class TXTv3 : PatchTXTBase
    {
        public TXTv3()
        {
        }

        internal override int ExtIdentifier()
        {
            var patchfile = Path.GetFullPath(Path.GetDirectoryName(ProjectData.SelectedFilePath) + @"\..\RPGMKTRANSPATCH");
            return File.Exists(patchfile) && Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath)) == "patch" ? 1 : -1;
        }

        internal override string Name()
        {
            return "RPGMTrans patch txt";
        }

        protected override string PatchFileID()
        {
            return "> RPGMAKER TRANS PATCH FILE VERSION 3.2";
        }
    }
}
