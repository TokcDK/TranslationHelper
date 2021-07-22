using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.WolfRPG
{
    abstract class WolfRPGBase : ProjectBase
    {
        protected WolfRPGBase() : base()
        {
            HideVarsBase = new Dictionary<string, string>
            {
                {"\\cself[", @"「?\\\\cself\[[0-9]{1,12}\]」?"},
                {"\\img[", @"\\\\img\[[^\]]+\]"},
                {"\\cdb[", @"\\\\cdb\[[^:]+:[^:]+:[^:\]]+\]"},
                {"\\udb[", @"\\\\udb\[[^:]+:[^:]+:[^:\]]+\]"}
            };
        }

        internal override string ProjectFolderName()
        {
            return "WolfRPG";
        }
        internal override bool TablesLinesDictAddEqual => true;

        internal override string OnlineTranslationProjectSpecificPosttranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            t = HardcodedFixes(o, t);

            return RestoreVARS(t);
        }
        //\\\\r\[[^\,]+\,[^\]]+\]
        internal override string HardcodedFixes(string original, string translation)
        {
            //fix escape sequences 
            if (Regex.IsMatch(translation, @"(?<!\\)\\[^sntr><#\\]"))
            {
                var sequences = new char[] { 'S', 'N', 'T', 'R' };
                var mc = Regex.Matches(translation, @"(?<!\\)\\[^sntr><#\\]");
                for (int i = mc.Count - 1; i >= 0; i--)
                {
                    foreach (var schar in sequences)
                    {
                        if (mc[i].Value == "\\" + schar)
                        {
                            translation = translation.Remove(mc[i].Index, mc[i].Length).Insert(mc[i].Index, ("\\" + schar).ToLower());
                        }
                    }
                }
            }
            return translation;
        }
    }
}
