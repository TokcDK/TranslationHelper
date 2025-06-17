using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Projects.zzzOtherProject
{
    internal class DummyProject : ProjectBase
    {
        public override string Name => "";

        protected override bool TryOpen()
        {
            return false;
        }

        protected override bool TrySave()
        {
            return false;
        }

        internal override bool IsValid()
        {
            return false;
        }
    }
}
