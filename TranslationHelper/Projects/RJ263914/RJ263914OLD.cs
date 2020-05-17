using System;
using System.IO;
using System.Text;
using TranslationHelper.Data;
using TranslationHelper.Extensions;
using TranslationHelper.Main.Functions;

namespace TranslationHelper.Projects.RJ263914
{
    class RJ263914OLD
    {
        readonly THDataWork thDataWork;
        public RJ263914OLD(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal string ProceedRubyRPGGame(string GameDirectory, bool IsWrite = false)
        {
            Properties.Settings.Default.THSelectedGameDir = GameDirectory;
            string binDir = Path.Combine(GameDirectory, "data", "bin");
            string[][] folderNames = new string[14][] {
                new string[2] { "enemes", "enemes" }
                ,
                new string[2] {"enemy", "enemy" }
                ,
                new string[2] {"item", "item" }
                //,
                //new string[2] {"map", "map" }
                ,
                new string[2] {"mappos", "mappos" }
                ,
                new string[2] {"onom", "onom" }
                ,
                new string[2] {"pants", "pants" }
                ,
                new string[2] {"plugin", "plugin" }
                ,
                new string[2] {"recollect", "*" }
                ,
                new string[2] {"skill", "skill" }
                ,
                new string[2] {"state", "state" }
                ,
                new string[2] {"submission", "mission" }
                ,
                new string[2] {"trophy", "trophy" }
                ,
                new string[2] {"tutorial", "tuto" }
                ,
                new string[2] {"type", "type" }
            };

            int folderNamesLength = folderNames.Length / 2;
            for (int i = 0; i < folderNamesLength; i++)
            {
                string targetDirPath = Path.Combine(binDir, folderNames[i][0]);
                if (Directory.Exists(targetDirPath))
                {
                    RubyRPGGameFIlesFromTheDir(targetDirPath, folderNames[i][1], IsWrite);
                }
            }
            return "RubyRPGGame";
        }

        private void RubyRPGGameFIlesFromTheDir(string targetDirPath, string extension, bool IsWrite = false)
        {
            bool DropItIn1File = (
                    extension == "item"
                    ||
                    extension == "enemy"
                    ||
                    extension == "mappos"
                    ||
                    extension == "mission"
                    ||
                    extension == "onom"
                    ||
                    extension == "pants"
                    ||
                    extension == "plugin"
                    ||
                    extension == "skill"
                    ||
                    extension == "state"
                    ||
                    extension == "trophy"
                    ||
                    extension == "type"
                    );

            string folderName = Path.GetFileName(targetDirPath);
            int rowIndex = 0;
            string processingFolderName = string.Empty;
            foreach (var filePath in Directory.EnumerateFiles(targetDirPath, "*." + extension, SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(filePath);
                //string tableName = (extension == "onom" ? Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(filePath))) + "_" + Path.GetFileName(Path.GetDirectoryName(filePath)) + "_" : string.Empty) + Path.GetFileName(filePath);
                string tableName = DropItIn1File ? Path.GetFileName(targetDirPath) : fileName;

                bool tableNotExists = thDataWork.THFilesElementsDataset.Tables[tableName] == null;
                if (tableNotExists)
                {
                    if (IsWrite)
                    {
                        continue;
                    }
                    else
                    {
                        thDataWork.THFilesElementsDataset.Tables.Add(tableName);
                        thDataWork.THFilesElementsDataset.Tables[tableName].Columns.Add("Original");

                        thDataWork.THFilesElementsDataset.Tables[tableName].Columns.Add("Translation");
                        thDataWork.Main.THFilesList.Invoke((Action)(() => thDataWork.Main.THFilesList.Items.Add(tableName)));

                        thDataWork.THFilesElementsDatasetInfo.Tables.Add(tableName);
                        thDataWork.THFilesElementsDatasetInfo.Tables[tableName].Columns.Add("Original");
                    }
                }

                if (folderName == "enemes")
                {
                    Open_enemesDirFile(filePath, IsWrite);
                }
                else if (folderName == "enemy")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    //Open_SelectedLinesFromDirFile(filePath, fileName, new int[4] { 2, 3, 19, 20 }, fileName);
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[4] { 2, 3, 19, 20 }, fileName, IsWrite, rowIndex);
                    processingFolderName = folderName;
                }
                else if (folderName == "item")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[2] { 1, 5 }, fileName, IsWrite, rowIndex);
                    processingFolderName = folderName;
                }
                else if (folderName == "mappos")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[2] { 2, 6 }, fileName, IsWrite, rowIndex);
                    processingFolderName = folderName;
                }
                else if (folderName == "map")
                {
                    Open_mapBynaryFromDirFile(filePath);
                }
                else if (folderName == "onom")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[1] { 1 },
                        Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(filePath))) + Path.DirectorySeparatorChar + Path.GetFileName(Path.GetDirectoryName(filePath)) + Path.DirectorySeparatorChar + fileName
                        , IsWrite, rowIndex
                        );
                    processingFolderName = folderName;
                }
                else if (folderName == "pants")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    string lineValue = File.ReadAllText(filePath);
                    if (IsWrite)
                    {
                        var row = thDataWork.THFilesElementsDataset.Tables[tableName].Rows[rowIndex];
                        if (lineValue == row[0] as string && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                        {
                            File.SetAttributes(filePath, FileAttributes.Normal);
                            File.WriteAllText(filePath, row[1] as string);
                        }
                    }
                    else
                    {
                        thDataWork.THFilesElementsDataset.Tables[tableName].Rows.Add(lineValue);
                        thDataWork.THFilesElementsDatasetInfo.Tables[tableName].Rows.Add(fileName);
                    }
                    processingFolderName = folderName;
                    rowIndex++;
                }
                else if (folderName == "plugin")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[2] { 1, 4 }, fileName, IsWrite, rowIndex);
                    processingFolderName = folderName;
                }
                else if (folderName == "recollect")
                {
                    Open_recollectDirFile(filePath, folderName, IsWrite);
                }
                else if (folderName == "skill")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[3] { 1, 18, 19 }, fileName, IsWrite, rowIndex);
                    processingFolderName = folderName;
                }
                else if (folderName == "state")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    rowIndex = Open_SelectedLinesFromDirFile(filePath, tableName, new int[5] { 1, 6, 7, 8, 9 }, fileName, IsWrite, rowIndex);
                    processingFolderName = folderName;
                }
                else if (folderName == "submission")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    string lineValue = File.ReadAllText(filePath);
                    if (IsWrite)
                    {
                        var row = thDataWork.THFilesElementsDataset.Tables[tableName].Rows[rowIndex];
                        if (lineValue == row[0] as string && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                        {
                            File.SetAttributes(filePath, FileAttributes.Normal);
                            File.WriteAllText(filePath, row[1] as string);
                        }
                    }
                    else
                    {
                        thDataWork.THFilesElementsDataset.Tables[tableName].Rows.Add(lineValue);
                        thDataWork.THFilesElementsDatasetInfo.Tables[tableName].Rows.Add(fileName);
                    }
                    processingFolderName = folderName;
                    rowIndex++;
                }
                else if (folderName == "trophy")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    string lineValue = File.ReadAllText(filePath);
                    if (IsWrite)
                    {
                        var row = thDataWork.THFilesElementsDataset.Tables[tableName].Rows[rowIndex];
                        if (lineValue == row[0] as string && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                        {
                            File.SetAttributes(filePath, FileAttributes.Normal);
                            File.WriteAllText(filePath, (row[1] as string));
                        }
                    }
                    else
                    {
                        thDataWork.THFilesElementsDataset.Tables[tableName].Rows.Add(lineValue);
                        thDataWork.THFilesElementsDatasetInfo.Tables[tableName].Rows.Add(fileName);
                    }
                    processingFolderName = folderName;
                    rowIndex++;
                }
                else if (folderName == "tutorial")
                {
                    Open_tutorialDirFile(filePath, folderName, IsWrite);
                }
                else if (folderName == "type")
                {
                    if (processingFolderName != folderName)
                    {
                        rowIndex = 0;
                    }
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length == 2 && !lines[1].IsDigitsOnly() && !string.IsNullOrEmpty(lines[1]))
                    {
                        if (IsWrite)
                        {
                            var row = thDataWork.THFilesElementsDataset.Tables[tableName].Rows[rowIndex];
                            if (lines[1] == row[0] as string && row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[0], row[1]))
                            {
                                lines[1] = (row[1] as string).Replace(", ", "、").Replace(",", "、");//замена на японскую запятую т.к. обычной запятой тут разделяются параметры
                                File.SetAttributes(filePath, FileAttributes.Normal);
                                File.WriteAllLines(filePath, lines);
                            }
                        }
                        else
                        {
                            thDataWork.THFilesElementsDataset.Tables[tableName].Rows.Add(lines[1], lines[0]);
                            thDataWork.THFilesElementsDatasetInfo.Tables[tableName].Rows.Add(fileName);
                        }
                        processingFolderName = folderName;
                        rowIndex++;
                    }
                }
            }
        }

#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        private void Open_mapBynaryFromDirFile(string filePath)
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
        {
            //var sss = "4079690290013B094922";
            //File.SetAttributes(filePath, FileAttributes.Normal);
            ////var m = MarshalExt.GetDataAsStructure(typeof(RubyGamemapFile), File.ReadAllBytes(filePath));
            //var b = File.ReadAllBytes(filePath);
            //var m = MarshalExt.GetDataAsStructure(b.GetType(), b);
            //using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open)))
            //{
            //    //var l = br.Re;
            //    var ss = Encoding.GetEncoding(932).GetString(br.ReadBytes(512));
            //    var aa = ss;
            //}
        }

        private void Open_tutorialDirFile(string filePath, string folderName, bool IsWrite = false)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                string line;
                int lineNum = 1;
                int rowIndex = 0;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.Length > 0)
                    {
                        string[] lineArray = line.Split(',');
                        if (lineArray.Length > 4)
                        {
                            //line = lineArray[4];
                            if (!lineArray[4].IsDigitsOnly() && !string.IsNullOrEmpty(lineArray[4]))
                            {
                                if (IsWrite)
                                {
                                    var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[rowIndex];
                                    if ((row[0] as string) == lineArray[4])
                                    {
                                        if (row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[1], row[0]))
                                        {
                                            lineArray[4] = (row[1] as string).Replace(", ", "、").Replace(",", "、");//замена на японскую запятую т.к. обычной запятой тут разделяются параметры
                                            line = string.Join(",", lineArray);
                                        }
                                        rowIndex++;
                                    }
                                }
                                else
                                {
                                    thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(lineArray[4]);
                                    thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(folderName + Path.DirectorySeparatorChar + fileName);
                                }
                            }
                        }
                    }
                    if (IsWrite)
                    {
                        sb.AppendLine(line);
                    }
                    lineNum++;
                }

            }
            if (IsWrite)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.WriteAllText(filePath, sb.ToString());
            }
        }

        private void Open_recollectDirFile(string filePath, string folderName, bool IsWrite = false)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                string line;
                int lineNum = 1;
                int rowIndex = 0;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.Length > 0 && line.StartsWith("message"))
                    {
                        string[] lines = line.Split(',');
                        //line = line.Split(',')[1];
                        if (!lines[1].IsDigitsOnly() && !string.IsNullOrEmpty(lines[1]))
                        {
                            if (IsWrite)
                            {
                                var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[rowIndex];
                                if ((row[0] as string) == lines[1])
                                {
                                    if (row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[1], row[0]))
                                    {
                                        lines[1] = (row[1] as string).Replace(", ", "、").Replace(",", "、");//замена на японскую запятую т.к. обычной запятой тут разделяются параметры
                                        line = string.Join(",", lines);
                                    }
                                    rowIndex++;
                                }
                            }
                            else
                            {
                                thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(lines[1]);
                                thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(folderName + Path.DirectorySeparatorChar + fileName);
                            }
                        }
                    }
                    if (IsWrite)
                    {
                        sb.AppendLine(line);
                    }
                    lineNum++;
                }

            }
            if (IsWrite)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.WriteAllText(filePath, sb.ToString());
            }
        }

        private int Open_SelectedLinesFromDirFile(string filePath, string tableName, int[] lineNumbers, string Info = "", bool IsWrite = false, int rowIndex = 0)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int lineNum = 1;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (FunctionsDigit.IsEqualsAnyNumberFromArray(lineNum, lineNumbers))
                    {
                        if (!line.IsDigitsOnly() && !string.IsNullOrEmpty(line) && line != "\\n")
                        {
                            if (IsWrite)
                            {
                                var row = thDataWork.THFilesElementsDataset.Tables[tableName].Rows[rowIndex];
                                string tempString = (row[0] as string);
                                if (tempString == line)
                                {

                                    if (row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[1], row[0]))
                                    {
                                        line = (row[1] as string).Replace(", ", "、").Replace(",", "、");//замена на японскую запятую т.к. обычной запятой тут разделяются параметры
                                    }
                                    rowIndex++;
                                }
                            }
                            else
                            {
                                thDataWork.THFilesElementsDataset.Tables[tableName].Rows.Add(line);
                                thDataWork.THFilesElementsDatasetInfo.Tables[tableName].Rows.Add(Info);
                            }
                        }
                    }
                    if (IsWrite)
                    {
                        sb.AppendLine(line);
                    }
                    lineNum++;
                }
            }
            if (IsWrite)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.WriteAllText(filePath, sb.ToString());
            }
            return rowIndex;
        }

        private void Open_enemesDirFile(string enemesFile, bool IsWrite = false)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(enemesFile))
            {
                string fileName = Path.GetFileName(enemesFile);
                string line;
                int lineNum = 1;
                int rowIndex = 0;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (lineNum > 2 && line.Length > 0 && line != "top" && line != "bottom")
                    {
                        string[] strings = line.Split(',');

                        for (int i = 0; i < strings.Length; i++)
                        {
                            if (!strings[i].IsDigitsOnly() && !string.IsNullOrEmpty(strings[i]))
                            {
                                if (IsWrite)
                                {
                                    var row = thDataWork.THFilesElementsDataset.Tables[fileName].Rows[rowIndex];
                                    if ((row[0] as string) == strings[i])
                                    {
                                        if (row[1] != null && !string.IsNullOrEmpty(row[1] as string) && !Equals(row[1], row[0]))
                                        {
                                            strings[i] = (row[1] as string).Replace(", ", "、").Replace(",", "、");//замена на японскую запятую т.к. обычной запятой тут разделяются параметры
                                            line = string.Join(",", strings);
                                        }
                                        rowIndex++;
                                    }
                                }
                                else
                                {
                                    thDataWork.THFilesElementsDataset.Tables[fileName].Rows.Add(strings[i]);
                                    thDataWork.THFilesElementsDatasetInfo.Tables[fileName].Rows.Add(fileName);
                                }
                            }
                        }
                    }
                    if (IsWrite)
                    {
                        sb.AppendLine(line);
                    }
                    lineNum++;
                }

            }
            if (IsWrite)
            {
                File.SetAttributes(enemesFile, FileAttributes.Normal);
                File.WriteAllText(enemesFile, sb.ToString());
            }
        }
    }
}
