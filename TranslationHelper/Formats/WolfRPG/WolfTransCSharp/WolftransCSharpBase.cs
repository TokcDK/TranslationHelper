﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats.WolfRPG.WolfTransCSharp
{
    internal abstract class WolftransCSharpBase:FormatBinaryBase
    {
        internal override bool IsValidString(string inputString)
        {
            return base.IsValidString(inputString) 
                && inputString != "\u25A0";// from wolftrans ruby
        }
    }
}
