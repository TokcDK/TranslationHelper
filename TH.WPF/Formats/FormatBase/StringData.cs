namespace FormatBase
{
    public class StringData
    {
        public StringData(string original, int index)
        {
            Original = original;
            Index = index;
        }

        public int Index { get; set; }
        public string Original { get; }
        public string Translation { get; set; } = "";
    }
}