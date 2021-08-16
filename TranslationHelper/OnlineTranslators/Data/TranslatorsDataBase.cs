using System.Collections.Generic;

namespace TranslationHelper.OnlineTranslators.Data
{
    abstract class TranslatorsDataBase
    {
        internal abstract List<string> GetLanguages(List<string> _Languages);

        internal abstract string Weblink { get; }
    }
}
