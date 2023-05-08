using System;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.RJ297684GoblinsCave
{
    class RJ297684GoblinsCave : PGroup1Base
    {
        public RJ297684GoblinsCave()
        {
            exeCRC = "7c2bfd95";
        }

        public override string Name => "[RJ297684]Goblins Cave";

        internal override bool IsValid()
        {
            return CheckKiriKiriBase() && exeCRC.Length > 0 && AppData.SelectedProjectFilePath.GetCrc32() == exeCRC;
        }

        protected override List<Type> FormatType()
        {
            return new List<Type>() { typeof(Formats.KiriKiri.Games.FGroup1.RJ297684GoblinsCave.KS) };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks" };
        }
    }
}
