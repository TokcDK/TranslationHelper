using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMMV
{
    public static class RPGMVUtils
    {
        public static void GetSkipCodes(this Dictionary<int, string> excludedCodes)
        {
            if (excludedCodes == null) return;

            var codesFile = THSettings.RPGMakerMVSkipCodesFilePath;
            if (!File.Exists(codesFile)) return;

            using (StreamReader sr = new StreamReader(codesFile))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line.Trim().StartsWith(";")) continue;

                    var codeInfo = line.Trim().Split(',');
                    string commment = "";
                    if (codeInfo.Length == 2 && codeInfo[1].Length > 0) commment = codeInfo[1];

                    var code = codeInfo[0];
                    if (!int.TryParse(code, out int codeValue)) continue;

                    excludedCodes.TryAdd(codeValue, commment);
                }
            }
        }

        /// <summary>
        /// returns event name if found
        /// </summary>
        /// <param name="currentEventCode"></param>
        /// <returns></returns>
        public static string GetCodeName(int currentEventCode)
        {
            if (EventCommandCodes.ContainsKey(currentEventCode))
            {
                var eventName = EventCommandCodes[currentEventCode];
                if (eventName.Length > 0)
                {
                    return "\r\nCommand action: \"" + eventName + "\"";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// list of event codes
        /// </summary>
        public static readonly Dictionary<int, string> EventCommandCodes = new Dictionary<int, string>()
        {
            { 0, "End Show Choices" },
            { 41, "Image name?" },
            { 101, "Show Text" },
            { 102, "Show Choices" },
            { 103, "Input Number" },
            { 104, "Select Item" },
            { 105, "Show Scrolling Text" },
            { 108, "Comment" },
            { 111, "Conditional Branch" },
            { 112, "Loop" },
            { 113, "Break Loop" },
            { 115, "Exit Event Processing" },
            { 117, "Common Event" },
            { 118, "Label" },
            { 119, "Jump to Label" },
            { 121, "Control Switches" },
            { 122, "Control Variables" },
            { 123, "Control Self Switch" },
            { 124, "Control Timer" },
            { 125, "Change Gold" },
            { 126, "Change Items" },
            { 127, "Change Weapons" },
            { 128, "Change Armor" },
            { 129, "Change Party Member" },
            { 132, "Change Battle BGM" },
            { 133, "Change Battle End ME" },
            { 134, "Change Save Access" },
            { 135, "Change Menu Access" },
            { 136, "Change Encounter Disable" },
            { 137, "Change Formation Access" },
            { 138, "Change Window Color" },
            { 139, "Change Defeat ME" },
            { 140, "Change Vehicle BGM" },
            { 201, "Transfer Player" },
            { 202, "Set Vehicle Location" },
            { 203, "Set Event Location" },
            { 204, "Scroll Map" },
            { 205, "Set Move Route" },
            { 206, "Getting On and Off Vehicles" },
            { 211, "Change Transparency" },
            { 212, "Show Animation" },
            { 213, "Show Balloon Icon" },
            { 214, "Temporarily Erase Event" },
            { 216, "Change Player Followers" },
            { 217, "Gather Followers" },
            { 221, "Fadeout Screen" },
            { 222, "Fadein Screen" },
            { 223, "Tint Screen" },
            { 224, "Screen Flash" },
            { 225, "Screen Shake" },
            { 230, "Wait" },
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
            { 281, "Change Map Name Display" },
            { 282, "Change Tileset" },
            { 283, "Change Battle Background" },
            { 284, "Change Parallax Background" },
            { 285, "Get Location Info" },
            { 301, "Battle Processing" },
            { 302, "Shop Processing" },
            { 303, "Name Input Processing" },
            { 311, "Change HP" },
            { 312, "Change MP" },
            { 313, "Change State" },
            { 314, "Recover All" },
            { 315, "Change EXP" },
            { 316, "Change Level" },
            { 317, "Change Parameters" },
            { 318, "Change Skills" },
            { 319, "Change Equipment" },
            { 320, "Change Name" },
            { 321, "Change Class" },
            { 322, "Change Actor Graphic" },
            { 323, "Change Vehicle Graphic" },
            { 324, "Change Nickname" },
            { 325, "Change Profile" },
            { 326, "Change TP" },
            { 331, "Change Enemy HP" },
            { 332, "Change Enemy MP" },
            { 333, "Change Enemy State" },
            { 334, "Enemy Recover All" },
            { 335, "Enemy Appear" },
            { 336, "Enemy Transform" },
            { 337, "Show Battle Animation" },
            { 339, "Force Action" },
            { 340, "Abort Battle" },
            { 342, "Change Enemy TP" },
            { 351, "Open Menu Screen" },
            { 352, "Open Save Screen" },
            { 353, "Game Over" },
            { 354, "Return to Title Screen" },
            { 355, "Script" },
            { 356, "Plugin command" },
            { 357, "Plugin command" },
            { 401, "Show Text" },
            { 402, "When [**] Choice" },
            { 403, "When Cancel" },
            { 405, "Show Text" },
            { 408, "Comment" },
            { 411, "Else" },
            { 412, "End Conditional Branch" },
            { 413, "Repeat Above" },
            { 601, "If Win" },
            { 602, "If Escape" },
            { 603, "If Lose" },
            { 604, "End Battle Result" },
            { 655, "Script" },
            { 657, "Plugin command" },
        };
    }
}
