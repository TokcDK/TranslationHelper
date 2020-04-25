using Newtonsoft.Json.Linq;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    abstract class JSBase : RPGMMVBase
    {
        public JSBase(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal abstract string JSName { get; }

        internal virtual string JSSubfolder => string.Empty;

        protected static bool IsValidToken(JToken token)
        {
            return token.Type == JTokenType.String && !string.IsNullOrWhiteSpace(token.ToString()) && !(Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(token.ToString()));
        }

        protected void GetStringsFromJToken(JToken token, string Jsonname)
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (!IsValidToken(token))
                    return;

                thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows.Add(token.ToString());
                thDataWork.THFilesElementsDatasetInfo.Tables[Jsonname].Rows.Add(token.Path);
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    GetStringsFromJToken(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                var arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    GetStringsFromJToken(array[i], Jsonname);
                }
            }
            else
            {
            }
        }

        protected int rowindex;
        protected void WriteStringsToJToken(JToken token, string Jsonname)
        {
            if (token == null)
            {
                return;
            }

            if (token is JValue)
            {
                if (!IsValidToken(token))
                    return;

                var row = thDataWork.THFilesElementsDataset.Tables[Jsonname].Rows[rowindex];
                if (Equals(token.ToString(), row[0]) && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                {
                    (token as JValue).Value = row[1] as string;
                }
                rowindex++;
            }
            else if (token is JObject obj)
            {
                //LogToFile("JObject Properties: \r\n" + obj.Properties());
                foreach (var property in obj.Properties())
                {
                    WriteStringsToJToken(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                var arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    WriteStringsToJToken(array[i], Jsonname);
                }
            }
            else
            {
            }
        }
    }
}
