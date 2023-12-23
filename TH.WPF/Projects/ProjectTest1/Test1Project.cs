using FormatBase;
using FormatTest1;
using ProjectBase;

namespace ProjectTest1
{
    public class Test1Project : IProject
    {
        public string Title => "Test project 1";

        public List<FileData>? FilesList => new();

        public bool IsValid(string selectedPath)
        {
            return File.Exists(selectedPath) && string.Equals(Path.GetExtension(selectedPath), ".txt", StringComparison.InvariantCultureIgnoreCase);
        }

        TXTTest1? _f;
        public bool TryOpen(string selectedPath)
        {
            _f = new TXTTest1();
            var fileInfo = new FileInfo(selectedPath);
            var b = _f.TryOpen(fileInfo);
            if (b) FilesList!.Add(new FileData(fileInfo, _f));
            return b && _f.StringsList != null && _f.StringsList.Count > 0;
        }

        public bool TrySave(string selectedPath)
        {
            return _f != null && _f.TrySave(null);
        }
    }
}