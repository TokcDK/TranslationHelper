using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.AliceSoft
{
    abstract class AliceSoftBase : FormatBase
    {
        protected AliceSoftBase(ProjectData projectData) : base(projectData)
        {
        }
    }
}
