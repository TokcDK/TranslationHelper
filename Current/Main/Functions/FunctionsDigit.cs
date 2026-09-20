using System;

namespace TranslationHelper.Main.Functions
{
    static class FunctionsDigit
    {
        public static bool IsEqualsAnyNumberFromArray(int lineNum, int[] lineNumbers)
        {
            return Array.IndexOf(lineNumbers, lineNum) != -1;
        }
    }
}
