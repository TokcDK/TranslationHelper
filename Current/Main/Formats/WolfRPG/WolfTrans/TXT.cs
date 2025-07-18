﻿using System.Collections.Generic;
using TranslationHelper.Formats.RPGMTransPatch;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.WolfRPG.WolfTrans
{
    class TXT : PatchTXTBase
    {
        public TXT(ProjectBase parentProject) : base(parentProject)
        {
        }

        public override string Description => "WolfRPG patch txt";

        protected override string PatchFileID()
        {
            return "> WOLF TRANS PATCH FILE VERSION 1.0";
        }

        ///*bug in wolftrans, sometime filenames placed in next line*/
        bool wolftransfail;
        protected override bool ContextExtraCondition()
        {
            return wolftransfail = (!startsWinContext && ParseData.Line.EndsWith(" < UNTRANSLATED"));
        }
        protected override bool IsExtraConditionExecuted(List<string> contextLines)
        {
            if (wolftransfail)
            {
                contextLines[contextLines.Count - 1] = contextLines[contextLines.Count - 1] + ParseData.Line;
                return true;
            }

            return false;
        }
        //------------------------------------------------------
    }
}
