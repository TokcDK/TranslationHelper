using System.Collections.Generic;
using System.IO;
using System.Linq;
using TranslationHelper.Data;
using TranslationHelper.Extensions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    class MakeTranslatedCopyIfFileWithTheNameExists : RowBase
    {
        public MakeTranslatedCopyIfFileWithTheNameExists(THDataWork thDataWork) : base(thDataWork)
        {
        }

        bool NeedToAddFilePaths = true;
        Dictionary<string, List<string>> GameFilesList;
        protected override void ActionsPreRowsApply()
        {
            if (NeedToAddFilePaths)
            {
                GameFilesList = new Dictionary<string, List<string>>();
                foreach (var file in Directory.GetFiles(Properties.Settings.Default.THSelectedGameDir, "*", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (GameFilesList.ContainsKey(name))
                    {
                        GameFilesList[name].Add(file);
                    }
                    else
                    {
                        GameFilesList.Add(Path.GetFileNameWithoutExtension(file), new List<string> { file });//add file name with path
                    }
                }
                NeedToAddFilePaths = false;
            }
        }

        protected override bool Apply()
        {
            string name;
            string trans;
            if (SelectedRow[1] != null
                && !string.IsNullOrWhiteSpace(trans = SelectedRow[1] + "")
                && !trans.IsMultiline() //ignore multiline
                && !trans.Intersect(Path.GetInvalidFileNameChars()).Any() //is valid for file/folder name
                && GameFilesList != null
                && GameFilesList.ContainsKey(name = Path.GetFileNameWithoutExtension(SelectedRow[0] as string)))
            {
                foreach (var path in GameFilesList[name])
                {
                    var targetPath = Path.Combine(Path.GetDirectoryName(path), trans + Path.GetExtension(path));
                    if (!File.Exists(targetPath))
                    {
                        File.Copy(path, targetPath);

                        //info about translated copy
                        File.WriteAllText(targetPath + ".tr.txt", "original name:" + name + "\r\noriginal file name is exists in table and in game dir\r\ncreated copy with translated name to prevent possible missing file errors");
                    }
                }
                return true;
            }
            return false;
        }
    }
}
