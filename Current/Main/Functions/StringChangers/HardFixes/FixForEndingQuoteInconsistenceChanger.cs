using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class FixForEndingQuoteInconsistenceChanger : StringChangerBase
    {
        public FixForEndingQuoteInconsistenceChanger()
        {
        }
        internal override string Description => $"{nameof(FixForEndingQuoteInconsistenceChanger)}";


        internal override string Change(string inputString, object extraData)
        {
            var translation = inputString;
            var original = extraData as string;
            if (translation.Length > 1 && translation[translation.Length - 1] == '"' && original.Length > 0 && original[original.Length - 1] != '"')
            {
                return translation.Remove(translation.Length - 1, 1) + original[original.Length - 1];

            }
            return inputString;
        }
    }
}
