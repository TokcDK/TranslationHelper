using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JsonParser
{
    abstract class JsonParserBase
    {
        protected JsonParserBase()
        {
            if (ProjectData.CurrentProject != null)
            {
                Format = ProjectData.CurrentProject.CurrentFormat;

                if (!string.IsNullOrWhiteSpace(ProjectData.FilePath))
                {
                    JsonName = Format.UseTableNameWithoutExtension ? Path.GetFileNameWithoutExtension(ProjectData.FilePath) : Path.GetFileName(ProjectData.FilePath);
                }
            }

            Init();
        }

        /// <summary>
        /// Path to json file
        /// </summary>
        protected FileInfo Json;

        /// <summary>
        /// Using now format
        /// </summary>
        internal FormatBase Format;

        /// <summary>
        /// Name of json file
        /// </summary>
        internal string JsonName;

        /// <summary>
        /// Json tokens which must be skipped from parse
        /// </summary>
        protected HashSet<JToken> SkipJsonTokens = new HashSet<JToken>();

        /// <summary>
        /// Enables check for tokens to skip from SkipJsonTokens
        /// </summary>
        protected bool UseSkipJsonTokens;

        /// <summary>
        /// Parse selected <paramref name="json"/> and then write
        /// </summary>
        /// <param name="json"></param>
        internal bool ParseFile(FileInfo json)
        {
            Format = ProjectData.CurrentProject.CurrentFormat;
            return Load(json);
        }

        /// <summary>
        /// Parse selected <paramref name="jsonString"/>
        /// </summary>
        /// <param name="jsonString"></param>
        internal bool ParseString(string jsonString)
        {
            Format = ProjectData.CurrentProject.CurrentFormat;
            JsonRoot = JToken.Parse(jsonString);

            try
            {
                Parse(JsonRoot);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse selected <paramref name="json"/> using functions from the <paramref name="format"/> then write changes in default ProjectData.FilePath
        /// </summary>
        /// <param name="json"></param>
        internal bool ParseUsingFormat(FormatBase format)
        {
            Format = format;
            return Load(new FileInfo(ProjectData.FilePath));
        }

        /// <summary>
        /// Root json token
        /// </summary>
        internal JToken JsonRoot;

        private bool Load(FileInfo json)
        {
            if (!json.Exists || json.Length == 0)
            {
                return false;
            }

            try
            {

                Json = json;

                JsonName = Format.UseTableNameWithoutExtension ? Path.GetFileNameWithoutExtension(json.FullName) : json.Name;

                //Example from here, answer 1: https://stackoverflow.com/questions/39673815/how-to-recursively-populate-a-treeview-with-json-data
                using (StreamReader reader = new StreamReader(json.FullName))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        JsonRoot = JToken.Load(jsonReader);
                    }
                }

                Parse(JsonRoot);
            }
            catch (JsonReaderException ex)
            {
                ProjectData.AppLog.LogToFile("Error occured while json read (json is empty or corrupted): \r\n" + ex);
                return false;
            }
            catch (Exception ex)
            {
                ProjectData.AppLog.LogToFile("Error occured while json parse: \r\n" + ex);
                return false;
            }

            WriteJsonFileInSaveMode();

            return true;
        }

        protected virtual void Init()
        {
        }

        private void WriteJsonFileInSaveMode()
        {
            if (ProjectData.SaveFileMode)
            {
                File.WriteAllText(Json.FullName, ResultJson());
            }
        }

        protected virtual string ResultJson()
        {
            return JsonRoot.ToString(Formatting.Indented);
        }

        /// <summary>
        /// Determines to parse only string token types as values. True by default
        /// </summary>
        protected bool ParseOnlyStringValues = true;

        /// <summary>
        /// Parse <paramref name="jsonToken"/>.
        /// ParseValue parse only string values by default.
        /// </summary>
        /// <param name="jsonToken"></param>
        protected void Parse(JToken jsonToken)
        {
            if (jsonToken == null || (UseSkipJsonTokens && SkipJsonTokens.Contains(jsonToken)))
            {
                return;
            }

            switch (jsonToken)
            {
                case JValue value:
                    {
                        if (ParseOnlyStringValues && jsonToken.Type != JTokenType.String)
                        {
                            return;
                        }

                        ParseValue(value);

                        break;
                    }

                case JObject jsonObject:
                    {
                        ParseJsonObject(jsonObject);

                        break;
                    }

                case JArray jsonArray:
                    {
                        ParseJsonArray(jsonArray);

                        break;
                    }

                default:
                    break;
            }
        }

        /// <summary>
        /// Parse <paramref name="jsonValue"/>
        /// </summary>
        /// <param name="jsonValue"></param>
        protected virtual void ParseValue(JValue jsonValue)
        {
        }

        /// <summary>
        /// Parse <paramref name="jsonObject"/>
        /// </summary>
        /// <param name="jsonObject"></param>
        protected virtual void ParseJsonObject(JObject jsonObject)
        {
            ParseJsonObjectProperties(jsonObject);
        }

        /// <summary>
        /// parse properties of the <paramref name="jsonObject"/>
        /// </summary>
        /// <param name="jsonObject"></param>
        protected virtual void ParseJsonObjectProperties(JObject jsonObject)
        {
            foreach (var property in jsonObject.Properties())
            {
                if (ParseJsonObjectProperty(property) == JsonObjectPropertyState.Break)
                {
                    break;
                }
            }
        }

        protected enum JsonObjectPropertyState
        {
            Continue = 0,
            Break = 1
        }

        /// <summary>
        /// Parse json object propery
        /// </summary>
        /// <param name="jsonProperty"></param>
        protected virtual JsonObjectPropertyState ParseJsonObjectProperty(JProperty jsonProperty)
        {
            try
            {
                Parse(jsonProperty.Value);
            }
            catch { }

            return JsonObjectPropertyState.Continue;
        }

        /// <summary>
        /// Parse <paramref name="jsonArray"/>
        /// </summary>
        /// <param name="jsonArray"></param>
        protected virtual void ParseJsonArray(JArray jsonArray)
        {
            ParseJsonArrayElements(jsonArray);
        }

        /// <summary>
        /// Parse elements of <paramref name="jsonArray"/>
        /// </summary>
        /// <param name="jsonArray"></param>
        protected virtual void ParseJsonArrayElements(JArray jsonArray)
        {
            for (int i = 0; i < jsonArray.Count // using for as the array can be changed when write
                ; i++)
            {
                ParseJsonArrayElement(jsonArray[i]);
            }
        }

        /// <summary>
        /// Parse <paramref name="jsonArrayElement"/>
        /// </summary>
        /// <param name="jsonArrayElement"></param>
        protected virtual void ParseJsonArrayElement(JToken jsonArrayElement)
        {
            try
            {
                Parse(jsonArrayElement);
            }
            catch { }
        }
    }
}
