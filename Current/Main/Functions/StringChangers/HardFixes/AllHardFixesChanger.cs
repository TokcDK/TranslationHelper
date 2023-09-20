using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.StringChangers.HardFixes
{
    class AllHardFixesChanger : StringChangerBase
    {
        public AllHardFixesChanger()
        {
            HardFixesList = new List<StringChangerBase>(8)
            {
                new ProjectSpecificFixesChanger(),
                new FixEnjpQuoteOnStringStart2NdLineChanger(),
                new FixEnjpQuoteOnStringStart1StLineChanger(),
                new FixForRpgmAkerQuotationInSomeStringsChanger(),
                new FixForRpgmAkerQuotationInSomeStrings2Changer(),
                new FixForEndingQuoteInconsistenceChanger(),
                new FixBrokenNameVarChanger(),
                new FixBrokenNameVar2Changer(),
            };
        }

        protected List<StringChangerBase> HardFixesList;

        internal override string Description => "All hard fixes in one";

        internal override string Change(string inputString, object extraData)
        {
            string str = inputString;
            foreach (var hardfix in HardFixesList)
            {
                str = hardfix.Change(str, extraData);
            }

            return str;
        }
    }
}
