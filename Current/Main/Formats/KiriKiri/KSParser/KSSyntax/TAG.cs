﻿using System.Collections.Generic;
using TranslationHelper.Formats.KiriKiri.Games.KSSyntax;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class TAG : KSSyntaxBase
    {
        public TAG()
        {
        }

        internal override string StartsWith => @"(?<!\[)\[\s*\w+";

        internal override string EndsWith => @"\]";

        internal override List<KSSyntaxBase> Include()
        {
            return new List<KSSyntaxBase>
            {
                new Attribute()
            };
        }
    }
}
