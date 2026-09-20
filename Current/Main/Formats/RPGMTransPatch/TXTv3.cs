using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class TXTv3 : PatchTXTBase
    {
        public TXTv3(IFormatHost host) : base(host)
        {
        }

        public override string Description => "RPGMTrans patch txt";

        protected override string PatchFileID()
        {
            return "> RPGMAKER TRANS PATCH FILE VERSION 3.2";
        }
    }
}
