using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class SetOrigToTransIfSoundsText : RowBase
    {
        public SetOrigToTransIfSoundsText() : base()
        {
        }

        protected override bool Apply()
        {
            if ((SelectedRow[0] + "").IsSoundsText())
            {
                SelectedRow[1] = SelectedRow[0];

                return true;
            }

            return false;
        }
    }
}
