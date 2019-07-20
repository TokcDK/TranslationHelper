using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TranslationHelper
{
    public partial class THMain : Form
    {
        //public const string THStrDGTranslationColumnName = "Translation";
        //public const string THStrDGOriginalColumnName = "Original";
        private THLang LangF;

        private string FVariant = "";
        private BindingList<THRPGMTransPatchFile> THRPGMTransPatchFiles; //Все файлы
        //DataTable dt;
        //private BindingSource THBS = new BindingSource();

        private string THSelectedDir;
        private string THRPGMTransPatchver;
        private string THSelectedSourceType;

        public THMain()
        {
            InitializeComponent();
            LangF = new THLang();
            fileToolStripMenuItem.Text = LangF.THStrfileToolStripMenuItemName;
            openToolStripMenuItem.Text = LangF.THStropenToolStripMenuItemName;
            saveToolStripMenuItem.Text = LangF.THStrsaveToolStripMenuItemName;
            saveAsToolStripMenuItem.Text = LangF.THStrsaveAsToolStripMenuItemName;
            editToolStripMenuItem.Text = LangF.THStreditToolStripMenuItemName;
            viewToolStripMenuItem.Text = LangF.THStrviewToolStripMenuItemName;
            optionsToolStripMenuItem.Text = LangF.THStroptionsToolStripMenuItemName;
            helpToolStripMenuItem.Text = LangF.THStrhelpToolStripMenuItemName;
            aboutToolStripMenuItem.Text = LangF.THStraboutToolStripMenuItemName;
            LangF.THReadLanguageFileToStrings();
            THRPGMTransPatchFiles = new BindingList<THRPGMTransPatchFile>();
            //dt = new DataTable();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog THFOpen = new OpenFileDialog
            {
                Filter = "RPGMaker file RPGMKTRANSPATCH|RPGMKTRANSPATCH|Game exe|*.exe|All files (*.*)|*.*"
            };

            if (THFOpen.ShowDialog() == DialogResult.OK)
            {
                if (THFOpen.OpenFile() != null)
                {
                    //Cleaning
                    THFilesListBox.Items.Clear();

                    //Disable menus
                    saveToolStripMenuItem.Enabled = false;
                    saveAsToolStripMenuItem.Enabled = false;
                    editToolStripMenuItem.Enabled = false;
                    viewToolStripMenuItem.Enabled = false;

                    var dir = new DirectoryInfo(Path.GetDirectoryName(THFOpen.FileName));
                    //MessageBox.Show("dir=" + dir);
                    THSelectedDir = dir.ToString();
                    //MessageBox.Show("THSelectedDir=" + THSelectedDir);

                    THSelectedSourceType = GetSourceType(THSelectedDir);
                    //MessageBox.Show("sType=" + sType);

                    if (THSelectedSourceType == "RPGMTransPatch")
                    {
                        //Cleaning of the type
                        THRPGMTransPatchFiles.Clear();

                        //string patchver;
                        var patchdir = dir;
                        if (Directory.Exists(THSelectedDir + "\\patch")) //если есть подпапка patch, тогда это версия патча 3.2
                        {
                            THRPGMTransPatchver = "3.2";
                            patchdir = new DirectoryInfo(Path.GetDirectoryName(THFOpen.FileName) + "\\patch");
                        }
                        else //иначе это версия 2
                        {
                            THRPGMTransPatchver = "2.0";
                        }

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
                        if (OpenRPGMTransPatchFiles(vRPGMTransPatchFiles, THRPGMTransPatchver))
                        {
                            saveToolStripMenuItem.Enabled = true;
                            saveAsToolStripMenuItem.Enabled = true;
                            editToolStripMenuItem.Enabled = true;
                            viewToolStripMenuItem.Enabled = true;
                            MessageBox.Show(THSelectedSourceType + " loaded!");
                        }
                    }
                }
            }
        }

        private string GetSourceType(String sPath)
        {
            string SourceType = "";

            //MessageBox.Show("sPath=" + sPath);
            if (File.Exists(sPath + "\\RPGMKTRANSPATCH"))
            {
                return "RPGMTransPatch";
            }

            return SourceType;
        }

        private int invalidformat;

        public bool OpenRPGMTransPatchFiles(List<string> ListFiles, string patchver = "2.0")
        {
            //MessageBox.Show("ListFiles=" + ListFiles);
            System.IO.StreamReader _file;   //Через что читаем
            string _context = "";           //Комментарий
            string _advice = "";            //Предел длины строки
            string _string;// = "";            //Переменная строки
            string _untrans = "";           //Непереведенный текст
            string _trans = "";             //Переведенный текст
            int _status = 0;             //Статус

            int verok = 0;                  //версия патча
            //THMain Main = new THMain();
            //var Main = (THMain)MainForm;
            //THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            THFileElementsDataGridView.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            //THFilesDataGridView.Columns.Add("Filename", "Text");
            THSourceTextBox.Enabled = false;
            THTargetTextBox.Enabled = false;

            //прогрессбар
            //progressBar.Maximum = ListFiles.Count;
            //progressBar.Value = 0;
            //List<RPGMTransPatchFile> THRPGMTransPatchFiles = new List<RPGMTransPatchFile>();
            //Читаем все файлы
            for (int i = 0; i < ListFiles.Count; i++)   //Обрабатываем всю строку
            {
                _file = new StreamReader(ListFiles[i]); //Задаем файл
                THRPGMTransPatchFiles.Add(new THRPGMTransPatchFile(Path.GetFileNameWithoutExtension(ListFiles[i]), ListFiles[i].ToString(), ""));    //Добaвляем файл

                if (patchver == "3.2")
                {
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        //Код для версии патча 3.2
                        if (_string.StartsWith("> BEGIN STRING"))
                        {
                            invalidformat = 2; //если нашло строку
                            _string = _file.ReadLine();

                            int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            //MessageBox.Show("1.0" + _string);
                            while (!_string.StartsWith("> CONTEXT:"))  //Ждем начало следующего блока
                            {
                                if (untranslines > 0)
                                    _untrans += "\r\n";
                                _untrans += _string;            //Пишем весь текст
                                _string = _file.ReadLine();
                                untranslines++;
                                //MessageBox.Show("1.1"+_string);
                            }
                            //MessageBox.Show("2.1"+_untrans);

                            int contextlines = 0;
                            while (_string.StartsWith("> CONTEXT:"))
                            {
                                if (contextlines > 0)
                                    _context += "\r\n";
                                _context += _string.Replace("> CONTEXT: ", "").Replace(" < UNTRANSLATED", "");// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий
                                //MessageBox.Show("_context ='" + _context + "'");
                                _string = _file.ReadLine();
                                contextlines++;

                                //MessageBox.Show("3"+_string);
                            }
                            //MessageBox.Show("4" + _context);

                            //MessageBox.Show("7.0" + _untrans);

                            int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                            while (!_string.StartsWith("> END"))      //Ждем конец блока
                            {
                                if (translines > 0)
                                    _trans += "\r\n";
                                _trans += _string;
                                _string = _file.ReadLine();
                                translines++;
                                //MessageBox.Show("_string ='" + _string + "'");
                                //MessageBox.Show("5" + _string);
                            }
                            //MessageBox.Show("6" + _trans);

                            //MessageBox.Show("7.1" + _untrans);
                            //_string = _file.ReadLine();

                            /*С условием проверки длины строки просто не загружался перевод, где первая строка была пустая
                            if (_string.Length > 0)                    //Если текст есть, ищем перевод
                            {
                                int translines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной
                                while (!_string.StartsWith("> END"))      //Ждем конец блока
                                {
                                    if (translines > 0)
                                        _trans += "\r\n";
                                    _trans += _string;
                                    _string = _file.ReadLine();
                                    translines++;
                                    //MessageBox.Show("_string ='" + _string + "'");
                                    //MessageBox.Show("5" + _string);
                                }
                                //MessageBox.Show("6" + _trans);

                                //MessageBox.Show("7.1" + _untrans);
                                _string = _file.ReadLine();
                            }*/

                            if (_untrans != "\r\n")
                            {
                                THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));  //Пишем
                            }

                            _context = "";  //Чистим
                            _untrans = "";  //Чистим
                            _trans = "";    //Чистим
                        }
                    }
                    if (invalidformat != 2) //если строки не были опознаны, значит формат неверен
                    {
                        invalidformat = 1;
                    }
                    _file.Close();  //Закрываем файл
                }
                else if (patchver == "2.0")
                {
                    verok = 1; //Версия опознана
                    while (!_file.EndOfStream)   //Читаем до конца
                    {
                        _string = _file.ReadLine();                       //Чтение
                        //Код для версии патча 2.0
                        if (_string.StartsWith("# CONTEXT"))               //Ждем начало блока
                        {
                            invalidformat = 2;//строка найдена, формат верен

                            if (_string.Split(' ')[3] != "itemAttr/UsageMessage")
                            {
                                _context = _string.Replace("# CONTEXT : ", ""); //Сохраняем коментарий

                                _string = _file.ReadLine();

                                if (_string.StartsWith("# ADVICE"))
                                {
                                    _advice = _string.Replace("# ADVICE : ", "");   //Вытаскиваем число предела

                                    _string = _file.ReadLine();
                                    while (!_string.StartsWith("# TRANSLATION"))  //Ждем начало следующего блока
                                    {
                                        _untrans = _untrans + _string + "\r\n";            //Пишем весь текст
                                        _string = _file.ReadLine();
                                    }
                                    if (_untrans.Length > 0)                    //Если текст есть, ищем перевод
                                    {
                                        _string = _file.ReadLine();
                                        while (!_string.StartsWith("# END"))      //Ждем конец блока
                                        {
                                            _trans = _trans + _string + "\r\n";
                                            _string = _file.ReadLine();
                                        }
                                        if (_untrans != "\r\n")
                                        {
                                            THRPGMTransPatchFiles[i].blocks.Add(new Block(_context, _advice, _untrans, _trans, _status));//Пишем
                                        }
                                    }
                                    _untrans = "";  //Чистим
                                    _trans = "";    //Чистим
                                }
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
                    MessageBox.Show(LangF.THStrRPGMTransPatchInvalidVersionMsg);
                    _file.Close();  //Закрываем файл
                    return false;
                }

                if (invalidformat == 1)
                {
                    MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                    invalidformat = 0;
                    return false;
                }

                //progressBar.Value++;
            }

            //MessageBox.Show("111");

            if (verok == 1 & invalidformat != 1)
            {
                //Запись в dataGridVivwer
                for (int i = 0; i < ListFiles.Count; i++)
                {
                    //MessageBox.Show("ListFiles=" + ListFiles[i]);
                    THFilesListBox.Items.Add(THRPGMTransPatchFiles[i].Name);
                    //THFilesDataGridView.Rows.Add();
                    //THFilesDataGridView.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name /*Path.GetFileNameWithoutExtension(ListFiles[i])*/;
                    //dGFiles.Rows.Add();
                    //dGFiles.Rows[i].Cells[0].Value = THRPGMTransPatchFiles[i].Name;
                }
                //ConnnectLinesToGrid(0); //подозрения, что вызывается 2 раза
                //MessageBox.Show("Готово!");
                FVariant = " * RPG Maker Trans Patch " + patchver;

                ActiveForm.Text += FVariant;
            }
            else if (invalidformat == 1)
            {
                MessageBox.Show(LangF.THStrRPGMTransPatchInvalidFormatMsg);
                return false;
            }

            //MessageBox.Show("111");
            //progressBar.Value = 0;
            return true;
        }

        private bool THFilesListBox_MouseClickBusy;

        private void THFilesListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (THFilesListBox_MouseClickBusy)
            {
                //return;
            }
            else
            {
                THFilesListBox_MouseClickBusy = true;

                //Пример с присваиванием Dataset. Они вроде быстро открываются, в отличие от List. Проверка не подтвердила ускорения, всё также
                //https://stackoverflow.com/questions/11099619/how-to-bind-dataset-to-datagridview-in-windows-application

                //THFileElementsDataGridView.DataSource = null;
                //THFileElementsDataGridView.RowCount = 100;

                //Пробовал также отсюда через BindingList
                //https://stackoverflow.com/questions/44433428/how-to-use-virtual-mode-for-large-data-in-datagridview
                //не помогает
                //var dataPopulateList = new BindingList<Block>(THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks);
                //THFileElementsDataGridView.DataSource = dataPopulateList;

                //Еще источник с рекомендацией ниже, но тоже от нее не заметил эффекта
                //https://stackoverflow.com/questions/3580237/best-way-to-fill-datagridview-with-large-amount-of-data
                //THFileElementsDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                //or even better .DisableResizing.
                //Most time consumption enum is DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
                //THFileElementsDataGridView.RowHeadersVisible = false; // set it to false if not needed

                //https://www.codeproject.com/Questions/784355/How-to-solve-performance-issue-in-Datagridview-Min
                THFileElementsDataGridView.DataSource = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks;//.GetRange(0, THRPGMTransPatchFilesFGetCellCount());
                // Поменял List на BindingList и вроде чуть быстрее стало загружаться
                try
                {
                    THFileElementsDataGridView.Columns["Context"].Visible = false;
                    THFileElementsDataGridView.Columns["Original"].Name = LangF.THStrDGOriginalColumnName;
                    THFileElementsDataGridView.Columns["Translation"].Name = LangF.THStrDGTranslationColumnName;
                    THFileElementsDataGridView.Columns[LangF.THStrDGOriginalColumnName].ReadOnly = true;
                    THFileElementsDataGridView.Columns["Status"].Visible = false;
                    if (FVariant == " * RPG Maker Trans Patch 3.2")
                    {
                        THFileElementsDataGridView.Columns["Advice"].Visible = false;
                    }
                }
                catch
                {
                }

                //THFileElementsDataGridView.RowHeadersVisible = true; // set it to false if not needed

                THFilesListBox_MouseClickBusy = false;
            }
        }

        private void THFileElementsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            THSourceTextBox.Enabled = true;
            THTargetTextBox.Enabled = true;
        }

        private void THFileElementsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Считывание значения ячейки в текстовое поле 1, вариант 2, для DataSet, ds.Tables[0]
                if (THSourceTextBox.Enabled && THFileElementsDataGridView.Rows.Count > 0 && e.RowIndex >= 0 && e.ColumnIndex >= 0) //Проверка на размер индексов, для избежания ошибки при попытке сортировки " должен быть положительным числом и его размер не должен превышать размер коллекции"
                {
                    THTargetTextBox.Clear();

                    if (!String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGOriginalColumnName].Value.ToString())) //проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                    {
                        THSourceTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGOriginalColumnName].Value.ToString(); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                                                                                                                                                    //https://github.com/caguiclajmg/WanaKanaSharp
                                                                                                                                                    //if (GetLocaleLangCount(THSourceTextBox.Text, "hiragana") > 0)
                                                                                                                                                    //{
                                                                                                                                                    //    GetWords(THSourceTextBox.Text);
                                                                                                                                                    //   var hepburnConverter = new HepburnConverter();
                                                                                                                                                    //   WanaKana.ToRomaji(hepburnConverter, THSourceTextBox.Text); // hiragana
                                                                                                                                                    //}
                                                                                                                                                    //также по японо ыфуригане
                                                                                                                                                    //https://docs.microsoft.com/en-us/uwp/api/windows.globalization.japanesephoneticanalyzer
                    }
                    if (!String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString())) //проверить, не пуста ли ячейка, иначе была бы ошибка // ошибка при попытке сортировки по столбцу
                    {
                        if (String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString()))
                        {
                            THTargetTextBox.Clear();
                        }

                        THTargetTextBox.Text = THFileElementsDataGridView.Rows[e.RowIndex].Cells[LangF.THStrDGTranslationColumnName].Value.ToString(); //Отображает в первом текстовом поле Оригинал текст из соответствующей ячейки
                    }

                    THInfoTextBox.Text = "";

                    if (!String.IsNullOrEmpty(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                    {
                        //gem furigana
                        //https://github.com/helephant/Gem
                        //var furigana = new Furigana(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                        //THInfoTextBox.Text += furigana.Reading + "\r\n";
                        //THInfoTextBox.Text += furigana.Expression + "\r\n";
                        //THInfoTextBox.Text += furigana.Hiragana + "\r\n";
                        //THInfoTextBox.Text += furigana.ReadingHtml + "\r\n";
                        THInfoTextBox.Text += THShowLangsOfString(THFileElementsDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), "all"); //Show all detected languages count info
                    }
                }
                //--------Считывание значения ячейки в текстовое поле 1
            }
            catch
            {
            }
        }

        //Пример виртуального режима
        //http://www.cyberforum.ru/post9306711.html
        private void THFileElementsDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            //MessageBox.Show("THFileElementsDataGridView_CellValueNeeded");
            //e.Value = THRPGMTransPatchFiles[THFilesListBox.SelectedIndex].blocks[e.RowIndex];
        }

        //Detect languages
        //source: https://stackoverflow.com/questions/15805859/detect-japanese-character-input-and-romajis-ascii
        private static IEnumerable<char> GetCharsInRange(string text, int min, int max)
        {
            //Usage:
            //var romaji = GetCharsInRange(searchKeyword, 0x0020, 0x007E);
            //var hiragana = GetCharsInRange(searchKeyword, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(searchKeyword, 0x30A0, 0x30FF);
            //var kanji = GetCharsInRange(searchKeyword, 0x4E00, 0x9FBF);
            return text.Where(e => e >= min && e <= max);
        }

        private string THShowLangsOfString(string target, string langlocale)
        {
            string ret = "";
            if (langlocale == "all")
            {
                var kanji = GetCharsInRange(target, 0x4E00, 0x9FBF);
                var romaji = GetCharsInRange(target, 0x0020, 0x007E);
                var hiragana = GetCharsInRange(target, 0x3040, 0x309F);
                var katakana = GetCharsInRange(target, 0x30A0, 0x30FF);

                ret += "Contains: \r\n";
                if (romaji.Any())
                {
                    ret += ("       romaji:" + GetLocaleLangCount(target, "romaji") + "\r\n");
                }
                if (kanji.Any())
                {
                    ret += ("       kanji:" + GetLocaleLangCount(target, "kanji") + "\r\n");
                }
                if (hiragana.Any())
                {
                    ret += ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + "\r\n");
                }
                if (katakana.Any())
                {
                    ret += ("       katakana:" + GetLocaleLangCount(target, "katakana") + "\r\n");
                }
                if (target.Length > GetLocaleLangCount(target, "all"))
                {
                    ret += ("       other:" + (target.Length - GetLocaleLangCount(target, "all")) + "\r\n");
                }
            }
            else if (langlocale.ToLower() == "romaji")
            {
                return ("       romaji:" + GetLocaleLangCount(target, "romaji") + "\r\n");
            }
            else if (langlocale.ToLower() == "kanji")
            {
                return ("       kanji:" + GetLocaleLangCount(target, "kanji") + "\r\n");
            }
            else if (langlocale.ToLower() == "hiragana")
            {
                return ("       hiragana:" + GetLocaleLangCount(target, "hiragana") + "\r\n");
            }
            else if (langlocale.ToLower() == "katakana")
            {
                return ("       katakana:" + GetLocaleLangCount(target, "katakana") + "\r\n");
            }
            else if (langlocale.ToLower() == "other")
            {
                return ("       other:" + (GetLocaleLangCount(target, "other")) + "\r\n");
            }

            return ret;
        }

        private int GetLocaleLangCount(string target, string langlocale)
        {
            //var romaji = GetCharsInRange(THSourceTextBox.Text, 0x0020, 0x007E);
            //var kanji = GetCharsInRange(THSourceTextBox.Text, 0x4E00, 0x9FBF);
            //var hiragana = GetCharsInRange(THSourceTextBox.Text, 0x3040, 0x309F);
            //var katakana = GetCharsInRange(THSourceTextBox.Text, 0x30A0, 0x30FF);

            int romaji = (GetCharsInRange(target, 0x0020, 0x007E)).Count();
            int kanji = (GetCharsInRange(target, 0x4E00, 0x9FBF)).Count();
            int hiragana = (GetCharsInRange(target, 0x3040, 0x309F)).Count();
            int katakana = (GetCharsInRange(target, 0x30A0, 0x30FF)).Count();

            int all = (romaji + kanji + hiragana + katakana);
            if (langlocale.ToLower() == "all")
            {
                return (all);
            }
            else if (langlocale.ToLower() == "romaji")
            {
                return (romaji);
            }
            else if (langlocale.ToLower() == "kanji")
            {
                return (kanji);
            }
            else if (langlocale.ToLower() == "hiragana")
            {
                return (hiragana);
            }
            else if (langlocale.ToLower() == "katakana")
            {
                return (katakana);
            }
            else if (langlocale.ToLower() == "other")
            {
                return (target.Length - all);
            }

            return all;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (THSelectedSourceType == "RPGMTransPatch")
            {
                THInfoTextBox.Text = "Saving...";
                SaveRPGMTransPatchFiles(THSelectedDir, THRPGMTransPatchver);
                THInfoTextBox.Text = "";
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog THSaveFolderBrowser = new FolderBrowserDialog
            {
                //MessageBox.Show(dirpath);
                SelectedPath = THSelectedDir //Установить начальный путь на тот, что был установлен при открытии.
            };

            if (THSaveFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                if (THSelectedSourceType == "RPGMTransPatch")
                {
                    if (SaveRPGMTransPatchFiles(THSaveFolderBrowser.SelectedPath, THRPGMTransPatchver))
                    {
                        THSelectedDir = THSaveFolderBrowser.SelectedPath;
                        MessageBox.Show("Сохранение завершено!");
                    }
                }
            }
        }

        public bool SaveRPGMTransPatchFiles(string SelectedDir, string patchver = "2.0")
        {
            string buffer;

            //Прогресс
            //pbstatuslabel.Visible = true;
            //pbstatuslabel.Text = "сохранение..";
            //progressBar.Maximum = 0;
            //for (int i = 0; i < ArrayTransFilses.Count; i++)
            //    for (int y = 0; y < ArrayTransFilses[i].blocks.Count; y++)
            //        progressBar.Maximum = progressBar.Maximum + ArrayTransFilses[i].blocks.Count;
            //MessageBox.Show(progressBar.Maximum.ToString());
            //progressBar.Value = 0;

            if (patchver == "3.2")
            {
                //запись в файл RPGMKTRANSPATCH строки > RPGMAKER TRANS PATCH V3
                //StreamWriter RPGMKTRANSPATCHwriter = new StreamWriter("RPGMKTRANSPATCH", true);
                //RPGMKTRANSPATCHwriter.WriteLine("> RPGMAKER TRANS PATCH V3");
                //RPGMKTRANSPATCHwriter.Close();

                for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                {
                    buffer = "> RPGMAKER TRANS PATCH FILE VERSION 3.2\r\n";
                    for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                    {
                        buffer += "> BEGIN STRING\r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Original + "\r\n";
                        //MessageBox.Show("1: " + ArrayTransFilses[i].blocks[y].Trans);
                        //MessageBox.Show("2: " + ArrayTransFilses[i].blocks[y].Context);
                        string[] str = THRPGMTransPatchFiles[i].blocks[y].Context.Split('\n');
                        //string str1 = "";
                        for (int g = 0; g < str.Count(); g++)
                        {
                            if (str.Count() > 1)
                            {
                                str[g] = str[g].Replace("\r", "");//очистка от знака переноса в отдельную переменную
                                buffer += "> CONTEXT: " + str[g] + "\r\n";
                            }
                            else
                            {
                                str[g] = str[g].Replace("\r", "");//очистка от знака переноса в отдельную переменную
                                if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == "\r\n")
                                {
                                    buffer += "> CONTEXT: " + str[g] + " < UNTRANSLATED\r\n";
                                }
                                else
                                {
                                    buffer += "> CONTEXT: " + str[g] + "\r\n";
                                }
                            }
                        }
                        //buffer += "\r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Translation + "\r\n";
                        buffer += "> END STRING\r\n\r\n";

                        //progressBar.Value++;
                        //MessageBox.Show(progressBar.Value.ToString());
                    }

                    if (!String.IsNullOrWhiteSpace(buffer))
                    {
                        if (!Directory.Exists(SelectedDir + "\\patch"))
                        {
                            Directory.CreateDirectory(SelectedDir + "\\patch");
                        }
                        String _path = SelectedDir + "\\patch\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                        File.WriteAllText(_path, buffer);
                        buffer = "";
                    }
                }
            }
            else if (patchver == "2.0")
            {
                for (int i = 0; i < THRPGMTransPatchFiles.Count; i++)
                {
                    buffer = "# RPGMAKER TRANS PATCH FILE VERSION 2.0\r\n";
                    for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                    {
                        buffer = buffer + "# TEXT STRING\r\n";
                        if (THRPGMTransPatchFiles[i].blocks[y].Translation == "\r\n")
                            buffer += "# UNTRANSLATED\r\n";
                        buffer += "# CONTEXT : " + THRPGMTransPatchFiles[i].blocks[y].Context + "\r\n";
                        buffer += "# ADVICE : " + THRPGMTransPatchFiles[i].blocks[y].Advice + "\r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Original;
                        buffer += "# TRANSLATION \r\n";
                        buffer += THRPGMTransPatchFiles[i].blocks[y].Translation;
                        buffer += "# END STRING\r\n\r\n";
                    }
                    if (!String.IsNullOrWhiteSpace(buffer))
                    {
                        String _path = SelectedDir + "\\" + THRPGMTransPatchFiles[i].Name + ".txt";
                        File.WriteAllText(_path, buffer);
                        buffer = "";
                    }
                }
            }
            //pbstatuslabel.Visible = false;
            //pbstatuslabel.Text = string.Empty;
            return true;
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            THAboutForm AboutForm = new THAboutForm();
            AboutForm.Show();
        }
    }

    //Материалы
    //Сортировка при виртуальном режиме DatagridView
    //http://qaru.site/questions/1486005/c-datagridview-virtual-mode-enable-sorting
}