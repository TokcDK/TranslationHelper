using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Formats.KiriKiri
{
    class CSV : FormatBase
    {
        public CSV(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return KiriKiriCSVOpen();
        }

        public bool KiriKiriCSVOpen()
        {
            FunctionsTable.SetTableAndColumns(thDataWork);

            using (StreamReader file = new StreamReader(thDataWork.FilePath, Encoding.GetEncoding(932)))
            {
                string fileName = Path.GetFileName(thDataWork.FilePath);
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
                                    _ = thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(columns[name]);
                                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("name");
                                }
                            }
                            if (detail > -1)
                            {
                                if (string.IsNullOrEmpty(columns[detail]) || FunctionsString.IsDigitsOnly(columns[detail]))
                                {
                                }
                                else
                                {
                                    _ = thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(columns[detail]);
                                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("detail");
                                }
                            }
                            if (type > -1)
                            {
                                if (string.IsNullOrEmpty(columns[type]) || FunctionsString.IsDigitsOnly(columns[type]))
                                {
                                }
                                else
                                {
                                    _ = thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(columns[type]);
                                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("type");
                                }
                            }
                            if (field > -1)
                            {
                                if (string.IsNullOrEmpty(columns[field]) || FunctionsString.IsDigitsOnly(columns[field]))
                                {
                                }
                                else
                                {
                                    _ = thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(columns[field]);
                                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("field");
                                }
                            }
                            if (comment > -1)
                            {
                                if (string.IsNullOrEmpty(columns[comment]) || FunctionsString.IsDigitsOnly(columns[comment]))
                                {
                                }
                                else
                                {
                                    try
                                    {
                                        _ = thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(columns[comment]);
                                        _ = thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add("comment");
                                    }
                                    catch
                                    {
                                        MessageBox.Show("thDataWork.THFilesElementsDataset.Tables[fileName]="+ thDataWork.THFilesElementsDataset.Tables[fileName]+ "\r\ncolumns[comment]="+ columns[comment]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return FunctionsTable.SetTableAndColumns(thDataWork,false);
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
