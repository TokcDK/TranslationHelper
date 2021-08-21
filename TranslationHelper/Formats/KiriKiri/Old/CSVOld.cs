using System;
using System.Data;
using System.IO;
using System.Text;
using TranslationHelper.Extensions;

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
                //_ = THFilesElementsDataset.Tables[0].Columns.Add(THSettings.OriginalColumnName());
                //_ = THFilesElementsDatasetInfo.Tables.Add(filename);
                //_ = THFilesElementsDatasetInfo.Tables[0].Columns.Add(THSettings.OriginalColumnName());
                //_ = THFilesElementsDataset.Tables[0].Columns.Add(THSettings.TranslationColumnName());
                //THFilesList.Invoke((Action)(() => THFilesList.AddItem(filename)));
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

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
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
                                if (string.IsNullOrEmpty(columns[name]) || columns[name].IsDigitsOnly())
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
                                if (string.IsNullOrEmpty(columns[detail]) || columns[detail].IsDigitsOnly())
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
                                if (string.IsNullOrEmpty(columns[type]) || columns[type].IsDigitsOnly())
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
                                if (string.IsNullOrEmpty(columns[field]) || columns[field].IsDigitsOnly())
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
                                if (string.IsNullOrEmpty(columns[comment]) || columns[comment].IsDigitsOnly())
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
