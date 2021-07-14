using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSBase
    {
        public KS() : base()
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
