using FormatBase;
using FormatTest1;
using ProjectBase;

namespace ProjectTest1
{
    public class Test1Project : IProject
    {
        public string Title => "Test project 1";

        public List<FileData>? FilesList => null;

        public bool IsValid(FileInfo selectedFilePath)
        {
            return string.Equals(selectedFilePath.Extension, ".txt", StringComparison.InvariantCultureIgnoreCase);
        }

        TXTTest1? _f;
        public bool TryOpen(FileInfo selectedFilePath)
        {
            _f = new TXTTest1();
            var b = _f.TryOpen(selectedFilePath);
            return b && _f.StringsList != null && f.StringsList.Count > 0;
        }

        public bool TrySave()
        {
            return _f != null && _f.TrySave(null);
        }
    }
}