using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG
{
    /// <summary>
    /// The <c>StandaloneContextList.thdata</c> side file that sits next to the game: extra context
    /// lines a translator attached to an original string.
    /// <para>
    /// The WolfRPG patch formats read it while they split strings into blocks, and the menu item of
    /// the same name writes it. The file itself, and the way it is parsed, is a property of the
    /// WolfRPG patch family, so it lives with the formats rather than with the menu that edits it.
    /// </para>
    /// </summary>
    internal static class StandaloneContextList
    {
        /// <summary>
        /// Marker written before the original string of a block.
        /// </summary>
        internal static readonly string BeginStringMarker = "> BEGIN STRING\r\n";

        /// <summary>
        /// Marker written after the original string of a block.
        /// </summary>
        internal static readonly string EndStringMarker = "\r\n> END STRING\r\n";

        /// <summary>
        /// Full path of the context list of the currently selected game.
        /// </summary>
        internal static readonly string StandaloneContextFilePath =
            Path.Combine(AppData.CurrentProject.SelectedGameDir, "StandaloneContextList.thdata");

        /// <summary>
        /// Reads the context list. Returns an empty list when the file does not exist or holds no
        /// valid block.
        /// </summary>
        /// <param name="standaloneContextFilePath">Path of the context list to read.</param>
        /// <returns>Original string to context lines.</returns>
        internal static Dictionary<string, HashSet<string>> LoadList(string standaloneContextFilePath)
        {
            var standaloneContextList = new Dictionary<string, HashSet<string>>();
            if (File.Exists(standaloneContextFilePath))
            {
                // load exist
                var blocks = File.ReadAllText(standaloneContextFilePath).Split(new[] { EndStringMarker }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var block in blocks)
                {
                    var keyvalue = block.Split(new[] { "\r\n" + BeginStringMarker }, StringSplitOptions.None);
                    if (keyvalue.Length != 2) // dont load invalid
                    {
                        continue;
                    }
                    var context = keyvalue[0];
                    var originalString = keyvalue[1];
                    Add(standaloneContextList, originalString, context);
                }
            }

            return standaloneContextList;
        }

        /// <summary>
        /// Adds a context line for an original string. Does nothing when the pair is already known.
        /// </summary>
        /// <param name="standaloneContextList">The list to add to.</param>
        /// <param name="stringValue">The original string.</param>
        /// <param name="contextLine">The context line belonging to it.</param>
        internal static void Add(Dictionary<string, HashSet<string>> standaloneContextList, string stringValue, string contextLine)
        {
            if (!standaloneContextList.ContainsKey(stringValue))
            {
                standaloneContextList.Add(stringValue, new HashSet<string>());
                standaloneContextList[stringValue].Add(contextLine);
            }
            else
            {
                if (!standaloneContextList[stringValue].Contains(contextLine)) // add only if context not in list
                {
                    standaloneContextList[stringValue].Add(contextLine);
                }
            }
        }

        /// <summary>
        /// Writes the context list back to disk.
        /// </summary>
        /// <param name="standaloneContextFilePath">Path of the context list to write.</param>
        /// <param name="standaloneContextList">The list to write.</param>
        internal static void SaveList(string standaloneContextFilePath, Dictionary<string, HashSet<string>> standaloneContextList)
        {
            var str = new StringBuilder();
            foreach (var val in standaloneContextList)
            {
                foreach (var context in val.Value)
                {
                    str.AppendLine(context);
                }
                str.Append(BeginStringMarker + val.Key + EndStringMarker);
            }

            File.WriteAllText(standaloneContextFilePath, str.ToString());
        }

        /// <summary>
        /// Removes the trailing untranslated tag from a context line.
        /// </summary>
        /// <param name="addedContextLine">The line to clean.</param>
        internal static void CleanContext(ref string addedContextLine)
        {
            foreach (var mark in new[]
            {
                        '<',
                        '#'
                    })
            {
                var tag = " " + mark + " UNTRANSLATED";
                if (addedContextLine.EndsWith(tag))
                {
                    addedContextLine = addedContextLine.Replace(tag, string.Empty);
                }
            }
        }
    }
}
