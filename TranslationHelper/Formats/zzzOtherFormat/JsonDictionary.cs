using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TranslationHelper.Data;
using TranslationHelper.Formats.IrisField;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.zzzOtherFormat
{
    internal class JsonDictionary:FormatStringBase
    {
        internal override string Ext()
        {
            return ".json";
        }
        protected override void ParseStringFileLines()
        {
            var Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ParseData.Reader.ReadToEnd());
            var keys = new List<string>(Dict.Keys);
            foreach (var key in keys)
            {
                var value = key;

                if (AddRowData(ref value, existsTranslation: Dict[key]) && ProjectData.SaveFileMode)
                {
                    Dict[key] = value;
                }

                if (ProjectData.SaveFileMode && Dict[key] == key) Dict[key] = "";
            }

            if (ProjectData.SaveFileMode) ParseData.ResultForWrite.Append(JsonConvert.SerializeObject(Dict, Formatting.Indented));
        }

        string Mod(string s)
        {
            foreach(var c in ENtoJPReplacementPairsOneLetterDict)
            {
                s = s.Replace(c.Key, c.Value);
            }

            return s;
        }

        readonly static Dictionary<char, char> ENtoJPReplacementPairsOneLetterDict = new Dictionary<char, char>
            {
                { 'a', 'ａ' },
                { 'A', 'Ａ' },
                { 'b', 'ｂ' },
                { 'B', 'Ｂ' },
                { 'c', 'ｃ' },
                { 'C', 'Ｃ' },
                { 'd', 'ｄ' },
                { 'D', 'Ｄ' },
                { 'e', 'ｅ' },
                { 'E', 'Ｅ' },
                { 'f', 'ｆ' },
                { 'F', 'Ｆ' },
                { 'g', 'ｇ' },
                { 'G', 'Ｇ' },
                { 'h', 'ｈ' },
                { 'H', 'Ｈ' },
                { 'i', 'ｉ' },
                { 'I', 'Ｉ' },
                { 'j', 'ｊ' },
                { 'J', 'Ｊ' },
                { 'k', 'ｋ' },
                { 'K', 'Ｋ' },
                { 'l', 'ｌ' },
                { 'L', 'Ｌ' },
                { 'm', 'ｍ' },
                { 'M', 'Ｍ' },
                { 'n', 'ｎ' },
                { 'N', 'Ｎ' },
                { 'o', 'ｏ' },
                { 'O', 'Ｏ' },
                { 'p', 'ｐ' },
                { 'P', 'Ｐ' },
                { 'q', 'ｑ' },
                { 'Q', 'Ｑ' },
                { 'r', 'ｒ' },
                { 'R', 'Ｒ' },
                { 's', 'ｓ' },
                { 'S', 'Ｓ' },
                { 't', 'ｔ' },
                { 'T', 'Ｔ' },
                { 'u', 'ｕ' },
                { 'U', 'Ｕ' },
                { 'v', 'ｖ' },
                { 'V', 'Ｖ' },
                { 'w', 'ｗ' },
                { 'W', 'Ｗ' },
                { 'x', 'ｘ' },
                { 'X', 'Ｘ' },
                { 'y', 'ｙ' },
                { 'Y', 'Ｙ' },
                { 'z', 'ｚ' },
                { 'Z', 'Ｚ' },
                { '0', '０' },
                { '1', '１' },
                { '2', '２' },
                { '3', '３' },
                { '4', '４' },
                { '5', '５' },
                { '6', '６' },
                { '7', '７' },
                { '8', '８' },
                { '9', '９' },
                { ',', '、' },
                { '.', '。' },
                { '\'', ' ' },
                { '”', ' ' },
                { '’', ' ' },
                { '{', ' ' },
                { '}', ' ' },
                { '[', ' ' },
                { ']', ' ' },
                { '(', '（' },
                { ')', '）' },
                { '#', ' ' },
                //{ '「', ' ' },
                //{ '『', ' ' },
                //{ '」', ' ' },
                //{ '』', ' ' },
                //{ '　', ' ' },
                //{ ' ', '_' }
            };
    }

}
