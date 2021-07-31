using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToLowerCaseAll : StringCaseMorphBase
    {
        public ToLowerCaseAll() : base()
        {
        }

        protected override VariantCase Variant => VariantCase.Lower;
    }
}
