using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.AliceSoft
{
    abstract class AliceSoftBase : FormatStringBase
    {
        protected AliceSoftBase(ProjectBase parentProject) : base(parentProject)
        {
        }
    }
}
