﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats.RPGMakerVXAce
{
    internal class RGSS3A : FormatBinaryBase
    {
        public override string Extension => ".rgss3a";

        protected override void ParseBytes()
        {
            var parser = new RGSS_Extractor.RGSS3AParser(ParseData.BReader);

            parser.ParseFile();

            //foreach(var entry in parser.entries)
            //{ 

            //}
        }
    }
}
