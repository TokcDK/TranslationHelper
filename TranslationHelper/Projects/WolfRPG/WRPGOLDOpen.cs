using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.WolfRPG
{
    class WRPGOLDOpen
    {
        readonly THDataWork thDataWork;
        public WRPGOLDOpen(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal string AnyTxt(string sPath)
        {
            string FolderPath = Path.GetDirectoryName(sPath);
            string FolderName = Path.GetFileName(FolderPath);

            if (Path.GetFileName(sPath) == "GameDat.txt")
            {
                using (StreamReader sr = new StreamReader(sPath))
                {
                    if (sr.ReadLine() == "> WOLF TRANS PATCH FILE VERSION 1.0")
                    {
                        return OpenWolfTransPatch(Path.GetDirectoryName(Path.GetDirectoryName(FolderPath)));
                    }
                }
            }
            else if (FolderName == "TextE" || FolderName == "TextH" || FolderName == "TextP")
            {
                string parentFolder = Path.GetDirectoryName(FolderPath);

                string Folder = Path.Combine(parentFolder, "TextE");
                ProceedTextEHPFolders(Folder);

                Folder = Path.Combine(parentFolder, "TextH");
                ProceedTextEHPFolders(Folder);

                Folder = Path.Combine(parentFolder, "TextP");
                ProceedTextEHPFolders(Folder);

                Properties.Settings.Default.THSelectedDir = parentFolder;

                return thDataWork.Main.THFilesList.Items.Count > 0 ? "Wolf RPG txt" : string.Empty;
            }
            return string.Empty;
        }

        private string OpenWolfTransPatch(string FolderPath)
        {
            //foreach (var txtFile in Directory.GetFiles(FolderPath, "*.txt", SearchOption.AllDirectories))
            //{
            //    string txtFilename = Path.GetFileName(txtFile);
            //    THFilesElementsDataset.Tables.Add(txtFilename);
            //    THFilesElementsDatasetInfo.Tables.Add(txtFilename);
            //    THFilesElementsDataset.Tables[txtFilename].Columns.Add("Original");
            //    THFilesElementsDatasetInfo.Tables[txtFilename].Columns.Add("Original");

            //    string line;
            //    using (StreamReader sr = new StreamReader(txtFile))
            //    {

            //    }
            //}

            string _context = string.Empty;           //Комментарий
            //string _advice = string.Empty;            //Предел длины строки
            string _string;// = string.Empty;            //Переменная строки
            string _original = string.Empty;           //Непереведенный текст
            string _translation = string.Empty;             //Переведенный текст
            //int _status = 0;             //Статус

            int errorsCount = 0;
            //Читаем все файлы
            foreach (var txtFile in Directory.EnumerateFiles(FolderPath, "*.txt", SearchOption.AllDirectories))
            {
                try
                {
                    string fname = Path.GetFileName(txtFile);
                    thDataWork.Main.ProgressInfo(true, T._("opening file: ") + fname + ".txt");
                    using (StreamReader _file = new StreamReader(txtFile))
                    {
                        thDataWork.THFilesElementsDataset.Tables.Add(fname);
                        thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Original");
                        thDataWork.THFilesElementsDataset.Tables[fname].Columns.Add("Translation");
                        //THFilesElementsDataset.Tables[fname].Columns.Add("Context");
                        //THFilesElementsDataset.Tables[fname].Columns.Add("Advice");
                        //THFilesElementsDataset.Tables[fname].Columns.Add("Status");

                        thDataWork.THFilesElementsDatasetInfo.Tables.Add(fname);
                        thDataWork.THFilesElementsDatasetInfo.Tables[fname].Columns.Add("Original");

                        while (!_file.EndOfStream)   //Читаем до конца
                        {
                            _string = _file.ReadLine();                       //Чтение

                            if (_string.StartsWith("> BEGIN STRING"))
                            {
                                _string = _file.ReadLine();

                                int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной                            
                                while (!_string.StartsWith("> CONTEXT"))  //Ждем начало следующего блока
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
                                while (_string.StartsWith("> CONTEXT") || _string.EndsWith("< UNTRANSLATED"))
                                {
                                    if (!_string.StartsWith("> CONTEXT") && _string.EndsWith("< UNTRANSLATED"))
                                    {
                                        _context += _string.Replace(" < UNTRANSLATED", string.Empty);
                                    }
                                    else
                                    {
                                        if (contextlines > 0)
                                        {
                                            _context += Environment.NewLine;
                                        }

                                        _context += _string.Replace("> CONTEXT ", string.Empty).Replace(" < UNTRANSLATED", string.Empty);// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий

                                    }

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
                                    thDataWork.THFilesElementsDataset.Tables[fname].Rows.Add(_original, _translation/*, _context, _advice, _status*/);
                                    thDataWork.THFilesElementsDatasetInfo.Tables[fname].Rows.Add(_context/* + Environment.NewLine + "Advice:" + Environment.NewLine + _advice*/);
                                }

                                _context = string.Empty;  //Чистим
                                _original = string.Empty;  //Чистим
                                _translation = string.Empty;    //Чистим
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            if (errorsCount > 0)
            {
                if (thDataWork.THFilesElementsDataset.Tables.Count == 0)
                {
                    return string.Empty;
                }
            }

            foreach (DataTable table in thDataWork.THFilesElementsDataset.Tables)
            {
                thDataWork.Main.THFilesList.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(table.TableName)));
            }

            Properties.Settings.Default.THSelectedDir = FolderPath;
            return "WOLF TRANS PATCH";
        }

        private void ProceedTextEHPFolders(string Folder)
        {
            string FolderName = Path.GetFileName(Folder);
            foreach (var txtFile in Directory.EnumerateFiles(Folder, "*.txt"))
            {
                string txtFilename = Path.GetFileName(txtFile);
                if (FolderName == "TextE" && txtFilename.Length > 0 && !Regex.IsMatch(txtFilename.Substring(0, 1), @"[0-9]"))
                {
                    continue;
                }

                thDataWork.THFilesElementsDataset.Tables.Add(txtFilename);
                thDataWork.THFilesElementsDatasetInfo.Tables.Add(txtFilename);
                thDataWork.THFilesElementsDataset.Tables[txtFilename].Columns.Add("Original");
                thDataWork.THFilesElementsDatasetInfo.Tables[txtFilename].Columns.Add("Original");

                OpenWolfRPGMakerTextEHP(txtFile);

                if (thDataWork.THFilesElementsDatasetInfo.Tables[txtFilename] == null || thDataWork.THFilesElementsDatasetInfo.Tables[txtFilename].Rows.Count == 0)
                {
                    thDataWork.THFilesElementsDataset.Tables.Remove(txtFilename);
                    thDataWork.THFilesElementsDatasetInfo.Tables.Remove(txtFilename);
                }
                else
                {
                    thDataWork.THFilesElementsDataset.Tables[txtFilename].Columns.Add("Translation");
                    thDataWork.Main.THFilesList.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(txtFilename)));
                }
            }
        }

        private void OpenWolfRPGMakerTextEHP(string sPath)
        {
            string FolderPath = Path.GetDirectoryName(sPath);
            string FolderName = Path.GetFileName(FolderPath);
            string FileName = Path.GetFileName(sPath);

            if (FolderName == "TextE" || FolderName == "TextH")
            {
                //THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                using (StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    bool recordstarted = false;
                    StringBuilder sb = new StringBuilder();
                    int cnt = 0;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        if (recordstarted)
                        {
                            if (line.Length > 0 && !line.StartsWith("/") && !line.StartsWith("END") && !FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(line))
                            {
                                if (cnt > 0)
                                {
                                    sb.Append(Environment.NewLine);
                                }
                                sb.Append(line);
                                cnt++;
                            }
                            else
                            {
                                thDataWork.THFilesElementsDataset.Tables[FileName].Rows.Add(sb.ToString());
                                thDataWork.THFilesElementsDatasetInfo.Tables[FileName].Rows.Add(FolderName);
                                recordstarted = false;
                                sb.Clear();
                                cnt = 0;
                            }
                        }
                        else
                        {
                            if (line.Length > 0 && !line.StartsWith("/") && !line.StartsWith("END") && !FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(line))
                            {
                                sb.Append(line);
                                cnt++;
                                recordstarted = true;
                            }
                        }
                    }
                }
            }
            else if (FolderName == "TextP")
            {
                thDataWork.THFilesElementsDataset.Tables[FileName].Rows.Add(File.ReadAllText(sPath, Encoding.GetEncoding(932)));
                thDataWork.THFilesElementsDatasetInfo.Tables[FileName].Rows.Add(FolderName);
            }
        }

        internal void ProceedWriteWolfRPGtxt()
        {
            for (int t = 0; t < thDataWork.THFilesElementsDataset.Tables.Count; t++)
            {
                string FilePath = Path.Combine(Properties.Settings.Default.THSelectedDir, thDataWork.THFilesElementsDatasetInfo.Tables[t].Rows[0][0].ToString(), thDataWork.THFilesElementsDataset.Tables[t].TableName);

                WriteWolfRPGMakerTextEHP(FilePath);
                //for (int r=0;r< THFilesElementsDataset.Tables[t].Rows.Count; r++)
                //{

                //}
            }
        }

        private void WriteWolfRPGMakerTextEHP(string sPath)
        {
            string FolderPath = Path.GetDirectoryName(sPath);
            string FolderName = Path.GetFileName(FolderPath);
            string FileName = Path.GetFileName(sPath);

            if (FolderName == "TextE" || FolderName == "TextH")
            {
                StringBuilder sb = new StringBuilder();
                //THFilesElementsDataset.Tables[0].Columns.Add("Translation");
                using (StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    int cnt = 0;
                    int r = 0;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        if (cnt > 0)
                        {
                            cnt--;
                            continue;
                        }

                        if (line.Length == 0 || line.StartsWith("/") || line.StartsWith("END") || FunctionsRomajiKana.SelectedLocalePercentFromStringIsNotValid(line))
                        {
                            sb.AppendLine(line);
                        }
                        else
                        {
                            string original = thDataWork.THFilesElementsDataset.Tables[FileName].Rows[r][0].ToString();
                            string translation = thDataWork.THFilesElementsDataset.Tables[FileName].Rows[r][1] + string.Empty;
                            sb.AppendLine(translation.Length > 0 ? translation : original);
                            r++;
                            //пропустить то же количество строк
                            //cnt равно количеству строк
                            cnt = (original.Length - original.Replace(Environment.NewLine, string.Empty).Length) / Environment.NewLine.Length;
                            continue;
                        }
                    }
                }

                File.WriteAllText(sPath, sb.ToString(), Encoding.GetEncoding(932));
            }
            else if (FolderName == "TextP")
            {
                File.WriteAllText(sPath, thDataWork.THFilesElementsDataset.Tables[FileName].Rows[0][1] + string.Empty, Encoding.GetEncoding(932));
            }
        }

        internal void WriteWOLFTRANSPATCH()
        {
            foreach (var file in Directory.EnumerateFiles(Properties.Settings.Default.THSelectedDir, "*.txt", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(file);
                if (thDataWork.THFilesElementsDataset.Tables[fileName] == null)
                {
                    continue;
                }

                try
                {
                    StringBuilder buffer = new StringBuilder();

                    int originalcolumnindex = thDataWork.THFilesElementsDataset.Tables[fileName].Columns["Original"].Ordinal;
                    int translationcolumnindex = thDataWork.THFilesElementsDataset.Tables[fileName].Columns["Translation"].Ordinal;
                    //int contextcolumnindex = THFilesElementsDatasetInfo.Tables[fileName].Columns["Context"].Ordinal;

                    thDataWork.Main.ProgressInfo(true, T._("saving file: ") + fileName);

                    buffer.AppendLine("> WOLF TRANS PATCH FILE VERSION 1.0");// + Environment.NewLine);
                                                                             //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                    for (int r = 0; r < thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Count; r++)
                    {
                        string ORIGINAL = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r][originalcolumnindex] as string;
                        buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                                                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                        buffer.AppendLine(ORIGINAL);// + Environment.NewLine);
                        string[] CONTEXT = (thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows[r][0] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                        //string str1 = string.Empty;
                        string TRANSLATION = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[r][translationcolumnindex] + string.Empty;
                        for (int g = 0; g < CONTEXT.Length; g++)
                        {
                            /*CONTEXT[g] = CONTEXT[g].Replace("\r", string.Empty);*///очистка от знака переноса, возникающего после разбития на строки по \n
                            if (CONTEXT.Length > 1)
                            {
                                buffer.AppendLine("> CONTEXT " + CONTEXT[g]);// + Environment.NewLine);
                            }
                            else
                            {   //if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                if (TRANSLATION.Length == 0 || TRANSLATION == ORIGINAL) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                {
                                    buffer.AppendLine("> CONTEXT " + CONTEXT[g] + " < UNTRANSLATED");// + Environment.NewLine);
                                }
                                else
                                {
                                    buffer.AppendLine("> CONTEXT " + CONTEXT[g]);// + Environment.NewLine);
                                }
                            }
                        }

                        buffer.AppendLine(TRANSLATION);// + Environment.NewLine);
                        buffer.AppendLine("> END STRING" + Environment.NewLine);// + Environment.NewLine);
                    }

                    if (string.IsNullOrWhiteSpace(buffer.ToString()))
                    {
                    }
                    else
                    {
                        buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки

                        string _path = file;

                        File.WriteAllText(_path, buffer.ToString());
                        //buffer = string.Empty;
                    }
                    buffer.Clear();
                }
                catch
                {
                    thDataWork.Main.ProgressInfo(false, string.Empty);
                    thDataWork.Main.SaveInAction = false;
                }
                finally
                {
                    thDataWork.Main.ProgressInfo(false, string.Empty);
                }

            }


            thDataWork.Main.SaveInAction = false;
        }
    }
}
