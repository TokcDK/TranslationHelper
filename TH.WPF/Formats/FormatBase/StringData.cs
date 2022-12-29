namespace FormatBase
{
    public class StringData
    {
        public StringData(string original, string info = "")
        {
            Original = original;
        }

        public string Original { get; }
        public string Translation { get; set; } = "";
        public string Info { get; set; } = "";
    }
}