using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats.RPGMMV
{
    internal static class RPGMVLists
    {
        /// <summary>
        /// codes which must be skipped
        /// </summary>
        internal static Dictionary<int, string> ExcludedCodes = new Dictionary<int, string>()
        {
            { 41, "Image name?" }, // file name
            { 44, "" }, // file name
            { 45, "" }, // file name
            //{ 118, "" }, // file name
            //{ 122, "" }, // file name
            { 123, "" }, // file name
            { 132, "" }, // file name
            { 231, "Show Picture" }, // file name
            { 232, "Move Picture" }, // file name
            { 233, "Rotate Picture" }, // file name
            { 234, "Tint Picture" }, // file name
            { 235, "Erase Picture" }, // file name
            { 236, "Set Weather" },
            { 241, "Play BGM" }, // file name
            { 242, "Fadeout BGM" }, // file name
            { 243, "Save BGM" }, // file name
            { 244, "Resume BGM" }, // file name
            { 245, "Play BGS" }, // file name
            { 246, "Fadeout BGS" }, // file name
            { 249, "Play ME" }, // file name
            { 250, "Play SE" }, // file name
            { 251, "Stop SE" }, // file name
            { 261, "Play Movie" }, // file name
            { 283, "" }, // file name
            { 320, "" }, // file name
            { 322, "" }, // file name
            { 323, "" }, // file name
            //{ 355, "" }, // file name
            { 356, "" }, // file name
            { 357, "Plugin command" }, // file name
            { 302, "" }, // file name
            { 405, "" }, // file name
            { 657, "Plugin command" }, // file name
            //{ 108, "Comment" }, // not to skip in some games
            //{ 408, "Comment" }, // not to skip in some games
        };
    }
}
