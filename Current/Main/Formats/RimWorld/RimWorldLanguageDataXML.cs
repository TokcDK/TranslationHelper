using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TranslationHelper.Formats.RimWorld
{
    internal class RimWorldLanguageDataXML : FormatStringBase
    {
        public override string Extension => ".xml";

        protected override void ParseFileContent()
        {
            // Load the XML document from the file, preserving whitespace
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                xmlDoc.Load(fileStream);
            }

            // Retrieve "LanguageData" elements and validate presence
            var languageDataNodes = xmlDoc.GetElementsByTagName("LanguageData");
            if (languageDataNodes.Count == 0)
            {
                // No "LanguageData" element found; exit early
                return;
            }
            var languageDataNode = languageDataNodes[0]; // Process only the first "LanguageData" element

            // Iterate over child elements of "LanguageData"
            foreach (XmlElement element in languageDataNode.ChildNodes.OfType<XmlElement>())
            {
                // Skip elements with empty or whitespace-only content
                if (string.IsNullOrWhiteSpace(element.InnerText))
                {
                    continue;
                }

                var content = element.InnerText;
                var elementDescription = $"Element: \"{element.Name}\"";

                // Process content and update element if in save mode
                if (AddRowData(ref content, elementDescription) && SaveFileMode)
                {
                    element.InnerText = content;
                }
            }

            // Append the modified XML to the result if conditions are met
            if (SaveFileMode && ParseData.Ret)
            {
                ParseData.ResultForWrite.Append(xmlDoc.OuterXml);
            }
        }
    }
}
