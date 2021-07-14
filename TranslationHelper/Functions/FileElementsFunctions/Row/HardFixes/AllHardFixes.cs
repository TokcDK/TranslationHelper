using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class AllHardFixes : HardFixesBase
    {
        public AllHardFixes(ProjectData projectData) : base(projectData)
        {
            HardFixesList = new List<HardFixesBase>()
            {
                new ProjectSpecificFixes(projectData),
                new FixENJPQuoteOnStringStart2ndLine(projectData),
                new FixENJPQuoteOnStringStart1stLine(projectData),
                new FixForRPGMAkerQuotationInSomeStrings(projectData),
                new FixForRPGMAkerQuotationInSomeStrings2(projectData),
                new FixForEndingQuoteInconsistence(projectData),
                new FixBrokenNameVar(projectData),
                new FixBrokenNameVar2(projectData),
                //new RemoveIeroglifs(projectData)
                //new LuaLiaFix(projectData),
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
