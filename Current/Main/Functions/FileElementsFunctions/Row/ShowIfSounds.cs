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
        public SetOrigToTransIfSoundsText()
        {
        }

        protected override bool Apply(RowData rowData)
        {
            var o = Original;
            if (o.IsSoundsText())
            {
                Translation = o;

                return true;
            }

            return false;
        }
    }
}
