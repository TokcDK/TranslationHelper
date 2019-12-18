using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXTv3 : RPGMTransPatchBase
    {
        public TXTv3(THDataWork thData) : base(thData)
        {
        }

        internal override bool Open()
        {
            throw new NotImplementedException();
        }

        internal override bool Save()
        {
            try
            {
                StringBuilder buffer = new StringBuilder();

                int originalcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
                int translationcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
                int contextcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;
                int advicecolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Advice"].Ordinal;
                int statuscolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Status"].Ordinal;

                //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                buffer.AppendLine("> RPGMAKER TRANS PATCH FILE VERSION 3.2");// + Environment.NewLine);
                                                                             //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                for (int y = 0; y < thDataWork.THFilesElementsDataset.Tables["tablename"].Rows.Count; y++)
                {
                    buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                                                        //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                    buffer.AppendLine(thDataWork.THFilesElementsDataset.Tables["tablename"].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine);
                                                                                                                               //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                                                                                                                               //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                                                                                                                               //string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                    string[] CONTEXT = (thDataWork.THFilesElementsDataset.Tables["tablename"].Rows[y][contextcolumnindex] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                    //string str1 = string.Empty;
                    string TRANSLATION = thDataWork.THFilesElementsDataset.Tables["tablename"].Rows[y][translationcolumnindex] + string.Empty;
                    for (int g = 0; g < CONTEXT.Length; g++)
                    {
                        /*CONTEXT[g] = CONTEXT[g].Replace("\r", string.Empty);*///очистка от знака переноса, возникающего после разбития на строки по \n
                        if (CONTEXT.Length > 1)
                        {
                            buffer.AppendLine("> CONTEXT: " + CONTEXT[g]);// + Environment.NewLine);
                        }
                        else
                        {   //if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                            if (TRANSLATION.Length == 0) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                            {
                                buffer.AppendLine("> CONTEXT: " + CONTEXT[g] + " < UNTRANSLATED");// + Environment.NewLine);
                            }
                            else
                            {
                                buffer.AppendLine("> CONTEXT: " + CONTEXT[g]);// + Environment.NewLine);
                            }
                        }
                    }
                    //buffer += Environment.NewLine;
                    //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation + Environment.NewLine;
                    buffer.AppendLine(TRANSLATION);// + Environment.NewLine);
                    buffer.AppendLine("> END STRING" + Environment.NewLine);// + Environment.NewLine);

                    //progressBar.Value++;
                    //MessageBox.Show(progressBar.Value.ToString());
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
