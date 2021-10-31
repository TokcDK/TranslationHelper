using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Extensions
{
    static class ExtensionsArray
    {
        /// <summary>
        /// Get values from the <paramref name="array"/> starting from <paramref name="startIndex"/> and with selected <paramref name="length"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] GetRange<T>(this T[] array, int startIndex, int length)
        {
            T[] rangeValuesArray = new T[length];
            Array.Copy(array, startIndex, rangeValuesArray, 0, length);
            return rangeValuesArray;
        }
    }
}
