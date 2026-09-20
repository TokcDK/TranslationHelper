using System.Text;
using TranslationHelper.Formats.Abstractions;
using TranslationHelper.Formats.TyranoBuilder.Extracted;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class KS : KSParserBase//KSOther//
    {
        public KS(IFormatHost host) : base(host)
        {
        }

        public override string Extension => ".ks";

        protected override Encoding DefaultEncoding()
        {
            return Encoding.Unicode;
        }
    }
}
