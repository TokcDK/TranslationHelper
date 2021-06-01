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

        protected override void ParseStringFilePreOpenExtra()
        {
            unused = false;
        }

        protected override int ParseStringFileLine()
        {
            //skip if not patch files
            if (!CheckSetPatchVersion())
            {
                return -1;
            }

            //skip if begin string not found
            if (IsBeginString())
            {
                ParseBeginEndBlock();
            }
            else
            {
                SaveModeAddLine();
            }

            return 0;
        }
    }
}
