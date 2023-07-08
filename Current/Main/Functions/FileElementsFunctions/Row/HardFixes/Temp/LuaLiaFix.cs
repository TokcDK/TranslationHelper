using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes.Temp
{
    class LuaLiaFix : HardFixesTempBase
    {
        public LuaLiaFix()
        {
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            bool lia;
            var original = rowData.Original;
            string translation;
            if (original.StartsWith("ルア") && ((lia = (translation = rowData.Translation).StartsWith("Lia")) || translation.StartsWith("Lila")))
            {
                rowData.Translation = "Lua" + translation.Remove(0, lia ? 3 : 4);
                return true;
            }

            return false; ;
        }
    }
}
