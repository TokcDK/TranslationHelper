﻿using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class TAG : KSSyntaxBase
    {
        public TAG(ProjectData projectData) : base(projectData)
        {
        }

        internal override string StartsWith => @"(?<!\[)\[\s*\w+";

        internal override string EndsWith => @"\]";

        internal override List<KSSyntaxBase> Include()
        {
            return new List<KSSyntaxBase>
            {
                new Attribute(projectData)
            };
        }
    }
}
