using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatBase
{
    public class FileExtractedStringInfo
    {
        /// <summary>
        /// Data of extracted string by the <seealso cref="IFormatParser"/>
        /// </summary>
        /// <param name="originalText">Text of original</param>
        /// <param name="translationText">Optional  text of translation</param>
        /// <param name="infoText">Optional text of useful information of the string</param>
        public FileExtractedStringInfo(string originalText, string? translationText, string? infoText)
        {
            OriginalText = originalText;
            TranslationText = translationText;
            InfoText = infoText;
        }

        /// <summary>
        /// String original text
        /// </summary>
        public string OriginalText { get; }

        /// <summary>
        /// Text of the original translation
        /// </summary>
        public string? TranslationText { get; set; }

        /// <summary>
        /// Info text for the extracted string
        /// </summary>
        public string? InfoText { get; set; }
    }

    public interface IFormatParser
    {
        /// <summary>
        /// Valid file extensions which can be parsed by the format
        /// </summary>
        string[] ValidExtensions { get; }

        /// <summary>
        /// Short description of te file format
        /// </summary>
        string ShortDescription { get; }

        /// <summary>
        /// Extract strings from <paramref name="fileContent"/>
        /// </summary>
        /// <param name="fileContent">source of file</param>
        /// <returns></returns>
        List<FileExtractedStringInfo> ExtractStrings(Stream fileContent);

        /// <summary>
        /// Write strings back into file
        /// </summary>
        /// <param name="fileExtractedStringInfos"></param>
        /// <returns></returns>
        Stream WriteStrings(List<FileExtractedStringInfo> fileExtractedStringInfos);
    }
}
