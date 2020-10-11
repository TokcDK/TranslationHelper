using System;
using System.Collections.Generic;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Formats.RPGMTrans
{
    class TXT : RPGMTransPatchBase
    {
        public TXT(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override void ParseStringFilePreOpenExtra()
        {
            unused = false;
            verchecked = false;
        }

        bool verchecked;
        bool unused;
        int patchversion;
        protected override int ParseStringFileLine()
        {
            if (!verchecked)
            {
                if (ParseData.line == "> RPGMAKER TRANS PATCH FILE VERSION 3.2")
                {
                    patchversion = 3;
                }
                else if (ParseData.line == "# RPGMAKER TRANS PATCH FILE VERSION 2.0")
                {
                    patchversion = 2;
                }
                else
                {
                    return -1; //Not a patch file, break parsing
                }

                verchecked = true;
            }

            var ret = 1;

            if (patchversion == 3)
            {
                //patch version 3 code
                if (ParseData.line.StartsWith("> BEGIN STRING"))
                {
                    var originalLines = new List<string>();
                    ReadLine();
                    do
                    {
                        originalLines.Add(ParseData.line);
                    }
                    while (!ReadLine().StartsWith("> CONTEXT:"));

                    var contextlines = new List<string>();
                    do
                    {
                        contextlines.Add(ParseData.line);
                    }
                    while (ReadLine().StartsWith("> CONTEXT:"));

                    var translatedLines = new List<string>();
                    do
                    {
                        translatedLines.Add(ParseData.line);
                    }
                    while (ReadLine() != "> END STRING");

                    var original = originalLines.Joined();
                    var translation = translatedLines.Joined();
                    var context = contextlines.Joined();
                    if (thDataWork.OpenFileMode)
                    {
                        AddRowData(new[] { original, translation }, context, true);
                    }
                    else
                    {
                        var translated = false;
                        if (IsValidString(original) && thDataWork.TablesLinesDict.ContainsKey(original) && translation != thDataWork.TablesLinesDict[original])
                        {
                            translated = true;
                            ParseData.Ret = true;
                            translation = thDataWork.TablesLinesDict[original];
                        }

                        //remove or add untranslated tag when translation was changed
                        if (translated)
                        {
                            context = context.Replace(" < UNTRANSLATED", string.Empty);
                        }
                        else if(!context.Contains(" < UNTRANSLATED"))
                        {
                            for (int i = 0; i < contextlines.Count; i++)
                            {
                                if(!contextlines[i].EndsWith(" < UNTRANSLATED"))
                                {
                                    contextlines[i] = contextlines[i] + " < UNTRANSLATED";
                                }

                            }
                        }

                        ParseData.line =
                            "> BEGIN STRING"
                            + Environment.NewLine
                            + original
                            + Environment.NewLine
                            + context
                            + Environment.NewLine
                            + translation
                            + Environment.NewLine
                            + "> END STRING"
                            ;
                    }
                }
            }
            else //patch version 2 code
            {
                if (!unused && ParseData.line == "# UNUSED TRANSLATABLES")//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
                {
                    unused = true;
                }
                
                if (ParseData.line.StartsWith("# TEXT STRING"))
                {
                    var untranslated = false;
                    while (!ReadLine().StartsWith("# CONTEXT"))
                    {
                        if (ParseData.line.StartsWith("# UNTRANSLATED"))
                        {
                            untranslated = true;
                        }
                    }

                    if (ParseData.line.Split(' ')[3] != "itemAttr/UsageMessage")
                    {
                        var context = ParseData.line; //контекст

                        ReadLine();

                        var advice = "";
                        //asdf advice Иногда advice отсутствует, например когда "# CONTEXT : Dialogue/SetHeroName" в патче VH
                        if (ParseData.line.StartsWith("# ADVICE"))//совет обычно содержит инфу о макс длине строки
                        {
                            advice = ParseData.line;
                            ReadLine();//читать след. строку, когда был совет
                        }

                        if (unused)
                        {
                            advice += Environment.NewLine + "UNUSED";//информа о начале блока неиспользуемых строк
                        }

                        var originalLines = new List<string>();
                        do
                        {
                            originalLines.Add(ParseData.line);
                        }
                        while (!ReadLine().StartsWith("# TRANSLATION"));//read while translatin block will start

                        var translatedLines = new List<string>();
                        do
                        {

                        }
                        while (!ReadLine().StartsWith("# END STRING"));

                        string original = originalLines.Joined();
                        string translation = translatedLines.Joined();
                        if (thDataWork.OpenFileMode)
                        {
                            AddRowData(new[] { original, translation }, context + Environment.NewLine + advice, true);
                        }
                        else
                        {
                            var translated = false;
                            if (IsValidString(original) && thDataWork.TablesLinesDict.ContainsKey(original) && translation != thDataWork.TablesLinesDict[original])
                            {
                                translated = true;
                                ParseData.Ret = true;
                                translation = thDataWork.TablesLinesDict[original];
                            }

                            ParseData.line =
                                "# TEXT STRING"
                                + Environment.NewLine
                                + (!translated ? "# UNTRANSLATED" + Environment.NewLine : string.Empty)
                                + context
                                + Environment.NewLine
                                + advice
                                + Environment.NewLine
                                + original
                                + Environment.NewLine
                                + "# TRANSLATION"
                                + Environment.NewLine
                                + translation
                                + Environment.NewLine
                                + "# END STRING";
                        }
                    }
                    else
                    {
                        ParseData.line =
                            "# TEXT STRING"
                            + Environment.NewLine
                            + (untranslated ? "# UNTRANSLATED" + Environment.NewLine : string.Empty)
                            + ParseData.line;
                    }
                }
            }

            if (thDataWork.SaveFileMode)
            {
                ParseData.ResultForWrite.AppendLine(ParseData.line);
            }

            return ret;
        }

        //bool OpenOld()
        //{
        //    try
        //    {
        //        int invalidformat = 0;

        //        string _context = string.Empty;           //Комментарий
        //        string _advice = string.Empty;            //Предел длины строки
        //        string _string;// = string.Empty;            //Переменная строки
        //        string _original = string.Empty;           //Непереведенный текст
        //        string _translation = string.Empty;             //Переведенный текст
        //        int _status = 0;             //Статус

        //        using (StreamReader _file = new StreamReader(thDataWork.FilePath)) //Задаем файл
        //        {
        //            string fname = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

        //            AddTables(fname, new string[] { "Context", "Advice", "Status" });
        //            var Table = thDataWork.THFilesElementsDataset.Tables[fname];
        //            var TableInfo = thDataWork.THFilesElementsDatasetInfo.Tables[fname];

        //            string UNUSED = string.Empty;
        //            while (!_file.EndOfStream)   //Читаем до конца
        //            {
        //                _string = _file.ReadLine();                       //Чтение
        //                if (Equals(_string, "# UNUSED TRANSLATABLES"))//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
        //                {
        //                    //MessageBox.Show(_string);
        //                    UNUSED = _string;
        //                }
        //                //Код для версии патча 2.0
        //                if (_string.StartsWith("# CONTEXT"))               //Ждем начало блока
        //                {
        //                    invalidformat = 2;//строка найдена, формат верен

        //                    if (_string.Split(' ')[3] != "itemAttr/UsageMessage")
        //                    {
        //                        _context = _string.Replace("# CONTEXT : ", string.Empty); //Сохраняем коментарий

        //                        _string = _file.ReadLine();

        //                        //asdf advice Иногда advice отсутствует, например когда "# CONTEXT : Dialogue/SetHeroName" в патче VH
        //                        if (_string.StartsWith("# ADVICE"))
        //                        {
        //                            _advice = _string.Replace("# ADVICE : ", string.Empty);   //Вытаскиваем число предела
        //                            _string = _file.ReadLine();
        //                        }
        //                        else
        //                        {
        //                            _advice = string.Empty;
        //                        }

        //                        if (UNUSED.Length == 0)
        //                        {
        //                        }
        //                        else
        //                        {
        //                            _advice += UNUSED;//добавление информации о начале блока неиспользуемых строк
        //                            UNUSED = string.Empty;//очистка переменной в целях оптимизации, чтобы не писать во все ADVICE
        //                        }

        //                        int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
        //                        while (!_string.StartsWith("# TRANSLATION"))  //Ждем начало следующего блока
        //                        {
        //                            if (untranslines > 0)
        //                            {
        //                                _original += Environment.NewLine;
        //                            }
        //                            _original += _string;            //Пишем весь текст
        //                            _string = _file.ReadLine();
        //                            untranslines++;
        //                        }
        //                        if (_original.Length > 0)                    //Если текст есть, ищем перевод
        //                        {
        //                            _string = _file.ReadLine();
        //                            int _translationlinescount = 0;
        //                            while (!_string.StartsWith("# END"))      //Ждем конец блока
        //                            {
        //                                if (_translationlinescount > 0)
        //                                {
        //                                    _translation += Environment.NewLine;
        //                                }
        //                                _translation += _string;
        //                                _string = _file.ReadLine();
        //                                _translationlinescount++;
        //                            }
        //                            if (_original != Environment.NewLine)
        //                            {
        //                                //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
        //                                Table.Rows.Add(_original, _translation, _context, _advice, _status);
        //                                if (TableInfo == null)
        //                                {
        //                                }
        //                                else
        //                                {
        //                                    TableInfo.Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
        //                                }
        //                            }
        //                        }

        //                        //Чистка
        //                        _original = string.Empty;
        //                        _translation = string.Empty;
        //                    }
        //                }
        //            }

        //            if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
        //            {
        //                return false; ;
        //            }

        //            return CheckTablesContent(fname);
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //bool SaveOld()
        //{
        //    try
        //    {
        //        int originalcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
        //        int translationcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
        //        int contextcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;
        //        int advicecolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Advice"].Ordinal;
        //        int statuscolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Status"].Ordinal;

        //        string fname = Path.GetFileNameWithoutExtension(thDataWork.FilePath);

        //        //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

        //        bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

        //        buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
        //                                                                     //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
        //        int TableRowsCount = thDataWork.THFilesElementsDataset.Tables[fname].Rows.Count;
        //        for (int y = 0; y < TableRowsCount; y++)
        //        {
        //            var row = thDataWork.THFilesElementsDataset.Tables[fname].Rows[y];
        //            string ADVICE = row[advicecolumnindex] + string.Empty;
        //            //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
        //            if (unusednotfound && ADVICE.Contains("# UNUSED TRANSLATABLES"))
        //            {
        //                buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
        //                ADVICE = ADVICE.Replace("# UNUSED TRANSLATABLES", string.Empty);
        //                unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
        //            }
        //            buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;

        //            string TRANSLATION = row[translationcolumnindex] + string.Empty;
        //            if (TRANSLATION.Length == 0)
        //            {
        //                buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
        //            }

        //            buffer.AppendLine("# CONTEXT : " + row[contextcolumnindex]);// + Environment.NewLine;
        //            if (ADVICE.Length == 0)
        //            {
        //                //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
        //            }
        //            else
        //            {
        //                buffer.AppendLine("# ADVICE : " + ADVICE);// + Environment.NewLine;
        //            }

        //            buffer.AppendLine(row[originalcolumnindex] + string.Empty);// + Environment.NewLine;
        //            buffer.AppendLine("# TRANSLATION ");// + Environment.NewLine;
        //            buffer.AppendLine(TRANSLATION);// + Environment.NewLine;
        //            buffer.AppendLine("# END STRING" + Environment.NewLine);// + Environment.NewLine;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    return true;
        //}
    }
}
