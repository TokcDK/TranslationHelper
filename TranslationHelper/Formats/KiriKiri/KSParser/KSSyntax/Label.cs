using TranslationHelper.Formats.KiriKiri.KSParser.KSSyntax;

namespace TranslationHelper.Formats.KiriKiri.KSParser.KSSyntax
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
