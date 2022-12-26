namespace FormatBase
{
    public class StringData
    {
        public StringData(string original)
        {
            Original = original;
        }

        public string Original { get; }
        public string Translation { get; set; } = "";
    }
}