using FormatBase;
using ProjectBase;

namespace ProjectTest1
{
    public class Test1Project : IProject
    {
        public string Title => "Test project 1";

        public List<FileData>? Files => null;

        public bool IsValid(FileInfo selectedFilePath)
        {
            throw new NotImplementedException();
        }

        public bool TryOpen(FileInfo selectedFilePath)
        {
            throw new NotImplementedException();
        }

        public bool TrySave()
        {
            throw new NotImplementedException();
        }
    }
}