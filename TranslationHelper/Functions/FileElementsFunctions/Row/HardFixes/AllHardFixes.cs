using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class AllHardFixes : HardFixesBase
    {
        public AllHardFixes(THDataWork thDataWork) : base(thDataWork)
        {
            HardFixesList = new List<HardFixesBase>()
            {
                new ProjectSpecificFixes(thDataWork),
                new FixENJPQuoteOnStringStart2ndLine(thDataWork),
                new FixENJPQuoteOnStringStart1stLine(thDataWork),
                new FixForRPGMAkerQuotationInSomeStrings(thDataWork),
                new FixForRPGMAkerQuotationInSomeStrings2(thDataWork),
                new FixForEndingQuoteInconsistence(thDataWork),
                new FixBrokenNameVar(thDataWork),
                new FixBrokenNameVar2(thDataWork),
                new RemoveIeroglifs(thDataWork)
                //new LuaLiaFix(thDataWork),
            };
        }

        protected List<HardFixesBase> HardFixesList;

        protected override bool Apply()
        {
            var ret = false;
            foreach (var hardfix in HardFixesList)
            {
                if (hardfix.Selected())
                {
                    ret= true;
                }
            }

            return ret;
        }
    }
}
