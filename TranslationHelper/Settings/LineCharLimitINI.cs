using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Settings
{
    class LineCharLimitINI : SettingsBase
    {
        public LineCharLimitINI(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override void Get()
        {
            Properties.Settings.Default.THOptionLineCharLimit = thDataWork.Main.Settings.THConfigINI.KeyExists("THOptionLineCharLimit", "General")
                    ? int.Parse(thDataWork.Main.Settings.THConfigINI.ReadINI("General", "LineCharLimit"), CultureInfo.GetCultureInfo("en-US"))
                    : 60;
        }

        internal override void Set()
        {
            throw new NotImplementedException();
        }
    }
}
