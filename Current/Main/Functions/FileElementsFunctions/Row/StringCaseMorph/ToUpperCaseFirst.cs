using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpperCaseFirst : StringCaseMorphBase
    {
        protected override VariantCase Variant => VariantCase.Upper;
    }
    class ToLowerCaseFirstAllLines : StringCaseMorphBase
    {
        protected override VariantCase Variant => VariantCase.UpperAllLines;
    }
}
