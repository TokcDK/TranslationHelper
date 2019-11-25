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

        /// <summary>
        /// Check if string A contains string B (if length of A > length of A with replaced B by "")<br></br><br></br>
        /// Speed check tests: https://cc.davelozinski.com/c-sharp/fastest-way-to-check-if-a-string-occurs-within-a-string
        /// </summary>
        /// <param name="StringAWhereSearch"></param>
        /// <param name="StringBToSearch"></param>
        /// <returns></returns>
        public static bool IsStringAContainsStringB(string StringAWhereSearch, string StringBToSearch)
        {
            int StringAInWhichSearchLength = StringAWhereSearch.Length;
            if (StringAInWhichSearchLength > 0 && StringBToSearch.Length > 0)//safe check for empty values
            {
                //if string A contains string B then string A with replaced stringB by empty will be
                return StringAInWhichSearchLength > StringAWhereSearch.Replace(StringBToSearch, string.Empty).Length;
            }
            return false;

        }
    }
}
