using Newtonsoft.Json;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS.JSSvar
{
    abstract class JssVarBase : JsBase
    {

        bool _startReadingSvar;
        StringBuilder _svar = new StringBuilder();

        protected abstract string SvarIdentifier { get; }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (_startReadingSvar)
            {
                if (ParseData.Line.TrimStart().StartsWith("};"))
                {
                    _svar.Append('}');

                    try
                    {
                        JsonParser.ParseString(_svar.ToString());
                    }
                    catch
                    {
                    }

                    try
                    {
                        //SplitTableCellValuesAndTheirLinesToDictionary(tablename, false, false);
                        var parseSuccess = JsonParser.ParseString(_svar.ToString());

                        if (!ParseData.Ret)
                        {
                            ParseData.Ret = parseSuccess;
                        }
                    }
                    catch
                    {
                    }

                    ParseData.ResultForWrite.AppendLine(JsonParser.JsonRoot.ToString(Formatting.Indented) + ";");

                    _startReadingSvar = false;

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
                    _svar.AppendLine(ParseData.Line);
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
                        _startReadingSvar = true;
                        if (ProjectData.SaveFileMode)
                        {
                            // add line with identifier without "{" of json block
                            ParseData.Line = ParseData.Line.Remove(ParseData.Line.Length - 1, 1);
                            SaveModeAddLine("\n");
                        }
                        _svar.AppendLine("{");
                    }
                }
            }

            if (!_startReadingSvar)
            {
                SaveModeAddLine("\n");
            }

            return ParseStringFileLineReturnState.Continue;
        }
    }
}
