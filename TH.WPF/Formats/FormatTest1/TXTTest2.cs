using FormatBase;

namespace FormatTest1
{
    public class TXTTest2 : IFormatParser
    {
        public string[] ValidExtensions => new[] { ".txt" };

        public string ShortDescription => "test format 1";

        public List<FileExtractedStringInfo> ExtractStrings(Stream fileContent)
        {
            var lines = new List<FileExtractedStringInfo>();
            using(StreamReader reader = new StreamReader(fileContent))
            {
                string? line;
                while (reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line!.StartsWith('@')) continue; // command
                    if (line.StartsWith('#')) continue; // comment

                    lines.Add(new FileExtractedStringInfo(line,"",""));
                }
            }

            return lines;
        }

        public Stream? WriteStrings(List<FileExtractedStringInfo> fileExtractedStringInfos, Stream? fileContent = null)
        {
            // when writing in target file need first read the file
            if(fileContent== null) { return null; }

            using (StreamReader reader = new StreamReader(fileContent))
            using (StreamWriter writer = new StreamWriter(fileContent))
            {
                string? line;
                while (reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line!.StartsWith('@')|| line.StartsWith('#'))
                    {
                        writer.WriteLine(line);
                    }

                    writer.WriteLine(line);
                }
            }

            return fileContent;
        }
    }
}