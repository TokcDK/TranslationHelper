using System.Collections.Generic;
using System.Data;
using System.IO;

namespace TranslationHelper.Data
{
    public class ProjectFileInfo
    {
        public FileInfo File;

        public string Info = "";

        public FileContent Content;
    }

    public class FileContent : List<FileRow> { /* list of FileRow here*/ }

    public class FileRow
    {
        public DataRow Row;
        public string Info = "";
    }

    public static class TestData
    {
        public static ProjectFileInfo GetFile()
        {
            return new ProjectFileInfo()
            {
                File = new FileInfo(@"c:\file.txt"),
                Content = new FileContent()
            {
                new FileRow(){ Row = null}
            }
            };
        }
    }
}
