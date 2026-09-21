namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToLowerCaseAll : StringCaseMorphBase
    {
        protected override CaseMorphVariant Variant => CaseMorphVariant.lower;
    }

    class ToLowerCaseFirst : StringCaseMorphBase
    {
        protected override CaseMorphVariant Variant => CaseMorphVariant.lower1st;
    }
}
