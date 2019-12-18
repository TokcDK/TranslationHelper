using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats
{
    abstract class FormatBase
    {
        protected THDataWork thDataWork;

        protected FormatBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal abstract bool Open();

        internal abstract bool Save();
    }
}
