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

        protected override int ParseStringFileLine()
        {
            return CheckAndParse();
        }
    }
}
