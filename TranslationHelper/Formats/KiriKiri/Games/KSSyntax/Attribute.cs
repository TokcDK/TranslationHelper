using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Attribute : KSSyntaxBase
    {
        public Attribute() : base()
        {
        }

        internal override string StartsWith => @"[A-Zaz]+ \= \""([^\""]*)\""";

        internal override string EndsWith => null;
    }
}
