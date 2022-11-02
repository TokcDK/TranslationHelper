using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Label : KSSyntaxBase
    {
        public Label()
        {
        }

        internal override string StartsWith => @"^\*";

        internal override string EndsWith => "\n";
    }
}
