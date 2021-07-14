using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Comment : KSSyntaxBase
    {
        public Comment() : base()
        {
        }

        internal override string StartsWith => @"^\t*;";

        internal override string EndsWith => @"\n";
    }
}
