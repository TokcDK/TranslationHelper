using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpper1st : StringCaseMorphBase
    {
        public ToUpper1st(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";

            if (translation.Length == 0 || Equals(SelectedRow[ColumnIndexOriginal], SelectedRow[ColumnIndexTranslation]))
                return false;

            var newtranslation = FunctionsString.StringToUpper(translation);
            if (newtranslation != translation)
            {
                SelectedRow[ColumnIndexTranslation] = newtranslation;
                return true;
            }
            return false;
        }
    }
}
