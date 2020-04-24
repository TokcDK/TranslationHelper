using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.RPGMMV
{
    class PLUGINS_JS : FormatBase
    {
        public PLUGINS_JS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseJSArrayOfJsons();
        }

        private bool ParseJSArrayOfJsons()
        {
            string line;

            string tablename = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

            if (!thDataWork.THFilesElementsDataset.Tables.Contains(tablename))
            {
                thDataWork.THFilesElementsDataset.Tables.Add(tablename);
                thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add("Original");
                thDataWork.THFilesElementsDataset.Tables[tablename].Columns.Add("Translation");
            }
            if (!thDataWork.THFilesElementsDatasetInfo.Tables.Contains(tablename))
            {
                thDataWork.THFilesElementsDatasetInfo.Tables.Add(tablename);
                thDataWork.THFilesElementsDatasetInfo.Tables[tablename].Columns.Add("Info");
            }

            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (line.TrimStart().StartsWith("{\"name\":"))
                    {
                        try
                        {
                            if (line.EndsWith(","))
                            {
                                line = line.Remove(line.Length - 1, 1);
                            }

                            JToken root;
                            root = JToken.Parse(line);

                            ProceedJToken(root, tablename);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return true;
        }

        private void ProceedJToken(JToken token, string Jsonname)
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
                    ProceedJToken(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                var arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ProceedJToken(array[i], Jsonname);
                }
            }
            else
            {
            }
        }

        internal override bool Save()
        {
            return ParseJSArrayOfJsonsWrite();
        }

        StringBuilder TranslatedResult;
        private bool ParseJSArrayOfJsonsWrite()
        {
            TranslatedResult = new StringBuilder();
            string line;
            rowindex = 0;
            string tablename = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                bool isJsonNotLast = false;
                bool IsNotFirstLine = false;
                while (!reader.EndOfStream)
                {
                    if (IsNotFirstLine)
                    {
                        TranslatedResult.AppendLine();
                    }

                    line = reader.ReadLine();

                    if (line.TrimStart().StartsWith("{\"name\":"))
                    {
                        try
                        {
                            if (line.EndsWith(","))
                            {
                                isJsonNotLast = true;
                                line = line.Remove(line.Length - 1, 1);
                            }
                            else
                            {
                                isJsonNotLast = false;
                            }

                            JToken root;
                            root = JToken.Parse(line);

                            ProceedJTokenWrite(root, tablename);

                            line = root.ToString(Formatting.None) + (isJsonNotLast ? "," : string.Empty);
                        }
                        catch
                        {
                        }
                        isJsonNotLast = !isJsonNotLast;
                    }

                    IsNotFirstLine = true;
                    TranslatedResult.Append(line);
                }
            }

            File.WriteAllText(thDataWork.FilePath, TranslatedResult.ToString());
            return true;
        }

        int rowindex;
        private void ProceedJTokenWrite(JToken token, string Jsonname)
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
                    ProceedJTokenWrite(property.Value, Jsonname);
                }
            }
            else if (token is JArray array)
            {
                var arrayCount = array.Count;
                for (int i = 0; i < arrayCount; i++)
                {
                    ProceedJTokenWrite(array[i], Jsonname);
                }
            }
            else
            {
            }
        }

        private static bool IsValidToken(JToken token)
        {
            return token.Type == JTokenType.String && !string.IsNullOrWhiteSpace(token.ToString()) && !(Properties.Settings.Default.OnlineTranslationSourceLanguage == "Japanese jp" && FunctionsRomajiKana.SelectedRomajiAndOtherLocalePercentFromStringIsNotValid(token.ToString()));
        }
    }
}
