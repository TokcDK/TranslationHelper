using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXTv2 : RPGMTransPatchBase
    {
        public TXTv2(THDataWork thData, StringBuilder sBuffer, int tableIndex) : base(thData, sBuffer, tableIndex)
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
                int originalcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
                int translationcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
                int contextcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;
                int advicecolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Advice"].Ordinal;
                int statuscolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Status"].Ordinal;

                //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

                buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
                                                                             //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                for (int y = 0; y < thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows.Count; y++)
                {
                    string ADVICE = thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[y][advicecolumnindex] + string.Empty;
                    //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
                    if (unusednotfound && ADVICE.Contains("# UNUSED TRANSLATABLES"))
                    {
                        buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
                        ADVICE = ADVICE.Replace("# UNUSED TRANSLATABLES", string.Empty);
                        unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
                    }
                    buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;

                    string TRANSLATION = thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[y][translationcolumnindex] + string.Empty;
                    if (TRANSLATION.Length == 0)
                    {
                        buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
                    }

                    buffer.AppendLine("# CONTEXT : " + thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[y][contextcolumnindex]);// + Environment.NewLine;
                    if (ADVICE.Length == 0)
                    {
                        //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
                    }
                    else
                    {
                        buffer.AppendLine("# ADVICE : " + ADVICE);// + Environment.NewLine;
                    }

                    buffer.AppendLine(thDataWork.THFilesElementsDataset.Tables[TableIndex].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine;
                    buffer.AppendLine("# TRANSLATION ");// + Environment.NewLine;
                    buffer.AppendLine(TRANSLATION);// + Environment.NewLine;
                    buffer.AppendLine("# END STRING" + Environment.NewLine);// + Environment.NewLine;
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
