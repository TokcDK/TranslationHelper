using System.Text;
using TranslationHelper.Formats.TyranoBuilder.Extracted;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSOther//KSParserBase//
    {
        public KS()
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
