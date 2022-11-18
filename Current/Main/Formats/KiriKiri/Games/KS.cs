using System.Text;
using TranslationHelper.Formats.TyranoBuilder.Extracted;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSParserBase//KSOther//
    {
        public KS()
        {
        }

        public override string Extension => ".ks";

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
