using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Formats.KiriKiri;

namespace TranslationHelper.Projects.KiriKiri
{
    [Obsolete]
    class KiriKiriOLD
    {
        
        public KiriKiriOLD()
        {
            
        }

        internal bool OpenDetect()
        {
            return
                ProjectData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".KS")
                ||
                ProjectData.SelectedFilePath.ToUpper(CultureInfo.InvariantCulture).EndsWith(".SCN")
                ;
        }

        internal string KiriKiriScriptScenario()
        {
            string filename = Path.GetFileNameWithoutExtension(ProjectData.SelectedFilePath);
            string extension = Path.GetExtension(ProjectData.SelectedFilePath);

            _ = ProjectData.FilesContent.Tables.Add(filename);
            _ = ProjectData.FilesContent.Tables[filename].Columns.Add(THSettings.OriginalColumnName());
            _ = ProjectData.FilesContentInfo.Tables.Add(filename);
            _ = ProjectData.FilesContentInfo.Tables[filename].Columns.Add(THSettings.OriginalColumnName());

            DataTable DT = KiriKiriScriptScenarioOpen(ProjectData.SelectedFilePath, ProjectData.FilesContent.Tables[0], ProjectData.FilesContentInfo.Tables[0]);
            if (DT == null || DT.Rows.Count == 0)
            {
                ProjectData.FilesContent.Tables.Remove(filename);
                ProjectData.FilesContentInfo.Tables.Remove(filename);
            }
            else
            {
                _ = ProjectData.FilesContent.Tables[0].Columns.Add(THSettings.TranslationColumnName());
                ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(filename)));
                if (extension == ".ks")
                {
                    return "KiriKiri script";
                }
                else if (extension == ".scn")
                {
                    return "KiriKiri script";
                }
            }
            return string.Empty;
        }

        internal bool KiriKiriGame()
        {
            bool ret = false;
            if (XP3.ExtractXP3files(ProjectData.SelectedFilePath))
            {
                var KiriKiriFiles = new List<string>();
                string DirName = Path.GetFileName(Path.GetDirectoryName(ProjectData.SelectedFilePath));
                string KiriKiriWorkFolder = Path.Combine(Application.StartupPath, "Work", "KiriKiri", DirName);

                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.scn", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.ks", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.csv", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.tsv", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*.tjs", SearchOption.AllDirectories))
                {
                    KiriKiriFiles.Add(file.FullName);
                }
                //foreach (FileInfo file in (new DirectoryInfo(KiriKiriWorkFolder)).GetFiles("*", SearchOption.AllDirectories))
                //{
                //    if (file.Extension == ".ks" || file.Extension == ".scn")
                //    {
                //    }
                //    else
                //    {
                //        File.Delete(file.FullName);
                //    }
                //}

                if (KiriKiriGameOpen(KiriKiriFiles))
                {
                    return true;
                }
            }
            return ret;
        }

        private bool KiriKiriGameOpen(List<string> kiriKiriFiles)
        {
            bool ret = false;
            string filename;

            try
            {
                for (int i = 0; i < kiriKiriFiles.Count; i++)
                {
                    filename = Path.GetFileName(kiriKiriFiles[i]);

                    _ = ProjectData.FilesContent.Tables.Add(filename);
                    _ = ProjectData.FilesContent.Tables[filename].Columns.Add(THSettings.OriginalColumnName());
                    _ = ProjectData.FilesContentInfo.Tables.Add(filename);
                    _ = ProjectData.FilesContentInfo.Tables[filename].Columns.Add(THSettings.OriginalColumnName());

                    DataTable DT = null;

                    if (filename.EndsWith(".ks") || filename.EndsWith(".scn") || filename.EndsWith(".tjs"))
                    {
                        DT = KiriKiriScriptScenarioOpen(kiriKiriFiles[i], ProjectData.FilesContent.Tables[filename], ProjectData.FilesContentInfo.Tables[filename]);
                    }
                    else if (filename.EndsWith(".csv"))
                    {
                        DT = CSVOld.KiriKiriCSVOpen(kiriKiriFiles[i], ProjectData.FilesContent.Tables[filename], ProjectData.FilesContentInfo.Tables[filename]);
                    }
                    else if (filename.EndsWith(".tsv"))
                    {
                        DT = TSVOld.KiriKiriTSVOpen(kiriKiriFiles[i], ProjectData.FilesContent.Tables[filename], ProjectData.FilesContentInfo.Tables[filename]);
                    }

                    if (DT == null || DT.Rows.Count == 0)
                    {
                        ProjectData.FilesContent.Tables.Remove(filename);
                        ProjectData.FilesContentInfo.Tables.Remove(filename);
                    }
                    else
                    {
                        ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(filename)));
                        _ = ProjectData.FilesContent.Tables[filename].Columns.Add(THSettings.TranslationColumnName());
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ret;
        }

        string KiriKiriVariableSearchRegexPattern = string.Empty;
        string KiriKiriVariableSearchRegexFullPattern = string.Empty;
        string KiriKiriQuotePattern;

        private DataTable KiriKiriScriptScenarioOpen(string sPath, DataTable DT = null, DataTable DTInfo = null)
        {
            try
            {
                //ks scn tjs open
                KiriKiriVariableSearchRegexPattern = @"( |'|\(|\[|,|\.)o\.";
                KiriKiriVariableSearchRegexFullPattern = @"((^o\.)|(" + KiriKiriVariableSearchRegexPattern + @"))([^\.|\s|'|\)|\]|;|:|,]+)";
                string Quote1 = "\"";
                //string Quote2 = "\'";
                KiriKiriQuotePattern = Quote1 + "(.+)" + Quote1 + ";$";
                bool TeachingFeelingCS = false;
                string tmpfile = File.ReadAllText(sPath);
                if (tmpfile.Contains("[p_]") || tmpfile.Contains("[lr_]"))
                {
                    TeachingFeelingCS = true;
                }
                tmpfile = null;
                using (StreamReader file = new StreamReader(sPath, TeachingFeelingCS ? Encoding.UTF8 : Encoding.GetEncoding(932)))
                {
                    string line;
                    bool iscomment = false;

                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith("/*"))
                        {
                            iscomment = true;
                        }


                        if ((iscomment || string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//") || Regex.IsMatch(line, @"\s*//")))
                        {
                        }
                        else
                        {
                            if (TeachingFeelingCS)//Teaching Feeling cs
                            {
                                if (line.Contains("[p_]") || line.Contains("[lr_]"))
                                {
                                    int lastMergeIndex = -1;
                                    int startMergeIndex = -1;
                                    string[] temp = line.Split(']');
                                    int Cnt = temp.Length;

                                    //поиск первой и последней части для записи
                                    for (int i = 0; i < Cnt; i++)
                                    {
                                        temp[i] += "]";
                                        string substring = temp[i];
                                        if (lastMergeIndex > -1 && i >= startMergeIndex && i <= lastMergeIndex)
                                        {
                                            line += temp[i];
                                        }
                                        else if (temp[i].EndsWith("[lr_]") || temp[i].EndsWith("[p_]"))
                                        {
                                            lastMergeIndex = i;
                                            startMergeIndex = i;
                                            while (startMergeIndex > 0 && (!temp[startMergeIndex - 1].Contains("[") || temp[startMergeIndex - 1].Contains("[name")))
                                            {
                                                startMergeIndex -= 1;
                                            }
                                        }
                                    }

                                    //обнуление, для записи нового чистого значения
                                    line = string.Empty;

                                    //запись значения из найденного диапазона
                                    if (lastMergeIndex > -1)
                                    {
                                        for (int i = startMergeIndex; i <= lastMergeIndex; i++)
                                        {
                                            line += temp[i];
                                        }
                                    }

                                    //убрать идентификатор окончания строки
                                    line = line.Replace("[lr_]", string.Empty).Replace("[p_]", string.Empty);

                                    //line = Regex.Replace(line, @"^\s*(\[[a-z\/_]+\])*((\[name\])?.+)\[(lr|p)_\]\s*$", "$2");
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(line);
                                        _ = DTInfo.Rows.Add("Teeching feeling cs");
                                    }
                                }
                            }
                            else
                            {
                                if (line == "@nanasi")//Life with daughter
                                {
                                    line = file.ReadLine();
                                    while (!line.EndsWith("[ll]") && !line.EndsWith("@s"))
                                    {
                                        while (line.StartsWith("@"))
                                        {
                                            line = file.ReadLine();
                                        }
                                        line += Environment.NewLine;
                                        line += file.ReadLine();
                                    }
                                    line = line.Remove(line.Length - 4);//удаление последних четырех символов "[ll]" или "\r\n@s"
                                    _ = DT.Rows.Add(line);
                                    _ = DTInfo.Rows.Add(string.Empty);
                                }
                                else if (line.EndsWith("[k]")) // text ;Magic Swordsman Rene
                                {
                                    line = line.Replace("[k]", string.Empty);
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(line);
                                        _ = DTInfo.Rows.Add("[k] = end of line");
                                    }
                                }
                                else if (line.StartsWith("*")) // text ;Magic Swordsman Rene
                                {
                                    line = line.Remove(0, 1);
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(line);
                                        _ = DTInfo.Rows.Add(string.Empty);
                                    }
                                }
                                else if (line.EndsWith("[r]")) //text, first line ;Magic Swordsman Rene
                                {
                                    line = line.Replace("[r]", string.Empty).Trim();
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(line);
                                        _ = DTInfo.Rows.Add("[r] = carriage return");
                                    }
                                }
                                else if (line.StartsWith("o.") || Regex.IsMatch(line, KiriKiriVariableSearchRegexPattern)) //variable, which is using even for displaing and should be translated in all files ;Magic Swordsman Rene
                                {
                                    MatchCollection matches = Regex.Matches(line, KiriKiriVariableSearchRegexFullPattern);

                                    bool startswith = line.StartsWith("o.");
                                    for (int m = 0; m < matches.Count; m++)
                                    {
                                        _ = DT.Rows.Add(matches[m].Value.Remove(0, startswith ? 2 : 3));
                                        _ = DTInfo.Rows.Add(T._("Variable>Must be Identical in all files>Only A-Za-z0-9" + Environment.NewLine + "line: " + line));
                                        if (startswith)
                                        {
                                            startswith = false;//o. в начале встречается только в первый раз
                                        }
                                    }
                                }
                                else if (line.StartsWith("@notice text="))// ; Magic Swordsman Rene
                                {
                                    line = line.Remove(0, 13);//удаление "@notice text="
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(line);
                                        _ = DTInfo.Rows.Add("@notice text=");
                                    }
                                }
                                else if (line.StartsWith("Name = '"))// ; Magic Swordsman Rene
                                {
                                    line = line.Remove(line.Length - 2, 2).Remove(0, 8);
                                    if (string.IsNullOrEmpty(line) || line.IsDigitsOnly())
                                    {
                                    }
                                    else
                                    {
                                        _ = DT.Rows.Add(line);
                                        _ = DTInfo.Rows.Add("Name = '");
                                    }
                                }
                                else if (Regex.IsMatch(line, "\\\"(.*?)\\\""))// ; Magic Swordsman Rene
                                {
                                    //https://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
                                    var matches = Regex.Matches(line, "\\\"(.*?)\\\"");
                                    string subline;
                                    foreach (var m in matches)
                                    {
                                        subline = m.ToString();
                                        if (string.IsNullOrWhiteSpace(subline.Replace("\"", string.Empty)) || line.Replace("\"", string.Empty).IsDigitsOnly())
                                        {
                                        }
                                        else
                                        {
                                            _ = DT.Rows.Add(subline.Remove(subline.Length - 1, 1).Remove(0, 1));
                                            _ = DTInfo.Rows.Add(line);
                                        }
                                    }
                                }
                            }
                        }

                        if (line.EndsWith("*/"))
                        {
                            iscomment = false;
                        }
                    }

                    return DT;
                }
            }
            catch
            {

            }

            return null;
        }

        internal string KiriKiriScenarioOpen(string sPath)
        {
            try
            {
                using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    //string original = string.Empty;
                    string filename = Path.GetFileNameWithoutExtension(sPath);
                    _ = ProjectData.FilesContent.Tables.Add(filename);
                    _ = ProjectData.FilesContent.Tables[0].Columns.Add(THSettings.OriginalColumnName());
                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith(";") || line.StartsWith("@") || Equals(line, string.Empty))
                        {
                        }
                        else
                        {
                            if (line.EndsWith("[k]"))
                            {
                                ProjectData.FilesContent.Tables[0].Rows.Add(line.Remove(line.Length - 3, 3));

                            }
                        }
                    }

                    if (ProjectData.FilesContent.Tables[0].Rows.Count > 0)
                    {
                        _ = ProjectData.FilesContent.Tables[0].Columns.Add(THSettings.TranslationColumnName());
                        ProjectData.Main.THFilesList.Invoke((Action)(() => ProjectData.Main.THFilesList.AddItem(filename)));
                    }
                    else
                    {
                        /*THMsg*/
                        MessageBox.Show(T._("Nothing to add"));
                        return string.Empty;
                    }
                }

                return "KiriKiri scenario";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return string.Empty;
        }

        internal void KiriKiriScriptScenarioWrite(string sPath)
        {
            try
            {
                //ks scn tjs open
                StringBuilder buffer = new StringBuilder();

                using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
                {
                    string line;
                    //string original = string.Empty;
                    string filename = Path.GetFileNameWithoutExtension(sPath);
                    int elementnumber = 0;
                    bool iscomment = false;
                    while (!file.EndOfStream)
                    {
                        line = file.ReadLine();

                        if (line.StartsWith("/*"))
                        {
                            iscomment = true;
                        }
                        else if (line.EndsWith("*/"))
                        {
                            iscomment = false;
                        }

                        if (iscomment || string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("//"))
                        {
                        }
                        else
                        {
                            if (line.EndsWith("[k]")) // text
                            {
                                string cline = line.Replace("[r]", string.Empty).Remove(line.Length - 3, 3);
                                if (string.IsNullOrEmpty(cline) || cline.IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    var row = ProjectData.FilesContent.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = row[1] + "[k]";
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.StartsWith("*")) // text
                            {
                                string cline = line.Remove(0, 1);
                                if (string.IsNullOrEmpty(cline) || cline.IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    var row = ProjectData.FilesContent.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = row[1] as string;
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.EndsWith("[r]")) //text, first line
                            {
                                string cline = line.Replace("[r]", string.Empty);
                                if (string.IsNullOrEmpty(cline) || cline.IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    var row = ProjectData.FilesContent.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = row[1] as string;
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.StartsWith("o.") || Regex.IsMatch(line, KiriKiriVariableSearchRegexPattern)) //variable, which is using even for displaing and should be translated in all files
                            {
                                MatchCollection matches = Regex.Matches(line, KiriKiriVariableSearchRegexFullPattern);

                                bool startswith = line.StartsWith("o.");
                                for (int m = 0; m < matches.Count; m++)
                                {
                                    var row = ProjectData.FilesContent.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(matches[m].Value.Remove(0, startswith ? 2 : 3), row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = line.Remove(matches[m].Index, matches[m].Value.Length).Insert(matches[m].Index, matches[m].Value.Replace(row[0] as string, row[1] as string));
                                    }
                                    if (startswith)
                                    {
                                        startswith = false;//o. в начале встречается только раз
                                    }
                                    elementnumber++;
                                }
                            }
                            else if (line.StartsWith("@notice text="))
                            {
                                string cline = line.Remove(0, 13);//удаление "@notice text="
                                if (string.IsNullOrEmpty(cline) || cline.IsDigitsOnly())
                                {
                                }
                                else
                                {
                                    var row = ProjectData.FilesContent.Tables[0].Rows[elementnumber];
                                    if (
                                        row[1] == null
                                        || string.IsNullOrEmpty(row[1] as string)
                                        && Equals(cline, row[0] as string)
                                        && !Equals(row[0], row[1])
                                       )
                                    {
                                        line = "@notice text=" + row[1];
                                    }
                                    elementnumber++;
                                }
                            }
                        }

                        buffer.AppendLine(line);
                    }
                }

                File.WriteAllText(sPath, buffer.ToString(), Encoding.GetEncoding(932));

                _ = MessageBox.Show(T._("finished") + "!");
            }
            catch
            {
            }
        }

        //private void KiriKiriScenarioWrite(string sPath)
        //{
        //    try
        //    {
        //        StringBuilder buffer = new StringBuilder();

        //        using (StreamReader file = new StreamReader(sPath, Encoding.GetEncoding(932)))
        //        {
        //            string line;
        //            string original = string.Empty;
        //            string filename = Path.GetFileNameWithoutExtension(sPath);
        //            int elementnumber = 0;
        //            while (!file.EndOfStream)
        //            {
        //                line = file.ReadLine();

        //                if (line.StartsWith(";") || line.StartsWith("@") || Equals(line, string.Empty))
        //                {
        //                }
        //                else
        //                {
        //                    if (line.EndsWith("[k]"))
        //                    {
        //                        if (
        //                            (THFilesElementsDataset.Tables[0].Rows[elementnumber][1] + string.Empty).Length > 0
        //                            && Equals(line.Remove(line.Length - 3, 3), THFilesElementsDataset.Tables[0].Rows[elementnumber][0] + string.Empty)
        //                            && !Equals(THFilesElementsDataset.Tables[0].Rows[elementnumber][0] + string.Empty, THFilesElementsDataset.Tables[0].Rows[elementnumber][1] + string.Empty)
        //                           )
        //                        {
        //                            line = THFilesElementsDataset.Tables[0].Rows[elementnumber][1] + "[k]";
        //                        }

        //                        elementnumber++;
        //                    }
        //                }

        //                buffer.AppendLine(line);
        //            }
        //        }

        //        File.Move(sPath, sPath + ".bak");
        //        File.WriteAllText(sPath, buffer.ToString(), Encoding.GetEncoding(932));

        //        _ = MessageBox.Show(T._("finished") + "!");
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = MessageBox.Show(ex.ToString());
        //    }
        //}
    }
}
