using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RimWorld
{
    internal class RimWorldLanguageDataXML : FormatStringBase
    {
        public override string Extension => ".xml";

        protected override void ParseFileContent()
        {
            XmlDocument xmldoc = new XmlDocument();
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            var languageDataNode = xmldoc.GetElementsByTagName("LanguageData");
            foreach (XmlNode subnode in languageDataNode[0].ChildNodes)
            {
                if (!(subnode is XmlElement el)) continue;

                var s = el.InnerText;

                if (AddRowData(ref s, el.Name) && SaveFileMode)
                {
                    el.InnerText = s;
                }
            }

            if(SaveFileMode && ParseData.Ret) ParseData.ResultForWrite.Append(xmldoc.Value);
        }
    }
}
