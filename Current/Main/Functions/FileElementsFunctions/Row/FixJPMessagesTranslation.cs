﻿using System.Collections.Generic;
using System.Globalization;
using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class FixJpMessagesTranslation : RowBase
    {
        public FixJpMessagesTranslation()
        {
            if (SessionData == null)
            {
                SessionData = new Dictionary<string, string>
                {
                { "は倒れた！", " is down!" },
                { "を倒した！", " was down!" },
                { "は立ち上がった！", " gets up." },
                { "は毒にかかった！", " is poisoned!" },
                { "に毒をかけた！", " was poisoned!" },
                { "の毒が消えた！", "'s poison is gone!" },
                { "は暗闇に閉ざされた！", " is in the dark!" },
                { "を暗闇に閉ざした！", " sent to the dark!" },
                { "の暗闇が消えた！", "'s darkness is gone!" },
                { "は沈黙した！", " is silenced!" },
                { "を沈黙させた！", " was silenced!" },
                { "の沈黙が解けた！", "'s silence is gone!" },
                { "は激昂した！", " is enraged!" },
                { "を激昂させた！", " was enraged!" },
                { "は我に返った！", "'s rage is gone!" },
                { "は混乱した！", " is confused!" },
                { "を混乱させた！", " was confused!" },
                { "は魅了された！", " is fascinated!" },
                { "を魅了した！", " was fascinated!" },
                { "は眠った！", " is sleeping!" },
                { "を眠らせた！", " was put to sleep!" },
                { "は眠っている。", " is sleeping." },
                { "は目を覚ました！", " is awake!" }
                };
            }
        }

        protected override bool Apply(RowBaseRowData rowData)
        {
            if(!THSettings.SourceLanguageIsJapanese)
            {
                return false;
            }

            var strOriginal = rowData.Original;
            var strTranslation = rowData.Translation;

            if (string.IsNullOrWhiteSpace(strTranslation))
            {
                return false;
            }

            if (SessionData.ContainsKey(strOriginal))
            {
                rowData.Translation = SessionData[strOriginal];
            }
            else if ((strOriginal.StartsWith("は") || strOriginal.StartsWith("を")) && !strTranslation.StartsWith(" "))
            {
                rowData.Translation = " " + strTranslation.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + strTranslation.Substring(1);
            }
            else if (strOriginal.StartsWith("の") && !strTranslation.StartsWith("'s ") && !strTranslation.StartsWith(" "))
            {
                rowData.Translation = "'s " + strTranslation.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + strTranslation.Substring(1);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
