using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.VirginLode2
{
    class VirginLode2 : PGroup1Base
    {
        public VirginLode2()
        {
            ExeCrc = "dacf4898da60741356cc5c254774e5cb";
        }

        internal override string Name()
        {
            return "Virgin Lode 2";
        }

        internal override bool Check()
        {
            if (CheckKiriKiriBase())
            {
                if (ExeCrc.Length > 0 && ProjectData.SelectedFilePath.GetMd5() == ExeCrc)
                {
                    return true;
                }
            }
            return false;
        }

        protected override List<System.Type> FormatType()
        {
            return new List<System.Type> { typeof(Formats.KiriKiri.Games.FGroup1.VirginLode2.Ks) };
        }

        protected override string[] Mask()
        {
            return new[] { "*.ks" };
        }
    }
}
