﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats
{
    internal interface IFormat
    {
        string Description { get; }
        string Extension { get; }
        bool Open(string filePath);
        bool Save(string jsFileInfo);
    }
}
