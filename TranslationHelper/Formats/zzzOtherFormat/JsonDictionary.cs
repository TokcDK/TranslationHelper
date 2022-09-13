using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.zzzOtherFormat
{
    internal class JsonDictionary : FormatStringBase
    {
        public override string Ext => ".json";
        protected override void ParseStringFileLines()
        {
            var Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ParseData.Reader.ReadToEnd());
            var keys = new List<string>(Dict.Keys);
            foreach (var key in keys)
            {
                var value = key;

                if (AddRowData(ref value, existsTranslation: Dict[key]) && SaveFileMode)
                {
                    Dict[key] = Mod(key, value);
                }

                //if (ProjectData.SaveFileMode && Dict[key] == key) Dict[key] = "";
            }

            if (SaveFileMode) ParseData.ResultForWrite.Append(JsonConvert.SerializeObject(Dict, Formatting.Indented));
        }

        string Mod(string o, string t)
        {
            if (t[0] == '【' && t.EndsWith("】")) return t;

            string tempTrans = t;

            var match = Regex.Match(t, @"^[a-zA-Z0-9_-]+:(.+)$");
            if (match.Success)
            {
                return t;
                //tempTrans = match.Groups[1].Value;
            }

            foreach (var c in ENtoJPReplacementPairsOneLetterDict)
            {
                tempTrans = tempTrans.Replace(c.Key, c.Value);
            }

            //if (match.Success)
            //{
            //    tempTrans = t
            //        .Remove(match.Groups[1].Index, match.Groups[1].Length)
            //        .Insert(match.Groups[1].Index, tempTrans);
            //}

            return tempTrans.Replace("\\Ｎ","\\N");
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
                //{ '\'', ' ' },
                { '\'', 'ˈ' },
                //{ '”', ' ' },
                { '”', '”' },
                //{ '’', ' ' },
                { '’', 'ˈ' },
                { '{', ' ' },
                { '}', ' ' },
                { '[', '【' },
                { ']', '】' },
                { '(', '（' },
                { ')', '）' },
                { '#', ' ' },
                { '?', '？' },
                { '!', '！' },
                //{ '「', ' ' },
                //{ '『', ' ' },
                //{ '」', ' ' },
                //{ '』', ' ' },
                //{ '　', ' ' },
                //{ ' ', '_' }
                { ' ', '・' }
            };
    }

}
