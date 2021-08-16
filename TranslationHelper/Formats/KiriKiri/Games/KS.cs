using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games
{
    class Ks : KsBase
    {
        public Ks()
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
