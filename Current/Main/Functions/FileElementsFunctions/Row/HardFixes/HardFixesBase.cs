using System;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JS;
using TranslationHelper.Functions.StringChangers;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    abstract class HardFixesBase : RowBase
    {
        protected HardFixesBase()
        {
        }

        protected abstract StringChangerBase Changer { get; }

        protected override bool Apply(RowBaseRowData rowData)
        {
            var str = Changer.Change(rowData.Translation, rowData.Original);

            if (!string.Equals(str, rowData.Translation))
            {
                rowData.Translation = str;
                return true;
            }

            return false;
        }
    }
}
