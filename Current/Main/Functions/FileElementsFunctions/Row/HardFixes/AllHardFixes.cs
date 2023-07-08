using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class AllHardFixes : HardFixesBase
    {
        public AllHardFixes()
        {
            HardFixesList = new List<HardFixesBase>(8)
            {
                new ProjectSpecificFixes(),
                new FixEnjpQuoteOnStringStart2NdLine(),
                new FixEnjpQuoteOnStringStart1StLine(),
                new FixForRpgmAkerQuotationInSomeStrings(),
                new FixForRpgmAkerQuotationInSomeStrings2(),
                new FixForEndingQuoteInconsistence(),
                new FixBrokenNameVar(),
                new FixBrokenNameVar2(),
                //new RemoveIeroglifs()
                //new LuaLiaFix(),
            };
        }

        protected List<HardFixesBase> HardFixesList;

        protected override bool Apply(RowData rowData)
        {
            var ret = false;

            foreach (var hardfix in HardFixesList)
            {
                if (SelectedRow != null ? hardfix.Selected(SelectedRow) : hardfix.Rows())
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}
