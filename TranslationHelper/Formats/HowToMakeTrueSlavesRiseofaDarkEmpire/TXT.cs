using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.HowToMakeTrueSlavesRiseofaDarkEmpire
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

            thDataWork.THFilesElementsDataset.Tables.Add(fileName).Columns.Add("Original");

            using (StreamReader sr = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
            {
                string line;
                bool readmode = false;
                int readmodelines = 0;
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    //commented or empty
                    if (!readmode && string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("//"))
                    {
                        continue;
                    }
                    else if (readmode)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(sb.ToString());
                            readmode = false;
                            readmodelines = 0;
                            sb.Clear();
                        }
                        else
                        {
                            if (readmodelines > 0)
                            {
                                sb.Append(Environment.NewLine);
                            }
                            sb.Append(line);
                            readmodelines++;
                        }
                        continue;
                    }

                    if (line.StartsWith("#MSG,"))
                    {
                        readmode = true;
                        continue;
                    }
                    else if (line.StartsWith("#MSGVOICE,"))
                    {
                        sr.ReadLine();
                        readmode = true;
                        continue;
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
