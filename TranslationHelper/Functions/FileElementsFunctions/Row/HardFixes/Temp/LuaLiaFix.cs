using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes.Temp
{
    class LuaLiaFix : HardFixesTempBase
    {
        public LuaLiaFix(THDataWork thDataWork) : base(thDataWork)
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
