using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.RPGMaker.Functions;

namespace TranslationHelper.Projects.RPGMTrans
{
    [Obsolete]
    class RPGMTransOLD
    {
        
        public RPGMTransOLD()
        {
            
        }

        internal string RPGMTransPatchPrepare(string sPath)
        {

            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            //MessageBox.Show("THFOpen.FileName=" + THFOpen.FileName);
            //MessageBox.Show("dir=" + dir);
            ProjectData.CurrentProject.SelectedDir = dir + string.Empty;
            //MessageBox.Show("ProjectData.CurrentProject.SelectedDir=" + ProjectData.CurrentProject.SelectedDir);

            //MessageBox.Show("sType=" + sType);

            //if (THSelectedSourceType == "RPGMakerTransPatch")
            //{
            //Cleaning of the type
            //THRPGMTransPatchFiles.Clear();
            //THFilesElementsDataset.Clear();

            //string patchver;
            var patchdir = dir;
            StreamReader patchfile = new StreamReader(sPath);
            //MessageBox.Show(patchfile.ReadLine());
            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(ProjectData.CurrentProject.SelectedDir, "patch"))) //если есть подпапка patch, тогда это версия патча 3
            {
                RPGMFunctions.RPGMTransPatchVersion = "3";
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(sPath), "patch"));
            }
            else //иначе это версия 2
            {
                RPGMFunctions.RPGMTransPatchVersion = "2";
            }
            patchfile.Close();

            var vRPGMTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.EnumerateFiles("*.txt"))
            {
                //MessageBox.Show("file.FullName=" + file.FullName);
                vRPGMTransPatchFiles.Add(file.FullName);
            }

            //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

            //THFilesDataGridView.Nodes.Add("main");
            //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
            //RPGMTransPatch.OpenTransFiles(files, patchver);
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles))
            {
                //MessageBox.Show(THSelectedSourceType + " loaded!");
                //THShowMessage(THSelectedSourceType + " loaded!");
                //ProgressInfo(false, string.Empty);
                //LogToFile(string.Empty, true);

                //Запись в dataGridVivwer
                for (int i = 0; i < ProjectData.CurrentProject.FilesContent.Tables.Count; i++)
                {
                    //MessageBox.Show("ListFiles=" + ListFiles[i]);
                    //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                    ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(ProjectData.CurrentProject.FilesContent.Tables[i].TableName)));
                    //THFilesDataGridView.Rows.Add();
                    //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                    //dGFiles.Rows.Add();
                    //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                }

                return "RPGMakerTransPatch";
            }
            //}
            return string.Empty;
        }

        private int invalidformat;
        public bool OpenRPGMTransPatchFiles(List<string> ListFiles)
        {
            if (ListFiles == null || ProjectData.CurrentProject.FilesContent == null)
                return false;

            //измерение времени выполнения
            //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
            //Stopwatch swatch = new Stopwatch();
            //swatch.Start();

            //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
            //MessageBox.Show("ListFiles=" + ListFiles);
            //MessageBox.Show("ListFiles[0]=" + ListFiles[0]);

            StreamReader _file;   //Через что читаем
            string _context = string.Empty;           //Комментарий
            string _advice = string.Empty;            //Предел длины строки
            string _string;// = string.Empty;            //Переменная строки
            string _original = string.Empty;           //Непереведенный текст
            string _translation = string.Empty;             //Переведенный текст
            int _status = 0;             //Статус

            int verok = 0;                  //версия патча
            //THMain Main = new THMain();
            //var Main = (THMain)MainForm;
            //THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //THFilesDataGridView.Columns.Add("Filename", "Text");

            //прогрессбар
            //progressBar.Maximum = ListFiles.Count;
            //progressBar.Value = 0;
            //List<RPGMTransPatchFile> THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                string fname = Path.GetFileNameWithoutExtension(ListFiles[i]);
                ProjectData.Main.ProgressInfo(true, T._("opening file: ") + fname + ".txt");
                _file = new StreamReader(ListFiles[i]); //Задаем файл
                //THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), string.Empty));    //Добaвляем файл
                _ = ProjectData.CurrentProject.FilesContent.Tables.Add(fname);
                _ = ProjectData.CurrentProject.FilesContent.Tables[i].Columns.Add(THSettings.OriginalColumnName());
                _ = ProjectData.CurrentProject.FilesContent.Tables[i].Columns.Add(THSettings.TranslationColumnName());
                _ = ProjectData.CurrentProject.FilesContent.Tables[i].Columns.Add("Context");
                _ = ProjectData.CurrentProject.FilesContent.Tables[i].Columns.Add("Advice");
                _ = ProjectData.CurrentProject.FilesContent.Tables[i].Columns.Add("Status");
                if (ProjectData.CurrentProject.FilesContentInfo == null)
                {
                }
                else
                {
                    _ = ProjectData.CurrentProject.FilesContentInfo.Tables.Add(fname);
                    _ = ProjectData.CurrentProject.FilesContentInfo.Tables[i].Columns.Add(THSettings.OriginalColumnName());
                }

                if (RPGMFunctions.RPGMTransPatchVersion == "3")
                {
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
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
                                _ = ProjectData.CurrentProject.FilesContent.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                if (ProjectData.CurrentProject.FilesContentInfo == null)
                                {
                                }
                                else
                                {
                                    _ = ProjectData.CurrentProject.FilesContentInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
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
                else if (RPGMFunctions.RPGMTransPatchVersion == "2")
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
                                        _ = ProjectData.CurrentProject.FilesContent.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                        if (ProjectData.CurrentProject.FilesContentInfo == null)
                                        {
                                        }
                                        else
                                        {
                                            _ = ProjectData.CurrentProject.FilesContentInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
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
                    ///*THMsg*/MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    _file.Close();  //Закрываем файл
                    return false;
                }

                if (invalidformat == 1)
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    ///*THMsg*/MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    invalidformat = 0;
                    return false;
                }

                //progressBar.Value++;
            }

            //MessageBox.Show("111");

            if (verok == 1 & invalidformat != 1)
            {
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                ProjectData.Main.FVariant = " * RPG Maker Trans Patch " + RPGMFunctions.RPGMTransPatchVersion;
            }
            else if (invalidformat == 1)
            {
                //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                ///*THMsg*/MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                return false;
            }

            //MessageBox.Show("111");
            //progressBar.Value = 0;

            //остановка таймера и запись времени
            //swatch.Stop();
            //LogToFile("time=" + swatch.Elapsed.ToString(), true);//asdf
            ///*THMsg*/MessageBox.Show("time=" + time);

            return true;
        }

        internal bool SaveRPGMTransPatchFiles(string SelectedDir, string patchver = "2")
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

                int originalcolumnindex = ProjectData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.OriginalColumnName()].Ordinal;
                int translationcolumnindex = ProjectData.CurrentProject.FilesContent.Tables[0].Columns[THSettings.TranslationColumnName()].Ordinal;
                int contextcolumnindex = ProjectData.CurrentProject.FilesContent.Tables[0].Columns["Context"].Ordinal;
                int advicecolumnindex = ProjectData.CurrentProject.FilesContent.Tables[0].Columns["Advice"].Ordinal;
                int statuscolumnindex = ProjectData.CurrentProject.FilesContent.Tables[0].Columns["Status"].Ordinal;

                if (patchver == "3")
                {
                    //запись в файл RPGMKTRANSPATCH строки > RPGMAKER TRANS PATCH V3
                    //StreamWriter RPGMKTRANSPATCHwriter = new StreamWriter("RPGMKTRANSPATCH", true);
                    //RPGMKTRANSPATCHwriter.WriteLine("> RPGMAKER TRANS PATCH V3");
                    //RPGMKTRANSPATCHwriter.Close();

                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < ProjectData.CurrentProject.FilesContent.Tables.Count; i++)
                    {
                        ProjectData.Main.ProgressInfo(true, T._("saving file: ") + ProjectData.CurrentProject.FilesContent.Tables[i].TableName);

                        buffer.AppendLine("> RPGMAKER TRANS PATCH FILE VERSION 3.2");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < ProjectData.CurrentProject.FilesContent.Tables[i].Rows.Count; y++)
                        {
                            buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                            buffer.AppendLine(ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine);
                            //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                            //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                            //string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                            string[] CONTEXT = (ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][contextcolumnindex] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                            //string str1 = string.Empty;
                            string TRANSLATION = ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
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
                            if (Directory.Exists(SelectedDir + "\\patch"))
                            {
                            }
                            else
                            {
                                Directory.CreateDirectory(SelectedDir + "\\patch");
                            }
                            buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string _path = SelectedDir + "\\patch\\" + ProjectData.CurrentProject.FilesContent.Tables[i].TableName + ".txt";
                            File.WriteAllText(_path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }

                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(SelectedDir, "RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(SelectedDir, "RPGMKTRANSPATCH"), "> RPGMAKER TRANS PATCH V3");
                    }
                }
                else if (patchver == "2")
                {
                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < ProjectData.CurrentProject.FilesContent.Tables.Count; i++)
                    {
                        ProjectData.Main.ProgressInfo(true, T._("saving file: ") + ProjectData.CurrentProject.FilesContent.Tables[i].TableName);

                        bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

                        buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < ProjectData.CurrentProject.FilesContent.Tables[i].Rows.Count; y++)
                        {
                            string ADVICE = ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][advicecolumnindex] + string.Empty;
                            //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
                            if (unusednotfound && ADVICE.Contains("# UNUSED TRANSLATABLES"))
                            {
                                buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
                                ADVICE = ADVICE.Replace("# UNUSED TRANSLATABLES", string.Empty);
                                unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
                            }
                            buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;
                            //if (THRPGMTransPatchFiles[i].blocks[y].Translation == "\r\n")
                            string TRANSLATION = ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
                            if (TRANSLATION.Length == 0)
                            {
                                buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
                            }
                            //buffer += "# CONTEXT : " + THRPGMTransPatchFiles[i].blocks[y].Context + Environment.NewLine;
                            buffer.AppendLine("# CONTEXT : " + ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][contextcolumnindex]);// + Environment.NewLine;
                            if (ADVICE.Length == 0)
                            {
                                //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
                            }
                            else
                            {
                                //buffer += "# ADVICE : " + THRPGMTransPatchFiles[i].blocks[y].Advice + Environment.NewLine;
                                buffer.AppendLine("# ADVICE : " + ADVICE);// + Environment.NewLine;
                            }
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original;
                            buffer.AppendLine(ProjectData.CurrentProject.FilesContent.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine;
                            buffer.AppendLine("# TRANSLATION ");// + Environment.NewLine;
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation;
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
                            string _path = Path.Combine(SelectedDir, ProjectData.CurrentProject.FilesContent.Tables[i].TableName + ".txt");
                            File.WriteAllText(_path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }


                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(SelectedDir, "RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(SelectedDir, "RPGMKTRANSPATCH"), string.Empty); ;
                    }
                }
                //pbstatuslabel.Visible = false;
                //pbstatuslabel.Text = string.Empty;
            }
            catch
            {
                //THInfoTextBox.Text = string.Empty;
                //THActionProgressBar.Visible = false;
                //THInfolabel.Invoke((Action)(() => THInfolabel.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Text = string.Empty));
                ProjectData.Main.ProgressInfo(false, string.Empty);
                ProjectData.Main.SaveInAction = false;
                return false;
            }
            finally
            {
                //THInfoTextBox.Text = string.Empty;
                //THActionProgressBar.Invoke((Action)(() => THActionProgressBar.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Visible = false));
                //THInfolabel.Invoke((Action)(() => THInfolabel.Text = string.Empty));
                ProjectData.Main.ProgressInfo(false, string.Empty);
            }

            ProjectData.Main.SaveInAction = false;


            //остановка таймера и запись времени
            //swatch.Stop();
            //LogToFile("time=" + swatch.Elapsed.ToString(), true);//asdf
            ///*THMsg*/MessageBox.Show("time=" + time);

            return true;

        }
    }
}
