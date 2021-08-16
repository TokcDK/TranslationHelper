using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Tag1 : KsSyntaxBase
    {
        public Tag1()
        {
        }

        internal override string StartsWith => @"^\t*@\s*\w+";

        internal override string EndsWith => @"(?<!\\)\n";

        internal override List<KsSyntaxBase> Include()
        {
            return new List<KsSyntaxBase>
            {
                new Attribute()
            };
        }
    }
}
