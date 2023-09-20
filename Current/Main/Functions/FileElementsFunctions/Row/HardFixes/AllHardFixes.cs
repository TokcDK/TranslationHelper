using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions.StringChangers;
using TranslationHelper.Functions.StringChangers.HardFixes;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class AllHardFixes : HardFixesBase
    {
        readonly StringChangerBase _changer = new AllHardFixesChanger();

        protected override bool Apply(RowBaseRowData rowData)
        {
            var str = _changer.Change(rowData.Translation, rowData.Original);

            if(!string.Equals(str, rowData.Translation))
            {
                rowData.Translation = str;
                return true;
            }

            return false;
        }
    }
}
