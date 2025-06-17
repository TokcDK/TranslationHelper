using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats
{
    internal interface IFormatMetadata
    {
        string Description { get; }
        string Extension { get; }
    }
    internal interface IFormatParser
    {
        bool Open(string filePath);
        bool Save(string jsFileInfo);
    }

    internal interface IFormat : IFormatMetadata, IFormatParser
    {
    }
}
