using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.ExtractedParser
{
    internal abstract class ExtractedByTranslationRulesParserBase:RowBase
    {
        protected override bool Apply()
        {
            return true;
        }
    }
}
