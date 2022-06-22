using Newtonsoft.Json;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS.JSSvar
{
    abstract class JSSVarBase : JSBase
    {

        bool StartReadingSvar;
        StringBuilder Svar = new StringBuilder();

        protected abstract string SvarIdentifier { get; }

        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (StartReadingSvar)
            {
                if (ParseData.Line.TrimStart().StartsWith("};"))
                {
                    Svar.Append('}');

                    try
                    {
                        JsonParser.ParseString(Svar.ToString(), this);
                    }
                    catch
                    {
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
                    catch
                    {
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
