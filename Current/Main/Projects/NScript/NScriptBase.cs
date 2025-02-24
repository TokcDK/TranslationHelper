using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Projects.NScript
{
    abstract class NScriptBase : ProjectBase
    {
        protected NScriptBase()
        {
            HideVarsBase = new Dictionary<string, string>
            {
                {"#", @"#[A-F0-9]{6}"},
                {"%", @"\%[a-z0-9_]+"},
                {"$", @"\$[a-z0-9_]+"}
            };
        }

        internal override string ProjectDBFolderName => "NScript";

        internal override string OnlineTranslationProjectSpecificPretranslationAction(string o, string t, int tind = -1, int rind = -1)
        {
            //なるほど、“(首薬/しゅやく)　(発太/はった)”ね。
            if (o.Contains("/") && o.Contains("(") && Regex.IsMatch(o, @"\(([^/]+)/[^\)]+\)"))
            {
                foreach (Match m in Regex.Matches(o, @"\(([^/]+)/[^\)]+\)"))
                {
                    o = o.Replace(m.Value, m.Result("$1"));//remove japanese Kana reading(しゅやく) part string
                }
            }

            return base.OnlineTranslationProjectSpecificPretranslationAction(o,t,tind,rind);
        }
    }
}
