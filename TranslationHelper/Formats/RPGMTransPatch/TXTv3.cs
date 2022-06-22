using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class TXTv3 : PatchTXTBase
    {
        public TXTv3()
        {
        }

        internal override int ExtIdentifier
        {
            get
            {
                var patchfile = Path.GetFullPath(Path.GetDirectoryName(AppData.SelectedFilePath) + @"\..\RPGMKTRANSPATCH");
                return File.Exists(patchfile) && Path.GetFileName(Path.GetDirectoryName(AppData.SelectedFilePath)) == "patch" ? 1 : -1;
            }
        }

        internal override string Name => "RPGMTrans patch txt";

        protected override string PatchFileID()
        {
            return "> RPGMAKER TRANS PATCH FILE VERSION 3.2";
        }
    }
}
