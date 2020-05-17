using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Settings
{
    internal abstract class SettingsBase
    {
        protected THDataWork thDataWork;

        public SettingsBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal abstract void Set();

        internal abstract void Get();
    }
}
