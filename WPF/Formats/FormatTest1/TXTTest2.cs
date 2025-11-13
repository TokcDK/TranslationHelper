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

                    if (LineParsingHelper.IsCommandOrComment(line)) continue;

                    lines.Add(new FileExtractedStringInfo(line!,"",""));
                }
            }

            return lines;
        }

        public Stream WriteStrings(List<FileExtractedStringInfo> fileExtractedStringInfos)
        {
            throw new NotImplementedException();
        }
    }
}