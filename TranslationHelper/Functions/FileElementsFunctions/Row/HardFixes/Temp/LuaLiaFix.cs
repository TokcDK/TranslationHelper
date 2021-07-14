using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes.Temp
{
    class LuaLiaFix : HardFixesTempBase
    {
        public LuaLiaFix(ProjectData projectData) : base(projectData)
        {
        }

        protected override bool Apply()
        {
            bool Lia;
            var original = SelectedRow[ColumnIndexOriginal] as string;
            string translation;
            if (original.StartsWith("ルア") && ((Lia = (translation = SelectedRow[ColumnIndexTranslation] + "").StartsWith("Lia")) || translation.StartsWith("Lila")))
            {
                SelectedRow[ColumnIndexTranslation] = "Lua" + translation.Remove(0, Lia ? 3 : 4);
                return true;
            }

            return false; ;
        }
    }
}
