using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Main.Functions
{
    class FunctionsDigit
    {
        public static bool IsEqualsAnyNumberFromArray(int lineNum, int[] lineNumbers)
        {
            foreach (var num in lineNumbers)
            {
                if (lineNum == num)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
