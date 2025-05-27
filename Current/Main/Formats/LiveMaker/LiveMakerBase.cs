using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.LiveMaker
{
    abstract class LiveMakerBase : FormatStringBase
    {
        protected LiveMakerBase(ProjectBase parentProject) : base(parentProject)
        {
        }
    }
}
