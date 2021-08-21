using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.LiveMaker
{
    class LSBCSV : LiveMakerBase
    {
        public LSBCSV()
        {
        }

        internal override string Ext()
        {
            return ".csv";
        }

        protected override void ParseStringFileOpen()
        {
            //reading all file and split it by \r\n because streamreader.readline also split by \n
            ParseData.LinesArray = File.ReadAllText(ProjectData.FilePath, ParseStringFileEncoding()).Split(new[] { "\r\n" }, System.StringSplitOptions.None);
            ParseStringFileLines();
        }
        protected override void ParseStringFileLines()
        {
            var lcnt = 0;
            foreach (var line in ParseData.LinesArray)//iterate all lines
            {
                ParseData.Line = line;
                lcnt++;
                if(string.IsNullOrWhiteSpace(line) && lcnt == ParseData.LinesArray.Length-1)
                {
                    //skip last line because stringbuilder adding new line and lmlsb will fail if will be empty line on the end
                }
                else if (ParseStringFileLine() == ParseStringFileLineReturnState.Break)
                {
                    break;
                }
            }

            if (ProjectData.SaveFileMode && ParseData.Ret)
            {
                var m = Regex.Match(ParseData.ResultForWrite.ToString(), @"^([\s\S]+\r\n)\r\n$");
                if (m.Success)
                {
                    ParseData.ResultForWrite.Clear();
                    ParseData.ResultForWrite.Append(m.Result("$1"));
                }
            }
        }

        protected override ParseStringFileLineReturnState ParseStringFileLine()
        {
            if (ParseData.TrimmedLine.StartsWith("pylm"))
            {
                var dataarray = ParseData.Line.Split(',');
                //while (dataarray.Length < 5)
                //{
                //    ParseData.line = ParseData.line + "\n" + ParseData.reader.ReadLine();
                //    dataarray = ParseData.line.Split(',');
                //}
                var original = dataarray[3];
                if (IsValidString(original))
                {
                    var translation = dataarray[4];
                    if (ProjectData.OpenFileMode)
                    {
                        if (!string.IsNullOrEmpty(translation))
                        {
                            AddRowData(new string[] { original, translation }, "", true, false);
                        }
                        else
                        {
                            AddRowData(original, "", true, false);
                        }
                    }
                    else
                    {
                        AddTranslation(ref translation, original);
                        dataarray[4] = translation;
                        ParseData.Line = string.Join(",", dataarray);
                    }
                }
            }

            SaveModeAddLine();

            return 0;
        }

        protected override string TranslationMod(string translation)
        {
            return translation
                .Replace(",", "、") //csv using , for split elements and because it must not be in translation
                .Replace("\r\n", "\n")//for internal message using \n as newline instead of \r\n
                ;
        }
    }
}
