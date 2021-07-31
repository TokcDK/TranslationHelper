using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpperCaseAll : StringCaseMorphBase
    {
        public ToUpperCaseAll() : base()
        {
        }

        protected override VariantCase Variant => VariantCase.UPPER;
    }
}
