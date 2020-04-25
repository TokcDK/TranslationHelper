using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXTv3 : RPGMTransPatchBase
    {
        public TXTv3(THDataWork thData, StringBuilder sBuffer) : base(thData, sBuffer)
        {
        }

        internal override bool Open()
        {
            try
            {
                int invalidformat = 0;

                string _context = string.Empty;           //Комментарий
                string _advice = string.Empty;            //Предел длины строки
                string _string;// = string.Empty;            //Переменная строки
                string _original = string.Empty;           //Непереведенный текст
                string _translation = string.Empty;             //Переведенный текст
                int _status = 0;             //Статус

                using (StreamReader _file = new StreamReader(thDataWork.FilePath)) //Задаем файл
                {
                    string fname = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

                    AddTables(fname, new string[] { "Context", "Advice", "Status" });
                    var Table = thDataWork.THFilesElementsDataset.Tables[fname];
                    var TableInfo = thDataWork.THFilesElementsDatasetInfo.Tables[fname];

                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine(); //Чтение

                        //Код для версии патча 3
                        if (_string.StartsWith("> BEGIN STRING"))
                        {
                            invalidformat = 2; //если нашло строку
                            _string = _file.ReadLine();

                            int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной                            
                            while (!_string.StartsWith("> CONTEXT:"))  //Ждем начало следующего блока
                            {
                                if (untranslines > 0)
                                {
                                    _original += Environment.NewLine;
                                }
                                _original += _string;            //Пишем весь текст
                                _string = _file.ReadLine();
                                untranslines++;
                            }

                            int contextlines = 0;
                            while (_string.StartsWith("> CONTEXT:"))
                            {
                                if (contextlines > 0)
                                {
                                    _context += Environment.NewLine;
                                }

                                _context += _string.Replace("> CONTEXT: ", string.Empty).Replace(" < UNTRANSLATED", string.Empty);// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий

                                _string = _file.ReadLine();
                                contextlines++;
                            }

                            int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            while (!_string.StartsWith("> END"))      //Ждем конец блока
                            {
                                if (translines > 0)
                                {
                                    _translation += Environment.NewLine;
                                }
                                _translation += _string;
                                _string = _file.ReadLine();
                                translines++;
                            }

                            if (_original == Environment.NewLine)
                            {
                            }
                            else
                            {
                                Table.Rows.Add(_original, _translation, _context, _advice, _status);
                                if (TableInfo == null)
                                {
                                }
                                else
                                {
                                    TableInfo.Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
                                }
                            }

                            //Чистка
                            _context = string.Empty;
                            _original = string.Empty;
                            _translation = string.Empty;
                        }
                    }

                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        return false;
                    }

                    return CheckTablesContent(fname);
                }
            }
            catch
            {
                return false;
            }
        }

        internal override bool Save()
        {
            try
            {
                //StringBuilder buffer = new StringBuilder();

                int originalcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
                int translationcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
                int contextcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;

                string fname = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

                //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                buffer.AppendLine("> RPGMAKER TRANS PATCH FILE VERSION 3.2");// + Environment.NewLine);

                int TableRowsCount = thDataWork.THFilesElementsDataset.Tables[fname].Rows.Count;
                for (int y = 0; y < TableRowsCount; y++)
                {
                    var row = thDataWork.THFilesElementsDataset.Tables[fname].Rows[y];
                    buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);

                    buffer.AppendLine(row[originalcolumnindex] + string.Empty);// + Environment.NewLine);

                    string[] CONTEXT = (row[contextcolumnindex] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);

                    string TRANSLATION = row[translationcolumnindex] + string.Empty;
                    for (int g = 0; g < CONTEXT.Length; g++)
                    {

                        if (CONTEXT.Length > 1)
                        {
                            buffer.AppendLine("> CONTEXT: " + CONTEXT[g]);// + Environment.NewLine);
                        }
                        else
                        {
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

                    buffer.AppendLine(TRANSLATION);// + Environment.NewLine);
                    buffer.AppendLine("> END STRING" + Environment.NewLine);// + Environment.NewLine);
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
