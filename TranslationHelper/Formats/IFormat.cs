using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats
{
    internal interface IFormat
    {
        string Name { get; }
        string Ext { get; }
        bool Open();
        bool Save();
    }
}
