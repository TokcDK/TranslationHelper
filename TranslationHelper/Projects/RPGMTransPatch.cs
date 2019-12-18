﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;

namespace TranslationHelper.Projects
{
    class RPGMTransPatch : ProjectBase
    {
        public int RPGMTransPatchVersion { get; private set; }

        public RPGMTransPatch(THDataWork thData) : base(thData)
        {
        }

        internal override bool OpenDetect(string sPath)
        {
            if (Path.GetFileName(sPath) == "RPGMKTRANSPATCH")
            {
                return true;
            }

            return false;
        }

        internal override string ProjectTitle()
        {
            return "RPG Maker Trans Patch";
        }

        internal override string ProjecFolderName()
        {
            return "RPGMTransPatch";
        }

        internal override bool Open()
        {
            throw new NotImplementedException();
        }

        private bool RPGMTransPatchPrepare()
        {

            var dir = new DirectoryInfo(Path.GetDirectoryName(thDataWork.OpenPath));

            //Properties.Settings.Default.THSelectedDir = dir + string.Empty;

            var patchdir = dir;
            StreamReader patchfile = new StreamReader(thDataWork.OpenPath);

            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "patch"))) //если есть подпапка patch, тогда это версия патча 3
            {
                RPGMTransPatchVersion = 3;
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(thDataWork.OpenPath), "patch"));
            }
            else //иначе это версия 2
            {
                RPGMTransPatchVersion = 2;
            }
            patchfile.Close();

            var vRPGMTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.GetFiles("*.txt"))
            {
                //MessageBox.Show("file.FullName=" + file.FullName);
                vRPGMTransPatchFiles.Add(file.FullName);
            }

            //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

            //THFilesDataGridView.Nodes.Add("main");
            //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
            //RPGMTransPatch.OpenTransFiles(files, RPGMTransPatchVersion);
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
            {
                return true;
            }
            //}
            return false;
        }

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles)
        {
            if (ListFiles == null || thDataWork.THFilesElementsDataset == null)
                return false;

            int invalidformat=0;

            StreamReader _file;   //Через что читаем
            string _context = string.Empty;           //Комментарий
            string _advice = string.Empty;            //Предел длины строки
            string _string;// = string.Empty;            //Переменная строки
            string _original = string.Empty;           //Непереведенный текст
            string _translation = string.Empty;             //Переведенный текст
            int _status = 0;             //Статус

            int verok = 0;                  //версия патча

            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                string fname = Path.GetFileNameWithoutExtension(ListFiles[i]);

                //ProgressInfo(true, T._("opening file: ") + fname + ".txt");

                _file = new StreamReader(ListFiles[i]); //Задаем файл

                _ = thDataWork.THFilesElementsDataset.Tables.Add(fname);
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Original");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Translation");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Context");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Advice");
                _ = thDataWork.THFilesElementsDataset.Tables[i].Columns.Add("Status");
                if (thDataWork.THFilesElementsDatasetInfo == null)
                {
                }
                else
                {
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables.Add(fname);
                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[i].Columns.Add("Original");
                }

                if (RPGMTransPatchVersion == 3)
                {
                    verok = 1; //Версия опознана
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
                                //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));  //Пишем
                                _ = thDataWork.THFilesElementsDataset.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                if (thDataWork.THFilesElementsDatasetInfo == null)
                                {
                                }
                                else
                                {
                                    _ = thDataWork.THFilesElementsDatasetInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
                                }
                            }

                            _context = string.Empty;  //Чистим
                            _original = string.Empty;  //Чистим
                            _translation = string.Empty;    //Чистим
                        }
                    }
                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else if (RPGMTransPatchVersion == 2)
                {
                    string UNUSED = string.Empty;
                    verok = 1; //Версия опознана
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
                                        _ = thDataWork.THFilesElementsDataset.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                        if (thDataWork.THFilesElementsDatasetInfo == null)
                                        {
                                        }
                                        else
                                        {
                                            _ = thDataWork.THFilesElementsDatasetInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
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
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    //THMsg.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    _file.Close();  //Закрываем файл
                    return false;
                }

                if (invalidformat == 1)
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    //invalidformat = 0;
                    return false;
                }

                //progressBar.Value++;
            }

            //MessageBox.Show("111");

            if (verok == 1 & invalidformat != 1)
            {
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                //FVariant = " * RPG Maker Trans Patch " + RPGMTransPatchVersion;
            }
            else if (invalidformat == 1)
            {
                //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                return false;
            }

            return true;
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }

        public bool SaveRPGMTransPatchFiles()
        {
            //измерение времени выполнения
            //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
            //Stopwatch swatch = new Stopwatch();
            //swatch.Start();

            try
            {
                StringBuilder buffer = new StringBuilder();

                //Прогресс
                //pbstatuslabel.Visible = true;
                //pbstatuslabel.Text = "сохранение..";
                //progressBar.Maximum = 0;
                //for (int i = 0; i < ArrayTransFilses.Count; i++)
                //    for (int y = 0; y < ArrayTransFilses[i].blocks.Count; y++)
                //        progressBar.Maximum = progressBar.Maximum + ArrayTransFilses[i].blocks.Count;
                //MessageBox.Show(progressBar.Maximum.ToString());
                //progressBar.Value = 0;

                int originalcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
                int translationcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
                int contextcolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;
                int advicecolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Advice"].Ordinal;
                int statuscolumnindex = thDataWork.THFilesElementsDataset.Tables[0].Columns["Status"].Ordinal;

                if (RPGMTransPatchVersion == 3)
                {
                    //запись в файл RPGMKTRANSPATCH строки > RPGMAKER TRANS PATCH V3
                    //StreamWriter RPGMKTRANSPATCHwriter = new StreamWriter("RPGMKTRANSPATCH", true);
                    //RPGMKTRANSPATCHwriter.WriteLine("> RPGMAKER TRANS PATCH V3");
                    //RPGMKTRANSPATCHwriter.Close();

                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                    {
                        //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                        buffer.AppendLine("> RPGMAKER TRANS PATCH FILE VERSION 3.2");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < thDataWork.THFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                            buffer.AppendLine(thDataWork.THFilesElementsDataset.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine);
                            //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                            //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                            //string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                            string[] CONTEXT = (thDataWork.THFilesElementsDataset.Tables[i].Rows[y][contextcolumnindex] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                            //string str1 = string.Empty;
                            string TRANSLATION = thDataWork.THFilesElementsDataset.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
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

                        if (string.IsNullOrWhiteSpace(buffer.ToString()))
                        {
                        }
                        else
                        {
                            if (Directory.Exists(Properties.Settings.Default.THSelectedDir + "\\patch"))
                            {
                            }
                            else
                            {
                                Directory.CreateDirectory(Properties.Settings.Default.THSelectedDir + "\\patch");
                            }
                            buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string _path = Properties.Settings.Default.THSelectedDir + "\\patch\\" + thDataWork.THFilesElementsDataset.Tables[i].TableName + ".txt";
                            File.WriteAllText(_path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }

                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(Properties.Settings.Default.THSelectedDir, "RPGMKTRANSPATCH"), "> RPGMAKER TRANS PATCH V3");
                    }
                }
                else if (RPGMTransPatchVersion == 2)
                {
                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < thDataWork.THFilesElementsDataset.Tables.Count; i++)
                    {
                        //ProgressInfo(true, T._("saving file: ") + thData.THFilesElementsDataset.Tables[i].TableName);

                        bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

                        buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < thDataWork.THFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            string ADVICE = thDataWork.THFilesElementsDataset.Tables[i].Rows[y][advicecolumnindex] + string.Empty;
                            //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
                            if (unusednotfound && ADVICE.Contains("# UNUSED TRANSLATABLES"))
                            {
                                buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
                                ADVICE = ADVICE.Replace("# UNUSED TRANSLATABLES", string.Empty);
                                unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
                            }
                            buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;

                            string TRANSLATION = thDataWork.THFilesElementsDataset.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
                            if (TRANSLATION.Length == 0)
                            {
                                buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
                            }

                            buffer.AppendLine("# CONTEXT : " + thDataWork.THFilesElementsDataset.Tables[i].Rows[y][contextcolumnindex]);// + Environment.NewLine;
                            if (ADVICE.Length == 0)
                            {
                                //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
                            }
                            else
                            {
                                buffer.AppendLine("# ADVICE : " + ADVICE);// + Environment.NewLine;
                            }

                            buffer.AppendLine(thDataWork.THFilesElementsDataset.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine;
                            buffer.AppendLine("# TRANSLATION ");// + Environment.NewLine;
                            buffer.AppendLine(TRANSLATION);// + Environment.NewLine;
                            buffer.AppendLine("# END STRING" + Environment.NewLine);// + Environment.NewLine;
                        }
                        if (string.IsNullOrWhiteSpace(buffer.ToString()))
                        {
                        }
                        else
                        {
                            buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string _path = Path.Combine(Properties.Settings.Default.THSelectedDir, thDataWork.THFilesElementsDataset.Tables[i].TableName + ".txt");
                            File.WriteAllText(_path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }


                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(Properties.Settings.Default.THSelectedDir, "RPGMKTRANSPATCH"), string.Empty); ;
                    }
                }
            }
            catch
            {
                //ProgressInfo(false, string.Empty);
                //SaveInAction = false;
                return false;
            }
            finally
            {
                //ProgressInfo(false, string.Empty);
            }

            return true;

        }
    }
}