﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
{
    class Label : KSSyntaxBase
    {
        public Label(ProjectData projectData) : base(projectData)
        {
        }

        internal override string StartsWith => @"^\*";

        internal override string EndsWith => @"\n";
    }
}
