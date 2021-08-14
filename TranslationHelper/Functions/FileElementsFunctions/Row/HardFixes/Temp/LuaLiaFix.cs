using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes.Temp
{
    class LuaLiaFix : HardFixesTempBase
    {
        public LuaLiaFix()
        {
        }

        protected override bool Apply()
        {
            bool lia;
            var original = SelectedRow[ColumnIndexOriginal] as string;
            string translation;
            if (original.StartsWith("ルア") && ((lia = (translation = SelectedRow[ColumnIndexTranslation] + "").StartsWith("Lia")) || translation.StartsWith("Lila")))
            {
                SelectedRow[ColumnIndexTranslation] = "Lua" + translation.Remove(0, lia ? 3 : 4);
                return true;
            }

            return false; ;
        }
    }
}
