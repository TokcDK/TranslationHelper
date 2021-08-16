using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXT : RPGMTransPatchBase
    {
        public TXT()
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        internal override bool ExtIdentifier()
        {
            var patchfile = Path.GetFullPath(Path.GetDirectoryName(ProjectData.SelectedFilePath) + @"\..\RPGMKTRANSPATCH");
            return File.Exists(patchfile) && Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath)) == "patch";
        }

        internal override string Name()
        {
            return "RPGMTrans patch txt";
        }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            return CheckAndParse();
        }
    }
}
