using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CompleteRomajiotherLines : RowBase
    {
        public CompleteRomajiotherLines()
        {
        }

        protected override bool Apply(RowData rowData)
        {
            var o = Original;
            var t = Translation;
            if ((string.IsNullOrEmpty(t) || !Equals(t, o)) && o.HaveMostOfRomajiOtherChars() || !o.HasLetters())
            {
                Translation = o;
                return true;
            }
            return false;
        }
    }
}
