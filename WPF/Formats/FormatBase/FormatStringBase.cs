namespace FormatBase
{
    /// <summary>
    /// base string file parser opens and writes the same file
    /// </summary>
    public abstract class FormatStringBase : IFormatString
    {
        public abstract string Extension { get; }

        public abstract string Description { get; }

        public virtual IEnumerable<(string original, string translaton, string info)> ExtractStrings(FileInfo sourceFileIfo)
        {
            if(!sourceFileIfo.Exists) yield break;

            foreach ((string original, string translaton, string info) stringData in EnumerateStrings(sourceFileIfo))
            {
                yield return stringData;
            }
        }

        protected virtual IEnumerable<(string original, string translaton, string info)> EnumerateStrings(FileInfo sourceFileIfo)
        {
            using var streamReader = new StreamReader(sourceFileIfo.FullName);

            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                foreach (var stringData in EnumerateStringsByLine(line, streamReader))
                {
                    yield return stringData;
                }
            }
        }

        protected virtual IEnumerable<(string original, string translaton, string info)> EnumerateStringsByLine(string? line, StreamReader streamReader) { yield break; }

        public bool WriteStrings(FileInfo? targetFileInfo, IEnumerable<(string original, string translaton, string info)> strings)
        {
            throw new NotImplementedException();
        }
    }
}
