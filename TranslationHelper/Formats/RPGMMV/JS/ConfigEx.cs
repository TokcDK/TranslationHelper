//using System.Text.RegularExpressions;
//using TranslationHelper.Data;

//namespace TranslationHelper.Formats.RPGMMV.JS
//{
//    class ConfigEx : JSBase
//    {
//        public ConfigEx()
//        {
//        }

//        public override string JSName => "ConfigEx.js";


//        protected override KeywordActionAfter ParseStringFileLine()
//        {
//            if (ParseData.IsComment)
//            {
//                if (ParseData.Line.Contains("*/"))
//                {
//                    ParseData.IsComment = false;
//                }
//            }
//            else
//            {
//                if (ParseData.Line.Contains("/*"))
//                {
//                    ParseData.IsComment = true;
//                }

//                if (!ParseData.Line.TrimStart().StartsWith("//") && ParseData.Line.Contains("addCommand('"))
//                {
//                    string StringToAdd = Regex.Replace(ParseData.Line, @".*addCommand\(\'([^']*)\'\, \'.*\'\)\;.*", "$1");
//                    if (!string.IsNullOrWhiteSpace(StringToAdd))
//                    {
//                        if (ProjectData.SaveFileMode)
//                        {
//                            var trans = StringToAdd;
//                            SetTranslation(ref trans);

//                            ParseData.Line = ParseData.Line.Replace(StringToAdd, trans);
//                        }
//                        else
//                        {
//                            string StringForInfo = Regex.Replace(ParseData.Line, @"\.addCommand\('[^']+', '([^']+)'\);", "$2");
//                            AddRowData(StringToAdd, "addCommand\\" + StringForInfo, CheckInput: true);
//                            //ProjectData.THFilesElementsDataset.Tables[tablename].Rows.Add(StringToAdd);
//                            //ProjectData.THFilesElementsDatasetInfo.Tables[tablename].Rows.Add("addCommand\\" + StringForInfo);
//                        }
//                    }
//                }
//            }

//            SaveModeAddLine();

//            return KeywordActionAfter.Continue;
//        }
//    }
//}
