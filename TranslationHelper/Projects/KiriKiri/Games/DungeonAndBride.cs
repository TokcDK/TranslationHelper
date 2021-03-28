using System.Collections.Generic;
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
            return new[] { "*.ks", "*.tjs","*.csv" };
        }
    }
}
