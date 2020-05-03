using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PLUGINS : JSBase
    {
        public PLUGINS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return ParseJSArrayOfJsons();
        }

        private bool ParseJSArrayOfJsons()
        {
            string line;

            string tablename = Path.GetFileName(thDataWork.FilePath);

            AddTables(tablename);

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

                            GetStringsFromJToken(root, tablename);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return CheckTablesContent(tablename);
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
            string tablename = Path.GetFileName(thDataWork.FilePath);

            thDataWork.Main.ProgressInfo(true, T._("Writing") + ": " + "plugins.js");

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

                            SplitTableCellValuesToDictionaryLines(tablename);
                            if (TableLines != null && TableLines.Count > 0)
                            {
                                WriteStringsToJTokenWithPreSplitlines(root, tablename);
                            }
                            else
                            {
                                WriteStringsToJToken(root, tablename);
                            }

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

        internal override string JSName => "plugins.js";
        internal override string JSSubfolder => string.Empty;
    }
}
