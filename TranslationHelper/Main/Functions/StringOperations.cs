using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Main.Functions
{
    static class StringOperations
    {
        /// <summary>
        /// Split string to lines
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitToLines(this string input)
        {
            //https://stackoverflow.com/a/23408020
            if (input == null)
            {
                yield break;
            }

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static bool IsMultiline(string input)
        {
            if (input != null)
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(input))
                {
                    int i = 0;
                    while (reader.ReadLine() != null)
                    {
                        i++;
                        if (i > 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
