using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.KiriKiri.Games.FGroup1.RJ297684GoblinsCave;

namespace TranslationHelper.Projects.KiriKiri.Games.PGroup1.RJ297684GoblinsCave
{
    class RJ297684GoblinsCave : PGroup1Base
    {
        public RJ297684GoblinsCave(THDataWork thDataWork) : base(thDataWork)
        {
            exeCRC = "7c2bfd95";
        }

        internal override bool Check()
        {
            return CheckKiriKiriBase() && exeCRC.Length > 0 && thDataWork.SPath.GetCrc32() == exeCRC;
        }

        internal override bool Open()
        {
            return ExtractXP3Data()
                && OpenFiles(new System.Collections.Generic.List<Formats.FormatBase>
                {
                    new KS(thDataWork)
                }
                );
        }

        internal override string Name()
        {
            return "[RJ297684]Goblins Cave";
        }

        internal override bool Save()
        {
            return SaveFiles(new System.Collections.Generic.List<Formats.FormatBase>
            {
                new KS(thDataWork)
            }
            );
        }
    }
}
