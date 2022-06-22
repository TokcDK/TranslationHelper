using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;
using TranslationHelper.Projects.WolfRPG.Menus;

namespace TranslationHelper.Formats.WolfRPG
{
    abstract class RPGMWolfTransPatchBase : FormatStringBase
    {
        protected RPGMWolfTransPatchBase()
        {
        }

        internal override string Ext => ".txt";

        protected override void PreOpenExtraActions()
        {
            unused = false;

            // check standalone context file exist
            if (AppData.SaveFileMode && File.Exists(AddToStandaloneContextList.StandaloneContextFilePath))
            {
                _contextSplitInfo = AddToStandaloneContextList.LoadList(AddToStandaloneContextList.StandaloneContextFilePath);
                if (_contextSplitInfo.Count > 0)
                {
                    _useContext = true;
                }
                else
                {
                    _contextSplitInfo = null;
                }
            }
        }

        /// <summary>
        /// true when patch version was checked
        /// </summary>
        protected bool verchecked;
        /// <summary>
        /// true if found unused string blocks for patch v2
        /// </summary>
        protected bool unused;

        protected override KeywordActionAfter ParseStringFileLine()
        {
            return CheckAndParse();
        }

        /// <summary>
        /// check if it is patch txt
        /// </summary>
        /// <returns></returns>
        protected bool CheckSetPatchVersion()
        {
            if (!verchecked)
            {
                if (ParseData.Line != PatchFileID())
                {
                    return false; //Not a patch file, break parsing
                }

                verchecked = true;
            }

            return true;
        }

        /// <summary>
        /// patch file identifier string.
        /// </summary>
        /// <returns></returns>
        protected abstract string PatchFileID();

        /// <summary>
        /// check if string block begin
        /// </summary>
        /// <returns></returns>
        protected bool IsBeginString()
        {
            if (!unused)//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
            {
                unused = GetUnused();
            }

            return ParseData.Line.StartsWith(StringBlockBeginId());
        }

        /// <summary>
        /// determine unused part of patch txt
        /// default for wolf,rpgm3
        /// </summary>
        protected virtual bool GetUnused()
        {
            return true;
        }

        /// <summary>
        /// determines begin of string block
        /// default for wolf,rpgm3
        /// </summary>
        /// <returns></returns>
        protected virtual string StringBlockBeginId()
        {
            return "> BEGIN STRING";
        }

        /// <summary>
        /// read advice for rpgm trans patch v2
        /// </summary>
        /// <param name="advice"></param>
        protected virtual void GetAdvice(out string advice)
        {
            advice = "";
        }

        /// <summary>
        /// read original lines
        /// </summary>
        /// <param name="originalLines"></param>
        protected void GetOriginal(out List<string> originalLines)
        {
            originalLines = new List<string>();
            GetOriginalPre(originalLines);

            while (!ReadLine().StartsWith(OriginalEndID()))
            {
                originalLines.Add(ParseData.Line);
            }

            GetOriginalPost();
        }

        /// <summary>
        /// original text end identifier
        /// default for wolf,rpgm3
        /// </summary>
        protected virtual string OriginalEndID()
        {
            return "> CONTEXT";
        }

        /// <summary>
        /// do nothing for wolf,rpgm3
        /// </summary>
        protected virtual void GetOriginalPost() { }

        /// <summary>
        /// read 1st line for rpgm trans patch v2
        /// do nothing for wolf,rpgm3
        /// </summary>
        /// <param name="originalLines"></param>
        protected virtual void GetOriginalPre(List<string> originalLines) { }

        /// <summary>
        /// true if current line is starts with context id
        /// </summary>
        protected bool startsWinContext;
        /// <summary>
        /// read context lines
        /// </summary>
        /// <param name="contextLines"></param>
        protected void GetContext(out List<string> contextLines)
        {
            contextLines = new List<string>();
            GetContextPre(contextLines);

            //add rest of context lines
            while (startsWinContext = ReadLine().StartsWith(ContextInfoID()) || ContextExtraCondition())
            {
                if (!IsExtraConditionExecuted(contextLines))
                {
                    PreContextAdd();

                    contextLines.Add(ParseData.Line);
                }
            }
        }

        /// <summary>
        /// pre context lines add action
        /// add line for wolf,rpgm3
        /// reset itemattr value for rpgm3
        /// </summary>
        /// <param name="contextLines"></param>
        protected virtual void GetContextPre(List<string> contextLines)
        {
            //add first context line (was set last to line in check of previous block)
            contextLines.Add(ParseData.Line);
        }

        /// <summary>
        /// execute before context line add
        /// no actions by default
        /// </summary>
        protected virtual void PreContextAdd()
        {
        }

        /// <summary>
        /// extra check for context line identify
        /// false by default
        /// using by wolf patch
        /// </summary>
        /// <returns></returns>
        protected virtual bool ContextExtraCondition()
        {
            return false; //(W && (wolftransfail = (!startsWinContext && ParseData.Line.EndsWith(" < UNTRANSLATED"))
        }

        /// <summary>
        /// action on extra condition execute
        /// false by default
        /// using by wolf patch
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsExtraConditionExecuted(List<string> contextLines)
        {
            return false;
        }

        /// <summary>
        /// context info identifier
        /// default for wolf,rpgm3
        /// </summary>
        protected virtual string ContextInfoID()
        {
            return "> CONTEXT";
        }

        /// <summary>
        /// read translation lines
        /// </summary>
        /// <param name="translationLines"></param>
        protected void GetTranslation(out List<string> translationLines)
        {
            translationLines = new List<string>();
            while (!ParseData.Line.StartsWith(EndTranslationID()))
            {
                translationLines.Add(ParseData.Line);
                ReadLine();
            }
        }

        /// <summary>
        /// string block and translation end identifier.
        /// default for wolf,rpgm3
        /// </summary>
        protected virtual string EndTranslationID()
        {
            return "> END STRING";
        }

        /// <summary>
        /// check and parse lines
        /// </summary>
        /// <returns></returns>
        protected KeywordActionAfter CheckAndParse()
        {
            //skip if not patch files
            if (!CheckSetPatchVersion())
            {
                return KeywordActionAfter.Break;
            }

            //skip if begin string not found
            if (IsBeginString())
            {
                ParseBeginEndBlock();
            }
            else
            {
                SaveModeAddLine(newline: "\n");
            }

            return KeywordActionAfter.Continue;
        }

        /// <summary>
        /// read string block parts data
        /// </summary>
        protected virtual void ParseBeginEndBlock()
        {
            ParseOriginalAdviceContext(out List<string> originalLines, out List<string> contextLines, out string advice);

            //add translation if exists
            GetTranslation(out List<string> translationLines);
            //------------------------------------

            //add begin end string block data
            GetSetBlock(originalLines, contextLines, translationLines, advice);
        }

        /// <summary>
        /// Parse original, context and advice blocks
        /// </summary>
        /// <param name="originalLines"></param>
        /// <param name="contextLines"></param>
        /// <param name="advice"></param>
        protected virtual void ParseOriginalAdviceContext(out List<string> originalLines, out List<string> contextLines, out string advice)
        {
            advice = "";
            //------------------------------------
            //read original
            GetOriginal(out originalLines);
            //------------------------------------
            //read context
            GetContext(out contextLines);
            //------------------------------------
        }

        /// <summary>
        /// add data to table or set all data to string block
        /// </summary>
        protected void GetSetBlock(List<string> originalLines, List<string> contextLines, List<string> translationLines, string advice)
        {
            var original = string.Join("\n", originalLines);
            var translation = string.Join("\n", translationLines);

            if (AppData.OpenFileMode)
            {
                if (IsOpenModeSkipTheBlock())
                {
                    return;
                }

                var context = string.Join("\r\n", contextLines);
                if (string.IsNullOrEmpty(translation))
                {
                    AddRowData(original, context + AdviceInfo(advice), CheckInput: false);
                }
                else
                {
                    AddRowData(new[] { original, translation }, context + AdviceInfo(advice), CheckInput: false);
                }
            }
            else
            {
                var trans = original;
                var translated =
                    AppData.SaveFileMode // save mode
                    && IsValidString(original) // valid original
                    && SetTranslation(ref trans, translation)  // translation found
                                                               // && trans != translation // new translation not equal to previous in case when was already old translation but it was removed
                    ;

                if (translated)
                {
                    ParseData.Ret = true; // even if new translation is empty or equal, it must be writed in file because it was changed
                    translation = (trans == original) ? string.Empty : trans;
                    translated = !string.IsNullOrEmpty(translation); // check if result translation is not empty and reset translated
                }

                SetContext(contextLines, translated); // here is again check translation because if it is empty or equals original must be added untranslated tag

                var splittedContext = CheckContextAndSplit(translated, contextLines, advice, original, translation);

                ParseData.Line = GetNewTextBlockContent(translated, contextLines, advice, original, translation);

                if (_useContext && splittedContext.Length > 0)
                {
                    ParseData.Line += "\n\n" + splittedContext; // merge context lines
                }

                SaveModeAddLine(newline: "\n");//add endstring line
            }
        }

        bool _useContext = false;
        Dictionary<string, HashSet<string>> _contextSplitInfo;
        private string CheckContextAndSplit(bool translated, List<string> contextLines, string advice, string original, string translation)
        {
            // skip if there is no context info
            if (!_useContext /*|| _contextSplitInfo.Count == 0* already checked in preaction*/ || !_contextSplitInfo.ContainsKey(original) || contextLines.Count == 1/*dont need to split block where is one context line*/)
            {
                return "";
            }

            // check if context lines is exist in list
            var newContextLines = new List<string>(); // new list of context lines
            foreach (var line in new List<string>(contextLines)/*iterate copy of context lines list because it will be changed*/)
            {
                var line2check = line;
                AddToStandaloneContextList.CleanContext(ref line2check); // clean context line from ntranslated tag
                if (_contextSplitInfo[original].Contains(line2check))
                {
                    // split string's copy to standalone block with selected found context
                    contextLines.Remove(line);
                    newContextLines.Add(line);
                }
            }

            if (newContextLines.Count > 0)
            {
                return GetNewTextBlockContent(translated, newContextLines, advice, original, translation);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// advice info
        /// using in rpgm2
        /// </summary>
        /// <param name="advice"></param>
        /// <returns></returns>
        protected virtual string AdviceInfo(string advice)
        {
            return "";
        }

        /// <summary>
        /// check if block is invalid and must be skipped
        /// need only for open file mode
        /// default for wolf,rpgm3
        /// </summary>
        protected virtual bool IsOpenModeSkipTheBlock()
        {
            return false;
        }

        /// <summary>
        /// text block set
        /// default for wolf,rpgm3
        /// </summary>
        protected virtual string GetNewTextBlockContent(bool translated, List<string> contextLines, string advice, string original, string translation)
        {
            return
                                StringBlockBeginId()
                                + "\n" +
                                original
                                + "\n" +
                                string.Join("\n", contextLines)
                                + "\n" +
                                translation
                                + "\n" +
                                EndTranslationID()
                                ;
        }

        /// <summary>
        /// set context when with translated\untranslated tags
        /// need only for wolftrans and rpgmtrans patch v3
        /// </summary>
        /// <param name="contextLines"></param>
        /// <param name="translated"></param>
        protected virtual void SetContext(List<string> contextLines, bool translated)
        {
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
    }
}
