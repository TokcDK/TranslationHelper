using System.Text;
using TranslationHelper.Formats.TyranoBuilder.Extracted;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSParserBase//KSOther//
    {
        public KS()
        {
        }

        internal override string Ext => ".ks";

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
