using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TranslationHelper.Data;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.WolfRPG
{
    class WrpgoldOpen
    {
        
        public WrpgoldOpen()
        {
            
        }

        internal string AnyTxt(string sPath)
        {
            string folderPath = Path.GetDirectoryName(sPath);
            string folderName = Path.GetFileName(folderPath);

            if (Path.GetFileName(sPath) == "GameDat.txt")
            {
                using (StreamReader sr = new StreamReader(sPath))
                {
                    if (sr.ReadLine() == "> WOLF TRANS PATCH FILE VERSION 1.0")
                    {
                        return OpenWolfTransPatch(Path.GetDirectoryName(Path.GetDirectoryName(folderPath)));
                    }
                }
            }
            else if (folderName == "TextE" || folderName == "TextH" || folderName == "TextP")
            {
                string parentFolder = Path.GetDirectoryName(folderPath);

                string folder = Path.Combine(parentFolder, "TextE");
                ProceedTextEhpFolders(folder);

                folder = Path.Combine(parentFolder, "TextH");
                ProceedTextEhpFolders(folder);

                folder = Path.Combine(parentFolder, "TextP");
                ProceedTextEhpFolders(folder);

                ProjectData.SelectedDir = parentFolder;

                return ProjectData.Main.THFilesList.Items.Count > 0 ? "Wolf RPG txt" : string.Empty;
            }
            return string.Empty;
        }

        private string OpenWolfTransPatch(string folderPath)
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

            string context = string.Empty;           //Комментарий
            //string _advice = string.Empty;            //Предел длины строки
            string @string;// = string.Empty;            //Переменная строки
            string original = string.Empty;           //Непереведенный текст
            string translation = string.Empty;             //Переведенный текст
            //int _status = 0;             //Статус

            int errorsCount = 0;
            //Читаем все файлы
            foreach (var txtFile in Directory.EnumerateFiles(folderPath, "*.txt", SearchOption.AllDirectories))
            {
                try
                {
                    string fname = Path.GetFileName(txtFile);
                    ProjectData.Main.ProgressInfo(true, T._("opening file: ") + fname + ".txt");
                    using (StreamReader file = new StreamReader(txtFile))
                    {
                        ProjectData.ThFilesElementsDataset.Tables.Add(fname);
                        ProjectData.ThFilesElementsDataset.Tables[fname].Columns.Add("Original");
                        ProjectData.ThFilesElementsDataset.Tables[fname].Columns.Add("Translation");
                        //THFilesElementsDataset.Tables[fname].Columns.Add("Context");
                        //THFilesElementsDataset.Tables[fname].Columns.Add("Advice");
                        //THFilesElementsDataset.Tables[fname].Columns.Add("Status");

                        ProjectData.ThFilesElementsDatasetInfo.Tables.Add(fname);
                        ProjectData.ThFilesElementsDatasetInfo.Tables[fname].Columns.Add("Original");

                        while (!file.EndOfStream)   //Читаем до конца
                        {
                            @string = file.ReadLine();                       //Чтение

                            if (@string.StartsWith("> BEGIN STRING"))
                            {
                                @string = file.ReadLine();

                                int untranslines = 0; //счетчик количества проходов по строкам текста для перевода, для записи переносов, если строк больше одной                            
                                while (!@string.StartsWith("> CONTEXT"))  //Ждем начало следующего блока
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
                                while (@string.StartsWith("> CONTEXT") || @string.EndsWith("< UNTRANSLATED"))
                                {
                                    if (!@string.StartsWith("> CONTEXT") && @string.EndsWith("< UNTRANSLATED"))
                                    {
                                        context += @string.Replace(" < UNTRANSLATED", string.Empty);
                                    }
                                    else
                                    {
                                        if (contextlines > 0)
                                        {
                                            context += Environment.NewLine;
                                        }

                                        context += @string.Replace("> CONTEXT ", string.Empty).Replace(" < UNTRANSLATED", string.Empty);// +"\r\n";//Убрал символ переноса, так как он остается при сохранении //Сохраняем коментарий

                                    }

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
                                    ProjectData.ThFilesElementsDataset.Tables[fname].Rows.Add(original, translation/*, _context, _advice, _status*/);
                                    ProjectData.ThFilesElementsDatasetInfo.Tables[fname].Rows.Add(context/* + Environment.NewLine + "Advice:" + Environment.NewLine + _advice*/);
                                }

                                context = string.Empty;  //Чистим
                                original = string.Empty;  //Чистим
                                translation = string.Empty;    //Чистим
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
                if (ProjectData.ThFilesElementsDataset.Tables.Count == 0)
                {
                    return string.Empty;
                }
            }

            foreach (DataTable table in ProjectData.ThFilesElementsDataset.Tables)
            {
                ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(table.TableName)));
            }

            ProjectData.SelectedDir = folderPath;
            return "WOLF TRANS PATCH";
        }

        private void ProceedTextEhpFolders(string folder)
        {
            string folderName = Path.GetFileName(folder);
            foreach (var txtFile in Directory.EnumerateFiles(folder, "*.txt"))
            {
                string txtFilename = Path.GetFileName(txtFile);
                if (folderName == "TextE" && txtFilename.Length > 0 && !Regex.IsMatch(txtFilename.Substring(0, 1), @"[0-9]"))
                {
                    continue;
                }

                ProjectData.ThFilesElementsDataset.Tables.Add(txtFilename);
                ProjectData.ThFilesElementsDatasetInfo.Tables.Add(txtFilename);
                ProjectData.ThFilesElementsDataset.Tables[txtFilename].Columns.Add("Original");
                ProjectData.ThFilesElementsDatasetInfo.Tables[txtFilename].Columns.Add("Original");

                OpenWolfRpgMakerTextEhp(txtFile);

                if (ProjectData.ThFilesElementsDatasetInfo.Tables[txtFilename] == null || ProjectData.ThFilesElementsDatasetInfo.Tables[txtFilename].Rows.Count == 0)
                {
                    ProjectData.ThFilesElementsDataset.Tables.Remove(txtFilename);
                    ProjectData.ThFilesElementsDatasetInfo.Tables.Remove(txtFilename);
                }
                else
                {
                    ProjectData.ThFilesElementsDataset.Tables[txtFilename].Columns.Add("Translation");
                    ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.Items.Add(txtFilename)));
                }
            }
        }

        private void OpenWolfRpgMakerTextEhp(string sPath)
        {
            string folderPath = Path.GetDirectoryName(sPath);
            string folderName = Path.GetFileName(folderPath);
            string fileName = Path.GetFileName(sPath);

            if (folderName == "TextE" || folderName == "TextH")
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
                            if (line.Length > 0 && !line.StartsWith("/") && !line.StartsWith("END") && !FunctionsRomajiKana.LocalePercentIsNotValid(line))
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
                                ProjectData.ThFilesElementsDataset.Tables[fileName].Rows.Add(sb.ToString());
                                ProjectData.ThFilesElementsDatasetInfo.Tables[fileName].Rows.Add(folderName);
                                recordstarted = false;
                                sb.Clear();
                                cnt = 0;
                            }
                        }
                        else
                        {
                            if (line.Length > 0 && !line.StartsWith("/") && !line.StartsWith("END") && !FunctionsRomajiKana.LocalePercentIsNotValid(line))
                            {
                                sb.Append(line);
                                cnt++;
                                recordstarted = true;
                            }
                        }
                    }
                }
            }
            else if (folderName == "TextP")
            {
                ProjectData.ThFilesElementsDataset.Tables[fileName].Rows.Add(File.ReadAllText(sPath, Encoding.GetEncoding(932)));
                ProjectData.ThFilesElementsDatasetInfo.Tables[fileName].Rows.Add(folderName);
            }
        }

        internal void ProceedWriteWolfRpGtxt()
        {
            for (int t = 0; t < ProjectData.ThFilesElementsDataset.Tables.Count; t++)
            {
                string filePath = Path.Combine(ProjectData.SelectedDir, ProjectData.ThFilesElementsDatasetInfo.Tables[t].Rows[0][0].ToString(), ProjectData.ThFilesElementsDataset.Tables[t].TableName);

                WriteWolfRpgMakerTextEhp(filePath);
                //for (int r=0;r< THFilesElementsDataset.Tables[t].Rows.Count; r++)
                //{

                //}
            }
        }

        private void WriteWolfRpgMakerTextEhp(string sPath)
        {
            string folderPath = Path.GetDirectoryName(sPath);
            string folderName = Path.GetFileName(folderPath);
            string fileName = Path.GetFileName(sPath);

            if (folderName == "TextE" || folderName == "TextH")
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

                        if (line.Length == 0 || line.StartsWith("/") || line.StartsWith("END") || FunctionsRomajiKana.LocalePercentIsNotValid(line))
                        {
                            sb.AppendLine(line);
                        }
                        else
                        {
                            string original = ProjectData.ThFilesElementsDataset.Tables[fileName].Rows[r][0].ToString();
                            string translation = ProjectData.ThFilesElementsDataset.Tables[fileName].Rows[r][1] + string.Empty;
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
            else if (folderName == "TextP")
            {
                File.WriteAllText(sPath, ProjectData.ThFilesElementsDataset.Tables[fileName].Rows[0][1] + string.Empty, Encoding.GetEncoding(932));
            }
        }

        internal void WriteWolftranspatch()
        {
            foreach (var file in Directory.EnumerateFiles(ProjectData.SelectedDir, "*.txt", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(file);
                if (ProjectData.ThFilesElementsDataset.Tables[fileName] == null)
                {
                    continue;
                }

                try
                {
                    StringBuilder buffer = new StringBuilder();

                    int originalcolumnindex = ProjectData.ThFilesElementsDataset.Tables[fileName].Columns["Original"].Ordinal;
                    int translationcolumnindex = ProjectData.ThFilesElementsDataset.Tables[fileName].Columns["Translation"].Ordinal;
                    //int contextcolumnindex = THFilesElementsDatasetInfo.Tables[fileName].Columns["Context"].Ordinal;

                    ProjectData.Main.ProgressInfo(true, T._("saving file: ") + fileName);

                    buffer.AppendLine("> WOLF TRANS PATCH FILE VERSION 1.0");// + Environment.NewLine);
                                                                             //for (int y = 0; y < THRPGMTransPatchFiles[i].blocks.Count; y++)
                    for (int r = 0; r < ProjectData.ThFilesElementsDataset.Tables[fileName].Rows.Count; r++)
                    {
                        string original = ProjectData.ThFilesElementsDataset.Tables[fileName].Rows[r][originalcolumnindex] as string;
                        buffer.AppendLine("> BEGIN STRING");// + Environment.NewLine);
                                                            //buffer += THRPGMTransPatchFiles[i].blocks[y].Original + Environment.NewLine;
                        buffer.AppendLine(original);// + Environment.NewLine);
                        string[] context = (ProjectData.ThFilesElementsDatasetInfo.Tables[fileName].Rows[r][0] + string.Empty).Split(new string[1] { Environment.NewLine }, StringSplitOptions.None/*'\n'*/);
                        //string str1 = string.Empty;
                        string translation = ProjectData.ThFilesElementsDataset.Tables[fileName].Rows[r][translationcolumnindex] + string.Empty;
                        for (int g = 0; g < context.Length; g++)
                        {
                            /*CONTEXT[g] = CONTEXT[g].Replace("\r", string.Empty);*///очистка от знака переноса, возникающего после разбития на строки по \n
                            if (context.Length > 1)
                            {
                                buffer.AppendLine("> CONTEXT " + context[g]);// + Environment.NewLine);
                            }
                            else
                            {   //if (String.IsNullOrEmpty(THRPGMTransPatchFiles[i].blocks[y].Translation)) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                if (translation.Length == 0 || translation == original) //if (ArrayTransFilses[i].blocks[y].Trans == Environment.NewLine)
                                {
                                    buffer.AppendLine("> CONTEXT " + context[g] + " < UNTRANSLATED");// + Environment.NewLine);
                                }
                                else
                                {
                                    buffer.AppendLine("> CONTEXT " + context[g]);// + Environment.NewLine);
                                }
                            }
                        }

                        buffer.AppendLine(translation);// + Environment.NewLine);
                        buffer.AppendLine("> END STRING" + Environment.NewLine);// + Environment.NewLine);
                    }

                    if (string.IsNullOrWhiteSpace(buffer.ToString()))
                    {
                    }
                    else
                    {
                        buffer.Remove(buffer.Length - 2, 2);//удаление лишнего символа \r\n с конца строки

                        string path = file;

                        File.WriteAllText(path, buffer.ToString());
                        //buffer = string.Empty;
                    }
                    buffer.Clear();
                }
                catch
                {
                    ProjectData.Main.ProgressInfo(false, string.Empty);
                    ProjectData.Main.SaveInAction = false;
                }
                finally
                {
                    ProjectData.Main.ProgressInfo(false, string.Empty);
                }

            }


            ProjectData.Main.SaveInAction = false;
        }
    }
}
