using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes.Temp
{
    class LuaLiaFix : HardFixesTempBase
    {
        public LuaLiaFix()
        {
        }

        protected override bool Apply(RowData rowData)
        {
            bool lia;
            var original = Original;
            string translation;
            if (original.StartsWith("ルア") && ((lia = (translation = Translation).StartsWith("Lia")) || translation.StartsWith("Lila")))
            {
                Translation = "Lua" + translation.Remove(0, lia ? 3 : 4);
                return true;
            }

            return false; ;
        }
    }
}
