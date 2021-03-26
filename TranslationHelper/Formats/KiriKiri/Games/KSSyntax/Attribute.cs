using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Attribute : KSSyntaxBase
    {
        public Attribute(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string StartsWith => @"[A-Zaz]+ \= \""([^\""]*)\""";

        internal override string EndsWith => null;
    }
}
