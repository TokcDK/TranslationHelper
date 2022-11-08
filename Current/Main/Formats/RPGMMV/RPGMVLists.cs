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
            { 41, "Image name?" },
            { 44, "" },
            { 45, "" },
            { 118, "label" },
            { 119, "jump to label" }, // must be skipped both with 118
            //{ 122, "Control variables" },
            { 123, "" },
            { 132, "" },
            { 205, "Set Move Route" },
            { 505, "Set Move Route" },
            { 231, "Show Picture" },
            { 232, "Move Picture" },
            { 233, "Rotate Picture" },
            { 234, "Tint Picture" },
            { 235, "Erase Picture" },
            { 236, "Set Weather" },
            { 241, "Play BGM" },
            { 242, "Fadeout BGM" },
            { 243, "Save BGM" },
            { 244, "Resume BGM" },
            { 245, "Play BGS" },
            { 246, "Fadeout BGS" },
            { 249, "Play ME" },
            { 250, "Play SE" },
            { 251, "Stop SE" },
            { 261, "Play Movie" },
            { 283, "" },
            { 284, "Change Parallax Background" },
            //{ 320, "Change name" }, // was ingame npc name text
            { 322, "" },
            { 323, "" },
            //{ 355, "Script command" }, // can contain text
            //{ 356, "Script command" }, // can have text to display
            //{ 357, "Plugin command" }, // sometime here can be text popup when activate npc
            { 302, "" },
            //{ 405, "Show Text" },
            { 657, "Plugin command" },
            //{ 108, "Comment" }, // not to skip in some games
            //{ 408, "Comment" }, // not to skip in some games
        };
    }
}
