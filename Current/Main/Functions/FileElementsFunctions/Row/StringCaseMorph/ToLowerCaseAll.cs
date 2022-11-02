using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToLowerCaseAll : StringCaseMorphBase
    {
        public ToLowerCaseAll()
        {
        }

        protected override VariantCase Variant => VariantCase.lower;
    }
}
