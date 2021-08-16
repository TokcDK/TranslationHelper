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

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (StartReadingSvar)
            {
                if (ParseData.line.TrimStart().StartsWith("};"))
                {
                    Svar.Append('}');

                    try
                    {
                        JsonParser.ParseString(Svar.ToString());
                    }
                    catch
                    {
                    }

                    try
                    {
                        //SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                        var parseSuccess = JsonParser.ParseString(Svar.ToString());

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

                    if (ProjectData.SaveFileMode)
                    {
                        return ParseStringFileLineReturnState.Continue;
                    }
                    else
                    {
                        return ParseStringFileLineReturnState.Break;
                    }

                }
                else
                {
                    Svar.AppendLine(ParseData.line);
                }
            }
            else
            {
                //comments
                if (ParseData.IsComment)
                {
                    if (ParseData.line.Contains("*/"))
                    {
                        ParseData.IsComment = false;
                    }
                    //continue;
                }
                else
                {
                    if (ParseData.line.TrimStart().StartsWith("//"))
                    {
                        //continue;
                    }
                    else if (ParseData.line.TrimStart().StartsWith("/*"))
                    {
                        if (!ParseData.line.Contains("*/"))
                        {
                            ParseData.IsComment = true;
                            //continue;
                        }
                    }//endcomments
                    else if (ParseData.line.TrimStart().StartsWith(SvarIdentifier))
                    {
                        StartReadingSvar = true;
                        if (ProjectData.SaveFileMode)
                        {
                            // add line with identifier without "{" of json block
                            ParseData.line = ParseData.line.Remove(ParseData.line.Length - 1, 1);
                            SaveModeAddLine("\n");
                        }
                        Svar.AppendLine("{");
                    }
                }
            }

            if (!StartReadingSvar)
            {
                SaveModeAddLine("\n");
            }

            return ParseStringFileLineReturnState.Continue;
        }
    }
}
