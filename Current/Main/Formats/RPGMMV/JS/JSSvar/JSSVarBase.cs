using System;
using Newtonsoft.Json;
using System.Text;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JS.JSSvar
{
    abstract class JSSVarBase : JSBase
    {

        bool StartReadingSvar;
        readonly StringBuilder Svar = new StringBuilder();

        protected JSSVarBase(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected abstract string SvarIdentifier { get; }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (StartReadingSvar)
            {
                if (ParseData.Line.TrimStart().StartsWith("};"))
                {
                    Svar.Append('}');

                    // NOTE: the same svar block is parsed twice below. The first call discards its
                    // result, the second one is the one actually used. Kept as-is on purpose:
                    // removing a call also removes the rows it adds in open mode (see changes.md).
                    try
                    {
                        JsonParser.ParseString(Svar.ToString(), this);
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug($"{GetType().Name}: svar pre-parse failed: {ex.Message}");
                    }

                    try
                    {
                        //SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                        var parseSuccess = JsonParser.ParseString(Svar.ToString(), this);

                        if (!ParseData.Ret)
                        {
                            ParseData.Ret = parseSuccess;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug($"{GetType().Name}: svar parse failed: {ex.Message}");
                    }

                    ParseData.ResultForWrite.AppendLine(JsonParser.JsonRoot.ToString(Formatting.Indented) + ";");

                    StartReadingSvar = false;

                    if (SaveFileMode)
                    {
                        return KeywordActionAfter.Continue;
                    }
                    else
                    {
                        return KeywordActionAfter.Break;
                    }

                }
                else
                {
                    Svar.AppendLine(ParseData.Line);
                }
            }
            else
            {
                //comments
                if (ParseData.IsComment)
                {
                    if (ParseData.Line.Contains("*/"))
                    {
                        ParseData.IsComment = false;
                    }
                    //continue;
                }
                else
                {
                    if (ParseData.Line.TrimStart().StartsWith("//"))
                    {
                        //continue;
                    }
                    else if (ParseData.Line.TrimStart().StartsWith("/*"))
                    {
                        if (!ParseData.Line.Contains("*/"))
                        {
                            ParseData.IsComment = true;
                            //continue;
                        }
                    }//endcomments
                    else if (ParseData.Line.TrimStart().StartsWith(SvarIdentifier))
                    {
                        StartReadingSvar = true;
                        if (SaveFileMode)
                        {
                            // add line with identifier without "{" of json block
                            ParseData.Line = ParseData.Line.Remove(ParseData.Line.Length - 1, 1);
                            SaveModeAddLine(newline: "\n");
                        }
                        Svar.AppendLine("{");
                    }
                }
            }

            if (!StartReadingSvar)
            {
                SaveModeAddLine(newline: "\n");
            }

            return KeywordActionAfter.Continue;
        }
    }
}
