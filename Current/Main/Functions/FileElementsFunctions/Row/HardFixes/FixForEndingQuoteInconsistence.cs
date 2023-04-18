using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForEndingQuoteInconsistence : HardFixesBase
    {
        public FixForEndingQuoteInconsistence()
        {
        }

        protected override bool Apply()
        {
            var translation = Translation;
            var original = Original;
            if (translation.Length > 1 && translation[translation.Length - 1] == '"' && original.Length > 0 && original[original.Length - 1] != '"')
            {
                Translation = translation.Remove(translation.Length - 1, 1) + original[original.Length - 1];
                return true;
            }
            return false;
        }
    }
}
