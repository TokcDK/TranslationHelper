using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Extensions
{
    static class ExtensionsDictionary
    {
        /// <summary>
        /// Add Key-Value if Key not exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dict"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        internal static void AddTry<T>(this Dictionary<T,T> Dict, T Key, T Value)
        {
            if (!Dict.ContainsKey(Key))
            {
                Dict.Add(Key, Value);
            }
        }
    }
}
