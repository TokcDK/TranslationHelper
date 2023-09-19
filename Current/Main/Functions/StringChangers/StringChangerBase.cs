using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Functions.StringChangers
{
    internal abstract class StringChangerBase
    {
        internal abstract string Description { get; }

        internal abstract string Change(string inputString, object extraData);

    }
}
