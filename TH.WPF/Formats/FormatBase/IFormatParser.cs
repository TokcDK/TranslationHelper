using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatBase
{
    public class FileExtractedStringInfo
    {
        public FileExtractedStringInfo(string originalText, string? translationText, string? infoText)
        {
            OriginalText = originalText;
            TranslationText = translationText;
            InfoText = infoText;
        }

        public string OriginalText { get; }
        public string? TranslationText { get; set; }
        public string? InfoText { get; set; }
    }

    public interface IFormatParser
    {
        /// <summary>
        /// Extract strings from <paramref name="fileContent"/>
        /// </summary>
        /// <param name="fileContent">source of file</param>
        /// <returns></returns>
        List<FileExtractedStringInfo> ExtractStrings(Stream fileContent);

        Stream WriteStrings(List<FileExtractedStringInfo> fileExtractedStringInfos);
    }
}
