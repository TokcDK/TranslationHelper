using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.WolfRPG
{
    abstract class RPGMWolfTransPatchBase : FormatBase
    {
        protected RPGMWolfTransPatchBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override void ParseStringFilePreOpenExtra()
        {
            unused = false;
        }

        /// <summary>
        /// true when patch version was checked
        /// </summary>
        protected bool verchecked;
        /// <summary>
        /// true if found unused string blocks for patch v2
        /// </summary>
        protected bool unused;

        /// <summary>
        /// is wolftrans patch
        /// </summary>
        bool IsWolfTrans = false;
        /// <summary>
        /// is rpgmtrans v2 patch
        /// </summary>
        bool IsRPGMTransVerson2 = false;

        /// <summary>
        /// check if it is patch txt
        /// </summary>
        /// <returns></returns>
        protected bool CheckSetPatchVersion()
        {
            if (!verchecked)
            {
                if (ParseData.line == "> RPGMAKER TRANS PATCH FILE VERSION 3.2")
                {
                    IsWolfTrans = false;
                    IsRPGMTransVerson2 = false;
                }
                else if (ParseData.line == "# RPGMAKER TRANS PATCH FILE VERSION 2.0")
                {
                    IsRPGMTransVerson2 = true;
                }
                else if (ParseData.line == "> WOLF TRANS PATCH FILE VERSION 1.0")
                {
                    IsWolfTrans = true;
                }
                else
                {
                    return false; //Not a patch file, break parsing
                }

                verchecked = true;
            }

            return true;
        }

        /// <summary>
        /// is wolftrans
        /// </summary>
        bool W => IsWolfTrans;
        /// <summary>
        /// is rpgmtrans v2
        /// </summary>
        bool P2 => !IsWolfTrans && IsRPGMTransVerson2;

        /// <summary>
        /// check if string block begin
        /// </summary>
        /// <returns></returns>
        protected bool IsBeginString()
        {
            if (!unused)//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
            {
                if (P2)
                {
                    if (ParseData.line == "# UNUSED TRANSLATABLES")
                    {
                        unused = true;
                    }
                }
                else
                {
                    unused = true;
                }
            }

            return ParseData.line.StartsWith(P2 ? "# TEXT STRING" : "> BEGIN STRING");
        }

        /// <summary>
        /// read advice for rpgm trans patch v2
        /// </summary>
        /// <param name="advice"></param>
        protected void GetAdvice(out string advice)
        {
            if (ParseData.line.StartsWith("# ADVICE")) //advice missing sometimes
            {
                advice = ParseData.line;
                ReadLine();
            }
            else
            {
                advice = "";
            }
        }

        /// <summary>
        /// read original lines
        /// </summary>
        /// <param name="originalLines"></param>
        protected void GetOriginal(out List<string> originalLines)
        {
            originalLines = new List<string>();
            if (P2)
            {
                //add first original line (was set last to line in check of previous block)
                originalLines.Add(ParseData.line);
            }

            while (!ReadLine().StartsWith(P2 ? "# TRANSLATION" : "> CONTEXT"))
            {
                originalLines.Add(ParseData.line);
            }

            if (P2)
            {
                ReadLine();//skip # TRANSLATION
            }
        }

        bool IsitemAttr = false;
        /// <summary>
        /// read context lines
        /// </summary>
        /// <param name="contextLines"></param>
        protected void GetContext(out List<string> contextLines)
        {
            contextLines = new List<string>();
            if (P2)
            {
                IsitemAttr = false;
            }
            else
            {
                //add first context line (was set last to line in check of previous block)
                contextLines.Add(ParseData.line);
            }

            //add rest of context lines
            bool wolftransfail = false;
            while (ReadLine().StartsWith(P2 ? "# CONTEXT" : "> CONTEXT") || (W && (wolftransfail = ParseData.line.EndsWith(" < UNTRANSLATED"))/*bug in wolftrans, sometime filenames placed in next line*/))
            {
                if (wolftransfail)
                {
                    contextLines[contextLines.Count - 1] = contextLines[contextLines.Count - 1] + ParseData.line;
                }
                else
                {
                    if (P2 && !IsitemAttr && ParseData.line.EndsWith("itemAttr/UsageMessage"))
                    {
                        IsitemAttr = true;
                    }

                    contextLines.Add(ParseData.line);
                }
            }
        }

        /// <summary>
        /// read translation lines
        /// </summary>
        /// <param name="translationLines"></param>
        protected void GetTranslation(out List<string> translationLines)
        {
            translationLines = new List<string>
            {
                //add last set line in check of previous context block
                ParseData.line
            };
            while (!ReadLine().StartsWith(P2 ? "# END STRING" : "> END STRING"))
            {
                translationLines.Add(ParseData.line);
            }
        }

        protected override void SaveModeAddLine(string newline = "\r\n", bool LastEmptyLine = false)
        {
            base.SaveModeAddLine("\n", false);
        }

        /// <summary>
        /// read string block parts data
        /// </summary>
        protected void ParseBeginEndBlock()
        {
            List<string> originalLines;
            List<string> contextLines;
            string advice = "";
            if (P2)
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
            else
            {
                //------------------------------------
                //read original
                GetOriginal(out originalLines);
                //------------------------------------
                //read context
                GetContext(out contextLines);
                //------------------------------------
            }

            //add translation if exists
            GetTranslation(out List<string> translationLines);
            //------------------------------------

            //add begin end string block data
            GetSetBlock(originalLines, contextLines, translationLines, advice);
        }

        /// <summary>
        /// add data to table or set all data to string block
        /// </summary>
        protected void GetSetBlock(List<string> originalLines, List<string> contextLines, List<string> translationLines, string advice)
        {
            var original = string.Join("\n", originalLines);
            var translation = string.Join("\n", translationLines);

            if (thDataWork.OpenFileMode)
            {
                if (P2 && IsitemAttr)//skip itemAttr for p2
                {
                    return;
                }

                var context = string.Join("\r\n", contextLines);
                if (string.IsNullOrEmpty(translation))
                {
                    AddRowData(original, context + (P2 ? "\r\n" + advice : ""), true);
                }
                else
                {
                    AddRowData(new[] { original, translation }, context + (P2 ? "\r\n" + advice : ""), true);
                }
            }
            else
            {
                var trans = original;
                var translated =
                    thDataWork.SaveFileMode // save mode
                    && IsValidString(original) // valid original
                    && SetTranslation(ref trans)  // translation found
                    && (!string.IsNullOrEmpty(trans)) // translation not null and not empty
                    && (original != trans && trans != translation) // translation not equal original and previous translation
                    ;

                if (translated)
                {
                    ParseData.Ret = true;
                    translation = trans;
                }

                SetContext(contextLines, !string.IsNullOrEmpty(translation));

                ParseData.line =
                    P2
                    ?
                                "# TEXT STRING"
                                + "\n"
                                + (!translated ? "# UNTRANSLATED" + "\n" : string.Empty)
                                + string.Join("\n", contextLines)
                                + "\n"
                                + advice
                                + "\n"
                                + original
                                + "\n"
                                + "# TRANSLATION"
                                + "\n"
                                + translation
                                + "\n"
                                + "# END STRING"

                    :
                                "> BEGIN STRING"
                                + "\n" +
                                original
                                + "\n" +
                                string.Join("\n", contextLines)
                                + "\n" +
                                translation
                                + "\n" +
                                "> END STRING"
                    ;

                SaveModeAddLine("\n");//add endstring line
            }
        }

        /// <summary>
        /// set context when with translated\untranslated tags
        /// </summary>
        /// <param name="contextLines"></param>
        /// <param name="translated"></param>
        protected void SetContext(List<string> contextLines, bool translated)
        {
            if (P2)
            {
                return;//need only for wolftrans and rpgmtrans patch v3
            }

            for (int i = 0; i < contextLines.Count; i++)
            {
                var ends = contextLines[i].TrimEnd().EndsWith(" < UNTRANSLATED");

                if (translated)
                {
                    if (ends)
                    {
                        contextLines[i] = contextLines[i].Replace(" < UNTRANSLATED", string.Empty);
                    }
                }
                else
                {
                    if (!ends)
                    {
                        contextLines[i] = contextLines[i] + " < UNTRANSLATED";
                    }
                }
            }
        }

        /// <summary>
        /// check and parse lines
        /// </summary>
        /// <returns></returns>
        protected int CheckAndParse()
        {
            //skip if not patch files
            if (!CheckSetPatchVersion())
            {
                return -1;
            }

            //skip if begin string not found
            if (IsBeginString())
            {
                ParseBeginEndBlock();
            }
            else
            {
                SaveModeAddLine();
            }

            return 0;
        }
    }
}
