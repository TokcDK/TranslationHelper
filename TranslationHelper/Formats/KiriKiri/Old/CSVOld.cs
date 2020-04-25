using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    public static class CSVOld
    {
        public static DataTable KiriKiriCSVOpen(string sPath, DataTable DT, DataTable DTInfo)
        {
            if (DT == null || DTInfo == null)
                return null;

            using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
            {
                string line;
                //string original = string.Empty;
                //_ = THFilesElementsDataset.Tables.Add(filename);
                //_ = THFilesElementsDataset.Tables[0].Columns.Add("Original");
                //_ = THFilesElementsDatasetInfo.Tables.Add(filename);
                //_ = THFilesElementsDatasetInfo.Tables[0].Columns.Add("Original");
                //_ = THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                //THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                bool IsFirstLineWasNotRead = true;
                int name = -1;
                int detail = -1;
                int type = -1;
                int field = -1;
                int comment = -1;
                string[] columns;
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();

                    if (line.StartsWith(";") || string.IsNullOrWhiteSpace(line))
                    {
                    }
                    else
                    {
                        columns = line.Split(new string[] { "	" }, StringSplitOptions.None);
                        if (IsFirstLineWasNotRead)
                        {
                            for (int i = 0; i < columns.Length; i++)
                            {
                                if (columns[i] == "name")
                                {
                                    name = i;
                                }
                                else if (columns[i] == "detail")
                                {
                                    detail = i;
                                }
                                else if (columns[i] == "type")
                                {
                                    type = i;
                                }
                                else if (columns[i] == "field")
                                {
                                    field = i;
                                }
                                else if (columns[i] == "comment")
                                {
                                    comment = i;
                                }
                            }
                            IsFirstLineWasNotRead = false;
                        }
                        else
                        {
                            if (name > -1)
                            {
                                if (string.IsNullOrEmpty(columns[name]) || FunctionsString.IsDigitsOnly(columns[name]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[name]);
                                    _ = DTInfo.Rows.Add("name");
                                }
                            }
                            if (detail > -1)
                            {
                                if (string.IsNullOrEmpty(columns[detail]) || FunctionsString.IsDigitsOnly(columns[detail]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[detail]);
                                    _ = DTInfo.Rows.Add("detail");
                                }
                            }
                            if (type > -1)
                            {
                                if (string.IsNullOrEmpty(columns[type]) || FunctionsString.IsDigitsOnly(columns[type]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[type]);
                                    _ = DTInfo.Rows.Add("type");
                                }
                            }
                            if (field > -1)
                            {
                                if (string.IsNullOrEmpty(columns[field]) || FunctionsString.IsDigitsOnly(columns[field]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[field]);
                                    _ = DTInfo.Rows.Add("field");
                                }
                            }
                            if (comment > -1)
                            {
                                if (string.IsNullOrEmpty(columns[comment]) || FunctionsString.IsDigitsOnly(columns[comment]))
                                {
                                }
                                else
                                {
                                    _ = DT.Rows.Add(columns[comment]);
                                    _ = DTInfo.Rows.Add("comment");
                                }
                            }
                        }
                    }
                }
            }

            return DT;
        }
    }
}
