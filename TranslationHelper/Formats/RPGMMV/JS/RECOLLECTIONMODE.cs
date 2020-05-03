using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class RECOLLECTIONMODE : JSBase
    {
        public RECOLLECTIONMODE(THDataWork thDataWork) : base(thDataWork)
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

            bool StartReadingRRMSvar = false;
            StringBuilder RRMSvar = new StringBuilder();
            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (!StartReadingRRMSvar && line.TrimStart().StartsWith("var rngd_recollection_mode_settings = {"))
                    {
                        StartReadingRRMSvar = true;
                        RRMSvar.AppendLine("{");
                    }
                    else if (StartReadingRRMSvar)
                    {
                        if (line.TrimStart().StartsWith("};"))
                        {
                            RRMSvar.Append("}");

                            try
                            {
                                JToken root;
                                root = JToken.Parse(RRMSvar.ToString());

                                GetStringsFromJToken(root, tablename);
                            }
                            catch
                            {
                            }

                            break;
                        }
                        else
                        {
                            RRMSvar.AppendLine(line);
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

            bool StartReadingRRMSvar = false;
            StringBuilder RRMSvar = new StringBuilder();
            using (StreamReader reader = new StreamReader(thDataWork.FilePath))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (!StartReadingRRMSvar && line.TrimStart().StartsWith("var rngd_recollection_mode_settings = {"))
                    {
                        StartReadingRRMSvar = true;
                        TranslatedResult.Append(line.Remove(line.Length - 1, 1));
                        RRMSvar.AppendLine("{");
                    }
                    else if (StartReadingRRMSvar)
                    {
                        if (line.TrimStart().StartsWith("};"))
                        {
                            RRMSvar.Append("}");

                            JToken root = JToken.Parse(RRMSvar.ToString());
                            try
                            {
                                SplitTableCellValuesToDictionaryLines(tablename);
                                if (TableLines != null && TableLines.Count > 0)
                                {
                                    WriteStringsToJTokenWithPreSplitlines(root, tablename);
                                }
                                else
                                {
                                    WriteStringsToJToken(root, tablename);
                                }
                            }
                            catch
                            {
                            }

                            StartReadingRRMSvar = false;
                            TranslatedResult.AppendLine(root.ToString(Formatting.Indented)+ ";");
                        }
                        else
                        {
                            RRMSvar.AppendLine(line);
                        }
                    }
                    else
                    {
                        TranslatedResult.AppendLine(line);
                    }
                }
            }

            try
            {
                File.WriteAllText(thDataWork.FilePath, TranslatedResult.ToString());
            }
            catch
            {
                return false;
            }
            return true;
        }

        internal override string JSName => "RecollectionMode.js";
    }
}
