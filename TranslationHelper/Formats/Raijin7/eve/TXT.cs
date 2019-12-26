using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (thDataWork.TempPath.Length == 0)
            {
                return false;
            }


            string fileName = Path.GetFileNameWithoutExtension(thDataWork.TempPath);
            thDataWork.THFilesElementsDataset.Tables.Add(fileName).Columns.Add("Original");
            using (StreamReader sr = new StreamReader(thDataWork.TempPath, Encoding.GetEncoding(932)))
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

            if (thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Count > 0)
            {
                thDataWork.THFilesElementsDataset.Tables[fileName].Columns.Add("Translation");
                return true;
            }
            else
            {
                thDataWork.THFilesElementsDataset.Tables.Remove(fileName);
                return false;
            }
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
