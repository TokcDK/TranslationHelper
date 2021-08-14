using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpperCaseAll : StringCaseMorphBase
    {
        public ToUpperCaseAll()
        {
        }

        protected override VariantCase Variant => VariantCase.UPPER;
    }
}
