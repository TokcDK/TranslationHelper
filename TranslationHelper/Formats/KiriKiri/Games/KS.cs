using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSBase
    {
        public KS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".ks";
        }

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
