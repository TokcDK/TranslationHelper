using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUPPERAll : StringCaseMorphBase
    {
        public ToUPPERAll(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            var translation = SelectedRow[ColumnIndexTranslation] + "";

            if (translation.Length == 0 || Equals(SelectedRow[ColumnIndexOriginal], SelectedRow[ColumnIndexTranslation]))
                return false;

            try
            {
                SelectedRow[ColumnIndexTranslation] = translation.ToUpperInvariant();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
