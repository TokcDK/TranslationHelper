using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.VirginLode2
{
    class VirginLode2 : PGroup1Base
    {
        public VirginLode2() : base()
        {
            exeCRC = "dacf4898da60741356cc5c254774e5cb";
        }

        internal override string Name()
        {
            return "Virgin Lode 2";
        }

        internal override bool Check()
        {
            if (CheckKiriKiriBase())
            {
                if (exeCRC.Length > 0 && ProjectData.SelectedFilePath.GetMD5() == exeCRC)
                {
                    return true;
                }
            }
            return false;
        }

        protected override List<Formats.FormatBase> Format()
        {
            return new List<Formats.FormatBase> { new TranslationHelper.Formats.KiriKiri.Games.FGroup1.VirginLode2.KS() };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks" };
        }
    }
}
