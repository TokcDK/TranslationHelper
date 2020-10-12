using System;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXT : RPGMTransPatchBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Ext()
        {
            return ".txt";
        }

        protected override void ParseStringFilePreOpenExtra()
        {
            unused = false;
            verchecked = false;
        }

        bool verchecked;
        bool unused;
        int patchversion;
        protected override int ParseStringFileLine()
        {
            if (!verchecked)
            {
                if (ParseData.line == "> RPGMAKER TRANS PATCH FILE VERSION 3.2")
                {
                    patchversion = 3;
                }
                else if (ParseData.line == "# RPGMAKER TRANS PATCH FILE VERSION 2.0")
                {
                    patchversion = 2;
                }
                else
                {
                    return -1; //Not a patch file, break parsing
                }

                verchecked = true;
            }

            var ret = 1;

            if (patchversion == 3)
            {
                //patch version 3 code
                if (ParseData.line.StartsWith("> BEGIN STRING"))
                {
                    var originalLines = new List<string>();
                    ReadLine();
                    do
                    {
                        originalLines.Add(ParseData.line);
                    }
                    while (!ReadLine().StartsWith("> CONTEXT:"));

                    var contextlines = new List<string>();
                    do
                    {
                        contextlines.Add(ParseData.line);
                    }
                    while (ReadLine().StartsWith("> CONTEXT:"));

                    var translatedLines = new List<string>();
                    do
                    {
                        translatedLines.Add(ParseData.line);
                    }
                    while (ReadLine() != "> END STRING");

                    var original = originalLines.Joined();
                    var translation = translatedLines.Joined();
                    var context = contextlines.Joined();
                    if (thDataWork.OpenFileMode)
                    {
                        AddRowData(new[] { original, translation }, context, true);
                    }
                    else
                    {
                        var translated = original!= translation && !string.IsNullOrEmpty(translation);
                        if (IsValidString(original) && thDataWork.TablesLinesDict.ContainsKey(original) && translation != thDataWork.TablesLinesDict[original])
                        {
                            translated = true;
                            ParseData.Ret = true;
                            translation = thDataWork.TablesLinesDict[original];
                        }

                        //remove or add untranslated tag when translation was changed
                        if (translated)
                        {
                            context = context.Replace(" < UNTRANSLATED", string.Empty);
                        }
                        else if (!context.Contains(" < UNTRANSLATED"))
                        {
                            for (int i = 0; i < contextlines.Count; i++)
                            {
                                if (!contextlines[i].EndsWith(" < UNTRANSLATED"))
                                {
                                    contextlines[i] = contextlines[i] + " < UNTRANSLATED";
                                }

                            }
                        }

                        ParseData.line =
                            "> BEGIN STRING"
                            + Environment.NewLine
                            + original
                            + Environment.NewLine
                            + context
                            + Environment.NewLine
                            + translation
                            + Environment.NewLine
                            + "> END STRING"
                            ;
                    }
                }
            }
            else //patch version 2 code
            {
                if (!unused && ParseData.line == "# UNUSED TRANSLATABLES")//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
                {
                    unused = true;
                }

                if (ParseData.line.StartsWith("# TEXT STRING"))
                {
                    var untranslated = false;
                    while (!ReadLine().StartsWith("# CONTEXT"))
                    {
                        if (ParseData.line.StartsWith("# UNTRANSLATED"))
                        {
                            untranslated = true;
                        }
                    }

                    if (ParseData.line.Split(' ')[3] != "itemAttr/UsageMessage")
                    {
                        var context = ParseData.line; //контекст

                        ReadLine();

                        var advice = "";
                        //asdf advice Иногда advice отсутствует, например когда "# CONTEXT : Dialogue/SetHeroName" в патче VH
                        if (ParseData.line.StartsWith("# ADVICE"))//совет обычно содержит инфу о макс длине строки
                        {
                            advice = ParseData.line;
                            ReadLine();//читать след. строку, когда был совет
                        }

                        if (unused)
                        {
                            advice += Environment.NewLine + "UNUSED";//информа о начале блока неиспользуемых строк
                        }

                        var originalLines = new List<string>();
                        do
                        {
                            originalLines.Add(ParseData.line);
                        }
                        while (!ReadLine().StartsWith("# TRANSLATION"));//read while translatin block will start

                        var translatedLines = new List<string>();
                        do
                        {

                        }
                        while (!ReadLine().StartsWith("# END STRING"));

                        string original = originalLines.Joined();
                        string translation = translatedLines.Joined();
                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(new[] { original, translation }, context + Environment.NewLine + advice, true);
                        }
                        else
                        {
                            var translated = false;
                            if (IsValidString(original) && thDataWork.TablesLinesDict.ContainsKey(original) && translation != thDataWork.TablesLinesDict[original])
                            {
                                translated = true;
                                ParseData.Ret = true;
                                translation = thDataWork.TablesLinesDict[original];
                            }

                            ParseData.line =
                                "# TEXT STRING"
                                + Environment.NewLine
                                + (!translated ? "# UNTRANSLATED" + Environment.NewLine : string.Empty)
                                + context
                                + Environment.NewLine
                                + advice
                                + Environment.NewLine
                                + original
                                + Environment.NewLine
                                + "# TRANSLATION"
                                + Environment.NewLine
                                + translation
                                + Environment.NewLine
                                + "# END STRING";
                        }
                    }
                    else
                    {
                        ParseData.line =
                            "# TEXT STRING"
                            + Environment.NewLine
                            + (untranslated ? "# UNTRANSLATED" + Environment.NewLine : string.Empty)
                            + ParseData.line;
                    }
                }
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

            return ret;
        }
    }
}
