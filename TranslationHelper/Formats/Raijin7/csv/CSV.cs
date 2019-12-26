using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.Raijin7
{
    class CSV : FormatBase
    {
        public CSV(THDataWork thDataWork) : base(thDataWork)
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
                int variant = 0;

                if (fileName == "fship" || fileName.StartsWith("pnet") || fileName.StartsWith("pson") || fileName == "wapon")
                {
                    variant = 1;
                }
                else if (fileName == "item")
                {
                    variant = 2;
                }
                else if (fileName == "spec_rate")
                {
                    variant = 3;
                }

                if (variant > 0)
                {
                    string line;
                    string Value;
                    int lineNumber = 0;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        //commented or empty
                        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                        {
                            continue;
                        }

                        if (variant == 1)
                        {
                            Value = line.Split(',')[1];
                            if (Value.Length > 0)
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                            }
                        }
                        else if (variant == 2)
                        {
                            Value = line.Split(',')[0];
                            if (Value.Length > 0)
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                            }

                            Value = line.Split(',')[1];
                            if (Value.Length > 0)
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                            }
                        }
                        else if (variant == 3)
                        {
                            if (lineNumber > 0)
                            {
                                Value = line.Split(',')[0];
                                if (Value.Length > 0)
                                {
                                    thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(Value);
                                }
                            }
                            else
                            {
                                foreach (var sValue in line.Split(','))
                                {
                                    if (sValue.Length > 0)
                                    {
                                        thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(sValue);
                                    }
                                }
                            }
                        }
                        lineNumber++;
                    }
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
