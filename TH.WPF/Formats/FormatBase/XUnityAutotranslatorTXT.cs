namespace FormatBase
{
    internal class XUnityAutotranslatorTXT : IFormatString
    {
        public string Extension => ".txt";

        public string Description => "XUnity Autotranslator translation txt";

        public List<StringData> StringsList => new();

        FileInfo? OpenPath;
        public bool TryOpen(FileInfo Path)
        {
            OpenPath = Path;

            if (!OpenPath.Exists) return false;
            if (OpenPath.Extension != Extension) return false;

            ReadStrings(OpenPath);

            return StringsList.Count > 0;
        }

        private void ReadStrings(FileInfo openPath)
        {
            using StreamReader sr = new(openPath.FullName);
            int lineNum = 0;
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                lineNum++;

                if (line == null) continue;
                if (string.IsNullOrWhiteSpace(line)) continue;
                var trimmedLine = line.TrimStart();
                if (trimmedLine.StartsWith("//")) continue;
                if (trimmedLine.StartsWith(";")) continue;
                //if (trimmedLine.StartsWith("sr:")) continue;
                //if (trimmedLine.StartsWith("r:")) continue;

                var lineData = line.Trim().Split('=');
                if (lineData.Length != 2) continue;

                var o = lineData[0];
                var data = new StringData(o, lineNum);
                StringsList.Add(data);
                var t = lineData[1];

                if (string.IsNullOrEmpty(t)) continue;
                if (string.Equals(o, t)) continue;

                data.Translation = t;
            }
        }

        public bool TrySave(FileInfo Path)
        {
            if (OpenPath == null) return false;
            if (!OpenPath.Exists) return false;

            int lineNum = 0;
            var newFileContent = new List<string>();
            var content = StringsList.ToDictionary(k => k.Index, v => v);
            using StreamReader sr = new(OpenPath.FullName);
            bool changed = false;
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                lineNum++;

                // when the line was loaded
                if (content.ContainsKey(lineNum)) 
                {
                    var data = content[lineNum];
                    var ot = line.Split("=");
                    var o = ot[0];
                    var t = ot[1];

                    // when line was changed
                    if (string.Equals(o,data.Original)
                        && !string.Equals(t,data.Translation) 
                        && !string.Equals(data.Original, data.Translation))
                    {
                        line = $"{data.Original}={data.Translation}";
                        changed = true;
                    }
                }

                // add line in new file
                newFileContent.Add(line);
            }

            if (changed) File.WriteAllText(Path.FullName, string.Join(Environment.NewLine, newFileContent));

            return changed;
        }
    }
}
