using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMaker.Functions;

namespace TranslationHelper.Projects.RPGMTrans
{
    class RpgmTransOld
    {
        
        public RpgmTransOld()
        {
            
        }

        internal string RpgmTransPatchPrepare(string sPath)
        {

            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            //MessageBox.Show("THFOpen.FileName=" + THFOpen.FileName);
            //MessageBox.Show("dir=" + dir);
            ProjectData.SelectedDir = dir + string.Empty;
            //MessageBox.Show("ProjectData.SelectedDir=" + ProjectData.SelectedDir);

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
            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(ProjectData.SelectedDir, "patch"))) //если есть подпапка patch, тогда это версия патча 3
            {
                RpgmFunctions.RpgmTransPatchVersion = "3";
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(sPath), "patch"));
            }
            else //иначе это версия 2
            {
                RpgmFunctions.RpgmTransPatchVersion = "2";
            }
            patchfile.Close();

            var vRpgmTransPatchFiles = new List<string>();

            foreach (FileInfo file in patchdir.EnumerateFiles("*.txt"))
            {
                //MessageBox.Show("file.FullName=" + file.FullName);
                vRpgmTransPatchFiles.Add(file.FullName);
            }

            //var RPGMTransPatch = new THRPGMTransPatchLoad(this);

            //THFilesDataGridView.Nodes.Add("main");
            //THRPGMTransPatchLoad RPGMTransPatch = new THRPGMTransPatchLoad();
            //RPGMTransPatch.OpenTransFiles(files, patchver);
            if (OpenRpgmTransPatchFiles(vRpgmTransPatchFiles))
            {
                //MessageBox.Show(THSelectedSourceType + " loaded!");
                //THShowMessage(THSelectedSourceType + " loaded!");
                //ProgressInfo(false, string.Empty);
                //LogToFile(string.Empty, true);

                //Запись в dataGridVivwer
                for (int i = 0; i < ProjectData.ThFilesElementsDataset.Tables.Count; i++)
                {
                    //MessageBox.Show("ListFiles=" + ListFiles[i]);
                    //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                    ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(ProjectData.ThFilesElementsDataset.Tables[i].TableName)));
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

        private int _invalidformat;
        public bool OpenRpgmTransPatchFiles(List<string> listFiles)
        {
            if (listFiles == null || ProjectData.ThFilesElementsDataset == null)
                return false;

            //измерение времени выполнения
            //http://www.cyberforum.ru/csharp-beginners/thread1090236.html
            //Stopwatch swatch = new Stopwatch();
            //swatch.Start();

            //MessageBox.Show("THRPGMTransPatchver=" + THRPGMTransPatchver);
            //MessageBox.Show("ListFiles=" + ListFiles);
            //MessageBox.Show("ListFiles[0]=" + ListFiles[0]);

            StreamReader file;   //Через что читаем
            string context = string.Empty;           //Комментарий
            string advice = string.Empty;            //Предел длины строки
            string @string;// = string.Empty;            //Переменная строки
            string original = string.Empty;           //Непереведенный текст
            string translation = string.Empty;             //Переведенный текст
            int status = 0;             //Статус

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
            for (int i = 0; i < listFiles.Count; i++)   //Обрабатываем всю строку
            {
                string fname = Path.GetFileNameWithoutExtension(listFiles[i]);
                ProjectData.Main.ProgressInfo(true, T._("opening file: ") + fname + ".txt");
                file = new StreamReader(listFiles[i]); //Задаем файл
                //THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), string.Empty));    //Добaвляем файл
                _ = ProjectData.ThFilesElementsDataset.Tables.Add(fname);
                _ = ProjectData.ThFilesElementsDataset.Tables[i].Columns.Add("Original");
                _ = ProjectData.ThFilesElementsDataset.Tables[i].Columns.Add("Translation");
                _ = ProjectData.ThFilesElementsDataset.Tables[i].Columns.Add("Context");
                _ = ProjectData.ThFilesElementsDataset.Tables[i].Columns.Add("Advice");
                _ = ProjectData.ThFilesElementsDataset.Tables[i].Columns.Add("Status");
                if (ProjectData.ThFilesElementsDatasetInfo == null)
                {
                }
                else
                {
                    _ = ProjectData.ThFilesElementsDatasetInfo.Tables.Add(fname);
                    _ = ProjectData.ThFilesElementsDatasetInfo.Tables[i].Columns.Add("Original");
                }

                if (RpgmFunctions.RpgmTransPatchVersion == "3")
                {
                    verok = 1; //Версия опознана
                    while (!file.EndOfStream)   //Читаем до конца
                    {
                        @string = file.ReadLine();                       //Чтение
                        //Код для версии патча 3
                        if (@string.StartsWith("> BEGIN STRING"))
                        {
                            _invalidformat = 2; //если нашло строку
                            @string = file.ReadLine();

                            int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной                            
                            while (!@string.StartsWith("> CONTEXT:"))  //Ждем начало следующего блока
                            {
                                if (untranslines > 0)
                                {
                                    original += Environment.NewLine;
                                }
                                original += @string;            //Пишем весь текст
                                @string = file.ReadLine();
                                untranslines++;
                            }

                            int contextlines = 0;
                            while (@string.StartsWith("> CONTEXT:"))
                            {
                                if (contextlines > 0)
                                {
                                    context += Environment.NewLine;
                                }

                                context += @string.Replace("> CONTEXT: ", string.Empty).Replace(" < UNTRANSLATED", string.Empty);// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий

                                @string = file.ReadLine();
                                contextlines++;
                            }

                            int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            while (!@string.StartsWith("> END"))      //Ждем конец блока
                            {
                                if (translines > 0)
                                {
                                    translation += Environment.NewLine;
                                }
                                translation += @string;
                                @string = file.ReadLine();
                                translines++;
                            }

                            if (original == Environment.NewLine)
                            {
                            }
                            else
                            {
                                //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));  //Пишем
                                _ = ProjectData.ThFilesElementsDataset.Tables[i].Rows.Add(original, translation, context, advice, status);
                                if (ProjectData.ThFilesElementsDatasetInfo == null)
                                {
                                }
                                else
                                {
                                    _ = ProjectData.ThFilesElementsDatasetInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + context + Environment.NewLine + "Advice:" + Environment.NewLine + advice);
                                }
                            }

                            context = string.Empty;  //Чистим
                            original = string.Empty;  //Чистим
                            translation = string.Empty;    //Чистим
                        }
                    }
                    if (_invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        _invalidformat = 1;
                    }
                    file.Close();  //Закрываем файл
                }
                else if (RpgmFunctions.RpgmTransPatchVersion == "2")
                {
                    string unused = string.Empty;
                    verok = 1; //Версия опознана
                    while (!file.EndOfStream)   //Читаем до конца
                    {
                        @string = file.ReadLine();                       //Чтение
                        if (Equals(@string, "# UNUSED TRANSLATABLES"))//означает, что перевод отсюда и далее не используется в игре и помечен RPGMakerTrans этой строкой
                        {
                            //MessageBox.Show(_string);
                            unused = @string;
                        }
                        //Код для версии патча 2.0
                        if (@string.StartsWith("# CONTEXT"))               //Ждем начало блока
                        {
                            _invalidformat = 2;//строка найдена, формат верен

                            if (@string.Split(' ')[3] != "itemAttr/UsageMessage")
                            {
                                context = @string.Replace("# CONTEXT : ", string.Empty); //Сохраняем коментарий

                                @string = file.ReadLine();

                                //asdf advice Иногда advice отсутствует, например когда "# CONTEXT : Dialogue/SetHeroName" в патче VH
                                if (@string.StartsWith("# ADVICE"))
                                {
                                    advice = @string.Replace("# ADVICE : ", string.Empty);   //Вытаскиваем число предела
                                    @string = file.ReadLine();
                                }
                                else
                                {
                                    advice = string.Empty;
                                }

                                if (unused.Length == 0)
                                {
                                }
                                else
                                {
                                    advice += unused;//добавление информации о начале блока неиспользуемых строк
                                    unused = string.Empty;//очистка переменной в целях оптимизации, чтобы не писать во все ADVICE
                                }

                                int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                                while (!@string.StartsWith("# TRANSLATION"))  //Ждем начало следующего блока
                                {
                                    if (untranslines > 0)
                                    {
                                        original += Environment.NewLine;
                                    }
                                    original += @string;            //Пишем весь текст
                                    @string = file.ReadLine();
                                    untranslines++;
                                }
                                if (original.Length > 0)                    //Если текст есть, ищем перевод
                                {
                                    @string = file.ReadLine();
                                    int translationlinescount = 0;
                                    while (!@string.StartsWith("# END"))      //Ждем конец блока
                                    {
                                        if (translationlinescount > 0)
                                        {
                                            translation += Environment.NewLine;
                                        }
                                        translation += @string;
                                        @string = file.ReadLine();
                                        translationlinescount++;
                                    }
                                    if (original != Environment.NewLine)
                                    {
                                        //THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
                                        _ = ProjectData.ThFilesElementsDataset.Tables[i].Rows.Add(original, translation, context, advice, status);
                                        if (ProjectData.ThFilesElementsDatasetInfo == null)
                                        {
                                        }
                                        else
                                        {
                                            _ = ProjectData.ThFilesElementsDatasetInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + context + Environment.NewLine + "Advice:" + Environment.NewLine + advice);
                                        }
                                    }
                                }
                                original = string.Empty;  //Чистим
                                translation = string.Empty;    //Чистим
                            }
                        }
                    }
                    if (_invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        _invalidformat = 1;
                    }
                    file.Close();  //Закрываем файл
                }
                else
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    ///*THMsg*/MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    file.Close();  //Закрываем файл
                    return false;
                }

                if (_invalidformat == 1)
                {
                    //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    ///*THMsg*/MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    _invalidformat = 0;
                    return false;
                }

                //progressBar.Value++;
            }

            //MessageBox.Show("111");

            if (verok == 1 & _invalidformat != 1)
            {
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                ProjectData.Main.FVariant = " * RPG Maker Trans Patch " + RpgmFunctions.RpgmTransPatchVersion;
            }
            else if (_invalidformat == 1)
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

        internal bool SaveRpgmTransPatchFiles(string selectedDir, string patchver = "2")
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

                int originalcolumnindex = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Original"].Ordinal;
                int translationcolumnindex = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Translation"].Ordinal;
                int contextcolumnindex = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Context"].Ordinal;
                int advicecolumnindex = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Advice"].Ordinal;
                int statuscolumnindex = ProjectData.ThFilesElementsDataset.Tables[0].Columns["Status"].Ordinal;

                if (patchver == "3")
                {
                    //запись в файл RPGMKTRANSPATCH строки > RPGMAKER TRANS PATCH V3
                    //StreamWriter RPGMKTRANSPATCHwriter = new StreamWriter("RPGMKTRANSPATCH", true);
                    //RPGMKTRANSPATCHwriter.WriteLine("> RPGMAKER TRANS PATCH V3");
                    //RPGMKTRANSPATCHwriter.Close();

                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < ProjectData.ThFilesElementsDataset.Tables.Count; i++)
                    {
                        ProjectData.Main.ProgressInfo(true, T._("saving file: ") + ProjectData.ThFilesElementsDataset.Tables[i].TableName);

                        buffer.AppendLine("> RPGMAKER TRANS PATCH FILE VERSION 3.2");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < ProjectData.ThFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                            buffer.AppendLine(ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine);
                            //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                            //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                            //string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                            string[] context = (ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][contextcolumnindex] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                            //string str1 = string.Empty;
                            string translation = ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
                            for (int g = 0; g < context.Length; g++)
                            {
                                /*CONTEXT[g] = CONTEXT[g].Replace("\r", string.Empty);*///очистка от знака переноса, возникающего после разбития на строки по \n
                                if (context.Length > 1)
                                {
                                    buffer.AppendLine("> CONTEXT: " + context[g]);// + Environment.NewLine);
                                }
                                else
                                {   //if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                    if (translation.Length == 0) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                    {
                                        buffer.AppendLine("> CONTEXT: " + context[g] + " < UNTRANSLATED");// + Environment.NewLine);
                                    }
                                    else
                                    {
                                        buffer.AppendLine("> CONTEXT: " + context[g]);// + Environment.NewLine);
                                    }
                                }
                            }
                            //buffer += Environment.NewLine;
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation + Environment.NewLine;
                            buffer.AppendLine(translation);// + Environment.NewLine);
                            buffer.AppendLine("> END STRING" + Environment.NewLine);// + Environment.NewLine);

                            //progressBar.Value++;
                            //MessageBox.Show(progressBar.Value.ToString());
                        }

                        if (string.IsNullOrWhiteSpace(buffer.ToString()))
                        {
                        }
                        else
                        {
                            if (Directory.Exists(selectedDir + "\\patch"))
                            {
                            }
                            else
                            {
                                Directory.CreateDirectory(selectedDir + "\\patch");
                            }
                            buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string path = selectedDir + "\\patch\\" + ProjectData.ThFilesElementsDataset.Tables[i].TableName + ".txt";
                            File.WriteAllText(path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }

                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(selectedDir, "RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(selectedDir, "RPGMKTRANSPATCH"), "> RPGMAKER TRANS PATCH V3");
                    }
                }
                else if (patchver == "2")
                {
                    //for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                    for (int i = 0; i < ProjectData.ThFilesElementsDataset.Tables.Count; i++)
                    {
                        ProjectData.Main.ProgressInfo(true, T._("saving file: ") + ProjectData.ThFilesElementsDataset.Tables[i].TableName);

                        bool unusednotfound = true;//для проверки начала неиспользуемых строк, в целях оптимизации

                        buffer.AppendLine("# RPGMAKER TRANS PATCH FILE VERSION 2.0");// + Environment.NewLine);
                        //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                        for (int y = 0; y < ProjectData.ThFilesElementsDataset.Tables[i].Rows.Count; y++)
                        {
                            string advice = ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][advicecolumnindex] + string.Empty;
                            //Если в advice была информация о начале блоков неиспользуемых, то вставить эту строчку
                            if (unusednotfound && advice.Contains("# UNUSED TRANSLATABLES"))
                            {
                                buffer.AppendLine("# UNUSED TRANSLATABLES");// + Environment.NewLine;
                                advice = advice.Replace("# UNUSED TRANSLATABLES", string.Empty);
                                unusednotfound = false;//в целях оптимизации, проверка двоичного значения быстрее, чемискать в строке
                            }
                            buffer.AppendLine("# TEXT STRING");// + Environment.NewLine;
                            //if (THRPGMTransPatchFiles[i].blocks[y].Translation == "\r\n")
                            string translation = ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][translationcolumnindex] + string.Empty;
                            if (translation.Length == 0)
                            {
                                buffer.AppendLine("# UNTRANSLATED");// + Environment.NewLine;
                            }
                            //buffer += "# CONTEXT : " + THRPGMTransPatchFiles[i].blocks[y].Context + Environment.NewLine;
                            buffer.AppendLine("# CONTEXT : " + ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][contextcolumnindex]);// + Environment.NewLine;
                            if (advice.Length == 0)
                            {
                                //иногда # ADVICE отсутствует и при записи нужно пропускать запись этого пункта
                            }
                            else
                            {
                                //buffer += "# ADVICE : " + THRPGMTransPatchFiles[i].blocks[y].Advice + Environment.NewLine;
                                buffer.AppendLine("# ADVICE : " + advice);// + Environment.NewLine;
                            }
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original;
                            buffer.AppendLine(ProjectData.ThFilesElementsDataset.Tables[i].Rows[y][originalcolumnindex] + string.Empty);// + Environment.NewLine;
                            buffer.AppendLine("# TRANSLATION ");// + Environment.NewLine;
                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Translation;
                            buffer.AppendLine(translation);// + Environment.NewLine;
                            buffer.AppendLine("# END STRING" + Environment.NewLine);// + Environment.NewLine;
                        }
                        if (string.IsNullOrWhiteSpace(buffer.ToString()))
                        {
                        }
                        else
                        {
                            buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки
                            //String _path = SelectedDir + "\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                            string path = Path.Combine(selectedDir, ProjectData.ThFilesElementsDataset.Tables[i].TableName + ".txt");
                            File.WriteAllText(path, buffer.ToString());
                            //buffer = string.Empty;
                        }
                        buffer.Clear();
                    }


                    //Запись самого файла патча, если вдруг сохраняется в произвольную папку
                    if (File.Exists(Path.Combine(selectedDir, "RPGMKTRANSPATCH")))
                    {
                    }
                    else
                    {
                        File.WriteAllText(Path.Combine(selectedDir, "RPGMKTRANSPATCH"), string.Empty); ;
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
