using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7.eve
{
    class TXT : FormatBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            if (thDataWork.FilePath.Length == 0)
            {
                return false;
            }


            string fileName = Path.GetFileNameWithoutExtension(thDataWork.FilePath);
            AddTables(fileName);
            using (StreamReader sr = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
            {
                string line;
                int lineNumber = 0;
                string Value;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    //commented or empty
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    {
                        continue;
                    }

                    if (line.TrimStart().StartsWith("set_run_msg")
                        || line.TrimStart().StartsWith("zin_old_msg")
                        || line.TrimStart().StartsWith("zin_reg_msg")
                        || line.TrimStart().StartsWith("zin_reg_event")
                        || line.TrimStart().StartsWith("zin_reg_qnar")
                        || line.TrimStart().StartsWith("zin_reg_title")
                        )
                    {
                        Value = line.Split(',')[1];
                        if (Value.Length > 0)
                        {
                            thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                        }
                    }
                    else if (line.TrimStart().StartsWith("get_item"))
                    {
                        Value = line.Split(',')[3];
                        if (Value.Length > 0)
                        {
                            thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                        }
                    }
                    else if (fileName.StartsWith("sn"))
                    {
                        if (lineNumber == 0)
                        {
                            Value = line.Split(',')[1];
                            if (Value.Length > 0)
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                            }
                        }
                        else
                        {
                            if (line.Length > 0)
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(line);
                            }
                        }
                    }
                    lineNumber++;
                }
            }

            return CheckTablesContent(fileName);
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
