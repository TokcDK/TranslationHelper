using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXT : RPGMTransPatchBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        internal override bool ExtIdentifier()
        {
            var patchfile = Path.GetFullPath(Path.GetDirectoryName(thDataWork.SPath) + @"\..\RPGMKTRANSPATCH");
            return File.Exists(patchfile) && Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath)) == "patch";
        }

        internal override string Name()
        {
            return "RPGMTrans patch txt";
        }

        protected override int ParseStringFileLine()
        {
            return CheckAndParse();
        }
    }
}
