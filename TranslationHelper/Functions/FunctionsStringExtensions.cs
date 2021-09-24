namespace TranslationHelper.Functions
{
    internal static class FunctionsStringExtensions
    {
        /// <summary>
        /// removes symbols which cause UnicodeEncodeError error in python script while text conversion
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static string CleanForShiftJIS2004(this string s)
        {
            return s
                .Replace("\"", string.Empty)//crashing scpack game
                .Replace("\u200B", string.Empty)//crashing scpack game
                .Replace("\u318D", string.Empty)//crashing scpack game
                .Replace("'", string.Empty)//crashing scpack game
                .Replace("(", "（")//scpack script have same symbol for scripts
                .Replace(")", "）")//scpack script have same symbol for scripts
                .Replace("...", "…")//just for cosmetic reason?
                ;
        }

        internal static bool ContainsAnyFromArray(this string strA, string[] array)
        {
            foreach (var str in array)
            {
                if (FunctionsString.IsStringAContainsStringB(strA, str))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
