namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpperCaseFirst : StringCaseMorphBase
    {
        protected override CaseMorphVariant Variant => CaseMorphVariant.Upper;
    }

    class ToLowerCaseFirstAllLines : StringCaseMorphBase
    {
        protected override CaseMorphVariant Variant => CaseMorphVariant.UpperAllLines;
    }
}
