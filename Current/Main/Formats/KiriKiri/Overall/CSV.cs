﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.KiriKiri
{
    class CSV : FormatStringBase
    {
        public override string Extension => ".csv";

        public CSV(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override bool TryOpen()
        {
            return KiriKiriCSVOpen();
        }

        public bool KiriKiriCSVOpen()
        {
            FunctionsTable.SetTableAndColumns(FilePath);

            using (StreamReader file = new StreamReader(GetOpenFilePath(), Encoding.GetEncoding(932)))
            {
                string fileName = Path.GetFileName(GetOpenFilePath());
                string line;
                //string original = string.Empty;
                //_ = THFilesElementsDataset.Tables.Add(filename);
                //_ = THFilesElementsDataset.Tables[0].Columns.Add(THSettings.OriginalColumnName);
                //_ = THFilesElementsDatasetInfo.Tables.Add(filename);
                //_ = THFilesElementsDatasetInfo.Tables[0].Columns.Add(THSettings.OriginalColumnName);
                //_ = THFilesElementsDataset.Tables[0].Columns.Add(THSettings.TranslationColumnName);
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
                                    _ = ParentProject.FilesContent.Tables[fileName].Rows.Add(columns[name]);
                                    _ = ParentProject.FilesContentInfo.Tables[fileName].Rows.Add("name");
                                }
                            }
                            if (detail > -1)
                            {
                                if (string.IsNullOrEmpty(columns[detail]) || columns[detail].IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    _ = ParentProject.FilesContent.Tables[fileName].Rows.Add(columns[detail]);
                                    _ = ParentProject.FilesContentInfo.Tables[fileName].Rows.Add("detail");
                                }
                            }
                            if (type > -1)
                            {
                                if (string.IsNullOrEmpty(columns[type]) || columns[type].IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    _ = ParentProject.FilesContent.Tables[fileName].Rows.Add(columns[type]);
                                    _ = ParentProject.FilesContentInfo.Tables[fileName].Rows.Add("type");
                                }
                            }
                            if (field > -1)
                            {
                                if (string.IsNullOrEmpty(columns[field]) || columns[field].IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    _ = ParentProject.FilesContent.Tables[fileName].Rows.Add(columns[field]);
                                    _ = ParentProject.FilesContentInfo.Tables[fileName].Rows.Add("field");
                                }
                            }
                            if (comment > -1)
                            {
                                if (string.IsNullOrEmpty(columns[comment]) || columns[comment].IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    try
                                    {
                                        _ = ParentProject.FilesContent.Tables[fileName].Rows.Add(columns[comment]);
                                        _ = ParentProject.FilesContentInfo.Tables[fileName].Rows.Add("comment");
                                    }
                                    catch
                                    {
                                        MessageBox.Show("ProjectData.THFilesElementsDataset.Tables[fileName]=" + ParentProject.FilesContent.Tables[fileName] + "\r\ncolumns[comment]=" + columns[comment]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return FunctionsTable.SetTableAndColumns(FilePath, false);
        }

        protected override bool TrySave()
        {
            throw new NotImplementedException();
        }
    }
}
