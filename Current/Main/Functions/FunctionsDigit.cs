﻿namespace TranslationHelper.Main.Functions
{
    static class FunctionsDigit
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
