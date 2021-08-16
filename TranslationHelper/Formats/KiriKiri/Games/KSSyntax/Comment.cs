using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Comment : KsSyntaxBase
    {
        public Comment()
        {
        }

        internal override string StartsWith => @"^\t*;";

        internal override string EndsWith => @"\n";
    }
}
