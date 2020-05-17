using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.KiriKiri
{
    class TJS : FormatBase
    {
        public TJS(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override bool Open()
        {
            return KiriKiriScenarioOpen();
        }

        private bool KiriKiriScenarioOpen()
        {
            try
            {
                using (StreamReader file = new StreamReader(thDataWork.SPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    //string original = string.Empty;
                    string filename = Path.GetFileNameWithoutExtension(thDataWork.SPath);
                    _ = thDataWork.THFilesElementsDataset.Tables.Add(filename);
                    _ = thDataWork.THFilesElementsDataset.Tables[0].Columns.Add("Original");
                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith(";") || line.StartsWith("@") || Equals(line, string.Empty))
                        {
                        }
                        else
                        {
                            if (line.EndsWith("[k]"))
                            {
                                thDataWork.THFilesElementsDataset.Tables[0].Rows.Add(line.Remove(line.Length - 3, 3));

                                //int i = 0;
                                //while (line.EndsWith("[k]"))
                                //{
                                //    if (i > 0)
                                //    {
                                //        original += Environment.NewLine;
                                //    }

                                //    original = original + line.Replace("[k]", string.Empty);

                                //    line = file.ReadLine();

                                //    if (line.EndsWith("[k]") && line.StartsWith("["))
                                //    {
                                //        THFilesElementsDataset.Tables[0].Rows.Add(original);
                                //        original = string.Empty;
                                //        i = 0;
                                //    }
                                //    else
                                //    {
                                //        i++;
                                //    }
                                //}
                                //if (original.Length > 0)
                                //{
                                //    THFilesElementsDataset.Tables[0].Rows.Add(original);
                                //    original = string.Empty;
                                //}
                            }
                        }
                    }

                    //if (thDataWork.THFilesElementsDataset.Tables[0].Rows.Count > 0)
                    //{
                    //    _ = thDataWork.THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                    //    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(filename)));
                    //}
                    //else
                    //{
                    //    /*THMsg*/MessageBox.Show(T._("Nothing to add"));
                    //    return string.Empty;
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return false;
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
