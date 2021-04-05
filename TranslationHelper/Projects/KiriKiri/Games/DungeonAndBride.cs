﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class DungeonAndBride : KiriKiriGameBase
    {
        public DungeonAndBride(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase() && ExtensionsFileFolder.GetCrc32(thDataWork.SPath) == "7c2bfd95";
        }

        internal override string Name()
        {
            return "Dungeon & Bride";
        }

        protected override List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase> {
                new TranslationHelper.Formats.KiriKiri.Games.FGroup1.VirginLode2.KS(thDataWork),
                new TranslationHelper.Formats.KiriKiri.Games.TJS(thDataWork),
                new TranslationHelper.Formats.KiriKiri.Games.CSV.CSV(thDataWork)
            };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks", "*.tjs", "*.csv" };
        }

        internal override void ReadLineMod(ref string line)
        {
            ReplaceLatinInlineTagsToJPLatin(ref line);
        }

        private void ReplaceLatinInlineTagsToJPLatin(ref string line)
        {
            if (string.IsNullOrWhiteSpace(line) || string.IsNullOrWhiteSpace(thDataWork.FilePath) || line.IndexOf('"') == -1)
            {
                return;
            }
            var mapname = Path.GetFileName(thDataWork.FilePath);
            if (!mapname.EndsWith("tjs"))
            {
                return;
            }
            var mc = Regex.Matches(line, @"(\""[^\""]+\"")");
            if (mc.Count > 0)
            {
                for (int i = mc.Count - 1; i >= 0; i--)
                {
                    var subarg = mc[i].Value;
                    if (!Regex.IsMatch(subarg, @"[rgypbvwa]{2}") && Regex.IsMatch(subarg, @"[a-z]{2,}"))//if more of one latin char like "fore" or kind of
                    {
                        continue;
                    }
                    if (mapname == "map.tjs" && subarg == "w")//exclude west way mark in map
                    {
                        continue;
                    }
                    if (mapname == "master.tjs" && subarg == "b")//exclude broken mark in master.tjs
                    {
                        continue;
                    }
                    subarg = subarg
                        .Replace("r", "ｒ")
                        .Replace("g", "ｇ")
                        .Replace("y", "ｙ")
                        .Replace("p", "ｐ")
                        .Replace("b", "ｂ")
                        .Replace("v", "ｖ")
                        .Replace("w", "ｗ")
                        .Replace("a", "ａ")
                        ;
                    if (subarg != mc[i].Value)
                    {
                        line = line.Remove(mc[i].Index, mc[i].Length).Insert(mc[i].Index, subarg);
                    }
                }
            }
        }
    }
}