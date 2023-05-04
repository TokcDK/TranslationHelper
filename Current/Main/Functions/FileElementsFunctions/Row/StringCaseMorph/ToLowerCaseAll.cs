using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToLowerCaseAll : StringCaseMorphBase
    {
        protected override VariantCase Variant => VariantCase.lower;
    }
    class ToLowerCaseFirst : StringCaseMorphBase
    {
        protected override VariantCase Variant => VariantCase.lower1st;
    }
}
