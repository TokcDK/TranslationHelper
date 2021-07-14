using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class AllHardFixes : HardFixesBase
    {
        public AllHardFixes() : base()
        {
            HardFixesList = new List<HardFixesBase>()
            {
                new ProjectSpecificFixes(),
                new FixENJPQuoteOnStringStart2ndLine(),
                new FixENJPQuoteOnStringStart1stLine(),
                new FixForRPGMAkerQuotationInSomeStrings(),
                new FixForRPGMAkerQuotationInSomeStrings2(),
                new FixForEndingQuoteInconsistence(),
                new FixBrokenNameVar(),
                new FixBrokenNameVar2(),
                //new RemoveIeroglifs()
                //new LuaLiaFix(),
            };
        }

        protected List<HardFixesBase> HardFixesList;

        protected override bool Apply()
        {
            var ret = false;

            foreach (var hardfix in HardFixesList)
            {
                if (SelectedRow != null ? hardfix.Selected(SelectedRow) : hardfix.Selected())
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}
