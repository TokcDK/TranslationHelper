using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class CompleteRomajiotherLines : RowBase
    {
        public CompleteRomajiotherLines()
        {
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            var o = rowData.Original;
            var t = rowData.Translation;
            if ((string.IsNullOrEmpty(t) || !Equals(t, o)) && o.HaveMostOfRomajiOtherChars() || !o.HasLetters())
            {
                rowData.Translation = o;
                return true;
            }
            return false;
        }
    }
}
