using FormatBase;

namespace FormatTest1
{
    public class TXTTest1 : IFormatString
    {
        public string Extension => ".txt";

        public string Description => "test format 1";

        public List<StringData> StringsList => new List<StringData>();

        public bool Open(FileInfo Path)
        {
            throw new NotImplementedException();
        }

        public bool Save(FileInfo Path)
        {
            throw new NotImplementedException();
        }
    }
}