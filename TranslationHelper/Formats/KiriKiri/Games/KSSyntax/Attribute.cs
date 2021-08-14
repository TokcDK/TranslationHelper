using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Attribute : KSSyntaxBase
    {
        public Attribute()
        {
        }

        internal override string StartsWith => @"[A-Zaz]+ \= \""([^\""]*)\""";

        internal override string EndsWith => null;
    }
}
