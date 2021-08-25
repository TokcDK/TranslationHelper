//using GetListOfSubClasses;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;

//namespace TranslationHelper.Formats.KiriKiri.Games.KSSyntax
//{
//    class CSBySyntax : FormatBase
//    {
//        protected override void ParseStringFilePreOpenExtra()
//        {
//            base.ParseStringFilePreOpenExtra();

//            KSParts = Inherited.GetListOfinheritedSubClasses<KSSyntaxBase>();
//        }

//        List<KSSyntaxBase> KSParts;
//        protected override ParseStringFileLineReturnState ParseStringFileLine()
//        {
//            if (ParseData.TrimmedLine.StartsWith(";"))
//            {
//                return 0;
//            }

//            foreach (var ksPart in KSParts)
//            {
//                var startMatch = Regex.Match(ParseData.Line, ksPart.StartsWith);
//                if (!startMatch.Success)
//                {
//                    continue;
//                }

//                if (ksPart.Include()!=null)
//                {

//                }

//            }

//            return 0;
//        }
//    }
//}
