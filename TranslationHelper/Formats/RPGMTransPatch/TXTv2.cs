using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXTv2 : RPGMTransPatchBase
    {
        public TXTv2(THDataWork thData, StringBuilder sBuffer) : base(thData, sBuffer)
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

                    _ = thDataWork.THFilesElementsDataset.Tables.Add(fname);
                    _ = thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Original");
                    _ = thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Translation");
                    _ = thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Context");
                    _ = thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Advice");
                    _ = thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Status");

                    _ = thDataWork.THFilesElementsDatasetInfo.Tables.Add(fname);
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[fname].Columns.Add("Original");

                    string UNUSED = string.Empty;
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        if (Equals(_string, "# UNUSED TRANSLATABLES"))//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
                        {
                            //MessageBox.Show(_string);
                            UNUSED = _string;
                        }
                        //Код для версии патча 2.0
                        if (_string.StartsWith("# CONTEXT"))               //Ждем начало блока
                        {
                            invalidformat = 2;//строка найдена, формат верен

                            if (_string.Split(' ')[3] != "itemAttr/UsageMessage")
                            {
                                _context = _string.Replace("# CONTEXT : ", string.Empty); //Сохраняем коментарий

                                _string = _file.ReadLine();

                                //asdf advice Иногда advice отсутствует, например когда "# CONTEXT : Dialogue/SetHeroName" в патче VH
                                if (_string.StartsWith("# ADVICE"))
                                {
                                    _advice = _string.Replace("# ADVICE : ", string.Empty);   //Вытаскиваем число предела
                                    _string = _file.ReadLine();
                                }
                                else
                                {
                                    _advice = string.Empty;
                                }

                                if (UNUSED.Length == 0)
                                {
                                }
                                else
                                {
                                    _advice += UNUSED;//добавление информации о начале блока неиспользуемых строк
                                    UNUSED = string.Empty;//очистка переменной в целях оптимизации, чтобы не писать во все ADVICE
                                }

                                int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                                while (!_string.StartsWith("# TRANSLATION"))  //Ждем начало следующего блока
                                {
                                    if (untranslines > 0)
                                    {
                                        _original += Environment.NewLine;
                                    }
                                    _original += _string;            //Пишем весь текст
                                    _string = _file.ReadLine();
                                    untranslines++;
                                }
                                if (_original.Length > 0)                    //Если текст есть, ищем перевод
                                {
                                    _string = _file.ReadLine();
                                    int _translationlinescount = 0;
                                    while (!_string.StartsWith("# END"))      //Ждем конец блока
                                    {
                                        if (_translationlinescount > 0)
                                        {
                                            _translation += Environment.NewLine;
                                        }
                                        _translation += _string;
                                        _string = _file.ReadLine();
                                        _translationlinescount++;
                                    }
                                    if (_original != Environment.NewLine)
                                    {
                                        //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
                                        _ = thDataWork.THFilesElementsDataset.Tables[fname].Rows.Add(_original, _translation, _context, _advice, _status);
                                        if (thDataWork.THFilesElementsDatasetInfo == null)
                                        {
                                        }
                                        else
                                        {
                                            _ = thDataWork.THFilesElementsDatasetInfo.Tables[fname].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
                                        }
                                    }
                                }
                                _original = string.Empty;  //Чистим
                                _translation = string.Empty;    //Чистим
                            }
                        }
                    }

                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        return false; ;
                    }

                    if (thDataWork.THFilesElementsDataset.Tables[fname].Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        thDataWork.THFilesElementsDataset.Tables.Remove(fname);
                        thDataWork.THFilesElementsDatasetInfo.Tables.Remove(fname);
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
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

                string fname = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

                //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

                buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
                                                                             //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                for (int y = 0; y < thDataWork.THFilesElementsDataset.Tables[fname].Rows.Count; y++)
                {
                    string ADVICE = thDataWork.THFilesElementsDataset.Tables[fname].Rows[y][advicecolumnindex] + string.Empty;
                    //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
                    if (unusednotfound && ADVICE.Contains("# UNUSED TRANSLATABLES"))
                    {
                        buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
                        ADVICE = ADVICE.Replace("# UNUSED TRANSLATABLES", string.Empty);
                        unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
                    }
                    buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;

                    string TRANSLATION = thDataWork.THFilesElementsDataset.Tables[fname].Rows[y][translationcolumnindex] + string.Empty;
                    if (TRANSLATION.Length == 0)
                    {
                        buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
                    }

                    buffer.AppendLine("# CONTEXT : " + thDataWork.THFilesElementsDataset.Tables[fname].Rows[y][contextcolumnindex]);// + Environment.NewLine;
                    if (ADVICE.Length == 0)
                    {
                        //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
                    }
                    else
                    {
                        buffer.AppendLine("# ADVICE : " + ADVICE);// + Environment.NewLine;
                    }

                    buffer.AppendLine(thDataWork.THFilesElementsDataset.Tables[fname].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine;
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
