using System;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.RJ297684GoblinsCave
{
    class Rj297684GoblinsCave : PGroup1Base
    {
        public Rj297684GoblinsCave()
        {
            ExeCrc = "7c2bfd95";
        }

        internal override string Name()
        {
            return "[RJ297684]Goblins Cave";
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase() && ExeCrc.Length > 0 && ProjectData.SelectedFilePath.GetCrc32() == ExeCrc;
        }

        protected override List<Type> FormatType()
        {
            return new List<Type>() { typeof(Formats.KiriKiri.Games.FGroup1.RJ297684GoblinsCave.Ks) };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks" };
        }
    }
}
