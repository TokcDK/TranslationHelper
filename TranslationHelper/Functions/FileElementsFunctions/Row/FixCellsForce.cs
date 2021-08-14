using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class FixCellsForce : FixCells
    {
        public FixCellsForce()
        {
        }

        protected override bool IsValidRow()
        {
            return true;
        }
    }
}
