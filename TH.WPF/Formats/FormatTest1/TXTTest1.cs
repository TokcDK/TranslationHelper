using FormatBase;

namespace FormatTest1
{
    public class TXTTest1 : IFormatString
    {
        public string Extension => ".txt";

        public string Description => "test format 1";

        public List<StringData> StringsList => new List<StringData>();

        public bool TryOpen(FileInfo Path)
        {
            throw new NotImplementedException();
        }

        public bool TrySave(FileInfo Path)
        {
            throw new NotImplementedException();
        }
    }
}