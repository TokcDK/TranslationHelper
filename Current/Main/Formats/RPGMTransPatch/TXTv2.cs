using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMTransPatch
{
    class TXTv2 : PatchTXTBase
    {
        public TXTv2(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override string PatchFileID()
        {
            return "# RPGMAKER TRANS PATCH FILE VERSION 2.0";
        }

        protected override bool GetUnused()
        {
            return ParseData.Line == "# UNUSED TRANSLATABLES";
        }

        protected override string StringBlockBeginId()
        {
            return "# TEXT STRING";
        }

        protected override void GetOriginalPre(List<string> originalLines)
        {
            //add first original line (was set last to line in check of previous block)
            originalLines.Add(ParseData.Line);
        }
        protected override string OriginalEndID()
        {
            return "# TRANSLATION";
        }
        protected override string ContextInfoID()
        {
            return "# CONTEXT";
        }
        bool IsitemAttr = false;
        protected override void GetContextPre(List<string> contextLines)
        {
            IsitemAttr = false;
        }
        protected override void PreContextAdd()
        {
            if (!IsitemAttr && ParseData.Line.EndsWith("itemAttr/UsageMessage"))
            {
                IsitemAttr = true;
            }
        }
        protected override void ParseOriginalAdviceContext(out List<string> originalLines, out List<string> contextLines, out string advice)
        {
            //------------------------------------
            //read context
            GetContext(out contextLines);
            //------------------------------------
            //read advice
            GetAdvice(out advice);
            //------------------------------------
            //read original
            GetOriginal(out originalLines);
            //------------------------------------
        }

        protected override bool IsOpenModeSkipTheBlock()
        {
            if (IsitemAttr) //skip itemAttr for p2 in open mode
            {
                return true;
            }

            return false;
        }
        protected override string AdviceInfo(string advice)
        {
            return "\r\n" + advice;
        }

        /// <summary>
        /// need only for wolftrans and rpgmtrans patch v3
        /// </summary>
        /// <returns></returns>
        protected override void SetContext(List<string> contextLines, bool translated)
        {
            return;
        }

        protected override string EndTranslationID()
        {
            return "# END STRING";
        }
        protected override string GetNewTextBlockContent(bool translated, List<string> contextLines, string advice, string original, string translation)
        {
            return
                                StringBlockBeginId()
                                + "\n"
                                + (!translated ? "# UNTRANSLATED" + "\n" : string.Empty)
                                + string.Join("\n", contextLines)
                                + "\n"
                                + advice
                                + "\n"
                                + original
                                + "\n"
                                + OriginalEndID()
                                + "\n"
                                + translation
                                + "\n"
                                + EndTranslationID()
                                ;
        }

        /// <summary>
        /// skip '# TRANSLATION'
        /// </summary>
        protected override void GetOriginalPost()
        {
            ReadLine();
        }

        protected override void GetAdvice(out string advice)
        {
            if (ParseData.Line.StartsWith("# ADVICE")) //advice missing sometimes
            {
                advice = ParseData.Line;
                ReadLine();
            }
            else
            {
                advice = "";
            }
        }

        public override string Description => "RPGMTrans patch txt";
    }
}
