using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.AliceSoft
{
    abstract class AliceSoftBase : ProjectBase
    {
        protected AliceSoftBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string ProjectTitlePrefix()
        {
            return "[AliceSoft]";
        }

        internal override string ProjectFolderName()
        {
            return "AliceSoft";
        }
    }
}
