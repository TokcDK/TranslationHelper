using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games
{
    class DungeonAndBride : KiriKiriGameBase
    {
        public DungeonAndBride()
        {
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase() && ExtensionsFileFolder.GetCrc32(ProjectData.SelectedFilePath) == "7c2bfd95";
        }

        internal override string Name()
        {
            return "Dungeon & Bride";
        }

        protected override List<System.Type> FormatType()
        {
            return new List<System.Type>(3) {
                typeof(Formats.KiriKiri.Games.FGroup1.VirginLode2.KS),
                typeof(Formats.KiriKiri.Games.TJS),
                typeof(Formats.KiriKiri.Games.CSV.CSV)
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
            if (string.IsNullOrWhiteSpace(line) || string.IsNullOrWhiteSpace(ProjectData.FilePath) || line.IndexOf('"') == -1)
            {
                return;
            }
            var mapname = Path.GetFileName(ProjectData.FilePath);
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
