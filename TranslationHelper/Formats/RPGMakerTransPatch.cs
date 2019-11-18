using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Formats
{
    internal class RPGMakerTransPatch : IOpenFormats, ISaveFormats
    {
        public bool Open(string sPath)
        {
            RPGMTransPatchPrepare(sPath);

            return false;
        }

        public bool Save(string sPath)
        {
            return false;
        }

        private string RPGMTransPatchPrepare(string sPath)
        {

            var dir = new DirectoryInfo(Path.GetDirectoryName(sPath));
            //MessageBox.Show("THFOpen.FileName=" + THFOpen.FileName);
            //MessageBox.Show("dir=" + dir);
            Properties.Settings.Default.THSelectedDir = dir + string.Empty;
            //MessageBox.Show("Properties.Settings.Default.THSelectedDir=" + Properties.Settings.Default.THSelectedDir);

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
            if (patchfile.ReadLine() == "> RPGMAKER TRANS PATCH V3" || Directory.Exists(Path.Combine(Properties.Settings.Default.THSelectedDir, "patch"))) //если есть подпапка patch, тогда это версия патча 3
            {
                Properties.Settings.Default.THRPGMTransPatchver = "3";
                patchdir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(sPath), "patch"));
            }
            else //иначе это версия 2
            {
                Properties.Settings.Default.THRPGMTransPatchver = "2";
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
            //RPGMTransPatch.OpenTransFiles(files, patchver);
            if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, Properties.Settings.Default.THRPGMTransPatchver, THFilesElementsDataset, THFilesElementsDatasetInfo))
            {
                //MessageBox.Show(THSelectedSourceType + " loaded!");
                //THShowMessage(THSelectedSourceType + " loaded!");
                //ProgressInfo(false, string.Empty);
                //LogToFile(string.Empty, true);

                //Запись в dataGridVivwer
                for (int i = 0; i < THFilesElementsDataset.Tables.Count; i++)
                {
                    //MessageBox.Show("ListFiles=" + ListFiles[i]);
                    //THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesListBox.Items.Add(DS.Tables[i].TableName);//asdf
                    THFilesList.Invoke((Action)(() => THFilesList.Items.Add(THFilesElementsDataset.Tables[i].TableName)));
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

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles, string patchver, DataSet DS, DataSet DSInfo)
        {
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
                ProgressInfo(true, T._("opening file: ") + fname + ".txt");
                _file = new StreamReader(ListFiles[i]); //Задаем файл
                //THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), string.Empty));    //Добaвляем файл
                _ = DS.Tables.Add(fname);
                _ = DS.Tables[i].Columns.Add("Original");
                _ = DS.Tables[i].Columns.Add("Translation");
                _ = DS.Tables[i].Columns.Add("Context");
                _ = DS.Tables[i].Columns.Add("Advice");
                _ = DS.Tables[i].Columns.Add("Status");
                if (DSInfo == null)
                {
                }
                else
                {
                    _ = DSInfo.Tables.Add(fname);
                    _ = DSInfo.Tables[i].Columns.Add("Original");
                }

                if (patchver == "3")
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
                                _ = DS.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                if (DSInfo == null)
                                {
                                }
                                else
                                {
                                    _ = DSInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
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
                else if (patchver == "2")
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
                                        _ = DS.Tables[i].Rows.Add(_original, _translation, _context, _advice, _status);
                                        if (DSInfo == null)
                                        {
                                        }
                                        else
                                        {
                                            _ = DSInfo.Tables[i].Rows.Add("Context:" + Environment.NewLine + _context + Environment.NewLine + "Advice:" + Environment.NewLine + _advice);
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
                FVariant = " * RPG Maker Trans Patch " + patchver;
            }
            else if (invalidformat == 1)
            {
                //MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                //THMsg.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                return false;
            }

            //MessageBox.Show("111");
            //progressBar.Value = 0;

            //остановка таймера и запись времени
            //swatch.Stop();
            //LogToFile("time=" + swatch.Elapsed.ToString(), true);//asdf
            //THMsg.Show("time=" + time);

            return true;
        }
    }
}
