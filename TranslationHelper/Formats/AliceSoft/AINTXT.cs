using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.AliceSoft
{
    class AINTXT : AliceSoftBase
    {
        public AINTXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        string lastgroupname = "";
        string ainstringpattern = @"^;[sm]\[[0-9]{1,10}\] \= \""(.+)\""$";
        protected override int ParseStringFileLine()
        {
            var m = Regex.Match(ParseData.line, ainstringpattern);
            if (m.Success)
            {
                var str = m.Result("$1");

                if (IsValidString(str))
                {
                    AddRowData(str, T._("Last group")+": "+ lastgroupname, true, false);
                }
            }
            else
            {
                lastgroupname = ParseData.line;
            }

            return 0;
        }
    }
}
