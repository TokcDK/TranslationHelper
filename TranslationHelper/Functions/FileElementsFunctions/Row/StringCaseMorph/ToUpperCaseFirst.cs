using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpperCaseFirst : StringCaseMorphBase
    {
        public ToUpperCaseFirst() : base()
        {
        }

        protected override VariantCase Variant => VariantCase.Upper;
    }
}
