using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.RJ297684GoblinsCave
{
    class RJ297684GoblinsCave : PGroup1Base
    {
        public RJ297684GoblinsCave() : base()
        {
            exeCRC = "7c2bfd95";
        }

        internal override string Name()
        {
            return "[RJ297684]Goblins Cave";
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase() && exeCRC.Length > 0 && ProjectData.SPath.GetCrc32() == exeCRC;
        }

        protected override List<FormatBase> Format()
        {
            return new List<FormatBase>() { new Formats.KiriKiri.Games.FGroup1.RJ297684GoblinsCave.KS() };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks" };
        }
    }
}
