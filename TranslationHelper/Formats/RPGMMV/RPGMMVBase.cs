using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV
{
    abstract class RPGMMVBase : FormatBase
    {
        public RPGMMVBase() : base()
        {
        }

        /// <summary>
        /// list of event codes
        /// </summary>
        protected readonly Dictionary<int, string> EventCodes = new Dictionary<int, string>(118)
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
            { 356, "Plugin" },
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
        };

        /// <summary>
        /// codes which must be skipped
        /// </summary>
        protected static readonly Dictionary<int, string> ExcludedCodes = new Dictionary<int, string>(19)
        {
            { 41, "Image name?" }, // file name
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
            { 108, "Comment" }, // not to skip in some games
            { 408, "Comment" }, // not to skip in some games
        };
    }
}
