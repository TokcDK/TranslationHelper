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
                .Replace("\"", string.Empty)
                .Replace("\u200B", string.Empty)
                .Replace("\u318D", string.Empty)
                .Replace("'", string.Empty)
                .Replace("(", "（")
                .Replace(")", "）")
                .Replace("...", "……")                
                ;
        }

        internal static bool IsStringAContainsAnyFromArray(this string strA, string[] array)
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
