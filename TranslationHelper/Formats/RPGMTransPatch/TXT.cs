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
            return File.Exists(Path.GetFullPath(Path.GetDirectoryName(thDataWork.SPath) + @"..\RPGMKTRANSPATCH")) && Path.GetFileName(Path.GetDirectoryName(thDataWork.SPath)) == "patch";
        }

        protected override int ParseStringFileLine()
        {
            return CheckAndParse();
        }
    }
}
