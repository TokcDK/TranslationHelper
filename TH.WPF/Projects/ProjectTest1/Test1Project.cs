using ProjectBase;

namespace ProjectTest1
{
    public class Test1Project : IProject
    {
        public string Title => "Test project 1";

        public bool IsValid(string selectedPath)
        {
            return true;
        }

        public bool Open(string selectedPath)
        {
            throw new NotImplementedException();
        }

        public bool Save(string selectedPath)
        {
            throw new NotImplementedException();
        }
    }
}