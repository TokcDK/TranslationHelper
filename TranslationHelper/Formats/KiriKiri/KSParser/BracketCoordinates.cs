using System.Collections.Generic;

namespace TranslationHelper.Formats.TyranoBuilder.Extracted
{
    public class BracketCoordinates
    {
        public BracketCoordinates(int openIndex)
        {
            OpenIndex = openIndex;
        }

        public int OpenIndex { get; }
        public int CloseIndex { get; set; }

        public List<BracketCoordinates> Parent { get; set; } = new List<BracketCoordinates>();
    }
}