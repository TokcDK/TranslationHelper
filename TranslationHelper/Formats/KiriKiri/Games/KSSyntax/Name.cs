using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Name : KsSyntaxBase
    {
        public Name()
        {
        }

        internal override string StartsWith => @"^\t*【";

        internal override string EndsWith => @"】";
    }
}
